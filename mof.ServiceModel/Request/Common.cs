using System;
using System.Collections.Generic;
using System.Text;

namespace mof.ServiceModels.Request.Common
{
    public class GetByID
    {
        public long? ID { get; set; }
        public string UserID { get; set; }
        public Paging Paging { get; set; }
    }
}
