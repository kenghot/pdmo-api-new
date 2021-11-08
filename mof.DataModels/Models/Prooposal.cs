using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class Prooposal
    {
        public long ProposalId { get; set; }
        public long PlanId { get; set; }
        public long? PlanFiveYearId { get; set; }
        public long? PlanExistId { get; set; }
        public long? PlanNewId { get; set; }
        public long? PlanFinRepId { get; set; }
        public long DataLog { get; set; }
        public byte[] TimeStamp { get; set; }

        public virtual DataLog DataLogNavigation { get; set; }
        public virtual Plan Plan { get; set; }
        public virtual Plan PlanExist { get; set; }
        public virtual Plan PlanFinRep { get; set; }
        public virtual Plan PlanFiveYear { get; set; }
        public virtual Plan PlanNew { get; set; }
    }
}
