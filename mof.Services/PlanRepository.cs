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

namespace mof.Services
{
    public partial class PlanRepository : IPlan
    {
        public MOFContext DB;
 
        private UserManager<ServiceModels.Identity.ApplicationUser> _user;
        private SignInManager<ServiceModels.Identity.ApplicationUser> _signin;
        private Microsoft.AspNetCore.Identity.UI.Services.IEmailSender _email;
        private IStringLocalizer<MessageLocalizer> _msglocalizer;
        private ISystemHelper _helper;
        private IAgreement _arg;
        private ICommon _com;
        public PlanRepository(MOFContext db, UserManager<ServiceModels.Identity.ApplicationUser> userManager,
            SignInManager<ServiceModels.Identity.ApplicationUser> signInManager,
            Microsoft.AspNetCore.Identity.UI.Services.IEmailSender email,
            IStringLocalizer<MessageLocalizer> msglocalizer,
            ISystemHelper helper,
            IAgreement arg,
            ICommon com
        
        )
        {
            DB = db;
            _user = userManager;
            _signin = signInManager;
            _email = email;
            _msglocalizer = msglocalizer;
            _helper = helper;
            _arg = arg;
            _com = com;
        }

        public async Task<ReturnObject<long?>> CreatePlan(string UserID, string PlanType, CreatePlanParameter p)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var u = _helper.UserValidate(UserID);
                if (!u.IsCompleted)
                {
                    ret.CloneMessage(u.Message);
                    return ret;
                }
                var pt = _helper.LOVCodeValidate(PlanType, ServiceModels.Constants.LOVGroup.Plan_Type._LOVGroupCode,null); 
                if (!pt.IsCompleted)
                {
                    ret.CloneMessage(pt.Message);
                    return ret;
                }
                var pr = _helper.LOVCodeValidate(p.PlanRelease, ServiceModels.Constants.LOVGroup.Plan_Release._LOVGroupCode, null);
                if (!pr.IsCompleted)
                {
                    ret.CloneMessage(pr.Message);
                    return ret;
                }
                var org = DB.Organization.Where(w => w.OrganizationId == p.OrganizationID).FirstOrDefault();
                if (org == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { "หน่วยงาน" });
                    return ret;
                }
                var plan = await DB.Plan.Where(w => w.OrganizationId == p.OrganizationID && w.StartYear == p.StartYear && w.PlanRelease == pr.Data.LOVKey
                && w.PlanTypeNavigation.Lovcode == PlanType && w.Month == p.Month).FirstOrDefaultAsync();
                if (plan != null)
                {
                    ret.AddMessage(eMessage.DataIsAlreadyExist.ToString(), "already exist", eMessageType.Error, new string[] { "แผนงาน" });
                    return ret;
                }
                // end of validation
                var newP = new Plan
                {

                    OrganizationId = p.OrganizationID,
                    PlanRelease = pr.Data.LOVKey,
                    PlanType = pt.Data.LOVKey,
                    StartYear = p.StartYear,
                    Month = p.Month,
                    
                };
                var newLog = new DataLog { LogDt = DateTime.Now, LogType = "C", TableName = "Plan", UserId = UserID, TableKey = -1 };
                newP.DataLogNavigation = newLog;
                newP.PlanCode = DateTime.Now.Ticks.ToString();
                DB.Plan.Add(newP);
                await DB.SaveChangesAsync();
                ret.Data = newP.PlanId;
                newP.PlanCode = PlanType + newP.PlanId.ToString().PadLeft(10, '0');
                newLog.TableKey = newP.PlanId;
                ReturnObject<LOV> chk;
                if (PlanType == ServiceModels.Constants.LOVGroup.Plan_Type.ข้อเสนอแผนบริหารหนี้)
                {
                    chk = _helper.LOVCodeValidate(ServiceModels.Constants.LOVGroup.Proposal_Status.จัดทำข้อเสนอแผนฯ, ServiceModels.Constants.LOVGroup.Proposal_Status._LOVGroupCode , null);
                }else
                {
                    chk = _helper.LOVCodeValidate(ServiceModels.Constants.LOVGroup.Plan_of_Proposal_Status.สร้างใหม่, ServiceModels.Constants.LOVGroup.Plan_of_Proposal_Status._LOVGroupCode, null);
                }
                long? status = null;
 
                    if (!chk.IsCompleted)
                    {
                        ret.CloneMessage(chk.Message);
                        return ret;
                    }
                    status = chk.Data.LOVKey;
               
                var log = new DataLog
                {
                    LogDt = DateTime.Now,
                    LogStatus = status,
                    LogType = "U",
                    Remark = "",
                    TableName = "PlanProposalStatus",
                    UserId = UserID
                };
                newP.ProposalStatusNavigation = log;
                await DB.SaveChangesAsync();

