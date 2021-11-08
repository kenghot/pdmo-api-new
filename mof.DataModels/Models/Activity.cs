using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class Activity
    {
        public Activity()
        {
            ProjAct = new HashSet<ProjAct>();
        }

        public long ActivityId { get; set; }
        public string ActivityName { get; set; }
        public string ActivityDetail { get; set; }
        public byte[] TimeStamp { get; set; }

        public virtual ICollection<ProjAct> ProjAct { get; set; }
    }
}
