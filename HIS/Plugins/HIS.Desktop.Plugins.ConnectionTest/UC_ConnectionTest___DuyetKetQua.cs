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
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Plugins.ConnectionTest.ADO;
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ConnectionTest
{
    /// <summary>
    /// Nút "Duyệt KQ / Hủy duyệt KQ" ở khu kết quả + cột "Người duyệt KQ", "TG duyệt" trên cây chỉ số.
    /// Dùng lại nguyên luồng Duyệt KQ / Hủy duyệt KQ cấp mẫu (cột "Duyệt KQ" trên lưới mẫu, Duyệt KQ theo lô):
    /// api/LisSample/ApproveResult, api/LisSample/UnapproveResult, dữ liệu LIS_SAMPLE.RESULT_APPROVAL_*.
    /// </summary>
    public partial class UC_ConnectionTest : HIS.Desktop.Utility.UserControlBase
    {
        #region DuyetKetQua - Declare

        /// <summary>true: nút đang ở chế độ Duyệt KQ; false: Hủy duyệt KQ.</summary>
        private bool isApproveResultMode = true;

        #endregion

        #region DuyetKetQua - Init

        /// <summary>
        /// Gán caption/tooltip nút và 2 cột người duyệt.
        /// Gọi SAU InitRestoreLayoutTreeListFromXml: layout XML cũ không chứa 2 cột mới.
        /// Nút luôn hiển thị như nút "Duyệt KQ theo lô" (không phụ thuộc config); Enabled/Caption theo trạng thái mẫu.
        /// 2 cột mặc định ẩn như các cột ẩn khác của cây chỉ số, người dùng tự bật qua Column Chooser
        /// (trạng thái ẩn/hiện được lưu bởi InitRestoreLayoutTreeListFromXml) -> không set Visible ở đây.
        /// </summary>
        private void InitApproveResultControls()
        {
            try
            {
                this.lciApproveResult.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                this.colResultApprover.Caption = GetLangValue("UC_ConnectionTest.colResultApprover.Caption", "Người duyệt KQ");
                this.colResultApprover.ToolTip = GetLangValue("UC_ConnectionTest.colResultApprover.ToolTip", "Người duyệt kết quả");
                this.colResultApproveTime.Caption = GetLangValue("UC_ConnectionTest.colResultApproveTime.Caption", "TG duyệt");
                this.colResultApproveTime.ToolTip = GetLangValue("UC_ConnectionTest.colResultApproveTime.ToolTip", "Thời gian duyệt kết quả");

                UpdateApproveResultButton();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region DuyetKetQua - Button state

        /// <summary>
        /// Caption/Enabled/Tooltip nút theo trạng thái mẫu đang chọn:
        /// CHAP_NHAN, CO_KQ -> "Duyệt KQ"; DUYET_KQ -> "Hủy duyệt KQ";
        /// TRA_KQ -> "Hủy duyệt KQ" (disable, cần hủy trả KQ trước); khác -> "Duyệt KQ" (disable).
        /// </summary>
        private void UpdateApproveResultButton()
        {
            try
            {
                long? sampleSttId = rowSample != null ? (long?)rowSample.SAMPLE_STT_ID : null;
                bool isApproved = sampleSttId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ;
                bool isReturned = sampleSttId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ;
                bool canApprove = sampleSttId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ
                    || sampleSttId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN;

                this.isApproveResultMode = !(isApproved || isReturned);
                if (this.isApproveResultMode)
                {
                    this.btnApproveResult.Text = GetLangValue("UC_ConnectionTest.btnApproveResult.Text", "Duyệt KQ");
                    this.btnApproveResult.ToolTip = canApprove
                        ? GetLangValue("UC_ConnectionTest.btnApproveResult.ToolTip", "Duyệt kết quả xét nghiệm của mẫu đang chọn")
                        : GetLangValue("UC_ConnectionTest.btnApproveResult.ToolTipNoResult", "Mẫu chưa có kết quả hoặc không ở trạng thái được duyệt");
                    this.btnApproveResult.Enabled = canApprove;
                }
                else
                {
                    this.btnApproveResult.Text = GetLangValue("UC_ConnectionTest.btnApproveResult.TextUnapprove", "Hủy duyệt KQ");
                    this.btnApproveResult.ToolTip = isApproved
                        ? GetLangValue("UC_ConnectionTest.btnApproveResult.ToolTipUnapprove", "Hủy duyệt kết quả xét nghiệm của mẫu đang chọn")
                        : GetLangValue("UC_ConnectionTest.btnApproveResult.ToolTipReturned", "Mẫu đã trả kết quả, cần hủy trả kết quả trước khi hủy duyệt");
                    this.btnApproveResult.Enabled = isApproved;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region DuyetKetQua - Tree columns

        /// <summary>
        /// Điền "Người duyệt KQ", "TG duyệt" của mẫu (LIS_SAMPLE.RESULT_APPROVAL_*) vào các dòng dịch vụ (dòng cha).
        /// Duyệt KQ theo mẫu nên mọi dịch vụ của mẫu cùng 1 người duyệt.
        /// </summary>
        private void FillApproveResultInfo(List<TestLisResultADO> dataList)
        {
            try
            {
                if (dataList == null || !dataList.Any())
                    return;

                string approverUsername = null;
                string approveTimeStr = null;
                if (rowSample != null && rowSample.RESULT_APPROVAL_TIME.HasValue)
                {
                    approverUsername = rowSample.RESULT_APPROVAL_USERNAME;
                    approveTimeStr = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(rowSample.RESULT_APPROVAL_TIME.Value);
                }

                foreach (var item in dataList)
                {
                    bool isServiceRow = item.IS_PARENT == 1 && item.IS_NO_EXECUTE != 1;
                    item.RESULT_APPROVER_USERNAME = isServiceRow ? approverUsername : null;
                    item.RESULT_APPROVE_TIME_STR = isServiceRow ? approveTimeStr : null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Sau Duyệt / Hủy duyệt KQ (từ nút mới, cột lưới mẫu hoặc form chọn giờ duyệt):
        /// đồng bộ trạng thái + người duyệt của mẫu vào rowSample và cây chỉ số, không load lại kết quả.
        /// Cây dùng SAMPLE_STT_ID để khóa/mở ô kết quả như luồng Duyệt KQ hiện có.
        /// </summary>
        private void SyncApproveResultToTree(long sampleId)
        {
            try
            {
                var sample = lstSampleAll != null ? lstSampleAll.FirstOrDefault(o => o.ID == sampleId) : null;
                if (sample == null || rowSample == null || rowSample.ID != sampleId)
                {
                    UpdateApproveResultButton();
                    return;
                }

                if (!Object.ReferenceEquals(sample, rowSample))
                {
                    rowSample.SAMPLE_STT_ID = sample.SAMPLE_STT_ID;
                    rowSample.RESULT_APPROVAL_TIME = sample.RESULT_APPROVAL_TIME;
                    rowSample.RESULT_APPROVAL_LOGINNAME = sample.RESULT_APPROVAL_LOGINNAME;
                    rowSample.RESULT_APPROVAL_USERNAME = sample.RESULT_APPROVAL_USERNAME;
                }

                if (lstLisResultADOs != null && lstLisResultADOs.Any())
                {
                    lstLisResultADOs.ForEach(o => o.SAMPLE_STT_ID = rowSample.SAMPLE_STT_ID);
                    FillApproveResultInfo(lstLisResultADOs);
                    this.treeListSereServTein.RefreshDataSource();
                }
                UpdateApproveResultButton();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region DuyetKetQua - Click

        private void btnApproveResult_Click(object sender, EventArgs e)
        {
            long? sampleId = null;
            try
            {
                // Thao tác trên mẫu đang hiển thị kết quả (rowSample), không phụ thuộc dòng đang focus:
                // handler Duyệt/Hủy duyệt cấp mẫu làm mới lưới bằng FocusedRowHandle - 1 / + 1 -> mẫu ở dòng đầu bị đẩy focus sang dòng 2.
                if (rowSample == null)
                {
                    UpdateApproveResultButton();
                    return;
                }
                sampleId = rowSample.ID;
                // Handler cấp mẫu đọc dòng đang focus -> đưa focus về đúng mẫu đang hiển thị
                if (!FocusSampleRow(sampleId.Value))
                {
                    UpdateApproveResultButton();
                    return;
                }
                var row = (LisSampleADO)gridViewSample.GetFocusedRow();

                if (this.isApproveResultMode)
                {
                    if (!CheckBeforeApproveResult(row))
                        return;
                    repositoryDuyetKetQuaE_ButtonClick(null, null);
                }
                else
                {
                    if (XtraMessageBox.Show(
                        String.Format(Resources.ResourceMessage.BanCoMuonHuyDuyetKetQuaMau, row.BARCODE),
                        MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        return;
                    repositoryHuyDuyetKetQuaE_ButtonClick(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                // Trả focus về mẫu vừa thao tác (handler cấp mẫu có thể đã đẩy focus sang dòng khác)
                if (sampleId.HasValue)
                    FocusSampleRow(sampleId.Value);
                UpdateApproveResultButton();
            }
        }

        /// <summary>
        /// Focus dòng mẫu theo ID trên lưới mẫu. Trả về false nếu mẫu không còn trên lưới.
        /// </summary>
        private bool FocusSampleRow(long sampleId)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < gridViewSample.DataRowCount; i++)
                {
                    var data = gridViewSample.GetRow(i) as LisSampleADO;
                    if (data != null && data.ID == sampleId)
                    {
                        if (gridViewSample.FocusedRowHandle != i)
                            gridViewSample.FocusedRowHandle = i;
                        result = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Kiểm tra bổ sung chỉ cho nút ở khu kết quả (các kiểm tra của luồng Duyệt KQ giữ nguyên):
        /// 1. Có giá trị / ghi chú chỉ số đã sửa mà chưa lưu -> yêu cầu lưu trước.
        /// 2. Chỉ tích một phần dịch vụ -> xác nhận duyệt áp dụng cho toàn bộ mẫu.
        /// </summary>
        private bool CheckBeforeApproveResult(LisSampleADO row)
        {
            bool result = true;
            try
            {
                this.treeListSereServTein.PostEditor();
                if (lstLisResultADOs != null && lstLisResultADOs.Any(o => o.Item_Edit_Value == 1))
                {
                    XtraMessageBox.Show(Resources.ResourceMessage.VuiLongLuuTruocKhiDuyet,
                        MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    return false;
                }

                if (IsPartialServiceChecked()
                    && XtraMessageBox.Show(
                        String.Format(Resources.ResourceMessage.DuyetApDungToanBoMau, row.BARCODE),
                        MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// true khi có ≥1 dịch vụ được tích (tích dòng chỉ số quy về dịch vụ cha) nhưng chưa tích đủ các dịch vụ thực hiện của mẫu.
        /// </summary>
        private bool IsPartialServiceChecked()
        {
            bool result = false;
            try
            {
                if (lstLisResultADOs == null || !lstLisResultADOs.Any())
                    return false;

                var allServiceKeys = new HashSet<string>(lstLisResultADOs
                    .Where(o => o.IS_PARENT == 1 && o.IS_NO_EXECUTE != 1)
                    .Select(o => o.CHILD_ID));
                if (!allServiceKeys.Any())
                    return false;

                var checkedServiceKeys = new HashSet<string>();
                List<TreeListNode> checkedNodes = this.treeListSereServTein.GetAllCheckedNodes();
                if (checkedNodes != null)
                {
                    foreach (var node in checkedNodes)
                    {
                        var data = this.treeListSereServTein.GetDataRecordByNode(node) as TestLisResultADO;
                        if (data == null)
                            continue;
                        string serviceKey = data.IS_PARENT == 1 ? data.CHILD_ID : data.PARENT_ID;
                        if (allServiceKeys.Contains(serviceKey))
                            checkedServiceKeys.Add(serviceKey);
                    }
                }

                result = checkedServiceKeys.Any() && checkedServiceKeys.Count < allServiceKeys.Count;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        #endregion
    }
}
