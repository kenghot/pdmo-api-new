using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class AgreementPaymentPlan
    {
        public long AgreementPaymentPlanId { get; set; }
        public long? AgreementId { get; set; }
        public long? PaymentPlanId { get; set; }
        public byte[] TimeStamp { get; set; }

        public virtual Agreement Agreement { get; set; }
        public virtual PaymentPlan PaymentPlan { get; set; }
    }
}
