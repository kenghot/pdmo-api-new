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
using mof.Services.Helper;
using static mof.Services.Helper.Utilities;

namespace mof.Services
{
    public partial class PlanRepository : IPlan
    {
        public enum ePlanProjType
        {
            Goverment,
            Normal
        }
        private void storePlanLoanSummary(ExpenseSummary ex, string sourceType, decimal amt)
        {
            if (sourceType == "L")
            {
                ex.LocalLoan += amt;
            }
            else if (sourceType == "F")
            {
                ex.ForeignLoan += amt;
            }
            else if (sourceType == "O")
            {
                ex.OtherLoan += amt;
            }
            if (sourceType != "O")
            {
                ex.TotalLoan += amt;
            }
            ex.GrandTotal += amt;
        }
        private void saveNewDebtAmt(List<LOV> lov, string periodType, int periodValue, string AmountType, List<AmountData> amts, ICollection<PlanActAmount> paa, PlanAct pa, string amountGroup)
        {
            //var chk = _helper.LOVCodeValidate(AmountType, ServiceModels.Constants.LOVGroup.Project_Amount_Type._LOVGroupCode, null);
            var chk = lov.Where(w => w.LOVCode == AmountType).FirstOrDefault(); // _helper.LOVCodeValidate(AmountType, ServiceModels.Constants.LOVGroup.Project_Amount_Type._LOVGroupCode, null);
            //if (!chk.IsCompleted)
            //{
            //    throw new Exception("Error LOVGroup Project Amount Type code " + AmountType);
            //}
            var paaFilter = paa.Where(w => w.Amount.AmountType == chk.LOVKey && w.Amount.PeriodValue == periodValue && w.Amount.PeriodType == periodType && w.Amount.AmountGroup == amountGroup).ToList();
            foreach (var tmp in paaFilter)
            {
                var find = amts.Where(w => w.CurrencyCode == tmp.Amount.Currency && w.SourceType == tmp.Amount.SourceType).FirstOrDefault();
                if (find == null)
                {
                    DB.PlanActAmount.Remove(tmp);
                }
            }
            foreach (var amt in amts)
            {

                var editpaa = paaFilter.Where(w => w.Amount.Currency == amt.CurrencyCode && w.Amount.SourceType == amt.SourceType).FirstOrDefault();
                if (editpaa == null)
                {
                    editpaa = new PlanActAmount
                    {

                        Amount = new DataModels.Models.Amount
                        {
                            AmountType = chk.LOVKey,
                            Amount1 = amt.Amount,
                            Currency = amt.CurrencyCode,
                            PeriodType = periodType,
                            PeriodValue = periodValue,
                            SourceType = amt.SourceType,
                            AmountGroup = amountGroup
                        }
                    };
                    pa.PlanActAmount.Add(editpaa);
                }

                editpaa.Amount.Amount1 = amt.Amount;
            }
        }
        //public async Task<ReturnObject<long?>> ModifyNewDebtPlanOld(NewDebtPlanModel p, string userID, long projectID, long planID)
        //{
        //    var ret = new ReturnObject<long?>(_msglocalizer);
        //    ret.IsCompleted = false;
        //    try
        //    {

        //        var plan = await DB.Plan.Include(pt => pt.PlanTypeNavigation).Where(w => w.PlanId == planID).FirstOrDefaultAsync();
        //        if (plan == null)
        //        {
        //            ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { "แผนงาน" });
        //            return ret;
        //        }

        //        //var project = await DB.Project.Where(w => w.ProjectId == plan.pr).FirstOrDefaultAsync();
        //        //if (project == null)
        //        //{
        //        //    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { "โครงการ" });
        //        //    return ret;
        //        //}

        //        var rate = _helper.GetCurrencyRate(plan.StartYear);
        //        if (!rate.IsCompleted)
        //        {
        //            ret.CloneMessage(rate.Message);
        //            return ret;
        //        }
        //        #region Activites
        //        foreach (var pdt in p.PlanDetails)
        //        {
        //            var planproj = await DB.PlanProject.Include(pl => pl.Plan).Include(pt => pt.Plan.PlanTypeNavigation).Where(w => w.PlanId == planID && w.ProjectId == pdt.ProjectID).FirstOrDefaultAsync();
        //            if (planproj == null)
        //            {
        //                ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { "โครงการในแผนงาน" });
        //                return ret;
        //            }
        //            var allpa = await DB.PlanAct.Include(i => i.PlanActAmount).ThenInclude(th => th.Amount).Where(w => w.PlanProjId == planproj.PlanProjectId).ToListAsync();
        //            var delpa = new List<PlanAct>();


        //            foreach (var pa in allpa)
        //            {
        //                var del = pdt.ActivityPlans.Where(w => w.Activity.ProjActID == pa.PlanActId).FirstOrDefault();
        //                if (del == null)
        //                {
        //                    DB.PlanAct.Remove(pa);
        //                    delpa.Add(pa);
        //                }
        //            }
        //            foreach (var del in delpa)
        //            {
        //                allpa.Remove(del);
        //            }
        //            foreach (var ap in pdt.ActivityPlans)
        //            {

        //                var act = ap.Activity;
        //                PlanAct newpa;

        //                newpa = allpa.Where(w => w.PlanActId == act.ProjActID).FirstOrDefault();
        //                if (newpa == null)
        //                {
        //                    newpa = new PlanAct();
        //                    planproj.PlanAct.Add(newpa);
        //                }
        //                newpa.ActivityName = act.ActivityName;
        //                var paa = newpa.PlanActAmount;

        //                var period = plan.StartYear;

        //                if (act.ResolutionAmounts != null) saveNewDebtAmt("Y", period, ServiceModels.Constants.LOVGroup.Project_Amount_Type.ค่าใช้จ่ายตามมติ, act.ResolutionAmounts, paa, newpa);
        //                if (act.ContractAmounts != null) saveNewDebtAmt("Y", period, ServiceModels.Constants.LOVGroup.Project_Amount_Type.ค่าใช้จ่ายตามสัญญาจ้าง, act.ContractAmounts, paa, newpa);
        //                if (act.SaveProceedData != null)
        //                {
        //                    foreach (var save in act.SaveProceedData)
        //                    {
        //                        // var strPeriod =   save.Year.ToString() + save.Month.ToString().PadLeft(2, '0');
        //                        // int.Parse(strPeriod);
        //                        if (save.Budget != null) saveNewDebtAmt("Y", period, ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินงบประมาณ, save.Budget, paa, newpa);
        //                        if (save.Revernue != null) saveNewDebtAmt("Y", period, ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินรายได้, save.Revernue, paa, newpa);
        //                        if (save.SignedLoan != null) saveNewDebtAmt("Y", period, ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินกู้ลงนาม, save.SignedLoan, paa, newpa);
        //                        if (save.DisburseLoan != null) saveNewDebtAmt("Y", period, ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินกู้เบิกจ่าย, save.DisburseLoan, paa, newpa);
        //                        if (save.Other != null) saveNewDebtAmt("Y", period, ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินจากแหล่งอื่นๆ, save.Other, paa, newpa);
        //                    }
        //                }
        //                if (ap.LoanTypePlans != null)
        //                {
        //                    foreach (var ltp in ap.LoanTypePlans)
        //                    {
        //                        var ltpAmt = ltp.LoanSourcePlans.Select(s => new AmountData
        //                        {
        //                            Amount = s.LoanAmount,
        //                            CurrencyCode = s.Currency,
        //                            SourceType = s.SourceType

        //                        }).ToList();
        //                        saveNewDebtAmt("Y", period, ltp.LoanType.SelectedType, ltpAmt, paa, newpa);
        //                    }
        //                }
        //                if (ap.LoanProcessPlan != null)
        //                {
        //                    var lppAmts = new List<SaveProceed>();

        //                    foreach (var lpp in ap.LoanProcessPlan)
        //                    {
        //                        foreach (var lppd in lpp.LoanProcessPlanDetails)
        //                        {
        //                            var save = lppAmts.Where(w => w.Month == lppd.Month && w.Year == plan.StartYear).FirstOrDefault();
        //                            if (save == null)
        //                            {
        //                                save = new SaveProceed
        //                                {
        //                                    Year = plan.StartYear,
        //                                    Month = lppd.Month,
        //                                    SignedLoan = new List<AmountData>(),
        //                                    DisburseLoan = new List<AmountData>()
        //                                };
        //                                lppAmts.Add(save);
        //                            };
        //                            storeAmountData(save.SignedLoan, lppd.SignedLoan, lpp.LoanSource.Currency, lpp.LoanSource.SourceType);
        //                            storeAmountData(save.DisburseLoan, lppd.DisburseLoan, lpp.LoanSource.Currency, lpp.LoanSource.SourceType);

        //                        }

        //                    }

        //                    foreach (var lppamt in lppAmts)
        //                    {
        //                        var strPeriod = lppamt.Year.ToString() + lppamt.Month.ToString().PadLeft(2, '0');
        //                        var pr = int.Parse(strPeriod);

        //                        if (lppamt.SignedLoan != null) saveNewDebtAmt("M", pr, ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินกู้ลงนาม, lppamt.SignedLoan, paa, newpa);
        //                        if (lppamt.DisburseLoan != null) saveNewDebtAmt("M", pr, ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินกู้เบิกจ่าย, lppamt.DisburseLoan, paa, newpa);

        //                    }
        //                }

        //            }
        //        }

        //        #endregion
        //        var newLog = new DataLog { LogDt = DateTime.Now, LogType = "U", TableName = "Plan", UserId = userID, TableKey = plan.PlanId };
        //        DB.DataLog.Add(newLog);
        //        plan.DataLog = newLog.LogId;

        //        await DB.SaveChangesAsync();
        //        ret.IsCompleted = true;
        //        ret.Data = plan.PlanId;
        //    }
        //    catch (Exception ex)
        //    {
        //        ret.AddError(ex);
        //    }
        //    return ret;
        //}
        public async Task<ReturnObject<long?>> ModifyNewDebtPlan(NewDebtPlanModel p, string userID, long projectID, long planID, string amountGroup, int? month)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {

                var plan = await DB.Plan.Include(pt => pt.PlanTypeNavigation).Where(w => w.PlanId == planID).FirstOrDefaultAsync();
                if (plan == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { "แผนงาน" });
                    return ret;
                }

                //var project = await DB.Project.Where(w => w.ProjectId == plan.pr).FirstOrDefaultAsync();
                //if (project == null)
                //{
                //    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { "โครงการ" });
                //    return ret;
                //}

                var rate = _helper.GetCurrencyRate(plan.StartYear);
                if (!rate.IsCompleted)
                {
                    ret.CloneMessage(rate.Message);
                    return ret;
                }
                var lovs = _helper.GetLOVByGroup(ServiceModels.Constants.LOVGroup.Project_Amount_Type._LOVGroupCode);
                var pl309 = await DB.Plan.Where(w => w.PlanId == planID && w.Organization.OrganizationCode == "309").FirstOrDefaultAsync();
                //delete project
                var allppj = p.PlanDetails.Select(s => s.PlanID).ToList();
                var tmp = await DB.PlanProject.Where(w => w.PlanId == planID).Select(s => s.PlanProjectId).ToListAsync();
                if (amountGroup == "P")
                {
                    var delppjs = await DB.PlanProject.Where(w => w.PlanId == planID && !allppj.Contains(w.PlanProjectId)).ToListAsync();
                    foreach (var del in delppjs)
                    {
                        DB.PlanProject.Remove(del);
                        p.PlanDetails.RemoveAll(r => r.PlanID == del.PlanProjectId);
                    }
                }
                int? mthRep = null;
                if (amountGroup == "R" && month.HasValue)
                {
                    mthRep = int.Parse(plan.StartYear.ToString().PadLeft(4, '0') + month.Value.ToString().PadLeft(2, '0'));
                }
                

                #region Activites
                foreach (var pdt in p.PlanDetails)
                {
                    if (pdt.PlanProjType != ePlanProjType.Normal.ToString())
                    {
                        continue;
                    }
                    //var planproj = await DB.PlanProject.Include(pl => pl.Plan).Include(pt => pt.Plan.PlanTypeNavigation).Where(w => w.PlanId == planID && w.ProjectId == pdt.ProjectID).FirstOrDefaultAsync();
                    var planproj = await DB.PlanProject
                        .Include(pl => pl.Plan).ThenInclude(pt => pt.PlanTypeNavigation)
                        .Include(pt => pt.Plan.PlanTypeNavigation)
                        .Include(pj => pj.Project).ThenInclude(t => t.ProjectTypeNavigation)
                        .Include(o => o.Plan.Organization).ThenInclude(ot => ot.OrgtypeNavigation)
                        .Where(w => w.PlanProjectId == pdt.PlanID).FirstOrDefaultAsync();
                    if (planproj == null)
                    {
                        ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { "โครงการในแผนงาน" });
                        return ret;
                    }

                    var allpa = await DB.PlanAct.Include(i => i.PlanActAmount).ThenInclude(th => th.Amount).Where(w => w.PlanProjId == planproj.PlanProjectId).ToListAsync();
                    var delpa = new List<PlanAct>();
                    planproj.IsNotRequiredApproval = pdt.isNotRequiredApproval;
                    //DB Trigger will be fired (GetMasterAgreement)
                    planproj.MasterAgreement = DateTime.Now.Ticks.ToString();
                    if (amountGroup == "P")
                    {
                        foreach (var pa in allpa)
                        {
                            var del = pdt.ActivityPlans.Where(w => w.Activity.ProjActID == pa.PlanActId).FirstOrDefault();
                            if (del == null)
                            {
                                DB.PlanAct.Remove(pa);
                                delpa.Add(pa);
                            }
                        }
                        foreach (var del in delpa)
                        {
                            allpa.Remove(del);
                        }
                    }
                     
