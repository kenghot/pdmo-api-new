using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class PlanActAmount
    {
        public long PlanActAmountId { get; set; }
        public long PlanActId { get; set; }
        public long AmountId { get; set; }
        public byte[] TimeStamp { get; set; }

        public virtual Amount Amount { get; set; }
        public virtual PlanAct PlanAct { get; set; }
    }
}
