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

namespace mof.api.Controllers
{
    /// <summary>
    /// สัญญา
    /// </summary>
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public class AgreementsController : Controller
    {
        private IOrganization _iorg;
        private UserManager<ApplicationUser> _user;
        SignInManager<ApplicationUser> _sign;
        IStringLocalizer<MessageLocalizer> _msglocalizer;
        RoleManager<ApplicationRole> _role;
        private ISystemHelper _helper;
        //private IPlan _plan;
        private IAgreement _agr;
        private ICommon _com;
        public AgreementsController(IOrganization iorg, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IStringLocalizer<MessageLocalizer> msglocalizer,
            RoleManager<ApplicationRole> role,
            ISystemHelper helper,
            ICommon com,
            IAgreement agr)
        {

            _iorg = iorg;
            _user = userManager;
            _sign = signInManager;
            _msglocalizer = msglocalizer;
            _role = role;
            _helper = helper;
            _com = com;
            _agr = agr;
        }
        /// <summary>
        /// Get agreement
        /// </summary>
        /// <param name="agreementID"></param>
        /// <returns></returns>
        [HttpGet("{agreementID}")]
        public async Task<ActionResult<ReturnObject<AgreementModel>>> Get([FromRoute][Required]long agreementID)
        {
            var ret = new ReturnObject<AgreementModel>(_msglocalizer);
            try
            {
                ret = await _agr.Get(agreementID);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// สร้าง สัญญา ใหม่
        /// </summary>
        /// <param name="p"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ReturnObject<long?>>> Create([FromBody]AgreementModel p, [FromHeader][Required] string userID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            try
            {
                ret = await _agr.Modify(p, true, userID);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// แก้ไข สัญญา 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="userID"></param>
        /// <param name="projID"></param>
        /// <returns></returns>
        [HttpPut("{agreementID}")]
        public async Task<ActionResult<ReturnObject<long?>>> Update([FromRoute]long agreementID, [FromBody]AgreementModel p, [FromHeader] string userID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            try
            {
                p.AgreementID = agreementID;
                ret = await _agr.Modify(p, false, userID);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// ค้นหา สัญญา
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost("list")]
        public async Task<ActionResult<ReturnList<AgreementModel>>> List(AgreementListParameter p)
        {
            var ret = new ReturnList<AgreementModel>(_msglocalizer);
            try
            {
                ret = await _agr.List(p);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// ผูกสัญญากับแผนตามลัษณะงาน
        /// </summary>
        /// <param name="agreementID"></param>
        /// <param name="userID"></param>
        /// <param name="planActID"></param>
        /// <returns></returns>
        [HttpPost("{agreementID}/planActivities/{planActID}")]
        public async Task<ActionResult<ReturnObject<long?>>> MapPlanAct([FromRoute]long agreementID, [FromRoute]long planActID, [FromHeader] string userID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            try
            {
        
                ret = await _agr.MapActivityToAgreement(agreementID,planActID);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// ลบสัญญากับแผนตามลัษณะงาน
        /// </summary>
        /// <param name="agreementID"></param>
        /// <param name="planActID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpDelete("{agreementID}/planActivities/{planActID}")]
        public async Task<ActionResult<ReturnObject<long?>>> RemovePlanAct([FromRoute]long agreementID, [FromRoute]long planActID, [FromHeader] string userID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            try
            {

                ret = await _agr.RemoveActivityFromAgreement(agreementID,planActID);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// ผูกสัญญากับแผนปรับโครงสร้างหนี้
        /// </summary>
        /// <param name="agreementID"></param>
        /// <param name="userID"></param>
        /// <param name="paymentPlanID"></param>
        /// <returns></returns>
        [HttpPost("{agreementID}/restructurePlan/{paymentPlanID}")]
        public async Task<ActionResult<ReturnObject<long?>>> MapPaymentPlan([FromRoute]long agreementID, [FromRoute]long paymentPlanID, [FromHeader] string userID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            try
            {

                ret = await _agr.MapPaymentPlanToAgreement(agreementID, paymentPlanID);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// ลบสัญญากับแผนปรับโครงสร้างหนี้
        /// </summary>
        /// <param name="paymentPlanID"></param>
        /// <param name="agreementID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpDelete("{agreementID}/restructureplan/{paymentPlanID}")]
        public async Task<ActionResult<ReturnObject<long?>>> RemovePaymentPlan([FromRoute]long agreementID,[FromRoute]long paymentPlanID, [FromHeader] string userID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            try
            {

                ret = await _agr.RemovePaymentPlanFromAgreement(agreementID,paymentPlanID);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// M197T1 แสดงแผนกู้ที่เกี่ยวข้องในสัญญา
        /// </summary>
        /// <param name="agreementID"></param>
        /// <returns></returns>
        [HttpGet("{agreementID}/agreementMapList")]
        public async Task<ActionResult<ReturnObject<AgreementMappingList>>> GetAgreementMapList([FromRoute]long agreementID)
        {
            var ret = new ReturnObject<AgreementMappingList>(_msglocalizer);
            try
            {
        

                ret = await _agr.GetAgreementMappingList(agreementID);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// M197T2:  แสดงรายการความเคลื่อนไหวเงินกู้ของสัญญา
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost("transactions")]
        public async Task<ActionResult<ReturnList<AgreementTransModel>>> GetAgreementTrans([FromBody]AgreementListParameter p)
        {
            var ret = new ReturnList<AgreementTransModel>(_msglocalizer);
            try
            {


                ret = await _agr.GetAgreementTrans(p);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
    }
}