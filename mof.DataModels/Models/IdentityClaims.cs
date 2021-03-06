using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class IdentityClaims
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int IdentityResourceId { get; set; }

        public virtual IdentityResources IdentityResource { get; set; }
    }
}
