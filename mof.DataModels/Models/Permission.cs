using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class Permission
    {
        public string PermissionCode { get; set; }
        public string PermissionName { get; set; }
        public string PermissionGroup { get; set; }
        public byte[] TimeStamp { get; set; }

        public virtual PermissionGroup PermissionGroupNavigation { get; set; }
    }
}
