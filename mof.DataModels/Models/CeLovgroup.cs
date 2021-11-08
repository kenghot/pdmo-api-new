using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class CeLovgroup
    {
        public CeLovgroup()
        {
            CeLov = new HashSet<CeLov>();
        }

        public string LovgroupCode { get; set; }
        public string LovgroupName { get; set; }
        public byte[] TimeStamp { get; set; }
        public bool IsCanceled { get; set; }

        public virtual ICollection<CeLov> CeLov { get; set; }
    }
}
