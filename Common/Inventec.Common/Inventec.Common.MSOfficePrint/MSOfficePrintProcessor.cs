/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.MSOfficePrint
{
    public class MSOfficePrintProcessor
    {
        string TemplateFile { get; set; }
        string ActivePrinter { get; set; }
        int? From { get; set; }
        int? To { get; set; }
        int? Copies { get; set; }
        bool? Preview { get; set; }

        public MSOfficePrintProcessor(string tempFile, int? from, int? to, int? copies, bool? preview, string activePrinter)
        {
            this.TemplateFile = tempFile;
            this.ActivePrinter = activePrinter;
            this.From = from;
            this.To = to;
            this.Copies = copies;
            this.Preview = preview;
        }

        public MSOfficePrintProcessor(MemoryStream stream, int? from, int? to, int? copies, bool? preview, string activePrinter)
        {
            this.ActivePrinter = activePrinter;
            this.From = from;
            this.To = to;
            this.Copies = copies;
            this.Preview = preview;
            this.TemplateFile = Path.GetTempFileName();
            using (FileStream outputStream = new FileStream(this.TemplateFile, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                stream.Position = 0;
                stream.CopyTo(outputStream);
            }
        }

        public bool Print()
        {
            bool success = false;
            try
            {

                Inventec.Common.Logging.LogSystem.Info("TemplateFile:" + this.TemplateFile);

                Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();

                //excelApp.Visible = true;
                // Open the Workbook:
                Microsoft.Office.Interop.Excel.Workbook wb = excelApp.Workbooks.Open(this.TemplateFile);//, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                // Get the first worksheet.
                // (Excel uses base 1 indexing, not base 0.)
                Microsoft.Office.Interop.Excel.Worksheet ws = (Microsoft.Office.Interop.Excel.Worksheet)wb.ActiveSheet;

                // Print out 1 copy to the default printer:
                ws.PrintOut(
                    this.From.HasValue ? this.From.Value : Type.Missing,
                    this.To.HasValue ? this.To.Value : Type.Missing,
                    this.Copies.HasValue ? this.Copies.Value : Type.Missing,
                    this.Preview.HasValue ? this.Preview.Value : Type.Missing,
                    !String.IsNullOrEmpty(this.ActivePrinter) ? this.ActivePrinter : Type.Missing,
                    Type.Missing,
                    Type.Missing,
                    Type.Missing);

                success = true;//Print success

                // Cleanup:
                GC.Collect();
                GC.WaitForPendingFinalizers();

                Marshal.FinalReleaseComObject(ws);

                wb.Close(false, Type.Missing, Type.Missing);
                Marshal.FinalReleaseComObject(wb);

                excelApp.Quit();
                Marshal.FinalReleaseComObject(excelApp);
                try
                {
                    if (File.Exists(this.TemplateFile))
                    {
                        File.Delete(this.TemplateFile);
                        Inventec.Common.Logging.LogSystem.Info("Delete file after print -> Ok");
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Info("Delete file after print -> Fail");
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return success;
        }
    }
}
