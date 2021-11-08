using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using mof.DataModels.Models;
using mof.IServices;
using mof.ServiceModels.Common;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Linq.Expressions;
using mof.Services.Helper;
using Microsoft.EntityFrameworkCore;
using mof.ServiceModels.Request;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Localization;
using mof.ServiceModels.Identity;
using System.Security.Claims;
using mof.ServiceModels.Common.Generic;
using System.Net.Mail;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;

namespace mof.Services
{
    public class IdentityRepository : IIdentityRepository

    {
        public MOFContext DB;
        public IDbConnection CN;
        private UserManager<ServiceModels.Identity.ApplicationUser> _user;
        private SignInManager<ServiceModels.Identity.ApplicationUser> _signin;
        private Microsoft.AspNetCore.Identity.UI.Services.IEmailSender _email;
        private IStringLocalizer<MessageLocalizer> _msglocalizer;
        private RoleManager<ApplicationRole> _role;
        private IOrganization _org;
        private ISystemHelper _sys;
        public IdentityRepository(MOFContext db, UserManager<ServiceModels.Identity.ApplicationUser> userManager,
            SignInManager<ServiceModels.Identity.ApplicationUser> signInManager,
            Microsoft.AspNetCore.Identity.UI.Services.IEmailSender email,
            IStringLocalizer<MessageLocalizer> msglocalizer,
            RoleManager<ApplicationRole> role,
            IOrganization org,
            ISystemHelper sys)
        {
            DB = db;
            CN = new SqlConnection(DB.GetConnectionString());
            _user = userManager;
            _signin = signInManager;
            _email = email;
            _msglocalizer = msglocalizer;
            _role = role;
            _org = org;
            _sys = sys;
        }

        public async Task<ReturnMessage> ConfirmEmail(string userId, string code)
        {
            var ret = new ReturnMessage(_msglocalizer);

            ret.IsCompleted = false;
            var user = await _user.FindByIdAsync(userId);
            if (user == null)
            {

                ret.AddMessage("UserNotFound", "User is not found.", eMessageType.Error);
                return ret;
            }
            var result = await _user.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                ret.IsCompleted = true;
            }
            else
            {
                ret.IsCompleted = false;
                foreach (var err in result.Errors)
                {
                    ret.AddMessage(err.Code, err.Description, eMessageType.Error);
                }

            }
            return ret;
        }

        public async Task<ReturnMessage> CreateRole(ApplicationRole role)
        {
            var ret = new ReturnMessage(_msglocalizer);

            ret.IsCompleted = false;
            var obj = new ApplicationRole(role.Name, role.ApplicationCode, role.Description);
            var result = await _role.CreateAsync(obj);

            if (result.Succeeded)
            {
                ret.IsCompleted = true;
            }
            else
            {
                ret.IsCompleted = false;
                foreach (var err in result.Errors)
                {
                    ret.AddMessage(err.Code, err.Description, eMessageType.Error);
                }

            }
            return ret;
        }

