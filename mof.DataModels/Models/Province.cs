using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class Province
    {
        public Province()
        {
            Organization = new HashSet<Organization>();
        }

        public long ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public byte[] TimeStamp { get; set; }

        public virtual ICollection<Organization> Organization { get; set; }
    }
}
