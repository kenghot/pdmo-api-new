using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class IipmAgency
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public string MinistryCode { get; set; }
        public long? MinistryId { get; set; }
        public string SId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? OrganizationId { get; set; }
    }
}
