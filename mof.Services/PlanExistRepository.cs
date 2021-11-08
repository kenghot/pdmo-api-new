using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using mof.DataModels.Models;
using mof.IServices;
using mof.ServiceModels.Common;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Linq.Expressions;
using mof.Services.Helper;
using Microsoft.EntityFrameworkCore;
using mof.ServiceModels.Request;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Localization;
using mof.ServiceModels.Request.Common;
using mof.ServiceModels.Response;
using mof.ServiceModels.Organization;
using mof.ServiceModels.Constants;
using mof.ServiceModels.Common.Generic;
using Newtonsoft.Json;
using mof.ServiceModels.Plan;
using mof.ServiceModels.Helper;
using mof.ServiceModels.Project;
using mof.ServiceModels.Agreement;

namespace mof.Services
{
    public partial class PlanRepository : IPlan
    {
       


        public async Task<ReturnList<ExistPlanAgreementList>> GetPlanAgreementList(PlanProjectListParameter p)
        {
            var ret = new ReturnList<ExistPlanAgreementList>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var sql = DB.PlanExist.Include(pt => pt.Plan.PlanTypeNavigation)
                .Include(ea => ea.PlanExistAgreement).ThenInclude(ag => ag.Agreement)
                .Include(pay => pay.PaymentPlan).ThenInclude(dbt => dbt.DebtPaymentPlanTypeNavigation)
                .Include(pay => pay.PaymentPlan).ThenInclude(py => py.PaymentSourceNavigation)
                .Include(pay => pay.PaymentPlan).ThenInclude(dp => dp.DebtPayAmt).ThenInclude(pamt => pamt.PlanAmountNavigation)
                .WhereIf(p.PlanID.HasValue, pl => pl.PlanId == p.PlanID.Value)
                .WhereIf(!string.IsNullOrEmpty(p.PlanRelease), r => r.Plan.PlanReleaseNavigation.Lovcode == p.PlanRelease)
                .WhereIf(p.OrganizationID.HasValue, o => o.Plan.OrganizationId == p.OrganizationID.Value)
                .WhereIf(p.StartYear > 0, y => y.Plan.StartYear == p.StartYear)
                .WhereIf(!string.IsNullOrEmpty(p.Paging.SearchText) , txt => txt.PlanExistAgreement.Where(w => w.Agreement.Description.Contains(p.Paging.SearchText)).Count() > 0)
                .Where(w => w.Plan.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.แผนบริหารหนี้เดิม);
                 ret.TotalRow = await sql.CountAsync();
                ret.PageNo = p.Paging.PageNo;
                ret.PageSize = p.Paging.PageSize;
                var exists = await sql.PageBy(pg => pg.PlanId, p.Paging.PageNo, p.Paging.PageSize, true).ToListAsync();
                var tmp = await _com.GetPlanAgreementList(exists);
                if (tmp.IsCompleted)
                {
                    ret.Data = tmp.Data;
                    ret.IsCompleted = true;
                }
                else
                {
                    ret.CloneMessage(tmp.Message);
                }
                //var data = new List<ExistPlanAgreementList>();
                //foreach (var ex in exists)
                //{
                //    var newEX = new ExistPlanAgreementList
                //    {
                //        Agreements = ex.PlanExistAgreement.Select(s => new AgreementModel
                //        {
                //            AgreementID = s.AgreementId,
                //            Description = s.Agreement.Description,
                //            ReferenceCode = s.Agreement.ReferenceCode
                //        }).ToList(),
                //        PlanExistID = ex.PlanExistId,
                //        DebtPaymentPlans = new List<DebtPaymentPlan>()

                //    };
                //    data.Add(newEX);
                //    foreach (var res in ex.PaymentPlan.Where(w => w.ManageType=="RES"))
                //    {
                //        var ppl = newEX.DebtPaymentPlans.Where(w => w.PaymentPlanID == res.PaymentPlanId).FirstOrDefault();
                //        if (ppl == null)
                //        {
                //            ppl = new DebtPaymentPlan
                //            {
                //                DebtPaymentPlanType = res.DebtPaymentPlanTypeNavigation.Lovvalue,
                //                IsRequestGuarantee = res.IsRequestGuarantee.HasValue ? res.IsRequestGuarantee.Value : false,
                //                LoanSourcePlans = new List<LoanSource>(),
                //                PaymentPlanID = res.PaymentPlanId,
                //                PaymentSource = res.PaymentSourceNavigation.Lovvalue

                //            };
                //            newEX.DebtPaymentPlans.Add(ppl);
                //        }
                //        foreach (var amt in res.DebtPayAmt)
                //        {
                //            var ls = ppl.LoanSourcePlans.Where(w => w.SourceType == amt.PlanAmountNavigation.SourceType && w.Currency == amt.PlanAmountNavigation.Currency).FirstOrDefault();
                //            if (ls == null)
                //            {
                //                ls = new LoanSource
                //                {
                //                    Currency = amt.PlanAmountNavigation.Currency,
                //                    SourceType = amt.PlanAmountNavigation.SourceType
                //                };
                //                ppl.LoanSourcePlans.Add(ls);
                //            }
                //            ls.LoanAmount += amt.PlanAmountNavigation.Amount1;
                //        }

                //    }
                //}
                //ret.Data = data;
                //ret.IsCompleted = true;



            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        private void ExistPlanSummary ( Summary sum , string TypeCode , decimal amt)
        {
            sum.Value += amt;
            if (TypeCode == ServiceModels.Constants.LOVGroup.Transaction_CashFlow_Type_from_GF.ชำระเงินต้น || TypeCode == ServiceModels.Constants.LOVGroup.Transaction_CashFlow_Type_from_GF.Installment)
            {
                sum.PrincipalValue += amt;
            }
            if (TypeCode == ServiceModels.Constants.LOVGroup.Transaction_CashFlow_Type_from_GF.ชำระดอกเบี้ย)
            {
                sum.InterestValue += amt;
            }
            if (TypeCode == ServiceModels.Constants.LOVGroup.Transaction_CashFlow_Type_from_GF.ค่าใช้จ่าย)
            {
                sum.FeeValue += amt;
            }
        }
       public async Task<ReturnObject<SearchExistingDebtPlanModel>> GetExistDebtPlan(PlanProjectListParameter p, eGetPlanType gettype, string amountGroup, int? month)
       {
            var ret = new ReturnObject<SearchExistingDebtPlanModel>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                //var ppjs = await DB.PlanProject
                //.Include(pl => pl.Plan)
                //.WhereIf(p.PlanID.HasValue, pl => pl.PlanId == p.PlanID.Value)
                //.WhereIf(!string.IsNullOrEmpty(p.ProjectType), pt => pt.ProjectTypeNavigation.Lovcode == p.ProjectType)
                //.WhereIf(!string.IsNullOrEmpty(p.PlanRelease), r => r.Plan.PlanReleaseNavigation.Lovcode == p.PlanRelease)
                //.WhereIf(p.OrganizationID.HasValue, o => o.Plan.OrganizationId == p.OrganizationID.Value)
                //.Where(w => w.Plan.StartYear == p.StartYear && w.Plan.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.แผนบริหารหนี้เดิม).ToListAsync();
                //List<Plan> plans = new List<Plan>();
                //var planx = await DB.Plan.Include(pt => pt.PlanTypeNavigation)
                //    .Include(e => e.PlanExist).ThenInclude(ea => ea.PlanExistAgreement)
                //    .Include(e => e.PlanExist).ThenInclude(pay => pay.PaymentPlan).ThenInclude(dbt => dbt.DebtPaymentPlanTypeNavigation)
                //    .Include(e => e.PlanExist).ThenInclude(pay => pay.PaymentPlan).ThenInclude(py => py.PaymentSourceNavigation)
                //    .Include(e => e.PlanExist).ThenInclude(pay => pay.PaymentPlan).ThenInclude(dp => dp.DebtPayAmt).ThenInclude(pamt => pamt.PlanAmountNavigation)
                //    .Include(e => e.PlanExist).ThenInclude(pay => pay.PaymentPlan).ThenInclude(dp => dp.DebtPayAmt).ThenInclude(iamt => iamt.InterestSaveAmountNavigation)
                //    .Where(w => w.PlanId == ID).FirstOrDefaultAsync();
                int? searchYear = (p.StartYear == 0) ? null : (int?)p.StartYear;
                ret.Data = new SearchExistingDebtPlanModel();
              var sql =   DB.Plan.Include(pt => pt.PlanTypeNavigation)
              .Include(att => att.PlanAttach)
              .Include(ex => ex.PlanExtend)
              .Include(e => e.PlanExist).ThenInclude(ea => ea.PlanExistAgreement)
              .Include(e => e.PlanExist).ThenInclude(ea => ea.PlanExistAgreement).ThenInclude(tt => tt.TransactionTypeNavigation)
              .Include(e => e.PlanExist).ThenInclude(ea => ea.PlanExistAgreement).ThenInclude(obj => obj.ObjectiveNavigation)
              .Include(e => e.PlanExist).ThenInclude(ea => ea.PlanExistAgreement).ThenInclude(pt => pt.PlanTypeNavigation)
              .Include(e => e.PlanExist).ThenInclude(pay => pay.PaymentPlan).ThenInclude(dbt => dbt.DebtPaymentPlanTypeNavigation)
              .Include(e => e.PlanExist).ThenInclude(pay => pay.PaymentPlan).ThenInclude(py => py.PaymentSourceNavigation)
              .Include(e => e.PlanExist).ThenInclude(pay => pay.PaymentPlan).ThenInclude(dp => dp.DebtPayAmt).ThenInclude(pamt => pamt.PlanAmountNavigation)
              .Include(e => e.PlanExist).ThenInclude(pay => pay.PaymentPlan).ThenInclude(dp => dp.DebtPayAmt).ThenInclude(iamt => iamt.InterestSaveAmountNavigation)
              .WhereIf(p.PlanID.HasValue, pl => pl.PlanId == p.PlanID.Value)
              .WhereIf(!string.IsNullOrEmpty(p.PlanRelease), r => r.PlanReleaseNavigation.Lovcode == p.PlanRelease)
              .WhereIf(p.OrganizationID.HasValue, o => o.OrganizationId == p.OrganizationID.Value)
              .WhereIf(searchYear.HasValue, y => y.StartYear == searchYear.Value)
              .Where(w => w.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.แผนบริหารหนี้เดิม ||
              w.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.รายงานประจำเดือน__แผนบริหารหนี้เดิม_) ;
                var plans = new List<Plan>();
                if (gettype == eGetPlanType.Search)
                {
                    ret.Data.TotalRec = await sql.CountAsync();
                     plans = await sql.PageBy(pg => pg.PlanId, p.Paging.PageNo, p.Paging.PageSize, true).ToListAsync();
                 
                }
                else
                {
                    plans = await sql.ToListAsync();
                }

        

                var data = new ExistingDebtPlanModel
                {
                    PlanDetails = new List<ExistingDebtPlanDetails>(),
                    //PlanDetailsByYear = new List<ExistingDebtPlanDetailsByYear>(),
                    PlanHeader = new PlanHeader(),
                    PlanSummary = new ExistingDebtPlanSummary(),
                    DebtSettlementInfo = new DebtSettlementInfoModel(),

                };

                if (p.PlanID.HasValue && gettype == eGetPlanType.GetByID)
                {

                    if (plans.Count == 0)
                    {
                        ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { "แผนงาน" });
                        return ret;
                    }

                    var head = await GetPlanHeaderByID(ServiceModels.Constants.LOVGroup.Plan_Type.แผนบริหารหนี้เดิม, p.PlanID.Value);
                    if (!head.IsCompleted)
                    {
                        ret.CloneMessage(head.Message);
                        return ret;
                    }
                    data.PlanHeader = head.Data;

                }
                if (gettype == eGetPlanType.GetByID)
                {
                    searchYear = plans[0].StartYear;
                }
                var rate = _helper.GetCurrencyRate(searchYear.Value);
                if (!rate.IsCompleted)
                {
                    ret.CloneMessage(rate.Message);
                    return ret;
                }
                data.PlanSummary = new ExistingDebtPlanSummary
                {
                    CurrencyData = rate.Data,
                    OverAllSummary = new List<Summary> {
                        new Summary { Title = "หนี้ที่ครบกำหนดชำระ" },
                        new Summary { Title = "วงเงินที่จะบริหาร"},
                        new Summary { Title = "เงินที่ให้กระทรวงการคลังค้ำประกัน"}
                    },
                    PaymentPlanSummary = new PlanSummary
                    {
                        PaymentPlans = new List<DebtPaymentPlan>(),
                    },
                    RestructurePlanSummary = new PlanSummary
                    {
                        PaymentPlans = new List<DebtPaymentPlan>()
                    }

                };
                foreach (var plan in plans)
                {
                    var info = plan.PlanExtend.Where(w => w.DataGroup == "DEBTSTTLEMENTINFO").FirstOrDefault();
                    if (info != null && info.Data != null)
                    {
                        try
                        {
                            data.DebtSettlementInfo = JsonConvert.DeserializeObject<DebtSettlementInfoModel>(info.Data);
                        }
                        catch
                        {
                            //convert ไม่ได้ ก็ไม่เอา
                        }

                    }
                    if (data.DebtSettlementInfo != null)
                    {
                        data.DebtSettlementInfo.AttatchFiles = new List<AttachFileData>();
                        foreach (var file in plan.PlanAttach)
                        {
                            var af = await DB.AttachFile.Where(w => w.AttachFileId == file.AttachFileId) 
                                .Select(s => new {FileDetail = s.FileDetail, FileExtension = s.FileExtension, FileName = s.FileName , FileSize = s.FileSize, AttachFileId = s.AttachFileId })
                                .FirstOrDefaultAsync();
                            if (af != null)
                            {
                                data.DebtSettlementInfo.AttatchFiles.Add(new AttachFileData
                                {
                                    FileDetail = af.FileDetail,
                                    FileExtension = af.FileExtension,
                                    FileName = af.FileName,
                                    FileSize = af.FileSize,
                                    ID = af.AttachFileId
                                });
                            }


                        }
                    }
                    string TranTypeCode = "";
                    #region Detail
                    foreach (var plEx in plan.PlanExist)
                    {
                        var pE = new ExistingDebtPlanDetails
                        {
                            PlanExistID = plEx.PlanExistId,
                            AgreementDetail = new List<AgreementModel>(),
                            DebtPlanSumAmount = new List<LoanSource>(),
                            InterestSavingSumAmount = new List<LoanSource>(),
                            PaymentPlan = new List<DebtPaymentPlan>(),
                            PaymentPlanSumAmount = new List<LoanSource>(),
                            RestructurePlan = new List<DebtPaymentPlan>(),
                            RestructurePlanSumAmount = new List<LoanSource>()
                        };
                    
                        data.PlanDetails.Add(pE);
                        pE.Year = plEx.Year;
                        pE.IsNotRequiredApproval = plEx.IsNotRequiredApproval;
                        #region Agreements
                        foreach (var plEA in plEx.PlanExistAgreement)
                        {
                            var pEA = await _arg.Get(plEA.AgreementId, plEx.Year, plEA.TransactionType);
                            if (!pEA.IsCompleted)
                            {
                                ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { "สัญญา" });
                                return ret;
                            }
                            pEA.Data.ActualDueAmount = plEA.ActualDueAmount.HasValue ? plEA.ActualDueAmount.Value : 0;
                            pEA.Data.ActualDueDate = plEA.ActualDueDate;
                            pEA.Data.Objective = plEA.Objective.HasValue ? new BasicData { Code = plEA.ObjectiveNavigation.Lovcode, 
                                Description = plEA.ObjectiveNavigation.Lovvalue, ID = plEA.ObjectiveNavigation.Lovkey  }  : null;
                            pEA.Data.PlanType = plEA.PlanType.HasValue ? new BasicData
                            {
                                Code = plEA.PlanTypeNavigation.Lovcode,
                                Description = plEA.PlanTypeNavigation.Lovvalue,
                                ID = plEA.PlanTypeNavigation.Lovkey
                            } : null;
                            pEA.Data.ActualMasterAgreement = plEA.MasterAgreement;
                            //pEA.Data.TransactionType = new BasicData
                            //{
                            //    Code = plEA.TransactionTypeNavigation.Lovcode,
                            //    Description = plEA.TransactionTypeNavigation.Lovvalue,
                            //    ID = plEA.TransactionTypeNavigation.Lovkey
                            //};
                            pE.AgreementDetail.Add(pEA.Data);
                            //data.PlanSummary.OverAllSummary[0].Value += pEA.Data.IncomingDueAmount;
                            if (plEx.Year == plan.StartYear )
                            {
                                // data.PlanSummary.OverAllSummary[0].Value += pEA.Data.ActualDueAmount;
                                TranTypeCode = pEA.Data.TransactionType.Code;
                                ExistPlanSummary(data.PlanSummary.OverAllSummary[0], pEA.Data.TransactionType.Code, pEA.Data.ActualDueAmount);
                            }
                          
                        }
                        #endregion
                        #region PaymentPlan & Restructure
                        
                        foreach (var payPl in plEx.PaymentPlan)
                        {
                            List<DebtPaymentPlan> managePlan = new List<DebtPaymentPlan>();
                            List<LoanSource> sumPlanDetail = new List<LoanSource>();
                            if (payPl.ManageType == "PAY")
                            {
                                managePlan = pE.PaymentPlan;
                                sumPlanDetail = pE.PaymentPlanSumAmount;
                            }
                            if (payPl.ManageType == "RES")
                            {
                                managePlan = pE.RestructurePlan;
                                sumPlanDetail = pE.RestructurePlanSumAmount;
                            }
                            var dpp = new DebtPaymentPlan
                            {
                                PaymentSource = payPl.PaymentSourceNavigation.Lovcode,
                                DebtPaymentPlanType = payPl.DebtPaymentPlanTypeNavigation.Lovcode,
                                InterestSaving = new InterestSaving
                                {
                                    calcurateReferences = new List<InterestCalculateRef>(),
                                    calculatedSaving = new List<LoanSource>()
                                },
                                IsRequestGuarantee = payPl.IsRequestGuarantee.Value,
                                LoanSourcePlans = new List<LoanSource>()
                            };
                            managePlan.Add(dpp);
                            foreach (var dpAmt in payPl.DebtPayAmt)
                            {
                                var r = rate.Data.Currency.Where(w => w.CurrencyCode == dpAmt.PlanAmountNavigation.Currency).FirstOrDefault();
                                if (r == null)
                                {
                                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { $"สกุลเงิน {dpAmt.PlanAmountNavigation.Currency} ของปี {plan.StartYear}" });
                                    return ret;
                                }
                                var planTHB = dpAmt.PlanAmountNavigation.Amount1 * r.CurrencyRate;
                                var intTHB = dpAmt.InterestSaveAmountNavigation.Amount1 * r.CurrencyRate;

                                var planAmt = new LoanSource
                                {
                                    Currency = dpAmt.PlanAmountNavigation.Currency,
                                    LoanAmount = dpAmt.PlanAmountNavigation.Amount1,
                                    SourceType = dpAmt.PlanAmountNavigation.SourceType,
                                    THBAmount = planTHB

                                };
                                var intAmt = new LoanSource
                                {
                                    Currency = dpAmt.InterestSaveAmountNavigation.Currency,
                                    LoanAmount = dpAmt.InterestSaveAmountNavigation.Amount1,
                                    SourceType = dpAmt.InterestSaveAmountNavigation.SourceType,
                                    THBAmount = intTHB

                                };
                                dpp.LoanSourcePlans.Add(planAmt);
                                dpp.InterestSaving.calculatedSaving.Add(intAmt);
                                try
                                {
                                    dpp.InterestSaving.calcurateReferences = JsonConvert.DeserializeObject<List<InterestCalculateRef>>(dpAmt.InterestReference);
                                }
                                catch
                                {
                                    //convert ไม่ได้ ก็ไม่เอา
                                }
                                #region summary
                                if (plEx.Year == plan.StartYear)
                                {
                                     
                                    ExistPlanSummary(data.PlanSummary.OverAllSummary[1], TranTypeCode, planTHB);
                                    //Utilities.SumLoanSource(sumPlanDetail, planAmt);
                                    if (payPl.IsRequestGuarantee.Value  )
                                    {
                                        data.PlanSummary.OverAllSummary[2].Value += planTHB;
                                        //Utilities.SumLoanSource(pE.InterestSavingSumAmount, planAmt);
                                    }
                                }
                                Utilities.SumLoanSource(sumPlanDetail, planAmt);
                                if (payPl.IsRequestGuarantee.Value)
                                {
                                    Utilities.SumLoanSource(pE.InterestSavingSumAmount, planAmt);
                                }
                                    //data.PlanSummary.OverAllSummary[1].Value += planTHB;


                                    PlanSummary sumPay = new PlanSummary();
                                if (payPl.ManageType == "PAY")
                                {
                                    sumPay = data.PlanSummary.PaymentPlanSummary;
                                }
                                if (payPl.ManageType == "RES")
                                {
                                    sumPay = data.PlanSummary.RestructurePlanSummary;
                                }
                                var sum = sumPay.PaymentPlans.Where(w => w.DebtPaymentPlanType == payPl.DebtPaymentPlanTypeNavigation.Lovcode &&
                                w.PaymentSource == payPl.PaymentSourceNavigation.Lovcode).FirstOrDefault();
                                if (sum == null)
                                {
                                    sum = new DebtPaymentPlan
                                    {
                                        DebtPaymentPlanType = payPl.DebtPaymentPlanTypeNavigation.Lovcode,
                                        PaymentSource = payPl.PaymentSourceNavigation.Lovcode,
                                        LoanSourcePlans = new List<LoanSource>()
                                    };
                                    sumPay.PaymentPlans.Add(sum);
                                }
                                if (plEx.Year == plan.StartYear)
                                {
                                    sumPay.PaymentPlanSumAmountTHB += planTHB;
                                    Utilities.SumLoanSource(sum.LoanSourcePlans, planAmt);
                                }

                            #endregion
                        }
                        }
                        #endregion
                    }
                    #endregion
                }
                var order = DB.CeLov.Where(w => w.LovgroupCode == ServiceModels.Constants.LOVGroup.DebtPaymentPlanType._LOVGroupCode)
                .OrderBy(o => o.OrderNo).Select(s => s.Lovcode).ToList();
                SortSumExistPlan(order, data.PlanSummary.PaymentPlanSummary);
                SortSumExistPlan(order, data.PlanSummary.RestructurePlanSummary);
                ret.Data.Data = data;
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        private void SortSumExistPlan(List<string> order, PlanSummary sum)
        {
            var old = new List<DebtPaymentPlan>(sum.PaymentPlans);
            sum.PaymentPlans = new List<DebtPaymentPlan>();
         
            foreach (var p in order)
            {
                var newp = old.Where(w => w.DebtPaymentPlanType == p).ToList();
                if (newp.Count > 0)
                {
                    sum.PaymentPlans.AddRange(newp);
                }
            }
        }
        public async Task<ReturnObject<ExistingDebtPlanModel>> GetExistDebtPlan(long? ID, string amountGroup, int? month)
        {
            var ret = new ReturnObject<ExistingDebtPlanModel>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var p = new PlanProjectListParameter
                {
                    PlanID = ID.Value,

                };
                var data = await GetExistDebtPlan(p, eGetPlanType.GetByID,amountGroup, month);
                if (!data.IsCompleted)
                {
                    ret.CloneMessage(data.Message);
                    return ret;
                }
                ret.Data = data.Data.Data;
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }


