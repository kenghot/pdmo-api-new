using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class CeLov
    {
        public CeLov()
        {
            AgreementTrans = new HashSet<AgreementTrans>();
            Amount = new HashSet<Amount>();
            CeLovextend = new HashSet<CeLovextend>();
            DataLog = new HashSet<DataLog>();
            InverseParent = new HashSet<CeLov>();
            OrganizationDebtCalculationNavigation = new HashSet<Organization>();
            OrganizationFieldNavigation = new HashSet<Organization>();
            OrganizationOrgstatusNavigation = new HashSet<Organization>();
            OrganizationOrgtypeNavigation = new HashSet<Organization>();
            OrganizationPdmotypeNavigation = new HashSet<Organization>();
            OrganizationSubFieldNavigation = new HashSet<Organization>();
            PaymentPlanDebtPaymentPlanTypeNavigation = new HashSet<PaymentPlan>();
            PaymentPlanPaymentSourceNavigation = new HashSet<PaymentPlan>();
            PlanExistAgreementObjectiveNavigation = new HashSet<PlanExistAgreement>();
            PlanExistAgreementPlanTypeNavigation = new HashSet<PlanExistAgreement>();
            PlanExistAgreementTransactionTypeNavigation = new HashSet<PlanExistAgreement>();
            PlanLoan = new HashSet<PlanLoan>();
            PlanPlanReleaseNavigation = new HashSet<Plan>();
            PlanPlanTypeNavigation = new HashSet<Plan>();
            PlanProject = new HashSet<PlanProject>();
            ProjActAmountAmountTypeNavigation = new HashSet<ProjActAmount>();
            ProjActAmountSourceLoanNavigation = new HashSet<ProjActAmount>();
            ProjectCreditChannel = new HashSet<Project>();
            ProjectProjectTypeNavigation = new HashSet<Project>();
            ProjectProvince = new HashSet<Project>();
            ProjectSector = new HashSet<Project>();
            ProjectStatus = new HashSet<Project>();
            RequestRequestStatusNavigation = new HashSet<Request>();
            RequestRequestTypeNavigation = new HashSet<Request>();
        }

        public long Lovkey { get; set; }
        public string LovgroupCode { get; set; }
        public string Lovcode { get; set; }
        public string Lovvalue { get; set; }
        public byte[] TimeStamp { get; set; }
        public bool IsCanceled { get; set; }
        public string OrderNo { get; set; }
        public string ParentGroup { get; set; }
        public string ParentLov { get; set; }
        public string Remark { get; set; }

        public virtual CeLovgroup LovgroupCodeNavigation { get; set; }
        public virtual CeLov Parent { get; set; }
        public virtual ICollection<AgreementTrans> AgreementTrans { get; set; }
        public virtual ICollection<Amount> Amount { get; set; }
        public virtual ICollection<CeLovextend> CeLovextend { get; set; }
        public virtual ICollection<DataLog> DataLog { get; set; }
        public virtual ICollection<CeLov> InverseParent { get; set; }
        public virtual ICollection<Organization> OrganizationDebtCalculationNavigation { get; set; }
        public virtual ICollection<Organization> OrganizationFieldNavigation { get; set; }
        public virtual ICollection<Organization> OrganizationOrgstatusNavigation { get; set; }
        public virtual ICollection<Organization> OrganizationOrgtypeNavigation { get; set; }
        public virtual ICollection<Organization> OrganizationPdmotypeNavigation { get; set; }
        public virtual ICollection<Organization> OrganizationSubFieldNavigation { get; set; }
        public virtual ICollection<PaymentPlan> PaymentPlanDebtPaymentPlanTypeNavigation { get; set; }
        public virtual ICollection<PaymentPlan> PaymentPlanPaymentSourceNavigation { get; set; }
        public virtual ICollection<PlanExistAgreement> PlanExistAgreementObjectiveNavigation { get; set; }
        public virtual ICollection<PlanExistAgreement> PlanExistAgreementPlanTypeNavigation { get; set; }
        public virtual ICollection<PlanExistAgreement> PlanExistAgreementTransactionTypeNavigation { get; set; }
        public virtual ICollection<PlanLoan> PlanLoan { get; set; }
        public virtual ICollection<Plan> PlanPlanReleaseNavigation { get; set; }
        public virtual ICollection<Plan> PlanPlanTypeNavigation { get; set; }
        public virtual ICollection<PlanProject> PlanProject { get; set; }
        public virtual ICollection<ProjActAmount> ProjActAmountAmountTypeNavigation { get; set; }
        public virtual ICollection<ProjActAmount> ProjActAmountSourceLoanNavigation { get; set; }
        public virtual ICollection<Project> ProjectCreditChannel { get; set; }
        public virtual ICollection<Project> ProjectProjectTypeNavigation { get; set; }
        public virtual ICollection<Project> ProjectProvince { get; set; }
        public virtual ICollection<Project> ProjectSector { get; set; }
        public virtual ICollection<Project> ProjectStatus { get; set; }
        public virtual ICollection<Request> RequestRequestStatusNavigation { get; set; }
        public virtual ICollection<Request> RequestRequestTypeNavigation { get; set; }
    }
}
