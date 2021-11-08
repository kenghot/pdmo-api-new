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
using mof.ServiceModels.Organization;
using mof.ServiceModels.Constants;
using mof.ServiceModels.Common.Generic;
using Newtonsoft.Json;
using mof.ServiceModels.Project;
using static mof.Services.Helper.Utilities;

namespace mof.Services
{
    public class ProjectRepository : IProject
    {
        public MOFContext DB;
        private UserManager<ServiceModels.Identity.ApplicationUser> _user;
        private SignInManager<ServiceModels.Identity.ApplicationUser> _signin;
        private Microsoft.AspNetCore.Identity.UI.Services.IEmailSender _email;
        private IStringLocalizer<MessageLocalizer> _msglocalizer;
        private ISystemHelper _helper;
        private ICommon _com;
        private IIIPMSync _iipm;
        public ProjectRepository(MOFContext db, UserManager<ServiceModels.Identity.ApplicationUser> userManager,
            SignInManager<ServiceModels.Identity.ApplicationUser> signInManager,
            Microsoft.AspNetCore.Identity.UI.Services.IEmailSender email,
            IStringLocalizer<MessageLocalizer> msglocalizer,
            ISystemHelper helper,
            ICommon com,
            IIIPMSync iipm
        )
        {
            DB = db;
            _user = userManager;
            _signin = signInManager;
            _email = email;
            _msglocalizer = msglocalizer;
            _helper = helper;
            _com = com;
            _iipm = iipm;
        }

