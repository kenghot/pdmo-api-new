using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class PlanProject
    {
        public PlanProject()
        {
            PlanAct = new HashSet<PlanAct>();
            PlanLoan = new HashSet<PlanLoan>();
            PlanProjectFile = new HashSet<PlanProjectFile>();
            PlanProjectResolution = new HashSet<PlanProjectResolution>();
        }

        public long PlanProjectId { get; set; }
        public long PlanId { get; set; }
        public long ProjectId { get; set; }
        public byte[] TimeStamp { get; set; }
        public long ProjectType { get; set; }
        public bool? IsNotRequiredApproval { get; set; }
        public string MasterAgreement { get; set; }
        public string CoordinatorName { get; set; }
        public string CoordinatorPosition { get; set; }
        public string CoordinatorTel { get; set; }
        public string CoordinatorEmail { get; set; }

        public virtual Plan Plan { get; set; }
        public virtual Project Project { get; set; }
        public virtual CeLov ProjectTypeNavigation { get; set; }
        public virtual ICollection<PlanAct> PlanAct { get; set; }
        public virtual ICollection<PlanLoan> PlanLoan { get; set; }
        public virtual ICollection<PlanProjectFile> PlanProjectFile { get; set; }
        public virtual ICollection<PlanProjectResolution> PlanProjectResolution { get; set; }
    }
}
