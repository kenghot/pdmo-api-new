using System;
using System.Collections.Generic;
using System.Text;
using mof.DataModels.Models;
using mof.ServiceModels.Common;
 

using System.Threading.Tasks;
using mof.ServiceModels.Identity;

namespace mof.IServices
{
    public interface IIdentityRepository
    {
        //Task<ReturnObject<IEnumerable<AspNetUsers>>> GetUsers();
        Task<ReturnList<ServiceModels.Identity.ApplicationUser>> SearchUsersAsync(ServiceModels.Request.Paging  gl);
        Task<ReturnObject<string>> Register(ServiceModels.Request.Register reg);
        Task<ReturnMessage> ConfirmEmail(string userId, string code);
        Task<ReturnMessage> CreateRole(ServiceModels.Identity.ApplicationRole role);
        Task<ReturnObject<ServiceModels.Identity.UserData>> GetUser(ServiceModels.Identity.ApplicationUser user);

        Task<ReturnObject<UserScreen>> GetUserScreen(string userID);
        Task<ReturnObject<string>> ModifyUserScreen(UserScreen u, bool IsCreate, string logUserID);
        Task<ReturnMessage> DeleteUserScreen(string UserId,  string logUserID);
        Task<List<UserPermissionGroup>> GetPermissionGroup();
        Task<ReturnObject<string>> ResetPassword(string userName);
        Task<ReturnMessage> ChangePassword(ChangePassword p, bool CheckPINOnly);
        Task<ReturnObject<UserData>> GetUserByToken(string token);
    }
}