                    foreach (var act in pdt.ActivityPlans)
                    {
                     

                        //var act = ap.Activity;
                        PlanAct newpa;

                        newpa = allpa.Where(w => w.PlanActId == act.Activity.ProjActID).FirstOrDefault();
                        if (newpa == null)
                        {
                            newpa = new PlanAct();
                            planproj.PlanAct.Add(newpa);
                        }
                        newpa.ActivityName = act.Activity.ActivityName;
                        var paa = newpa.PlanActAmount;

                        var period = plan.StartYear;
                        var actPeriod = mthRep.HasValue ? mthRep.Value : plan.StartYear;
                        var actPrType = mthRep.HasValue ? "M" : "Y";

                        //if (act.ResolutionAmounts != null)  saveNewDebtAmt("Y", period, ServiceModels.Constants.LOVGroup.Project_Amount_Type.ค่าใช้จ่ายตามมติ, act.ResolutionAmounts, paa, newpa);
                        //if (act.ContractAmounts != null)  saveNewDebtAmt("Y", period, ServiceModels.Constants.LOVGroup.Project_Amount_Type.ค่าใช้จ่ายตามสัญญาจ้าง, act.ContractAmounts, paa, newpa);
                        //if (act.SaveProceedData != null)
                        //{
                        //    foreach (var save in act.SaveProceedData)
                        //    {
                        // var strPeriod =   save.Year.ToString() + save.Month.ToString().PadLeft(2, '0');
                        // int.Parse(strPeriod);
                        if (act.ActivityPlanDetail.Budget != null) saveNewDebtAmt(lovs.Data, actPrType, actPeriod, ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินงบประมาณ, act.ActivityPlanDetail.Budget, paa, newpa, amountGroup);
                        if (act.ActivityPlanDetail.Revernue != null) saveNewDebtAmt(lovs.Data, actPrType, actPeriod, ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินรายได้, act.ActivityPlanDetail.Revernue, paa, newpa, amountGroup);
                        //if (act.ActivityPlanDetail.SignedLoan != null) saveNewDebtAmt(lovs.Data, actPrType, actPeriod, ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินกู้ลงนาม, act.ActivityPlanDetail.SignedLoan, paa, newpa, amountGroup);
                        //if (act.ActivityPlanDetail.DisburseLoan != null) saveNewDebtAmt(lovs.Data, actPrType, actPeriod, ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินกู้เบิกจ่าย, act.ActivityPlanDetail.DisburseLoan, paa, newpa, amountGroup);
                        if (act.ActivityPlanDetail.Other != null) saveNewDebtAmt(lovs.Data, actPrType, actPeriod, ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินจากแหล่งอื่นๆ, act.ActivityPlanDetail.Other, paa, newpa, amountGroup);
                        //    }
                        //}
                        if (act.LoanTypePlans != null && amountGroup == "P")
                        {
                            var ltps = act.LoanTypePlans;
                            if (pl309 != null)
                            {
                                ltps = ltps.Where(w => w.LoanType.SelectedType != ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้มาเพื่อให้กู้ต่อ_ของรัฐบาล_).ToList();
                            };
                            foreach (var ltp in ltps)
                            {
                                var ltpAmt = ltp.LoanSourcePlans.Select(s => new AmountData
                                {
                                    Amount = s.LoanAmount,
                                    CurrencyCode = s.Currency,
                                    SourceType = s.SourceType

                                }).ToList();
                                saveNewDebtAmt(lovs.Data, "Y", period, ltp.LoanType.SelectedType, ltpAmt, paa, newpa, amountGroup);
                            }
                        }

                        if (act.LoanProcessPlan != null && amountGroup == "P")
                        {
                            var lppAmts = new List<SaveProceed>();

                            foreach (var lpp in act.LoanProcessPlan)
                            {
                                foreach (var lppd in lpp.LoanProcessPlanDetails)
                                {
                                    var save = lppAmts.Where(w => w.Month == lppd.Month && w.Year == plan.StartYear).FirstOrDefault();
                                    if (save == null)
                                    {
                                        save = new SaveProceed
                                        {
                                            Year = plan.StartYear,
                                            Month = lppd.Month,
                                            SignedLoan = new List<AmountData>(),
                                            DisburseLoan = new List<AmountData>()
                                        };
                                        lppAmts.Add(save);
                                    };
                                    storeAmountData(save.SignedLoan, lppd.SignedLoan, lpp.LoanSource.Currency, lpp.LoanSource.SourceType);
                                    storeAmountData(save.DisburseLoan, lppd.DisburseLoan, lpp.LoanSource.Currency, lpp.LoanSource.SourceType);

                                }

                            }

                            foreach (var lppamt in lppAmts)
                            {
                                var strPeriod = lppamt.Year.ToString() + lppamt.Month.ToString().PadLeft(2, '0');
                                var pr = int.Parse(strPeriod);

                                if (lppamt.SignedLoan != null) saveNewDebtAmt(lovs.Data, "M", pr, ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินกู้ลงนาม, lppamt.SignedLoan, paa, newpa, amountGroup);
                                if (lppamt.DisburseLoan != null) saveNewDebtAmt(lovs.Data, "M", pr, ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินกู้เบิกจ่าย, lppamt.DisburseLoan, paa, newpa, amountGroup);

                            }
                        }

                    }
       
                }


                #endregion
                var newLog = new DataLog { LogDt = DateTime.Now, LogType = "U", TableName = "Plan", UserId = userID, TableKey = plan.PlanId };
                DB.DataLog.Add(newLog);
                plan.DataLog = newLog.LogId;

                await DB.SaveChangesAsync();
                ret.IsCompleted = true;
                ret.Data = plan.PlanId;
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
        private void storeAmountData(List<AmountData> amts, decimal amount, string currency, string sourcetype)
        {
            var find = amts.Where(w => w.SourceType == sourcetype && w.CurrencyCode == currency).FirstOrDefault();
            if (find == null)
            {
                find = new AmountData
                {
                    CurrencyCode = currency,
                    SourceType = sourcetype,

                };
                amts.Add(find);
            }
            find.Amount = amount;
        }
        private void newDebtSummary(NewDebtPlanSummary ndps, string projectType, string sourceType, string selectedType, decimal amount,bool isSummary)
        {
            var source = ndps.OverAllByLoanSource.Where(w => w.SourceType == sourceType).FirstOrDefault();
            if (source == null)
            {
                source = new LoanSource
                {
                    SourceType = sourceType,
                    Currency = "THB"
                };
                ndps.OverAllByLoanSource.Add(source);
            }
            source.THBAmount += amount;
            source.LoanAmount += amount;
            var sall = ndps.OverAllByLoanSource.Where(w => w.SourceType == "A").FirstOrDefault();
            if (sall == null)
            {
                sall = new LoanSource
                {
                    SourceType = "A",
                    Currency = "THB"
                };
                ndps.OverAllByLoanSource.Add(sall);
            }
            sall.THBAmount += amount;
            sall.LoanAmount += amount;
            var selected = ndps.OverAllByLoanType.Where(w => w.SelectedType == selectedType).FirstOrDefault();
            if (selected == null)
            {
                selected = new LoanType
                {
                    SelectedType = selectedType,
                    Currency = "THB"
                };
                ndps.OverAllByLoanType.Add(selected);
            }
            selected.LoanAmount += amount;
            if (selectedType == ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้ต่อจากกระทรวงการคลัง_ของรัฐวิสาหกิจ_ && isSummary)
            {
                selected = ndps.OverAllByLoanType.Where(w => w.SelectedType == ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้มาเพื่อให้กู้ต่อ_ของรัฐบาล_).FirstOrDefault();
                if (selected == null)
                {
                    selected = new LoanType
                    {
                        SelectedType = ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้มาเพื่อให้กู้ต่อ_ของรัฐบาล_,
                        Currency = "THB"
                    };
                    ndps.OverAllByLoanType.Add(selected);
                }
                selected.LoanAmount += amount;
            }
            ///// project type
            var pj = ndps.OverAllByProjectType.Where(w => w.ProjectType == projectType).FirstOrDefault();
            if (pj == null)
            {
                pj = new ProjectTypeLoanSummary
                {
                    ProjectType = projectType,
                    LoanTypeSumAmount = new List<LoanType>()
                };
                ndps.OverAllByProjectType.Add(pj);
            }
            pj.ProjectTypeSumAmount += amount;
            var pjselect = pj.LoanTypeSumAmount.Where(w => w.SelectedType == selectedType).FirstOrDefault();
            if (pjselect == null)
            {
                pjselect = new LoanType
                {
                    SelectedType = selectedType,
                    Currency = "THB"
                };
                pj.LoanTypeSumAmount.Add(pjselect);
            }
            pjselect.LoanAmount += amount;

        }
        public async Task<ReturnObject<NewDebtPlanModel>> GetNewDebtPlanOld(long? ID)
        {
            var ret = new ReturnObject<NewDebtPlanModel>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var data = new NewDebtPlanModel
                {
                    PlanSummary = new NewDebtPlanSummary
                    {
                        OverAllByLoanSource = new List<LoanSource>(),
                        OverAllByLoanType = new List<LoanType>(),
                        OverAllByProjectType = new List<ProjectTypeLoanSummary>()
                    },
                    PlanDetails = new List<NewDebtPlanDetails>()
                };
                //var lovAmt = _helper.GetLOVByGroup(ServiceModels.Constants.LOVGroup.Project_Amount_Type._LOVGroupCode);

                //var loanTypeCodes = new string[]
                //{
                //     ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้ตรง,
                //     ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้มาเพื่อให้กู้ต่อ,
                //     ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้โดยขอค้ำจากกระทรวงการคลัง
                //};
                if (ID.HasValue)
                {


                    var head = await GetPlanHeaderByID(ServiceModels.Constants.LOVGroup.Plan_Type.แผนก่อหนี้ใหม่, ID.Value);
                    if (!head.IsCompleted)
                    {
                        ret.CloneMessage(head.Message);
                        return ret;
                    }
                    data.PlanHeader = head.Data;

                }

                var pl = await DB.Plan.Include(pj => pj.PlanProject).Where(w => w.PlanId == ID).FirstOrDefaultAsync();
                if (pl == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { "แผน" });
                    return ret;
                }
                var rate = _helper.GetCurrencyRate(pl.StartYear);
                if (!rate.IsCompleted)
                {
                    ret.CloneMessage(rate.Message);
                    return ret;
                }
                var plsum = data.PlanSummary;
                plsum.CurrencyData = rate.Data;


                #region PlanDetail
                foreach (var pj in pl.PlanProject)
                {
                    var plandt = new NewDebtPlanDetails();
                    plandt.PlanID = pj.PlanProjectId;
                    plandt.ProjectID = pj.ProjectId;
                    var proj = await DB.Project.Include(t => t.ProjectTypeNavigation).Where(w => w.ProjectId == pj.ProjectId).FirstOrDefaultAsync();
                    if (proj != null)
                    {
                        plandt.ProjectCode = proj.ProjectCode;
                        plandt.ProjectTHName = proj.ProjectThname;
                        plandt.IsNewProject = proj.IsNewProject;
                        plandt.PdmoAgreement = proj.Pdmoagreement;
                        plandt.ActivityPlanSummary = new ActivityPlanSummary
                        {
                            LoanSourcePlans = new List<LoanSourcePlan>()
                        };
                        plandt.YearPlanSummary = new LoanPeriod
                        {
                            LoanSources = new List<LoanSource>()
                        };
                        plandt.YearPlanSummary.PeriodType = "Y";
                        plandt.YearPlanSummary.PeriodValue = pl.StartYear;
                        //plandt.ProjectDetail = new ProjectModel
                        //{
                        //    ProjectID = proj.ProjectId,
                        //    ProjectCode = proj.ProjectCode,
                        //    ProjectTHName = proj.ProjectThname
                        //};
                    }
                    #region from Loan5Y
                    plandt.FromFiveYearPlan = new LoanPeriod
                    {
                        PeriodType = "Y",
                        PeriodValue = pl.StartYear,
                        LoanSources = new List<LoanSource>()
                    };

                    var l5ys = await DB.PlanProject.Where(w => w.ProjectId == pj.ProjectId &&
                    w.Plan.StartYear == pl.StartYear && w.Plan.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.แผน_5_ปี)
                    .Select(s => s.PlanLoan).FirstOrDefaultAsync();
                    if (l5ys != null)
                    {
                        var l5 = l5ys.Where(w => w.PeriodType == "Y" && w.PeriodValue == pl.StartYear);
                        foreach (var l in l5)
                        {
                            var r = rate.Data.Currency.Where(w => w.CurrencyCode == l.LoanCurrency).FirstOrDefault();
                            if (r == null)
                            {
                                ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { $"สกุลเงิน {l.LoanCurrency} ของปี {pl.StartYear}" });
                                return ret;
                            }
                            plandt.FromFiveYearPlan.LoanSources.Add(new LoanSource
                            {
                                Currency = l.LoanCurrency,
                                LoanAmount = l.LoanAmount,
                                LoanSourceID = l.PlanLoanId,
                                SourceType = l.LoanType,
                                THBAmount = l.LoanAmount * r.CurrencyRate
                            });
                        }
                    }

                    #endregion
                    #region Activities
                    var placts = await DB.PlanAct.Where(w => w.PlanProjId == pj.PlanProjectId).ToListAsync();
                    plandt.ActivityPlans = new List<ActivityPlan>();
                   
