using System;
using System.Collections.Generic;
using System.Text;

namespace mof.ServiceModels.Reports
{
    public class NewDebtPlanRep
    {
        public DateTime RepDate { get; set; }
        public string OrganizationName { get; set; }
        public string Ministry { get; set; }
        public string Year { get; set; }

        public List<NewDebtPlanRepItem> Items { get; set; } = new List<NewDebtPlanRepItem>();
        public List<ProjectDisbSign> DisbSigns { get; set; } = new List<ProjectDisbSign>();
        public List<ProjLoan> ProjLoans { get; set; } = new List<ProjLoan>();
        public List<ProjLoan> ProjLoans2 { get; set; } = new List<ProjLoan>();
        //public List<ProjLoanExpense> Expenses { get; set; } = new List<ProjLoanExpense>();
        //public List<ActivityLoan> LActivityLoans { get; set; } = new List<ActivityLoan>();
        //public List<ActivityLoan> FActivityLoans { get; set; } = new List<ActivityLoan>();
    }
    public class ProjLoan
    {
        public string Year { get; set; }
        public string ProjectTHName { get; set; }
        public string ProjectENName { get; set; }
        public string ProjOjective { get; set; }
        public decimal Firr { get; set; }
        public decimal Eirr { get; set; }
        public string Duration { get; set; }
        public decimal LMatAmt { get; set; }
        public decimal LMatPer { get; set; }
        public decimal FMatAmt { get; set; }
        public decimal FMatPer { get; set; }
        #region การดำเนินงานในปีงบประมาณ
        public decimal Budget { get; set; }
        public decimal Revernue { get; set; }
        public decimal LInvestLoan { get; set; }
        public decimal  FInvestLoan { get; set; }
        public string FInvestLoanCur { get; set; }
        public string LoanFinDeptName { get; set; }
        public decimal LLoanFinDept { get; set; }
        public decimal FLoanFinDept { get; set; }
        public string FLoanFinDeptCur { get; set; }
        public string OtherLoanText { get; set; }
        public decimal OtherLoan { get; set; }


        #endregion
        //public List<TestGroup> TestGroups { get; set; } = new List<TestGroup>();

        public List<ProjLoanExpense> Expenses { get; set; } = new List<ProjLoanExpense>();
        public List<ActivityLoan> LActivityLoans { get; set; } = new List<ActivityLoan>();
        public List<ActivityLoan> FActivityLoans { get; set; } = new List<ActivityLoan>();
        public List<LFActivityLoan> LFLActivityLoans { get; set; } = new List<LFActivityLoan>();

    }
    public class LFActivityLoan
    {
        public string ActivityName { get; set; }
        public decimal LInvest { get; set; }
        public decimal LFinDept { get; set; }
        public decimal FInvest { get; set; }
        public decimal FFinDept { get; set; }
    }
    //public class TestGroup
    //{
    //    public List<ProjLoanExpense> Expenses { get; set; } = new List<ProjLoanExpense>();
    //    public List<ActivityLoan> LActivityLoans { get; set; } = new List<ActivityLoan>();
    //    public List<ActivityLoan> FActivityLoans { get; set; } = new List<ActivityLoan>();

