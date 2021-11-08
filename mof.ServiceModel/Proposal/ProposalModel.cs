using System;
using mof.ServiceModels.Common.Generic;
using mof.ServiceModels.Plan;
using mof.ServiceModels.Request;

namespace mof.ServiceModels.Proposal
{
    public class ProposalModel
    {
        /// <summary>
        /// S182 secion a ข้อมูลข้อเสนอแผนเบื้องต้น 
        /// </summary>
        public PlanHeader ProposalHeader { get; set; }
        /// <summary>
        /// ข้อมูลเบื้องต้น - แผน 5 ปี
        /// </summary>
        public PlanHeader FiveYearPlan { get; set; }
        /// <summary>
        /// ข้อมูลเบื้องต้น - แผนการก่อหนี้ใหม่
        /// </summary>
        public PlanHeader NewDebtPlan { get; set; }
        /// <summary>
        /// ข้อมูลเบื้องต้น - แผนการบริหารหนี้เดิม
        /// </summary>
        public PlanHeader ExistingDebtPlan { get; set; }
        /// <summary>
        /// ข้อมูลเบื้องต้น - รายงานสถานะการเงิน
        /// </summary>
        public PlanHeader FinancialReport { get; set; }


    }
    public class ProposalHeader {
        /// <summary>
        /// Proposal ID
        /// </summary>
        public long ProposalID { get; set; }
        /// <summary>
        /// รหัสข้อเสนอแผน
        /// </summary>
        public string ProposalCode { get; set; }

        /// <summary>
        /// ข้อเสนอสำหรับปี 
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// ข้อมูลสำหรับ จัดทำร่างแผน / ปรับปรุงแผนครั้งที่ 1 / ปรับปรุงแผนครั้งที่ 2
        /// </summary>
        public string Release { get; set; }

        /// <summary>
        /// ข้อมูลการสร้าง
        /// </summary>
        public LogData CreateLog { get; set; }
        /// <summary>
        /// แก้ไขล่าสุด
        /// </summary>
        public LogData ChangeLog { get; set; }
        /// <summary>
        /// สถานะข้อเสนอ PC: จัดเตรียมข้อมูล / WT: รอตรวจสอบ  / DN : รับข้อเสนอ
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// หน่วยงาน
        /// </summary>
        public BasicData Organization { get; set; }
    }

    public class ProposalListModel
    {
        public long ProposalID { get; set; }
        public int Year { get; set; }
        public string Release { get; set; }
        public string FiveYearStatus { get; set; }
        public string NewDebtStatus { get; set; }
        public string ExistDebtStatus { get; set; }
        public string FinRepStatus { get; set; }
        public string ProposalStatus { get; set; }
    }

    public class ProposalListParameter
    {
        /// <summary>
        /// ปี พ.ศ.   nullคือ เอาทั้งหมด
        /// </summary>
        public int? StartYear { get; set; }
        /// <summary>
        /// "LOVGROUPCODE = PLR ไม่ใส่คือเอาหมด"
        /// </summary>
        public string PlanRelease { get; set; }
        /// <summary>
        /// รหัส แผน ถ้าใส่ null คือรวมทุกแผน ในปีที่ระบุ
        /// </summary>
        public long? ProposalID { get; set; }
        /// <summary>
        /// LOVGROUPCODE = PPSTATUS
        /// </summary>
        public string ProposalStatus { get; set; }
        /// <summary>
        /// org id : null คือเอาหมด
        /// </summary>
        public long? OrganizationID { get; set; }
        /// <summary>
        /// searchtxt จะ search จาก รหัส หรือ ชื่อโครงการ
        /// </summary>
        public Paging Paging { get; set; }
    }
}
