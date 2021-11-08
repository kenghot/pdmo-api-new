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

namespace mof.api.Controllers
{
  
    [Route("v1/[controller]/[action]")]
    [ApiController]
    //[Authorize]
    public class IdentityController : Controller
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
        public IdentityController(IIdentityRepository acc, UserManager<ApplicationUser> userManager, 
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
        [HttpGet]
        public async Task<ActionResult> Test()
        {
            try {
                //var rets = new List<ReturnObject<string>>();
                //var os = _mof.Organization.Where(w => w.OrganizationId >= 50);
                //foreach (var o in os)
                //{
                //    Register reg = new Register
                //    {
                //        ConfirmPassword = "Test@1234",
                //        EFirstName = o.OrganizationCode,
                //        ELastName = "Test",
                //        Email = o.OrganizationCode + "@test.com",
                //        OrganizationID = o.OrganizationId,
                //        Password = "Test@1234",
                //        TFirstName = o.OrganizationCode,
                //        TLastName = "Test",
                //        UserName = o.OrganizationCode
                //    };
                //    var ret = await _iden.Register(reg);
                //    rets.Add(ret);
                //    var u = await _user.FindByEmailAsync(reg.Email);
                //    if (u != null)
                //    {
                //        var ur = new UserRoles
                //        {
                //            UserId = u.Id,
                //            RoleId = "475a0535-1047-43ba-8d28-42f5cf7dab38"
                //        };
                //        _mof.UserRoles.Add(ur);
                        
                //    }
                //}

               // _mof.SaveChanges();

                return Ok();
            } catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ReturnMessage>> InitAllPassword()
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var users = await _mof.Users.ToListAsync();
                foreach (var u in users)
                {
                    var user = await _user.FindByIdAsync(u.Id);
                    if (user == null)
                    {
                        ret.IsCompleted = false;
                        ret.AddMessage("UserNotFound", "User is not found.", eMessageType.Error);
                         
                    }
                    user.PasswordHash = _user.PasswordHasher.HashPassword(user,"Init@1234");
                    var result = await _user.UpdateAsync(user);
                    //user.Pin = null;
                    //user.PincreatedDt = null;
                    //await DB.SaveChangesAsync();
                    if (result.Succeeded)
                    {
                        ret.IsCompleted = true;
                    }else
                    {
                        ret.IsCompleted = false;
                        ret.AddMessage($"can not change password of user {u.UserName}", "error.", eMessageType.Error);
                    }
                }
                ret.IsCompleted = true;
                return ret;
            }  
            catch (Exception ex) {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }


        }
        /// <summary>
        /// Login
        /// </summary>
        /// <param name="model">Login Model</param>
        /// <returns></returns>
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult<ReturnObject<UserData>>> Login([FromBody]ServiceModels.Request.Login model)
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
                            ret.AddMessage("EmailNotConfirm",  "Email isn't confirmed.", eMessageType.Error );
                        }

                        if (!await _user.IsPhoneNumberConfirmedAsync(user))
                        {
                            // Phone Number isn't confirmed.
                            ret.AddMessage( "PhoneNotConfirm",  "Phone Number isn't confirmed.",  eMessageType.Error);
                        }
                    }
                    else if (signinResult.IsLockedOut)
                    {
                        // Account is locked out.
                        ret.AddMessage( "AlreadyLockOut",   "Account is locked out.",  eMessageType.Error);
                    }
                    else if (signinResult.RequiresTwoFactor)
                    {
                        // 2FA required.
                        ret.AddMessage("2FARequired",   "2FA required.",   eMessageType.Error);
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
                                ret.AddMessage( "InvalidPass",  "Password is incorrect.", eMessageType.Error);
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
                        //var x = await helper.GetIDSConfig();
                        //var url = _cfg.GetSection("IdentityServerUrl").Value;
                        ret.Data.JwtAccessToken = tokenResponse.AccessToken;
                        //ret.Data.JwtAccessToken = "token test " + url + " : " + x.disc.TokenEndpoint;
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
                    ret.AddMessage( "InvalidInput",   "InvalidInput",  eMessageType.Error  );
                    return BadRequest(ret);
                }
            }
            catch (Exception ex) {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }


    
           
 
        }
        //[HttpPost]
        //public async Task<ActionResult<(IdentityResult result , string UserID)>> Create([FromBody]ServiceModels.Identity.ApplicationUser user)
        //{
        //    var u = await _user.CreateAsync(user);

        //    return (result: u, UserID: "test5555");
        //}
        [HttpPost]
        public async Task<ActionResult<ReturnMessage>> CreateRole(ApplicationRole role)
        {
            var ret = new ReturnMessage (_msglocalizer);
            try
            {
                ret = await _iden.CreateRole(role);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        [HttpPost]
        public async Task<ActionResult<ReturnObject<string>>> Register([FromBody]Register reg)
        {
            var ret = new ReturnObject<string>(_msglocalizer);
            try
            {
                ret = await _iden.Register(reg);
            }catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// ไม่ต้องใส่ newpassword
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]

        public async Task<ActionResult<ReturnMessage>> CheckPINIsValid([FromBody]ChangePassword p)
        {
            var ret = new ReturnMessage(_msglocalizer);
            try
            {
                ret = await _iden.ChangePassword(p, true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// ต้องเรียก ResetPassword ก่อน
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]

        public async Task<ActionResult<ReturnMessage>> ChangePassword([FromBody]ChangePassword p)
        {
            var ret = new ReturnMessage(_msglocalizer);
            try
            {
                ret = await _iden.ChangePassword(p,false );
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        [HttpPost]
        public async Task<ActionResult< ReturnList<ServiceModels.Identity.ApplicationUser>>> SearchUser ([FromBody] ServiceModels.Request.Paging gl)
        {
            var ret = new ReturnList<ServiceModels.Identity.ApplicationUser>(_msglocalizer);
            try
            {
                ret = await _iden.SearchUsersAsync(gl);
            } catch (Exception ex)
            {
             
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
           
            return Ok(ret);
        }
        /// <summary>
        /// get user for maintenace screen
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userID}")]
        public async Task<ActionResult<ReturnObject<UserScreen>>> UserProfiles([FromRoute]string userId)
        {
            var ret = new ReturnObject<UserScreen>(_msglocalizer);
            try
            {
                ret = await _iden.GetUserScreen(userId);
            }
            catch (Exception ex)
            {

                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }

            return Ok(ret);
        }
        /// <summary>
        /// Create user (ReturnObject.Data is UserID)
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ReturnObject<string>>> UserProfiles([FromBody]UserScreen user,[FromHeader][Required]string userID)
        {
            var ret = new ReturnObject<string>(_msglocalizer);
            try
            {
                ret = await _iden.ModifyUserScreen(user,true,userID);
            }
            catch (Exception ex)
            {

                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }

            return Ok(ret);
        }
        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpDelete("/v1/[controller]/UserProfiles/{Id}")]
        public async Task<ActionResult<ReturnMessage>> DeleteUserProfiles([FromRoute][Required] string Id,[FromHeader][Required]string userID)
        {
            var ret = new ReturnMessage(_msglocalizer);
            try
            {
                ret = await _iden.DeleteUserScreen( Id, userID);
            }
            catch (Exception ex)
            {

                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }

            return Ok(ret);
        }
        /// <summary>
        /// update user (ReturnObject.Data is UserID)
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut("/v1/[controller]/UserProfiles/")]
        public async Task<ActionResult<ReturnObject<string>>> UpdateUserProfiles([FromBody]UserScreen user, [FromHeader][Required]string userID)
        {
            var ret = new ReturnObject<string>(_msglocalizer);
            try
            {
                ret = await _iden.ModifyUserScreen(user, false, userID);
            }
            catch (Exception ex)
            {

                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }

            return Ok(ret);
        }

        /// <summary>
        /// get Permission Groups
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<ReturnObject<List<UserPermissionGroup>>>> PermissionGroups()
        {
            var ret = new ReturnObject<List<UserPermissionGroup>>(_msglocalizer);
            try
            {
                var data = await _iden.GetPermissionGroup();
                ret.Data = data;
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {

                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }

            return Ok(ret);
        }

        /// <summary>
        /// get Role
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<ReturnObject<List<UserRolesSelect>>>> Roles()
        {
            var ret = new ReturnObject<List<UserRolesSelect>>(_msglocalizer);
            try
            {
                var data =  await _mof.Roles.Where(w => w.ApplicationCode == "pdmo").Select(s => new UserRolesSelect
                {
                    IsSelected = false,
                    RoleId = s.Id,
                    RoleName = s.Name

                }).ToListAsync();
                ret.Data = data;
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {

                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }

            return Ok(ret);
        }
        /// <summary>
        /// ร้องขอแก้ password โดย pin จะส่งไปที่ email ตามค่าใน Return.Data
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userName}")]
        [AllowAnonymous]
        public async Task<ActionResult<ReturnObject<string>>> ResetPassword([FromRoute]string userName)
        {
            var ret = new ReturnObject<string>(_msglocalizer);
            try
            {
                ret = await _iden.ResetPassword(userName);
            }
            catch (Exception ex)
            {

                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }

            return Ok(ret);
        }
    }
}
