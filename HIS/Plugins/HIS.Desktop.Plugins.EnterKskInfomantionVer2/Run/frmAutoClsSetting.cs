/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Form "Thiết lập" (mở từ nút ⚙ ở đáy form nhập KSK), gồm 2 tab:
 *
 *  Tab 1 "Tự động lấy CLS" — cấu hình dịch vụ cho "Tự động lấy kết quả CLS".
 *      Đổ datasource V_HIS_SERVICE theo loại (XN cho Máu & Nước tiểu, CĐHA cho Chẩn đoán hình ảnh),
 *      cho chọn NHIỀU (GridCheckMarksSelection - nhái cboObject), LƯU/HIỂN LẠI qua ControlState.
 *
 *  Tab 2 "Mặc định nhập KSK (trẻ dưới 6 tuổi)" — lưới 5 cột người dùng TỰ THÊM/BỚT dòng:
 *      Dùng → Mục → Nội dung → Giá trị mặc định (3 cột sau phụ thuộc nhau), cột 5 là nút +/-.
 *      Chỉ dòng tích "Dùng" mới đem áp; dòng bỏ tích vẫn được LƯU nguyên giá trị.
 *      Danh mục dựng ĐỘNG từ layout các mục IV, V, VI của tab "Trẻ em dưới 6 tuổi" và được
 *      truyền vào qua constructor (form này không thấy control của form kia).
 *      Chưa chọn "Mục" thì "Nội dung" không có dữ liệu; chưa chọn "Nội dung" thì "Giá trị" rỗng.
 *
 * UI cả 2 tab khai báo trong Designer, file này chỉ chứa logic.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Library.CacheClient;
