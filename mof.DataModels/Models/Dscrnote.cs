using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class Dscrnote
    {
        public long DscrnoteId { get; set; }
        public long? PlanId { get; set; }
        public int? Year { get; set; }
        public decimal? Dscr { get; set; }
        public string Reason { get; set; }
        public string Solution { get; set; }
        public string ProgressUpdate { get; set; }
        public byte[] TimeStamp { get; set; }

        public virtual Plan Plan { get; set; }
    }
}
