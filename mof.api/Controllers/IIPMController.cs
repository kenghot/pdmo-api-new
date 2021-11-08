using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;

using mof.IServices;
using mof.ServiceModels.Common;
using mof.ServiceModels.Identity;
using mof.ServiceModels.IIPMModel;

namespace mof.api.Controllers
{
    /// <summary>
    /// เชื่อมต่อ IIPM
    /// </summary>
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public class IIPMController : Controller
    {
        private IOrganization _iorg;
        private UserManager<ApplicationUser> _user;
        SignInManager<ApplicationUser> _sign;
        IStringLocalizer<MessageLocalizer> _msglocalizer;
        RoleManager<ApplicationRole> _role;
        private ISystemHelper _helper;
        private IPlan _plan;
        private IConfiguration _config;
        private IIIPM _iipm;
        private readonly IHttpContextAccessor _http;
        public IIPMController(IOrganization iorg, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IStringLocalizer<MessageLocalizer> msglocalizer,
            RoleManager<ApplicationRole> role,
            ISystemHelper helper,
            IPlan plan,
            IConfiguration config,
            IIIPM iipm,
            IHttpContextAccessor http)
        {

            _iorg = iorg;
            _user = userManager;
            _sign = signInManager;
            _msglocalizer = msglocalizer;
            _role = role;
            _helper = helper;
            _plan = plan;
            _config = config;
            _iipm = iipm;
            _http = http;
        }
 
        [HttpPost("Integrate")]
        public async Task<ActionResult<ReturnMessage>> IntegrateIIPMData([FromBody]IntegrateConfig p,[FromHeader]string userID)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                ret = await _iipm.IntegrateIIPMData(p);
                //if ( !ret.IsCompleted)
                //{
                //    ret.CloneMessage(ret.Message);
                //    return ret;
                //}
                //ret = await _iipm.CopyIIPMData(p, userID);

            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return ret;
        }
        [HttpPost("CopyData")]
        public async Task<ActionResult<ReturnMessage>> CopyIIPMData([FromBody]IntegrateConfig p,[FromHeader]string userID)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                ret = await _iipm.CopyIIPMData(p,userID);

            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return ret;
        }
        [HttpPost("ProjectExtend")]
        public async Task<ActionResult<ReturnMessage>> ProjectExtend([FromBody] ProjectExtendRequest p, [FromHeader] string userID)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                ret = await _iipm.ProjectExtend(p, userID);

            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return ret;
        }
    }
}