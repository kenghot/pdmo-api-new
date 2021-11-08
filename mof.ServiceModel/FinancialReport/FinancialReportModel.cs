using System;
using System.Collections.Generic;
using mof.ServiceModels.Common.Generic;
using mof.ServiceModels.Plan;
using mof.ServiceModels.Agreement;

namespace mof.ServiceModels.FinancialReport
{
    public class FinancialReportModel
    {
        /// <summary>
        /// S162 secion a ข้อมูลรายงานเบื้องต้น 
        /// </summary>
        public PlanHeader PlanHeader { get; set; }

        /// <summary>
        /// S162 secion b  งบดุล
        /// </summary>
        public List<BalanceSheet> BalanceSheet { get; set; }
        /// <summary>
        /// S162 secion b  งบกำไรขาดทุน
        /// </summary>
        public List<IncomeStatement> IncomeStatements { get; set; }
        /// <summary>
        /// S162 secion b  งบกระแสเงินสด
        /// </summary>
        public List<CashFlow> CashFlows { get; set; }
        /// <summary>
        /// S162 secion b  ภาระหนี้ของหน่วยงาน
        /// </summary>
        public Debt DebtSummary { get; set; }
        /// <summary>
        /// S162 secion b  อัตราส่วนทางการเงิน
        /// </summary>
        public List<FinancialRatio> FinancialRatio { get; set; }
        /// <summary>
        /// S162 secion b  อัตราส่วนทางการเงิน - ตารางชี้แจงเหตุผล
        /// </summary>
        public List<DSCRNote> DSCRNotes { get; set; }
        /// <summary>
        /// S162 Section c เอกสารประกอบ
        /// </summary>
        public List<AttachFileData> AttachFiles { get; set; }
        public List<COffer> Coffers { get; set; }
    }
    public class COffer
    {
        public string Title { get; set; }
        public decimal Amount { get; set; }
    }
    /// <summary>
    /// S162 secion b  tab งบดุล
    /// </summary>
    public class BalanceSheet {
        /// <summary>
        /// ปี
        /// </summary>
        public string Year { get; set; }
        /// <summary>
        /// 1. สินทรัพย์
        /// </summary>
        public decimal Asset { get; set; }
        /// <summary>
        /// 1.1 สินทรัพย์หมุนเวียน
        /// </summary>
        public decimal CurrentAsset { get; set; }
        /// <summary>
        /// 1.2 สินทรัพยถาวร
        /// </summary>
        public decimal FixedAsset { get; set; }
        /// <summary>
        /// 2 หนี้สินและส่วนของทุน
        /// </summary>
        public decimal LiabilityEquity { get; set; }
        /// <summary>
        /// 2.1 หนี้สิน
        /// </summary>
        public decimal Liability { get; set; }
        /// <summary>
        /// 2.1.1 หนี้สินหมุนเวียน
        /// </summary>
        public decimal CurrentLiability { get; set; }
        /// <summary>
        /// 2.1.2 หนี้สินระยะยาว
        /// </summary>
        public decimal LongTermLiability { get; set; }
        /// <summary>
        /// 2.2 ส่วนของทุน
        /// </summary>
        public decimal Equity { get; set; }
    }
    /// <summary>
    /// S162 secion b  tab งบกำไรขาดทุน
    /// </summary>
    public class IncomeStatement
    {
        /// <summary>
        /// ปี
        /// </summary>
        public string Year { get; set; }
        /// <summary>
        /// 1. รายได้
        /// </summary>
        public decimal Revenue { get; set; }
        /// <summary>
        /// 2. ค่าใช้จ่าย
        /// </summary>
        public decimal Expense { get; set; }
        /// <summary>
        /// 3. กำไรขาดทุน จากการดำเนินงาน
        /// </summary>
        public decimal GlossProfit { get; set; }
        /// <summary>
        /// 4. กำไรขาดทุน ก่อนหักดอกเบี้ยภาษี ค่าเสือม และตัดจำหน่าย (EBITDA)
        /// </summary>
        public decimal EBITDA { get; set; }
        /// <summary>
        /// 5. กำไรขาดทุน สุทธิ
        /// </summary>
        public decimal NetProfit { get; set; }
    }
    /// <summary>
    /// S162 secion b  tab กระแสเงินสด
    /// </summary>
    public class CashFlow
    {
        /// <summary>
        /// ปี
        /// </summary>
        public string Year { get; set; }
        /// <summary>
        /// 1. กระแสเงินจากการดำเนินงาน
        /// </summary>
        public decimal Operation { get; set; }
        /// <summary>
        /// 2. กระแสเงินสดจากการลงทุน
        /// </summary>
        public decimal Investing { get; set; }
        /// <summary>
        /// 3. กระแสเงินสดจากการจัดหาเงิน
        /// </summary>
        public decimal Financing { get; set; }
        /// <summary>
        /// 4.1 เงินสดสุทธฺรับมาก (น้อย) กว่าเงินสดจ่าย
        /// </summary>
        public decimal NetCashFlow { get; set; }
        /// <summary>
        /// 4.2 เงินสดคงเหลือต้นงวด
        /// </summary>
        public decimal CashBalanceBeginning { get; set; }
        /// <summary>
        /// 4.3 เงินสดคงเหลือปลาดงวด
        /// </summary>
        public decimal CashBalanceEnding { get; set; }
    }
    /// <summary>
    /// S162 secion b  tab ภาระหนี้ของหน่วยงาน
    /// </summary>
    public class Debt {
        /// <summary>
        /// S162T41 ยอดหนี้คงค้าง
        /// </summary>
        public List<AgreementModel> CurrentDebt { get; set; }
        /// <summary>
        /// S162T42 ประมาณการภาระหนี้ทั้งหมด - ในประเทศ
        /// </summary>
        public List<DebtEstimation> ORGLocalDebtEstimation { get; set; }
        /// <summary>
        /// S162T42 ประมาณการภาระหนี้ทั้งหมด - ต่างประเทศ
        /// </summary>
        public List<ORGForeignDebtEstimationList> ORGForeignDebtEstimationList { get; set; }
        /// <summary>
        /// S162T42 ประมาณการภาระหนี้ที่รัฐต้องรับภาระ - ในประเทศ
        /// </summary>
        public List<MiniDebtEstimation> GOVLocalDebtEstimation { get; set; }
        /// <summary>
        /// S162T42 ประมาณการภาระหนี้ที่รัฐต้องรับภาระ - ต่างประเทศ
        /// </summary>
        public List<GOVForeignDebtEstimationList> GOVForeignDebtEstimationList { get; set; }
    }
    public class ORGForeignDebtEstimationList {
        /// <summary>
        /// สกุลเงิน
        /// </summary>
        public string Currency;
        /// <summary>
        /// S162T42 ประมาณการภาระหนี้ทั้งหมด - ต่างประเทศ
        /// </summary>
        public List<DebtEstimation> ORGForeignDebtEstimation { get; set; }
    }
    public class GOVForeignDebtEstimationList {
        /// <summary>
        /// สกุลเงิน
        /// </summary>
        public string Currency;
        /// <summary>
        /// S162T42 ประมาณการภาระหนี้ที่รัฐต้องรับภาระ - ต่างประเทศ
        /// </summary>
        public List<MiniDebtEstimation> GOVForeignDebtEstimation { get; set; }
    }
    public class DebtEstimation{
        /// <summary>
        /// ปี
        /// </summary>
        public string Year { get; set; }
        /// <summary>
        /// S162T42 1. ยอดหนี้คงค้าง
        /// </summary>
        public decimal OutStandingDebt { get; set; }
        /// <summary>
        /// S162T42 2. การเบิกจ่าย
        /// </summary>
        public decimal Disbursement { get; set; }
        /// <summary>
        /// S162T42 3.การชำระคืน - 3.1 เงินต้น
        /// </summary>
        public decimal Principle { get; set; }
        /// <summary>
        /// S162T42 3.การชำระคืน - 3.2 ดอกเบี้ยค่าธรรมเนียม
        /// </summary>
        public decimal Interest { get; set; }
        /// <summary>
        /// S162T42 4. กระแสเงินไหลเข้าสุทธิ
        /// </summary>
        public decimal NetCashFlow { get; set; }

    }
    public class MiniDebtEstimation
    {
        /// <summary>
        /// ปี
        /// </summary>
        public string Year { get; set; }
        /// <summary>
        /// <summary>
        /// S162T42 ประมาณการภาระหนี้ที่รัฐบาลต้องรับภาระ - เงินต้น
        /// </summary>
        public decimal Principle { get; set; }
        /// <summary>
        /// S162T42 ประมาณการภาระหนี้ที่รัฐบาลต้องรับภาระ - ดอกเบี้ยค่าธรรมเนียม
        /// </summary>
        public decimal Interest { get; set; }
        /// <summary>
        /// S162T42 ประมาณการภาระหนี้ที่รัฐบาลต้องรับภาระ - ค่าธรรมเนียม
        /// </summary>
        public decimal Fee { get; set; }

    }
    /// <summary>
    /// S162 secion b  tab อัตราส่วนทางการเงิน
    /// </summary>
    public class FinancialRatio {
        /// <summary>
        /// ปี
        /// </summary>
        public string Year { get; set; }
        /// <summary>
        /// 1. Debt Service Coverage Rato : (EBITDA/ภาระหนี้)
        /// </summary>
        public decimal DSRC { get; set; }
        public decimal GovDebt { get; set; }
        /// <summary>
        /// ภาระเงินต้น  : รัฐรับภาระ
        /// </summary>
        public decimal pGovDebt { get; set; }
        /// <summary>
        /// ภาระดอกเบี้ย  : รัฐรับภาระ
        /// </summary>
        public decimal iGovDebt { get; set; }
        public decimal pDebt { get; set;}
        /// <summary>
        /// ภาระหนี้ เงินต้นที่ครบกำหนด : รัฐรับภาระ
        /// </summary>
        public decimal pDebtG { get; set; }
        /// <summary>
        /// ภาระหนี้ เงินต้นที่ครบกำหนด : หน่วยงานรับภาระ
        /// </summary>
        public decimal pDebtO { get; set; }
        public decimal iDebt { get; set; }
        /// <summary>
        /// ภาระหนี้ ดอกเบี้ยที่ครบกำหนด : รัฐรับภาระ
        /// </summary>
        public decimal iDebtG { get; set; }
        /// <summary>
        /// ภาระหนี้ ดอกเบี้ยที่ครบกำหนด : หน่วยงานรับภาระ
        /// </summary>
        public decimal iDebtO { get; set; }
        /// <summary>
        /// 2. อัตราส่วนหนี้สินต่อทุน : (หนี้สินรวม/ส่วนของผู้ถือหุ้น) 
        /// </summary>
        public decimal LERatio { get; set; }
        /// <summary>
        /// 3. อัตราส่วนหนี้สินต่อสินทรัพย์รวม : (หนี้สินรวม/สินทรัพย์รวม) 
        /// </summary>
        public decimal LARatio { get; set; }
        /// <summary>
        /// 4. อัตราส่วนเงินทุนหมุนเวียน : (ทรัพย์สินหมุนเวียน/หนี้สินหมุนเวียน)
        /// </summary>
        public decimal CACLRatio { get; set; }
        /// <summary>
        /// 5. อัตราผลตอบแทนต่อสินทรัพย์ถาวร : (กำไรสุทธิ/สินทรัพย์ถาวร) 
        /// </summary>
        public decimal NPFARatio { get; set; }
        /// <summary>
        /// 6. อัตราผลตอบแทนต่อสินทรัพย์รวม : (กำไรสุทธิ/สินทรัพย์รวม) 
        /// </summary>
        public decimal NPARatio { get; set; }
        /// <summary>
        /// 7. อัตราผลตอบแทนต่อยอดขาย : (กำไรสุทธิ/ยอดขาย)
        /// </summary>
        public decimal NPRRatio { get; set; }

