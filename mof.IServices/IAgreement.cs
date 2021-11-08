using mof.ServiceModels.Agreement;
using mof.ServiceModels.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace mof.IServices
{
    public interface IAgreement
    {
        Task<ReturnObject<long?>> Modify(AgreementModel a,bool isCreate,string userID);
        Task<ReturnObject<AgreementModel>> Get(long id, int? year = null, long? transType = null);
        Task<ReturnList<AgreementModel>> List(AgreementListParameter p);
        Task<ReturnObject<long?>> MapActivityToAgreement(long agreementID, long planActID);
        Task<ReturnObject<long?>> RemoveActivityFromAgreement(long agreementID, long planActID );
        Task<ReturnObject<long?>> MapPaymentPlanToAgreement(long agreementID, long paymentPlanID);
        Task<ReturnObject<long?>> RemovePaymentPlanFromAgreement(long agreementID, long paymentPlanID );
        Task<ReturnObject<AgreementMappingList>> GetAgreementMappingList(long agreementID);
        Task<ReturnList<AgreementTransModel>> GetAgreementTrans(AgreementListParameter p);

    }
}