        private    ReturnMessage ProjCheckList(Project p, List<BasicData> data , eProjectTOMany eMany)
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            foreach (var cb in data)
            {
            
                var chk = _helper.LOVCodeValidate(cb.Code, LOVGroup.สถานะโครงการ._LOVGroupCode, null);
                if (!chk.IsCompleted)
                {
                    ret.CloneMessage(chk.Message);
                    return ret;
                }
            
                p.ProjToMany.Add(new ProjToMany {
                    GroupCode = eMany.ToString(),
                    ManyId = chk.Data.LOVKey
                });
             }
            ret.IsCompleted = true;
            return ret;
        }
        private void saveProjectExpense(int periodValue, string AmountType, List<AmountData> amts, ICollection<ProjActAmount> paa, ProjAct pa)
        {
            var chk = _helper.LOVCodeValidate(AmountType, ServiceModels.Constants.LOVGroup.Project_Amount_Type._LOVGroupCode,null);
            if (!chk.IsCompleted)
            {
                throw new Exception("Error LOVGroup Project Amount Type code " + AmountType);
            }
       
            var paaFilter = paa.Where(w => w.AmountType == chk.Data.LOVKey && w.PeriodValue == periodValue).ToList();
            foreach (var tmp in paaFilter)
            {
                var find =   amts.Where(w => w.CurrencyCode == tmp.Currency && w.SourceType == tmp.SourceType).FirstOrDefault();
                if (find == null)
                {
                    DB.ProjActAmount.Remove(tmp);
                }
            }
            foreach (var amt in amts)
            {

                //var editpaa = paaFilter.Where(w => w.Currency == amt.CurrencyCode && w.SourceType == amt.SourceType).FirstOrDefault();
                ProjActAmount editpaa;
                if (amt.SourceLoan == null)
                {
                    editpaa = paaFilter.Where(w => w.Currency == amt.CurrencyCode && w.SourceType == amt.SourceType).FirstOrDefault();
                }else
                {
                    editpaa = paaFilter.Where(w => w.Currency == amt.CurrencyCode && w.SourceLoan == amt.SourceLoan.ID).FirstOrDefault();
                    var lov = DB.CeLov.Where(w => w.Lovkey == amt.SourceLoan.ID).FirstOrDefault();
                    if (lov != null)
                    {
                        if (!string.IsNullOrEmpty(lov.Remark) )
                        {
                            amt.SourceType = lov.Remark;
                        }
                    }
                    
                }
                    
                if (editpaa == null)
                {
                    editpaa = new ProjActAmount
                    {
                        AmountType = chk.Data.LOVKey,
                        Amount = amt.Amount,
                        Currency = amt.CurrencyCode,
                        PeriodType = "M",
                        PeriodValue =  periodValue,
                        
                        SourceLoan = amt.SourceLoan == null ? null : amt.SourceLoan.ID
                    };
                   
                    pa.ProjActAmount.Add(editpaa);
                }
                editpaa.SourceType = amt.SourceType;
                editpaa.Amount = amt.Amount;
            }
        }
        public async Task<ReturnObject<long?>> ModifyMin(ProjectModelMin p, bool isCreate, string userID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            ret.IsCompleted = false;
            try {
                ProjectModel proj = new ProjectModel();
           
                proj.Activities = new List<ActivityData>();
                proj.EIRR = 0;
                proj.EndDate = new DateTime(2099, 12, 31);
                proj.FIRR = 0;
                proj.LimitAmount = p.LimitAmount;
                proj.Materials = new List<MaterialSource>();
                proj.OrganizationID = p.OrganizationID;
                proj.PDMOAgreement = 1;
                proj.ProcessTime = "0";
                proj.ProjectBackground = "-";
                proj.ProjectCode = p.ProjectCode;
                proj.ProjectENName = p.ProjectENName;
                proj.ProjectID = p.ProjectID;
                proj.ProjectObjective = p.ProjectRemark;
                proj.ProjectScope = "-";
                proj.ProjectStatuses = new List<ProjectStatus>();
                proj.ProjectTarget = "-";
                proj.ProjectTHName = p.ProjectTHName;
                proj.ProjectType = p.ProjectType;
                proj.ResolutionAgreement = 1;
                proj.StartDate  = new DateTime(1973, 3, 28);
                proj.StartYear = p.StartYear;
                proj.LimitAmount = p.LimitAmount;
                proj.IsNewProject = true;
         
                ret = await Modify(proj, isCreate, userID);
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }

        private void saveProjectAmt( List<Amount> amts, Project pj)
        {
            //var chk = _helper.LOVCodeValidate(AmountType, ServiceModels.Constants.LOVGroup.Project_Amount_Type._LOVGroupCode, null);
            //if (!chk.IsCompleted)
            //{
            //    throw new Exception("Error LOVGroup Project Amount Type code " + AmountType);
            //}
            //var pjaFilter = pja.Where(w => w.Amount.AmountType == chk.Data.LOVKey).ToList(); //&& w.Amount.PeriodValue == periodValue && w.Amount.PeriodType == periodType).ToList();
            ICollection<ProjAmt> pja = pj.ProjAmt;
            var delAmts = new List<ProjAmt>();
            foreach (var tmp in pja)
            {
                var find = amts.Where(w => w.Currency == tmp.Amount.Currency && w.SourceType == tmp.Amount.SourceType 
                && w.PeriodType == tmp.Amount.PeriodType && w.PeriodValue == tmp.Amount.PeriodValue).FirstOrDefault();
                if (find == null)
                {
                    delAmts.Add(tmp);
                   
                }
            }
            foreach (var d in delAmts)
            {
                pj.ProjAmt.Remove(d);
            }
            foreach (var amt in amts)
            {

                var editpja = pja.Where(w => w.Amount.Currency == amt.Currency && w.Amount.SourceType == amt.SourceType
                 && w.Amount.PeriodType == amt.PeriodType && w.Amount.PeriodValue == amt.PeriodValue).FirstOrDefault();
                if (editpja == null)
                {
                    editpja = new ProjAmt
                    {

                        Amount = new DataModels.Models.Amount
                        {
                            AmountType = amt.AmountType,
                            Amount1 = amt.Amount1,
                            Currency = amt.Currency,
                            PeriodType = amt.PeriodType,
                            PeriodValue = amt.PeriodValue,
                            SourceType = amt.SourceType,
                        }
                    };
                    pj.ProjAmt.Add(editpja);
                }

                editpja.Amount.Amount1 = amt.Amount1;
            }
        }
        public void ModifyExtendData(string GroupCode,Project proj,List<ProjectExtendModel> exts)
        {
            var extIds = exts.Where(w => w.Id.HasValue).Select(s => s.Id.Value).ToList();

            var delexts = proj.ProjectExtend.Where(w => !extIds.Contains(w.Id) && w.GroupCode == GroupCode).ToList();
            DB.ProjectExtend.RemoveRange(delexts);


            foreach (var ext in exts)
            {
                var l = proj.ProjectExtend.Where(w => w.Id == ext.Id).FirstOrDefault();
                if (l == null)
                {
                    l = new ProjectExtend();
                    proj.ProjectExtend.Add(l);
                }
           
                l.ExtendData = ext.ExtendData;
                l.GroupCode = GroupCode;
 

            }
        }
        public async Task<ReturnObject<long?>> Modify(ProjectModel p, bool isCreate, string userID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                if (p.EndDate < p.StartDate)
                {
                    ret.AddMessage(eMessage.DateRangeError.ToString(), "date", eMessageType.Error);
                    return ret;
                }
                Project proj;
                // check proj code is already exist
                if (string.IsNullOrEmpty(p.ProjectCode))
                {
                    p.ProjectCode = DateTime.Now.Ticks.ToString();
                }
                if (isCreate)
                {
                    p.ProjectCode = $"temp_{DateTime.Now.Ticks}";
                    proj = await DB.Project.Where(w => w.ProjectCode == p.ProjectCode).FirstOrDefaultAsync();
                    if (proj != null)
                    {
                        ret.AddMessage(eMessage.DataIsAlreadyExist.ToString(), "already exist", eMessageType.Error, new string[] { "โครงการ" });
                        return ret;
                    }
                    proj = new Project();
                    DB.Project.Add(proj);
                }
                else
                {
                    proj = await DB.Project.Include(pt => pt.ProjectTypeNavigation).Include(pja => pja.ProjAmt).ThenInclude(amt => amt.Amount)
                        .Include(loc => loc.ProjectLocation)
                        .Include(Res => Res.ProjectResolution)
                        .Include(ext => ext.ProjectExtend)
                        .Where(w => w.ProjectId == p.ProjectID).FirstOrDefaultAsync();
                    if (proj == null)
                    {
                        ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { "โครงการ" });
                        return ret;
                    }
                    var o = await DB.Project.Where(w => w.ProjectCode == p.ProjectCode && w.ProjectId != p.ProjectID).FirstOrDefaultAsync();
                    if (o != null)
                    {
                        ret.AddMessage(eMessage.DataIsAlreadyExist.ToString(), "already exist", eMessageType.Error, new string[] { $"หมายเลขโครงการ ({p.ProjectCode}) " });
                        return ret;
                    }
                    if (await _com.IsProjectShortForm(p.ProjectType.Code))
                    {
                        if (! await _com.IsProjectShortForm(proj.ProjectTypeNavigation.Lovcode))
                        {
                            ret.Message.Add(new MessageData
                            {
                                Code = "ModifyProjMin",
                                Message = "แก้ไข สำหรับโครงการ กู้เพื่อชดเชยขาดดุล กับ กู้เพื่อชดเชย ขาดดุลเลื่อมปี เท่านั้น",
                                Language = "th",
                                MessageHint = "error",
                                MessageType = "error"
                            });
                            return ret;
                        }
                    }
                    var del = await DB.ProjToMany.Where(w => w.ProjectId == p.ProjectID).ToListAsync();

                    DB.ProjToMany.RemoveRange(del);
                }
                var rate = _helper.GetCurrencyRate(p.StartYear);
                if (!rate.IsCompleted)
                {
                    if ( ! await _com.IsProjectShortForm(p.ProjectType.Code))
                    {
                        ret.CloneMessage(rate.Message);
                        return ret;
                    }
               
                }
                // validate lov
                var chk = _helper.LOVCodeValidate(p.ProjectType.Code, LOVGroup.Project_Type._LOVGroupCode, null);
                if (!chk.IsCompleted)
                {
                    ret.CloneMessage(chk.Message);
                    return ret;
                }


                //end validate lov

                foreach (var pjt in p.ProjectStatuses)
                {
                    
                 
                    if (pjt.ResolutionCheck)
                    {
                        var pjtNew = new ProjToMany();
                        pjtNew.GroupCode = eProjectTOMany.RESOLUTION.ToString();
                        pjtNew.ManyId = pjt.ID;
                        proj.ProjToMany.Add(pjtNew);
                    }
                    if (pjt.PDMOCheck)
                    {
                        var pjtNew = new ProjToMany();
                        pjtNew.GroupCode = eProjectTOMany.PDMO.ToString();
                        pjtNew.ManyId = pjt.ID;
                        proj.ProjToMany.Add(pjtNew);
                    }
                }
                
                // Material
                proj.ProjectType = chk.Data.LOVKey;
                List<ProjMaterial> mats;
              
                if (isCreate)
                {
                    mats = new List<ProjMaterial>();
                }
                else
                {
                    mats = await DB.ProjMaterial.Where(w => w.ProjectId == p.ProjectID).ToListAsync();
                    DB.ProjMaterial.RemoveRange(mats);
                }
                foreach (var mat in p.Materials)
                {
                    var r = rate.Data.Currency.Where(w => w.CurrencyCode == mat.CurrencyCode).FirstOrDefault();
                    if (r == null)
                    {
                        ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { $"สกุลเงิน {mat.CurrencyCode}" });
                        return ret;
                    }
                    var pm = new ProjMaterial
                    {
                        CurrencyCode = mat.CurrencyCode,
                        LimitAmount = mat.LimitAmount,
                        SourceType = mat.SourceType
                    };
                    proj.ProjMaterial.Add(pm);
                }