                    foreach (var plact in placts)
                    {

                        var actplan = new ActivityPlan
                        {
                            Activity = new ActivityData
                            {
                                ProjActID = plact.PlanActId,
                                ActivityName = plact.ActivityName,
                                ContractAmounts = new List<AmountData>(),
                                ResolutionAmounts = new List<AmountData>(),
                                SaveProceedData = new List<SaveProceed>(),
                                Years = new List<ProceedByYear>(),
                                TotalProceedByActivity = new ProceedData(),
                            },
                            ActivityPlanDetail = new SaveProceed(),
                            LoanProcessPlan = new List<LoanProcessPlan>(),
                            LoanTypePlans = new List<LoanTypePlan>()
                        };
                        
                       // var period = new PeriodObject(paAmt.Amount.PeriodValue);
                       string year = pl.StartYear.ToString();
                        List<AmountData> amt = new List<AmountData>();
                        var totYear = actplan.Activity.Years.Where(w => w.Year.Detail == year).FirstOrDefault();
                        if (totYear == null)
                        {
                            totYear = new ProceedByYear
                            {
                                Year = new ProceedData { Detail = year },
                                Months = new List<ProceedData>()
                            };

                            actplan.Activity.Years.Add(totYear);
                        }
                        //var totMonth = totYear.Months.Where(w => w.Detail == period.StringMonth).FirstOrDefault();
                        //if (totMonth == null)
                        //{
                        //    totMonth = new ProceedData
                        //    {
                        //        Detail = period.StringMonth

                        //    };
                        //    totYear.Months.Add(totMonth);
                        //}
                        //var proceed = actplan.Activity.SaveProceedData.Where(w => w.Month == period.Month && w.Year == year).FirstOrDefault();
                        // var proceed = actplan.Activity.SaveProceedData.Where(w => w.Year == pl.StartYear).FirstOrDefault();

                        //if (proceed == null)
                        //{
                        //    proceed = new SaveProceed
                        //    {
                        //        Revernue = new List<AmountData>(),
                        //        Budget = new List<AmountData>(),
                        //        SignedLoan = new List<AmountData>(),
                        //        DisburseLoan = new List<AmountData>(),
                        //        Other = new List<AmountData>()
                        //    };
                        //    proceed.Year = pl.StartYear;

                        //    actplan.Activity.SaveProceedData.Add(proceed);
                        //}
                        var proceed = new SaveProceed
                        {
                            Revernue = new List<AmountData>(),
                            Budget = new List<AmountData>(),
                            SignedLoan = new List<AmountData>(),
                            DisburseLoan = new List<AmountData>(),
                            Other = new List<AmountData>()
                        };
                        proceed.Year = pl.StartYear;

                        var paAmts = await DB.PlanActAmount.Include(i => i.Amount).ThenInclude(th => th.AmountTypeNavigation).Where(w => w.PlanActId == plact.PlanActId).ToListAsync();
                        foreach (var paAmt in paAmts)
                        {
                            var r = rate.Data.Currency.Where(w => w.CurrencyCode == paAmt.Amount.Currency).FirstOrDefault();
                            if (r == null)
                            {
                                ret.AddMessage(eMessage.DataIsNotFound.ToString(), "currency", eMessageType.Error, new string[] { $"อัตราแลกเปลี่ยน {paAmt.Amount.Currency} ปี {pl.StartYear}" });
                                return ret;
                            }
                            plandt.ActivityPlanSummary.Total += paAmt.Amount.Amount1 * r.CurrencyRate;
                            #region LoanTypePlans
                            if (loanTypeCodes.Contains(paAmt.Amount.AmountTypeNavigation.Lovcode))
                            {

                                // var lovcode = Utilities.GetLovCodeFromKey(lovAmt.Data, paAmt.Amount.AmountType);
                                var ly = actplan.LoanTypePlans.Where(w => w.LoanType.SelectedType == paAmt.Amount.AmountTypeNavigation.Lovcode).FirstOrDefault();
                                if (ly == null)
                                {
                                    ly = new LoanTypePlan
                                    {
                                        LoanType = new LoanType
                                        {
                                            SelectedType = paAmt.Amount.AmountTypeNavigation.Lovcode
                                        },
                                        LoanSourcePlans = new List<LoanSource>()
                                    };
                                    actplan.LoanTypePlans.Add(ly);
                                }
                                var ls = new LoanSource
                                {
                                    Currency = paAmt.Amount.Currency,
                                    LoanAmount = paAmt.Amount.Amount1,
                                    LoanSourceID = paAmt.PlanActAmountId,
                                    SourceType = paAmt.Amount.SourceType,
                                    THBAmount = paAmt.Amount.Amount1 * r.CurrencyRate
                                };
                                ly.LoanSourcePlans.Add(ls);
                                newDebtSummary(plsum, proj.ProjectTypeNavigation.Lovcode, paAmt.Amount.SourceType, paAmt.Amount.AmountTypeNavigation.Lovcode, ls.THBAmount,false);
                                plandt.ActivityPlanSummary.Loan += ls.THBAmount;
                                var actsource = plandt.ActivityPlanSummary.LoanSourcePlans.Where(w => w.LoanSource.SourceType == paAmt.Amount.SourceType).FirstOrDefault();
                                if (actsource == null)
                                {
                                    actsource = new LoanSourcePlan
                                    {
                                        LoanSource = new LoanSource
                                        {
                                            SourceType = paAmt.Amount.SourceType,
                                            Currency = "THB",
                                        },
                                        LoanTypePlans = new List<LoanType>()
                                    };
                                    plandt.ActivityPlanSummary.LoanSourcePlans.Add(actsource);
                                }
                                actsource.LoanSource.LoanAmount += ls.THBAmount;
                                actsource.LoanSource.THBAmount = actsource.LoanSource.LoanAmount;
                                var acttype = actsource.LoanTypePlans.Where(w => w.SelectedType == paAmt.Amount.AmountTypeNavigation.Lovcode).FirstOrDefault();
                                if (acttype == null)
                                {
                                    acttype = new LoanType
                                    {
                                        SelectedType = paAmt.Amount.AmountTypeNavigation.Lovcode,
                                        Currency = "THB"
                                    };
                                    actsource.LoanTypePlans.Add(acttype);
                                }
                                acttype.LoanAmount += ls.THBAmount;
                                var sl = proceed.SignedLoan.Where(w => w.SourceType == paAmt.Amount.SourceType && w.CurrencyCode == paAmt.Amount.Currency).FirstOrDefault();
                                if (sl == null)
                                {
                                    sl = new AmountData
                                    {
                                        CurrencyCode = paAmt.Amount.Currency,
                                        SourceType = paAmt.Amount.SourceType,

                                    };
                                    proceed.SignedLoan.Add(sl);
                                }
                                sl.Amount += paAmt.Amount.Amount1;
                                sl.THBAmount += (paAmt.Amount.Amount1 * r.CurrencyRate);

                                var ysum = plandt.YearPlanSummary.LoanSources.Where(w => w.SourceType == paAmt.Amount.SourceType && w.Currency == paAmt.Amount.Currency).FirstOrDefault();
                                if (ysum == null)
                                {
                                    ysum = new LoanSource
                                    {
                                        Currency = paAmt.Amount.Currency,
                                        SourceType = paAmt.Amount.SourceType
                                    };
                                    plandt.YearPlanSummary.LoanSources.Add(ysum);
                                }
                                ysum.LoanAmount += paAmt.Amount.Amount1;
                            }
                            #endregion

                            else
                            {
                                #region SaveProceed
                                if (paAmt.Amount.PeriodType == "Y")
                                {
                                    var newamt = new AmountData
                                    {
                                        Amount = paAmt.Amount.Amount1,
                                        CurrencyCode = paAmt.Amount.Currency,
                                        SourceType = paAmt.Amount.SourceType,
                                        THBAmount = paAmt.Amount.Amount1 * r.CurrencyRate
                                    };
                                    if (paAmt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.ค่าใช้จ่ายตามมติ)
                                    {
                                        actplan.Activity.ResolutionAmounts.Add(newamt);
                                        actplan.Activity.TotalResolution += newamt.THBAmount;
                                        //storePlanLoanSummary(actplan.ResolutionExpSum, newamt.SourceType, newamt.THBAmount);
                                    }
                                    else if (paAmt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.ค่าใช้จ่ายตามสัญญาจ้าง)
                                    {
                                        actplan.Activity.ContractAmounts.Add(newamt);
                                        actplan.Activity.TotalContract += newamt.THBAmount;
                                        //storePlanLoanSummary(data.actplan.Activity, newamt.SourceType, newamt.THBAmount);
                                    }
                                    else
                                    {


                                        actplan.Activity.TotalProceedByActivity.Total += newamt.THBAmount;
                                        totYear.Year.Total += newamt.THBAmount;
                                        //totMonth.Total += newamt.THBAmount;
                                        if (paAmt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินรายได้)
                                        {
                                            amt = proceed.Revernue;
                                            actplan.Activity.TotalProceedByActivity.Revernue += newamt.THBAmount;
                                            totYear.Year.Revernue += newamt.THBAmount;
                                            plandt.ActivityPlanSummary.Revernue += newamt.THBAmount;
                                            //totMonth.Revernue += newamt.THBAmount;
                                        }
                                        if (paAmt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินงบประมาณ)
                                        {
                                            amt = proceed.Budget;
                                            actplan.Activity.TotalProceedByActivity.Budget += newamt.THBAmount;
                                            totYear.Year.Budget += newamt.THBAmount;
                                            plandt.ActivityPlanSummary.Budget += newamt.THBAmount;
                                            //totMonth.Budget += newamt.THBAmount;
                                        }
                                        //if (paAmt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินกู้ลงนาม)
                                        //{
                                        //    amt = proceed.SignedLoan;
                                        //    actplan.Activity.TotalProceedByActivity.SignedLoan += newamt.THBAmount;
                                        //    totYear.Year.SignedLoan += newamt.THBAmount;
                                        //    //totMonth.SignedLoan += newamt.THBAmount;
                                        //}
                                        if (paAmt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินกู้เบิกจ่าย)
                                        {
                                            amt = proceed.DisburseLoan;
                                            actplan.Activity.TotalProceedByActivity.DisburseLoan += newamt.THBAmount;
                                            totYear.Year.DisburseLoan += newamt.THBAmount;
                                            //totMonth.DisburseLoan += newamt.THBAmount;
                                        }
                                        if (paAmt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินจากแหล่งอื่นๆ)
                                        {
                                            amt = proceed.Other;
                                            actplan.Activity.TotalProceedByActivity.Other += newamt.THBAmount;
                                            totYear.Year.Other += newamt.THBAmount;
                                            plandt.ActivityPlanSummary.Other += newamt.THBAmount;
                                            //totMonth.Other += newamt.THBAmount;
                                        }
                                        amt.Add(newamt);

                                    }
                                }
                                #endregion
                                #region LoanProcessPlan
                                if (paAmt.Amount.PeriodType == "M")
                                {
                                    var period = new PeriodObject(paAmt.Amount.PeriodValue);
                                    var lpp = actplan.LoanProcessPlan.Where(w => w.LoanSource.SourceType == paAmt.Amount.SourceType && w.LoanSource.Currency == paAmt.Amount.Currency).FirstOrDefault();
                                    if (lpp == null)
                                    {
                                        lpp = new LoanProcessPlan
                                        {
                                            LoanSource = new LoanSource
                                            {
                                                Currency = paAmt.Amount.Currency,
                                                LoanSourceID = paAmt.Amount.AmountId,
                                                SourceType = paAmt.Amount.SourceType,

                                            },
                                            LoanProcessPlanDetails = new List<LoanProcessPlanDetail>()
                                        };


                                        actplan.LoanProcessPlan.Add(lpp);
                                    }
                                    lpp.LoanSource.LoanAmount += paAmt.Amount.Amount1;
                                    lpp.LoanSource.THBAmount += paAmt.Amount.Amount1 * r.CurrencyRate;


                                    var lppd = lpp.LoanProcessPlanDetails.Where(w => w.Month == period.Month).FirstOrDefault();
                                    if (lppd == null)
                                    {
                                        lppd = new LoanProcessPlanDetail
                                        {
                                            Month = period.Month
                                        };
                                        lpp.LoanProcessPlanDetails.Add(lppd);
                                    }
                                    if (paAmt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินกู้ลงนาม)
                                    {
                                        lppd.SignedLoan = paAmt.Amount.Amount1;
                                        lpp.SignedLoanSumAmount += paAmt.Amount.Amount1;
                                    }
                                    if (paAmt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินกู้เบิกจ่าย)
                                    {
                                        lppd.DisburseLoan = paAmt.Amount.Amount1;
                                        lpp.DisburseLoanSumAmount += paAmt.Amount.Amount1;
                                    }

                                }
                                #endregion

                            }

                        }
                       
                        plandt.ActivityPlans.Add(actplan);
                    }

