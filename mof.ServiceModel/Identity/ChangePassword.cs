using System;
using System.Collections.Generic;
using System.Text;

namespace mof.ServiceModels.Identity
{
    public class ChangePassword
    {
        public string UserName { get; set; }
        public string PIN { get; set; }
        public string NewPassword { get; set; }
    }

}
