using Aspose.Cells;
using Aspose.Cells.Rendering;
using Inventec.Common.FlexCelPrint;
using Inventec.Common.Print.License;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.Print
{
    class PrintNowProcess
    {
        DelegateEventLog eventLog;
        DelegateReturnEventPrint eventPrint;
        DelegatePrintLog printLog;
        Workbook workbook;
        string defaultPrintName;
        int numCopy;

        internal PrintNowProcess(MemoryStream data, string defaultPrintName, int numCopy, DelegateEventLog eventLog = null, DelegateReturnEventPrint eventPrint = null, DelegatePrintLog printLog = null)
        {
            this.eventLog = eventLog;
            this.eventPrint = eventPrint;
            this.printLog = printLog;
            this.defaultPrintName = defaultPrintName;
            this.numCopy = numCopy;

            data.Position = 0;
            LicenceProcess.SetLicenseForAspose();
            workbook = new Workbook(data);
        }

        internal void PrintNow()
        {
            Inventec.Common.Logging.LogSystem.Debug("PrintNow.1");
            PrinterSettings PrinterSettings = new System.Drawing.Printing.PrinterSettings();
            string printerName = PrinterSettings.PrinterName;
            if (!String.IsNullOrEmpty(defaultPrintName))
            {
                printerName = defaultPrintName;
            }
            // Apply different Image/Print options.
            Aspose.Cells.Rendering.ImageOrPrintOptions options = new Aspose.Cells.Rendering.ImageOrPrintOptions();
            //options.ImageType = Drawing.ImageType.Tiff;
            options.PrintingPage = PrintingPageType.Default;

            // To print a whole workbook, iterate through the sheets and print them, or use the WorkbookRender class.
            WorkbookRender wr = new WorkbookRender(workbook, options);

            try
            {
                wr.PageCount = this.numCopy;
                // Print the workbook.
                wr.ToPrinter(printerName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            Inventec.Common.Logging.LogSystem.Debug("PrintNow.2");
        }
    }
}
