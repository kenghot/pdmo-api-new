using mof.DataModels.Models;
using mof.ServiceModels.Agreement;
using mof.ServiceModels.Common;
using mof.ServiceModels.Plan;
using mof.ServiceModels.PublicDebt;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace mof.IServices
{
    public interface ICommon
    {
        Task<ReturnList<NewDebtPlanActList>> GetNewDebtActList(List<PlanAct> p);
        Task<ReturnList<ExistPlanAgreementList>> GetPlanAgreementList(List<PlanExist> p);

        Task<bool> IsProjectShortForm(string projType);
        Task<String> GetMasterAgreemet(long ppjId);
     

        #region Report
        Task<ReturnObject<PublicDebtDashBoard>> PublicDebtSummaryDashboard();
        #endregion
    }
}
