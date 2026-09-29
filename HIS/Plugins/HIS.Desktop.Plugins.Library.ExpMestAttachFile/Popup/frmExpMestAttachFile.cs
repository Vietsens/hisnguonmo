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
using HIS.Desktop.Common;
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
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Library.ExpMestAttachFile.Popup
{
    /// <summary>
    /// v57853 - Choose files / capture photos of the prescription.
    /// Pending mode (ticket not saved yet): returns the files to the caller, nothing sent to EMR.
    /// Direct mode (saved ticket): merges the files into one PDF and creates the EMR document right away.
    /// </summary>
    public partial class frmExpMestAttachFile : FormBase
    {
        #region Declare

        private const string CAMERA_MODULE_LINK = "HIS.Desktop.Plugins.Camera";
        private const string IMAGE__OPEN = "images/actions/open_16x16.png";
        private const string IMAGE__CAPTURE = "images/content/image_16x16.png";

        /// <summary>null = pending mode</summary>
        private readonly ExpMestAttachInfoADO expMest;
        private readonly List<AttachFileADO> files = new List<AttachFileADO>();
        /// <summary>Files that existed before opening (pending mode) -> must not be disposed on cancel</summary>
        private readonly HashSet<AttachFileADO> originalFiles = new HashSet<AttachFileADO>();
        private readonly string originalDocumentName;
        private bool isConfirmed = false;

        /// <summary>Pending mode result (valid when DialogResult = OK)</summary>
        public PendingAttachADO PendingResult { get; private set; }

        /// <summary>Direct mode: true when the EMR document was created</summary>
        public bool IsSaved { get; private set; }

        private bool IsPendingMode
        {
            get { return this.expMest == null; }
        }

        #endregion

        #region Construct

        /// <summary>Pending mode — edit the files chosen before the ticket is saved</summary>
        public frmExpMestAttachFile(PendingAttachADO current)
        {
            InitializeComponent();
            if (current != null && current.Files != null)
            {
                this.files.AddRange(current.Files);
                foreach (var file in current.Files)
                    this.originalFiles.Add(file);
                this.originalDocumentName = current.DocumentName;
            }
            SetIcon();
        }

        /// <summary>Direct mode — attach new files to a saved ticket</summary>
        public frmExpMestAttachFile(ExpMestAttachInfoADO expMest)
        {
            InitializeComponent();
            this.expMest = expMest;
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

        private void frmExpMestAttachFile_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                SetButtonImages();
                this.txtDocumentName.Text = !String.IsNullOrWhiteSpace(this.originalDocumentName)
                    ? this.originalDocumentName
                    : Resources.ResourceLanguageManager.GetValue("frmExpMestAttachFile.DefaultDocumentName", "Đơn thuốc ngoại viện");
                BindGrid();
                this.btnChooseFile.Focus();
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
                this.Text = Resources.ResourceLanguageManager.GetValue("frmExpMestAttachFile.Text", this.Text);
                if (!this.IsPendingMode)
                    this.Text += " — " + this.expMest.EXP_MEST_CODE;
                this.lciDocumentName.Text = Resources.ResourceLanguageManager.GetValue("frmExpMestAttachFile.lciDocumentName.Text", this.lciDocumentName.Text);
                this.btnChooseFile.Text = Resources.ResourceLanguageManager.GetValue("frmExpMestAttachFile.btnChooseFile.Text", this.btnChooseFile.Text);
                this.btnCapture.Text = Resources.ResourceLanguageManager.GetValue("frmExpMestAttachFile.btnCapture.Text", this.btnCapture.Text);
                this.btnRotateLeft.ToolTip = Resources.ResourceLanguageManager.GetValue("frmExpMestAttachFile.btnRotateLeft.ToolTip", "");
                this.btnRotateRight.ToolTip = Resources.ResourceLanguageManager.GetValue("frmExpMestAttachFile.btnRotateRight.ToolTip", "");
                this.btnOk.Text = this.IsPendingMode
                    ? Resources.ResourceLanguageManager.GetValue("frmExpMestAttachFile.btnOk.Text.Buffer", this.btnOk.Text)
                    : Resources.ResourceLanguageManager.GetValue("frmExpMestAttachFile.btnOk.Text.Save", "Lưu (Ctrl S)");
                this.btnClose.Text = Resources.ResourceLanguageManager.GetValue("frmExpMestAttachFile.btnClose.Text", this.btnClose.Text);
                this.gcStt.Caption = Resources.ResourceLanguageManager.GetValue("frmExpMestAttachFile.gcStt.Caption", this.gcStt.Caption);
                this.gcFileName.Caption = Resources.ResourceLanguageManager.GetValue("frmExpMestAttachFile.gcFileName.Caption", this.gcFileName.Caption);
                this.gcDelete.ToolTip = Resources.ResourceLanguageManager.GetValue("frmExpMestAttachFile.gcDelete.ToolTip", "");
                this.lblHint.Text = String.Format(
                    Resources.ResourceLanguageManager.GetValue("frmExpMestAttachFile.lblHint.Text", "{0}"),
                    AttachFileConfig.MaxFileSizeMB);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetButtonImages()
        {
            try
            {
                this.btnChooseFile.Image = DevExpress.Images.ImageResourceCache.Default.GetImage(IMAGE__OPEN);
                this.btnCapture.Image = DevExpress.Images.ImageResourceCache.Default.GetImage(IMAGE__CAPTURE);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Files

        private void btnChooseFile_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFile = new OpenFileDialog())
                {
                    openFile.Multiselect = true;
                    openFile.Filter = AttachFileConfig.OPEN_FILE_FILTER;
                    if (openFile.ShowDialog() != DialogResult.OK)
                        return;

                    List<string> errors = new List<string>();
                    var loaded = AttachFileValidator.LoadFiles(openFile.FileNames, errors);
                    AddFiles(loaded);
                    ShowErrors(errors);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCapture_Click(object sender, EventArgs e)
        {
            try
            {
                var moduleData = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.currentModuleRaws != null
                    ? HIS.Desktop.LocalStorage.LocalData.GlobalVariables.currentModuleRaws.FirstOrDefault(o => o.ModuleLink == CAMERA_MODULE_LINK)
                    : null;
                if (moduleData == null || !moduleData.IsPlugin || moduleData.ExtensionInfo == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Khong tim thay moduleLink = " + CAMERA_MODULE_LINK);
                    XtraMessageBox.Show(Resources.ResourceMessage.KhongTimThayModuleCamera,
                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    return;
                }

                List<object> listArgs = new List<object>();
                listArgs.Add((DelegateSelectData)ReceiveCaptureImage);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(PluginInstance.GetModuleWithWorkingRoom(moduleData, 0, 0), listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Callback of the camera module: one call per captured photo</summary>
        private void ReceiveCaptureImage(object data)
        {
            try
            {
                System.Drawing.Image image = data as System.Drawing.Image;
                if (image == null) return;

                int order = this.files.Count(o => o.IsCapture) + 1;
                string fileName = String.Format(Resources.ResourceLanguageManager.GetValue("frmExpMestAttachFile.CaptureFileName", "Ảnh chụp {0}.jpg"), order);
                long size;
                string error = AttachFileValidator.ValidateCapture(image, fileName, out size);
                if (!String.IsNullOrEmpty(error))
                {
                    ShowErrors(new List<string> { error });
                    return;
                }

                AttachFileADO ado = new AttachFileADO();
                ado.FileName = fileName;
                ado.Image = new System.Drawing.Bitmap(image);
                ado.FileSize = size;
                ado.IsCapture = true;
                AddFiles(new List<AttachFileADO> { ado });
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AddFiles(List<AttachFileADO> newFiles)
        {
            if (newFiles == null || newFiles.Count == 0) return;
            this.files.AddRange(newFiles);
            BindGrid();
            this.gridViewFiles.FocusedRowHandle = this.files.Count - 1;
            ShowPreview(this.files.LastOrDefault());
        }

        private void ShowErrors(List<string> errors)
        {
            if (errors == null || errors.Count == 0) return;
            XtraMessageBox.Show(String.Join(Environment.NewLine, errors),
                MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void BindGrid()
        {
            this.gridViewFiles.BeginUpdate();
            try
            {
                this.gridControlFiles.DataSource = null;
                this.gridControlFiles.DataSource = this.files.ToList();
            }
            finally
            {
                this.gridViewFiles.EndUpdate();
            }
            if (this.files.Count == 0)
                ShowPreview(null);
        }

        private void repoBtnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                AttachFileADO row = this.gridViewFiles.GetFocusedRow() as AttachFileADO;
                if (row == null) return;
                // Pending files are not saved anywhere yet -> delete without restriction
                this.files.Remove(row);
                if (!this.originalFiles.Contains(row) && row.Image != null)
                    row.Image.Dispose();
                BindGrid();
                ShowPreview(this.gridViewFiles.GetFocusedRow() as AttachFileADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewFiles_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.FieldName == "STT")
                    e.Value = e.ListSourceRowIndex + 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewFiles_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                ShowPreview(this.gridViewFiles.GetFocusedRow() as AttachFileADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ShowPreview(AttachFileADO file)
        {
            try
            {
                bool isPdf = file != null && file.IsPdf;
                this.pdfViewerPreview.Visible = isPdf;
                this.picPreview.Visible = !isPdf;
                this.btnRotateLeft.Enabled = file != null && !isPdf;
                this.btnRotateRight.Enabled = file != null && !isPdf;
                if (isPdf)
                {
                    this.pdfViewerPreview.LoadDocument(file.PdfPath);
                }
                else
                {
                    this.picPreview.Image = file != null ? file.Image : null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRotateLeft_Click(object sender, EventArgs e)
        {
            Rotate(System.Drawing.RotateFlipType.Rotate270FlipNone);
        }

        private void btnRotateRight_Click(object sender, EventArgs e)
        {
            Rotate(System.Drawing.RotateFlipType.Rotate90FlipNone);
        }

        private void Rotate(System.Drawing.RotateFlipType type)
        {
            try
            {
                AttachFileADO row = this.gridViewFiles.GetFocusedRow() as AttachFileADO;
                if (row == null || row.Image == null) return;
                row.Image.RotateFlip(type);
                this.picPreview.Image = null;
                this.picPreview.Image = row.Image;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Save / Close

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.btnOk.Enabled) return;
                this.btnOk.Enabled = false;
                try
                {
                    if (this.IsPendingMode)
                        ConfirmPending();
                    else
                        SaveDirect();
                }
                finally
                {
                    this.btnOk.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Pending mode: an empty list is allowed (user removed every file = no attachment)</summary>
        private void ConfirmPending()
        {
            this.PendingResult = new PendingAttachADO();
            this.PendingResult.DocumentName = GetDocumentName();
            this.PendingResult.Files = this.files.ToList();
            this.isConfirmed = true;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void SaveDirect()
        {
            if (this.files.Count == 0)
            {
                XtraMessageBox.Show(Resources.ResourceMessage.ChuaChonTepDinhKem,
                    MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                return;
            }
            if (!AttachDocumentWorker.GetDocumentTypeId().HasValue)
            {
                XtraMessageBox.Show(String.Format(Resources.ResourceMessage.KhongTimThayLoaiVanBan, AttachFileConfig.DOCUMENT_TYPE_CODE),
                    MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                return;
            }

            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                WaitingManager.Show();
                byte[] pdf = PdfMergeUtil.MergeToPdf(this.files);
                var created = pdf != null ? AttachDocumentWorker.CreateDocument(this.expMest, pdf, GetDocumentName(), param) : null;
                success = created != null;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            MessageManager.Show(this, param, success);
            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
            if (success)
            {
                ExpMestAttachFileProcessor.WriteAuditLog("AttachPrescription", this.expMest.EXP_MEST_CODE, this.files.Count);
                this.IsSaved = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private string GetDocumentName()
        {
            string name = (this.txtDocumentName.Text ?? "").Trim();
            return !String.IsNullOrEmpty(name)
                ? name
                : Resources.ResourceLanguageManager.GetValue("frmExpMestAttachFile.DefaultDocumentName", "Đơn thuốc ngoại viện");
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
                if (keyData == (Keys.Control | Keys.S))
                {
                    btnOk_Click(null, null);
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

        /// <summary>
        /// FormBase.OnFormClosing disposes every control -> detach the preview image FIRST
        /// so the images handed back to the caller (pending mode) are not disposed with the PictureEdit.
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                this.picPreview.Image = null;
                this.pdfViewerPreview.CloseDocument();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            base.OnFormClosing(e);
        }

        private void frmExpMestAttachFile_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (this.IsPendingMode && this.isConfirmed)
                {
                    // Caller owns the kept files now; originals removed by the user are no longer referenced
                    foreach (var file in this.originalFiles)
                    {
                        if (!this.files.Contains(file) && file.Image != null)
                            file.Image.Dispose();
                    }
                    return;
                }
                // Cancelled / direct mode: dispose only what this form created, the caller keeps its originals
                foreach (var file in this.files)
                {
                    if (!this.originalFiles.Contains(file) && file.Image != null)
                        file.Image.Dispose();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
    }
}
