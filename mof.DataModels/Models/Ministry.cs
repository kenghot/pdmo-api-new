using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class Ministry
    {
        public long MinistryId { get; set; }
        public string MinistryCode { get; set; }
        public string MinistryThname { get; set; }
        public string MinistryEnname { get; set; }
        public byte[] TimeStamp { get; set; }
        public bool IsCanceled { get; set; }
    }
}