    //}
    public class ProjLoanExpense
    {
        public string ActivityName { get; set; }
        public decimal LResolution { get; set; }
        public decimal FResolution { get; set; }
        public decimal LContract { get; set; }
        public decimal FContract { get; set; }
        public decimal LInvest { get; set; }
        public decimal LFinDept { get; set; }
        public decimal FInvest { get; set; }
        public decimal FFinDept { get; set; }
        public decimal Budget { get; set; }
        public decimal Revernue { get; set; }
        public decimal SignedLoan { get; set; }
        public decimal DisburseLoan { get; set; }
        public decimal Other { get; set; }
        public decimal Total { get; set; }
    }
    public class ActivityLoan
    {
        public string ActivityName { get; set; }
        public decimal Invest { get; set; }
        public decimal FinDept { get; set; }
 
    }
    public class ProjectDisbSign
    {
        public string ProjectTHName { get; set; }
        public List<DisbSign> Activities { get; set; } = new List<DisbSign>();
        public List<DisbSign> ActivitiesF { get; set; } = new List<DisbSign>();
    }
    public class DisbSign
    {
        public string LoanSource { get; set; }
        public string LoanSourceText { get; set; }
        public string ActivityName { get; set; }
        public string LoanType { get; set; }
        public decimal LoanAmt { get; set; }
        public string Currency { get; set; }
        public MonthAmt Sign { get; set; } = new MonthAmt();
        public MonthAmt Disb { get; set; } = new MonthAmt();
        
    }
    public class MonthAmt
    {
        public decimal M1 { get; set; }
        public decimal M2 { get; set; }
        public decimal M3 { get; set; }
        public decimal M4 { get; set; }
        public decimal M5 { get; set; }
        public decimal M6 { get; set; }
        public decimal M7 { get; set; }

        public decimal M8 { get; set; }
        public decimal M9 { get; set; }
        public decimal M10 { get; set; }
        public decimal M11 { get; set; }
        public decimal M12 { get; set; }
        public decimal Total {  get
            {
                return M1 + M2 + M3 + M4 + M5 + M6 + M7 + M8 + M9 + M10 + M11 + M12;
            } }
    }
    public class NewDebtPlanRepItem
    {
        public long ProjectID { get; set; }
        public string ProjectTHName { get; set; }
        /// <summary>
        /// ใหม่
        /// </summary>
        public string IsNew { get; set; }
        /// <summary>
        /// ต่อเนื่อง
        /// </summary>
        public string IsCont { get; set; }
        /// <summary>
        /// ยุทธศาสตร์ชาติ
        /// </summary>
        public string IsStrategic { get; set; }
        /// <summary>
        /// แผนการปฏิรูป
        /// </summary>
        public string IsReform { get; set; }
        /// <summary>
        /// แผนพัฒนาเศรษฐกิจ  และสังคมแห่งชาติ
        /// </summary>
        public string IsDevPlan { get; set; }
        /// <summary>
        /// รายงานการศึกษาความเหมาะสม  ด้านเทคนิค เศรษฐกิจ สังคม การเงิน
        /// </summary>
        public string IsEduRep { get; set; }
        /// <summary>
        /// รายงานการวิเคราะห์ ผลกระทบสิ่งแวดล้อม
        /// </summary>
        public string IsAnalystRep { get; set; }
        /// <summary>
        /// คณะกรรมการ ของรัฐวิสาหกิจ
        /// </summary>
        public string StateEntAppv { get; set; }
        /// <summary>
        /// กระทรวง ต้นสังกัด
        /// </summary>
        public string MinistryAppv { get; set; }
        /// <summary>
        /// สศช.
        /// </summary>
        public string PDMOAppv { get; set; }
        /// <summary>
        /// ครม.
        /// </summary>
        public string GOVAppr { get; set; }
        /// <summary>
        /// เงินกู้ต่างประเทศ	cur
        /// </summary>
        public string FLoanCur { get; set; }
        /// <summary>
        /// เงินกู้ต่างประเทศ	thb
        /// </summary>
        public decimal FLoanTHB { get; set; }
        /// <summary>
        /// เงินกู้ในประเทศ ลงทุน ในโครงการพัฒนา
        /// </summary>
        public decimal LLoanDev { get; set; }
        /// <summary>
        /// เงินกู้ในประเทศ ดำเนิน โครงการ 
        /// </summary>
        public decimal LLoanProcess { get; set; }
        /// <summary>
        /// เงินกู้ในประเทศ เงินทุนหมุนเวียน
        /// </summary>
        public decimal LLoanRevolv { get; set; }
        /// <summary>
        /// เงินกู้ต่อจากกระทรวงการคลัง ในประเทศ
        /// </summary>
        public decimal LFIN { get; set; }
        /// <summary>
        /// เงินกู้ต่อจากกระทรวงการคลัง ต่างประเทศ
        /// </summary>
        public string FFIN { get; set; }

    }
}
