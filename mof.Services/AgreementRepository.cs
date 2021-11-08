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
    public partial class AgreementRepository : IAgreement
    {


        public MOFContext DB;

        private UserManager<ServiceModels.Identity.ApplicationUser> _user;
        private SignInManager<ServiceModels.Identity.ApplicationUser> _signin;
        private Microsoft.AspNetCore.Identity.UI.Services.IEmailSender _email;
        private IStringLocalizer<MessageLocalizer> _msglocalizer;
        private ISystemHelper _helper;
        private ICommon _com;
       // private IPlan _plan;
        //private string[] debtTrans;
        public AgreementRepository(MOFContext db, UserManager<ServiceModels.Identity.ApplicationUser> userManager,
        SignInManager<ServiceModels.Identity.ApplicationUser> signInManager,
        Microsoft.AspNetCore.Identity.UI.Services.IEmailSender email,
        IStringLocalizer<MessageLocalizer> msglocalizer,
        ISystemHelper helper ,
        ICommon com
        //IPlan plan
        )
        {
            DB = db;
            _user = userManager;
            _signin = signInManager;
            _email = email;
            _msglocalizer = msglocalizer;
            _helper = helper;
            _com = com;
             //debtTrans = new string[] {
             //       ServiceModels.Constants.LOVGroup.Transaction_CashFlow_Type_from_GF.ชำระเงินต้น,
             //       ServiceModels.Constants.LOVGroup.Transaction_CashFlow_Type_from_GF.ชำระดอกเบี้ย,
             //       ServiceModels.Constants.LOVGroup.Transaction_CashFlow_Type_from_GF.Installment,
             //   };

            //  _plan = plan;
        }

        public async Task<ReturnObject<long?>> RemovePaymentPlanFromAgreement(long agreementID, long paymentPlanID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {

                var agrPay = await DB.AgreementPaymentPlan.Where(w => w.AgreementId == agreementID && w.PaymentPlanId == paymentPlanID).FirstOrDefaultAsync();
                if (agrPay == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "agreement map", eMessageType.Error, new string[] { "" });
                    return ret;
                }

                DB.AgreementPaymentPlan.Remove(agrPay);
                DB.SaveChanges();

                ret.IsCompleted = true;

            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;

        }
        public async Task<ReturnObject<long?>> MapPaymentPlanToAgreement(long agreementID, long paymentPlanID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var agr = await DB.Agreement.Where(w => w.AgreementId == agreementID).FirstOrDefaultAsync();
                if (agr == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "agreement", eMessageType.Error, new string[] { _msglocalizer[eMessage.Agreement.ToString()] });
                    return ret;
                }
                var pact = await DB.PaymentPlan.Where(w => w.PaymentPlanId == paymentPlanID).FirstOrDefaultAsync();
                if (pact == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "restructure", eMessageType.Error, new string[] { _msglocalizer[eMessage.Restructure.ToString()] });
                    return ret;
                }
                var agrAct = await DB.AgreementPaymentPlan.Where(w => w.AgreementId == agreementID && w.PaymentPlanId == paymentPlanID).FirstOrDefaultAsync();
                if (agrAct != null)
                {
                    ret.AddMessage(eMessage.AgreementRestructureAlreadyMap.ToString(), "agreement map", eMessageType.Error, null);
                    return ret;
                }
                var map = new AgreementPaymentPlan
                {
                    AgreementId = agreementID,
                    PaymentPlanId = paymentPlanID
                };
                DB.AgreementPaymentPlan.Add(map);
                DB.SaveChanges();
                ret.Data = map.AgreementPaymentPlanId;
                ret.IsCompleted = true;

            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        public async Task<ReturnObject<long?>> RemoveActivityFromAgreement(long agreementID, long planActID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
         
                var agrAct = await DB.AgreementAct.Where(w => w.AgreementId == agreementID && w.PlanActId == planActID).FirstOrDefaultAsync();
                if (agrAct == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "agreement map", eMessageType.Error, new string[] { ""});
                    return ret;
                }
       
                DB.AgreementAct.Remove(agrAct);
                DB.SaveChanges();
           
                ret.IsCompleted = true;

            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        public async Task<ReturnObject<long?>> MapActivityToAgreement(long agreementID, long planActID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var agr = await DB.Agreement.Where(w => w.AgreementId == agreementID).FirstOrDefaultAsync();
                if (agr == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "agreement", eMessageType.Error, new string[] { _msglocalizer[eMessage.Agreement.ToString()] });
                    return ret;
                }
                var pact = await DB.PlanAct.Where(w => w.PlanActId == planActID).FirstOrDefaultAsync();
                if (pact == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "activity", eMessageType.Error, new string[] { _msglocalizer[eMessage.Activity.ToString()] });
                    return ret;
                }
                var agrAct = await DB.AgreementAct.Where(w => w.AgreementId == agreementID && w.PlanActId == planActID).FirstOrDefaultAsync();
                if (agrAct != null)
                {
                    ret.AddMessage(eMessage.AgreementActivityAlreadyMap.ToString(), "agreement map", eMessageType.Error, null  );
                    return ret;
                }
                var map = new AgreementAct
                {
                    AgreementId = agreementID,
                    PlanActId = planActID
                };
                DB.AgreementAct.Add(map);
                DB.SaveChanges();
                ret.Data = map.AgreementActId;
                ret.IsCompleted = true;

            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        private DateTime[] AgreementPeroid(int? year)
        {
            var ret = new DateTime[] { new DateTime(), new DateTime()};
            if (year.HasValue)
            {
                if (year > 2500)
                {
                    year -= 543;
                }
                ret[0] = new DateTime(year.Value -1 , 10, 1);
                ret[1] = new DateTime(year.Value , 9, 30);

            }
            else
            {
                ret[0] = new DateTime(2018, 10, 1);
                ret[1] = new DateTime(2099, 10, 1);
            }
            return ret;
        }
        public async Task<ReturnObject<AgreementModel>> Get(long id, int? year = null, long? transType = null)
        {
            var ret = new ReturnObject<AgreementModel>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
     
                var a = await DB.Agreement.Where(w => w.AgreementId == id).Select(s => new AgreementModel
                    {
                        AgreementID = s.AgreementId,
                        Counterparty = s.Counterparty,
                        Description = s.Description,
                        EndDate = s.EndDate,
                        GFTRRefCode = s.GftrrefCode,
                        InterestRate = s.InterestRate,
                        LoanAge = s.LoanAge,
                        Organization = new BasicData
                        {
                            Code = s.Organization.OrganizationCode,
                            ID = s.Organization.OrganizationId,
                            Description = s.Organization.OrganizationThname
                        },
                        ReferenceCode = s.ReferenceCode,
                        SignDate = s.SignDate,
                        StartDate = s.StartDate,
                        IsGuarantee = s.IsGuarantee,
                        LoanAmonthTHB = s.LoanAmountThb,
                        LoanAmount = s.LoanAmount,
                        LoanCurrency = s.LoanCurrency,
                        OutStandingDebt = s.OutStandingDebt,
                        OutStandingDebtTHB = s.OutStandingDebtThb,
                        SourceType = s.SourceType,
                    DebtSubType = s.DebtSubType,
                    DebtType = s.DebtType,
                    InterestFomula = s.InterestFormula,
                    MasterAgreement = s.MasterAgreement,
                    IncomingDueAmount = s.IncomingDueAmount,
                    IncomingDueDate = s.IncomingDueDate, 
                    IsMapped = (s.AgreementAct.FirstOrDefault() != null) || (s.AgreementPaymentPlan.FirstOrDefault() != null)

                }
                ).FirstOrDefaultAsync();
               
  
                if (a == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "agreement", eMessageType.Error, new string[] { _msglocalizer[eMessage.Agreement.ToString()] });
                    return ret;

                }

                //var trans = await DB.AgreementTrans.Where(w => w.AgreementId == a.AgreementID
                //&& w.PostinDate >= DateTime.Now.Date
                //&& CommonConstants.FtypeGroupPaid.Contains(w.FlowTypeNavigation.ParentLov)
                //&& (w.Status == "ทำการผ่านรายการเสร็จแล้ว" || w.Status == "ทำเครื่องหมายเพื่อผ่านรายการ")
                //).OrderBy(o => o.PostinDate)
                //.FirstOrDefaultAsync();
                //if (trans != null)
                //{
                //    a.IncomingDueAmount = Math.Abs(trans.Amount);
                //    a.IncomingDueDate = trans.PostinDate.Value;
                //    a.LoanCurrency = trans.CurrencyCode;

                //}
                string[] filterTrans = new string[] { "" };
              
                if (transType.HasValue)
                {
                    var lov = await DB.CeLov.Where(w => w.Lovkey == transType.Value).FirstOrDefaultAsync();
                    if (lov != null)
                    {
                        filterTrans = new string[] { lov.Lovcode };
                        a.TransactionType = new BasicData { Code = lov.Lovcode, Description = lov.Lovvalue, ID = lov.Lovkey };
                    }
                } else
                {
                    filterTrans = CommonConstants.FtypeGroupPaid;
                }
                var period = AgreementPeroid(year);
                var allTrans = await DB.AgreementTrans.Where(w => w.AgreementId == a.AgreementID
                && (w.Status == "ทำการผ่านรายการเสร็จแล้ว" || w.Status == "ทำเครื่องหมายเพื่อผ่านรายการ" || w.Status == "ระงับการผ่านรายการ")
                 )
                    .Include(i => i.FlowTypeNavigation)
                    .OrderBy(o => o.PostinDate).ToListAsync();

                //var trans = await DB.AgreementTrans.Where(w => w.AgreementId == a.AgreementID
                //&& (w.PostinDate >= period[0]  && w.PostinDate <= period[1])
                //&& filterTrans.Contains(w.FlowTypeNavigation.ParentLov)
                //&& (w.Status == "ทำการผ่านรายการเสร็จแล้ว" || w.Status == "ทำเครื่องหมายเพื่อผ่านรายการ" || w.Status == "ระงับการผ่านรายการ")
                //).OrderBy(o => o.PostinDate).ToListAsync();
                ////.FirstOrDefaultAsync();
                //if (trans.Count > 0)
                //{
                //    a.IncomingDueAmount = 0;
                //}
                //foreach (var tr in trans)
                //{
                //    a.IncomingDueAmount += Math.Abs(tr.Amount);
                //    a.IncomingDueDate = tr.PostinDate.Value;
                //    a.LoanCurrency = tr.CurrencyCode;
                //    a.PostingDate = tr.PostinDate.Value;
                //}
                //kenghot 2021-3-15
                decimal IncomingDueAmount = 0;
                decimal OutStandingDebtTHB = 0;
                decimal OutStandingDebt  = 0;
                int refYear = !year.HasValue ? 2599 : year.Value - 543;
                DateTime refDate = new DateTime(refYear, 3, 1, 0, 0, 0);

                foreach (var tr in allTrans)
                {
                    if (tr.PostinDate <= refDate)
                    {
                        if (tr.FlowTypeNavigation.ParentLov == ServiceModels.Constants.LOVGroup.Transaction_CashFlow_Type_from_GF.เบิกเงินกู้)
                        {
                            OutStandingDebtTHB += Math.Abs(tr.Amount);
                            OutStandingDebt += Math.Abs(tr.BaseAmount);
                        }
                        if (tr.FlowTypeNavigation.ParentLov == ServiceModels.Constants.LOVGroup.Transaction_CashFlow_Type_from_GF.ชำระเงินต้น ||
                            tr.FlowTypeNavigation.ParentLov == ServiceModels.Constants.LOVGroup.Transaction_CashFlow_Type_from_GF.Installment)
                        {
                            OutStandingDebtTHB -= Math.Abs(tr.Amount);
                            OutStandingDebt -= Math.Abs(tr.BaseAmount);
                        }
                    }
                    if (tr.PostinDate >= period[0] && tr.PostinDate <= period[1] && filterTrans.Contains(tr.FlowTypeNavigation.ParentLov))
                    {
                        IncomingDueAmount += Math.Abs(tr.Amount);
                        a.IncomingDueDate = tr.PostinDate.Value;
                        a.LoanCurrency = tr.CurrencyCode;
                        a.PostingDate = tr.PostinDate.Value;
                    }
                }
                if (IncomingDueAmount > 0)
                {
                    a.IncomingDueAmount = IncomingDueAmount;
                }
                if (OutStandingDebt != 0)
                {
                    a.OutStandingDebt = OutStandingDebt;
                    a.OutStandingDebtTHB = OutStandingDebtTHB;
                }
                ret.Data = a;
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }

        public async Task<ReturnObject<long?>> Modify(AgreementModel a, bool isCreate, string userID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                Agreement aNew;
                if (isCreate)
                {
                    aNew = await DB.Agreement.Where(w => w.GftrrefCode == a.GFTRRefCode).FirstOrDefaultAsync();
                    if (aNew != null)
                    {
                        ret.AddMessage(eMessage.DataIsAlreadyExist.ToString(), "already exist", eMessageType.Error, new string[] { _msglocalizer[eMessage.Agreement.ToString()] });
                        return ret;
                    }
                    aNew = new Agreement();
             
                    DB.Agreement.Add(aNew);
                }
                else
                {
                    aNew = await DB.Agreement.Include(i => i.Organization)
                       .Where(w => w.AgreementId == a.AgreementID).FirstOrDefaultAsync();
                    if (aNew == null)
                    {
                        ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { _msglocalizer[eMessage.Agreement.ToString()] });
                        return ret;
                    }
                    var o = await DB.Agreement.Where(w => w.AgreementId == a.AgreementID && w.GftrrefCode != a.GFTRRefCode).FirstOrDefaultAsync();
                    if (o != null)
                    {
                        ret.AddMessage(eMessage.DataIsAlreadyExist.ToString(), "already exist", eMessageType.Error, new string[] { _msglocalizer[eMessage.Agreement.ToString()] });
                        return ret;
                    }
                }
                var org = await DB.Organization.Where(w => w.OrganizationCode == a.Organization.Code).FirstOrDefaultAsync();
                if (org == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { _msglocalizer[eMessage.Organization.ToString()] });
                    return ret;
                }
           
                aNew.Counterparty = a.Counterparty;
                aNew.Description = a.Description;
                aNew.EndDate = a.EndDate;
                aNew.GftrrefCode = a.GFTRRefCode;
                aNew.IncomingDueAmount = a.IncomingDueAmount;
                aNew.IncomingDueDate = a.IncomingDueDate;
                aNew.InterestRate = a.InterestRate;
                aNew.LoanAge = a.LoanAge;
                aNew.LoanAmount = a.LoanAmount;
                aNew.OrganizationId = a.Organization.ID.Value;
                aNew.ReferenceCode = a.ReferenceCode;
                aNew.SignDate = a.SignDate;
                aNew.StartDate = a.StartDate;
                aNew.IsGuarantee = a.IsGuarantee;
                aNew.LoanAmountThb = a.LoanAmonthTHB;
                aNew.LoanAmount = a.LoanAmount;
                aNew.LoanCurrency = a.LoanCurrency;
                aNew.OutStandingDebt = a.OutStandingDebt;
                aNew.OutStandingDebtThb = a.OutStandingDebtTHB;
                aNew.SourceType = a.SourceType;

                DB.SaveChanges();
                ret.Data = aNew.AgreementId;
                ret.IsCompleted = true;


            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        public async Task<ReturnList<AgreementModel>> List(AgreementListParameter p)
        {
            var ret = new ReturnList<AgreementModel>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                bool isGov = false;
                if (p.OrganizationID.HasValue)
                {
                    var org = await DB.Organization.Include(t => t.OrgtypeNavigation).Where(w => w.OrganizationId == p.OrganizationID.Value).FirstOrDefaultAsync();
                    if (org.OrgtypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.ประเภทหน่วยงาน.ส่วนราชการ__กระทรวง_ทบวง_กรม__)
                    {
                        isGov = true;
                    }
                }
                string[] filterTrans = new string[] { "" };
                //var lov = await DB.CeLov.Where(w => w.Lovcode == p.TransTypeCode && w.LovgroupCode == ServiceModels.Constants.LOVGroup.Transaction_CashFlow_Type_from_GF._LOVGroupCode).FirstOrDefaultAsync();
              
                if (!string.IsNullOrEmpty( p.TransTypeCode))
                {
                    
                    filterTrans = new string[] { p.TransTypeCode };
                }
                else
                {
                    filterTrans = CommonConstants.FtypeGroupPaid;
                }
                var ttype = new string[] { "10D", "11D", "12D" };
                IQueryable<AgreementModel> tmp;
                var period = AgreementPeroid(p.BudgetYear);
              //  tmp = DB.Agreement
              //  .WhereIf(p.OrganizationID.HasValue, w => w.OrganizationId == p.OrganizationID)
              //  .WhereIf(p.AgreementID.HasValue, w => w.AgreementId == p.AgreementID)
              //  .WhereIf(!string.IsNullOrEmpty(p.Paging.SearchText), t => t.Description.Contains(p.Paging.SearchText) || t.GftrrefCode.Contains(p.Paging.SearchText))
              //  .WhereIf(isGov, g => !ttype.Contains(g.Ttyp) || string.IsNullOrEmpty(g.Ttyp))
              //  .Where(w => w.IncomingDueDate > DateTime.Now.Date || (w.AgreementTrans.Where(tr => (tr.PostinDate >= period[0] && tr.PostinDate <= period[1])
              //&& filterTrans.Contains(tr.FlowTypeNavigation.ParentLov)
              //&& (tr.Status == "ทำการผ่านรายการเสร็จแล้ว" || tr.Status == "ทำเครื่องหมายเพื่อผ่านรายการ" || tr.Status == "ระงับการผ่านรายการ")
              //    ).OrderBy(op => op.PostinDate).FirstOrDefault() != null))
              // .Select(s => new AgreementModel
              // {
              //     AgreementID = s.AgreementId,
              //    // PostingDate = (DateTime)s.AgreementTrans.Where(tr => tr.PostinDate >= DateTime.Now.Date
              //    // && CommonConstants.FtypeGroupPaid.Contains(tr.FlowTypeNavigation.ParentLov)
              //    // && (tr.Status == "ทำการผ่านรายการเสร็จแล้ว" || tr.Status == "ทำเครื่องหมายเพื่อผ่านรายการ" || tr.Status == "ระงับการผ่านรายการ")
              //    //).OrderBy(op => op.PostinDate).FirstOrDefault().PostinDate
              // });
               tmp = DB.AgreementTrans
                 .WhereIf(p.OrganizationID.HasValue, w => w.Agreement.OrganizationId == p.OrganizationID)
                 .WhereIf(p.AgreementID.HasValue, w => w.Agreement.AgreementId == p.AgreementID)
                 .WhereIf(!string.IsNullOrEmpty(p.Paging.SearchText), t => t.Agreement.Description.Contains(p.Paging.SearchText) || t.Agreement.GftrrefCode.Contains(p.Paging.SearchText))
                 .WhereIf(isGov, g => !ttype.Contains(g.Agreement.Ttyp) || string.IsNullOrEmpty(g.Agreement.Ttyp))
                 .Where(w => w.PostinDate >= period[0] && w.PostinDate <= period[1]
                 && filterTrans.Contains(w.FlowTypeNavigation.ParentLov)
                 && (w.Status == "ทำการผ่านรายการเสร็จแล้ว" || w.Status == "ทำเครื่องหมายเพื่อผ่านรายการ" || w.Status == "ระงับการผ่านรายการ"))
                 .GroupBy(g => new { g.Agreement.AgreementId, g.FlowTypeNavigation.ParentLov })
                 .Select(s => new AgreementModel { 
                     AgreementID = s.Key.AgreementId,
                     IncomingDueAmount = s.Sum(sum => sum.Amount),
                     IncomingDueDate = s.Max(max => max.PostinDate.Value),
                     TransactionType = new BasicData { Code = s.Key.ParentLov}
                 }).OrderBy(o => o.AgreementID) ;


                ret.TotalRow = await tmp.Select(s => s.AgreementID).CountAsync();
                ret.PageNo = p.Paging.PageNo;
                ret.PageSize = p.Paging.PageSize;

                IQueryable<AgreementModel> sel;

                sel = tmp.PageBy(b => b.PostingDate, p.Paging.PageNo, p.Paging.PageSize,false);
     

                var agrs = await sel.ToListAsync();
                //for (var i = 0; i < agrs.Count(); i++)
                //{
                //    var a = await Get(agrs[i].AgreementID, p.BudgetYear,lov?.Lovkey);
                //    if (a.IsCompleted)
                //    {
                //        agrs[i] = a.Data;
                //    }
                //}
                long oldID = 0;
                AgreementModel agr = new AgreementModel();
                var glov = _helper.GetLOVByGroup(ServiceModels.Constants.LOVGroup.Transaction_CashFlow_Type_from_GF._LOVGroupCode);
                for (var i = 0; i < agrs.Count(); i++) {
                    var agrRec = agrs[i];
                    if (agrRec.AgreementID != oldID)
                    {
                        agr = await DB.Agreement.Where(w => w.AgreementId == agrRec.AgreementID).Select(s => new AgreementModel
                        {
                            AgreementID = s.AgreementId,
                            Counterparty = s.Counterparty,
                            Description = s.Description,
                            EndDate = s.EndDate,
                            GFTRRefCode = s.GftrrefCode,
                            InterestRate = s.InterestRate,
                            LoanAge = s.LoanAge,
                            Organization = new BasicData
                            {
                                Code = s.Organization.OrganizationCode,
                                ID = s.Organization.OrganizationId,
                                Description = s.Organization.OrganizationThname
                            },
                            ReferenceCode = s.ReferenceCode,
                            SignDate = s.SignDate,
                            StartDate = s.StartDate,
                            IsGuarantee = s.IsGuarantee,
                            LoanAmonthTHB = s.LoanAmountThb,
                            LoanAmount = s.LoanAmount,
                            LoanCurrency = s.LoanCurrency,
                            OutStandingDebt = s.OutStandingDebt,
                            OutStandingDebtTHB = s.OutStandingDebtThb,
                            SourceType = s.SourceType,
                            DebtSubType = s.DebtSubType,
                            DebtType = s.DebtType,
                            InterestFomula = s.InterestFormula,
                            MasterAgreement = s.MasterAgreement,
                            IncomingDueAmount = s.IncomingDueAmount,
                            IncomingDueDate =  s.IncomingDueDate,
                            IsMapped = (s.AgreementAct.FirstOrDefault() != null) || (s.AgreementPaymentPlan.FirstOrDefault() != null)

                        }
                        ).FirstOrDefaultAsync();
                         oldID = agrRec.AgreementID;
                    }

                    agrRec.Counterparty = agr.Counterparty;
                    agrRec.Description = agr.Description;
                    agrRec.EndDate = agr.EndDate;
                    agrRec.GFTRRefCode = agr.GFTRRefCode;
                    agrRec.InterestRate = agr.InterestRate;
                    agrRec.LoanAge = agr.LoanAge;
                    agrRec.Organization = new BasicData
                            {
                                Code = agr.Organization.Code,
                                ID = agr.Organization.ID,
                                Description = agr.Organization.Description
                            };
                    agrRec.ReferenceCode = agr.ReferenceCode;
                    agrRec.SignDate = agr.SignDate;
                    agrRec.StartDate = agr.StartDate;
                    agrRec.IsGuarantee = agr.IsGuarantee;
                    agrRec.LoanAmonthTHB = agr.LoanAmonthTHB ;
                    agrRec.LoanAmount = agr.LoanAmount;
                    agrRec.LoanCurrency = agr.LoanCurrency;
                    agrRec.OutStandingDebt = agr.OutStandingDebt;
                    agrRec.OutStandingDebtTHB = agr.OutStandingDebtTHB;
                    agrRec.SourceType = agr.SourceType;
                    agrRec.DebtSubType = agr.DebtSubType;
                    agrRec.DebtType = agr.DebtType;
                    agrRec.InterestFomula = agr.InterestFomula;
                    agrRec.MasterAgreement = agr.MasterAgreement;
                    agrRec.IsMapped = agr.IsMapped;
                    var lov = glov.Data.Where(w => w.LOVCode == agrRec.TransactionType.Code).FirstOrDefault();
                    if (lov !=null)
                    {
                        agrRec.TransactionType = new BasicData { Code = lov.LOVCode, ID = lov.LOVKey, Description = lov.LOVValue };
                    }

                }
                ret.Data = agrs;
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }

        public async Task<ReturnObject<AgreementMappingList>> GetAgreementMappingList(long agreementID)
        {
            var ret = new ReturnObject<AgreementMappingList>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                ret.Data = new AgreementMappingList
                {
                    AgreementActiviteMaps = new List<NewDebtPlanActList>(),
                    AgreementRestructureMaps = new List<ExistPlanAgreementList>()
                };

                var agacts = await DB.AgreementAct.Where(w => w.AgreementId == agreementID).ToListAsync();
                foreach (var agact in agacts)
                {
                    var pacts = await DB.PlanAct
                         .Include(pl => pl.PlanProj.Plan)
                         .Include(pj => pj.PlanProj.Project)
                         .Include(a => a.PlanActAmount).ThenInclude(amt => amt.Amount).ThenInclude(at => at.AmountTypeNavigation)
                         .Where(w => w.PlanActId == agact.PlanActId).ToListAsync();
                    var acts = await _com.GetNewDebtActList(pacts);
                    if (acts.IsCompleted)
                    {
                        ret.Data.AgreementActiviteMaps.AddRange(acts.Data);
                    }
                    else
                    {
                        ret.CloneMessage(acts.Message);
                        return ret;
                    }
                }
                var agpays = await DB.AgreementPaymentPlan.Where(w => w.AgreementId == agreementID).ToListAsync();
                foreach (var agpay in agpays)
                {
                    var pe = await DB.PlanExist.Include(pt => pt.Plan.PlanTypeNavigation)
                    .Include(ea => ea.PlanExistAgreement).ThenInclude(ag => ag.Agreement)
                    .Include(pay => pay.PaymentPlan).ThenInclude(dbt => dbt.DebtPaymentPlanTypeNavigation)
                    .Include(pay => pay.PaymentPlan).ThenInclude(py => py.PaymentSourceNavigation)
                    .Include(pay => pay.PaymentPlan).ThenInclude(dp => dp.DebtPayAmt).ThenInclude(pamt => pamt.PlanAmountNavigation)
                    .Where(w => w.PaymentPlan.Where(pp => pp.PaymentPlanId == agpay.PaymentPlanId).Count() > 0).ToListAsync();
                   
                    foreach (var clearpe in pe)
                    {
                        var delpps = new List<PaymentPlan>();
                        foreach (var clearpp in clearpe.PaymentPlan)
                        {
                            if (clearpp.PaymentPlanId != agpay.PaymentPlanId)
                            {
                                delpps.Add(clearpp);
                            }
                           
                           
                        }
                        foreach (var del in delpps)
                        {
                            clearpe.PaymentPlan.Remove(del);
                         }
                    }
                    var pays = await _com.GetPlanAgreementList(pe);
                    if (pays.IsCompleted)
                    {
                        ret.Data.AgreementRestructureMaps.AddRange(pays.Data);
                    }
                    else
                    {
                        ret.CloneMessage(pays.Message);
                        return ret;
                    }
                }

                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        public async Task<ReturnList<AgreementTransModel>> GetAgreementTrans(AgreementListParameter p)
        {
            var ret = new ReturnList<AgreementTransModel>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var sql =  DB.AgreementTrans
                    .Include(a => a.Agreement)
                    .WhereIf(p.AgreementID.HasValue, w => w.AgreementId == p.AgreementID)
                    .WhereIf(p.OrganizationID.HasValue, o => o.Agreement.OrganizationId == p.OrganizationID)
                    .WhereIf(!string.IsNullOrEmpty(p.Paging.SearchText), txt => txt.Agreement.Description.Contains(p.Paging.SearchText))
                    .Where(w => w.PostinDate >= DateTime.Now.Date && w.PostinDate <= DateTime.Now.Date.AddYears(2)
                     && CommonConstants.FtypeGroupPaid.Contains(w.FlowTypeNavigation.ParentLov)
                     && (w.Status == "ทำการผ่านรายการเสร็จแล้ว" || w.Status == "ทำเครื่องหมายเพื่อผ่านรายการ"))
                    .PageBy(at => at.PostinDate, p.Paging.PageNo, p.Paging.PageSize);

                ret.TotalRow =  await sql.CountAsync();
                ret.PageNo = p.Paging.PageNo;
                ret.PageSize = p.Paging.PageSize;
                var agrs = await sql.Select(s => new AgreementTransModel
                {
                    Agreement = new AgreementModel
                    {
                        AgreementID = s.AgreementId.Value,
                        Counterparty = s.Agreement.Counterparty,
                        Description = s.Agreement.Description,
                        EndDate = s.Agreement.EndDate,
                        GFTRRefCode = s.Agreement.GftrrefCode,
                        IncomingDueAmount = s.Agreement.IncomingDueAmount,
                        IncomingDueDate = s.Agreement.IncomingDueDate,
                        ReferenceCode = s.Agreement.ReferenceCode,
                        MasterAgreement = s.Agreement.MasterAgreement,
                        InterestFomula = s.Agreement.InterestFormula,
                        InterestRate = s.Agreement.InterestRate,
                        DebtSubType = s.Agreement.DebtSubType,
                        DebtType = s.Agreement.DebtType
                    },
                    Amount = s.Amount,
                    FlowType = s.FlowTypeNavigation.Lovvalue,
                    FlowTypeCode = s.FlowTypeNavigation.Lovcode,
                    PostingDate = s.PostinDate.Value,
                    AmountTHB = s.BaseAmount,
                    Curr = s.CurrencyCode,
                    Description = s.Description,
                    GFTRRefCode = s.GftrrefCode,
                    Status = s.Status,
                    IsMapped = (s.Agreement.AgreementAct.FirstOrDefault() != null) || (s.Agreement.AgreementPaymentPlan.FirstOrDefault() != null)
                }).ToListAsync();
                ret.Data = agrs;
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
