/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.MedicineMediStockSummaryVertical.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Common.Modules;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MedicineMediStockSummaryVertical
{
    public partial class ucMedicineMediStockSummaryVertical : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare

        // ID quy ước cho dòng "(Tất cả chi nhánh)" — branch ID thực luôn > 0
        const long ALL_BRANCH_ID = 0;

        Inventec.Desktop.Common.Modules.Module currentModule;
        string loginName;
        bool isNotLoadWhileChangeControlStateInFirst = false;

        // Cờ chặn đệ quy khi xử lý tích chọn 1-trong-2 (Thuốc/Vật tư)
        bool isCheckChanging = false;

        List<HIS_BRANCH> branchList;

        // Dữ liệu đang hiển thị trên grid
        List<MediStockSummaryVerticalADO> gridData = new List<MediStockSummaryVerticalADO>();

        // Các cột số liệu có ô tổng ở footer + màu chữ tương ứng.
        readonly List<TotalColInfo> totalColumns = new List<TotalColInfo>();

        // Màu chữ (dịu, không chói)
        static readonly Color COLOR_IN = Color.FromArgb(0, 128, 0);      // xanh lá - SL nhập
        static readonly Color COLOR_OUT = Color.FromArgb(230, 126, 34);  // cam     - SL xuất
        static readonly Color COLOR_AMOUNT = Color.Maroon;              // tồn hiện tại

        private class TotalColInfo
        {
            public string FieldName;
            public Color Color;   // Color.Empty = dùng màu mặc định của footer
        }

        // Yêu cầu "Xem tồn kho theo kho" nhận từ danh mục Thuốc/Vật tư (qua RequestStore).
        HIS.Desktop.LocalStorage.LocalData.MedicineMediStockVerticalRequest pendingRequest;
        bool isLoaded = false;

        #endregion

        #region Constructor

        // Constructor rỗng - BẮT BUỘC để WinForms Designer mở được giao diện
        public ucMedicineMediStockSummaryVertical()
        {
            InitializeComponent();
            try
            {
                this.loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public ucMedicineMediStockSummaryVertical(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            try
            {
                InitializeComponent();
                this.currentModule = module;
                this.loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                // Lắng nghe yêu cầu "Xem tồn kho theo kho" từ danh mục Thuốc/Vật tư
                // (xử lý cả khi tab UC đang mở sẵn — ShowModule không chạy lại Processor).
                HIS.Desktop.LocalStorage.LocalData.MedicineMediStockVerticalRequestStore.RequestRaised += OnExternalRequest;
                this.Disposed += ucMedicineMediStockSummaryVertical_Disposed;

                // Tự vẽ dòng tổng ở footer (nhãn "Tổng số lượng" merge các cột + giá trị ở cột cuối).
                gridViewData.CustomDrawFooter += GridViewData_CustomDrawFooter;

                // Giữ màu chữ theo cột kể cả khi focus/chọn dòng (không bị đổi về đen khi focus).
                gridViewData.RowCellStyle += GridViewData_RowCellStyle;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ucMedicineMediStockSummaryVertical_Disposed(object sender, EventArgs e)
        {
            HIS.Desktop.LocalStorage.LocalData.MedicineMediStockVerticalRequestStore.RequestRaised -= OnExternalRequest;
        }

        #endregion

        #region Load

        private void ucMedicineMediStockSummaryVertical_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                InitComboData();
                SetCaptionByLanguageKey();
                InitTabIndex();
                SetDefaultValue();
                InitControlState();
                // Mở từ menu (UserControl): dựng cột mặc định + grid rỗng, KHÔNG tự tìm kiếm (Xử lý #1)
                InitGridDefault();
                WaitingManager.Hide();

                isLoaded = true;
                // Xử lý #2: nếu đã có yêu cầu (mở từ danh mục) -> điền loại đã chọn + tìm ngay.
                if (pendingRequest != null)
                {
                    ApplyPendingRequest();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Nhận yêu cầu "Xem tồn kho theo kho" từ danh mục Thuốc/Vật tư (qua RequestStore).
        /// Được gọi cả khi tab UC đang mở sẵn lẫn ngay sau khi tạo mới.
        /// </summary>
        private void OnExternalRequest(HIS.Desktop.LocalStorage.LocalData.MedicineMediStockVerticalRequest request)
        {
            try
            {
                if (this.IsDisposed || request == null) return;
                this.pendingRequest = request;

                if (!this.isLoaded) return; // chưa Load xong -> sẽ áp dụng ở cuối Load

                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new Action(ApplyPendingRequest));
                }
                else
                {
                    ApplyPendingRequest();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Xử lý #2: chọn đúng radio Thuốc/Vật tư, điền loại vào cboType rồi tự động tìm kiếm.
        /// </summary>
        private void ApplyPendingRequest()
        {
            try
            {
                if (pendingRequest == null) return;
                var request = pendingRequest;
                pendingRequest = null;

                isCheckChanging = true;
                chkMedicine.Checked = request.IsMedicine;
                chkMaterial.Checked = !request.IsMedicine;
                isCheckChanging = false;

                LoadTypeData();
                cboType.EditValue = request.TypeId;
                UpdateInformationButtonState();

                PerformSearch();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Init

        private void InitComboData()
        {
            InitBranchCombo();
            InitDateButtons();
            ConfigTypeComboFilter();
        }

        /// <summary>Thêm nút X (Delete) cho 2 ô ngày; nhấn X thì xóa giá trị ô đó.</summary>
        private void InitDateButtons()
        {
            try
            {
                dteFromDate.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Delete));
                dteToDate.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Delete));
                dteFromDate.Properties.ButtonClick += dteFromDate_ButtonClick;
                dteToDate.Properties.ButtonClick += dteToDate_ButtonClick;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dteFromDate_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button != null && e.Button.Kind == ButtonPredefines.Delete)
            {
                dteFromDate.EditValue = null;
            }
        }

        private void dteToDate_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button != null && e.Button.Kind == ButtonPredefines.Delete)
            {
                dteToDate.EditValue = null;
            }
        }

        /// <summary>Cho phép gõ để lọc danh sách loại theo cả Mã + Tên, popup hiện ngay khi gõ.</summary>
        private void ConfigTypeComboFilter()
        {
            try
            {
                cboType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboType.Properties.ImmediatePopup = true;
                cboType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                // Tăng kích thước popup khi xổ danh sách (rộng + cao hơn)
                cboType.Properties.PopupFormSize = new Size(650, 450);
                // Cột trong popup co giãn theo tỉ lệ (Mã = 1/3 Tên, set ở LoadTypeData)
                cboTypeView.OptionsView.ColumnAutoWidth = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Chọn mặc định: ≥2 chi nhánh -> "(Tất cả chi nhánh)"; 1 chi nhánh -> chính nó.</summary>
        private void SelectDefaultBranch()
        {
            if (this.branchList != null && this.branchList.Count >= 2)
            {
                cboBranch.EditValue = ALL_BRANCH_ID; // "(Tất cả chi nhánh)"
            }
            else if (this.branchList != null && this.branchList.Count == 1)
            {
                cboBranch.EditValue = this.branchList[0].ID;
            }
            else
            {
                cboBranch.EditValue = null;
            }
        }

        private void InitBranchCombo()
        {
            try
            {
                this.branchList = BackendDataWorker.Get<HIS_BRANCH>()
                    .OrderBy(o => o.BRANCH_NAME)
                    .ToList();

                List<HIS_BRANCH> displayList = new List<HIS_BRANCH>();
                // Từ 2 chi nhánh trở lên: bổ sung dòng "(Tất cả chi nhánh)" ở đầu
                if (this.branchList != null && this.branchList.Count >= 2)
                {
                    displayList.Add(new HIS_BRANCH()
                    {
                        ID = ALL_BRANCH_ID,
                        BRANCH_NAME = "(Tất cả chi nhánh)",
                        BRANCH_CODE = ""
                    });
                }
                if (this.branchList != null)
                {
                    displayList.AddRange(this.branchList);
                }

                cboBranch.Properties.DataSource = displayList;
                cboBranch.Properties.DisplayMember = "BRANCH_NAME";
                cboBranch.Properties.ValueMember = "ID";

                cboBranchView.Columns.Clear();
                AddLookupColumn(cboBranchView, "BRANCH_CODE", "Mã", 30);
                AddLookupColumn(cboBranchView, "BRANCH_NAME", "Tên chi nhánh", 70);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            // TODO: gán caption đa ngôn ngữ cho control nếu cần
        }

        private void InitTabIndex()
        {
            // TODO: thiết lập thứ tự tab cho control nếu cần
        }

        private void SetDefaultValue()
        {
            try
            {
                // Chi nhánh mặc định
                SelectDefaultBranch();

                // Loại mặc định: Thuốc (đặt dưới cờ chặn để không kích hoạt LoadTypeData 2 lần)
                isCheckChanging = true;
                chkMedicine.Checked = true;
                chkMaterial.Checked = false;
                isCheckChanging = false;

                LoadTypeData();
                UpdateInformationButtonState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {
            isNotLoadWhileChangeControlStateInFirst = true;
            // TODO: đọc trạng thái checkbox local qua ControlStateWorker nếu cần
            isNotLoadWhileChangeControlStateInFirst = false;
        }

        #endregion

        #region Combo Loại thuốc / vật tư

        /// <summary>
        /// Nạp danh mục Loại thuốc hoặc Loại vật tư cho cboType theo checkbox đang tích.
        /// </summary>
        private void LoadTypeData()
        {
            try
            {
                cboType.EditValue = null;
                cboTypeView.Columns.Clear();

                if (chkMedicine.Checked)
                {
                    // Bind theo view V_HIS_MEDICINE_TYPE (khóa = ID = MEDICINE_TYPE_ID phía API)
                    var data = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>()
                        .Where(o => o.IS_ACTIVE == 1)
                        .OrderBy(o => o.MEDICINE_TYPE_NAME)
                        .ToList();
                    cboType.Properties.DataSource = data;
                    cboType.Properties.DisplayMember = "MEDICINE_TYPE_NAME";
                    cboType.Properties.ValueMember = "ID";
                    AddLookupColumn(cboTypeView, "MEDICINE_TYPE_CODE", "Mã", 100);
                    AddLookupColumn(cboTypeView, "MEDICINE_TYPE_NAME", "Tên loại thuốc", 300);
                }
                else if (chkMaterial.Checked)
                {
                    // Bind theo view V_HIS_MATERIAL_TYPE (khóa = ID = MATERIAL_TYPE_ID phía API)
                    var data = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>()
                        .Where(o => o.IS_ACTIVE == 1)
                        .OrderBy(o => o.MATERIAL_TYPE_NAME)
                        .ToList();
                    cboType.Properties.DataSource = data;
                    cboType.Properties.DisplayMember = "MATERIAL_TYPE_NAME";
                    cboType.Properties.ValueMember = "ID";
                    AddLookupColumn(cboTypeView, "MATERIAL_TYPE_CODE", "Mã", 100);
                    AddLookupColumn(cboTypeView, "MATERIAL_TYPE_NAME", "Tên loại vật tư", 300);
                }
                else
                {
                    cboType.Properties.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // Xử lý #7: đổi Thuốc <-> Vật tư => xóa kết quả hiện tại, giữ nguyên bộ lọc ngày, chờ nhấn Tìm kiếm.
        private void chkMedicine_CheckedChanged(object sender, EventArgs e)
        {
            if (isCheckChanging) return;
            try
            {
                isCheckChanging = true;
                if (chkMedicine.Checked)
                {
                    chkMaterial.Checked = false;
                }
                isCheckChanging = false;

                LoadTypeData();
                ClearGridResult();
                UpdateInformationButtonState();
            }
            catch (Exception ex)
            {
                isCheckChanging = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkMaterial_CheckedChanged(object sender, EventArgs e)
        {
            if (isCheckChanging) return;
            try
            {
                isCheckChanging = true;
                if (chkMaterial.Checked)
                {
                    chkMedicine.Checked = false;
                }
                isCheckChanging = false;

                LoadTypeData();
                ClearGridResult();
                UpdateInformationButtonState();
            }
            catch (Exception ex)
            {
                isCheckChanging = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboType_EditValueChanged(object sender, EventArgs e)
        {
            UpdateInformationButtonState();
        }

        /// <summary>
        /// (Nút "Thông tin" đã được gỡ khỏi giao diện — giữ hàm rỗng để các nơi gọi không phải sửa.
        /// Nếu thêm lại SimpleButton tên btnInformation thì bật lại logic enable tại đây.)
        /// </summary>
        private void UpdateInformationButtonState()
        {
        }

        #endregion

        #region Button Delete (xóa giá trị combo)

        private void cboBranch_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            // Nhấn X ở Chi nhánh -> tự chọn lại "(Tất cả chi nhánh)" (không để trống).
            if (e.Button != null && e.Button.Kind == ButtonPredefines.Delete)
            {
                SelectDefaultBranch();
            }
        }

        private void cboType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button != null && e.Button.Kind == ButtonPredefines.Delete)
            {
                cboType.EditValue = null;
            }
        }

        #endregion

        #region Button Thông tin / Tìm kiếm / Xuất Excel

        private void btnSearch_Click(object sender, EventArgs e)
        {
            PerformSearch();
        }

        /// <summary>Thực hiện tra cứu: validate -> dựng cột -> gọi API tương ứng.</summary>
        private void PerformSearch()
        {
            try
            {
                // Xử lý #3: chưa chọn loại thuốc/vật tư (ô từ khóa trống) -> cảnh báo, không gọi API
                if (cboType.EditValue == null || string.IsNullOrEmpty(cboType.EditValue.ToString()))
                {
                    ShowWarning(chkMaterial.Checked
                        ? "Chưa chọn loại vật tư cần tra cứu"
                        : "Chưa chọn loại thuốc cần tra cứu");
                    return;
                }

                bool hasFrom = dteFromDate.EditValue != null;
                bool hasTo = dteToDate.EditValue != null;
                bool hasDateRange = hasFrom && hasTo;

                // Xử lý #4: từ ngày > đến ngày -> cảnh báo, không gọi API
                if (hasFrom && hasTo)
                {
                    DateTime from = Convert.ToDateTime(dteFromDate.EditValue);
                    DateTime to = Convert.ToDateTime(dteToDate.EditValue);
                    if (from.Date > to.Date)
                    {
                        ShowWarning("Từ ngày phải nhỏ hơn hoặc bằng đến ngày");
                        return;
                    }
                    // TODO: validate khoảng thời gian không vượt giới hạn tối đa (mặc định 5 năm, qua config — chốt key sau).
                }

                WaitingManager.Show();
                BuildGridColumns(hasDateRange);
                if (hasDateRange)
                {
                    // Xử lý #6: có khoảng thời gian -> gọi API mới lấy dữ liệu theo kỳ
                    LoadStockByPeriod();
                }
                else
                {
                    // Xử lý #5: không có khoảng thời gian -> gọi API hiện có lấy tồn kho hiện tại
                    LoadCurrentStock();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // Template xuất (Tmp/Exp): single-tag header + band <#Stock.PROP;> (row dữ liệu) + single-tag tổng <#TOTAL_*;>.
        const string EXPORT_TEMPLATE_NAME = "EXPORT_TONKHO_THEOKHO.xlsx";

        /// <summary>
        /// Xuất Excel theo cơ chế template Inventec (giống chức năng dùng EXPORT_TUBENHAN.xlsx — MedicalStoreV2):
        /// Store.ReadTemplate -> ProcessSingleTag (header + tổng) -> ProcessObjectTag.AddObjectData("Stock", list) -> OutFile.
        /// Engine tự bung dòng theo band <#Stock.PROP;>, không fill thủ công từng ô.
        /// </summary>
        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                // Xử lý #10: chưa có kết quả -> thông báo, không tạo file
                if (this.gridData == null || this.gridData.Count == 0)
                {
                    ShowWarning("Chưa có dữ liệu để xuất");
                    return;
                }

                string templatePath = System.IO.Path.Combine(
                    System.Windows.Forms.Application.StartupPath, "Tmp", "Exp", EXPORT_TEMPLATE_NAME);
                if (!System.IO.File.Exists(templatePath))
                {
                    ShowWarning("Không tìm thấy template xuất Excel:\n" + templatePath);
                    return;
                }

                // Cho người dùng chọn ổ/thư mục + tên file để lưu
                string defaultName = "TonKhoTheoKho_" + MakeSafeFileName(cboType.Text) + "_"
                    + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                string outPath;
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Title = "Chọn nơi lưu file Excel";
                    sfd.Filter = "Excel 2007 trở lên (*.xlsx)|*.xlsx|Excel 97-2003 (*.xls)|*.xls";
                    sfd.FileName = defaultName;
                    sfd.DefaultExt = "xlsx";
                    sfd.OverwritePrompt = true;
                    sfd.RestoreDirectory = true;
                    if (sfd.ShowDialog(this) != DialogResult.OK)
                        return; // người dùng bấm Hủy
                    outPath = sfd.FileName;
                }

                WaitingManager.Show();

                Inventec.Common.FlexCellExport.Store store = new Inventec.Common.FlexCellExport.Store(true);
                if (!store.ReadTemplate(System.IO.Path.GetFullPath(templatePath)))
                {
                    WaitingManager.Hide();
                    ShowWarning("Không đọc được template (có thể đang được mở):\n" + templatePath);
                    return;
                }
                store.SetCommonFunctions();

                // Tag đơn: header (đơn vị, loại, từ/đến ngày...) + dòng tổng.
                new Inventec.Common.FlexCellExport.ProcessSingleTag().ProcessData(store, BuildSingleTags());

                // Band dữ liệu: key "Stock" <-> tag <#Stock.PROP;> (engine tự bung mỗi kho 1 dòng).
                new Inventec.Common.FlexCellExport.ProcessObjectTag().AddObjectData(store, "Stock", BuildExportRows());

                bool ok = store.OutFile(outPath);
                WaitingManager.Hide();

                if (ok)
                {
                    if (XtraMessageBox.Show("Xuất file thành công. Mở file ngay?", "Thông báo",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(outPath);
                    }
                }
                else
                {
                    ShowWarning("Xuất file Excel thất bại.");
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                ShowWarning("Lỗi khi xuất Excel: " + ex.Message);
            }
        }

        /// <summary>Tag đơn cho template: gộp header (BuildExportKeys) + 4 ô tổng (TOTAL_*).</summary>
        private Dictionary<string, object> BuildSingleTags()
        {
            Dictionary<string, object> d = new Dictionary<string, object>();
            foreach (var kv in BuildExportKeys())
            {
                d[kv.Key] = kv.Value;
            }

            bool hasDateRange = dteFromDate.EditValue != null && dteToDate.EditValue != null;
            // Không chọn kỳ: chỉ có "Tồn hiện tại" -> đổ vào cột Tồn cuối kỳ; các ô tổng còn lại để trống.
            d["TOTAL_BEGIN"] = hasDateRange ? (object)gridData.Sum(r => r.BeginAmount ?? 0) : "";
            d["TOTAL_IN"] = hasDateRange ? (object)gridData.Sum(r => r.InAmount ?? 0) : "";
            d["TOTAL_OUT"] = hasDateRange ? (object)gridData.Sum(r => r.ExportAmount ?? 0) : "";
            d["TOTAL_END"] = hasDateRange
                ? gridData.Sum(r => r.EndAmount ?? 0)
                : gridData.Sum(r => r.Amount ?? 0);
            return d;
        }

        /// <summary>
        /// List bind vào band "Stock". Không chọn kỳ: Tồn hiện tại đổ vào EndAmount (cột Tồn cuối kỳ),
        /// các cột đầu/nhập/xuất để trống — giữ đúng cấu trúc template 4 cột.
        /// </summary>
        private List<MediStockSummaryVerticalADO> BuildExportRows()
        {
            bool hasDateRange = dteFromDate.EditValue != null && dteToDate.EditValue != null;
            List<MediStockSummaryVerticalADO> list = new List<MediStockSummaryVerticalADO>();
            foreach (var r in gridData)
            {
                list.Add(new MediStockSummaryVerticalADO
                {
                    Stt = r.Stt,
                    MediStockName = r.MediStockName,
                    MediStockCode = r.MediStockCode,
                    BeginAmount = hasDateRange ? r.BeginAmount : null,
                    InAmount = hasDateRange ? r.InAmount : null,
                    ExportAmount = hasDateRange ? r.ExportAmount : null,
                    EndAmount = hasDateRange ? r.EndAmount : r.Amount
                });
            }
            return list;
        }

        /// <summary>Tập key header (đổ vào các tag &lt;#KEY;&gt; của template).</summary>
        private Dictionary<string, string> BuildExportKeys()
        {
            Dictionary<string, string> d = new Dictionary<string, string>();

            HIS_BRANCH br = null;
            if (cboBranch.EditValue != null && branchList != null)
            {
                long bid = Convert.ToInt64(cboBranch.EditValue);
                br = branchList.FirstOrDefault(o => o.ID == bid);
            }
            if (br == null && branchList != null) br = branchList.FirstOrDefault();

            d["ORGANIZATION_NAME"] = br != null ? br.BRANCH_NAME : "";
            d["PARENT_ORGANIZATION_NAME"] = br != null ? br.PARENT_ORGANIZATION_NAME : "";
            d["CURRENT_DATE_SEPARATE_STR"] = System.DateTime.Now.ToString("dd/MM/yyyy");
            d["TIME_FROM_STR"] = dteFromDate.EditValue != null ? Convert.ToDateTime(dteFromDate.EditValue).ToString("dd/MM/yyyy") : "";
            d["TIME_TO_STR"] = dteToDate.EditValue != null ? Convert.ToDateTime(dteToDate.EditValue).ToString("dd/MM/yyyy") : "";

            string code = "", name = "", unit = "", manufacturer = "", national = "";
            long typeId = cboType.EditValue != null ? Convert.ToInt64(cboType.EditValue) : 0;
            if (chkMedicine.Checked)
            {
                var t = (cboType.Properties.DataSource as List<V_HIS_MEDICINE_TYPE>);
                var sel = t != null ? t.FirstOrDefault(o => o.ID == typeId) : null;
                if (sel != null)
                {
                    code = sel.MEDICINE_TYPE_CODE;
                    name = sel.MEDICINE_TYPE_NAME;
                    unit = sel.SERVICE_UNIT_NAME;
                    manufacturer = sel.MANUFACTURER_NAME;
                }
            }
            else
            {
                var t = (cboType.Properties.DataSource as List<V_HIS_MATERIAL_TYPE>);
                var sel = t != null ? t.FirstOrDefault(o => o.ID == typeId) : null;
                if (sel != null)
                {
                    code = sel.MATERIAL_TYPE_CODE;
                    name = sel.MATERIAL_TYPE_NAME;
                    unit = sel.SERVICE_UNIT_NAME;
                    manufacturer = sel.MANUFACTURER_NAME;
                }
            }
            d["MATERIAL_TYPE_CODE"] = code;
            d["MATERIAL_TYPE_NAME"] = name;
            d["SERVICE_UNIT_NAME"] = unit;
            d["MANUFACTURER_NAME"] = manufacturer;
            d["NATIONAL_NAME"] = national; // TODO: lấy nước SX nếu view có cột tương ứng
            return d;
        }

        private string MakeSafeFileName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "BaoCao";
            foreach (char ch in System.IO.Path.GetInvalidFileNameChars())
            {
                name = name.Replace(ch, '_');
            }
            return name.Trim();
        }

        #endregion

        #region Grid

        /// <summary>Mở từ menu: dựng cột chế độ "tồn hiện tại" + grid rỗng, không tự tìm kiếm.</summary>
        private void InitGridDefault()
        {
            BuildGridColumns(false);
            BindGridData(new List<MediStockSummaryVerticalADO>());
        }

        /// <summary>Xóa kết quả đang hiển thị, giữ nguyên cấu trúc cột hiện tại.</summary>
        private void ClearGridResult()
        {
            BindGridData(new List<MediStockSummaryVerticalADO>());
        }

        /// <summary>
        /// Xử lý #5: gọi API hiện có lấy tồn kho hiện tại cho loại đang chọn -> đảo chiều phía client
        /// (mỗi kho 1 hàng) -> cột "Tồn hiện tại".
        /// </summary>
        private void LoadCurrentStock()
        {
            try
            {
                long typeId = Convert.ToInt64(cboType.EditValue);

                // Truyền DANH SÁCH kho (MEDI_STOCK_IDs số nhiều) vào filter để gọi 1 lần cho nhiều kho.
                // Lưu ý: list phải LUÔN có giá trị — "(Tất cả chi nhánh)" -> tất cả kho hoạt động (GetTargetMediStockIds),
                // nếu để rỗng backend không có phạm vi kho -> rỗng. KHÔNG set MEDICINE_TYPE_IDs (lọc loại ở client theo Id,
                // giống AggrApprove/ApproveExpMestBCS).
                List<long> mediStockIds = GetTargetMediStockIds();
                CommonParam param = new CommonParam();
                List<MediStockSummaryVerticalADO> data = new List<MediStockSummaryVerticalADO>();

                if (chkMedicine.Checked)
                {
                    HisMedicineTypeStockViewFilter filter = new HisMedicineTypeStockViewFilter();
                    filter.MEDI_STOCK_IDs = mediStockIds;
                    filter.IS_ACTIVE = true;

                    var rs = new BackendAdapter(param).Get<List<HisMedicineTypeInStockSDO>>(
                        "api/HisMedicineType/GetInStockMedicineType", ApiConsumers.MosConsumer, filter, param);

                    // Đảo chiều client: lọc đúng loại đang chọn (Id) rồi gom theo kho -> mỗi kho 1 hàng.
                    if (rs != null)
                    {
                        data = rs.Where(o => o.Id == typeId && o.MediStockId.HasValue)
                                 .GroupBy(o => o.MediStockId.Value)
                                 .Select(g => new MediStockSummaryVerticalADO
                                 {
                                     MediStockCode = g.First().MediStockCode,
                                     MediStockName = g.First().MediStockName,
                                     Amount = g.Sum(x => x.TotalAmount ?? 0)
                                 })
                                 .OrderBy(o => o.MediStockName)
                                 .ToList();
                    }
                }
                else
                {
                    HisMaterialTypeStockViewFilter filter = new HisMaterialTypeStockViewFilter();
                    filter.MEDI_STOCK_IDs = mediStockIds;
                    filter.IS_ACTIVE = true;

                    var rs = new BackendAdapter(param).Get<List<HisMaterialTypeInStockSDO>>(
                        "api/HisMaterialType/GetInStockMaterialType", ApiConsumers.MosConsumer, filter, param);

                    if (rs != null)
                    {
                        data = rs.Where(o => o.Id == typeId && o.MediStockId.HasValue)
                                 .GroupBy(o => o.MediStockId.Value)
                                 .Select(g => new MediStockSummaryVerticalADO
                                 {
                                     MediStockCode = g.First().MediStockCode,
                                     MediStockName = g.First().MediStockName,
                                     Amount = g.Sum(x => x.TotalAmount ?? 0)
                                 })
                                 .OrderBy(o => o.MediStockName)
                                 .ToList();
                    }
                }

                BindGridData(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Danh sách MEDI_STOCK_ID cần tra: theo chi nhánh đang chọn; nếu "(Tất cả chi nhánh)"/rỗng -> tất cả kho hoạt động.
        /// Luôn trả list có giá trị để truyền MEDI_STOCK_IDs (số nhiều) vào filter — gọi 1 lần cho nhiều kho.
        /// </summary>
        private List<long> GetTargetMediStockIds()
        {
            List<long> ids = GetMediStockIdsByBranch();
            if (ids != null && ids.Count > 0) return ids;

            try
            {
                return BackendDataWorker.Get<HIS_MEDI_STOCK>()
                    .Where(o => o.IS_ACTIVE == 1)
                    .Select(o => o.ID)
                    .Distinct()
                    .ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return new List<long>();
            }
        }

        /// <summary>
        /// Xử lý #6: gọi API mới lấy tồn kho theo kỳ cho loại đang chọn ->
        /// cột Tồn đầu kỳ / Nhập / Xuất / Tồn cuối kỳ.
        /// </summary>
        private void LoadStockByPeriod()
        {
            try
            {
                long typeId = Convert.ToInt64(cboType.EditValue);
                List<long> mediStockIds = GetMediStockIdsByBranch();
                long fromTime = ToYmdHms(Convert.ToDateTime(dteFromDate.EditValue), false); // đầu ngày 000000
                long toTime = ToYmdHms(Convert.ToDateTime(dteToDate.EditValue), true);       // cuối ngày 235959

                List<long> stockIds = (mediStockIds != null && mediStockIds.Count > 0) ? mediStockIds : null;

                CommonParam param = new CommonParam();
                List<StockByWarehouseResultSDO> rs;
                if (chkMedicine.Checked)
                {
                    HisMedicineTypeStockByWarehouseReqSDO req = new HisMedicineTypeStockByWarehouseReqSDO();
                    req.MEDICINE_TYPE_ID = typeId;
                    req.MEDI_STOCK_IDs = stockIds;
                    req.FROM_TIME = fromTime;
                    req.TO_TIME = toTime;
                    rs = new BackendAdapter(param).Post<List<StockByWarehouseResultSDO>>(
                        "api/HisMedicineType/GetStockByWarehouseVertical", ApiConsumers.MosConsumer, req, param);
                }
                else
                {
                    HisMaterialTypeStockByWarehouseReqSDO req = new HisMaterialTypeStockByWarehouseReqSDO();
                    req.MATERIAL_TYPE_ID = typeId;
                    req.MEDI_STOCK_IDs = stockIds;
                    req.FROM_TIME = fromTime;
                    req.TO_TIME = toTime;
                    rs = new BackendAdapter(param).Post<List<StockByWarehouseResultSDO>>(
                        "api/HisMaterialType/GetStockByWarehouseVertical", ApiConsumers.MosConsumer, req, param);
                }

                List<MediStockSummaryVerticalADO> data = new List<MediStockSummaryVerticalADO>();
                if (rs != null)
                {
                    data = rs.OrderBy(o => o.MEDI_STOCK_NAME)
                             .Select(o => new MediStockSummaryVerticalADO
                             {
                                 MediStockCode = o.MEDI_STOCK_CODE,
                                 MediStockName = o.MEDI_STOCK_NAME,
                                 BeginAmount = o.OPEN_QUANTITY,
                                 InAmount = o.IN_QUANTITY,
                                 ExportAmount = o.OUT_QUANTITY,
                                 EndAmount = o.CLOSE_QUANTITY
                             })
                             .ToList();
                }

                BindGridData(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Đổi DateTime -> số yyyyMMddHHmmss (kiểu thời gian chuẩn HIS).
        /// isEndOfDay = false: lấy 00:00:00 (đầu kỳ); true: lấy 23:59:59 (cuối kỳ).
        /// </summary>
        private long ToYmdHms(DateTime date, bool isEndOfDay)
        {
            string suffix = isEndOfDay ? "235959" : "000000";
            return long.Parse(date.ToString("yyyyMMdd") + suffix);
        }

        /// <summary>
        /// Quy đổi Chi nhánh đang chọn -> danh sách MEDI_STOCK_ID (để truyền API).
        /// "(Tất cả chi nhánh)" hoặc rỗng -> trả về list rỗng = tất cả kho hoạt động.
        /// </summary>
        private List<long> GetMediStockIdsByBranch()
        {
            List<long> result = new List<long>();
            try
            {
                if (cboBranch.EditValue == null) return result;
                long branchId = Convert.ToInt64(cboBranch.EditValue);
                if (branchId == ALL_BRANCH_ID) return result; // tất cả kho

                // MEDI_STOCK không có BRANCH_ID trực tiếp -> qua ROOM -> BRANCH
                var roomIds = BackendDataWorker.Get<V_HIS_ROOM>()
                    .Where(o => o.BRANCH_ID == branchId)
                    .Select(o => o.ID)
                    .ToList();

                result = BackendDataWorker.Get<HIS_MEDI_STOCK>()
                    .Where(o => o.IS_ACTIVE == 1 && roomIds.Contains(o.ROOM_ID))
                    .Select(o => o.ID)
                    .Distinct()
                    .ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>Gán dữ liệu lên grid, đánh lại STT theo thứ tự hiện hành.</summary>
        private void BindGridData(List<MediStockSummaryVerticalADO> data)
        {
            this.gridData = data ?? new List<MediStockSummaryVerticalADO>();
            RenumberStt();
            grdData.DataSource = this.gridData;
            grdData.RefreshDataSource();
        }

        private void RenumberStt()
        {
            if (this.gridData == null) return;
            for (int i = 0; i < this.gridData.Count; i++)
            {
                this.gridData[i].Stt = i + 1;
            }
        }

        /// <summary>
        /// Dựng cột GridControl theo chế độ lọc thời gian:
        /// - Không chọn từ/đến ngày: STT, Tên kho, Mã kho, Tồn hiện tại (đậm, màu maroon).
        /// - Có chọn từ/đến ngày: STT, Tên kho, Mã kho, Tồn đầu kỳ, Số lượng nhập, Số lượng xuất, Tồn cuối kỳ.
        /// Dòng footer: "Tổng cộng toàn viện" + tổng số lượng tồn.
        /// </summary>
        private void BuildGridColumns(bool hasDateRange)
        {
            gridViewData.Columns.Clear();
            gridViewData.OptionsView.ShowFooter = true;
            gridViewData.OptionsView.ColumnAutoWidth = true; // Tên/Mã co giãn lấp phần còn lại
            gridViewData.OptionsBehavior.Editable = false;
            // Không cho cell đang focus đổi màu chữ (mặc định DevExpress vẽ chữ focused cell về đen).
            gridViewData.OptionsSelection.EnableAppearanceFocusedCell = false;
            SetupFooterAppearance();
            totalColumns.Clear();

            int idx = 0;

            // STT: cố định 50, căn giữa cả header + cell
            GridColumn colStt = AddGridColumn("Stt", "STT", idx++, 50);
            SetFixedWidth(colStt, 50);
            SetCellAlign(colStt, HorzAlignment.Center);
            SetHeaderAlign(colStt, HorzAlignment.Center);

            // Tên kho: CO GIÃN (lấp phần còn lại), sắp xếp ABC
            GridColumn colName = AddGridColumn("MediStockName", "Tên kho", idx++, 300);
            colName.OptionsColumn.AllowSort = DefaultBoolean.True; // Xử lý #9: sắp xếp ABC

            // Mã kho: cố định 250
            GridColumn colCode = AddGridColumn("MediStockCode", "Mã kho", idx++, 250);
            SetFixedWidth(colCode, 250);

            if (!hasDateRange)
            {
                // Tồn hiện tại cố định 200; phần còn lại dành cho Tên kho
                GridColumn colAmount = AddGridColumn("Amount", "Tồn hiện tại", idx++, 200);
                SetFixedWidth(colAmount, 200);
                ApplyNumericFormat(colAmount);
                ApplyForeColor(colAmount, COLOR_AMOUNT, true);
                totalColumns.Add(new TotalColInfo { FieldName = "Amount", Color = COLOR_AMOUNT });
            }
            else
            {
                // 4 cột SL: chia đều + rộng (cố định 250 mỗi cột); Tên kho lấp phần còn lại
                const int wNum = 250;

                GridColumn colBegin = AddGridColumn("BeginAmount", "Tồn đầu kỳ", idx++, wNum);
                SetFixedWidth(colBegin, wNum);
                ApplyNumericFormat(colBegin);
                totalColumns.Add(new TotalColInfo { FieldName = "BeginAmount", Color = Color.Empty });

                GridColumn colIn = AddGridColumn("InAmount", "SL nhập trong kỳ", idx++, wNum);
                SetFixedWidth(colIn, wNum);
                ApplyNumericFormat(colIn);
                ApplyForeColor(colIn, COLOR_IN, false);
                colIn.ToolTip = "Số lượng nhập trong kỳ";
                totalColumns.Add(new TotalColInfo { FieldName = "InAmount", Color = COLOR_IN });

                GridColumn colExport = AddGridColumn("ExportAmount", "SL xuất trong kỳ", idx++, wNum);
                SetFixedWidth(colExport, wNum);
                ApplyNumericFormat(colExport);
                ApplyForeColor(colExport, COLOR_OUT, false);
                colExport.ToolTip = "Số lượng xuất trong kỳ";
                totalColumns.Add(new TotalColInfo { FieldName = "ExportAmount", Color = COLOR_OUT });

                GridColumn colEnd = AddGridColumn("EndAmount", "Tồn cuối kỳ", idx++, wNum);
                SetFixedWidth(colEnd, wNum);
                ApplyNumericFormat(colEnd);
                ApplyForeColor(colEnd, COLOR_AMOUNT, false); // Tồn cuối kỳ: maroon
                totalColumns.Add(new TotalColInfo { FieldName = "EndAmount", Color = COLOR_AMOUNT });
            }
        }

        #endregion

        #region Helper

        private void ShowWarning(string message)
        {
            XtraMessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private GridColumn AddGridColumn(string fieldName, string caption, int visibleIndex, int width)
        {
            GridColumn col = gridViewData.Columns.AddField(fieldName);
            col.Caption = caption;
            col.Visible = true;
            col.VisibleIndex = visibleIndex;
            col.Width = width;
            col.OptionsColumn.AllowEdit = false;
            col.OptionsColumn.AllowSort = DefaultBoolean.False;
            return col;
        }

        private void ApplyNumericFormat(GridColumn col)
        {
            col.DisplayFormat.FormatType = FormatType.Numeric;
            col.DisplayFormat.FormatString = "n2";
            SetCellAlign(col, HorzAlignment.Far);   // cell căn phải
            SetHeaderAlign(col, HorzAlignment.Far); // header căn phải
        }

        private void SetCellAlign(GridColumn col, HorzAlignment align)
        {
            col.AppearanceCell.TextOptions.HAlignment = align;
            col.AppearanceCell.Options.UseTextOptions = true;
        }

        private void SetHeaderAlign(GridColumn col, HorzAlignment align)
        {
            col.AppearanceHeader.TextOptions.HAlignment = align;
            col.AppearanceHeader.Options.UseTextOptions = true;
        }

        private void ApplyForeColor(GridColumn col, Color color, bool bold)
        {
            // Tô màu cả CELL lẫn HEADER
            col.AppearanceCell.ForeColor = color;
            col.AppearanceCell.Options.UseForeColor = true;
            col.AppearanceHeader.ForeColor = color;
            col.AppearanceHeader.Options.UseForeColor = true;
            if (bold)
            {
                col.AppearanceCell.Font = new Font(this.Font, FontStyle.Bold);
                col.AppearanceCell.Options.UseFont = true;
            }
        }

        private void AddLookupColumn(GridView view, string fieldName, string caption, int width)
        {
            GridColumn col = view.Columns.AddField(fieldName);
            col.Caption = caption;
            col.Visible = true;
            col.VisibleIndex = view.Columns.Count - 1;
            col.Width = width;
        }

        /// <summary>Cố định độ rộng cột (không co giãn theo ColumnAutoWidth).</summary>
        private void SetFixedWidth(GridColumn col, int width)
        {
            col.Width = width;
            col.MinWidth = width;
            col.MaxWidth = width;
        }

        /// <summary>Dòng tổng (footer): chữ đậm + size lớn hơn.</summary>
        private void SetupFooterAppearance()
        {
            gridViewData.Appearance.FooterPanel.Font = new Font(this.Font.FontFamily, this.Font.Size + 3f, FontStyle.Bold);
            gridViewData.Appearance.FooterPanel.Options.UseFont = true;
            gridViewData.Appearance.FooterPanel.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            gridViewData.Appearance.FooterPanel.Options.UseTextOptions = true;
        }

        /// <summary>Tổng của 1 cột số liệu trên dữ liệu đang hiển thị.</summary>
        private decimal ComputeColumnSum(string fieldName)
        {
            decimal sum = 0;
            if (gridData == null) return sum;
            foreach (var r in gridData)
            {
                switch (fieldName)
                {
                    case "Amount": sum += r.Amount ?? 0; break;
                    case "BeginAmount": sum += r.BeginAmount ?? 0; break;
                    case "InAmount": sum += r.InAmount ?? 0; break;
                    case "ExportAmount": sum += r.ExportAmount ?? 0; break;
                    case "EndAmount": sum += r.EndAmount ?? 0; break;
                }
            }
            return sum;
        }

        /// <summary>
        /// Vẽ dòng tổng (chữ đậm + lớn theo FooterPanel):
        /// - Nhãn "Tổng cộng toàn viện" trải (merge) các cột trước cột số liệu đầu tiên.
        /// - Mỗi cột số liệu 1 ô tổng, đặt đúng độ rộng cột, màu chữ theo cột tương ứng.
        /// </summary>
        /// <summary>
        /// Áp lại màu chữ theo cột cho mọi dòng (kể cả dòng đang focus/được chọn) để màu không bị
        /// FocusedRow/SelectedRow appearance đè về đen. Màu lấy từ totalColumns (In=xanh, Out=cam, Amount/End=maroon).
        /// </summary>
        private void GridViewData_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                if (totalColumns == null || e.Column == null) return;
                TotalColInfo tc = totalColumns.FirstOrDefault(t => t.FieldName == e.Column.FieldName);
                if (tc != null && !tc.Color.IsEmpty)
                {
                    e.Appearance.ForeColor = tc.Color;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GridViewData_CustomDrawFooter(object sender, RowObjectCustomDrawEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view == null) return;
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridViewInfo viewInfo =
                    view.GetViewInfo() as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridViewInfo;
                if (viewInfo == null) return;

                Rectangle footer = e.Bounds;
                e.Appearance.FillRectangle(e.Cache, footer);

                if (totalColumns == null || totalColumns.Count == 0) { e.Handled = true; return; }

                Font font = e.Appearance.Font;
                Color defFore = e.Appearance.GetForeColor();

                using (StringFormat sf = new StringFormat())
                {
                    sf.Alignment = StringAlignment.Far;       // căn phải
                    sf.LineAlignment = StringAlignment.Center;
                    sf.FormatFlags = StringFormatFlags.NoWrap;

                    int firstLeft = footer.Right;
                    foreach (TotalColInfo tc in totalColumns)
                    {
                        GridColumn col = view.Columns[tc.FieldName];
                        if (col == null || !col.Visible) continue;

                        Rectangle b = viewInfo.ColumnsInfo[col].Bounds; // X/Width của cột
                        if (b.Left < firstLeft) firstLeft = b.Left;

                        decimal s = ComputeColumnSum(tc.FieldName);
                        Color c = tc.Color.IsEmpty ? defFore : tc.Color;
                        using (SolidBrush br = new SolidBrush(c))
                        {
                            Rectangle vr = new Rectangle(b.Left, footer.Top, Math.Max(0, b.Width - 6), footer.Height);
                            e.Cache.DrawString(s.ToString("n2"), font, br, vr, sf);
                        }
                    }

                    // Nhãn trải từ mép trái tới cột số liệu đầu tiên
                    Rectangle lbl = new Rectangle(footer.Left, footer.Top,
                        Math.Max(0, firstLeft - footer.Left - 8), footer.Height);
                    using (SolidBrush brLbl = new SolidBrush(defFore))
                    {
                        e.Cache.DrawString("Tổng cộng toàn viện", font, brLbl, lbl, sf);
                    }
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion
    }
}
