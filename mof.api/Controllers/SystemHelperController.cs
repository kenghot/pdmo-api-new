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
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc.Versioning;
using mof.ServiceModels.Common.Generic;
using System.ComponentModel.DataAnnotations;
using mof.ServiceModels.Helper;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Net.Mime;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace mof.api.Controllers
{
 
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public class SystemHelperController : Controller
    {

 
            private ISystemHelper _sys;
            private UserManager<ApplicationUser> _user;
            SignInManager<ApplicationUser> _sign;
            IStringLocalizer<MessageLocalizer> _msglocalizer;
            RoleManager<ApplicationRole> _role;
            private IConfiguration _conf;
            public MOFContext DB;
            public SystemHelperController(MOFContext db,ISystemHelper sys, UserManager<ApplicationUser> userManager,
                SignInManager<ApplicationUser> signInManager,
                IStringLocalizer<MessageLocalizer> msglocalizer,
                RoleManager<ApplicationRole> role,
                IConfiguration conf)
            {

                _sys = sys;
                _user = userManager;
                _sign = signInManager;
                _msglocalizer = msglocalizer;
                _role = role;
                DB = db;
            _conf = conf;
            }
        [HttpGet("SendMail")]
        [AllowAnonymous]
        public async Task<ActionResult<ReturnMessage>> SendMail([FromQuery] string toEmail)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var email = new EmailConfig();
                _conf.GetSection("Email").Bind(email);
                ret.Message.Add(new MessageData { Message = JsonConvert.SerializeObject(email) });
                var msg = $"PIN code = 1234 โดย PIN จะถูกยกเลิกภายใน 1 ชั่วโมง";  
        
                var mail = new MailMessage
                {
                    Sender = new MailAddress("noreply@pdmo.go.th"),
                    Body = msg,
                    Subject = "Change password"
                };
                mail.From = new MailAddress("noreply@pdmo.go.th");
                mail.To.Add(new MailAddress(toEmail));
                var send = await _sys.SendEmail(mail);
                if (!send.IsCompleted)
                {
                    ret.CloneMessage(send.Message);
                    ret.Message.Add(new MessageData { Message = JsonConvert.SerializeObject(email) });
                    return ret;

                }
                ret.IsCompleted = true;
             
                return ret;
                 
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);

        }
        /// <summary>
        /// แสดงค่า LOV โดยระบุ LOVCode
        /// </summary>
        /// <param name="LOVCode">ต้องระบุ</param>
        /// <returns></returns>
        [HttpGet("GetLOVByCode")]
        public ActionResult<ReturnObject<LOV>> GetLOVByCode([FromQuery][Required]string LOVCode, [FromQuery][Required]string LOVGroupCode)
        {
            var ret = new ReturnObject<LOV>(_msglocalizer);
            try
            {
                ret = _sys.GetLOVByCode(LOVCode, LOVGroupCode);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// แสดงค่า LOV โดยระบุ LOVGroupcode
        /// </summary>
        /// <param name="LOVGroupCode">ระบุ LOVGroupCode ถ้าไม่ระบุคือเอาทั้งหมด</param>
        /// <returns></returns>
        [HttpGet("GetLOVByGroup")]
        public ActionResult<ReturnObject<List<LOV>>> GetLOVByGroup([FromQuery]string LOVGroupCode)
        {
            var ret = new ReturnObject<List<LOV>>(_msglocalizer);
            try
            {
                ret = _sys.GetLOVByGroup(LOVGroupCode);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// แสดงค่า LOV โดยระบุ ParentGroupCode, ParentLOVCode
        /// </summary>
        /// <param name="ParentGroupCode">ต้องระบุ</param>
        /// <param name="ParentLOVCode">ต้องระบุ</param>
        /// <returns></returns>
        [HttpGet("GetLOVByParent")]
        public ActionResult<ReturnObject<List<LOV>>> GetLOVByParent([FromQuery][Required]string ParentGroupCode, [FromQuery][Required]string ParentLOVCode)
        {
            var ret = new ReturnObject<List<LOV>>(_msglocalizer);
            try
            {
              
                ret = _sys.GetLOVByParent(ParentGroupCode, ParentLOVCode);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// Get list of LOVGroup
        /// </summary>
        /// <param name="LOVGroupCode">ระบุ LOVGroupCode ถ้าไม่ระบุคือเอาทั้งหมด</param>
        /// <returns></returns>
        [HttpGet("GetLOVGroup")]
        public ActionResult<ReturnObject<List<BasicData>>> GetLOVGroup([FromQuery]string LOVGroupCode)
        {
            var ret = new ReturnObject<List<BasicData>>(_msglocalizer);
            try
            {
                ret = _sys.GetLOVGroup(LOVGroupCode);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        [HttpGet("Currencies")]
        public ActionResult<ReturnObject<CurrencyData>> Currencies([FromQuery]int year)
        {
            var ret = new ReturnObject<CurrencyData>(_msglocalizer);
            try
            {
                ret = _sys.GetCurrencyRate(year);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }

        [HttpGet("DownloadFile/{fileID}")]
        [AllowAnonymous]
        public async Task<ActionResult> DownloadFile([FromRoute]long fileID)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var f = await DB.AttachFile.Where(w => w.AttachFileId == fileID).FirstOrDefaultAsync();
                if (f == null)
                {
                    ret.AddMessage("File is not found", "not found", eMessageType.Exception);
                    return Ok(ret);
                }
                var media = mof.Services.Helper.Utilities.GetMMIEType(f.FileExtension,_msglocalizer);
                if (!media.IsCompleted)
                {
                    ret.CloneMessage(media.Message);
                    return Ok(ret);
                }

             
                Stream stream = new MemoryStream(f.FileData);
                var file = new FileStreamResult(stream, media.Data);
                file.FileDownloadName = $"{f.FileName}";
                return file;
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }


            
       
        }
        [HttpPost("UploadFile")]
        public async Task<ActionResult<ReturnObject<long?>>> UploadFile([FromBody]AttachFileData data)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {

                ret = await _sys.UploadFile(DB,data,true);
                

            } catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }

            return Ok(ret);
        }
        /// <summary>
        /// แสดงวัตถุประสงการกู้ แยกตามหน่วยงาน
        /// </summary>
        /// <param name="orgID"></param>
        /// <returns></returns>
        [HttpGet("GetProjectTypeByOrg")]
       public async Task<ActionResult<ReturnObject<List<BasicData>>>> GetProjectTypeByOrg([FromQuery][Required]long orgID)
        {
            var ret = new ReturnObject<List<BasicData>>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {

                ret = await _sys.GetProjectType(orgID);


            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }

            return Ok(ret);
        }
        /// <summary>
        /// get data for screen Currency 
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        [HttpGet("CurrencyData")]
        public async Task<ActionResult<ReturnObject<CurrencyData>>> GetCurrencyData([FromQuery][Required]int year)
        {
            var ret = new ReturnObject<CurrencyData>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {

                ret =  await _sys.GetCurrencyRateScreen(year);


            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }

            return Ok(ret);
        }
        /// <summary>
        /// update data data for screen Currency 
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        [HttpPut("CurrencyData")]
        public async Task<ActionResult<ReturnMessage>> UpdateCurrencyData([FromBody][Required]CurrencyData currency)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {

                ret = await _sys.UpdateCurrencyRateScreen(currency);


            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }

            return Ok(ret);
        }
        /// <summary>
        /// List Parameter
        /// </summary>
        /// <returns></returns>
        [HttpGet("Parameters/list")]
        public async Task<ActionResult<ReturnObject<List<ParameterData>>>> ListParameter()
        {
            var ret = new ReturnObject<List<ParameterData>>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {

                ret = await _sys.ListParameter();


            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }

            return Ok(ret);
        }
        /// <summary>
        /// Update Parameter
        /// </summary>
        /// <returns></returns>
        [HttpPut("Parameters")]
        public async Task<ActionResult<ReturnMessage>> updateParameter([FromBody]ParameterData d,[FromHeader][Required] string userID)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {

                ret = await _sys.ModifyParameter(d,false,userID);


            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }

            return Ok(ret);
        }
        /// <summary>
        /// Creat Parameter
        /// </summary>
        /// <returns></returns>
        [HttpPost("Parameters")]
        public async Task<ActionResult<ReturnMessage>> CreateParameter([FromBody]ParameterData d, [FromHeader][Required] string userID)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {

                ret = await _sys.ModifyParameter(d, true, userID);


            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }

            return Ok(ret);
        }

        /// <summary>
        /// List ข้อกำหนดหน่วยงาน ตาม สบน.
        /// </summary>
        /// <returns></returns>
        [HttpGet("PDMORegulations/list")]
        public async Task<ActionResult<ReturnList<PDMORegulationData>>> ListPDMORegulations()
        {
            var ret = new ReturnList<PDMORegulationData>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {

                ret = await _sys.GetPDMORegulationList();


            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }

            return Ok(ret);
        }
        /// <summary>
        /// Update ข้อกำหนดหน่วยงาน ตาม สบน.
        /// </summary>
        /// <returns></returns>
        [HttpPut("PDMORegulations")]
        public async Task<ActionResult<ReturnMessage>> updatePDMORegulations([FromBody]PDMORegulationData p, [FromHeader][Required] string userID)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {

                ret = await _sys.ModifyPDMORegulation(p, false, userID);


            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }

            return Ok(ret);
        }

        /// <summary>
        /// List CurrencyScreen
        /// </summary>
        /// <returns></returns>
        [HttpGet("CurrencyScreen/list")]
        public async Task<ActionResult<ReturnObject<List<CurrencyScreen>>>> ListCurrencyScreen()
        {
            var ret = new ReturnObject<List<CurrencyScreen>>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {

                ret = await _sys.ListCurrency();


            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }

            return Ok(ret);
        }
        /// <summary>
        /// Update CurrencyScreen
        /// </summary>
        /// <returns></returns>
        [HttpPut("CurrencyScreen")]
        public async Task<ActionResult<ReturnMessage>> updateCurrencyScreen([FromBody]CurrencyScreen d, [FromHeader][Required] string userID)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {

                ret = await _sys.ModifyCurrency(d, false, userID);


            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }

            return Ok(ret);
        }
        /// <summary>
        /// Creat CurrencyScreen
        /// </summary>
        /// <returns></returns>
        [HttpPost("CurrencyScreen")]
        public async Task<ActionResult<ReturnMessage>> CreateCurrencyScreen([FromBody]CurrencyScreen d, [FromHeader][Required] string userID)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {

                ret = await _sys.ModifyCurrency(d, true, userID);


            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }

            return Ok(ret);
        }
        /// <summary>
        /// Delete CurrencyScreen
        /// </summary>
        /// <returns></returns>
        [HttpDelete("CurrencyScreen")]
        public async Task<ActionResult<ReturnMessage>> DeleteCurrencyScreen([FromBody]CurrencyScreen d, [FromHeader][Required] string userID)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {

                ret = await _sys.DeleteCurrency(d.CurrencyCode,  userID);


            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }

            return Ok(ret);
        }
    }

}