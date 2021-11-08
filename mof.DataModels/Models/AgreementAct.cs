using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class AgreementAct
    {
        public long AgreementActId { get; set; }
        public long? AgreementId { get; set; }
        public long? PlanActId { get; set; }
        public byte[] TimeStamp { get; set; }

        public virtual Agreement Agreement { get; set; }
        public virtual PlanAct PlanAct { get; set; }
    }
}
