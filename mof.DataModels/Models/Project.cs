using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class Project
    {
        public Project()
        {
            PlanProject = new HashSet<PlanProject>();
            ProjAct = new HashSet<ProjAct>();
            ProjAmt = new HashSet<ProjAmt>();
            ProjMaterial = new HashSet<ProjMaterial>();
            ProjToMany = new HashSet<ProjToMany>();
            ProjectExtend = new HashSet<ProjectExtend>();
            ProjectLocation = new HashSet<ProjectLocation>();
            ProjectResolution = new HashSet<ProjectResolution>();
        }

        public long ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectThname { get; set; }
        public string ProjectEnname { get; set; }
        public bool IsCanceled { get; set; }
        public byte[] TimeStamp { get; set; }
        public decimal LimitAmount { get; set; }
        public int StartYear { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ProjectBackground { get; set; }
        public string ProjectTarget { get; set; }
        public string ProjectObjective { get; set; }
        public string ProjectScope { get; set; }
        public decimal Firr { get; set; }
        public decimal Eirr { get; set; }
        public long ProjectType { get; set; }
        public long DataLog { get; set; }
        public long OrganizationId { get; set; }
        public int Pdmoagreement { get; set; }
        public int ResolutionAgreement { get; set; }
        public bool IsNewProject { get; set; }
        public string CapitalSource { get; set; }
        public string ProjectBranch { get; set; }
        public long? ProvinceId { get; set; }
        public long? SectorId { get; set; }
        public long? StatusId { get; set; }
        public long? CreditChannelId { get; set; }
        public bool? IsGovBurden { get; set; }
        public bool? IsOnGoing { get; set; }
        public bool? HasEld { get; set; }
        public string DirectorName { get; set; }
        public string DirectorPosition { get; set; }
        public string DirectorMail { get; set; }
        public string DirectorTel { get; set; }
        public string MapDrawing { get; set; }

        public virtual CeLov CreditChannel { get; set; }
        public virtual DataLog DataLogNavigation { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual CeLov ProjectTypeNavigation { get; set; }
        public virtual CeLov Province { get; set; }
        public virtual CeLov Sector { get; set; }
        public virtual CeLov Status { get; set; }
        public virtual ICollection<PlanProject> PlanProject { get; set; }
        public virtual ICollection<ProjAct> ProjAct { get; set; }
        public virtual ICollection<ProjAmt> ProjAmt { get; set; }
        public virtual ICollection<ProjMaterial> ProjMaterial { get; set; }
        public virtual ICollection<ProjToMany> ProjToMany { get; set; }
        public virtual ICollection<ProjectExtend> ProjectExtend { get; set; }
        public virtual ICollection<ProjectLocation> ProjectLocation { get; set; }
        public virtual ICollection<ProjectResolution> ProjectResolution { get; set; }
    }
}
