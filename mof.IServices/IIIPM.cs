using mof.ServiceModels.Common;
using mof.ServiceModels.IIPMModel;
using mof.ServiceModels.Project;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace mof.IServices
{
    public interface IIIPM
    {
        Task<ReturnObject<LoginRespone>> Login(LoginRequest p);
        Task<ReturnMessage> IntegrateIIPMData(IntegrateConfig p);
        Task<ReturnMessage> CopyIIPMData(IntegrateConfig p,string UserID);
        /// <summary>
        /// Extended data of project 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        Task<ReturnMessage> ProjectExtend(ProjectExtendRequest p, string UserID);
   
    }
}
