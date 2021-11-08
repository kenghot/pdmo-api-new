using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class Orgorgtype
    {
        public long OrgorgtypeId { get; set; }
        public long OrganizationId { get; set; }
        public long Lovid { get; set; }

        public virtual Organization Organization { get; set; }
    }
}
