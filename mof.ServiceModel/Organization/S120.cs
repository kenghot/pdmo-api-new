using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using mof.ServiceModels.Common.Generic;
using mof.ServiceModels.Request;

namespace mof.ServiceModels.Organization
{
    #region ChangeRequest

    public class ChangeRequestsParameter
    {
        /// <summary>
        /// ข้อมูลตั้งแต่วันที่
        /// </summary>
        [Required]
        public DateTime FromDate { get; set; }
        /// <summary>
        /// ข้อมูลถึงวันที่
        /// </summary>
        [Required]
        public DateTime ToDate { get; set; }
        /// <summary>
        /// สถานะ (ถ้าไม่ใส่คือเอาทั้งหนด) LVGroupCode = RQSTATUS
        /// </summary>
        public string RequestStatus { get; set; }
        /// <summary>
        /// Paging
        /// </summary>
        public Paging Paging { get; set; }
    }

    /// <summary>
    /// รายการคำขอปรับปรุงข้อมูลหน่วยงาน
    /// </summary>
    public class ChangeRequests
    {
        /// <summary>
        /// วันที่มีการยื่นคำร้อง
        /// </summary>
        public DateTime RequestDateTime { get; set; }
        /// <summary>
        /// หน่วยงานที่มีการยื่นคำร้อง
        /// </summary>
        public BasicData Organization { get; set; }
        /// <summary>
        /// กระทรวงที่มีการยื่นคำร้อง
        /// </summary>
        public string Ministry { get; set; }
        /// <summary>
        /// ประเภทของหน่วยงาน
        /// </summary>
        public string OrgType { get; set; }
        /// <summary>
        /// รหัสผู้จัดทำ
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// ผู้จัดทำ
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// สถานะ
        /// </summary>
        public string RequestStatus { get; set; }
    }
    #endregion

    #region OrganizatinList
    public class OrganizationListParameter
    {

        /// <summary>
        /// ประเภาทหน่วย (ถ้าไม่ใส่คือเอาทั้งหนด) LVGroupCode = ORGTYPE
        /// </summary>
        public string ORGTypeCode { get; set; }
        /// <summary>
        /// Paging (SearchText ไว้สำหรับ หาชื่อหน่วยงาน)
        /// </summary>
        public Paging Paging { get; set; }
    }
    /// <summary>
    /// แสดงรายชื่อหน่วยงาน
    /// </summary>
    public class OrganizationList
    {

        /// <summary>
        /// หน่วยงานที่มีการยื่นคำร้อง
        /// </summary>
        public BasicData Organization { get; set; }
        /// <summary>
        /// กระทรวงที่มีการยื่นคำร้อง
        /// </summary>
        public string Ministry { get; set; }
        /// <summary>
        /// สถานะตามกฎหมาย
        /// </summary>
        public List<string> OrgStatus { get; set; }
        /// <summary>
        /// สถานะ
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// ประเภทหน่วยงาน
        /// </summary>
        public BasicData OrgType { get; set; }
    }
    #endregion
}
