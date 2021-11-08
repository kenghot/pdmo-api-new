using ClosedXML.Excel;
using ClosedXML.Report;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using mof.DataModels.Models;
using mof.IServices;
using mof.ServiceModels.Common;
using mof.ServiceModels.FinancialReport;
using mof.ServiceModels.Helper;
using mof.ServiceModels.Plan;
using mof.ServiceModels.Project;
using mof.ServiceModels.Reports;
using mof.ServiceModels.Reports.ExistingPlan;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mof.Services
{
    public class ReportRepository : IReport
    {
        public MOFContext DB;

        private UserManager<ServiceModels.Identity.ApplicationUser> _user;
        private SignInManager<ServiceModels.Identity.ApplicationUser> _signin;
        private Microsoft.AspNetCore.Identity.UI.Services.IEmailSender _email;
        private IStringLocalizer<MessageLocalizer> _msglocalizer;
        private ISystemHelper _helper;
        private ICommon _com;
        private IPlan _plan;
        private IProject _proj;
        private readonly IHostingEnvironment _hostingEnvironment;
        // private IPlan _plan;
        //private string[] debtTrans;
        public ReportRepository(MOFContext db, UserManager<ServiceModels.Identity.ApplicationUser> userManager,
        SignInManager<ServiceModels.Identity.ApplicationUser> signInManager,
        Microsoft.AspNetCore.Identity.UI.Services.IEmailSender email,
        IStringLocalizer<MessageLocalizer> msglocalizer,
        ISystemHelper helper,
        ICommon com,
        IPlan plan,
         IHostingEnvironment hostingEnvironment,
         IProject proj
        )
        {
            DB = db;
            _user = userManager;
            _signin = signInManager;
            _email = email;
            _msglocalizer = msglocalizer;
            _helper = helper;
            _com = com;
            _plan = plan;
            _hostingEnvironment = hostingEnvironment;
            _proj = proj;
        }
        public async Task<ReturnObject<FileContentResult>> ExistingPlanByAgreement(long planID)
        {
            var ret = new ReturnObject<FileContentResult>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var exPl = await _plan.GetExistDebtPlan(planID, "P", null);
                //var str = System.IO.File.ReadAllText("E:\\Works\\mof\\docs\\report\\json\\exist.json");
                //JObject json = JObject.Parse(str);
                //var exPl = json.ToObject<ReturnObject<ExistingDebtPlanModel>>();
                if (!exPl.IsCompleted)
                {
                    ret.CloneMessage(exPl.Message);
                    return ret;
                }
                var data = exPl.Data;
                var exist = new ExistingPlanByAgreement {
                    RepDate = DateTime.Now.Date,
                    Ministry = "",
                    OrganizationName = data.PlanHeader.Organization.Description,
                    Year = data.PlanHeader.PlanYear.ToString(),
                    Year5Y = $"{data.PlanHeader.PlanYear}-{data.PlanHeader.PlanYear + 5}",
                    Info = data.DebtSettlementInfo != null ? new DebtSettlementInfoModel {
                        FeeAmount = data.DebtSettlementInfo.FeeAmount,
                        InterestAmount = data.DebtSettlementInfo.InterestAmount,
                        LoanAmount = data.DebtSettlementInfo.LoanAmount
                    } : new DebtSettlementInfoModel()
                };
                var Items = new List<EPBAItem>();
                var Paids = exist.PaidItems;
                //{
                //    new EPBAItem { AgreementText = $"GF001{Environment.NewLine}g44/n999\n7777777" , FItem = new EPBAAmountItem{ CCS = 1 , Interest = 2 , IRS = 3 , Other = 4 , PrePayment = 5 , Refinance = 6 ,  RePayment = 7 , RollOver = 8 , Total = 9} , LItem = new EPBAAmountItem{ CCS = 1 , Interest = 2 , IRS = 3 , Other = 4 , PrePayment = 5 , Refinance = 6 ,  RePayment = 7 , RollOver = 8 , Total = 9} },
                //    new EPBAItem { AgreementText = "GF002" , FItem = new EPBAAmountItem{ CCS = 11 , Interest = 22 , IRS = 33 , Other = 4 , PrePayment = 5 , Refinance = 6 ,  RePayment = 7 , RollOver = 8 , Total = 9} , LItem = new EPBAAmountItem{ CCS = 1 , Interest = 2 , IRS = 3 , Other = 4 , PrePayment = 5 , Refinance = 6 ,  RePayment = 7 , RollOver = 8 , Total = 9} }
                //};
                foreach (var pd in data.PlanDetails)
                {
                    //if (planYearOnly)
                    //{
                    //    if (pd.Year.Value != data.PlanHeader.PlanYear)
                    //    {
                    //        continue;
                    //    }
                    //}else
                    //{
                    //    if (pd.Year.Value == data.PlanHeader.PlanYear)
                    //    {
                    //        continue;
                    //    }
                    //}
                    var rowAmt = Items.Where(w => w.PlanExistID == pd.PlanExistID).FirstOrDefault();

                    if (rowAmt == null)
                    {
                        rowAmt = new EPBAItem { PlanExistID = pd.PlanExistID.Value, 
                            Year = pd.Year.HasValue ? pd.Year.Value.ToString() : exPl.Data.PlanHeader.PlanYear.ToString() , 
                            LItem = new EPBAAmountItem() , FItem = new EPBAAmountItem() };
                        Items.Add(rowAmt);
                    }
                    PaidItem paidItem = new PaidItem();
                    if (pd.Year == exPl.Data.PlanHeader.PlanYear)
                    {

                        Paids.Add(paidItem);
                       


                    }
                    List<AgreementItem> agrItems = new List<AgreementItem>();
                    foreach (var agr in pd.AgreementDetail)
                    {
                        var trtxt = agr.TransactionType != null ? $"({agr.TransactionType.Description})" : "";
                        rowAmt.AgreementText += $"{Environment.NewLine}{trtxt} {agr.GFTRRefCode}: {agr.Description}";
                        rowAmt.TransType = agr.TransactionType != null ? agr.TransactionType : new ServiceModels.Common.Generic.BasicData { Description = "ไม่พบข้อมูล" };
                        #region Paid items
                        var iagr = new AgreementItem
                        {
                            AgreementId = agr.AgreementID,
                            GFCode = agr.GFTRRefCode,
                            AgreementName = agr.Description,
                            DebtSubType = agr.DebtSubType,
                            DueAmount = agr.IncomingDueAmount / 1000000,
                            InterestRate = agr.InterestRate,
                            LoanAge = decimal.Round( agr.LoanAge / 365, 2, MidpointRounding.AwayFromZero),
                            OutStandingDebt  = agr.OutStandingDebt / 1000000,
                            OutStandingDebtTHB = agr.OutStandingDebtTHB / 1000000,
                            LoanDate = agr.StartDate,
                            LoanLeftYear = agr.EndDate.Year - DateTime.Now.Year,
                            LoanAmount = agr.LoanAmount / 1000000,
                            ActualDueAmount = agr.ActualDueAmount / 1000000,
                            DueDate = agr.IncomingDueDate,
                            TransType = rowAmt.TransType,
                            LoanCurr = agr.LoanCurrency


                        };
                        var r = exPl.Data.PlanSummary.CurrencyData.Currency.Where(w => w.CurrencyCode == agr.LoanCurrency).FirstOrDefault();
                        if (r != null)
                        {
                            iagr.LoanAmountTHB = iagr.LoanAmount * r.CurrencyRate ;
                        }
                        if (iagr.LoanCurr == "THB")
                        {
                            iagr.OutStandingDebt = 0;
                            iagr.LoanAmount = 0;
                        }
                        paidItem.ActualDueAmount += iagr.ActualDueAmount;
                        agrItems.Add(iagr);
                        #endregion
                    }
                    #region Paid items
                    paidItem.Agreements = agrItems.ToList();
                    //paidItem.Agreements.AddRange(agrItems.ToList());
                    #endregion

                    rowAmt.Agreements = agrItems.ToList();


                    var allPlan = new List<DebtPaymentPlan>();
                    allPlan.AddRange(pd.PaymentPlan);
                    allPlan.AddRange(pd.RestructurePlan);
                    var resTxts = pd.RestructurePlan.Select(s => new { 
                        s.DebtPaymentPlanType, 
                        s.IsRequestGuarantee,
                        THB = s.LoanSourcePlans.Sum(sum => sum.THBAmount)
                    }).ToList();
                    foreach (var txt in resTxts)
                    {
                        if (txt.THB > 0)
                        {
                            var lov = _helper.GetLOVByCode(txt.DebtPaymentPlanType, ServiceModels.Constants.LOVGroup.DebtPaymentPlanType._LOVGroupCode);
                            if (lov.IsCompleted)
                            {
                                var gua = txt.IsRequestGuarantee ? "ค้ำประกัน" : "ไม่ค้ำประกัน";
                                paidItem.RestructureTxt += $"{lov.Data.LOVValue}/{gua} : {txt.THB / 1000000 :##,#0.#0} ล้านบาท |";
                            }
                        }
                    }
                    foreach (var ap in allPlan)
                    {
                                              
                        foreach (var amt in ap.LoanSourcePlans)
                        {   
                            EPBAAmountItem item = amt.SourceType == "L" ? rowAmt.LItem : rowAmt.FItem;
                           
                            if (ap.DebtPaymentPlanType ==  ServiceModels.Constants.LOVGroup.DebtPaymentPlanType.ชำระตามกำหนด){
                                item.RePayment += amt.THBAmount;
                                item.Total += amt.THBAmount;
                                paidItem.PaidAmount.THBAmt += amt.THBAmount / 1000000;
                                paidItem.PaidAmount.AddCurrAmt(amt.Currency, amt.LoanAmount / 1000000);

                            }else if (ap.DebtPaymentPlanType == ServiceModels.Constants.LOVGroup.DebtPaymentPlanType.ชำระหนี้ล่วงหน้า)
                            {
                                item.PrePayment += amt.THBAmount;
                                item.Total += amt.THBAmount;
                                paidItem.PaidAmount.THBAmt += amt.THBAmount / 1000000;
                                paidItem.PaidAmount.AddCurrAmt(amt.Currency, amt.LoanAmount / 1000000);
                            }
                            else if (ap.DebtPaymentPlanType == ServiceModels.Constants.LOVGroup.DebtPaymentPlanType.Roll_over)
                            {
                                item.RollOver += amt.THBAmount;
                                item.Total += amt.THBAmount;
                                paidItem.RestructureAmt.THBAmt += amt.THBAmount / 1000000;
                                paidItem.RestructureAmt.AddCurrAmt(amt.Currency, amt.LoanAmount / 1000000);
                            }
                            else if (ap.DebtPaymentPlanType == ServiceModels.Constants.LOVGroup.DebtPaymentPlanType.Refinance)
                            {
                                item.Refinance += amt.THBAmount;
                                item.Total += amt.THBAmount;
                                paidItem.RestructureAmt.THBAmt += amt.THBAmount / 1000000;
                                paidItem.RestructureAmt.AddCurrAmt(amt.Currency, amt.LoanAmount / 1000000);
                            }
                            else if (ap.DebtPaymentPlanType == ServiceModels.Constants.LOVGroup.DebtPaymentPlanType.Cross_Currency_Swap)
                            {
                                item.CCS += amt.THBAmount;
                                item.Total += amt.THBAmount;
                                paidItem.RestructureAmt.THBAmt += amt.THBAmount / 1000000;
                                paidItem.RestructureAmt.AddCurrAmt(amt.Currency, amt.LoanAmount / 1000000);
                            }
                            else if (ap.DebtPaymentPlanType == ServiceModels.Constants.LOVGroup.DebtPaymentPlanType.Interrest_Rate_)
                            {
                                item.IRS += amt.THBAmount;
                                item.Total += amt.THBAmount;
                                paidItem.RestructureAmt.THBAmt += amt.THBAmount / 1000000;
                                paidItem.RestructureAmt.AddCurrAmt(amt.Currency, amt.LoanAmount / 1000000);
                            }
                            else 
                            {
                                item.Other += amt.THBAmount;
                                item.Total += amt.THBAmount;
                                paidItem.RestructureAmt.THBAmt += amt.THBAmount / 1000000;
                                paidItem.RestructureAmt.AddCurrAmt(amt.Currency, amt.LoanAmount / 1000000);
                            }

                        }

                        if (paidItem.RestructureAmt.THBAmt > 0)
                        {

                        }
                    }
                    foreach (var item in Items)
                    {
                        if ( !string.IsNullOrEmpty (item.AgreementText))
                        {
                            item.AgreementText = item.AgreementText.TrimStart();
                        }
                    }
                }
                exist.Items = Items.Where(w => w.Year == data.PlanHeader.PlanYear.ToString()).ToList();
                exist.Forcasts = Items.Where(w => w.Year != data.PlanHeader.PlanYear.ToString()).ToList();
                if (exist.Items.Count == 0)
                {
                    exist.Items.Add(new EPBAItem());
                }
                if (exist.Forcasts.Count == 0)
                {
                    exist.Forcasts.Add(new EPBAItem());
                }
                //exist.PaidItems.AddRange(exist.PaidItems.ToList());
                var items = exist.PaidItems.ToList();
                exist.PaidCurrItems = items.Where(w => w.Agreements.FirstOrDefault() != null && w.Agreements.First().LoanCurr != "THB").ToList();
                exist.PaidItems = items.Where(w => w.Agreements.FirstOrDefault() != null && w.Agreements.First().LoanCurr == "THB").ToList();
                string repName ;
                //if (planYearOnly)
                //{
                    repName = "ExistingPlanByAgreement";
                //}else
                //{
                //    repName = "ExistingPlanByAgreementForcast";
                //}
                var rep = GetReportExcelFile("excels",repName, exist);

                ret = rep;

            }
            catch (Exception ex)
            {
                ret.AddError(ex);
                
            }
            return ret;
        }
        
        private string FLoanToString(Dictionary<string,decimal> dic, CurrencyData rate, ref decimal thb)
        {
            string ret = "";
            List<string> txts = new List<string>();
            foreach (var d in dic)
            {
                var r = rate.Currency.Where(w => w.CurrencyCode == d.Key).FirstOrDefault();
                decimal cr = 1;
                if (r != null)
                {
                    cr = r.CurrencyRate;
                }
                thb += d.Value * cr;
                txts.Add($"{d.Value/(decimal)1000000:##,#0.#0} ({d.Key})");
            }
            ret = string.Join(',', txts);
            return ret;
        }
        private List<LoanSource> ToLoanSource(List<AmountData> amts)
        {
            return amts.Select(s => new LoanSource { Currency = s.CurrencyCode, LoanAmount = s.Amount, SourceType = s.SourceType, THBAmount = s.THBAmount }).ToList();
        }
        private void SumAmountData(List<CurrecyRateData> rate, List<LoanSource> amts, ref decimal LTot, ref decimal Ftot,ref decimal Otot, ref string FtotCur)
        {
            var famt = new Dictionary<string, decimal>();
            LTot = 0;
            Ftot = 0;
            FtotCur = "";
            foreach (var amt in amts)
            {
                if (amt.SourceType == "L")
                {
                    LTot += amt.LoanAmount / 1000000;
                }else if (amt.SourceType == "O") {
                    Otot += amt.LoanAmount / 1000000;
                }
                else if (amt.SourceType == "F") {
                    var r = rate.Where(w => w.CurrencyCode == amt.Currency).FirstOrDefault();
                    if (r != null)
                    {
                        Ftot += r.CurrencyRate * (amt.LoanAmount / 1000000);
                    }
                    decimal val;
                    if (famt.TryGetValue(amt.Currency, out val))
                    {

                        famt[amt.Currency] = val + (amt.LoanAmount / 1000000);
                    }
                    else
                    {

                        famt.Add(amt.Currency, amt.LoanAmount / 1000000);
                    }

                }
                foreach (var item in famt)
                {
                    FtotCur += $"{item.Value:##,#0.#0} ({item.Key}) ";
                }
            }

        }
        public async Task<ReturnObject<FileContentResult>> NewDebtPlanRep(long planID)
        {
            var ret = new ReturnObject<FileContentResult>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var nPl = await _plan.GetNewDebtPlan(planID, "P", null);
                //var str = System.IO.File.ReadAllText("E:\\Works\\mof\\docs\\report\\json\\newdebt_10503.json");
                //JObject json = JObject.Parse(str);
                //var nPl = json.ToObject<ReturnObject<NewDebtPlanModel>>();
                if (!nPl.IsCompleted)
                {
                    ret.CloneMessage(nPl.Message);
                    return ret;
                }
                var data = nPl.Data;
                var nRep = new NewDebtPlanRep
                {
                    RepDate = DateTime.Now.Date,
                    Ministry = "",
                    OrganizationName = data.PlanHeader.Organization.Description,
                    Year = data.PlanHeader.PlanYear.ToString()
                };

                nRep.Items = new List<NewDebtPlanRepItem>();
                foreach (var pj in data.PlanDetails)
                {
                    var nItem = new NewDebtPlanRepItem();
                    var pjObj = await _proj.Get(pj.ProjectID);
                    var pjDat = pjObj.Data;
                    var pjLoan = new ProjLoan();
                    var pjDisbSign = new ProjectDisbSign();
                    var pjMD = await _proj.Get(pj.ProjectID);
                    nRep.DisbSigns.Add(pjDisbSign);
                    nRep.Items.Add(nItem);
                    nRep.ProjLoans.Add(pjLoan);
                    nItem.ProjectID = pj.ProjectID;
                    nItem.ProjectTHName = pj.ProjectTHName;
                    pjDisbSign.ProjectTHName = pj.ProjectTHName;
                    #region Project Loan 1.1
                    pjLoan.Year = data.PlanHeader.PlanYear.ToString();
                    pjLoan.ProjectTHName = pj.ProjectTHName;
                    pjLoan.ProjectENName = pjMD.Data.ProjectENName;
                    pjLoan.ProjOjective = pjMD.Data.ProjectObjective;
                    pjLoan.Firr = pjMD.Data.FIRR;
                    pjLoan.Eirr = pjMD.Data.EIRR;
                    foreach (var mat in pjMD.Data.Materials)
                    {
                        if (mat.SourceType == "L")
                        {
                            pjLoan.LMatAmt += mat.THBAmount/1000000;
                            pjLoan.LMatPer += mat.Ratio;
                        }else
                        {
                            pjLoan.FMatAmt += mat.THBAmount/1000000;
                            pjLoan.FMatPer += mat.Ratio;
                        }
                    }
                    pjLoan.Duration = $"(พ.ศ. {pjMD.Data.StartDate.Year + 543} - พ.ศ. {pjMD.Data.EndDate.Year + 543})";
                    foreach (var pjact in pjMD.Data.Activities)
                    {
                        var pjLExp = new ProjLoanExpense
                        {
                            ActivityName = pjact.ActivityName
                        };
                        pjLoan.Expenses.Add(pjLExp);
                        decimal f, l, o;
                        string txt;
                        f = 0; l = 0; o = 0; txt = "";
                        SumAmountData(nPl.Data.PlanSummary.CurrencyData.Currency
                            , ToLoanSource(pjact.ContractAmounts)
                            ,ref l,ref f,ref o,ref txt);
                        pjLExp.FContract = f;
                        pjLExp.LContract = l;
                        SumAmountData(nPl.Data.PlanSummary.CurrencyData.Currency
                        , ToLoanSource(pjact.ResolutionAmounts)
                        , ref l, ref f, ref o, ref txt);
                        pjLExp.FResolution = f;
                        pjLExp.LResolution = l;
                        pjLExp.Budget = pjact.TotalProceedByActivity.Budget / 1000000;
                        pjLExp.Revernue = pjact.TotalProceedByActivity.Revernue / 1000000;
                        pjLExp.SignedLoan = pjact.TotalProceedByActivity.SignedLoan / 1000000;
                        pjLExp.DisburseLoan = pjact.TotalProceedByActivity.DisburseLoan / 1000000;
                        pjLExp.Other = pjact.TotalProceedByActivity.Other / 1000000;
                        pjLExp.Total = pjact.TotalProceedByActivity.Total / 1000000;
             

                    }
                    
                    #endregion
                    if (pj.IsNewProject)
                    {
                        nItem.IsNew = "X";
                    }
                    else
                    {
                        nItem.IsCont = "X";
                    }
                    foreach (var pjStat in pjDat.ProjectStatuses)
                    {
                        if (pjStat.Code == ServiceModels.Constants.LOVGroup.สถานะโครงการ.ความสอดคล้องกับนโยบายยุทธศาสตร์ชาติ && pjStat.PDMOCheck && pjStat.ResolutionCheck)
                        {
                            nItem.IsStrategic = "X";
                        }
                        if (pjStat.Code == ServiceModels.Constants.LOVGroup.สถานะโครงการ.ความสอดคล้องกับแผนการปฏิรูปประเทศ && pjStat.PDMOCheck && pjStat.ResolutionCheck)
                        {
                            nItem.IsReform = "X";
                        }
                        if (pjStat.Code == ServiceModels.Constants.LOVGroup.สถานะโครงการ.ความสอดคล้องกับแผนพัฒนาเศรษฐกิจและสังคมแห่งชาติ && pjStat.PDMOCheck && pjStat.ResolutionCheck)
                        {
                            nItem.IsDevPlan = "X";
                        }
                        if (pjStat.Code == ServiceModels.Constants.LOVGroup.สถานะโครงการ.มีรายงานการวิเคราะห์ผลกระทบสิ่งแวดล้อม && pjStat.PDMOCheck && pjStat.ResolutionCheck)
                        {
                            nItem.IsAnalystRep = "X";
                        }
                        if (pjStat.Code == ServiceModels.Constants.LOVGroup.สถานะโครงการ.มีรายงานการศึกษาความเหมาะสมด้านเทคนิค_เศรษฐกิจ_สังคม_การเงิน && pjStat.PDMOCheck && pjStat.ResolutionCheck)
                        {
                            nItem.IsEduRep = "X";
                        }
                        if (pjStat.Code == ServiceModels.Constants.LOVGroup.สถานะโครงการ.ได้รับการอนุมัติจากคณะรัฐมนตรี && pjStat.PDMOCheck && pjStat.ResolutionCheck)
                        {
                            nItem.GOVAppr = "X";
                        }
                        if (pjStat.Code == ServiceModels.Constants.LOVGroup.สถานะโครงการ.ได้รับการอนุมัติจากกระทรวงเจ้าสังกัด && pjStat.PDMOCheck && pjStat.ResolutionCheck)
                        {
                            nItem.MinistryAppv = "X";
                        }
                        if (pjStat.Code == ServiceModels.Constants.LOVGroup.สถานะโครงการ.ได้รับการอนุมัติจากสภาพัฒนาการเศรษฐกิจและสังคมแห่งชาติ && pjStat.PDMOCheck && pjStat.ResolutionCheck)
                        {
                            nItem.PDMOAppv = "X";
                        }
                        if (pjStat.Code == ServiceModels.Constants.LOVGroup.สถานะโครงการ.ได้รับการอนุมัติจากคณะกรรมการของหน่วยงาน && pjStat.PDMOCheck && pjStat.ResolutionCheck)
                        {
                            nItem.StateEntAppv = "X";
                        }
                    }
                    var fLoanFromGov = new Dictionary<string, decimal>();
                    var fLoanGov = new Dictionary<string, decimal>();
                    Dictionary<string, decimal> fLoan;
                    bool isFromGov;
                    var r = new Random();
                    //for (var i = 0; i <= r.Next(15); i++)
                    //{
                    //    pjDisbSign.Activities.Add(new DisbSign { ActivityName = $"ssss{i}" });

                    //}

                    foreach (var act in pj.ActivityPlans)
                    {
                        #region แบบฟอร์มที่ 1 สรุปความต้องการใช้เงินกู้ในประเทศและต่างประเทศ 
                        foreach (var loan in act.LoanTypePlans)
                        {
                            if (loan.LoanType.SelectedType == ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้ต่อจากกระทรวงการคลัง_ของรัฐวิสาหกิจ_)
                            {
                                fLoan = fLoanFromGov;
                                isFromGov = true;
                            }
                            else
                            {
                                fLoan = fLoanGov;
                                isFromGov = false;
                            }
                            foreach (var amt in loan.LoanSourcePlans)
                            {
                                if (amt.SourceType == "L")
                                {
                                    if (isFromGov)
                                    {
                                        nItem.LFIN += amt.LoanAmount;
                                    }
                                    else
                                    {
                                        if (pj.ProjectType == ServiceModels.Constants.LOVGroup.Project_Type.กู้เพื่อดำเนินโครงการหรือเพื่อใช้เป็นเงินทุนหมุนเวียนในการดำเนินกิจการทั่วไป)
                                        {
                                            nItem.LLoanRevolv += amt.LoanAmount;
                                        }
                                        else if (pj.ProjectType == ServiceModels.Constants.LOVGroup.Project_Type.กู้เพื่อลงทุนในโครงการพัฒนา)
                                        {
                                            nItem.LLoanDev += amt.LoanAmount;
                                        }
                                        else
                                        {
                                            nItem.LLoanProcess += amt.LoanAmount;
                                        }
                                    }

                                }
                                else
                                {
                                    decimal val;
                                    if (fLoan.TryGetValue(amt.Currency, out val))
                                    {

                                        fLoan[amt.Currency] = val + amt.LoanAmount;
                                    }
                                    else
                                    {

                                        fLoan.Add(amt.Currency, amt.LoanAmount);
                                    }
                                }
                            }
                        }
                        #endregion
                        #region ProjLoan Activities (1.1)
                     
                        decimal f, l,o;
                        string txt = "";
                        f = 0; l = 0;o = 0;
                        SumAmountData(nPl.Data.PlanSummary.CurrencyData.Currency,
                            ToLoanSource( act.ActivityPlanDetail.Budget),
                            ref l, ref f,ref o,ref txt);

                        pjLoan.Budget = l + f;
                        f = 0; l = 0; o = 0 ; txt = "";
                        SumAmountData(nPl.Data.PlanSummary.CurrencyData.Currency,
                           ToLoanSource(act.ActivityPlanDetail.Revernue),
                           ref l, ref f,ref  o, ref txt);
                        pjLoan.Revernue = l + f;
                        pjLoan.OtherLoanText = "";
                        var LActLoans = new ActivityLoan { ActivityName = act.Activity.ActivityName };
                        var FActLoans = new ActivityLoan { ActivityName = act.Activity.ActivityName };
                        var LFActLoans = new LFActivityLoan { ActivityName = act.Activity.ActivityName };
                        pjLoan.LActivityLoans.Add(LActLoans);
                        pjLoan.FActivityLoans.Add(FActLoans);
                        pjLoan.LFLActivityLoans.Add(LFActLoans);
                        foreach (var loan in act.LoanTypePlans)
                        {
                            f = 0; l = 0; o = 0; txt = "";
                            SumAmountData(nPl.Data.PlanSummary.CurrencyData.Currency,
                            loan.LoanSourcePlans, ref l, ref f,ref o, ref txt);
                            if (loan.LoanType.SelectedType == ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้มาเพื่อให้กู้ต่อ_ของรัฐบาล_ || loan.LoanType.SelectedType == ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้ต่อจากกระทรวงการคลัง_ของรัฐวิสาหกิจ_)
                            {
                                pjLoan.LoanFinDeptName = loan.LoanType.SelectedType == ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้มาเพื่อให้กู้ต่อ_ของรัฐบาล_ ? "เงินกู้มาเพื่อให้กู้ต่อ" : "เงินกู้ต่อจากกระทรวงการคลัง";
                                pjLoan.LLoanFinDept += l;
                                pjLoan.FLoanFinDept += f;
                                LActLoans.FinDept += l;
                                FActLoans.FinDept += f;
                                LFActLoans.LFinDept += l;
                                LFActLoans.FFinDept += f;
                                pjLoan.FLoanFinDeptCur = txt;
                            }else
                            {
                                //pjLoan.OtherLoan += l + f ;       
                                //if (l + f > 0)
                                //{
                                //    var lt =   _helper.GetLOVByCode(loan.LoanType.SelectedType, ServiceModels.Constants.LOVGroup.Project_Amount_Type._LOVGroupCode);
                                //    if (lt.IsCompleted)
                                //    {
                                //        pjLoan.OtherLoanText += lt.Data.LOVValue + " ";
                                //    }
                                //}
                                LActLoans.Invest += l;
                                FActLoans.Invest += f;
                                LFActLoans.LInvest += l;
                                LFActLoans.FInvest += f;
                                pjLoan.LInvestLoan += l;
                                pjLoan.FInvestLoan += f;
                                 
                                pjLoan.FLoanFinDeptCur = "";
                            }
                            //if (pj.ProjectType == ServiceModels.Constants.LOVGroup.Project_Type.เพื่อโครงการลงทุน_พัฒนา )
                            //{
                            //    pjLoan.LInvestLoan += l;
                            //    pjLoan.FInvestLoan += f;
                            //    pjLoan.FLoanFinDeptCur = "";
                            //}
                        }
                        if (pjLoan.OtherLoanText.Trim()  == "")
                        {
                            pjLoan.OtherLoanText = "โปรดระบุ";
                        }
                       
                        #endregion
                        #region แบบฟอร์มที่ 1.1 (ต่อ)

                        //var nDisbSignF = new DisbSign();
                        //pjDisbSign.Activities.Add(nDisbSign);
                        //pjDisbSign.ActivitiesF.Add(nDisbSignF);

                        //nDisbSignF.ActivityName = act.Activity.ActivityName;
                        //pjDisbSign.Activities.Add(new DisbSign { ActivityName = $"ssss" });
                        decimal loanAmt = 0;
                        foreach (var loan in act.LoanTypePlans)
                        {
                            foreach (var lamt in loan.LoanSourcePlans)
                            {
                                loanAmt += lamt.THBAmount;
                            }
                        }
                        foreach (var proc in act.LoanProcessPlan)
                        {
                            var nDisbSign = new DisbSign {
                                ActivityName = act.Activity.ActivityName,
                                LoanSource = proc.LoanSource.SourceType,
                                LoanSourceText = proc.LoanSource.SourceType == "L" ? "เงินกู้ในประเทศเพื่อลงทุนในโครงการพัฒนา" : "เงินกู้ต่างประเทศเพื่อลงทุนในโครงการพัฒนา",
                                Currency = proc.LoanSource.Currency,
                                LoanAmt = loanAmt,
                                LoanType = pj.ProjectTypeLabel
                            };
                            
                            foreach (var amt in proc.LoanProcessPlanDetails)
                            {
                                StoreMonthAmt(nDisbSign.Disb, amt.Month, amt.DisburseLoan / 1000000 );
                                StoreMonthAmt(nDisbSign.Sign, amt.Month, amt.SignedLoan / 1000000);
                            }
                            if (true || nDisbSign.LoanAmt > 0 || nDisbSign.Sign.Total > 0 || nDisbSign.Disb.Total > 0)
                            {
                                pjDisbSign.Activities.Add(nDisbSign);
                            }
                        }
                        #endregion
                    }
                    pjDisbSign.Activities = pjDisbSign.Activities.OrderByDescending(o => o.LoanSource).ToList();
                    decimal thb = 0;
                    nItem.FLoanCur = FLoanToString(fLoanGov, data.PlanSummary.CurrencyData, ref thb);
                    nItem.FLoanTHB = thb;
                    thb = 0;
                    nItem.FFIN = FLoanToString(fLoanFromGov, data.PlanSummary.CurrencyData, ref thb);

                }
                nRep.ProjLoans2 = nRep.ProjLoans.ToList();
                //foreach (var test in nRep.ProjLoans)
                //{
                //    var tests = new TestGroup
                //    {
                //        Expenses = test.Expenses.ToList(),
                //        FActivityLoans = test.FActivityLoans.ToList(),
                //        LActivityLoans = test.LActivityLoans.ToList()
                //    };
                //    test.TestGroups.Add(tests);
                //}
                //nRep.Expenses = nRep.ProjLoans[0].Expenses.ToList();
                //nRep.LActivityLoans = nRep.ProjLoans[0].LActivityLoans.ToList();
                string repName;
                repName = "NewDebtPlan";

                //var jsonString =   JsonConvert.SerializeObject(nRep);

                //System.IO.File.WriteAllText(_hostingEnvironment.WebRootPath + "/data.json", jsonString);

                var rep = GetReportExcelFile("excels", repName, nRep);

                ret = rep;

            }
            catch (Exception ex)
            {
                ret.AddError(ex);

            }
            return ret;
        }
        private void StoreMonthAmt(MonthAmt obj, int month, decimal amt)
        {
            if (month == 1)
            {
                obj.M1 = amt;
            }
            if (month == 2)
            {
                obj.M2 = amt;
            }
            if (month == 3)
            {
                obj.M3 = amt;
            }
            if (month == 4)
            {
                obj.M4 = amt;
            }
            if (month == 5)
            {
                obj.M5 = amt;
            }
            if (month == 6)
            {
                obj.M6 = amt;
            }
            if (month == 7)
            {
                obj.M7 = amt;
            }
            if (month == 8)
            {
                obj.M8 = amt;
            }
            if (month == 9)
            {
                obj.M9 = amt;
            }
            if (month == 10)
            {
                obj.M10 = amt;
            }
            if (month == 11)
            {
                obj.M11 = amt;
            }
            if (month == 12)
            {
                obj.M12 = amt;
            }
        }
        public async Task<ReturnObject<FileContentResult>> FinancialPlanRep(long planID)
        {
            var ret = new ReturnObject<FileContentResult>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var fPl = await _plan.GetFinPlan(planID);
                //var str = System.IO.File.ReadAllText("E:\\Works\\mof\\docs\\report\\json\\financial.json");
                //JObject json = JObject.Parse(str);
                //var fPl = json.ToObject<ReturnObject<FinancialReportModel>>();

                if (!fPl.IsCompleted)
                {
                    ret.CloneMessage(fPl.Message);
                    return ret;
                }
                var data = fPl.Data;
                var fRep = new FinancialRep
                {
                    RepDate = DateTime.Now.Date,
                    Ministry = "",
                    OrganizationName = data.PlanHeader.Organization.Description,
                    Year = data.PlanHeader.PlanYear.ToString()
                };

                fRep.FinancialRatios = fPl.Data.FinancialRatio;
                fRep.Ratio = new FNRepRatio();
                List<FinancialRatio> ratio = new List<FinancialRatio>();
                if (fPl.Data.FinancialRatio != null)
                {
                    ratio = fPl.Data.FinancialRatio.OrderBy(o => o.Year).ToList();
                }
                
                fRep.Ratio.R1 = ratio.Count >= 1 ? ratio[0] : new ServiceModels.FinancialReport.FinancialRatio { Year = "" };
                fRep.Ratio.R2 = ratio.Count >= 2 ? ratio[1] : new ServiceModels.FinancialReport.FinancialRatio { Year = "" };
                fRep.Ratio.R3 = ratio.Count >= 3 ? ratio[2] : new ServiceModels.FinancialReport.FinancialRatio { Year = "" };
                fRep.Ratio.R4 = ratio.Count >= 4 ? ratio[3] : new ServiceModels.FinancialReport.FinancialRatio { Year = "" };
                fRep.Ratio.R5 = ratio.Count >= 5 ? ratio[4] : new ServiceModels.FinancialReport.FinancialRatio { Year = "" };
                fRep.Ratio.R6 = ratio.Count >= 6 ? ratio[5] : new ServiceModels.FinancialReport.FinancialRatio { Year = "" };
                fRep.Ratio.R7 = ratio.Count >= 7 ? ratio[6] : new ServiceModels.FinancialReport.FinancialRatio { Year = "" };
                fRep.Ratio.R8 = ratio.Count >= 8 ? ratio[7] : new ServiceModels.FinancialReport.FinancialRatio { Year = "" };
                fRep.Ratio.R9 = ratio.Count >= 9 ? ratio[8] : new ServiceModels.FinancialReport.FinancialRatio { Year = "" };
                fRep.Ratio.R10 = ratio.Count >= 10 ? ratio[9] : new ServiceModels.FinancialReport.FinancialRatio { Year = "" };
                string repName;
                repName = "FinancialReport";
                
              
                var rep = GetReportExcelFile("excels", repName, fRep);

                ret = rep;

            }
            catch (Exception ex)
            {
                ret.AddError(ex);

            }
            return ret;
        }
        public  ReturnObject<FileContentResult> GetReportExcelFile(string ReportFolder , string ReportName, object data)
        {
            var ret = new ReturnObject<FileContentResult>(_msglocalizer);
         
            ret.IsCompleted = false;
            try
            {
                string mime = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; // MIME header with default value
               
                string webRootPath = _hostingEnvironment.WebRootPath; // determine the path to the wwwroot folder

                var assembly = typeof(mof.Resources.Class1).Assembly; // .GetTypeInfo().Assembly;
                var names = assembly.GetManifestResourceNames();
                XLTemplate template ;
                var excel = assembly.GetManifestResourceStream($"mof.Resources.{ReportFolder}.{ReportName}.xlsx");
               
                template = new XLTemplate(excel);
               


               // var template = new XLTemplate (webRootPath + $"/{ReportFolder}/{ReportName}.xlsx");


                template.AddVariable(data);
                template.Generate();
                
                try
                {

                    using (MemoryStream stream = new MemoryStream())
                    {
                        template.SaveAs(stream);
                        //var wb = new XLWorkbook(stream);
                        var file = new FileContentResult( stream.ToArray(), mime);
                       

                        file.FileDownloadName = $"{ReportName}_{DateTime.Now.Ticks.ToString()}.xlsx";
                        ret.IsCompleted = true;
                        ret.Data = file;
                    }
                }
                catch (Exception ex)
                {
                    ret.AddError(ex);
                  
                }

            }
            catch (Exception ex)
            {
                 ret.AddError(ex);
               
            }
            return ret;
        }
    }
}
