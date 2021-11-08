using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class PlanExtend
    {
        public long PlanExtendId { get; set; }
        public long? PlanId { get; set; }
        public string Data { get; set; }
        public string DataGroup { get; set; }

        public virtual Plan Plan { get; set; }
    }
}
