using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class ProjAct
    {
        public ProjAct()
        {
            ProjActAmount = new HashSet<ProjActAmount>();
        }

        public long ProjActId { get; set; }
        public long ProjectId { get; set; }
        public byte[] TimeStamp { get; set; }
        public string ActivityName { get; set; }

        public virtual Project Project { get; set; }
        public virtual ICollection<ProjActAmount> ProjActAmount { get; set; }
    }
}
