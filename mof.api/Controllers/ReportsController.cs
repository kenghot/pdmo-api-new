using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mof.ServiceModels;
using mof.ServiceModels.Common;
using mof.ServiceModels.Response;
using mof.ServiceModels.Request;
using mof.DataModels.Models;
using Microsoft.AspNetCore.Authorization;
using mof.IServices;
using mof.Services;
using Microsoft.AspNetCore.Identity;
using System.Net.Http;
using IdentityModel.Client;
using System.Globalization;
using Microsoft.Extensions.Localization;
using System.Threading;
using mof.ServiceModels.Identity;
using System.Security.Claims;
using IdentityServer4.AccessTokenValidation;
using mof.ServiceModels.Organization;
using System.ComponentModel.DataAnnotations;
using mof.ServiceModels.Common.Generic;
using mof.ServiceModels.Plan;
using System.ComponentModel;
using mof.ServiceModels.Agreement;
using mof.ServiceModels.MonthlyReport;
using mof.ServiceModels.PublicDebt;
using FastReport.Utils;
using FastReport;
using FastReport.Export.Html;
using FastReport.Export.PdfSimple;
using Microsoft.AspNetCore.Hosting;
using mof.ServiceModels.Reports;
using System.Data;
using System.IO;
using Newtonsoft.Json.Linq;
using mof.ServiceModels.IIPMModel;
using mof.ServiceModels.Reports.ExistingPlan;
using ClosedXML.Report;

