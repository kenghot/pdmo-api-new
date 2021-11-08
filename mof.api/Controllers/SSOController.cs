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
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using IdentityServer4.AccessTokenValidation;

namespace mof.api.Controllers
{

    [Route("v1/[controller]/[action]")]
    [ApiController]
   
    public class SSOController : Controller
    {
        private IIdentityRepository _iden;
        private UserManager<ApplicationUser> _user;
        SignInManager<ApplicationUser> _sign;
        IStringLocalizer<MessageLocalizer> _msglocalizer;
        RoleManager<ApplicationRole> _role;
        MOFContext _mof;
        private IOrganization _org;
        private ISystemHelper _sys;
        private IConfiguration _cfg;
        public SSOController(IIdentityRepository acc, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IStringLocalizer<MessageLocalizer> msglocalizer,
            RoleManager<ApplicationRole> role,
            MOFContext mof,
            IOrganization org,
            ISystemHelper sys,
            IConfiguration cfg)
        {

            _iden = acc;
            _user = userManager;
            _sign = signInManager;
            _msglocalizer = msglocalizer;
            _role = role;
            _mof = mof;
            _org = org;
            _sys = sys;
            _cfg = cfg;
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="model">Login Model</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ReturnObject<UserData>>> Login([FromBody] ServiceModels.Request.Login model)
        {
            var ret = new ReturnObject<UserData>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                if (ModelState.IsValid)
                {
                    var signinResult = await _sign.PasswordSignInAsync(model.UserName, model.Password, model.RememberLogin, lockoutOnFailure: true);
                    var user = await _user.FindByNameAsync(model.UserName);
                    if (signinResult.IsNotAllowed)
                    {
                        if (!await _user.IsEmailConfirmedAsync(user))
                        {
                            // Email isn't confirmed.
                            ret.AddMessage("EmailNotConfirm", "Email isn't confirmed.", eMessageType.Error);
                        }

                        if (!await _user.IsPhoneNumberConfirmedAsync(user))
                        {
                            // Phone Number isn't confirmed.
                            ret.AddMessage("PhoneNotConfirm", "Phone Number isn't confirmed.", eMessageType.Error);
                        }
                    }
                    else if (signinResult.IsLockedOut)
                    {
                        // Account is locked out.
                        ret.AddMessage("AlreadyLockOut", "Account is locked out.", eMessageType.Error);
                    }
                    else if (signinResult.RequiresTwoFactor)
                    {
                        // 2FA required.
                        ret.AddMessage("2FARequired", "2FA required.", eMessageType.Error);
                    }
                    else
                    {
                        // Username or password is incorrect.
                        if (user == null)
                        {
                            // Username is incorrect.
                            ret.AddMessage("UserNotFound", "User is not found.", eMessageType.Error);
                        }
                        else
                        {
                            // Password is incorrect.
                            if (!signinResult.Succeeded)
                            {
                                ret.AddMessage("InvalidPass", "Password is incorrect.", eMessageType.Error);
                            }

                        }
                    }
                    if (ret.Message.Count == 0)
                    {

                        var client = new HttpClient();
                        var helper = new Helper(_cfg);
                        var tokenResponse = await helper.GetIDSToken(model.UserName, model.Password);

                        if (tokenResponse.IsError)
                        {
                            ret.AddMessage("TokenError", tokenResponse.Error, eMessageType.Exception);
                            ret.AppException = tokenResponse.Exception;
                            return ret;
                        }
                        var uData = await _iden.GetUser(user);
                        ret = uData;
                        ret.Data.JwtAccessToken = tokenResponse.AccessToken;
                        ret.IsCompleted = true;
                        var u = _mof.Users.Where(w => w.Id == uData.Data.UserProfile.Id).FirstOrDefault();
                        if (u != null)
                        {
                            u.LastAccess = DateTime.Now;
                            _mof.SaveChanges();
                        }
                        ret.IsCompleted = true;
                        return Ok(ret);

                    }
                    else
                    {
                        //ret.Message.Add(new ErrorMessage { ErrorCode = "LoginFail", ErrorDesc = "LoginFail", Language = "en" });
                        return StatusCode(500, ret);
                    }



                }
                else
                {
                    ret.AddMessage("InvalidInput", "InvalidInput", eMessageType.Error);
                    return BadRequest(ret);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }





        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
        public async Task<ActionResult<ReturnObject<UserData>>> GetUserByToken()
        {
            var ret = new ReturnObject<UserData>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var hdr = Request.Headers["Authorization"];
                var token = hdr.ToString().Replace("Bearer ", "");
                ret = await _iden.GetUserByToken(token);
                return ret;
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
        }
    }
}