                log.TableKey = newP.PlanId;
                await DB.SaveChangesAsync();
                if (PlanType == ServiceModels.Constants.LOVGroup.Plan_Type.แผนก่อหนี้ใหม่)
                {
                    await DuplicateNewDebtPlan(newP.PlanId, p);
                }
                if (PlanType == ServiceModels.Constants.LOVGroup.Plan_Type.แผนบริหารหนี้เดิม)
                {
                    await DuplicateExistPlan(newP.PlanId, p);
                }
                ret.IsCompleted = true;

            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }

        public async Task<ReturnList<PlanHeader>> GetPlanList(string PlanType , PlanListParameter p, long? PlanID)
        {
            var ret = new ReturnList<PlanHeader>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                long? status = null;
                ReturnObject<LOV> chk;
                if (!string.IsNullOrEmpty(p.PlanStatus))
                {
                    if (PlanType == ServiceModels.Constants.LOVGroup.Plan_Type.ข้อเสนอแผนบริหารหนี้)
                    {
                        chk = _helper.LOVCodeValidate(p.PlanStatus, ServiceModels.Constants.LOVGroup.Proposal_Status._LOVGroupCode, null);
                    }
                    else
                    {
                        chk = _helper.LOVCodeValidate(p.PlanStatus, ServiceModels.Constants.LOVGroup.Plan_of_Proposal_Status._LOVGroupCode, null);
                    }
                    
                    if (!chk.IsCompleted)
                    {
                        ret.CloneMessage(chk.Message);
                        return ret;
                    }
                    status = chk.Data.LOVKey;
                }
                long? release = null;
                ReturnObject<LOV> chkRelease;
                if (!string.IsNullOrEmpty(p.PlanRelease))
                {

                    chkRelease = _helper.LOVCodeValidate(p.PlanRelease, ServiceModels.Constants.LOVGroup.Plan_Release._LOVGroupCode, null);
 
                    if (!chkRelease.IsCompleted)
                    {
                        ret.CloneMessage(chkRelease.Message);
                        return ret;
                    }
                    release = chkRelease.Data.LOVKey;
                }

                IQueryable<Plan> tmp;
                if (PlanID.HasValue)
                {
                    tmp = DB.Plan.Where(w => w.PlanId == PlanID);
                }else
                {
                    tmp = DB.Plan
                    .WhereIf(!string.IsNullOrEmpty(PlanType), pt => pt.PlanTypeNavigation.Lovcode == PlanType)
                    .WhereIf(p.OrganizationID.HasValue, w => w.OrganizationId == p.OrganizationID)
                    .WhereIf(p.Year.HasValue, y => y.StartYear == p.Year)
                    .WhereIf(status.HasValue, s => s.ProposalStatusNavigation.LogStatus == status)
                    .WhereIf(release.HasValue, r => r.PlanRelease == release.Value)
                    .WhereIf(!string.IsNullOrEmpty(p.Paging.SearchText), st => st.Organization.OrganizationThname.Contains(p.Paging.SearchText));
                }


                ret.TotalRow = await tmp.Select(s => s.PlanId).CountAsync();
                ret.PageNo = p.Paging.PageNo;
                ret.PageSize = p.Paging.PageSize;
                //todo Planstatus from proposal
                var plans = await tmp.PageBy(b => b.PlanCode, p.Paging.PageNo, p.Paging.PageSize)
                     .Join(DB.Users, pl => pl.DataLogNavigation.UserId, u => u.Id, (pl, u) => new { pl, u })
                     .GroupJoin(DB.Users, pl => pl.pl.ProposalStatusNavigation.UserId, upp => upp.Id, (pp, upp) => new { pp, upp })
                             .Select(s => new PlanHeader
                             {
                                 ChangeLog = new LogData
                                 {
                                     Action = s.pp.pl.DataLogNavigation.LogType,
                                     ActionTime = s.pp.pl.DataLogNavigation.LogDt,
                                     ActionType = "Data",
                                     ID = s.pp.pl.DataLogNavigation.LogId,
                                     UserID = s.pp.pl.DataLogNavigation.UserId,
                                     UserName = s.pp.u.TfirstName + " " + s.pp.u.TlastName
                                 },
                                 Organization = new BasicData { Code = s.pp.pl.Organization.OrganizationCode,
                                     Description = s.pp.pl.Organization.OrganizationThname,
                                     ID = s.pp.pl.OrganizationId
                                 },
                                 PlanCode = s.pp.pl.PlanCode,
                                 PlanID = s.pp.pl.PlanId,
                                 PlanRelease = s.pp.pl.PlanReleaseNavigation.Lovvalue,

                                 PlanYear = s.pp.pl.StartYear,
                                 PlanYearTxt = s.pp.pl.StartYear.ToString(),
                                 Proposals = s.pp.pl.ProposalPlanPlan.Select(pps => new BasicData { Code = pps.Proposal.PlanCode, ID = pps.Proposal.PlanId }).ToList(),
                                 PlanStatusLog = (s.pp.pl.ProposalStatus.HasValue) ? new LogData {
                                     Action = s.pp.pl.ProposalStatusNavigation.LogStatusNavigation.Lovcode,
                                     ActionType = s.pp.pl.ProposalStatusNavigation.LogStatusNavigation.Lovvalue,
                                     ActionTime = s.pp.pl.ProposalStatusNavigation.LogDt,
                                     ID = s.pp.pl.ProposalStatus.Value,
                                     Remark = s.pp.pl.ProposalStatusNavigation.Remark,
                                     UserID = s.pp.pl.ProposalStatusNavigation.UserId,
                                     UserName = s.upp.FirstOrDefault().TfirstName + " " + s.upp.FirstOrDefault().TlastName
                                 } : new LogData(),
                                 PlanStatus = s.pp.pl.ProposalStatusNavigation.LogStatusNavigation.Lovvalue,
                                 Month = (s.pp.pl.Month.HasValue) ? s.pp.pl.Month.Value : (int?)null,
                                 ReviewComment = (s.pp.pl.ProposalStatus.HasValue) ? new ReviewComment
                                 {
                                     Comment = s.pp.pl.ProposalStatusNavigation.Remark,
                                     ResultStatus = s.pp.pl.ProposalStatusNavigation.LogStatusNavigation.Lovcode,
                                     ResultDescription = s.pp.pl.ProposalStatusNavigation.LogStatusNavigation.Lovvalue,
                                     ReviewDate = s.pp.pl.ProposalStatusNavigation.LogDt

                                 } : new ReviewComment()



                            }
                            ).ToListAsync();
        //        var plans = await tmp.PageBy(b => b.PlanCode, p.Paging.PageNo, p.Paging.PageSize)
        //.Join(DB.Users, pl => pl.DataLogNavigation.UserId, u => u.Id, (pl, u) => new { pl, u })
        //.Join(DB.Users, pl => pl.pl.ProposalStatusNavigation.UserId, upp => upp.Id, (pp, upp) => new { pp, upp })
        //        .Select(s => new PlanHeader
        //        {
        //            ChangeLog = new LogData
        //            {
        //                Action = s.pl.DataLogNavigation.LogType,
        //                ActionTime = s.pl.DataLogNavigation.LogDt,
        //                ActionType = "Data",
        //                ID = s.pl.DataLogNavigation.LogId,
        //                UserID = s.pl.DataLogNavigation.UserId,
        //                UserName = s.u.TfirstName + " " + s.u.TlastName
        //            },
        //            Organization = new BasicData
        //            {
        //                Code = s.pl.Organization.OrganizationCode,
        //                Description = s.pl.Organization.OrganizationThname,
        //                ID = s.pl.OrganizationId
        //            },
        //            PlanCode = s.pl.PlanCode,
        //            PlanID = s.pl.PlanId,
        //            PlanRelease = s.pl.PlanReleaseNavigation.Lovvalue,

        //            PlanYear = s.pl.StartYear,
        //            PlanYearTxt = s.pl.StartYear.ToString(),
        //            Proposals = s.pl.ProposalPlanPlan.Select(pps => new BasicData { Code = pps.Proposal.PlanCode, ID = pps.Proposal.PlanId }).ToList(),
        //            PlanStatusLog = new LogData
        //            {
        //                Action = s.pl.ProposalStatusNavigation.LogStatusNavigation.Lovcode,
        //                ActionType = s.pl.ProposalStatusNavigation.LogStatusNavigation.Lovvalue,
        //                ActionTime = s.pl.ProposalStatusNavigation.LogDt,
        //                ID = s.pl.ProposalStatus.Value,
        //                Remark = s.pl.ProposalStatusNavigation.Remark,
        //                UserID = s.pl.ProposalStatusNavigation.UserId,

        //            },
        //            PlanStatus = s.pl.ProposalStatusNavigation.LogStatusNavigation.Lovvalue


        //        }
        //        ).ToListAsync();
                // var plans = await sel.ToListAsync();

                foreach (var pn in plans)
                {
                    if (PlanType == ServiceModels.Constants.LOVGroup.Plan_Type.แผน_5_ปี)
                    {
                        pn.PlanYearTxt = string.Format("{0}-{1}", pn.PlanYear, pn.PlanYear + 4);
                    }
                    else {
                        pn.PlanYearTxt = string.Format("{0}",pn.PlanYear);
                    }
                    if (PlanID.HasValue)
                    {
                        var log = DB.DataLog.Where(w => w.TableName == "Plan" && w.TableKey == pn.PlanID && w.LogType == "C").FirstOrDefault();
                        if (log != null)
                        {
                            pn.CreateLog = new LogData { Action = "C", ActionTime = log.LogDt, ActionType = "Data", ID = log.LogId, UserID = log.UserId };
                            var user = DB.Users.Where(w => w.Id == log.UserId).FirstOrDefault();
                            if (user != null)
                            {
                                pn.CreateLog.UserName = user.TfirstName + " " + user.TlastName;
                            }
                        }

                    }



                }
                 
                ret.Data = plans;
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        public async Task<ReturnObject<PlanHeader>> GetPlanHeaderByID(string PlanType, long ID)
        {
            var ret = new ReturnObject<PlanHeader>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {

                var p = new PlanListParameter { Paging = new Paging { PageNo = 1, PageSize = 1 }, PlanStatus = "" };
                var plan = await GetPlanList(PlanType, p, ID );
                if (!plan.IsCompleted)
                {
                    ret.CloneMessage(plan.Message);
                    return ret;
                }
                if (plan.Data.Count == 0)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { "แผน" });
                    return ret;
                }
                ret.Data = plan.Data[0];
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        private void SumAmountToYearLoan(decimal dec, int year, string source, List<TotalLoanByYear> data )
        {
            var d = data.Where(w => w.Year == year).FirstOrDefault();
            if (d == null)
            {
                d = new TotalLoanByYear { Year = year };
                data.Add(d);
            }
            if (source == "F")
            {
                d.ForeignLoan += dec;
            }else if(source == "L")
            {
                d.LocalLoan += dec;
            }else
            {
                d.OtherLoan += dec;
            }
            d.GrandTotal += dec;
        }
        private void FillSummary5Y (int startYear,List<TotalLoanByYear> data)
        {
            for (var i = data.Count; i < 5; i++)
            {
                data.Add(new TotalLoanByYear
                {
                    ForeignLoan = 0,
                    GrandTotal = 0,
                    LocalLoan = 0,
                    OtherLoan = 0,
                    Year = startYear + i
                });
            }
        }
        public async Task<ReturnObject<Plan5YDetail>> GetPlanDetail(string PlanType,int StartYear, long? ID, long? orgID, string planRelease)
        {
            var ret = new ReturnObject<Plan5YDetail>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var data = new Plan5YDetail {   LoanByProjType = new List<ProjTypeLoanByYear>()};
                if (ID.HasValue)
                {
                    var head = await GetPlanHeaderByID(PlanType, ID.Value);
                    if (!head.IsCompleted)
                    {
                        ret.CloneMessage(head.Message);
                        return ret;
                    }
                    data.PlanHeader = head.Data;
                }
                var rate = _helper.GetCurrencyRate(StartYear);
               if (!rate.IsCompleted)
                {
                    ret.CloneMessage(rate.Message);
                    return ret;
                }
                data.CurrencyData = rate.Data;
                var pjtAll = new ProjTypeLoanByYear
                {
                    ProjectTypeCode = "ALL",
                    ProjectTypeName = "ทั้งหมด",
                    LoanByYear = new List<TotalLoanByYear>()
                };
                data.LoanByProjType.Add(pjtAll);
                var plQ = DB.PlanLoan.WhereIf(ID.HasValue, w => w.PlanProject.PlanId == ID)
                            .WhereIf(!string.IsNullOrEmpty(planRelease), pr => pr.PlanProject.Plan.PlanReleaseNavigation.Lovcode == planRelease)
                            .WhereIf(orgID.HasValue, o => o.PlanProject.Plan.OrganizationId == orgID.Value);
               

                plQ  =  plQ.Include(proj => proj.PlanProject.ProjectTypeNavigation).Where(w => w.PlanProject.Plan.StartYear == StartYear)
                    .OrderBy(o => o.PeriodValue);
                var planloans = await plQ.ToListAsync();
                foreach (var pl in planloans)
                {
                    var pjt = data.LoanByProjType.Where(w => w.ProjectTypeCode == pl.PlanProject.ProjectTypeNavigation.Lovcode).FirstOrDefault();
                    if (pjt == null)
                    {
                        pjt = new ProjTypeLoanByYear {
                            LoanByYear  = new List<TotalLoanByYear>(),
                            ProjectTypeCode = pl.PlanProject.ProjectTypeNavigation.Lovcode,
                            ProjectTypeName = pl.PlanProject.ProjectTypeNavigation.Lovvalue
                        };
                        data.LoanByProjType.Add(pjt);
                    }
                    var cRate = rate.Data.Currency.Where(w => w.CurrencyCode == pl.LoanCurrency).FirstOrDefault();
                    if (cRate == null)
                    {
                        ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { $"สกุลเงิน {pl.LoanCurrency}" });
                        return ret;
                    }
                    decimal amt = pl.LoanAmount * cRate.CurrencyRate;
                    SumAmountToYearLoan(amt, pl.PeriodValue, pl.LoanType, pjtAll.LoanByYear );
                    SumAmountToYearLoan(amt, pl.PeriodValue, pl.LoanType, pjt.LoanByYear);
                    //SumAmountToYearLoan(amt, pl.PeriodValue, pl.LoanType, data.LoanSummary);
                    //var pjtCode = pl.PlanProject.ProjectTypeNavigation.Lovcode;
                    //if (pjtCode == ServiceModels.Constants.LOVGroup.Project_Type.เพื่อการดำเนินงาน)
                    //{
                    //    SumAmountToYearLoan(amt, pl.PeriodValue, pl.LoanType, data.LoanForOperation);
                    //}
                    //else if (pjtCode == ServiceModels.Constants.LOVGroup.Project_Type.เพื่อโครงการทั่วไป)
                    //{
                    //    SumAmountToYearLoan(amt, pl.PeriodValue, pl.LoanType, data.LoanForGeneral);
                    //}
                    //else if (pjtCode == ServiceModels.Constants.LOVGroup.Project_Type.เพื่อโครงการลงทุน_พัฒนา)
                    //{
                    //    SumAmountToYearLoan(amt, pl.PeriodValue, pl.LoanType, data.LoanForDEV);
                    //}else
                    //{
                    //    ret.Message.Add(new MessageData { Code = "InvalidParm", Message = $"Invalid ProjectTypeCode ({pjtCode})", MessageType = eMessageType.Error.ToString(), MessageHint = "pram error" });
                    //    return ret;
                    //}

                }
                foreach (var loan in data.LoanByProjType )
                {
                    FillSummary5Y(StartYear, loan.LoanByYear);
                }
                //FillSummary5Y(StartYear, data.LoanSummary);
                //FillSummary5Y(StartYear, data.LoanForOperation);
                //FillSummary5Y(StartYear, data.LoanForGeneral);
                //FillSummary5Y(StartYear, data.LoanForDEV);
                ret.Data = data;
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        public async  Task<ReturnObject<long?>> ModifyPlanProject(  PlanProjectSource p, string userID,long projectID, long planID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var project = await DB.Project.Where(w => w.ProjectId == projectID).FirstOrDefaultAsync();
                if(project == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { "โครงการ" });
                    return ret;
                }
                var plan = await DB.Plan.Include(pt => pt.PlanTypeNavigation).Where(w => w.PlanId == planID).FirstOrDefaultAsync();
                if (plan == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { "แผนงาน" });
                    return ret;
                }
                var planproj = await DB.PlanProject.Include(pl => pl.Plan).Include(pt => pt.Plan.PlanTypeNavigation).Where(w => w.PlanId == planID &&  w.ProjectId == projectID).FirstOrDefaultAsync();
                if (planproj == null)
                {
                    planproj = new PlanProject { Project = project, Plan = plan };
                    DB.PlanProject.Add(planproj);
                }
                //check year
                // 5 year
                var fromYear = planproj.Plan.StartYear;
                var toYear = fromYear;
                if (planproj.Plan.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.แผน_5_ปี)
                {
                    toYear += 4;

                }
                foreach (var chk in p.LoanPeriods)
                {
                    if (chk.PeriodValue < fromYear || chk.PeriodValue > toYear)
                    {
                        ret.Message.Add(new MessageData
                        {
                            Message = $"ข้อมูลปีต้องอยู่ระหว่าง {fromYear.ToString()}-{toYear.ToString()}",
                            Code = "NotInRange",
                            MessageHint = "Not in range",
                            MessageType = eMessageType.Error.ToString()
                        });
                        return ret;
                    }
                    if (chk.PeriodType != "Y")
                    {
                        ret.Message.Add(new MessageData
                        {
                            Message = $"PeriodType ต้องเป็น Y เท่านั้น",
                            Code = "NotInRange",
                            MessageHint = "Not in range",
                            MessageType = eMessageType.Error.ToString()
                        });
                        return ret;
                    }
                }
                //end check year
                //List<PlanLoan> del = new List<PlanLoan>();
                var ppj = await DB.PlanProject
                    .Include(i => i.PlanProjectResolution)
                    .Include(f => f.PlanProjectFile)
                    .Where(w => w.PlanId == planID  && w.ProjectId == projectID).FirstOrDefaultAsync();
                if (ppj != null)
                {
                    ppj.CoordinatorEmail = p.CoordinatorEmail;
                    ppj.CoordinatorName = p.CoordinatorName;
                    ppj.CoordinatorPosition = p.CoordinatorPosition;
                    ppj.CoordinatorTel = p.CoordinatorTel;
                }
                #region resolutions
                var resIds = p.Resolutions.Where(w => w.Id.HasValue).Select(s => s.Id.Value).ToList();

                var delres = ppj.PlanProjectResolution.Where(w => !resIds.Contains(w.Id)).ToList();
                DB.PlanProjectResolution.RemoveRange(delres);


                foreach (var res in p.Resolutions)
                {
                    var l = ppj.PlanProjectResolution.Where(w => w.Id == res.Id).FirstOrDefault();
                    if (l == null)
                    {
                        l = new PlanProjectResolution();
                        ppj.PlanProjectResolution.Add(l);
                    }
                    l.Date = res.Date;
                    l.Detail = res.Detail;
                    l.Amount = res.Amount;
                    if (res.File != null)
                    {
                        AttachFile att = await DB.AttachFile.Where(w => w.AttachFileId == res.File.ID).FirstOrDefaultAsync();
                        if (res.File.ClearFile && att != null)
                        {
                            l.FileId = null;
                            DB.AttachFile.Remove(att);
                        }
                        else
                        {
                            if (att == null)
                            {
                                att = new AttachFile
                                {

                                };
                                l.File = att;
                            }
                            att.FileDetail = res.File.FileDetail;
                            att.FileExtension = res.File.FileExtension;
                            att.FileName = res.File.FileName; ;
                            att.FileSize = res.File.FileSize;
                            if (res.File.FileData != null)
                            {
                                att.FileData = res.File.FileData;
                            }
                        }


                    }

                }
                #endregion
                #region Files
                var fileIds = p.Files.Where(w => w.Id.HasValue).Select(s => s.Id.Value).ToList();

                var delfile = ppj.PlanProjectFile.Where(w => !fileIds.Contains(w.Id)).ToList();
                DB.PlanProjectFile.RemoveRange(delfile);


                foreach (var file in p.Files)
                {
                    var l = ppj.PlanProjectFile.Where(w => w.Id == file.Id).FirstOrDefault();
                    if (l == null)
                    {
                        l = new PlanProjectFile();
                        ppj.PlanProjectFile.Add(l);
                    }
                    l.Detail = file.Detail;
                    if (file.File != null)
                    {
                        AttachFile att = await DB.AttachFile.Where(w => w.AttachFileId == file.File.ID).FirstOrDefaultAsync();
                        if (file.File.ClearFile && att != null)
                        {
                            l.FileId = null;
                            DB.AttachFile.Remove(att);
                        }
                        else
                        {
                            if (att == null)
                            {
                                att = new AttachFile
                                {

                                };
                                l.File = att;
                            }
                            att.FileDetail = file.File.FileDetail;
                            att.FileExtension = file.File.FileExtension;
                            att.FileName = file.File.FileName; ;
                            att.FileSize = file.File.FileSize;
                            if (file.File.FileData != null)
                            {
                                att.FileData = file.File.FileData;
                            }
                        }


                    }

                }
                #endregion
                var all = await DB.PlanLoan.Where(w => w.PlanProjectId == planproj.PlanProjectId).ToListAsync();
                // Initial Soure of loan
                foreach (var a in all)
                {
                    if (a.SourceLoanId == null)
                    {
                        var lov = await GetLOVFromSourceType(a.LoanType);
                        if (lov != null)
                        {
                            a.SourceLoanId = lov.Lovkey;
                             
                        }
                    }else
                    {
                        var lov = await DB.CeLov.Where(w => w.Lovkey == a.SourceLoanId).FirstOrDefaultAsync();
                        if (lov != null &&  !string.IsNullOrEmpty(lov.Remark))
                        {
                            a.LoanType = lov.Remark;
                        }
                    }
                }
                var rate = _helper.GetCurrencyRate(plan.StartYear);
                foreach (var year in p.LoanPeriods)
                {
                    foreach (var loan in year.LoanSources)
                    {
                        var r = rate.Data.Currency.Where(w => w.CurrencyCode == loan.Currency).FirstOrDefault();
                        
                        if (r == null)
                        {
                            ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { $"สกุลเงิน {loan.Currency} ปี {plan.StartYear.ToString()}" });
                            return ret;
                        }
                        var sumMonth = loan.Jan.GetValueOrDefault(0) + loan.Feb.GetValueOrDefault(0) + loan.Mar.GetValueOrDefault(0) + loan.Apr.GetValueOrDefault(0) + loan.May.GetValueOrDefault(0) + loan.Jun.GetValueOrDefault(0) + loan.Jul.GetValueOrDefault(0) + loan.Aug.GetValueOrDefault(0) + loan.Sep.GetValueOrDefault(0) + loan.Oct.GetValueOrDefault(0) + loan.Nov.GetValueOrDefault(0) + loan.Dec.GetValueOrDefault(0);
                         
                        PlanLoan planloan;
                        //planloan = all.Where(w => w.PeriodValue == year.PeriodValue && w.LoanCurrency == loan.Currency && w.LoanType == loan.SourceType
                        //&& w.PeriodType == year.PeriodType
                        //).FirstOrDefault();
                        planloan = all.Where(w => w.PeriodValue == year.PeriodValue && w.LoanCurrency == loan.Currency && w.SourceLoanId == loan.SourceLoan.ID
                        && w.PeriodType == year.PeriodType
                        ).FirstOrDefault();
                        if (planloan != null)
                        {
                            all.Remove(planloan);
                           
                        }else
                        {
                            planloan = new PlanLoan() ;
                            //DB.PlanLoan.Add(planloan);
                            //planproj.PlanLoan.Add(planloan);
                            planloan.PlanProject = planproj;
                            planloan.LoanCurrency = loan.Currency;
     
                            planloan.PeriodType = year.PeriodType;
                            planloan.PeriodValue = year.PeriodValue;
                            planloan.SourceLoanId = loan.SourceLoan.ID;
                            var lov = await DB.CeLov.Where(w => w.Lovkey == loan.SourceLoan.ID).FirstOrDefaultAsync();
                            if (lov != null && !string.IsNullOrEmpty(lov.Remark))
                            {
                                planloan.LoanType = lov.Remark;
                            }
                            DB.PlanLoan.Add(planloan);
                        }
                 
                        planloan.LoanAmount = sumMonth == 0 ? loan.LoanAmount : sumMonth;
                        planloan.Jan = loan.Jan.GetValueOrDefault(0);
                        planloan.Feb = loan.Feb.GetValueOrDefault(0);
                        planloan.Mar = loan.Mar.GetValueOrDefault(0);
                        planloan.Apr = loan.Apr.GetValueOrDefault(0);
                        planloan.May = loan.May.GetValueOrDefault(0);
                        planloan.Jun = loan.Jun.GetValueOrDefault(0);
                        planloan.Jul = loan.Jul.GetValueOrDefault(0);
                        planloan.Aug = loan.Aug.GetValueOrDefault(0);
                        planloan.Sep = loan.Sep.GetValueOrDefault(0);
                        planloan.Oct = loan.Oct.GetValueOrDefault(0);
                        planloan.Nov = loan.Nov.GetValueOrDefault(0);
                        planloan.Dec = loan.Dec.GetValueOrDefault(0);

                        planloan.Thbamount = planloan.LoanAmount  * r.CurrencyRate;

                    }
                }
                var newLog = new DataLog { LogDt = DateTime.Now, LogType = "U", TableName = "Plan", UserId = userID, TableKey = planproj.Plan.PlanId  };
                DB.DataLog.Add(newLog);
                planproj.Plan.DataLog = newLog.LogId;
                DB.PlanLoan.RemoveRange(all);
                await DB.SaveChangesAsync();
                ret.IsCompleted = true;
                ret.Data = planproj.PlanProjectId;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        private async Task<ReturnObject<decimal>> getProjectLimit(long projId, int year, CurrencyData rate)
        {
            var ret = new ReturnObject<decimal>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                ret.Data = 0;
                var pjamt = await DB.ProjActAmount.Where(w => w.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.ค่าใช้จ่ายตามมติ
                && w.ProjectAct.Project.ProjectId == projId).ToListAsync();
                foreach (var amt in pjamt)
                {
                    var r = rate.Currency.Where(w => w.CurrencyCode == amt.Currency).FirstOrDefault();
                    if (r == null)
                    {
                        ret.AddMessage(eMessage.RateOfYearIsNotFound.ToString(), "rate", eMessageType.Error, new string[] { amt.Currency, year.ToString() });
                        return ret;

                    }
                    ret.Data += amt.Amount * r.CurrencyRate;
                }
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        public async Task<ReturnList<PlanProjectList>> GetPlanProjectList(string PlanType, PlanProjectListParameter p)
        {
            var ret = new ReturnList<PlanProjectList>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var planproj = DB.PlanProject.WhereIf(p.PlanID.HasValue, w => w.PlanId == p.PlanID) 
                .WhereIf(!string.IsNullOrEmpty(p.Paging.SearchText), w => w.Project.ProjectCode.Contains(p.Paging.SearchText) || w.Project.ProjectThname.Contains(p.Paging.SearchText))
                .WhereIf(!string.IsNullOrEmpty(p.ProjectType) , t => t.ProjectTypeNavigation.Lovcode == p.ProjectType)
                .Where(w => w.Plan.StartYear == p.StartYear);
     
                ret.TotalRow = await planproj.CountAsync();

                planproj = planproj.PageBy(pp => pp.ProjectId, p.Paging.PageNo, p.Paging.PageSize, true);
                ret.PageSize = p.Paging.PageSize;
                ret.PageNo = p.Paging.PageNo;
                var rate = _helper.GetCurrencyRate(p.StartYear);
                if (!rate.IsCompleted)
                {
                    ret.CloneMessage(rate.Message);
                    return ret;
                }
                
                ret.Data = await planproj.Select(s => new PlanProjectList
                {
                    PlanID = s.PlanId,
                    ProjectID = s.ProjectId,
                    PlanProjecID = s.PlanProjectId,
                    CoordinatorTel = s.CoordinatorTel,
                    CoordinatorPosition = s.CoordinatorPosition,
                    CoordinatorName = s.CoordinatorName,
                    CoordinatorEmail = s.CoordinatorEmail,
                    Resolutions = s.PlanProjectResolution.Select(res => new ProjectResolutionModel
                    {
                        Id = res.Id,
                        Amount = res.Amount,
                        Date = res.Date,
                        Detail = res.Detail,
                        File = !res.FileId.HasValue ? null : new AttachFileData
                        {
                            FileDetail = res.File.FileDetail,
                            FileExtension = res.File.FileExtension,
                            FileName = res.File.FileName,
                            FileSize = res.File.FileSize,
                            ID = res.File.AttachFileId
                        }
                    }).ToList(),
                    Files = s.PlanProjectFile.Select(res => new ProjectResolutionModel
                    {
                        Id = res.Id,
                        Detail = res.Detail,
                        File = !res.FileId.HasValue ? null : new AttachFileData
                        {
                            FileDetail = res.File.FileDetail,
                            FileExtension = res.File.FileExtension,
                            FileName = res.File.FileName,
                            FileSize = res.File.FileSize,
                            ID = res.File.AttachFileId
                        }
                    }).ToList(),
                    ProjectDetail = new ProjectModel {
                        ProjectCode = s.Project.ProjectCode,
                        ProjectID = s.Project.ProjectId,
                        ProjectTHName = s.Project.ProjectThname,
                        //LimitAmount =    s.Project.LimitAmount,
                        ProjectENName = s.Project.ProjectEnname,
                        ProjectType = new BasicData
                        {
                            Code = s.ProjectTypeNavigation.Lovcode,
                            Description = s.ProjectTypeNavigation.Lovvalue,
                            ID = s.ProjectTypeNavigation.Lovkey
                        },
                        StartYear = s.Project.StartYear,
                        PDMOAgreement = s.Project.Pdmoagreement,
                        ResolutionAgreement = s.Project.ResolutionAgreement,
                        IsNewProject = s.Project.IsNewProject,
                        StartDate = s.Project.StartDate
                    }
                    

                }).ToListAsync();
          

                foreach (var loan in ret.Data)
                {
                    var limit = await getProjectLimit(loan.ProjectID, p.StartYear, rate.Data);
                    if (!limit.IsCompleted)
                    {
                        ret.CloneMessage(limit.Message);
                        return ret;
                    }
                    loan.ProjectDetail.LimitAmount = limit.Data;
                    loan.LoanByYears = new List<LoanPeriod>();
                    var ploans = await  DB.PlanLoan
                        .Include(i => i.SourceLoan)
                        .Where(w => w.PlanProjectId == loan.PlanProjecID)
                        .OrderBy(o => o.PeriodValue)
                        .ToListAsync();
                     
                    foreach ( var l in ploans)
                    {
                        var cRate = rate.Data.Currency.Where(w => w.CurrencyCode == l.LoanCurrency).FirstOrDefault();
                        if (cRate == null)
                        {
                            ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { $"สกุลเงิน { l.LoanCurrency}" });
                            return ret;
                        }
                        var lp = loan.LoanByYears.Where(w => w.PeriodValue == l.PeriodValue).FirstOrDefault();
                        if (lp == null)
                        {
                            lp = new LoanPeriod { PeriodType = "Y" , PeriodValue = l.PeriodValue , LoanSources = new List<LoanSource>() };
                            loan.LoanByYears.Add(lp);
                        }
                        if (!l.SourceLoanId.HasValue)
                        {
                            var lov = await GetLOVFromSourceType(l.LoanType);
                            if (lov != null)
                            {
                                l.SourceLoanId = lov.Lovkey;
                                l.SourceLoan = lov;
                            }
                        }
                        var ls = lp.LoanSources.Where(w => w.Currency == l.LoanCurrency && w.SourceLoan.ID == l.SourceLoanId).FirstOrDefault();
                        if (ls == null)
                        {
                            ls = new LoanSource { Currency = l.LoanCurrency, SourceType = l.LoanType,
                                SourceLoan = l.SourceLoan == null ? null : new BasicData
                                {
                                    ID = l.SourceLoan.Lovkey,
                                    Code = l.SourceLoan.Lovcode,
                                    Description = l.SourceLoan.Lovvalue
                                },

                            };
                            lp.LoanSources.Add(ls);
                        }
                        //if (ls.SourceLoan == null)
                        //{
                        //    var lov = await GetLOVFromSourceType(ls.SourceType);
                        //    if (lov != null)
                        //    {
                        //        ls.SourceLoan = new BasicData
                        //        {
                        //            ID = lov.Lovkey,
                        //            Code = lov.Lovcode,
                        //            Description = lov.Lovvalue
                        //        };
                        //    }
                        //}
                        ls.LoanAmount += l.LoanAmount;
                        ls.Jan += l.Jan.GetValueOrDefault(0);
                                ls.Feb += l.Feb.GetValueOrDefault(0);
                                ls.Mar += l.Mar.GetValueOrDefault(0);
                                ls.Apr += l.Apr.GetValueOrDefault(0);
                                ls.May += l.May.GetValueOrDefault(0);
                                ls.Jun += l.Jun.GetValueOrDefault(0);
                                ls.Jul += l.Jul.GetValueOrDefault(0);
                                ls.Aug += l.Aug.GetValueOrDefault(0);
                                ls.Sep += l.Sep.GetValueOrDefault(0);
                                ls.Oct += l.Oct.GetValueOrDefault(0);
                                ls.Nov += l.Nov.GetValueOrDefault(0);
                                ls.Dec += l.Dec.GetValueOrDefault(0);                        
                        //decimal amt =  l.LoanAmount * cRate.CurrencyRate;
                        //SumAmountToYearLoan(amt, l.PeriodValue, l.LoanType, loan.LoanByYears);
                    }
                    for (var i = loan.LoanByYears.Count(); i < 5; i ++)
                    {
                        var lpNew = new LoanPeriod
                        {
                            PeriodType = "Y",
                            PeriodValue = p.StartYear + i
                        };
                        //lpNew.LoanSources = new List<LoanSource>();
                        //lpNew.LoanSources.Add(new LoanSource {
                        //    Currency = "",
                        //    LoanAmount = 0,
                        //    SourceType = "L"
                        //});
                        //lpNew.LoanSources.Add(new LoanSource
                        //{
                        //    Currency = "",
                        //    LoanAmount = 0,
                        //    SourceType = "F"
                        //});
                        loan.LoanByYears.Add(lpNew);
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
        public async Task<CeLov> GetLOVFromSourceType(string sourceType)
        {
            string lovcode;
            if (sourceType == "L") { lovcode = ServiceModels.Constants.LOVGroup.Source_of_Loan.แหล่งเงินกู้ในประเทศ; }
            else if (sourceType == "F") { lovcode = ServiceModels.Constants.LOVGroup.Source_of_Loan.แหล่งเงินกู้ต่างประเทศ; }
            else { lovcode = ServiceModels.Constants.LOVGroup.Source_of_Loan.อื่นๆ; }
            var lov = await DB.CeLov.Where(w => w.Lovcode == lovcode && w.LovgroupCode == ServiceModels.Constants.LOVGroup.Source_of_Loan._LOVGroupCode).FirstOrDefaultAsync();
            return lov;
        }
        public async Task<ReturnMessage> NewDebtGetDataFromP5Y(long PlanID)
        {
            var ret = new ReturnMessage(_msglocalizer);
            try
            {
                var plan = await DB.Plan.Where(w => w.PlanId == PlanID && w.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.แผนก่อหนี้ใหม่).FirstOrDefaultAsync();
                if (plan == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { "แผน" });
                }
                var p5 = await DB.Plan.Where(w => w.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.แผน_5_ปี &&
                w.StartYear == plan.StartYear &&
                w.OrganizationId == plan.OrganizationId &&
                w.PlanRelease == plan.PlanRelease
                ).FirstOrDefaultAsync();

                // copy planproject
                var planproj5 = await DB.PlanProject.Where(w => w.PlanId == p5.PlanId).ToListAsync();
                foreach (var pp in planproj5)
                {
                    PlanProject ppNew = await DB.PlanProject.Where(w => w.PlanId == PlanID && w.ProjectId == pp.ProjectId).FirstOrDefaultAsync();
                    PlanLoan plNew;
                    bool isNewPP = false;
                    
                    if (ppNew == null)
                    {
                        ppNew = new PlanProject();
                        ppNew.PlanId = PlanID;
                        ppNew.ProjectId = pp.ProjectId;
                        DB.PlanProject.Add(ppNew);
                        isNewPP = true;
                    }
                    var planloans = await DB.PlanLoan.Where(w => w.PlanProjectId == pp.PlanProjectId && w.PeriodValue == plan.StartYear).ToListAsync();
                    foreach (var pl in planloans)
                    {
                        bool IsNewPL = false;
                        if ( isNewPP)
                        {
                            plNew = new PlanLoan();
                            ppNew.PlanLoan.Add(plNew);
                            IsNewPL = true;
                        }else
                        {
                            plNew = await DB.PlanLoan.Where(w => w.PlanProjectId == ppNew.PlanProjectId &&
                            w.PeriodType == pl.PeriodType &&
                            w.PeriodValue == pl.PeriodValue &&
                            w.LoanCurrency == pl.LoanCurrency).FirstOrDefaultAsync();
                            if (plNew == null)
                            {
                                plNew = new PlanLoan();
                                ppNew.PlanLoan.Add(plNew);
                                IsNewPL = true;
                            }
                        }
                        if (IsNewPL)
                        {
                            plNew.PeriodType = pl.PeriodType;
                            plNew.PeriodValue = pl.PeriodValue;
                            plNew.LoanType = pl.LoanType;
                            plNew.LoanAmount = pl.LoanAmount;
                            plNew.LoanCurrency = pl.LoanCurrency;
                        }

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

        public async Task<ReturnObject<long?>> AddProjectToPlan(long planID, long projID, string projType, bool AddActivies,string userID)
        {
            var ret = new  ReturnObject<long?>(_msglocalizer);
            ret.IsCompleted = false;
            try {
      
                var proj = await DB.Project.Include(i => i.ProjectTypeNavigation).Where(w => w.ProjectId == projID).FirstOrDefaultAsync();
                if (proj == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { "โครงการ" });
                    return ret;
                }
                var plan = await DB.Plan.Where(w => w.PlanId == planID).FirstOrDefaultAsync();
                if (plan == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { "แผนงาน" });
                    return ret;
                }
                var dup = await DB.PlanProject.Where(w => w.PlanId == planID && w.ProjectId == projID).FirstOrDefaultAsync();
                if (dup != null)
                {
                    ret.AddMessage(eMessage.ProjIsAlreadyInPlan.ToString(), "dup", eMessageType.Error, new string[] { proj.ProjectCode, plan.PlanCode });
                    return ret;
                }
                long pjtKey;
                if (string.IsNullOrEmpty(projType))
                {
                    pjtKey = proj.ProjectType;
                }else
                {
                    var ck = _helper.LOVCodeValidate(projType, ServiceModels.Constants.LOVGroup.Project_Type._LOVGroupCode, null);
                    if (!ck.IsCompleted)
                    {
                        ret.CloneMessage(ck.Message);
                        return ret;
                    }
                    pjtKey = ck.Data.LOVKey;
                }
            
                var pp = new PlanProject { PlanId = planID, ProjectId = projID, ProjectType = pjtKey };
                var log = new DataLog { LogDt = DateTime.Now, LogType = "U", TableName = "Plan", UserId = userID , TableKey = planID};
                plan.DataLogNavigation = log;
                if (AddActivies)
                {
                    var acts = await DB.ProjAct.Where(w => w.ProjectId == projID).ToListAsync();
                     if (await _com.IsProjectShortForm(proj.ProjectTypeNavigation.Lovcode))
                    {
                        if (acts.Count() == 0)
                        {
                            acts.Add(new ProjAct
                            {
                                ActivityName = proj.ProjectThname
                            });
                        }
                    }
                    foreach (var act in acts)
                    {
                        var pa = new PlanAct
                        {
                           ActivityName = act.ActivityName,
                           ProjActId = act.ProjActId,
                        };
                        pp.PlanAct.Add(pa);
                    }
                }

                DB.PlanProject.Add(pp);
                await DB.SaveChangesAsync();
                ret.IsCompleted = true;
                ret.Data = pp.PlanProjectId;

            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;

        }

        public async Task<ReturnMessage> RemoveProjectFromPlan(long planID, long projID, string userID)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var pjpl = await DB.PlanProject
                    .Include(pa => pa.PlanAct)
                    .Where(w => w.ProjectId == projID && w.PlanId == planID).FirstOrDefaultAsync();
                if (pjpl == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { "โครงการในแผน" });
                    return ret;
                }
                foreach (var pa in pjpl.PlanAct)
                {
                    var delact = await DB.PlanAct.Where(w => w.ReferencePlanActId == pa.PlanActId).FirstOrDefaultAsync();
                    if (delact != null)
                    {
                        delact.ReferencePlanActId = null;
                    }
                }
                var plan = await DB.Plan.Where(w => w.PlanId == planID).FirstOrDefaultAsync();
                var log = new DataLog { LogDt = DateTime.Now, LogType = "U", TableName = "Plan", UserId = userID, TableKey = planID };
                plan.DataLogNavigation = log;

                DB.PlanProject.Remove(pjpl);
                await DB.SaveChangesAsync();
                ret.IsCompleted = true;
            

            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        public async Task<ReturnMessage> PlanAttachFile(MOFContext db, Plan plan, List<AttachFileData> afs)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                ////clear flie
                var delFiles = new List<PlanAttach>();
                foreach (var file in plan.PlanAttach)
                {
                    var del = afs.Where(w => w.ID == file.AttachFileId).FirstOrDefault();
                    if (del == null)
                    {
                        delFiles.Add(file);
                    }
                }
                foreach (var d in delFiles)
                {
                    plan.PlanAttach.Remove(d);
                    db.AttachFile.Remove(d.AttachFile);
                }
                foreach (var f in afs)
                {
                    if (!f.ID.HasValue) f.ID = -1;
                    var att = await db.AttachFile.Where(w => w.AttachFileId == f.ID.Value).FirstOrDefaultAsync();
                    if (att == null)
                    {
                        att = new AttachFile
                        {
                            FileDetail = f.FileDetail,
                            FileExtension = f.FileExtension,
                            FileName = f.FileName,
                            FileSize = f.FileSize
                        };
                        plan.PlanAttach.Add(new PlanAttach
                        {
                            AttachFile = att
                        });
                    }

                    if (f.FileData?.Length > 0)
                    {
                        att.FileData = f.FileData;
                    }
                    if (f.ClearFile)
                    {
                        att.FileData = null;
                    }

                    //var up = await _helper.UploadFile(db, f,false );

                    //if (!up.IsCompleted)
                    //{
                    //    ret.CloneMessage(up.Message);
                    //    ret.IsCompleted = false;
                    //    return ret;
                    //}
                    f.FileData = null;
                }
                //await db.SaveChangesAsync();
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        public async Task<ReturnObject<long?>> GetPreviousPlanRevision(CreatePlanParameter p, string planType)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                string revCode = "";
                if (p.PlanRelease == "M3")
                {
                    revCode = ServiceModels.Constants.LOVGroup.Plan_Release.ปรับปรุงแผนฯ_ครั้งที่_2;
                }
                else
                if (p.PlanRelease == ServiceModels.Constants.LOVGroup.Plan_Release.ปรับปรุงแผนฯ_ครั้งที่_2)
                {
                    revCode = ServiceModels.Constants.LOVGroup.Plan_Release.ปรับปรุงแผนฯ_ครั้งที่_1;
                }else if (p.PlanRelease == ServiceModels.Constants.LOVGroup.Plan_Release.ปรับปรุงแผนฯ_ครั้งที่_1)
                {
                    revCode = ServiceModels.Constants.LOVGroup.Plan_Release.จัดทำแผนฯ_ตั้งต้น;
                }else
                {
                    return ret;
                }
                var pl = await DB.Plan.Where(w => w.OrganizationId == p.OrganizationID && w.StartYear == p.StartYear && w.PlanReleaseNavigation.Lovcode == revCode && w.PlanTypeNavigation.Lovcode == planType).FirstOrDefaultAsync();
                if (pl != null)
                {
                    ret.Data = pl.PlanId;
                    ret.IsCompleted = true;
                }
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        public async Task<ReturnMessage> DeletePlan(long planID, string userID)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var pl = await DB.Plan.Where(w => w.PlanId == planID).FirstOrDefaultAsync();
                var u = await DB.Users.Where(w => w.Id == userID).FirstOrDefaultAsync();
                if (u == null)
                {
                    ret.AddMessage(eMessage.CodeIsNotFound.ToString(), "user", eMessageType.Error, new string[] { _msglocalizer[eMessage.User.ToString()] });
                    return ret;
                }
                if (pl != null)
                {
                    var pp = await DB.ProposalPlan.Where(w => w.PlanId == planID).FirstOrDefaultAsync();
                    if (pp != null)
                    {
                        ret.AddMessage(eMessage.AlreadyProposed.ToString(), "user", eMessageType.Error, null  );
                        return ret;
                    }
                    var log = new DataLog
                    {
                        LogDt = DateTime.Now,
                        LogType = "D",
                        TableKey = planID,
                        TableName = "Plan",
                        UserId = userID
                    };
                    DB.DataLog.Add(log);
                    DB.Plan.Remove(pl);
                    await DB.SaveChangesAsync();
                }
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
