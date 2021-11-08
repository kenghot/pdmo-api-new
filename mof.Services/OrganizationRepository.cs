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

namespace mof.Services
{
    public class OrganizationRepository : mof.IServices.IOrganization
    {
        public enum eORGToManyGroup
        {
            LawOFDebt,
            AttchFile,

        }
        public MOFContext DB;
        private UserManager<ServiceModels.Identity.ApplicationUser> _user;
        private SignInManager<ServiceModels.Identity.ApplicationUser> _signin;
        private Microsoft.AspNetCore.Identity.UI.Services.IEmailSender _email;
        private IStringLocalizer<MessageLocalizer> _msglocalizer;
        private ISystemHelper _helper;
        public OrganizationRepository(MOFContext db, UserManager<ServiceModels.Identity.ApplicationUser> userManager,
            SignInManager<ServiceModels.Identity.ApplicationUser> signInManager,
            Microsoft.AspNetCore.Identity.UI.Services.IEmailSender email,
            IStringLocalizer<MessageLocalizer> msglocalizer,
            ISystemHelper helper
)
        {
            DB = db;
            _user = userManager;
            _signin = signInManager;
            _email = email;
            _msglocalizer = msglocalizer;
            _helper = helper;
    
        }
        public async Task<ReturnMessage> OrganizationAttachFile(MOFContext db, long? OrgID, List<AttachFileData> afs, string GroupCode, bool IsChangeRequest,bool isCreate )
        {
            var ret = new ReturnMessage(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                //clear flie
                if (!IsChangeRequest && !isCreate)
                {
                    var del = await db.OrgtoMany.Where(w => w.GroupCode == GroupCode && w.OrganizationId == OrgID.Value).ToListAsync();
                    foreach (var d in del)
                    {
                        var find = afs.Where(w => w.ID == d.ManyId).Select(s => s.ID).FirstOrDefault();
                        if (find == null)
                        {
                            var delf = await db.AttachFile.Where(w => w.AttachFileId == d.ManyId).FirstOrDefaultAsync();
                            if (delf != null)
                            {
                                db.AttachFile.Remove(delf);
                            }
                        }
                      
                    }
                    
                }
                foreach (var f in afs)
                {

                    var up = await _helper.UploadFile(db, f,true);

                    if (!up.IsCompleted)
                    {
                        ret.CloneMessage(up.Message);
                        ret.IsCompleted = false;
                        return ret;
                    }
                    f.FileData = null;
                }
                await db.SaveChangesAsync();
                ret.IsCompleted = true;
            }catch (Exception ex) {
                ret.AddError(ex);
            }
            return ret;
        }
        
        public async Task<ReturnObject<long?>> Modify(ORGModel org, bool isCreate,string UserID, eORGModifyType mod)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                Organization oNew;
                // check attach file type
                var atts = new List<string>();
                if (org.AttachFiles != null )
                {
                    atts.AddRange(org.AttachFiles.Select(s => s.FileExtension));
                }
                if (org.LawOfDebts != null)
                {
                    atts.AddRange(org.LawOfDebts.Select(s => s.FileExtension));
                }
                
                foreach (var ext in atts)
                {
                    var media = mof.Services.Helper.Utilities.GetMMIEType(ext, _msglocalizer);
                    if (!media.IsCompleted)
                    {
                        ret.CloneMessage(media.Message);
                        return ret;
                    }
                }
                // check org code is already exist
                if (isCreate)
                {
                    oNew = await DB.Organization.Where(w => w.OrganizationCode == org.OrganizationCode ).FirstOrDefaultAsync();
                    if (oNew != null)
                    {
                        ret.AddMessage(eMessage.DataIsAlreadyExist.ToString(), "already exist", eMessageType.Error, new string[] { "รหัสหน่วยงาน" });
                        return ret;
                    }
                    oNew = new Organization();
                    //if (isChangeRequest)
                    //{
                    //    oNew.ChangeRequest = org.OrganizeID;
                    //}
                    DB.Organization.Add(oNew);
                }
                else {
                    oNew = await DB.Organization.Include(i => i.OrgtoMany).Include(ishare => ishare.ShareHolderOrganization)
                        .Include(rq => rq.RequestStatusNavigation).Include(rr => rr.RequestStatusNavigation.RequestStatusNavigation)
                        .Where(w => w.OrganizationId == org.OrganizeID).FirstOrDefaultAsync();
                    if (oNew == null)
                    {
                        ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { "หน่วยงาน" });
                        return ret;
                    }
                    var o = await DB.Organization.Where(w => w.OrganizationCode == org.OrganizationCode && w.OrganizationId != org.OrganizeID).FirstOrDefaultAsync();
                    if (o != null)
                    {
                        ret.AddMessage(eMessage.DataIsAlreadyExist.ToString(), "already exist", eMessageType.Error, new string[] { "รหัสหน่วยงาน" });
                        return ret;
                    }
                }
                var rqType = _helper.GetLOVByCode(LOVGroup.Request_Type.Change_Organization_Data, LOVGroup.Request_Type._LOVGroupCode);
                if (mod == eORGModifyType.CancelChange)
                {
                    if (oNew.RequestStatusNavigation.RequestStatusNavigation.Lovcode != ServiceModels.Constants.LOVGroup.Request_Status.ยื่นคำขอปรับปรุงข้อมูล)
                    {
                        ret.AddMessage(eMessage.AllowOnlyStatus.ToString(), "allow status", eMessageType.Error, new string[] { "ยื่นคำร้องแก้ไขข้อมูล" });
                        return ret;
                    }

                        var rqStatus = _helper.GetLOVByCode(ServiceModels.Constants.LOVGroup.Request_Status.ยกเลิกคำขอ, LOVGroup.Request_Status._LOVGroupCode);
                        oNew.RequestStatusNavigation = new Request
                        {
                            IssueId = org.OrganizeID,
                            RequestDt = DateTime.Now,
                            RequestStatus = rqStatus.Data.LOVKey,
                            RequestType = rqType.Data.LOVKey,
                            UserId = UserID
                        };
                    oNew.RequestData = null;
                    DB.SaveChanges();

                    ret.IsCompleted = true;
                    return ret;
                
                }
                //======= check valid
                ReturnObject<LOV> chk;
                //var oNew = new Organization();
                var  uchk = _helper.UserValidate(UserID);
                if (!uchk.IsCompleted)
                {
                    ret.CloneMessage(uchk.Message);
                    return ret;
                }
                
                chk = _helper.LOVCodeValidate(org.DebtCalculation.Code, LOVGroup.การนับหนี้._LOVGroupCode ,null);
                if (!chk.IsCompleted)
                {
                    ret.CloneMessage(chk.Message);
                    return ret;
                }
                oNew.DebtCalculation = chk.Data.LOVKey;

                chk = _helper.LOVCodeValidate(org.ORGType.Code, LOVGroup.ประเภทหน่วยงาน._LOVGroupCode, null);
                if (!chk.IsCompleted)
                {
                    ret.CloneMessage(chk.Message);
                    return ret;
                }
                oNew.Orgtype = chk.Data.LOVKey;

