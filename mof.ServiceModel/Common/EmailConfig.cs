using System;
using System.Collections.Generic;
using System.Text;

namespace mof.ServiceModels.Common
{
    public class EmailConfig
    {
        public int Port { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public string Host { get; set; }
        public bool EnableSsl { get; set; }
        public string EmailUser { get; set; }
        public string EmailPassword { get; set; }

    }
}
