using mof.ServiceModels.Organization;
using System;
using System.Collections.Generic;
using System.Text;

namespace mof.ServiceModels.Identity
{
    public class UserProfile
    {
        public string Id { get; set; }
        /// <summary>
        ///  รหัสผู้ใช้งาน
        /// </summary>
        public string UserName { get; set; }
        public string Email { get; set; }
        /// <summary>
        /// ชื่อภาษาไทย
        /// </summary>
        public string TFirstName { get; set; }
        /// <summary>
        /// นามสกุลภาษาไทย
        /// </summary>
        public string TLastName { get; set; }
        public string EFirstName { get; set; }
        public string ELastName { get; set; }
        public DateTime? LastAccess { get; set; }
        /// <summary>
        /// ส่วนงาน
        /// </summary>
        public string Department { get; set; }
        /// <summary>
        /// เบอร์ติดต่อ 1
        /// </summary>
        public string Tel1 { get; set; }
        /// <summary>
        /// เบอร์ติดต่อ 2
        /// </summary>
        public string Tel2 { get; set; }
        public bool? IsActive { get; set; }
        /// <summary>
        /// ที่อยู่
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// ตำแหน่ง
        /// </summary>
        public string Position { get; set; }
        public ORGModel Organization { get; set; }
    }
    public class UserScreen
    {
        /// <summary>
        /// s312a : A1 - A10
        /// </summary>
        public UserProfile UserProfile { get; set; }
        /// <summary>
        /// s132b : A
        /// </summary>
        public List<UserRolesSelect> Roles { get; set; }
        /// <summary>
        /// s132b : B - E
        /// </summary>
        public List<UserPermissionGroup> Permissions { get; set; }
    }
    public class UserRolesSelect
    {
        public bool IsSelected { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
      
    }
    public class UserPermissionGroup
    {
        public string PermissionGroupCode { get; set; }
        public string PermissionGroupName { get; set; }
        public string PermissionGroupDetail { get; set; }
        public List<UserPermission> UserPermissions { get; set; }

    }
    public class UserPermission
    {
        public string PermissionCode { get; set; }
        public string PermissionName { get; set; }
        public string PermissionDetail { get; set; }
        public bool IsSelected { get; set; }

    }
}
