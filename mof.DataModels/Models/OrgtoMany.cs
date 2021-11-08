using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class OrgtoMany
    {
        public long OrgtoManyId { get; set; }
        public long OrganizationId { get; set; }
        public long ManyId { get; set; }
        public string GroupCode { get; set; }
        public byte[] TimeStamp { get; set; }

        public virtual Organization Organization { get; set; }
    }
}
