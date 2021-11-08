using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class Orglod
    {
        public long Orglodid { get; set; }
        public long OrganizationId { get; set; }
        public long LawOfdebtId { get; set; }
        public byte[] TimeStamp { get; set; }

        public virtual LawOfdebt LawOfdebt { get; set; }
        public virtual Organization Organization { get; set; }
    }
}
