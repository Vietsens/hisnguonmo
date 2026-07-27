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
using System.Linq;

namespace HIS.Desktop.Plugins.CoordinationServiceReqCLS
{
    public partial class UCCoordinationServiceReqCLS
    {
        #region Auto reload — copy nghiệp vụ từ HIS.Desktop.Plugins.PrepareAndExport
        /// <summary>Khóa định danh timer tự động tải lại (đăng ký qua MemoryProcessor của UserControlBase).</summary>
        const string timerLoadCPA = "timerCoordinationServiceReqCLS";

        /// <summary>Worker đọc/ghi trạng thái control (ControlState).</summary>
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;

        /// <summary>Danh sách trạng thái control hiện tại.</summary>
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;

        /// <summary>
        /// Checkbox "Tự động tải lại sau" — bật/tắt Timer làm mới danh sách.
        /// Copy nguyên logic chkAutoLoadTab_CheckedChanged của PrepareAndExport.
        /// </summary>
        private void chkAutoReload_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                spnAutoReloadSeconds.Enabled = chkAutoReload.Checked;

                SaveState();
                RunTimerLoadCPA();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Đăng ký/khởi động lại Timer tự động tải theo số giây cấu hình.</summary>
        private void RunTimerLoadCPA()
        {
            try
            {
                if (chkAutoReload.Checked && spnAutoReloadSeconds.EditValue != null && spnAutoReloadSeconds.Value > 0)
                {
                    StopTimer(this.currentModule.ModuleLink, timerLoadCPA);
                    var timerLoadCPA_Interval = (int)(spnAutoReloadSeconds.Value * 1000);
                    DisposeTimer(this.currentModule.ModuleLink, timerLoadCPA);
                    RegisterTimer(this.currentModule.ModuleLink, timerLoadCPA, timerLoadCPA_Interval, timerLoadCPA_Tick);
                    StartTimer(this.currentModule.ModuleLink, timerLoadCPA);
                }
                else
                {
                    // Bỏ tích (hoặc số giây không hợp lệ) → DỪNG HẲN timer: Stop + Dispose
                    StopTimer(this.currentModule.ModuleLink, timerLoadCPA);
                    DisposeTimer(this.currentModule.ModuleLink, timerLoadCPA);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Chu kỳ Timer — tự động tải lại danh sách CLS.</summary>
        private void timerLoadCPA_Tick()
        {
            try
            {
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Lưu trạng thái checkbox + số giây vào ControlState (nhớ giữa các phiên).</summary>
        private void SaveState()
        {
            try
            {
                if (controlStateWorker == null)
                    controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();

                UpsertControlState(chkAutoReload.Name, chkAutoReload.Checked ? "1" : "0");

                if (chkAutoReload.Checked && spnAutoReloadSeconds.EditValue != null)
                    UpsertControlState(spnAutoReloadSeconds.Name, spnAutoReloadSeconds.Value.ToString());

                controlStateWorker.SetData(currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Thêm mới hoặc cập nhật 1 bản ghi ControlState theo KEY.</summary>
        private void UpsertControlState(string key, string value)
        {
            HIS.Desktop.Library.CacheClient.ControlStateRDO item =
                (currentControlStateRDO != null && currentControlStateRDO.Count > 0)
                    ? currentControlStateRDO.FirstOrDefault(o => o.KEY == key && o.MODULE_LINK == moduleLink)
                    : null;

            if (item != null)
            {
                item.VALUE = value;
            }
            else
            {
                if (currentControlStateRDO == null)
                    currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();

                currentControlStateRDO.Add(new HIS.Desktop.Library.CacheClient.ControlStateRDO
                {
                    KEY = key,
                    VALUE = value,
                    MODULE_LINK = moduleLink
                });
            }
        }

        /// <summary>Đọc trạng thái đã lưu (checkbox + số giây) khi mở chức năng.</summary>
        private void InitControlState()
        {
            try
            {
                controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (currentControlStateRDO != null && currentControlStateRDO.Count > 0)
                {
                    foreach (var item in currentControlStateRDO)
                    {
                        if (item.KEY == chkAutoReload.Name)
                        {
                            chkAutoReload.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == spnAutoReloadSeconds.Name)
                        {
                            decimal sec;
                            if (Decimal.TryParse(item.VALUE, out sec) && sec > 0)
                                spnAutoReloadSeconds.Value = sec;
                        }
                    }
                }
                spnAutoReloadSeconds.Enabled = chkAutoReload.Checked;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Dọn dẹp khi đóng chức năng: dừng + hủy Timer, giải phóng dữ liệu.</summary>
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                if (this.currentModule != null)
                {
                    StopTimer(this.currentModule.ModuleLink, timerLoadCPA);
                    DisposeTimer(this.currentModule.ModuleLink, timerLoadCPA);
                }
                listAllServiceReq = null;
                listPatient = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
    }
}
