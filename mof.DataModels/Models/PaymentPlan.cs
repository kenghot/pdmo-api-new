using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class PaymentPlan
    {
        public PaymentPlan()
        {
            AgreementPaymentPlan = new HashSet<AgreementPaymentPlan>();
            DebtPayAmt = new HashSet<DebtPayAmt>();
        }

        public long PaymentPlanId { get; set; }
        public long? PlanExistId { get; set; }
        public string ManageType { get; set; }
        public long? DebtPaymentPlanType { get; set; }
        public long? PaymentSource { get; set; }
        public bool? IsRequestGuarantee { get; set; }

        public virtual CeLov DebtPaymentPlanTypeNavigation { get; set; }
        public virtual CeLov PaymentSourceNavigation { get; set; }
        public virtual PlanExist PlanExist { get; set; }
        public virtual ICollection<AgreementPaymentPlan> AgreementPaymentPlan { get; set; }
        public virtual ICollection<DebtPayAmt> DebtPayAmt { get; set; }
    }
}
