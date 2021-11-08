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
using mof.ServiceModels.Proposal;

namespace mof.Services
{
    public partial class PlanRepository : IPlan
    {



        private async Task<PlanHeader> storePlanHeader(List<ProposalPlan> pps, string projType  )
        {

            var ptype = pps.Where(w => w.Plan.PlanTypeNavigation.Lovcode == projType).FirstOrDefault();
            if (ptype != null)
            {
                var header = await  GetPlanHeaderByID( projType, ptype.PlanId.Value);
                if (header.IsCompleted)
                {
                    return header.Data;
                }

            }
            return null;
        }
        public async Task<ReturnMessage> UpdateProposalStatus(long proposalID, string userID)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var plan = await DB.Plan
                    .Include(l => l.ProposalStatusNavigation).ThenInclude(lpp => lpp.LogStatusNavigation)
                    .Where(w => w.PlanId == proposalID && w.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.ข้อเสนอแผนบริหารหนี้).FirstOrDefaultAsync();
                if (plan == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "plan", eMessageType.Error, new string[] { _msglocalizer[eMessage.Plan.ToString()] });
                    return ret;
                }
                var props = await DB.ProposalPlan.Include(i => i.Proposal).ThenInclude(pp => pp.ProposalStatusNavigation).ThenInclude(lpp => lpp.LogStatusNavigation)
                .Include(i => i.Plan).ThenInclude(pp => pp.ProposalStatusNavigation).ThenInclude(lpp => lpp.LogStatusNavigation)
    .           Where(w => w.ProposalId == proposalID).ToListAsync();
                var tot = props.GroupBy(g =>   g.Plan.ProposalStatusNavigation.LogStatusNavigation.Lovcode ).Select(s => new { s.Key, Count = s.Count() }).ToList();
                var ppStatus = ServiceModels.Constants.LOVGroup.Proposal_Status.จัดทำข้อเสนอแผนฯ;

                if (tot.Count == 1)
                {
                    if (tot[0].Key == ServiceModels.Constants.LOVGroup.Plan_of_Proposal_Status.เสนอแผนฯ)
                    {
                        ppStatus = ServiceModels.Constants.LOVGroup.Proposal_Status.เสนอแผน;
                    }
                    if (tot[0].Key == ServiceModels.Constants.LOVGroup.Plan_of_Proposal_Status.เห็นชอบ_โดย_สบน_)
                    {
                        ppStatus = ServiceModels.Constants.LOVGroup.Proposal_Status.รับข้อเสนอโดย_สบน_;
                    }
                    if (tot[0].Key == ServiceModels.Constants.LOVGroup.Plan_of_Proposal_Status.เห็นชอบโดย_คณะอนุฯ)
                    {
                        ppStatus = ServiceModels.Constants.LOVGroup.Proposal_Status.เห็นชอบโดย_คณะอนุฯ_;
                    }
                    if (tot[0].Key == ServiceModels.Constants.LOVGroup.Plan_of_Proposal_Status.เห็นชอบและอนุมัติโดย_คนน_)
                    {
                        ppStatus = ServiceModels.Constants.LOVGroup.Proposal_Status.เห็นชอบและอนุมัติโดย_คนน_;
                    }

                }
                var consider = tot.Where(w => w.Key == ServiceModels.Constants.LOVGroup.Plan_of_Proposal_Status.รอแก้ไข).FirstOrDefault();