                    #endregion
                    plandt.YearPlanSummary.LoanSources = plandt.YearPlanSummary.LoanSources.OrderByDescending(o => o.SourceType).ToList();
                    data.PlanDetails.Add(plandt);
                }
                
                #endregion
                ret.Data = data;
                ret.IsCompleted = true;
            }

            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        private void SortNewDebt(NewDebtPlanSummary model)
        {
            #region OverAllByLoanType
            var old = new List<LoanType>(model.OverAllByLoanType);
            model.OverAllByLoanType = new List<LoanType>();
            var LTorder = DB.CeLov.Where(w => w.LovgroupCode == ServiceModels.Constants.LOVGroup.Project_Amount_Type._LOVGroupCode)
                .OrderBy(o => o.OrderNo).Select(s => s.Lovcode).ToList();
            //new string[] { ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้ตรง__รัฐบาล_ ,
            //ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้ต่อจากกระทรวงการคลัง_ของรัฐวิสาหกิจ_,
            //ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้มาเพื่อให้กู้ต่อ_ของรัฐบาล_ ,
            //ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้โดยขอค้ำจากกระทรวงการคลัง ,
            //ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้โดยไม่ขอค้ำจากกระทรวงการคลัง,
            //};
            foreach (var lt in LTorder)
            {
                var newlt = old.Where(w => w.SelectedType == lt).ToList();
                if (newlt.Count > 0)
                {
                    model.OverAllByLoanType.AddRange(newlt);
                }
            }

            #endregion
            #region OverAllByProjectType
            var oldPJ = new List<ProjectTypeLoanSummary>(model.OverAllByProjectType);
            model.OverAllByProjectType = new List<ProjectTypeLoanSummary>();
            var PJorder = DB.CeLov.Where(w => w.LovgroupCode == ServiceModels.Constants.LOVGroup.Project_Type._LOVGroupCode)
                .OrderBy(o => o.OrderNo).Select(s => s.Lovcode).ToList();
            foreach (var pj in PJorder)
            {
                var newpj = oldPJ.Where(w => w.ProjectType == pj).FirstOrDefault();
                if (newpj != null)
                {
                    model.OverAllByProjectType.Add(newpj);
                    var oldlt = new List<LoanType>(newpj.LoanTypeSumAmount);
                    newpj.LoanTypeSumAmount = new List<LoanType>();
                    foreach (var lt in LTorder)
                    {
                        var newlt = oldlt.Where(w => w.SelectedType == lt).ToList();
                        if (newlt.Count > 0)
                        {
                            newpj.LoanTypeSumAmount.AddRange(newlt);
                        }
                    }
                }
            }
            #endregion
        }
        private bool InitialLoanType(List<PlanActAmount> paa, string amtType, int year)
        {
            bool ret = false;
            var amtCount = paa.Where(w => w.Amount.AmountTypeNavigation.Lovcode == amtType).Count();

            if (amtCount == 0)
            {
                paa.Add(new PlanActAmount {
                    Amount = new Amount
                    {
                        Amount1 = 0,
                        SourceType = "L",
                        PeriodType = "Y",
                        PeriodValue = year,
                        AmountTypeNavigation = new CeLov { Lovcode = amtType },
                        Currency = "THB",

                    }

                });
                paa.Add(new PlanActAmount
                {
                    Amount = new Amount
                    {
                        Amount1 = 0,
                        SourceType = "F",
                        PeriodType = "Y",
                        PeriodValue = year,
                        AmountTypeNavigation = new CeLov { Lovcode = amtType },
                        Currency = "USD",

                    }

                });
                ret = true;
            }

            return ret;

        }
        public async Task<ReturnObject<NewDebtPlanModel>> GetNewDebtPlan(long? ID, string amountGroup, int? month)
        {
            var ret = new ReturnObject<NewDebtPlanModel>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var loanGov = new string[] {
                            ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้ตรง__รัฐบาล_,
                            ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้มาเพื่อให้กู้ต่อ_ของรัฐบาล_
                         };
                var loanSEN = new string[] {
                            ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้โดยขอค้ำจากกระทรวงการคลัง,
                            ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้โดยไม่ขอค้ำจากกระทรวงการคลัง,
                            ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้ต่อจากกระทรวงการคลัง_ของรัฐวิสาหกิจ_
                            };

                var data = new NewDebtPlanModel {
                    PlanSummary = new NewDebtPlanSummary {
                        OverAllByLoanSource = new List<LoanSource>(),
                        OverAllByLoanType = new List<LoanType>(),
                        OverAllByProjectType = new List<ProjectTypeLoanSummary>()
                    },
                    PlanDetails = new List<NewDebtPlanDetails>()
                };
                //var lovAmt = _helper.GetLOVByGroup(ServiceModels.Constants.LOVGroup.Project_Amount_Type._LOVGroupCode);

                //var loanTypeCodes = new string[]
                //{
                //     ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้ตรง,
                //     ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้มาเพื่อให้กู้ต่อ,
                //     ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้โดยขอค้ำจากกระทรวงการคลัง
                //};
                if (ID.HasValue)
                {


                    var head = await GetPlanHeaderByID(ServiceModels.Constants.LOVGroup.Plan_Type.แผนก่อหนี้ใหม่, ID.Value);
                    if (!head.IsCompleted)
                    {
                        ret.CloneMessage(head.Message);
                        return ret;
                    }
                    data.PlanHeader = head.Data;

                }
                #region for org 309 only
                var pl309 = await DB.Plan
                    .Include(ppj => ppj.PlanProject).ThenInclude(pj => pj.Project).ThenInclude(o => o.Organization).ThenInclude(ot => ot.OrgtypeNavigation)
                    .Include(ppj => ppj.PlanProject).ThenInclude(ppa => ppa.PlanAct).ThenInclude(paa => paa.PlanActAmount)
                    .ThenInclude(amt => amt.Amount).ThenInclude(t => t.AmountTypeNavigation)
                    .Where(w => w.PlanId == ID.Value && w.Organization.OrganizationCode == "309").FirstOrDefaultAsync();
                var allData = new Dictionary<string, List<PlanProject>>();
                
                #region Add GOV proj to 309
                if (pl309 != null)
                {
                    var govPPJ = new List<PlanProject>();
                    var govs = await DB.PlanProject
                    .Include(og => og.Plan.Organization.OrgtypeNavigation)
                    .Include(pj => pj.ProjectTypeNavigation)
                    .Include(pj => pj.PlanAct).ThenInclude(amt => amt.PlanActAmount).ThenInclude(a => a.Amount).ThenInclude(t => t.AmountTypeNavigation)
                    .Where(w => w.Plan.StartYear == pl309.StartYear
                    && w.Plan.Organization.OrganizationCode != "309"
                    && w.Plan.Organization.OrgtypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.ประเภทหน่วยงาน.ส่วนราชการ__กระทรวง_ทบวง_กรม__
                    && w.Plan.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.แผนก่อหนี้ใหม่
                    && w.Plan.PlanRelease == pl309.PlanRelease).ToListAsync();
                    foreach (var gov in govs)
                    {
                        
                        var delPA = new List<PlanAct>();
                        foreach (var govact in gov.PlanAct)
                        {
                            var tot = govact.PlanActAmount.Where(w => w.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้ตรง__รัฐบาล_).Sum(s => s.Amount.Amount1);
                            if (tot <= 0)
                            {
                                delPA.Add(govact);
                            }

                        }
                        foreach (var del in delPA)
                        {
                            gov.PlanAct.Remove(del);
                        }

                        if (gov.PlanAct.Count > 0)
                        {
                            govPPJ.Add(gov);
                        }
                    }
                    if (govPPJ.Count > 0)
                    {
                        allData.Add(ePlanProjType.Goverment.ToString(), govPPJ);
                    }
                }
                #endregion
                if (pl309 != null && amountGroup == "P")
                {
                    #region delete reference planact that planrelease != this plan 
                    var allact = await DB.PlanAct.Where(w => w.PlanProj.Plan.PlanId == pl309.PlanId).ToListAsync();
                    var actErr = await DB.PlanAct
                        .Include(r => r.ReferencePlanAct).ThenInclude(pp => pp.PlanProj).ThenInclude(p => p.Plan)
                        .Where(w => w.PlanProj.Plan.PlanId == pl309.PlanId && w.ReferencePlanActId.HasValue && w.ReferencePlanAct.PlanProj.Plan.PlanRelease != pl309.PlanRelease).ToListAsync();
                    if (actErr.Count > 0)
                    {
                        foreach (var err in actErr)
                        {
                            DB.PlanAct.Remove(err);
                        }
                        await DB.SaveChangesAsync();
                    }


                    #endregion
                    var source = await DB.PlanAct
                   .Include(ppj => ppj.PlanProj)
                   //.Include(paa => paa.PlanActAmount).ThenInclude(amt => amt.Amount).ThenInclude(l => l.AmountTypeNavigation)
                    .Where(w => w.PlanActAmount.Where(a => a.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้ต่อจากกระทรวงการคลัง_ของรัฐวิสาหกิจ_).Sum(s => s.Amount.Amount1) > 0                    
                    && w.PlanProj.Plan.StartYear == pl309.StartYear
                    && w.PlanProj.Plan.Organization.OrganizationCode != "309"
                    && w.PlanProj.Plan.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.แผนก่อหนี้ใหม่
                    && w.PlanProj.Plan.PlanRelease == pl309.PlanRelease
                    ).ToListAsync();
              
                    foreach (var refer in source)
                    {
                        var ppj309 = pl309.PlanProject.Where(w => w.ProjectId == refer.PlanProj.ProjectId && w.PlanId == pl309.PlanId).FirstOrDefault();
                        if (ppj309 == null)
                        {
                            ppj309 = new PlanProject
                            {
                                PlanId = pl309.PlanId,
                                ProjectId = refer.PlanProj.ProjectId,
                                IsNotRequiredApproval = refer.PlanProj.IsNotRequiredApproval,
                                ProjectType = refer.PlanProj.ProjectType
                            };
                            pl309.PlanProject.Add(ppj309);

                        }

                        var ppa309 = ppj309.PlanAct.Where(w => w.ReferencePlanActId == refer.PlanActId).FirstOrDefault();
                        if (ppa309 == null)
                        {
                            ppa309 = new PlanAct
                            {
                                ActivityName = refer.ActivityName,
                                ReferencePlanActId = refer.PlanActId,
                            };
                            ppj309.PlanAct.Add(ppa309);
                        }
                    }
                   
                    await DB.SaveChangesAsync();
                    #region delete กู้มาให้กู้ต่อที่เป็น 0
                    var delPpj = pl309.PlanProject
                        .Where(w => w.Project.Organization.OrgtypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.ประเภทหน่วยงาน.รัฐวิสาหกิจ).ToList();

                    foreach (var ppj309 in delPpj)
                    {
                        var delpa = ppj309.PlanAct.ToList();
                        foreach (var pa309 in delpa)
                        {
                            var sumDirect = pa309.PlanActAmount.Sum(s => s.Amount.Amount1);
                               // .Where(a => a.Amount.AmountTypeNavigation.LovgroupCode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้ตรง).Sum(s => s.Amount.Amount1);
                            if (sumDirect == 0)
                            {
                                var rf = await DB.PlanAct
                                    .Include(paa => paa.PlanActAmount)
                                    .ThenInclude(amt => amt.Amount).ThenInclude(t => t.AmountTypeNavigation)
                                    .Where(w => w.PlanActId == pa309.ReferencePlanActId).FirstOrDefaultAsync();
                                if (rf == null )
                                {
                                    ppj309.PlanAct.Remove(pa309);
                                }else
                                {
                                    var sumFL = rf.PlanActAmount
                                    .Where(a => a.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้ต่อจากกระทรวงการคลัง_ของรัฐวิสาหกิจ_).Sum(s => s.Amount.Amount1);
                                    if (sumFL == 0)
                                    {
                                        ppj309.PlanAct.Remove(pa309);
                                    }
                                }
                            }
                        }
                        if (ppj309.PlanAct.Count() == 0)
                        {
                            pl309.PlanProject.Remove(ppj309);
                        }
                    }
                    await DB.SaveChangesAsync();
                    #endregion  

                }

                #endregion
                var pl = await DB.Plan
                    .Include(og => og.Organization.OrgtypeNavigation)
                    .Include(pj => pj.PlanProject).ThenInclude(obj => obj.ProjectTypeNavigation)
                     .Include(pj => pj.PlanProject).ThenInclude(act => act.PlanAct).ThenInclude(amt => amt.PlanActAmount).ThenInclude(a => a.Amount).ThenInclude(t => t.AmountTypeNavigation)
                    .Where(w => w.PlanId == ID).FirstOrDefaultAsync();
                if (pl == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { "แผน" });
                    return ret;
                }
                var rate = _helper.GetCurrencyRate(pl.StartYear);
                if (!rate.IsCompleted)
                {
                    ret.CloneMessage(rate.Message);
                    return ret;
                }
                var plsum = data.PlanSummary;
                plsum.CurrencyData = rate.Data;
                int? mthRep = null;
                if (amountGroup == "R" && month.HasValue)
                {
                    mthRep = int.Parse(pl.StartYear.ToString().PadLeft(4, '0') + month.Value.ToString().PadLeft(2, '0'));
                }

                #region PlanDetail
                allData.Add(ePlanProjType.Normal.ToString(), pl.PlanProject.ToList());
                foreach (var dic in allData)
                {
                    // foreach (var pj in pl.PlanProject)
                     
                    foreach (var pj in dic.Value)
                    {
                        var plandt = new NewDebtPlanDetails();
                        //plandt.PlanID = pj.PlanId;
                        // kenghot
                        plandt.PlanProjType = dic.Key;
                        plandt.PlanID = pj.PlanProjectId;
                        plandt.ProjectID = pj.ProjectId;
                        plandt.isNotRequiredApproval = pj.IsNotRequiredApproval;
                        plandt.ProjectType = pj.ProjectTypeNavigation.Lovcode;
                        plandt.ProjectTypeLabel = pj.ProjectTypeNavigation.Lovvalue;
                        plandt.LoanTypeLabel = "";
                        var proj = await DB.Project.Include(t => t.ProjectTypeNavigation).Include(o => o.Organization).Where(w => w.ProjectId == pj.ProjectId).FirstOrDefaultAsync();
                        if (proj != null)
                        {
                            plandt.ProjectCode = proj.ProjectCode;
                            plandt.ProjectTHName = proj.ProjectThname;
                            plandt.IsNewProject = proj.IsNewProject;
                            plandt.PdmoAgreement = proj.Pdmoagreement;
                            plandt.Organization = new BasicData
                            {
                                Code = proj.Organization.OrganizationCode,
                                Description = proj.Organization.OrganizationThname,
                                ID = proj.Organization.OrganizationId
                            };
                            plandt.ActivityPlanSummary = new ActivityPlanSummary
                            {
                                LoanSourcePlans = new List<LoanSourcePlan>()
                            };
                            plandt.YearPlanSummary = new LoanPeriod
                            {
                                LoanSources = new List<LoanSource>()
                            };
                            plandt.YearPlanSummary.PeriodType = "Y";
                            plandt.YearPlanSummary.PeriodValue = pl.StartYear;
                            //plandt.ProjectDetail = new ProjectModel
                            //{
                            //    ProjectID = proj.ProjectId,
                            //    ProjectCode = proj.ProjectCode,
                            //    ProjectTHName = proj.ProjectThname
                            //};
                        }
                        var sumact = pj.PlanAct.SelectMany(m => m.PlanActAmount).GroupBy(g => g.Amount.AmountTypeNavigation.Lovcode).Select(s => new { lovcode = s.Key, tot = s.Sum(sum => sum.Amount.Amount1) });
                        sumact = sumact.Where(w => w.tot != 0);
                        //ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้ตรง,
                        // ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้ต่อจากกระทรวงการคลัง_ของรัฐวิสาหกิจ_,
                        // ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้โดยขอค้ำจากกระทรวงการคลัง,
                        // ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้โดยไม่ขอค้ำจากกระทรวงการคลัง,
                        // ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้มาเพื่อให้กู้ต่อ_ของรัฐบาล_
                        foreach (var s in sumact)
                        {
                            if (s.lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้ต่อจากกระทรวงการคลัง_ของรัฐวิสาหกิจ_)
                            {
                                if (pl309 != null)
                                {
                                    plandt.LoanTypeLabel += "กู้มาให้กู้ต่อ | ";
                                }
                                else
                                {
                                    plandt.LoanTypeLabel += "กู้ต่อจาก กระทรวงการคลัง | ";
                                }
                            }
                            if (s.lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้โดยขอค้ำจากกระทรวงการคลัง)
                            {

                                plandt.LoanTypeLabel += "กู้โดยขอค้ำ กระทรวงการคลัง | ";

                            }
                            if (s.lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้โดยไม่ขอค้ำจากกระทรวงการคลัง)
                            {

                                plandt.LoanTypeLabel += "กู้โดยไม่ขอค้ำ กระทรวงการคลัง | ";

                            }
                            if (s.lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้ตรง__รัฐบาล_)
                            {

                                
                                if (pl309 != null && dic.Key == ePlanProjType.Goverment.ToString())
                                {
                                    plandt.LoanTypeLabel += $"กู้ตรง ({pj.Plan.Organization.OrganizationThname}) | ";
                                }else
                                {
                                    plandt.LoanTypeLabel += "กู้ตรง | ";
                                }
                            }
                        }

                        if (dic.Key == ePlanProjType.Goverment.ToString())
                        {

                        }

                        #region from Loan5Y
                        plandt.FromFiveYearPlan = new LoanPeriod
                        {
                            PeriodType = "Y",
                            PeriodValue = pl.StartYear,
                            LoanSources = new List<LoanSource>()
                        };


                        var l5ys = await DB.PlanProject.Where(w => w.ProjectId == pj.ProjectId &&
                        w.Plan.StartYear == pl.StartYear && w.Plan.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.แผน_5_ปี)
                        .OrderByDescending(o => o.Plan.PlanId)
                        .Select(s => s.PlanLoan).FirstOrDefaultAsync();
                        if (l5ys != null)
                        {
                            var l5 = l5ys.Where(w => w.PeriodType == "Y" && w.PeriodValue == pl.StartYear);
                            foreach (var l in l5)
                            {
                                var r = rate.Data.Currency.Where(w => w.CurrencyCode == l.LoanCurrency).FirstOrDefault();
                                if (r == null)
                                {
                                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { $"สกุลเงิน {l.LoanCurrency} ของปี {pl.StartYear}" });
                                    return ret;
                                }
                                plandt.FromFiveYearPlan.LoanSources.Add(new LoanSource
                                {
                                    Currency = l.LoanCurrency,
                                    LoanAmount = l.LoanAmount,
                                    LoanSourceID = l.PlanLoanId,
                                    SourceType = l.LoanType,
                                    THBAmount = l.LoanAmount * r.CurrencyRate
                                });
                            }
                        }

                        #endregion
                        #region Activities
                        var placts = await DB.PlanAct.Where(w => w.PlanProjId == pj.PlanProjectId).ToListAsync();
                        plandt.ActivityPlans = new List<ActivityPlan>();
                        plandt.MasterAgreement = pj.MasterAgreement; // await _com.GetMasterAgreemet(plandt.PlanID);
                        foreach (var plact in placts)
                        {

                            var actplan = new ActivityPlan
                            {
                                Activity = new ActivityData
                                {
                                    ProjActID = plact.PlanActId,
                                    ActivityName = plact.ActivityName,
                                    ContractAmounts = new List<AmountData>(),
                                    ResolutionAmounts = new List<AmountData>(),
                                    SaveProceedData = new List<SaveProceed>(),
                                    Years = new List<ProceedByYear>(),
                                    TotalProceedByActivity = new ProceedData(),
                                },
                                ActivityPlanDetail = new SaveProceed(),
                                LoanProcessPlan = new List<LoanProcessPlan>(),
                                LoanTypePlans = new List<LoanTypePlan>()
                            };
                            // var period = new PeriodObject(paAmt.Amount.PeriodValue);
                            //actplan.MasterAgreement = plact.MasterAgreement;
                            string year = pl.StartYear.ToString();
                            List<AmountData> amt = new List<AmountData>();
                            var totYear = actplan.Activity.Years.Where(w => w.Year.Detail == year).FirstOrDefault();
                            if (totYear == null)
                            {
                                totYear = new ProceedByYear
                                {
                                    Year = new ProceedData { Detail = year },
                                    Months = new List<ProceedData>()
                                };
     
                                actplan.Activity.Years.Add(totYear);
                            }

                            var proceed = new SaveProceed
                            {
                                Revernue = new List<AmountData>(),
                                Budget = new List<AmountData>(),
                                SignedLoan = new List<AmountData>(),
                                DisburseLoan = new List<AmountData>(),
                                Other = new List<AmountData>()
                            };
                            proceed.Year = pl.StartYear;
                            actplan.ActivityPlanDetail = proceed;
                            var amtSql = DB.PlanActAmount.Include(i => i.Amount).ThenInclude(th => th.AmountTypeNavigation)
                               .Where(w => w.PlanActId == plact.PlanActId && w.Amount.AmountGroup == amountGroup);

                            if (mthRep.HasValue)
                            {
                                amtSql = amtSql.Where(w => w.Amount.PeriodValue == mthRep.Value);
                            }
                            //var paAmts = await DB.PlanActAmount.Include(i => i.Amount).ThenInclude(th => th.AmountTypeNavigation)
                            //    .Where(w => w.PlanActId == plact.PlanActId)
                            //    .OrderBy(o => o.Amount.AmountTypeNavigation.Lovcode).OrderBy(o2 => o2.Amount.SourceType)
                            //    .ToListAsync();
                            var paAmts = await amtSql.OrderBy(o => o.Amount.AmountTypeNavigation.Lovcode).OrderBy(o2 => o2.Amount.SourceType)
                                .ToListAsync();
                            #region for org 309
                            if (pl309 != null && plact.ReferencePlanActId.HasValue)
                            {
                                var paRefs = await DB.PlanActAmount
                                .Include(ppa => ppa.PlanAct).ThenInclude(ppj => ppj.PlanProj).ThenInclude(p => p.Plan).ThenInclude(o => o.Organization)
                                .Include(i => i.Amount).ThenInclude(th => th.AmountTypeNavigation)
                                .Where(w => w.PlanActId == plact.ReferencePlanActId.Value && w.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้ต่อจากกระทรวงการคลัง_ของรัฐวิสาหกิจ_)
                                .OrderBy(o => o.Amount.AmountTypeNavigation.Lovcode).OrderBy(o2 => o2.Amount.SourceType)
                                .ToListAsync();

                                foreach (var paRef in paRefs)
                                {
                                    paRef.Amount.AmountTypeNavigation.Lovcode = ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้มาเพื่อให้กู้ต่อ_ของรัฐบาล_;
                                    if (!plandt.LoanTypeLabel.Contains("กู้มาให้กู้ต่อ") && paRef.Amount.Amount1 != 0)
                                    {
                                        plandt.LoanTypeLabel += $"กู้มาให้กู้ต่อ ({paRef.PlanAct.PlanProj.Plan.Organization.OrganizationThname}) | ";

                                    }



                                }
                                paAmts.AddRange(paRefs);
                                var rmCoount = paAmts.RemoveAll(rm => loanSEN.Contains(rm.Amount.AmountTypeNavigation.Lovcode));
                                var added = InitialLoanType(paAmts, ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้ตรง__รัฐบาล_, pl.StartYear);
                            }
                            if (pl309 != null || pl.Organization.OrgtypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.ประเภทหน่วยงาน.ส่วนราชการ__กระทรวง_ทบวง_กรม__)
                            {
                                var added = InitialLoanType(paAmts, ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้ตรง__รัฐบาล_, pl.StartYear);
                            }
                            #endregion
                            #region special thanks
                            if (pl.Organization.OrgtypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.ประเภทหน่วยงาน.รัฐวิสาหกิจ || pl.Organization.OrgtypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.ประเภทหน่วยงาน.องค์การมหาชน
                                || pl.Organization.OrgtypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.ประเภทหน่วยงาน.รัฐวิสาหกิจที่ทำธุรกิจให้กู้ยืมเงิน)
                            {
                                var rmCoount = paAmts.RemoveAll(rm => loanGov.Contains(rm.Amount.AmountTypeNavigation.Lovcode));
                                foreach (var t in loanSEN)
                                {
                                    var added = InitialLoanType(paAmts, t, pl.StartYear);
                                }
                            }

                            #endregion
                            foreach (var paAmt in paAmts)
                            {
                                var r = rate.Data.Currency.Where(w => w.CurrencyCode == paAmt.Amount.Currency).FirstOrDefault();
                                if (r == null)
                                {
                                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "currency", eMessageType.Error, new string[] { $"อัตราแลกเปลี่ยน {paAmt.Amount.Currency} ปี {pl.StartYear}" });
                                    return ret;
                                }
                                plandt.ActivityPlanSummary.Total += paAmt.Amount.Amount1 * r.CurrencyRate;
                                #region LoanTypePlans
                                if (loanTypeCodes.Contains(paAmt.Amount.AmountTypeNavigation.Lovcode))
                                {

                                    // var lovcode = Utilities.GetLovCodeFromKey(lovAmt.Data, paAmt.Amount.AmountType);
                                    var ly = actplan.LoanTypePlans.Where(w => w.LoanType.SelectedType == paAmt.Amount.AmountTypeNavigation.Lovcode).FirstOrDefault();
                                    if (ly == null)
                                    {
                                        ly = new LoanTypePlan
                                        {
                                            LoanType = new LoanType
                                            {
                                                SelectedType = paAmt.Amount.AmountTypeNavigation.Lovcode
                                            },
                                            LoanSourcePlans = new List<LoanSource>()
                                        };
                                        actplan.LoanTypePlans.Add(ly);
                                    }
                                    var ls = new LoanSource
                                    {
                                        Currency = paAmt.Amount.Currency,
                                        LoanAmount = paAmt.Amount.Amount1,
                                        LoanSourceID = paAmt.PlanActAmountId,
                                        SourceType = paAmt.Amount.SourceType,
                                        THBAmount = paAmt.Amount.Amount1 * r.CurrencyRate
                                    };
                                    ly.LoanSourcePlans.Add(ls);
                                    newDebtSummary(plsum, proj.ProjectTypeNavigation.Lovcode, paAmt.Amount.SourceType, paAmt.Amount.AmountTypeNavigation.Lovcode, ls.THBAmount, false);
                                    plandt.ActivityPlanSummary.Loan += ls.THBAmount;
                                    var actsource = plandt.ActivityPlanSummary.LoanSourcePlans.Where(w => w.LoanSource.SourceType == paAmt.Amount.SourceType).FirstOrDefault();
                                    if (actsource == null)
                                    {
                                        actsource = new LoanSourcePlan
                                        {
                                            LoanSource = new LoanSource
                                            {
                                                SourceType = paAmt.Amount.SourceType,
                                                Currency = "THB",
                                            },
                                            LoanTypePlans = new List<LoanType>()
                                        };
                                        plandt.ActivityPlanSummary.LoanSourcePlans.Add(actsource);
                                    }
                                    actsource.LoanSource.LoanAmount += ls.THBAmount;
                                    actsource.LoanSource.THBAmount = actsource.LoanSource.LoanAmount;
                                    var acttype = actsource.LoanTypePlans.Where(w => w.SelectedType == paAmt.Amount.AmountTypeNavigation.Lovcode).FirstOrDefault();
                                    if (acttype == null)
                                    {
                                        acttype = new LoanType
                                        {
                                            SelectedType = paAmt.Amount.AmountTypeNavigation.Lovcode,
                                            Currency = "THB"
                                        };
                                        actsource.LoanTypePlans.Add(acttype);
                                    }
                                    acttype.LoanAmount += ls.THBAmount;
                                    var sl = proceed.SignedLoan.Where(w => w.SourceType == paAmt.Amount.SourceType && w.CurrencyCode == paAmt.Amount.Currency).FirstOrDefault();
                                    if (sl == null)
                                    {
                                        sl = new AmountData
                                        {
                                            CurrencyCode = paAmt.Amount.Currency,
                                            SourceType = paAmt.Amount.SourceType,

                                        };
                                        proceed.SignedLoan.Add(sl);
                                    }
                                    sl.Amount += paAmt.Amount.Amount1;
                                    sl.THBAmount += (paAmt.Amount.Amount1 * r.CurrencyRate);

                                    var ysum = plandt.YearPlanSummary.LoanSources.Where(w => w.SourceType == paAmt.Amount.SourceType && w.Currency == paAmt.Amount.Currency).FirstOrDefault();
                                    if (ysum == null)
                                    {
                                        ysum = new LoanSource
                                        {
                                            Currency = paAmt.Amount.Currency,
                                            SourceType = paAmt.Amount.SourceType
                                        };
                                        plandt.YearPlanSummary.LoanSources.Add(ysum);
                                    }
                                    ysum.LoanAmount += paAmt.Amount.Amount1;
                                }
                                #endregion

                                else
                                {
                                    #region SaveProceed

                                    if (true)  //(paAmt.Amount.PeriodType == "Y")
                                    {
                                        var newamt = new AmountData
                                        {
                                            Amount = paAmt.Amount.Amount1,
                                            CurrencyCode = paAmt.Amount.Currency,
                                            SourceType = paAmt.Amount.SourceType,
                                            THBAmount = paAmt.Amount.Amount1 * r.CurrencyRate
                                        };
                                        if (paAmt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.ค่าใช้จ่ายตามมติ)
                                        {
                                            actplan.Activity.ResolutionAmounts.Add(newamt);
                                            actplan.Activity.TotalResolution += newamt.THBAmount;
                                            //storePlanLoanSummary(actplan.ResolutionExpSum, newamt.SourceType, newamt.THBAmount);
                                        }
                                        else if (paAmt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.ค่าใช้จ่ายตามสัญญาจ้าง)
                                        {
                                            actplan.Activity.ContractAmounts.Add(newamt);
                                            actplan.Activity.TotalContract += newamt.THBAmount;
                                            //storePlanLoanSummary(data.actplan.Activity, newamt.SourceType, newamt.THBAmount);

                                        }
                                        else
                                        {


                                            actplan.Activity.TotalProceedByActivity.Total += newamt.THBAmount;
                                            totYear.Year.Total += newamt.THBAmount;
                                            //totMonth.Total += newamt.THBAmount;
                                            if (paAmt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินรายได้)
                                            {
                                                amt = proceed.Revernue;
                                                actplan.Activity.TotalProceedByActivity.Revernue += newamt.THBAmount;
                                                totYear.Year.Revernue += newamt.THBAmount;
                                                plandt.ActivityPlanSummary.Revernue += newamt.THBAmount;
                                                amt.Add(newamt);
                                                //totMonth.Revernue += newamt.THBAmount;
                                            }
                                            if (paAmt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินงบประมาณ)
                                            {
                                                amt = proceed.Budget;
                                                actplan.Activity.TotalProceedByActivity.Budget += newamt.THBAmount;
                                                totYear.Year.Budget += newamt.THBAmount;
                                                plandt.ActivityPlanSummary.Budget += newamt.THBAmount;
                                                amt.Add(newamt);
                                                //totMonth.Budget += newamt.THBAmount;
                                            }
                                            //if (paAmt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินกู้ลงนาม)
                                            //{
                                            //    amt = proceed.SignedLoan;
                                            //    actplan.Activity.TotalProceedByActivity.SignedLoan += newamt.THBAmount;
                                            //    totYear.Year.SignedLoan += newamt.THBAmount;
                                            //    //totMonth.SignedLoan += newamt.THBAmount;
                                            //}
                                            //if (paAmt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินกู้เบิกจ่าย)
                                            //{
                                            //    amt = proceed.DisburseLoan;
                                            //    actplan.Activity.TotalProceedByActivity.DisburseLoan += newamt.THBAmount;
                                            //    totYear.Year.DisburseLoan += newamt.THBAmount;
                                            //    //totMonth.DisburseLoan += newamt.THBAmount;
                                            //    amt.Add(newamt);
                                            //}
                                            if (paAmt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินจากแหล่งอื่นๆ)
                                            {
                                                amt = proceed.Other;
                                                actplan.Activity.TotalProceedByActivity.Other += newamt.THBAmount;
                                                totYear.Year.Other += newamt.THBAmount;
                                                plandt.ActivityPlanSummary.Other += newamt.THBAmount;
                                                //totMonth.Other += newamt.THBAmount;
                                                amt.Add(newamt);
                                            }


                                        }
                                    }
                                    #endregion
                                    #region LoanProcessPlan (Plan)
                                    if (paAmt.Amount.PeriodType == "M" && amountGroup == "P")
                                    {
                                        var period = new PeriodObject(paAmt.Amount.PeriodValue);
                                        var lpp = actplan.LoanProcessPlan.Where(w => w.LoanSource.SourceType == paAmt.Amount.SourceType && w.LoanSource.Currency == paAmt.Amount.Currency).FirstOrDefault();
                                        if (lpp == null)
                                        {
                                            lpp = new LoanProcessPlan
                                            {
                                                LoanSource = new LoanSource
                                                {
                                                    Currency = paAmt.Amount.Currency,
                                                    LoanSourceID = paAmt.Amount.AmountId,
                                                    SourceType = paAmt.Amount.SourceType,

                                                },
                                                LoanProcessPlanDetails = new List<LoanProcessPlanDetail>()
                                            };


                                            actplan.LoanProcessPlan.Add(lpp);
                                        }
                                        lpp.LoanSource.LoanAmount += paAmt.Amount.Amount1;
                                        lpp.LoanSource.THBAmount += paAmt.Amount.Amount1 * r.CurrencyRate;


                                        var lppd = lpp.LoanProcessPlanDetails.Where(w => w.Month == period.Month).FirstOrDefault();
                                        if (lppd == null)
                                        {
                                            lppd = new LoanProcessPlanDetail
                                            {
                                                Month = period.Month
                                            };
                                            lpp.LoanProcessPlanDetails.Add(lppd);
                                        }
                                        if (paAmt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินกู้ลงนาม)
                                        {
                                            lppd.SignedLoan = paAmt.Amount.Amount1;
                                            lpp.SignedLoanSumAmount += paAmt.Amount.Amount1;
                                        }
                                        if (paAmt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินกู้เบิกจ่าย)
                                        {
                                            lppd.DisburseLoan = paAmt.Amount.Amount1;
                                            lpp.DisburseLoanSumAmount += paAmt.Amount.Amount1;
                                        }

                                    }
                                    #endregion

                                }

                            }

                            plandt.ActivityPlans.Add(actplan);
                        }

                        #endregion

                        #region ดึงรายงานผลจาก gf
                        if (amountGroup == "R")
                        {
                            var mapGFPj = new List<string>();
                            foreach (var ract in plandt.ActivityPlans)
                            {
                                var mapGF = new List<string>();
                                ract.LoanProcessPlan = new List<LoanProcessPlan>();
                                var agas = await DB.AgreementAct.Include(a => a.Agreement).Where(w => w.PlanActId == ract.Activity.ProjActID).ToListAsync();
                                foreach (var aga in agas)
                                {
                                    mapGF.Add(aga.Agreement.GftrrefCode);
                                    var ag = await DB.Agreement.Where(w => w.AgreementId == aga.AgreementId).FirstOrDefaultAsync();
                                    if (ag != null)
                                    {
                                        storeLoanSourcePlan(ract, ag.SourceType, ag.LoanCurrency, ag.LoanAmount, ag.LoanAmountThb, ag.SignDate.Month, ag.LoanAmount, 0);

                                        var trans = await DB.AgreementTrans.Where(w => w.AgreementId == ag.AgreementId
                                        && w.PostinDate.Value.Year >= pl.StartYear - 543
                                        && w.FlowTypeNavigation.ParentLov == ServiceModels.Constants.LOVGroup.Transaction_CashFlow_Type_from_GF.เบิกเงินกู้
                                         && (w.Status == "ทำการผ่านรายการเสร็จแล้ว" || w.Status == "ทำเครื่องหมายเพื่อผ่านรายการ")
                                        ).ToListAsync();
                                        foreach (var tran in trans)
                                        {
                                            storeLoanSourcePlan(ract, (tran.CurrencyCode == "THB") ? "L" : "F", tran.CurrencyCode, tran.Amount, tran.BaseAmount, tran.PostinDate.Value.Month, 0, tran.Amount);
                                        }


                                    }
                                }
                                if (mapGF.Count() > 0)
                                {
                                    ract.Activity.GFCode = string.Join(',', mapGF);
                                    mapGFPj.AddRange(mapGF);
                                }
                            }
                            if (mapGFPj.Count() > 0)
                            {
                                plandt.GFCode = string.Join(',', mapGFPj);
                            }
                        }
                        #endregion
                        plandt.YearPlanSummary.LoanSources = plandt.YearPlanSummary.LoanSources.OrderByDescending(o => o.SourceType).ToList();
                        data.PlanDetails.Add(plandt);
                    }
                }

                #endregion
                #region Initail data

                if (amountGroup == "P")
                {
                    var allacts = data.PlanDetails.SelectMany(s => s.ActivityPlans);
                    foreach (var ini in allacts)
                    {

                        if (ini.LoanProcessPlan == null || ini.LoanProcessPlan.Count() == 0)

                        {
                            if (ini.LoanProcessPlan == null)
                            {
                                ini.LoanProcessPlan = new List<LoanProcessPlan>();
                            }
                            var lpp = new LoanProcessPlan
                            {

                                LoanSource = new LoanSource { Currency = "THB", LoanAmount = 0, SourceType = "L" },
                                DisburseLoanSumAmount = 0,
                                SignedLoanSumAmount = 0,
                                LoanProcessPlanDetails = new List<LoanProcessPlanDetail>()

                            };
                            for (var i = 1; i <= 12; i++)
                            {
                                lpp.LoanProcessPlanDetails.Add(new LoanProcessPlanDetail
                                {
                                    DisburseLoan = 0,
                                    Month = i,
                                    SignedLoan = 0
                                });
                            }
                            ini.LoanProcessPlan.Add(lpp);
                            var lpp2 = JsonConvert.DeserializeObject<LoanProcessPlan>(JsonConvert.SerializeObject(lpp));
                            lpp2.LoanSource.Currency = "USD";
                            lpp2.LoanSource.SourceType = "F";
                            lpp2.LoanSource.LoanAmount = 0;
                            ini.LoanProcessPlan.Add(lpp2);
                        }
                    }
                }
                SortNewDebt(data.PlanSummary);
                #endregion
                ret.Data = data;
                ret.IsCompleted = true;
            }

            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        private void storeLoanSourcePlan(ActivityPlan ract , string sourceType, string currency, decimal amt , decimal thb, int month ,decimal signedAmt, decimal disbAmt)
        {
            var lpp = ract.LoanProcessPlan.Where(w => w.LoanSource.SourceType == sourceType && w.LoanSource.Currency == currency).FirstOrDefault();
            if (lpp == null)
            {
                lpp = new LoanProcessPlan
                {
                    LoanSource = new LoanSource
                    {
                        Currency = currency,

                        SourceType = sourceType,

                    },
                    LoanProcessPlanDetails = new List<LoanProcessPlanDetail>()
                };


                ract.LoanProcessPlan.Add(lpp);
            }
            lpp.LoanSource.LoanAmount += amt;
            lpp.LoanSource.THBAmount += thb;


            var lppd = lpp.LoanProcessPlanDetails.Where(w => w.Month == month).FirstOrDefault();
            if (lppd == null)
            {
                lppd = new LoanProcessPlanDetail
                {
                    Month = month
                };
                lpp.LoanProcessPlanDetails.Add(lppd);
            }

            lppd.SignedLoan += signedAmt;
            lpp.SignedLoanSumAmount += signedAmt;
            lppd.DisburseLoan += disbAmt;
            lpp.DisburseLoanSumAmount += disbAmt;
        }
        public async Task<ReturnList<NewDebtPlanActList>> GetNewDebtActList(PlanProjectListParameter p)
        {
            var ret = new ReturnList<NewDebtPlanActList>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var q = DB.PlanAct
                           .Include(pl => pl.PlanProj.Plan)
                           .Include(pj => pj.PlanProj.Project)
                           .Include(a => a.PlanActAmount).ThenInclude(amt => amt.Amount).ThenInclude(at => at.AmountTypeNavigation)
                           .WhereIf(p.PlanID.HasValue, pl => pl.PlanProj.PlanId == p.PlanID.Value)
                           .WhereIf(!string.IsNullOrEmpty(p.ProjectType), pt => pt.PlanProj.ProjectTypeNavigation.Lovcode == p.ProjectType)
                           .WhereIf(!string.IsNullOrEmpty(p.PlanRelease), r => r.PlanProj.Plan.PlanReleaseNavigation.Lovcode == p.PlanRelease)
                           .WhereIf(p.OrganizationID.HasValue, o => o.PlanProj.Plan.OrganizationId == p.OrganizationID.Value)
                           .WhereIf(!string.IsNullOrEmpty(p.Paging.SearchText),  txt => txt.ActivityName.Contains(p.Paging.SearchText) || txt.PlanProj.Project.ProjectThname.Contains(p.Paging.SearchText))
                           .WhereIf(p.StartYear > 0, y => y.PlanProj.Plan.StartYear == p.StartYear)
                           .Where(w => w.PlanProj.Plan.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.แผนก่อหนี้ใหม่);
                ret.TotalRow = await q.CountAsync();
                ret.PageNo = p.Paging.PageNo;
                ret.PageSize = p.Paging.PageSize;
                var acts = await q.PageBy(b => b.PlanProj.PlanId, p.Paging.PageNo, p.Paging.PageSize, true).ToListAsync();
                var tmp = await _com.GetNewDebtActList(acts);
                if (tmp.IsCompleted)
                {
                    ret.Data = tmp.Data;
                    ret.IsCompleted = true;
                }
                else
                {
                    ret.CloneMessage(tmp.Message);
                }
                //var data = new List<NewDebtPlanActList>();
                //foreach (var act in acts)
                //{
                //    var pact = new NewDebtPlanActList
                //    {
                //        ActivityName = act.ActivityName,
                //        ProjectTHName = act.PlanProj.Project.ProjectThname,
                //        PlanActID = act.PlanActId,
                //        LoanSource = new List<LoanSource>()
                //    };
                //    foreach (var amt in act.PlanActAmount)
                //    {
                //        if (loanTypeCodes.Contains(amt.Amount.AmountTypeNavigation.Lovcode))
                //        {
                //            pact.LoanSource.Add(new LoanSource
                //            {
                //                Currency = amt.Amount.Currency,
                //                LoanAmount = amt.Amount.Amount1,
                //                SourceType = amt.Amount.SourceType
                //            });
                //        }
                //    }
                //    data.Add(pact);
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

        public async Task<ReturnList<NewDebtPlanDetails>> GetNewPlanProjectList(PlanProjectListParameter p)
        {
            var ret = new ReturnList<NewDebtPlanDetails>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var q =  DB.PlanProject
                    .Include(pl => pl.Plan)
                    .WhereIf(p.PlanID.HasValue, pl => pl.PlanId == p.PlanID.Value)
                    .WhereIf(!string.IsNullOrEmpty(p.ProjectType), pt => pt.ProjectTypeNavigation.Lovcode == p.ProjectType)
                    .WhereIf(!string.IsNullOrEmpty(p.PlanRelease), r => r.Plan.PlanReleaseNavigation.Lovcode == p.PlanRelease)
                    .WhereIf(p.OrganizationID.HasValue, o => o.Plan.OrganizationId == p.OrganizationID.Value)
                    .WhereIf(!string.IsNullOrEmpty(p.Paging.SearchText), txt => txt.Project.ProjectThname.Contains(p.Paging.SearchText))
                    .Where(w => w.Plan.StartYear == p.StartYear && w.Plan.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.แผนก่อหนี้ใหม่);
                ret.TotalRow = await q.CountAsync();
                ret.PageNo = p.Paging.PageNo;
                ret.PageSize = p.Paging.PageSize;
                var ppjs = await q.PageBy(b => b.ProjectId, p.Paging.PageNo, p.Paging.PageSize,true).ToListAsync();

                //var loanTypeCodes = new string[]
                //{
                //     ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้ตรง,
                //     ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้มาเพื่อให้กู้ต่อ,
                //     ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้โดยขอค้ำจากกระทรวงการคลัง
                //};
                var data = new List<NewDebtPlanDetails>();
                #region PlanDetail
                foreach (var pj in ppjs)
                {
         
                    var pl = pj.Plan;
                    var rate = _helper.GetCurrencyRate(pl.StartYear);
                    if (!rate.IsCompleted)
                    {
                        ret.CloneMessage(rate.Message);
                        return ret;
                    }
                    var plandt = new NewDebtPlanDetails();
                    plandt.PlanID = pj.PlanProjectId;
                    plandt.ProjectID = pj.ProjectId;
                    var proj = await DB.Project.Include(t => t.ProjectTypeNavigation).Where(w => w.ProjectId == pj.ProjectId).FirstOrDefaultAsync();
                    if (proj != null)
                    {
                        plandt.ProjectCode = proj.ProjectCode;
                        plandt.ProjectTHName = proj.ProjectThname;
                        plandt.IsNewProject = proj.IsNewProject;
                        plandt.PdmoAgreement = proj.Pdmoagreement;
                        plandt.ActivityPlanSummary = new ActivityPlanSummary
                        {
                            LoanSourcePlans = new List<LoanSourcePlan>()
                        };
                        plandt.YearPlanSummary = new LoanPeriod
                        {
                            LoanSources = new List<LoanSource>()
                        };
                        plandt.YearPlanSummary.PeriodType = "Y";
                        plandt.YearPlanSummary.PeriodValue = pl.StartYear;
                  
                    }
                    #region from Loan5Y
                    plandt.FromFiveYearPlan = new LoanPeriod
                    {
                        PeriodType = "Y",
                        PeriodValue = pl.StartYear,
                        LoanSources = new List<LoanSource>()
                    };

                    var l5ys = await DB.PlanProject.Where(w => w.ProjectId == pj.ProjectId &&
                    w.Plan.StartYear == pl.StartYear && w.Plan.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.แผน_5_ปี)
                    .Select(s => s.PlanLoan).FirstOrDefaultAsync();
                    if (l5ys != null)
                    {
                        var l5 = l5ys.Where(w => w.PeriodType == "Y" && w.PeriodValue == pl.StartYear);
                        foreach (var l in l5)
                        {
                            var r = rate.Data.Currency.Where(w => w.CurrencyCode == l.LoanCurrency).FirstOrDefault();
                            if (r == null)
                            {
                                ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { $"สกุลเงิน {l.LoanCurrency} ของปี {pl.StartYear}" });
                                return ret;
                            }
                            plandt.FromFiveYearPlan.LoanSources.Add(new LoanSource
                            {
                                Currency = l.LoanCurrency,
                                LoanAmount = l.LoanAmount,
                                LoanSourceID = l.PlanLoanId,
                                SourceType = l.LoanType,
                                THBAmount = l.LoanAmount * r.CurrencyRate
                            });
                        }
                    }

                    #endregion
                    #region Activities
                    var placts = await DB.PlanAct.Where(w => w.PlanProjId == pj.PlanProjectId).ToListAsync();
                    plandt.ActivityPlans = new List<ActivityPlan>();
                    foreach (var plact in placts)
                    {

                        //var actplan = new ActivityPlan
                        //{
                        //    Activity = new ActivityData
                        //    {
                        //        ProjActID = plact.PlanActId,
                        //        ActivityName = plact.ActivityName,
                        //        ContractAmounts = new List<AmountData>(),
                        //        ResolutionAmounts = new List<AmountData>(),
                        //        SaveProceedData = new List<SaveProceed>(),
                        //        Years = new List<ProceedByYear>(),
                        //        TotalProceedByActivity = new ProceedData(),
                        //    },
                        //    ActivityPlanDetail = new SaveProceed(),
                        //    LoanProcessPlan = new List<LoanProcessPlan>(),
                        //    LoanTypePlans = new List<LoanTypePlan>()
                        //};
                        // var period = new PeriodObject(paAmt.Amount.PeriodValue);
                        string year = pl.StartYear.ToString();
                        List<AmountData> amt = new List<AmountData>();
                        //var totYear = actplan.Activity.Years.Where(w => w.Year.Detail == year).FirstOrDefault();
                        //if (totYear == null)
                        //{
                        //    totYear = new ProceedByYear
                        //    {
                        //        Year = new ProceedData { Detail = year },
                        //        Months = new List<ProceedData>()
                        //    };

                        //    actplan.Activity.Years.Add(totYear);
                        //}
 
                        //var proceed = new SaveProceed
                        //{
                        //    Revernue = new List<AmountData>(),
                        //    Budget = new List<AmountData>(),
                        //    SignedLoan = new List<AmountData>(),
                        //    DisburseLoan = new List<AmountData>(),
                        //    Other = new List<AmountData>()
                        //};
                        //proceed.Year = pl.StartYear;
                        //actplan.ActivityPlanDetail = proceed;
                        var paAmts = await DB.PlanActAmount.Include(i => i.Amount).ThenInclude(th => th.AmountTypeNavigation).Where(w => w.PlanActId == plact.PlanActId).ToListAsync();
                        foreach (var paAmt in paAmts)
                        {
                            var r = rate.Data.Currency.Where(w => w.CurrencyCode == paAmt.Amount.Currency).FirstOrDefault();
                            if (r == null)
                            {
                                ret.AddMessage(eMessage.DataIsNotFound.ToString(), "currency", eMessageType.Error, new string[] { $"อัตราแลกเปลี่ยน {paAmt.Amount.Currency} ปี {pl.StartYear}" });
                                return ret;
                            }
                            //plandt.ActivityPlanSummary.Total += paAmt.Amount.Amount1 * r.CurrencyRate;
                            #region LoanTypePlans
                            if (loanTypeCodes.Contains(paAmt.Amount.AmountTypeNavigation.Lovcode))
                            {

                                //// var lovcode = Utilities.GetLovCodeFromKey(lovAmt.Data, paAmt.Amount.AmountType);
                                //var ly = actplan.LoanTypePlans.Where(w => w.LoanType.SelectedType == paAmt.Amount.AmountTypeNavigation.Lovcode).FirstOrDefault();
                                //if (ly == null)
                                //{
                                //    ly = new LoanTypePlan
                                //    {
                                //        LoanType = new LoanType
                                //        {
                                //            SelectedType = paAmt.Amount.AmountTypeNavigation.Lovcode
                                //        },
                                //        LoanSourcePlans = new List<LoanSource>()
                                //    };
                                //    actplan.LoanTypePlans.Add(ly);
                                //}
                                //var ls = new LoanSource
                                //{
                                //    Currency = paAmt.Amount.Currency,
                                //    LoanAmount = paAmt.Amount.Amount1,
                                //    LoanSourceID = paAmt.PlanActAmountId,
                                //    SourceType = paAmt.Amount.SourceType,
                                //    THBAmount = paAmt.Amount.Amount1 * r.CurrencyRate
                                //};
                                //ly.LoanSourcePlans.Add(ls);
                                //newDebtSummary(plsum, proj.ProjectTypeNavigation.Lovcode, paAmt.Amount.SourceType, paAmt.Amount.AmountTypeNavigation.Lovcode, ls.THBAmount);
                                //plandt.ActivityPlanSummary.Loan += ls.THBAmount;
                                //var actsource = plandt.ActivityPlanSummary.LoanSourcePlans.Where(w => w.LoanSource.SourceType == paAmt.Amount.SourceType).FirstOrDefault();
                                //if (actsource == null)
                                //{
                                //    actsource = new LoanSourcePlan
                                //    {
                                //        LoanSource = new LoanSource
                                //        {
                                //            SourceType = paAmt.Amount.SourceType,
                                //            Currency = "THB",
                                //        },
                                //        LoanTypePlans = new List<LoanType>()
                                //    };
                                //    plandt.ActivityPlanSummary.LoanSourcePlans.Add(actsource);
                                //}
                                //actsource.LoanSource.LoanAmount += ls.THBAmount;
                                //actsource.LoanSource.THBAmount = actsource.LoanSource.LoanAmount;
                                //var acttype = actsource.LoanTypePlans.Where(w => w.SelectedType == paAmt.Amount.AmountTypeNavigation.Lovcode).FirstOrDefault();
                                //if (acttype == null)
                                //{
                                //    acttype = new LoanType
                                //    {
                                //        SelectedType = paAmt.Amount.AmountTypeNavigation.Lovcode,
                                //        Currency = "THB"
                                //    };
                                //    actsource.LoanTypePlans.Add(acttype);
                                //}
                                //acttype.LoanAmount += ls.THBAmount;
                                //var sl = proceed.SignedLoan.Where(w => w.SourceType == paAmt.Amount.SourceType && w.CurrencyCode == paAmt.Amount.Currency).FirstOrDefault();
                                //if (sl == null)
                                //{
                                //    sl = new AmountData
                                //    {
                                //        CurrencyCode = paAmt.Amount.Currency,
                                //        SourceType = paAmt.Amount.SourceType,

                                //    };
                                //    proceed.SignedLoan.Add(sl);
                                //}
                                //sl.Amount += paAmt.Amount.Amount1;
                                //sl.THBAmount += (paAmt.Amount.Amount1 * r.CurrencyRate);

                                var ysum = plandt.YearPlanSummary.LoanSources.Where(w => w.SourceType == paAmt.Amount.SourceType && w.Currency == paAmt.Amount.Currency).FirstOrDefault();
                                if (ysum == null)
                                {
                                    ysum = new LoanSource
                                    {
                                        Currency = paAmt.Amount.Currency,
                                        SourceType = paAmt.Amount.SourceType
                                    };
                                    plandt.YearPlanSummary.LoanSources.Add(ysum);
                                }
                                ysum.LoanAmount += paAmt.Amount.Amount1;
                                
                            }
                            #endregion

                            //else
                            //{
                            //    #region SaveProceed

                            //    if (paAmt.Amount.PeriodType == "Y")
                            //    {
                            //        var newamt = new AmountData
                            //        {
                            //            Amount = paAmt.Amount.Amount1,
                            //            CurrencyCode = paAmt.Amount.Currency,
                            //            SourceType = paAmt.Amount.SourceType,
                            //            THBAmount = paAmt.Amount.Amount1 * r.CurrencyRate
                            //        };
                            //        if (paAmt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.ค่าใช้จ่ายตามมติ)
                            //        {
                            //            actplan.Activity.ResolutionAmounts.Add(newamt);
                            //            actplan.Activity.TotalResolution += newamt.THBAmount;
                            //            //storePlanLoanSummary(actplan.ResolutionExpSum, newamt.SourceType, newamt.THBAmount);
                            //        }
                            //        else if (paAmt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.ค่าใช้จ่ายตามสัญญาจ้าง)
                            //        {
                            //            actplan.Activity.ContractAmounts.Add(newamt);
                            //            actplan.Activity.TotalContract += newamt.THBAmount;
                            //            //storePlanLoanSummary(data.actplan.Activity, newamt.SourceType, newamt.THBAmount);
                            //        }
                            //        else
                            //        {


                            //            actplan.Activity.TotalProceedByActivity.Total += newamt.THBAmount;
                            //            totYear.Year.Total += newamt.THBAmount;
                            //            //totMonth.Total += newamt.THBAmount;
                            //            if (paAmt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินรายได้)
                            //            {
                            //                amt = proceed.Revernue;
                            //                actplan.Activity.TotalProceedByActivity.Revernue += newamt.THBAmount;
                            //                totYear.Year.Revernue += newamt.THBAmount;
                            //                plandt.ActivityPlanSummary.Revernue += newamt.THBAmount;
                            //                //totMonth.Revernue += newamt.THBAmount;
                            //            }
                            //            if (paAmt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินงบประมาณ)
                            //            {
                            //                amt = proceed.Budget;
                            //                actplan.Activity.TotalProceedByActivity.Budget += newamt.THBAmount;
                            //                totYear.Year.Budget += newamt.THBAmount;
                            //                plandt.ActivityPlanSummary.Budget += newamt.THBAmount;
                            //                //totMonth.Budget += newamt.THBAmount;
                            //            }
                            //            //if (paAmt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินกู้ลงนาม)
                            //            //{
                            //            //    amt = proceed.SignedLoan;
                            //            //    actplan.Activity.TotalProceedByActivity.SignedLoan += newamt.THBAmount;
                            //            //    totYear.Year.SignedLoan += newamt.THBAmount;
                            //            //    //totMonth.SignedLoan += newamt.THBAmount;
                            //            //}
                            //            if (paAmt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินกู้เบิกจ่าย)
                            //            {
                            //                amt = proceed.DisburseLoan;
                            //                actplan.Activity.TotalProceedByActivity.DisburseLoan += newamt.THBAmount;
                            //                totYear.Year.DisburseLoan += newamt.THBAmount;
                            //                //totMonth.DisburseLoan += newamt.THBAmount;
                            //            }
                            //            if (paAmt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินจากแหล่งอื่นๆ)
                            //            {
                            //                amt = proceed.Other;
                            //                actplan.Activity.TotalProceedByActivity.Other += newamt.THBAmount;
                            //                totYear.Year.Other += newamt.THBAmount;
                            //                plandt.ActivityPlanSummary.Other += newamt.THBAmount;
                            //                //totMonth.Other += newamt.THBAmount;
                            //            }
                            //            amt.Add(newamt);

                            //        }
                            //    }
                            //    #endregion
                            //    #region LoanProcessPlan
                            //    if (paAmt.Amount.PeriodType == "M")
                            //    {
                            //        var period = new PeriodObject(paAmt.Amount.PeriodValue);
                            //        var lpp = actplan.LoanProcessPlan.Where(w => w.LoanSource.SourceType == paAmt.Amount.SourceType && w.LoanSource.Currency == paAmt.Amount.Currency).FirstOrDefault();
                            //        if (lpp == null)
                            //        {
                            //            lpp = new LoanProcessPlan
                            //            {
                            //                LoanSource = new LoanSource
                            //                {
                            //                    Currency = paAmt.Amount.Currency,
                            //                    LoanSourceID = paAmt.Amount.AmountId,
                            //                    SourceType = paAmt.Amount.SourceType,

                            //                },
                            //                LoanProcessPlanDetails = new List<LoanProcessPlanDetail>()
                            //            };


                            //            actplan.LoanProcessPlan.Add(lpp);
                            //        }
                            //        lpp.LoanSource.LoanAmount += paAmt.Amount.Amount1;
                            //        lpp.LoanSource.THBAmount += paAmt.Amount.Amount1 * r.CurrencyRate;


                            //        var lppd = lpp.LoanProcessPlanDetails.Where(w => w.Month == period.Month).FirstOrDefault();
                            //        if (lppd == null)
                            //        {
                            //            lppd = new LoanProcessPlanDetail
                            //            {
                            //                Month = period.Month
                            //            };
                            //            lpp.LoanProcessPlanDetails.Add(lppd);
                            //        }
                            //        if (paAmt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินกู้ลงนาม)
                            //        {
                            //            lppd.SignedLoan = paAmt.Amount.Amount1;
                            //            lpp.SignedLoanSumAmount += paAmt.Amount.Amount1;
                            //        }
                            //        if (paAmt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินกู้เบิกจ่าย)
                            //        {
                            //            lppd.DisburseLoan = paAmt.Amount.Amount1;
                            //            lpp.DisburseLoanSumAmount += paAmt.Amount.Amount1;
                            //        }

                            //    }
                            //    #endregion

                            //}

                        }
                        //plandt.ActivityPlans.Add(actplan);
                    }

                    #endregion
                    data.Add(plandt);
                }
                ret.Data = data;
                ret.IsCompleted = true;
                #endregion
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;

        }

        public async Task<ReturnObject<NewDebtPlanSummary>> GetNewPlanSummary(PlanProjectListParameter p)
        {
            var ret = new ReturnObject<NewDebtPlanSummary>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var ppjs = await DB.PlanProject
             .Include(pl => pl.Plan)
             .WhereIf(p.PlanID.HasValue, pl => pl.PlanId == p.PlanID.Value)
             .WhereIf(!string.IsNullOrEmpty(p.ProjectType), pt => pt.ProjectTypeNavigation.Lovcode == p.ProjectType)
             .WhereIf(!string.IsNullOrEmpty(p.PlanRelease), r => r.Plan.PlanReleaseNavigation.Lovcode == p.PlanRelease)
             .WhereIf(p.OrganizationID.HasValue, o => o.Plan.OrganizationId == p.OrganizationID.Value)
             .Where(w => w.Plan.StartYear == p.StartYear && w.Plan.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.แผนก่อหนี้ใหม่).ToListAsync();
    
             
                //var loanTypeCodes = new string[]
                //{
                //     ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้ตรง,
                //     ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้มาเพื่อให้กู้ต่อ,
                //     ServiceModels.Constants.LOVGroup.Project_Amount_Type.กู้โดยขอค้ำจากกระทรวงการคลัง
                //};
                var data = new NewDebtPlanSummary {
                    CurrencyData = new CurrencyData(),
                    OverAllByLoanSource = new List<LoanSource>(),
                    OverAllByLoanType = new List<LoanType>(),
                    OverAllByProjectType = new List<ProjectTypeLoanSummary>()
                };
                var rate = _helper.GetCurrencyRate(p.StartYear);
                if (!rate.IsCompleted)
                {
                    ret.CloneMessage(rate.Message);
                    return ret;
                }
                data.CurrencyData = rate.Data;
                #region PlanDetail
                foreach (var pj in ppjs)
                {

                    var pl = pj.Plan;
       
                    var plandt = new NewDebtPlanDetails();
                    plandt.PlanID = pj.PlanProjectId;
                    plandt.ProjectID = pj.ProjectId;
                    var proj = await DB.Project.Include(t => t.ProjectTypeNavigation).Where(w => w.ProjectId == pj.ProjectId).FirstOrDefaultAsync();
                    //if (proj != null)
                    //{
                    //    plandt.ProjectCode = proj.ProjectCode;
                    //    plandt.ProjectTHName = proj.ProjectThname;
                    //    plandt.IsNewProject = proj.IsNewProject;
                    //    plandt.PdmoAgreement = proj.Pdmoagreement;
                    //    plandt.ActivityPlanSummary = new ActivityPlanSummary
                    //    {
                    //        LoanSourcePlans = new List<LoanSourcePlan>()
                    //    };
                    //    plandt.YearPlanSummary = new LoanPeriod
                    //    {
                    //        LoanSources = new List<LoanSource>()
                    //    };
                    //    plandt.YearPlanSummary.PeriodType = "Y";
                    //    plandt.YearPlanSummary.PeriodValue = pl.StartYear;

                    //}
                    #region from Loan5Y
                    //plandt.FromFiveYearPlan = new LoanPeriod
                    //{
                    //    PeriodType = "Y",
                    //    PeriodValue = pl.StartYear,
                    //    LoanSources = new List<LoanSource>()
                    //};

                    //var l5ys = await DB.PlanProject.Where(w => w.ProjectId == pj.ProjectId &&
                    //w.Plan.StartYear == pl.StartYear && w.Plan.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.แผน_5_ปี)
                    //.Select(s => s.PlanLoan).FirstOrDefaultAsync();
                    //if (l5ys != null)
                    //{
                    //    var l5 = l5ys.Where(w => w.PeriodType == "Y" && w.PeriodValue == pl.StartYear);
                    //    foreach (var l in l5)
                    //    {
                    //        var r = rate.Data.Currency.Where(w => w.CurrencyCode == l.LoanCurrency).FirstOrDefault();
                    //        if (r == null)
                    //        {
                    //            ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { $"สกุลเงิน {l.LoanCurrency} ของปี {pl.StartYear}" });
                    //            return ret;
                    //        }
                    //        plandt.FromFiveYearPlan.LoanSources.Add(new LoanSource
                    //        {
                    //            Currency = l.LoanCurrency,
                    //            LoanAmount = l.LoanAmount,
                    //            LoanSourceID = l.PlanLoanId,
                    //            SourceType = l.LoanType,
                    //            THBAmount = l.LoanAmount * r.CurrencyRate
                    //        });
                    //    }
                    //}

                    #endregion
                    #region Activities
                    var placts = await DB.PlanAct.Where(w => w.PlanProjId == pj.PlanProjectId).ToListAsync();
                    plandt.ActivityPlans = new List<ActivityPlan>();
                    foreach (var plact in placts)
                    {

                        //var actplan = new ActivityPlan
                        //{
                        //    Activity = new ActivityData
                        //    {
                        //        ProjActID = plact.PlanActId,
                        //        ActivityName = plact.ActivityName,
                        //        ContractAmounts = new List<AmountData>(),
                        //        ResolutionAmounts = new List<AmountData>(),
                        //        SaveProceedData = new List<SaveProceed>(),
                        //        Years = new List<ProceedByYear>(),
                        //        TotalProceedByActivity = new ProceedData(),
                        //    },
                        //    ActivityPlanDetail = new SaveProceed(),
                        //    LoanProcessPlan = new List<LoanProcessPlan>(),
                        //    LoanTypePlans = new List<LoanTypePlan>()
                        //};
                        // var period = new PeriodObject(paAmt.Amount.PeriodValue);
                        string year = pl.StartYear.ToString();
                        List<AmountData> amt = new List<AmountData>();
                        //var totYear = actplan.Activity.Years.Where(w => w.Year.Detail == year).FirstOrDefault();
                        //if (totYear == null)
                        //{
                        //    totYear = new ProceedByYear
                        //    {
                        //        Year = new ProceedData { Detail = year },
                        //        Months = new List<ProceedData>()
                        //    };

                        //    actplan.Activity.Years.Add(totYear);
                        //}

                        //var proceed = new SaveProceed
                        //{
                        //    Revernue = new List<AmountData>(),
                        //    Budget = new List<AmountData>(),
                        //    SignedLoan = new List<AmountData>(),
                        //    DisburseLoan = new List<AmountData>(),
                        //    Other = new List<AmountData>()
                        //};
                        //proceed.Year = pl.StartYear;
                        //actplan.ActivityPlanDetail = proceed;
                        var paAmts = await DB.PlanActAmount.Include(i => i.Amount).ThenInclude(th => th.AmountTypeNavigation).Where(w => w.PlanActId == plact.PlanActId).ToListAsync();
                        foreach (var paAmt in paAmts)
                        {
                            var r = rate.Data.Currency.Where(w => w.CurrencyCode == paAmt.Amount.Currency).FirstOrDefault();
                            if (r == null)
                            {
                                ret.AddMessage(eMessage.DataIsNotFound.ToString(), "currency", eMessageType.Error, new string[] { $"อัตราแลกเปลี่ยน {paAmt.Amount.Currency} ปี {pl.StartYear}" });
                                return ret;
                            }
                            //plandt.ActivityPlanSummary.Total += paAmt.Amount.Amount1 * r.CurrencyRate;
                            #region LoanTypePlans
                            if (loanTypeCodes.Contains(paAmt.Amount.AmountTypeNavigation.Lovcode))
                            {

                                // var lovcode = Utilities.GetLovCodeFromKey(lovAmt.Data, paAmt.Amount.AmountType);
                                //var ly = actplan.LoanTypePlans.Where(w => w.LoanType.SelectedType == paAmt.Amount.AmountTypeNavigation.Lovcode).FirstOrDefault();
                                //if (ly == null)
                                //{
                                //    ly = new LoanTypePlan
                                //    {
                                //        LoanType = new LoanType
                                //        {
                                //            SelectedType = paAmt.Amount.AmountTypeNavigation.Lovcode
                                //        },
                                //        LoanSourcePlans = new List<LoanSource>()
                                //    };
                                //    actplan.LoanTypePlans.Add(ly);
                                //}
                                var ls = new LoanSource
                                {
                                    Currency = paAmt.Amount.Currency,
                                    LoanAmount = paAmt.Amount.Amount1,
                                    LoanSourceID = paAmt.PlanActAmountId,
                                    SourceType = paAmt.Amount.SourceType,
                                    THBAmount = paAmt.Amount.Amount1 * r.CurrencyRate
                                };
                               // ly.LoanSourcePlans.Add(ls);
                                newDebtSummary(data, proj.ProjectTypeNavigation.Lovcode, paAmt.Amount.SourceType, paAmt.Amount.AmountTypeNavigation.Lovcode, ls.THBAmount,true);
                                //plandt.ActivityPlanSummary.Loan += ls.THBAmount;
                                //var actsource = plandt.ActivityPlanSummary.LoanSourcePlans.Where(w => w.LoanSource.SourceType == paAmt.Amount.SourceType).FirstOrDefault();
                                //if (actsource == null)
                                //{
                                //    actsource = new LoanSourcePlan
                                //    {
                                //        LoanSource = new LoanSource
                                //        {
                                //            SourceType = paAmt.Amount.SourceType,
                                //            Currency = "THB",
                                //        },
                                //        LoanTypePlans = new List<LoanType>()
                                //    };
                                //    plandt.ActivityPlanSummary.LoanSourcePlans.Add(actsource);
                                //}
                                //actsource.LoanSource.LoanAmount += ls.THBAmount;
                                //actsource.LoanSource.THBAmount = actsource.LoanSource.LoanAmount;
                                //var acttype = actsource.LoanTypePlans.Where(w => w.SelectedType == paAmt.Amount.AmountTypeNavigation.Lovcode).FirstOrDefault();
                                //if (acttype == null)
                                //{
                                //    acttype = new LoanType
                                //    {
                                //        SelectedType = paAmt.Amount.AmountTypeNavigation.Lovcode,
                                //        Currency = "THB"
                                //    };
                                //    actsource.LoanTypePlans.Add(acttype);
                                //}
                                //acttype.LoanAmount += ls.THBAmount;
                                //var sl = proceed.SignedLoan.Where(w => w.SourceType == paAmt.Amount.SourceType && w.CurrencyCode == paAmt.Amount.Currency).FirstOrDefault();
                                //if (sl == null)
                                //{
                                //    sl = new AmountData
                                //    {
                                //        CurrencyCode = paAmt.Amount.Currency,
                                //        SourceType = paAmt.Amount.SourceType,

                                //    };
                                //    proceed.SignedLoan.Add(sl);
                                //}
                                //sl.Amount += paAmt.Amount.Amount1;
                                //sl.THBAmount += (paAmt.Amount.Amount1 * r.CurrencyRate);

                                //var ysum = plandt.YearPlanSummary.LoanSources.Where(w => w.SourceType == paAmt.Amount.SourceType && w.Currency == paAmt.Amount.Currency).FirstOrDefault();
                                //if (ysum == null)
                                //{
                                //    ysum = new LoanSource
                                //    {
                                //        Currency = paAmt.Amount.Currency,
                                //        SourceType = paAmt.Amount.SourceType
                                //    };
                                //    plandt.YearPlanSummary.LoanSources.Add(ysum);
                                //}
                                //ysum.LoanAmount += paAmt.Amount.Amount1;

                            }
                            #endregion

              
                        }
                        //plandt.ActivityPlans.Add(actplan);
                    }

                    #endregion
                    //data.Add(plandt);
                }
                SortNewDebt(data);
                ret.Data = data;
                ret.IsCompleted = true;
                #endregion
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        public async Task<ReturnMessage> DuplicateNewDebtPlan(long newPlan, CreatePlanParameter p)
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
                var old = await GetPreviousPlanRevision(p, ServiceModels.Constants.LOVGroup.Plan_Type.แผนก่อหนี้ใหม่);
                if (!old.IsCompleted)
                {
                    return ret;
                }
                var oldPl = await DB.Plan
                    .Include(pj => pj.PlanProject).ThenInclude(pj1 => pj1.PlanAct).ThenInclude(pj2 => pj2.PlanActAmount).ThenInclude(pj3 => pj3.Amount)
                    .Include(pj => pj.PlanProject).ThenInclude(pj1 => pj1.PlanAct).ThenInclude(pj2 => pj2.AgreementAct)
                    .Where(w => w.PlanId == old.Data.Value).FirstOrDefaultAsync();
                foreach (var pj in oldPl.PlanProject)
                {
                    var newPj = new PlanProject
                    {
                        ProjectId = pj.ProjectId,
                        ProjectType = pj.ProjectType,
                        IsNotRequiredApproval = pj.IsNotRequiredApproval
                    };
                    newPl.PlanProject.Add(newPj);
                    foreach (var pa in pj.PlanAct)
                    {
                        var newPa = new PlanAct
                        {
                            ActivityName = pa.ActivityName,
                            ProjActId = pa.ProjActId,
                            ReferencePlanActId = pa.ReferencePlanActId
                            //Todo : reference to Loan
                        };
                        newPj.PlanAct.Add(newPa);
                        foreach (var paa in pa.PlanActAmount)
                        {
                            var newPaa = new PlanActAmount
                            {
                                Amount = new Amount
                                {
                                    Amount1 = paa.Amount.Amount1,
                                    AmountType = paa.Amount.AmountType,
                                    Currency = paa.Amount.Currency,
                                    PeriodType = paa.Amount.PeriodType,
                                    PeriodValue = paa.Amount.PeriodValue,
                                    SourceType = paa.Amount.SourceType,
                                    AmountGroup = paa.Amount.AmountGroup
                                }
                            };
                            newPa.PlanActAmount.Add(newPaa);
                          
                        }
                        foreach (var aa in pa.AgreementAct)
                        {
                            var newAa = new AgreementAct
                            {
                                AgreementId = aa.AgreementId
                            };
                            newPa.AgreementAct.Add(newAa);
                        }
                       // pa.AgreementAct.Clear();
                        
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
