using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class Plan
    {
        public Plan()
        {
            Dscrnote = new HashSet<Dscrnote>();
            PlanAgreement = new HashSet<PlanAgreement>();
            PlanAttach = new HashSet<PlanAttach>();
            PlanExist = new HashSet<PlanExist>();
            PlanExtend = new HashSet<PlanExtend>();
            PlanFinance = new HashSet<PlanFinance>();
            PlanProject = new HashSet<PlanProject>();
            ProposalPlanPlan = new HashSet<ProposalPlan>();
            ProposalPlanProposal = new HashSet<ProposalPlan>();
        }

        public long PlanId { get; set; }
        public string PlanCode { get; set; }
        public int StartYear { get; set; }
        public long PlanType { get; set; }
        public long OrganizationId { get; set; }
        public byte[] TimeStamp { get; set; }
        public long PlanRelease { get; set; }
        public long DataLog { get; set; }
        public int? Month { get; set; }
        public long? ProposalStatus { get; set; }

        public virtual DataLog DataLogNavigation { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual CeLov PlanReleaseNavigation { get; set; }
        public virtual CeLov PlanTypeNavigation { get; set; }
        public virtual DataLog ProposalStatusNavigation { get; set; }
        public virtual ICollection<Dscrnote> Dscrnote { get; set; }
        public virtual ICollection<PlanAgreement> PlanAgreement { get; set; }
        public virtual ICollection<PlanAttach> PlanAttach { get; set; }
        public virtual ICollection<PlanExist> PlanExist { get; set; }
        public virtual ICollection<PlanExtend> PlanExtend { get; set; }
        public virtual ICollection<PlanFinance> PlanFinance { get; set; }
        public virtual ICollection<PlanProject> PlanProject { get; set; }
        public virtual ICollection<ProposalPlan> ProposalPlanPlan { get; set; }
        public virtual ICollection<ProposalPlan> ProposalPlanProposal { get; set; }
    }
}
