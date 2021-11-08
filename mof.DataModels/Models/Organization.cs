using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class Organization
    {
        public Organization()
        {
            Agreement = new HashSet<Agreement>();
            InverseOrgaffiliateNavigation = new HashSet<Organization>();
            Orglod = new HashSet<Orglod>();
            OrgtoMany = new HashSet<OrgtoMany>();
            Plan = new HashSet<Plan>();
            Project = new HashSet<Project>();
            ShareHolderOrganization = new HashSet<ShareHolder>();
            ShareHolderOrgshareHolderNavigation = new HashSet<ShareHolder>();
        }

        public long OrganizationId { get; set; }
        public string OrganizationCode { get; set; }
        public byte[] TimeStamp { get; set; }
        public string OrganizationThname { get; set; }
        public string OrganizationEnname { get; set; }
        public bool IsCanceled { get; set; }
        public long Orgtype { get; set; }
        public string EstablishmentLaw { get; set; }
        public string Remark { get; set; }
        public bool HasLoanPower { get; set; }
        public long DebtCalculation { get; set; }
        public long? SubField { get; set; }
        public string PublicDebtSection { get; set; }
        public string FinanceDebtSection { get; set; }
        public string LoanPowerSection { get; set; }
        public long? Field { get; set; }
        public long? Orgaffiliate { get; set; }
        public long? ChangeRequest { get; set; }
        public long? RequestStatus { get; set; }
        public string RequestData { get; set; }
        public decimal? Pdapropotion { get; set; }
        public decimal? Fdapropotion { get; set; }
        public long Orgstatus { get; set; }
        public string Address { get; set; }
        public string Tel { get; set; }
        public long? Province { get; set; }
        public long? Pdmotype { get; set; }

        public virtual CeLov DebtCalculationNavigation { get; set; }
        public virtual CeLov FieldNavigation { get; set; }
        public virtual Organization OrgaffiliateNavigation { get; set; }
        public virtual CeLov OrgstatusNavigation { get; set; }
        public virtual CeLov OrgtypeNavigation { get; set; }
        public virtual CeLov PdmotypeNavigation { get; set; }
        public virtual Province ProvinceNavigation { get; set; }
        public virtual Request RequestStatusNavigation { get; set; }
        public virtual CeLov SubFieldNavigation { get; set; }
        public virtual ICollection<Agreement> Agreement { get; set; }
        public virtual ICollection<Organization> InverseOrgaffiliateNavigation { get; set; }
        public virtual ICollection<Orglod> Orglod { get; set; }
        public virtual ICollection<OrgtoMany> OrgtoMany { get; set; }
        public virtual ICollection<Plan> Plan { get; set; }
        public virtual ICollection<Project> Project { get; set; }
        public virtual ICollection<ShareHolder> ShareHolderOrganization { get; set; }
        public virtual ICollection<ShareHolder> ShareHolderOrgshareHolderNavigation { get; set; }
    }
}
