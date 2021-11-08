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
    /// แผนก่อหนี้ใหม่
    /// </summary>
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public class NewDebtPlansController : Controller
    {
        private IOrganization _iorg;
        private UserManager<ApplicationUser> _user;
        SignInManager<ApplicationUser> _sign;
        IStringLocalizer<MessageLocalizer> _msglocalizer;
        RoleManager<ApplicationRole> _role;
        private ISystemHelper _helper;
        private IPlan _plan;
        public NewDebtPlansController(IOrganization iorg, UserManager<ApplicationUser> userManager,
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
        ///  S140 section A  แผนบริหารหนี้ประจำปีงบประมาณ (แผนก่อหนี้ใหม่)
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost("list")]
        public async Task<ActionResult<ReturnList<PlanHeader>>> List(PlanListParameter p)
        {
            var ret = new ReturnList<PlanHeader>(_msglocalizer);
            try
            {

                ret = await _plan.GetPlanList(ServiceModels.Constants.LOVGroup.Plan_Type.แผนก่อหนี้ใหม่, p, null);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// สร้างแผนก่อหนี้ใหม่
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ReturnObject<long?>>> Create([FromHeader]string UserID, [FromBody]CreatePlanParameter p)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            try
            {

                ret = await _plan.CreatePlan(UserID, ServiceModels.Constants.LOVGroup.Plan_Type.แผนก่อหนี้ใหม่, p);
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
            [FromQuery]
            [Description("ประเภทโครงการ LOVGroupCode = PJTYPE")]
            string projType,
            [FromHeader][Required]string userID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            try
            {
                ret = await _plan.AddProjectToPlan(planID, projID, projType,true, userID);
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
        /// รายละเอียดแผนก่อหนี้ใหม่
        /// </summary>
        /// <returns></returns>
        [HttpGet("{planID}")]
        public async Task<ActionResult<ReturnObject<NewDebtPlanModel>>> GetPlanSummary([FromRoute]long? planID)
        {    
            var ret = new ReturnObject<NewDebtPlanModel>(_msglocalizer);
            try
            {

                ret = await _plan.GetNewDebtPlan(planID,"P",null);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// แก้ไข แผน
        /// </summary>
        /// <returns></returns>
        [HttpPut("{PlanID}")]
        public async Task<ActionResult<ReturnMessage>> UpdatePlanLoan(NewDebtPlanModel p, [FromHeader][Required]string UserID,
             [FromRoute][Required]long PlanID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            try
            {

                ret = await _plan.ModifyNewDebtPlan(p, UserID, 0, PlanID,"P",null);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// แสดงรายการโครงการ  
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost("projectlist")]
        public async Task<ActionResult<ReturnList<NewDebtPlanDetails>>> GetPlanProjectList(PlanProjectListParameter p)
        {
            var ret = new ReturnList<NewDebtPlanDetails>(_msglocalizer);
            try
            {

                ret = await _plan.GetNewPlanProjectList(p);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// แสดงยอดรวมเงินกู้ : paging ไม่ได้ใช้ 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost("summary")]
        public async Task<ActionResult<ReturnObject<NewDebtPlanSummary>>> Summary(PlanProjectListParameter p)
        {
            var ret = new ReturnObject<NewDebtPlanSummary>(_msglocalizer);
            try
            {

                ret = await _plan.GetNewPlanSummary(p);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// แสดงรายการของลักษณะงาน  
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost("activitylist")]
        public async Task<ActionResult<ReturnList<NewDebtPlanActList>>> GetPlanActivityList(PlanProjectListParameter p)
        {
            var ret = new ReturnList<NewDebtPlanActList>(_msglocalizer);
            try
            {

                ret = await _plan.GetNewDebtActList(p);
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
                ret = await _plan.DeletePlan(planID, userID);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }

    }
}