using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class PlanAgreement
    {
        public long PlanAgreementId { get; set; }
        public long? PlanId { get; set; }
        public long? AgreementId { get; set; }
        public byte[] TimeStamp { get; set; }

        public virtual Agreement Agreement { get; set; }
        public virtual Plan Plan { get; set; }
    }
}
