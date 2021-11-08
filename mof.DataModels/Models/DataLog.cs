using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class DataLog
    {
        public DataLog()
        {
            Parameter = new HashSet<Parameter>();
            PlanDataLogNavigation = new HashSet<Plan>();
            PlanProposalStatusNavigation = new HashSet<Plan>();
            Project = new HashSet<Project>();
        }

        public long LogId { get; set; }
        public string TableName { get; set; }
        public long TableKey { get; set; }
        public string UserId { get; set; }
        public DateTime LogDt { get; set; }
        public string LogType { get; set; }
        public byte[] TimeStamp { get; set; }
        public string Remark { get; set; }
        public long? LogStatus { get; set; }

        public virtual CeLov LogStatusNavigation { get; set; }
        public virtual ICollection<Parameter> Parameter { get; set; }
        public virtual ICollection<Plan> PlanDataLogNavigation { get; set; }
        public virtual ICollection<Plan> PlanProposalStatusNavigation { get; set; }
        public virtual ICollection<Project> Project { get; set; }
    }
}
