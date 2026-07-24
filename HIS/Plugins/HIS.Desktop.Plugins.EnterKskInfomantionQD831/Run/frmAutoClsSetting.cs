/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Form cấu hình dịch vụ cho "Tự động lấy kết quả CLS" (mở từ nút setting cạnh checkbox).
 * UI thiết kế trong Designer (LayoutControl + 4 GridLookUpEdit + nút Lưu). File này chỉ chứa logic:
 * đổ datasource V_HIS_SERVICE theo loại (XN cho Huyết học/Sinh hóa máu/Sinh hóa nước tiểu,
 * CĐHA cho Siêu âm ổ bụng), cho chọn NHIỀU (GridCheckMarksSelection), LƯU/HIỂN LẠI qua ControlState.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Library.CacheClient;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.EnterKskInfomantionQD831.Run
{
    public partial class frmAutoClsSetting : DevExpress.XtraEditors.XtraForm
    {
        internal const string MODULE_LINK = "HIS.Desktop.Plugins.EnterKskInfomantionQD831";
        // Key ControlState ứng với 4 memo tab Khám cận lâm sàng (dùng chung với hàm auto-get bên form chính).
        internal const string KEY_HUYET_HOC = "AutoCls_HuyetHoc";
        internal const string KEY_SINH_HOA_MAU = "AutoCls_SinhHoaMau";
        internal const string KEY_SINH_HOA_NUOC_TIEU = "AutoCls_SinhHoaNuocTieu";
        internal const string KEY_SIEU_AM_OB = "AutoCls_SieuAmOB";

        // Danh sách dịch vụ đang tick theo từng combo.
        private readonly Dictionary<GridLookUpEdit, List<V_HIS_SERVICE>> selecteds = new Dictionary<GridLookUpEdit, List<V_HIS_SERVICE>>();

        private ControlStateWorker controlStateWorker;
        private List<ControlStateRDO> currentControlStateRDO;

        public frmAutoClsSetting()
        {
            InitializeComponent();
            try
            {
                try { this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetEntryAssembly().Location); }
                catch (Exception exIcon) { LogSystem.Warn(exIcon); }

                InitCombos();
                InitControlState();
                LoadSavedSelection();
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        /// <summary>Gán datasource V_HIS_SERVICE theo loại + bật chọn nhiều cho 4 combo.</summary>
        private void InitCombos()
        {
            var all = BackendDataWorker.Get<V_HIS_SERVICE>();
            var xn = all.Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN && o.IS_ACTIVE == 1)
                        .OrderBy(o => o.SERVICE_NAME).ToList();
            var cdha = all.Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA && o.IS_ACTIVE == 1)
                        .OrderBy(o => o.SERVICE_NAME).ToList();

            SetupMultiCombo(this.cboHuyetHoc, xn);
            SetupMultiCombo(this.cboSinhHoaMau, xn.ToList());
            SetupMultiCombo(this.cboSinhHoaNuocTieu, xn.ToList());
            SetupMultiCombo(this.cboSieuAmOB, cdha);
        }

        /// <summary>Cấu hình 1 GridLookUpEdit thành combo chọn nhiều (GridCheckMarksSelection).</summary>
        private void SetupMultiCombo(GridLookUpEdit cbo, List<V_HIS_SERVICE> ds)
        {
            this.selecteds[cbo] = new List<V_HIS_SERVICE>();

            cbo.Properties.DataSource = ds;
            cbo.Properties.DisplayMember = "SERVICE_NAME";
            cbo.Properties.ValueMember = "ID";
            cbo.Properties.NullText = "";

            var colCode = cbo.Properties.View.Columns.AddField("SERVICE_CODE");
            colCode.VisibleIndex = 1; colCode.Width = 90; colCode.Caption = "Mã DV";
            var colName = cbo.Properties.View.Columns.AddField("SERVICE_NAME");
            colName.VisibleIndex = 2; colName.Width = 320; colName.Caption = "Tên dịch vụ";
            cbo.Properties.PopupFormWidth = 430;
            cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
            cbo.Properties.View.OptionsSelection.MultiSelect = true;
            cbo.Properties.ImmediatePopup = true;
            cbo.Properties.View.OptionsFind.AlwaysVisible = true;
            cbo.Properties.View.OptionsFind.FindMode = DevExpress.XtraEditors.FindMode.Always;
            cbo.Properties.View.OptionsFind.FindFilterColumns = "SERVICE_CODE;SERVICE_NAME";
            cbo.Properties.View.OptionsFind.FindNullPrompt = "Tìm theo mã / tên dịch vụ...";

            GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
            gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(Combo_SelectionChanged);
            cbo.Properties.Tag = gridCheck;
            gridCheck.ClearSelection(cbo.Properties.View);

            cbo.CustomDisplayText -= Combo_CustomDisplayText;
            cbo.CustomDisplayText += Combo_CustomDisplayText;

            EditorButton del = new EditorButton(ButtonPredefines.Delete);
            del.ToolTip = "Xóa dịch vụ đang chọn";
            cbo.Properties.Buttons.Add(del);
            cbo.ButtonClick -= Combo_ClearButtonClick;
            cbo.ButtonClick += Combo_ClearButtonClick;
        }

        private void Combo_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                GridCheckMarksSelection gridCheck = sender as GridCheckMarksSelection;
                if (gridCheck == null) return;
                GridLookUpEdit cbo = selecteds.Keys.FirstOrDefault(c => ReferenceEquals(c.Properties.Tag, gridCheck));
                if (cbo == null) return;

                var list = new List<V_HIS_SERVICE>();
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (V_HIS_SERVICE sv in gridCheck.Selection)
                {
                    if (sv == null) continue;
                    if (sb.Length > 0) sb.Append(", ");
                    sb.Append(sv.SERVICE_NAME);
                    list.Add(sv);
                }
                selecteds[cbo] = list;
                cbo.Text = sb.ToString();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void Combo_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                GridLookUpEdit cbo = sender as GridLookUpEdit;
                e.DisplayText = "";
                if (cbo != null && selecteds.ContainsKey(cbo) && selecteds[cbo] != null && selecteds[cbo].Count > 0)
                    e.DisplayText = string.Join("; ", selecteds[cbo].Select(o => o.SERVICE_NAME).ToArray());
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void Combo_ClearButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e == null || e.Button == null || e.Button.Kind != ButtonPredefines.Delete) return;
                GridLookUpEdit cbo = sender as GridLookUpEdit;
                if (cbo == null) return;
                GridCheckMarksSelection gridCheck = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheck != null) gridCheck.ClearSelection(cbo.Properties.View);
                selecteds[cbo] = new List<V_HIS_SERVICE>();
                cbo.EditValue = null;
                cbo.Text = string.Empty;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        // ===== ControlState (local) =====
        private void InitControlState()
        {
            this.controlStateWorker = new ControlStateWorker();
            this.currentControlStateRDO = controlStateWorker.GetData(MODULE_LINK) ?? new List<ControlStateRDO>();
        }

        private string GetStateValue(string key)
        {
            var item = currentControlStateRDO.FirstOrDefault(o => o.KEY == key && o.MODULE_LINK == MODULE_LINK);
            return item != null ? item.VALUE : null;
        }

        private void SetStateValue(string key, string value)
        {
            var item = currentControlStateRDO.FirstOrDefault(o => o.KEY == key && o.MODULE_LINK == MODULE_LINK);
            if (item != null) item.VALUE = value;
            else currentControlStateRDO.Add(new ControlStateRDO() { KEY = key, VALUE = value, MODULE_LINK = MODULE_LINK });
        }

        /// <summary>Đọc CSV ID đã lưu và tick lại trên từng combo.</summary>
        private void LoadSavedSelection()
        {
            ApplySavedToCombo(this.cboHuyetHoc, GetStateValue(KEY_HUYET_HOC));
            ApplySavedToCombo(this.cboSinhHoaMau, GetStateValue(KEY_SINH_HOA_MAU));
            ApplySavedToCombo(this.cboSinhHoaNuocTieu, GetStateValue(KEY_SINH_HOA_NUOC_TIEU));
            ApplySavedToCombo(this.cboSieuAmOB, GetStateValue(KEY_SIEU_AM_OB));
        }

        private void ApplySavedToCombo(GridLookUpEdit cbo, string csvIds)
        {
            try
            {
                GridCheckMarksSelection gridCheck = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheck == null) return;
                gridCheck.ClearSelection(cbo.Properties.View);
                selecteds[cbo] = new List<V_HIS_SERVICE>();
                if (!string.IsNullOrEmpty(csvIds))
                {
                    var ds = cbo.Properties.DataSource as List<V_HIS_SERVICE>;
                    if (ds != null)
                    {
                        foreach (string s in csvIds.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            var row = ds.FirstOrDefault(o => o.ID.ToString() == s.Trim());
                            if (row != null && !gridCheck.Selection.Contains(row))
                                gridCheck.Selection.Add(row);
                        }
                    }
                }
                gridCheck.OnSelectionChanged();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private string GetCsvIds(GridLookUpEdit cbo)
        {
            if (!selecteds.ContainsKey(cbo) || selecteds[cbo] == null) return "";
            return string.Join(";", selecteds[cbo].Select(o => o.ID.ToString()).ToArray());
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SetStateValue(KEY_HUYET_HOC, GetCsvIds(this.cboHuyetHoc));
                SetStateValue(KEY_SINH_HOA_MAU, GetCsvIds(this.cboSinhHoaMau));
                SetStateValue(KEY_SINH_HOA_NUOC_TIEU, GetCsvIds(this.cboSinhHoaNuocTieu));
                SetStateValue(KEY_SIEU_AM_OB, GetCsvIds(this.cboSieuAmOB));
                controlStateWorker.SetData(currentControlStateRDO);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }
    }
}
