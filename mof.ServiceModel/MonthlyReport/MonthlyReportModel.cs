using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using mof.ServiceModels.Plan;
using mof.ServiceModels.Request;

namespace mof.ServiceModels.MonthlyReport
{
    public class MonthlyReportModel
    {
        /// <summary>
        /// S162 secion a ข้อมูลรายงานเบื้องต้น 
        /// </summary>
        public PlanHeader PlanHeader { get; set; }
        /// <summary>
        /// ข้อมูลเบื้องต้น - แผนการก่อหนี้ใหม่
        /// </summary>
        public PlanHeader NewDebtPlan { get; set; }
        /// <summary>
        /// ข้อมูลรายการ - ผลก่อหนี้ใหม่
        /// </summary>
        public List<NewDebtPlanDetails> NewDebtPlanReportDetails { get; set; }
        /// <summary>
        /// ข้อมูลเบื้องต้น - แผนการบริหารหนี้เดิม
        /// </summary>
        public PlanHeader ExistingDebtPlan { get; set; }
        /// <summary>
        /// ข้อมูลรายการ - ผลการบริหารหนี้เดิม
        /// </summary>
        public List<ExistingDebtPlanDetails> ExistingDebtPlanReportDetails { get; set; }

    }
    public class MonthlyReportParameter
    {
        /// <summary>
        /// ปี พ.ศ. เริ่มต้น
        /// </summary>
        [Required]
        public int StartYear { get; set; }
        /// <summary>
        /// "LOVGROUPCODE = PLR ไม่ใส่คือเอาหมด"
        /// </summary>
        public string PlanRelease { get; set; }
        /// <summary>
        /// รหัส แผน ถ้าใส่ null คือรวมทุกแผน ในปีที่ระบุ
        /// </summary>
        public long? PlanID { get; set; }
        /// <summary>
        /// ปรเภทโครงการ LOVGroupCode = PJTYPE ไม่ใส่คือเอาหมด
        /// </summary>
        public string ProjectType { get; set; }
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
