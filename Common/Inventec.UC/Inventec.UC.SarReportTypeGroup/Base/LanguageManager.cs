using DevExpress.XtraEditors.Controls;
using Inventec.Common.Logging;
using Inventec.Common.XmlConfig;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReportTypeGroup.Base
{
    public class LanguageManager
    {
        private static CultureInfo cultureInfo { get; set; }

        public static bool Init(CultureInfo current)
        {
            bool result = false;
            try
            {
                cultureInfo = current;
            }
            catch (Exception ex)
            {
                result = false;
                LogSystem.Error(ex);
            }
            return result;
        }

        public static CultureInfo GetCulture()
        {
            CultureInfo result = null;
            try
            {
                if (cultureInfo == null)
                {
                    cultureInfo = new CultureInfo("vi");
                }
                result = cultureInfo;
            }
            catch (Exception ex)
            {
                result = null;
                LogSystem.Error(ex);
            }
            return result;
        }
    }
}
