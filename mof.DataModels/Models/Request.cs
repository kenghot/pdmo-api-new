using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class Request
    {
        public Request()
        {
            Organization = new HashSet<Organization>();
        }

        public long RequestId { get; set; }
        public long RequestType { get; set; }
        public long RequestStatus { get; set; }
        public string UserId { get; set; }
        public DateTime RequestDt { get; set; }
        public string RequestData { get; set; }
        public long? IssueId { get; set; }
        public byte[] TimeStamp { get; set; }

        public virtual CeLov RequestStatusNavigation { get; set; }
        public virtual CeLov RequestTypeNavigation { get; set; }
        public virtual ICollection<Organization> Organization { get; set; }
    }
}
