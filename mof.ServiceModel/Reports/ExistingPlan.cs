using mof.ServiceModels.Common.Generic;
using mof.ServiceModels.Plan;
using System;
using System.Collections.Generic;
using System.Text;

namespace mof.ServiceModels.Reports.ExistingPlan
{
    #region ExistingPlanByAgreement

    public class ExistingPlanByAgreement
    {
        public DateTime RepDate { get; set; }
        public string OrganizationName { get; set; }
        public string Ministry { get; set; }
        public string Year { get; set; } 
        public string Year5Y { get; set; }
        public List<EPBAItem> Items { get; set; } = new List<EPBAItem>();
        public List<EPBAItem> Forcasts { get; set; } = new List<EPBAItem>();
        public List<PaidItem> PaidItems { get; set; } = new List<PaidItem>();
        public List<PaidItem> PaidCurrItems { get; set; } = new List<PaidItem>();
        public DebtSettlementInfoModel Info { get; set; } = new DebtSettlementInfoModel();
    }
    public class PaidItem
    {
       
        public long PlanExistID { get; set; }
        public List<AgreementItem> Agreements { get; set; } = new List<AgreementItem>();
        public decimal ActualDueAmount { get; set; }
        public AmountItem PaidAmount { get; set; } = new AmountItem();
        public AmountItem RestructureAmt { get; set; } = new AmountItem();
        public string RestructureTxt { get; set; }
    }
    public class AmountItem
    {
        public decimal THBAmt { get; set; }
        private Dictionary<string, decimal> currAmt = new Dictionary<string, decimal>();
        public Dictionary<string, decimal> CurrAmt {
            get {
                return currAmt;
            }
            set {
                currAmt = value;
            } }
        public string CurrAmtTxt
        {
            get
            {
                string ret = "";
                foreach (var item in currAmt)
                {   
                    ret += $"{item.Value:##,#0.#0} ({item.Key}) ";
                }
                return ret;
            }
        }
        public void AddCurrAmt ( string currCode, decimal amt)
        {
            decimal val;
            if (currAmt.TryGetValue(currCode, out val))
            {

                currAmt[currCode] = val + (amt  );
            }
            else
            {

                currAmt.Add(currCode, amt );
            }
        }

    }
    public class AgreementItem
    {
        public long AgreementId { get; set; }
        public string GFCode { get; set; }
        public string AgreementName { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal LoanAmountTHB { get; set; }
        public string   LoanCurr { get; set; }
        public decimal LoanAge { get; set; }
        public decimal InterestRate { get; set; }
        public decimal DueAmount { get; set; }
        public DateTime DueDate { get; set; }
        public string DebtSubType { get; set; }
        public DateTime LoanDate { get; set; }
        public Decimal OutStandingDebt { get; set; }
        public Decimal OutStandingDebtTHB { get; set; }
        public int LoanLeftYear { get; set; }
        public decimal ActualDueAmount { get; set; }
        public BasicData TransType { get; set; } = new BasicData { Description = "ไม่พบข้อมูล" };

    }
    public class EPBAItem
    {
        public long PlanExistID { get; set; }
        public string Year { get; set; } = "";
        public List<AgreementItem> Agreements { get; set; } = new List<AgreementItem>();
        public string AgreementText { get; set; } = "";
        public BasicData TransType { get; set; } = new BasicData { Description = "ไม่พบข้อมูล" };
        public EPBAAmountItem LItem { get; set; } = new EPBAAmountItem();
        public EPBAAmountItem FItem { get; set; } = new EPBAAmountItem();
    }
    public class EPBAAmountItem
    {
        public decimal RePayment { get; set; }
        public decimal PrePayment { get; set; }
        public decimal RollOver { get; set; }
        public decimal Refinance { get; set; }
        public decimal CCS { get; set; }
        public decimal IRS { get; set; }
        public decimal Other { get; set; }
        public decimal Total { get; set; }
        public decimal Interest { get; set; }
    }
    #endregion
}