                //end material
                #region Activities
                var allpa = await DB.ProjAct.Include(i => i.ProjActAmount).Where(w => w.ProjectId == p.ProjectID).ToListAsync();
                var delpa = new List<ProjAct>();
                if (!isCreate)
                {
                    foreach (var pa in allpa)
                    {
                        var del = p.Activities.Where(w => w.ProjActID == pa.ProjActId).FirstOrDefault();
                        if (del == null)
                        {
                            DB.ProjAct.Remove(pa);
                        }
                    }
                     
                }
                if (p.BuggetAllocationPlans != null)
                {
                    var projamt = new List<Amount>();
                    chk = _helper.LOVCodeValidate(ServiceModels.Constants.LOVGroup.Project_Amount_Type.แผนการใช้จ่ายเงิน, ServiceModels.Constants.LOVGroup.Project_Amount_Type._LOVGroupCode, null);
                    if (!chk.IsCompleted)
                    {
                        throw new Exception("Error LOVGroup Project Amount Type code " + ServiceModels.Constants.LOVGroup.Project_Amount_Type.แผนการใช้จ่ายเงิน);
                    }
                    foreach (var amt in p.BuggetAllocationPlans)
                    {
                     
                        foreach (var source in amt.BuggetAllocations)
                        {
                            var imonth = 1;
                           
                            foreach (var m in source.Months)
                            {
                                var pr = int.Parse(amt.Year.ToString() + imonth.ToString().PadLeft(2, '0'));
                               var a = new Amount
                                {
                                    PeriodType = "M",
                                    PeriodValue = pr,
                                    AmountType = chk.Data.LOVKey,
                                    SourceType = source.Source.SourceType,
                                    Currency = source.Source.Currency,
                                    Amount1 = m,


                                };
                                projamt.Add(a);
                                imonth += 1;
                            }
                        }
                       
                    }
                    saveProjectAmt(projamt, proj);
                }
                foreach (var act in p.Activities)
                {
                    ProjAct newpa;
              
                    newpa = allpa.Where(w => w.ProjActId == act.ProjActID).FirstOrDefault();
                    if (newpa == null)
                    {
                        newpa = new ProjAct();
                        proj.ProjAct.Add(newpa);
                    }
                    newpa.ActivityName = act.ActivityName;
       
                    saveProjectExpense(0,ServiceModels.Constants.LOVGroup.Project_Amount_Type.ค่าใช้จ่ายตามมติ, act.ResolutionAmounts, newpa.ProjActAmount, newpa);
                    saveProjectExpense(0,ServiceModels.Constants.LOVGroup.Project_Amount_Type.ค่าใช้จ่ายตามสัญญาจ้าง, act.ContractAmounts, newpa.ProjActAmount, newpa);
                    if (act.SaveProceedData != null)
                    {
                        foreach (var save in act.SaveProceedData)
                        {
                            var strPeriod = save.Year.ToString() + save.Month.ToString().PadLeft(2, '0');
                            var period = int.Parse(strPeriod);
                            saveProjectExpense(period, ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินงบประมาณ, save.Budget, newpa.ProjActAmount, newpa);
                            saveProjectExpense(period, ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินรายได้, save.Revernue, newpa.ProjActAmount, newpa);
                            saveProjectExpense(period, ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินกู้ลงนาม, save.SignedLoan, newpa.ProjActAmount, newpa);
                            saveProjectExpense(period, ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินกู้เบิกจ่าย, save.DisburseLoan, newpa.ProjActAmount, newpa);
                            saveProjectExpense(period, ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินจากแหล่งอื่นๆ, save.Other, newpa.ProjActAmount, newpa);
                        }
                    }
                    
                   
                }

                #endregion
                proj.IsNewProject = p.IsNewProject;
                proj.Eirr = p.EIRR;
                proj.EndDate = p.EndDate;
                proj.Firr = p.FIRR;
                proj.IsCanceled = false;
                proj.LimitAmount = p.LimitAmount;
                proj.ProjectBackground = p.ProjectBackground;

                proj.ProjectCode =  p.ProjectCode;
                proj.ProjectEnname = (p.ProjectENName == null) ? ""  : p.ProjectENName;
                proj.ProjectObjective = p.ProjectObjective;
                proj.ProjectScope = string.IsNullOrEmpty(p.ProjectScope) ? "" : p.ProjectScope;
                proj.ProjectTarget = p.ProjectTarget;
                proj.ProjectThname = p.ProjectTHName;
                proj.StartDate = p.StartDate;
                proj.CapitalSource = p.CapitalSource;
                proj.ProjectBranch = p.ProjectBranch;
                //if (p.StartYear ==123456)
                //{
                //    proj.StartYear = 123456;

                //}
                //else
                //{

                //    proj.StartYear = p.StartDate.Year + 543;
                //}
                proj.StartYear = p.StartYear; // p.StartDate.Year + 543;
                proj.OrganizationId = p.OrganizationID;
                proj.Pdmoagreement = p.PDMOAgreement;
                proj.ResolutionAgreement = p.ResolutionAgreement;
                #region iipm
                var locsId = p.Locations.Where(w => w.Id.HasValue).Select(s => s.Id.Value).ToList();

