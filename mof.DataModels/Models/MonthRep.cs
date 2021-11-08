using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class MonthRep
    {
        public long MonthRepId { get; set; }
        public long? NewDebtId { get; set; }
        public long? ExistDebtId { get; set; }
        public byte[] TimeStamp { get; set; }
    }
}
