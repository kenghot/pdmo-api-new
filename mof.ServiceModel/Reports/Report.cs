using System;
using System.Collections.Generic;
using System.Text;

namespace mof.ServiceModels.Reports
{
    public class FReport
    {
        public int Id { get; set; }
        public string ReportName { get; set; }
    }
    public class FReportQuery
    {
        // Format of resulting report: png, pdf, html
        public string Format { get; set; }
        // Enable Inline preview in browser (generates "inline" or "attachment")
        public bool Inline { get; set; }
        // Value of "Parameter" variable in report
        public string Parameter { get; set; }
    }
}
