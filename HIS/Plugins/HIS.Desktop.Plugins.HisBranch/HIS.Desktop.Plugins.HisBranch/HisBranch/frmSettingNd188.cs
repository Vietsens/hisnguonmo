
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utilities;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using ApiConsumers = HIS.Desktop.ApiConsumer.ApiConsumers;

namespace HIS.Desktop.Plugins.HisBranch.HisBranch
{
    public partial class frmSettingNd188 : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        // Ma cap benh vien luu vao HEIN_FACILITY_CLASS (theo thu tu item trong combo)
        private const string FACILITY_CLASS_BASIC = "BASIC";        // Co ban  (index 0)
        private const string FACILITY_CLASS_SPECIALIZED = "SPECIALIZED"; // Chuyen sau (index 1)
        private const string FACILITY_CLASS_INITIAL = "INITIAL";    // Ban dau (index 2)

        private readonly long branchId;
        private readonly string branchName;
        private readonly Action onSavedCallback;
        private MOS.EFMODEL.DataModels.HIS_BRANCH currentBranch;
        #endregion

        #region Construct
        /// <summary>
        /// Popup thiet lap muc huong ND 188 cho co so dang chon.
        /// </summary>
        /// <param name="branchId">ID co so (HIS_BRANCH.ID)</param>
        /// <param name="branchName">Ten co so (hien thi tren tieu de)</param>
        /// <param name="onSavedCallback">Callback goi sau khi luu thanh cong (de form cha refresh)</param>
        public frmSettingNd188(long branchId, string branchName, Action onSavedCallback)
            : base()
        {
            try
            {
                InitializeComponent();
                this.branchId = branchId;
                this.branchName = branchName;
                this.onSavedCallback = onSavedCallback;
                this.Load += new EventHandler(this.frmSettingNd188_Load);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Private method
        private void frmSettingNd188_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                SetCaptionByLanguageKey();
                if (!string.IsNullOrEmpty(branchName))
                {
                    this.Text = this.Text + " — " + branchName;
                }
                LoadData();
                spnClassifyPoint.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(
                    HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath,
                    System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager(
                    "HIS.Desktop.Plugins.HisBranch.Resources.Lang",
                    typeof(frmSettingNd188).Assembly);

                this.Text = GetLang("frmSettingNd188.Text", this.Text);
                this.lblClassifyPoint.Text = GetLang("frmSettingNd188.lblClassifyPoint.Text", this.lblClassifyPoint.Text);
                this.lblFacilityClass.Text = GetLang("frmSettingNd188.lblFacilityClass.Text", this.lblFacilityClass.Text);
                this.lblFormerLevel.Text = GetLang("frmSettingNd188.lblFormerLevel.Text", this.lblFormerLevel.Text);
                this.lblNote.Text = GetLang("frmSettingNd188.lblNote.Text", this.lblNote.Text);
                this.btnSave.Text = GetLang("frmSettingNd188.btnSave.Text", this.btnSave.Text);
                this.btnClose.Text = GetLang("frmSettingNd188.btnClose.Text", this.btnClose.Text);

                // Nap lai item cap benh vien theo ngon ngu (giu nguyen thu tu index)
                cboFacilityClass.Properties.Items.Clear();
                cboFacilityClass.Properties.Items.Add(GetLang("frmSettingNd188.cboFacilityClass.Basic", "Cơ bản"));
                cboFacilityClass.Properties.Items.Add(GetLang("frmSettingNd188.cboFacilityClass.Specialized", "Chuyên sâu"));
                cboFacilityClass.Properties.Items.Add(GetLang("frmSettingNd188.cboFacilityClass.Initial", "Ban đầu"));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Lay text theo ngon ngu, neu rong thi giu gia tri mac dinh (Designer).</summary>
        private string GetLang(string key, string defaultValue)
        {
            try
            {
                string value = Inventec.Common.Resource.Get.Value(
                    key,
                    Resources.ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture());
                return string.IsNullOrEmpty(value) ? defaultValue : value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return defaultValue;
        }

        /// <summary>Nap du lieu tu HIS_BRANCH cua co so dang chon.</summary>
        private void LoadData()
        {
            CommonParam param = new CommonParam();
            try
            {
                HisBranchFilter filter = new HisBranchFilter();
                filter.ID = branchId;
                var list = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_BRANCH>>(
                    HisRequestUriStore.MOSHIS_BRANCH_GET, ApiConsumers.MosConsumer, filter, param);
                currentBranch = (list != null && list.Count > 0) ? list[0] : null;

                if (currentBranch != null)
                {
                    spnClassifyPoint.EditValue = currentBranch.BHYT_CLASSIFY_POINT;
                    cboFacilityClass.SelectedIndex = GetFacilityClassIndex(currentBranch.HEIN_FACILITY_CLASS);
                    spnFormerLevel.EditValue = ParseFormerLevel(currentBranch.HEIN_FORMER_LEVEL_CODE);
                }
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Ma cap benh vien -> index item combo.</summary>
        private int GetFacilityClassIndex(string facilityClass)
        {
            if (string.IsNullOrEmpty(facilityClass)) return -1;
            switch (facilityClass.Trim().ToUpper())
            {
                case FACILITY_CLASS_BASIC: return 0;
                case FACILITY_CLASS_SPECIALIZED: return 1;
                case FACILITY_CLASS_INITIAL: return 2;
                default: return -1;
            }
        }

        /// <summary>Index item combo -> ma cap benh vien luu DB.</summary>
        private string GetFacilityClassCode(int index)
        {
            switch (index)
            {
                case 0: return FACILITY_CLASS_BASIC;
                case 1: return FACILITY_CLASS_SPECIALIZED;
                case 2: return FACILITY_CLASS_INITIAL;
                default: return null;
            }
        }

        /// <summary>HEIN_FORMER_LEVEL_CODE (string) -> gia tri so cho SpinEdit (null neu rong).</summary>
        private object ParseFormerLevel(string code)
        {
            int value;
            if (!string.IsNullOrEmpty(code) && int.TryParse(code.Trim(), out value))
            {
                return value;
            }
            return null;
        }

        /// <summary>Kiem tra hop le: so diem phan loai >= 0 (neu co nhap).</summary>
        private bool ValidateData()
        {
            try
            {
                if (spnClassifyPoint.EditValue != null)
                {
                    decimal point = Convert.ToDecimal(spnClassifyPoint.EditValue);
                    if (point < 0)
                    {
                        ShowWarning();
                        spnClassifyPoint.Focus();
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        private void ShowWarning()
        {
            try
            {
                XtraMessageBox.Show(
                    MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.NguoiDungNhapDuLieuKhongHopLe),
                    MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Button handler
        private void btnSave_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                if (!ValidateData()) return;

                if (currentBranch == null)
                {
                    LoadData();
                    if (currentBranch == null)
                    {
                        XtraMessageBox.Show(
                            MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.ThongBaoDuLieuTrong),
                            MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaLoi),
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                currentBranch.BHYT_CLASSIFY_POINT = (spnClassifyPoint.EditValue == null)
                    ? (decimal?)null : Convert.ToDecimal(spnClassifyPoint.EditValue);
                currentBranch.HEIN_FACILITY_CLASS = GetFacilityClassCode(cboFacilityClass.SelectedIndex);
                currentBranch.HEIN_FORMER_LEVEL_CODE = (spnFormerLevel.EditValue == null)
                    ? null : Convert.ToInt32(spnFormerLevel.EditValue).ToString();

                WaitingManager.Show();
                HisBranchSDO sdo = new HisBranchSDO();
                sdo.Branch = currentBranch;
                var result = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_BRANCH>(
                    HisRequestUriStore.MOSHIS_BRANCH_UPDATE, ApiConsumers.MosConsumer, sdo, param);
                WaitingManager.Hide();

                bool success = result != null;
                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);

                if (success)
                {
                    currentBranch = result;
                    BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_BRANCH>();
                    if (onSavedCallback != null) onSavedCallback();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
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
        #endregion

        #region Windows Form Designer generated code
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblClassifyPoint = new DevExpress.XtraEditors.LabelControl();
            this.spnClassifyPoint = new DevExpress.XtraEditors.SpinEdit();
            this.lblFacilityClass = new DevExpress.XtraEditors.LabelControl();
            this.cboFacilityClass = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblFormerLevel = new DevExpress.XtraEditors.LabelControl();
            this.spnFormerLevel = new DevExpress.XtraEditors.SpinEdit();
            this.lblNote = new DevExpress.XtraEditors.LabelControl();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.spnClassifyPoint.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboFacilityClass.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnFormerLevel.Properties)).BeginInit();
            this.SuspendLayout();
            //
            // lblClassifyPoint
            //
            this.lblClassifyPoint.Appearance.Options.UseTextOptions = true;
            this.lblClassifyPoint.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblClassifyPoint.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblClassifyPoint.Location = new System.Drawing.Point(12, 22);
            this.lblClassifyPoint.Name = "lblClassifyPoint";
            this.lblClassifyPoint.Size = new System.Drawing.Size(224, 13);
            this.lblClassifyPoint.TabIndex = 0;
            this.lblClassifyPoint.Text = "Số điểm phân loại:";
            //
            // spnClassifyPoint
            //
            this.spnClassifyPoint.EditValue = null;
            this.spnClassifyPoint.Location = new System.Drawing.Point(240, 18);
            this.spnClassifyPoint.Name = "spnClassifyPoint";
            this.spnClassifyPoint.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.spnClassifyPoint.Properties.Appearance.Options.UseTextOptions = true;
            this.spnClassifyPoint.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.spnClassifyPoint.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spnClassifyPoint.Properties.IsFloatValue = false;
            this.spnClassifyPoint.Properties.MaxValue = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.spnClassifyPoint.Properties.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spnClassifyPoint.Size = new System.Drawing.Size(180, 20);
            this.spnClassifyPoint.TabIndex = 1;
            //
            // lblFacilityClass
            //
            this.lblFacilityClass.Appearance.Options.UseTextOptions = true;
            this.lblFacilityClass.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblFacilityClass.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblFacilityClass.Location = new System.Drawing.Point(12, 53);
            this.lblFacilityClass.Name = "lblFacilityClass";
            this.lblFacilityClass.Size = new System.Drawing.Size(224, 13);
            this.lblFacilityClass.TabIndex = 2;
            this.lblFacilityClass.Text = "Cấp bệnh viện:";
            //
            // cboFacilityClass
            //
            this.cboFacilityClass.Location = new System.Drawing.Point(240, 49);
            this.cboFacilityClass.Name = "cboFacilityClass";
            this.cboFacilityClass.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboFacilityClass.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cboFacilityClass.Properties.Items.AddRange(new object[] {
            "Cơ bản",
            "Chuyên sâu",
            "Ban đầu"});
            this.cboFacilityClass.Size = new System.Drawing.Size(180, 20);
            this.cboFacilityClass.TabIndex = 3;
            //
            // lblFormerLevel
            //
            this.lblFormerLevel.Appearance.Options.UseTextOptions = true;
            this.lblFormerLevel.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblFormerLevel.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblFormerLevel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblFormerLevel.Location = new System.Drawing.Point(12, 80);
            this.lblFormerLevel.Name = "lblFormerLevel";
            this.lblFormerLevel.Size = new System.Drawing.Size(224, 28);
            this.lblFormerLevel.TabIndex = 4;
            this.lblFormerLevel.Text = "Tuyến bệnh viện trước ngày 01/01/2025:";
            //
            // spnFormerLevel
            //
            this.spnFormerLevel.EditValue = null;
            this.spnFormerLevel.Location = new System.Drawing.Point(240, 83);
            this.spnFormerLevel.Name = "spnFormerLevel";
            this.spnFormerLevel.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.spnFormerLevel.Properties.Appearance.Options.UseTextOptions = true;
            this.spnFormerLevel.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.spnFormerLevel.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spnFormerLevel.Properties.IsFloatValue = false;
            this.spnFormerLevel.Properties.MaxValue = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.spnFormerLevel.Properties.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spnFormerLevel.Size = new System.Drawing.Size(180, 20);
            this.spnFormerLevel.TabIndex = 5;
            //
            // lblNote
            //
            this.lblNote.Appearance.ForeColor = System.Drawing.Color.Gray;
            this.lblNote.Appearance.Options.UseForeColor = true;
            this.lblNote.Location = new System.Drawing.Point(240, 110);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(250, 13);
            this.lblNote.TabIndex = 6;
            this.lblNote.Text = "(1: Trung ương; 2: Tỉnh; 3: Huyện; 4: Xã)";
            //
            // btnSave
            //
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnSave.Appearance.ForeColor = System.Drawing.Color.Black;
            this.btnSave.Appearance.Options.UseBackColor = true;
            this.btnSave.Appearance.Options.UseForeColor = true;
            this.btnSave.Location = new System.Drawing.Point(336, 144);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(90, 30);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Lưu";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnClose
            //
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(432, 144);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(90, 30);
            this.btnClose.TabIndex = 8;
            this.btnClose.Text = "Đóng";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // frmSettingNd188
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 188);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblNote);
            this.Controls.Add(this.spnFormerLevel);
            this.Controls.Add(this.lblFormerLevel);
            this.Controls.Add(this.cboFacilityClass);
            this.Controls.Add(this.lblFacilityClass);
            this.Controls.Add(this.spnClassifyPoint);
            this.Controls.Add(this.lblClassifyPoint);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSettingNd188";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "⚙ Thiết lập mức hưởng ngoại trú trái tuyến (NĐ 188)";
            ((System.ComponentModel.ISupportInitialize)(this.spnClassifyPoint.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboFacilityClass.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnFormerLevel.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private DevExpress.XtraEditors.LabelControl lblClassifyPoint;
        private DevExpress.XtraEditors.SpinEdit spnClassifyPoint;
        private DevExpress.XtraEditors.LabelControl lblFacilityClass;
        private DevExpress.XtraEditors.ComboBoxEdit cboFacilityClass;
        private DevExpress.XtraEditors.LabelControl lblFormerLevel;
        private DevExpress.XtraEditors.SpinEdit spnFormerLevel;
        private DevExpress.XtraEditors.LabelControl lblNote;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        #endregion
    }
}
