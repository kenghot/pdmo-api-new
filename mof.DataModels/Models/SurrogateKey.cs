using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class SurrogateKey
    {
        public string GroupCode { get; set; }
        public string Prefix { get; set; }
        public long? Runno { get; set; }
    }
}
