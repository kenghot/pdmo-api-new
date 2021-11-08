using Microsoft.Extensions.Localization;
using mof.DataModels.Models;
using mof.ServiceModels.Common;
using mof.ServiceModels.Common.Generic;
using mof.ServiceModels.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using mof.ServiceModels.Plan;
using System.Net.Http;
using System.Threading.Tasks;

namespace mof.Services.Helper
{
   
    public class Utilities
    {
        public static string YMDDiffString(DateTime fromDate, DateTime toDate)
        {
            DateTime zeroTime = new DateTime(1, 1, 1);
        

            TimeSpan span = toDate - fromDate;

            // because we start at year 1 for the Gregorian 
            // calendar, we must subtract a year here.

            int years = (zeroTime + span).Year - 1;
            int months = (zeroTime + span).Month - 1;
            int days = (zeroTime + span).Day;


            return $"{years} ปี {months} เดือน {days} วัน";
        }
        public static long StringToKey(string code)
        {
            var bs = Encoding.ASCII.GetBytes(code);
            string ret = "1";
            foreach (var b in bs)
            {
                ret += b.ToString().PadLeft(2, '0');
            }
            return long.Parse(ret);

        }
        public static void SumLoanSource(List<LoanSource> lss, LoanSource ls)
        {
            var sum = lss.Where(w => w.Currency == ls.Currency && w.SourceType == ls.SourceType).FirstOrDefault();
            if (sum == null)
            {
                sum = new LoanSource
                {
                    Currency = ls.Currency,
                    SourceType = ls.SourceType,

                };
                lss.Add(sum);
            }
            sum.LoanAmount += ls.LoanAmount;
            sum.THBAmount += ls.THBAmount;
        }
        public static long GetLovKeyFromCode(List<LOV> lovs, string lovCode)
        {
            var lov = lovs.Where(w => w.LOVCode == lovCode).FirstOrDefault();
            if (lov != null)
            {
                return lov.LOVKey;
            }else
            {
                return -1;
            }
        }
        public static string GetLovCodeFromKey(List<LOV> lovs, long lovKey)
        {
            var lov = lovs.Where(w => w.LOVKey == lovKey).FirstOrDefault();
            if (lov != null)
            {
                return lov.LOVCode;
            }
            else
            {
                return null;
            }
        }
        public static ReturnObject<string> GetMMIEType(string extension, IStringLocalizer<MessageLocalizer> msg)
        {
            var ret = new ReturnObject<string>(msg);
            ret.IsCompleted = false;
            string mime = "";
            if (ServiceModels.Constants.ConstantValue.MIME.TryGetValue(extension,out mime))
            {
                ret.IsCompleted = true;
                ret.Data = mime;
            } 
            else
            {
                ret.AddMessage(eMessage.FileTypeNotAllow.ToString(), "file", eMessageType.Error, new string[] { extension });
            }
            return ret;

        }
        
        public class PeriodObject
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public string StringYear { get; set; }
            public string StringMonth { get; set; }
            public int Full { get; set; }
            public PeriodObject(int periodVal)
            {
                Full = periodVal;
                string str = periodVal.ToString().PadLeft(6,'0');
                Year = int.Parse(str.Substring(0, 4));
                Month = int.Parse(str.Substring(4, 2));
                var d = new DateTime( Year, Month, 1);

                StringYear = Year.ToString();
                StringMonth = d.ToString("MMMM", new CultureInfo("th-TH")) ;
            }
        }


    }
}
