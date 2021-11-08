using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class ProjMaterial
    {
        public long ProjMaterialId { get; set; }
        public long ProjectId { get; set; }
        public decimal LimitAmount { get; set; }
        public string CurrencyCode { get; set; }
        public byte[] TimeStamp { get; set; }
        public string SourceType { get; set; }
        public decimal CurrencyRate { get; set; }

        public virtual Project Project { get; set; }
    }
}
