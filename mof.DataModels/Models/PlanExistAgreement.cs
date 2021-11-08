using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class PlanExistAgreement
    {
        public long PlanExistAgreementId { get; set; }
        public long? PlanExistId { get; set; }
        public long AgreementId { get; set; }
        public byte[] TimeStamp { get; set; }
        public decimal? ActualDueAmount { get; set; }
        public long? TransactionType { get; set; }
        public DateTime? ActualDueDate { get; set; }
        public long? Objective { get; set; }
        public string MasterAgreement { get; set; }
        public long? PlanType { get; set; }

        public virtual Agreement Agreement { get; set; }
        public virtual CeLov ObjectiveNavigation { get; set; }
        public virtual PlanExist PlanExist { get; set; }
        public virtual CeLov PlanTypeNavigation { get; set; }
        public virtual CeLov TransactionTypeNavigation { get; set; }
    }
}
