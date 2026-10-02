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
using EMR.EFMODEL.DataModels;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Plugins.Library.ExpMestAttachFile.ADO;
using HIS.Desktop.Plugins.Library.ExpMestAttachFile.Base;
using HIS.Desktop.Plugins.Library.ExpMestAttachFile.Config;
using HIS.Desktop.Utility;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Library.ExpMestAttachFile.Popup
{
    /// <summary>
    /// v57853 - Attached prescriptions of one sale export ticket:
    /// view/print (any status), attach new (any status), delete (only when the ticket is not finished/paid — requirement #6).
    /// Pattern of frmImpMestAttachList (v42244).
    /// </summary>
    public partial class frmExpMestAttachList : FormBase
    {
        #region Declare

        private const string IMAGE__VIEW = "images/print/preview_16x16.png";
        private const string IMAGE__DELETE = "images/edit/delete_16x16.png";

        private readonly ExpMestAttachInfoADO expMest;
        private readonly long roomId;
        private readonly bool isAllowDelete;
        private List<V_EMR_DOCUMENT> documents = new List<V_EMR_DOCUMENT>();

        /// <summary>True when a document was added/deleted -> caller refreshes its marks</summary>
        public bool HasChange { get; private set; }

        #endregion

        #region Construct

        public frmExpMestAttachList(ExpMestAttachInfoADO expMest, long roomId)
        {
            InitializeComponent();
            this.expMest = expMest;
            this.roomId = roomId;
            this.isAllowDelete = ExpMestAttachFileProcessor.IsAllowDelete(expMest);
            SetIcon();
        }

        #endregion

        #region Load

        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmExpMestAttachList_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                SetRowButtons();
                this.lciReadOnly.Visibility = this.isAllowDelete
                    ? DevExpress.XtraLayout.Utils.LayoutVisibility.Never
                    : DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                LoadDocuments();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                this.Text = String.Format(Resources.ResourceLanguageManager.GetValue("frmExpMestAttachList.Text", "{0}"), this.expMest.EXP_MEST_CODE);
                this.btnAttachNew.Text = Resources.ResourceLanguageManager.GetValue("frmExpMestAttachList.btnAttachNew.Text", this.btnAttachNew.Text);
                this.btnRefresh.Text = Resources.ResourceLanguageManager.GetValue("frmExpMestAttachList.btnRefresh.Text", this.btnRefresh.Text);
                this.btnClose.Text = Resources.ResourceLanguageManager.GetValue("frmExpMestAttachList.btnClose.Text", this.btnClose.Text);
                this.lblReadOnly.Text = Resources.ResourceLanguageManager.GetValue("frmExpMestAttachList.lblReadOnly.Text", this.lblReadOnly.Text);
                this.gcStt.Caption = Resources.ResourceLanguageManager.GetValue("frmExpMestAttachList.gcStt.Caption", this.gcStt.Caption);
                this.gcDocumentName.Caption = Resources.ResourceLanguageManager.GetValue("frmExpMestAttachList.gcDocumentName.Caption", this.gcDocumentName.Caption);
                this.gcCreateTime.Caption = Resources.ResourceLanguageManager.GetValue("frmExpMestAttachList.gcCreateTime.Caption", this.gcCreateTime.Caption);
                this.gcCreator.Caption = Resources.ResourceLanguageManager.GetValue("frmExpMestAttachList.gcCreator.Caption", this.gcCreator.Caption);
                this.gcModifyTime.Caption = Resources.ResourceLanguageManager.GetValue("frmExpMestAttachList.gcModifyTime.Caption", this.gcModifyTime.Caption);
                this.gcModifier.Caption = Resources.ResourceLanguageManager.GetValue("frmExpMestAttachList.gcModifier.Caption", this.gcModifier.Caption);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Icons from the DevExpress gallery; the "disable" delete button is greyed and only explains why</summary>
        private void SetRowButtons()
        {
            try
            {
                SetButton(this.repoBtnView, IMAGE__VIEW, Resources.ResourceLanguageManager.GetValue("frmExpMestAttachList.gcView.ToolTip", "Xem / In"), true);
                string deleteTip = Resources.ResourceLanguageManager.GetValue("frmExpMestAttachList.gcDelete.ToolTip", "Xóa");
                SetButton(this.repoBtnDelete, IMAGE__DELETE, deleteTip, true);
                SetButton(this.repoBtnDeleteDisable, IMAGE__DELETE, Resources.ResourceMessage.PhieuDaHoanTatKhongDuocXoa, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetButton(DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repo, string imagePath, string toolTip, bool enabled)
        {
            DevExpress.XtraEditors.Controls.EditorButton btn = repo.Buttons[0];
            try
            {
                System.Drawing.Image image = DevExpress.Images.ImageResourceCache.Default.GetImage(imagePath);
                btn.Image = (image != null && !enabled) ? ToolStripRenderer.CreateDisabledImage(image) : image;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            // Missing gallery image -> show text so the button stays usable
            if (btn.Image == null)
                btn.Caption = toolTip;
            btn.ToolTip = toolTip;
        }

        #endregion

        #region Grid

        private void LoadDocuments()
        {
            CommonParam param = new CommonParam();
            try
            {
                WaitingManager.Show();
                this.documents = AttachDocumentWorker.GetDocuments(this.expMest, param);
                WaitingManager.Hide();
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);

                this.gridViewDocument.BeginUpdate();
                try
                {
                    this.gridControlDocument.DataSource = this.documents;
                }
                finally
                {
                    this.gridViewDocument.EndUpdate();
                }
                this.lblCount.Text = String.Format(
                    Resources.ResourceLanguageManager.GetValue("frmExpMestAttachList.lblCount.Text", "{0}"),
                    this.documents.Count);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewDocument_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (!e.IsGetData) return;
                IList source = ((DevExpress.XtraGrid.Views.Base.BaseView)sender).DataSource as IList;
                if (source == null || e.ListSourceRowIndex < 0 || e.ListSourceRowIndex >= source.Count) return;
                V_EMR_DOCUMENT data = source[e.ListSourceRowIndex] as V_EMR_DOCUMENT;
                if (data == null) return;

                if (e.Column.FieldName == "STT")
                    e.Value = e.ListSourceRowIndex + 1;
                else if (e.Column.FieldName == "CREATE_TIME_STR")
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewDocument_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0 && e.Column.FieldName == "DELETE")
                    e.RepositoryItem = this.isAllowDelete ? this.repoBtnDelete : this.repoBtnDeleteDisable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewDocument_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                ViewDocument(this.gridViewDocument.GetFocusedRow() as V_EMR_DOCUMENT);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repoBtnView_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                ViewDocument(this.gridViewDocument.GetFocusedRow() as V_EMR_DOCUMENT);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repoBtnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                DeleteDocument(this.gridViewDocument.GetFocusedRow() as V_EMR_DOCUMENT);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Scenario 4: finished/paid ticket -> explain why the evidence cannot be deleted</summary>
        private void repoBtnDeleteDisable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                XtraMessageBox.Show(Resources.ResourceMessage.PhieuDaHoanTatKhongDuocXoa,
                    MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Actions

        /// <summary>Download the merged PDF -> temp file -> full-screen viewer (zoom, print) like v42244</summary>
        private void ViewDocument(V_EMR_DOCUMENT doc)
        {
            CommonParam param = new CommonParam();
            string tempFile = null;
            try
            {
                if (doc == null) return;
                WaitingManager.Show();
                var fileSdo = AttachDocumentWorker.DownloadDocument(doc.ID, param);
                WaitingManager.Hide();
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                if (fileSdo == null)
                {
                    XtraMessageBox.Show(Resources.ResourceMessage.KhongTaiDuocNoiDungTaiLieu,
                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string ext = String.IsNullOrEmpty(fileSdo.Extension)
                    ? ".pdf"
                    : (fileSdo.Extension.StartsWith(".") ? fileSdo.Extension : "." + fileSdo.Extension);
                string tempDir = Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, "temp");
                if (!Directory.Exists(tempDir))
                    Directory.CreateDirectory(tempDir);
                tempFile = Path.Combine(tempDir, Guid.NewGuid().ToString() + ext);
                File.WriteAllBytes(tempFile, Convert.FromBase64String(fileSdo.Base64Data));

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor()
                    .GenerateInputADO(doc.TREATMENT_CODE ?? this.expMest.EXP_MEST_CODE, doc.DOCUMENT_CODE, doc.DOCUMENT_NAME, this.roomId);
                inputADO.IsOutsideTreatment = doc.IS_OUTSIDE_TREATMENT;
                new Inventec.Common.SignLibrary.SignLibraryGUIProcessor().ShowPopup(tempFile, inputADO);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                DeleteTempFile(tempFile);
            }
        }

        private void DeleteTempFile(string tempFile)
        {
            try
            {
                if (!String.IsNullOrEmpty(tempFile) && File.Exists(tempFile))
                    File.Delete(tempFile);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DeleteDocument(V_EMR_DOCUMENT doc)
        {
            CommonParam param = new CommonParam();
            try
            {
                if (doc == null) return;
                // Double check on action (the grid button may be stale if the ticket status changed)
                if (!this.isAllowDelete)
                {
                    repoBtnDeleteDisable_ButtonClick(null, null);
                    return;
                }
                if (XtraMessageBox.Show(
                    MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong),
                    MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                WaitingManager.Show();
                bool success = AttachDocumentWorker.DeleteDocument(doc.ID, param);
                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                if (success)
                {
                    ExpMestAttachFileProcessor.WriteAuditLog("DeletePrescriptionAttach", this.expMest.EXP_MEST_CODE, doc.DOCUMENT_CODE);
                    this.HasChange = true;
                    LoadDocuments();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAttachNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (!AttachDocumentWorker.GetDocumentTypeId().HasValue)
                {
                    XtraMessageBox.Show(String.Format(Resources.ResourceMessage.KhongTimThayLoaiVanBan, AttachFileConfig.DOCUMENT_TYPE_CODE),
                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    return;
                }
                using (frmExpMestAttachFile frm = new frmExpMestAttachFile(this.expMest))
                {
                    frm.ShowDialog();
                    if (frm.IsSaved)
                    {
                        this.HasChange = true;
                        LoadDocuments();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDocuments();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {
            try
            {
                if (keyData == (Keys.Control | Keys.N))
                {
                    btnAttachNew_Click(null, null);
                    return true;
                }
                if (keyData == Keys.F5)
                {
                    btnRefresh_Click(null, null);
                    return true;
                }
                if (keyData == Keys.Escape)
                {
                    this.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion
    }
}
