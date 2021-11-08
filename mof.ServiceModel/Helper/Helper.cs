using mof.ServiceModels.Common.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace mof.ServiceModels.Helper
{
    public class CurrencyData
    {
        public int Year { get; set; }
        /// <summary>
        /// Rate ณ.วันที่
        /// </summary>
        public DateTime RateAsOF { get; set; }
        /// <summary>
        /// แหล่งที่มา
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// หมายเหตุ
        /// </summary>
        public string Remark { get; set; }
        public List<CurrecyRateData> Currency { get; set; }

    }
    public class CurrecyRateData
    {
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public decimal CurrencyRate { get; set; }
    }

    public class ParameterData
    {
        public int Year { get; set; }
        public decimal GDP { get; set; }
        /// <summary>
        /// ดอกเบี้ย
        /// </summary>
        public decimal Interest { get; set; }
        /// <summary>
        /// รายได้จากการส่งออกสินค้าและบริการ
        /// </summary>
        public decimal ExportIncome { get; set; }
        /// <summary>
        /// งบขำระหนี้
        /// </summary>
        public decimal DebtSettlement { get; set; }
        /// <summary>
        /// ประมาณการรายได้ประจำปีงบประมาณ
        /// </summary>
        public decimal EstIncome { get; set; }

        public LogData Log { get; set; }
    }
    public class CurrencyScreen
    {
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
     
        public LogData Log { get; set; }
    }
}
