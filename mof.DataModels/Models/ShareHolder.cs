using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class ShareHolder
    {
        public long ShareHolderId { get; set; }
        public long OrganizationId { get; set; }
        public long? OrgshareHolder { get; set; }
        public decimal Proportion { get; set; }
        public byte[] TimeStamp { get; set; }
        public string OrganizationName { get; set; }

        public virtual Organization Organization { get; set; }
        public virtual Organization OrgshareHolderNavigation { get; set; }
    }
}
