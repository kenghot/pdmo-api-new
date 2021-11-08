using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class ProjectLocation
    {
        public long Id { get; set; }
        public long ProjectId { get; set; }
        public string Location { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public virtual Project Project { get; set; }
    }
}
