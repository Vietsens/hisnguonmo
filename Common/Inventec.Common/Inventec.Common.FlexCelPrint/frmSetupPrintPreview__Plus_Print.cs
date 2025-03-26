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
using Inventec.Common.FlexCelPrint.DocumentPrint;
using Inventec.Common.FlexCelPrint.FormPrintReason;
using Inventec.Common.Logging;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Inventec.Common.FlexCelPrint
{
    /// <summary>
    /// Printing / Previewing and Exporting xls files.
    /// </summary>
    public partial class frmSetupPrintPreview : DevExpress.XtraEditors.XtraForm
    {
        public void Print()
        {
            try
            {
                if (this.emrInputADO != null)
                    this.emrInputADO.PaperSizeDefault = currentPaperSize;

                if (isPreview == true) return;

                if (!LoadPreferences()) return;


                if (this.PrintLog != null)
                {
                    string messagerError = "";
                    string printReason = null;
                    if (this.isPrintExceptionReason && this.isShowPrintExceptionReason)
                    {
                        if (this.getNumOrderPrint != null && this.getNumOrderPrint() > 1)
                        {
                            using (var form = new frmPrintReason())
                            {
                                var result = form.ShowDialog();
                                if (result == DialogResult.OK)
                                {
                                    printReason = form.ReturnValue;
                                }
                            }
                            if (String.IsNullOrWhiteSpace(printReason))
                                return;
                        }
                    }
                    if (this.PrintLog(ref messagerError, printReason))
                    {
                        this.ProcessPrintThread(flexCelPrintDocument1);
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(messagerError);
                        return;
                    }
                }
                else
                    this.ProcessPrintThread(flexCelPrintDocument1);

                #region Print
                #endregion
                if (this.eventLog != null)
                {
                    this.eventLog();
                }

                if (this.eventPrint != null)
                {
                    this.eventPrint();
                }

                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessPrintThread(FlexCel.Render.FlexCelPrintDocument printDocument)
        {
            try
            {
                DocumentPrint.ThreadPrint thred = new DocumentPrint.ThreadPrint(printDocument);
                thred.Print();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
