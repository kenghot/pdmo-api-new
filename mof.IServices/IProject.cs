using mof.DataModels.Models;
using mof.ServiceModels.Common;
using mof.ServiceModels.Plan;
using mof.ServiceModels.Project;
using mof.ServiceModels.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace mof.IServices
{
    public interface IProject
    {
        Task<ReturnObject<long?>> Modify(ProjectModel p , bool isCreate, string userID);
        Task<ReturnObject<long?>> ModifyMin(ProjectModelMin p, bool isCreate, string userID);
        Task<ReturnObject<ProjectModel>> Get(long ProjID);
        Task<ReturnObject<ProjectModel>> Get(IQueryable<Project> p);
        Task<ReturnObject<ProjectModel>> Get(IQueryable<Project> p,bool NotCheckRate);
        Task<ReturnList<ProjectModel>> List(ProjectListParameter p);
        Task<ReturnObject<List<ProjectStatus>>> GetProjectStatusList();
        List<AmountData> InitialSourceLoanAmount(List<AmountData> p);


    }
}
