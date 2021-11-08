using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class Parameter
    {
        public int Year { get; set; }
        public decimal Gdp { get; set; }
        public long DataLog { get; set; }
        public byte[] TimeStamp { get; set; }
        public decimal Interest { get; set; }
        public decimal ExportIncome { get; set; }
        public decimal DebtSettlement { get; set; }
        public decimal EstIncome { get; set; }
        public decimal? Budget { get; set; }

        public virtual DataLog DataLogNavigation { get; set; }
    }
}
