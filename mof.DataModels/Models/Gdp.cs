using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class Gdp
    {
        public int Year { get; set; }
        public decimal Gdp1 { get; set; }
        public long DataLog { get; set; }
        public byte[] TimeStamp { get; set; }

        public virtual DataLog DataLogNavigation { get; set; }
    }
}
