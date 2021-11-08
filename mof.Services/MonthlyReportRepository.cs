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
using mof.ServiceModels.MonthlyReport;

namespace mof.Services
{
    public partial class PlanRepository  
    {

        public async Task<ReturnObject<long?>> CreateMonthRep(CreatePlanParameter p, string userID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                if (!p.Month.HasValue)
                {
                    ret.AddMessage(eMessage.PleaseEnterValue.ToString(), "month", eMessageType.Error, new string[] { _msglocalizer[eMessage.Month.ToString()] });
                    return ret;
                }
                if (p.Month.Value <=0 || p.Month.Value > 12)
                {
                    ret.Message.Add(new MessageData
                    {
                        Code = "Month1-12",
                        Message = "Month allow 1-12",
                        MessageType =  eMessageType.Error.ToString()
                    });
                    return ret;
                }
                var newSource = DB.Plan.Include(r => r.PlanReleaseNavigation).Where(w => w.OrganizationId == p.OrganizationID && w.StartYear == p.StartYear &&
                w.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.แผนก่อหนี้ใหม่)
                .OrderByDescending(o => o.PlanId).FirstOrDefault();
                if (newSource == null)
                {
                    ret.Message.Add(new MessageData {
                        Code = "CreatMRepNewDept",
                        Message = "ไม่พบแผนก่อหนี้ใหม่ของหน่วยงาน ในปีที่กำหนด",
                        MessageType = eMessageType.Error.ToString()
                    });
                    return ret;
                }
                var exSource = DB.Plan.Include(r => r.PlanReleaseNavigation).Where(w => w.OrganizationId == p.OrganizationID && w.StartYear == p.StartYear &&
                w.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.แผนบริหารหนี้เดิม)
                .OrderByDescending(o => o.PlanId).FirstOrDefault();

                if (exSource == null)
                {
                    ret.Message.Add(new MessageData
                    {
                        Code = "CreatMRepExistDept",
                        Message = "ไม่พบแผนบริหารหนี้เดิมของหน่วยงาน ในปีที่กำหนด",
                        MessageType = eMessageType.Error.ToString()
                    });
                    return ret;
                }
                p.PlanRelease = ServiceModels.Constants.LOVGroup.Plan_Release.จัดทำแผนฯ_ตั้งต้น;
                var mth = await CreatePlan(userID  , ServiceModels.Constants.LOVGroup.Plan_Type.รายงานประจำเดือน, p);
                if (!mth.IsCompleted)
                {
                    ret.CloneMessage(mth.Message);
                    return ret;
                }
                //p.PlanRelease = newSource.PlanReleaseNavigation.Lovcode;
                //var mthNew = await CreatePlan(userID, ServiceModels.Constants.LOVGroup.Plan_Type.รายงานประจำเดือน__แผนก่อหนี้ใหม่_, p);
                //if (!mthNew.IsCompleted)
                //{
                //    mthNew.CloneMessage(mthNew.Message);
                //    return ret;
                //}
                //p.PlanRelease = exSource.PlanReleaseNavigation.Lovcode;
                //var mthExist = await CreatePlan(userID, ServiceModels.Constants.LOVGroup.Plan_Type.รายงานประจำเดือน__แผนบริหารหนี้เดิม_, p);
                //if (!mthExist.IsCompleted)
                //{
                //    ret.CloneMessage(mthExist.Message);
                //    return ret;
                //}
                #region dup newdebt
                
                //var dupNews = await DB.PlanProject.Include(a => a.PlanAct).Where(w => w.PlanId == newSource.PlanId).ToListAsync();
              
                //foreach (var dupNew in dupNews)
                //{
                //    var ppj = new PlanProject
                //    {
                //        PlanId = mthNew.Data.Value,
                //        ProjectId = dupNew.ProjectId,
                //        ProjectType = dupNew.ProjectType
                //    };
                //    DB.PlanProject.Add(ppj);
                //    foreach (var act in dupNew.PlanAct)
                //    {
                //        ppj.PlanAct.Add(new PlanAct
                //        {
                //            ActivityName = act.ActivityName,
                //            ProjActId = 0
                //        });
                //    }
                //}
                #endregion
                //#region dup exist
                //var exData = await GetExistDebtPlan(exSource.PlanId);
                //if (exData.IsCompleted)
                //{
                //    var exUpdate = await ModifyExistDebtPlan(exData.Data, userID, mthExist.Data.Value, "R",null);
                //}
                //#endregion
                //var newmth = new MonthRep { MonthRepId = mth.Data.Value, NewDebtId = mthNew.Data, ExistDebtId = mthExist.Data };
                //DB.MonthRep.Add(newmth);
                //await DB.SaveChangesAsync();
                ret.IsCompleted = true;
                ret.Data = mth.Data;

            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }

