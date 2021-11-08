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
using mof.ServiceModels.PublicDebt;

namespace mof.Services
{
    public partial class CommonRepository : ICommon
    {


        public MOFContext DB;
        private IStringLocalizer<MessageLocalizer> _msglocalizer;
        private ISystemHelper _helper;
        public CommonRepository(MOFContext db ,
            IStringLocalizer<MessageLocalizer> msglocalizer,
            ISystemHelper helper
        )
        {
            DB = db;
            _msglocalizer = msglocalizer;
            _helper = helper;
 
        }
        public async Task<ReturnList<ExistPlanAgreementList>> GetPlanAgreementList(List<PlanExist> p)
        {
            var ret = new ReturnList<ExistPlanAgreementList>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {

                var exists = p;
                var data = new List<ExistPlanAgreementList>();
                foreach (var ex in exists)
                {
                    var newEX = new ExistPlanAgreementList
                    {
                        Agreements = ex.PlanExistAgreement.Select(s => new AgreementModel
                        {
                            AgreementID = s.AgreementId,
                            Description = s.Agreement.Description,
                            ReferenceCode = s.Agreement.ReferenceCode
                        }).ToList(),
                        PlanExistID = ex.PlanExistId,
                        DebtPaymentPlans = new List<DebtPaymentPlan>()

                    };
                    data.Add(newEX);
                    foreach (var res in ex.PaymentPlan.Where(w => w.ManageType == "RES"))
                    {
                        var ppl = newEX.DebtPaymentPlans.Where(w => w.PaymentPlanID == res.PaymentPlanId).FirstOrDefault();
                        if (ppl == null)
                        {
                            ppl = new DebtPaymentPlan
                            {
                                DebtPaymentPlanType = res.DebtPaymentPlanTypeNavigation.Lovvalue,
                                IsRequestGuarantee = res.IsRequestGuarantee.HasValue ? res.IsRequestGuarantee.Value : false,
                                LoanSourcePlans = new List<LoanSource>(),
                                PaymentPlanID = res.PaymentPlanId,
                                PaymentSource = res.PaymentSourceNavigation.Lovvalue

                            };
                            newEX.DebtPaymentPlans.Add(ppl);
                        }
                        foreach (var amt in res.DebtPayAmt)
                        {
                            var ls = ppl.LoanSourcePlans.Where(w => w.SourceType == amt.PlanAmountNavigation.SourceType && w.Currency == amt.PlanAmountNavigation.Currency).FirstOrDefault();
                            if (ls == null)
                            {
                                ls = new LoanSource
                                {
                                    Currency = amt.PlanAmountNavigation.Currency,
                                    SourceType = amt.PlanAmountNavigation.SourceType
                                };
                                ppl.LoanSourcePlans.Add(ls);
                            }
                            ls.LoanAmount += amt.PlanAmountNavigation.Amount1;
                        }

                    }
                }
                ret.Data = data;
                ret.IsCompleted = true;



            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        private string[] loanTypeCodes = new string[]
{
                     ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้ตรง__รัฐบาล_,
                     ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้ต่อจากกระทรวงการคลัง_ของรัฐวิสาหกิจ_,
                     ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้โดยขอค้ำจากกระทรวงการคลัง,
                     ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้โดยไม่ขอค้ำจากกระทรวงการคลัง,
                     ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้มาเพื่อให้กู้ต่อ_ของรัฐบาล_

        };
        public async Task<ReturnList<NewDebtPlanActList>> GetNewDebtActList(List<PlanAct> p)
        {
            var ret = new ReturnList<NewDebtPlanActList>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
 
                var acts = p;
                var data = new List<NewDebtPlanActList>();
                foreach (var act in acts)
                {
                    var pact = new NewDebtPlanActList
                    {
                        ActivityName = act.ActivityName,
                        ProjectTHName = act.PlanProj.Project.ProjectThname,
                        PlanActID = act.PlanActId,
                        LoanSource = new List<LoanSource>()
                    };
                    foreach (var amt in act.PlanActAmount)
                    {
                        if (loanTypeCodes.Contains(amt.Amount.AmountTypeNavigation.Lovcode))
                        {
                            pact.LoanSource.Add(new LoanSource
                            {
                                Currency = amt.Amount.Currency,
                                LoanAmount = amt.Amount.Amount1,
                                SourceType = amt.Amount.SourceType
                            });
                        }
                    }
                    data.Add(pact);
                }
                ret.Data = data;
                ret.IsCompleted = true;


            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        public async Task<bool> IsProjectShortForm(string projType)
        {
            var chk = await DB.CeLov.Where(w => w.Lovcode == projType && w.LovgroupCode == ServiceModels.Constants.LOVGroup.Project_Type._LOVGroupCode).FirstOrDefaultAsync();
            if (chk != null)
            {
                return (chk.Lovcode == ServiceModels.Constants.LOVGroup.Project_Type.กู้เพื่อชดเชยการขาดดุลสำหรับปีงบประมาณ || chk.Lovcode == ServiceModels.Constants.LOVGroup.Project_Type.กู้เพื่อชดเชยการขาดดุลสำหรับการเบิกจ่ายกันเหลื่อมปี
                    || chk.Lovcode == ServiceModels.Constants.LOVGroup.Project_Type.กู้เพื่อบริหารสภาพคล่องของเงินคงคลัง);
            }
            return false;
            
        }
        #region Report
        public async Task<ReturnObject<PublicDebtDashBoard>> PublicDebtSummaryDashboard()
        {
            var ret = new ReturnObject<PublicDebtDashBoard>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var bc = DateTime.Now.Year + 543;
                var prm = DB.Parameter.Where(w => w.Year == bc).FirstOrDefault();
                if (prm == null || prm.Gdp == 0)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "gdp", eMessageType.Error, new string[] { "GDP" });
                    return ret;
                }
                var rate = _helper.GetCurrencyRate(bc);
                if (!rate.IsCompleted)
                {
                    ret.CloneMessage(rate.Message);
                    return ret;
                }
                //var agas = await DB.AgreementAct
                //    .GroupBy(g => new
                //    {
                //        orgtype = g.PlanAct.PlanProj.Plan.Organization.OrgtypeNavigation.Lovvalue,
                //        currency = g.Agreement.LoanCurrency
                //    })
                //    .Select(s => new {
                //        orgtype = s.Key.orgtype,
                //        currency = s.Key.currency,
                //        amount = s.Sum(sum => sum.Agreement.OutStandingDebt) }).ToListAsync();
                var data = new PublicDebtDashBoard
                {
                    publicDebtAndGDPComposeChart = new List<CommonComposeChart>(),
                    publicDebtCurrencyPieChart = new List<CommonPieChart> {
                       new CommonPieChart {name = "หนี้ในประเทศ",color = "#B19648" },
                       new CommonPieChart {name = "หนี้ต่างประเทศ", color = "#1B5BA4" }
                    },
                    publicDebtRemainningAgePieChart = new List<CommonPieChart>{
                       new CommonPieChart {name = "หนี้ระยะสั้น",color = "#1B5BA4" },
                       new CommonPieChart {name = "หนี้ระยะยาว", color = "#B19648" }
                    },
                    publicDebtSummaryTable = new List<PublicDebtSummary>()
                };
                //var ags = await DB.AgreementAct
                //    .Include(ag => ag.Agreement)
                //    .Include(pa => pa.PlanAct).ThenInclude(ppj => ppj.PlanProj)
                //    .ThenInclude(pl => pl.Plan).ThenInclude(o => o.Organization).ThenInclude(l => l.OrgtypeNavigation)
                //    .Where(w => w.Agreement.OutStandingDebt > 0 && w.PlanAct != null).ToListAsync();
                var ags = await DB.Agreement
                           .Include(o => o.Organization).ThenInclude(ot => ot.OrgtypeNavigation)
                           .Where(w => w.OutStandingDebt > 0 ).ToListAsync();
                decimal allDebt = 0;
                decimal outstanding = 0;
                foreach (var ag in ags)
                {
                    var r = rate.Data.Currency.Where(w => w.CurrencyCode == ag.LoanCurrency).FirstOrDefault();
                    if (r == null)
                    {
                        ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { $"สกุลเงิน {ag.LoanCurrency} ของปี {bc}" });
                        return ret;
                    }
                    outstanding = r.CurrencyRate * ag.OutStandingDebt;
                    allDebt += outstanding;
                    var org = data.publicDebtSummaryTable.Where(w => w.elementsDebt == ag.Organization.OrgtypeNavigation.Lovvalue).FirstOrDefault();
                    if (org == null)
                    {
                        org = new PublicDebtSummary
                        {
                            elementsDebt = ag.Organization.OrgtypeNavigation.Lovvalue,
                           
                        };
                        data.publicDebtSummaryTable.Add(org);
                       
                    }
                    org.millionBaht += outstanding;
                    if (!string.IsNullOrEmpty(ag.SourceType))
                    {
                        var debtsource = (ag.SourceType == "L") ? "หนี้ในประเทศ" : "หนี้ต่างประเทศ";
                        var chart = data.publicDebtCurrencyPieChart.Where(w => w.name == debtsource).FirstOrDefault();
                        if (chart != null)
                        {
                            chart.value += (outstanding / 1000000);
                        }
                    }
                    var age = (((TimeSpan)(DateTime.Now - ag.StartDate)).Days > 5 * 365) ? "หนี้ระยะยาว" : "หนี้ระยะสั้น";
                    var chartAge = data.publicDebtRemainningAgePieChart.Where(w => w.name == age).FirstOrDefault();
                    if (chartAge != null)
                    {
                        chartAge.value += (outstanding / 1000000);
                    }
                }
                foreach (var avg in data.publicDebtSummaryTable)
                {
                    avg.fiveYearPlan = Math.Round( (avg.millionBaht / allDebt) * 100 , 4);
                    avg.gdp = Math.Round((avg.millionBaht / prm.Gdp) * 100, 4);
                    avg.millionBaht = avg.millionBaht / 1000000;
                }
                data.publicDebtGDPValue = prm.Gdp;
                data.publicDebtGDPPercent = (allDebt / prm.Gdp) * 100;
                Random random = new Random();
          
                for (var i = bc - 9; i <= bc; i++)
                {
                    data.publicDebtAndGDPComposeChart.Add(new CommonComposeChart
                    {
                        name = i.ToString(),
                        gdp = random.Next(25, 75),
                        a = random.Next(25, 75),
                        b = random.Next(25, 75),
                        c = random.Next(25, 75),
                        d = random.Next(25, 75),
                        e = random.Next(25, 75),
                        f = random.Next(25, 75),
                        g = random.Next(25, 75)
                    });
                   
                }
                ret.Data = data;
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        #endregion
        public async Task<String> GetMasterAgreemet_Backup(long ppjId)
        {

            var p = await DB.PlanProject
                       .Include(pl => pl.Plan).ThenInclude(pt => pt.PlanTypeNavigation)
                       .Include(pt => pt.Plan.PlanTypeNavigation)
                       .Include(pj => pj.Project).ThenInclude(t => t.ProjectTypeNavigation)
                       .Include(o => o.Plan.Organization).ThenInclude(ot => ot.OrgtypeNavigation)
                       .Include(pa => pa.PlanAct).ThenInclude(paa => paa.PlanActAmount).ThenInclude(amt => amt.Amount)
                       .Include(pa => pa.PlanAct).ThenInclude(paa => paa.PlanActAmount).ThenInclude(amt => amt.Amount).ThenInclude(lov => lov.AmountTypeNavigation)
                       .Where(w => w.PlanProjectId == ppjId).FirstOrDefaultAsync();
            string ma = "";
            decimal L, F;
            L = 0;
            F = 0;
            foreach (var tmp in p.PlanAct)
            {
                foreach (var pa in tmp.PlanActAmount.Where(w => loanTypeCodes.Contains(w.Amount.AmountTypeNavigation.Lovcode)))
                {
                    if (pa.Amount.SourceType == "F")
                    {
                        F += pa.Amount.Amount1;
                    }
                    if (pa.Amount.SourceType == "L")
                    {
                        L += pa.Amount.Amount1;
                    }
                }

            }
            var st = "L";
            if (F > 0 && L == 0)
            {
                st = "F";
            }
            string orgType, Debt, ojt;
            orgType = Debt = ojt = "";
            if (p.Plan.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.แผนก่อหนี้ใหม่)

            {
                var gov = new string[] { ServiceModels.Constants.LOVGroup.ประเภทหน่วยงาน.ส่วนราชการ__กระทรวง_ทบวง_กรม__ };
                if (gov.Contains(p.Plan.Organization.OrgtypeNavigation.Lovcode))
                {
                    orgType = "G";
                }
                else if (p.Plan.Organization.OrgtypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.ประเภทหน่วยงาน.รัฐวิสาหกิจที่ทำธุรกิจให้กู้ยืมเงิน)
                {
                    orgType = "F";
                }
                else if (p.Plan.Organization.OrgtypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.ประเภทหน่วยงาน.รัฐวิสาหกิจ)
                {
                    orgType = "S";
                }
                else if (p.Plan.Organization.OrgtypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.ประเภทหน่วยงาน.หน่วยงานอื่น_ๆ_ของรัฐ)
                {
                    orgType = "A";
                }
                else
                {
                    orgType = "O";
                }

                Debt = (st == "L") ? "A" : "B";

                if (p.Project.ProjectTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Type.กู้เพื่อลงทุนในโครงการพัฒนา)
                {
                    ojt = "01";
                }
                else if (p.Project.ProjectTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Type.กู้เพื่อโครงการ)
                {
                    ojt = "03";
                }
                else
                {
                    ojt = "09";
                }

                ma = $"{orgType}-{p.Plan.StartYear.ToString().PadRight(4, ' ').Substring(2, 2)}-{Debt}-{ojt}";
            }
            //var edit = await DB.PlanProject.Where(w => w.PlanProjectId == ppjId).FirstOrDefaultAsync();
            //if (edit != null)
            //{
            //    edit.MasterAgreement = ma;
            //    await DB.SaveChangesAsync();
            //}

            return ma;

        }
        public async Task<String> GetMasterAgreemet(long ppjId)
        {

            var ret = await DB.PlanProject.Where(w => w.PlanProjectId == ppjId).Select( s => new { s.PlanProjectId, data = MOFContext.GetMasterAgreement(ppjId) }).FirstOrDefaultAsync();
            if (ret != null)
            {
                return ret.data;
            }else
            {
                return "";
            }


        }
    }
}
