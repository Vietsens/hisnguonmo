/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseSyncList
 * Tự động đẩy ca bệnh lên cổng ECDS theo chu kỳ (Timer).
 * - Nhớ trạng thái checkbox + số phút qua ControlState (rule §14).
 * - Mỗi tick: reload trang hiện tại -> đẩy các ca CHƯA auto-đẩy trong phiên (HashSet chống trùng/spam).
 * - Chạy nền im lặng qua RunSyncForRows(..., silent:true) trong __Process.cs.
 */
using HIS.Desktop.Plugins.InfectiousDiseaseSyncList.Config;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.InfectiousDiseaseSyncList.MainForm
{
    public partial class UCInfectiousDiseaseSyncList
    {
        #region Init
        /// <summary>Khởi tạo Timer + khôi phục trạng thái tự động đẩy đã lưu.</summary>
        private void InitAutoPush()
        {
            try
            {
                autoPushTimer = new System.Windows.Forms.Timer();
                autoPushTimer.Tick += autoPushTimer_Tick;

                InitControlState();   // khôi phục checkbox + số phút

                if (chkAutoPush != null && chkAutoPush.Checked) StartAutoPushTimer();
                else StopAutoPushTimer();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }
        #endregion

        #region Timer control
        private void StartAutoPushTimer()
        {
            try
            {
                if (autoPushTimer == null) return;
                int minutes = GetIntervalMinutes();
                autoPushTimer.Interval = minutes * 60000;
                autoPushTimer.Stop();
                autoPushTimer.Start();
                if (lblAutoStatus != null)
                    lblAutoStatus.Text = "Tự động: bật (mỗi " + minutes + " phút)";
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void StopAutoPushTimer()
        {
            try
            {
                if (autoPushTimer != null) autoPushTimer.Stop();
                if (lblAutoStatus != null) lblAutoStatus.Text = "Tự động: tắt";
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private int GetIntervalMinutes()
        {
            int minutes = 5;
            try
            {
                if (spnAutoInterval != null && spnAutoInterval.EditValue != null)
                    minutes = Convert.ToInt32(spnAutoInterval.Value);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            if (minutes < 1) minutes = 1;
            return minutes;
        }
        #endregion

        #region Events
        private void chkAutoPush_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!isNotLoadWhileChangeControlStateInFirst)
                    SaveControlState(chkAutoPush.Name, chkAutoPush.Checked ? "1" : "");

                if (chkAutoPush.Checked) StartAutoPushTimer();
                else StopAutoPushTimer();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void spnAutoInterval_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!isNotLoadWhileChangeControlStateInFirst)
                    SaveControlState(spnAutoInterval.Name, GetIntervalMinutes().ToString());

                // Áp dụng chu kỳ mới nếu đang bật
                if (chkAutoPush != null && chkAutoPush.Checked) StartAutoPushTimer();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Chu kỳ tự động: reload trang hiện tại rồi đẩy các ca chưa auto-đẩy.</summary>
        private void autoPushTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (chkAutoPush == null || !chkAutoPush.Checked) return;
                if (isSyncing) return;   // đang có phiên đẩy (tay hoặc auto) -> bỏ tick

                if (!EcdsConfigCFG.IsValid())
                {
                    if (lblAutoStatus != null) lblAutoStatus.Text = "Tự động: chưa cấu hình ECDS";
                    return;
                }

                // Reload im lặng trang hiện tại để bắt ca mới phát sinh.
                LoadListPaging(new CommonParam(0, currentPageSize));

                var rows = (listData ?? new List<ADO.EcdsSyncGridRowADO>())
                    .Where(r => r != null && r.Source != null && !autoAttemptedIds.Contains(r.TREATMENT_ID))
                    .Select(r => r.Source)
                    .ToList();

                if (rows.Count == 0)
                {
                    if (lblAutoStatus != null)
                        lblAutoStatus.Text = "Tự động " + DateTime.Now.ToString("HH:mm") + ": không có ca mới";
                    return;
                }

                RunSyncForRows(rows, true);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }
        #endregion

        #region ControlState (rule §14)
        private void InitControlState()
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = true;

                controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                currentControlStateRDO = controlStateWorker.GetData(moduleLink);

                if (currentControlStateRDO != null && currentControlStateRDO.Count > 0)
                {
                    foreach (var item in currentControlStateRDO)
                    {
                        if (item.KEY == chkAutoPush.Name)
                        {
                            chkAutoPush.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == spnAutoInterval.Name)
                        {
                            int m;
                            if (int.TryParse(item.VALUE, out m) && m >= 1)
                                spnAutoInterval.EditValue = m;
                        }
                    }
                }

                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                isNotLoadWhileChangeControlStateInFirst = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveControlState(string key, string value)
        {
            try
            {
                if (controlStateWorker == null)
                    controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                if (currentControlStateRDO == null)
                    currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();

                var item = currentControlStateRDO.FirstOrDefault(
                    o => o.KEY == key && o.MODULE_LINK == moduleLink);
                if (item != null)
                {
                    item.VALUE = value;
                }
                else
                {
                    currentControlStateRDO.Add(new HIS.Desktop.Library.CacheClient.ControlStateRDO
                    {
                        KEY = key,
                        MODULE_LINK = moduleLink,
                        VALUE = value
                    });
                }

                controlStateWorker.SetData(currentControlStateRDO);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }
        #endregion
    }
}