        public async Task<ReturnObject<UserData>> GetUser(ApplicationUser user)
        {
            var ret = new ReturnObject<UserData>(_msglocalizer);
            user.PasswordHash = "";
            user.SecurityStamp = "";
            var roles = await _user.GetRolesAsync(user);

            //var userRoles = roles.Select(r => new Claim(ClaimTypes.Role, r)).ToArray();
            //var userRoles = roles.Select(r => new Claim(ClaimTypes.Role, r)).ToArray();
            //var appRoles = roles.Select(r => new ApplicationRole(r,"mof",""));
            var userClaims = await _user.GetClaimsAsync(user).ConfigureAwait(false);

            var data = new UserData();
            ret.Data = data;
            data.UserProfile = JsonConvert.DeserializeObject<UserDetail>(JsonConvert.SerializeObject(user));
            var r = DB.UserRoles.Where(w => w.UserId == user.Id).Select(s => new ApplicationRole
            {
                Id = s.RoleId,
                ApplicationCode = s.Role.ApplicationCode,
                Description = s.Role.Description,
                Name = s.Role.Name
            });
            if (user.OrganizationID.HasValue)
            {
                var org = await DB.Organization.Where(w => w.OrganizationId == user.OrganizationID.Value).FirstOrDefaultAsync();
                if (org != null)
                {
                    data.UserProfile.OrganizationCode = org.OrganizationCode;
                }
            }
            var ucs = await _user.GetClaimsAsync(user);
            var claims = DB.UserRoles.Where(w => w.UserId == user.Id).Select(s => s.Role.RoleClaims);
            var roleClaims = new List<string>();
            foreach (var c in claims)
            {
                roleClaims = roleClaims.Union(c.Select(s => s.ClaimValue)).ToList();
            }
            data.UserClaims = new List<string>();
            foreach (var uc in ucs)
            {
                data.UserClaims.Add(uc.Value);
                var rc = roleClaims.Where(w => w == uc.Value).FirstOrDefault();
                if (rc == null)
                {
                    roleClaims.Add(uc.Value);
                }
            }

            data.Permissions = roleClaims;
            data.Roles = r.ToList();


            ret.IsCompleted = true;
            return ret;
        }

        public async Task<ReturnObject<string>> Register(Register reg)
        {
            ReturnObject<string> ret = new ReturnObject<string>(_msglocalizer);

            var result = await _user.CreateAsync(reg.AppUser, reg.Password);
            if (result.Succeeded)
            {
                var code = await _user.GenerateEmailConfirmationTokenAsync(reg.AppUser);
                var callbackUrl = $"{reg.ConfrimUrl}?userid={reg.AppUser.Id}&code={code}"; //  c Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, HttpContext.Request.Scheme);
                await _email.SendEmailAsync(reg.Email, "Confirm register", HtmlEncoder.Default.Encode(callbackUrl));
                await _signin.SignInAsync(reg.AppUser, isPersistent: false);
                ret.IsCompleted = true;
                ret.Data = callbackUrl;
            }
            else
            {
                ret.IsCompleted = false;
                string code = "";
                foreach (var err in result.Errors)
                {
                    switch (err.Code)
                    {
                        case "DuplicateUserName":
                            {
                                code = reg.UserName;
                                break;
                            }
                        case "DuplicateEmail":
                            {
                                code = reg.Email;
                                break;
                            }
                    }
                    ret.AddMessage(err.Code, err.Description, eMessageType.Error, new string[] { code });
                }

            }
            return ret;
        }



