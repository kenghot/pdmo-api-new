using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class ApiSession
    {
        public ApiSession()
        {
            ApiLog = new HashSet<ApiLog>();
        }

        public long Id { get; set; }
        public DateTime? SessionDt { get; set; }
        public DateTime? FinishDt { get; set; }
        public byte[] Action { get; set; }

        public virtual ICollection<ApiLog> ApiLog { get; set; }
    }
}
