using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class LawOfdebt
    {
        public LawOfdebt()
        {
            Orglod = new HashSet<Orglod>();
        }

        public long LawOfdebtId { get; set; }
        public string Detail { get; set; }
        public byte[] TimeStamp { get; set; }
        public string Title { get; set; }

        public virtual ICollection<Orglod> Orglod { get; set; }
    }
}
