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
using mof.ServiceModels.FinancialReport;
using mof.ServiceModels.Agreement;

namespace mof.Services
{
    public partial class PlanRepository : IPlan
    {


        private void updateFinAmount(Plan plan, List<Amount> amts)
        {
            var dels = new List<PlanFinance>();
            foreach (var db in plan.PlanFinance)
            {
                var amt = amts.Where(w => w.PeriodType == db.Amount.PeriodType && w.PeriodValue == db.Amount.PeriodValue
                && w.SourceType == db.Amount.SourceType && w.AmountType == db.Amount.AmountType && w.Currency == db.Amount.Currency).FirstOrDefault();
                if (amt == null)
                {
                    dels.Add(db);
                }
            }
            foreach (var del in dels)
            {
                plan.PlanFinance.Remove(del);
                // dbPF.Remove(del);
            }
            foreach (var a in amts)
            {
                var amt = plan.PlanFinance.Where(w => w.Amount.PeriodType == a.PeriodType && w.Amount.PeriodValue == a.PeriodValue
                && w.Amount.SourceType == a.SourceType && w.Amount.AmountType == a.AmountType && w.Amount.Currency == a.Currency).FirstOrDefault();
                if (amt == null)
                {
                    amt = new PlanFinance
                    {
                        Amount = new Amount
                        {
                            Currency = a.Currency,
                            PeriodType = a.PeriodType,
                            PeriodValue = a.PeriodValue,
                            AmountType = a.AmountType,
                            SourceType = a.SourceType
                        }

                    };

                    plan.PlanFinance.Add(amt);
                }
                amt.Amount.Amount1 = a.Amount1;
            }
        }

        private void storeFinAmount(List<LOV> lovs, List<Amount> amts, decimal val, string amtType, string currency,
            string periodType, int periodValue, string sourceType)
        {
            amts.Add(new Amount
            {
                Amount1 = val,
                AmountType = Utilities.GetLovKeyFromCode(lovs, amtType),
                Currency = currency,
                PeriodType = periodType,
                PeriodValue = periodValue,
                SourceType = sourceType

            });
        }
        public async Task<ReturnObject<long?>> ModifyFinRep(FinancialReportModel p, string userID, long planID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var plan = await DB.Plan.Include(pt => pt.PlanTypeNavigation)
                    .Include(c => c.PlanExtend)
                    .Include(ag => ag.PlanAgreement)
                    .Include(f => f.PlanFinance).ThenInclude(a => a.Amount)
                    .Include(file => file.PlanAttach).ThenInclude(att => att.AttachFile)
                    .Include(d => d.Dscrnote)
                    .Where(w => w.PlanId == planID).FirstOrDefaultAsync();

                if (plan == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "plan", eMessageType.Error, new string[] { _msglocalizer[eMessage.Plan.ToString()] });
                    return ret;

                }
                var rate = _helper.GetCurrencyRate(plan.StartYear);
                if (!rate.IsCompleted)
                {
                    ret.CloneMessage(rate.Message);
                    return ret;
                }
                var lovs = _helper.GetLOVByGroup(ServiceModels.Constants.LOVGroup.Financial_Report._LOVGroupCode);
                var allAmts = new List<Amount>();
                #region coffer
                var remC = plan.PlanExtend.Where(w => w.DataGroup == "COFFER").ToList();
                foreach (var co in remC)
                {
                    plan.PlanExtend.Remove(co);
                }
                if (p.Coffers != null && p.Coffers.Count > 0)
                {
                    plan.PlanExtend.Add(new PlanExtend
                    {
                        Data = JsonConvert.SerializeObject(p.Coffers),
                        DataGroup = "COFFER"
                    });
                }
                #endregion
                #region BalanceSheet
                foreach (var fld in p.BalanceSheet)
                {

                    storeFinAmount(lovs.Data, allAmts, fld.Asset, ServiceModels.Constants.LOVGroup.Financial_Report.BAL01_สินทรัพย์, "THB", "Y", int.Parse(fld.Year), "L");
                    storeFinAmount(lovs.Data, allAmts, fld.CurrentAsset, ServiceModels.Constants.LOVGroup.Financial_Report.BAL0101_สินทรัพย์หมุนเวียน, "THB", "Y", int.Parse(fld.Year), "L");
                    storeFinAmount(lovs.Data, allAmts, fld.FixedAsset, ServiceModels.Constants.LOVGroup.Financial_Report.BAL0102_สินทรัพยถาวร, "THB", "Y", int.Parse(fld.Year), "L");
                    storeFinAmount(lovs.Data, allAmts, fld.LiabilityEquity, ServiceModels.Constants.LOVGroup.Financial_Report.BAL02_หนี้สินและส่วนของทุน, "THB", "Y", int.Parse(fld.Year), "L");
                    storeFinAmount(lovs.Data, allAmts, fld.Liability, ServiceModels.Constants.LOVGroup.Financial_Report.BAL0201_หนี้สิน, "THB", "Y", int.Parse(fld.Year), "L");
                    storeFinAmount(lovs.Data, allAmts, fld.CurrentLiability, ServiceModels.Constants.LOVGroup.Financial_Report.BAL020101_หนี้สินหมุนเวียน, "THB", "Y", int.Parse(fld.Year), "L");
                    storeFinAmount(lovs.Data, allAmts, fld.LongTermLiability, ServiceModels.Constants.LOVGroup.Financial_Report.BAL020102_หนี้สินระยะยาว, "THB", "Y", int.Parse(fld.Year), "L");
                    storeFinAmount(lovs.Data, allAmts, fld.Equity, ServiceModels.Constants.LOVGroup.Financial_Report.BAL0202_ส่วนของทุน, "THB", "Y", int.Parse(fld.Year), "L");

                }
                #endregion
                #region IncomeStatement
                foreach (var fld in p.IncomeStatements)
                {

                    storeFinAmount(lovs.Data, allAmts, fld.Revenue, ServiceModels.Constants.LOVGroup.Financial_Report.PLS01_รายได้, "THB", "Y", int.Parse(fld.Year), "L");
                    storeFinAmount(lovs.Data, allAmts, fld.Expense, ServiceModels.Constants.LOVGroup.Financial_Report.PLS02_ค่าใช้จ่าย, "THB", "Y", int.Parse(fld.Year), "L");
                    storeFinAmount(lovs.Data, allAmts, fld.GlossProfit, ServiceModels.Constants.LOVGroup.Financial_Report.PLS03_กำไรขาดทุน_จากการดำเนินงาน, "THB", "Y", int.Parse(fld.Year), "L");
                    storeFinAmount(lovs.Data, allAmts, fld.EBITDA, ServiceModels.Constants.LOVGroup.Financial_Report.PLS04_กำไรขาดทุน_ก่อนหักดอกเบี้ยภาษี_ค่าเสือม_และตัดจำหน่าย__EBITDA_, "THB", "Y", int.Parse(fld.Year), "L");
                    storeFinAmount(lovs.Data, allAmts, fld.NetProfit, ServiceModels.Constants.LOVGroup.Financial_Report.PLS05_กำไรขาดทุน_สุทธิ, "THB", "Y", int.Parse(fld.Year), "L");


                }
                #endregion
                #region CashFlows
                foreach (var fld in p.CashFlows)
                {

                    storeFinAmount(lovs.Data, allAmts, fld.Operation, ServiceModels.Constants.LOVGroup.Financial_Report.CF01_กระแสเงินจากการดำเนินงาน, "THB", "Y", int.Parse(fld.Year), "L");
                    storeFinAmount(lovs.Data, allAmts, fld.Investing, ServiceModels.Constants.LOVGroup.Financial_Report.CF02_กระแสเงินสดจากการลงทุน, "THB", "Y", int.Parse(fld.Year), "L");
                    storeFinAmount(lovs.Data, allAmts, fld.Financing, ServiceModels.Constants.LOVGroup.Financial_Report.CF03_กระแสเงินสดจากการจัดหาเงิน, "THB", "Y", int.Parse(fld.Year), "L");
                    storeFinAmount(lovs.Data, allAmts, fld.NetCashFlow, ServiceModels.Constants.LOVGroup.Financial_Report.CF0401_เงินสดสุทธฺรับมาก__น้อย__กว่าเงินสดจ่าย, "THB", "Y", int.Parse(fld.Year), "L");
                    storeFinAmount(lovs.Data, allAmts, fld.CashBalanceBeginning, ServiceModels.Constants.LOVGroup.Financial_Report.CF0402_เงินสดคงเหลือต้นงวด, "THB", "Y", int.Parse(fld.Year), "L");
                    storeFinAmount(lovs.Data, allAmts, fld.CashBalanceEnding, ServiceModels.Constants.LOVGroup.Financial_Report.CF0403_เงินสดคงเหลือปลาดงวด, "THB", "Y", int.Parse(fld.Year), "L");

                }
                #endregion
                #region Debt
                //var delAgrs = new List<PlanAgreement>();
                //foreach (var agr in plan.PlanAgreement)
                //{
                //    var del = p.DebtSummary.CurrentDebt.Where(w => w.AgreementID == agr.AgreementId).FirstOrDefault();
                //    if (del == null)
                //    {
                //        delAgrs.Add(agr);
                //    }
                //}
                //foreach (var d in delAgrs)
                //{
                //    plan.PlanAgreement.Remove(d);
                //}
                //foreach (var agr in p.DebtSummary.CurrentDebt)
                //{
                //    var pa = plan.PlanAgreement.Where(w => w.AgreementId == agr.AgreementID).FirstOrDefault();
                //    if (pa == null)
                //    {
                //        pa = new PlanAgreement
                //        {
                //            AgreementId = agr.AgreementID
                //        };
                //        plan.PlanAgreement.Add(pa);
                //    }

