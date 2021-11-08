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

namespace mof.api.Controllers
{
    [Route("v1")]
    [ApiController]
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public class CommonController : Controller
    {
        private IOrganization _iorg;
        private UserManager<ApplicationUser> _user;
        SignInManager<ApplicationUser> _sign;
        IStringLocalizer<MessageLocalizer> _msglocalizer;
        RoleManager<ApplicationRole> _role;
        private ISystemHelper _helper;
        private ICommon _com;
        public CommonController(IOrganization iorg, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IStringLocalizer<MessageLocalizer> msglocalizer,
            RoleManager<ApplicationRole> role,
            ISystemHelper helper,
            ICommon com)
        {

            _iorg = iorg;
            _user = userManager;
            _sign = signInManager;
            _msglocalizer = msglocalizer;
            _role = role;
            _helper = helper;
            _com = com;
        }
        /// <summary>
        /// แสดงประกาศความช่วยเหลือทางวิชาการ .  
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("technicalAsisitances")]
        public async Task<ActionResult<ReturnObject<List<TechicalAssistance>>>> GetTechicalAssistances()
        {
            var ret = new ReturnObject<List<TechicalAssistance>>(_msglocalizer);
            try
            {
                ret = await _iorg.GetTechicalAssistances();
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// Master Agreement  
        /// </summary>
        /// <param name="PlanProjectId"></param>
        /// <returns></returns>
        [HttpGet("GetMasterAgreement")]
        [AllowAnonymous]
        public async Task<ActionResult<ReturnObject<string>>> GetMasterAgreement([FromQuery] long PlanProjectId)
        {
            var ret = new ReturnObject<string>(_msglocalizer);
            try
            {
                var gma = await _com.GetMasterAgreemet(PlanProjectId);
                ret.Data = gma;
                ret.IsCompleted = true;

            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
    }
}