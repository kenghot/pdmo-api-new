using mof.ServiceModels.Common.Generic;
using mof.ServiceModels.Helper;
using mof.ServiceModels.Project;
using mof.ServiceModels.Request;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace mof.ServiceModels.Plan
{
    /// <summary>
    /// รายละเอียดแผน 5 ปี (section a / b) 
    /// </summary>
    public class Plan5YDetail
    {
        /// <summary>
        /// รายละเอียดแผน ส่วนรายละเอียดแผน
        /// </summary>
        public PlanHeader PlanHeader { get; set; }
        /// <summary>
        /// Remark : ProjTypeCode = "ALL" หมายถึงยอดรวมทั้งหมด
        /// </summary>
        public List<ProjTypeLoanByYear> LoanByProjType { get; set; }
        ///// <summary>
        ///// ภาพรวม ความต้องการเงินกู้
        ///// </summary>
        //public List<TotalLoanByYear> LoanSummary {get;set;}
        ///// <summary>
        ///// ความต้องการเงินกู้ เพื่อโครงการลงทุน/พัฒนา
        ///// </summary>
        //public List<TotalLoanByYear> LoanForDEV { get; set; }
        ///// <summary>
        ///// ความต้องการเงินกู้ เพื่อโครงการทั่วไป
        ///// </summary>
        //public List<TotalLoanByYear> LoanForGeneral { get; set; }
        ///// <summary>
        ///// ความต้องการเงินกู้ เพื่อการดำเนินงาน
        ///// </summary>
        //public List<TotalLoanByYear> LoanForOperation { get; set; }
        /// <summary>
        /// อัตราแลกเปลี่ยน ณ.วันที่
        /// </summary>
        //public DateTime CurrencyRateDate { get; set; }
        /// <summary>
        ///  รายละเอียด อัตราแลกเปลี่ยน
        /// </summary>
        public CurrencyData CurrencyData { get; set; }
    }
    public class ProjTypeLoanByYear
    {
        /// <summary>
        /// Project type code
        /// </summary>
        public string ProjectTypeCode { get; set; }
        /// <summary>
        /// Project type name
        /// </summary>
        public string ProjectTypeName { get; set; }
        public List<TotalLoanByYear> LoanByYear { get; set; }
    }
    /// <summary>
    /// ความต้องการเงินกู้
    /// </summary>
    public class TotalLoanByYear
    {
        /// <summary>
        /// ปี พ.ศ.
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// ยอดรวมทั้งหมด (บาท)
        /// </summary>
        public decimal GrandTotal { get; set; }
        /// <summary>
        /// เงินกู้ในประเทศ
        /// </summary>
        public decimal LocalLoan { get; set; }
        /// <summary>
        /// เงินกู้ต่างประเทศ
        /// </summary>
        public decimal ForeignLoan { get; set; }
        /// <summary>
        /// เงินกู้อื่นๆ
        /// </summary>
        public decimal OtherLoan { get; set; }
    }
    public class PlanProjectListParameter
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

    public class PlanProjectList
    {
        public long PlanProjecID { get; set; }
        public long PlanID { get; set; }
        public long ProjectID { get; set; }
        public ProjectModel ProjectDetail { get; set; }
        public List<LoanPeriod> LoanByYears { get; set; }
        public string CoordinatorName { get; set; }
        public string CoordinatorPosition { get; set; }
        public string CoordinatorTel { get; set; }
        public string CoordinatorEmail { get; set; }
        public List<ProjectResolutionModel> Resolutions { get; set; } = new List<ProjectResolutionModel>();
        public List<ProjectResolutionModel> Files { get; set; } = new List<ProjectResolutionModel>();
    }
}
