using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class ProjectExtend
    {
        public long Id { get; set; }
        public long ProjectId { get; set; }
        public string ExtendData { get; set; }
        public string GroupCode { get; set; }

        public virtual Project Project { get; set; }
    }
}
