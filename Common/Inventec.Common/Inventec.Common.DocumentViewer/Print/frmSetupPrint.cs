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
using Inventec.Common.DocumentViewer.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.Common.DocumentViewer.Print
{
    public delegate void DelegatePrintFile(string PrinterName, PrintRange PrintRange = PrintRange.AllPages, int FromPage = 0, int ToPage = 0);

    public partial class frmSetupPrint : DevExpress.XtraEditors.XtraForm
    {
        #region Declare
        DelegatePrintFile delegatePrintFile;
        int totalPageCount = 0;

        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "Inventec.Common.DocumentViewer";

        enum PageRange
        {
            AllPages = 1,
            CurrentPage = 2,
            SomePages = 3,
        }
        #endregion

        #region Constructor
        public frmSetupPrint(DelegatePrintFile PrintFile, int TotalPageCount)
        {
            InitializeComponent();
            this.delegatePrintFile = PrintFile;
            this.totalPageCount = TotalPageCount;
        }

        #endregion

        #region Onload
        private void frmSetupPrint_Load(object sender, EventArgs e)
        {
            try
            {
                if (PrinterSettings.InstalledPrinters == null || PrinterSettings.InstalledPrinters.Count <= 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không có máy in nào được cài đặt", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
                //LoadPrinters
                LoadPrinters();

                // SetControlState
                InitControlState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {
            try
            {
                numericUpDownFromPage.Enabled = false;
                numericUpDownToPage.Enabled = false;
                numericUpDownFromPage.EditValue = null;
                numericUpDownToPage.EditValue = null;
                numericUpDownFromPage.Properties.MinValue = 1;
                numericUpDownToPage.Properties.MinValue = 1;
                numericUpDownFromPage.Properties.MaxValue = this.totalPageCount;
                numericUpDownToPage.Properties.MaxValue = this.totalPageCount;

                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    string pageRangeDefault = "";
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == ControlStateConstant.PrinterName)
                        {
                            cboPrinters.EditValue = item.VALUE;
                        }
                        else if (item.KEY == ControlStateConstant.PageRange)
                        {
                            pageRangeDefault = item.VALUE;
                        }
                    }
                    if (pageRangeDefault == ((int)PageRange.AllPages).ToString())
                        chkAllPages.Checked = true;
                    else if (pageRangeDefault == ((int)PageRange.CurrentPage).ToString())
                        chkCurrentPage.Checked = true;
                    else if (pageRangeDefault == ((int)PageRange.SomePages).ToString())
                        chkPages.Checked = true;
                    else
                        chkAllPages.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPrinters()
        {
            try
            {
                //Load all avaiable printers
                if (PrinterSettings.InstalledPrinters != null)
                {
                    cboPrinters.Properties.Items.AddRange(PrinterSettings.InstalledPrinters);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        #endregion

        private void chkPages_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                numericUpDownFromPage.Enabled = numericUpDownToPage.Enabled = chkPages.Checked;
                if (chkPages.Checked)
                {
                    if (numericUpDownFromPage.EditValue == null)
                        numericUpDownFromPage.EditValue = 1;
                    if (numericUpDownToPage.EditValue == null)
                        numericUpDownToPage.EditValue = this.totalPageCount;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                SaveControlState_cboPrinters();
                SaveControlState_PageRange();

                if (this.delegatePrintFile != null)
                {
                    string printerName = cboPrinters.Text;
                    PrintRange printRange = PrintRange.AllPages;
                    if (chkPages.Checked)
                        printRange = PrintRange.SomePages;
                    else if (chkCurrentPage.Checked)
                        printRange = PrintRange.CurrentPage;

                    var fromPage = numericUpDownFromPage.Value;
                    var toPage = numericUpDownToPage.Value;
                    this.delegatePrintFile(printerName, printRange, (int)fromPage, (int)toPage);

                }
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveControlState_cboPrinters()
        {
            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.PrinterName && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = cboPrinters.Text ?? "";
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.PrinterName;
                    csAddOrUpdate.VALUE = cboPrinters.Text ?? "";
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveControlState_PageRange()
        {
            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.PageRange && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    if (chkAllPages.Checked)
                        csAddOrUpdate.VALUE = ((int)PageRange.AllPages).ToString();
                    else if (chkCurrentPage.Checked)
                        csAddOrUpdate.VALUE = ((int)PageRange.CurrentPage).ToString();
                    else if (chkPages.Checked)
                        csAddOrUpdate.VALUE = ((int)PageRange.SomePages).ToString();
                    else
                        csAddOrUpdate.VALUE = "";
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.PageRange;
                    if (chkAllPages.Checked)
                        csAddOrUpdate.VALUE = ((int)PageRange.AllPages).ToString();
                    else if (chkCurrentPage.Checked)
                        csAddOrUpdate.VALUE = ((int)PageRange.CurrentPage).ToString();
                    else if (chkPages.Checked)
                        csAddOrUpdate.VALUE = ((int)PageRange.SomePages).ToString();
                    else
                        csAddOrUpdate.VALUE = "";

                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
