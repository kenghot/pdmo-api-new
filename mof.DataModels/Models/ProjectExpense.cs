using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class ProjectExpense
    {
        public long ProjectExpenseId { get; set; }
        public long ProjectId { get; set; }
        public byte[] TimeStamp { get; set; }
        public string ExpenseName { get; set; }

        public virtual Project Project { get; set; }
    }
}