                if (org.Field != null && !string.IsNullOrEmpty(org.Field.Code))
                {

             
                    chk = _helper.LOVCodeValidate(org.Field.Code, LOVGroup.สาขา._LOVGroupCode,null  );
                    if (!chk.IsCompleted)
                    {
                        ret.CloneMessage(chk.Message);
                        return ret;
                    }
                    oNew.Field = chk.Data.LOVKey;
                }
                if (org.SubField != null && !string.IsNullOrEmpty(org.SubField.Code))
                {

                     chk = _helper.LOVCodeValidate(org.SubField.Code, LOVGroup.สาขาย่อย._LOVGroupCode, oNew.Field);
                     if (!chk.IsCompleted)
                    {
                        ret.CloneMessage(chk.Message);
                        return ret;
                    }
                    oNew.SubField = chk.Data.LOVKey;
                }
                if (org.PDMOType != null && org.PDMOType.ID.HasValue && org.PDMOType.ID != 0)
                {

                    chk = _helper.LOVCodeValidate(org.PDMOType.Code, LOVGroup.ประเภท_อปท_._LOVGroupCode, null);
                    if (!chk.IsCompleted)
                    {
                        ret.CloneMessage(chk.Message);
                        return ret;
                    }
                    oNew.Pdmotype = chk.Data.LOVKey;
                }else
                {
                    oNew.Pdmotype = null;
                }

                if (org.Template != null && !string.IsNullOrEmpty(org.Template.Code))
                {

                    chk = _helper.LOVCodeValidate(org.Template.Code, LOVGroup.สถานะองค์กร._LOVGroupCode, null);
                    if (!chk.IsCompleted)
                    {
                        ret.CloneMessage(chk.Message);
                        return ret;
                    }
                    oNew.Orgstatus = chk.Data.LOVKey;
                }
                foreach (var pda in org.PublicDebtActs)
                {
                    chk = _helper.LOVCodeValidate(pda.Code, LOVGroup.พรบ__หนี้สาธารณะ._LOVGroupCode, null);
                    if (!chk.IsCompleted)
                    {
                        ret.CloneMessage(chk.Message);
                        return ret;
                    }
                    if (pda.IsSelected)
                    {
                        if (oNew.OrgtoMany.Where(w => w.ManyId == chk.Data.LOVKey).FirstOrDefault() == null)
                        {
                            oNew.OrgtoMany.Add(new OrgtoMany { GroupCode = LOVGroup.พรบ__หนี้สาธารณะ._LOVGroupCode, ManyId = chk.Data.LOVKey });
                        }
                    }
                }
                foreach (var fda in org.FinanceDebtActs)
                {
                    chk = _helper.LOVCodeValidate(fda.Code, LOVGroup.พรบ__วินัยการเงินการคลัง._LOVGroupCode, null);
                    if (!chk.IsCompleted)
                    {
                        ret.CloneMessage(chk.Message);
                        return ret;
                    }
                    if (fda.IsSelected)
                    {
                        if (oNew.OrgtoMany.Where(w => w.ManyId == chk.Data.LOVKey).FirstOrDefault() == null)
                        {
                            oNew.OrgtoMany.Add(new OrgtoMany { GroupCode = LOVGroup.พรบ__วินัยการเงินการคลัง._LOVGroupCode, ManyId = chk.Data.LOVKey });
                        }
                        
                    }
                   
                }
                // delete non select pda fda
                if (!isCreate)
                {
                    
                    foreach (var om in oNew.OrgtoMany)
                    {
                        if (om.GroupCode == LOVGroup.พรบ__วินัยการเงินการคลัง._LOVGroupCode)
                        {
                            
                            if (org.FinanceDebtActs.Where(w => w.ID == om.ManyId && w.IsSelected).FirstOrDefault() == null)
                            {
                                DB.OrgtoMany.Remove(om);
                            }
                        }
                        if (om.GroupCode == LOVGroup.พรบ__หนี้สาธารณะ._LOVGroupCode)
                        {
                            if (org.PublicDebtActs.Where(w => w.ID == om.ManyId && w.IsSelected).FirstOrDefault() == null)
                            {
                                DB.OrgtoMany.Remove(om);
                            }
                        }
                    }

                }
                // check หน่วยงานต้นสังกัด
                if (org.ORGAffiliate != null)
                {


                    var chkorg = DB.Organization.Include(i => i.OrgtypeNavigation).Where(w => w.OrganizationCode == org.ORGAffiliate.Code).FirstOrDefault();
                   
                    
                    //var c = DB.Organization.Where(w => w.OrganizationCode == org.ORGAffiliate.Code)
                    //    .Select(s => new BasicData { Code = s.OrgtypeNavigation.LovgroupCode, Description = s.OrgtypeNavigation.Lovvalue }).FirstOrDefault();
                    if (chkorg == null)
                    {
                        ret.AddMessage(eMessage.DataIsNotFound.ToString(), "no affiliate", eMessageType.Error, new string[] { "หน่วยงานต้นสังกัด" });
                        return ret;
                    }
                    if ( chkorg.Orgaffiliate.HasValue || chkorg.OrgtypeNavigation.ParentLov !=  ServiceModels.Constants.LOVGroup.กลุ่มหน่วยงาน.หน่วยงานรัฐบาล)
                    {
                        ret.AddMessage(eMessage.CodeIsNotValid.ToString(), "invalid affiliate", eMessageType.Error, new string[] { "หน่วยงานต้นสังกัด" });
                        return ret;
                    }
                    oNew.Orgaffiliate = chkorg.OrganizationId;
                }
                // check ShareHolder
                var sh = SetShareHolder(org);
                if (!sh.IsCompleted)
                {
                    ret.CloneMessage(sh.Message);
                    return ret;
                }
                //=======end  check valid
                
                if (!isCreate)
                {
                    // ===  share holder
                    //foreach (var tmp in oNew.OrgtoMany.Where(w => w.GroupCode == eORGToManyGroup.LawOFDebt.ToString() || w.GroupCode == eORGToManyGroup.AttchFile.ToString()))
                    //{
                    //    var l = org.LawOfDebts.Where(w => w.ID == tmp.ManyId).FirstOrDefault();
                    //    var a = org.AttachFiles.Where(w => w.ID == tmp.ManyId).FirstOrDefault();
                    //    if (l == null && a == null)
                    //    {
                    //        DB.OrgtoMany.Remove(tmp);
                    //    }
                    //}
                    DB.ShareHolder.RemoveRange(oNew.ShareHolderOrganization);
               
                }
     

                // == end delete 
                // share holder
              
                foreach (var s in org.ShareHolders)
                {
                    oNew.ShareHolderOrganization.Add(new ShareHolder { OrgshareHolder = s.ShareHolderID, OrganizationName = s.ShareHolderID.HasValue ? null : s.ShareHolderName, Proportion = s.Proportion });
                     
                }
                oNew.Pdapropotion = org.PDAProportion;
                oNew.Fdapropotion = org.FDAProportion;

                oNew.EstablishmentLaw = org.EstablishmentLaw;
                oNew.FinanceDebtSection = org.FinanceDebtSection;
               
                oNew.HasLoanPower = org.HasLoanPower;
                oNew.IsCanceled = false;
                oNew.LoanPowerSection = org.LoanPowerSection;
                oNew.OrganizationCode = org.OrganizationCode;
                oNew.Address = org.Address;
                oNew.Tel = org.Tel;
                oNew.Province = (org.Province != null) ? org.Province.ID : null;
                
                oNew.OrganizationEnname = org.OrganizationTHName;
                oNew.OrganizationThname = org.OrganizationTHName;
                //oNew.Orglod
                oNew.PublicDebtSection = org.PublicDebtSection;
                oNew.Remark = org.Remark;
                //todo : calculate orgstatus
                if (isCreate || oNew.Orgstatus == 0)
                {
                    oNew.Orgstatus = 20054;
                }
                
