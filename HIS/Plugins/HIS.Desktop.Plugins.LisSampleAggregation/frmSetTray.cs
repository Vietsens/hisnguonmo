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
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.LisSampleAggregation
{
    public delegate bool DelegateSaveData(long? trayNumber, DateTime trayTime, bool isPrint);
    public partial class frmSetTray : Form
    {
        long? trayNumber;
        DateTime trayTime;

        DelegateSaveData actSaveData;
        Action<MPS.ProcessorBase.PrintConfig.PreviewType?> actRunPrinterWithType;

        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        const string moduleLink = "HIS.Desktop.Plugins.LisSampleAggregation";

        public frmSetTray(long? trayNumber, DateTime trayTime, DelegateSaveData actSaveData, Action<MPS.ProcessorBase.PrintConfig.PreviewType?> actRunPrinterWithType)
        {
            InitializeComponent();
            this.trayNumber = trayNumber;
            this.trayTime = trayTime;
            this.actSaveData = actSaveData;
            this.actRunPrinterWithType = actRunPrinterWithType;
        }

        private void frmSetTray_Load(object sender, EventArgs e)
        {
            try
            {
                InitControlState();
                spinTrayFrom.EditValue = this.trayNumber;
                dtTimeFrom.EditValue = this.trayTime;
                btnSave.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void InitControlState()
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = true;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == ControlStateConstan.chkPrint)
                        {
                            chkIsPrint.Checked = item.VALUE == "1";
                        }
                    }
                }
                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.actSaveData != null)
                {
                    bool success = this.actSaveData((long?)spinTrayFrom.Value, dtTimeFrom.DateTime, chkIsPrint.Checked);
                    if (chkIsPrint.Checked && success)
                    {
                        this.Close();
                    }
                    else if (success)
                        btnPrint.Focus();
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
                if (this.actRunPrinterWithType != null)
                {
                    this.actRunPrinterWithType(null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsPrint_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.chkPrint && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkIsPrint.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstan.chkPrint;
                    csAddOrUpdate.VALUE = (chkIsPrint.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }

                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1Ctrl_S_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void barButtonItem2_Ctrl_P_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnPrint_Click(null, null);
        }

        private void spinTrayFrom_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
