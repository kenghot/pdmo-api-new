using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class IipmApproval
    {
        public long ApprovalId { get; set; }
        public string Detail { get; set; }
        public byte[] Pdmocode { get; set; }
        public bool? IsActive { get; set; }
    }
}