                //======  manage attach
                if (mod != eORGModifyType.CancelChange)
                {
                    DbContextOptionsBuilder<MOFContext> opt = new DbContextOptionsBuilder<MOFContext>();
                    opt.UseSqlServer(DB.Database.GetDbConnection());
                    var dbAtt = new MOFContext(opt.Options);
                    var oatt = await OrganizationAttachFile(dbAtt,org.OrganizeID, org.LawOfDebts,  eORGToManyGroup.LawOFDebt.ToString(), mod == eORGModifyType.ChaneRequest, isCreate);
                    if (!oatt.IsCompleted)
                    {
                        ret.CloneMessage(oatt.Message);
                        return ret;
                    }
                    oatt = await OrganizationAttachFile(dbAtt, org.OrganizeID, org.AttachFiles, eORGToManyGroup.AttchFile.ToString(), mod == eORGModifyType.ChaneRequest, isCreate);
                    if (!oatt.IsCompleted)
                    {
                        ret.CloneMessage(oatt.Message);
                        return ret;
                    }
                }

                //=== end manage attach
                if (mod == eORGModifyType.Normal || mod == eORGModifyType.Approve)
                {
                    //====== law of debt , attach files
                    if (!isCreate)
                    {
                        var del = oNew.OrgtoMany.Where(w => w.GroupCode == eORGToManyGroup.LawOFDebt.ToString() || w.GroupCode == eORGToManyGroup.AttchFile.ToString()).ToList();
                        DB.OrgtoMany.RemoveRange(del);
                    }
                    foreach (var l in org.LawOfDebts)
                    {
                        var newMany = new OrgtoMany { GroupCode = eORGToManyGroup.LawOFDebt.ToString(), ManyId = l.ID.Value };
                        oNew.OrgtoMany.Add(newMany);

                    }
                    foreach (var l in org.AttachFiles)
                    {
                        var newMany = new OrgtoMany { GroupCode = eORGToManyGroup.AttchFile.ToString(), ManyId = l.ID.Value };
                        oNew.OrgtoMany.Add(newMany);

                    }


                    //var lods = new List<AttachFile>();
                    //var attaches = new List<AttachFile>();
                    //foreach (var lod in org.LawOfDebts)
                    //{
                    //    var att = oNew.OrgtoMany.Where(w => w.ManyId == lod.ID && w.GroupCode == eORGToManyGroup.LawOFDebt.ToString()).FirstOrDefault();
                    //    if (att == null)
                    //    {
                    //        var newaf = CopyEnitiy.NewAttachFile(lod, "Organization", eORGToManyGroup.LawOFDebt.ToString(), null, null);
                    //        lods.Add(newaf);
                    //    }
                    //    else
                    //    {
                    //        var a = DB.AttachFile.Where(w => w.AttachFileId == lod.ID).Select(s => s).FirstOrDefault();
                    //        if (a != null)
                    //        {
                    //            a.FileDetail = lod.FileDetail;
                    //            a.FileExtension = lod.FileExtension;
                    //            a.FileName = lod.FileName;
                    //            a.FileSize = lod.FileSize;
                               
                    //        }
                    //    }


                    //}
                    //foreach (var attach in org.AttachFiles)
                    //{
                    //    var att = oNew.OrgtoMany.Where(w => w.ManyId == attach.ID && w.GroupCode == eORGToManyGroup.AttchFile.ToString()).FirstOrDefault();
                    //    if (att == null)
                    //    {
                    //        var newatt = CopyEnitiy.NewAttachFile(attach, "Organization", eORGToManyGroup.AttchFile.ToString(), null, null);
                    //        attaches.Add(newatt);
                    //    }
                    //    else
                    //    {
                    //        var a = DB.AttachFile.Where(w => w.AttachFileId == attach.ID).Select(s => s).FirstOrDefault();
                    //        if (a != null)
                    //        {
                    //            a.FileDetail = attach.FileDetail;
                    //            a.FileExtension = attach.FileExtension;
                    //            a.FileName = attach.FileName;
                    //            a.FileSize = attach.FileSize;
                          
                    //        }
                    //    }
                    //}

                    if (mod == eORGModifyType.Approve)
                    {
                        oNew.RequestData = null;
                    }
                    string rq = "";
                    if (isCreate)
                    {
                        //rq = LOVGroup.Request_Status.สร้างข้อมูล;
                    }else
                    {
                        if (mod == eORGModifyType.Approve)
                        {
                            rq = LOVGroup.Request_Status.อนุมัติคำขอ;
                        }
                        if (mod == eORGModifyType.Normal)
                        {
                            //rq = LOVGroup.Request_Status.แก้ไขข้อมูล;
                        }
                    }
                    if (rq != "")
                    {
                        var rqStatus = _helper.GetLOVByCode(rq, LOVGroup.Request_Status._LOVGroupCode);
                        oNew.RequestStatusNavigation = new Request
                        {
                            IssueId = org.OrganizeID,
                            RequestDt = DateTime.Now,
                            RequestStatus = rqStatus.Data.LOVKey,
                            RequestType = rqType.Data.LOVKey,
                            UserId = UserID
                        };
                    }

                    
                   // DB.AddRange(lods);
                   // DB.AddRange(attaches);
                    //======end law of debt, attach files
                    DB.SaveChanges();
                    //save law of debt & attach file to db
                    //foreach (var l in lods)
                    //{
                    //    var newMany = new OrgtoMany { GroupCode = eORGToManyGroup.LawOFDebt.ToString(), ManyId = l.AttachFileId };
                    //    oNew.OrgtoMany.Add(newMany);

                    //}
                    //foreach (var l in attaches)
                    //{
                    //    var newMany = new OrgtoMany { GroupCode = eORGToManyGroup.AttchFile.ToString(), ManyId = l.AttachFileId };
                    //    oNew.OrgtoMany.Add(newMany);

                    //}


                   // DB.SaveChanges();
                }
                else if (mod == eORGModifyType.ChaneRequest)
                {
                    var json = JsonConvert.SerializeObject(org);

                    DbContextOptionsBuilder<MOFContext> opt = new DbContextOptionsBuilder<MOFContext>();
                    opt.UseSqlServer(DB.Database.GetDbConnection());
                    var dbTmp = new MOFContext(opt.Options);
                    var o = await dbTmp.Organization.Where(w => w.OrganizationId == org.OrganizeID).FirstOrDefaultAsync();
                   
           
                    var rqStatus = _helper.GetLOVByCode(LOVGroup.Request_Status.ยื่นคำขอปรับปรุงข้อมูล, LOVGroup.Request_Status._LOVGroupCode);

                    o.RequestData = json;
                    o.RequestStatusNavigation = new Request
                    {
                        IssueId = org.OrganizeID,
                        RequestDt = DateTime.Now,
                        RequestStatus = rqStatus.Data.LOVKey,
                        RequestType = rqType.Data.LOVKey,
                        RequestData = json,
                        UserId = UserID
                    };
                    dbTmp.SaveChanges();
                }
                

              