        public virtual async Task<ReturnList<ServiceModels.Identity.ApplicationUser>> SearchUsersAsync(ServiceModels.Request.Paging gl)
        {
            var pagedList = new ReturnList<ServiceModels.Identity.ApplicationUser>(_msglocalizer);
            //try
            //{
            Expression<Func<ServiceModels.Identity.ApplicationUser, bool>> searchCondition = x => x.UserName.Contains(gl.SearchText) || x.Email.Contains(gl.SearchText);


            var users = await _user.Users
                .Where(w => !w.UserName.Contains("*del*") || !w.Email.Contains("*del*"))
                .WhereIf(!string.IsNullOrEmpty(gl.SearchText), searchCondition).PageBy(x => x.Id, gl.PageNo, gl.PageSize).ToListAsync();

            pagedList.Data.AddRange(users);

            pagedList.TotalRow = await _user.Users.WhereIf(!string.IsNullOrEmpty(gl.SearchText), searchCondition).CountAsync();
            pagedList.PageSize = gl.PageSize;
            pagedList.PageNo = gl.PageNo;
            pagedList.IsCompleted = true;

            //}
            //catch (Exception ex)
            //{
            //    pagedList.IsCompleted = false;
            //    pagedList.Message.Add(new ErrorMessage { ErrorCode = "SYSERR", ErrorDesc = ex.Message, Language = "en" });
            //    return pagedList;
            //}



            return pagedList;
        }
        public async Task<ReturnMessage> DeleteUserScreen(string userID, string logUserID)
        {
            var ret = new ReturnObject<ReturnMessage>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {

                var user = await DB.Users.Where(w => w.Id == userID).FirstOrDefaultAsync();
                if (user == null)
                {

                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "user", eMessageType.Error, new string[] { "ผู้ใช้งาน" });
                    return ret;
                }
                var del = $"*del*{DateTime.Now.Ticks.ToString()}";
                user.UserName += del;
                user.Email += del;
                await DB.SaveChangesAsync();
                ret.IsCompleted = true;

            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        public async Task<ReturnObject<UserScreen>> GetUserScreen(string userID)
        {
            var ret = new ReturnObject<UserScreen>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var data = new UserScreen();
                var user = await _user.FindByIdAsync(userID);
                if (user == null || user.Email.Contains("*del*"))
                {

                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "user", eMessageType.Error, new string[] { "ผู้ใช้งาน" });
                    return ret;
                }
                var userData = await GetUser(user);
                if (!userData.IsCompleted)
                {
                    ret.CloneMessage(userData.Message);
                    return ret;
                }
                data.UserProfile = new UserProfile
                {
                    Address = user.Address,
                    Department = user.Department,
                    EFirstName = user.EFirstName,
                    ELastName = user.ELastName,
                    Email = user.Email,
                    Id = user.Id,
                    IsActive = user.IsActive,
                    LastAccess = user.LastAccess,
                    Position = user.Position,
                    Tel1 = user.Tel1,
                    Tel2 = user.Tel2,
                    TFirstName = user.TFirstName,
                    TLastName = user.TLastName,
                    UserName = user.UserName

                };
                if (user.OrganizationID.HasValue)
                {
                    var org = await _org.Get(user.OrganizationID.Value, false, false);
                    if (org.IsCompleted)
                    {
                        data.UserProfile.Organization = org.Data;
                    }
                }
                data.Permissions = await GetPermissionGroup();
                var per = data.Permissions.SelectMany(s => s.UserPermissions);
                foreach (var p in userData.Data.UserClaims)
                {
                    var sel = per.Where(w => w.PermissionCode == p).FirstOrDefault();
                    if (sel != null)
                    {
                        sel.IsSelected = true;
                    }
                }
                data.Roles = await DB.Roles.Where(w => w.ApplicationCode == "pdmo").Select(s => new UserRolesSelect
                {
                    IsSelected = false,
                    RoleId = s.Id,
                    RoleName = s.Description

                }).ToListAsync();

                foreach (var r in userData.Data.Roles)
                {
                    var sel = data.Roles.Where(w => w.RoleId == r.Id).FirstOrDefault();
                    if (sel != null)
                    {
                        sel.IsSelected = true;
                    }
                }
                ret.Data = data;
                ret.IsCompleted = true;

            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        public async Task<ReturnObject<string>> ModifyUserScreen(UserScreen u, bool IsCreate, string logUserID)
        {
            var ret = new ReturnObject<string>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                ApplicationUser appUser;
                if (IsCreate)
                {
                    appUser = new ApplicationUser
                    {
                        Email = u.UserProfile.Email,
                        UserName = u.UserProfile.UserName

                    };
                }
                else
                {
                    appUser = await _user.FindByIdAsync(u.UserProfile.Id);
                    if (appUser == null)
                    {
                        ret.AddMessage(eMessage.DataIsNotFound.ToString(), "user", eMessageType.Error, new string[] { _msglocalizer[eMessage.User.ToString()] });
                        return ret;
                    }
                }

                appUser.OrganizationID = u.UserProfile.Organization.OrganizeID;
                appUser.EFirstName = u.UserProfile.EFirstName;
                appUser.ELastName = u.UserProfile.ELastName;
                appUser.TFirstName = u.UserProfile.TFirstName;
                appUser.TLastName = u.UserProfile.TLastName;
                appUser.Address = u.UserProfile.Address;
                appUser.Department = u.UserProfile.Department;
                appUser.IsActive = u.UserProfile.IsActive;
                appUser.Position = u.UserProfile.Position;
                appUser.Tel1 = u.UserProfile.Tel1;
                appUser.Tel2 = u.UserProfile.Tel2;
                appUser.EmailConfirmed = true;

                if (IsCreate)
                {
                    var password = "Kk@" + DateTime.Now.Ticks.ToString();
                    var reg = new Register
                    {
                        Email = u.UserProfile.Email,
                        UserName = u.UserProfile.UserName,
                        ConfirmPassword = password,
                        AppUser = appUser,
                        Password = password

                    };
                    reg.AppUser = appUser;
                    var regist = await Register(reg);
                    if (!regist.IsCompleted)
                    {
                        ret.CloneMessage(regist.Message);
                        return ret;
                    }
                    appUser = await _user.FindByNameAsync(u.UserProfile.UserName);
                    if (appUser == null)
                    {
                        ret.AddMessage(eMessage.DataIsNotFound.ToString(), "user", eMessageType.Error, new string[] { _msglocalizer[eMessage.User.ToString()] });
                        return ret;
                    }
                }
                else
                {

                    var update = await _user.UpdateAsync(appUser);
                    if (!update.Succeeded)
                    {
                        foreach (var err in update.Errors)
                        {
                            ret.AddMessage(err.Code, "update user", eMessageType.Error, new string[] { err.Description });

                        }
                        return ret;
                    }
                }
                var uClaims = await _user.GetClaimsAsync(appUser);
                var remClaim = await _user.RemoveClaimsAsync(appUser, uClaims);
                var uRoles = await _user.GetRolesAsync(appUser);

                var remRole = await _user.RemoveFromRolesAsync(appUser, uRoles);

                //var newRoles = u.Roles.Where(w => w.IsSelected).Select(s => s.RoleName);
                var newRoles = new List<string>();
                foreach (var r in u.Roles.Where(w =>w.IsSelected ))
                {
                    var rid = await DB.Roles.Where(w => w.Id == r.RoleId).FirstOrDefaultAsync();
                    if (rid != null)
                    {
                        newRoles.Add(rid.Name);
                    }
                }
                var allClaims = u.Permissions.SelectMany(s => s.UserPermissions);
                var newClaims = new List<Claim>();  //allClaims.Where(w => w.IsSelected).Select(s => )
                foreach (var cl in allClaims.Where(w => w.IsSelected))
                {
                    newClaims.Add(new Claim("mof/permission", cl.PermissionCode));
                }

                var addRole = await _user.AddToRolesAsync(appUser, newRoles);
                var addClaim = await _user.AddClaimsAsync(appUser, newClaims);


                ret.Data = appUser.Id;
                ret.IsCompleted = true;
                //if (IsCreate)
                //{
                //   await ResetPassword(appUser.Id);
                //}

            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;

        }
        public async Task<List<UserPermissionGroup>> GetPermissionGroup()
        {
            var ret = await DB.PermissionGroup.Select(s => new UserPermissionGroup
            {

                PermissionGroupCode = s.PermissionGroupCode,
                PermissionGroupDetail = s.PermissionGroupDetail,
                PermissionGroupName = s.PermissionGroupName,
                UserPermissions = s.Permission.Select(p => new UserPermission
                {
                    IsSelected = false,
                    PermissionCode = p.PermissionCode,
                    PermissionName = p.PermissionName
                }).ToList()
            }).ToListAsync();
            return ret;
        }
        /// <summary>
        /// return.Data คือ email ขอ user ที่ระบุ
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<ReturnObject<string>> ResetPassword(string userName)
        {
            var ret = new ReturnObject<string>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                // var user = await _user.FindByIdAsync(userId);
                var user = await DB.Users.Where(w => w.UserName == userName).FirstOrDefaultAsync();
                if (user == null)
                {

                    ret.AddMessage("UserNotFound", "User is not found.", eMessageType.Error);
                    return ret;
                }
                var randon = new Random();
                var code = randon.Next(0, 999999).ToString().PadLeft(6,'0');
                user.Pin = code;
                user.PincreatedDt = DateTime.Now;
                await DB.SaveChangesAsync();
                var msg = $"PIN code = {code} โดย PIN จะถูกยกเลิกภายใน 1 ชั่วโมง"; //  c Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, HttpContext.Request.Scheme);
                //await _email.SendEmailAsync(user.Email, "Confirm register", HtmlEncoder.Default.Encode(callbackUrl));
                var mail = new MailMessage
                {
                    Sender = new MailAddress("noreply@pdmo.go.th"),
                    Body = msg,
                    Subject = "Change password"
                };
                mail.From = new MailAddress("noreply@pdmo.go.th");
                mail.To.Add(new MailAddress(user.Email));
                var send = await _sys.SendEmail(mail);
                if (!send.IsCompleted)
                {
                    ret.CloneMessage(send.Message);
                    return ret;
                }
                ret.IsCompleted = true;
                ret.Data = user.Email;


            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }

            return ret;
        }

