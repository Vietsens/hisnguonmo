using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SAR.EFMODEL.Decorator
{
    public partial class DenyUpdateDecorator
    {
        private static Dictionary<Type, List<string>> properties = new Dictionary<Type, List<string>>();
        private static bool isLoaded = false;
        private static void Load()
        {
            try
            {
                if (!isLoaded)
                {
                    LoadSarPrint();
                    LoadSarFormField();
                    LoadSarRetyFofi();
                    LoadSarPrintType();
                    LoadSarReport();
                    LoadSarReportCalendar();
                    LoadSarReportStt();
                    LoadSarReportTemplate();
                    LoadSarReportType();
                    LoadSarUserReportType();
                    LoadSarFormType();
                    LoadSarFormData();
                    LoadSarForm();
                    LoadSarPrintLog();
                    LoadSarReportTypeGroup();
                    LoadSarPrintTypeCfg();

                    isLoaded = true;
                }
            }
            catch (Exception)
            {

            }
        }

        public static List<string> Get<RAW>()
        {
            try
            {
                Load();
                if (properties.ContainsKey(typeof(RAW)))
                {
                    return properties[typeof(RAW)];
                }
            }
            catch (Exception)
            {

            }
            return null;
        }
    }
}