        public async Task<ReturnObject<long?>> ModifyExistDebtPlan(ExistingDebtPlanModel p, string userID,  long planID, string amountGroup, int? month)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var plan = await DB.Plan.Include(pt => pt.PlanTypeNavigation)
                    .Include(patt => patt.PlanAttach).ThenInclude(att => att.AttachFile)
                    .Include(ex => ex.PlanExtend)
                    .Include(e => e.PlanExist).ThenInclude(ea => ea.PlanExistAgreement)
                    .Include(e => e.PlanExist).ThenInclude(ea => ea.PlanExistAgreement).ThenInclude(tt => tt.TransactionTypeNavigation)
                    .Where(w => w.PlanId == planID).FirstOrDefaultAsync();

                if (plan == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { _msglocalizer[eMessage.Plan.ToString()] });
                    return ret;
                }


                var rate = _helper.GetCurrencyRate(plan.StartYear);
                if (!rate.IsCompleted)
                {
                    ret.CloneMessage(rate.Message);
                    return ret;
                }
                #region delete data
                var delExist = new List<PlanExist>();
                foreach (var del in plan.PlanExist)
                {
                    var d = p.PlanDetails.Where(w => w.PlanExistID == del.PlanExistId).FirstOrDefault();
                    if (d == null)
                    {
                        delExist.Add(del);
                    }
                }
                foreach (var del in delExist)
                {
                    plan.PlanExist.Remove(del);
                    // DB.PlanExist.Remove(del);
                }
                #endregion

                var lovDebtPlType = _helper.GetLOVByGroup(ServiceModels.Constants.LOVGroup.DebtPaymentPlanType._LOVGroupCode);
                var lovPaySource = _helper.GetLOVByGroup(ServiceModels.Constants.LOVGroup.Payment_Source._LOVGroupCode);
                var lovTransType = _helper.GetLOVByGroup(ServiceModels.Constants.LOVGroup.Transaction_CashFlow_Type_from_GF._LOVGroupCode);
                foreach (var pPDG in p.PlanDetails) // p.PlanDetails)
                {

                    if (!pPDG.PlanExistID.HasValue)
                    {
                        pPDG.PlanExistID = 0;
                    }
                    var dbPlEx = plan.PlanExist.Where(w => w.PlanExistId == pPDG.PlanExistID).FirstOrDefault();
                    if (dbPlEx == null)
                    {
                        dbPlEx = new PlanExist();
                        plan.PlanExist.Add(dbPlEx);
                    }
                    else
                    {
                        #region delete PlanExistAgree
                        var delExAg = new List<PlanExistAgreement>();
                        foreach (var delea in dbPlEx.PlanExistAgreement)
                        {

                            var del = pPDG.AgreementDetail.Where(w => w.AgreementID == delea.AgreementId && w.TransactionType.Code == w.TransactionType?.Code).FirstOrDefault();
                            if (del == null)
                            {
                                delExAg.Add(delea);
                            }
                        }
                        foreach (var d in delExAg)
                        {
                            dbPlEx.PlanExistAgreement.Remove(d);
                            DB.PlanExistAgreement.Remove(d);
                        }
                        #endregion
                    }

                    //if (dbPlEx != null)
                    //{
                    dbPlEx.IsNotRequiredApproval = pPDG.IsNotRequiredApproval;
                    dbPlEx.Year = pPDG.Year;
                    //}
                    foreach (var pArg in pPDG.AgreementDetail)
                    {
                        var lov = lovTransType.Data.Where(w => w.LOVCode == pArg.TransactionType.Code).FirstOrDefault();
                        if (lov == null)
                        {
                            ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { $"TransactionType {pArg.TransactionType.Code}" });
                            return ret;
                        }
                        var plArg = dbPlEx.PlanExistAgreement.Where(w => w.AgreementId == pArg.AgreementID && w.TransactionType == lov.LOVKey).FirstOrDefault();
                        if (plArg == null)
                        {
                            plArg = new PlanExistAgreement
                            {
                                AgreementId = pArg.AgreementID,
                                TransactionType = lov.LOVKey

                            };
                            dbPlEx.PlanExistAgreement.Add(plArg);
                        }
                        plArg.ActualDueAmount = pArg.ActualDueAmount;
                        plArg.Objective = pArg.Objective != null ? pArg.Objective.ID : null;
                        plArg.PlanType = pArg.PlanType != null ? pArg.PlanType.ID : null;
                        plArg.ActualDueDate = pArg.ActualDueDate;
                        //DB Trigger will be fired (GetMasterAgreement)
                        plArg.MasterAgreement = DateTime.Now.Ticks.ToString();
                    }
                    #region PaymentPlan
                    var payamt = pPDG.PaymentPlan;
                    //แผนการบริหารหนี้เดิม PAY = แผนการชำระหนี้, RES = แผนการปรับโครงสร้างหนี้
                    var manageType = "PAY";

                    var dbPayPls = await DB.PaymentPlan.Include(ps => ps.PaymentSourceNavigation).Include(d => d.DebtPaymentPlanTypeNavigation).Include(d => d.DebtPayAmt)
                        .ThenInclude(pa => pa.PlanAmountNavigation).Include(d => d.DebtPayAmt)
                        .ThenInclude(pa => pa.InterestSaveAmountNavigation).Where(w => w.PlanExistId == pPDG.PlanExistID).ToListAsync();
                    if (pPDG.PaymentPlan == null) pPDG.PaymentPlan = new List<DebtPaymentPlan>();
                    if (pPDG.RestructurePlan == null) pPDG.RestructurePlan = new List<DebtPaymentPlan>();
                    deleltPaymentPlan(pPDG.PaymentPlan, dbPayPls, "PAY");
                    deleltPaymentPlan(pPDG.RestructurePlan, dbPayPls, "RES");
                    updatePaymentPlan(pPDG.PaymentPlan, dbPayPls, "PAY", dbPlEx, lovDebtPlType.Data, lovPaySource.Data, amountGroup);
                    updatePaymentPlan(pPDG.RestructurePlan, dbPayPls, "RES", dbPlEx, lovDebtPlType.Data, lovPaySource.Data, amountGroup);
                    #endregion
                }
                 
                
                if (p.DebtSettlementInfo != null && p.DebtSettlementInfo.AttatchFiles != null)
                {
                    var oatt = await PlanAttachFile(DB, plan, p.DebtSettlementInfo.AttatchFiles);
                    if (!oatt.IsCompleted)
                    {
                        ret.CloneMessage(oatt.Message);
                        return ret;
                    }
                    p.DebtSettlementInfo.AttatchFiles = null;
                }

                var remC = plan.PlanExtend.Where(w => w.DataGroup == "DEBTSTTLEMENTINFO").ToList();
                foreach (var co in remC)
                {
                    plan.PlanExtend.Remove(co);
                }
                if (p.DebtSettlementInfo != null)
                {
                    plan.PlanExtend.Add(new PlanExtend
                    {
                        Data = JsonConvert.SerializeObject(p.DebtSettlementInfo),
                        DataGroup = "DEBTSTTLEMENTINFO"
                    });
                }
                var newLog = new DataLog { LogDt = DateTime.Now, LogType = "U", TableName = "Plan", UserId = userID, TableKey = plan.PlanId };
                DB.DataLog.Add(newLog);
                plan.DataLog = newLog.LogId;

                await DB.SaveChangesAsync();
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        private void updatePaymentPlan(List<DebtPaymentPlan> dpps, List<PaymentPlan> pps, string manageType, PlanExist pe, List<LOV> lovDebtPlanType, List<LOV> lovPaySource, string amountGroup)
        {
            
            foreach (var dpp in dpps)
            {
                if (dpp.LoanSourcePlans.Count() != dpp.InterestSaving.calculatedSaving.Count())
                {
                    throw new Exception("จำนวน LoanSourcePlans ไม่เท่ากับ InterestSaving.calculatedSaving ");
                }
                var dbPayPl = pps.Where(w => w.ManageType == manageType && w.DebtPaymentPlanTypeNavigation.Lovcode == dpp.DebtPaymentPlanType && w.PaymentSourceNavigation.Lovcode == dpp.PaymentSource).FirstOrDefault();
                if (dbPayPl == null)
                {
                    var dpId = Utilities.GetLovKeyFromCode(lovDebtPlanType, dpp.DebtPaymentPlanType);
                    var psId = Utilities.GetLovKeyFromCode(lovPaySource, dpp.PaymentSource);
                    dbPayPl = new PaymentPlan
                    {
                        ManageType = manageType,
                        //IsRequestGuarantee = dpp.IsRequestGuarantee,
                        PaymentSource = psId,
                        DebtPaymentPlanType = dpId,
                        DebtPayAmt = new List<DebtPayAmt>()
                    };
                    pe.PaymentPlan.Add(dbPayPl);
                    //pps.Add(dbPayPl);
                }
                dbPayPl.IsRequestGuarantee = dpp.IsRequestGuarantee;
                var delAmt = new List<DebtPayAmt>();
                foreach (var da in dbPayPl.DebtPayAmt)
                {
                    var delda = dpp.LoanSourcePlans.Where(w => w.SourceType == da.PlanAmountNavigation.SourceType && w.Currency == da.PlanAmountNavigation.Currency).FirstOrDefault();
                    if (delda == null)
                    {
                        delAmt.Add(da);
                    }
                }
                foreach (var d in delAmt)
                {
                    DB.DebtPayAmt.Remove(d);
                    dbPayPl.DebtPayAmt.Remove(d);
                }

                int rec = 0;
                foreach (var damt in dpp.LoanSourcePlans)
                {
                    Amount pAmt;
                    Amount intAmt;
                    var intSave = dpp.InterestSaving.calculatedSaving[rec];
                    var dpa = dbPayPl.DebtPayAmt.Where(w => w.PlanAmountNavigation.SourceType == damt.SourceType && w.PlanAmountNavigation.Currency == damt.Currency).FirstOrDefault();
                    
                    if (dpa == null)
                    {
                        pAmt = new Amount
                        {
                            Currency = damt.Currency,
                            SourceType = damt.SourceType,
                            PeriodType = "Y",
                            PeriodValue = 0,
                            AmountGroup = amountGroup
                        };
                        intAmt = new Amount
                        {
                            Currency = damt.Currency,
                            SourceType = damt.SourceType,
                            PeriodType = "Y",
                            PeriodValue = 0,
                            AmountGroup = amountGroup
                        };
                        dpa = new DebtPayAmt
                        {
                            PlanAmountNavigation = pAmt,
                            InterestSaveAmountNavigation = intAmt

                        };
                        dbPayPl.DebtPayAmt.Add(dpa);
                    }else
                    {
                        pAmt = dpa.PlanAmountNavigation;
                        intAmt = dpa.InterestSaveAmountNavigation;
                    }
                    pAmt.Amount1 = damt.LoanAmount;
                    intAmt.Amount1 = intSave.LoanAmount;
                    dpa.InterestReference = JsonConvert.SerializeObject(dpp.InterestSaving.calcurateReferences);
                    rec += 1;
                }
            }
        }
        private void  deleltPaymentPlan(List<DebtPaymentPlan> p, List<PaymentPlan> db,string manageType)
        {
            var mts = db.Where(w => w.ManageType == manageType).ToList();
            var delPP = new List<PaymentPlan>();
            foreach (var mt in mts)
            {
                var del = p.Where(w => w.DebtPaymentPlanType == mt.DebtPaymentPlanTypeNavigation.Lovcode && w.PaymentSource == mt.PaymentSourceNavigation.Lovcode).FirstOrDefault();
                if (del == null)
                {
                    delPP.Add(mt);
                }
            }
            foreach (var d in delPP)
            {
                db.Remove(d);
                DB.PaymentPlan.Remove(d);
            }
           

        }
        public async Task<ReturnObject<long?>> AddAgreementToPlan(long planID, long agreementID, string userID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {

                //var proj = await DB.Agreement.Where(w => w.AgreementId == agreementID).FirstOrDefaultAsync();
                //if (proj == null)
                //{
                //    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { "สัญญา" });
                //    return ret;
                //}
                //var plan = await DB.Plan.Where(w => w.PlanId == planID).FirstOrDefaultAsync();
                //if (plan == null)
                //{
                //    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { "แผนงาน" });
                //    return ret;
                //}
                //var dup = await DB.PlanProject.Where(w => w.PlanId == planID && w.ProjectId == projID).FirstOrDefaultAsync();
                //if (dup != null)
                //{
                //    ret.AddMessage(eMessage.ProjIsAlreadyInPlan.ToString(), "dup", eMessageType.Error, new string[] { proj.ProjectCode, plan.PlanCode });
                //    return ret;
                //}
                //long pjtKey;
                //if (string.IsNullOrEmpty(projType))
                //{
                //    pjtKey = proj.ProjectType;
                //}
                //else
                //{
                //    var ck = _helper.LOVCodeValidate(projType, ServiceModels.Constants.LOVGroup.Project_Type._LOVGroupCode, null);
                //    if (!ck.IsCompleted)
                //    {
                //        ret.CloneMessage(ck.Message);
                //        return ret;
                //    }
                //    pjtKey = ck.Data.LOVKey;
                //}

                //var pp = new PlanProject { PlanId = planID, ProjectId = projID, ProjectType = pjtKey };
                //var log = new DataLog { LogDt = DateTime.Now, LogType = "U", TableName = "Plan", UserId = userID, TableKey = planID };
                //plan.DataLogNavigation = log;
                //if (AddActivies)
                //{
                //    var acts = await DB.ProjAct.Where(w => w.ProjectId == projID).ToListAsync();
                //    foreach (var act in acts)
                //    {
                //        var pa = new PlanAct
                //        {
                //            ActivityName = act.ActivityName,
                //            ProjActId = act.ProjActId,
                //        };
                //        pp.PlanAct.Add(pa);
                //    }
                //}

                //DB.PlanProject.Add(pp);
                //await DB.SaveChangesAsync();
                //ret.IsCompleted = true;
                //ret.Data = pp.PlanProjectId;

            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;

        }
        public async Task<ReturnMessage> DuplicateExistPlan(long newPlan, CreatePlanParameter p)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var newPl = await DB.Plan.Where(w => w.PlanId == newPlan).FirstOrDefaultAsync();
                if (newPl == null)
                {
                    return ret;
                }
                var old = await GetPreviousPlanRevision(p, ServiceModels.Constants.LOVGroup.Plan_Type.แผนบริหารหนี้เดิม );
                if (!old.IsCompleted)
                {
                    return ret;
                }
                var oldPl = await DB.Plan
                    .Include(e => e.PlanExist).ThenInclude(ea => ea.PlanExistAgreement)
                    //.Include(e => e.PlanExist).ThenInclude(pay => pay.PaymentPlan) ///.ThenInclude(dbt => dbt.DebtPaymentPlanTypeNavigation)
                    //.Include(e => e.PlanExist).ThenInclude(pay => pay.PaymentPlan) //.ThenInclude(py => py.PaymentSourceNavigation)
                    .Include(e => e.PlanExist).ThenInclude(pay => pay.PaymentPlan).ThenInclude(dp => dp.DebtPayAmt).ThenInclude(pamt => pamt.PlanAmountNavigation)
                    .Include(e => e.PlanExist).ThenInclude(pay => pay.PaymentPlan).ThenInclude(dp => dp.DebtPayAmt).ThenInclude(iamt => iamt.InterestSaveAmountNavigation)
                    .Include(e => e.PlanExist).ThenInclude(pay => pay.PaymentPlan).ThenInclude(ap => ap.AgreementPaymentPlan)
                    .Where(w => w.PlanId == old.Data.Value).FirstOrDefaultAsync();
                foreach (var pe in oldPl.PlanExist)
                {
                    var newPe = new PlanExist
                    {
                        Year = pe.Year
                    };
                    newPl.PlanExist.Add(newPe);
                    foreach (var pea in pe.PlanExistAgreement)
                    {
                        var newPea = new PlanExistAgreement
                        {
                            AgreementId = pea.AgreementId,
                            ActualDueAmount = pea.ActualDueAmount,
                            TransactionType = pea.TransactionType

                        };
                        newPe.PlanExistAgreement.Add(newPea);
                    }
                    foreach (var pp in pe.PaymentPlan)
                    {
                        var newPp = new PaymentPlan
                        {
                            ManageType = pp.ManageType,
                            IsRequestGuarantee = pp.IsRequestGuarantee,
                            DebtPaymentPlanType = pp.DebtPaymentPlanType,
                            PaymentSource = pp.PaymentSource
                        };
                        newPe.PaymentPlan.Add(newPp);
                        foreach (var dpa in pp.DebtPayAmt)
                        {
                            var newDpa = new DebtPayAmt
                            {
                                InterestReference = dpa.InterestReference,
                                PlanAmountNavigation = new Amount
                                {
                                    Amount1 = dpa.PlanAmountNavigation.Amount1,
                                    AmountType = dpa.PlanAmountNavigation.AmountType,
                                    Currency = dpa.PlanAmountNavigation.Currency,
                                    PeriodType = dpa.PlanAmountNavigation.PeriodType,
                                    PeriodValue = dpa.PlanAmountNavigation.PeriodValue,
                                    SourceType = dpa.PlanAmountNavigation.SourceType,
                                    AmountGroup = dpa.PlanAmountNavigation.AmountGroup
                                },
                                InterestSaveAmountNavigation = new Amount
                                {
                                    Amount1 = dpa.InterestSaveAmountNavigation.Amount1,
                                    AmountType = dpa.InterestSaveAmountNavigation.AmountType,
                                    Currency = dpa.InterestSaveAmountNavigation.Currency,
                                    PeriodType = dpa.InterestSaveAmountNavigation.PeriodType,
                                    PeriodValue = dpa.InterestSaveAmountNavigation.PeriodValue,
                                    SourceType = dpa.InterestSaveAmountNavigation.SourceType,
                                    AmountGroup = dpa.InterestSaveAmountNavigation.AmountGroup,
                                }
                            };
                            newPp.DebtPayAmt.Add(newDpa);
                        }
                        foreach (var app in pp.AgreementPaymentPlan)
                        {
                            var newApp = new AgreementPaymentPlan
                            {
                                AgreementId = app.AgreementId
                            };
                            newPp.AgreementPaymentPlan.Add(newApp);
                        }
                       // pp.AgreementPaymentPlan.Clear();
                    }
                }
             

                await DB.SaveChangesAsync();
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
    }
}
