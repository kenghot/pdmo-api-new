using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class CurrencyRate
    {
        public long RateId { get; set; }
        public int CurrencyYear { get; set; }
        public string CurrencyCode { get; set; }
        public decimal CurrencyRate1 { get; set; }
        public byte[] TimeStamp { get; set; }

        public virtual Currency CurrencyCodeNavigation { get; set; }
        public virtual CurrencyYear CurrencyYearNavigation { get; set; }
    }
}
