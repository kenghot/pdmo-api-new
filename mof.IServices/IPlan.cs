using mof.DataModels.Models;
using mof.ServiceModels.Common;
using mof.ServiceModels.Common.Generic;
using mof.ServiceModels.FinancialReport;
using mof.ServiceModels.MonthlyReport;
using mof.ServiceModels.Plan;
using mof.ServiceModels.Project;
using mof.ServiceModels.Proposal;
using mof.ServiceModels.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace mof.IServices
{
    public interface IPlan
    {
        Task<ReturnObject<long?>> GetPreviousPlanRevision(CreatePlanParameter p,string planType);

        Task<ReturnObject<long?>> CreatePlan(string UserID,string PlanType, CreatePlanParameter p);
        Task<ReturnList<PlanHeader>> GetPlanList(string PlanType, PlanListParameter p, long? PlanID);
        Task<ReturnObject<PlanHeader>> GetPlanHeaderByID(string PlanType, long ID);
        Task<ReturnObject<Plan5YDetail>> GetPlanDetail(string PlanType,int StartYear,long? ID, long? orgID,string planRelease);
        Task<ReturnList<PlanProjectList>> GetPlanProjectList(string PlanType, PlanProjectListParameter p);
        Task<ReturnObject<long?>> ModifyPlanProject(PlanProjectSource p, string userID, long projectID, long planID);
        Task<ReturnMessage> DeletePlan(long planID, string userID);
        Task<ReturnObject<long?>> AddProjectToPlan(long planID, long projID, string projType, bool AddActivies, string userID);
        Task<ReturnMessage> RemoveProjectFromPlan(long planID, long projID, string userID);
        Task<ReturnMessage> PlanAttachFile(MOFContext db, Plan plan, List<AttachFileData> afs);
        #region New Debt

        Task<ReturnObject<NewDebtPlanModel>> GetNewDebtPlan(long? ID, string amountGroup, int? month);
        Task<ReturnMessage> NewDebtGetDataFromP5Y(long PlanID);
        Task<ReturnObject<long?>> ModifyNewDebtPlan(NewDebtPlanModel p, string userID, long projectID, long planID, string amountGroup, int? month);
        Task<ReturnList<NewDebtPlanDetails>> GetNewPlanProjectList(PlanProjectListParameter p);
        Task<ReturnObject<NewDebtPlanSummary>> GetNewPlanSummary(PlanProjectListParameter p);
        Task<ReturnList<NewDebtPlanActList>> GetNewDebtActList(PlanProjectListParameter p);
        Task<ReturnMessage> DuplicateNewDebtPlan(long newPlan, CreatePlanParameter p);

        #endregion

        #region Exsiting Debt
        Task<ReturnObject<SearchExistingDebtPlanModel>> GetExistDebtPlan(PlanProjectListParameter p, eGetPlanType gettype, string amountGroup, int? month);
        Task<ReturnObject<ExistingDebtPlanModel>> GetExistDebtPlan(long? id, string amountGroup, int? month);
        Task<ReturnObject<long?>> ModifyExistDebtPlan(ExistingDebtPlanModel p, string userID,long planID, string amountGroup,int? month);
        Task<ReturnObject<long?>> AddAgreementToPlan(long planID, long agreementID,  string userID);
        Task<ReturnList<ExistPlanAgreementList>> GetPlanAgreementList(PlanProjectListParameter p);
        Task<ReturnMessage> DuplicateExistPlan(long newPlan, CreatePlanParameter p);
        #endregion

        #region Financial Report
        Task<ReturnObject<long?>> ModifyFinRep(FinancialReportModel p, string userID, long planID);
        Task<ReturnObject<FinancialReportModel>> GetFinPlan(long? ID);
        Task<ReturnObject<Debt>> GetFinPlanDebtSummary(PlanProjectListParameter p, eGetPlanType gettype);
        #endregion

        #region proposal
        Task<ReturnObject<long?>> Modify(ProposalModel a, bool isCreate, string userID);
        Task<ReturnObject<ProposalModel>> GetProposal(long id);
        Task<ReturnMessage> AddPlanToProposal(long proposalID, long planID, string userID);
        Task<ReturnMessage> RemovePlanFromProposal(long proposalID, string planType, string userID);
        Task<ReturnMessage> UpdatePlanFlowStatus(ProposalModel p,bool isProposing, string userID);
        Task<ReturnList<ProposalModel>> GetProposalStatusList(ProposalListParameter p);
        Task<ReturnMessage> UpdateProposalStatus(long proposalID,string userID);
        #endregion

        #region Monthly Report
        Task<ReturnObject<long?>> ModifyMonthRep(MonthlyReportModel p, long projID, string userID);
        Task<ReturnObject<long?>> CreateMonthRep(CreatePlanParameter p,string userID);
        Task<ReturnObject<MonthlyReportModel>> GetMonthRep(long ProjID);
        Task<ReturnList<MonthlyReportModel>> ListMonthRep(MonthlyReportParameter p);
        #endregion
    }
}
