using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mof.ServiceModels;
using mof.ServiceModels.Common;
using mof.ServiceModels.Response;
using mof.ServiceModels.Request;
using mof.DataModels.Models;
using Microsoft.AspNetCore.Authorization;
using mof.IServices;
using mof.Services;
using Microsoft.AspNetCore.Identity;
using System.Net.Http;
using IdentityModel.Client;
using System.Globalization;
using Microsoft.Extensions.Localization;
using System.Threading;
using mof.ServiceModels.Identity;
using System.Security.Claims;
using IdentityServer4.AccessTokenValidation;
using mof.ServiceModels.Organization;
using System.ComponentModel.DataAnnotations;
using mof.ServiceModels.Common.Generic;
using mof.ServiceModels.Plan;
using System.ComponentModel;

namespace mof.api.Controllers
{
    /// <summary>
    /// เงินกู้ระยะปานกลาง (5ปี)
    /// </summary>
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public class FiveYearPlansController : Controller
    {
        private IOrganization _iorg;
        private UserManager<ApplicationUser> _user;
        SignInManager<ApplicationUser> _sign;
        IStringLocalizer<MessageLocalizer> _msglocalizer;
        RoleManager<ApplicationRole> _role;
        private ISystemHelper _helper;
        private IPlan _plan;
        public FiveYearPlansController(IOrganization iorg, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IStringLocalizer<MessageLocalizer> msglocalizer,
            RoleManager<ApplicationRole> role,
            ISystemHelper helper,
            IPlan plan)
        {

            _iorg = iorg;
            _user = userManager;
            _sign = signInManager;
            _msglocalizer = msglocalizer;
            _role = role;
            _helper = helper;
            _plan = plan;
        }
        /// <summary>
        /// รายละเอียดแผน 5 ปี S132 section A / B
        /// </summary>
        /// <returns></returns>
        [HttpGet("{planID}/summary")]
        public async Task<ActionResult<ReturnObject<Plan5YDetail>>> GetPlanSummary([FromRoute]long? planID,
        [FromQuery]
        [Required]
        [Description("ตั้งแต่ปี (ปีเริ่มต้น)")]
        int startYear)
        {
            var ret = new ReturnObject<Plan5YDetail>(_msglocalizer);
            try
            {
                ret = await _plan.GetPlanDetail(ServiceModels.Constants.LOVGroup.Plan_Type.แผน_5_ปี, startYear, planID,null,"");
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// S135: 5 Year plan Summary Dashboard
        /// </summary>
        /// <returns></returns>
        [HttpGet("summary")]
        public async Task<ActionResult<ReturnObject<Plan5YDetail>>> GetPlanSummary([FromQuery]long? planID, 
        [FromQuery]
        [Required]
        [Description("ตั้งแต่ปี (ปีเริ่มต้น)")]
        int startYear,
        [FromQuery]
        [Description("organization id ไม่ใส่คือเอาหมด")]
        long? orgID,
        [FromQuery]
        [Description("LOVGROUPCODE = PLR ไม่ใส่คือเอาหมด")]
        string PlanRelease 
            )
        {
            var ret = new ReturnObject<Plan5YDetail>(_msglocalizer);
            try
            {
                ret = await _plan.GetPlanDetail(ServiceModels.Constants.LOVGroup.Plan_Type.แผน_5_ปี, startYear, planID,orgID,PlanRelease);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// S130 รายการแผนความต้องการเงินกู้ระยะปานกลาง (5ปี)
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost("list")]
        public async Task<ActionResult<ReturnList<PlanHeader>>> List(PlanListParameter p)
        {
            var ret = new ReturnList<PlanHeader>(_msglocalizer);
            try
            {

                ret = await _plan.GetPlanList(ServiceModels.Constants.LOVGroup.Plan_Type.แผน_5_ปี, p,null);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// สร้างแผน 5 ปี
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ReturnObject<long?>>> Create([FromHeader]string UserID,[FromBody]CreatePlanParameter p)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            try
            {

                ret = await _plan.CreatePlan(UserID, ServiceModels.Constants.LOVGroup.Plan_Type.แผน_5_ปี, p);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// แก้ไข เพิ่มเติม แหล่งเงินกู้ของแผน
        /// </summary>
        /// <returns></returns>
        [HttpPost("{PlanID}/Projects/{ProjectID}/loansources")]
        public async Task<ActionResult<ReturnMessage>> UpdatePlanLoan(PlanProjectSource p,[FromHeader][Required]string UserID,
            [FromRoute][Required]long ProjectID, [FromRoute][Required]long PlanID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            try
            {

                ret = await _plan.ModifyPlanProject(p, UserID, ProjectID, PlanID);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// แสดงรายการโครงการ S132 section C
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost("projectlist")]
        public async Task<ActionResult<ReturnList<PlanProjectList>>> GetPlanProjectList(PlanProjectListParameter p)
        {
            var ret = new ReturnList<PlanProjectList>(_msglocalizer);
            try
            {

                ret = await _plan.GetPlanProjectList(ServiceModels.Constants.LOVGroup.Plan_Type.แผน_5_ปี, p);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// เพิ่มโครงการเข้าไปในแผน
        /// </summary>
        /// <param name="projID"></param>
        /// <param name="planID"></param>
        /// <param name="userID"></param>
        /// <param name="projType">ประเภทโครงการ LOVGroupCode = PJTYPE</param>
        /// <returns></returns>
        [HttpPut("{planID}/project/{projID}")]
        public async Task<ActionResult<ReturnObject<long?>>> AddProjToPlan(
            [FromRoute][Required]long projID, [FromRoute][Required]long planID,
            [FromQuery][Required]
            [Description("ประเภทโครงการ LOVGroupCode = PJTYPE")]
            string projType, 
            [FromHeader][Required]string userID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            try
            {
                ret = await _plan.AddProjectToPlan(planID, projID, projType, false, userID);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// ลบ โครงการออกจากแผน
        /// </summary>
        /// <param name="projID"></param>
        /// <param name="planID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpDelete("{planID}/project/{projID}")]
        public async Task<ActionResult<ReturnMessage>> AddProjToPlan(
            [FromRoute][Required]long projID, [FromRoute][Required]long planID,
            [FromHeader][Required]string userID)
        {
            var ret = new ReturnMessage(_msglocalizer);
            try
            {
                ret = await _plan.RemoveProjectFromPlan(planID, projID, userID);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// ลบ แผน
        /// </summary>
        /// <param name="planID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpDelete("{planID}")]
        public async Task<ActionResult<ReturnMessage>> DeletePlan(
            [FromRoute][Required]long planID,
            [FromHeader][Required]string userID)
        {
            var ret = new ReturnMessage(_msglocalizer);
            try
            {
                ret = await _plan.DeletePlan(planID,  userID);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
    }
}
