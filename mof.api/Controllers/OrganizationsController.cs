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
  
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public class OrganizationsController : Controller
    {
        private IOrganization _iorg;
        private UserManager<ApplicationUser> _user;
        SignInManager<ApplicationUser> _sign;
        IStringLocalizer<MessageLocalizer> _msglocalizer;
        RoleManager<ApplicationRole> _role;
        private ISystemHelper _helper;
        public OrganizationsController(IOrganization iorg, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IStringLocalizer<MessageLocalizer> msglocalizer,
            RoleManager<ApplicationRole> role,
            ISystemHelper helper)
        {

            _iorg = iorg;
            _user = userManager;
            _sign = signInManager;
            _msglocalizer = msglocalizer;
            _role = role;
            _helper = helper;
        }

        /// <summary>
        /// รายการหน่วยงานต้นสังกัด
        /// </summary>
        /// <returns></returns>
        [HttpGet("Affiliates")]
        public async Task<ActionResult<ReturnList<BasicData>>> Affiliates()
        {
            var ret = new ReturnList<BasicData>(_msglocalizer);
            try
            {

                ret = await _iorg.GetAffiliates();
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// แสดงระเบียบปฎิบัติตามกฎหมายที่สอดคล้องกับระเบียบ สบน.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}/PDMOLawReguration")]
        [AllowAnonymous]
        public async Task<ActionResult<ReturnObject<string>>> PDMOLawReguration([FromRoute][Required] long id)
        {

            var ret = new ReturnObject<string>(_msglocalizer);
            try
            {

                ret = await _iorg.GetPDMOLawReguration(id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// Create Organization
        /// </summary>
        /// <param name="org"></param>
        /// <param name="UserID"></param>
        /// <returns>Return.Data : รหัส Organization ID ที่สร้างได้</returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<ReturnObject<long?>>> Create([FromBody]ORGModel org,[FromHeader][Required] string UserID)
        {

            var ret = new  ReturnObject<long?>(_msglocalizer);
            try
            {
               
               ret = await _iorg.Modify(org,true,UserID, eORGModifyType.Normal);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// Update Organization
        /// </summary>
        /// <param name="org"></param>
        /// <param name="UserID"></param>
        /// <param name="id"></param>
        /// <returns>Return.Data : รหัส Organization ID ที่สร้างได้</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ReturnObject<long?>>> Update([FromBody]ORGModel org, [FromHeader][Required] string UserID, long id)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            try
            {
                org.OrganizeID = id;
                ret = await _iorg.Modify(org, false, UserID,  eORGModifyType.Normal);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }

        /// <summary>
        /// ยืนยันการร้องขอเปลี่ยนแปลงข้อมูลหน่วยงาน
        /// </summary>
        /// <param name="org"></param>
        /// <param name="UserID"></param>
        /// <param name="id"></param>
        [HttpPut("{id}/changerequest")]
        public async Task<ActionResult<ReturnObject<long?>>> SendChangeRequest([FromBody]ORGModel org, [FromHeader][Required] string UserID, long id)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            try
            {
                org.OrganizeID = id;
                ret = await _iorg.Modify(org, false, UserID,  eORGModifyType.ChaneRequest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// อนุมัติการร้องขอเปลี่ยนแปลงข้อมูลหน่วยงาน
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="id"></param>
        [HttpGet("{id}/approverequest")]
        public async Task<ActionResult<ReturnObject<long?>>> Approve([FromHeader][Required] string UserID, [FromRoute]long id)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            try
            {
               
                ret = await _iorg.ApproveChange(id , UserID);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// ยกเลิการร้องขอเปลี่ยนแปลงข้อมูลหน่วยงาน
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="id"></param>
        [HttpGet("{id}/cancelrequest")]
        public async Task<ActionResult<ReturnObject<long?>>> CancelChange([FromHeader][Required] string UserID, [FromRoute]long id)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            try
            {
                var org = new ORGModel();
                org.OrganizeID = id;
                ret = await _iorg.Modify(org, false, UserID, eORGModifyType.CancelChange);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// Get Organization
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ReturnObject<ORGModel>>> Get(long id)
        {
            var ret = new ReturnObject<ORGModel>(_msglocalizer);
            try
            {
                ret = await _iorg.Get(id,false,true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// ดึงข้อมูล เพื่อร้องขอเปลี่ยนแปลงข้อมูลหน่วยงาน
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/changerequest")]
        public async Task<ActionResult<ReturnObject<ORGModel>>> GetChangeRequest(long id)
        {
            var ret = new ReturnObject<ORGModel>(_msglocalizer);
            try
            {
                ret = await _iorg.Get(id, true,true );
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// Get Loan Summary Dashboard (S100)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("GetLoanSummaryDashboard")]
        public async Task<ActionResult<ReturnObject<LoanSummaryDashboard>>> GetLoanSummaryDashboard([FromBody]ServiceModels.Request.Common.GetByID id)
        {
            var ret = new ReturnObject<LoanSummaryDashboard>(_msglocalizer);
            try
            {
                ret = await _iorg.GetLoanSummaryDashboard(id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }

        #region s120
        /// <summary>
        /// S120 : รายการคำขอปรับปรุงข้อมูลหน่วยงาน
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost("changerequestlist")]
        public async Task<ActionResult<ReturnList<ORGModel>>> ChangeRequestList(ChangeRequestsParameter p)
        {
            var ret = new ReturnList<ORGModel>(_msglocalizer);
            try
            {

                ret = await _iorg.GetChangeRequest(p);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// S120 : แสดงรายชื่อหน่วยงาน
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost("list")]
        [AllowAnonymous]
        public async Task<ActionResult<ReturnList<OrganizationList>>> List(OrganizationListParameter p)
        {
            var ret = new ReturnList<OrganizationList>(_msglocalizer);
            try
            {

                ret = await _iorg.GetOrganizationList(p);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }

        /// <summary>
        /// S120 : สรุปยอดหน่วยงานแยกตามประเถทหน่วยงาน
        /// </summary>
        /// <returns></returns>
        [HttpGet("countbytype")]
        public async Task<ActionResult<ReturnObject<List<ORGCountByType>>>> CountByType()
        {
            var ret = new ReturnObject<List<ORGCountByType>>(_msglocalizer);
            try
            {

                ret = await _iorg.GetSummaryORG();
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// คำนวนสัดส่วนผู้ถือหุ้น และ สถานะโครงการ
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        [HttpPost("CalculateORGStatus")]
        public async Task<ActionResult<ReturnObject<CalORGStatusResponse>>> CalculateORGStatus([FromBody]CalORGStatusRequest org)
        {
            var ret = new ReturnObject<CalORGStatusResponse>(_msglocalizer);
            try
            {

                ret = await _iorg.CalculateORGStatus(org);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        #endregion
    }
}