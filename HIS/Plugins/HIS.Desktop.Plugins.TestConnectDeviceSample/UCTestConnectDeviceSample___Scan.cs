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
using ACS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.TestConnectDeviceSample.ADO;
using HIS.Desktop.Plugins.TestConnectDeviceSample.Base;
using HIS.Desktop.Plugins.TestConnectDeviceSample.Resources;
using HIS.Desktop.Plugins.TestConnectDeviceSample.Worker;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.Message;
using LIS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TestConnectDeviceSample
{
    /// <summary>
    /// Scan rules (design 60557, section I.4). Every port feeds one queue; the UI thread handles one scan at a time
    /// and waits while any modal window is open, so a popup never opens over another one.
    /// </summary>
    public partial class UCTestConnectDeviceSample : UserControlBase
    {
        #region Declare
        private readonly Queue<PendingScanADO> pendingScans = new Queue<PendingScanADO>();
        private readonly object pendingLock = new object();
        private readonly RepeatScanFilter repeatFilter = new RepeatScanFilter();
        private bool isProcessing = false;
        #endregion

        [DllImport("user32.dll")]
        private static extern bool IsWindowEnabled(IntPtr hWnd);

        /// <summary>
        /// Called on the serial port thread: only queues the scan and posts to the UI thread (never Invoke, which
        /// would block the port thread while a popup is open).
        /// </summary>
        internal void OnLineReceived(string portName, string code, DateTime receivedAt)
        {
            try
            {
                PendingScanADO scan = new PendingScanADO(portName, code, receivedAt);
                lock (this.pendingLock)
                {
                    if (this.repeatFilter.IsRepeat(portName, code, receivedAt))
                        return;
                    this.pendingScans.Enqueue(scan);
                }
                // If posting fails (control being closed) the port check timer still picks the scan up
                if (this.IsHandleCreated && !this.IsDisposed)
                    this.BeginInvoke(new Action<PendingScanADO>(this.OnScanQueued), scan);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnScanQueued(PendingScanADO scan)
        {
            try
            {
                if (!this.isProcessing && !IsModalWindowOpen())
                {
                    ProcessPendingScans();
                    return;
                }

                int waiting;
                bool stillQueued;
                lock (this.pendingLock)
                {
                    waiting = this.pendingScans.Count;
                    stillQueued = this.pendingScans.Contains(scan);
                }
                if (stillQueued)
                    Notify(scan.PortName, scan.Code, scan.ReceivedAt,
                        String.Format(ResourceMessage.DaNhanChoXuLy, scan.PortName, scan.Code, waiting), false);
                UpdatePendingLabel();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool HasPendingScans()
        {
            try
            {
                lock (this.pendingLock)
                {
                    return this.pendingScans.Count > 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        /// <summary>
        /// Any dialog shown with ShowDialog, or a native message box (it disables the main window).
        /// </summary>
        private bool IsModalWindowOpen()
        {
            try
            {
                foreach (Form form in Application.OpenForms)
                {
                    if (form.Visible && form.Modal)
                        return true;
                }
                Form main = this.ParentForm;
                return main != null && main.IsHandleCreated && !IsWindowEnabled(main.Handle);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        private void ProcessPendingScans()
        {
            if (this.isProcessing)
                return;
            this.isProcessing = true;
            try
            {
                while (!IsModalWindowOpen())
                {
                    PendingScanADO scan;
                    lock (this.pendingLock)
                    {
                        if (this.pendingScans.Count == 0)
                            break;
                        scan = this.pendingScans.Dequeue();
                    }
                    UpdatePendingLabel();
                    ProcessScan(scan);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                this.isProcessing = false;
                UpdatePendingLabel();
                RefreshTiles();
            }
        }

        private void ProcessScan(PendingScanADO scan)
        {
            try
            {
                // Port closed while the scan was waiting: say so, the user may already have seen "đã nhận"
                if (!this.readers.ContainsKey(scan.PortName) || this.lostPorts.Contains(scan.PortName))
                {
                    Notify(scan.PortName, scan.Code, scan.ReceivedAt,
                        String.Format(ResourceMessage.BoLuotQuetDoCongNgat, scan.PortName, scan.Code), false);
                    return;
                }

                this.lastScanByPort[scan.PortName] = scan;
                if (ScanTextUtil.IsAccountCard(scan.Code))
                    ProcessAccountCard(scan);
                else
                    ProcessSampleScan(scan);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Rules 2-5: bind the port to the card owner; block a card of someone else on a bound port.
        /// </summary>
        private void ProcessAccountCard(PendingScanADO scan)
        {
            try
            {
                string loginname = ScanTextUtil.GetLoginname(scan.Code);
                ACS_USER user;
                string errorMessage;
                if (!AccountLookupWorker.TryGetActiveUser(loginname, out user, out errorMessage))
                {
                    Notify(scan.PortName, scan.Code, scan.ReceivedAt, errorMessage, false);
                    return;
                }

                string userText = FormatUser(user.USERNAME, user.LOGINNAME);
                TestDeviceSampleADO current = SessionStore.GetByPort(scan.PortName);
                if (current != null)
                {
                    if (String.Equals(current.Loginname, user.LOGINNAME, StringComparison.OrdinalIgnoreCase))
                    {
                        Notify(scan.PortName, scan.Code, scan.ReceivedAt,
                            String.Format(ResourceMessage.CongDangDangNhapTaiKhoan, scan.PortName, userText), true);
                        return;
                    }
                    string currentText = FormatUser(current.Username, current.Loginname);
                    Notify(scan.PortName, scan.Code, scan.ReceivedAt, String.Format(ResourceMessage.CongDangDangNhapNguoiKhac,
                        scan.PortName, currentText, FormatTime(current.SessionTimeStart)), false);
                    EventLogWorker.Log(String.Format(ResourceMessage.NhatKyChanThe, scan.PortName, currentText, userText));
                    return;
                }

                // One account, one port: the old port must not keep recording for someone who moved away
                foreach (TestDeviceSampleADO old in SessionStore.GetByLoginname(user.LOGINNAME))
                {
                    SessionStore.Remove(old);
                    EventLogWorker.Log(String.Format(ResourceMessage.NhatKyDangXuat, old.TeminalCode,
                        FormatUser(old.Username, old.Loginname), String.Format(ResourceMessage.LyDoChuyenCong, scan.PortName)));
                    Notify(old.TeminalCode, "", scan.ReceivedAt,
                        String.Format(ResourceMessage.ChuyenTaiKhoanSangCongKhac, old.TeminalCode, userText, scan.PortName), false);
                }

                SessionStore.Bind(scan.PortName, user, scan.ReceivedAt);
                AccountLookupWorker.MergeIntoCache(user);
                EventLogWorker.Log(String.Format(ResourceMessage.NhatKyDangNhap, scan.PortName, userText,
                    scan.ReceivedAt.ToString("dd/MM/yyyy HH:mm:ss")));
                Notify(scan.PortName, scan.Code, scan.ReceivedAt, String.Format(ResourceMessage.DaDangNhap, scan.PortName, userText), true);
                RefreshTiles();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Rules 6-11: find the waiting sample(s) of the room and open the sample info popup for each one.
        /// </summary>
        private void ProcessSampleScan(PendingScanADO scan)
        {
            try
            {
                if (SessionStore.GetByPort(scan.PortName) == null)
                {
                    Notify(scan.PortName, scan.Code, scan.ReceivedAt, String.Format(ResourceMessage.CongChuaCoNguoiLayMau, scan.PortName), false);
                    return;
                }
                if (this.room == null)
                {
                    Notify(scan.PortName, scan.Code, scan.ReceivedAt, ResourceMessage.KhongXacDinhPhongLamViec, false);
                    return;
                }

                SampleLookupResult found;
                string problem;
                WaitingManager.Show();
                try
                {
                    found = SampleLookupWorker.Find(scan.Code, this.room, scan.ReceivedAt);
                    problem = GetSampleProblem(scan, found);
                }
                finally
                {
                    WaitingManager.Hide();
                }

                if (problem != null)
                {
                    Notify(scan.PortName, scan.Code, scan.ReceivedAt, problem, false);
                    return;
                }

                if (found.FutureSamples.Any())
                    Notify(scan.PortName, scan.Code, scan.ReceivedAt, FormatScheduled(found.FutureSamples), false);

                bool isFirst = true;
                foreach (V_LIS_SAMPLE due in found.DueSamples)
                {
                    // The account may have been signed out while the previous popup was open
                    TestDeviceSampleADO session = SessionStore.GetByPort(scan.PortName);
                    if (session == null)
                    {
                        Notify(scan.PortName, scan.Code, scan.ReceivedAt, String.Format(ResourceMessage.CongChuaCoNguoiLayMau, scan.PortName), false);
                        return;
                    }
                    // shortcut: a sample taken elsewhere while its own popup is open is still not caught (same as the manual button)
                    V_LIS_SAMPLE sample = isFirst ? due : ReloadIfStillWaiting(scan, due);
                    isFirst = false;
                    if (sample == null)
                        continue;
                    if (!OpenSampleInfo(scan, session, sample))
                        return;
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Message for every case where no popup opens (rules 7-10), or null when the due samples can be opened.
        /// </summary>
        private string GetSampleProblem(PendingScanADO scan, SampleLookupResult found)
        {
            try
            {
                if (found.IsApiError)
                    return String.Format(ResourceMessage.LoiTraCuuMau, scan.Code);

                if (!found.DueSamples.Any() && !found.FutureSamples.Any())
                {
                    V_LIS_SAMPLE recent = found.RecentSample;
                    if (recent != null && recent.SAMPLE_TIME.HasValue)
                        return String.Format(ResourceMessage.MauDaLay, recent.BARCODE ?? scan.Code,
                            Inventec.Common.DateTime.Convert.TimeNumberToTimeString(recent.SAMPLE_TIME.Value),
                            FormatUser(recent.SAMPLE_USERNAME, recent.SAMPLE_LOGINNAME));
                    if (ScanTextUtil.IsServiceReqCode(scan.Code))
                        return String.Format(ResourceMessage.KhongCoMauChoLayTheoMaYLenh, scan.Code, found.WindowFrom.ToString("dd/MM/yyyy"));
                    return String.Format(ResourceMessage.KhongCoMauChoLay, scan.Code, found.WindowFrom.ToString("dd/MM/yyyy"));
                }

                if (found.PendingTreatmentCodes.Count > 1)
                {
                    string patients = String.Join("; ", found.DueSamples.Concat(found.FutureSamples)
                        .Select(o => FormatPatient(o)).Distinct());
                    return String.Format(ResourceMessage.TrungMauNhieuHoSo, scan.Code, found.PendingTreatmentCodes.Count, patients);
                }

                if (!found.DueSamples.Any())
                    return FormatScheduled(found.FutureSamples);

                bool? paused = SampleLookupWorker.IsTreatmentPaused(found.PendingTreatmentCodes.First());
                if (!paused.HasValue)
                    return String.Format(ResourceMessage.LoiTraCuuMau, scan.Code);
                if (paused.Value)
                    return ResourceMessage.HoSoDaKetThucDieuTri;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return String.Format(ResourceMessage.LoiTraCuuMau, scan.Code);
            }
            return null;
        }

        /// <summary>
        /// Opens "Thông tin lấy mẫu" for one sample with the bound account and the scan time.
        /// Returns false when the popup cannot be opened at all (no point trying the next sample).
        /// </summary>
        private bool OpenSampleInfo(PendingScanADO scan, TestDeviceSampleADO session, V_LIS_SAMPLE sample)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws
                    .FirstOrDefault(o => o.ModuleLink == ModuleLinkString.SampleInfo);
                if (moduleData == null || !moduleData.IsPlugin || moduleData.ExtensionInfo == null || this.currentModuleBase == null)
                {
                    Notify(scan.PortName, scan.Code, scan.ReceivedAt, ResourceMessage.ChuaCoQuyenThongTinLayMau, false);
                    return false;
                }

                // A copy: the popup reads SAMPLE_LOGINNAME to preselect the sampler
                V_LIS_SAMPLE dataSend = new V_LIS_SAMPLE();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_LIS_SAMPLE>(dataSend, sample);
                dataSend.SAMPLE_LOGINNAME = session.Loginname;
                dataSend.SAMPLE_USERNAME = session.Username;

                object instance = PluginInstance.GetPluginInstance(
                    PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId),
                    new List<object> { dataSend, scan.ReceivedAt });
                Form popup = instance as Form;
                if (popup == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("SampleInfo did not return a form" + Inventec.Common.Logging.LogUtil.TraceData("sampleId", sample.ID));
                    Notify(scan.PortName, scan.Code, scan.ReceivedAt, ResourceMessage.ChuaCoQuyenThongTinLayMau, false);
                    return false;
                }

                popup.Text = String.Format(ResourceMessage.TieuDePopupLayMau, popup.Text, scan.PortName, FormatUser(session.Username, session.Loginname));
                DialogResult dialogResult = this.ParentForm != null ? popup.ShowDialog(this.ParentForm) : popup.ShowDialog();
                // dataSend is the popup's own sample object: after a successful save it holds what the server stored
                if (dialogResult == DialogResult.OK)
                    AfterSampleSaved(scan, session, dataSend);
                else
                    Notify(scan.PortName, scan.Code, scan.ReceivedAt,
                        String.Format(ResourceMessage.DongPopupChuaLuu, scan.PortName, sample.BARCODE ?? scan.Code), false);
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// Rule 13: refresh the sample collection list of the room and log when the saved sampler differs from the port.
        /// </summary>
        private void AfterSampleSaved(PendingScanADO scan, TestDeviceSampleADO session, V_LIS_SAMPLE sample)
        {
            try
            {
                SampleCollectionRoomRefresher.Refresh(this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId);

                V_LIS_SAMPLE saved = SampleLookupWorker.GetSampleById(sample.ID) ?? sample;
                string loginname = saved.SAMPLE_LOGINNAME ?? session.Loginname;
                string username = saved.SAMPLE_USERNAME ?? session.Username;
                Notify(scan.PortName, scan.Code, scan.ReceivedAt, String.Format(ResourceMessage.DaLayMau, scan.PortName,
                    saved.BARCODE ?? scan.Code, FormatPatient(saved), FormatUser(username, loginname)), true);

                if (!String.Equals(loginname, session.Loginname, StringComparison.OrdinalIgnoreCase))
                {
                    EventLogWorker.Log(String.Format(ResourceMessage.NhatKyDoiNguoiLayMau, saved.BARCODE ?? scan.Code,
                        saved.SERVICE_REQ_CODE, scan.PortName, FormatUser(session.Username, session.Loginname), FormatUser(username, loginname)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Second and later popups of one scan: the sample may have been taken on another workstation while the
        /// previous popup was open, or its treatment ended. Returns the fresh sample when it can still be taken,
        /// otherwise reports and returns null.
        /// </summary>
        private V_LIS_SAMPLE ReloadIfStillWaiting(PendingScanADO scan, V_LIS_SAMPLE sample)
        {
            try
            {
                V_LIS_SAMPLE current = SampleLookupWorker.GetSampleById(sample.ID);
                if (current == null)
                {
                    Notify(scan.PortName, scan.Code, scan.ReceivedAt, String.Format(ResourceMessage.LoiTraCuuMau, sample.BARCODE ?? scan.Code), false);
                    return null;
                }
                if (current.SAMPLE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM
                    && current.SAMPLE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                {
                    Notify(scan.PortName, scan.Code, scan.ReceivedAt, String.Format(ResourceMessage.MauDaLay, current.BARCODE ?? scan.Code,
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeString(current.SAMPLE_TIME ?? 0),
                        FormatUser(current.SAMPLE_USERNAME, current.SAMPLE_LOGINNAME)), false);
                    return null;
                }
                // Rule 9 again: the treatment may have been ended elsewhere while the previous popup was open
                bool? paused = SampleLookupWorker.IsTreatmentPaused(current.TREATMENT_CODE);
                if (paused != false)
                {
                    Notify(scan.PortName, scan.Code, scan.ReceivedAt, paused.HasValue
                        ? ResourceMessage.HoSoDaKetThucDieuTri
                        : String.Format(ResourceMessage.LoiTraCuuMau, current.BARCODE ?? scan.Code), false);
                    return null;
                }
                return current;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }
    }
}