                if (consider != null)
                {
                    ppStatus = ServiceModels.Constants.LOVGroup.Proposal_Status.ระหว่างการทบทวนพิจารณาแผน;
                }
                if (ppStatus != plan.ProposalStatusNavigation.LogStatusNavigation.Lovcode)
                {
                    long? status = null;
                    var chk = _helper.LOVCodeValidate(ppStatus, ServiceModels.Constants.LOVGroup.Proposal_Status._LOVGroupCode, null);
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
                        Remark = "Auto update status",
                        TableKey = plan.PlanId,
                        TableName = "PlanProposalStatus",
                        UserId = userID
                    };
                    plan.ProposalStatusNavigation = log;
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
        public async Task<ReturnObject<ProposalModel>> GetProposal(long id)
        {
            var ret = new ReturnObject<ProposalModel>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var plan = await DB.Plan.Where(w => w.PlanId == id && w.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.ข้อเสนอแผนบริหารหนี้).FirstOrDefaultAsync();
                if (plan ==null )
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "plan", eMessageType.Error, new string[] { _msglocalizer[eMessage.Plan.ToString()] });
                    return ret;
                }
                var props = await DB.ProposalPlan.Include(i => i.Proposal).ThenInclude(t => t.PlanTypeNavigation)
                    .Include(i => i.Plan).ThenInclude(pt => pt.PlanTypeNavigation)
                    .Where(w => w.ProposalId == id).ToListAsync();

                var data = new ProposalModel();
                var ph = await GetPlanHeaderByID(ServiceModels.Constants.LOVGroup.Plan_Type.ข้อเสนอแผนบริหารหนี้, id);
                if (ph.IsCompleted)
                {
                    data.ProposalHeader = ph.Data;
                }
                data.FiveYearPlan = await storePlanHeader(props, ServiceModels.Constants.LOVGroup.Plan_Type.แผน_5_ปี);
                data.FinancialReport = await storePlanHeader(props, ServiceModels.Constants.LOVGroup.Plan_Type.รายงานสถานะทางการเงินและภาระหนี้);
                data.NewDebtPlan = await storePlanHeader(props, ServiceModels.Constants.LOVGroup.Plan_Type.แผนก่อหนี้ใหม่);
                data.ExistingDebtPlan = await storePlanHeader(props, ServiceModels.Constants.LOVGroup.Plan_Type.แผนบริหารหนี้เดิม);

                ret.Data = data;
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        public async Task<ReturnMessage> AddPlanToProposal(long proposalID, long planID, string userID)
        {
            var ret = new ReturnObject<ReturnMessage>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var ppsPlan = await DB.Plan.Where(w => w.PlanId == proposalID && w.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.ข้อเสนอแผนบริหารหนี้).FirstOrDefaultAsync();
                if (ppsPlan == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "plan", eMessageType.Error, new string[] { _msglocalizer[eMessage.Proposal.ToString()] });
                    return ret;
                }
                var plan = await DB.Plan.Include(pt => pt.PlanTypeNavigation).Where(w => w.PlanId == planID).FirstOrDefaultAsync();
                if (plan == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "plan", eMessageType.Error, new string[] { _msglocalizer[eMessage.Plan.ToString() ] });
                    return ret;
                }
                if (plan.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.ข้อเสนอแผนบริหารหนี้)
                {
                    ret.AddMessage(eMessage.DataIsNotAllow.ToString(), "plan", eMessageType.Error, new string[] { "ข้อเสนอแผนบริหารหนี้" });
                    return ret;
                }

                var pps = await DB.ProposalPlan
                    .Include(p => p.Plan).ThenInclude(pt => pt.PlanTypeNavigation)
                    .Where(w => w.ProposalId == proposalID).ToListAsync();
                var err = pps.Where(w => w.Plan.PlanTypeNavigation.Lovcode == plan.PlanTypeNavigation.Lovcode).FirstOrDefault();
                if (err != null)
                {
                    {
                        ret.AddMessage(eMessage.DataIsAlreadyExist.ToString(), "plan", eMessageType.Error, new string[] { plan.PlanTypeNavigation.Lovvalue });
                        return ret;
                    }
                }

                DB.ProposalPlan.Add(new ProposalPlan {
                    PlanId = planID,
                    ProposalId = proposalID
                });
                ppsPlan.DataLogNavigation = new DataLog
                {
                    LogDt = DateTime.Now,
                    LogType = "U",
                    Remark  = "AddPlanToProposal",
                    UserId = userID,
                    TableName = "Plan",
                    TableKey = ppsPlan.PlanId
                };
                await DB.SaveChangesAsync();

                var up = await UpdateProposalStatus(proposalID, userID);
                if (!up.IsCompleted)
                {
                    ret.CloneMessage(up.Message);
                    return ret;
                }
                ret.IsCompleted = true;

            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        public async Task<ReturnMessage> RemovePlanFromProposal(long proposalID, string planType, string userID)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var pps = await DB.ProposalPlan.Include(p => p.Proposal).Where(w => w.ProposalId == proposalID && w.Plan.PlanTypeNavigation.Lovcode == planType).FirstOrDefaultAsync();
                if (pps == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "plan", eMessageType.Error, new string[] { _msglocalizer[eMessage.Plan.ToString()] });
                    return ret;
                }
                pps.Proposal.DataLogNavigation = new DataLog
                {
                    LogDt = DateTime.Now,
                    LogType = "U",
                    Remark = "RemovePlanFromProposal",
                    UserId = userID,
                    TableName = "Plan",
                    TableKey = proposalID
                };
                DB.ProposalPlan.Remove(pps);
                await DB.SaveChangesAsync();

       

                var up = await UpdateProposalStatus(proposalID, userID);
                if (!up.IsCompleted)
                {
                    ret.CloneMessage(up.Message);
                    return ret;
                }

                ret.IsCompleted = true;

            }
             catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;

        }
        public async Task<ReturnObject<long?>> Modify(ProposalModel a, bool isCreate, string userID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
              


            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }

        public async Task<ReturnMessage> UpdatePlanFlowStatus(PlanHeader p , string userID, bool forceSubmitDB, string lovGroupCode, bool isProposing)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                if (p == null || p.PlanID == 0)
                {
                    ret.IsCompleted = true;
                    return ret;
                }
                var plan = await DB.Plan.Include(l => l.ProposalStatusNavigation).ThenInclude(ls => ls.LogStatusNavigation).Where(w => w.PlanId == p.PlanID).FirstOrDefaultAsync();
                if (plan == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "proposal", eMessageType.Error, new string[] { _msglocalizer[eMessage.Plan.ToString()] });
                    return ret;
                }
                ReturnObject<LOV> chk = new ReturnObject<LOV>(_msglocalizer);
                var changeStatus = p.ReviewComment.ResultStatus;
                var pStatus = plan.ProposalStatusNavigation.LogStatusNavigation.Lovcode;
                if (isProposing)
                {
                    if  ( (new string[] {ServiceModels.Constants.LOVGroup.Plan_of_Proposal_Status.รอแก้ไข , ServiceModels.Constants.LOVGroup.Plan_of_Proposal_Status.สร้างใหม่ }).Contains(pStatus))
                    {
                        changeStatus = ServiceModels.Constants.LOVGroup.Plan_of_Proposal_Status.เสนอแผนฯ;
                    }else
                    {
                        ret.IsCompleted = true;
                        return ret;
                    }

                }
                if (p.ReviewComment  != null )
                {
                    long? status = null;
                    if (!string.IsNullOrEmpty(p.ReviewComment.ResultStatus))
                    {
                        chk = _helper.LOVCodeValidate(changeStatus, lovGroupCode, null);
                        if (!chk.IsCompleted)
                        {
                            ret.CloneMessage(chk.Message);
                            return ret;
                        }
                        status = chk.Data.LOVKey;
                    }

                    var log = new DataLog
                    {
                        LogDt = DateTime.Now,
                        LogStatus = status,
                        LogType = "U",
                        Remark = p.ReviewComment.Comment ,
                        TableKey = p.PlanID,
                        TableName = "PlanProposalStatus",
                        UserId = userID
                    };
                    plan.ProposalStatusNavigation = log;
                    if (forceSubmitDB)
                    {
                        await DB.SaveChangesAsync();
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
        public async Task<ReturnMessage> UpdatePlanFlowStatus(ProposalModel p,bool isProposing , string userID)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                //var save = await UpdatePlanFlowStatus(p.ProposalHeader, userID, false , ServiceModels.Constants.LOVGroup.Proposal_Status._LOVGroupCode, isProposing);
                //if (!save.IsCompleted)
                //{
                //    ret.CloneMessage(save.Message);
                //    return ret;
                //}
                var save = await UpdatePlanFlowStatus(p.NewDebtPlan, userID, false, ServiceModels.Constants.LOVGroup.Plan_of_Proposal_Status._LOVGroupCode, isProposing);
                if (!save.IsCompleted)
                {
                    ret.CloneMessage(save.Message);
                    return ret;
                }
                save = await UpdatePlanFlowStatus(p.FiveYearPlan, userID, false, ServiceModels.Constants.LOVGroup.Plan_of_Proposal_Status._LOVGroupCode, isProposing);
                if (!save.IsCompleted)
                {
                    ret.CloneMessage(save.Message);
                    return ret;
                }
                 save = await UpdatePlanFlowStatus(p.ExistingDebtPlan, userID, false, ServiceModels.Constants.LOVGroup.Plan_of_Proposal_Status._LOVGroupCode, isProposing);
                if (!save.IsCompleted)
                {
                    ret.CloneMessage(save.Message);
                    return ret;
                }
                save = await UpdatePlanFlowStatus(p.FinancialReport, userID, false, ServiceModels.Constants.LOVGroup.Plan_of_Proposal_Status._LOVGroupCode, isProposing);
                if (!save.IsCompleted)
                {
                    ret.CloneMessage(save.Message);
                    return ret;
                }
       

                await DB.SaveChangesAsync();
      
                var up = await UpdateProposalStatus(p.ProposalHeader.PlanID, userID);
                if (!up.IsCompleted)
                {
                    ret.CloneMessage(up.Message);
                    return ret;
                }
                ret.IsCompleted = true;

            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }

        public async  Task<ReturnList<ProposalModel>> GetProposalStatusList(ProposalListParameter p)
        {
            var ret = new ReturnList<ProposalModel>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                //var sql = DB.ProposalPlan
                //    .WhereIf(p.ProposalID.HasValue, w => w.ProposalId == p.ProposalID.Value)
                //    .WhereIf(p.OrganizationID.HasValue, o => o.Proposal.OrganizationId == p.OrganizationID.Value)
                //    .WhereIf(p.StartYear.HasValue, y => y.Proposal.StartYear == p.StartYear.Value)
                //    .WhereIf(!string.IsNullOrEmpty(p.PlanRelease), r => r.Proposal.PlanReleaseNavigation.Lovcode == p.PlanRelease)
                //    .GroupBy(g => g.ProposalId)
                //    .Select(s => new { proposalID = s.Key  });
                var sql = DB.Plan
                    .WhereIf(p.ProposalID.HasValue, w => w.PlanId == p.ProposalID.Value)
                    .WhereIf(p.OrganizationID.HasValue, o => o.OrganizationId == p.OrganizationID.Value)
                    .WhereIf(p.StartYear.HasValue, y => y.StartYear == p.StartYear.Value)
                    .WhereIf(!string.IsNullOrEmpty(p.PlanRelease), r => r.PlanReleaseNavigation.Lovcode == p.PlanRelease)
                    .WhereIf(!string.IsNullOrEmpty(p.ProposalStatus), ps => ps.ProposalStatusNavigation.LogStatusNavigation.Lovcode == p.ProposalStatus)
                    .Where(w => w.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.ข้อเสนอแผนบริหารหนี้);
                    //.GroupBy(g => g.ProposalId)
                    //.Select(s => new { proposalID = s.Key });

                ret.TotalRow = await sql.CountAsync();
                ret.PageSize = p.Paging.PageSize;
                ret.PageNo = p.Paging.PageNo;
                var pros = await sql.PageBy(sort => sort.PlanId, p.Paging.PageNo, p.Paging.PageSize, true).ToListAsync();
                var data = new List<ProposalModel>();
                foreach (var prop in pros)
                {
                    var proposal = await GetProposal(prop.PlanId);
                    if (proposal.IsCompleted)
                    {
                        data.Add(proposal.Data);                         
                    }
                        
                     
                    //var pl = await DB.Plan.Include(r => r.PlanReleaseNavigation)
                    //    .Include(st => st.ProposalStatusNavigation).ThenInclude(l => l.LogStatusNavigation)
                    //    .Where(w => w.PlanId == prop.proposalID).FirstOrDefaultAsync();
                    //var plist = new ProposalModel
                    //{
                    //    ProposalID = prop.proposalID.Value,
                    //    Release = pl.PlanReleaseNavigation.Lovvalue,
                    //    Year = pl.StartYear,
                    //    ProposalStatus = (pl.ProposalStatus.HasValue) ? pl.ProposalStatusNavigation.LogStatusNavigation.Lovvalue : ""
                    //};
                    //data.Add(plist);
                    //foreach (var plan in prop.plans)
                    //{
                    //    pl = await DB.Plan.Include(t => t.PlanTypeNavigation)
                    //        .Include(st => st.ProposalStatusNavigation).ThenInclude(l => l.LogStatusNavigation)
                    //        .Where(w => w.PlanId == plan.PlanId).FirstOrDefaultAsync();
                    //    var planstatus = (pl.ProposalStatus.HasValue) ? pl.ProposalStatusNavigation.LogStatusNavigation.Lovvalue : "";
                    //    if (pl.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.รายงานสถานะทางการเงินและภาระหนี้)
                    //    {
                    //        plist.FinRepStatus = planstatus;
                    //    }
                    //    if (pl.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.แผน_5_ปี)
                    //    {
                    //        plist.FiveYearStatus = planstatus;
                    //    }
                    //    if (pl.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.แผนก่อหนี้ใหม่)
                    //    {
                    //        plist.NewDebtStatus = planstatus;
                    //    }
                    //    if (pl.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.แผนบริหารหนี้เดิม)
                    //    {
                    //        plist.ExistDebtStatus = planstatus;
                    //    }
                    //}
                  

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


    }
}
