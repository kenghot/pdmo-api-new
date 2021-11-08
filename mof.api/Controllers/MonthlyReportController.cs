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
using mof.ServiceModels.MonthlyReport;

namespace mof.api.Controllers
{
    /// <summary>
    /// สัญญา
    /// </summary>
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public class MonthlyReportsController : Controller
    {
        private IOrganization _iorg;
        private UserManager<ApplicationUser> _user;
        SignInManager<ApplicationUser> _sign;
        IStringLocalizer<MessageLocalizer> _msglocalizer;
        RoleManager<ApplicationRole> _role;
        private ISystemHelper _helper;
        private IPlan _plan;
        private IAgreement _agr;
 
        public MonthlyReportsController(IOrganization iorg, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IStringLocalizer<MessageLocalizer> msglocalizer,
            RoleManager<ApplicationRole> role,
            ISystemHelper helper,
            IPlan plan,
            IAgreement agr
        )
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
        /// Get monthly report
        /// </summary>
        /// <param name="monthlyID"></param>
        /// <returns></returns>
        [HttpGet("{monthlyID}")]
        public async Task<ActionResult<ReturnObject<MonthlyReportModel>>> Get([FromRoute][Required]long monthlyID)
        {
            var ret = new ReturnObject<MonthlyReportModel>(_msglocalizer);
            try
            {
                ret = await _plan.GetMonthRep(monthlyID);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }

        /// <summary>
        /// ค้นหา  รายงาน
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost("list")]
        public async Task<ActionResult<ReturnList<PlanHeader>>> List(PlanListParameter p)
        {
            var ret = new ReturnList<PlanHeader>(_msglocalizer);
            try
            {
                ret = await _plan.GetPlanList(ServiceModels.Constants.LOVGroup.Plan_Type.รายงานประจำเดือน, p, null);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// สร้างรายงานประจำเดือน
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
                ret = await _plan.CreateMonthRep(  p,UserID);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// แก้ไข รายงานประจำเดือน
        /// </summary>
        /// <returns></returns>
        [HttpPut("{PlanID}")]
        public async Task<ActionResult<ReturnMessage>> UpdateMonthReport(MonthlyReportModel p, [FromHeader][Required]string UserID,
             [FromRoute][Required]long PlanID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            try
            {

                ret = await _plan.ModifyMonthRep(p,  PlanID, UserID);
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