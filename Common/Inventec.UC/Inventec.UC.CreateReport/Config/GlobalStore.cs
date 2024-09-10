using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.CreateReport.Config
{
    internal class GlobalStore
    {
        internal static long NumberPage { get; set; }
        internal static string pathFileIcon { get; set; }
        internal static List<SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE> reportTemplates;
        internal static SAR.EFMODEL.DataModels.SAR_REPORT_TYPE reportType;

        public static DateTime? ConvertDateStringToSystemDate(string date)
        {
            DateTime? result = DateTime.MinValue;
            try
            {
                if (!String.IsNullOrEmpty(date))
                {
                    date = date.Replace(" ", "");
                    int day = Int16.Parse(date.Substring(0, 2));
                    int month = Int16.Parse(date.Substring(3, 2));
                    int year = Int16.Parse(date.Substring(6, 4));
                    result = new DateTime(year, month, day);
                }
            }
            catch (Exception ex)
            {
                result = null;
            }

            return result;
        }

       
    }
}
