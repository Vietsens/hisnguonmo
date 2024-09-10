using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.Print
{
    public class FlexCelPrintStore
    {
        public static List<string> GetSystemPrintNames()
        {
            List<string> systemPrintNames = new List<string>();
            try
            {
                foreach (String printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                {
                    systemPrintNames.Add(printer);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
            return systemPrintNames;
        }
    }
}
