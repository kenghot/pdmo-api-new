using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel ;
using mof.ServiceModels.Common.Generic;
using System.ComponentModel.DataAnnotations;

namespace mof.ServiceModels.Common.Generic
{
    public enum eMessage
    {
        Activity,
        Agreement,
        AgreementActivityAlreadyMap,
        AgreementRestructureAlreadyMap,
        AllowOnlyStatus,
        AlreadyProposed,
        [Description("{codename}")]
        CodeIsNotValid,
        CodeIsRequired,
        [Description("{codename}")]
        CodeIsNotFound,
        Currency,
        [Description("{data}")]
        DataIsAlreadyExist,
        [Description("{data}")]
        DataIsNotFound,
        [Description("{data}")]
        DataIsNotAllow,
        /// <summary>
        /// start date greater than end date
        /// </summary>
        DateRangeError,
        [Description("{extension}")]
        FileTypeNotAllow,
        Interrest,
        [Description("{codename}")]
        IsRequired,
        Month,
        Organization,
        PINError,
        PINExpired,
        Plan,
        PleaseEnterValue,
        Proposal,
        /// <summary>
        /// Project {0} is already in plan {1}
        /// </summary>
        ProjIsAlreadyInPlan,
        /// <summary>
        /// Rate of {0} year {1} is not found
        /// </summary>
        RateOfYearIsNotFound,
        Restructure,
        [Description("{statusname}")]
        UserIsNotFound,
        User,


    }
    public class BasicData
    {
        public long? ID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsSelected { get; set; }
    }
    public class LogData
    {
        public long? ID { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public DateTime ActionTime { get; set; }
        public string ActionCode { get; set; }
        public string Action { get; set; }
        public string ActionType { get; set; }
        public string Remark { get; set; }
    }
    public class AttachFileData
    {
        public long? ID { get; set; }
        /// <summary>
        /// รายละเอียดหรือคำอธิบายไฟล์
        /// </summary>
        public string FileDetail { get; set; }
        /// <summary>
        /// ชื่อไฟล์
        /// </summary>
        [Required]
        public string FileName { get; set; }
        /// <summary>
        /// นามสกุลไฟล์
        /// </summary>
        [Required]
        public string FileExtension { get; set; }
        /// <summary>
        /// ขนาดไฟล์
        /// </summary>
        [Required]
        public long FileSize { get; set; }

        /// <summary>
        /// string base64 
        /// </summary>
        public byte[] FileData { get; set; }
        /// <summary>
        /// ภ้าเป็น true จะลบ file แต่ไม่ลบรายละเอียดต่างๆ
        /// </summary>
        public bool ClearFile { get; set; }
    }
    public class LOV
    {
        public long LOVKey { get; set; }
        public string LOVCode { get; set; }
        public string LOVValue { get; set; }
        public string LOVGroupCode { get; set; }
        public bool IsCanceled { get; set; }
        public string OrderNo { get; set; }
        public string ParentGroup { get; set; }
        public string ParentLOV { get; set; }
        public string Remark { get; set; }
    }
    public class LOVExtend
    {
        public long LOVExtendKey { get; set; }
        public LOV Lov { get; set; }
        public string LovExtendDetail { get; set; }
        public string LovExtendValue { get; set; }
    }
    public enum eGetPlanType
    {
        GetByID,
        GetSummary,
        Search
    };
    public enum eLOVExtendType
    {
        /// <summary>
        /// html 
        /// </summary>
        PDMOLAWREG,
        /// <summary>
        /// Detail
        /// </summary>
        PDMOLAWREGDT,
        /// <summary>
        /// สำหรับออกรายงาน กรอบวงเงิน รายประเภทหน่วยงาน
        /// </summary>
        PJTypeLimit

    }
    public class PDMORegulationData
    {
        public long LOVKey { get; set; }
        public string REGCode { get; set; }
        public string REGName { get; set; }
        public string REGDetail { get; set; }
        public string REGHtml { get; set; }
    }
    public class CommonConstants
    {
        #region ftype

        public static string[] FtypeGroupPaid
        {
            get
            {
                return new string[] {
                    ServiceModels.Constants.LOVGroup.Transaction_CashFlow_Type_from_GF.ชำระเงินต้น,
                    ServiceModels.Constants.LOVGroup.Transaction_CashFlow_Type_from_GF.ชำระดอกเบี้ย,
                    ServiceModels.Constants.LOVGroup.Transaction_CashFlow_Type_from_GF.Installment,
                };
            }
        }
        public static string[] FtypeFinishStatus
        {
            get
            {
                return new string[]
                {
                     "ทำการผ่านรายการเสร็จแล้ว" ,  "ทำเครื่องหมายเพื่อผ่านรายการ" 
                };
            }
        }
        #endregion
 
    }


}
