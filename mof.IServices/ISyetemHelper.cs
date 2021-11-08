using mof.DataModels.Models;
using mof.ServiceModels.Common;
using mof.ServiceModels.Common.Generic;
using mof.ServiceModels.Helper;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace mof.IServices
{
    public interface ISystemHelper
    {
        ReturnObject<List<BasicData>> GetLOVGroup(string LOVGroupCode);
        ReturnObject<List<LOV>> GetLOVByGroup(string LOVGroupCode);
        ReturnObject<LOV> GetLOVByCode(string LOVCode,string LOVGroupCode);
        ReturnObject<List<LOV>> GetLOVByParent(string ParentGroupCode,string ParentLovCode);

        ReturnMessage UserValidate(string UserID);
        ReturnObject<LOV> LOVCodeValidate(string LOVCode, string LOVGroupCode,long? ParentKey);

        ReturnObject<CurrencyData> GetCurrencyRate(int year);


        Task<ReturnObject<long?>> UploadFile(MOFContext db,AttachFileData af, bool isDbSave);

        Task<ReturnObject<LogData>> GetDataLog(long id);

        Task<ReturnObject<List<BasicData>>> GetProjectType(long orgID);
        Task<ReturnObject<List<BasicData>>> GetLoanType(string planType, long orgID);
        #region Configuration
        Task<ReturnObject<CurrencyData>> GetCurrencyRateScreen(int year);
        Task<ReturnMessage> UpdateCurrencyRateScreen(CurrencyData currency);
        Task<ReturnObject<List<CurrencyScreen>>> ListCurrency();
        Task<ReturnMessage> ModifyCurrency(CurrencyScreen d, bool isCreate, string userID);
        Task<ReturnMessage> DeleteCurrency(string code, string userID);
        Task<ReturnObject<List<ParameterData>>> ListParameter();
        Task<ReturnMessage> ModifyParameter(ParameterData d, bool isCreate, string userID);
        Task<ReturnList<LOVExtend>> GetLovExtendListByLOVGroup(string lovGroupCode);
        Task<ReturnMessage> ModifyLovExtend(ParameterData d, bool isCreate, string userID);
        Task<ReturnList<PDMORegulationData>> GetPDMORegulationList();
        Task<ReturnMessage> ModifyPDMORegulation(PDMORegulationData p, bool isCreate, string userID);
        #endregion

        #region utility
        Task<ReturnMessage> SendEmail(MailMessage mail);
        #endregion
        Task<ReturnObject<HttpResponseMessage>> RequestHttp(HttpClient client, HttpRequestMessage request, string action,long? sessionId);
        Task<ReturnObject<long?>> GetSurrogateKey(string groupCode, string prefix);
        Task<ReturnObject<long?>> GetSurrogateKeyAsnyc(MOFContext db, string groupCode, string prefix);
    }
}
