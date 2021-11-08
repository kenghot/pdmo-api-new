using mof.ServiceModels.Common.Generic;
using mof.ServiceModels.Helper;
using mof.ServiceModels.Plan;
using mof.ServiceModels.Request;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace mof.ServiceModels.Project
{
    public enum eProjectExtendGroup 
    {
        PRODUCTIVITY,
        SCOPE,
        BENEFIT,
        OBJECTIVE,
    }
    public enum eProjectTOMany
    {
        /// <summary>
        /// สบน Cecklist
        /// </summary>
        PDMO,
        /// <summary>
        /// มติ Checklist
        /// </summary>
        RESOLUTION
    }
    public class ProjectModelMin
    {
        public long? ProjectID { get; set; }
        /// <summary>
        /// รหัสหน่วยงาน
        /// </summary>
        [Required]
        public long OrganizationID { get; set; }

        public int StartYear { get; set; }
        public string ProjectCode { get; set; }
        /// <summary>
        /// ประเภทโครงการ LOVGroupCode = PJTYPE
        /// </summary>
        [Required]
        public BasicData ProjectType { get; set; }

        /// <summary>
        /// M172T1 Data A : ชื่อโครงการ (ภาษาไทย)
        /// </summary>
        [Required]
        public string ProjectTHName { get; set; }
        /// <summary>
        /// M172T1 Data B : ชื่อโครงการ (ภาษาอังกฤษ)
        /// </summary>
        [Required]
        public string ProjectENName { get; set; }
        /// <summary>
        /// หมายเหตุ
        /// </summary>
        [Required]
        public string ProjectRemark { get; set; }

        public decimal LimitAmount { get; set; }

    }
    public class ProjectModel
    {
        public long? ProjectID { get; set; }
        /// <summary>
        /// รหัสหน่วยงาน
        /// </summary>
        [Required]
        public long OrganizationID { get; set; }
        /// <summary>
        /// ข้อมูลหน่วยงาน (read only)
        /// </summary>
        public BasicData Organization { get; set; }
        public string ProjectCode { get; set; }
        /// <summary>
        /// ประเภทโครงการ LOVGroupCode = PJTYPE
        /// </summary>
        [Required]
        public BasicData ProjectType { get; set; }
        /// <summary>
        /// กรอบวงเงิน
        /// </summary>
        public decimal LimitAmount { get; set; }
        /// <summary>
        /// ปี พ.ศ. ที่เริ่ม display only
        /// </summary>
        public int StartYear { get; set; }
        /// <summary>
        /// M172T1 Data A : ชื่อโครงการ (ภาษาไทย)
        /// </summary>
        [Required]
        public string ProjectTHName { get; set; }
        /// <summary>
        /// M172T1 Data B : ชื่อโครงการ (ภาษาอังกฤษ)
        /// </summary>
        public string ProjectENName { get; set; }
        /// <summary>
        /// M172T1 Data C : วันที่เริ่ม
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }
        /// <summary>
        /// M172T1 Data D : วันที่สิ้นสุด
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }
        /// <summary>
        /// M172T1 Data E : ระยะเวลาดำเนินงาน
        /// </summary>
        public String ProcessTime { get; set; }
        /// <summary>
        /// M172T1 Data F : ความเป็นมาโครงการ
        /// </summary>
        [Required]
        public string ProjectBackground { get; set; }
        /// <summary>
        /// M172T1 Data G : เป้าหมายโครงการ
        /// </summary>
        [Required]
        public string ProjectTarget { get; set; }
        /// <summary>
        /// M172T1 Data H : วัตถุประสงค์โครงการ
        /// </summary>
        [Required]
        public string ProjectObjective { get; set; }
        /// <summary>
        /// M172T1 Data I : ขอบเขตการดำเนินโครงการ
        /// </summary>
        [Required]
        public string ProjectScope { get; set; }
        /// <summary>
        /// หมายเหตุ (สำหรับแสดง กู้เพื่อชดเชยขาดดุล กับ กู้เพื่อชดเชย ขาดดุลเลื่อมปี)
        /// </summary>
        public string ProjectRemark { get; set; }
        /// <summary>
        /// M172T1 Data J : แหล่งวัสดุอุปกรณ์
        /// </summary>
        [Required]
        public List<MaterialSource> Materials { get; set; }
        /// <summary>
        /// M172T1 Data N : ผลตอบแทนจากการลงทุนแบบ FIRR
        /// </summary>
        [Required]
        public decimal FIRR { get; set; }
        /// <summary>
        /// M172T1 Data O : ผลตอบแทนจากการลงทุนแบบ EIRR
        /// </summary>
        [Required]
        public decimal EIRR { get; set; }
        /// <summary>
        ///  true = โครงการใหม่, false = โครงการต่อเนื่อง
        /// </summary>
        [Required]
        public bool IsNewProject { get; set; }
        #region tab2
        /// <summary>
        /// M172T2 ส่วน สบน. / มติ checkbox สำหรับตอน create ให้เรียก v1/projects/GetProjStatusList เพื่อแสดงรายการ list ทั้งหมด
        /// </summary>
        [Required]
        public List<ProjectStatus> ProjectStatuses { get; set; }

        /// <summary>
        /// ระดับควาเห็นชอบ ของ สบน (1,2,3)
        /// </summary>
        [IntRange(AllowableValues = new[] { 1, 2, 3 }, ErrorMessage = "เห็นชอบ 1 , 2, 3")]
        public int PDMOAgreement { get; set; }
        /// <summary>
        /// ระดับควาเห็นชอบ ของ มติ (1,2,3)
        /// </summary>
        [IntRange(AllowableValues = new[] { 1, 2, 3 }, ErrorMessage = "เห็นชอบ 1 , 2, 3")]
        public int ResolutionAgreement { get; set; }
        #endregion
        #region tab3,4
        /// <summary>
        /// (read only) M172T3 : A-E สรุปรายการค่าใช้จ่ายตามมติ  
        /// </summary>
        public ExpenseSummary ResolutionExpSum { get; set; }
        /// <summary>
        /// (read only) M172T3 : F-J สรุปรายการค่าใช้จ่ายตา สัญญาจ้าง
        /// </summary>
        public ExpenseSummary ContractExpSum { get; set; }
        /// <summary>
        /// M172T3, M172T4 :  รายละเอียดลักษณะงาน
        /// </summary>
        public List<ActivityData> Activities { get; set; }

        #endregion

        #region tab5:แผนการใช้จ่ายเงิน
        public List<BuggetAllocationYearPlan> BuggetAllocationPlans { get; set; }
        #endregion
        /// <summary>
        /// (Read only) ข้อมูลการแก้ไขล่าสุด 
        /// </summary>
        public LogData LastUpdate { get; set; }
        /// <summary>
        /// (Read only) อัตตราแลกเปลี่ยน
        /// </summary>
        public CurrencyData CurrencyData { get; set; }
        public string ProjectBranch { get; set; }
        public string CapitalSource { get; set; }
        #region iipm
        public List<ProjectLocationModel> Locations { get; set; } = new List<ProjectLocationModel>();
        public List<ProjectResolutionModel> Resolutions { get; set; } = new List<ProjectResolutionModel>();
        public List<ProjectExtendModel> Benefits { get; set; } = new List<ProjectExtendModel>();
        public List<ProjectExtendModel> Objectives { get; set; } = new List<ProjectExtendModel>();
        public List<ProjectExtendModel> Scopes { get; set; } = new List<ProjectExtendModel>();
        public List<ProjectExtendModel> Productivities { get; set; } = new List<ProjectExtendModel>();
        public List<ProjectExtendModel> ExtendDatas { get; set; } = new List<ProjectExtendModel>();
        public BasicData Province { get; set; }
        public BasicData Sector { get; set; }
        public BasicData Status { get; set; }
        public BasicData CreditChannel { get; set; }
        public bool? IsGovBurden { get; set; }
        public bool? IsOnGoing { get; set; }
        public bool? HasEld { get; set; }
        public string DirectorName { get; set; }
        public string DirectorPosition { get; set; }
        public string DirectorMail { get; set; }
        public string DirectorTel { get; set; }
        public string MapDrawing { get; set; }
        #endregion
    }
    public class SourceOfLoanAmount
    {
        public BasicData SourceLoan { get; set; }
        public List<AmountData> Amounts { get; set; }

    }
    public class ProjectLocationModel
    {
        public long? Id { get; set; }
        public int No { get; set; }
        public string Location { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

    }
    public class ProjectResolutionModel
    {
        public long? Id { get; set; }
        public int No { get; set; }
        public string Detail { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public AttachFileData File { get; set; }
    }
    public class BuggetAllocationYearPlan {
        public int Year { get; set; }
        public List<BuggetAllocation> BuggetAllocations { get; set; }

    }
    public class BuggetAllocation{
        /// <summary>
        /// แหล่งเงินและยอดรวม
        /// </summary>
        public LoanSource Source{ get; set; }
        /// <summary>
        /// แผนการใช้จ่ายรายเดือน[0-11]  index 0: เดือนตุลา - 11: เดือนกันยา 
        /// </summary>
        public List<decimal> Months { get; set; }

    }
    public class ProjectExtendModel
    {

        public long? Id { get; set; }
        public int No { get; set; }
        public string ExtendData { get; set; }
        public string GroupCode { get; set; }
    }
    public class ActivityData
    {
        /// <summary>
        /// ถ้าเป็น new ส่ง null มา
        /// </summary>
        public long? ProjActID { get; set; }
        /// <summary>
        /// M17211 Sect B : ระบุลักษณะงาน
        /// </summary>
        [Required]
        public string ActivityName { get; set; }
        /// <summary>
        /// M17211 tab 1  : แหล่งที่มา ตามมติ
        /// </summary>
        public List<AmountData> ResolutionAmounts { get; set; }
        /// <summary>
        /// M17211 tab 2  : แหล่งที่มา ตามสัญญาจ้าง
        /// </summary>
        public List<AmountData> ContractAmounts { get; set; }
        /// <summary>
        /// (Read only) M17211 tab 1  : ยอดรวม (บาท) แหล่งที่มา ตามมติ
        /// </summary>
        public decimal TotalResolution { get; set; }
        /// <summary>
        /// (Read only) M17211 tab 2  : ยอดรวม (บาท) แหล่งที่มา ตามสัญญาจ้าง
        /// </summary>
        public decimal TotalContract { get; set; }
        /// <summary>
        /// (Read only) M172T4 data A-F: ยอดรวมการดำเนินการแต่ละลักษณะงาน
        /// </summary>
        public ProceedData TotalProceedByActivity { get; set; }
        /// <summary>
        /// (Read only) M17232 รายละเอียดวงเงินที่ดำเนินการแล้วรายปี / รายเดือน
        /// </summary>
        public List<ProceedByYear> Years { get; set; }
        /// <summary>
        /// Model สำหรับ บันทึกค่าวงเงินที่ดำเนินการแล้วต่างๆ (รายเดือน)
        /// </summary>
        public List<SaveProceed> SaveProceedData { get; set; }
        /// <summary>
        /// GFCode ที่ผูก
        /// </summary>
        public string GFCode { get; set; }
    }

    public class SaveProceed
    {
        /// <summary>
        /// ปี พ.ศ. เช่น 2562
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// เดือน 1-12 (1 = มกราคม) ถ้าเป็นรายปีใส่ 0
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// ข้อมูลสำหรับบันทึก เงินงบประมาณ
        /// </summary>
        public List<AmountData> Budget { get; set; }
        /// <summary>
        /// ข้อมูลสำหรับบันทึก เงินรายได้
        /// </summary>
        public List<AmountData> Revernue { get; set; }
        /// <summary>
        /// ข้อมูลสำหรับบันทึก เงินกู้ลงนาม
        /// </summary>
        public List<AmountData> SignedLoan { get; set; }
        /// <summary>
        /// ข้อมูลสำหรับบันทึก เงินกู้เบิกจ่าย
        /// </summary>
        public List<AmountData> DisburseLoan { get; set; }
        /// <summary>
        /// ข้อมูลสำหรับบันทึก เงินจากแหล่งอื่น
        /// </summary>
        public List<AmountData> Other { get; set; }


    }
    public class ProceedByYear
    {
        /// <summary>
        /// วงเงินที่ดำเนินการแล้วรายปี
        /// </summary>
        public ProceedData Year { get; set; }
        /// <summary>
        /// วงเงินที่ดำเนินการแล้วรายเดือน
        /// </summary>
        public List<ProceedData> Months { get; set; }
    }
    public class ProceedData {
        /// <summary>
        /// รายละเอียด 
        /// </summary>
        public string Detail { get; set; }
        /// <summary>
        /// M17232 Data C : เงินงบประมาณ
        /// </summary>
        public decimal Budget { get; set; }
        /// <summary>
        /// M17232 Data D : เงินรายได้
        /// </summary>
        public decimal Revernue { get; set; }
        /// <summary>
        /// M17232 Data E : เงินกู้ลงนาม
        /// </summary>
        public decimal SignedLoan { get; set; }
        /// <summary>
        /// M17232 Data F : เงินกู้เบิกจ่าย
        /// </summary>
        public decimal DisburseLoan { get; set; }
        /// <summary>
        /// M17232 Data G : เงินจากแหล่งอื่น
        /// </summary>
        public decimal Other { get; set; }
        /// <summary>
        /// M17232   : ยอดรวม
        /// </summary>
        public decimal Total { get; set; }

    }
    public class MaterialSource
    {

        /// <summary>
        ///  แหล่งวัสดุอุปกรณ์ (L = ในประเทศ , F = ต่างประเทศ)
        /// </summary>
        [StringRange(AllowableValues = new[] { "L", "F" }, ErrorMessage = "แหล่งวัสดุอุปกรณ์ (L = ในประเทศ , F = ต่างประเทศ)")]
        public string SourceType { get; set; }
        /// <summary>
        /// วงเงิน
        /// </summary>
        [Required]
        public decimal LimitAmount { get; set; }
        /// <summary>
        /// สกุลเงิน
        /// </summary>
        [Required]
        public string CurrencyCode { get; set; }
        /// <summary>
        /// คิดเป็น (บาท) display only
        /// </summary>
        public decimal THBAmount { get; set; }
        /// <summary>
        /// สัดส่วน display only
        /// </summary>
        public decimal Ratio { get; set; }
    }
 
    public class ExpenseSummary
    {
        /// <summary>
        /// M172T3 data A and F : รวมวงเงิน
        /// </summary>
        public decimal GrandTotal { get; set; }
        /// <summary>
        /// M172T3 data B and G : รวมเงินกู้
        /// </summary>
        public decimal TotalLoan { get; set; }
        /// <summary>
        /// M172T3 data C and H : เงินกู้ ในประเทศ
        /// </summary>
        public decimal LocalLoan { get; set; }
        /// <summary>
        /// M172T3 data D and I : เงินกู้ ต่างประเทศ
        /// </summary>
        public decimal ForeignLoan { get; set; }
        /// <summary>
        /// M172T3 data E and J : เงินกู้ อื่นๆ
        /// </summary>
        public decimal OtherLoan { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<SourceOfLoanAmount> SumBySources { get; set; } = new List<SourceOfLoanAmount>();
    }
    public class AmountData
    {
        /// <summary>
        ///  แหล่งเงิน (L = ในประเทศ , F = ต่างประเทศ, O = อื่นๆ)
        /// </summary>
        [StringRange(AllowableValues = new[] { "L", "F", "O" }, ErrorMessage = "แหล่งวัสดุอุปกรณ์ (L = ในประเทศ , F = ต่างประเทศ, O = อื่นๆ)")]
        [Required]
        public string SourceType { get; set; }
        /// <summary>
        /// จำนวนเงิน
        /// </summary>
        [Required]
        public decimal Amount { get; set; }
        /// <summary>
        /// สกุลเงิน
        /// </summary>
        [Required]
        public string CurrencyCode { get; set; }
        /// <summary>
        /// คิดเป็น (บาท) display only
        /// </summary>
        public decimal THBAmount { get; set; }
        /// <summary>
        /// แหล่งเงินกู้ จาก LOV
        /// </summary>
        public BasicData SourceLoan { get; set; } = new BasicData { Code = "", ID = 0, Description = "", IsSelected = false };
    }
    public class ProjectStatus
    {
        /// <summary>
        /// (ระบบสร้างให้) Project status id 
        /// </summary>
        [Required]
        public long ID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// สบน check / uncheck
        /// </summary>
        [Required]
        public bool PDMOCheck { get; set; }
        /// <summary>
        /// มติ check / uncheck
        /// </summary>
        [Required]
        public bool ResolutionCheck { get; set; }
    }
    #region parameter
    public class ProjectListParameter
    {
        /// <summary>
        /// รหัสหน่วยงาน ถ้าเป็น null คือเอาทั้งหมด
        /// </summary>
        public long? OrganizationID { get; set; }
        /// <summary>
        /// ประเภทโครงการ (ถ้าไม่ใส่คือเอาทั้งหมด) LOVGroupCode = PJTYPE
        /// </summary>
        public string ProjectType { get; set; }
        /// <summary>
        /// ปีเริ่มต้นโครงการ ถ้าเป็น null คือเอาทั้งหมด
        /// </summary>
        public int? StartYear { get; set; }
        /// <summary>
        /// SearchText สำหรับหาชื่อโครงการ ทั้งภาษาไทยและ eng
        /// </summary>
        public Paging Paging { get; set; }
    }
    #endregion
}
