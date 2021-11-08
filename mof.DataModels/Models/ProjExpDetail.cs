using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class ProjExpDetail
    {
        public long ProjExpDetialId { get; set; }
        public long ProjectExpenseId { get; set; }
        public string ExpenseType { get; set; }
        public string ExpenseSource { get; set; }
        public string CurrencyCode { get; set; }
        public byte[] TimeStamp { get; set; }
    }
}
