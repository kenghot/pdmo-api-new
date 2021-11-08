using System;
using System.Collections.Generic;
using mof.ServiceModels.Agreement;
using mof.ServiceModels.Common.Generic;
using mof.ServiceModels.Helper;

namespace mof.ServiceModels.Plan
{
    public class ExistingDebtPlanModel
    {
        /// <summary>
        /// S152 secion a ข้อมูลแผนเบื้องต้น 
        /// </summary>
        public PlanHeader PlanHeader { get; set; }

        /// <summary>
        /// S152 section b ข้อมูลสรุปภาพรวมแผนการก่อหนี้ใหม่ 
        /// </summary>
        public ExistingDebtPlanSummary PlanSummary { get; set; }
        /// <summary>
        /// S152 section c รายการสัญญาที่ผูกกับแผน 
        /// </summary>
        public List<ExistingDebtPlanDetails> PlanDetails { get; set; }
        //public List<ExistingDebtPlanDetailsByYear> PlanDetailsByYear { get; set; }
        public DebtSettlementInfoModel DebtSettlementInfo { get; set; }


    }
    public class ExistingDebtPlanDetailsByYear
    {
        public int Year { get; set; }
        public List<ExistingDebtPlanDetails> PlanDetails { get; set; }
    }
    public class DebtSettlementInfoModel
    {
        public decimal LoanAmount { get; set; }
        public decimal InterestAmount { get; set; }
        public decimal FeeAmount { get; set; }
        public List<AttachFileData> AttatchFiles {get;set;}
    }
    public class SearchExistingDebtPlanModel
    {
 
        public ExistingDebtPlanModel Data { get; set; }
        public int TotalRec { get; set; }
        

    }
    /// <summary>
    /// S152 section b ข้อมูลสรุปภาพรวมแผนการก่อหนี้ใหม่ 
    /// </summary>
    public class ExistingDebtPlanSummary
    {
        /// <summary>
        /// S152 section b ตัวเลขสรุป
        /// </summary>
        public List<Summary> OverAllSummary { get; set; }
        /// <summary>
        /// S152 section b แผนการชำระหนี้
        /// </summary>
        public PlanSummary PaymentPlanSummary { get; set; }
        /// <summary>
        /// S152 section b แผนปรับโครงสร้างหนี้
        /// </summary>
        public PlanSummary RestructurePlanSummary { get; set; }
        /// <summary>
        ///  รายละเอียด อัตราแลกเปลี่ยน
        /// </summary>
        public CurrencyData CurrencyData { get; set; }

    }
    public class Summary {
        /// <summary>
        /// S152 section b สรุปตัวเลขภาพรวม : หนี้ที่ครบกำหนดชำระ / วงเงินที่จะบริหาร / วงเงินที่ให้กค.ค้ำประกัน
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// S152 section b สรุปตัวเลขภาพรวม : ตัวเลขสรุป
        /// </summary>
        public decimal Value { get; set; }
        /// <summary>
        /// ดอกเบีย
        /// </summary>
        public decimal InterestValue { get; set; }
        /// <summary>
        /// อื่นๆ / ค่าธรรมเนียม
        /// </summary>
        public decimal FeeValue { get; set; }
        /// <summary>
        /// เงินต้น
        /// </summary>
        public decimal PrincipalValue { get; set; }
    }
    public class PlanSummary
    {
        public List<DebtPaymentPlan> PaymentPlans { get; set; }
        public decimal PaymentPlanSumAmountTHB { get; set; }

    }
    /// <summary>
    /// M1522 แผนการบริหารหนี้เดิม - แต่ละรายการ ( 1 บรรทัดนับเป็น 1 รายการ)
    /// </summary>
    public class DebtPaymentPlan {
        /// <summary>
        /// สำหรับ Map กับ Agreement
        /// </summary>
        public long? PaymentPlanID { get; set; }
        /// <summary>
        /// LOVGroupCode = 'PAYTYPE'  (ชำระตามกำหนด / ชำระหนี้ล่วงหน้า / Roll-over / Refinance / Cross Currency Swap / Interrest Rate) 
        /// </summary>
        public string DebtPaymentPlanType { get; set; }
        /// <summary>
        /// LOVGroupCode = 'PAYSRC'   แหล่งเงินที่จะนำมาชำระ : เงินรายได้ / เงินงบประมาณ / เงินกู้ / อื่นๆ
        /// </summary>
        public string PaymentSource { get; set; }
        /// <summary>
        /// แหล่งเงินกู้ จะมีค่าเมื่อ PaymentSource = เงินกู้
        /// </summary>
        public List<LoanSource> LoanSourcePlans { get; set; }
        /// <summary>
        /// ต้องการให้ กค. ค้ำประกัน
        /// </summary>
        public Boolean IsRequestGuarantee;
        /// <summary>
        /// M1522 ประมาณการประหยัดดอกเบี้ย
        /// </summary>
        public InterestSaving InterestSaving { get; set; }
    }

