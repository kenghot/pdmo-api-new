using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class YProjectTa
    {
        public long ProjectId { get; set; }
        public string ProjectNameTh { get; set; }
        public string ProjectNameEn { get; set; }
        public string ProjectDesc { get; set; }
        public string ProjectObjective { get; set; }
        public string ProjectScope { get; set; }
        public string ProjectExpected { get; set; }
        public string ProjectStartDate { get; set; }
        public string ProjectEndDate { get; set; }
        public string ProjectConcordance1 { get; set; }
        public string ProjectConcordance1Detail { get; set; }
        public string ProjectConcordance2 { get; set; }
        public string ProjectConcordance2Detail { get; set; }
        public string ProjectConcordance3 { get; set; }
        public string ProjectConcordance3Detail { get; set; }
        public string ProjectConcordance4 { get; set; }
        public string ProjectConcordance4Detail { get; set; }
        public string ProjectConcordance5 { get; set; }
        public string ProjectConcordance5Detail { get; set; }
        public string ProjectConcordance6 { get; set; }
        public string ProjectConcordance6Detail { get; set; }
        public string ProjectHelpReceived { get; set; }
        public string ProjectHelpReceivedDetail { get; set; }
        public string ProjectRemark { get; set; }
        public string ProjectContractFirstName { get; set; }
        public string ProjectContractLastName { get; set; }
        public string ProjectContractPosition { get; set; }
        public string ProjectContractDepartment { get; set; }
        public string ProjectContractOrg { get; set; }
        public string ProjectContractTel { get; set; }
        public string ProjectContractFax { get; set; }
        public string ProjectContractEmail { get; set; }
        public int? ProjectYear { get; set; }
        public int? OrganizationId { get; set; }
    }
}
