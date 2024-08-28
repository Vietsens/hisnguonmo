using FlexCel.Core;
using FlexCel.Render;
using FlexCel.XlsAdapter;
using Inventec.Common.FlexCelPrint.DocumentPrint;
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
    public partial class frmSetupPrintPreviewMerge : DevExpress.XtraEditors.XtraForm
    {
        public void Print()
        {
            try
            {
                if (this.partialFiles_Excel != null && this.partialFiles_Excel.Count > 0)
                {
                    if (!LoadPreferences()) return;

                    Inventec.Common.Logging.LogSystem.Debug("ThreadPrintPartialForExcel.Begin!");
                    ThreadPrintPartial threadPrintPartial = new ThreadPrintPartial(this.partialFiles_Excel, this.flexCelPrintDocument1);
                    threadPrintPartial.Print();
                    Inventec.Common.Logging.LogSystem.Debug("ThreadPrintPartialForExcel.End!");
                }
                else if (!this.isPdfOnly)
                {

                    if (this.emrInputADO != null)
                        this.emrInputADO.PaperSizeDefault = currentPaperSize;

                    if (isPreview == true) return;

                    if (!LoadPreferences()) return;


                    if (this.PrintLog != null)
                    {
                        string messagerError = "";
                        if (this.PrintLog(ref messagerError, null))
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
                }

                if (this.partialFiles_Pdf != null && this.partialFiles_Pdf.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("ThreadPrintPartialForPDF.Begin!");
                    ThreadPrintPartialForPDF threadPrintPartialForPDF = new ThreadPrintPartialForPDF(this.partialFiles_Pdf);
                    threadPrintPartialForPDF.Print();
                    Inventec.Common.Logging.LogSystem.Debug("ThreadPrintPartialForPDF.End!");
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
