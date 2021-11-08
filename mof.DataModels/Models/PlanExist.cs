using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class PlanExist
    {
        public PlanExist()
        {
            PaymentPlan = new HashSet<PaymentPlan>();
            PlanExistAgreement = new HashSet<PlanExistAgreement>();
        }

        public long PlanExistId { get; set; }
        public long? PlanId { get; set; }
        public byte[] TimeStamp { get; set; }
        public bool? IsNotRequiredApproval { get; set; }
        public int? Year { get; set; }

        public virtual Plan Plan { get; set; }
        public virtual ICollection<PaymentPlan> PaymentPlan { get; set; }
        public virtual ICollection<PlanExistAgreement> PlanExistAgreement { get; set; }
    }
}
