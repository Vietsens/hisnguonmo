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
using System.Text;
using System.Threading.Tasks;
using DevExpress.Spreadsheet;
using DevExpress.Utils;
using DevExpress.XtraRichEdit;

namespace Inventec.Common.FileConvert
{
    public class ConvertByPDFNet
    {
        public ConvertByPDFNet() { }

        private pdftron.PDFNetLoader pdfNetLoader = pdftron.PDFNetLoader.Instance();

        public bool DocToPdfByPdfTron(string inputFile, string outputFile)
        {
            bool success = false;
            try
            {
                pdftron.PDFNet.Initialize("I-Warez 2015:OEM:AZBYCXAZBYCXAZBYCXAZBYCXAZBYCXAZBYCX");
                if (String.IsNullOrEmpty(inputFile))
                    throw new ArgumentNullException("inFile is null");

                if (String.IsNullOrEmpty(outputFile))
                {
                    throw new ArgumentNullException("outFile is null");
                }

                Boolean uninstallPrinterWhenDone = false;
                if (pdftron.PDF.Convert.Printer.IsInstalled("PDFTron Creator"))
                {
                    pdftron.PDF.Convert.Printer.SetPrinterName("PDFTron Creator");
                }
                else if (!pdftron.PDF.Convert.Printer.IsInstalled())
                {
                    try
                    {
                        Console.WriteLine("Installing printer (requires administrator)");
                        pdftron.PDF.Convert.Printer.Install();
                        Console.WriteLine("Installed printer " + pdftron.PDF.Convert.Printer.GetPrinterName());
                        // the function ConvertToXpsFromFile may require the printer so leave it installed
                        // uninstallPrinterWhenDone = true;
                    }
                    catch (pdftron.Common.PDFNetException e)
                    {
                        Console.WriteLine("ERROR: Unable to install printer");
                        Console.WriteLine(e.Message);
                    }
                }


                using (pdftron.PDF.PDFDoc pdfdoc = new pdftron.PDF.PDFDoc())
                {
                    if (pdftron.PDF.Convert.RequiresPrinter(inputFile))
                    {
                        Console.WriteLine("Printing file: " + inputFile);
                    }
                    pdftron.PDF.Convert.ToPdf(pdfdoc, inputFile);
                    pdfdoc.Save(outputFile, pdftron.SDF.SDFDoc.SaveOptions.e_linearized);
                }

                if (uninstallPrinterWhenDone)
                {
                    try
                    {
                        Console.WriteLine("Uninstalling printer (requires administrator)");
                        pdftron.PDF.Convert.Printer.Uninstall();
                        Console.WriteLine("Uninstalled printer " + pdftron.PDF.Convert.Printer.GetPrinterName());
                    }
                    catch (pdftron.Common.PDFNetException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                success = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return success;
        }
    }
}
