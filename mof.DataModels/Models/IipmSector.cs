using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class IipmSector
    {
        public long SectId { get; set; }
        public string Name { get; set; }
        public int? Level { get; set; }
        public long? ParentId { get; set; }
        public bool? IsActive { get; set; }
    }
}
