using mof.ServiceModels.FinancialReport;
using System;
using System.Collections.Generic;
using System.Text;

namespace mof.ServiceModels.Reports
{
    public class FinancialRep
    {
        public DateTime RepDate { get; set; }
        public string OrganizationName { get; set; }
        public string Ministry { get; set; }
        public string Year { get; set; }
        public List<FinancialRatio> FinancialRatios { get; set; }
        public FNRepRatio Ratio;
    }
    public class FNRepRatio
    {
        public FinancialRatio R1;
        public FinancialRatio R2;
        public FinancialRatio R3;
        public FinancialRatio R4;
        public FinancialRatio R5;
        public FinancialRatio R6;
        public FinancialRatio R7;
        public FinancialRatio R8;
        public FinancialRatio R9;
        public FinancialRatio R10;
    }
}
