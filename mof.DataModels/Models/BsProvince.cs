using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class BsProvince
    {
        public long ProvinceId { get; set; }
        public int? ProvinceCode { get; set; }
        public int? SectionCode { get; set; }
        public string ProvinceName { get; set; }
    }
}
