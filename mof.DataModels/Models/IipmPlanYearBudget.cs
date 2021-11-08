using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class IipmPlanYearBudget
    {
        public long Pybid { get; set; }
        public long? ProjectPlanId { get; set; }
        public int? Year { get; set; }
        public decimal? Budget { get; set; }
        public long? SourceOfffundId { get; set; }
    }
}
