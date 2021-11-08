using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class PlanLoan
    {
        public long PlanLoanId { get; set; }
        public long PlanProjectId { get; set; }
        public string PeriodType { get; set; }
        public int PeriodValue { get; set; }
        public string LoanType { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal? Thbamount { get; set; }
        public string LoanCurrency { get; set; }
        public byte[] TimeStamp { get; set; }
        public long? SourceLoanId { get; set; }
        public decimal? Jan { get; set; }
        public decimal? Feb { get; set; }
        public decimal? Mar { get; set; }
        public decimal? Apr { get; set; }
        public decimal? May { get; set; }
        public decimal? Jun { get; set; }
        public decimal? Jul { get; set; }
        public decimal? Aug { get; set; }
        public decimal? Sep { get; set; }
        public decimal? Oct { get; set; }
        public decimal? Nov { get; set; }
        public decimal? Dec { get; set; }

        public virtual PlanProject PlanProject { get; set; }
        public virtual CeLov SourceLoan { get; set; }
    }
}
