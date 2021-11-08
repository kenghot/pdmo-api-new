using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class ApiLog
    {
        public long Id { get; set; }
        public long? SessionId { get; set; }
        public string Request { get; set; }
        public string RequestContent { get; set; }
        public DateTime? RequestDt { get; set; }
        public string ResponseStatus { get; set; }
        public string Response { get; set; }
        public DateTime? ResponseDt { get; set; }
        public string Action { get; set; }
        public string ApiEndpoint { get; set; }

        public virtual ApiSession Session { get; set; }
    }
}
