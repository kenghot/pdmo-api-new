using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class YContracts
    {
        public long ContractId { get; set; }
        public int? ContractProvince { get; set; }
        public string ContractOrgType { get; set; }
        public string ContractOrgName { get; set; }
        public string ContractSource { get; set; }
        public string ContractNumber { get; set; }
        public int? ContractPurpose { get; set; }
        public int? ContractProjectType { get; set; }
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public int? ContractDurationYear { get; set; }
        public double? ContractPaperAmount { get; set; }
        public double? ContractActualAmount { get; set; }
        public string ContractInterestType { get; set; }
        public double? ContractInterestRate { get; set; }
        public double? ContractLoanPaidback { get; set; }
        public double? ContractLoanPending { get; set; }
        public string ContractRemarks { get; set; }
        public int? ContractQuarter { get; set; }
        public int? ContractYear { get; set; }
        public int? OrganizationId { get; set; }
        public string ContractInterestMeta { get; set; }
    }
}
