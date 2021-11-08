using System;
using System.Collections.Generic;
using mof.ServiceModels.Common.Generic;
using mof.ServiceModels.Helper;
using mof.ServiceModels.Project;

namespace mof.ServiceModels.Plan
{
    /// <summary>
    /// S142 รายละเอียดแผนการก่อหนี้ใหม่ 
    /// </summary>
    public class NewDebtPlanModel
    {
        /// <summary>
        /// S142 secion a ข้อมูลแผนเบื้องต้น 
        /// </summary>
        public PlanHeader PlanHeader { get; set; }

        /// <summary>
        /// S142 section b ข้อมูลสรุปภาพรวมแผนการก่อหนี้ใหม่ 
        /// </summary>
        public NewDebtPlanSummary PlanSummary { get; set; }
        ///
        /// <summary>
        /// S142 section c รายการโครงการที่ผูกกับแผน 
        /// </summary>
        public List<NewDebtPlanDetails> PlanDetails { get; set; }

    }
    /// <summary>
    /// S142 secion b ข้อมูลแผนเบื้องต้น 
    /// </summary>
    public class NewDebtPlanSummary
    {
        /// <summary>
        /// ภาพรวมตามแหล่งเงินกู้
        /// </summary>
        public List<LoanSource> OverAllByLoanSource { get; set; }
        /// <summary>
        /// ภาพรวมตามลักษณะการกู้
        /// </summary>
        public List<LoanType> OverAllByLoanType { get; set; }
        /// <summary>
        /// ภาพรวมตามวัตถุประสงค์การกู้
        /// </summary>
        public List<ProjectTypeLoanSummary> OverAllByProjectType { get; set; }
         /// <summary>
        ///  รายละเอียด อัตราแลกเปลี่ยน
        /// </summary>
        public CurrencyData CurrencyData { get; set; }
    }
    public class ProjectTypeLoanSummary
    {
        /// <summary>
        /// ประเภทโครงการ(วัตุประสงค์การกู้) LOVGroupCode = PJTYPE
        /// </summary>
        public string ProjectType { get; set; }
        public decimal ProjectTypeSumAmount { get; set; }
        /// <summary>
        /// ตัวเลขสรุปแยกตามลักษณะการกู้ของประเภทโครงการ
        /// </summary>
        public List<LoanType> LoanTypeSumAmount { get; set; }
    }
    /// <summary>
    /// S142 secion c ข้อมูลแผนเบื้องต้น 
    /// </summary>
    public class NewDebtPlanDetails
    {
        /// <summary>
        /// ถ้าค่า ไม่เท่ากับ Normal จะ edit ไม่ได้
        /// </summary>
        public string PlanProjType { get; set; }
        /// <summary>
        /// !!!!!!  This is PlanProjectId !!!!!!!!!!!!
        /// </summary>
        public long PlanID { get; set; }
        /// <summary>
        /// รหัสโครงการ
        /// </summary>
        public long ProjectID { get; set; }
        ///// <summary>
        ///// รายละเอียดโครงการ
        ///// </summary>
        //public ProjectModel ProjectDetail { get; set; }
        public string ProjectCode { get; set; }
        /// <summary>
        /// ชื่อโครงการ
        /// </summary>
        public string ProjectTHName { get; set; }
        /// <summary>
        /// รายละเอียดหน่วยงาน
        /// </summary>
        public BasicData Organization { get; set; }
        /// <summary>
        /// true = โครงการใหม่ , false = โครงการต่อเนื่อง
        /// </summary>
        public bool IsNewProject { get; set; }
        /// <summary>
        /// สถานะความพร้อมในการกู้
        /// </summary>
        public int PdmoAgreement { get; set; }
        /// <summary>
        /// ข้อมูลจากแผน5ปี เฉพาะปีงบประมาณที่กำลังทำแผนปี
        /// </summary>
        public LoanPeriod FromFiveYearPlan { get; set; }
        /// <summary>
        /// ข้อมูลสรุปตัวเลขแผนปีโดยรวมทุกลักษณะการกู้
        /// </summary>
        public LoanPeriod YearPlanSummary { get; set; }
        /// <summary>
        /// M144 รายละเอียดแผนการดำเนินงานและแผนการกู้แยกตามลักษณะงาน
        /// </summary>
        public List<ActivityPlan> ActivityPlans { get; set; }
        /// <summary>
        /// M144T5 สรุปแผนการดำเนินงาน
        /// </summary>
        public ActivityPlanSummary ActivityPlanSummary { get; set; }
        /// <summary>
        /// ไม่ต้องขออนุมัติครม. ภายใต้กรอบแผน
        /// </summary>
        public bool? isNotRequiredApproval { get; set; }
        /// <summary>
        ///  ประเภทโครงการ(วัตุประสงค์การกู้) LOVGroupCode = PJTYPE 
        /// </summary>
        public string ProjectType { get; set; }

        /// <summary>
        ///  text display ประเภทโครงการ(วัตุประสงค์การกู้) LOVGroupCode = PJTYPE 
        /// </summary>
        public string ProjectTypeLabel { get; set; }