                ret.IsCompleted = true;
                ret.Data = oNew.OrganizationId;
                
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }

        public async Task<ReturnObject<LoanSummaryDashboard>> GetLoanSummaryDashboard(GetByID id)
        {
            var ret = new ReturnObject<LoanSummaryDashboard>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var data = new LoanSummaryDashboard();
                ///////// section A
                data.CurrencyRateDate = DateTime.Now;
                data.OutstandingDept = 25000000;
                data.MaturityDebt = new  Dictionary<string, decimal>();
                data.MaturityDebt.Add("Year", 2556);
                data.MaturityDebt.Add("Dept", 1200000);
                data.DebtTermRatioGraph = new List<GraphData> {
                    new GraphData { Group1="ระยะสั้น", Amount = 1000000, Percent = 10.99M },
                    new GraphData { Group1="ระยะยาว", Amount = 9000000, Percent = 89.01M }
                };
                data.DebtSourceGraph = new List<GraphData> {
                    new GraphData { Group1="หนี้ต่างประเทศ", Amount = 1000000, Percent = 10.99M },
                    new GraphData { Group1="หนี้ในประเทศ", Amount = 9000000, Percent = 89.01M }
                };
                data.MaturityDebt5Y = new List<GraphData> {
                    new GraphData { Group1="2562", Group2 = "Domestic", Amount = 1000000, Percent = 10.99M },
                    new GraphData { Group1="2562", Group2 = "Inter", Amount = 9000000, Percent = 89.01M },
                    new GraphData { Group1="2563", Group2 = "Domestic", Amount = 9000000, Percent = 89.01M },
                    new GraphData { Group1="2563", Group2 = "Inter", Amount = 1000000, Percent = 10.99M  },
                    new GraphData { Group1="2564", Group2 = "Domestic", Amount = 1000000, Percent = 10.99M },
                    new GraphData { Group1="2564", Group2 = "Inter", Amount = 9000000, Percent = 89.01M },
                    new GraphData { Group1="2565", Group2 = "Domestic", Amount = 1000000, Percent = 10.99M },
                    new GraphData { Group1="2565", Group2 = "Inter", Amount = 9000000, Percent = 89.01M },
                    new GraphData { Group1="2566", Group2 = "Domestic", Amount = 1000000, Percent = 10.99M },
                    new GraphData { Group1="2566", Group2 = "Inter", Amount = 9000000, Percent = 89.01M },
                };


                //////// section B
                data.LoanPlan5YStart = 2562;
                data.LoanPlan5YEnd = 2567;
                data.LoanPlan5YDue = DateTime.Now.Date.AddMonths(2);
                data.PublicLoanPlanYear = 2563;
                data.PublicLoanPlanDeu = DateTime.Now.Date.AddMonths(10);

                data.AdjustPublicLoanPlan = new List<AdjustPublicLoan>();
                data.AdjustPublicLoanPlan.Add(new AdjustPublicLoan { DueDate = DateTime.Now.AddDays(65), Session = 1, Status = "ผ่านมติที่ประชุมคณะทำงาน", Year = 2562 , IsCompleted = true });
                data.AdjustPublicLoanPlan.Add(new AdjustPublicLoan { DueDate = DateTime.Now.AddDays(200), Session = 1, Status = "ยังไม่มีแผน", Year = 2563,IsCompleted = false  });

                data.SignAndReimburseReport = new List<SignAndReimbursePlan>();
                var dt = new DateTime(2019, 10, 1);
                Random gen = new Random();
                for (var i = 0; i < 12; i++)
                {
                    var sr = new SignAndReimbursePlan();
                    sr.Month = dt;
                    sr.IsSentReport = (i < 4) ? true : false;
                    sr.HasSigned = gen.Next(100) <= 50 ? true : false;
                    sr.HasReimbursed = gen.Next(100) <= 50 ? true : false;
                    dt.AddMonths(1);
                }
                //-===================================

                ret.IsCompleted = true;
                ret.Data = data;
                return ret;
            } catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        /// <summary>
        /// แสดงประกาศความช่วยเหลือทางวิชาการ .  
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ReturnObject<List<TechicalAssistance>>> GetTechicalAssistances()
        {
            var ret = new ReturnObject<List<TechicalAssistance>>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                ret.Data = new List<TechicalAssistance>();
                var r = new Random();
                for (var i = 0; i < 4; i++)
                {
                    var tmp = new TechicalAssistance();
                    tmp.Detail = $"รายละเอียดการประกาศ หมายเลขที่ {i.ToString()}";
                    tmp.ExpiryDate = DateTime.Now.AddDays(60);
                    tmp.ImageUrl = $"http://image.url/id/{i.ToString()}";
                    tmp.Interstors = r.Next(5000);
                    tmp.Title = $"หัวข้อการประกาศ หมายเลขที่ {i.ToString()}";
                    ret.Data.Add(tmp);

                }
                ret.IsCompleted = true;
            }catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        private void AddAct(List<BasicData> basic, string lovCode, List<LOV> lovs)
        {
            var lov = lovs.Where(w => w.LOVCode == lovCode).FirstOrDefault();
            if (lov != null)
            {
                basic.Add(new BasicData
                {
                    Code = lov.LOVCode,
                    Description = lov.LOVValue,
                    ID = lov.LOVKey,
                    IsSelected = true
                });
            }
        }
        private async Task<bool> isGovORActAB(long? orgId)
        {
             
            if (orgId.HasValue)
            {
                var org = await DB.Organization.Include(t => t.OrgtypeNavigation).Include(m => m.OrgtoMany).Where(w => w.OrganizationId == orgId).FirstOrDefaultAsync();
                var allgov = new string[] { ServiceModels.Constants.LOVGroup.ประเภทหน่วยงาน.ส่วนราชการ__กระทรวง_ทบวง_กรม__ , ServiceModels.Constants.LOVGroup.ประเภทหน่วยงาน.หน่วยงานอื่น_ๆ_ของรัฐ,
                ServiceModels.Constants.LOVGroup.ประเภทหน่วยงาน.หน่วยงานอิสระ};
                if (allgov.Contains(org.OrgtypeNavigation.Lovcode))
                {
                    return true;
                }
                if (org.OrgtypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.ประเภทหน่วยงาน.รัฐวิสาหกิจ)
                {
                    foreach (var many in org.OrgtoMany)
                    {
                        if (many.GroupCode == ServiceModels.Constants.LOVGroup.พรบ__วินัยการเงินการคลัง._LOVGroupCode ||
                            many.GroupCode == ServiceModels.Constants.LOVGroup.พรบ__หนี้สาธารณะ._LOVGroupCode)
                        {
                            var lov = await DB.CeLov.Where(w => w.Lovkey == many.ManyId).FirstOrDefaultAsync();
                            if (lov != null)
                            {
                                if (lov.Lovcode == ServiceModels.Constants.LOVGroup.พรบ__หนี้สาธารณะ.รัฐวิสาหกิจประเภท__ก_ || 
                                   lov.Lovcode == ServiceModels.Constants.LOVGroup.พรบ__วินัยการเงินการคลัง.รัฐวิสาหกิจประเภท__1_ ||
                                   lov.Lovcode == ServiceModels.Constants.LOVGroup.พรบ__หนี้สาธารณะ.รัฐวิสาหกิจประเภท__ข_ ||
                                   lov.Lovcode == ServiceModels.Constants.LOVGroup.พรบ__วินัยการเงินการคลัง.รัฐวิสาหกิจประเภท__2_
                                   )
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
          

            return false;
        }
        private async Task<decimal[]> govPropotionOld(ShareHD share)
        {
            Func<bool, decimal[]> ret;
            decimal[] prop = new decimal[] { 0, 0, 0 };

            if (await isGovORActAB(share.ShareHolderID))
            {
                return new decimal[] { share.Proportion, share.Proportion, 1 };
            }
            var orgs = await DB.ShareHolder.Where(w => w.OrganizationId == share.ShareHolderID).ToListAsync();
            decimal tot = 0;
            decimal gov = 0;
            foreach (var org in orgs)
            {
                tot += org.Proportion;
                if (await isGovORActAB(org.OrgshareHolder))
                {
                    gov += org.Proportion;
                }
            }
            if (tot > 0)
            {
                gov = (gov / tot);
            }
            //return new decimal[] { share.Proportion * gov, share.Proportion, 0 };
            return new decimal[] { share.Proportion * gov, 0, 0 };

        }
        private async Task<decimal[]> govPropotion(ShareHD share)
        {
            Func<bool, decimal[]> ret;
            decimal[] prop = new decimal[] { 0, 0 , 0};
            decimal fda = 0;
            decimal isGov = 0;
            if (await isGovORActAB(share.ShareHolderID))
            {
                //return new decimal[] { share.Proportion, share.Proportion, 1};
                fda = share.Proportion;
                isGov = 1;
            }
            var orgs = await DB.ShareHolder.Where(w => w.OrganizationId == share.ShareHolderID).ToListAsync();
            decimal tot = 0;
            decimal gov = 0;
            foreach (var org in orgs)
            {
                tot += org.Proportion;
                if (await isGovORActAB(org.OrgshareHolder))
                {
                   gov += org.Proportion;
                } 
            }
            if (tot > 0)
            {
                gov = (gov / tot);
            }else
            {
                if (isGov == 1)
                {
                    gov = 1;
                }
            }
            //return new decimal[] { share.Proportion * gov, share.Proportion, 0 };
            return new decimal[] { share.Proportion * gov, fda, isGov};
       
        }
        public async Task<ReturnObject<CalORGStatusResponse>> CalculateORGStatus(CalORGStatusRequest org)
        {
            var ret = new ReturnObject<CalORGStatusResponse>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                ret.Data = new CalORGStatusResponse();
                ret.Data.FinanceDebtActs = new List<BasicData>();
                ret.Data.PublicDebtActs = new List<BasicData>();
                var pda = _helper.GetLOVByGroup(ServiceModels.Constants.LOVGroup.พรบ__หนี้สาธารณะ._LOVGroupCode);
                var fda = _helper.GetLOVByGroup(ServiceModels.Constants.LOVGroup.พรบ__วินัยการเงินการคลัง._LOVGroupCode);

                if (org.ShareHolders == null)
                {
                    org.ShareHolders = new List<ShareHD>();
                }
                decimal tot = 0;
                bool isActC = false;
                foreach (var sh in org.ShareHolders)
                {
                    var prop = await govPropotion(sh);
                    ret.Data.PDAProportion += prop[0];
                    ret.Data.FDAProportion += prop[1];
                    if (prop[2] == 0)
                    {
                        isActC = true;
                    }

                    tot += sh.Proportion;
                }
                
                var lov = await DB.CeLov.Include(i => i.InverseParent).Where(w => w.Lovcode == org.ORGType.Code).FirstOrDefaultAsync();
                if (lov == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { "ประเภทหน่วยงาน" });
                    return ret;
                }
                // รัฐวิสาหะกิจ
                if (lov.Lovcode == ServiceModels.Constants.LOVGroup.ประเภทหน่วยงาน.รัฐวิสาหกิจ)
                {
                    if (tot == 100)
                    {
                       if (ret.Data.PDAProportion == 100)
                        {
                            AddAct(ret.Data.PublicDebtActs, ServiceModels.Constants.LOVGroup.พรบ__หนี้สาธารณะ.รัฐวิสาหกิจประเภท__ก_ , pda.Data);
                            AddAct(ret.Data.FinanceDebtActs, ServiceModels.Constants.LOVGroup.พรบ__วินัยการเงินการคลัง.รัฐวิสาหกิจประเภท__1_, fda.Data);
                        }else if (ret.Data.PDAProportion >= 50 && ret.Data.PDAProportion < 100 && !isActC)
                        {
                            AddAct(ret.Data.PublicDebtActs, ServiceModels.Constants.LOVGroup.พรบ__หนี้สาธารณะ.รัฐวิสาหกิจประเภท__ข_, pda.Data);
                            AddAct(ret.Data.FinanceDebtActs, ServiceModels.Constants.LOVGroup.พรบ__วินัยการเงินการคลัง.รัฐวิสาหกิจประเภท__2_, fda.Data);
                        }else if (ret.Data.PDAProportion >= 50 && ret.Data.PDAProportion < 100 && isActC)
                        {
                            AddAct(ret.Data.PublicDebtActs, ServiceModels.Constants.LOVGroup.พรบ__หนี้สาธารณะ.รัฐวิสาหกิจประเภท__ค_, pda.Data);
                            AddAct(ret.Data.FinanceDebtActs, ServiceModels.Constants.LOVGroup.พรบ__วินัยการเงินการคลัง.รัฐวิสาหกิจประเภท__3_, fda.Data);
                        }
                    }
                }
                if (lov.Lovcode == ServiceModels.Constants.LOVGroup.ประเภทหน่วยงาน.กองทุนนิติบุคคล)
                {
                    AddAct(ret.Data.PublicDebtActs, ServiceModels.Constants.LOVGroup.พรบ__หนี้สาธารณะ.หน่วยงานในการกำกับดูแลของรัฐ, pda.Data);
                    AddAct(ret.Data.FinanceDebtActs, ServiceModels.Constants.LOVGroup.พรบ__วินัยการเงินการคลัง.ทุนหมุนเวียนที่มีฐานะเป็นนิติบุคคล, fda.Data);
                }
                if (lov.Lovcode == ServiceModels.Constants.LOVGroup.ประเภทหน่วยงาน.มหาวิทยาลัยในการกำกับดูแลของรัฐ)
                {
                    AddAct(ret.Data.PublicDebtActs, ServiceModels.Constants.LOVGroup.พรบ__หนี้สาธารณะ.หน่วยงานในการกำกับดูแลของรัฐ, pda.Data);
                    AddAct(ret.Data.FinanceDebtActs, ServiceModels.Constants.LOVGroup.พรบ__วินัยการเงินการคลัง.หน่วยงานอื่นของรัฐตามที่กฎหมายกำหนด, fda.Data);
                }
                if (lov.Lovcode == ServiceModels.Constants.LOVGroup.ประเภทหน่วยงาน.องค์การมหาชน)
                {
                    AddAct(ret.Data.PublicDebtActs, ServiceModels.Constants.LOVGroup.พรบ__หนี้สาธารณะ.หน่วยงานในการกำกับดูแลของรัฐ, pda.Data);
                    AddAct(ret.Data.FinanceDebtActs, ServiceModels.Constants.LOVGroup.พรบ__วินัยการเงินการคลัง.องค์การมหาชน, fda.Data);
                }
                if (lov.Lovcode == ServiceModels.Constants.LOVGroup.ประเภทหน่วยงาน.หน่วยงานอื่น_ๆ_ของรัฐ)
                {
                    AddAct(ret.Data.PublicDebtActs, ServiceModels.Constants.LOVGroup.พรบ__หนี้สาธารณะ.หน่วยงานในการกำกับดูแลของรัฐ, pda.Data);
                    AddAct(ret.Data.FinanceDebtActs, ServiceModels.Constants.LOVGroup.พรบ__วินัยการเงินการคลัง.หน่วยงานอื่นของรัฐตามที่กฎหมายกำหนด, fda.Data);
                }
                if (lov.Lovcode == ServiceModels.Constants.LOVGroup.ประเภทหน่วยงาน.หน่วยงานอิสระ)
                {
                    AddAct(ret.Data.PublicDebtActs, ServiceModels.Constants.LOVGroup.พรบ__หนี้สาธารณะ.หน่วยงานในการกำกับดูแลของรัฐ, pda.Data);
                    AddAct(ret.Data.FinanceDebtActs, ServiceModels.Constants.LOVGroup.พรบ__วินัยการเงินการคลัง.หน่วยงานอื่นของรัฐตามที่กฎหมายกำหนด, fda.Data);
                }
                if (lov.Lovcode == ServiceModels.Constants.LOVGroup.ประเภทหน่วยงาน.ส่วนราชการ__กระทรวง_ทบวง_กรม__)
                {
                    AddAct(ret.Data.PublicDebtActs, ServiceModels.Constants.LOVGroup.พรบ__หนี้สาธารณะ.หน่วยงานของรัฐ, pda.Data);
                    AddAct(ret.Data.FinanceDebtActs, ServiceModels.Constants.LOVGroup.พรบ__วินัยการเงินการคลัง.ส่วนราชการ, fda.Data);
                }
                if (lov.Lovcode == ServiceModels.Constants.LOVGroup.ประเภทหน่วยงาน.องค์กรปกครองส่วนท้องถิ่น)
                {
                    AddAct(ret.Data.PublicDebtActs, ServiceModels.Constants.LOVGroup.พรบ__หนี้สาธารณะ.องค์กรปกครองส่วนท้องถิ่น, pda.Data);
                    AddAct(ret.Data.FinanceDebtActs, ServiceModels.Constants.LOVGroup.พรบ__วินัยการเงินการคลัง.องค์การปกครองส่วนท้องถิ่น, fda.Data);
                }
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        private ReturnMessage SetShareHolder(ORGModel org)
        {   //todo re calculate poportion
            var ret = new ReturnMessage(_msglocalizer);
            
            ret.IsCompleted = false;
            decimal pda = 0, fda = 0;
            if (org.ShareHolders == null)
            {
                org.ShareHolders = new List<ShareHD>();
            }
            foreach (var s in org.ShareHolders)
            {
                if (s.ShareHolderID.HasValue)
                {
                    var o = DB.Organization.Include(i => i.OrgtypeNavigation).Include(m => m.OrgtoMany).Where(w => w.OrganizationId == s.ShareHolderID).FirstOrDefault();
                    if (o != null)
                    { 
                        s.ShareHolderName = o.OrganizationThname;
                        if (o.ShareHolderOrganization != null)
                        {
                            
                            s.FinanceDebtActs = o.OrgtoMany.Join(DB.CeLov , om => om.ManyId, c => c.Lovkey, (om,c) => new { om, c })
                                .Where(w => w.om.GroupCode == ServiceModels.Constants.LOVGroup.พรบ__วินัยการเงินการคลัง._LOVGroupCode).
                                Select( sl => 
                                new BasicData {
                                    ID = sl.om.ManyId,
                                    Code = sl.c.Lovcode,
                                    Description = sl.c.Lovvalue
                                }).ToList();

                            s.PublicDebtActs = o.OrgtoMany.Join(DB.CeLov, om => om.ManyId, c => c.Lovkey, (om, c) => new { om, c })
                               .Where(w => w.om.GroupCode == ServiceModels.Constants.LOVGroup.พรบ__หนี้สาธารณะ._LOVGroupCode).
                               Select(sl =>
                              new BasicData
                               {
                                   ID = sl.om.ManyId,
                                   Code = sl.c.Lovcode,
                                   Description = sl.c.Lovvalue
                               }).ToList();
                        }
                      
                    }else
                    {
                 
                        ret.AddMessage(eMessage.CodeIsNotValid.ToString(), "not valid", eMessageType.Error, new string[] { "หน่วยงาน" });
                        return ret;

                    }

                    fda += s.Proportion;
                }else
                {
                    if (string.IsNullOrEmpty(s.ShareHolderName))
                    {
                        ret.AddMessage(eMessage.IsRequired.ToString(), "not valid", eMessageType.Error, new string[] { "หน่วยงานเอกชน" });
                        return ret;
                    }
                    //s.ORGType = "หน่วยงานภาคเอกชน";
                    pda += s.Proportion;
                }
            }
            var tot = pda + fda;
            //org.FDAProportion = 0;
            //org.FDAProportion = 0;
            //if (tot > 0)
            //{
            //    org.PDAProportion = Math.Round((pda / tot) * 100, 2);
            //    org.FDAProportion = 100 - org.PDAProportion;
            //}
            
         
            ret.IsCompleted = true;
            return ret;
        }
        public async Task<ReturnObject<ORGModel>> Get(long id, bool IsChangeRequest,bool isGetAll)
        {
            var ret = new ReturnObject<ORGModel> (_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var query = DB.Organization.Select(s => s);
                //if (IsChangeRequest)
                //{
                //    query = query.Where(w => w.ChangeRequest.HasValue && w.IsCanceled == false);
                //}
                //else
                //{
                //    query = query.Where(w => !w.ChangeRequest.HasValue);
                //}
                var test = (from t in DB.Organization where t.OrganizationId == id select t).FirstOrDefault();
                var q = from o in query
                        where o.OrganizationId == id
                        select new ORGModel
                        { OrganizationCode = o.OrganizationCode,
                            DebtCalculation = new BasicData { ID = o.DebtCalculation, Code = o.DebtCalculationNavigation.Lovcode, Description = o.DebtCalculationNavigation.Lovvalue },
                            EstablishmentLaw = o.EstablishmentLaw,
                            Field = (o.FieldNavigation != null) ? new BasicData { ID = o.Field, Code = o.FieldNavigation.Lovcode, Description = o.FieldNavigation.Lovvalue } : null,
                            FinanceDebtSection = o.FinanceDebtSection,
                            HasLoanPower = o.HasLoanPower,
                            LoanPowerSection = o.LoanPowerSection,
                            ORGAffiliate = new BasicData
                            {
                                Code = o.OrgaffiliateNavigation.OrganizationCode,
                                Description = o.OrgaffiliateNavigation.OrganizationThname,
                                ID = o.OrgaffiliateNavigation.OrganizationId
                            },
                            OrganizationTHName = o.OrganizationThname,
                            ORGType = (o.OrgtypeNavigation != null) ? new BasicData { ID = o.Orgtype, Code = o.OrgtypeNavigation.Lovcode, Description = o.OrgtypeNavigation.Lovvalue } : null,
                            OrganizeID = o.OrganizationId,

                            PublicDebtSection = o.PublicDebtSection,
                            Remark = o.Remark,
                            SubField = (o.SubFieldNavigation != null) ? new BasicData { ID = o.SubField, Code = o.SubFieldNavigation.Lovcode, Description = o.SubFieldNavigation.Lovvalue } : null,
                            RequestStatus = (o.RequestStatusNavigation != null) ?
                            new LogData { 
                                ActionCode = o.RequestStatusNavigation.RequestStatusNavigation.Lovcode,
                                Action = o.RequestStatusNavigation.RequestStatusNavigation.Lovvalue,
                                ActionTime = o.RequestStatusNavigation.RequestDt, ActionType = o.RequestStatusNavigation.RequestTypeNavigation.Lovvalue, ID = o.RequestStatusNavigation.RequestId,
                                UserID = o.RequestStatusNavigation.UserId }
                            : null,
                            PDAProportion = o.Pdapropotion.HasValue ? o.Pdapropotion.Value : 0,
                            FDAProportion = o.Fdapropotion.HasValue ? o.Fdapropotion.Value : 0,

                            ShareHolders = (isGetAll) ? o.ShareHolderOrganization.Select(s => new ShareHD
                            {
                                Proportion = s.Proportion,
                                ShareHolderID = s.OrgshareHolder,
                                ShareHolderName = (s.OrgshareHolder.HasValue) ? s.OrgshareHolderNavigation.OrganizationThname : s.OrganizationName,
                                //FinanceDebtActs = s.Organization.OrgtoMany.Where(w => w.GroupCode == ServiceModels.Constants.LOVGroup.พรบ__วินัยการเงินการคลัง._LOVGroupCode)
                                //.Select(s2 => new BasicData { ID = s2.ManyId }).ToList(),
                                //PublicDebtActs = s.Organization.OrgtoMany.Where(w => w.GroupCode == ServiceModels.Constants.LOVGroup.พรบ__หนี้สาธารณะ._LOVGroupCode)
                                //.Select(s2 => new BasicData { ID = s2.ManyId }).ToList(),
                            }).ToList() : new List<ShareHD>(),
                            Address = o.Address,
                            Tel = o.Tel,
                            Province = new BasicData
                            {
                                Code = "",
                                ID = o.ProvinceNavigation.ProvinceId,
                                Description = o.ProvinceNavigation.ProvinceName
                            },
                            PDMOType = new BasicData
                            {
                                Code = o.PdmotypeNavigation.Lovcode,
                                ID = o.PdmotypeNavigation.Lovkey,
                                Description = o.PdmotypeNavigation.Lovvalue
                            },
                            Template = new BasicData
                            {
                                Code = o.OrgstatusNavigation.Lovcode,
                                ID = o.OrgstatusNavigation.Lovkey,
                                Description = o.OrgstatusNavigation.Lovvalue
                            }
                            
                        };
    
                var org = await q.FirstOrDefaultAsync();
                if (org == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { "หน่วยงาน" });
                    return ret;
                }
                if (IsChangeRequest )
                {
                    var req = await DB.Organization.Include(i => i.RequestStatusNavigation).Include(ii => ii.RequestStatusNavigation.RequestStatusNavigation).Where(w => w.OrganizationId == id)
                        .Select(s => new { s.RequestData, s.RequestStatusNavigation , status = s.RequestStatusNavigation.RequestStatusNavigation }).FirstOrDefaultAsync();
                    if (!string.IsNullOrEmpty(req.RequestData))
                    {
                        org = JsonConvert.DeserializeObject<ORGModel>(req.RequestData);
                        if (req.RequestStatusNavigation != null)
                        {
                            org.RequestStatus = new LogData
                            {
                                ActionCode = req.status.Lovcode,
                                Action = req.status.Lovvalue,
                                ActionType = "หน่วยงาน",
                                ActionTime = req.RequestStatusNavigation.RequestDt,
                                ID = req.RequestStatusNavigation.RequestId,
                                UserID = req.RequestStatusNavigation.UserId
                            };
                        }
                        var u = await DB.Users.Where(w => w.Id == org.RequestStatus.UserID).FirstOrDefaultAsync();
                        if (u != null)
                        {
                            org.RequestStatus.UserName = u.TfirstName + ' ' + u.TlastName;
                        }
                        CopyEnitiy.GetLOVBasicData(_helper, org.ORGType, LOVGroup.ประเภทหน่วยงาน._LOVGroupCode);
                        CopyEnitiy.GetLOVBasicData(_helper, org.Field, LOVGroup.สาขา._LOVGroupCode);
                        CopyEnitiy.GetLOVBasicData(_helper, org.SubField, LOVGroup.สาขาย่อย._LOVGroupCode);
                        CopyEnitiy.GetLOVBasicData(_helper, org.DebtCalculation, LOVGroup.การนับหนี้._LOVGroupCode);
                        var aff = await DB.Organization.Where(w => w.OrganizationCode == org.ORGAffiliate.Code).FirstOrDefaultAsync();
                        if (aff != null)
                        {
                            org.ORGAffiliate.ID = aff.OrganizationId;
                            org.ORGAffiliate.Description = aff.OrganizationThname;
                        }
                        else
                        {
                            org.ORGAffiliate.ID = 0;
                            org.ORGAffiliate.Description = "";
                        }
                        foreach (var p in org.PublicDebtActs)
                        {
                            CopyEnitiy.GetLOVBasicData(_helper, p, LOVGroup.พรบ__หนี้สาธารณะ._LOVGroupCode);
                        }
                        foreach (var f in org.FinanceDebtActs)
                        {
                            CopyEnitiy.GetLOVBasicData(_helper, f, LOVGroup.พรบ__วินัยการเงินการคลัง._LOVGroupCode);
                        }
                        SetShareHolder(org);

                        ret.Data = org;
                        ret.IsCompleted = true;
                        return ret;
                    } else
                    {
                        ret.Data = null;
                        ret.IsCompleted = true;
                        return ret;
                    }



                }
                if (org.RequestStatus != null) { 
                    var u = await DB.Users.Where(w => w.Id == org.RequestStatus.UserID).FirstOrDefaultAsync();
                    if (u != null)
                    {
                        org.RequestStatus.UserName = u.TfirstName + ' ' + u.TlastName;
                    }
                }
                // pda, fda
                org.PublicDebtActs = new List<BasicData>();
                org.FinanceDebtActs = new List<BasicData>();
                if (isGetAll)
                {
                    SetShareHolder(org);
                    org.FinanceDebtActs = new List<BasicData>();
                    org.PublicDebtActs = new List<BasicData>();
                    var toMany = DB.OrgtoMany.Where(w => w.OrganizationId == id).ToList();

                    org.LawOfDebts = new List<AttachFileData>();
                    org.AttachFiles = new List<AttachFileData>();
                    foreach (var m in toMany)
                    {
                        if (m.GroupCode == eORGToManyGroup.LawOFDebt.ToString())
                        {
                            var law = DB.AttachFile.Where(w => w.AttachFileId == m.ManyId).FirstOrDefault();
                            if (law != null)
                            {
                                org.LawOfDebts.Add(CopyEnitiy.NewAttachFileData(law));
                            }
                        }

                        if (m.GroupCode == eORGToManyGroup.AttchFile.ToString())
                        {
                            var att = DB.AttachFile.Where(w => w.AttachFileId == m.ManyId).FirstOrDefault();
                            if (att != null)
                            {
                                org.AttachFiles.Add(CopyEnitiy.NewAttachFileData(att));
                            }
                        }
                        if (m.GroupCode == ServiceModels.Constants.LOVGroup.พรบ__หนี้สาธารณะ._LOVGroupCode)
                        {
                            var lov = DB.CeLov.Where(w => w.Lovkey == m.ManyId).FirstOrDefault();
                            if (lov != null)
                            {
                                org.PublicDebtActs.Add(new BasicData { Code = lov.Lovcode, Description = lov.Lovvalue, ID = lov.Lovkey, IsSelected = true });

                            }
                        }
                        if (m.GroupCode == ServiceModels.Constants.LOVGroup.พรบ__วินัยการเงินการคลัง._LOVGroupCode)
                        {
                            var lov = DB.CeLov.Where(w => w.Lovkey == m.ManyId).FirstOrDefault();
                            if (lov != null)
                            {
                                org.FinanceDebtActs.Add(new BasicData { Code = lov.Lovcode, Description = lov.Lovvalue, ID = lov.Lovkey, IsSelected = true });

                            }
                        }
                    }
                }
        
                // share holder

                // last approve 

                var ap = await DB.Request.Where(w => w.RequestTypeNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Request_Type.Change_Organization_Data &&
                w.RequestStatusNavigation.Lovcode == ServiceModels.Constants.LOVGroup.Request_Status.อนุมัติคำขอ && w.IssueId == id).FirstOrDefaultAsync();
                if (ap != null)
                {
                    org.LastApproved = new LogData { Action = "อนุมัติคำร้อง", ActionType = "ข้อมูลหน่วยงาน", ActionTime = ap.RequestDt, ID = ap.RequestId, UserID = ap.UserId };
                    var apu = await DB.Users.Where(w => w.Id == ap.UserId).FirstOrDefaultAsync();
                    if (apu != null)
                    {
                        org.LastApproved.UserName = apu.TfirstName + " " + apu.TlastName;
                    }
                }
                ret.Data =  org;
                ret.IsCompleted = true;
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }

        public async Task<ReturnList<BasicData>> GetAffiliates()
        {
            var ret = new ReturnList<BasicData>(_msglocalizer) ;
            ret.IsCompleted = false;
            try
            {
                var aff = await DB.Organization .Where(w => !w.Orgaffiliate.HasValue && w.IsCanceled == false && !w.ChangeRequest.HasValue &&
                  w.OrgtypeNavigation.ParentLov == ServiceModels.Constants.LOVGroup.กลุ่มหน่วยงาน.หน่วยงานรัฐบาล)
                  .Select(s => new BasicData { Code = s.OrganizationCode, Description = s.OrganizationThname, ID = s.OrganizationId }).ToListAsync();
                ret.Data = aff;
                ret.IsCompleted = true;
            }catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }

        public async Task<ReturnObject<long?>> ApproveChange(long id, string UserID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var req = await DB.Organization.Where(w => w.OrganizationId == id).Select(s => s.RequestData).FirstOrDefaultAsync();
                if (req == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { "การร้องขอ" });
                    return ret;
                }
                var org = JsonConvert.DeserializeObject<ORGModel>(req);
                ret = await Modify(org, false, UserID,  eORGModifyType.Approve);
                 
                
            }catch(Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }

        public async Task<ReturnObject<long?>> CancelChange(long id, string UserID)
        {
            var ret = new ReturnObject<long?>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var req = await DB.Organization.Where(w => w.OrganizationId == id).Select(s => s.RequestData).FirstOrDefaultAsync();
                if (req == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { "การร้องขอ" });
                    return ret;
                }
                var org = JsonConvert.DeserializeObject<ORGModel>(req);
                ret = await Modify(org, false, UserID, eORGModifyType.Approve);


            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        public async Task<ReturnObject<string>> GetPDMOLawReguration(long id)
        {
            var ret = new ReturnObject<string>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var stat = await DB.Organization.Where(w => w.OrganizationId == id).Select(s => s.Orgstatus).FirstOrDefaultAsync();
                var lov = await DB.CeLovextend.Where(w => w.ExtendType == ServiceModels.Constants.eLOVExtend.PDMOLAWREG.ToString() && w.Lovkey == stat).FirstOrDefaultAsync();

                if (lov == null)
                {
                    ret.AddMessage(eMessage.DataIsNotFound.ToString(), "not found", eMessageType.Error, new string[] { "ข้อกฎหมายตามระเบียบ สบน." });
                    return ret;
                }
                ret.IsCompleted = true;
                ret.Data = lov.ExtendValue;
                


            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        #region s120
        public async Task<ReturnList<ORGModel>> GetChangeRequest(ChangeRequestsParameter p)
        {
            var ret = new ReturnList<ORGModel>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
               var req = DB.Organization.Where(w => w.RequestStatusNavigation.RequestDt >= p.FromDate && w.RequestStatusNavigation.RequestDt <= p.ToDate);
                req = req.WhereIf(!string.IsNullOrEmpty(p.RequestStatus), x => x.RequestStatusNavigation.RequestStatusNavigation.Lovcode == p.RequestStatus);
                ret.TotalRow = await req.Select(s => s.OrganizationId).CountAsync();
                req = req.PageBy(x => x.OrganizationId, p.Paging.PageNo, p.Paging.PageSize, true);
                ret.IsCompleted = true;

                var data = await req.Select(s => new ORGModel
                {
                    ORGAffiliate = new BasicData { Code = s.OrgaffiliateNavigation.OrganizationCode, Description = s.OrgaffiliateNavigation.OrganizationThname, ID = s.OrgaffiliateNavigation.OrganizationId },
                    OrganizeID = s.OrganizationId,
                    OrganizationCode = s.OrganizationCode,
                    OrganizationTHName = s.OrganizationThname,
                    ORGType = new BasicData { Code = s.OrgtypeNavigation.Lovcode, Description = s.OrgtypeNavigation.Lovvalue , ID = s.OrgtypeNavigation.Lovkey},
                    RequestStatus = new LogData {  
                        ActionCode = s.RequestStatusNavigation.RequestStatusNavigation.Lovcode,
                        Action = s.RequestStatusNavigation.RequestStatusNavigation.Lovvalue , ActionTime = s.RequestStatusNavigation.RequestDt,
                        ActionType = s.RequestStatusNavigation.RequestTypeNavigation.Lovvalue, ID = s.RequestStatusNavigation.RequestId, UserID = s.RequestStatusNavigation.UserId
                        }

                }).ToListAsync();
                ret.Data = data;
                //ret.Data = await req.Select(s => new ChangeRequests
                //{
                //    Ministry = s.OrgaffiliateNavigation.OrganizationThname,
                //    Organization = new BasicData { Code = s.OrganizationCode, ID = s.OrganizationId, Description = s.OrganizationThname },
                //    OrgType = s.OrgtypeNavigation.Lovvalue,
                //    RequestDateTime = s.RequestStatusNavigation.RequestDt, 
                //    RequestStatus = s.RequestStatusNavigation.RequestStatusNavigation.Lovvalue,
                //    UserID = s.RequestStatusNavigation.UserId,
                   
                //}).ToListAsync();
                foreach (var u in ret.Data)
                {
                    var user = await DB.Users.Where(w => w.Id == u.RequestStatus.UserID).FirstOrDefaultAsync();
                    if (user != null)
                    {
                        u.RequestStatus.UserName = user.TfirstName + " " + user.TlastName;
                    }
                }
                ret.PageSize = p.Paging.PageSize;
                ret.PageNo = p.Paging.PageNo;
                
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }
        public async Task<ReturnList<OrganizationList>> GetOrganizationList(OrganizationListParameter p)
        {
            var ret = new ReturnList<OrganizationList>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var req = DB.Organization.WhereIf(!string.IsNullOrEmpty(p.ORGTypeCode), w => w.OrgtypeNavigation.Lovcode == p.ORGTypeCode)
                    .WhereIf(!string.IsNullOrEmpty(p.Paging.SearchText), s => s.OrganizationThname.Contains(p.Paging.SearchText));
                ret.TotalRow = await req.Select(s => s.OrganizationId).CountAsync();

                req = req.PageBy(x => x.OrganizationId, p.Paging.PageNo, p.Paging.PageSize, true);
                ret.IsCompleted = true;

                ret.Data = await req.OrderBy(o => o.OrganizationCode).Select(s => new OrganizationList
                {
                    Ministry = s.OrgaffiliateNavigation.OrganizationThname,
                    Organization = new BasicData { Code = s.OrganizationCode, ID = s.OrganizationId, Description = s.OrganizationThname },
                    Status = "?? คืออะไร ??",
                    OrgStatus = s.OrgtoMany.Where(w => w.GroupCode == ServiceModels.Constants.LOVGroup.พรบ__หนี้สาธารณะ._LOVGroupCode
                    || w.GroupCode == ServiceModels.Constants.LOVGroup.พรบ__วินัยการเงินการคลัง._LOVGroupCode).Join(DB.CeLov, o => o.ManyId, c => c.Lovkey, (o,c) => c.Lovvalue).ToList(),
                    OrgType = new BasicData
                    {
                        Code = s.OrgtypeNavigation.Lovcode,
                        Description = s.OrgtypeNavigation.Lovvalue,
                        ID = s.OrgtypeNavigation.Lovkey
                    }
 
                }).ToListAsync();

                ret.PageSize = p.Paging.PageSize;
                ret.PageNo = p.Paging.PageNo;
                
            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }

        public async Task<ReturnObject<List<ORGCountByType>>> GetSummaryORG()
        {
            var ret = new ReturnObject<List<ORGCountByType>>(_msglocalizer);
            ret.IsCompleted = false;
            try
            {
                var sum = await DB.Organization.GroupBy(g => g.OrgtypeNavigation.Lovvalue).Select(s => new ORGCountByType {
                   OrgTypeName = s.Key,OrgCount = s.Count() }  ).ToListAsync();
                ret.Data = sum;
                ret.IsCompleted = true;

            }
            catch (Exception ex)
            {
                ret.AddError(ex);
            }
            return ret;
        }

 
        #endregion
    }
}
