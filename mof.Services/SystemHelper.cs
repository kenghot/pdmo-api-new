using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using mof.DataModels.Models;
using mof.IServices;
using mof.ServiceModels.Common;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Linq.Expressions;
using mof.Services.Helper;
using Microsoft.EntityFrameworkCore;
using mof.ServiceModels.Request;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Localization;
using mof.ServiceModels.Request.Common;
using mof.ServiceModels.Response;
using mof.ServiceModels.Identity;
using mof.ServiceModels.Common.Generic;
using mof.ServiceModels.Helper;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace mof.Services
{
    public class SystemHelper : IServices.ISystemHelper
    {

        public MOFContext DB;
        public IDbConnection CN;
        private UserManager<ServiceModels.Identity.ApplicationUser> _user;
        private SignInManager<ServiceModels.Identity.ApplicationUser> _signin;
        private Microsoft.AspNetCore.Identity.UI.Services.IEmailSender _email;
        private IConfiguration _conf;
        private IStringLocalizer<MessageLocalizer> _msglocalizer;
        private RoleManager<ApplicationRole> _role;
        public SystemHelper(MOFContext db, UserManager<ServiceModels.Identity.ApplicationUser> userManager,
            SignInManager<ServiceModels.Identity.ApplicationUser> signInManager,
            Microsoft.AspNetCore.Identity.UI.Services.IEmailSender email,
            IStringLocalizer<MessageLocalizer> msglocalizer,
            RoleManager<ApplicationRole> role,
            IConfiguration conf)
        {
            DB = db;
            CN = new SqlConnection(DB.GetConnectionString());
            _user = userManager;
            _signin = signInManager;
            _email = email;
            _msglocalizer = msglocalizer;
            _role = role;
            _conf = conf;
        }
        public async Task<ReturnObject<List<CurrencyScreen>>> ListCurrency()
        {
            var ret = new ReturnObject<List<CurrencyScreen>>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var data = await DB.Currency
                     
                    .OrderByDescending(o => o.CurrencyCode).Select(s => new CurrencyScreen
                    { 
                        CurrencyCode = s.CurrencyCode,
                        CurrencyName = s.CurrencyName
                    }).ToListAsync();
                foreach (var d in data)
                {
                    var key = Helper.Utilities.StringToKey(d.CurrencyCode);
                    var log = await DB.DataLog.Where(w => w.TableName == "Currency" && w.TableKey == key).OrderByDescending(o => o.LogId).FirstOrDefaultAsync();
                    if (log != null)
                    {
                        d.Log = new LogData
                        {
                            Action = log.LogType,
                            ActionTime = log.LogDt,
                            ActionType = log.LogType,
                            UserID = log.UserId,


                        };
                        var u = await DB.Users.Where(w => w.Id == d.Log.UserID).FirstOrDefaultAsync();
                        if (u != null)
                        {
                            d.Log.UserName = u.TfirstName;
                        }
                    }
                                 
                }
                ret.Data = data;
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;

        }
        public async Task<ReturnMessage> ModifyCurrency(CurrencyScreen d, bool isCreate, string userID)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
            
                var curr = await DB.Currency.Where(w => w.CurrencyCode == d.CurrencyCode).FirstOrDefaultAsync();
                if (isCreate)
                {
                    if (curr != null)
                    {
                        ret.AddMessage(eMessage.DataIsAlreadyExist.ToString(), "currency", eMessageType.Error, new string[] { _msglocalizer[eMessage.Currency.ToString()]});
                        return ret;

                    }
                    curr = new Currency
                    {
                         CurrencyCode = d.CurrencyCode

                    };
                    DB.Currency.Add(curr);
                }
                else
                {
                    if (curr == null)
                    {
                        ret.AddMessage(eMessage.DataIsNotFound.ToString(), "currency", eMessageType.Error, new string[] { _msglocalizer[eMessage.Currency.ToString()] });
                        return ret;

                    }
                }
                curr.CurrencyName = d.CurrencyName;
                
                var log  = new DataLog
                {
                    LogDt = DateTime.Now,
                    LogType = (isCreate) ? "C" : "U",
                    TableKey = Helper.Utilities.StringToKey(d.CurrencyCode),
                    TableName = "Currency",
                    UserId = userID
                };
                DB.DataLog.Add(log); 
                await DB.SaveChangesAsync();
                ret.IsCompleted = true;

            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        public async Task<ReturnMessage> DeleteCurrency(string code, string userID)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {

                var curr = await DB.Currency.Where(w => w.CurrencyCode == code).FirstOrDefaultAsync();
 
                    if (curr == null)
                    {
                        ret.AddMessage(eMessage.DataIsNotFound.ToString(), "currency", eMessageType.Error, new string[] { _msglocalizer[eMessage.Currency.ToString()] });
                        return ret;

                    }

                DB.Currency.Remove(curr);

                var log = new DataLog
                {
                    LogDt = DateTime.Now,
                    LogType = "D",
                    TableKey = Helper.Utilities.StringToKey(code),
                    TableName = "Currency",
                    UserId = userID
                };
                DB.DataLog.Add(log);
                await DB.SaveChangesAsync();
                ret.IsCompleted = true;

            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        /// <summary>
        /// for currency rate screen
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<ReturnObject<CurrencyData>> GetCurrencyRateScreen(int year)
        {
            var data = new ReturnObject<CurrencyData>(_msglocalizer);
            data.IsCompleted = false;

            try
            {
                var ret = await DB.CurrencyYear.Where(w => w.Year == year).Select(s => new CurrencyData
                {
                    RateAsOF = s.RateDate,
                    Remark = s.Remark,
                    Source = s.Source,
                    Year = s.Year
                }).FirstOrDefaultAsync();
                if (ret == null)
                {
                    ret = new CurrencyData
                    {
                        Year = year
                    };
                }

                ret.Currency = new List<CurrecyRateData>();
                foreach (var c in DB.Currency)
                {
                    ret.Currency.Add(new CurrecyRateData
                    {
                        CurrencyCode = c.CurrencyCode,
                        CurrencyName = c.CurrencyName,
                        CurrencyRate = 0
                    });
                }
                foreach (var rate in  DB.CurrencyRate.Where(w => w.CurrencyYear == year))
                {
                    var cur = ret.Currency.Where(w => w.CurrencyCode == rate.CurrencyCode).FirstOrDefault();
                    if (cur != null)
                    {
                        cur.CurrencyRate = rate.CurrencyRate1;
                    } 
                }
                data.Data = ret;
                data.IsCompleted = true;
            }
            catch (Exception ex)
            {
                data.AddError(ex);
            }

            return data;
        }
        public async Task<ReturnMessage> UpdateCurrencyRateScreen(CurrencyData currency)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                if (currency.Year < 2500 || currency.Year > 2660)
                {
                    ret.Message.Add(new MessageData
                    {
                        Code = "CurrYearDataRange",
                        MessageType = eMessageType.Error.ToString(),
                        Message = "ปีต้องอยู่ระหว่าง 2500 - 2660"
                    });
                    return ret;
                }
                var year = await DB.CurrencyYear.Include(i => i.CurrencyRate).Where(w => w.Year == currency.Year).FirstOrDefaultAsync();
                if (year == null)
                {
                    year = new CurrencyYear
                    {
                        Year = currency.Year,
                        
                    };
                    DB.CurrencyYear.Add(year);
                    year.RateDate = currency.RateAsOF;
                    year.Remark = currency.Remark;
                    year.Source = currency.Source;

                }
                foreach (var cur in currency.Currency)
                {
                    var dbcur = year.CurrencyRate.Where(w => w.CurrencyCode == cur.CurrencyCode).FirstOrDefault();
                    if (dbcur == null)
                    {
                        dbcur = new CurrencyRate
                        {
                            CurrencyCode = cur.CurrencyCode,
                            CurrencyYear = year.Year,

                        };
                        year.CurrencyRate.Add(dbcur);
                    }
                    dbcur.CurrencyRate1 = cur.CurrencyRate;
                }
                await DB.SaveChangesAsync();
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }

            return ret;
        }
        public ReturnObject<CurrencyData> GetCurrencyRate(int year)
        {
            var data = new ReturnObject<CurrencyData>(_msglocalizer);
            data.IsCompleted = false;
         
            try
            {
                var ret = DB.CurrencyYear.Where(w => w.Year == year).Select(s => new CurrencyData
                {
                    RateAsOF = s.RateDate,
                    Remark = s.Remark,
                    Source = s.Source,
                    Year = s.Year
                }).FirstOrDefault();
                if (ret != null)
                {
                    ret.Currency = DB.CurrencyRate.Where(w => w.CurrencyYear == year).Select(s => new CurrecyRateData
                    {
                        CurrencyCode = s.CurrencyCode,
                        CurrencyName = s.CurrencyCodeNavigation.CurrencyName,
                        CurrencyRate = s.CurrencyRate1
                    }).ToList();
                    var thb = ret.Currency.Where(w => w.CurrencyCode == "THB").FirstOrDefault();
                    if (thb == null)
                    {
                        ret.Currency.Add(new CurrecyRateData { CurrencyCode = "THB", CurrencyName = "Thai Baht", CurrencyRate = 1 });
                    }
                    data.Data = ret;
                    data.IsCompleted = true;
                }
                else
                {
                    data.AddMessage(eMessage.DataIsNotFound.ToString(), "rate", eMessageType.Error, new string[] { $"อัตราแลกเปลี่ยน ปี {year}" });
                }
            }
            catch (Exception ex)
            {
                data.AddError(ex);
            }

            return data;
        }
        /// <summary>
        /// แสดงค่า LOV โดยระบุ LOVCode
        /// </summary>
        /// <param name="LOVCode">ต้องระบุ</param>
        /// <returns></returns>
        public ReturnObject<LOV> GetLOVByCode(string LOVCode,string LOVGroupCode)
        {
            var ret = new ReturnObject<LOV>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                if (string.IsNullOrEmpty(LOVCode)) {
                    ret.AddMessage("CodeRequired", "code is required", eMessageType.Error);
                    return ret;
                }
                var lov =   DB.CeLov.Where(w => w.Lovcode == LOVCode && w.LovgroupCode == LOVGroupCode && w.IsCanceled == false)
                    .Select(s => new LOV
                    {
                        IsCanceled = s.IsCanceled,
                        LOVCode = s.Lovcode,
                        LOVGroupCode = s.LovgroupCode,
                        LOVKey = s.Lovkey,
                        LOVValue = s.Lovvalue,
                        OrderNo = s.OrderNo,
                        ParentGroup = s.ParentGroup,
                        ParentLOV = s.ParentLov
                    }).FirstOrDefault();
                ret.Data = lov;
                ret.IsCompleted = true;
                return ret;
            }catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        /// <summary>
        /// แสดงค่า LOV โดยระบุ LOVGroupcode
        /// </summary>
        /// <param name="LOVGroupCode">ระบุ LOVGroupCode ถ้าไม่ระบุคือเอาทั้งหมด</param>
        /// <returns></returns>
        public ReturnObject<List<LOV>> GetLOVByGroup(string LOVGroupCode)
        {
            var ret = new ReturnObject<List<LOV>>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var lov = DB.CeLov.Where(w => w.IsCanceled == false)
                    .Select(s => new LOV
                    {
                        IsCanceled = s.IsCanceled,
                        LOVCode = s.Lovcode,
                        LOVGroupCode = s.LovgroupCode,
                        LOVKey = s.Lovkey,
                        LOVValue = s.Lovvalue,
                        OrderNo = s.OrderNo,
                        ParentGroup = s.ParentGroup,
                        ParentLOV = s.ParentLov,
                        Remark = s.Remark
                    });
                if (!string.IsNullOrEmpty(LOVGroupCode))
                {
                    lov = lov.Where(w => w.LOVGroupCode == LOVGroupCode);
                }
               ret.Data = lov.OrderBy(o => o.LOVGroupCode).ThenBy(o2 => o2.OrderNo).ToList();
                ret.IsCompleted = true;
                return ret;

            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }

        public ReturnObject<List<LOV>> GetLOVByParent(string ParentGroupCode, string ParentLovCode)
        {
            var ret = new ReturnObject<List<LOV>>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var lov = DB.CeLov.Where(w => w.ParentGroup == ParentGroupCode && w.ParentLov == ParentLovCode && w.IsCanceled == false)
                    .Select(s => new LOV
                    {
                        IsCanceled = s.IsCanceled,
                        LOVCode = s.Lovcode,
                        LOVGroupCode = s.LovgroupCode,
                        LOVKey = s.Lovkey,
                        LOVValue = s.Lovvalue,
                        OrderNo = s.OrderNo,
                        ParentGroup = s.ParentGroup,
                        ParentLOV = s.ParentLov
                    });
   
                ret.Data = lov.OrderBy(o => o.LOVGroupCode).ThenBy(o2 => o2.OrderNo).ToList();
                ret.IsCompleted = true;
                return ret;

            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }

        /// <summary>
        /// Get list of LOVGroup
        /// </summary>
        /// <param name="LOVGroupCode">ระบุ LOVGroupCode ถ้าไม่ระบุคือเอาทั้งหมด</param>
        /// <returns></returns>
        public ReturnObject<List<BasicData>> GetLOVGroup(string LOVGroupCode)
        {
            var ret = new ReturnObject<List<BasicData>>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var lov = DB.CeLovgroup.Select(s => s);
                if (!string.IsNullOrEmpty(LOVGroupCode))
                {
                    lov = lov.Where(w => w.LovgroupCode == LOVGroupCode);
                }
                ret.Data = lov.OrderBy(o => o.LovgroupCode).Select(s => new BasicData { Code = s.LovgroupCode, Description = s.LovgroupName }).ToList();
                ret.IsCompleted = true;
                return ret;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }

        public ReturnObject<LOV> LOVCodeValidate(string LOVCode, string LOVGroupCode,long? ParentKey)
        {
            var ret = new ReturnObject<LOV>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var gName = LOVGroupCode;
                if (!string.IsNullOrEmpty(LOVGroupCode))
                {
                    var g = DB.CeLovgroup.Where(w => w.LovgroupCode == LOVGroupCode).FirstOrDefault();
                    if (g == null)
                    {
                        ret.AddMessage(eMessage.CodeIsNotValid.ToString(), "not valid", eMessageType.Error, new string[] { $"{gName}" });
                        return ret;
                    }else
                    {
                        gName = g.LovgroupName;
                    }
                }
                var lov = DB.CeLov.Where(w => w.Lovcode == LOVCode && w.LovgroupCode == LOVGroupCode)
                    .Select(s => new LOV { IsCanceled = s.IsCanceled , LOVCode = s.Lovcode, LOVGroupCode = s.LovgroupCode , LOVKey = s.Lovkey, LOVValue = s.Lovvalue, OrderNo = s.OrderNo ,
                    ParentGroup = s.ParentGroup , ParentLOV = s.ParentLov})
                    .FirstOrDefault();
                if (lov == null)
                {
                    ret.AddMessage(eMessage.CodeIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { $"{gName}" } );
                    return ret;
                }
                if (!string.IsNullOrEmpty(LOVGroupCode))
                {
                   
                    if (lov.LOVGroupCode != LOVGroupCode)
                    {
                        ret.AddMessage(eMessage.CodeIsNotValid.ToString(), "not valid", eMessageType.Error, new string[] { $"{gName} ({LOVCode})" });
                        return ret;
                    }
                }
                if (ParentKey.HasValue)

                {
                    var p = DB.CeLov.Where(w => w.Lovkey == ParentKey.Value).FirstOrDefault();
                    if (p == null)
                    {
                        ret.AddMessage(eMessage.CodeIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { $"Parent key {ParentKey.Value}" });
                        return ret;
                    }
                    if (lov.ParentGroup != p.LovgroupCode || lov.ParentLOV != p.Lovcode)
                    {
                        ret.AddMessage(eMessage.CodeIsNotValid.ToString(), "not valid", eMessageType.Error, new string[] { $"Parent {p.LovgroupCodeNavigation.LovgroupName} : {p.ParentLov}" });
                        return ret;
                    }
                }
                ret.IsCompleted = true;
                ret.Data = lov;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }

        public ReturnMessage UserValidate(string UserID)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var u = DB.Users.Where(w => w.Id == UserID).FirstOrDefault();
                if (u == null)
                {
                    ret.AddMessage(eMessage.UserIsNotFound.ToString(), "not found", eMessageType.Error);
                    return ret;
                }
                ret.IsCompleted = true;
                return ret;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }

        public async Task<ReturnObject<long?>> UploadFile(MOFContext db,AttachFileData af, bool isDbSave) {
            var ret = new ReturnObject<long?>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var media = mof.Services.Helper.Utilities.GetMMIEType(af.FileExtension, _msglocalizer);
                if (!media.IsCompleted)
                {
                    ret.CloneMessage(media.Message);
                    return  ret ;
                }
                AttachFile att;
                if (af.ID.HasValue)
                {
                    att = await db.AttachFile.Where(w => w.AttachFileId == af.ID.Value).FirstOrDefaultAsync();
                    if (att == null)
                    {
                        att = new AttachFile
                        {
                            FileName = af.FileName,
                            FileDetail = af.FileDetail,
                            FileExtension = af.FileExtension,
                            FileSize = af.FileSize

                        };
                        DB.AttachFile.Add(att);
                    }
                    else
                    {
                        att.FileName = af.FileName;
                        att.FileDetail = af.FileDetail;
                        att.FileExtension = af.FileExtension;
                        att.FileSize = af.FileSize;
                    }
                }else
                {
                    att = new AttachFile
                    {
                        FileName = af.FileName,
                        FileDetail = af.FileDetail,
                        FileExtension = af.FileExtension,
                        FileSize = af.FileSize

                    };
                    db.AttachFile.Add(att);
                }
 
                if (af.FileData?.Length > 0)
                {
                    att.FileData = af.FileData;
                }

                if (af.ClearFile)
                {
                    att.FileData = null;
                }
                if (isDbSave)
                {
                    await db.SaveChangesAsync();
                }
                
                ret.IsCompleted = true;
                ret.Data = att.AttachFileId;
                af.ID = att.AttachFileId;
                return ret;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;

        }
        public async Task<ReturnObject<LogData>> GetDataLog(long id)
        {
            var ret = new ReturnObject<LogData>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var l = await  DB.DataLog.Where(w => w.LogId == id).FirstOrDefaultAsync();
                if (l !=null)
                {
                    ret.Data = new LogData
                    {
                        Action = (l.LogType == "C") ? "Create" : (l.LogType == "U") ? "Update" : (l.LogType == "R") ? "Read" : (l.LogType == "D") ? "Delete" : "",
                        ActionTime = l.LogDt,
                        ActionType = l.LogType,
                        ID = l.LogId,
                        UserID = l.UserId
                    };
                    if (ret.Data.UserID != null)
                    {
                        var u = await DB.Users.Where(w => w.Id == ret.Data.UserID).FirstOrDefaultAsync();
                        if (u != null)
                        {
                            ret.Data.UserName = u.TfirstName + " " + u.TlastName;
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

        public async Task<ReturnObject<List<BasicData>>> GetProjectType(long orgID)

        {
            var ret = new ReturnObject<List<BasicData>>(_msglocalizer);
            ret.IsCompleted = false; 
            try
            {
                List<string> code = new List<string>();
                var org = await DB.Organization.Include(i => i.OrgtypeNavigation).Where(w => w.OrganizationId == orgID).FirstOrDefaultAsync();
                if (org == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { "หน่วยงาน" });
                    return ret;
                }
                if (org.OrgtypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.ประเภทหน่วยงาน.รัฐวิสาหกิจ)
                {
                    code.Add(ServiceModels.Constants.LOVGroup.Project_Type.กู้เพื่อลงทุนในโครงการพัฒนา);
                    code.Add(ServiceModels.Constants.LOVGroup.Project_Type.กู้เพื่อโครงการ);
                    code.Add(ServiceModels.Constants.LOVGroup.Project_Type.กู้เพื่อดำเนินโครงการหรือเพื่อใช้เป็นเงินทุนหมุนเวียนในการดำเนินกิจการทั่วไป);
                 
                }
                else
                {
                    code.Add(ServiceModels.Constants.LOVGroup.Project_Type.กู้เพื่อชดเชยการขาดดุลสำหรับปีงบประมาณ);
                    code.Add(ServiceModels.Constants.LOVGroup.Project_Type.กู้เพื่อชดเชยการขาดดุลสำหรับการเบิกจ่ายกันเหลื่อมปี);
                    code.Add(ServiceModels.Constants.LOVGroup.Project_Type.กู้เพื่อโครงการอื่นๆ);

                    code.Add(ServiceModels.Constants.LOVGroup.Project_Type.กู้เพื่อลงทุนในโครงการพัฒนา);
                    code.Add(ServiceModels.Constants.LOVGroup.Project_Type.กู้เพื่อโครงการ);
                    code.Add(ServiceModels.Constants.LOVGroup.Project_Type.กู้เพื่อดำเนินโครงการหรือเพื่อใช้เป็นเงินทุนหมุนเวียนในการดำเนินกิจการทั่วไป);
                    code.Add(ServiceModels.Constants.LOVGroup.Project_Type.กู้เพื่อบริหารสภาพคล่องของเงินคงคลัง);
                    code.Add(ServiceModels.Constants.LOVGroup.Project_Type.กู้เพื่อ_พ_ร_ก__เราไม่ทิ้งกัน_2020__COVID_19_);
                    code.Add("COVID21");

                }
                var data = await DB.CeLov.Where(w => w.LovgroupCode == ServiceModels.Constants.LOVGroup.Project_Type._LOVGroupCode && code.Contains(w.Lovcode))
                    .Select(s => new BasicData { Code = s.Lovcode, Description = s.Lovvalue, ID = s.Lovkey }).ToListAsync();
                ret.Data = data;
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }

        public async Task<ReturnObject<List<BasicData>>> GetLoanType(string planType, long orgID)
        {
            var ret = new ReturnObject<List<BasicData>>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {

                var org = await DB.Organization.Where(w => w.OrganizationId == orgID).FirstOrDefaultAsync();
                if (org == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { "หน่วยงาน" });
                    return ret;
                }
                List<string> code = new List<string>();
                if (planType == ServiceModels.Constants.LOVGroup.Plan_Type.แผนบริหารหนี้เดิม)
                {
                    //code.Add(ServiceModels.Constants.LOVGroup.Loan_Type.)
                }
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        public async Task<ReturnObject<HttpResponseMessage>> RequestHttp(HttpClient client, HttpRequestMessage request, string action, long? sessionId)
        {
            var ret = new ReturnObject<HttpResponseMessage>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
              
                var options = new DbContextOptionsBuilder<MOFContext>();
                options.UseSqlServer(_conf.GetConnectionString("DefaultConnection")); 
                var db = new MOFContext(options.Options);
                if (!sessionId.HasValue)
                {
                    var sess = new ApiSession { SessionDt = DateTime.Now };
                    db.ApiSession.Add(sess);
                    await db.SaveChangesAsync();
                    sessionId = sess.Id;
                }
                var newLog = new ApiLog
                {
                    Action = action,
                    ApiEndpoint = request.RequestUri.AbsoluteUri,
                    Request = request.ToString(),
                    RequestDt = DateTime.Now,
                    RequestContent = request.Content == null ? "" : request.Content.ReadAsStringAsync().Result,
                    SessionId = sessionId

                };
                db.ApiLog.Add(newLog);
                await db.SaveChangesAsync();

                var resp = await client.SendAsync(request);

                ret.Data = resp;
                newLog.Response = await resp.Content.ReadAsStringAsync();
                newLog.ResponseDt = DateTime.Now;
                newLog.ResponseStatus = resp.ReasonPhrase;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;

        }
        #region parameter
        public async Task<ReturnObject<List<ParameterData>>> ListParameter()
        {
            var ret = new ReturnObject<List<ParameterData>>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var data = await DB.Parameter
                    .Join(DB.Users, g => g.DataLogNavigation.UserId, u => u.Id, (g, u) => new { g, u })
                    .OrderByDescending(o => o.g.Year).Select(s => new ParameterData
                    {
                    Year = s.g.Year,
               
                        GDP = s.g.Gdp,
                        EstIncome = s.g.EstIncome,
                        ExportIncome = s.g.ExportIncome,
                        Interest = s.g.Interest,
                        DebtSettlement = s.g.DebtSettlement,
                        Log = new LogData
                    {
                        Action = s.g.DataLogNavigation.LogType,
                        ActionTime = s.g.DataLogNavigation.LogDt,
                        ActionType = s.g.DataLogNavigation.LogType,
                        UserID = s.g.DataLogNavigation.UserId,
                        UserName = s.u.TfirstName

                    }
                }).ToListAsync();
                ret.Data = data;
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;

        }
        public async Task<ReturnMessage> ModifyParameter(ParameterData d, bool isCreate, string userID)
        {
            var ret = new ReturnObject<List<ParameterData>>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                if (d.Year < 2500 || d.Year > 2660)
                {
                    ret.Message.Add(new MessageData
                    {
                        Code = "CurrYearDataRange",
                        MessageType = eMessageType.Error.ToString(),
                        Message = "ปีต้องอยู่ระหว่าง 2500 - 2660"
                    });
                    return ret;
                }
                var gdp = await DB.Parameter.Include(l => l.DataLogNavigation).Where(w => w.Year == d.Year).FirstOrDefaultAsync();
                if (isCreate)
                {
                    if (gdp != null)
                    {
                        ret.AddMessage(eMessage.DataIsAlreadyExist.ToString(), "Parameter", eMessageType.Error, new string[] { "Parameter" });
                        return ret;

                    }
                    gdp = new Parameter
                    {
                        Year = d.Year,

                    };
                    DB.Parameter.Add(gdp);
                }else
                {
                    if (gdp == null)
                    {
                        ret.AddMessage(eMessage.DataIsNotFound.ToString(), "Parameter", eMessageType.Error, new string[] { "Parameter" });
                        return ret;

                    }
                }
                gdp.Gdp = d.GDP;
                gdp.EstIncome = d.EstIncome;
                gdp.ExportIncome = d.ExportIncome;
                gdp.Interest = d.Interest;
                gdp.DebtSettlement = d.DebtSettlement;
                gdp.DataLogNavigation = new DataLog
                {
                    LogDt = DateTime.Now,
                    LogType = (isCreate) ? "C" : "U",
                    TableKey = (long)d.Year,
                    TableName = "Parameter",
                    UserId = userID
                };
                await DB.SaveChangesAsync();
                ret.IsCompleted = true;

            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        #endregion
        #region LOVExtend
        public async Task<ReturnList<LOVExtend>> GetLovExtendListByLOVGroup(string lovGroupCode)
        {
            var ret = new ReturnList<LOVExtend>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {

            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        public async Task<ReturnMessage> ModifyLovExtend(ParameterData d, bool isCreate, string userID)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {

            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        #endregion
        #region PDMORegulation
        public async Task<ReturnList<PDMORegulationData>> GetPDMORegulationList()
        {
            var ret = new ReturnList<PDMORegulationData>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var data = new List<PDMORegulationData>();
                var lovs = await DB.CeLov.Include(i => i.CeLovextend).Where(w => w.LovgroupCode == ServiceModels.Constants.LOVGroup.สถานะองค์กร._LOVGroupCode).OrderBy(o => o.OrderNo).ToListAsync();
                foreach (var lov in lovs)
                {

                    var rNew = new PDMORegulationData
                    {
                        LOVKey = lov.Lovkey,
                        REGCode = lov.Lovcode,
                        REGName = lov.Lovvalue,

                    };
                    data.Add(rNew);
                    var detail = lov.CeLovextend.Where(w => w.ExtendType == eLOVExtendType.PDMOLAWREGDT.ToString()).FirstOrDefault();
                    if (detail != null)
                    {
                        rNew.REGDetail = detail.ExtendValue;
                    }
                    var html = lov.CeLovextend.Where(w => w.ExtendType == eLOVExtendType.PDMOLAWREG.ToString()).FirstOrDefault();
                    if (html != null)
                    {
                        rNew.REGHtml = html.ExtendValue;
                    }
                }
                ret.Data = data;
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        public async Task<ReturnMessage> ModifyPDMORegulation(PDMORegulationData p, bool isCreate, string userID)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var lov = await DB.CeLov.Include(i => i.CeLovextend).Where(w => w.Lovkey == p.LOVKey).FirstOrDefaultAsync();
                if (lov == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "pdmoreg", eMessageType.Error, new string[] { _msglocalizer["PDMOREG"] });
                    return ret;
                }
                
                var detail = lov.CeLovextend.Where(w => w.ExtendType == eLOVExtendType.PDMOLAWREGDT.ToString()).FirstOrDefault();
                if (detail == null)
                {
                    detail = new CeLovextend
                    {
                        ExtendType = eLOVExtendType.PDMOLAWREGDT.ToString()
                    };
                    lov.CeLovextend.Add(detail);
                    
                }
                detail.ExtendValue = p.REGDetail;
                var html = lov.CeLovextend.Where(w => w.ExtendType == eLOVExtendType.PDMOLAWREG.ToString()).FirstOrDefault();
                if (html == null)
                {
                    html = new CeLovextend
                    {
                        ExtendType = eLOVExtendType.PDMOLAWREG.ToString()
                    };
                    lov.CeLovextend.Add(html);
                }
                html.ExtendValue = p.REGHtml;
                await DB.SaveChangesAsync();
                ret.IsCompleted = true;

            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }

        public async Task<ReturnMessage> SendEmail(MailMessage mail)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var email = new EmailConfig();
                _conf.GetSection("Email").Bind(email);
                // Credentials
                
                // Mail message
                
                mail.IsBodyHtml = true;
             
                // Smtp client
                //var client = new SmtpClient()
                //{
                //    Port = 587,
                //    DeliveryMethod = SmtpDeliveryMethod.Network,
                //    UseDefaultCredentials = false,
                //    Host = "smtp.gmail.com",
                //    EnableSsl = true,
                //    Credentials = credentials
                //};
                var client = new SmtpClient()
                {
                    Port = email.Port,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = email.UseDefaultCredentials,
                    Host = email.Host,
                    EnableSsl = email.EnableSsl,
                     
                };
                if (!string.IsNullOrEmpty(email.EmailUser) && !string.IsNullOrEmpty(email.EmailUser))
                {
                    var credentials = new NetworkCredential(email.EmailUser, email.EmailPassword);
                    client.Credentials = credentials;
                }
                client.Send(mail);
                ret.IsCompleted = true;
            }
            catch (System.Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }

        private async Task<ReturnObject<long?>> GetSurrogateKeyCommon(MOFContext db, string groupCode, string prefix)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var dbMain = db == null? DB : db;
                long? run;
                var surr = await dbMain.SurrogateKey.Where(w => w.GroupCode == groupCode && w.Prefix == prefix).FirstOrDefaultAsync();
                if (surr == null)
                {
                    run = 1;
                    surr = new SurrogateKey
                    {
                        GroupCode = groupCode,
                        Prefix = prefix,

                    };
                    dbMain.SurrogateKey.Add(surr);
                }
                else
                {
                    run = surr.Runno.GetValueOrDefault(0) + 1;
                   
                }
                surr.Runno = run;
                if (db == null)
                {
                    await dbMain.SaveChangesAsync();
                }
                
                ret.Data = run;
                ret.IsCompleted = true;

            }
            catch (System.Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }

        public async Task<ReturnObject<long?>> GetSurrogateKey(string groupCode, string prefix)
        {
            return await GetSurrogateKeyCommon(null, groupCode, prefix);
        }

        public async Task<ReturnObject<long?>> GetSurrogateKeyAsnyc(MOFContext db, string groupCode, string prefix)
        {
            return await GetSurrogateKeyCommon(db, groupCode, prefix);
        }
        #endregion
    }
}
