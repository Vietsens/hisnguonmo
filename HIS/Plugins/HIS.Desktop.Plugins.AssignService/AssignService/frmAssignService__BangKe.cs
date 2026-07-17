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
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using HIS.Desktop.Plugins.AssignService.ADO;
using HIS.Desktop.Plugins.Library.PrintBordereau;
using HIS.Desktop.Plugins.Library.PrintBordereau.ADO;
using HIS.Desktop.Plugins.Library.PrintBordereau.Base;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignService.AssignService
{
    public partial class frmAssignService : HIS.Desktop.Utility.FormBase
    {
        /// <summary>
        /// Tiền tố KEY lưu trạng thái tích chọn bảng kê xuống ControlState (local, theo máy).
        /// </summary>
        const string BANG_KE_CONTROL_STATE_PREFIX = "bangke_";

        /// <summary>
        /// 1 KEY duy nhất lưu toàn bộ mã bảng kê đã tích (CSV) - thay vì mỗi bảng kê 1 key.
        /// Tránh bơm ~40 dòng vào ControlState làm chậm mọi lần ghi SQLite (42 UPDATE ~5s).
        /// </summary>
        const string BANG_KE_SELECTED_KEY = "bangke_selected_codes";

        /// <summary>Danh sách bảng kê có thể in (nạp động theo bệnh nhân qua thư viện PrintBordereau).</summary>
        List<BangKeInADO> lstBangKe = new List<BangKeInADO>();

        /// <summary>
        /// Click nút Bảng kê: nạp động danh sách bảng kê in được theo đợt điều trị hiện tại
        /// rồi mở popup tích chọn (giống nút thiết lập in).
        /// </summary>
        private void btnBangKe_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.treatmentId <= 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        "Chưa có đợt điều trị để in bảng kê.",
                        "Thông báo",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Information);
                    return;
                }

                Inventec.Common.Logging.LogSystem.Info("___BANGKE-OPEN___ [A] btnBangKe_Click begin. treatmentId=" + this.treatmentId
                    + "; cachedFor=" + bangKeMenuLoadedForTreatmentId + "; lstBangKe=" + (lstBangKe != null ? lstBangKe.Count : 0));

                WaitingManager.Show();
                LoadBangKeMenu();
                WaitingManager.Hide();

                Inventec.Common.Logging.LogSystem.Info("___BANGKE-OPEN___ [D] show popup. lstBangKe=" + (lstBangKe != null ? lstBangKe.Count : 0));
                popupControlContainerBangKe.ShowPopup(new Point(btnBangKe.Bounds.X, btnBangKe.Bounds.Bottom - 170));
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Nạp danh sách bảng kê từ thư viện PrintBordereau (đúng nội/ngoại trú, BHYT/viện phí của BN),
        /// khôi phục trạng thái tích chọn đã lưu, rồi bind vào lưới.
        /// </summary>
        /// <summary>Đợt điều trị đã nạp menu bảng kê (cache) - tránh gọi lại InitMenuPrint (nặng) mỗi lần mở popup.</summary>
        long bangKeMenuLoadedForTreatmentId = 0;

        private void LoadBangKeMenu()
        {
            try
            {
                // Nếu đã nạp menu cho đúng đợt điều trị này rồi -> tái sử dụng, KHÔNG gọi lại InitMenuPrint (nặng, gọi API).
                bool needLoadMenu = !(bangKeMenuLoadedForTreatmentId == this.treatmentId && lstBangKe != null && lstBangKe.Count > 0);

                if (needLoadMenu)
                {
                    lstBangKe = new List<BangKeInADO>();

                    long patientId = this.currentHisTreatment != null
                        ? this.currentHisTreatment.PATIENT_ID
                        : (this.patientPrint != null ? this.patientPrint.ID : 0);

                    ReloadMenuOption reloadMenu = new ReloadMenuOption();
                    reloadMenu.Type = ReloadMenuOption.MenuType.NORMAL;
                    reloadMenu.ReloadMenu = new HIS.Desktop.Common.DelegateSelectData(CollectBangKeMenuItem);

                    PrintBordereauProcessor processor = new PrintBordereauProcessor(
                        this.currentModule != null ? this.currentModule.RoomId : 0,
                        this.currentModule != null ? this.currentModule.RoomTypeId : 0,
                        this.treatmentId,
                        patientId,
                        null,
                        reloadMenu);

                    Inventec.Common.Logging.LogSystem.Info("___BANGKE-OPEN___ [B] begin InitMenuPrint (nạp dữ liệu điều trị + dựng menu)...");
                    processor.InitMenuPrint();
                    Inventec.Common.Logging.LogSystem.Info("___BANGKE-OPEN___ [C] end InitMenuPrint. lstBangKe=" + lstBangKe.Count);

                    bangKeMenuLoadedForTreatmentId = this.treatmentId;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info("___BANGKE-OPEN___ [B'] Dùng lại menu đã cache (không gọi InitMenuPrint).");
                }

                // Khôi phục trạng thái tích chọn đã lưu tại máy (ControlState) - từ 1 key CSV
                var selectedCodes = GetSelectedBangKeCodes();
                if (selectedCodes.Count > 0)
                {
                    foreach (var item in lstBangKe)
                        item.Check = selectedCodes.Contains(item.PrintTypeCode);
                }

                gridViewBangKe.BeginUpdate();
                gridControlBangKe.DataSource = lstBangKe;
                gridViewBangKe.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Callback thư viện PrintBordereau trả về từng item menu (DXMenuItem: Caption = tên, Tag = mã Mps).
        /// </summary>
        private void CollectBangKeMenuItem(object data)
        {
            try
            {
                DXMenuItem menuItem = data as DXMenuItem;
                if (menuItem == null || menuItem.Tag == null)
                    return;

                string printTypeCode = menuItem.Tag.ToString();
                if (string.IsNullOrWhiteSpace(printTypeCode))
                    return;

                // Tránh trùng mã
                if (lstBangKe.Any(o => o.PrintTypeCode == printTypeCode))
                    return;

                lstBangKe.Add(new BangKeInADO(printTypeCode, menuItem.Caption));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Đánh dấu có thay đổi tích chọn bảng kê cần ghi xuống ControlState khi đóng popup.</summary>
        bool bangKeStateDirty = false;

        /// <summary>
        /// Gom danh sách mã bảng kê đang tích thành 1 key CSV trong currentControlStateRDO (RAM),
        /// đồng thời dọn các key "bangke_&lt;mã&gt;" kiểu cũ (mỗi bảng kê 1 dòng) để lần ghi SQLite tới xóa chúng.
        /// </summary>
        private void UpdateBangKeSelectedInMemory()
        {
            try
            {
                if (this.currentControlStateRDO == null)
                    this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();

                // Dọn key cũ kiểu mỗi-bảng-kê-1-dòng (giữ lại key CSV)
                this.currentControlStateRDO.RemoveAll(o => o.MODULE_LINK == moduleLink
                    && o.KEY != null
                    && o.KEY.StartsWith(BANG_KE_CONTROL_STATE_PREFIX)
                    && o.KEY != BANG_KE_SELECTED_KEY);

                string csv = string.Join(",", lstBangKe.Where(o => o.Check && !string.IsNullOrWhiteSpace(o.PrintTypeCode))
                                                       .Select(o => o.PrintTypeCode)
                                                       .Distinct());

                var cs = this.currentControlStateRDO.FirstOrDefault(o => o.KEY == BANG_KE_SELECTED_KEY && o.MODULE_LINK == moduleLink);
                if (cs != null)
                {
                    cs.VALUE = csv;
                }
                else
                {
                    cs = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    cs.KEY = BANG_KE_SELECTED_KEY;
                    cs.VALUE = csv;
                    cs.MODULE_LINK = moduleLink;
                    this.currentControlStateRDO.Add(cs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewBangKe_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName != "Check")
                    return;

                // CHỈ cập nhật trạng thái trong RAM (nhanh) - KHÔNG ghi SQLite mỗi tick.
                // Gom toàn bộ mã đã tích vào 1 key CSV duy nhất (không tạo ~40 key riêng).
                UpdateBangKeSelectedInMemory();
                bangKeStateDirty = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Ghi trạng thái tích chọn bảng kê xuống ControlState (1 lần) nếu có thay đổi.</summary>
        private void PersistBangKeState()
        {
            try
            {
                if (!bangKeStateDirty)
                    return;
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    this.controlStateWorker.SetData(this.currentControlStateRDO);
                }
                bangKeStateDirty = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemCheckEditBangKe_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckEdit edit = sender as CheckEdit;
                if (edit != null)
                {
                    gridViewBangKe.SetRowCellValue(gridViewBangKe.FocusedRowHandle, colBangKeCheck, edit.Checked);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void popupControlContainerBangKe_CloseUp(object sender, EventArgs e)
        {
            if (this.gridViewBangKe.IsEditing)
                this.gridViewBangKe.CloseEditor();

            if (this.gridViewBangKe.FocusedRowModified)
                this.gridViewBangKe.UpdateCurrentRow();

            // Ghi trạng thái tích chọn xuống ControlState 1 lần khi đóng popup (thay vì mỗi tick)
            PersistBangKeState();
        }

        /// <summary>Đọc danh sách mã bảng kê đã tích từ key CSV trong ControlState (RAM).</summary>
        private List<string> GetSelectedBangKeCodes()
        {
            try
            {
                if (this.currentControlStateRDO == null)
                    return new List<string>();

                var cs = this.currentControlStateRDO.FirstOrDefault(o => o.KEY == BANG_KE_SELECTED_KEY && o.MODULE_LINK == moduleLink);
                if (cs == null || string.IsNullOrWhiteSpace(cs.VALUE))
                    return new List<string>();

                return cs.VALUE.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                               .Select(c => c.Trim())
                               .Where(c => !string.IsNullOrWhiteSpace(c))
                               .Distinct()
                               .ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return new List<string>();
            }
        }

        /// <summary>
        /// In các bảng kê đã tích chọn qua thư viện PrintBordereau.
        /// Được gọi từ luồng lưu-in (giống btnConfiguration): tích chọn chỉ lưu cấu hình,
        /// khi bấm nút In chính mới in kèm.
        /// </summary>
        /// <param name="printNow">In ngay</param>
        /// <param name="isSign">Ký số EMR</param>
        /// <param name="isPrintPreview">Xem trước</param>
        private void InBangKeDaChon(bool printNow, bool isSign, bool isPrintPreview)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("___BANGKE___ [1] Vào InBangKeDaChon. printNow=" + printNow + "; isSign=" + isSign + "; isPrintPreview=" + isPrintPreview
                    + "; currentControlStateRDO=" + (this.currentControlStateRDO != null ? this.currentControlStateRDO.Count.ToString() : "null")
                    + "; moduleLink=" + moduleLink);

                // Lấy mã bảng kê đã tích từ ControlState (1 key CSV) - bền vững, không phụ thuộc việc mở popup.
                var selectedCodes = GetSelectedBangKeCodes();

                Inventec.Common.Logging.LogSystem.Info("___BANGKE___ [2] selectedCodes=" + selectedCodes.Count + " [" + string.Join(",", selectedCodes) + "]");

                if (selectedCodes.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Info("___BANGKE___ [2.1] Không có bảng kê nào được tích -> thoát.");
                    return;
                }

                long patientId = this.patientPrint != null
                    ? this.patientPrint.ID
                    : (this.currentHisTreatment != null ? this.currentHisTreatment.PATIENT_ID : 0);

                Inventec.Common.Logging.LogSystem.Info("___BANGKE___ [3] treatmentId=" + this.treatmentId + "; patientId=" + patientId
                    + "; patientPrint=" + (this.patientPrint != null ? this.patientPrint.ID.ToString() : "null")
                    + "; RoomId=" + (this.currentModule != null ? this.currentModule.RoomId.ToString() : "null"));

                if (this.treatmentId <= 0 || patientId <= 0)
                {
                    Inventec.Common.Logging.LogSystem.Info("___BANGKE___ [3.1] Thiếu treatmentId/patientId -> thoát.");
                    return;
                }

                // Ánh xạ tùy chọn in giống hàm InYeuCauThanhToanQR
                PrintOption.Value? printOption = null;
                if (printNow && !isSign)
                    printOption = PrintOption.Value.PRINT_NOW;
                else if (printNow && isSign)
                    printOption = PrintOption.Value.PRINT_NOW_AND_EMR_SIGN_NOW;
                else if (isPrintPreview && isSign)
                    printOption = PrintOption.Value.EMR_SIGN_AND_PRINT_PREVIEW;
                else if (!printNow && isSign)
                    printOption = PrintOption.Value.EMR_SIGN_NOW;
                // else (isPrintPreview only) -> null: mở dialog/preview mặc định

                PrintBordereauProcessor processor = new PrintBordereauProcessor(
                    this.currentModule != null ? this.currentModule.RoomId : 0,
                    this.currentModule != null ? this.currentModule.RoomTypeId : 0,
                    this.treatmentId,
                    patientId,
                    null,
                    null,
                    GetDocmentSigned);

                Inventec.Common.Logging.LogSystem.Info("___BANGKE___ [4] printOption=" + (printOption.HasValue ? printOption.Value.ToString() : "null") + " -> bắt đầu in " + selectedCodes.Count + " bảng kê.");

                foreach (var code in selectedCodes)
                {
                    Inventec.Common.Logging.LogSystem.Info("___BANGKE___ [5] processor.Print(" + code + ")");
                    processor.Print(code, printOption, null);
                    Inventec.Common.Logging.LogSystem.Info("___BANGKE___ [6] Đã gọi xong Print(" + code + ")");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("___BANGKE___ [ERR] " + ex.ToString());
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
