using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class ProjActAmount
    {
        public long ProjActAmountId { get; set; }
        public long ProjectActId { get; set; }
        public long AmountType { get; set; }
        public string PeriodType { get; set; }
        public int PeriodValue { get; set; }
        public string SourceType { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public byte[] TimeStamp { get; set; }
        public long? SourceLoan { get; set; }

        public virtual CeLov AmountTypeNavigation { get; set; }
        public virtual ProjAct ProjectAct { get; set; }
        public virtual CeLov SourceLoanNavigation { get; set; }
    }
}
