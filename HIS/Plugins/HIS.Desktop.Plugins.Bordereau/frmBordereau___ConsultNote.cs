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
    /// Cột "Ghi chú tư vấn" (HIS_SERE_SERV.CONSULT_NOTE) cho bảng kê thanh toán.
    /// - Ô nhập nhiều dòng (MemoEdit, WordWrap), tách biệt với cột "Ghi chú thanh toán".
    /// - Phân quyền nút: DÙNG CHUNG mã control với "Ghi chú thanh toán" (HIS000054) —
    ///   tài khoản sửa được ghi chú thanh toán thì đồng thời sửa được ghi chú tư vấn;
    ///   tài khoản không có quyền → ô ở trạng thái chỉ đọc.
    /// - Khi sửa → gọi UpdatePayslipInfoProcess với UpdateField.CONSULT_NOTE (cùng API 3.1.1
    ///   api/HisSereServ/UpdatePayslipInfo, chỉ khác loại trường cập nhật).
    /// </summary>
    public partial class frmBordereau
    {
        #region Declare ConsultNote

        /// <summary>Giới hạn độ dài CONSULT_NOTE theo backend (2000 byte).</summary>
        private const int CONSULT_NOTE_MAX_BYTE = 2000;

        /// <summary>True nếu tài khoản hiện tại được phép sửa Ghi chú tư vấn.</summary>
        private bool isAllowEditConsultNote = false;

        /// <summary>Cột Ghi chú tư vấn (bound CONSULT_NOTE) — tạo runtime, đặt ngay sau "Ghi chú thanh toán".</summary>
        private GridColumn gridColConsultNote;

        /// <summary>Editor cho phép sửa (có quyền).</summary>
        private RepositoryItemMemoEdit repositoryItemMemoEditConsultNote;

        /// <summary>Editor chỉ đọc (không quyền).</summary>
        private RepositoryItemMemoEdit repositoryItemMemoEditConsultNote_Disable;

        #endregion

        /// <summary>
        /// Khởi tạo cột "Ghi chú tư vấn": xác định quyền, repository editor, cột grid.
        /// Gọi trong frmBordereau_Load NGAY SAU InitPaymentNoteColumn (để tái sử dụng quyền đã tính
        /// và đặt cột ngay sau cột "Ghi chú thanh toán").
        /// </summary>
        private void InitConsultNoteColumn()
        {
            try
            {
                // 1. Quyền sửa DÙNG CHUNG với "Ghi chú thanh toán" (HIS000054).
                //    isAllowEditPaymentNote đã được tính trong InitPaymentNoteColumn (gọi trước).
                this.isAllowEditConsultNote = this.isAllowEditPaymentNote;

                // 2. Repository editor — nhập nhiều dòng, WordWrap.
                this.repositoryItemMemoEditConsultNote = new RepositoryItemMemoEdit();
                this.repositoryItemMemoEditConsultNote.Name = "repositoryItemMemoEditConsultNote";
                this.repositoryItemMemoEditConsultNote.Appearance.Options.UseTextOptions = true;
                this.repositoryItemMemoEditConsultNote.Appearance.TextOptions.WordWrap = WordWrap.Wrap;

                this.repositoryItemMemoEditConsultNote_Disable = new RepositoryItemMemoEdit();
                this.repositoryItemMemoEditConsultNote_Disable.Name = "repositoryItemMemoEditConsultNote_Disable";
                this.repositoryItemMemoEditConsultNote_Disable.Appearance.Options.UseTextOptions = true;
                this.repositoryItemMemoEditConsultNote_Disable.Appearance.TextOptions.WordWrap = WordWrap.Wrap;
                this.repositoryItemMemoEditConsultNote_Disable.ReadOnly = true;
                this.repositoryItemMemoEditConsultNote_Disable.Enabled = false;

                this.gridControlBordereau.RepositoryItems.AddRange(new RepositoryItem[]
                {
                    this.repositoryItemMemoEditConsultNote,
                    this.repositoryItemMemoEditConsultNote_Disable
                });

                // 3. Cột grid — bound CONSULT_NOTE, đặt ngay sau "Ghi chú thanh toán".
                this.gridColConsultNote = new GridColumn();
                this.gridColConsultNote.Caption = GetConsultNoteRes("frmBordereau.gridColConsultNote.Caption", "Ghi chú tư vấn");
                this.gridColConsultNote.FieldName = "CONSULT_NOTE";
                this.gridColConsultNote.Name = "gridColConsultNote";
                this.gridColConsultNote.ToolTip = this.gridColConsultNote.Caption;
                // Không quyền → disable hẳn cột: AllowEdit=false + ReadOnly=true (editor không mở được).
                this.gridColConsultNote.OptionsColumn.AllowEdit = this.isAllowEditConsultNote;
                this.gridColConsultNote.OptionsColumn.ReadOnly = !this.isAllowEditConsultNote;
                this.gridColConsultNote.ColumnEdit = this.isAllowEditConsultNote
                    ? this.repositoryItemMemoEditConsultNote
                    : this.repositoryItemMemoEditConsultNote_Disable;
                this.gridColConsultNote.Width = 150;
                this.gridColConsultNote.Visible = true;

                this.gridViewBordereau.Columns.Add(this.gridColConsultNote);
                // Đặt ngay sau cột "Ghi chú thanh toán" (gridColPaymentNote).
                if (this.gridColPaymentNote != null)
                    this.gridColConsultNote.VisibleIndex = this.gridColPaymentNote.VisibleIndex + 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Lưu Ghi chú tư vấn của 1 dòng dịch vụ.
        /// Validate độ dài ≤ 2000 byte → gọi UpdatePayslipInfoProcess với UpdateField.CONSULT_NOTE.
        /// </summary>
        private void UpdateConsultNoteProcess(SereServADO sereServADO)
        {
            try
            {
                if (sereServADO == null)
                    return;

                // An toàn: không có quyền thì không gọi API (UI đã ở trạng thái chỉ đọc).
                if (!this.isAllowEditConsultNote)
                    return;

                string note = sereServADO.CONSULT_NOTE;
                if (!string.IsNullOrEmpty(note)
                    && Encoding.UTF8.GetByteCount(note) > CONSULT_NOTE_MAX_BYTE)
                {
                    XtraMessageBox.Show(
                        GetConsultNoteRes("frmBordereau.Message.ConsultNoteMaxLength", "Ghi chú tư vấn vượt quá độ dài cho phép"),
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
                sdo.Field = UpdateField.CONSULT_NOTE;

                this.UpdatePayslipInfoProcess(sdo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Lấy chuỗi đa ngôn ngữ, fallback giá trị mặc định khi thiếu key.</summary>
        private string GetConsultNoteRes(string key, string defaultValue)
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
