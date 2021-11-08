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
using mof.ServiceModels.Project;
using Microsoft.AspNetCore.Http;

namespace mof.api.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public class ProjectsController : Controller
    {
        private IOrganization _iorg;
        private UserManager<ApplicationUser> _user;
        SignInManager<ApplicationUser> _sign;
        IStringLocalizer<MessageLocalizer> _msglocalizer;
        RoleManager<ApplicationRole> _role;
        private ISystemHelper _helper;
        private IProject _proj;
        private readonly IHttpContextAccessor _http;
        private readonly IIIPMSync _iipm;
        public ProjectsController(IOrganization iorg, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IStringLocalizer<MessageLocalizer> msglocalizer,
            RoleManager<ApplicationRole> role,
            ISystemHelper helper,
            IProject proj,
            IHttpContextAccessor http,
            IIIPMSync iipm)
        {

            _iorg = iorg;
            _user = userManager;
            _sign = signInManager;
            _msglocalizer = msglocalizer;
            _role = role;
            _helper = helper;
            _proj = proj;
            _http = http;
            _iipm = iipm;
        }
        /// <summary>
        /// Get project by project id
        /// </summary>
        /// <param name="projID"></param>
        /// <returns></returns>
        [HttpGet("{projID}")]
        public async Task<ActionResult<ReturnObject<ProjectModel>>> Get([FromRoute][Required]long projID)
        {
            var ret = new ReturnObject<ProjectModel>(_msglocalizer);
            try
            {
                ret = await _proj.Get(projID);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// สร้าง project ใหม่
        /// </summary>
        /// <param name="p"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ReturnObject<long?>>> Create([FromBody]ProjectModel p, [FromHeader][Required] string userID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            try
            {
                ret = await _proj.Modify(p, true, userID);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// สร้าง project ใหม่ (สำหรับ  กู้เพื่อชดเชยขาดดุล กับ กู้เพื่อชดเชย ขาดดุลเลื่อมปี)
        /// </summary>
        /// <param name="p"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpPost("Minimum")]
        public async Task<ActionResult<ReturnObject<long?>>> CreateMin([FromBody]ProjectModelMin p, [FromHeader][Required] string userID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            try
            {
                ret = await _proj.ModifyMin(p, true, userID);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// แก้ไข project 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="userID"></param>
        /// <param name="projID"></param>
        /// <returns></returns>
        [HttpPut("{projID}")]
        public async Task<ActionResult<ReturnObject<long?>>> Update([FromRoute]long projID,[FromBody]ProjectModel p, [FromHeader] string userID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            try
            {
                p.ProjectID = projID;
                ret = await _proj.Modify(p, false, userID);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// แก้ไข project (สำหรับ  กู้เพื่อชดเชยขาดดุล กับ กู้เพื่อชดเชย ขาดดุลเลื่อมปี)
        /// </summary>
        /// <param name="p"></param>
        /// <param name="userID"></param>
        /// <param name="projID"></param>
        /// <returns></returns>
        [HttpPut("Minimum/{projID}")]
        public async Task<ActionResult<ReturnObject<long?>>> UpdateMin([FromRoute][Required]long projID, [FromBody]ProjectModelMin p, [FromHeader] string userID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            try
            {
                p.ProjectID = projID;
                ret = await _proj.ModifyMin(p, false, userID);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// ค้นหา โครงการ
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost("list")]
        public async Task<ActionResult<ReturnList<ProjectModel>>> List(ProjectListParameter p)
        {
            var ret = new ReturnList<ProjectModel>(_msglocalizer);
            try
            {
                ret = await _proj.List(p );
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// แสดงค่า รายการ checkbox สำหรับหน้าสถานะโครงการ 
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetProjStatusList")]
        public async Task<ActionResult<ReturnObject<List<ProjectStatus>>>> GetProjectStatusList()
        {
            var ret = new ReturnObject<List<ProjectStatus>>(_msglocalizer);
            try
            {
                ret = await _proj.GetProjectStatusList();
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// สร้าง model สำหรับแหล่งเงินกู้
        /// </summary>
        /// <returns></returns>
        [HttpGet("InitSourceLoanModel")]
        public ActionResult<ReturnObject<List<AmountData>>> InitSourceLoanModel()
        {
            var ret = new ReturnObject<List<AmountData>>(_msglocalizer);
            try
            {
                var sl = new List<AmountData>();
                sl = _proj.InitialSourceLoanAmount(sl);
                ret.Data = sl;
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