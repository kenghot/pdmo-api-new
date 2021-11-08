using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class MasterAgreementMapping
    {
        public string Orgtype { get; set; }
        public string Pjtype { get; set; }
        public string LocalLoan { get; set; }
        public string ForeignLoan { get; set; }
    }
}
