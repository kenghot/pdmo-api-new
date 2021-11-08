using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class PermissionGroup
    {
        public PermissionGroup()
        {
            Permission = new HashSet<Permission>();
        }

        public string PermissionGroupCode { get; set; }
        public string PermissionGroupName { get; set; }
        public byte[] TimeStamp { get; set; }
        public string PermissionGroupDetail { get; set; }

        public virtual ICollection<Permission> Permission { get; set; }
    }
}
