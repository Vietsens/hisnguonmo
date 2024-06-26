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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PrescriptionAbsentList.Run
{
    public partial class frmSetting : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        Inventec.Desktop.Common.Modules.Module _moduleData;
        MOS.SDO.WorkPlaceSDO WorkPlaceSDO;

        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        const string moduleLink = "HIS.Desktop.Plugins.PrescriptionAbsentList";
        ConfigADO currentAdo;
        #endregion

        #region Construct

        public frmSetting()
        {
            InitializeComponent();
        }

        public frmSetting(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                this._moduleData = moduleData;
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
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
        }
        #endregion

        private void frmSetting_Load(object sender, EventArgs e)
        {
            try
            {
                this.WorkPlaceSDO = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this._moduleData.RoomId);
                lblRoomName.Text = this.WorkPlaceSDO != null ? WorkPlaceSDO.RoomName : "";
                spRowNumber.EditValue = null;
                InitControlState();
                spRowNumber.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitControlState()
        {
            isNotLoadWhileChangeControlStateInFirst = true;
            try
            {
                this.controlStateWorker = new Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    this.currentAdo = new ConfigADO();
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == "ConfigADO")
                        {
                            if (!string.IsNullOrEmpty(item.VALUE))
                            {
                                currentAdo = JsonConvert.DeserializeObject<ConfigADO>(item.VALUE);
                            }
                        }
                    }
                    if (currentAdo != null)
                    {
                        spRowNumber.EditValue = (int)currentAdo.rowNumber;
                        spnCoChuSTT.EditValue = (int)currentAdo.CoChuSTT;
                        spnCoChuTenBN.EditValue = (int)currentAdo.CoChuTenBN;
                        spnCoChuTenQuay.EditValue = (int)currentAdo.CoChuTenQuay;
                        if (currentAdo.autoOpenWaitingScreen)
                        {
                            chkAutoOpenWaitingScreen.Checked = true;
                            btnOpenScreen_Click(null, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhileChangeControlStateInFirst = false;
        }

        private void btnOpenScreen_Click(object sender, EventArgs e)
        {
            try
            {
                if (spRowNumber.EditValue == null || spRowNumber.Value <= 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bắt buộc nhập giá trị 'Số dòng trên 1 trang'");
                    return;
                }
                if (isNotLoadWhileChangeControlStateInFirst == false)
                {
                    WaitingManager.Show();
                    ConfigADO ado = new ConfigADO();
                    ado.rowNumber = (decimal)spRowNumber.EditValue;
                    ado.CoChuSTT = spnCoChuSTT.EditValue != null ? (decimal?)spnCoChuSTT.EditValue : null;
                    ado.CoChuTenBN = spnCoChuTenBN.EditValue != null ? (decimal?)spnCoChuTenBN.EditValue : null;
                    ado.CoChuTenQuay = spnCoChuTenQuay.EditValue != null ? (decimal?)spnCoChuTenQuay.EditValue : null;
                    ado.autoOpenWaitingScreen = chkAutoOpenWaitingScreen.Checked;
                    this.currentAdo = ado;
                    string textJson = JsonConvert.SerializeObject(ado);

                    HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == "ConfigADO" && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                    if (csAddOrUpdate != null)
                    {
                        csAddOrUpdate.VALUE = textJson;
                    }
                    else
                    {
                        csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                        csAddOrUpdate.KEY = "ConfigADO";
                        csAddOrUpdate.VALUE = textJson;
                        csAddOrUpdate.MODULE_LINK = moduleLink;
                        if (this.currentControlStateRDO == null)
                            this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                        this.currentControlStateRDO.Add(csAddOrUpdate);
                    }
                    this.controlStateWorker.SetData(this.currentControlStateRDO);
                    WaitingManager.Hide();
                }

                //Mở màn hình 'Danh sách vắng mặt phát thuốc'
                frmPrescriptionAbsentList frm = new frmPrescriptionAbsentList(this._moduleData, this.currentAdo);
                ShowFormProcessor.ShowFormInExtendMonitor(frm);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spRowNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnOpenScreen_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
