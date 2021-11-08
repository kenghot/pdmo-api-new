using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class ProjToMany
    {
        public long ProjManyId { get; set; }
        public long ProjectId { get; set; }
        public string GroupCode { get; set; }
        public long ManyId { get; set; }
        public byte[] TimeStamp { get; set; }

        public virtual Project Project { get; set; }
    }
}
