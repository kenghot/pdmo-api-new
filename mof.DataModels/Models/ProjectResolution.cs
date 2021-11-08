using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class ProjectResolution
    {
        public long Id { get; set; }
        public long ProjectId { get; set; }
        public string Detail { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public long? FileId { get; set; }

        public virtual AttachFile File { get; set; }
        public virtual Project Project { get; set; }
    }
}
