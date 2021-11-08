using mof.ServiceModels.Common.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace mof.ServiceModels.Organization
{
    public class CalORGStatusRequest
    {
        /// <summary>
        /// S121 Section A Data C : ประเภทหน่วยงาน LOVGroupCode = ORGTYPE
        /// </summary>
        [Required]
        public BasicData ORGType { get; set; }
        /// <summary>
        /// S121 Section A Data I : รายชื่อผู้ถือหุ้นของหน่วยงาน
        /// </summary>
        public List<ShareHD> ShareHolders { get; set; }

    }
    public class CalORGStatusResponse
    {
        /// <summary>
        /// S121 Section B Data A : ประเภทของหน่วยงานที่สร้างตาม พรบ. หนี้สาธารณะ LOVGroupCode = PDA
        /// </summary>
        public List<BasicData> PublicDebtActs { get; set; }
        /// <summary>
        /// S121 Section B Data C : ประเภทของหน่วยงานที่สร้างตาม พรบ. วินัยการเงินการคลัง  LOVGroupCode = FDA
        /// </summary>
        public List<BasicData> FinanceDebtActs { get; set; }
        /// <summary>
        /// S121 Section A Data L : สัดส่วนทุนที่เป็นของรัฐบาลตาม พรบ. หนี้สาธารณะ
        /// </summary>
        public decimal PDAProportion { get; set; }
        /// <summary>
        /// S121 Section A Data M : สัดส่วนทุนที่เป็นของรัฐบาลตาม พรบ. วินัยฯ
        /// </summary>
        public decimal FDAProportion { get; set; }
    }
}
