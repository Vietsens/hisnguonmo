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
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.WaitingScreenPttt.ADO;
using MOS.EFMODEL.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.WaitingScreenPttt
{
    public partial class frmSetupWaitingScreenPttt : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module module;
        long departmentId;
        bool isLoading;

        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;

        public frmSetupWaitingScreenPttt(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            this.module = module;
        }

        private void frmSetupWaitingScreenPttt_Load(object sender, EventArgs e)
        {
            try
            {
                isLoading = true;
                SetIcon();

                V_HIS_ROOM currentRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == module.RoomId);
                if (currentRoom != null)
                {
                    departmentId = currentRoom.DEPARTMENT_ID;
                    txtDepartment.Text = currentRoom.DEPARTMENT_NAME;
                }

                List<V_HIS_BED_ROOM> bedRooms = BackendDataWorker.Get<V_HIS_BED_ROOM>()
                    .Where(o => o.DEPARTMENT_ID == departmentId && o.IS_ACTIVE == 1)
                    .OrderBy(o => o.BED_ROOM_NAME)
                    .ToList();
                gridControlBedRoom.DataSource = bedRooms;

                RestoreSetting();

                tgOpenScreen.IsOn = GetOpenedWaitingScreen() != null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                isLoading = false;
            }
        }

        private void RestoreSetting()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(ModuleLink);
                if (this.currentControlStateRDO == null)
                    return;

                var item = this.currentControlStateRDO.FirstOrDefault(o => o.KEY == "SettingADO");
                if (item == null || string.IsNullOrEmpty(item.VALUE))
                    return;

                SettingADO setting = JsonConvert.DeserializeObject<SettingADO>(item.VALUE);
                if (setting == null)
                    return;

                if (setting.RELOAD_SECONDS >= 60)
                    spinReloadSeconds.EditValue = setting.RELOAD_SECONDS;

                if (setting.BED_ROOM_IDs != null && setting.BED_ROOM_IDs.Count > 0)
                {
                    gridViewBedRoom.ClearSelection();
                    for (int i = 0; i < gridViewBedRoom.DataRowCount; i++)
                    {
                        V_HIS_BED_ROOM row = gridViewBedRoom.GetRow(i) as V_HIS_BED_ROOM;
                        if (row != null && setting.BED_ROOM_IDs.Contains(row.ID))
                            gridViewBedRoom.SelectRow(i);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveSetting(SettingADO setting)
        {
            try
            {
                string textJson = JsonConvert.SerializeObject(setting);
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateValue = this.currentControlStateRDO != null ? this.currentControlStateRDO.FirstOrDefault(o => o.KEY == "SettingADO" && o.MODULE_LINK == ModuleLink) : null;
                if (csAddOrUpdateValue != null)
                {
                    csAddOrUpdateValue.VALUE = textJson;
                }
                else
                {
                    csAddOrUpdateValue = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdateValue.KEY = "SettingADO";
                    csAddOrUpdateValue.VALUE = textJson;
                    csAddOrUpdateValue.MODULE_LINK = ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdateValue);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<long> GetSelectedBedRoomIds()
        {
            List<long> result = new List<long>();
            foreach (int rowHandle in gridViewBedRoom.GetSelectedRows())
            {
                V_HIS_BED_ROOM row = gridViewBedRoom.GetRow(rowHandle) as V_HIS_BED_ROOM;
                if (row != null)
                    result.Add(row.ID);
            }
            return result;
        }

        private frmWaitingScreenPttt GetOpenedWaitingScreen()
        {
            return Application.OpenForms.OfType<frmWaitingScreenPttt>().FirstOrDefault();
        }

        private void tgOpenScreen_Toggled(object sender, EventArgs e)
        {
            try
            {
                if (isLoading)
                    return;

                frmWaitingScreenPttt opened = GetOpenedWaitingScreen();
                if (!tgOpenScreen.IsOn)
                {
                    if (opened != null)
                        opened.Close();
                    return;
                }

                if (opened != null)
                    return;

                List<long> bedRoomIds = GetSelectedBedRoomIds();
                if (bedRoomIds.Count == 0)
                {
                    XtraMessageBox.Show("Vui lòng chọn ít nhất một buồng.", "Thông báo");
                    isLoading = true;
                    tgOpenScreen.IsOn = false;
                    isLoading = false;
                    return;
                }

                int reloadSeconds = Convert.ToInt32(spinReloadSeconds.Value);
                if (reloadSeconds < 60)
                {
                    XtraMessageBox.Show("Thời gian tải lại tối thiểu là 60 giây.", "Thông báo");
                    spinReloadSeconds.EditValue = 60;
                    isLoading = true;
                    tgOpenScreen.IsOn = false;
                    isLoading = false;
                    return;
                }

                SettingADO setting = new SettingADO();
                setting.BED_ROOM_IDs = bedRoomIds;
                setting.RELOAD_SECONDS = reloadSeconds;
                SaveSetting(setting);

                frmWaitingScreenPttt waitingScreen = new frmWaitingScreenPttt(module, departmentId, bedRoomIds, reloadSeconds);
                waitingScreen.FormClosed += (s, args) =>
                {
                    if (!this.IsDisposed && tgOpenScreen.IsOn)
                    {
                        isLoading = true;
                        tgOpenScreen.IsOn = false;
                        isLoading = false;
                    }
                };
                ShowOnExtendMonitor(waitingScreen);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowOnExtendMonitor(frmWaitingScreenPttt waitingScreen)
        {
            Screen secondScreen = Screen.AllScreens.FirstOrDefault(o => !o.Primary);
            if (secondScreen == null)
            {
                waitingScreen.WindowState = FormWindowState.Maximized;
                waitingScreen.Show();
                return;
            }

            waitingScreen.FormBorderStyle = FormBorderStyle.None;
            waitingScreen.StartPosition = FormStartPosition.Manual;
            waitingScreen.Location = secondScreen.Bounds.Location;
            waitingScreen.WindowState = FormWindowState.Maximized;
            waitingScreen.Show();
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
