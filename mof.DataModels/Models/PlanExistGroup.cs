using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class PlanExistGroup
    {
        public long PlanExistGroupId { get; set; }
        public long PlanId { get; set; }
        public long PlanExistId { get; set; }
        public TimeSpan? TimeStamp { get; set; }

        public virtual Plan Plan { get; set; }
        public virtual PlanExist PlanExist { get; set; }
    }
}
