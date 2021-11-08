using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class DebtPayAmt
    {
        public long DebtPayAmtId { get; set; }
        public long? PaymentPlanId { get; set; }
        public long? PlanAmount { get; set; }
        public long? InterestSaveAmount { get; set; }
        public string InterestReference { get; set; }
        public byte[] TimeStamp { get; set; }

        public virtual Amount InterestSaveAmountNavigation { get; set; }
        public virtual PaymentPlan PaymentPlan { get; set; }
        public virtual Amount PlanAmountNavigation { get; set; }
    }
}
