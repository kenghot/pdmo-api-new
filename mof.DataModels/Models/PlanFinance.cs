using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class PlanFinance
    {
        public long PlanFinanceId { get; set; }
        public long? PlanId { get; set; }
        public long? AmountId { get; set; }
        public byte[] TimeStamp { get; set; }
        public string Data { get; set; }

        public virtual Amount Amount { get; set; }
        public virtual Plan Plan { get; set; }
    }
}
