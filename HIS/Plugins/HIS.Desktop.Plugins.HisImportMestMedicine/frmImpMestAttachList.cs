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
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisImportMestMedicine
{
    // v42244 (v1.3) - Màn hình "Danh sách tài liệu đính kèm" của 1 phiếu nhập.
    // Mở từ menu chuột phải "Đính kèm file" (thay cho việc mở thẳng form đính kèm).
    // Theo pattern màn hình "Danh sách văn bản" (EMR.Desktop.Plugins.EmrDocumentList.UCEmrDocumentList).
    public partial class frmImpMestAttachList : Form
    {
        #region Declare

        // v42244 - HIS_CODE tổng hợp của phiếu nhập ("{MaSite} IMP_MEST_CODE:.. DOCUMENT_NUMBER:..") - lọc tài liệu theo phiếu
        private readonly string hisCode;
        // v42244 - Mã nhập (IMP_MEST_CODE) - dùng làm TreatmentCode khi tạo tài liệu ngoài-điều-trị + dựng InputADO cho viewer
        private readonly string impMestCode;
        private readonly string loginName;
        // v42244 - Phòng hiện tại (dựng InputADO cho viewer ký số)
        private readonly long roomId;
        // v42244 - callback reload lưới phiếu nhập ở màn hình cha (gọi khi có thay đổi tài liệu)
        private readonly Action actRefeshParent;

        // Nguồn dữ liệu grid: tài liệu đã đính kèm của phiếu
        private List<EMR_DOCUMENT> documents = new List<EMR_DOCUMENT>();
        // Có thay đổi (thêm/sửa/xóa) trong phiên -> báo màn hình cha reload khi đóng
        private bool hasChange = false;

        #endregion

        #region Construct

        // v42244 - hisCode: khóa lọc/lưu theo phiếu; impMestCode: TreatmentCode tài liệu ngoài-điều-trị; roomId: cho viewer
        public frmImpMestAttachList(string hisCode, string impMestCode, string loginName, long roomId, Action actRefeshParent)
        {
            InitializeComponent();
            this.hisCode = hisCode;
            this.impMestCode = impMestCode;
            this.loginName = loginName;
            this.roomId = roomId;
            this.actRefeshParent = actRefeshParent;
            SetIcon();
        }

        #endregion

        #region Init

        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void frmImpMestAttachList_Load(object sender, EventArgs e)
        {
            try
            {
                Config.ConfigKey.GetConfigKey();
                SetTitle();
                SetupRowButtons();
                LoadDocumentList();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SetTitle()
        {
            try
            {
                // Tiêu đề: "Danh sách tài liệu đính kèm — Phiếu nhập: {IMP_MEST_CODE}"
                this.Text = string.IsNullOrEmpty(this.impMestCode)
                    ? "Danh sách tài liệu đính kèm"
                    : string.Format("Danh sách tài liệu đính kèm — Phiếu nhập: {0}", this.impMestCode);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        // v42244 - Gán caption cho 3 nút thao tác trên dòng (Xem/Sửa/Xóa) - nút dạng glyph có chữ
        private void SetupRowButtons()
        {
            try
            {
                SetButtonCaption(this.repoBtnView, "Xem");
                SetButtonCaption(this.repoBtnEdit, "Sửa");
                SetButtonCaption(this.repoBtnDelete, "Xóa");
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SetButtonCaption(DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repo, string caption)
        {
            repo.Buttons.Clear();
            DevExpress.XtraEditors.Controls.EditorButton btn = new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph);
            btn.Caption = caption;
            repo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { btn });
        }

        #endregion

        #region Load list

        // v42244 - Nạp danh sách tài liệu đã đính của phiếu (lọc theo HIS_CODE tổng hợp)
        private void LoadDocumentList()
        {
            CommonParam param = new CommonParam();
            try
            {
                if (!Config.ConfigKey.IsHasConnectionEmr || string.IsNullOrWhiteSpace(this.hisCode))
                {
                    BindGrid(new List<EMR_DOCUMENT>());
                    return;
                }

                WaitingManager.Show();
                EmrDocumentFilter filter = new EmrDocumentFilter();
                filter.HIS_CODE = this.hisCode;
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.ORDER_FIELD = "CREATE_TIME";
                filter.ORDER_DIRECTION = "DESC";

                var result = new BackendAdapter(param).Get<List<EMR_DOCUMENT>>(
                    "api/EmrDocument/Get", ApiConsumers.EmrConsumer, filter, param);
                WaitingManager.Hide();

                BindGrid(result ?? new List<EMR_DOCUMENT>());
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void BindGrid(List<EMR_DOCUMENT> data)
        {
            try
            {
                this.documents = data ?? new List<EMR_DOCUMENT>();
                gridViewDocList.BeginUpdate();
                gridControlDocList.DataSource = this.documents;
                gridViewDocList.EndUpdate();
                this.lblCount.Text = string.Format("{0} tài liệu", this.documents.Count);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridViewDocList_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (!e.IsGetData)
                    return;

                var source = ((BaseView)sender).DataSource as IList;
                if (source == null || e.ListSourceRowIndex < 0 || e.ListSourceRowIndex >= source.Count)
                    return;

                EMR_DOCUMENT data = source[e.ListSourceRowIndex] as EMR_DOCUMENT;
                if (data == null)
                    return;

                if (e.Column.FieldName == "STT")
                {
                    e.Value = e.ListSourceRowIndex + 1;
                }
                else if (e.Column.FieldName == "LOAI")
                {
                    e.Value = GetTypeDisplay(data);
                }
                else if (e.Column.FieldName == "CREATE_TIME_STR")
                {
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                }
                else if (e.Column.FieldName == "MODIFY_TIME_STR")
                {
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        // Loại hiển thị: mọi tài liệu đính kèm được gộp thành 1 PDF khi lưu -> mặc định "PDF";
        // nếu backend lưu DOCUMENT_FILE_TYPE là ảnh thì hiển thị "Ảnh".
        private string GetTypeDisplay(EMR_DOCUMENT data)
        {
            string fileType = (data.DOCUMENT_FILE_TYPE ?? "").ToLower();
            if (string.IsNullOrEmpty(fileType) || fileType.Contains("pdf"))
                return "PDF";
            return "Ảnh";
        }

        #endregion

        #region Row actions (Xem / Sửa / Xóa)

        private EMR_DOCUMENT GetFocusedDocument()
        {
            return gridViewDocList.GetFocusedRow() as EMR_DOCUMENT;
        }

        // v42244 - Xem: tải nội dung tài liệu (merged PDF) -> mở viewer toàn màn hình qua SignLibrary (như "Danh sách văn bản")
        private void repoBtnView_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                ViewDocument(GetFocusedDocument());
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        // v42244 - Sửa: mở form đính kèm chọn file thay thế -> lưu bản mới thành công -> xóa mềm bản cũ
        private void repoBtnEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                ReplaceDocument(GetFocusedDocument());
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        // v42244 - Xóa: xác nhận -> xóa mềm (IS_DELETE=1, IS_ACTIVE=0) -> refresh
        private void repoBtnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                DeleteDocument(GetFocusedDocument());
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void ViewDocument(EMR_DOCUMENT doc)
        {
            CommonParam param = new CommonParam();
            string tempFile = null;
            try
            {
                if (doc == null)
                    return;
                if (!Config.ConfigKey.IsHasConnectionEmr)
                    return;

                WaitingManager.Show();
                EMR.SDO.EmrDocumentDownloadFileSDO sdo = new EMR.SDO.EmrDocumentDownloadFileSDO();
                EmrDocumentViewFilter viewFilter = new EmrDocumentViewFilter();
                viewFilter.ID = doc.ID;
                sdo.EmrDocumentViewFilter = viewFilter;
                sdo.IsMerge = true;

                var files = new BackendAdapter(param).Post<List<EMR.SDO.EmrDocumentFileSDO>>(
                    "api/EmrDocument/DownloadFile", ApiConsumers.EmrConsumer, sdo, param);
                WaitingManager.Hide();

                var fileSdo = (files != null) ? files.FirstOrDefault(o => !string.IsNullOrEmpty(o.Base64Data)) : null;
                if (fileSdo == null)
                {
                    XtraMessageBox.Show(
                        Resources.ResourceMessage.KhongTaiDuocNoiDungTaiLieu,
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    return;
                }

                // Ghi nội dung ra file tạm rồi mở viewer toàn màn hình
                string ext = string.IsNullOrEmpty(fileSdo.Extension)
                    ? ".pdf"
                    : (fileSdo.Extension.StartsWith(".") ? fileSdo.Extension : "." + fileSdo.Extension);
                string tempDir = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, "temp");
                if (!Directory.Exists(tempDir))
                    Directory.CreateDirectory(tempDir);
                tempFile = System.IO.Path.Combine(tempDir, Guid.NewGuid().ToString() + ext);
                File.WriteAllBytes(tempFile, Convert.FromBase64String(fileSdo.Base64Data));

                string treatmentCode = !string.IsNullOrEmpty(doc.TREATMENT_CODE) ? doc.TREATMENT_CODE : this.impMestCode;
                Inventec.Common.SignLibrary.ADO.InputADO inputADO =
                    new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor()
                        .GenerateInputADO(treatmentCode, doc.DOCUMENT_CODE, doc.DOCUMENT_NAME, this.roomId);
                // EMR_DOCUMENT.IS_OUTSIDE_TREATMENT và InputADO.IsOutsideTreatment đều là short?
                inputADO.IsOutsideTreatment = doc.IS_OUTSIDE_TREATMENT;

                Inventec.Common.SignLibrary.SignLibraryGUIProcessor libraryProcessor = new Inventec.Common.SignLibrary.SignLibraryGUIProcessor();
                libraryProcessor.ShowPopup(tempFile, inputADO);

                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
            finally
            {
                try
                {
                    if (!string.IsNullOrEmpty(tempFile) && File.Exists(tempFile))
                        File.Delete(tempFile);
                }
                catch { }
            }
        }

        private void ReplaceDocument(EMR_DOCUMENT oldDoc)
        {
            try
            {
                if (oldDoc == null)
                    return;

                // Mở form đính kèm để chọn bản thay thế (không truyền callback - list tự refresh theo IsSaved)
                frmImpMestAttachFile frm = new frmImpMestAttachFile(this.hisCode, this.impMestCode, this.loginName, null);
                frm.ShowDialog();

                if (frm.IsSaved)
                {
                    // Lưu bản mới thành công -> xóa mềm bản cũ
                    bool deletedOld = SoftDeleteDocumentSilent(oldDoc.ID);
                    if (!deletedOld)
                    {
                        // Bản mới đã lưu nhưng bản cũ chưa xóa -> báo để user xóa thủ công (tránh trùng lặp âm thầm)
                        XtraMessageBox.Show(
                            Resources.ResourceMessage.DaLuuBanMoiNhungKhongXoaDuocBanCu,
                            HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    this.hasChange = true;
                    LoadDocumentList();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void DeleteDocument(EMR_DOCUMENT doc)
        {
            CommonParam param = new CommonParam();
            try
            {
                if (doc == null)
                    return;
                if (!Config.ConfigKey.IsHasConnectionEmr)
                    return;

                if (XtraMessageBox.Show(
                    HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong),
                    HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                WaitingManager.Show();
                var success = new BackendAdapter(param).Post<bool>(
                    EMR.URI.EmrDocument.DELETE, ApiConsumers.EmrConsumer, doc.ID, param);
                WaitingManager.Hide();

                MessageManager.Show(this, param, success);
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);

                if (success)
                {
                    this.hasChange = true;
                    LoadDocumentList();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        // Xóa mềm bản cũ khi thay thế - không hiện xác nhận (đã xác nhận bằng hành động lưu bản mới).
        // Trả về true nếu backend xóa thành công; caller sẽ cảnh báo user nếu false.
        private bool SoftDeleteDocumentSilent(long documentId)
        {
            CommonParam param = new CommonParam();
            try
            {
                WaitingManager.Show();
                var success = new BackendAdapter(param).Post<bool>(
                    EMR.URI.EmrDocument.DELETE, ApiConsumers.EmrConsumer, documentId, param);
                WaitingManager.Hide();
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                if (!success)
                    LogSystem.Warn("v42244 - Khong xoa mem duoc ban cu sau khi thay the. DocumentId=" + documentId);
                return success;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
                return false;
            }
        }

        #endregion

        #region Toolbar

        // v42244 - Đính kèm mới: mở form đính kèm với HIS_CODE của phiếu; lưu thành công -> refresh danh sách
        private void btnAttachNew_Click(object sender, EventArgs e)
        {
            try
            {
                frmImpMestAttachFile frm = new frmImpMestAttachFile(this.hisCode, this.impMestCode, this.loginName, null);
                frm.ShowDialog();
                if (frm.IsSaved)
                {
                    this.hasChange = true;
                    LoadDocumentList();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDocumentList();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            try
            {
                // Có thay đổi tài liệu -> báo màn hình cha reload lưới phiếu nhập
                if (this.hasChange && this.actRefeshParent != null)
                    this.actRefeshParent();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            base.OnFormClosed(e);
        }

        #endregion
    }
}
