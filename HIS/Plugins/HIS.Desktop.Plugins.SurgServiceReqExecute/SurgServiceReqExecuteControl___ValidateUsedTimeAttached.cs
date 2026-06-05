/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * Kiem tra USED_TIME cua thuoc/vat tu dinh kem (con cua dich vu) > thoi gian Ket thuc dich vu khi Luu/Ket thuc.
 * Config: HIS.Desktop.Plugins.SurgServiceReqExecute.WarningUsedTimeAttachedGreaterProcessingTimeOption
 *   = 1 -> chan khong xu ly tiep
 *   = 2 -> canh bao Co/Khong; Co -> tiep tuc, Khong -> chan
 *   khac -> khong kiem tra
 */
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute
{
    public partial class SurgServiceReqExecuteControl : UserControlBase
    {
        /// <summary>
        /// Kiem tra thoi gian thuc hien (USED_TIME) cua thuoc/vat tu dinh kem co lon hon thoi gian Ket thuc dich vu khong.
        /// Tra ve true neu duoc phep xu ly tiep, false neu bi chan.
        /// </summary>
        private bool ValidateUsedTimeAttachedGreaterFinishTime()
        {
            try
            {
                string option = Config.HisConfigKeys.WarningUsedTimeAttachedGreaterProcessingTimeOption;
                if (option != "1" && option != "2")
                {
                    return true; // Khong kiem tra nghiep vu nay
                }

                if (this.serviceReq == null)
                    return true;

                // Thoi gian Ket thuc nhap tren man hinh
                long finishTime = dtFinish.EditValue == null
                    ? 0
                    : (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtFinish.DateTime) ?? 0);
                if (finishTime <= 0)
                    return true; // Chua nhap thoi gian Ket thuc -> khong kiem tra

                // PARENT_IDs = ID dich vu dang xu ly (HIS_SERE_SERV.ID).
                // Neu Xu ly gop -> lay tat ca ID dich vu cua y lenh dang xu ly.
                List<long> parentIds = new List<long>();
                if (chkSaveGroup.Checked && sereServbyServiceReqs != null && sereServbyServiceReqs.Count > 0)
                {
                    parentIds = sereServbyServiceReqs.Select(o => o.ID).Distinct().ToList();
                }
                else if (this.sereServ != null)
                {
                    parentIds.Add(this.sereServ.ID);
                }
                if (parentIds.Count == 0)
                    return true;

                Inventec.Desktop.Common.Message.WaitingManager.Show();
                CommonParam param = new CommonParam();
                HisSereServViewFilter filter = new HisSereServViewFilter();
                filter.PARENT_IDs = parentIds;
                filter.TREATMENT_ID = this.serviceReq.TREATMENT_ID;
                var data = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_2>>(
                    RequestUriStore.HIS_SERE_SERV_GETVIEW_2,
                    ApiConsumers.MosConsumer,
                    filter,
                    param);
                Inventec.Desktop.Common.Message.WaitingManager.Hide();

                if (data == null || data.Count == 0)
                    return true;

                // Loc an toan theo PARENT_ID + USED_TIME > thoi gian Ket thuc
                var parentIdSet = new HashSet<long>(parentIds);
                var offending = data
                    .Where(o => parentIdSet.Contains(o.PARENT_ID ?? 0) && (o.USED_TIME ?? 0) > finishTime)
                    .OrderByDescending(o => o.USED_TIME ?? 0)
                    .ToList();

                if (offending.Count == 0)
                    return true;

                var first = offending[0];
                string baseMessage = string.Format(
                    Resources.ResourceMessage.ThoiGianThucHienVtytLonHonThoiGianKetThuc,
                    first.TDL_SERVICE_REQ_CODE,
                    first.TDL_SERVICE_NAME,
                    FormatUsedTimeDisplay((long)(first.USED_TIME ?? 0)));

                if (option == "1")
                {
                    // Chan
                    XtraMessageBox.Show(
                        baseMessage,
                        Resources.ResourceMessage.ThongBao,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return false;
                }
                else // option == "2"
                {
                    // Canh bao Co/Khong
                    string confirmMessage = baseMessage + " " + Resources.ResourceMessage.BanCoMuonTiepTuc;
                    DialogResult dr = XtraMessageBox.Show(
                        confirmMessage,
                        Resources.ResourceMessage.ThongBao,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);
                    return dr == DialogResult.Yes;
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                return true; // Loi nghiep vu kiem tra -> khong chan luu
            }
        }

        /// <summary>Format USED_TIME (yyyyMMddHHmmss) -> dd/MM/yyyy HH:mm</summary>
        private string FormatUsedTimeDisplay(long usedTime)
        {
            try
            {
                if (usedTime <= 0) return "";
                DateTime? dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(usedTime);
                if (dt.HasValue)
                    return dt.Value.ToString("dd/MM/yyyy HH:mm");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return usedTime.ToString();
        }
    }
}