    public class ExistingDebtPlanDetails {

        /// <summary>
        /// รหัสแผนบริหารหนี้เดิม ถ้าแอดมาใหม่ใส่ null
        /// </summary>
        public long? PlanExistID { get; set; }
        public int? Year { get; set; }
        /// <summary>
        /// รายละเอียดสัญญา 1 แผน มีหลายสัญญา ถ้า add สัญญามาใหม่ ใส AgreementID  = null
        /// </summary>
        public List<AgreementModel> AgreementDetail { get; set; }
        /// <summary>
        /// M1522 แผนการบริหารหนี้เดิม - แผนการชำระหนี้
        /// </summary>
        public List<DebtPaymentPlan> PaymentPlan { get; set; }
        /// <summary>
        /// S142 section c ตารางรายละเอียดแผน column วงเงินที่จะชำระหนี้
        /// </summary>
        public List<LoanSource> PaymentPlanSumAmount { get; set; } //ยอดรวม PaymentPlan
        /// <summary>
        /// M1522 แผนการบริหารหนี้เดิม - แผนการปรับโครงสร้างหนี้
        /// </summary>
        public List<DebtPaymentPlan> RestructurePlan { get; set; }
        /// <summary>
        /// S142 section c ตารางรายละเอียดแผน column วงเงินที่จะปรับโครงสร้างหนี้
        /// </summary>
        public List<LoanSource> RestructurePlanSumAmount { get; set; }

        /// <summary>
        /// M1522 สรุปยอดวงเงินบริหารหนี้ แยกตามแหล่งเงิน
        /// </summary>
        public List<LoanSource> DebtPlanSumAmount { get; set; } //ทั้งหมดรวมกัน
        /// <summary>
        /// M1522  สรุปยอดเงินประหยัดดอกเบี้ย แยกตามแหล่งเงิน
        /// </summary>
        public List<LoanSource> InterestSavingSumAmount { get; set; }
        /// <summary>
        /// ไม่ต้องขออนุมัติ ครม. ภายใต้กรอบแผน
        /// </summary>
        public bool? IsNotRequiredApproval { get; set; }
    }
    public class InterestSaving {
        /// <summary>
        /// จำนวนเงินที่ประหยัดดอกเบี๊ยไปจากแผน (หน่วย THB)
        /// </summary>
        public List<LoanSource> calculatedSaving { get; set; }
        /// <summary>
        /// ค่าอ้างอิง  เพื่อใช้ในการคำนวนดอกเบี๊ยที่ประหยัดได้ ส่งมาตามค่าที่ต้องการนำไปใช้คำนวนในแต่ละรูปแบบ DebtPaymentPlan
        /// </summary>
        public List<InterestCalculateRef> calcurateReferences { get; set; }
       
    }
    public class InterestCalculateRef{
        /// <summary>
        /// คำอธิบายค่าที่ให้กรอกเพื่อนำไปคำนวน
        /// </summary>
        public string refLabel;
        /// <summary>
        /// ประเภทของ input objectที่จะให้กรอกใน ui / TEXT / DATETIME / INTEGER
        /// </summary>
        public string refType;
        /// <summary>
        /// ค่าที่ให้กรอกเพื่อนำไปคำนวน
        /// </summary>
        public string refValue;
    }

    public class ExistPlanAgreementList
    {

        public long PlanExistID { get; set; }
        /// <summary>
        /// รายการ สัญญา
        /// </summary>
        public List<AgreementModel> Agreements { get; set; }

        /// <summary>
        /// แผนปรับโครงสร้างหนี้
        /// </summary>
        public List<DebtPaymentPlan> DebtPaymentPlans { get; set; }


    }
}
