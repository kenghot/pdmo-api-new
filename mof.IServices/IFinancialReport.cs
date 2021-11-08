using mof.ServiceModels.Common;
using mof.ServiceModels.FinancialReport;
using mof.ServiceModels.Plan;
using mof.ServiceModels.Project;
using mof.ServiceModels.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace mof.IServices
{
    public interface IFinancialReport
    {
        Task<ReturnObject<long?>> ModifyFinRep(FinancialReportModel p, string userID, long planID);
        Task<ReturnObject<FinancialReportModel>> GetFinPlan(long? ID);
    }
}
