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
using mof.ServiceModels.IIPMSyncModel;

namespace mof.Services
{
    public class IIPMSyncRepository : IIIPMSync
    {
 
        private IConfiguration _config;
        private IStringLocalizer<MessageLocalizer> _msglocalizer;
        private ISystemHelper _helper;
        private MOFContext _db;
        private readonly IHostingEnvironment _host;
        private readonly IHttpContextAccessor _http;
 
        public IIPMSyncRepository(IConfiguration config, IStringLocalizer<MessageLocalizer> msg, MOFContext db, IHostingEnvironment host, IHttpContextAccessor http, ISystemHelper helper)
        {
            _config = config;
            _msglocalizer = msg;
            _db = db;
            _helper = helper;
            _host = host;
            _http = http;
        }
        private string _baseUrl;
        public string baseUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_baseUrl))
                {
                    var cf = _config.GetSection("ApiEndpoint");
                    _baseUrl = cf.GetSection("IIPM").Value;
                }

                return _baseUrl;
            }
        }
        public  HttpClient CreateClient()
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
            {
                return true;
            };
            HttpClient client = new HttpClient(httpClientHandler);
            var token = _http.HttpContext.Request.Headers["Authorization"].ToString();
            token = token.Replace("Bearer ", "");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return client;
        }
        public async Task<ReturnMessage> ModifyProject(ProjectModel proj, bool isAdd)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var options = new DbContextOptionsBuilder<MOFContext>();
                options.UseSqlServer(_config.GetConnectionString("DefaultConnection"));
                var db = new MOFContext(options.Options);

                var sess = new ApiSession { SessionDt = DateTime.Now };
 
                db.ApiSession.Add(sess);
                await db.SaveChangesAsync();
                using (var client = CreateClient())
                {
                    HttpRequestMessage req = new HttpRequestMessage(isAdd ? HttpMethod.Post : HttpMethod.Put, $"{baseUrl}/repo/gip/Projects" + (isAdd ? "" : $"/{proj.ProjectCode}") );
                    
                    var pj = new ServiceModels.IIPMSyncModel.IIPMProject
                    {
                        agencyId = proj.Organization.Code,
                        background = proj.ProjectBackground,
                        budget = proj.ResolutionExpSum.GrandTotal,
                        code = proj.ProjectCode,
                        endedAt = proj.EndDate,
                        flagTypeId = "GIP",
                        goal = proj.ProjectTarget,
                        kindTypeId = proj.ProjectType?.Code,
                        operationTypeCode = proj.Status?.Code,
                        operationAt = DateTime.Now.Date,
                        name = proj.ProjectTHName,
                        provinceCode = proj.Province.Code,
                        startedAt = proj.StartDate,
                        directorMail = proj.DirectorMail,
                        directorName = proj.DirectorName,
                        directorPosition = proj.DirectorPosition,
                        directorTel = proj.DirectorTel,
                        
                    };
                    long sector;
                    if (long.TryParse(proj.Sector.Code,out sector) && (sector != 0)) {
                        pj.sectorId = sector;
                    }
                    var imp = proj.Materials.Where(w => w.SourceType == "F").FirstOrDefault();
                    if (imp != null)
                    {
                        pj.importContent = Math.Round( imp.Ratio,2);
                    }
                    if (proj.CreditChannel != null)
                    {
                        pj.creditChannelId = long.Parse(proj.CreditChannel.Code);
                    }
                    if (proj.IsGovBurden.HasValue)
                    {
                        pj.isGovBurden = proj.IsGovBurden.Value;
                    }
                    if (proj.IsOnGoing.HasValue)
                    {
                        pj.isOnGoing = proj.IsOnGoing.Value;
                    }
                    if (proj.HasEld.HasValue)
                    {
                        pj.hasEld = proj.HasEld.Value;
                    }

                    req.Content = new StringContent( JsonConvert.SerializeObject(pj), Encoding.UTF8, "application/json");
                    #region map
                    var maps = new List<ProjectExtendModel>();
                    
                    #endregion
                    var res = await _helper.RequestHttp(client,req, "iipm-modify-project",sess.Id);
                    if (res.Data.IsSuccessStatusCode)
                    {
                        await ModifyProjectExtend(client, "Scopes", "scope", proj.ProjectCode,"gip", proj.Scopes, sess.Id,false);
                        await ModifyProjectExtend(client, "Benefitses", "benefits", proj.ProjectCode, "gip", proj.Benefits, sess.Id,false);
                        await ModifyProjectExtend(client, "Objectives", "objective", proj.ProjectCode, "gip", proj.Objectives, sess.Id,false);
                        await ModifyProjectExtend(client, "Outputs", "output", proj.ProjectCode, "gip", proj.Productivities, sess.Id,false);
                        await ModifyProjectExtend(client, "Wkts", "wkt", proj.ProjectCode, "gis", maps, sess.Id, true);
                        await ModifyProjectLocation(client, proj.ProjectCode, proj.Locations, sess.Id);
                        await ModifyProjectResolution(client, proj.ProjectCode, proj.Resolutions, sess.Id);
                        ////get
                        //req = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/repo/gip/Projects/{proj.ProjectCode}/Scopes/");
                        //res = await _helper.RequestHttp(client, req, "iipm-get-project-scope", sess.Id);
                        //if (res.Data.IsSuccessStatusCode)
                        //{
                        //    var json = await res.Data.Content.ReadAsStringAsync();
                        //    var iipmScope = JsonConvert.DeserializeObject<IIPMProjectScope>(json);
                        //    var i = 0;
                        //    //add , edit
                        //    foreach (var pdmoScope in proj.Scopes)
                        //    {
                        //        IIPMScope row;
                        //        if (i > iipmScope.items.Count - 1)
                        //        { // add
                        //            req = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/repo/gip/Projects/{proj.ProjectCode}/Scopes/");
                        //            row = new IIPMScope { scope = pdmoScope.ExtendData };
                        //            req.Content = new StringContent(JsonConvert.SerializeObject(row), Encoding.UTF8, "application/json");
                        //            res = await _helper.RequestHttp(client, req, "iipm-modify-project-scope", sess.Id);
                        //        }
                        //        else
                        //        {// edit
                        //            row = iipmScope.items[i];

                        //            if (pdmoScope.ExtendData != row.scope)
                        //            {
                        //                req = new HttpRequestMessage(HttpMethod.Put, $"{baseUrl}/repo/gip/Projects/{proj.ProjectCode}/Scopes/{row.id}");
                        //                row.scope = pdmoScope.ExtendData;
                        //                req.Content = new StringContent(JsonConvert.SerializeObject(row), Encoding.UTF8, "application/json");
                        //                res = await _helper.RequestHttp(client, req, "iipm-modify-project-scope", sess.Id);
                        //            }

                        //        } 

                        //        i++;
                        //    }
                        //    // delete
                        //    for (var j = proj.Scopes.Count; j < iipmScope.items.Count; j++)
                        //    {
                        //        var row = iipmScope.items[j];
                        //        req = new HttpRequestMessage(HttpMethod.Delete, $"{baseUrl}/repo/gip/Projects/{proj.ProjectCode}/Scopes/{row.id}");
                        //        res = await _helper.RequestHttp(client, req, "iipm-delete-project-scope", sess.Id);
                        //    }
                        //}


                    }
                    else
                    {
                        ret.AddMessage("IIPMAPIError", "IIPM sync data error (iipm-modify-project)", eMessageType.Error);
                        return ret;
                    }
                }
                ret.IsCompleted = true;

            }
            catch (Exception ex)
            {
                ret.AddMessage("IIPMAPIError", "IIPM sync data error (iipm-modify-project)", eMessageType.Error);
                ret.AddError(ex);
            }
            return ret;
        }
        private async Task<ReturnMessage> ModifyProjectExtend(HttpClient client, string extName,string extField, string projCode,string repo , List<ProjectExtendModel> projExtends, long sessId,bool deleteExisting)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                //get
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                var req = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/repo/{repo}/Projects/{projCode}/{extName}/");
                var res = await _helper.RequestHttp(client, req, $"iipm-get-project-{extField}", sessId);
                if (res.Data.IsSuccessStatusCode)
                {
                    var json = await res.Data.Content.ReadAsStringAsync();
                    JObject jo = JObject.Parse(json);
                    JArray jas = JArray.Parse(jo["items"].ToString());
                    var iipmExts = new List<BasicData>();
                    foreach (var ja in jas)
                    {
                        iipmExts.Add(new BasicData
                        {
                            ID = long.Parse(ja["id"].ToString()),
                            Description = ja[extField].ToString()
                        });
                    }
                    //var iipmScope = JsonConvert.DeserializeObject<IIPMProjectScope>(json);
                    var i = 0;
                    //add , edit
                    foreach (var pdmoExt in projExtends)
                    {
                        BasicData row;
                        JObject jBody = new JObject();
                        if (i > iipmExts.Count - 1)
                        { // add
                            req = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/repo/{repo}/Projects/{projCode}/{extName}/");
                            jBody.Add(new JProperty(extField, pdmoExt.ExtendData));
                            req.Content = new StringContent(jBody.ToString(), Encoding.UTF8, "application/json");

                            res = await _helper.RequestHttp(client, req, $"iipm-modify-project-{extField}", sessId);
                        }
                        else
                        {// edit
                            row = iipmExts[i];

                            if (pdmoExt.ExtendData != row.Description)
                            {
                                req = new HttpRequestMessage(HttpMethod.Put, $"{baseUrl}/repo/{repo}/Projects/{projCode}/{extName}/{row.ID}");
                                jBody.Add(new JProperty(extField, pdmoExt.ExtendData));
                                //jBody.Add(new JProperty("id", row.ID.ToString()));
                                req.Content = new StringContent(jBody.ToString(), Encoding.UTF8, "application/json");
                                res = await _helper.RequestHttp(client, req, $"iipm-modify-project-{extField}", sessId);
                            }

                        }

                        i++;
                    }
                    // delete
                    for (var j = projExtends.Count; j < iipmExts.Count; j++)
                    {
                        var row = iipmExts[j];
                        req = new HttpRequestMessage(HttpMethod.Delete, $"{baseUrl}/repo/{repo}/Projects/{projCode}/{extName}/{row.ID}");
                        res = await _helper.RequestHttp(client, req, $"iipm-delete-project-{extField}", sessId);
                    }
                }
                else
                {
                    ret.AddMessage("IIPMAPIError", $"IIPM sync data error (iipm-modify-project-{extField})", eMessageType.Error);
                    return ret;
                }
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
      
        }
        private async Task ModifyProjectLocation(HttpClient client, string projCode, List<ProjectLocationModel> projExtends, long sessId)
        {
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            var extName = "Areas";
            var extField = "area";
            //get
            var req = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/repo/gip/Projects/{projCode}/{extName}/");
            var res = await _helper.RequestHttp(client, req, $"iipm-get-project-{extField}", sessId);
            if (res.Data.IsSuccessStatusCode)
            {
                var json = await res.Data.Content.ReadAsStringAsync();
                JObject jo = JObject.Parse(json);
                JArray jas = JArray.Parse(jo["items"].ToString());
                //var iipmExts = new List<BasicData>();
                //foreach (var ja in jas)
                //{
                //    iipmExts.Add(new BasicData
                //    {
                //        ID = long.Parse(ja["id"].ToString()),
                //        Description = ja[extField].ToString()
                //    });
                //}
        
                var i = 0;
                //add , edit
                foreach (var pdmoExt in projExtends)
                {
                  
                    JObject jBody = new JObject();
                    jBody.Add(new JProperty("area", pdmoExt.Location));
                    jBody.Add(new JProperty("latitude", pdmoExt.Latitude));
                    jBody.Add(new JProperty("longitude", pdmoExt.Longitude));
                    if (i > jas.Count - 1)
                    { // add
                        req = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/repo/gip/Projects/{projCode}/{extName}/");
                    
                        req.Content = new StringContent(jBody.ToString(), Encoding.UTF8, "application/json");

                        res = await _helper.RequestHttp(client, req, $"iipm-modify-project-{extField}", sessId);
                    }
                    else
                    {// edit
                        var row = jas[i];
                        
                        if (pdmoExt.Location != row["area"].ToString() || pdmoExt.Latitude !=  (double?)row["latitude"] || pdmoExt.Longitude != (double?)row["longitude"])
                        {
                            req = new HttpRequestMessage(HttpMethod.Put, $"{baseUrl}/repo/gip/Projects/{projCode}/{extName}/{row["id"].ToString()}");
                            //jBody.Add(new JProperty("id", row["id"]));
                            req.Content = new StringContent(jBody.ToString(), Encoding.UTF8, "application/json");
                            res = await _helper.RequestHttp(client, req, $"iipm-modify-project-{extField}", sessId);
                        }

                    }

                    i++;
                }
                // delete
                for (var j = projExtends.Count; j < jas.Count; j++)
                {
                    var row = jas[j];
                    req = new HttpRequestMessage(HttpMethod.Delete, $"{baseUrl}/repo/gip/Projects/{projCode}/{extName}/{row["id"].ToString()}");
                    res = await _helper.RequestHttp(client, req, $"iipm-delete-project-{extField}", sessId);
                }
            }
        }
        private async Task ModifyProjectResolution(HttpClient client, string projCode, List<ProjectResolutionModel> projExtends, long sessId)
        {
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "multipart/form-data");
            var extName = "Cabinets";
            var extField = "cabinet";
            //get
            var req = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/repo/gip/Projects/{projCode}/{extName}/");
            var res = await _helper.RequestHttp(client, req, $"iipm-get-project-{extField}", sessId);
            if (res.Data.IsSuccessStatusCode)
            {
                var json = await res.Data.Content.ReadAsStringAsync();
                JObject jo = JObject.Parse(json);
                JArray jas = JArray.Parse(jo["items"].ToString());

                var i = 0;
                //add , edit
                foreach (var pdmoExt in projExtends)
                {

                    var nvc = new List<KeyValuePair<string, string>>();
                    nvc.Add(new KeyValuePair<string, string>("name", pdmoExt.Detail));
                    nvc.Add(new KeyValuePair<string, string>("budget", pdmoExt.Amount.ToString()));
                    nvc.Add(new KeyValuePair<string, string>("approvedAt", string.Format("{0}-{1}-{2}",pdmoExt.Date.Year,pdmoExt.Date.Month,pdmoExt.Date.Day)));
                  
                    if (i > jas.Count - 1)
                    { // add
                        req = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/repo/gip/Projects/{projCode}/{extName}/");

                        req.Content = new FormUrlEncodedContent(nvc); // jBody.ToString(), Encoding.UTF8, "application/json");

                        res = await _helper.RequestHttp(client, req, $"iipm-modify-project-{extField}", sessId);
                    }
                    else
                    {// edit
                        var row = jas[i];

                        if (pdmoExt.Detail != row["name"].ToString() || pdmoExt.Amount != (decimal)row["budget"] || pdmoExt.Date != (DateTime)row["approvedAt"])
                        {
                            req = new HttpRequestMessage(HttpMethod.Put, $"{baseUrl}/repo/gip/Projects/{projCode}/{extName}/{row["id"].ToString()}");
                            //jBody.Add(new JProperty("id", row["id"]));
                            req.Content = new FormUrlEncodedContent(nvc);
                            res = await _helper.RequestHttp(client, req, $"iipm-modify-project-{extField}", sessId);
                        }

                    }

                    i++;
                }
                // delete
                for (var j = projExtends.Count; j < jas.Count; j++)
                {
                    var row = jas[j];
                    req = new HttpRequestMessage(HttpMethod.Delete, $"{baseUrl}/repo/gip/Projects/{projCode}/{extName}/{row["id"].ToString()}");
                    res = await _helper.RequestHttp(client, req, $"iipm-delete-project-{extField}", sessId);
                }
            }
        }
    }
   
}
