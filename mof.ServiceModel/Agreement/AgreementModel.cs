using mof.ServiceModels.Common.Generic;
using mof.ServiceModels.Plan;
using mof.ServiceModels.Request;
using System;
using System.Collections.Generic;

namespace mof.ServiceModels.Agreement
{
    public class AgreementModel
    {

        public long AgreementID { get; set; }
        /// <summary>
        /// S152 section c ตารางสัญญา /M1522 รหัสอ้างอิงจาก GFTR
        /// </summary>
        public string GFTRRefCode { get; set; }
        /// <summary>
        /// S152 section c ตารางสัญญา /M1522 ชื่อสัญญา
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// M1522 คู่สัญญา
        /// </summary>
        public string Counterparty { get; set; }
        /// <summary>
        /// M1522 รหัสอ้างอิง
        /// </summary>
        public string ReferenceCode { get; set; }
        /// <summary>
        /// S152 section c ตารางสัญญา /M1522 อัตราดอกเบี้ย
        /// </summary>
        public decimal InterestRate { get; set; }
        /// <summary>
        /// การกำหนด
        /// </summary>
        public string DebtType { get; set; }
        /// <summary>
        /// การกำหนด
        /// </summary>
        public string DebtSubType { get; set; }
        /// <summary>
        /// สูตรดอกเบี้ย
        /// </summary>
        public string InterestFomula { get; set; }
        /// <summary>
        /// ข้อตกลง
        /// </summary>
        public string MasterAgreement { get; set; }
        /// <summary>
        /// M1522 วันที่ลงนาม
        /// </summary>
        public DateTime SignDate { get; set; }
        /// <summary>
        /// M1522 วันที่เริ่มสัญญา
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// M1522 วันที่สิ้นสุดสัญญา
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// S152 section c ตารางสัญญา /M1522 วันที่ครบกำหนดชำระ
        /// </summary>
        public DateTime IncomingDueDate { get; set; }
        /// <summary>
        /// L = ในประเทศ , F = ต่างประเทศ , O = อื่นๆ, A = ทั้งหมด
        /// </summary>
        public string SourceType { get; set; }
        /// <summary>
        /// กู้โดยขอค้ำจากกระทรวงการคลัง
        /// </summary>
        public bool IsGuarantee { get; set; }
        /// <summary>
        /// M1522 วงเงินตามสัญญา
        /// </summary>
        public decimal LoanAmount { get; set; }
        /// <summary>
        /// S162T41 สกุลเงินที่กู้
        /// </summary>
        public string LoanCurrency { get; set; }
        /// <summary>
        /// M1522 วงเงินตามสัญญา เงินบาท
        /// </summary>
        public decimal LoanAmonthTHB { get; set; }
        /// <summary>
        /// M1522 วงเงินคงเหลือ (ยอดหนี้คงค้าง) ตามสกุลเงิน LoanCurrency
        /// </summary>
        public decimal OutStandingDebt { get; set; }
        /// <summary>
        /// M1522 วงเงินคงเหลือ (ยอดหนี้คงค้าง) ตามสกุลเงินบาท
        /// </summary>
        public decimal OutStandingDebtTHB { get; set; }
        /// <summary>
        /// S152 section c ตารางสัญญา /M1522 วงเงินที่จะครบกำหนดชำระ
        /// </summary>
        public decimal IncomingDueAmount { get; set; }
        /// <summary>
        /// M1522 อายุเงินกู้
        /// </summary>
        public decimal LoanAge { get; set; }
        /// <summary>
        /// หน่วยงาน
        /// </summary>
        public BasicData Organization { get; set; }
        /// <summary>
        /// เชื่อมโยงกับแผน
        /// </summary>
        public bool IsMapped { get; set; }
    
        public DateTime? PostingDate { get; set; }
        /// <summary>
        /// LOVGroupCode = 'GroupFTyp'
        /// </summary>
        public BasicData TransactionType { get; set; }

        public decimal ActualDueAmount { get; set; }
        public DateTime? ActualDueDate { get; set; }
        public string ActualMasterAgreement { get; set; }
        /// <summary>
        /// วัตถุประสงค์ LOVGroupCode = 'EDOBJ'
        /// </summary>
        public BasicData Objective { get; set; }
        /// <summary>
        /// ประเภทการบริหารหนี LOVGroupCode = 'EDPT'
        /// </summary>
        public BasicData PlanType { get; set; }
    }
    public class AgreementMappingList
    {
        public List<NewDebtPlanActList> AgreementActiviteMaps { get; set; }
        public List<ExistPlanAgreementList> AgreementRestructureMaps { get; set; }
    }
    /// <summary>
    /// M197T2
    /// </summary>
    public class AgreementTransModel
    {
        public AgreementModel Agreement { get; set; }
        public string FlowTypeCode { get; set; }
        public string FlowType { get; set; }
        /// <summary>
        /// วันที่ชำระ
        /// </summary>
        public DateTime PostingDate { get; set; }
        public decimal Amount { get; set; }
        public string GFTRRefCode { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public decimal AmountTHB { get; set; }
        public string Curr { get; set; }
        /// <summary>
        /// เชื่อมโยงกับแผน
        /// </summary>
        public bool IsMapped { get; set; }
    }
    #region parameter
    public class AgreementListParameter
    {
        /// <summary>
        /// รหัสหน่วยงาน ถ้าเป็น null คือเอาทั้งหมด
        /// </summary>
        public long? OrganizationID { get; set; }
        /// <summary>
        /// AgreementID ถ้าเป็น null คือเอาทั้งหมด
        /// </summary>
        public long? AgreementID { get; set; }
        public bool? HasOutstandingDebtOnly { get; set; }
        /// <summary>
        /// สัญญาครบกำหนด null = ทั้งหมด , true = ครบกำหนดแล้ว , false = ยังไม่ครบกำหนด
        /// </summary>
        public bool? Dued { get; set; }
        /// <summary>
        /// ปีงบประมาณ , ถ้า null คือจะเอาข้อมูล ตั้งแต่ ตค. 2018
        /// </summary>
        public int? BudgetYear { get; set; }
        /// <summary>
        /// SearchText สำหรับหาชื่อสัญญา  
        /// </summary>
        public Paging Paging { get; set; }
        /// <summary>
        /// LOVGroupCode GroupFTyp
        /// </summary>
        public string TransTypeCode { get; set; }
    }
    #endregion

    
}
