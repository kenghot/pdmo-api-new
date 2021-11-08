using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class IipmProjectPlan
    {
        public long ProjPlanId { get; set; }
        public long? ProjId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public long? UpdatedBy { get; set; }
        public long? PlanConfigId { get; set; }
        public string CoordinatorName { get; set; }
        public string CoordinatorPosition { get; set; }
        public string CoordinatorTel { get; set; }
        public string CoordinatorMail { get; set; }
        public int? Year { get; set; }
        public bool? IsEnable { get; set; }
        public bool? IsActive { get; set; }
    }
}
