using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class Agreement
    {
        public Agreement()
        {
            AgreementAct = new HashSet<AgreementAct>();
            AgreementPaymentPlan = new HashSet<AgreementPaymentPlan>();
            AgreementTrans = new HashSet<AgreementTrans>();
            PlanAgreement = new HashSet<PlanAgreement>();
            PlanExistAgreement = new HashSet<PlanExistAgreement>();
        }

        public long AgreementId { get; set; }
        public string GftrrefCode { get; set; }
        public string Description { get; set; }
        public string Counterparty { get; set; }
        public string ReferenceCode { get; set; }
        public decimal InterestRate { get; set; }
        public DateTime SignDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime IncomingDueDate { get; set; }
        public decimal LoanAmountThb { get; set; }
        public decimal OutStandingDebtThb { get; set; }
        public decimal IncomingDueAmount { get; set; }
        public decimal LoanAge { get; set; }
        public long OrganizationId { get; set; }
        public decimal OutStandingDebt { get; set; }
        public decimal LoanAmount { get; set; }
        public string LoanCurrency { get; set; }
        public bool IsGuarantee { get; set; }
        public string SourceType { get; set; }
        public string AcctAssRefName { get; set; }
        public string MasterAgreement { get; set; }
        public string InterestFormula { get; set; }
        public string DebtType { get; set; }
        public string DebtSubType { get; set; }
        public string Ptyp { get; set; }
        public string Ttyp { get; set; }
        public string AgreementNameTh { get; set; }
        public long? ForOrganization { get; set; }
        public string TbillFlag { get; set; }

        public virtual Organization Organization { get; set; }
        public virtual ICollection<AgreementAct> AgreementAct { get; set; }
        public virtual ICollection<AgreementPaymentPlan> AgreementPaymentPlan { get; set; }
        public virtual ICollection<AgreementTrans> AgreementTrans { get; set; }
        public virtual ICollection<PlanAgreement> PlanAgreement { get; set; }
        public virtual ICollection<PlanExistAgreement> PlanExistAgreement { get; set; }
    }
}
