using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class CurrencyYear
    {
        public CurrencyYear()
        {
            CurrencyRate = new HashSet<CurrencyRate>();
        }

        public int Year { get; set; }
        public DateTime RateDate { get; set; }
        public string Source { get; set; }
        public string Remark { get; set; }
        public byte[] TimeStamp { get; set; }

        public virtual ICollection<CurrencyRate> CurrencyRate { get; set; }
    }
}