                //}
                #region ORGLocalDebtEstimation
                foreach (var fld in p.DebtSummary.ORGLocalDebtEstimation)
                {

                    storeFinAmount(lovs.Data, allAmts, fld.OutStandingDebt, ServiceModels.Constants.LOVGroup.Financial_Report.DAL01_ยอดหนี้คงค้าง__ประมาณการภาระหนี้ทั้งหมด___ในประเทศ_, "THB", "Y", int.Parse(fld.Year), "L");
                    storeFinAmount(lovs.Data, allAmts, fld.Disbursement, ServiceModels.Constants.LOVGroup.Financial_Report.DAL02_การเบิกจ่าย__ประมาณการภาระหนี้ทั้งหมด___ในประเทศ_, "THB", "Y", int.Parse(fld.Year), "L");
                    storeFinAmount(lovs.Data, allAmts, fld.Principle, ServiceModels.Constants.LOVGroup.Financial_Report.DAL0301_การชำระคืน_เงินต้น__ประมาณการภาระหนี้ทั้งหมด___ในประเทศ_, "THB", "Y", int.Parse(fld.Year), "L");
                    storeFinAmount(lovs.Data, allAmts, fld.Interest, ServiceModels.Constants.LOVGroup.Financial_Report.DAL0302_การชำระคืน__ดอกเบี้ยค่าธรรมเนียม__ประมาณการภาระหนี้ทั้งหมด___ในประเทศ_, "THB", "Y", int.Parse(fld.Year), "L");
                    storeFinAmount(lovs.Data, allAmts, fld.NetCashFlow, ServiceModels.Constants.LOVGroup.Financial_Report.DAL04_กระแสเงินไหลเข้าสุทธิ__ประมาณการภาระหนี้ทั้งหมด___ในประเทศ_, "THB", "Y", int.Parse(fld.Year), "L");

                }
                #endregion
                #region ORGForeignDebtEstimationList
                foreach (var f in p.DebtSummary.ORGForeignDebtEstimationList)
                {
                    foreach (var fld in f.ORGForeignDebtEstimation)
                    {
                        storeFinAmount(lovs.Data, allAmts, fld.OutStandingDebt, ServiceModels.Constants.LOVGroup.Financial_Report.DAF01_ยอดหนี้คงค้าง__ประมาณการภาระหนี้ทั้งหมด___ต่างประเทศ_, f.Currency, "Y", int.Parse(fld.Year), "L");
                        storeFinAmount(lovs.Data, allAmts, fld.Disbursement, ServiceModels.Constants.LOVGroup.Financial_Report.DAF02_การเบิกจ่าย__ประมาณการภาระหนี้ทั้งหมด___ต่างประเทศ_, f.Currency, "Y", int.Parse(fld.Year), "L");
                        storeFinAmount(lovs.Data, allAmts, fld.Principle, ServiceModels.Constants.LOVGroup.Financial_Report.DAF0301_การชำระคืน_เงินต้น__ประมาณการภาระหนี้ทั้งหมด___ต่างประเทศ_, f.Currency, "Y", int.Parse(fld.Year), "L");
                        storeFinAmount(lovs.Data, allAmts, fld.Interest, ServiceModels.Constants.LOVGroup.Financial_Report.DAF0302_การชำระคืน__ดอกเบี้ยค่าธรรมเนียม__ประมาณการภาระหนี้ทั้งหมด___ต่างประเทศ_, f.Currency, "Y", int.Parse(fld.Year), "L");
                        storeFinAmount(lovs.Data, allAmts, fld.NetCashFlow, ServiceModels.Constants.LOVGroup.Financial_Report.DAF04_กระแสเงินไหลเข้าสุทธิ__ประมาณการภาระหนี้ทั้งหมด___ต่างประเทศ_, f.Currency, "Y", int.Parse(fld.Year), "L");
                    }

                }
                #endregion
                #region GOVLocalDebtEstimation
                foreach (var fld in p.DebtSummary.GOVLocalDebtEstimation)
                {

                    storeFinAmount(lovs.Data, allAmts, fld.Principle, ServiceModels.Constants.LOVGroup.Financial_Report.GDL01_เงินต้น__ประมาณการภาระหนี้ที่รัฐบาลต้องรับภาระ___ในประเทศ_, "THB", "Y", int.Parse(fld.Year), "L");
                    storeFinAmount(lovs.Data, allAmts, fld.Interest, ServiceModels.Constants.LOVGroup.Financial_Report.GDL02_ดอกเบี้ยค่าธรรมเนียม__ประมาณการภาระหนี้ที่รัฐบาลต้องรับภาระ___ในประเทศ_, "THB", "Y", int.Parse(fld.Year), "L");
                    storeFinAmount(lovs.Data, allAmts, fld.Fee, ServiceModels.Constants.LOVGroup.Financial_Report.GDL03_ค่าธรรมเนียม__ประมาณการภาระหนี้ที่รัฐบาลต้องรับภาระ___ในประเทศ_, "THB", "Y", int.Parse(fld.Year), "L");

                }
                #endregion
                #region GOVForeignDebtEstimationList
                foreach (var f in p.DebtSummary.GOVForeignDebtEstimationList)
                {
                    foreach (var fld in f.GOVForeignDebtEstimation)
                    {
                        storeFinAmount(lovs.Data, allAmts, fld.Principle, ServiceModels.Constants.LOVGroup.Financial_Report.GDF01_ยอดหนี้คงค้าง__ประมาณการภาระหนี้ที่รัฐต้องรับภาระ___ต่างประเทศ_, f.Currency, "Y", int.Parse(fld.Year), "L");
                        storeFinAmount(lovs.Data, allAmts, fld.Interest, ServiceModels.Constants.LOVGroup.Financial_Report.GDF02_ดอกเบี้ยค่าธรรมเนียม__ประมาณการภาระหนี้ที่รัฐต้องรับภาระ___ต่างประเทศ_, f.Currency, "Y", int.Parse(fld.Year), "L");
                        storeFinAmount(lovs.Data, allAmts, fld.Fee, ServiceModels.Constants.LOVGroup.Financial_Report.GDF03_ค่าธรรมเนียม__ประมาณการภาระหนี้ที่รัฐต้องรับภาระ___ต่างประเทศ_, f.Currency, "Y", int.Parse(fld.Year), "L");

                    }

                }
                #endregion
                #endregion
                var delDscrs = new List<Dscrnote>();
                foreach (var dbDscr in plan.Dscrnote)
                {
                    var del = p.DSCRNotes.Where(w => w.DSRCNoteID == dbDscr.DscrnoteId).FirstOrDefault();
                    if (del == null)
                    {
                        delDscrs.Add(dbDscr);
                    }
                }
                foreach (var d in delDscrs)
                {
                    plan.Dscrnote.Remove(d);
                }
                foreach (var dscr in p.DSCRNotes)
                {
                    var newDscr = plan.Dscrnote.Where(w => w.DscrnoteId == dscr.DSRCNoteID).FirstOrDefault();
                    if (newDscr == null)
                    {
                        newDscr = new Dscrnote
                        {

                        };
                        plan.Dscrnote.Add(newDscr);
                    }
                    newDscr.ProgressUpdate = dscr.ProgressUpdate;
                    newDscr.Reason = dscr.Reason;
                    newDscr.Dscr = dscr.DSCR;
                    newDscr.Solution = dscr.Solution;
                    newDscr.Year = dscr.Year;
                }
                if (p.AttachFiles != null)
                {
                    var oatt = await PlanAttachFile(DB, plan, p.AttachFiles);
                    if (!oatt.IsCompleted)
                    {
                        ret.CloneMessage(oatt.Message);
                        return ret;
                    }
                }