                var delloc = proj.ProjectLocation.Where(w => !locsId.Contains(w.Id)).ToList();
                DB.ProjectLocation.RemoveRange(delloc);
                
            
                foreach (var loc in p.Locations)
                {
                    var l = proj.ProjectLocation.Where(w => w.Id == loc.Id).FirstOrDefault();
                    if (l == null)
                    {
                        l = new DataModels.Models.ProjectLocation();
                        proj.ProjectLocation.Add(l);
                    }
                    l.Latitude = loc.Latitude;
                    l.Location = loc.Location;
                    l.Longitude = loc.Longitude;

                }
                var resIds = p.Resolutions.Where(w => w.Id.HasValue).Select(s => s.Id.Value).ToList();

                var delres = proj.ProjectResolution.Where(w => !resIds.Contains(w.Id)).ToList();
                DB.ProjectResolution.RemoveRange(delres);


                foreach (var res in p.Resolutions)
                {
                    var l = proj.ProjectResolution.Where(w => w.Id == res.Id).FirstOrDefault();
                    if (l == null)
                    {
                        l = new ProjectResolution();
                        proj.ProjectResolution.Add(l);
                    }
                    l.Date = res.Date;
                    l.Detail = res.Detail;
                    l.Amount = res.Amount;
                    if (res.File != null)
                    {
                        AttachFile att = await DB.AttachFile.Where(w => w.AttachFileId == res.File.ID).FirstOrDefaultAsync();
                        if (res.File.ClearFile && att != null)
                        {
                            l.FileId = null;
                            DB.AttachFile.Remove(att);
                        }else
                        {
                            if (att == null)
                            {
                                att = new AttachFile
                                {
                                    FileDetail = res.File.FileDetail,
                                    FileExtension = res.File.FileExtension,
                                    FileName = res.File.FileName,
                                    FileSize = res.File.FileSize
                                };
                                l.File = att;
                            }
                            att.FileDetail = res.File.FileDetail;
                            att.FileExtension = res.File.FileExtension;
                            att.FileName = res.File.FileName; ;
                            att.FileSize = res.File.FileSize;
                            if (res.File.FileData != null)
                            {
                                att.FileData = res.File.FileData;
                            }
                            
                        }


                    }

                }
                #region Extend data 
                ModifyExtendData(eProjectExtendGroup.BENEFIT.ToString(), proj, p.Benefits);
                ModifyExtendData(eProjectExtendGroup.OBJECTIVE.ToString(), proj, p.Objectives);
                ModifyExtendData(eProjectExtendGroup.PRODUCTIVITY.ToString(), proj, p.Productivities);
                ModifyExtendData(eProjectExtendGroup.SCOPE.ToString(), proj, p.Scopes);
                #endregion
                proj.IsGovBurden = p.IsGovBurden;
                proj.IsOnGoing = p.IsOnGoing;
                proj.HasEld = p.HasEld;
                proj.ProvinceId = p.Province?.ID;
                proj.SectorId = p.Sector?.ID;
                proj.StatusId = p.Status?.ID;
                proj.CreditChannelId = p.CreditChannel?.ID;
                proj.DirectorMail = p.DirectorMail;
                proj.DirectorName = p.DirectorName;
                proj.DirectorPosition = p.DirectorPosition;
                proj.DirectorTel = p.DirectorTel;
                proj.MapDrawing = p.MapDrawing;
                #endregion
                // add log

                var log = new DataLog
                {
                    LogDt = DateTime.Now,
                    LogType = (isCreate) ? "C" : "U",
                    TableName = "Project",
                    TableKey = proj.ProjectId,
                    UserId = userID

                };
                proj.DataLogNavigation = log;
   
                
                
