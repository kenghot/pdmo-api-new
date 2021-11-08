using System;
using System.Collections.Generic;
using System.Text;

namespace mof.ServiceModels.IIPMModel
{
   
        public class LoginRespone
        {
            public long LoggedIn { get; set; }
            public string Token { get; set; }
        }
        public class LoginRequest
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }
    public class PlanYearBudget
    {
        public int id { get; set; }
        public int projectPlanId { get; set; }
        public int year { get; set; }
        public decimal? budget { get; set; }
        public int? sourceOfFundId { get; set; }
    }

    public class CurrentProjectPlan
    {
        public List<PlanYearBudget> planYearBudgets { get; set; }
        public List<PlanApproval> planApprovals { get; set; }
        public int id { get; set; }
        public int projectId { get; set; }
        public DateTime? createdAt { get; set; }
        public int? createdBy { get; set; }
        public DateTime? updatedAt { get; set; }
        public int? updatedBy { get; set; }
        public int planConfigId { get; set; }
        public string coordinatorName { get; set; }
        public string coordinatorPosition { get; set; }
        public string coordinatorTel { get; set; }
        public string coordinatorMail { get; set; }
        public string currentPlanApprovalStatus { get; set; }
        public PlanConfig planConfig { get; set; }
    }
    public class PlanConfig
    {
        public DateTime? submitStartedAt { get; set; }
        public DateTime? submitFinishedAt { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public int? year { get; set; }
        public bool? isEnable { get; set; }
        public bool? isActive { get; set; }
    }
    public class Agency
    {
        public string ministryCode { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public bool? isActive { get; set; }
        public long id { get; set; }
        public long? ministryId { get; set; }
        public string sId { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
    }

    public class Province
    {
        public string code { get; set; }
        public string name { get; set; }
        public bool? isFlood { get; set; }
        public int flagGroup { get; set; }
        public int provinceGroup { get; set; }
    }

    public class Sector
    {
        public int id { get; set; }
        public string name { get; set; }
        public int level { get; set; }
        public int parentId { get; set; }
        public bool? isActive { get; set; }
    }

    public class ProjectInfo
    {
        public Agency agency { get; set; }
        public Province province { get; set; }
        public Sector sector { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string provinceCode { get; set; }
        public int agencyId { get; set; }
        public string directorName { get; set; }
        public string directorPosition { get; set; }
        public string background { get; set; }
        public string goal { get; set; }
        public DateTime? approvedAt { get; set; }
        public decimal? budget { get; set; }
        public int sectorId { get; set; }
        public DateTime? startedAt { get; set; }
        public DateTime? endedAt { get; set; }
        public bool? isPlanLocked { get; set; }
        public decimal? importContent { get; set; }
        public DateTime? createdAt { get; set; }
        public int? createdBy { get; set; }
        public DateTime? updatedAt { get; set; }
        public int? updatedBy { get; set; }
        public bool? isOnGoing { get; set; }
        public string idRef { get; set; }
        public string flagTypeId { get; set; }
        public string kindTypeId { get; set; }
        public string operationTypeCode { get; set; }
        public DateTime? operationAt { get; set; }
        public string directorMail { get; set; }
        public string directorTel { get; set; }
        public bool? hasPvy { get; set; }
        public bool? hasEld { get; set; }
        public bool? isGovBurden { get; set; }
        public long? creditChannelId { get; set; }
        public string code { get; set; }
        public int projectID { get; set; }
        public string projectScope { get; set; }
        public string projectArea { get; set; }
        public bool? projectLocked { get; set; }
        public bool? projectLogFrameLocked { get; set; }
    }

    public class ProjectIIPMModel
    {
        public CurrentProjectPlan currentProjectPlan { get; set; }
        public ProjectInfo projectInfo { get; set; }
        public int id { get; set; }
        public int currentProjectPlanId { get; set; }
        public DateTime? createdAt { get; set; }
        public int? createdBy { get; set; }
        public DateTime? updatedAt { get; set; }
        public int? updatedBy { get; set; }
    }
    public class ProjectPlanModel
    {
        public List<ProjectIIPMModel> items { get; set; }
    }
    public class IntegrateConfig
    {
        public bool Ministry { get; set; }
        public bool Sector { get; set; }
        public bool Agency { get; set; }
        public bool ProjectPlan { get; set; }
        public List<long> IIPMProjectIDs { get; set; }
        public bool FromJson { get; set; } = false;
        
    }
    public class ProjectExtendRequest
    {
        public List<long> ProjectIDs { get; set; }
        public bool Objective { get; set; }
        public bool Scope { get; set; }
        public bool FromJson { get; set; } = false;

    }
    public class Approval
    {
        public long id { get; set; }
        public string detail { get; set; }
        public bool? isActive { get; set; }
    }
    public class ApprovalProvision
    {
        public long? id { get; set; }
        public string detail { get; set; }
        public bool? isActive { get; set; }
        public DateTime? createdAt { get; set; }
        public int? createdBy { get; set; }
        public DateTime? updatedAt { get; set; }
        public int? updatedBy { get; set; }
    }

    public class ProvisionItem
    {
        public ApprovalProvision approvalProvision { get; set; }
        public long? id { get; set; }
        public long? approvalProvisionId { get; set; }
        public long? planConfigId { get; set; }
    }

    public class ApprovalItem
    {
        public ProvisionItem provisionItem { get; set; }
        public long? planApprovalId { get; set; }
        public long? provisionItemId { get; set; }
        public bool? isCheck { get; set; }
        public long? id { get; set; }
    }

    public class PlanApproval
    {
        public List<ApprovalItem> approvalItems { get; set; }
        public long? id { get; set; }
        public long? projectPlanId { get; set; }
        public string approvedAt { get; set; }
        public long? approvedBy { get; set; }
        public long? status { get; set; }
        public string comment { get; set; }
        public string role { get; set; }
    }


}