        /// <summary>
        /// text display ลักษณะการกู้  >> if( project นี้มีลักษณะการกู้เป็น กู้ต่อจาก กค. ) return กู้ต่อจาก กค.
        ///                                        >>  else if( project นี้มีลักษณะการกู้เป็น กู้มาให้กู้ต่อ ) return กู้มาให้กู้ต่อ  
        ///                                        >>  else return กู้ตรง
        /// </summary>
        public string LoanTypeLabel { get; set; }
        /// <summary>
        /// GFcode
        /// </summary>
        public string GFCode { get; set; }
        public string MasterAgreement { get; set; }

    }
    /// <summary>
    /// M144 รายละเอียดแผนปีแยกตามลักษณะงาน
    /// </summary>
    public class ActivityPlan
    {
        /// <summary>
        ///  รายละเอียดลักษณะงาน
        /// </summary>
        public ActivityData Activity { get; set; }
        /// <summary>
        /// แผนการดำเนินงานขตามลักษณะงาน ทั้งปีงบประมาณ Month = 0 
        /// * total ของ LoanTypePlans ใน SignedLoan
        /// </summary>
        public SaveProceed ActivityPlanDetail { get; set; }
        /// <summary>
        /// M143/M144T3V1 แผนการกู้ตามลักษณะงานและลักษณะการกู้ ทั้งปีงบประมาณ 
        /// </summary>
        public List<LoanTypePlan> LoanTypePlans { get; set; }
        /// <summary>
        /// M145/M144T3V2 แผนการกู้ตามลักษณะงานและลักษณะการกู้ ทั้งปีงบประมาณ 
        /// </summary>
        public List<LoanProcessPlan> LoanProcessPlan { get; set; }
       


    }
    /// <summary>
    /// M143/M144T3 แผนการกู้ตามลักษณะการกู้ ทั้งปีงบประมาณ 
    /// </summary>
    public class LoanTypePlan
    {
        /// <summary>
        /// ลักษณะการกู้
        /// </summary>
        public LoanType LoanType { get; set; }
        /// <summary>
        /// ข้อมูลสำหรับบันทึกแผนการกู้แยกตามแหล่งเงินกู้
        /// </summary>
        public List<LoanSource> LoanSourcePlans { get; set; }
    }
    /// <summary>
    /// แผนการกู้ตามแหล่งเงินกู้  ทั้งปีงบประมาณ 
    /// </summary>
    public class LoanSourcePlan
    {
        /// <summary>
        /// ข้อมูลสำหรับบันทึกแผนการกู้แยกตามแหล่งเงินกู้
        /// </summary>
        public LoanSource LoanSource { get; set; }
        /// <summary>
        /// ลักษณะการกู้
        /// </summary>
        public List<LoanType> LoanTypePlans { get; set; }
    }
    /// <summary>
    /// M145/M144T3V2 แผนการกู้ตามลักษณะงานและลักษณะการกู้ ทั้งปีงบประมาณ 
    /// </summary>
    public class LoanProcessPlan
    {
        /// <summary>
        /// ข้อมูลสรุปยอดรวมทุกลักษณะการกู้ แยกตามแหล่งเงินกู้
        /// </summary>
        public LoanSource LoanSource { get; set; }
        /// <summary>
        /// รายละเอียดรายเดือน
        /// </summary>
        public List<LoanProcessPlanDetail> LoanProcessPlanDetails { get; set; }
        /// <summary>
        /// สรุปยอดลงนาม
        /// </summary>
        public decimal SignedLoanSumAmount { get; set; }
        /// <summary>
        /// สรุปยอดเบิกจ่าย
        /// </summary>
        public decimal DisburseLoanSumAmount { get; set; }
    }
    public class LoanProcessPlanDetail
    {
        /// <summary>
        /// เดือน 1-12 (1 = มกราคม) 
        /// </summary>
        public int Month { get; set; }
        /// <summary>
        /// ข้อมูลสำหรับบันทึก เงินกู้ลงนาม
        /// </summary>
        public decimal SignedLoan { get; set; }
        /// <summary>
        /// ข้อมูลสำหรับบันทึก เงินกู้เบิกจ่าย
        /// </summary>
        public decimal DisburseLoan { get; set; }
    }
    public class ActivityPlanSummary
    {
        /// <summary>
        /// เงินงบประมาณ
        /// </summary>
        public decimal Budget { get; set; }
        /// <summary>
        /// เงินรายได้
        /// </summary>
        public decimal Revernue { get; set; }
        /// <summary>
        /// เงินกู้
        /// </summary>
        public decimal Loan { get; set; }
        public List<LoanSourcePlan> LoanSourcePlans { get; set; }
        /// <summary>
        /// เงินจากแหล่งอื่น
        /// </summary>
        public decimal Other { get; set; }
        /// <summary>
        /// ยอดรวม
        /// </summary>
        public decimal Total { get; set; }
    }
    public class NewDebtPlanActList
    {

        public long PlanActID { get; set; }
        /// <summary>
        /// รายการ
        /// </summary>
        public string ProjectTHName { get; set; }
        /// <summary>
        /// รายละเอียด
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// แหล่งเงินกู้ & วงเงินตามแผน
        /// </summary>
        public List<LoanSource> LoanSource { get; set; }
        
    }
}
