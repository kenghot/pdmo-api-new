using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class CeLovextend
    {
        public long CeLovextendKey { get; set; }
        public long Lovkey { get; set; }
        public string ExtendType { get; set; }
        public string ExtendValue { get; set; }
        public string TimeStamp { get; set; }

        public virtual CeLov LovkeyNavigation { get; set; }
    }
}
