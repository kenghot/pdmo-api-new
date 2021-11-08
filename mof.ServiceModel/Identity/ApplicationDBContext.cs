 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using mof.ServiceModels.Identity;
using Microsoft.AspNetCore.Identity;
namespace mof.ServiceModels.Identity
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Core Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Core Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);


            builder.Entity<ApplicationRole>().ToTable(TableConsts.IdentityRoles);
            //builder.Entity<IdentityRole>().ToTable(TableConsts.IdentityRoles);
            builder.Entity<IdentityRoleClaim<string>>().ToTable(TableConsts.IdentityRoleClaims);
            builder.Entity<IdentityUserRole<string>>().ToTable(TableConsts.IdentityUserRoles);

            builder.Entity<ApplicationUser>().ToTable(TableConsts.IdentityUsers);
            builder.Entity<IdentityUserLogin<string>>().ToTable(TableConsts.IdentityUserLogins);
            builder.Entity<IdentityUserClaim<string>>().ToTable(TableConsts.IdentityUserClaims);
            builder.Entity<IdentityUserToken<string>>().ToTable(TableConsts.IdentityUserTokens);
            
            //// Add this, so that IdentityRole can share a table with ApplicationRole:
            //builder.Entity<IdentityRole>().ToTable("Roles");

            //// Change these from IdentityRole to ApplicationRole:
            //var entityTypeConfiguration1 =
            //    builder.Entity<ApplicationRole>().ToTable("Roles");
            //entityTypeConfiguration1.Property((ApplicationRole r) => r.Name).IsRequired();
        }
        public static class TableConsts
        {
            public const string IdentityRoles = "Roles";
            public const string IdentityRoleClaims = "RoleClaims";
            public const string IdentityUserRoles = "UserRoles";
            public const string IdentityUsers = "Users";
            public const string IdentityUserLogins = "UserLogins";
            public const string IdentityUserClaims = "UserClaims";
            public const string IdentityUserTokens = "UserTokens";
        }
    }
}