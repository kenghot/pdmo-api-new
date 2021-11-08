using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace mof.ServiceModels
{
    public class IdentityServerConfig
    {
        //public string TokenEndpoint { get; set; }
        public string Client { get; set; }
        public string ClientSecret { get; set; }
        //public string IntrospectionEndpoint { get; set; }
        public DiscoveryResponse disc { get; set; }
    }
}