        /// <summary>
        /// EBIDA
        /// </summary>
        public decimal EBIDA { get; set; }
        /// <summary>
        /// debt
        /// </summary>
        public decimal Debt { get; set; }
        /// <summary>
        /// หนี้สินรวม
        /// </summary>
        public decimal Liability { get; set; }
        /// <summary>
        /// ส่วนของผู้ถือหุ้น
        /// </summary>
        public decimal Equity { get; set; }
        /// <summary>
        /// สินทรัพย์
        /// </summary>
        public decimal Asset { get; set; }
        /// <summary>
        /// สินทรัพย์หมุนเวียน
        /// </summary>
        public decimal CurrentAsset { get; set; }
        /// <summary>
        /// หนี้สินหมุนเวียน
        /// </summary>
        public decimal CurrentLiability { get; set; }
       
        /// <summary>
        /// กำไรสุทธิ
        /// </summary>
        public decimal NetProfit { get; set; }
        /// <summary>
        /// สินทรัพย์ถาวร
        /// </summary>
        public decimal FixedAsset { get; set; }
        /// <summary>
        /// ยอดขาย
        /// </summary>
        public decimal Revenue { get; set; }

    }
    /// <summary>
    /// S162 secion b  tab อัตราส่วนทางการเงิน  - ชี้แจงเหตุผล
    /// </summary>
    public class DSCRNote {
        public long? DSRCNoteID { get; set; }
        /// <summary>
        /// ปี
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// DSRC:  1. Debt Service Coverage Rato : (EBITDA/ภาระหนี้)
        /// </summary>
        public decimal DSCR { get; set; }
        /// <summary>
        /// ชี้แจงเหตุผลที่ DSRC ต่ำกว่าที่กำหนด
        /// </summary>
        public string Reason { get; set; }
        /// <summary>
        /// แนวทางการแก้ไข
        /// </summary>
        public string Solution { get; set; }
        /// <summary>
        /// ความคืบหน้า
        /// </summary>
        public string ProgressUpdate { get; set; }

    }
}