using HIS.Desktop.Plugins.EnterKskInfomantionVer2.ADO;
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

        // ===== Tab 2: danh mục dựng động do form gọi truyền vào (null => ẩn hẳn tab) =====
        private List<KskDefaultGroupADO> allDefaultGroups;
        private List<KskDefaultFieldADO> allDefaultFields;
        private List<KskDefaultValueADO> allDefaultValues;

        // BindingList để thêm/bớt dòng bằng nút +/- là lưới tự vẽ lại, không phải bind lại DataSource.
        private BindingList<KskDefaultRowADO> defaultRows;

        /// <summary>
        /// True khi người dùng bấm "Áp dụng ngay" — form gọi đọc cờ này sau ShowDialog để điền
        /// mặc định vào bản ghi đang mở. Bấm "Lưu" thì chỉ lưu, không áp.
        /// </summary>
        public bool IsApplyNowRequested { get; private set; }

        public frmAutoClsSetting()
            : this(null, null, null)
        {
        }

        /// <summary>
        /// Mở form Thiết lập kèm danh mục cho tab "Mặc định khám lâm sàng".
        /// Truyền null (hoặc rỗng) 3 danh mục → ẩn tab 2, form chạy y như bản chỉ có cấu hình CLS.
        /// </summary>
        public frmAutoClsSetting(List<KskDefaultGroupADO> groups,
                                 List<KskDefaultFieldADO> fields,
                                 List<KskDefaultValueADO> values)
        {
            InitializeComponent();

            try { this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetEntryAssembly().Location); }
            catch (Exception exIcon) { LogSystem.Warn(exIcon); }

            // 2 tab init ĐỘC LẬP: gộp chung 1 try thì InitCombos hỏng (cache V_HIS_SERVICE chưa nạp)
            // là tab "Mặc định khám lâm sàng" không bao giờ được dựng mà không báo gì.
            try
            {
                InitCombos();
                InitControlState();
                LoadSavedSelection();
            }
            catch (Exception ex) { LogSystem.Error(ex); }

            try { InitUnderSixDefaultTab(groups, fields, values); }
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

            // DÒNG LỌC (auto filter row) trên CẢ 2 CỘT Mã DV + Tên dịch vụ — làm đúng khuôn
            // cboTransporterLoginName ("Người vận chuyển" ở Kết thúc điều trị → Thông tin vận chuyển),
            // xem HIS.Desktop.Plugins.TreatmentFinish\CloseTreatment\FormTransfer.cs (InitComboTransporterLoginName).
            // Cách cũ (find panel + FindFilterColumns) KHÔNG chạy trong popup của GridLookUpEdit này.
            cbo.Properties.View.OptionsView.GroupDrawMode = DevExpress.XtraGrid.Views.Grid.GroupDrawMode.Office;
            cbo.Properties.View.OptionsView.HeaderFilterButtonShowMode = DevExpress.XtraEditors.Controls.FilterButtonShowMode.SmartTag;
            cbo.Properties.View.OptionsView.ShowAutoFilterRow = true;
            cbo.Properties.View.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            cbo.Properties.View.OptionsView.ShowDetailButtons = false;
            cbo.Properties.View.OptionsView.ShowGroupPanel = false;
            cbo.Properties.View.OptionsView.ShowIndicator = false;

            var colCode = cbo.Properties.View.Columns.AddField("SERVICE_CODE");
            colCode.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
            colCode.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            colCode.VisibleIndex = 1; colCode.Width = 90; colCode.Caption = "Mã DV";

            var colName = cbo.Properties.View.Columns.AddField("SERVICE_NAME");
            colName.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
            colName.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            colName.VisibleIndex = 2; colName.Width = 320; colName.Caption = "Tên dịch vụ";

            cbo.Properties.PopupFormWidth = 430;
            cbo.Properties.PopupFormMinSize = new System.Drawing.Size(430, 320);   // đủ cao để thấy dòng lọc + danh sách
            cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
            cbo.Properties.View.OptionsSelection.MultiSelect = true;
            cbo.Properties.ImmediatePopup = true;

            GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
            gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(Combo_SelectionChanged);
            cbo.Properties.Tag = gridCheck;
            gridCheck.ClearSelection(cbo.Properties.View);

            cbo.CustomDisplayText -= Combo_CustomDisplayText;
            cbo.CustomDisplayText += Combo_CustomDisplayText;
            // Mở popup: bỏ điều kiện lọc của lần trước, nếu không lần sau mở ra danh sách vẫn đang bị lọc.
            cbo.Popup -= Combo_Popup;
            cbo.Popup += Combo_Popup;

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

        /// <summary>
        /// Mở popup combo dịch vụ: xóa điều kiện đã gõ ở dòng lọc lần trước để lần nào mở ra cũng
        /// thấy đủ danh mục (không xóa thì mở lại vẫn đang bị lọc theo từ khóa cũ).
        /// </summary>
        private void Combo_Popup(object sender, EventArgs e)
        {
            try
            {
                GridLookUpEdit cbo = sender as GridLookUpEdit;
                if (cbo == null || cbo.Properties == null) return;
                GridView view = cbo.Properties.View as GridView;
                if (view == null) return;
                view.ClearColumnsFilter();
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
                SaveAll();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        /// <summary>
        /// Lưu cả 2 tab. Tab 2 lưu qua KskDefaultSettingUtil (key riêng, không trộn vào 3 key CLS).
        /// Mỗi tab lưu độc lập: tab kia init lỗi thì tab này vẫn lưu được.
        /// </summary>
        private void SaveAll()
        {
            if (this.controlStateWorker != null && this.currentControlStateRDO != null)
            {
                SetStateValue(KEY_BLOOD, GetCsvIds(this.cboBlood));
                SetStateValue(KEY_URINE, GetCsvIds(this.cboUrine));
                SetStateValue(KEY_DIIM, GetCsvIds(this.cboDiim));
                controlStateWorker.SetData(currentControlStateRDO);
            }

            if (this.defaultRows != null)
                KskDefaultSettingUtil.Save(BuildDefaultRows(), this.chkAutoApplyDefault.Checked);
        }

        #region ===== Tab 2: Mặc định khám lâm sàng (dưới 6 tuổi) =====

        /// <summary>
        /// Dựng tab 2: gán danh mục cho 3 cột lookup, nạp lại thiết lập đã lưu, luôn chừa 1 dòng "+" ở cuối.
        /// Không có danh mục (form mở từ chỗ khác) → bỏ hẳn tab và nút "Áp dụng ngay".
        /// </summary>
        private void InitUnderSixDefaultTab(List<KskDefaultGroupADO> groups,
                                            List<KskDefaultFieldADO> fields,
                                            List<KskDefaultValueADO> values)
        {
            try
            {
                if (groups == null || groups.Count == 0 || fields == null || fields.Count == 0)
                {
                    this.xtraTabControlSetting.TabPages.Remove(this.tabUnderSixDefault);
                    this.lciApplyNow.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.Text = "Cấu hình dịch vụ tự động lấy kết quả CLS";
                    return;
                }

                this.allDefaultGroups = groups;
                this.allDefaultFields = fields;
                this.allDefaultValues = values ?? new List<KskDefaultValueADO>();

                // Ở trạng thái nghỉ 3 lookup giữ danh mục ĐẦY ĐỦ để lưới luôn dò ra nhãn hiển thị.
                // Lúc mở editor mới thu hẹp theo dòng (xem gvDefault_ShownEditor) rồi trả lại ở HiddenEditor.
                this.repoGroup.DataSource = this.allDefaultGroups;
                this.repoField.DataSource = this.allDefaultFields;
                this.repoValue.DataSource = this.allDefaultValues;

                List<KskDefaultRowADO> saved;
                bool autoApply;
                KskDefaultSettingUtil.Load(out saved, out autoApply);
                this.chkAutoApplyDefault.Checked = autoApply;

                this.defaultRows = new BindingList<KskDefaultRowADO>();
                foreach (var item in saved)
                {
                    // Bỏ qua field đã bị xóa khỏi Designer để thiết lập cũ không làm lưới hiện dòng rác.
                    var field = this.allDefaultFields.FirstOrDefault(o => o.FIELD_NAME == item.FIELD_NAME);
                    if (field == null) continue;
                    if (!this.allDefaultValues.Any(o => o.VALUE_KEY == item.VALUE_KEY)) continue;   // giá trị không còn trong RadioGroup
                    this.defaultRows.Add(new KskDefaultRowADO()
                    {
                        IS_USED = item.IS_USED,
                        GROUP_NAME = field.GROUP_NAME,
                        FIELD_NAME = field.FIELD_NAME,
                        VALUE_KEY = item.VALUE_KEY
                    });
                }
                EnsureTrailingBlankRow();

                this.grdDefault.DataSource = this.defaultRows;
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        /// <summary>Dòng cuối là dòng "+" nên phải luôn tồn tại (kể cả khi chưa cấu hình gì).</summary>
        private void EnsureTrailingBlankRow()
        {
            if (this.defaultRows == null) return;
            var last = this.defaultRows.LastOrDefault();
            if (last == null || !string.IsNullOrEmpty(last.FIELD_NAME) || !string.IsNullOrEmpty(last.GROUP_NAME))
                this.defaultRows.Add(NewDefaultRow());
        }

        /// <summary>Dòng mới mặc định ĐANG dùng — thêm dòng là để dùng, bỏ tích là hành động chủ động.</summary>
        private KskDefaultRowADO NewDefaultRow()
        {
            return new KskDefaultRowADO() { IS_USED = true };
        }

        /// <summary>
        /// Gom các dòng đã chọn đủ Nội dung + Giá trị để lưu — LẤY CẢ dòng không tích "Dùng"
        /// (cờ dùng lưu kèm) nên bỏ tích không làm mất cấu hình đã nhập.
        /// </summary>
        private List<KskDefaultRowADO> BuildDefaultRows()
        {
            var result = new List<KskDefaultRowADO>();
            if (this.defaultRows == null) return result;
            foreach (var row in this.defaultRows)
            {
                if (row == null || string.IsNullOrEmpty(row.FIELD_NAME) || string.IsNullOrEmpty(row.VALUE_KEY)) continue;
                if (KskDefaultSettingUtil.ParseValueFromKey(row.VALUE_KEY) == null) continue;
                result.Add(row);
            }
            return result;
        }

        /// <summary>Cột 4: dòng cuối cùng là nút +, các dòng trên là nút -.</summary>
        private void gvDefault_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column != this.colAction || this.defaultRows == null) return;
                e.RepositoryItem = (e.RowHandle == this.defaultRows.Count - 1) ? (DevExpress.XtraEditors.Repository.RepositoryItem)this.repoAdd : this.repoRemove;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Thu hẹp danh mục của editor đang mở theo dòng hiện tại: Nội dung lọc theo Mục,
        /// Giá trị lọc theo Nội dung. Chưa chọn cột trước → danh sách rỗng (đúng yêu cầu).
        /// </summary>
        private void gvDefault_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                GridView gv = sender as GridView;
                if (gv == null) return;
                GridLookUpEdit edit = gv.ActiveEditor as GridLookUpEdit;
                if (edit == null) return;
                KskDefaultRowADO row = gv.GetRow(gv.FocusedRowHandle) as KskDefaultRowADO;
                if (row == null) return;

                // Đổi DataSource làm editor mất EditValue -> giữ lại rồi gán trả, nếu không vào
                // sửa dòng đã cấu hình là giá trị cũ bị xóa.
                object current = edit.EditValue;

                if (gv.FocusedColumn == this.colField)
                {
                    edit.Properties.DataSource = string.IsNullOrEmpty(row.GROUP_NAME)
                        ? new List<KskDefaultFieldADO>()
                        : this.allDefaultFields.Where(o => o.GROUP_NAME == row.GROUP_NAME).ToList();
                }
                else if (gv.FocusedColumn == this.colValue)
                {
                    edit.Properties.DataSource = string.IsNullOrEmpty(row.FIELD_NAME)
                        ? new List<KskDefaultValueADO>()
                        : this.allDefaultValues.Where(o => o.FIELD_NAME == row.FIELD_NAME).ToList();
                }
                else return;

                edit.EditValue = current;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Trả lại danh mục đầy đủ cho 2 lookup để các dòng khác hiển thị đúng nhãn.</summary>
        private void gvDefault_HiddenEditor(object sender, EventArgs e)
        {
            try
            {
                if (this.allDefaultFields == null) return;
                this.repoField.DataSource = this.allDefaultFields;
                this.repoValue.DataSource = this.allDefaultValues;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Đổi Mục → bỏ Nội dung + Giá trị; đổi Nội dung → bỏ Giá trị (tránh cặp lệch nhau).</summary>
        private void gvDefault_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                GridView gv = sender as GridView;
                if (gv == null) return;
                KskDefaultRowADO row = gv.GetRow(e.RowHandle) as KskDefaultRowADO;
                if (row == null) return;

                // Gán trực tiếp lên ADO (không SetRowCellValue) để không kích lại CellValueChanged.
                if (e.Column == this.colGroup) { row.FIELD_NAME = null; row.VALUE_KEY = null; }
                else if (e.Column == this.colField) { row.VALUE_KEY = null; }
                else return;

                gv.RefreshRow(e.RowHandle);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Nút + ở dòng cuối: thêm 1 dòng trắng và nhảy vào cột Mục của dòng mới.</summary>
        private void repoAdd_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (this.defaultRows == null) return;
                this.gvDefault.CloseEditor();
                this.defaultRows.Add(NewDefaultRow());
                this.gvDefault.FocusedRowHandle = this.defaultRows.Count - 1;
                this.gvDefault.FocusedColumn = this.colGroup;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Nút - : bỏ dòng đó. Xóa hết thì thêm lại 1 dòng trắng để còn chỗ đặt nút +.</summary>
        private void repoRemove_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (this.defaultRows == null) return;
                KskDefaultRowADO row = this.gvDefault.GetRow(this.gvDefault.FocusedRowHandle) as KskDefaultRowADO;
                if (row == null) return;
                this.gvDefault.CloseEditor();
                this.defaultRows.Remove(row);
                if (this.defaultRows.Count == 0) this.defaultRows.Add(NewDefaultRow());
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Ghi thiết lập CẢ 2 TAB ra file JSON để cóp sang máy khác.</summary>
        private void btnExportJson_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.defaultRows != null) this.gvDefault.CloseEditor();

                KskSettingFileADO data = BuildSettingFile();
                int rowCount = data.ROWS.Count;
                int svCount = data.AUTO_CLS_BLOOD.Count + data.AUTO_CLS_URINE.Count + data.AUTO_CLS_DIIM.Count;
                if (rowCount == 0 && svCount == 0)
                {
                    XtraMessageBox.Show("Cả 2 tab đều chưa có thiết lập nào để xuất.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Title = "Xuất thiết lập (Tự động lấy CLS + Mặc định nhập KSK)";
                    sfd.Filter = "File JSON (*.json)|*.json|Tất cả (*.*)|*.*";
                    sfd.DefaultExt = "json";
                    sfd.AddExtension = true;
                    sfd.FileName = "ThietLapKsk_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".json";
                    if (sfd.ShowDialog(this) != DialogResult.OK) return;

                    string json = KskDefaultSettingUtil.BuildJson(data,
                        this.allDefaultGroups, this.allDefaultFields, this.allDefaultValues);
                    // UTF8 KHÔNG BOM: file JSON có BOM là một số trình đọc khác báo lỗi ký tự đầu.
                    System.IO.File.WriteAllText(sfd.FileName, json, new System.Text.UTF8Encoding(false));

                    XtraMessageBox.Show("Đã xuất ra file:\r\n" + sfd.FileName
                        + "\r\n\r\n - Tự động lấy CLS: " + svCount + " dịch vụ"
                        + "\r\n - Mặc định nhập KSK: " + rowCount + " dòng",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                XtraMessageBox.Show("Không xuất được file: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Gom trạng thái hiện tại của cả 2 tab thành 1 object để xuất.</summary>
        private KskSettingFileADO BuildSettingFile()
        {
            var data = new KskSettingFileADO();
            data.AUTO_CLS_BLOOD = ToServiceRefs(this.cboBlood);
            data.AUTO_CLS_URINE = ToServiceRefs(this.cboUrine);
            data.AUTO_CLS_DIIM = ToServiceRefs(this.cboDiim);
            if (this.defaultRows != null)
            {
                data.ROWS = BuildDefaultRows();
                data.AUTO_APPLY = this.chkAutoApplyDefault.Checked;
            }
            return data;
        }

        /// <summary>Dịch vụ đang tick ở 1 combo → danh sách ID/mã/tên để ghi ra file.</summary>
        private List<KskServiceRefADO> ToServiceRefs(GridLookUpEdit cbo)
        {
            var result = new List<KskServiceRefADO>();
            if (cbo == null || !this.selecteds.ContainsKey(cbo) || this.selecteds[cbo] == null) return result;
            foreach (V_HIS_SERVICE sv in this.selecteds[cbo])
            {
                if (sv == null) continue;
                result.Add(new KskServiceRefADO() { ID = sv.ID, CODE = sv.SERVICE_CODE, NAME = sv.SERVICE_NAME });
            }
            return result;
        }

        /// <summary>
        /// Đọc thiết lập CẢ 2 TAB từ file JSON. CHỈ nạp lên giao diện, chưa ghi — người dùng xem lại
        /// rồi bấm "Lưu" mới thực sự thay thiết lập của máy này.
        /// Phần nào không có trong file thì giữ nguyên, không xóa.
        /// </summary>
        private void btnImportJson_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Title = "Nhập thiết lập (Tự động lấy CLS + Mặc định nhập KSK)";
                    ofd.Filter = "File JSON (*.json)|*.json|Tất cả (*.*)|*.*";
                    ofd.CheckFileExists = true;
                    ofd.Multiselect = false;
                    if (ofd.ShowDialog(this) != DialogResult.OK) return;

                    KskSettingFileADO data;
                    string error;
                    if (!KskDefaultSettingUtil.TryParseJson(System.IO.File.ReadAllText(ofd.FileName), out data, out error))
                    {
                        XtraMessageBox.Show(error, "Không nhập được", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var report = new System.Text.StringBuilder();
                    if (XtraMessageBox.Show("Nhập từ file sẽ THAY THẾ thiết lập đang hiển thị của những phần có trong file. Tiếp tục?",
                            "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

                    // ===== Tab 1: Tự động lấy CLS =====
                    if (data.HasAutoCls)
                    {
                        int okBlood, okUrine, okDiim, missBlood, missUrine, missDiim;
                        ImportServices(this.cboBlood, data.AUTO_CLS_BLOOD, out okBlood, out missBlood);
                        ImportServices(this.cboUrine, data.AUTO_CLS_URINE, out okUrine, out missUrine);
                        ImportServices(this.cboDiim, data.AUTO_CLS_DIIM, out okDiim, out missDiim);
                        report.Append("Tự động lấy CLS: nhận ")
                              .Append(okBlood + okUrine + okDiim).Append(" dịch vụ");
                        int miss = missBlood + missUrine + missDiim;
                        if (miss > 0) report.Append(", bỏ ").Append(miss).Append(" dịch vụ không có trong danh mục máy này");
                        report.Append("\r\n");
                    }
                    else
                    {
                        report.Append("Tự động lấy CLS: file không có phần này → giữ nguyên\r\n");
                    }

                    // ===== Tab 2: Mặc định nhập KSK =====
                    if (data.ROWS.Count > 0)
                    {
                        if (this.defaultRows == null || this.allDefaultFields == null)
                        {
                            report.Append("Mặc định nhập KSK: file có ").Append(data.ROWS.Count)
                                  .Append(" dòng nhưng tab này chưa dựng được → bỏ qua\r\n");
                        }
                        else
                        {
                            var kept = new List<KskDefaultRowADO>();
                            var dropped = new List<string>();
                            foreach (var item in data.ROWS)
                            {
                                var field = this.allDefaultFields.FirstOrDefault(o => o.FIELD_NAME == item.FIELD_NAME);
                                if (field == null) { dropped.Add(item.FIELD_NAME + " (không còn ô này)"); continue; }
                                if (!this.allDefaultValues.Any(o => o.VALUE_KEY == item.VALUE_KEY))
                                {
                                    dropped.Add(field.FIELD_CAPTION + " (giá trị không hợp lệ)");
                                    continue;
                                }
                                kept.Add(new KskDefaultRowADO()
                                {
                                    IS_USED = item.IS_USED,
                                    GROUP_NAME = field.GROUP_NAME,
                                    FIELD_NAME = field.FIELD_NAME,
                                    VALUE_KEY = item.VALUE_KEY
                                });
                            }

                            if (kept.Count > 0)
                            {
                                this.gvDefault.CloseEditor();
                                this.defaultRows.Clear();
                                foreach (var row in kept) this.defaultRows.Add(row);
                                EnsureTrailingBlankRow();
                                this.chkAutoApplyDefault.Checked = data.AUTO_APPLY;
                            }

                            report.Append("Mặc định nhập KSK: nhận ").Append(kept.Count).Append(" dòng");
                            if (dropped.Count > 0)
                            {
                                report.Append(", bỏ ").Append(dropped.Count).Append(" dòng:\r\n   - ")
                                      .Append(string.Join("\r\n   - ", dropped.Take(8).ToArray()));
                                if (dropped.Count > 8) report.Append("\r\n   - ...");
                            }
                            report.Append("\r\n");
                        }
                    }
                    else
                    {
                        report.Append("Mặc định nhập KSK: file không có phần này → giữ nguyên\r\n");
                    }

                    report.Append("\r\nBấm \"Lưu\" để ghi lại cho máy này.");
                    XtraMessageBox.Show(report.ToString(), "Kết quả nhập", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                XtraMessageBox.Show("Không nhập được file: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Tick lại 1 combo dịch vụ theo danh sách trong file: ưu tiên khớp ID, không thấy thì khớp
        /// CODE (máy khác CSDL thì ID lệch nhưng mã dịch vụ thường giữ nguyên).
        /// </summary>
        private void ImportServices(GridLookUpEdit cbo, List<KskServiceRefADO> refs, out int matched, out int missing)
        {
            matched = 0;
            missing = 0;
            try
            {
                if (cbo == null || refs == null) return;
                var ds = cbo.Properties.DataSource as List<V_HIS_SERVICE>;
                if (ds == null) { missing = refs.Count; return; }

                var ids = new List<string>();
                foreach (var r in refs)
                {
                    if (r == null) continue;
                    V_HIS_SERVICE found = ds.FirstOrDefault(o => o.ID == r.ID);
                    if (found == null && !string.IsNullOrEmpty(r.CODE))
                        found = ds.FirstOrDefault(o => o.SERVICE_CODE == r.CODE);
                    if (found == null) { missing++; continue; }
                    ids.Add(found.ID.ToString());
                    matched++;
                }
                // Dùng lại đúng khuôn nạp trạng thái đã lưu (CSV ID, phân tách bằng ';').
                ApplySavedToCombo(cbo, string.Join(";", ids.ToArray()));
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }
        /// <summary>Lưu rồi báo form gọi điền ngay mặc định vào bản ghi đang mở.</summary>
        private void btnApplyNow_Click(object sender, EventArgs e)
        {
            try
            {
                this.gvDefault.CloseEditor();
                SaveAll();
                this.IsApplyNowRequested = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        #endregion
    }
}
