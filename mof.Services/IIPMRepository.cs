using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using mof.DataModels.Models;
using mof.IServices;
using mof.ServiceModels.Common;
using mof.ServiceModels.IIPMModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Newtonsoft.Json.Linq;
using mof.ServiceModels.Project;
using mof.ServiceModels.Common.Generic;
using mof.ServiceModels.Plan;
using Microsoft.AspNetCore.Hosting;
using mof.Services.Helper;
using Microsoft.AspNetCore.Http;

namespace mof.Services
{
    public partial class IIPMRepository : IIIPM
    {
        private string IIPMPassword = "trsys@pdmo";
        private string IIPMUserName = "trsys";
        private IConfiguration _config;
        private IStringLocalizer<MessageLocalizer> _msglocalizer;
        private ISystemHelper _helper;
        private IProject _proj;
        private IPlan _plan;
        private MOFContext _db;
        private readonly IHostingEnvironment _host;
        private readonly IHttpContextAccessor _http;
        private bool isFromJson;
        #region endpoint
        private string baseUrl;

        public string BaseUrl
        {
            get
            {
                if (string.IsNullOrEmpty(baseUrl))
                {
                    var cf = _config.GetSection("ApiEndpoint");
                    baseUrl = cf.GetSection("IIPM").Value;
                }

                return baseUrl;
            }
        }
        public string LoginURL
        {
            get
            {
                return BaseUrl + "/api/Auths/Login";
            }
        }
        public string AgencyURL
        {
            get
            {
                return BaseUrl + "/repo/gip/Agencies";
            }
        }
        public string MinistryURL
        {
            get
            {
                return BaseUrl + "/repo/gip/Ministries";
            }
        }
        public string SectorURL
        {
            get
            {
                return BaseUrl + "/repo/gip/Sectors";
            }
        }
        public string ApprovalURL
        {
            get
            {
                return BaseUrl + "/repo/pvy/Configs/Approvals";
            }
        }
        public string ProjPlanURL
        {
            get
            {
                //return BaseUrl + "/repo/pvy/Projects/Active?$expand=currentProjectPlan($expand=planYearBudgets,planConfig),projectInfo($expand=agency,province,sector)";
                return BaseUrl + "/repo/pvy/Projects/Active?$expand=currentProjectPlan($expand=planYearBudgets,planConfig,planApprovals($expand=approvalItems($expand=provisionItem($expand=approvalProvision)))),projectInfo($expand=agency,province,sector)";
            }
        }
        public string ProjObjectivesURL(long id)
        {
           
                return BaseUrl + $"/repo/gip/Projects/{id}/Objectives";
             
        }
        public string ProjScopesURL(long id)
        {

            return BaseUrl + $"/repo/gip/Projects/{id}/Scopes";

        }
        #endregion
        public IIPMRepository(IConfiguration config, IStringLocalizer<MessageLocalizer> msg, MOFContext db,IProject proj, IPlan plan, IHostingEnvironment host,IHttpContextAccessor http)
        {
            _config = config;
            _msglocalizer = msg;
            _db = db;
            _proj = proj;
            _plan = plan;
            _host = host;
            _http = http;
        }

        public async Task<ReturnObject<LoginRespone>> Login(LoginRequest p)
        {
            var ret = new ReturnObject<LoginRespone>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var httpClientHandler = new HttpClientHandler();
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
                {
                    return true;
                };
                using (HttpClient client = new HttpClient(httpClientHandler))
                {
                    string strPayload = JsonConvert.SerializeObject(p);
                    HttpContent c = new StringContent(strPayload, Encoding.UTF8, "application/json");
                    var resp = await client.PostAsync(LoginURL, c);
                    if (!resp.IsSuccessStatusCode)
                    {
                        ret.AddMessage( "api ipm login", "error", eMessageType.Error);
                        ret.Message[0].Message = JsonConvert.SerializeObject(resp);
                        return ret;
                    }
                    var content = await resp.Content.ReadAsStringAsync();
                    ret.Data = JsonConvert.DeserializeObject<LoginRespone>(content);
                }
                ret.IsCompleted = true;

            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }

        public async Task<ReturnMessage> IntegrateIIPMData(IntegrateConfig p)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                isFromJson = p.FromJson;
                var login = await Login(new LoginRequest { Password = IIPMPassword, UserName = IIPMUserName});
                if (!login.IsCompleted)
                {
                    ret.CloneMessage(login.Message);
                    return ret;
                }

                var httpClientHandler = new HttpClientHandler();
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
                {
                    return true;
                };
                using (HttpClient client = new HttpClient(httpClientHandler))
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", login.Data.Token);

                    if (p.Sector)
                    {
                        var sect = await IntegrateSector(client);
                        if (!sect.IsCompleted)
                        {
                            ret.CloneMessage(sect.Message);
                            return ret;
                        }
                        var apv = await IntegrateApproval(client);
                        if (!apv.IsCompleted)
                        {
                            ret.CloneMessage(apv.Message);
                            return ret;
                        }
                    }
                    if (p.Ministry  )
                    {
                        var mis = await IntegrateMinistry(client);
                        if (!mis.IsCompleted)
                        {
                            ret.CloneMessage(mis.Message);
                            return ret;
                        }
                    }

                    #region agency
                    if (p.Agency)
                    {
                        var ag = await IntegrateAgency(client);
                        if (!ag.IsCompleted)
                        {
                            ret.CloneMessage(ag.Message);
                            return ret;
                        }
                    }