                updateFinAmount(plan, allAmts);
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
        public async Task<ReturnObject<Debt>> GetFinPlanDebtSummary(PlanProjectListParameter p, eGetPlanType gettype)
        {
            var ret = new ReturnObject<Debt>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                int? searchYear = (p.StartYear == 0) ? null : (int?)p.StartYear;
                var sql = DB.PlanFinance
                .Include(a => a.Amount).ThenInclude(amt => amt.AmountTypeNavigation)
                .Include(a => a.Plan.PlanAgreement)
                .Include(att => att.Plan.PlanAttach).ThenInclude(f => f.AttachFile)
                .Include(pl => pl.Plan)
                .WhereIf(p.PlanID.HasValue, pl => pl.PlanId == p.PlanID.Value)
                .WhereIf(!string.IsNullOrEmpty(p.PlanRelease), r => r.Plan.PlanReleaseNavigation.Lovcode == p.PlanRelease)
                .WhereIf(p.OrganizationID.HasValue, o => o.Plan.OrganizationId == p.OrganizationID.Value)
                .WhereIf(searchYear.HasValue, y => y.Plan.StartYear == searchYear.Value)
                .Where(w => w.Plan.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.รายงานสถานะทางการเงินและภาระหนี้);
                var pfs = await sql.ToListAsync();

                var data = new Debt {
                    CurrentDebt = new List<AgreementModel>(),
                    GOVForeignDebtEstimationList = new List<GOVForeignDebtEstimationList>(),
                    GOVLocalDebtEstimation = new List<MiniDebtEstimation>(),
                    ORGForeignDebtEstimationList = new List<ORGForeignDebtEstimationList>(),
                    ORGLocalDebtEstimation = new List<DebtEstimation>()
                };

                

                var rate = _helper.GetCurrencyRate(p.StartYear);
                if (!rate.IsCompleted)
                {
                    ret.CloneMessage(rate.Message);
                    return ret;
                }
                #region CurrentDebt
                var ags = await DB.Agreement.Where(w => 
                w.OutStandingDebt != 0).ToListAsync();


                foreach (var ag in ags)
                {
                    var agDT = await _arg.Get(ag.AgreementId,p.StartYear);
                    var r = rate.Data.Currency.Where(w => w.CurrencyCode == agDT.Data.LoanCurrency).FirstOrDefault();
                    if (r == null)
                    {
                        ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { $"สกุลเงิน {agDT.Data.LoanCurrency}" });
                        return ret;
                    }
                    agDT.Data.OutStandingDebtTHB = agDT.Data.OutStandingDebt * r.CurrencyRate;
                    if (agDT.IsCompleted)
                    {
                        data.CurrentDebt.Add(agDT.Data);
                    }
                }
                #endregion
                foreach (var amt in pfs)
                {

                    #region ORGLocalDebtEstimation
                    if (ORGLocalDebtEstimationCode.Contains(amt.Amount.AmountTypeNavigation.Lovcode))
                    {
                        var fld = getDebtEstimation(data.ORGLocalDebtEstimation, amt.Amount.PeriodValue);
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.DAL01_ยอดหนี้คงค้าง__ประมาณการภาระหนี้ทั้งหมด___ในประเทศ_) fld.OutStandingDebt = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.DAL02_การเบิกจ่าย__ประมาณการภาระหนี้ทั้งหมด___ในประเทศ_) fld.Disbursement = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.DAL0301_การชำระคืน_เงินต้น__ประมาณการภาระหนี้ทั้งหมด___ในประเทศ_) fld.Principle = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.DAL0302_การชำระคืน__ดอกเบี้ยค่าธรรมเนียม__ประมาณการภาระหนี้ทั้งหมด___ในประเทศ_) fld.Interest = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.DAL04_กระแสเงินไหลเข้าสุทธิ__ประมาณการภาระหนี้ทั้งหมด___ในประเทศ_) fld.NetCashFlow = amt.Amount.Amount1;

                    }

                    #endregion
                    #region ORGForeignDebtEstimationList
                    if (ORGForeignDebtEstimationListCode.Contains(amt.Amount.AmountTypeNavigation.Lovcode))
                    {
                        var fld = getORGForeignDebtEstimationList(data.ORGForeignDebtEstimationList, amt.Amount.PeriodValue, amt.Amount.Currency);
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.DAF01_ยอดหนี้คงค้าง__ประมาณการภาระหนี้ทั้งหมด___ต่างประเทศ_) fld.OutStandingDebt = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.DAF02_การเบิกจ่าย__ประมาณการภาระหนี้ทั้งหมด___ต่างประเทศ_) fld.Disbursement = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.DAF0301_การชำระคืน_เงินต้น__ประมาณการภาระหนี้ทั้งหมด___ต่างประเทศ_) fld.Principle = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.DAF0302_การชำระคืน__ดอกเบี้ยค่าธรรมเนียม__ประมาณการภาระหนี้ทั้งหมด___ต่างประเทศ_) fld.Interest = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.DAF04_กระแสเงินไหลเข้าสุทธิ__ประมาณการภาระหนี้ทั้งหมด___ต่างประเทศ_) fld.NetCashFlow = amt.Amount.Amount1;

                    }
                    #endregion
                    #region GOVLocalDebtEstimation
                    if (GOVLocalDebtEstimationCode.Contains(amt.Amount.AmountTypeNavigation.Lovcode))
                    {
                        var fld = getGOVLocalDebtEstimation(data.GOVLocalDebtEstimation, amt.Amount.PeriodValue);
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.GDL01_เงินต้น__ประมาณการภาระหนี้ที่รัฐบาลต้องรับภาระ___ในประเทศ_) fld.Principle = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.GDL02_ดอกเบี้ยค่าธรรมเนียม__ประมาณการภาระหนี้ที่รัฐบาลต้องรับภาระ___ในประเทศ_) fld.Interest = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.GDL03_ค่าธรรมเนียม__ประมาณการภาระหนี้ที่รัฐบาลต้องรับภาระ___ในประเทศ_) fld.Fee = amt.Amount.Amount1;


                    }

                    #endregion
                    #region GOVForeignDebtEstimationList
                    if (GOVForeignDebtEstimationListCode.Contains(amt.Amount.AmountTypeNavigation.Lovcode))
                    {
                        var fld = getGOVForeignDebtEstimationList(data.GOVForeignDebtEstimationList, amt.Amount.PeriodValue, amt.Amount.Currency);
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.GDF01_ยอดหนี้คงค้าง__ประมาณการภาระหนี้ที่รัฐต้องรับภาระ___ต่างประเทศ_) fld.Principle = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.GDF02_ดอกเบี้ยค่าธรรมเนียม__ประมาณการภาระหนี้ที่รัฐต้องรับภาระ___ต่างประเทศ_) fld.Interest = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.GDF03_ค่าธรรมเนียม__ประมาณการภาระหนี้ที่รัฐต้องรับภาระ___ต่างประเทศ_) fld.Fee = amt.Amount.Amount1;


                    }
                    #endregion


                }
                #region Agreement
                //foreach (var agr in plan.PlanAgreement)
                //{
                //    var agree = await _arg.Get(agr.AgreementId.Value);
                //    if (agree.IsCompleted)
                //    {
                //        data.DebtSummary.CurrentDebt.Add(agree.Data);
                //    }
                //}
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
        public async Task<ReturnObject<FinancialReportModel>> GetFinPlan(long? ID)
        {
            var ret = new ReturnObject<FinancialReportModel>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var plan = await DB.Plan.Include(pt => pt.PlanTypeNavigation)
                    .Include(c => c.PlanExtend)
                    .Include(a => a.PlanAgreement)
                    .Include(att => att.PlanAttach).ThenInclude(f => f.AttachFile)
                    .Include(f => f.PlanFinance).ThenInclude(a => a.Amount).ThenInclude(amt => amt.AmountTypeNavigation)
                    .Include(d => d.Dscrnote)
                    .Where(w => w.PlanId == ID).FirstOrDefaultAsync();
                if (plan == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { "แผนงาน" });
                    return ret;
                }

                var data = new FinancialReportModel
                {
                    BalanceSheet = new List<BalanceSheet>(),
                    CashFlows = new List<CashFlow>(),
                    AttachFiles = new List<AttachFileData>(),
                    DebtSummary = new Debt {
                        CurrentDebt = new List<AgreementModel>(),
                        GOVForeignDebtEstimationList = new List<GOVForeignDebtEstimationList>(),
                        GOVLocalDebtEstimation = new List<MiniDebtEstimation>(),
                        ORGForeignDebtEstimationList = new List<ORGForeignDebtEstimationList>(),
                        ORGLocalDebtEstimation = new List<DebtEstimation>()
                    },
                    DSCRNotes = new List<DSCRNote>(),
                    FinancialRatio = new List<FinancialRatio>(),
                    IncomeStatements = new List<IncomeStatement>()
                };
                var coffer = plan.PlanExtend.Where(w => w.DataGroup == "COFFER").FirstOrDefault();
                if (coffer != null && coffer.Data != null)
                {
                    try
                    {
                        data.Coffers = JsonConvert.DeserializeObject<List<COffer>>(coffer.Data);
                    }
                    catch
                    {
                        //convert ไม่ได้ ก็ไม่เอา
                    }
                    
                }
                if (ID.HasValue)
                {


                    var head = await GetPlanHeaderByID(ServiceModels.Constants.LOVGroup.Plan_Type.รายงานสถานะทางการเงินและภาระหนี้, ID.Value);
                    if (!head.IsCompleted)
                    {
                        ret.CloneMessage(head.Message);
                        return ret;
                    }
                    data.PlanHeader = head.Data;

                }

                var rate = _helper.GetCurrencyRate(plan.StartYear);
                if (!rate.IsCompleted)
                {
                    ret.CloneMessage(rate.Message);
                    return ret;
                }

                foreach (var dsrc in plan.Dscrnote)
                {
                    data.DSCRNotes.Add(new DSCRNote
                    {
                        DSCR = dsrc.Dscr.Value,
                        DSRCNoteID = dsrc.DscrnoteId,
                        ProgressUpdate = dsrc.ProgressUpdate,
                        Reason = dsrc.Reason,
                        Solution = dsrc.Solution,
                        Year = dsrc.Year.Value
                    });
                }
                #region CurrentDebt
                var ags = await DB.Agreement.Where(w => w.OrganizationId == plan.OrganizationId &&
                w.OutStandingDebt != 0).ToListAsync();
             

                foreach (var ag in ags)
                {
                    var agDT = await _arg.Get(ag.AgreementId,plan.StartYear);
                    var r = rate.Data.Currency.Where(w => w.CurrencyCode == agDT.Data.LoanCurrency).FirstOrDefault();
                    if (r == null)
                    {
                        ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { $"สกุลเงิน {agDT.Data.LoanCurrency}" });
                        return ret;
                    }
                    agDT.Data.OutStandingDebtTHB = agDT.Data.OutStandingDebt * r.CurrencyRate;
                    if (agDT.IsCompleted)
                    {
                        data.DebtSummary.CurrentDebt.Add(agDT.Data);
                    }
                }
                #endregion
                foreach (var amt in plan.PlanFinance.OrderBy(o => o.Amount.PeriodValue))
                {
                    #region BalanceSheet
                    if (balanceSheetCode.Contains(amt.Amount.AmountTypeNavigation.Lovcode))
                    {
                        var fld = getBalanceSheet(data.BalanceSheet, amt.Amount.PeriodValue);
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.BAL01_สินทรัพย์) fld.Asset = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.BAL0101_สินทรัพย์หมุนเวียน) fld.CurrentAsset = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.BAL0102_สินทรัพยถาวร) fld.FixedAsset = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.BAL02_หนี้สินและส่วนของทุน) fld.LiabilityEquity = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.BAL0201_หนี้สิน) fld.Liability = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.BAL020101_หนี้สินหมุนเวียน) fld.CurrentLiability = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.BAL020102_หนี้สินระยะยาว) fld.LongTermLiability = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.BAL0202_ส่วนของทุน) fld.Equity = amt.Amount.Amount1;
                    }


                    #endregion
                    #region IncomingStatement
                    if (incomingStatementCode.Contains(amt.Amount.AmountTypeNavigation.Lovcode))
                    {
                        var fld = getIncomingStatement(data.IncomeStatements, amt.Amount.PeriodValue);
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.PLS01_รายได้) fld.Revenue = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.PLS02_ค่าใช้จ่าย) fld.Expense = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.PLS03_กำไรขาดทุน_จากการดำเนินงาน) fld.GlossProfit = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.PLS04_กำไรขาดทุน_ก่อนหักดอกเบี้ยภาษี_ค่าเสือม_และตัดจำหน่าย__EBITDA_) fld.EBITDA = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.PLS05_กำไรขาดทุน_สุทธิ) fld.NetProfit = amt.Amount.Amount1;

                    }


                    #endregion
                    #region CashFlow
                    if (cashFlowCode.Contains(amt.Amount.AmountTypeNavigation.Lovcode))
                    {
                        var fld = getCashFlow(data.CashFlows, amt.Amount.PeriodValue);
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.CF01_กระแสเงินจากการดำเนินงาน) fld.Operation = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.CF02_กระแสเงินสดจากการลงทุน) fld.Investing = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.CF03_กระแสเงินสดจากการจัดหาเงิน) fld.Financing = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.CF0401_เงินสดสุทธฺรับมาก__น้อย__กว่าเงินสดจ่าย) fld.NetCashFlow = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.CF0402_เงินสดคงเหลือต้นงวด) fld.CashBalanceBeginning = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.CF0403_เงินสดคงเหลือปลาดงวด) fld.CashBalanceEnding = amt.Amount.Amount1;

                    }
                    #endregion
                    #region ORGLocalDebtEstimation
                    if (ORGLocalDebtEstimationCode.Contains(amt.Amount.AmountTypeNavigation.Lovcode))
                    {
                        var fld = getDebtEstimation(data.DebtSummary.ORGLocalDebtEstimation, amt.Amount.PeriodValue);
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.DAL01_ยอดหนี้คงค้าง__ประมาณการภาระหนี้ทั้งหมด___ในประเทศ_) fld.OutStandingDebt = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.DAL02_การเบิกจ่าย__ประมาณการภาระหนี้ทั้งหมด___ในประเทศ_) fld.Disbursement = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.DAL0301_การชำระคืน_เงินต้น__ประมาณการภาระหนี้ทั้งหมด___ในประเทศ_) fld.Principle = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.DAL0302_การชำระคืน__ดอกเบี้ยค่าธรรมเนียม__ประมาณการภาระหนี้ทั้งหมด___ในประเทศ_) fld.Interest = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.DAL04_กระแสเงินไหลเข้าสุทธิ__ประมาณการภาระหนี้ทั้งหมด___ในประเทศ_) fld.NetCashFlow = amt.Amount.Amount1;

                    }

                    #endregion
                    #region ORGForeignDebtEstimationList
                    if (ORGForeignDebtEstimationListCode.Contains(amt.Amount.AmountTypeNavigation.Lovcode))
                    {
                        var fld = getORGForeignDebtEstimationList(data.DebtSummary.ORGForeignDebtEstimationList, amt.Amount.PeriodValue,amt.Amount.Currency);
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.DAF01_ยอดหนี้คงค้าง__ประมาณการภาระหนี้ทั้งหมด___ต่างประเทศ_) fld.OutStandingDebt = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.DAF02_การเบิกจ่าย__ประมาณการภาระหนี้ทั้งหมด___ต่างประเทศ_) fld.Disbursement = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.DAF0301_การชำระคืน_เงินต้น__ประมาณการภาระหนี้ทั้งหมด___ต่างประเทศ_) fld.Principle = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.DAF0302_การชำระคืน__ดอกเบี้ยค่าธรรมเนียม__ประมาณการภาระหนี้ทั้งหมด___ต่างประเทศ_) fld.Interest = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.DAF04_กระแสเงินไหลเข้าสุทธิ__ประมาณการภาระหนี้ทั้งหมด___ต่างประเทศ_) fld.NetCashFlow = amt.Amount.Amount1;

                    }
                    #endregion
                    #region GOVLocalDebtEstimation
                    if (GOVLocalDebtEstimationCode.Contains(amt.Amount.AmountTypeNavigation.Lovcode))
                    {
                        var fld = getGOVLocalDebtEstimation(data.DebtSummary.GOVLocalDebtEstimation, amt.Amount.PeriodValue);
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.GDL01_เงินต้น__ประมาณการภาระหนี้ที่รัฐบาลต้องรับภาระ___ในประเทศ_) fld.Principle = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.GDL02_ดอกเบี้ยค่าธรรมเนียม__ประมาณการภาระหนี้ที่รัฐบาลต้องรับภาระ___ในประเทศ_) fld.Interest = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.GDL03_ค่าธรรมเนียม__ประมาณการภาระหนี้ที่รัฐบาลต้องรับภาระ___ในประเทศ_) fld.Fee = amt.Amount.Amount1;
                        

                    }

                    #endregion
                    #region GOVForeignDebtEstimationList
                    if (GOVForeignDebtEstimationListCode.Contains(amt.Amount.AmountTypeNavigation.Lovcode))
                    {
                        var fld = getGOVForeignDebtEstimationList(data.DebtSummary.GOVForeignDebtEstimationList, amt.Amount.PeriodValue, amt.Amount.Currency);
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.GDF01_ยอดหนี้คงค้าง__ประมาณการภาระหนี้ที่รัฐต้องรับภาระ___ต่างประเทศ_) fld.Principle = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.GDF02_ดอกเบี้ยค่าธรรมเนียม__ประมาณการภาระหนี้ที่รัฐต้องรับภาระ___ต่างประเทศ_) fld.Interest = amt.Amount.Amount1;
                        if (amt.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Financial_Report.GDF03_ค่าธรรมเนียม__ประมาณการภาระหนี้ที่รัฐต้องรับภาระ___ต่างประเทศ_) fld.Fee = amt.Amount.Amount1;


                    }
                    #endregion
                 

                }
                #region Agreement
                foreach (var agr in plan.PlanAgreement)
                {
                    var agree = await _arg.Get(agr.AgreementId.Value, agr.Plan.StartYear);
                    if (agree.IsCompleted)
                    {
                        data.DebtSummary.CurrentDebt.Add(agree.Data);
                    }
                }
                #endregion
                #region Attach file
                foreach (var file in plan.PlanAttach)
                {
                    data.AttachFiles.Add(new AttachFileData
                    {
                        FileDetail = file?.AttachFile?.FileDetail,
                        FileExtension = file?.AttachFile?.FileExtension,
                        FileName = file?.AttachFile?.FileName,
                        FileSize = file.AttachFile.FileSize,
                        ID = file.AttachFile?.AttachFileId
                    });

                }
                #endregion
                #region finacial ratio
                foreach (var bal in data.BalanceSheet)
                {
                    var fld = getFinancialRatio(data.FinancialRatio,  bal.Year );
                    fld.Liability = bal.LongTermLiability + bal.CurrentLiability;
                    fld.Equity = bal.Equity;
                    fld.FixedAsset = bal.FixedAsset;
                    if (fld.Equity != 0)
                    {
                        fld.LERatio = fld.Liability / fld.Equity;
                    }
                    fld.Asset = bal.CurrentAsset + bal.FixedAsset;
                    if (fld.Asset != 0)
                    {
                        fld.LARatio = fld.Liability / fld.Asset;
                    }
                    fld.CurrentAsset = bal.CurrentAsset;
                    fld.CurrentLiability = bal.CurrentLiability;
                    if (fld.CurrentLiability != 0)
                    {
                        fld.CACLRatio = fld.CurrentAsset / fld.CurrentLiability;
                    }

                }


                // var agrs = await DB.AgreementTrans.Where(w => w.Agreement.OrganizationId == plan.OrganizationId
                //   && w.PostinDate.Value.Year > plan.StartYear - 543
                //   && (w.FlowTypeNavigation.ParentLov == ServiceModels.Constants.LOVGroup.Transaction_CashFlow_Type_from_GF.ชำระดอกเบี้ย
                //   || w.FlowTypeNavigation.ParentLov == ServiceModels.Constants.LOVGroup.Transaction_CashFlow_Type_from_GF.ชำระเงินต้น)
                //   && (w.Status == "ทำการผ่านรายการเสร็จแล้ว" || w.Status == "ทำเครื่องหมายเพื่อผ่านรายการ"))
                //.Select(s => new { Year = s.PostinDate.Value.Year, FType = s.FlowTypeNavigation.ParentLov, IsGuarantee = s.Agreement.IsGuarantee, Amt = s.BaseAmount }).ToListAsync();
                // var agrTrs =agrs.GroupBy(g => new {  g.Year, g.FType, gt = g.IsGuarantee })
                // .Select(s => new { Year = s.Key.Year, FType = s.Key.FType, IsGuarantee = s.Key.gt, Amt = s.Sum(sum => sum.Amt) }).ToList();


                //var sumTr = agrTrs.GroupBy(g => new { FType = g.FType, IsGuarantee = g.IsGuarantee }).Select(s => new { FType = s.Key.FType, IsGuarantee = s.Key.IsGuarantee, Amt = s.Sum(sum => sum.Amt) }).ToList();
                foreach (var fld in data.DebtSummary.GOVLocalDebtEstimation)
                {
                    var rat = getFinancialRatio(data.FinancialRatio,fld.Year);
                    rat.GovDebt += fld.Principle + fld.Interest;
                    rat.pDebtG += fld.Principle;
                    rat.iDebtG += fld.Interest;

                    rat.iDebtO -= fld.Fee;
                }
                foreach (var gfd in data.DebtSummary.GOVForeignDebtEstimationList)
                {
                    var curr = rate.Data.Currency.Where(w => w.CurrencyCode == gfd.Currency).FirstOrDefault();
                    if (curr == null)
                    {
                        ret.AddMessage(eMessage.DataIsNotFound.ToString(), "rate", eMessageType.Error, new string[] { $"อัตราแลกเปลี่ยนของ {gfd.Currency} ปี {data.PlanHeader.PlanYear}" });
                    }
                    foreach (var fld in gfd.GOVForeignDebtEstimation)
                    {
                        var rat = getFinancialRatio(data.FinancialRatio, fld.Year);
                       
                        rat.pDebtG += (fld.Principle * curr.CurrencyRate);
                        rat.iDebtG += (fld.Interest * curr.CurrencyRate);
                        rat.iDebtO -= (fld.Fee * curr.CurrencyRate);
                        rat.GovDebt = rat.pDebtG + rat.iDebtG;


                        rat.pGovDebt = rat.pDebtG;
                        rat.iGovDebt = rat.iDebtG;

                    }
                }

                foreach (var fld in data.DebtSummary.ORGLocalDebtEstimation)
                {
                    var rat = getFinancialRatio(data.FinancialRatio, fld.Year);
              
                    rat.pDebtO += fld.Principle;
                    rat.iDebtO += fld.Interest;
                  

                }
                foreach (var gfd in data.DebtSummary.ORGForeignDebtEstimationList)
                {
                    var curr = rate.Data.Currency.Where(w => w.CurrencyCode == gfd.Currency).FirstOrDefault();
                    if (curr == null)
                    {
                        ret.AddMessage(eMessage.DataIsNotFound.ToString(), "rate", eMessageType.Error, new string[] { $"อัตราแลกเปลี่ยนของ {gfd.Currency} ปี {data.PlanHeader.PlanYear}" });
                    }
                    foreach (var fld in gfd.ORGForeignDebtEstimation)
                    {
                        var rat = getFinancialRatio(data.FinancialRatio, fld.Year);

                        rat.pDebtO += (fld.Principle * curr.CurrencyRate);
                        rat.iDebtO += (fld.Interest * curr.CurrencyRate);
                    
                        
                    }
                }
                foreach (var f in data.FinancialRatio)
                {
                    f.pDebtO = f.pDebtO - f.pDebtG;
                    f.iDebtO = f.iDebtO - f.iDebtG;

                    f.pDebt = f.pDebtG + f.pDebtO;
                    f.iDebt = f.iDebtG + f.iDebtO;

                    f.Debt = f.pDebt + f.iDebt;
                }
                //decimal allInt, allPin;
                //allInt = 0;
                //allPin = 0;
                //foreach (var item in sumTr)
                //{
                //    if (item.IsGuarantee)
                //    {
                //        if (item.FType == ServiceModels.Constants.LOVGroup.Transaction_CashFlow_Type_from_GF.ชำระดอกเบี้ย)
                //        {
                //            allInt = item.Amt;
                //        }
                //        if (item.FType == ServiceModels.Constants.LOVGroup.Transaction_CashFlow_Type_from_GF.ชำระเงินต้น)
                //        {
                //            allPin = item.Amt;
                //        }
                //    }

                //}
                //foreach (var agr in agrTrs)
                //{
                //    //var fld = getFinancialRatio(data.FinancialRatio, (agr.Year + 543).ToString());
                //    var fld = data.FinancialRatio.Where(w => w.Year == (agr.Year + 543).ToString()).FirstOrDefault();
                //    if (fld == null)
                //    {
                //        continue;
                //    }

                //    fld.Debt += agr.Amt;
                //    if (agr.FType == ServiceModels.Constants.LOVGroup.Transaction_CashFlow_Type_from_GF.ชำระดอกเบี้ย)
                //    {
                //        if (agr.IsGuarantee)
                //        {
                //            fld.iDebtG += agr.Amt;
                //            fld.iDebt += agr.Amt;
                //        }
                //        else
                //        {
                //            fld.iDebtO += agr.Amt;
                //            fld.iDebt += agr.Amt;
                //        }

                //    }
                //    if (agr.FType == ServiceModels.Constants.LOVGroup.Transaction_CashFlow_Type_from_GF.ชำระเงินต้น)
                //    {
                //        if (agr.IsGuarantee)
                //        {
                //            fld.pDebtG += agr.Amt;
                //            fld.pDebt += agr.Amt;
                //        }
                //        else
                //        {
                //            fld.pDebtO += agr.Amt;
                //            fld.pDebt += agr.Amt;
                //        }

                //    }
                //    fld.pGovDebt = allPin;
                //    fld.iGovDebt = allInt;
                //    fld.GovDebt = allPin + allInt;
                //}
                foreach (var inc in data.IncomeStatements)
                {
                    var fld = getFinancialRatio(data.FinancialRatio, inc.Year);
                    fld.NetProfit = inc.NetProfit;
                    if (fld.FixedAsset != 0)
                    {
                        fld.NPFARatio = fld.NetProfit / fld.FixedAsset;
                    }
                    if (fld.Asset != 0)
                    {
                        fld.NPARatio = fld.NetProfit / fld.Asset;
                    }
                    fld.Revenue = inc.Revenue;
                    if (fld.Revenue != 0)
                    {
                        fld.NPRRatio = fld.NetProfit / fld.Revenue;
                    }
                    fld.EBIDA = inc.EBITDA;
                    var interest = await DB.Parameter.Where(w => w.Year == int.Parse(inc.Year)).FirstOrDefaultAsync();
                    if (fld.Debt != 0)
                    {
                        fld.DSRC = fld.EBIDA / Math.Abs(fld.Debt);
                    }

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

        #region Store DB to Model
        private string[] balanceSheetCode = new string[] {
            ServiceModels.Constants.LOVGroup.Financial_Report.BAL01_สินทรัพย์,
            ServiceModels.Constants.LOVGroup.Financial_Report.BAL0101_สินทรัพย์หมุนเวียน,
            ServiceModels.Constants.LOVGroup.Financial_Report.BAL0102_สินทรัพยถาวร,
            ServiceModels.Constants.LOVGroup.Financial_Report.BAL02_หนี้สินและส่วนของทุน,
            ServiceModels.Constants.LOVGroup.Financial_Report.BAL0201_หนี้สิน,
            ServiceModels.Constants.LOVGroup.Financial_Report.BAL020101_หนี้สินหมุนเวียน,
            ServiceModels.Constants.LOVGroup.Financial_Report.BAL020102_หนี้สินระยะยาว,
            ServiceModels.Constants.LOVGroup.Financial_Report.BAL0202_ส่วนของทุน,

        };
        private FinancialRatio getFinancialRatio(List<FinancialRatio> f, string year)
        {
            var ret = f.Where(w => w.Year == year).FirstOrDefault();
            if (ret == null)
            {
                ret = new FinancialRatio
                {
                    Year = year 
                };
                f.Add(ret);
            }
            return ret;
        }
        private BalanceSheet getBalanceSheet(List<BalanceSheet> b, int year)
        {
            var ret = b.Where(w => w.Year == year.ToString()).FirstOrDefault();
            if (ret == null)
            {
                ret = new BalanceSheet
                {
                    Year = year.ToString()
                };
                b.Add(ret);
            }
            return ret;
        }
        #endregion
        #region IncomingStatement
        private string[] incomingStatementCode = new string[]
        {
            ServiceModels.Constants.LOVGroup.Financial_Report.PLS01_รายได้,
            ServiceModels.Constants.LOVGroup.Financial_Report.PLS02_ค่าใช้จ่าย,
            ServiceModels.Constants.LOVGroup.Financial_Report.PLS03_กำไรขาดทุน_จากการดำเนินงาน,
            ServiceModels.Constants.LOVGroup.Financial_Report.PLS04_กำไรขาดทุน_ก่อนหักดอกเบี้ยภาษี_ค่าเสือม_และตัดจำหน่าย__EBITDA_,
            ServiceModels.Constants.LOVGroup.Financial_Report.PLS05_กำไรขาดทุน_สุทธิ,

        };
        private IncomeStatement getIncomingStatement(List<IncomeStatement> b, int year)
        {
            var ret = b.Where(w => w.Year == year.ToString()).FirstOrDefault();
            if (ret == null)
            {
                ret = new IncomeStatement
                {
                    Year = year.ToString()
                };
                b.Add(ret);
            }
            return ret;
        }
        #endregion
        #region CashFlow
        private string[] cashFlowCode = new string[]
        {
            ServiceModels.Constants.LOVGroup.Financial_Report.CF01_กระแสเงินจากการดำเนินงาน,
            ServiceModels.Constants.LOVGroup.Financial_Report.CF02_กระแสเงินสดจากการลงทุน,
            ServiceModels.Constants.LOVGroup.Financial_Report.CF03_กระแสเงินสดจากการจัดหาเงิน,
            ServiceModels.Constants.LOVGroup.Financial_Report.CF0401_เงินสดสุทธฺรับมาก__น้อย__กว่าเงินสดจ่าย,
            ServiceModels.Constants.LOVGroup.Financial_Report.CF0402_เงินสดคงเหลือต้นงวด,
            ServiceModels.Constants.LOVGroup.Financial_Report.CF0403_เงินสดคงเหลือปลาดงวด,

        };
        private CashFlow getCashFlow(List<CashFlow> b, int year)
        {
            var ret = b.Where(w => w.Year == year.ToString()).FirstOrDefault();
            if (ret == null)
            {
                ret = new CashFlow
                {
                    Year = year.ToString()
                };
                b.Add(ret);
            }
            return ret;
        }
        #endregion
        #region ORGLocalDebtEstimation
        private string[] ORGLocalDebtEstimationCode = new string[] {
            ServiceModels.Constants.LOVGroup.Financial_Report.DAL01_ยอดหนี้คงค้าง__ประมาณการภาระหนี้ทั้งหมด___ในประเทศ_,
            ServiceModels.Constants.LOVGroup.Financial_Report.DAL02_การเบิกจ่าย__ประมาณการภาระหนี้ทั้งหมด___ในประเทศ_,
            ServiceModels.Constants.LOVGroup.Financial_Report.DAL0301_การชำระคืน_เงินต้น__ประมาณการภาระหนี้ทั้งหมด___ในประเทศ_,
            ServiceModels.Constants.LOVGroup.Financial_Report.DAL0302_การชำระคืน__ดอกเบี้ยค่าธรรมเนียม__ประมาณการภาระหนี้ทั้งหมด___ในประเทศ_,
            ServiceModels.Constants.LOVGroup.Financial_Report.DAL04_กระแสเงินไหลเข้าสุทธิ__ประมาณการภาระหนี้ทั้งหมด___ในประเทศ_,
        };
        private DebtEstimation getDebtEstimation(List<DebtEstimation> b, int year)
        {
            var ret = b.Where(w => w.Year == year.ToString()).FirstOrDefault();
            if (ret == null)
            {
                ret = new DebtEstimation
                {
                    Year = year.ToString()
                };
                b.Add(ret);
            }
            return ret;
        }
        #endregion
        #region ORGForeignDebtEstimationList
        private string[] ORGForeignDebtEstimationListCode = new string[] {
            ServiceModels.Constants.LOVGroup.Financial_Report.DAF01_ยอดหนี้คงค้าง__ประมาณการภาระหนี้ทั้งหมด___ต่างประเทศ_,
            ServiceModels.Constants.LOVGroup.Financial_Report.DAF02_การเบิกจ่าย__ประมาณการภาระหนี้ทั้งหมด___ต่างประเทศ_,
            ServiceModels.Constants.LOVGroup.Financial_Report.DAF0301_การชำระคืน_เงินต้น__ประมาณการภาระหนี้ทั้งหมด___ต่างประเทศ_,
            ServiceModels.Constants.LOVGroup.Financial_Report.DAF0302_การชำระคืน__ดอกเบี้ยค่าธรรมเนียม__ประมาณการภาระหนี้ทั้งหมด___ต่างประเทศ_,
            ServiceModels.Constants.LOVGroup.Financial_Report.DAF04_กระแสเงินไหลเข้าสุทธิ__ประมาณการภาระหนี้ทั้งหมด___ต่างประเทศ_,
        };
        private DebtEstimation getORGForeignDebtEstimationList(List<ORGForeignDebtEstimationList> b, int year, string currency)
        {
            var ret = b.Where(w => w.Currency == currency).FirstOrDefault();
            if (ret == null)
            {
                ret = new ORGForeignDebtEstimationList
                {
                    Currency = currency,
                    ORGForeignDebtEstimation = new List<DebtEstimation>()
                };
                b.Add(ret);
            }
            var d =  ret.ORGForeignDebtEstimation.Where(w => w.Year == year.ToString()).FirstOrDefault();
            if (d == null)
            {
                d = new DebtEstimation
                {
                    Year = year.ToString()
                };
                ret.ORGForeignDebtEstimation.Add(d);
            }
            return d;
        }
        #endregion
        #region GOVLocalDebtEstimation
        private string[] GOVLocalDebtEstimationCode = new string[] {
            ServiceModels.Constants.LOVGroup.Financial_Report.GDL01_เงินต้น__ประมาณการภาระหนี้ที่รัฐบาลต้องรับภาระ___ในประเทศ_,
            ServiceModels.Constants.LOVGroup.Financial_Report.GDL02_ดอกเบี้ยค่าธรรมเนียม__ประมาณการภาระหนี้ที่รัฐบาลต้องรับภาระ___ในประเทศ_,
            ServiceModels.Constants.LOVGroup.Financial_Report.GDL03_ค่าธรรมเนียม__ประมาณการภาระหนี้ที่รัฐบาลต้องรับภาระ___ในประเทศ_,

        };
        private MiniDebtEstimation getGOVLocalDebtEstimation(List<MiniDebtEstimation> b, int year)
        {
            var ret = b.Where(w => w.Year == year.ToString()).FirstOrDefault();
            if (ret == null)
            {
                ret = new MiniDebtEstimation
                {
                    Year = year.ToString()
                };
                b.Add(ret);
            }
            return ret;
        }
        #endregion
        #region GOVForeignDebtEstimationList
        private string[] GOVForeignDebtEstimationListCode = new string[] {
            ServiceModels.Constants.LOVGroup.Financial_Report.GDF01_ยอดหนี้คงค้าง__ประมาณการภาระหนี้ที่รัฐต้องรับภาระ___ต่างประเทศ_,
            ServiceModels.Constants.LOVGroup.Financial_Report.GDF02_ดอกเบี้ยค่าธรรมเนียม__ประมาณการภาระหนี้ที่รัฐต้องรับภาระ___ต่างประเทศ_,
            ServiceModels.Constants.LOVGroup.Financial_Report.GDF03_ค่าธรรมเนียม__ประมาณการภาระหนี้ที่รัฐต้องรับภาระ___ต่างประเทศ_,
     
        };
        private MiniDebtEstimation getGOVForeignDebtEstimationList(List<GOVForeignDebtEstimationList> b, int year, string currency)
        {
            var ret = b.Where(w => w.Currency == currency).FirstOrDefault();
            if (ret == null)
            {
                ret = new GOVForeignDebtEstimationList
                {
                    Currency = currency,
                    GOVForeignDebtEstimation = new List<MiniDebtEstimation>()
                };
                b.Add(ret);
            }
            var d = ret.GOVForeignDebtEstimation.Where(w => w.Year == year.ToString()).FirstOrDefault();
            if (d == null)
            {
                d = new MiniDebtEstimation
                {
                    Year = year.ToString()
                };
                ret.GOVForeignDebtEstimation.Add(d);
            }
            return d;
        }
        #endregion
    }



}
