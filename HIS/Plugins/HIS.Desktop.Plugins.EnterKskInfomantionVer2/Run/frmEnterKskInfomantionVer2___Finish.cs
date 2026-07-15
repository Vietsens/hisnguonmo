/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * "Tự động kết thúc" (chkAutoFinish) + "Kết thúc y lệnh khám" (btnFinishServiceReq) — xử lý tương tự
 * EnterKskInfomantion (V1): nút gọi api/HisServiceReq/Finish kết thúc y lệnh hiện tại; checkbox lưu trạng
 * thái local (ControlState), khi tích thì sau mỗi lần Lưu thành công sẽ tự động kết thúc y lệnh.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraEditors;
using HIS.Desktop.Library.CacheClient;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        private const string FINISH_MODULE_LINK = "HIS.Desktop.Plugins.EnterKskInfomantionVer2";
        private const string KEY_CHK_AUTO_FINISH = "chkAutoFinish";
        private bool isSuppressAutoFinishEvent = false;
        private ControlStateWorker finishControlStateWorker;
        private List<ControlStateRDO> finishControlStateRDO;

        /// <summary>Nạp trạng thái chkAutoFinish từ ControlState local + gắn enable ban đầu. Gọi 1 lần ở Load.</summary>
        private void InitFinishFeature()
        {
            try
            {
                isSuppressAutoFinishEvent = true;
                this.finishControlStateWorker = new ControlStateWorker();
                this.finishControlStateRDO = finishControlStateWorker.GetData(FINISH_MODULE_LINK) ?? new List<ControlStateRDO>();
                var item = finishControlStateRDO.FirstOrDefault(o => o.KEY == KEY_CHK_AUTO_FINISH && o.MODULE_LINK == FINISH_MODULE_LINK);
                if (item != null) this.chkAutoFinish.Checked = item.VALUE == "1";
                isSuppressAutoFinishEvent = false;

                UpdateFinishButtonEnable();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Lưu trạng thái chkAutoFinish vào ControlState local.</summary>
        private void chkAutoFinish_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isSuppressAutoFinishEvent) return;
                if (finishControlStateRDO == null) finishControlStateRDO = new List<ControlStateRDO>();
                var item = finishControlStateRDO.FirstOrDefault(o => o.KEY == KEY_CHK_AUTO_FINISH && o.MODULE_LINK == FINISH_MODULE_LINK);
                if (item != null) item.VALUE = chkAutoFinish.Checked ? "1" : "";
                else finishControlStateRDO.Add(new ControlStateRDO() { KEY = KEY_CHK_AUTO_FINISH, VALUE = chkAutoFinish.Checked ? "1" : "", MODULE_LINK = FINISH_MODULE_LINK });
                if (finishControlStateWorker != null) finishControlStateWorker.SetData(finishControlStateRDO);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Nút "Kết thúc y lệnh khám" -> kết thúc y lệnh hiện tại (có thông báo).</summary>
        private void btnFinishServiceReq_Click(object sender, EventArgs e)
        {
            FinishCurrentServiceReq(true);
        }

        /// <summary>Bật/tắt nút Kết thúc theo trạng thái y lệnh (đã kết thúc thì tắt).</summary>
        private void UpdateFinishButtonEnable()
        {
            try
            {
                if (btnFinishServiceReq == null) return;
                bool canFinish = currentServiceReq != null && currentServiceReq.ID > 0
                    && currentServiceReq.SERVICE_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT;
                btnFinishServiceReq.Enabled = canFinish;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Kết thúc y lệnh khám hiện tại: POST api/HisServiceReq/Finish (giống V1 FinishProcess).
        /// showMessage=true khi bấm nút; false khi auto (sau Lưu — đã có thông báo Lưu rồi).
        /// </summary>
        private void FinishCurrentServiceReq(bool showMessage)
        {
            try
            {
                if (currentServiceReq == null || currentServiceReq.ID <= 0) return;
                if (currentServiceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                {
                    if (showMessage)
                        XtraMessageBox.Show("Y lệnh khám đã được kết thúc.", "Thông báo",
                            System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                    return;
                }

                WaitingManager.Show();
                var param = new CommonParam();
                var result = new BackendAdapter(param).Post<HIS_SERVICE_REQ>(
                    "api/HisServiceReq/Finish", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, currentServiceReq.ID, param);
                WaitingManager.Hide();

                if (result != null)
                {
                    // currentServiceReq là V_HIS_SERVICE_REQ (view) còn API trả HIS_SERVICE_REQ -> chỉ cập nhật trạng thái.
                    currentServiceReq.SERVICE_REQ_STT_ID = result.SERVICE_REQ_STT_ID;
                    UpdateFinishButtonEnable();
                    try { LoadYlenhList(); } catch { }          // refresh danh sách (loại y lệnh đã kết thúc)
                    if (showMessage)
                        XtraMessageBox.Show("Đã kết thúc y lệnh khám.", "Thông báo",
                            System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                }
                else if (showMessage)
                {
                    MessageManager.Show(this, param, false);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }
    }
}
