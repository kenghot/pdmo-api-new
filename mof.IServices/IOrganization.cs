using System;
using System.Collections.Generic;
using System.Text;
using mof.DataModels.Models;
using mof.ServiceModels.Common;
using mof.ServiceModels;

using System.Threading.Tasks;
using mof.ServiceModels.Organization;
using mof.ServiceModels.Common.Generic;
using mof.ServiceModels.Request;

namespace mof.IServices
{
    public interface IOrganization
    {

        Task<ReturnObject<ServiceModels.Response.LoanSummaryDashboard>> GetLoanSummaryDashboard(ServiceModels.Request.Common.GetByID id);
        Task<ReturnObject<List<ServiceModels.Response.TechicalAssistance>>> GetTechicalAssistances();

        Task<ReturnObject<long?>> Modify(ORGModel org, bool isCreate,string UserID, eORGModifyType mod);
        Task<ReturnObject<ORGModel>> Get(long id, bool IsChangeRequest, bool isGetAll);

        Task<ReturnList<BasicData>> GetAffiliates();

        Task<ReturnObject<long?>> ApproveChange(long id, string UserID);
        Task<ReturnObject<long?>> CancelChange(long id, string UserID);
        Task<ReturnObject<string>> GetPDMOLawReguration(long id);

        Task<ReturnObject<CalORGStatusResponse>> CalculateORGStatus(CalORGStatusRequest org);
        
        #region s120
        Task<ReturnList<ORGModel>> GetChangeRequest(ChangeRequestsParameter p);
        Task<ReturnList<OrganizationList>> GetOrganizationList(OrganizationListParameter p);
        Task<ReturnObject<List<ORGCountByType>>> GetSummaryORG();
        #endregion
    }
}