                #region prepare data for IIPM
                p.Organization = await DB.Organization.Where(w => w.OrganizationId == p.OrganizationID)
                                   .Select(s => new BasicData
                                   {
                                       Code = s.OrganizationCode,
                                       Description = s.OrganizationThname,
                                       ID = s.OrganizationId
                                   }).FirstOrDefaultAsync();
                if (p.Status != null)
                {
                    p.Status = await DB.CeLov.Where(w => w.Lovkey == p.Status.ID).Select(s => new BasicData
                    {
                        ID = s.Lovkey,
                        Code = s.Lovcode,
                        Description = s.Lovvalue
                    }).FirstOrDefaultAsync();
                }
                if (p.CreditChannel != null)
                {
                    p.CreditChannel = await DB.CeLov.Where(w => w.Lovkey == p.CreditChannel.ID).Select(s => new BasicData
                    {
                        ID = s.Lovkey,
                        Code = s.Lovcode,
                        Description = s.Lovvalue
                    }).FirstOrDefaultAsync();
                }
                if (p.Province != null)
                {
                    p.Province = await DB.CeLov.Where(w => w.Lovkey == p.Province.ID).Select(s => new BasicData
                    {
                        ID = s.Lovkey,
                        Code = s.Lovcode,
                        Description = s.Lovvalue
                    }).FirstOrDefaultAsync();
                }
                //mapping from lov remark
                if (p.ProjectType != null)
                {
                    p.ProjectType = await DB.CeLov.Where(w => w.Lovkey == p.ProjectType.ID).Select(s => new BasicData
                    {
                        ID = s.Lovkey,
                        Code = s.Remark,
                        Description = s.Lovvalue
                    }).FirstOrDefaultAsync();
                }
                //mapping from lov remark
                if (p.Sector != null)
                {
                    p.Sector = await DB.CeLov.Where(w => w.Lovkey == p.Sector.ID).Select(s => new BasicData
                    {
                        ID = s.Lovkey,
                        Code = s.Remark,
                        Description = s.Lovvalue
                    }).FirstOrDefaultAsync();
                }
                #endregion
                if (isCreate)
                {
                    //TODO project type for generate project code 
                    var prefix = $"{p.Organization.Code}_{p.StartYear.ToString().Substring(2, 2)}{p.ProjectType.Code.Substring(0,1)}";
                    var pjcode = await _helper.GetSurrogateKeyAsnyc(DB ,"ProjectCode", prefix);
                    proj.ProjectCode = $"{prefix}{pjcode.Data.Value.ToString().PadLeft(3,'0')}";
                    p.ProjectCode = proj.ProjectCode;
                   
                }
                var iipm = await _iipm.ModifyProject(p, isCreate);
                if (!iipm.IsCompleted)
                {
                    ret.CloneMessage(iipm.Message);
                    return ret;
                }
                await DB.SaveChangesAsync();
                if (isCreate)
                {
                    log.TableKey = proj.ProjectId;
                    await DB.SaveChangesAsync();
                }
                ret.IsCompleted = true;
                ret.Data = proj.ProjectId;
               
                
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }

        public async Task<ReturnObject<ProjectModel>> Get(long ProjID)
        {
            var ret = new ReturnObject<ProjectModel>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var proj =  DB.Project
                    .Include(pt => pt.ProjectTypeNavigation)
                    .Include(o => o.Organization)
                    .Include(prv => prv.Province)
                    .Include(sec => sec.Sector)
                    .Include(st => st.Status)
                    .Include(cd => cd.CreditChannel)
                    .Where(w => w.ProjectId == ProjID);
                if (  proj.Count() == 0)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { "โครงการ" });
                    return ret;
                }
                return await (Get(proj));
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        private void storeProjecExpenseSummary(ExpenseSummary ex, AmountData amtdata)
        {
            if (amtdata.SourceType == "L")
            {
                ex.LocalLoan += amtdata.Amount;
            }
            else if (amtdata.SourceType == "F")
            {
                ex.ForeignLoan += amtdata.Amount;
            }
            else if (amtdata.SourceType == "O")
            {
                ex.OtherLoan += amtdata.Amount;
            }
            if (amtdata.SourceType != "O")
            {
                ex.TotalLoan += amtdata.Amount;
            } 
            ex.GrandTotal += amtdata.Amount;
            if (amtdata.SourceLoan != null && amtdata.Amount > 0)
            {
                var sl = ex.SumBySources.Where(w => w.SourceLoan.ID == amtdata.SourceLoan.ID).FirstOrDefault();
                if (sl == null)
                {
                    sl = new SourceOfLoanAmount
                    {
                        Amounts = new List<AmountData>(),
                        SourceLoan = new BasicData
                        {
                            Code = amtdata.SourceLoan.Code,
                            Description = amtdata.SourceLoan.Description,
                            ID = amtdata.SourceLoan.ID
                            
                        }
                    };
                    ex.SumBySources.Add(sl);
                }
                var amt = sl.Amounts.Where(w => w.CurrencyCode == amtdata.CurrencyCode).FirstOrDefault();
                if (amt == null)
                {
                    amt = new AmountData
                    {
                        
                        CurrencyCode = amtdata.CurrencyCode,
                        SourceType = amtdata.SourceType,

                    };
                    sl.Amounts.Add(amt);
                }
                amt.Amount += amtdata.Amount;
                amt.THBAmount += amtdata.THBAmount;
            }
 
        }
        public async Task<ReturnObject<ProjectModel>> Get(IQueryable<Project> p)
        {
            return await Get(p, false);
        }
        private void StoreProjectExtend(List<ProjectExtendModel> exts , ProjectExtendModel data)
        {
            exts.Add(new ProjectExtendModel
            {
                Id = data.Id,
                ExtendData = data.ExtendData,
                GroupCode = data.GroupCode,
                No = data.No
            });
        }
        public async Task<ReturnObject<ProjectModel>> Get(IQueryable<Project> p, bool notCheckRate)
        {
            var ret = new ReturnObject<ProjectModel>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var data = new ProjectModel();
       
                data = await p.Select(s => new ProjectModel {
                    EIRR = s.Eirr,
                    EndDate = s.EndDate,
                    FIRR = s.Firr,
                    IsNewProject = s.IsNewProject,
                    LimitAmount = s.LimitAmount,
                    OrganizationID = s.OrganizationId,
                    Organization = s.Organization == null ? null : new BasicData
                    {
                        Code = s.Organization.OrganizationCode,
                        ID = s.Organization.OrganizationId,
                        Description = s.Organization.OrganizationThname
                    },
                    PDMOAgreement = s.Pdmoagreement,
                    ProjectBackground = s.ProjectBackground,
                    ProjectCode = s.ProjectCode,
                    ProjectENName = s.ProjectEnname,
                    ProjectID = s.ProjectId,
                    ProjectObjective = s.ProjectObjective,
                    ProjectScope = s.ProjectScope,
                    ProjectTarget = s.ProjectTarget,
                    ProjectRemark = s.ProjectObjective,
                    ProjectTHName = s.ProjectThname,

                    ProjectType = new BasicData { Code = s.ProjectTypeNavigation.Lovcode, ID = s.ProjectTypeNavigation.Lovkey , Description = s.ProjectTypeNavigation.Lovvalue},
                    ResolutionAgreement = s.ResolutionAgreement,
                    StartDate = s.StartDate,
                    StartYear = s.StartYear,
                    LastUpdate = new LogData {  ID = s.DataLog},
                    CapitalSource = s.CapitalSource,
                    ProjectBranch = s.ProjectBranch,
                    Locations = s.ProjectLocation.Select(loc => new ServiceModels.Project.ProjectLocationModel
                    {
                        Id = loc.Id,
                        Latitude = loc.Latitude,
                        Longitude = loc.Longitude,
                        Location = loc.Location
                    }).ToList(),
                    Resolutions = s.ProjectResolution.Select(res => new ProjectResolutionModel
                    {
                        Id = res.Id,
                        Amount = res.Amount,
                        Date = res.Date,
                        Detail = res.Detail,
                        File = !res.FileId.HasValue ? null :  new AttachFileData {
                            FileDetail = res.File.FileDetail,
                            FileExtension = res.File.FileExtension,
                            FileName = res.File.FileName,
                            FileSize = res.File.FileSize,
                            ID = res.File.AttachFileId
                        }
                    }).ToList(),
                    ExtendDatas = s.ProjectExtend.Select(ext => new ProjectExtendModel
                    {
                        Id = ext.Id,
                        ExtendData = ext.ExtendData,
                        GroupCode = ext.GroupCode
                    }).ToList(),
                    Province = s.Province == null ? null : new BasicData
                    {
                        ID = s.Province.Lovkey,
                        Code = s.Province.Lovcode,
                        Description = s.Province.Lovvalue
                    },
                    Sector = s.Sector == null ? null : new BasicData
                    {
                        ID = s.Sector.Lovkey,
                        Code = s.Sector.Lovcode,
                        Description = s.Sector.Lovvalue
                    },
                    Status = s.Status == null ? null : new BasicData
                    {
                        ID = s.Status.Lovkey,
                        Code = s.Status.Lovcode,
                        Description = s.Status.Lovvalue
                    },
                    CreditChannel = s.CreditChannel == null ? null : new BasicData
                    {
                        ID = s.CreditChannel.Lovkey,
                        Code = s.CreditChannel.Lovcode,
                        Description = s.CreditChannel.Lovvalue
                    },
                    IsGovBurden = s.IsGovBurden,
                    IsOnGoing = s.IsOnGoing,
                    HasEld = s.HasEld,
                    DirectorMail = s.DirectorMail,
                    DirectorName = s.DirectorName,
                    DirectorTel = s.DirectorTel,
                    DirectorPosition = s.DirectorPosition,
                    MapDrawing = s.MapDrawing
                }).FirstOrDefaultAsync();
                var rate = _helper.GetCurrencyRate(data.StartYear);
                if (!rate.IsCompleted)
                {
                    if (!await _com.IsProjectShortForm(data.ProjectType.Code) && !notCheckRate)
                    {
                        ret.CloneMessage(rate.Message);
                        return ret;
                    }
                    
                }
                if (rate.Data == null)
                {
                    rate = _helper.GetCurrencyRate(DateTime.Now.Year + 543);
                }

                data.ProcessTime = Helper.Utilities.YMDDiffString(data.StartDate, data.EndDate); // string.Format("{0} ปี ({1:##,##} วัน)", years, days);
                //check list
       
                var pjMany = await DB.ProjToMany.Where(w => w.ProjectId == data.ProjectID).ToListAsync();
                data.ProjectStatuses = await DB.CeLov.Where(w => w.LovgroupCode == ServiceModels.Constants.LOVGroup.สถานะโครงการ._LOVGroupCode).OrderBy(o => o.OrderNo)
                    .Select(s => new ProjectStatus
                    {
                        ID = s.Lovkey,
                        Code = s.Lovcode,
                        Description = s.Lovvalue,
                        PDMOCheck = false ,
                        ResolutionCheck = false ,
                    }).ToListAsync();
                #region extend data
                foreach (var ext in data.ExtendDatas)
                {
                    if (ext.GroupCode == eProjectExtendGroup.BENEFIT.ToString())
                    {
                        StoreProjectExtend(data.Benefits, ext);
                    }
                    if (ext.GroupCode == eProjectExtendGroup.OBJECTIVE.ToString())
                    {
                        StoreProjectExtend(data.Objectives, ext);
                    }
                    if (ext.GroupCode == eProjectExtendGroup.PRODUCTIVITY.ToString())
                    {
                        StoreProjectExtend(data.Productivities, ext);
                    }
                    if (ext.GroupCode == eProjectExtendGroup.SCOPE.ToString())
                    {
                        StoreProjectExtend(data.Scopes, ext);
                    }

                }
                data.ExtendDatas = null;
                #endregion
                foreach (var mn in pjMany)
                {
                    var check = data.ProjectStatuses.Where(w => w.ID == mn.ManyId).FirstOrDefault();
                    if (check != null)
                    {
                        if (mn.GroupCode == eProjectTOMany.PDMO.ToString())
                        {
                            check.PDMOCheck = true;
                        }
                        if (mn.GroupCode == eProjectTOMany.RESOLUTION.ToString())
                        {
                            check.ResolutionCheck = true;
                        }
                    }
                }
                //Material
                var mats = await DB.ProjMaterial.Where(w => w.ProjectId == data.ProjectID).ToListAsync();
                data.Materials = new List<MaterialSource>();
                decimal totBaht = 0;
                foreach (var mat in mats)
                {
                    var r = rate.Data.Currency.Where(w => w.CurrencyCode == mat.CurrencyCode).FirstOrDefault();
                    if (r == null)
                    {
                        ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { $"อัตราแลกเปลี่ยน {mat.CurrencyCode}" });
                        return ret;
                    }
                    var ms = new MaterialSource
                    {
                        CurrencyCode = mat.CurrencyCode,
                        LimitAmount = mat.LimitAmount,
                        THBAmount = mat.LimitAmount * r.CurrencyRate,
                        SourceType = mat.SourceType
                    };
                    totBaht += ms.THBAmount;
                    data.Materials.Add(ms);

                }
                foreach (var mat in data.Materials)
                {
                  
                    mat.Ratio = (totBaht > 0) ?(mat.THBAmount / totBaht) * 100 : 0;
                }
                //end material
                #region แผนการใช้จ่ายเงิน
                var pjas = await  DB.ProjAmt
                    .Include(amt => amt.Amount).ThenInclude(l => l.AmountTypeNavigation)
                    .Where(w => w.ProjectId == data.ProjectID).ToListAsync();
                data.BuggetAllocationPlans = new List<BuggetAllocationYearPlan>();
                foreach (var pja in pjas)
                {
                    
                    if (pja.Amount.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.แผนการใช้จ่ายเงิน)
                    {

                        var period = new Utilities.PeriodObject(pja.Amount.PeriodValue);
                        var r = rate.Data.Currency.Where(w => w.CurrencyCode == pja.Amount.Currency).FirstOrDefault();
                        if (r == null)
                        {
                            ret.AddMessage(eMessage.RateOfYearIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { pja.Amount.Currency, data.StartYear.ToString() });
                            return ret;
                        }

                        var bg = data.BuggetAllocationPlans.Where(w => w.Year == period.Year).FirstOrDefault();

                        if (bg == null)
                        {
                            bg = new BuggetAllocationYearPlan
                            {
                                BuggetAllocations = new List<BuggetAllocation>(),
                                Year = period.Year
                            };
                            data.BuggetAllocationPlans.Add(bg);
                        }
                        var source = bg.BuggetAllocations.Where(w => w.Source.Currency == pja.Amount.Currency &&
                        w.Source.SourceType == pja.Amount.SourceType).FirstOrDefault();
                        if (source == null)
                        {
                            source = new BuggetAllocation
                            {
                                Months = new List<decimal> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                Source = new ServiceModels.Plan.LoanSource
                                {
                                    Currency = pja.Amount.Currency,
                                    LoanSourceID = pja.Amount.AmountId,
                                    SourceType = pja.Amount.SourceType
                                }

                            };
                            bg.BuggetAllocations.Add(source);
                        }
                        source.Source.LoanAmount += pja.Amount.Amount1;
                        source.Source.THBAmount += pja.Amount.Amount1 * r.CurrencyRate;
                        try
                        {
                            source.Months[period.Month - 1] = pja.Amount.Amount1;
                        }
                        catch (Exception)
                        {

                            throw new Exception("โปรดตรวจสอบ  BuggetAllocation.Months ProjAmtID = " + pja.ProjAmtId.ToString());
                        }
                        
                       
                    }
                }
                #endregion
                //expense 
                #region expense
                var pas = await DB.ProjAct.Include(pa => pa.ProjActAmount)
                    .Include(amt => amt.ProjActAmount).ThenInclude(lov => lov.SourceLoanNavigation)
                    .Include("ProjActAmount.AmountTypeNavigation").Where(w => w.ProjectId == data.ProjectID).ToListAsync();
                data.Activities = new List<ActivityData>();
                data.ResolutionExpSum = new ExpenseSummary();
                data.ContractExpSum = new ExpenseSummary();
                foreach (var pa in pas)
                {
                    var act = new ActivityData
                    {
                        ActivityName = pa.ActivityName,
                        ProjActID = pa.ProjActId,
                        ContractAmounts = new List<AmountData>(),
                        ResolutionAmounts = new List<AmountData>(),
                        SaveProceedData = new List<SaveProceed>(),
                        Years = new List<ProceedByYear>(),
                        TotalProceedByActivity = new ProceedData(),
                    };
                    act.TotalProceedByActivity.Detail = pa.ActivityName;
        
                    data.Activities.Add(act);
                    
                    foreach (var paa in pa.ProjActAmount)
                    {
                        var r = rate.Data.Currency.Where(w => w.CurrencyCode == paa.Currency).FirstOrDefault();
                        if (r == null)
                        {
                            ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { $"อัตราแลกเปลี่ยน {paa.Currency} ปี {data.StartYear}" });
                            return ret;
                        }
                        var newamt = new AmountData
                        {
                            Amount = paa.Amount,
                            CurrencyCode = paa.Currency,
                            SourceType = paa.SourceType,
                            THBAmount = paa.Amount * r.CurrencyRate,
                            SourceLoan = paa.SourceLoanNavigation == null ? null : new BasicData { 
                                Code = paa.SourceLoanNavigation.Lovcode,
                                Description = paa.SourceLoanNavigation.Lovvalue,
                                ID = paa.SourceLoanNavigation.Lovkey
                            }

                        };
                        if (paa.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.ค่าใช้จ่ายตามมติ)
                        {
                            act.ResolutionAmounts.Add(newamt);
                            act.TotalResolution += newamt.THBAmount;
                            storeProjecExpenseSummary(data.ResolutionExpSum, newamt);
                        }
                        else if (paa.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.ค่าใช้จ่ายตามสัญญาจ้าง)
                        {
                            act.ContractAmounts.Add(newamt);
                            act.TotalContract += newamt.THBAmount;
                            storeProjecExpenseSummary(data.ContractExpSum, newamt);
                        }else
                        {
                            var period = new PeriodObject(paa.PeriodValue);
                            List<AmountData> amt = new List<AmountData>();
                            var totYear = act.Years.Where(w => w.Year.Detail == period.StringYear).FirstOrDefault();
                            if (totYear == null)
                            {
                                totYear = new ProceedByYear {
                                    Year = new ProceedData { Detail = period.StringYear },
                                    Months = new List<ProceedData>()
                                };

                                act.Years.Add(totYear);
                            }
                            var totMonth = totYear.Months.Where(w => w.Detail == period.StringMonth).FirstOrDefault();
                            if (totMonth == null)
                            {
                                totMonth = new ProceedData
                                {
                                    Detail = period.StringMonth
                                 
                                };
                                totYear.Months.Add(totMonth);
                            }
                            var proceed = act.SaveProceedData.Where(w => w.Month == period.Month && w.Year == period.Year).FirstOrDefault();
                            if (proceed == null)
                            {
                                proceed = new SaveProceed
                                {
                                    Revernue = new List<AmountData>(),
                                    Budget = new List<AmountData>(),
                                    SignedLoan = new List<AmountData>(),
                                    DisburseLoan = new List<AmountData>(),
                                    Other = new List<AmountData>()
                                };
                                proceed.Year = period.Year;
                                proceed.Month = period.Month;
                                act.SaveProceedData.Add(proceed);
                            }

                            act.TotalProceedByActivity.Total += newamt.THBAmount;
                            totYear.Year.Total += newamt.THBAmount;
                            totMonth.Total += newamt.THBAmount;
                            if (paa.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินรายได้)
                            {
                                amt = proceed.Revernue;
                                act.TotalProceedByActivity.Revernue += newamt.THBAmount;
                                totYear.Year.Revernue += newamt.THBAmount;
                                totMonth.Revernue += newamt.THBAmount;
                            }
                            if (paa.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินงบประมาณ)
                            {
                                amt = proceed.Budget;
                                act.TotalProceedByActivity.Budget += newamt.THBAmount;
                                totYear.Year.Budget += newamt.THBAmount;
                                totMonth.Budget += newamt.THBAmount;
                            }
                            if (paa.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินกู้ลงนาม)
                            {
                                amt = proceed.SignedLoan;
                                act.TotalProceedByActivity.SignedLoan += newamt.THBAmount;
                                totYear.Year.SignedLoan += newamt.THBAmount;
                                totMonth.SignedLoan += newamt.THBAmount;
                            }
                            if (paa.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินกู้เบิกจ่าย)
                            {
                                amt = proceed.DisburseLoan;
                                act.TotalProceedByActivity.DisburseLoan += newamt.THBAmount;
                                totYear.Year.DisburseLoan += newamt.THBAmount;
                                totMonth.DisburseLoan += newamt.THBAmount;
                            }
                            if (paa.AmountTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Project_Amount_Type.เงินจากแหล่งอื่นๆ)
                            {
                                amt = proceed.Other;
                                act.TotalProceedByActivity.Other += newamt.THBAmount;
                                totYear.Year.Other += newamt.THBAmount;
                                totMonth.Other += newamt.THBAmount;
                            }
                            amt.Add(newamt);
                        }
                    }
                }
                #endregion
                foreach (var act in data.Activities)
                {
                    act.ResolutionAmounts = InitialSourceLoanAmount(act.ResolutionAmounts);
                }
                
                //end expense
                data.CurrencyData = rate.Data;

                var log = await  _helper.GetDataLog(data.LastUpdate.ID.Value);
                data.LastUpdate = log.Data;
                ret.IsCompleted = true;
                ret.Data = data;
                
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        private void CopyLOVToBasicData(LOV lov, BasicData b)
        {
            b.Code = lov.LOVCode;
            b.ID = lov.LOVKey;
            b.Description = lov.LOVValue;
        }
        public List<AmountData> InitialSourceLoanAmount(List<AmountData> p)
        {
            var ret = new List<AmountData>();
            var sls = _helper.GetLOVByGroup(ServiceModels.Constants.LOVGroup.Source_of_Loan._LOVGroupCode);
            var lovL = sls.Data.Where(w => w.LOVCode == ServiceModels.Constants.LOVGroup.Source_of_Loan.แหล่งเงินกู้ในประเทศ).FirstOrDefault();
            var lovF = sls.Data.Where(w => w.LOVCode == ServiceModels.Constants.LOVGroup.Source_of_Loan.แหล่งเงินกู้ต่างประเทศ).FirstOrDefault();
            var lovO = sls.Data.Where(w => w.LOVCode == ServiceModels.Constants.LOVGroup.Source_of_Loan.อื่นๆ).FirstOrDefault();
            p.RemoveAll(w => w.Amount == 0);
            foreach (var tmp in p)
            {
                if (tmp.SourceLoan == null)
                {
                    tmp.SourceLoan = new BasicData();
                    if (tmp.SourceType == "L")
                    {
                        CopyLOVToBasicData(lovL, tmp.SourceLoan);
                    }
                    else if (tmp.SourceType == "F")
                    {
                        CopyLOVToBasicData(lovF, tmp.SourceLoan);
                    }
                    else
                    {
                        CopyLOVToBasicData(lovO, tmp.SourceLoan);
                    }
                }
            }
            foreach (var sl in sls.Data)
            {
                var amts = p.Where(w => w.SourceLoan.ID == sl.LOVKey).ToList();
                if (amts.Count > 0)
                {
                    foreach (var amt in amts)
                    {
                        ret.Add(amt);
                    }
                }else
                {
                    var newAmt = new AmountData
                    {
                        SourceType = sl.Remark,
                        Amount = 0,
                        CurrencyCode = sl.LOVCode == ServiceModels.Constants.LOVGroup.Source_of_Loan.แหล่งเงินกู้ต่างประเทศ ? "USD" : "THB",
                        THBAmount = 0,
                        SourceLoan = new BasicData()
                    };
                    CopyLOVToBasicData(sl, newAmt.SourceLoan);
                    ret.Add(newAmt);
                }

     
            }
            return ret;
        }
        public async Task<ReturnList<ProjectModel>> List(ProjectListParameter p)
        {
            var ret = new ReturnList<ProjectModel>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                IQueryable<Project> tmp;
                tmp = DB.Project
                .WhereIf(!string.IsNullOrEmpty(p.ProjectType), pj => pj.ProjectTypeNavigation.Lovcode == p.ProjectType)
                .WhereIf(p.OrganizationID.HasValue, w => w.OrganizationId == p.OrganizationID)
                .WhereIf(p.StartYear.HasValue, y => y.StartYear == p.StartYear)
                .WhereIf(!string.IsNullOrEmpty(p.Paging.SearchText), t => t.ProjectEnname.Contains(p.Paging.SearchText) ||
               t.ProjectThname.Contains(p.Paging.SearchText));
     


                ret.TotalRow = await tmp.Select(s => s.ProjectId).CountAsync();
                ret.PageNo = p.Paging.PageNo;
                ret.PageSize = p.Paging.PageSize;
                //todo Planstatus from proposal
                var sel = tmp.PageBy(b => b.ProjectCode, p.Paging.PageNo, p.Paging.PageSize)
                     .Join(DB.Users, pj => pj.DataLogNavigation.UserId, u => u.Id, (pj, u) => new { pj, u })
                             .Select(s => new ProjectModel
                             {
                                 LastUpdate = new LogData
                                 {
                                     Action = s.pj.DataLogNavigation.LogType,
                                     ActionTime = s.pj.DataLogNavigation.LogDt,
                                     ActionType = "Data",
                                     ID = s.pj.DataLogNavigation.LogId,
                                     UserID = s.pj.DataLogNavigation.UserId,
                                     UserName = s.u.TfirstName + " " + s.u.TlastName
                                 },
                                 Organization = new BasicData
                                 {
                                     Code = s.pj.Organization.OrganizationCode,
                                     Description = s.pj.Organization.OrganizationThname,
                                     ID = s.pj.OrganizationId
                                 },
                                 EIRR = s.pj.Eirr,
                                 EndDate = s.pj.EndDate,
                                 FIRR = s.pj.Firr,
                                 LimitAmount = s.pj.LimitAmount,
                                 OrganizationID = s.pj.OrganizationId,
                                 PDMOAgreement = s.pj.Pdmoagreement,
                                 ProjectCode = s.pj.ProjectCode,
                                 ProjectID = s.pj.ProjectId,
                                 ProjectENName = s.pj.ProjectEnname,
                                 ProjectTHName = s.pj.ProjectThname,
                                 ProjectType = new BasicData {
                                     Code = s.pj.ProjectTypeNavigation.Lovcode ,
                                     Description = s.pj.ProjectTypeNavigation.Lovvalue,
                                     ID = s.pj.ProjectTypeNavigation.Lovkey 
                                 },
                                 ResolutionAgreement = s.pj.ResolutionAgreement,
                                 StartDate = s.pj.StartDate,
                                 StartYear = s.pj.StartYear

                             }
                             );
                var projs = await sel.ToListAsync();
                foreach (var pj in projs)
                {
                    var pjdata = await Get(pj.ProjectID.Value);
                    if (!pjdata.IsCompleted)
                    {
                        ret.CloneMessage(pjdata.Message);
                        return ret;
                    }
                    pj.LimitAmount = (pjdata.Data?.ResolutionExpSum?.GrandTotal).Value;
                     
                }
             

                ret.Data = projs;
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }

        public async Task<ReturnObject<List<ProjectStatus>>> GetProjectStatusList() {
            var ret = new ReturnObject<List<ProjectStatus>>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                ret.Data = await DB.CeLov.Where(w => w.LovgroupCode == ServiceModels.Constants.LOVGroup.สถานะโครงการ._LOVGroupCode).OrderBy(o => o.OrderNo)
                   .Select(s => new ProjectStatus
                   {
                       ID = s.Lovkey,
                       Code = s.Lovcode,
                       Description = s.Lovvalue,
                       PDMOCheck = false,
                       ResolutionCheck = false,
                   }).ToListAsync();
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