namespace mof.api.Controllers
{
    /// <summary>
    /// รายงาน
    /// </summary>
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public class ReportsController : Controller
    {
        private IOrganization _iorg;
        private UserManager<ApplicationUser> _user;
        SignInManager<ApplicationUser> _sign;
        IStringLocalizer<MessageLocalizer> _msglocalizer;
        RoleManager<ApplicationRole> _role;
        private ISystemHelper _helper;
        private IPlan _plan;
        private IAgreement _agr;
        private ICommon _com;
        private readonly IHostingEnvironment _hostingEnvironment;
        private IReport _rep;
        public ReportsController(IOrganization iorg, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IStringLocalizer<MessageLocalizer> msglocalizer,
            RoleManager<ApplicationRole> role,
            ISystemHelper helper,
            IPlan plan,
            IAgreement agr,
            ICommon com,
            IHostingEnvironment hostingEnvironment,
            IReport rep
        )
        {

            _iorg = iorg;
            _user = userManager;
            _sign = signInManager;
            _msglocalizer = msglocalizer;
            _role = role;
            _helper = helper;
            _plan = plan;
            _agr = agr;
            _com = com;
            _hostingEnvironment = hostingEnvironment;
            _rep = rep;


        }

        /// <summary>
        /// Dashboard สรุป หนี้สาธารณะคงค้าง
        /// </summary>
        /// <returns></returns>
        [HttpGet("publicDebtSummaryDashboard")]
        public async Task<ActionResult<ReturnObject<PublicDebtDashBoard>>> publicDebtSummaryDashboard( )
        {
            var ret = new ReturnObject<PublicDebtDashBoard>(_msglocalizer);
            try
            {

                ret = await _com.PublicDebtSummaryDashboard();
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        FReport[] reportItems = new FReport[]
                                    {
                                        new FReport { Id = 1, ReportName = "Master-Detail.frx" },
                                        new FReport { Id = 2, ReportName = "Barcode.frx" }
                                    };
        // Attribute has required id parameter
        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult Get(int id, [FromQuery] FReportQuery query)
        {
            string mime = "application/" + query.Format; // MIME header with default value
                                                         // Find report
            FReport reportItem = reportItems.FirstOrDefault((p) => p.Id == id); // we get the value of the collection by id
            if (reportItem != null)
            {
                string webRootPath = _hostingEnvironment.WebRootPath; // determine the path to the wwwroot folder
                string reportPath = (webRootPath + "/App_Data/" + reportItem.ReportName); // determine the path to the report
                string dataPath = (webRootPath + "/App_Data/nwind.xml");// determine the path to the database
                using (MemoryStream stream = new MemoryStream()) // Create a stream for the report
                {
                    try
                    {
                        using (DataSet dataSet = new DataSet())
                        {
                            // Fill the source by data
                            dataSet.ReadXml(dataPath);
                            // Turn on web mode FastReport
                            Config.WebMode = true;
                            using (FastReport.Report report = new FastReport.Report())
                            {
                                report.Load(reportPath); // Download the report
                                report.RegisterData( dataSet, "NorthWind"); // Register data in the report
                                if (query.Parameter != null)
                                {
                                    report.SetParameterValue("Parameter", query.Parameter); // Set the value of the report parameter if the parameter value is passed to the URL
                                }
                                report.Prepare();//Prepare the report
                                                 // If pdf format is selected
                                if (query.Format == "pdf")
                                {
                                    // Export report to PDF

                                    PDFSimpleExport pdf = new PDFSimpleExport();
                                    // Use the stream to store the report, so as not to create unnecessary files
                                    report.Export(pdf, stream);
                                    
                                }
                                // If html report format is selected
                                else if (query.Format == "html")
                                {
                                    // Export Report to HTML
                                    HTMLExport html = new HTMLExport();
                                    html.SinglePage = true; // Single page report
                                    html.Navigator = false; // Top navigation bar
                                    html.EmbedPictures = true; // Embeds images into a document
                                    report.Export(html, stream);
                                    mime = "text/" + query.Format; // Override mime for html
                                }
                            }
                        }
                        // Get the name of the resulting report file with the necessary extension var file = String.Concat(Path.GetFileNameWithoutExtension(reportPath), ".", query.Format);
                        // If the inline parameter is true, then open the report in the browser
                        var file = String.Concat(Path.GetFileNameWithoutExtension(reportPath), ".", query.Format);
                        if (query.Inline)
                            return File(stream.ToArray(), mime);
                        else
                            // Otherwise download the report file 
                            return File(stream.ToArray(), mime, file); // attachment
                    }
                    // Handle exceptions
                    catch
                    {
                        return new NoContentResult();
                    }
                    finally
                    {
                        stream.Dispose();
                    }
                }
            }
            else
                return NotFound();
        }
        [HttpGet("Test/{repName}")]
        [AllowAnonymous]
        public async Task<IActionResult> Test(string repName, [FromQuery] FReportQuery query)
        {
            string mime = "application/" + query.Format; // MIME header with default value
                                                         // Find report
 
                string webRootPath = _hostingEnvironment.WebRootPath; // determine the path to the wwwroot folder
                string reportPath = (webRootPath + "/reports/" + repName + ".frx"); // determine the path to the report
  
                using (MemoryStream stream = new MemoryStream()) // Create a stream for the report
                {
                    try
                    {
                        using (HttpClient client = new HttpClient())
                        {
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJzY2hlbWUiOiJpaXBtd2ViLmFwaSIsInVzZXJJZCI6IjQ0ODEiLCJhZ2VuY3kiOiIwMzAwOSIsInJvbGUiOiJTeXN0ZW0iLCJwZXJtaXNzaW9uIjpbIlBWWV9MaXN0UHJvamVjdCIsIlBWWV9SZWFkUHJvamVjdCIsIlBWWV9VcGRhdGVQcm9qZWN0IiwiUFZZX0xpc3RQbGFuIiwiUFZZX1JlYWRQbGFuIiwiUFZZX1VwZGF0ZVBsYW4iLCJQVllfTGlzdEFwcHJvdmFsIiwiUFZZX1JlYWRBcHByb3ZhbCJdLCJleHAiOjE2MTMwNzM2MTIsImlzcyI6Imh0dHA6Ly9jc3JzLmt1LmFjLnRoIiwiYXVkIjoidHJzeXMifQ.-V5CFEVkZu0GjRv7CGJUZ-UiYCf0NzcQkUekTgUxdK3lzGbD-99JRZi2QZpYyZKBIh0uyaFk72xQDQwRqf0y1A");
                        // Fill the source by data
                        var resp = await client.GetStringAsync("http://csrs.ku.ac.th/iipmweb/repo/pvy/Projects/Active?$expand=currentProjectPlan($expand=planYearBudgets,planConfig),projectInfo($expand=agency,province,sector)");
                            JObject json = JObject.Parse(resp);
                            var pjp = json.ToObject<ProjectPlanModel>();

                        var pjps = new List<ProjectPlanModel> { pjp };
                        // Turn on web mode FastReport
                            Config.WebMode = true;
                            using (FastReport.Report report = new FastReport.Report())
                            {
                                report.Load(reportPath); // Download the report
                            report.RegisterData(pjps, "JSON"); // Register data in the report
                                if (query.Parameter != null)
                                {
                                    report.SetParameterValue("Parameter", query.Parameter); // Set the value of the report parameter if the parameter value is passed to the URL
                                }
                                report.Prepare();//Prepare the report
                                                 // If pdf format is selected
                                if (query.Format == "pdf")
                                {
                                    // Export report to PDF

                                    PDFSimpleExport pdf = new PDFSimpleExport();
                                    // Use the stream to store the report, so as not to create unnecessary files
                                    report.Export(pdf, stream);

                                }
                                // If html report format is selected
                                else if (query.Format == "html")
                                {
                                    // Export Report to HTML
                                    HTMLExport html = new HTMLExport();
                                    
                                    html.SinglePage = true; // Single page report
                                    html.Navigator = false; // Top navigation bar
                                    html.EmbedPictures = true; // Embeds images into a document
                                    report.Export(html, stream);
                                    mime = "text/" + query.Format; // Override mime for html
                                }
                            FastReport.Web.WebReport w = new FastReport.Web.WebReport();
                            
                            }
                        }
                        // Get the name of the resulting report file with the necessary extension var file = String.Concat(Path.GetFileNameWithoutExtension(reportPath), ".", query.Format);
                        // If the inline parameter is true, then open the report in the browser
                        var file = String.Concat(Path.GetFileNameWithoutExtension(reportPath), ".", query.Format);
                        if (query.Inline)
                            return File(stream.ToArray(), mime);
                        else
                            // Otherwise download the report file 
                            return File(stream.ToArray(), mime, file); // attachment
                    }
                    // Handle exceptions
                    catch (Exception ex)
                    {
                        return new NoContentResult();
                    }
                    finally
                    {
                        stream.Dispose();
                    }
                }
 
               
        }
        /// <summary>
        /// แบบฟอร์มที่ 1 สรุปความต้องการใช้เงินกู้ในประเทศและต่างประเทศ
        /// </summary>
        /// <param name="planID"></param>
        /// <returns></returns>
        [HttpGet("NewDebtPlan/{planID}")]
        [AllowAnonymous]
        public async Task<IActionResult> NewDebtPlan([FromRoute]long planID)
        {
            var ret = new ReturnObject<FileContentResult>(_msglocalizer);
            ret.IsCompleted = false;

            try
            {
                ret = await _rep.NewDebtPlanRep(planID);
                if (ret.IsCompleted)
                {
                    return ret.Data;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        /// <summary>
        /// แบบฟอร์มที่ 2 & 3 : สรุปความต้องการชำระหนี้ และบริหารหนี้ (การปรับโครงสร้างหนี้และการบริหารความเสี่ยง) หนี้ในประเทศและต่างประเทศ 
        /// </summary>
        /// <param name="planID"></param>
        /// <returns></returns>
        [HttpGet("ExistingPlanByAgreement/{planID}")]
        [AllowAnonymous]
        public async Task<IActionResult> ExistingPlanByAgreement([FromRoute]long planID)
        {
            var ret = new ReturnObject<FileContentResult>(_msglocalizer);
            ret.IsCompleted = false;

            try
            {
                ret = await _rep.ExistingPlanByAgreement(planID);
                if (ret.IsCompleted)
                {
                    return ret.Data;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
        ///// <summary>
        ///// แบบฟอร์มที่ 3 : สรุปความต้องการชำระหนี้ และบริหารหนี้ (การปรับโครงสร้างหนี้และการบริหารความเสี่ยง) หนี้ในประเทศและต่างประเทศ 5 ปีข้างหน้า
        ///// </summary>
        ///// <param name="planID"></param>
        ///// <returns></returns>
        //[HttpGet("ExistingPlanByAgreementForcast/{planID}")]
        //[AllowAnonymous]
        //public async Task<IActionResult> ExistingPlanByAgreementForcast([FromRoute]long planID)
        //{
        //    var ret = new ReturnObject<FileContentResult>(_msglocalizer);
        //    ret.IsCompleted = false;

        //    try
        //    {
        //        ret = await _rep.ExistingPlanByAgreement(planID, false);
        //        if (ret.IsCompleted)
        //        {
        //            return ret.Data;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
        //    }
        //    return Ok(ret);
        //}
        /// <summary>
        /// แบบฟอร์มที่ 4 สรุปสถานะทางการเงินและภาระหนี้
        /// </summary>
        /// <param name="planID"></param>
        /// <returns></returns>
        [HttpGet("FinancialReport/{planID}")]
        [AllowAnonymous]
        public async Task<IActionResult> FinancialReoprt([FromRoute]long planID)
        {
            var ret = new ReturnObject<FileContentResult>(_msglocalizer);
            ret.IsCompleted = false;

            try
            {
                ret = await _rep.FinancialPlanRep(planID);
                if (ret.IsCompleted)
                {
                    return ret.Data;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, Helper.ThrowException(ex, _msglocalizer));
            }
            return Ok(ret);
        }
    }
}