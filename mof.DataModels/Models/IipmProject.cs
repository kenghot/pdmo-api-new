using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class IipmProject
    {
        public long ProjId { get; set; }
        public string Name { get; set; }
        public string ProvinceCode { get; set; }
        public long? AgencyId { get; set; }
        public string Background { get; set; }
        public string Goal { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public decimal? Budget { get; set; }
        public long? SectorId { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public bool? IsPlanLocked { get; set; }
        public decimal? ImportContent { get; set; }
        public DateTime? CreatedAt { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public long? UpdatedBy { get; set; }
        public bool? IsOnGoing { get; set; }
        public string IdRef { get; set; }
        public string FlagTypeId { get; set; }
        public string KindTypeId { get; set; }
        public string OperationTypeCode { get; set; }
        public DateTime? OperationAt { get; set; }
        public string DirectorMail { get; set; }
        public string DirectorTel { get; set; }
        public bool? HasPvy { get; set; }
        public bool? HasEld { get; set; }
        public bool? IsGovBurden { get; set; }
        public long? CreditChannelId { get; set; }
        public string Code { get; set; }
        public string ProjectScope { get; set; }
        public string ProjectArea { get; set; }
        public bool? ProjectLocked { get; set; }
        public bool? ProjectLogFrameLocked { get; set; }
        public long? PdmoprojId { get; set; }
    }
}
