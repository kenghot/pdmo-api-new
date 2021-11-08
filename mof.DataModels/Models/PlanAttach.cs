using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class PlanAttach
    {
        public long PlanAttachId { get; set; }
        public long? PlanId { get; set; }
        public long? AttachFileId { get; set; }
        public byte[] TimeStamp { get; set; }

        public virtual AttachFile AttachFile { get; set; }
        public virtual Plan Plan { get; set; }
    }
}
