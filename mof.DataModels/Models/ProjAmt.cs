using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class ProjAmt
    {
        public long ProjAmtId { get; set; }
        public long? ProjectId { get; set; }
        public long? AmountId { get; set; }
        public byte[] TimeStamp { get; set; }

        public virtual Amount Amount { get; set; }
        public virtual Project Project { get; set; }
    }
}
