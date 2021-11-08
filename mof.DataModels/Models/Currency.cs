using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class Currency
    {
        public Currency()
        {
            CurrencyRate = new HashSet<CurrencyRate>();
        }

        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public byte[] TimeStamp { get; set; }

        public virtual ICollection<CurrencyRate> CurrencyRate { get; set; }
    }
}
