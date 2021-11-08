using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class PlanAct
    {
        public PlanAct()
        {
            AgreementAct = new HashSet<AgreementAct>();
            InverseReferencePlanAct = new HashSet<PlanAct>();
            PlanActAmount = new HashSet<PlanActAmount>();
        }

        public long PlanActId { get; set; }
        public long PlanProjId { get; set; }
        public string ActivityName { get; set; }
        public byte[] TimeStamp { get; set; }
        public long ProjActId { get; set; }
        public long? ReferencePlanActId { get; set; }
        public string MasterAgreement { get; set; }

        public virtual PlanProject PlanProj { get; set; }
        public virtual PlanAct ReferencePlanAct { get; set; }
        public virtual ICollection<AgreementAct> AgreementAct { get; set; }
        public virtual ICollection<PlanAct> InverseReferencePlanAct { get; set; }
        public virtual ICollection<PlanActAmount> PlanActAmount { get; set; }
    }
}
