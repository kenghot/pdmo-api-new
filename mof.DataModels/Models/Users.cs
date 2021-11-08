using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class Users
    {
        public Users()
        {
            UserClaims = new HashSet<UserClaims>();
            UserLogins = new HashSet<UserLogins>();
            UserRoles = new HashSet<UserRoles>();
            UserTokens = new HashSet<UserTokens>();
        }

        public string Id { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string TfirstName { get; set; }
        public string TlastName { get; set; }
        public string EfirstName { get; set; }
        public string ElastName { get; set; }
        public long? OrganizationId { get; set; }
        public DateTime? LastAccess { get; set; }
        public string Department { get; set; }
        public string Tel1 { get; set; }
        public string Tel2 { get; set; }
        public bool? IsActive { get; set; }
        public string Address { get; set; }
        public string Position { get; set; }
        public string Pin { get; set; }
        public DateTime? PincreatedDt { get; set; }

        public virtual ICollection<UserClaims> UserClaims { get; set; }
        public virtual ICollection<UserLogins> UserLogins { get; set; }
        public virtual ICollection<UserRoles> UserRoles { get; set; }
        public virtual ICollection<UserTokens> UserTokens { get; set; }
    }
}
