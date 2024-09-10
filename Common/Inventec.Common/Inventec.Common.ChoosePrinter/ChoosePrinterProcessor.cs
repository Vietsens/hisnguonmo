using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ChoosePrinter
{
    public class ChoosePrinterProcessor
    {
        public static string GetPrinter()
        {
            string result = "";
            try
            {
                var formChoose = new ChoosePrinter();
                formChoose.ShowDialog();
                result = formChoose.PrinterName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
