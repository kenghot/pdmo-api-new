using Microsoft.AspNetCore.Mvc;
using mof.ServiceModels.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace mof.IServices
{
    public interface IReport
    {
        Task<ReturnObject<FileContentResult>> ExistingPlanByAgreement(long planID);
        Task<ReturnObject<FileContentResult>> NewDebtPlanRep(long planID);
        Task<ReturnObject<FileContentResult>> FinancialPlanRep(long planID);
    }
}