        public async Task<ReturnObject<long?>> ModifyMonthRep(MonthlyReportModel p,long planID, string userID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var mth = await DB.Plan.Where(w => w.PlanId == planID).FirstOrDefaultAsync();
                if (mth == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "plan", eMessageType.Error, new string[] { _msglocalizer[eMessage.Plan.ToString()] });
                    return ret;
                }
                var newSource = DB.Plan.Include(r => r.PlanReleaseNavigation).Where(w => w.OrganizationId == mth.OrganizationId && w.StartYear == mth.StartYear &&
                w.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.แผนก่อหนี้ใหม่)
                .OrderByDescending(o => o.PlanId).FirstOrDefault();
                if (newSource == null)
                {
                    ret.Message.Add(new MessageData
                    {
                        Code = "CreatMRepNewDept",
                        Message = "ไม่พบแผนก่อหนี้ใหม่ของหน่วยงาน ในปีที่กำหนด",
                        MessageType = eMessageType.Error.ToString()
                    });
                    return ret;
                }
                var exSource = DB.Plan.Include(r => r.PlanReleaseNavigation).Where(w => w.OrganizationId == mth.OrganizationId && w.StartYear == mth.StartYear &&
                w.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.แผนบริหารหนี้เดิม)
                .OrderByDescending(o => o.PlanId).FirstOrDefault();

                if (exSource == null)
                {
                    ret.Message.Add(new MessageData
                    {
                        Code = "CreatMRepExistDept",
                        Message = "ไม่พบแผนบริหารหนี้เดิมของหน่วยงาน ในปีที่กำหนด",
                        MessageType = eMessageType.Error.ToString()
                    });
                    return ret;
                }
                ExistingDebtPlanModel ex = new ExistingDebtPlanModel
                {
                    PlanHeader = p.ExistingDebtPlan,
                    //Todo New ExistingPlan effect
                    PlanDetails = p.ExistingDebtPlanReportDetails
                };
                //var updateEx = await ModifyExistDebtPlan(ex, userID, mth.ExistDebtId.Value,"R",null);
                //if (!updateEx.IsCompleted)
                //{
                //    ret.CloneMessage(updateEx.Message);
                //    return ret;
                //}

                NewDebtPlanModel n = new NewDebtPlanModel
                {
                    PlanHeader = p.NewDebtPlan,
                    PlanDetails = p.NewDebtPlanReportDetails
                };
                var updateNew = await ModifyNewDebtPlan(n, userID, 0,newSource.PlanId,"R",mth.Month.Value);
                if (!updateNew.IsCompleted)
                {
                    ret.CloneMessage(updateNew.Message);
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
        public async Task<ReturnObject<MonthlyReportModel>> GetMonthRep(long planID)
        {
            var ret = new ReturnObject<MonthlyReportModel>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                //var mth = await DB.MonthRep.Where(w => w.MonthRepId == ProjID).FirstOrDefaultAsync();
                //if (mth == null)
                //{
                //    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "plan", eMessageType.Error, new string[] { _msglocalizer[eMessage.Plan.ToString()] });
                //    return ret;
                //}
                var mthHD = await GetPlanHeaderByID(ServiceModels.Constants.LOVGroup.Plan_Type.รายงานประจำเดือน, planID);
                if (!mthHD.IsCompleted)
                {
                    ret.CloneMessage(mthHD.Message);
                    return ret;
                }
                var newSource = DB.Plan.Include(r => r.PlanReleaseNavigation).Where(w => w.OrganizationId == mthHD.Data.Organization.ID && w.StartYear == mthHD.Data.PlanYear &&
                w.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.แผนก่อหนี้ใหม่)
                .OrderByDescending(o => o.PlanId).FirstOrDefault();
                if (newSource == null)
                {
                    ret.Message.Add(new MessageData
                    {
                        Code = "CreatMRepNewDept",
                        Message = "ไม่พบแผนก่อหนี้ใหม่ของหน่วยงาน ในปีที่กำหนด",
                        MessageType = eMessageType.Error.ToString()
                    });
                    return ret;
                }
                var exSource = DB.Plan.Include(r => r.PlanReleaseNavigation).Where(w => w.OrganizationId == mthHD.Data.Organization.ID && w.StartYear == mthHD.Data.PlanYear &&
                w.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.แผนบริหารหนี้เดิม)
                .OrderByDescending(o => o.PlanId).FirstOrDefault();

                if (exSource == null)
                {
                    ret.Message.Add(new MessageData
                    {
                        Code = "CreatMRepExistDept",
                        Message = "ไม่พบแผนบริหารหนี้เดิมของหน่วยงาน ในปีที่กำหนด",
                        MessageType = eMessageType.Error.ToString()
                    });
                    return ret;
                }
                var newPl = await GetNewDebtPlan(newSource.PlanId,"R", mthHD.Data.Month);
                if (!newPl.IsCompleted)
                {
                    ret.CloneMessage(newPl.Message);
                    return ret;
                }
         
                var exPl = await GetExistDebtPlan(exSource.PlanId, "R", mthHD.Data.Month);
                if (!exPl.IsCompleted)
                {
                    ret.CloneMessage(exPl.Message);
                    return ret;
                }
             

                ret.Data = new MonthlyReportModel
                {
                    ExistingDebtPlan = exPl.Data.PlanHeader,
                    //Todo New ExistingPlan effect
                    ExistingDebtPlanReportDetails = exPl.Data.PlanDetails,
                    PlanHeader = mthHD.Data,
                    NewDebtPlan = newPl.Data.PlanHeader,
                    NewDebtPlanReportDetails = newPl.Data.PlanDetails,
                };
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        public async Task<ReturnList<MonthlyReportModel>> ListMonthRep(MonthlyReportParameter p)
        {
            var ret = new  ReturnList<MonthlyReportModel> (_msglocalizer);
            ret.IsCompleted = false;
            try
            {
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
