using System;
using System.Collections.Generic;
using System.Text;

namespace mof.ServiceModels.PublicDebt
{
    public class PublicDebtDashBoard
    {   
        public decimal publicDebtGDPPercent { get; set; }
        public decimal publicDebtGDPValue { get; set; }
        public List<PublicDebtSummary> publicDebtSummaryTable { get; set; }
        public List<CommonPieChart> publicDebtRemainningAgePieChart { get; set; }
        public List<CommonPieChart> publicDebtCurrencyPieChart { get; set; }
        public List<CommonComposeChart> publicDebtAndGDPComposeChart { get; set; }
    }

    public class PublicDebtSummary
    {
        public string elementsDebt { get; set; }
        public decimal millionBaht { get; set; }
        public decimal fiveYearPlan { get; set; }
        public decimal gdp { get; set; }
    }
    public class CommonPieChart
    {
        public string name { get; set; }
        public decimal value { get; set; }
        public string color { get; set; }
    }
    public class CommonComposeChart
    {
        public string name { get; set; }
        public decimal a { get; set; }
        public decimal b { get; set; }
        public decimal c { get; set; }
        public decimal d { get; set; }
        public decimal e { get; set; }
        public decimal f { get; set; }
        public decimal g { get; set; }
        public decimal gdp { get; set; }
    }
}
