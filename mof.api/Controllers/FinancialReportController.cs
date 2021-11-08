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
using mof.ServiceModels.FinancialReport;

namespace mof.api.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public class FinancialReportController : Controller
    {
        private IOrganization _iorg;
        private UserManager<ApplicationUser> _user;
        SignInManager<ApplicationUser> _sign;
        IStringLocalizer<MessageLocalizer> _msglocalizer;
        RoleManager<ApplicationRole> _role;
        //private IFinancialReport _fin;
        private ISystemHelper _helper;
        private IPlan _plan;
        public FinancialReportController(IOrganization iorg, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IStringLocalizer<MessageLocalizer> msglocalizer,
            RoleManager<ApplicationRole> role,
            ISystemHelper helper,
            IPlan plan
           )
        {

            _iorg = iorg;
            _user = userManager;
            _sign = signInManager;
            _msglocalizer = msglocalizer;
            _role = role;
            _helper = helper;
            _plan = plan;
           // _fin = fin;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportID"></param>
        /// <returns></returns>
        [HttpGet("{reportID}")]
        public async Task<ActionResult<ReturnObject<FinancialReportModel>>> Get([FromRoute][Required]long reportID)
        {
            var ret = new ReturnObject<FinancialReportModel>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                ret = await _plan.GetFinPlan(reportID);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// สร้างรายงานสถานะทางการเงินและภาระหนี้
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

                ret = await _plan.CreatePlan(UserID, ServiceModels.Constants.LOVGroup.Plan_Type.รายงานสถานะทางการเงินและภาระหนี้, p);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// รายการรายงานสถานะทางการเงินและภาระหนี้
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost("list")]
        public async Task<ActionResult<ReturnList<PlanHeader>>> List(PlanListParameter p)
        {
            var ret = new ReturnList<PlanHeader>(_msglocalizer);
            try
            {

                ret = await _plan.GetPlanList(ServiceModels.Constants.LOVGroup.Plan_Type.รายงานสถานะทางการเงินและภาระหนี้, p, null);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// แก้ไข รายงานสถานะทางการเงินและภาระหนี้
        /// </summary>
        /// <param name="PlanID"></param>
        /// <param name="p"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpPut("{PlanID}")]
        public async Task<ActionResult<ReturnObject<long?>>> Update([FromRoute]long PlanID, [FromBody]FinancialReportModel p, [FromHeader][Required]string userID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            try
            {

                ret = await _plan.ModifyFinRep(p, userID, PlanID);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);

        }
        /// <summary>
        /// financial report summary
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost("debtsummary")]
        public async Task<ActionResult<ReturnObject<Debt>>> GetFinPlanDebtSummary([FromBody]PlanProjectListParameter p)
        {
            var ret = new ReturnObject<Debt>(_msglocalizer);
            try
            {

                ret = await _plan.GetFinPlanDebtSummary(p, eGetPlanType.GetSummary);
            
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