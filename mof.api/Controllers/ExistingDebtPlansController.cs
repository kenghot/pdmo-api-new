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
    /// แผนบริหารหนี้เดิม
    /// </summary>
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public class ExistingDebtPlansController : Controller
    {
        private IOrganization _iorg;
        private UserManager<ApplicationUser> _user;
        SignInManager<ApplicationUser> _sign;
        IStringLocalizer<MessageLocalizer> _msglocalizer;
        RoleManager<ApplicationRole> _role;
        private ISystemHelper _helper;
        private IPlan _plan;
        public ExistingDebtPlansController(IOrganization iorg, UserManager<ApplicationUser> userManager,
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
        ///  S140 section B  แผนบริหารหนี้ประจำปีงบประมาณ (แผนบริหารหนี้เดิม)
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost("list")]
        public async Task<ActionResult<ReturnList<PlanHeader>>> List(PlanListParameter p)
        {
            var ret = new ReturnList<PlanHeader>(_msglocalizer);
            try
            {

                ret = await _plan.GetPlanList(ServiceModels.Constants.LOVGroup.Plan_Type.แผนบริหารหนี้เดิม, p, null);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// สร้างแผนบริหารหนี้เดิม
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

                ret = await _plan.CreatePlan(UserID, ServiceModels.Constants.LOVGroup.Plan_Type.แผนบริหารหนี้เดิม, p);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// S142 รายละเอียดแผนหนี้เดิม
        /// </summary>
        /// <param name="PlanID"></param>
        /// <returns></returns>
        [HttpGet("{PlanID}")]
        public async Task<ActionResult<ReturnObject<ExistingDebtPlanModel>>> GetExistDebtPlan([FromRoute]long PlanID)
        {
            var ret = new ReturnObject<ExistingDebtPlanModel>(_msglocalizer);
            try
            {
                ret = await _plan.GetExistDebtPlan(PlanID,"P",null);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);

        }

        /// <summary>
        /// S142 รายละเอียดแผนหนี้เดิม
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost("summary")]
        public async Task<ActionResult<ReturnObject<ExistingDebtPlanSummary>>> GetExistDebtSummary([FromBody]PlanProjectListParameter p)
        {
            var ret = new ReturnObject<ExistingDebtPlanSummary>(_msglocalizer);
            try
            {
              
                var data = await _plan.GetExistDebtPlan(p, eGetPlanType.GetSummary,"P",null);
                if (!data.IsCompleted)
                {
                    ret.CloneMessage(data.Message);
                    return Ok(ret);
                }
                ret.Data = data.Data.Data.PlanSummary;
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);

        }
        /// <summary>
        /// S142 รายละเอียดแผนหนี้เดิม
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost("agreementlist")]
        public async Task<ActionResult<ReturnList<ExistingDebtPlanDetails>>> AgreementList([FromBody]PlanProjectListParameter p)
             //public async Task<ActionResult<ReturnList<ExistingDebtPlanDetails>>> AgreementList([FromBody]PlanProjectListParameter p)
        {
            var ret = new ReturnList<ExistingDebtPlanDetails>(_msglocalizer);
            try
            {

                var data = await _plan.GetExistDebtPlan(p, eGetPlanType.Search, "P", null);
                if (!data.IsCompleted)
                {
                    ret.CloneMessage(data.Message);
                    return Ok(ret);
                }
                //Todo New ExistingPlan effect
                ret.Data = data.Data.Data.PlanDetails;
                ret.PageNo = p.Paging.PageNo;
                ret.PageSize = p.Paging.PageSize;
                ret.TotalRow = data.Data.TotalRec;
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);

        }
        /// <summary>
        /// M198SearchPlanDialog tab 2
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost("planRestructureList")]
        public async Task<ActionResult<ReturnList<ExistPlanAgreementList>>> GetPlanAgreementList([FromBody]PlanProjectListParameter p)
        {
            var ret = new ReturnList<ExistPlanAgreementList>(_msglocalizer);
            try
            {

                ret = await _plan.GetPlanAgreementList(p);
             
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);

        }
        /// <summary>
        /// แก้ไข รายละเอียดแผนหนี้เดิม
        /// </summary>
        /// <param name="PlanID"></param>
        /// <param name="p"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpPut("{PlanID}")]
        public async Task<ActionResult<ReturnObject<long?>>> Update([FromRoute]long PlanID,[FromBody]ExistingDebtPlanModel p,[FromHeader][Required]string userID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            try
            {

                ret = await _plan.ModifyExistDebtPlan(p, userID, PlanID, "P",null);
            }
            catch (Exception ex)
            {
                return StatusCode(200, Helper.ThrowException(ex, _msglocalizer));
               // return StatusCode(200, ex);
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