                    #endregion
                    if (p.ProjectPlan)
                    {
                        var pjp = await IntegrateProjectPlan(client);
                        if (!pjp.IsCompleted)
                        {
                            ret.CloneMessage(pjp.Message);
                            return ret;
                        }
                    }

                }
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        public async Task<ReturnMessage> IntegrateSector(HttpClient client)
        {
            var ret = new ReturnMessage(_msglocalizer);
    
            ret.IsCompleted = false;
            try
            {


                string resp;
                if (isFromJson)
                {
                    resp = System.IO.File.ReadAllText(_host.WebRootPath + "\\iipm\\sectors.json");
                }else
                {
                    resp = await client.GetStringAsync(SectorURL);
                }
                 
                JObject json = JObject.Parse(resp);
                var sects = json.GetValue("items").ToObject<List<Sector>>();
                foreach (var sect in sects)
                {
                    var old = await _db.IipmSector.Where(w => w.SectId == sect.id).FirstOrDefaultAsync();
                    if (old == null)
                    {
                        old = new IipmSector { SectId = sect.id };
                        _db.IipmSector.Add(old);
                    }
                    old.IsActive = sect.isActive;
                    old.Level = sect.level;
                    old.Name = sect.name;
                    old.ParentId = sect.parentId;
                    

                }

                await _db.SaveChangesAsync();
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        public async Task<ReturnMessage> IntegrateAgency(HttpClient client)
        {
            var ret = new ReturnMessage(_msglocalizer);
            IipmAgency ag;
            ret.IsCompleted = false;
            try
            {

                string resp;
                if (isFromJson)
                {
                    resp = System.IO.File.ReadAllText(_host.WebRootPath + "\\iipm\\agencies.json");
                }
                else
                {
                    resp = await client.GetStringAsync(AgencyURL);
                }
                    
                JObject json = JObject.Parse(resp);
                var agencies =  json.GetValue("items").ToObject<List<Agency>>() ;
                foreach (var agency in agencies)
                {
                    var old = await _db.IipmAgency.Where(w => w.Id == agency.id).FirstOrDefaultAsync();
                    if (old == null)
                    {
                        old = new IipmAgency { Id = agency.id };
                        _db.IipmAgency.Add(old);
                    }
                    old.EndDate = SqlDate(agency.endDate);
            
                    old.Code = agency.code;
                    
                    old.IsActive = agency.isActive;
                    old.MinistryCode = agency.ministryCode;
                    old.MinistryId = agency.ministryId;
                    old.Name = agency.name;
                    old.SId = agency.sId;
                    old.StartDate = agency.startDate;
                    old.OrganizationId = await MapOrg(agency.sId);

                }

                await _db.SaveChangesAsync();
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        public async Task<long?> MapOrg(string sId)
        {
            long? ret = null;
            if (!string.IsNullOrEmpty(sId)) 
            {
                var o = await _db.Organization.Where(w => w.OrganizationCode == sId).FirstOrDefaultAsync();
                if (o != null)
                {
                    ret = o.OrganizationId;
                }
            }
            return ret;
        }
        public async Task<ReturnMessage> IntegrateApproval(HttpClient client)
        {
            var ret = new ReturnMessage(_msglocalizer);
            IipmAgency ag;
            ret.IsCompleted = false;
            try
            {
                string resp;
                if (isFromJson)
                {
                    resp = System.IO.File.ReadAllText(_host.WebRootPath + "\\iipm\\approvals.json");
                }
                else
                {
                    resp = await client.GetStringAsync(ApprovalURL);
                }

             
                JObject json = JObject.Parse(resp);
                var apvs = json.GetValue("items").ToObject<List<Approval>>();
                foreach (var apv in apvs)
                {
                    var old = await _db.IipmApproval.Where(w => w.ApprovalId == apv.id).FirstOrDefaultAsync();
                    if (old == null)
                    {
                        old = new IipmApproval { ApprovalId = apv.id };
                        _db.IipmApproval.Add(old);
                    }
                    old.Detail = apv.detail;
                    old.IsActive = apv.isActive;
 
                }

                await _db.SaveChangesAsync();
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        public async Task<ReturnMessage> IntegrateMinistry(HttpClient client)
        {
            var ret = new ReturnMessage(_msglocalizer);
            IipmAgency ag;
            ret.IsCompleted = false;
            try
            {
                string resp;
                if (isFromJson)
                {
                    resp = System.IO.File.ReadAllText(_host.WebRootPath + "\\iipm\\ministries.json");
                }
                else
                {
                    resp = await client.GetStringAsync(AgencyURL);
                }

  
                JObject json = JObject.Parse(resp);
                var ministies = json.GetValue("items").ToObject<List<Agency>>();
                foreach (var minis  in ministies)
                {
                    var old = await _db.IipmMinistry.Where(w => w.Id == minis.id).FirstOrDefaultAsync();
                    if (old == null)
                    {
                        old = new IipmMinistry { Id = minis.id };
                        _db.IipmMinistry.Add(old);
                    }
                    old.EndDate = SqlDate(minis.endDate);

                    old.Code = minis.code;

                    old.IsActive = minis.isActive;
     
                    old.Name = minis.name;
                    old.SId = minis.sId;
                    old.StartDate = SqlDate(minis.startDate);
                    old.OrganizationId = await MapOrg(minis.sId);
                   
                }

                await _db.SaveChangesAsync();
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        public async Task<ReturnMessage> IntegrateProjectPlan(HttpClient client)
        {
            var ret = new ReturnMessage(_msglocalizer);
             
            ret.IsCompleted = false;
            try
            {


                string resp;
                if (isFromJson)
                {
                    resp = System.IO.File.ReadAllText(_host.WebRootPath + "\\iipm\\plan.json");
                }
                else
                {
                    resp = await client.GetStringAsync(ProjPlanURL);
                }
                 
                JObject json = JObject.Parse(resp);
                var pjp = json.ToObject<ProjectPlanModel>();

                foreach (var pj in pjp.items)
                {
                
                    var pjInfo = await _db.IipmProject.Where(w => w.ProjId == pj.id).FirstOrDefaultAsync();
                    if (pjInfo == null)
                    {
                        pjInfo = new IipmProject { ProjId = pj.projectInfo.id };
                        _db.IipmProject.Add(pjInfo);
                    }
                    var p = pj.projectInfo;
                    
                    pjInfo.AgencyId = p.agencyId;
                    pjInfo.ApprovedAt = p.approvedAt;
                    pjInfo.Background = p.background;
                    pjInfo.Budget = p.budget;
                    pjInfo.Code = p.code;
                    pjInfo.CreatedAt = SqlDate(p.createdAt);
                    pjInfo.CreatedBy = p.createdBy;
                    pjInfo.CreditChannelId = p.creditChannelId;
                    pjInfo.DirectorMail = p.directorMail;
                    pjInfo.DirectorTel = p.directorTel;
                    pjInfo.EndedAt = SqlDate(p.endedAt);
                    pjInfo.FlagTypeId = p.flagTypeId;
                    pjInfo.Goal = p.goal;
                    pjInfo.HasEld = p.hasEld;
                    pjInfo.HasPvy = p.hasPvy;
                    pjInfo.IdRef = p.idRef;
                    pjInfo.ImportContent = p.importContent;
                    pjInfo.IsGovBurden = p.isGovBurden;
                    pjInfo.IsOnGoing = p.isOnGoing;
                    pjInfo.IsPlanLocked = p.isPlanLocked;
                    pjInfo.KindTypeId = p.kindTypeId;
                    pjInfo.Name = p.name;
                    pjInfo.OperationAt = SqlDate(p.operationAt);
                    pjInfo.OperationTypeCode = p.operationTypeCode;
                    pjInfo.ProjectArea = p.projectArea;
                    pjInfo.ProjectLocked = p.projectLocked;
                    pjInfo.ProjectLogFrameLocked = p.projectLogFrameLocked;
                    pjInfo.ProjectScope = p.projectScope;
                    pjInfo.ProvinceCode = p.provinceCode;
                    pjInfo.SectorId = p.sectorId;
                    pjInfo.StartedAt = SqlDate(p.startedAt);
                    pjInfo.UpdatedAt = SqlDate(p.updatedAt);
                    pjInfo.UpdatedBy = p.updatedBy;
                    await _db.SaveChangesAsync();
                    if (pj.currentProjectPlan != null)
                    {
                        var delpjp = await _db.IipmProjectPlan.Where(w => w.ProjId == pj.id).ToListAsync();
                        _db.IipmProjectPlan.RemoveRange(delpjp);
                        await _db.SaveChangesAsync();
                        var cpjp = pj.currentProjectPlan;
                        var pl = await _db.IipmProjectPlan.Where(w => w.ProjPlanId == cpjp.id).FirstOrDefaultAsync();
                        if (pl == null)
                        {
                            pl = new IipmProjectPlan { ProjPlanId = cpjp.id };
                            _db.IipmProjectPlan.Add(pl);
                        }

                        pl.CoordinatorMail = cpjp.coordinatorMail;
                        pl.CoordinatorName = cpjp.coordinatorName;
                        pl.CoordinatorPosition = cpjp.coordinatorPosition;
                        pl.CoordinatorTel = cpjp.coordinatorTel;
                        pl.CreatedAt = SqlDate(cpjp.createdAt);
                        pl.CreatedBy = cpjp.createdBy;
                        pl.PlanConfigId = cpjp.planConfigId;
                        pl.ProjId = cpjp.projectId;
                        pl.UpdatedAt = SqlDate(cpjp.updatedAt);
                        pl.UpdatedBy = pl.UpdatedBy;
                        pl.Year = cpjp.planConfig.year;
                        pl.IsActive = cpjp.planConfig.isActive;
                        pl.IsEnable = cpjp.planConfig.isEnable;
                        await _db.SaveChangesAsync();
                        if (cpjp.planYearBudgets != null)
                        {
                            List<IipmPlanYearBudget> ys = new List<IipmPlanYearBudget>();
                            var delpy = await _db.IipmPlanYearBudget.Where(w => w.ProjectPlanId == pl.ProjPlanId).ToListAsync();
                            _db.IipmPlanYearBudget.RemoveRange(delpy);
                            await _db.SaveChangesAsync();
                            foreach (var py in cpjp.planYearBudgets)
                            {
                                var ye = ys.Where(w => w.Pybid == py.id).FirstOrDefault();
                                if (ye != null)
                                {
                                    continue;
                                }
                                    var y = await _db.IipmPlanYearBudget.Where(w => w.Pybid == py.id).FirstOrDefaultAsync();
                                if (y == null)
                                {
                                    y = new IipmPlanYearBudget { Pybid = py.id };
                                    //_db.IipmPlanYearBudget.Add(y);
                                }
                                y.Budget = py.budget;
                                y.ProjectPlanId = py.projectPlanId;
                                y.SourceOfffundId = py.sourceOfFundId;
                                y.Year = py.year;
                             
                                ys.Add(y);
                                
                            }
                            _db.IipmPlanYearBudget.AddRange(ys);
                            await _db.SaveChangesAsync();
                        }
                    }

                    
                }
                
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
   

        public DateTime? SqlDate(DateTime? d)
        {
            if (d?.Year > 9999)
            {
                return new DateTime(9999, d.Value.Month, d.Value.Day, d.Value.Hour, d.Value.Minute, d.Value.Second);
            }
            if (d?.Year < 1753)
            {
                return new DateTime(1753, d.Value.Month, d.Value.Day, d.Value.Hour, d.Value.Minute, d.Value.Second);
            }
            return d;
        }
        private string GetSourceOfFund(long id)
        {
            string ret = "L";
            switch(id)
            {
                case  2:
                    ret = "F";
                    break;
                case 5:
                    ret = "O";
                    break;

            }
            return ret;
        }
        public async Task<ReturnMessage> CopyIIPMData(IntegrateConfig p, string UserID)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                bool isAdd = false;
                
                
                //var pjIIPMs = await _db.IipmProject.ToListAsync();
                var pjplIIPMs = await _db.IipmProjectPlan.ToListAsync();
                var allrec = pjplIIPMs.Count();
                var irec = 0;
                var log = new DataLog
                {
                    LogDt = DateTime.Now,
                    TableName = "IIPM_CopyData",
                    TableKey = 1,
                    Remark = $"0 / {allrec}",
                    LogType = "C",
                    UserId = UserID
                };
                _db.DataLog.Add(log);
                await _db.SaveChangesAsync();
                foreach (var pjpl in pjplIIPMs)
                {
                    if (p.IIPMProjectIDs != null && !p.IIPMProjectIDs.Contains(pjpl.ProjId.Value))
                    {
                        continue;
                    }
                    #region project

                    ProjectModel pjMD = new ProjectModel { };
                    var pjIIPM = await _db.IipmProject.Where(w => w.ProjId == pjpl.ProjId).FirstOrDefaultAsync();
                    //if (pjIIPM == null || !pjIIPM.StartedAt.HasValue || !pjIIPM.EndedAt.HasValue)
                    //{
                    //    continue;
                    //}
                    var agentIIPM = await _db.IipmAgency.Where(w => w.Id == pjIIPM.AgencyId.Value).FirstOrDefaultAsync();
                    if (agentIIPM == null || !agentIIPM.OrganizationId.HasValue)
                    {
                        continue;
                    }
                    var pjBGs = await _db.IipmPlanYearBudget.Where(w => w.ProjectPlanId == pjpl.ProjPlanId).ToListAsync();

                    var pjDB = await _db.Project.Where(w => w.ProjectCode == "IIPM-" + pjIIPM.ProjId.ToString()).FirstOrDefaultAsync();
                    if (pjDB == null)
                    {
                        isAdd = true;
                        //pjMD.Activities = new List<ActivityData> {
                        //    new ActivityData
                        //    {
                        //        ActivityName = pjIIPM.Name,
                        //        ResolutionAmounts = new List<AmountData>(),
                        //        ContractAmounts =new List<AmountData>()
                        //    }
                        //};  
                        pjMD.Activities = new List<ActivityData>();
                    }
                    else
                    {
                        isAdd = false;
                       // var pj = await _proj.Get(pjDB.ProjectId);
                        var pjtmp = _db.Project.Include(pt => pt.ProjectTypeNavigation).Include(o => o.Organization)
                        .Where(w => w.ProjectId == pjDB.ProjectId);
                       //if (pjDB.ProjectId == 2464)
                       // {
                       //     var x = 0;
                       // }
                            var pj = await _proj.Get( pjtmp,true);
                            if (!pj.IsCompleted)
                            {
                                ret.CloneMessage(pj.Message);
                                ret.AddMessage("get proj", "", eMessageType.Error);
                                return ret;
                            }
                            pjMD = pj.Data;
                       
                       
                     
                    }
                    long pdmoPlID;
                    var pl5y = await _db.Plan
                        .Where(w => w.OrganizationId == agentIIPM.OrganizationId.Value && w.StartYear == pjpl.Year.Value &&
                        w.PlanTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Plan_Type.แผน_5_ปี).OrderByDescending(o => o.PlanReleaseNavigation.OrderNo).FirstOrDefaultAsync();
                    if (pl5y != null)
                    {
                        pdmoPlID = pl5y.PlanId;
                    }else
                    {
                        var newpl = await _plan.CreatePlan(UserID, ServiceModels.Constants.LOVGroup.Plan_Type.แผน_5_ปี, new ServiceModels.Plan.CreatePlanParameter
                        {
                            OrganizationID = agentIIPM.OrganizationId.Value,
                            StartYear = pjpl.Year.Value,
                            PlanRelease = ServiceModels.Constants.LOVGroup.Plan_Release.จัดทำแผนฯ_ตั้งต้น
                        });
                        if (!newpl.IsCompleted)
                        {
                            ret.CloneMessage(newpl.Message);
                            ret.AddMessage("clean plan", "", eMessageType.Error);
                            return ret;
                        }else
                        {
                            pdmoPlID = newpl.Data.Value;
                            pl5y = await _db.Plan.Where(w => w.PlanId == pdmoPlID).FirstOrDefaultAsync();
                        }
                    }
                    pjMD.OrganizationID = agentIIPM.OrganizationId.Value;
                    pjMD.ProjectCode = "IIPM-" + pjIIPM.ProjId.ToString();

                    var defalutDate = new DateTime(2000, 1, 1);
                    pjMD.LimitAmount = 0;
                    pjMD.ProjectTHName = pjIIPM.Name;

                    pjMD.ProjectBackground = pjIIPM.Background;
                    pjMD.ProjectTarget = pjIIPM.Goal;
                    if (isAdd)
                    {
                        pjMD.ProjectObjective =  "-";
                        pjMD.StartDate = pjIIPM.StartedAt.HasValue ? pjIIPM.StartedAt.Value : defalutDate;
                        pjMD.StartYear = DateTime.Now.Year + 543;
                        pjMD.EndDate = pjIIPM.EndedAt.HasValue ? pjIIPM.EndedAt.Value : defalutDate;
                        pjMD.ProjectType = new BasicData
                        {
                            Code = ServiceModels.Constants.LOVGroup.Project_Type.กู้เพื่อลงทุนในโครงการพัฒนา,

                        };
                    }
                    pjMD.ProjectScope = pjIIPM.ProjectScope;

                    pjMD.Materials = new List<MaterialSource>();
                    pjMD.FIRR = 0;
                    pjMD.EIRR = 0;
                    pjMD.IsNewProject = false;
                    pjMD.ProjectStatuses = new List<ProjectStatus>();
                    var sector = await _db.IipmSector.Where(w => w.SectId == pjIIPM.SectorId.Value).FirstOrDefaultAsync();
                    if (sector != null)
                    {
                        sector = await _db.IipmSector.Where(w => w.SectId == sector.ParentId.Value).FirstOrDefaultAsync();
                        if (sector != null)
                        {
                            pjMD.ProjectBranch = sector.Name;
                        }
                    }
                    
                    var proj = await _proj.Modify(pjMD, isAdd, UserID);
                    if (proj.IsCompleted)
                    {

                        var pjEdit = await _db.Project.Where(w => w.ProjectId == proj.Data).FirstOrDefaultAsync();
                        if (pjEdit != null)
                        {
                            pjIIPM.PdmoprojId = proj.Data;
                            pjEdit.StartYear = pjEdit.StartDate.Year + 543 ;
                            await _db.SaveChangesAsync();
                        }
                        if (isAdd)
                        {
                            pjMD.ProjectID = proj.Data;
                            pjIIPM.PdmoprojId = proj.Data;
                            await _db.SaveChangesAsync();
                        }
                    }else
                    {
                        ret.CloneMessage(proj.Message);
                        ret.AddMessage("modify proj", "", eMessageType.Error);
                        return ret;
                    }
                    #endregion
                    //add project to plan
                    long pdmoPlpjID;
                    var pdmoPjpl = await _db.PlanProject.Where(w => w.PlanId == pdmoPlID && w.ProjectId == pjMD.ProjectID).FirstOrDefaultAsync();
                    if (pdmoPjpl == null)
                    {
                        var addpj = await _plan.AddProjectToPlan(pdmoPlID, pjMD.ProjectID.Value, ServiceModels.Constants.LOVGroup.Project_Type.กู้เพื่อโครงการอื่นๆ, true, UserID);
                        if (!addpj.IsCompleted)
                        {
                            ret.CloneMessage(addpj.Message);
                            ret.Message[0].Message += " (Add project to plan)";
                            ret.AddMessage("Add project to plan", "", eMessageType.Error);
                            return ret;

                        }else
                        {
                            pdmoPlpjID = addpj.Data.Value;
                        }
                        
                    }else
                    {
                        pdmoPlpjID = pdmoPjpl.PlanProjectId;
                    }
                    var budgets = await _db.IipmPlanYearBudget.Where(w => w.ProjectPlanId == pjpl.ProjPlanId).ToListAsync();
                    PlanProjectSource psource = new PlanProjectSource();
                    psource.PlanProjectID = pdmoPlpjID;
                    psource.LoanPeriods = new List<LoanPeriod>();
                    foreach (var bg in budgets.Where(w => w.SourceOfffundId == 1 || w.SourceOfffundId == 2))
                    {
                        if (bg.Budget.HasValue && bg.Year.HasValue)
                        {
                            if (bg.Year.Value <= pl5y.StartYear + 4 && bg.Year.Value >= pl5y.StartYear)
                            {
                                psource.LoanPeriods.Add(new LoanPeriod
                                {
                                    PeriodType = "Y",
                                    PeriodValue = bg.Year.Value,
                                    LoanSources = new List<LoanSource>
                                {
                                    new LoanSource
                                    {
                                        Currency = "THB",
                                        LoanAmount = bg.Budget.Value,
                                        SourceType = bg.SourceOfffundId.Value == 1 ? "L" : "F" // GetSourceOfFund(bg.SourceOfffundId.Value)
                                    }
                                }
                                });
                            }
                    
                        }
                       
                    }

                    var loan = await _plan.ModifyPlanProject(psource, UserID, pjMD.ProjectID.Value, pdmoPlID);
                    if (!loan.IsCompleted) {
                        ret.CloneMessage(loan.Message);
                        ret.Message[0].Message += " (budget data)";
                        
                        return ret;
                    }
                    irec++;
                    log.Remark = $"{irec} / {allrec}";
                    await _db.SaveChangesAsync();
                }

                ret.IsCompleted = true;

            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        private async Task<string> GetStringFromAPI(HttpClient client,string url,string jsonFileName)
        {
            string resp = "";
            if (!string.IsNullOrEmpty(jsonFileName))
            {
                resp = System.IO.File.ReadAllText(_host.WebRootPath + $"\\iipm\\{jsonFileName}");
            }
            else
            {
                try
                {
                    resp = await client.GetStringAsync(url);
                }catch(Exception ex)
                {
                    resp = "";
                }
                
            }
            return resp;
        }
        public async Task<ReturnMessage> ProjectExtend(ProjectExtendRequest p, string UserID)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                isFromJson = p.FromJson;
              
       
                var log = new DataLog
                {
                    LogDt = DateTime.Now,
                    TableName = "IIPM_ProjectExtend",
                    TableKey = 1,
                    Remark ="",
                    LogType = "C",
                    UserId = UserID
                };
                _db.DataLog.Add(log);
                await _db.SaveChangesAsync();

                var httpClientHandler = new HttpClientHandler();
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
                {
                    return true;
                };
                using (HttpClient client = new HttpClient(httpClientHandler))
                {
                    if (!isFromJson)
                    {
                        var login = await Login(new LoginRequest { Password = IIPMPassword, UserName = IIPMUserName });
                        if (!login.IsCompleted)
                        {
                            ret.CloneMessage(login.Message);
                            return ret;
                        }
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", login.Data.Token);
                    }

                    
                    var pjs = await _db.IipmProject.WhereIf(p.ProjectIDs != null && p.ProjectIDs.Count > 0, w => p.ProjectIDs.Contains(w.ProjId)).ToListAsync();
                    foreach (var pj in pjs)
                    {
                        if (p.Objective)
                        {
                            try
                            {
                                string resp = await GetStringFromAPI(client,ProjObjectivesURL(pj.ProjId), isFromJson ? "objectives.json" : "");
 
                                if (!string.IsNullOrEmpty(resp))
                                {
                                    var JO = JObject.Parse(resp);
                                    JArray JA = (JArray)JO["items"];
                                    var objectives = "";
                                    foreach (var j in JA)
                                    {
                                        if (j["objective"] != null)
                                        {
                                            if (!string.IsNullOrEmpty(j["objective"].ToString()))
                                            {
                                                objectives += j["objective"].ToString() + "\r\n";
                                            }
                                        }

                                    }
                                    if (!string.IsNullOrEmpty(objectives))
                                    {
                                        var pjtmp = await _db.Project.Where(w => w.ProjectId == pj.PdmoprojId).FirstOrDefaultAsync();
                                        if (pjtmp != null)
                                        {
                                            pjtmp.ProjectObjective = objectives;
                                            await _db.SaveChangesAsync();
                                        }
                                    }
                                }
                            }catch (Exception ex)
                            {
                                ret.AddError(ex);
                                ret.AddMessage("error update project objective", JsonConvert.SerializeObject(pj), eMessageType.Error);
                                return ret;
                            }
  
                        }
                        if (p.Scope)
                        {
                            try
                            {
                                string resp = await GetStringFromAPI(client, ProjScopesURL(pj.ProjId), isFromJson ? "scopes.json" : "");

                                if (!string.IsNullOrEmpty(resp))
                                {
                                    var JO = JObject.Parse(resp);
                                    JArray JA = (JArray)JO["items"];
                                    var scopes = "";
                                    foreach (var j in JA)
                                    {
                                        if (j["scope"] != null)
                                        {
                                            if (!string.IsNullOrEmpty(j["scope"].ToString()))
                                            {
                                                scopes += j["scope"].ToString() + "\r\n";
                                            }
                                        }

                                    }
                                    if (!string.IsNullOrEmpty(scopes))
                                    {
                                        var pjtmp = await _db.Project.Where(w => w.ProjectId == pj.PdmoprojId).FirstOrDefaultAsync();
                                        if (pjtmp != null)
                                        {
                                            pjtmp.ProjectScope = scopes;
                                            await _db.SaveChangesAsync();
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                ret.AddError(ex);
                                ret.AddMessage("error update project scope", JsonConvert.SerializeObject(pj), eMessageType.Error);
                                return ret;
                            }

                        }
                    }
                }
                log.Remark = $"finish at {DateTime.Now}";
                await _db.SaveChangesAsync();

                
                ret.IsCompleted = true;

            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
    }
}