        public async Task<ReturnMessage> ChangePassword(ChangePassword p, bool CheckPINOnly)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {

                var user = await DB.Users.Where(w => w.UserName == p.UserName).FirstOrDefaultAsync();
                if (user == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "user", eMessageType.Error, new string[] { eMessage.User.ToString() }) ;
                    return ret;
                }
                if (string.IsNullOrEmpty(user.Pin) || user.Pin != p.PIN)
                {
                    ret.AddMessage(eMessage.PINError.ToString(), "user", eMessageType.Error, null);
                    return ret;
                }
                var expire = DateTime.Now.AddHours(-1);
                if (!user.PincreatedDt.HasValue || expire > user.PincreatedDt.Value)
                {
                    ret.AddMessage(eMessage.PINExpired.ToString(), "user", eMessageType.Error, null);
                    return ret;
                }
                var u = await _user.FindByNameAsync(p.UserName);
                if (u == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "user", eMessageType.Error, new string[] { eMessage.User.ToString() });
                    return ret;
                }
                if (CheckPINOnly)
                {
                    ret.IsCompleted = true;
                }else
                {
                    u.PasswordHash = _user.PasswordHasher.HashPassword(u, p.NewPassword);
                    u.IsActive = true;
                    var result = await _user.UpdateAsync(u);
                    //user.Pin = null;
                    //user.PincreatedDt = null;
                    //await DB.SaveChangesAsync();
                    if (result.Succeeded)
                    {
                        ret.IsCompleted = true;
                    }
                    else
                    {
                        ret.IsCompleted = false;
                        foreach (var err in result.Errors)
                        {
                            ret.AddMessage(err.Code, err.Description, eMessageType.Error);
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }

            return ret;
        }

        public async Task<ReturnObject<UserData>> GetUserByToken(string token)
        {
            var ret = new ReturnObject<UserData>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
                if (DateTime.Now > securityToken.ValidTo)
                {
                    ret.AddMessage("TokenExpire", "Token is expired", eMessageType.Error);
                    return ret;
                }
                var claim = securityToken.Claims.Where(w => w.Type == "sub").FirstOrDefault();
                if (claim == null)
                {
                    ret.AddMessage("TokenNotValid", "Token is not valid (sub)", eMessageType.Error);
                    return ret;
                }
                var u = await _user.FindByIdAsync(claim.Value);
                var user  = await GetUser(u);

                if (!user.IsCompleted)
                {
                    ret.CloneMessage(ret.Message);
                    return ret;
                }
                //user.Data.JwtClaim = securityToken.Claims.ToList();
                ret.Data = user.Data;
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }

            return ret;
        }
    }
}
