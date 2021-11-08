
using mof.ServiceModels.Common;
using mof.ServiceModels.Proposal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace mof.IServices
{
    public interface IProposal
    {
        Task<ReturnObject<long?>> Modify(ProposalModel a,bool isCreate,string userID);
        Task<ReturnObject<ProposalModel>> Get(long id);
        Task<ReturnMessage> AddPlanToProposal(long proposalID, long planID, string userID);
        Task<ReturnMessage> RemovePlanFromProposal(long proposalID, string planType, string userID);

    }
}
