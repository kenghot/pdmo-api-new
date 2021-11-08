using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class AgreementTrans
    {
        public long AgreemantTransId { get; set; }
        public DateTime? PostinDate { get; set; }
        public long? AgreementId { get; set; }
        public byte[] TimeStamp { get; set; }
        public string GftrrefCode { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public long FlowType { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public decimal BaseAmount { get; set; }

        public virtual Agreement Agreement { get; set; }
        public virtual CeLov FlowTypeNavigation { get; set; }
    }
}
