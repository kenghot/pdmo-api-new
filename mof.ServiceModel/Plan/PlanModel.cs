using mof.ServiceModels.Common.Generic;
using mof.ServiceModels.Request;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace mof.ServiceModels.Plan
{
    public class CreatePlanParameter
    {
        /// <summary>
        /// แผนสำหรับ LOVGroupCode = PLR
        /// </summary>
        [Required]
        public string PlanRelease { get; set; }
        /// <summary>
        /// ปีเริ่มต้น
        /// </summary>
        [Required]
        public int StartYear { get; set; }
        /// <summary>
        /// รหัสหน่วยงาน
        /// </summary>
        [Required]
        public long OrganizationID { get; set; }
        /// <summary>
        /// เดือน 1-12 (1 = มกราคม)
        /// </summary>
        public int? Month { get; set; }
    }
    public class PlanListParameter
    {
        /// <summary>
        /// รหัสหน่วยงาน ถ้าเป็น null คือเอาทั้งหมด
        /// </summary>
        public long? OrganizationID { get; set; }
        /// <summary>
        /// สถานะของแผน (ถ้าไม่ใส่คือเอาทั้งหมด) LOVGroupCode = PPSTATUS หรือ PPPSTATUS (สำหรับ proposal)
        /// </summary>
        public string PlanStatus { get; set; }
        /// <summary>
        /// จัดทำครั้งที่ LOVGroupCode = PLR
        /// </summary>
        public string PlanRelease { get; set; }
        /// <summary>
        /// ปี (null คือเอาทั้งหมด)
        /// </summary>
        public int? Year { get; set; }
        /// <summary>
        /// searchtext สำหรับกรองชื่อหน่วยงาน
        /// </summary>
        public Paging Paging { get; set; }
    }
    public class PlanHeader
    {
        /// <summary>
        /// Plan ID
        /// </summary>
        public long PlanID { get; set; }
        /// <summary>
        /// หมายเลขแผน
        /// </summary>
        public string PlanCode { get; set; }
        /// <summary>
        /// แผนสำหรับปี (ค.ศ.)
        /// </summary>
        public int PlanYear { get; set; }
        /// <summary>
        /// e.g. "2562-2566"
        /// </summary>
        public string PlanYearTxt { get; set; }
        /// <summary>
        /// เดือน 1-12 ( 1 = มกราคม)
        /// </summary>
        public int? Month { get; set; }
        /// <summary>
        /// แผนสำหรับ e.g. ปรับปรุงแผนครั้งที่ x
        /// </summary>
        public string PlanRelease { get; set; }
        /// <summary>
        /// ข้อมูลการสร้าง
        /// </summary>
        public LogData CreateLog { get; set; }
        /// <summary>
        /// แก้ไขล่าสุด
        /// </summary>
        public LogData ChangeLog { get; set; }
        /// <summary>
        /// สถานะ
        /// </summary>
        public string PlanStatus { get; set; }
        /// <summary>
        /// หน่วยงาน
        /// </summary>
        public BasicData Organization { get; set; }
        /// <summary>
        /// ข้อเสนอแผนน
        /// </summary>
        public List<BasicData> Proposals { get; set; }
        /// <summary>
        /// สถานะแผน
        /// </summary>.
        public LogData PlanStatusLog { get; set; }
        /// <summary>
        /// ความเห็นเจ้าหน้าที่  รหัสถานะของแผน (LOVGroupCode = 'PPStatus')
        /// </summary>
        public ReviewComment ReviewComment;
    }
    public class ReviewComment {
        /// <summary>
        /// S182 สำหรับเจ้าหน้าที่ ผลการตรวจสอบ
        /// </summary>
        public DateTime ReviewDate { get; set; }
        /// <summary>
        /// S182 สำหรับเจ้าหน้าที่ ผลการตรวจสอบ รหัสถานะของแผน (LOVGroupCode = 'PPStatus')
        /// </summary>
        public string ResultStatus { get; set; }
        /// <summary>
        /// S182 สำหรับเจ้าหน้าที่ ผลการตรวจสอบ
        /// </summary>
        public string ResultDescription { get; set; }
        /// <summary>
        /// S182 สำหรับเจ้าหน้าที่ ความเห็นเจ้าหน้าที่
        /// </summary>
        public string Comment { get; set; }
    }

}
