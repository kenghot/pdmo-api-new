using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class ProposalPlan
    {
        public long ProposalPlanId { get; set; }
        public long? ProposalId { get; set; }
        public long? PlanId { get; set; }
        public byte[] TimeStamp { get; set; }

        public virtual Plan Plan { get; set; }
        public virtual Plan Proposal { get; set; }
    }
}
