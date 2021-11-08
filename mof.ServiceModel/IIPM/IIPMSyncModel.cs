using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace mof.ServiceModels.IIPMSyncModel
{
    public class IIPMProject
    {
        [Required]
        public string code { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public string provinceCode { get; set; }
        /// <summary>
        /// Sid = organizationCode
        /// </summary>
        [Required]
        public string agencyId { get; set; }
        /// <summary>
        /// รหัสสาขาการลงทุนย่อย
        /// </summary>
        [Required]
        public long? sectorId { get; set; }
        [Required]
        public string background { get; set; }
        [Required]
        public string goal { get; set; }
        /// <summary>
        /// วงเงินมติ ครม./กรอบวงเงิน
        /// </summary>
        [Required]
        public decimal budget { get; set; }
        /// <summary>
        /// Import content (ร้อยละ: %))
        /// </summary>
        [Required]
        public decimal importContent { get; set; }
        [Required]
        public DateTime startedAt { get; set; }
        [Required]
        public DateTime endedAt { get; set; }
        /// <summary>
        /// ประเภทโครงการ (แยกตามแหล่งเงิน) ให้ระบุ เป็น "GIP”
        /// </summary>
        [Required]
        public string flagTypeId { get; set; }
        /// <summary>
        /// ประเภทโครงการ
        /// </summary>
        [Required]
        public string kindTypeId { get; set; }
        /// <summary>
        /// สถานะโครงการ
        /// </summary>
        [Required]
        public string operationTypeCode { get; set; }
        /// <summary>
        /// วันที่โครงการแล้วเสร็จโครงการแล้วเสร็จทั้งการดำาเนินกงานและการเบิกจ่าย ซึ่งสถานะโครงการต้อง "GIP”เสร็จสิ้นโครงการ”
        /// </summary>
        public DateTime operationAt { get; set; }
        /// <summary>
        /// ประเภทของการกู้เงิน
        /// </summary>
        public long? creditChannelId { get; set; }
        public string directorName { get; set; }
        public  string directorPosition { get; set;}
        public string directorMail { get; set; }
        public string directorTel { get; set; }
        /// <summary>
        /// รัฐรับภาระ
        /// </summary>
        public bool isGovBurden { get; set; }
        /// <summary>
        /// เป็นโครงการต่อเนื่อง
        /// </summary>
        public bool isOnGoing { get; set; }
        /// <summary>
        /// เป็นโครงการกู้ต่อ
        /// </summary>
        public bool hasEld { get; set; }

    }

    public class IIPMProjectScope
    {
        public List<IIPMScope> items { get; set; }
    }
    public class IIPMScope
    {
        public long id { get; set; }
        public string scope { get; set; }

    }
    public class IIPMProjectBenefit
    {
        public List<IIPMBenefit> items { get; set; }
    }
    public class IIPMBenefit
    {
        public long id { get; set; }
        public string benefits { get; set; }

    }
}
