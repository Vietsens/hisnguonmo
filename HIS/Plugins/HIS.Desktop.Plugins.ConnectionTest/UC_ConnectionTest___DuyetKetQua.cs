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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Plugins.ConnectionTest.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using LIS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ConnectionTest
{
    /// <summary>
    /// Duyệt / Hủy duyệt kết quả THEO TỪNG DỊCH VỤ (LIS_SAMPLE_SERVICE.SERVICE_APPROVER_* / SERVICE_APPROVE_TIME).
    /// - Nút "Duyệt KQ / Hủy duyệt KQ" luôn hiển thị (như "Duyệt KQ theo lô"), áp dụng cho các dịch vụ được tích.
    /// - Bật khi có dịch vụ được tích đã đủ kết quả (Đã có KQ / Đã trả KQ) và chưa duyệt; mẫu chưa chấp nhận -> mờ.
    /// - Dịch vụ đã duyệt: khóa ô kết quả (như trạng thái Duyệt KQ của mẫu); cột Người duyệt KQ / TG duyệt theo dịch vụ.
    /// API: api/LisSampleService/Get (đọc), api/LisSampleService/ApproveResult, api/LisSampleService/UnapproveResult.
    /// </summary>
    public partial class UC_ConnectionTest : HIS.Desktop.Utility.UserControlBase
    {
        #region DuyetKetQua - Declare

        private const string URI_SAMPLE_SERVICE_GET = "api/LisSampleService/Get";
        private const string URI_SAMPLE_SERVICE_APPROVE_RESULT = "api/LisSampleService/ApproveResult";
        private const string URI_SAMPLE_SERVICE_UNAPPROVE_RESULT = "api/LisSampleService/UnapproveResult";

        /// <summary>true: nút đang ở chế độ Duyệt KQ; false: Hủy duyệt KQ.</summary>
        private bool isApproveResultMode = true;

        /// <summary>Thông tin duyệt theo dịch vụ của mẫu đang chọn, key = LIS_SAMPLE_SERVICE.ID.</summary>
        private Dictionary<long, SampleServiceApproveADO> dicSampleServiceApprove = new Dictionary<long, SampleServiceApproveADO>();

        /// <summary>Dịch vụ sẽ gửi khi bấm nút (tính trong UpdateApproveResultButton).</summary>
        private List<SampleServiceApproveADO> approveTargetServices = new List<SampleServiceApproveADO>();

        #endregion

        #region DuyetKetQua - Init

        /// <summary>
        /// Gán caption/tooltip nút và 2 cột người duyệt. Gọi SAU InitRestoreLayoutTreeListFromXml.
        /// Nút luôn hiển thị như "Duyệt KQ theo lô"; 2 cột mặc định ẩn (bật qua Column Chooser) -> không set Visible.
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

        #region DuyetKetQua - Load data

        /// <summary>
        /// Nạp thông tin duyệt theo dịch vụ của mẫu (1 lần gọi api/LisSampleService/Get theo SAMPLE_ID).
        /// </summary>
        private void LoadSampleServiceApprove(long? sampleId)
        {
            try
            {
                this.dicSampleServiceApprove = new Dictionary<long, SampleServiceApproveADO>();
                if (!sampleId.HasValue || sampleId.Value <= 0)
                    return;

                CommonParam param = new CommonParam();
                LisSampleServiceFilter filter = new LisSampleServiceFilter();
                filter.SAMPLE_ID = sampleId.Value;
                var data = new BackendAdapter(param).Get<List<SampleServiceApproveADO>>(URI_SAMPLE_SERVICE_GET, ApiConsumers.LisConsumer, filter, param);
                if (data != null)
                {
                    this.dicSampleServiceApprove = data.GroupBy(o => o.ID).ToDictionary(g => g.Key, g => g.First());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gán cờ IS_SERVICE_APPROVED cho mọi dòng của dịch vụ đã duyệt (khóa ô kết quả)
        /// và "Người duyệt KQ", "TG duyệt" cho dòng dịch vụ (dòng cha).
        /// </summary>
        private void FillApproveResultInfo(List<TestLisResultADO> dataList)
        {
            try
            {
                if (dataList == null || !dataList.Any())
                    return;

                foreach (var item in dataList)
                {
                    SampleServiceApproveADO info = null;
                    if (item.SAMPLE_SERVICE_ID.HasValue)
                        this.dicSampleServiceApprove.TryGetValue(item.SAMPLE_SERVICE_ID.Value, out info);

                    bool isApproved = info != null && info.SERVICE_APPROVE_TIME.HasValue;
                    item.IS_SERVICE_APPROVED = isApproved;
                    bool isServiceRow = item.IS_PARENT == 1;
                    item.RESULT_APPROVER_USERNAME = isServiceRow && isApproved ? info.SERVICE_APPROVER_USERNAME : null;
                    item.RESULT_APPROVE_TIME_STR = isServiceRow && isApproved
                        ? Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(info.SERVICE_APPROVE_TIME.Value)
                        : null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Sau Duyệt / Hủy duyệt KQ cấp mẫu (cột lưới mẫu): đồng bộ trạng thái mẫu vào cây chỉ số (khóa/mở ô theo DUYET_KQ).
        /// </summary>
        private void SyncApproveResultToTree(long sampleId)
        {
            try
            {
                var sample = lstSampleAll != null ? lstSampleAll.FirstOrDefault(o => o.ID == sampleId) : null;
                if (sample != null && rowSample != null && rowSample.ID == sampleId)
                {
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
                        this.treeListSereServTein.RefreshDataSource();
                    }
                }
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
        /// Dịch vụ (LIS_SAMPLE_SERVICE) đang được tích trên cây — tích dòng chỉ số quy về dịch vụ của nó. Bỏ dịch vụ không thực hiện.
        /// </summary>
        private List<SampleServiceApproveADO> GetCheckedSampleServices()
        {
            List<SampleServiceApproveADO> result = new List<SampleServiceApproveADO>();
            try
            {
                HashSet<long> ids = new HashSet<long>();
                List<TreeListNode> checkedNodes = this.treeListSereServTein.GetAllCheckedNodes();
                if (checkedNodes != null)
                {
                    foreach (var node in checkedNodes)
                    {
                        var data = this.treeListSereServTein.GetDataRecordByNode(node) as TestLisResultADO;
                        if (data != null && data.SAMPLE_SERVICE_ID.HasValue)
                            ids.Add(data.SAMPLE_SERVICE_ID.Value);
                    }
                }
                foreach (var id in ids)
                {
                    SampleServiceApproveADO info;
                    if (this.dicSampleServiceApprove.TryGetValue(id, out info) && info.IS_NO_EXECUTE != 1)
                        result.Add(info);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private static bool IsServiceHasFullResult(SampleServiceApproveADO info)
        {
            return info.SAMPLE_SERVICE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_SERVICE_STT.ID__DA_CO_KQ
                || info.SAMPLE_SERVICE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_SERVICE_STT.ID__DA_TRA_KQ;
        }

        /// <summary>
        /// Caption/Enabled/Tooltip theo các dịch vụ được tích:
        /// có dịch vụ đủ KQ chưa duyệt -> "Duyệt KQ" (mờ nếu mẫu chưa chấp nhận);
        /// còn lại toàn dịch vụ đã duyệt -> "Hủy duyệt KQ" (mờ nếu mẫu đã trả KQ);
        /// không có dịch vụ đủ KQ -> "Duyệt KQ" mờ.
        /// </summary>
        private void UpdateApproveResultButton()
        {
            try
            {
                this.approveTargetServices = new List<SampleServiceApproveADO>();
                long? sampleSttId = rowSample != null ? (long?)rowSample.SAMPLE_STT_ID : null;
                bool isSampleNotAccepted = sampleSttId == null
                    || sampleSttId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM
                    || sampleSttId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM
                    || sampleSttId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI;
                bool isSampleReturned = sampleSttId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ;

                var checkedServices = rowSample != null ? GetCheckedSampleServices() : new List<SampleServiceApproveADO>();
                var approvable = checkedServices.Where(o => !o.SERVICE_APPROVE_TIME.HasValue && IsServiceHasFullResult(o)).ToList();
                var approved = checkedServices.Where(o => o.SERVICE_APPROVE_TIME.HasValue).ToList();

                // Chẩn đoán trạng thái nút: 1 dòng log mỗi lần tính (không log trong vòng lặp)
                Inventec.Common.Logging.LogSystem.Debug("UpdateApproveResultButton: SAMPLE_ID=" + (rowSample != null ? rowSample.ID.ToString() : "null")
                    + "; SAMPLE_STT_ID=" + (sampleSttId.HasValue ? sampleSttId.Value.ToString() : "null")
                    + "; loadedServices=" + this.dicSampleServiceApprove.Count
                    + "; checked=[" + String.Join(", ", checkedServices.Select(o => o.ID + ":" + o.SERVICE_CODE + "/stt=" + o.SAMPLE_SERVICE_STT_ID + "/noExecute=" + o.IS_NO_EXECUTE + "/approved=" + o.SERVICE_APPROVE_TIME.HasValue)) + "]"
                    + "; approvable=" + approvable.Count + "; approved=" + approved.Count);

                if (approvable.Any())
                {
                    this.isApproveResultMode = true;
                    this.approveTargetServices = approvable;
                    this.btnApproveResult.Text = GetLangValue("UC_ConnectionTest.btnApproveResult.Text", "Duyệt KQ");
                    this.btnApproveResult.Enabled = !isSampleNotAccepted;
                    this.btnApproveResult.ToolTip = isSampleNotAccepted
                        ? GetLangValue("UC_ConnectionTest.btnApproveResult.ToolTipNotAccepted", "Mẫu chưa được chấp nhận, chưa thể duyệt kết quả")
                        : GetLangValue("UC_ConnectionTest.btnApproveResult.ToolTip", "Duyệt kết quả các dịch vụ được tích");
                }
                else if (approved.Any())
                {
                    this.isApproveResultMode = false;
                    this.approveTargetServices = approved;
                    this.btnApproveResult.Text = GetLangValue("UC_ConnectionTest.btnApproveResult.TextUnapprove", "Hủy duyệt KQ");
                    this.btnApproveResult.Enabled = !isSampleReturned;
                    this.btnApproveResult.ToolTip = isSampleReturned
                        ? GetLangValue("UC_ConnectionTest.btnApproveResult.ToolTipReturned", "Mẫu đã trả kết quả, cần hủy trả kết quả trước khi hủy duyệt")
                        : GetLangValue("UC_ConnectionTest.btnApproveResult.ToolTipUnapprove", "Hủy duyệt kết quả các dịch vụ được tích");
                }
                else
                {
                    this.isApproveResultMode = true;
                    this.btnApproveResult.Text = GetLangValue("UC_ConnectionTest.btnApproveResult.Text", "Duyệt KQ");
                    this.btnApproveResult.Enabled = false;
                    this.btnApproveResult.ToolTip = GetLangValue("UC_ConnectionTest.btnApproveResult.ToolTipNoResult", "Dịch vụ được tích chưa có đủ kết quả");
                }
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
            try
            {
                // Thao tác trên mẫu đang hiển thị kết quả (rowSample), không phụ thuộc dòng đang focus trên lưới mẫu
                // (focus có thể bị đổi bởi bàn phím hoặc handler Duyệt KQ cấp mẫu: FocusedRowHandle - 1 / + 1).
                var row = rowSample;
                if (row == null || this.approveTargetServices == null || !this.approveTargetServices.Any())
                {
                    UpdateApproveResultButton();
                    return;
                }

                var targets = this.approveTargetServices.ToList();
                string serviceNames = String.Join(", ", targets.Select(o => !String.IsNullOrWhiteSpace(o.SERVICE_NAME) ? o.SERVICE_NAME : o.SERVICE_CODE));
                if (this.isApproveResultMode)
                {
                    if (!CheckNoUnsavedResult(targets))
                        return;
                }
                else if (XtraMessageBox.Show(
                    String.Format(Resources.ResourceMessage.BanCoMuonHuyDuyetKetQuaDichVu, serviceNames),
                    MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }

                this.btnApproveResult.Enabled = false;
                ProcessApproveServices(row, targets, this.isApproveResultMode);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                UpdateApproveResultButton();
            }
        }

        /// <summary>
        /// Không cho duyệt khi còn giá trị / ghi chú chỉ số của dịch vụ được duyệt đã sửa mà chưa lưu.
        /// </summary>
        private bool CheckNoUnsavedResult(List<SampleServiceApproveADO> targets)
        {
            bool result = true;
            try
            {
                this.treeListSereServTein.PostEditor();
                var targetIds = new HashSet<long>(targets.Select(o => o.ID));
                if (lstLisResultADOs != null && lstLisResultADOs.Any(o => o.Item_Edit_Value == 1 && o.SAMPLE_SERVICE_ID.HasValue && targetIds.Contains(o.SAMPLE_SERVICE_ID.Value)))
                {
                    XtraMessageBox.Show(Resources.ResourceMessage.VuiLongLuuTruocKhiDuyet,
                        MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
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
        /// Gọi api/LisSampleService/ApproveResult hoặc UnapproveResult cho các dịch vụ, cập nhật cây chỉ số theo kết quả trả về.
        /// </summary>
        private void ProcessApproveServices(LisSampleADO sample, List<SampleServiceApproveADO> targets, bool isApprove)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                WaitingManager.Show();
                SampleServiceApproveSDO sdo = new SampleServiceApproveSDO();
                sdo.SampleId = sample.ID;
                sdo.SampleServiceIds = targets.Select(o => o.ID).ToList();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));

                var rs = new BackendAdapter(param).Post<List<SampleServiceApproveADO>>(
                    isApprove ? URI_SAMPLE_SERVICE_APPROVE_RESULT : URI_SAMPLE_SERVICE_UNAPPROVE_RESULT,
                    ApiConsumers.LisConsumer, sdo, param);
                if (rs != null)
                {
                    success = true;
                    foreach (var item in rs)
                    {
                        this.dicSampleServiceApprove[item.ID] = item;
                    }
                    FillApproveResultInfo(lstLisResultADOs);
                    this.treeListSereServTein.RefreshDataSource();
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion
    }
}
