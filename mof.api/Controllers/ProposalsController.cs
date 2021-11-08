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
using mof.ServiceModels.Agreement;
using mof.ServiceModels.Proposal;

namespace mof.api.Controllers
{
    /// <summary>
    /// Proposals
    /// </summary>
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public class ProposalsController : Controller
    {
        private IOrganization _iorg;
        private UserManager<ApplicationUser> _user;
        SignInManager<ApplicationUser> _sign;
        IStringLocalizer<MessageLocalizer> _msglocalizer;
        RoleManager<ApplicationRole> _role;
        private ISystemHelper _helper;
        private IPlan _plan;
        private IAgreement _agr;

        public ProposalsController(IOrganization iorg, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IStringLocalizer<MessageLocalizer> msglocalizer,
            RoleManager<ApplicationRole> role,
            ISystemHelper helper,
            IPlan plan,
            IAgreement agr)
        {

            _iorg = iorg;
            _user = userManager;
            _sign = signInManager;
            _msglocalizer = msglocalizer;
            _role = role;
            _helper = helper;
            _plan = plan;
            _agr = agr;
           
        }
        /// <summary>
        /// แสดงรายการ แผนนำเสนอ
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost("list")]
        public async Task<ActionResult<ReturnList<PlanHeader>>> List(PlanListParameter p)
        {
            var ret = new ReturnList<PlanHeader>(_msglocalizer);
            try
            {

                ret = await _plan.GetPlanList(ServiceModels.Constants.LOVGroup.Plan_Type.ข้อเสนอแผนบริหารหนี้, p, null);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// Get proposal
        /// </summary>
        /// <param name="proposalID"></param>
        /// <returns></returns>
        [HttpGet("{proposalID}")]
        public async Task<ActionResult<ReturnObject<ProposalModel>>> Get([FromRoute][Required]long proposalID)
        {
            var ret = new ReturnObject<ProposalModel>(_msglocalizer);
            try
            {
                ret = await _plan.GetProposal(proposalID);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// สร้าง proposal ใหม่
        /// </summary>
        /// <param name="p"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ReturnObject<long?>>> Create([FromBody]CreatePlanParameter p, [FromHeader][Required] string userID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            try
            {
                ret = await _plan.CreatePlan(userID, ServiceModels.Constants.LOVGroup.Plan_Type.ข้อเสนอแผนบริหารหนี้, p);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// เพิ่มแผนเข้าไปใน ข้อเสนอ : proposalID = Data ที่ได้มาจาก post v1/proposals : planID คือแผนที่จะเพิ่มเข้าไป
        /// </summary>
        /// <param name="proposalID"></param>
        /// <param name="planID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpPut("{proposalID}/plan/{planID}")]
        public async Task<ActionResult<ReturnMessage>> AddPlan(
            
            [FromRoute]
            long proposalID, 
            [FromRoute]long planID,[FromHeader][Required]string userID)
        {
            var ret = new ReturnMessage(_msglocalizer);
            try
            {
                ret = await _plan.AddPlanToProposal(proposalID, planID, userID);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// ลบแผนใน ข้อเสนอ : proposalID = Data ที่ได้มาจาก post v1/proposals : planType = ประเภทแผนที่จะลบ ( LOVGroupDoce = "PLTYPE" )
        /// </summary>
        /// <param name="proposalID"></param>
        /// <param name="planType"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpDelete("{proposalID}/plan/{planType}")]
        public async Task<ActionResult<ReturnMessage>> RemovePlan(

            [FromRoute]
            long proposalID,
            [FromRoute]string planType, [FromHeader][Required]string userID)
        {
            var ret = new ReturnMessage(_msglocalizer);
            try
            {
                ret = await _plan.RemovePlanFromProposal(proposalID, planType, userID);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }

        /// <summary>
        /// <br>update สถานะแผนใน ข้อเสนอแผน ค่าที่จะไป update คือ planid ใน planheader ของแต่ละตัว </br> 
        /// ส่วนสถานะ อยู่ใน PlanHeader.PlanStatusLog
        /// </summary>
        /// <param name="p"></param>
        /// <param name="userID"></param>
        /// <param name="isProposing"></param>
        /// <returns></returns>
        [HttpPut("updatestatus")]
        public async Task<ActionResult<ReturnMessage>> UpdatePlanFlowStatus([FromBody][Required]ProposalModel p,[FromHeader][Required]string userID, [FromQuery]bool? isProposing)
        {
            var ret = new ReturnMessage(_msglocalizer);
            try
            {
                var ispp = isProposing.HasValue ? isProposing.Value : false;
                ret = await _plan.UpdatePlanFlowStatus(p,ispp,userID);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// แสดงรายการ สถานะของแผน ใน proposal
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost("planstatuslist")]
        public async Task<ActionResult<ReturnList<ProposalModel>>> GetProposalStatusList(ProposalListParameter p)
        {
            var ret = new ReturnList<ProposalModel>(_msglocalizer);
            try
            {
                ret = await _plan.GetProposalStatusList(p);
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