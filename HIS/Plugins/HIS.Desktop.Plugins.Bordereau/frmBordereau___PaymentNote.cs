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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.Plugins.Bordereau.ADO;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.LibraryMessage;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Bordereau
{
    /// <summary>
    /// Cột "Ghi chú thanh toán" (HIS_SERE_SERV.PAYMENT_NOTE) cho bảng kê thanh toán.
    /// - Ô nhập nhiều dòng (MemoEdit, WordWrap), tách biệt với cột ghi chú/mô tả hiện có.
    /// - Phân quyền nút: chỉ tài khoản được cấp quyền control (vd Sale) mới sửa được;
    ///   tài khoản không có quyền → ô ở trạng thái chỉ đọc. Dùng cơ chế phân quyền control
    ///   sẵn có của HIS (GlobalVariables.AcsAuthorizeSDO.ControlInRoles theo CONTROL_CODE).
    /// - Khi sửa → gọi UpdatePayslipInfoProcess với UpdateField.PAYMENT_NOTE (API 3.1.1).
    /// </summary>
    public partial class frmBordereau
    {
        #region Declare PaymentNote

        /// <summary>
        /// Mã control phân quyền sửa "Ghi chú thanh toán" (CONTROL_CODE trong ACS_CONTROL).
        /// Record ACS_CONTROL mã này đã được tạo + gán cho role được phép (vd Sale) qua
        /// ACS_CONTROL_ROLE. Quyền được check thuần bằng CONTROL_CODE trên dữ liệu ACS đã load
        /// lúc đăng nhập (GlobalVariables.AcsAuthorizeSDO.ControlInRoles) — giống ExpenseList,
        /// AccountBook (HIS000022)...
        /// </summary>
        private const string BTN_EDIT_PAYMENT_NOTE_CONTROL_CODE = "HIS000054";

        /// <summary>Giới hạn độ dài PAYMENT_NOTE theo backend (2000 byte).</summary>
        private const int PAYMENT_NOTE_MAX_BYTE = 2000;

        /// <summary>True nếu tài khoản hiện tại được phép sửa Ghi chú thanh toán.</summary>
        private bool isAllowEditPaymentNote = false;

        /// <summary>Cột Ghi chú thanh toán (bound PAYMENT_NOTE) — tạo runtime, thêm cuối grid.</summary>
        private GridColumn gridColPaymentNote;

        /// <summary>Editor cho phép sửa (có quyền).</summary>
        private RepositoryItemMemoEdit repositoryItemMemoEditPaymentNote;

        /// <summary>Editor chỉ đọc (không quyền).</summary>
        private RepositoryItemMemoEdit repositoryItemMemoEditPaymentNote_Disable;

        #endregion

        /// <summary>
        /// Khởi tạo cột "Ghi chú thanh toán": xác định quyền, repository editor, cột grid.
        /// Gọi trong frmBordereau_Load (sau VisableColumnInGrid, trước khi bind data).
        /// </summary>
        private void InitPaymentNoteColumn()
        {
            try
            {
                // 1. Xác định quyền sửa theo phân quyền control sẵn có của HIS (ACS_CONTROL).
                this.isAllowEditPaymentNote = CheckAllowEditPaymentNote();

                // 2. Repository editor — nhập nhiều dòng, WordWrap.
                this.repositoryItemMemoEditPaymentNote = new RepositoryItemMemoEdit();
                this.repositoryItemMemoEditPaymentNote.Name = "repositoryItemMemoEditPaymentNote";
                this.repositoryItemMemoEditPaymentNote.Appearance.Options.UseTextOptions = true;
                this.repositoryItemMemoEditPaymentNote.Appearance.TextOptions.WordWrap = WordWrap.Wrap;

                this.repositoryItemMemoEditPaymentNote_Disable = new RepositoryItemMemoEdit();
                this.repositoryItemMemoEditPaymentNote_Disable.Name = "repositoryItemMemoEditPaymentNote_Disable";
                this.repositoryItemMemoEditPaymentNote_Disable.Appearance.Options.UseTextOptions = true;
                this.repositoryItemMemoEditPaymentNote_Disable.Appearance.TextOptions.WordWrap = WordWrap.Wrap;
                this.repositoryItemMemoEditPaymentNote_Disable.ReadOnly = true;
                this.repositoryItemMemoEditPaymentNote_Disable.Enabled = false;

                this.gridControlBordereau.RepositoryItems.AddRange(new RepositoryItem[]
                {
                    this.repositoryItemMemoEditPaymentNote,
                    this.repositoryItemMemoEditPaymentNote_Disable
                });

                // 3. Cột grid — bound PAYMENT_NOTE, đặt cuối (sau "Đi kèm DV").
                this.gridColPaymentNote = new GridColumn();
                this.gridColPaymentNote.Caption = GetPaymentNoteRes("frmBordereau.gridColPaymentNote.Caption", "Ghi chú thanh toán");
                this.gridColPaymentNote.FieldName = "PAYMENT_NOTE";
                this.gridColPaymentNote.Name = "gridColPaymentNote";
                this.gridColPaymentNote.ToolTip = this.gridColPaymentNote.Caption;
                // Không quyền → disable hẳn cột: AllowEdit=false + ReadOnly=true (editor không mở được).
                this.gridColPaymentNote.OptionsColumn.AllowEdit = this.isAllowEditPaymentNote;
                this.gridColPaymentNote.OptionsColumn.ReadOnly = !this.isAllowEditPaymentNote;
                this.gridColPaymentNote.ColumnEdit = this.isAllowEditPaymentNote
                    ? this.repositoryItemMemoEditPaymentNote
                    : this.repositoryItemMemoEditPaymentNote_Disable;
                this.gridColPaymentNote.Width = 150;
                this.gridColPaymentNote.Visible = true;

                this.gridViewBordereau.Columns.Add(this.gridColPaymentNote);
                // Đặt ngay sau cột "Đi kèm DV" (gridColumnDvDinhKem) thay vì cuối grid.
                this.gridColPaymentNote.VisibleIndex = this.gridColumnDvDinhKem.VisibleIndex + 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Kiểm tra quyền sửa Ghi chú thanh toán theo phân quyền control của HIS.
        /// Tài khoản toàn quyền (IsFull) hoặc role có chứa CONTROL_CODE → cho phép.
        /// </summary>
        private bool CheckAllowEditPaymentNote()
        {
            bool result = false;
            try
            {
                var acs = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.AcsAuthorizeSDO;
                if (acs == null)
                    return false;

                if (acs.IsFull)
                {
                    Inventec.Common.Logging.LogSystem.Debug("CheckAllowEditPaymentNote: allow do IsFull=true (tai khoan toan quyen).");
                    return true;
                }

                if (acs.ControlInRoles != null)
                {
                    result = acs.ControlInRoles.Any(o => o.CONTROL_CODE == BTN_EDIT_PAYMENT_NOTE_CONTROL_CODE);
                }

                Inventec.Common.Logging.LogSystem.Debug(string.Format(
                    "CheckAllowEditPaymentNote: IsFull=false, ControlInRoles.Count={0}, co HIS000054={1}",
                    acs.ControlInRoles != null ? acs.ControlInRoles.Count : 0, result));
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Lưu Ghi chú thanh toán của 1 dòng dịch vụ.
        /// Validate độ dài ≤ 2000 byte → gọi UpdatePayslipInfoProcess với UpdateField.PAYMENT_NOTE.
        /// </summary>
        private void UpdatePaymentNoteProcess(SereServADO sereServADO)
        {
            try
            {
                if (sereServADO == null)
                    return;

                // An toàn: không có quyền thì không gọi API (UI đã ở trạng thái chỉ đọc).
                if (!this.isAllowEditPaymentNote)
                    return;

                string note = sereServADO.PAYMENT_NOTE;
                if (!string.IsNullOrEmpty(note)
                    && Encoding.UTF8.GetByteCount(note) > PAYMENT_NOTE_MAX_BYTE)
                {
                    XtraMessageBox.Show(
                        GetPaymentNoteRes("frmBordereau.Message.PaymentNoteMaxLength", "Ghi chú thanh toán vượt quá độ dài cho phép"),
                        MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                HIS_SERE_SERV sereServ = new HIS_SERE_SERV();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV>(sereServ, sereServADO);

                HisSereServPayslipSDO sdo = new HisSereServPayslipSDO();
                sdo.SereServs = new List<HIS_SERE_SERV> { sereServ };
                sdo.TreatmentId = this.currentTreatment.ID;
                sdo.Field = UpdateField.PAYMENT_NOTE;

                this.UpdatePayslipInfoProcess(sdo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Lấy chuỗi đa ngôn ngữ, fallback giá trị mặc định khi thiếu key.</summary>
        private string GetPaymentNoteRes(string key, string defaultValue)
        {
            try
            {
                string value = Inventec.Common.Resource.Get.Value(
                    key,
                    Base.ResourceLangManager.LanguageFrmBorderau,
                    LanguageManager.GetCulture());
                return string.IsNullOrEmpty(value) ? defaultValue : value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return defaultValue;
        }
    }
}
