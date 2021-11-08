using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class PlanProjectResolution
    {
        public long Id { get; set; }
        public long PlanProjectId { get; set; }
        public string Detail { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public long? FileId { get; set; }

        public virtual AttachFile File { get; set; }
        public virtual PlanProject PlanProject { get; set; }
    }
}
