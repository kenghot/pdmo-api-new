using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class Amount
    {
        public Amount()
        {
            DebtPayAmtInterestSaveAmountNavigation = new HashSet<DebtPayAmt>();
            DebtPayAmtPlanAmountNavigation = new HashSet<DebtPayAmt>();
            PlanActAmount = new HashSet<PlanActAmount>();
            PlanFinance = new HashSet<PlanFinance>();
            ProjAmt = new HashSet<ProjAmt>();
        }

        public long AmountId { get; set; }
        public long? AmountType { get; set; }
        public string PeriodType { get; set; }
        public int PeriodValue { get; set; }
        public string SourceType { get; set; }
        public decimal Amount1 { get; set; }
        public string Currency { get; set; }
        public byte[] TimeStamp { get; set; }
        public string AmountGroup { get; set; }

        public virtual CeLov AmountTypeNavigation { get; set; }
        public virtual ICollection<DebtPayAmt> DebtPayAmtInterestSaveAmountNavigation { get; set; }
        public virtual ICollection<DebtPayAmt> DebtPayAmtPlanAmountNavigation { get; set; }
        public virtual ICollection<PlanActAmount> PlanActAmount { get; set; }
        public virtual ICollection<PlanFinance> PlanFinance { get; set; }
        public virtual ICollection<ProjAmt> ProjAmt { get; set; }
    }
}
