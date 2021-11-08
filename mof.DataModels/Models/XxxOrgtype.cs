using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class XxxOrgtype
    {
        public long OrgtypeId { get; set; }
        public string OrgtypeCode { get; set; }
        public string OrgtypeThname { get; set; }
        public string OrgtypeEnname { get; set; }
        public bool IsCanceled { get; set; }
        public byte[] TimeStamp { get; set; }
        public int? OrderNo { get; set; }
    }
}
