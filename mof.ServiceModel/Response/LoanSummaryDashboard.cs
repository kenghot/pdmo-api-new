using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
namespace mof.ServiceModels.Response
{
    /// <summary>
    /// S100 : Loan summary dashboard
    /// </summary>
    public class LoanSummaryDashboard
    {


        #region "Section A"
 
        /// <summary>
        /// S100 section A Property 1 :	วันที่ล่าสุดที่มีข้อมูลอัตราแลกเปลี่ยน
        /// </summary>
        public DateTime CurrencyRateDate { get; set; }

        /// <summary>
        /// S100 Section A Data A : สรุปยอดหนี้คงค้างของหน่วยงาน
        /// </summary>

        public decimal OutstandingDept { get; set; }
        /// <summary>
        /// S100 Section A Data B : สรุปยอดหนี้ครบกำหนดในปีงบ ประมาณปัจจุบัน 
        /// </summary>
        public Dictionary<string,decimal> MaturityDebt { get; set; }
        /// <summary>
        /// S100 Section A Data C : สัดส่วนหนี้คงค้าง
        /// </summary>
        public List<GraphData> DebtTermRatioGraph { get; set; }
        /// <summary>
        /// S100 Section A Data D : แหล่งเงินกู้
        /// </summary>
        public List<GraphData> DebtSourceGraph { get; set; }
        /// <summary>
        /// S100 Section A Data E : หนี้ครบกำหนดในระยะเวลา 5 ปี
        /// </summary>
        public List<GraphData> MaturityDebt5Y { get; set; }

        #endregion

        #region "Section B"
        /// <summary>
        /// S100 Section B Property 1 : ปีที่เริ่มของ แผนโครงการเงินกู้ 5 ปี  e.g. : 2562
        /// </summary>
        public int LoanPlan5YStart { get; set; }
        /// <summary>
        /// S100 Section B Property 1 : ปีที่สิ้นสุดของ แผนโครงการเงินกู้ 5 ปี  e.g. : 2562
        /// </summary>
        public int LoanPlan5YEnd { get; set; }
        /// <summary>
        /// S100 Section B Property 1 : วันกำหนดส่ง แผนโครงการเงินกู้ 5 ปี   
        /// </summary>
        public DateTime LoanPlan5YDue { get; set; }

        /// <summary>
        /// S100 Section B Property 2 : แสดงปีของแผนบริหารหนี้สาธารณะประจำปีงบประมาณ xxxx  e.g. : 2563
        /// </summary>
        public int PublicLoanPlanYear { get; set; }

        /// <summary>
        /// S100 Section B Property 2 : แสดวันกำหนดส่ง แผนบริหารหนี้สาธารณะประจำปีงบประมาณ  
        /// </summary>
        public DateTime PublicLoanPlanDeu { get; set; }
        /// <summary>
        /// S100 Section B Property 5 : ปรับแผนบริหารหนี้สาธารณะ
        /// </summary>
        public List<AdjustPublicLoan> AdjustPublicLoanPlan { get; set; }
        /// <summary>
        /// S100 Section B Property 7 : รายงานแผลการลงนามสัญญาและการเบิกเงินกู้ 
        /// </summary>
        public List<SignAndReimbursePlan> SignAndReimburseReport { get; set; }
        #endregion

        #region "C"
        /// <summary>
        /// S100 Section C Property 3 : ชื่อของผู้อนุมัติการแก้ไขข้อมูลหน่วยงานล่าสุด
        /// </summary>
        public string EditOrgApprover { get; set; }
        /// <summary>
        /// S100 Section C Property 3 : วันที่ อนุมัติการแก้ไขข้อมูลหน่วยงานล่าสุด
        /// </summary>
        public DateTime EditOrgApprovedDate { get; set; }
        /// <summary>
        /// S100 Section C Data A : ประเภทหน่วยงานตามกฎหมาย พรบ. หนี้สาธารณะ
        /// </summary>
        public List<string> OrgTypePublicDebtAct { get; set; }
        /// <summary>
        /// S100 Section C Data B : ประเภทหน่วยงานตามกฎหมาย พรบ.วินัยการเงินการคลังของรัฐ
        /// </summary>
        public List<string> OrgTypeFinancialAct { get; set; }
        /// <summary>
        /// S100 Section C Data C : อำนาจในการกู้เงิน
        /// </summary>
        public bool HasLoanAuthority { get; set; }
        /// <summary>
        /// S100 Section C Data D : สถานะการนับเป็นหนี้สาธารณะตามฐานข้อมูลของกลุ่มกฎหมาย สบน.
        /// </summary>
        public bool PublicDebtStatus { get; set; }
        #endregion

        #region "D"
      
        //public List<TechicalAssistance> TechicalAssistanceAnnouce { get; set; }
        #endregion
    }

    /// <summary>
    /// S100 Section B Property 5 : ข้อมูลสำหรับแสดงผลของกราฟ
    /// </summary>
    public class GraphData
    {
        public string Group1 { get; set; }
        public string Group2 { get; set; }
        public decimal Amount { get; set; }
        public decimal Percent { get; set; }
    }
    /// <summary>
    /// รายการ ปรับแผนบริหารหนี้สาธารณะ
    /// </summary>
    public class AdjustPublicLoan
    {
        /// <summary>
        /// แผน ปี xxxx
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// รอบที่
        /// </summary>
        public int Session { get; set; }
        /// <summary>
        /// สถานะ
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// กำหนดส่ง
        /// </summary>
        public DateTime DueDate { get; set; }
        /// <summary>
        /// จบเรียบร้อย
        /// </summary>
        public bool IsCompleted { get; set; }
    }
    /// <summary>
    /// รายงานแผลการลงนามสัญญาและการเบิกเงินกู้
    /// </summary>
    public class SignAndReimbursePlan
    {
        /// <summary>
        ///  เดือน 
        /// </summary>
        public DateTime Month { get; set; }
        /// <summary>
        /// มีแผนลงนาม
        /// </summary>
        public bool HasSigned { get; set; }
        /// <summary>
        /// มีแผนการเบิก
        /// </summary>
        public bool HasReimbursed { get; set; }
        /// <summary>
        /// ส่งรายงานแล้วหรือยัง
        /// </summary>
        public bool IsSentReport { get; set; }
    }
    /// <summary>
    /// S100 Section D : แสดงประกาศความช่วยเหลือทางวิชาการ 
    /// </summary>
    public class TechicalAssistance
    {
        /// <summary>
        /// S100 Section D Data A : รูปภาพประกาศความช่วยเหลือทางวิชาการ
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// S100 Section D Data B : รายการประกาศความช่วยเหลือทางวิชาการ
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// S100 Section D Data C : ระยะเวลาประกาศความช่วยเหลือทางวิชาการ
        /// </summary>
        public DateTime ExpiryDate { get; set; }
        /// <summary>
        /// S100 Section D Data D : รายละเอียดประกาศความช่วยเหลือทางวิชาการ
        /// </summary>
        public string Detail { get; set; }
        /// <summary>
        /// S100 Section D Data E : จำนวนผู้สนใจ
        /// </summary>
        public int Interstors { get; set; }
    }
}
