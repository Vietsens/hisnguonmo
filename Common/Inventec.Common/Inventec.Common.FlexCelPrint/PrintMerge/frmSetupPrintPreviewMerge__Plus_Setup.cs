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
using FlexCel.Core;
using FlexCel.Render;
using FlexCel.XlsAdapter;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Inventec.Common.FlexCelPrint
{
    /// <summary>
    /// Printing / Previewing and Exporting xls files.
    /// </summary>
    public partial class frmSetupPrintPreviewMerge : DevExpress.XtraEditors.XtraForm
    {
        private bool DoSetup()
        {
            pageSetupDialog1.AllowMargins = false;
            pageSetupDialog1.AllowOrientation = false;
            pageSetupDialog1.AllowPaper = false;
            pageSetupDialog1.AllowPrinter = false;
            pageSetupDialog1.Reset();

            pageSetupDialog1.PrinterSettings = flexCelPrintDocument1.PrinterSettings;
            pageSetupDialog1.PageSettings = flexCelPrintDocument1.DefaultPageSettings;
            pageSetupDialog1.Document = flexCelPrintDocument1;

            bool Result = pageSetupDialog1.ShowDialog() == DialogResult.OK;
            try
            {
                if (Result)
                {
                    flexCelPrintDocument1.DefaultPageSettings = pageSetupDialog1.PageSettings;
                    flexCelPrintDocument1.PrinterSettings = pageSetupDialog1.PrinterSettings;

                    flexCelPrintDocument1.DefaultPageSettings.PaperSource = pageSetupDialog1.PageSettings.PaperSource;
                    rdLandscape.Checked = flexCelPrintDocument1.DefaultPageSettings.Landscape;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return Result;
        }

        #region Hard Margins
        //Shows how to read the hard margins from a printer if you really need to.

        private void flexCelPrintDocument1_BeforePrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.InterpolationMode = InterpolationMode.Bicubic;
        }

        [DllImport("gdi32.dll")]
        private static extern Int32 GetDeviceCaps(IntPtr hdc, Int32 capindex);

        /// <summary>
        /// This event will adjust for a better position on the page for some printers. 
        /// It is not normally necessary, and it has to make an unmanaged call to GetDeviceCaps,
        /// but it is given here as an example of how it could be done.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void flexCelPrintDocument1_GetPrinterHardMargins(object sender, FlexCel.Render.PrintHardMarginsEventArgs e)
        {
            const int PHYSICALOFFSETX = 112;
            const int PHYSICALOFFSETY = 113;

            double DpiX = e.Graphics.DpiX;
            double DpiY = e.Graphics.DpiY;

            IntPtr Hdc = e.Graphics.GetHdc();
            try
            {
                e.XMargin = (float)(GetDeviceCaps(Hdc, PHYSICALOFFSETX) * 100.0 / DpiX);
                e.YMargin = (float)(GetDeviceCaps(Hdc, PHYSICALOFFSETY) * 100.0 / DpiY);
            }

            finally
            {
                e.Graphics.ReleaseHdc(Hdc);
            }

        }
        #endregion

    }
}
