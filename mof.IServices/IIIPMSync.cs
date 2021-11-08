using mof.ServiceModels.Common;
using mof.ServiceModels.IIPMModel;
using mof.ServiceModels.Project;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace mof.IServices
{
    public interface IIIPMSync
    {
      
        Task<ReturnMessage> ModifyProject(ProjectModel proj,bool isAdd);
    }
}
