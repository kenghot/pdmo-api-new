using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class PlanProjectFile
    {
        public long Id { get; set; }
        public long PlanProjectId { get; set; }
        public string Detail { get; set; }
        public long? FileId { get; set; }

        public virtual AttachFile File { get; set; }
        public virtual PlanProject PlanProject { get; set; }
    }
}
