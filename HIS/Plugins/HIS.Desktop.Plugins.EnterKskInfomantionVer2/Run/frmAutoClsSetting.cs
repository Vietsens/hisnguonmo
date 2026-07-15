/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Form cấu hình dịch vụ cho "Tự động lấy kết quả CLS" (mở từ nút setting cạnh checkbox).
 * UI thiết kế trong Designer (LayoutControl + 3 GridLookUpEdit + nút Lưu). File này chỉ chứa logic:
 * đổ datasource V_HIS_SERVICE theo loại (XN cho Máu & Nước tiểu, CĐHA cho Chẩn đoán hình ảnh),
 * cho chọn NHIỀU (GridCheckMarksSelection - nhái cboObject), LƯU/HIỂN LẠI trạng thái tích qua ControlState.
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

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmAutoClsSetting : DevExpress.XtraEditors.XtraForm
    {
        private const string MODULE_LINK = "HIS.Desktop.Plugins.EnterKskInfomantionVer2";
        private const string KEY_BLOOD = "AutoCls_Blood";
        private const string KEY_URINE = "AutoCls_Urine";
        private const string KEY_DIIM = "AutoCls_Diim";

        // Danh sách dịch vụ đang tick theo từng combo (giống objectSelecteds ở cboObject).
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

        /// <summary>Gán datasource V_HIS_SERVICE theo loại + bật chọn nhiều cho 3 combo.</summary>
        private void InitCombos()
        {
            var all = BackendDataWorker.Get<V_HIS_SERVICE>();
            var xn = all.Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN && o.IS_ACTIVE == 1)
                        .OrderBy(o => o.SERVICE_NAME).ToList();
            var cdha = all.Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA && o.IS_ACTIVE == 1)
                        .OrderBy(o => o.SERVICE_NAME).ToList();

            SetupMultiCombo(this.cboBlood, xn);
            SetupMultiCombo(this.cboUrine, xn.ToList());   // Máu & Nước tiểu cùng loại XN (user tự chọn)
            SetupMultiCombo(this.cboDiim, cdha);
        }

        /// <summary>Cấu hình 1 GridLookUpEdit thành combo chọn nhiều (GridCheckMarksSelection) — nhái cboObject.</summary>
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
            // Ô tìm kiếm trong popup: tìm theo MỌI cột hiển thị (Mã DV + Tên dịch vụ).
            cbo.Properties.ImmediatePopup = true;
            cbo.Properties.View.OptionsFind.AlwaysVisible = true;
            cbo.Properties.View.OptionsFind.FindMode = DevExpress.XtraEditors.FindMode.Always;       // gõ tới đâu lọc tới đó
            cbo.Properties.View.OptionsFind.FindFilterColumns = "SERVICE_CODE;SERVICE_NAME";         // tìm theo 2 cột Mã + Tên
            cbo.Properties.View.OptionsFind.FindNullPrompt = "Tìm theo mã / tên dịch vụ...";

            GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
            gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(Combo_SelectionChanged);
            cbo.Properties.Tag = gridCheck;
            gridCheck.ClearSelection(cbo.Properties.View);

            cbo.CustomDisplayText -= Combo_CustomDisplayText;
            cbo.CustomDisplayText += Combo_CustomDisplayText;

            // Nút Xóa: bỏ hết tick.
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
            ApplySavedToCombo(this.cboBlood, GetStateValue(KEY_BLOOD));
            ApplySavedToCombo(this.cboUrine, GetStateValue(KEY_URINE));
            ApplySavedToCombo(this.cboDiim, GetStateValue(KEY_DIIM));
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
                SetStateValue(KEY_BLOOD, GetCsvIds(this.cboBlood));
                SetStateValue(KEY_URINE, GetCsvIds(this.cboUrine));
                SetStateValue(KEY_DIIM, GetCsvIds(this.cboDiim));
                controlStateWorker.SetData(currentControlStateRDO);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }
    }
}
