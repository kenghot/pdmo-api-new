using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using mof.ServiceModels.Common.Generic;
namespace mof.ServiceModels.Organization
{
    public enum eORGModifyType
    {
        Normal,
        ChaneRequest,
        Approve,
        CancelChange
    }
    public class ORGModel
    {
        #region "section a"
        /// <summary>
        /// ตอน create ให้ใส่ null 
        /// </summary>
        public long? OrganizeID { get; set; }
        /// <summary>
        /// รหัสหน่วยงาน
        /// </summary>
        [Required]
        public string OrganizationCode { get; set; }
        /// <summary>
        /// S121 Section A Data A : ชื่อหน่วยงาน
        /// </summary>
        [Required]
        public string OrganizationTHName { get; set; }
        /// <summary>
        /// S121 Section A Data B : หน่วยงานต้นสังกัด โดยได้ข้อมูลจาก api "organizations/affiliates"
        /// </summary>
        public BasicData ORGAffiliate { get; set; }
        /// <summary>
        /// S121 Section A Data C : ประเภทหน่วยงาน LOVGroupCode = ORGTYPE
        /// </summary>
        [Required]
        public BasicData ORGType { get; set; }
        /// <summary>
        /// S121 Section A Data D : สาขาของหน่วยงาน LOVGroupCode = FIELD
        /// </summary>
        public BasicData Field { get; set; }
        /// <summary>
        /// S121 Section A Data E : สาขาย่อยของหน่วยงาน LOVGroupCode = SFIELD
        /// </summary>
        public BasicData SubField { get; set; }
        /// <summary>
        /// S121 Section A Data F : กฎหมายที่เกี่ยวข้องในการจัดตั้งหน่วยงาน
        /// </summary>
        [Required]
        public string EstablishmentLaw { get; set; }
        /// <summary>
        /// S121 Section A Data I : รายชื่อผู้ถือหุ้นของหน่วยงาน
        /// </summary>
        public List<ShareHD> ShareHolders { get; set; }
        /// <summary>
        /// S121 Section A Data L : สัดส่วนทุนที่เป็นของรัฐบาลตาม พรบ. หนี้สาธารณะ
        /// </summary>
        public decimal PDAProportion { get; set; }
        /// <summary>
        /// S121 Section A Data M : สัดส่วนทุนที่เป็นของรัฐบาลตาม พรบ. วินัยฯ
        /// </summary>
        public decimal FDAProportion { get; set; }
        /// <summary>
        /// S121 Section A Data N : แสดงข้อกฎหมายที่เกี่ยวข้องกับการก่อหนี้ของหน่วยงาน
        /// </summary>
        public List<AttachFileData> LawOfDebts { get; set; }
        /// <summary>
        /// S121 Section A Data O : เอกสารประกอบ
        /// </summary>
        public List<AttachFileData> AttachFiles { get; set; }
        /// <summary>
        /// S121 Section A Data P : หมายเหตุ
        /// </summary>
        public string Remark { get; set; }
        #endregion
        #region "section b"
        /// <summary>
        /// S121 Section B Data A : ประเภทของหน่วยงานที่สร้างตาม พรบ. หนี้สาธารณะ LOVGroupCode = PDA
        /// </summary>
        [Required]
        public List<BasicData> PublicDebtActs { get; set; }
        /// <summary>
        /// S121 Section B Data B : มาตราที่เกี่ยวข้องในการจัดประเภทหน่วยงานตาม พรบ. หนี้สาธารณะ
        /// </summary>
        [Required]
        public string PublicDebtSection { get; set; }
        /// <summary>
        /// S121 Section B Data C : ประเภทของหน่วยงานที่สร้างตาม พรบ. วินัยการเงินการคลัง  LOVGroupCode = FDA
        /// </summary>
        [Required]
        public List<BasicData> FinanceDebtActs { get; set; }
        /// <summary>
        /// S121 Section B Data D : ข้อมูลมาตราที่เกี่ยวข้องในการจัดประเภทหน่วยตาม พรบ. วินัยการเงินการคลัง
        /// </summary>
        [Required]
        public string FinanceDebtSection { get; set; }
        /// <summary>
        /// S121 Section B Data E : อำนาจในการกู้เงินของหน่วยงาน 
        /// </summary>
        [Required]
        public bool HasLoanPower { get; set; }
        /// <summary>
        /// S121 Section B Data F : ข้อกฎหมายที่เกี่ยวข้องกรณีหน่วยงานมีอำนาจกู้
        /// </summary>
        [Required]
        public string LoanPowerSection { get; set; }
        /// <summary>
        /// S121 Section B Data G : วิธีการนับหนี้สาธารณะ LOVGroupCode = DCAL
        /// </summary>
        [Required]
        public BasicData DebtCalculation { get; set; }
        #endregion
        /// <summary>
        /// สถานะการร้องขอ (read only)
        /// </summary>
        public LogData RequestStatus { get; set; }
        /// <summary>
        /// อนุมัติล่าสุด
        /// </summary>
        public LogData LastApproved { get; set; }
        /// <summary>
        /// ที่อยู่
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// เบอร์ติดต่อ
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// จังหวัด
        /// </summary>
        public BasicData Province { get; set; }
        /// <summary>
        /// ประเภท อปท
        /// </summary>
        public BasicData PDMOType { get; set; }
        /// <summary>
        /// รายละเอียด ผู้ร้องขอการเปลี่ยนแปลงข้อมูล
        /// </summary>
        public ChangeRequestContact RequestorContact { get; set; }
        /// <summary>
        /// template
        /// </summary>
        public BasicData Template { get; set; }

    }

    public class ShareHD
    {
        /// <summary>
        /// รหัสหน่วยงาน **** หากเป็นเอกชน ให้ใส่ null *****
        /// </summary>
        public long? ShareHolderID { get; set; }
        /// <summary>
        /// ชื่อหน่วยงานผู้ถือหุ้น ในกรณีที่เป็น เอกชน ต้องระบุค่านี้มาด้วย หากมีการแก้ไขหรือเพิ่มข้อมมูล
        /// </summary>
        public string ShareHolderName { get; set; }
        /// <summary>
        /// ประเภทของหน่วยงานที่สร้างตาม พรบ. หนี้สาธารณะ LOVGroupCode = PDA
        /// </summary>
        public List<BasicData> PublicDebtActs { get; set; } = new List<BasicData>();
        /// <summary>
        /// ประเภทของหน่วยงานที่สร้างตาม พรบ. วินัยการเงินการคลัง  LOVGroupCode = FDA
        /// </summary>
        public List<BasicData> FinanceDebtActs { get; set; } = new List<BasicData>();
        /// <summary>
        /// สัดส่วนผู้ถือหุ้น (%)
        /// </summary>
        public decimal Proportion { get; set; }
        
    }

    public class ORGCountByType
    {
        public string OrgTypeName { get; set; }
        public int OrgCount { get; set; }
    }
    public class ChangeRequestContact
    {
        public string ContactName { get; set; }
        public string ContactTel { get; set; }
    }
}
