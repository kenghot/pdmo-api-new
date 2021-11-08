using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
namespace mof.ServiceModels.Identity
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string TFirstName { get; set; }
        public string TLastName { get; set; }
        public string EFirstName { get; set; }
        public string ELastName { get; set; }
        public long? OrganizationID { get; set; }
        public DateTime? LastAccess { get; set; }
        public string Department { get; set; }
        public string Tel1 { get; set; }
        public string Tel2 { get; set; }
        public bool? IsActive { get; set; }
        public string Address { get; set; }
        public string Position { get; set; }
    }
    public class ApplicationRole : IdentityRole
    {
        public string ApplicationCode { get; set; }
        public string Description { get; set; }
        public ApplicationRole() : base() { }
        public ApplicationRole(string name, string applicationCode , string description) : base(name)
        {
            this.ApplicationCode = applicationCode;
            this.Description = description;
        }
  
    }

   public class UserData
    {
        public string JwtAccessToken { get;set;}
        public UserDetail UserProfile { get; set; }
        public List<string> Permissions { get; set; }
        public List<string> UserClaims { get; set; }
        public List<ApplicationRole> Roles { get; set; }
        //public List<Claim> JwtClaim { get; set; }
    }
    public class UserDetail : ApplicationUser
    {
        public string OrganizationCode { get; set; }
    }
}
