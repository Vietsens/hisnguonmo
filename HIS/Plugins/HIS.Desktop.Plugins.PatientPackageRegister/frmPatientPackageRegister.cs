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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PatientPackageRegister
{
    public partial class frmPatientPackageRegister : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module Module { get; set; }

        // Input data truyền vào từ chức năng khác
        private HIS_PATIENT inputPatient;
        private HIS_PATIENT_PACKAGE inputPatientPackage;

        // True = sửa gói đã có, False = đăng ký mới
        private bool isEditMode;

        // Mã trạng thái gói (đồng bộ STATUS_CODE trong HIS_PATIENT_PACKAGE)
        private const string STATUS_CODE__REGISTERED = "REGISTERED";
        private const string STATUS_CODE__IN_USE = "IN_USE";
        private const string STATUS_CODE__LOCKED = "LOCKED";

        // Key HisConfig: mã đối tượng thanh toán Viện phí (mặc định cho Đối tượng TT)
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__HOSPITAL_FEE = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE";

        // Giá trị ảo cho mục "Tất cả" trong cboLoaiDV
        private const long SERVICE_TYPE_ID__ALL = -1;

        // Mã loại phiếu in (SAR report) cho phiếu gói dịch vụ
        private const string PRINT_TYPE_CODE__MPS000514 = "Mps000514";

        // Số ID tối đa mỗi lần gọi API lấy lô thuốc/vật tư — ngắt lô tránh URI (GET) quá dài
        private const int MEDI_MATE_QUERY_CHUNK_SIZE = 500;

        // Chặn nạp lại danh mục dịch vụ trong lúc đang khởi tạo form
        private bool isFormLoading = true;

        // Nút "+" trên lưới danh mục dịch vụ
        private RepositoryItemButtonEdit repoBtnAddService;

        // Danh sách dịch vụ đã thêm vào gói (nguồn dữ liệu grdDichVuTrongGoi)
        private System.ComponentModel.BindingList<PackageServiceADO> selectedPackageServices = new System.ComponentModel.BindingList<PackageServiceADO>();

        // Snapshot chi tiết gói gốc (khi sửa) — để tính danh sách sửa/xóa/thêm khi lưu
        private List<HIS_PATIENT_PACKAGE_DT> originalPackageDetails = new List<HIS_PATIENT_PACKAGE_DT>();

        // NUM_ORDER của loại dịch vụ (HIS_SERVICE_TYPE) theo tên — để sắp xếp nhóm cha lưới danh mục
        private Dictionary<string, long> serviceTypeNumOrderByName = new Dictionary<string, long>();

        // Tên loại dịch vụ (HIS_SERVICE_TYPE.SERVICE_TYPE_NAME) theo ID — gán nhóm cho dòng thuốc/vật tư lấy từ type catalog
        private Dictionary<long, string> serviceTypeNameById = new Dictionary<long, string>();

        // Validate trường bắt buộc (cột NOT NULL của HIS_PATIENT_PACKAGE) + chặn số ký tự theo độ dài cột
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();

        // Tên đối tượng thanh toán (HIS_PATIENT_TYPE.PATIENT_TYPE_NAME) theo ID — hiển thị ở popup danh sách gói
        private Dictionary<long, string> patientTypeNameById = new Dictionary<long, string>();

        // Danh mục thuốc/vật tư theo SERVICE_ID + tập SERVICE_TYPE_ID từng loại — dựng 1 lần từ cache RAM
        // (định tuyến đúng khi 1 SERVICE_ID trùng giữa 2 loại). Hiển thị danh mục: LAST_EXP_PRICE ?? LAST_IMP_PRICE.
        private Dictionary<long, V_HIS_MEDICINE_TYPE> medTypeBySvcId;
        private Dictionary<long, V_HIS_MATERIAL_TYPE> matTypeBySvcId;
        private HashSet<long> medServiceTypeIds;
        private HashSet<long> matServiceTypeIds;
        // Nhớ đơn giá lô gần nhất (IMP_PRICE) đã truy vấn từ HIS_MEDICINE/HIS_MATERIAL — tránh gọi API lại
        private Dictionary<long, decimal> medLoImpPriceBySvcId = new Dictionary<long, decimal>();
        private Dictionary<long, decimal> matLoImpPriceBySvcId = new Dictionary<long, decimal>();
        // Đánh dấu SERVICE_ID đã truy vấn lô (cả khi không có lô) để không gọi API lại ở các lần nạp danh mục sau
        private HashSet<long> medLoQueriedSvcIds = new HashSet<long>();
        private HashSet<long> matLoQueriedSvcIds = new HashSet<long>();

        public frmPatientPackageRegister()
        {
            InitializeComponent();
        }

        public frmPatientPackageRegister(Inventec.Desktop.Common.Modules.Module _Module)
            : this(_Module, null, null)
        {
        }

        public frmPatientPackageRegister(Inventec.Desktop.Common.Modules.Module _Module, HIS_PATIENT _patient, HIS_PATIENT_PACKAGE _patientPackage)
            : base(_Module)
        {
            InitializeComponent();
            SetIcon();
            this.Module = _Module;
            this.inputPatient = _patient;
            this.inputPatientPackage = _patientPackage;
            this.isEditMode = _patientPackage != null;
            this.AddBarManager(this.barManager1);
        }

        private void frmPatientPackageRegister_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                LoadMauGoi();
                LoadDoiTuongTT();
                LoadTrangThai();
                LoadLoaiDV();
                SetupCatalogGrid();
                SetupPackageDetailGrid();
                SetupRequiredValidation();
                dteNgayDangKy.EditValueChanged += dteNgayDangKy_EditValueChanged;
                // Nút "X" (Delete) trên các GridLookUp → clear về mặc định
                cboMauGoi.Properties.ButtonClick += Cbo_ClearButtonClick;
                cboDoiTuongTT.Properties.ButtonClick += Cbo_ClearButtonClick;
                cboTrangThai.Properties.ButtonClick += Cbo_ClearButtonClick;
                cboLoaiDV.Properties.ButtonClick += Cbo_ClearButtonClick;
                // Nhấn mũi tên dropdown của nút Danh sách gói → cùng hành vi với bấm nút (hiện danh sách gói)
                btnDanhSachGoi.ArrowButtonClick += btnDanhSachGoi_Click;
                // Ô từ khóa danh mục DV: chỉ lọc khi nhấn Enter (tránh lag)
                txtTimKiemDV.KeyDown += txtTimKiemDV_KeyDown;
                ApplyInputData();
                LoadServiceCatalog();
                isFormLoading = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Load danh sách mẫu gói (HIS_PACKAGE) đang hoạt động + là mẫu cho bệnh nhân
        /// (IS_ACTIVE = 1 AND IS_PATIENT_TEMP = 1) vào GridLookUpEdit cboMauGoi.
        /// Lấy từ cache BackendDataWorker (không gọi API).
        /// </summary>
        private void LoadMauGoi()
        {
            try
            {
                List<HIS_PACKAGE> packages = BackendDataWorker.Get<HIS_PACKAGE>();
                if (packages != null)
                {
                    packages = packages
                        .Where(p => p.IS_ACTIVE == 1 && p.IS_PATIENT_TEMP == 1)
                        .ToList();
                }

                cboMauGoi.Properties.DataSource = packages;
                cboMauGoi.Properties.ValueMember = "ID";
                cboMauGoi.Properties.DisplayMember = "PACKAGE_NAME";
                cboMauGoi.Properties.NullText = "";
                // Hiển thị 2 cột: Mã + Tên
                ConfigCodeNameColumns(gridViewMauGoi, "PACKAGE_CODE", "PACKAGE_NAME");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Load danh sách đối tượng thanh toán (HIS_PATIENT_TYPE) đang hoạt động vào
        /// cboDoiTuongTT, hiển thị 2 cột Mã + Tên. Mặc định chọn đối tượng Viện phí theo
        /// HisConfig key MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE.
        /// </summary>
        private void LoadDoiTuongTT()
        {
            try
            {
                List<HIS_PATIENT_TYPE> allPatientTypes = BackendDataWorker.Get<HIS_PATIENT_TYPE>() ?? new List<HIS_PATIENT_TYPE>();
                // Map ID → tên (toàn bộ, kể cả đã khóa) để hiển thị tên đối tượng TT của gói đã đăng ký ở popup
                patientTypeNameById = allPatientTypes.GroupBy(o => o.ID).ToDictionary(g => g.Key, g => g.First().PATIENT_TYPE_NAME);

                List<HIS_PATIENT_TYPE> patientTypes = allPatientTypes.Where(o => o.IS_ACTIVE == 1).ToList();

                cboDoiTuongTT.Properties.DataSource = patientTypes;
                cboDoiTuongTT.Properties.ValueMember = "ID";
                cboDoiTuongTT.Properties.DisplayMember = "PATIENT_TYPE_NAME";
                cboDoiTuongTT.Properties.NullText = "";
                ConfigCodeNameColumns(gridViewDoiTuongTT, "PATIENT_TYPE_CODE", "PATIENT_TYPE_NAME");

                // Mặc định đối tượng Viện phí theo cấu hình hệ thống
                SetDefaultDoiTuongTT();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Đặt đối tượng thanh toán mặc định = đối tượng Viện phí theo HisConfig
        /// MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE (dựa trên danh sách đã bind cboDoiTuongTT).
        /// </summary>
        private void SetDefaultDoiTuongTT()
        {
            try
            {
                string feeCode = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__PATIENT_TYPE_CODE__HOSPITAL_FEE);
                List<HIS_PATIENT_TYPE> patientTypes = cboDoiTuongTT.Properties.DataSource as List<HIS_PATIENT_TYPE>;
                if (!string.IsNullOrWhiteSpace(feeCode) && patientTypes != null)
                {
                    HIS_PATIENT_TYPE defaultType = patientTypes.FirstOrDefault(o => o.PATIENT_TYPE_CODE == feeCode);
                    cboDoiTuongTT.EditValue = defaultType != null ? (object)defaultType.ID : null;
                }
                else
                {
                    cboDoiTuongTT.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Click nút "X" (Delete) trên GridLookUpEdit → clear về giá trị mặc định:
        /// Mẫu gói → rỗng; Đối tượng TT → Viện phí; Trạng thái → Đăng ký; Loại dịch vụ → Tất cả.
        /// </summary>
        private void Cbo_ClearButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button == null || e.Button.Kind != ButtonPredefines.Delete) return;

                if (sender == cboMauGoi)
                {
                    cboMauGoi.EditValue = null;
                }
                else if (sender == cboDoiTuongTT)
                {
                    SetDefaultDoiTuongTT();
                }
                else if (sender == cboTrangThai)
                {
                    cboTrangThai.EditValue = STATUS_CODE__REGISTERED;
                }
                else if (sender == cboLoaiDV)
                {
                    cboLoaiDV.EditValue = SERVICE_TYPE_ID__ALL;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Tạo danh sách trạng thái gói cho cboTrangThai: REGISTERED / IN_USE / LOCKED ↔
        /// Đăng ký / Đang sử dụng / Đã khóa. Mặc định REGISTERED khi đăng ký mới.
        /// </summary>
        private void LoadTrangThai()
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("CODE", typeof(string));
                dt.Columns.Add("NAME", typeof(string));
                dt.Rows.Add(STATUS_CODE__REGISTERED, "Đăng ký");
                dt.Rows.Add(STATUS_CODE__IN_USE, "Đang sử dụng");
                dt.Rows.Add(STATUS_CODE__LOCKED, "Đã khóa");

                cboTrangThai.Properties.DataSource = dt;
                cboTrangThai.Properties.ValueMember = "CODE";
                cboTrangThai.Properties.DisplayMember = "NAME";
                cboTrangThai.Properties.NullText = "";

                gridViewTrangThai.OptionsView.ShowGroupPanel = false;
                gridViewTrangThai.OptionsBehavior.AutoPopulateColumns = false;
                gridViewTrangThai.Columns.Clear();
                GridColumn colTrangThai = gridViewTrangThai.Columns.AddVisible("NAME", "Trạng thái");
                colTrangThai.VisibleIndex = 0;
                SetGridHeaderBold(gridViewTrangThai);

                // Gói mới mặc định trạng thái Đăng ký
                if (!isEditMode)
                {
                    cboTrangThai.EditValue = STATUS_CODE__REGISTERED;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Cấu hình View của GridLookUpEdit hiển thị đúng 2 cột: Mã + Tên.
        /// </summary>
        private void ConfigCodeNameColumns(GridView view, string codeField, string nameField)
        {
            try
            {
                if (view == null) return;

                view.OptionsView.ShowGroupPanel = false;
                view.OptionsBehavior.AutoPopulateColumns = false;
                view.Columns.Clear();

                GridColumn colCode = view.Columns.AddVisible(codeField, "Mã");
                colCode.VisibleIndex = 0;
                colCode.Width = 100;

                GridColumn colName = view.Columns.AddVisible(nameField, "Tên");
                colName.VisibleIndex = 1;
                colName.Width = 250;

                SetGridHeaderBold(view);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// In đậm tiêu đề (header) của GridView.
        /// </summary>
        private void SetGridHeaderBold(GridView view)
        {
            if (view == null) return;
            view.Appearance.HeaderPanel.FontStyleDelta = System.Drawing.FontStyle.Bold;
        }

        /// <summary>
        /// Căn giữa cả tiêu đề (header) và nội dung (cell) của 1 cột.
        /// </summary>
        private void SetColumnCenter(GridColumn col)
        {
            if (col == null) return;
            col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        }

        /// <summary>
        /// Cột nút: width cố định (không cho resize) + ẩn tiêu đề.
        /// </summary>
        private void SetButtonColumnFixed(GridColumn col, int width)
        {
            if (col == null) return;
            col.Caption = "";
            col.Width = width;
            col.MinWidth = width;
            col.MaxWidth = width;
            col.OptionsColumn.FixedWidth = true;
            col.OptionsColumn.AllowSize = false;
        }

        /// <summary>
        /// NUM_ORDER của loại dịch vụ theo tên (không có → xếp cuối).
        /// </summary>
        private long GetServiceTypeNumOrder(object serviceTypeName)
        {
            string name = serviceTypeName == null ? "" : serviceTypeName.ToString();
            long order;
            if (serviceTypeNumOrderByName.TryGetValue(name, out order)) return order;
            return long.MaxValue;
        }

        /// <summary>
        /// Sắp xếp nhóm cha (loại dịch vụ) trên lưới danh mục theo NUM_ORDER thay vì theo tên.
        /// </summary>
        private void gvDanhMucDV_CustomColumnSort(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnSortEventArgs e)
        {
            try
            {
                if (e.Column != null && e.Column.FieldName == "SERVICE_TYPE_NAME")
                {
                    e.Result = GetServiceTypeNumOrder(e.Value1).CompareTo(GetServiceTypeNumOrder(e.Value2));
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Dòng cha chỉ hiển thị tên loại (vd "Khám"), bỏ tiền tố "Loại dịch vụ:".
        /// </summary>
        private void gvDanhMucDV_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
                if (view != null && info != null)
                {
                    object value = view.GetGroupRowValue(info.RowHandle);
                    info.GroupText = value == null ? "" : value.ToString();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Load danh sách loại dịch vụ (HIS_SERVICE_TYPE) vào cboLoaiDV, loại trừ Máu và
        /// Suất ăn (IMSys.DbConfig HIS_SERVICE_TYPE.ID__MAU / ID__AN). Thêm mục "Tất cả" ở đầu.
        /// </summary>
        private void LoadLoaiDV()
        {
            try
            {
                List<HIS_SERVICE_TYPE> serviceTypes = BackendDataWorker.Get<HIS_SERVICE_TYPE>();

                // Map tên loại → NUM_ORDER (sắp xếp nhóm cha) và ID → tên loại (gán nhóm cho thuốc/vật tư)
                serviceTypeNumOrderByName.Clear();
                serviceTypeNameById.Clear();
                if (serviceTypes != null)
                {
                    foreach (HIS_SERVICE_TYPE st in serviceTypes)
                    {
                        if (!string.IsNullOrEmpty(st.SERVICE_TYPE_NAME) && !serviceTypeNumOrderByName.ContainsKey(st.SERVICE_TYPE_NAME))
                        {
                            serviceTypeNumOrderByName[st.SERVICE_TYPE_NAME] = st.NUM_ORDER ?? long.MaxValue;
                        }
                        if (!serviceTypeNameById.ContainsKey(st.ID))
                            serviceTypeNameById[st.ID] = st.SERVICE_TYPE_NAME;
                    }
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("ID", typeof(long));
                dt.Columns.Add("SERVICE_TYPE_NAME", typeof(string));
                dt.Rows.Add(SERVICE_TYPE_ID__ALL, "Tất cả");

                if (serviceTypes != null)
                {
                    foreach (HIS_SERVICE_TYPE st in serviceTypes
                        .Where(o => o.IS_ACTIVE == 1
                                 && o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU
                                 && o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN)
                        .OrderBy(o => o.SERVICE_TYPE_NAME))
                    {
                        dt.Rows.Add(st.ID, st.SERVICE_TYPE_NAME);
                    }
                }

                cboLoaiDV.Properties.DataSource = dt;
                cboLoaiDV.Properties.ValueMember = "ID";
                cboLoaiDV.Properties.DisplayMember = "SERVICE_TYPE_NAME";
                cboLoaiDV.Properties.NullText = "";

                gridViewLoaiDV.OptionsView.ShowGroupPanel = false;
                gridViewLoaiDV.OptionsBehavior.AutoPopulateColumns = false;
                gridViewLoaiDV.Columns.Clear();
                GridColumn colLoai = gridViewLoaiDV.Columns.AddVisible("SERVICE_TYPE_NAME", "Loại dịch vụ");
                colLoai.VisibleIndex = 0;
                SetGridHeaderBold(gridViewLoaiDV);

                cboLoaiDV.EditValue = SERVICE_TYPE_ID__ALL;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Cấu hình lưới danh mục dịch vụ (grdDanhMucDV): 4 cột Mã DV / Tên dịch vụ / Đơn giá /
        /// nút "+", gom nhóm theo Loại dịch vụ (SERVICE_TYPE_NAME) làm dòng cha. Gọi 1 lần khi load form.
        /// </summary>
        private void SetupCatalogGrid()
        {
            try
            {
                gvDanhMucDV.OptionsView.ShowGroupPanel = false;
                gvDanhMucDV.OptionsView.ColumnAutoWidth = true;
                gvDanhMucDV.OptionsBehavior.AutoPopulateColumns = false;
                gvDanhMucDV.OptionsBehavior.AutoExpandAllGroups = true;
                // In đậm chữ dòng cha (loại dịch vụ)
                gvDanhMucDV.Appearance.GroupRow.FontStyleDelta = System.Drawing.FontStyle.Bold;
                SetGridHeaderBold(gvDanhMucDV);
                // Dòng cha: chỉ hiện tên loại + sắp xếp nhóm theo NUM_ORDER
                gvDanhMucDV.CustomDrawGroupRow += gvDanhMucDV_CustomDrawGroupRow;
                gvDanhMucDV.CustomColumnSort += gvDanhMucDV_CustomColumnSort;
                gvDanhMucDV.Columns.Clear();

                // Cột gom nhóm: Loại dịch vụ (dòng cha)
                GridColumn colType = gvDanhMucDV.Columns.AddVisible("SERVICE_TYPE_NAME", "Loại dịch vụ");
                colType.GroupIndex = 0;
                colType.OptionsColumn.AllowEdit = false;

                GridColumn colCode = gvDanhMucDV.Columns.AddVisible("SERVICE_CODE", "Mã DV");
                colCode.VisibleIndex = 0;
                colCode.Width = 100;
                colCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
                colCode.OptionsColumn.AllowEdit = false;
                SetColumnCenter(colCode);

                GridColumn colName = gvDanhMucDV.Columns.AddVisible("SERVICE_NAME", "Tên dịch vụ");
                colName.VisibleIndex = 1;
                colName.Width = 400;
                colName.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
                colName.OptionsColumn.AllowEdit = false;

                GridColumn colPrice = gvDanhMucDV.Columns.AddVisible("PRICE", "Đơn giá");
                colPrice.VisibleIndex = 2;
                colPrice.Width = 110;
                colPrice.OptionsColumn.AllowEdit = false;
                colPrice.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                colPrice.DisplayFormat.FormatString = "n0";
                SetColumnCenter(colPrice);

                // Cột nút "+" (unbound)
                repoBtnAddService = new RepositoryItemButtonEdit();
                repoBtnAddService.TextEditStyle = TextEditStyles.HideTextEditor;
                repoBtnAddService.Buttons[0].Kind = ButtonPredefines.Plus;
                repoBtnAddService.ButtonClick += repoBtnAddService_ButtonClick;
                grdDanhMucDV.RepositoryItems.Add(repoBtnAddService);

                GridColumn colAdd = gvDanhMucDV.Columns.AddVisible("ADD_BTN");
                colAdd.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                colAdd.VisibleIndex = 3;
                colAdd.Caption = " ";
                colAdd.OptionsColumn.ShowCaption = false;
                colAdd.ColumnEdit = repoBtnAddService;
                SetButtonColumnFixed(colAdd, 30);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Nạp danh mục dịch vụ:
        ///   - Dịch vụ KHÁC (Khám/XN/CDHA/PT/Giường…): theo chính sách giá V_HIS_SERVICE_PATY (ngày đăng ký,
        ///     đối tượng TT, gói, loại DV) — như cũ; loại trừ Máu, Suất ăn, và Thuốc/Vật tư.
        ///   - THUỐC / VẬT TƯ: lấy trực tiếp từ danh mục V_HIS_MEDICINE_TYPE / V_HIS_MATERIAL_TYPE (không qua
        ///     chính sách giá). Đơn giá vẫn theo logic cũ (ApplyMedicineMaterialPrice: LAST_EXP_PRICE → lô → LAST_IMP_PRICE).
        /// SL mặc định khi thêm vào gói = 1 (xem AddServiceToPackageDetail).
        /// </summary>
        private void LoadServiceCatalog()
        {
            try
            {
                long patientTypeId = GetLongEditValue(cboDoiTuongTT.EditValue);
                long serviceTypeFilter = GetLongEditValue(cboLoaiDV.EditValue);
                long? packageId = null;
                long packageIdValue = GetLongEditValue(cboMauGoi.EditValue);
                if (packageIdValue > 0) packageId = packageIdValue;

                long registerTime = 0;
                if (dteNgayDangKy.EditValue is DateTime)
                {
                    registerTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(((DateTime)dteNgayDangKy.EditValue).Date) ?? 0;
                }

                // Chưa đủ điều kiện (chưa có đối tượng TT / ngày đăng ký) → để trống
                if (patientTypeId <= 0 || registerTime <= 0)
                {
                    grdDanhMucDV.DataSource = null;
                    return;
                }

                // Cần tập SERVICE_TYPE_ID của thuốc/vật tư để loại khỏi nguồn chính sách giá
                EnsureMediMatePriceMap();

                List<long> patientTypeIds = new List<long> { patientTypeId };
                long idMau = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU;
                long idAn = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN;

                // Dịch vụ KHÔNG phải thuốc/vật tư → chính sách giá V_HIS_SERVICE_PATY (như cũ)
                List<V_HIS_SERVICE_PATY> data = BranchDataWorker.DicServicePatyInBranch
                    .SelectMany(o => o.Value)
                    .Where(o => o.IS_ACTIVE == 1
                             && o.SERVICE_TYPE_ID != idMau
                             && o.SERVICE_TYPE_ID != idAn
                             && !medServiceTypeIds.Contains(o.SERVICE_TYPE_ID)   // thuốc lấy riêng từ HIS_MEDICINE_TYPE
                             && !matServiceTypeIds.Contains(o.SERVICE_TYPE_ID)   // vật tư lấy riêng từ HIS_MATERIAL_TYPE
                             && (serviceTypeFilter <= 0 || o.SERVICE_TYPE_ID == serviceTypeFilter)
                             // đối tượng thanh toán (gồm đối tượng kế thừa)
                             && (o.PATIENT_TYPE_ID == patientTypeId
                                 || BranchDataWorker.CheckPatientTypeInherit(o.INHERIT_PATIENT_TYPE_IDS, patientTypeIds))
                             // ngày đăng ký nằm trong hiệu lực chính sách giá
                             && (!o.FROM_TIME.HasValue || o.FROM_TIME.Value <= registerTime)
                             && (!o.TO_TIME.HasValue || o.TO_TIME.Value >= registerTime)
                             // package_id đang chọn (null ↔ chính sách giá lẻ)
                             && ((!packageId.HasValue && !o.PACKAGE_ID.HasValue)
                                 || (packageId.HasValue && o.PACKAGE_ID == packageId.Value)))
                    .GroupBy(o => o.SERVICE_ID)
                    .Select(g => g.OrderByDescending(x => x.PRIORITY).ThenByDescending(x => x.ID).First())
                    .ToList();

                // Thuốc / Vật tư → lấy trực tiếp từ danh mục type (KHÔNG theo chính sách giá), tôn trọng filter loại DV
                if (serviceTypeFilter <= 0 || medServiceTypeIds.Contains(serviceTypeFilter))
                    data.AddRange(BuildCatalogRowsFromMedicineTypes(serviceTypeFilter));
                if (serviceTypeFilter <= 0 || matServiceTypeIds.Contains(serviceTypeFilter))
                    data.AddRange(BuildCatalogRowsFromMaterialTypes(serviceTypeFilter));

                data = data
                    .OrderBy(o => o.SERVICE_TYPE_NAME)
                    .ThenBy(o => o.SERVICE_NAME)
                    .ToList();

                // Đơn giá thuốc/vật tư theo logic cũ (LAST_EXP_PRICE → lô → LAST_IMP_PRICE)
                ApplyMedicineMaterialPrice(data);

                grdDanhMucDV.DataSource = data;
                gvDanhMucDV.ExpandAllGroups();
                ApplyCatalogSearchFilter();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Dựng dòng danh mục cho THUỐC từ V_HIS_MEDICINE_TYPE (IS_ACTIVE=1), lọc theo loại DV đang chọn.
        /// Mỗi loại thuốc = 1 dòng (Mã/Tên = MEDICINE_TYPE_CODE/NAME); đơn giá để ApplyMedicineMaterialPrice tính.
        /// </summary>
        private List<V_HIS_SERVICE_PATY> BuildCatalogRowsFromMedicineTypes(long serviceTypeFilter)
        {
            List<V_HIS_SERVICE_PATY> rows = new List<V_HIS_SERVICE_PATY>();
            try
            {
                List<V_HIS_MEDICINE_TYPE> medTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                if (medTypes == null) return rows;
                foreach (V_HIS_MEDICINE_TYPE t in medTypes)
                {
                    if (t.IS_ACTIVE != 1) continue;
                    if (serviceTypeFilter > 0 && t.SERVICE_TYPE_ID != serviceTypeFilter) continue;
                    rows.Add(new V_HIS_SERVICE_PATY
                    {
                        SERVICE_ID = t.SERVICE_ID,
                        SERVICE_CODE = t.MEDICINE_TYPE_CODE,
                        SERVICE_NAME = t.MEDICINE_TYPE_NAME,
                        SERVICE_TYPE_ID = t.SERVICE_TYPE_ID,
                        SERVICE_TYPE_NAME = GetServiceTypeName(t.SERVICE_TYPE_ID),
                        PRICE = 0
                    });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return rows;
        }

        /// <summary>
        /// Dựng dòng danh mục cho VẬT TƯ từ V_HIS_MATERIAL_TYPE (IS_ACTIVE=1), lọc theo loại DV đang chọn.
        /// </summary>
        private List<V_HIS_SERVICE_PATY> BuildCatalogRowsFromMaterialTypes(long serviceTypeFilter)
        {
            List<V_HIS_SERVICE_PATY> rows = new List<V_HIS_SERVICE_PATY>();
            try
            {
                List<V_HIS_MATERIAL_TYPE> matTypes = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                if (matTypes == null) return rows;
                foreach (V_HIS_MATERIAL_TYPE t in matTypes)
                {
                    if (t.IS_ACTIVE != 1) continue;
                    if (serviceTypeFilter > 0 && t.SERVICE_TYPE_ID != serviceTypeFilter) continue;
                    rows.Add(new V_HIS_SERVICE_PATY
                    {
                        SERVICE_ID = t.SERVICE_ID,
                        SERVICE_CODE = t.MATERIAL_TYPE_CODE,
                        SERVICE_NAME = t.MATERIAL_TYPE_NAME,
                        SERVICE_TYPE_ID = t.SERVICE_TYPE_ID,
                        SERVICE_TYPE_NAME = GetServiceTypeName(t.SERVICE_TYPE_ID),
                        PRICE = 0
                    });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return rows;
        }

        /// <summary>
        /// Tên loại dịch vụ theo ID (HIS_SERVICE_TYPE.SERVICE_TYPE_NAME), rỗng nếu không có.
        /// </summary>
        private string GetServiceTypeName(long serviceTypeId)
        {
            string name;
            return serviceTypeNameById.TryGetValue(serviceTypeId, out name) ? name : "";
        }

        /// <summary>
        /// Dựng 1 lần (cache trong field) đơn giá kho thuốc/vật tư theo SERVICE_ID, lấy từ cache RAM
        /// V_HIS_MEDICINE_TYPE / V_HIS_MATERIAL_TYPE: ưu tiên LAST_EXP_PRICE, không có thì LAST_IMP_PRICE
        /// (giá nhập lô gần nhất do backend tính sẵn). Tách riêng 2 bảng + tập SERVICE_TYPE_ID của từng
        /// loại để định tuyến đúng (1 SERVICE_ID có thể tồn tại ở cả thuốc lẫn vật tư). KHÔNG gọi API.
        /// </summary>
        private void EnsureMediMatePriceMap()
        {
            if (medTypeBySvcId != null) return;

            medTypeBySvcId = new Dictionary<long, V_HIS_MEDICINE_TYPE>();
            matTypeBySvcId = new Dictionary<long, V_HIS_MATERIAL_TYPE>();
            medServiceTypeIds = new HashSet<long>();
            matServiceTypeIds = new HashSet<long>();
            try
            {
                List<V_HIS_MEDICINE_TYPE> medTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                if (medTypes != null)
                {
                    foreach (V_HIS_MEDICINE_TYPE t in medTypes)
                    {
                        medServiceTypeIds.Add(t.SERVICE_TYPE_ID);
                        if (!medTypeBySvcId.ContainsKey(t.SERVICE_ID))
                            medTypeBySvcId[t.SERVICE_ID] = t;
                    }
                }

                List<V_HIS_MATERIAL_TYPE> matTypes = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                if (matTypes != null)
                {
                    foreach (V_HIS_MATERIAL_TYPE t in matTypes)
                    {
                        matServiceTypeIds.Add(t.SERVICE_TYPE_ID);
                        if (!matTypeBySvcId.ContainsKey(t.SERVICE_ID))
                            matTypeBySvcId[t.SERVICE_ID] = t;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Dựng Đơn giá cho thuốc/vật tư NGAY khi nạp danh mục (định tuyến theo SERVICE_TYPE_ID, chống trùng SERVICE_ID):
        ///   1) LAST_EXP_PRICE (cache) — nếu có;
        ///   2) IMP_PRICE lô HIS_MEDICINE/HIS_MATERIAL gần nhất (IMP_TIME lớn nhất) — gộp 1 call/loại + nhớ kết quả;
        ///   3) LAST_IMP_PRICE (cache) — nếu loại đó không có lô;
        ///   4) giữ giá chính sách — cuối cùng.
        /// Memo (đã truy vấn) giúp các lần nạp lại do đổi filter KHÔNG gọi lại API → không lag.
        /// </summary>
        private void ApplyMedicineMaterialPrice(List<V_HIS_SERVICE_PATY> data)
        {
            if (data == null || data.Count == 0) return;
            try
            {
                EnsureMediMatePriceMap();

                // Lượt 1: áp LAST_EXP_PRICE (cache) ngay; gom dòng thiếu LAST_EXP_PRICE để lấy giá lô
                List<V_HIS_SERVICE_PATY> medFallback = new List<V_HIS_SERVICE_PATY>();
                List<V_HIS_SERVICE_PATY> matFallback = new List<V_HIS_SERVICE_PATY>();
                Dictionary<long, long> medNeed = new Dictionary<long, long>(); // serviceId -> medicineTypeId (chưa truy vấn lô)
                Dictionary<long, long> matNeed = new Dictionary<long, long>();

                foreach (V_HIS_SERVICE_PATY paty in data)
                {
                    if (medServiceTypeIds.Contains(paty.SERVICE_TYPE_ID))
                    {
                        V_HIS_MEDICINE_TYPE mety;
                        if (medTypeBySvcId.TryGetValue(paty.SERVICE_ID, out mety))
                        {
                            if (mety.LAST_EXP_PRICE.HasValue) paty.PRICE = mety.LAST_EXP_PRICE.Value;
                            else
                            {
                                medFallback.Add(paty);
                                if (!medLoQueriedSvcIds.Contains(paty.SERVICE_ID) && !medNeed.ContainsKey(paty.SERVICE_ID))
                                    medNeed[paty.SERVICE_ID] = mety.ID;
                            }
                        }
                    }
                    else if (matServiceTypeIds.Contains(paty.SERVICE_TYPE_ID))
                    {
                        V_HIS_MATERIAL_TYPE maty;
                        if (matTypeBySvcId.TryGetValue(paty.SERVICE_ID, out maty))
                        {
                            if (maty.LAST_EXP_PRICE.HasValue) paty.PRICE = maty.LAST_EXP_PRICE.Value;
                            else
                            {
                                matFallback.Add(paty);
                                if (!matLoQueriedSvcIds.Contains(paty.SERVICE_ID) && !matNeed.ContainsKey(paty.SERVICE_ID))
                                    matNeed[paty.SERVICE_ID] = maty.ID;
                            }
                        }
                    }
                }

                // Lượt 2: gộp truy vấn lô (chỉ cho dịch vụ chưa truy vấn) — tối đa 1 call thuốc + 1 call vật tư
                LoadLoImpPriceMedicineBatch(medNeed);
                LoadLoImpPriceMaterialBatch(matNeed);

                // Lượt 3: áp giá lô (hoặc LAST_IMP_PRICE) cho các dòng thiếu LAST_EXP_PRICE
                decimal lo;
                foreach (V_HIS_SERVICE_PATY paty in medFallback)
                {
                    if (medLoImpPriceBySvcId.TryGetValue(paty.SERVICE_ID, out lo)) paty.PRICE = lo;
                    else
                    {
                        V_HIS_MEDICINE_TYPE mety;
                        if (medTypeBySvcId.TryGetValue(paty.SERVICE_ID, out mety) && mety.LAST_IMP_PRICE.HasValue)
                            paty.PRICE = mety.LAST_IMP_PRICE.Value;
                    }
                }
                foreach (V_HIS_SERVICE_PATY paty in matFallback)
                {
                    if (matLoImpPriceBySvcId.TryGetValue(paty.SERVICE_ID, out lo)) paty.PRICE = lo;
                    else
                    {
                        V_HIS_MATERIAL_TYPE maty;
                        if (matTypeBySvcId.TryGetValue(paty.SERVICE_ID, out maty) && maty.LAST_IMP_PRICE.HasValue)
                            paty.PRICE = maty.LAST_IMP_PRICE.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gộp truy vấn lô HIS_MEDICINE cho nhiều loại thuốc (1 call). Mỗi dịch vụ lấy IMP_PRICE của lô có
        /// IMP_TIME lớn nhất, lưu memo theo SERVICE_ID (TDL_SERVICE_ID); đánh dấu đã truy vấn (kể cả không có
        /// lô) để khỏi gọi lại ở các lần nạp danh mục sau.
        /// </summary>
        private void LoadLoImpPriceMedicineBatch(Dictionary<long, long> svcToType)
        {
            if (svcToType == null || svcToType.Count == 0) return;
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                List<long> typeIds = svcToType.Values.Distinct().ToList();
                // Ngắt thành từng lô tối đa 500 ID/lần gọi để URI (GET) không vượt giới hạn độ dài
                for (int i = 0; i < typeIds.Count; i += MEDI_MATE_QUERY_CHUNK_SIZE)
                {
                    List<long> chunk = typeIds.GetRange(i, Math.Min(MEDI_MATE_QUERY_CHUNK_SIZE, typeIds.Count - i));
                    HisMedicineFilter filter = new HisMedicineFilter();
                    filter.MEDICINE_TYPE_IDs = chunk;
                    filter.IS_ACTIVE = 1;
                    List<HIS_MEDICINE> lots = new BackendAdapter(param)
                        .Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumers.MosConsumer, filter, param);
                    if (lots != null)
                    {
                        foreach (var g in lots.GroupBy(o => o.TDL_SERVICE_ID))
                        {
                            HIS_MEDICINE latest = g.OrderByDescending(x => x.IMP_TIME ?? 0).ThenByDescending(x => x.ID).First();
                            medLoImpPriceBySvcId[g.Key] = latest.IMP_PRICE;
                        }
                    }
                }
                WaitingManager.Hide();
                foreach (long svc in svcToType.Keys) medLoQueriedSvcIds.Add(svc);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gộp truy vấn lô HIS_MATERIAL cho nhiều loại vật tư (1 call). Tương tự LoadLoImpPriceMedicineBatch.
        /// </summary>
        private void LoadLoImpPriceMaterialBatch(Dictionary<long, long> svcToType)
        {
            if (svcToType == null || svcToType.Count == 0) return;
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                List<long> typeIds = svcToType.Values.Distinct().ToList();
                // Ngắt thành từng lô tối đa 500 ID/lần gọi để URI (GET) không vượt giới hạn độ dài
                for (int i = 0; i < typeIds.Count; i += MEDI_MATE_QUERY_CHUNK_SIZE)
                {
                    List<long> chunk = typeIds.GetRange(i, Math.Min(MEDI_MATE_QUERY_CHUNK_SIZE, typeIds.Count - i));
                    HisMaterialFilter filter = new HisMaterialFilter();
                    filter.MATERIAL_TYPE_IDs = chunk;
                    filter.IS_ACTIVE = 1;
                    List<HIS_MATERIAL> lots = new BackendAdapter(param)
                        .Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumers.MosConsumer, filter, param);
                    if (lots != null)
                    {
                        foreach (var g in lots.GroupBy(o => o.TDL_SERVICE_ID))
                        {
                            HIS_MATERIAL latest = g.OrderByDescending(x => x.IMP_TIME ?? 0).ThenByDescending(x => x.ID).First();
                            matLoImpPriceBySvcId[g.Key] = latest.IMP_PRICE;
                        }
                    }
                }
                WaitingManager.Hide();
                foreach (long svc in svcToType.Keys) matLoQueriedSvcIds.Add(svc);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Lọc nhanh lưới danh mục theo từ khóa trong txtTimKiemDV (Mã / Tên dịch vụ).
        /// </summary>
        private void ApplyCatalogSearchFilter()
        {
            try
            {
                string keyword = txtTimKiemDV.EditValue == null ? "" : txtTimKiemDV.EditValue.ToString().Trim();
                if (string.IsNullOrEmpty(keyword))
                {
                    gvDanhMucDV.ActiveFilterString = "";
                }
                else
                {
                    // Lọc theo Mã DV / Tên dịch vụ — không phân biệt hoa thường
                    string kw = keyword.ToUpper().Replace("'", "''");
                    gvDanhMucDV.ActiveFilterString =
                        string.Format("Contains(Upper([SERVICE_CODE]), '{0}') Or Contains(Upper([SERVICE_NAME]), '{0}')", kw);
                }
                gvDanhMucDV.ExpandAllGroups();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dteNgayDangKy_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (isFormLoading) return;
                LoadServiceCatalog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repoBtnAddService_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                V_HIS_SERVICE_PATY paty = gvDanhMucDV.GetFocusedRow() as V_HIS_SERVICE_PATY;
                AddServiceToPackageDetail(paty);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Thêm 1 dịch vụ từ danh mục vào danh sách "Dịch vụ trong gói" (tránh trùng theo SERVICE_ID).
        /// TODO: hoàn thiện cột/định mức khi chốt thiết kế nhóm "Dịch vụ trong gói".
        /// </summary>
        private void AddServiceToPackageDetail(V_HIS_SERVICE_PATY paty)
        {
            try
            {
                if (paty == null) return;
                if (selectedPackageServices.Any(o => o.IS_NONE_SERVICE == 0 && o.SERVICE_ID == paty.SERVICE_ID)) return;

                // Đơn giá đã resolve sẵn trên dòng danh mục (LAST_EXP_PRICE / giá lô / LAST_IMP_PRICE)
                decimal price = Convert.ToDecimal(paty.PRICE);
                PackageServiceADO ado = new PackageServiceADO
                {
                    SERVICE_ID = paty.SERVICE_ID,
                    SERVICE_CODE = paty.SERVICE_CODE,
                    SERVICE_NAME = paty.SERVICE_NAME,
                    PRICE = price,
                    AMOUNT = 1,
                    TOTAL_PRICE = price,
                    IS_NONE_SERVICE = 0
                };
                selectedPackageServices.Add(ado);
                UpdateTotalAmount();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Cấu hình lưới "Dịch vụ trong gói" (grdDichVuTrongGoi): 4 cột Mã DV / Tên dịch vụ /
        /// SL / Thành tiền + nút "-" để xóa dòng. SL và Thành tiền cho phép sửa; đổi SL tự tính
        /// lại Thành tiền = SL × Đơn giá. Gọi 1 lần khi load form (DataSource là BindingList nên
        /// thêm/xóa tự cập nhật lưới).
        /// </summary>
        private void SetupPackageDetailGrid()
        {
            try
            {
                gvDichVuTrongGoi.OptionsView.ShowGroupPanel = false;
                gvDichVuTrongGoi.OptionsView.ColumnAutoWidth = false;
                gvDichVuTrongGoi.OptionsBehavior.AutoPopulateColumns = false;
                gvDichVuTrongGoi.Columns.Clear();
                SetGridHeaderBold(gvDichVuTrongGoi);

                GridColumn colCode = gvDichVuTrongGoi.Columns.AddVisible("SERVICE_CODE", "Mã DV");
                colCode.VisibleIndex = 0;
                colCode.Width = 100;
                colCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
                colCode.OptionsColumn.AllowEdit = false;
                SetColumnCenter(colCode);

                GridColumn colName = gvDichVuTrongGoi.Columns.AddVisible("SERVICE_NAME", "Tên dịch vụ");
                colName.VisibleIndex = 1;
                colName.Width = 300;
                colName.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
                // Tạo mới: cho sửa tên dịch vụ (lưu vào CreateSdo); Sửa gói đã có: khóa tên.
                // (đồng bộ lại theo chế độ trong RefreshPackageDetailGridMode)
                colName.OptionsColumn.AllowEdit = !isEditMode;

                // SL (cho phép sửa)
                RepositoryItemSpinEdit repoSL = new RepositoryItemSpinEdit();
                repoSL.IsFloatValue = false;
                repoSL.MinValue = 1;
                repoSL.MaxValue = 1000000;
                grdDichVuTrongGoi.RepositoryItems.Add(repoSL);
                GridColumn colAmount = gvDichVuTrongGoi.Columns.AddVisible("AMOUNT", "SL");
                colAmount.VisibleIndex = 2;
                colAmount.Width = 50;
                colAmount.ColumnEdit = repoSL;
                SetColumnCenter(colAmount);

                // SL đã dùng (AMOUNT_USED) — chỉ hiển thị khi sửa gói đã tồn tại, không cho sửa
                GridColumn colUsed = gvDichVuTrongGoi.Columns.AddVisible("AMOUNT_USED", "SL đã dùng");
                colUsed.VisibleIndex = 3;
                colUsed.Width = 80;
                colUsed.OptionsColumn.AllowEdit = false;
                SetColumnCenter(colUsed);
                colUsed.Visible = isEditMode;

                // Thành tiền (cho phép sửa)
                RepositoryItemTextEdit repoThanhTien = new RepositoryItemTextEdit();
                repoThanhTien.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
                repoThanhTien.Mask.EditMask = "n0";
                repoThanhTien.Mask.UseMaskAsDisplayFormat = true;
                grdDichVuTrongGoi.RepositoryItems.Add(repoThanhTien);
                GridColumn colTotal = gvDichVuTrongGoi.Columns.AddVisible("TOTAL_PRICE", "Thành tiền");
                colTotal.VisibleIndex = 4;
                colTotal.Width = 110;
                colTotal.ColumnEdit = repoThanhTien;
                SetColumnCenter(colTotal);

                // Nút "-" xóa dòng (cột unbound)
                RepositoryItemButtonEdit repoRemove = new RepositoryItemButtonEdit();
                repoRemove.TextEditStyle = TextEditStyles.HideTextEditor;
                repoRemove.Buttons[0].Kind = ButtonPredefines.Minus;
                repoRemove.ButtonClick += repoBtnRemoveService_ButtonClick;
                grdDichVuTrongGoi.RepositoryItems.Add(repoRemove);
                GridColumn colRemove = gvDichVuTrongGoi.Columns.AddVisible("REMOVE_BTN");
                colRemove.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                colRemove.VisibleIndex = 5;
                colRemove.Caption = " ";
                colRemove.OptionsColumn.ShowCaption = false;
                colRemove.ColumnEdit = repoRemove;
                SetButtonColumnFixed(colRemove, 30);

                gvDichVuTrongGoi.CellValueChanged += gvDichVuTrongGoi_CellValueChanged;
                gvDichVuTrongGoi.ValidatingEditor += gvDichVuTrongGoi_ValidatingEditor;
                gvDichVuTrongGoi.OptionsView.ColumnAutoWidth = true;
                grdDichVuTrongGoi.DataSource = selectedPackageServices;
                // Nút "Phí gói" bật theo trạng thái đã có bệnh nhân (xem SetBottomButtonsEnabled),
                // không phụ thuộc việc chọn mẫu gói.
                UpdateTotalAmount();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Đổi SL → tính lại Thành tiền = SL × Đơn giá. Thành tiền vẫn cho sửa tay sau đó.
        /// </summary>
        private void gvDichVuTrongGoi_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                PackageServiceADO row = e.Column != null ? gvDichVuTrongGoi.GetRow(e.RowHandle) as PackageServiceADO : null;
                if (row != null)
                {
                    if (e.Column.FieldName == "AMOUNT")
                    {
                        // Đổi SL → Thành tiền = SL × đơn giá hiện tại
                        row.TOTAL_PRICE = row.AMOUNT * row.PRICE;
                        gvDichVuTrongGoi.RefreshRow(e.RowHandle);
                    }
                    else if (e.Column.FieldName == "TOTAL_PRICE")
                    {
                        // Sửa tay Thành tiền → cập nhật lại đơn giá (PRICE) = Thành tiền / SL
                        // để khi đổi SL sau đó không bị nhảy về đơn giá cũ × SL
                        if (row.AMOUNT != 0) row.PRICE = row.TOTAL_PRICE / row.AMOUNT;
                    }
                }

                // SL hoặc Thành tiền thay đổi → cập nhật lại tổng cộng
                UpdateTotalAmount();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Validate khi sửa cột SL: không cho nhập nhỏ hơn SL đã dùng (AMOUNT_USED).
        /// </summary>
        private void gvDichVuTrongGoi_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
        {
            try
            {
                if (gvDichVuTrongGoi.FocusedColumn == null || gvDichVuTrongGoi.FocusedColumn.FieldName != "AMOUNT") return;

                PackageServiceADO row = gvDichVuTrongGoi.GetRow(gvDichVuTrongGoi.FocusedRowHandle) as PackageServiceADO;
                if (row == null || e.Value == null) return;

                decimal newAmount = Convert.ToDecimal(e.Value);
                if (newAmount < row.AMOUNT_USED)
                {
                    e.Valid = false;
                    e.ErrorText = "Số lượng không được nhỏ hơn số lượng đã dùng (" + row.AMOUNT_USED.ToString("n0") + ").";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Đồng bộ lưới "Dịch vụ trong gói" theo chế độ:
        ///   - Cột "SL đã dùng" (AMOUNT_USED): chỉ hiển thị khi sửa gói đã tồn tại (isEditMode).
        ///   - Cột "Tên dịch vụ" (SERVICE_NAME): chỉ cho sửa khi tạo mới (lưu vào CreateSdo).
        /// </summary>
        private void RefreshPackageDetailGridMode()
        {
            try
            {
                GridColumn colUsed = gvDichVuTrongGoi.Columns["AMOUNT_USED"];
                if (colUsed != null)
                {
                    colUsed.Visible = isEditMode;
                    // Đặt lại vị trí (giữa SL và Thành tiền) khi hiện lại cột
                    if (isEditMode) colUsed.VisibleIndex = 3;
                }

                // Tạo mới: cho sửa tên dịch vụ; Sửa gói đã có: khóa tên
                GridColumn colName = gvDichVuTrongGoi.Columns["SERVICE_NAME"];
                if (colName != null)
                {
                    colName.OptionsColumn.AllowEdit = !isEditMode;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Bấm "-" → xóa dòng dịch vụ khỏi danh sách "Dịch vụ trong gói".
        /// </summary>
        private void repoBtnRemoveService_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                PackageServiceADO row = gvDichVuTrongGoi.GetRow(gvDichVuTrongGoi.FocusedRowHandle) as PackageServiceADO;
                if (row == null) return;

                // Chỉ cho xóa khi chưa sử dụng và chưa thanh toán
                if (row.AMOUNT_USED > 0 || row.AMOUNT_PREPAID > 0)
                {
                    XtraMessageBox.Show("Không thể xóa dịch vụ đã sử dụng hoặc đã thanh toán.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                selectedPackageServices.Remove(row);
                UpdateTotalAmount();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Cập nhật label tổng cộng = tổng Thành tiền của toàn bộ dịch vụ trong gói.
        /// </summary>
        private void UpdateTotalAmount()
        {
            try
            {
                decimal total = selectedPackageServices.Sum(o => o.TOTAL_PRICE);
                lblTongCong.Text = "Tổng cộng: " + total.ToString("n0");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Đọc giá trị long từ EditValue của control lookup (0 nếu null/không hợp lệ).
        /// </summary>
        private long GetLongEditValue(object editValue)
        {
            try
            {
                if (editValue == null) return 0;
                long result;
                if (long.TryParse(editValue.ToString(), out result)) return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return 0;
        }

        /// <summary>
        /// Đánh dấu các trường bắt buộc theo cột NOT NULL của HIS_PATIENT_PACKAGE:
        ///   Tên gói (PACKAGE_NAME, VARCHAR2(200)) / Ngày đăng ký (REGISTER_DATE) /
        ///   Đối tượng TT (PATIENT_TYPE_ID) / Trạng thái (STATUS_CODE, VARCHAR2(20)).
        /// Mỗi trường: tiêu đề (caption) đổi sang màu maroon + bắt buộc nhập. Riêng ô text
        /// (Tên gói) còn chặn vượt số ký tự theo độ dài cột bằng ControlMaxLengthValidationRule
        /// (đếm theo UTF-8 byte — khớp VARCHAR2 BYTE của Oracle).
        /// (PATIENT_ID lấy theo bệnh nhân đã tìm; TOTAL_PAID/REFUNDED/USED do hệ thống tính
        /// nên không hiển thị nhập trên form.)
        /// </summary>
        private void SetupRequiredValidation()
        {
            try
            {
                // Tiêu đề các trường bắt buộc → màu maroon
                SetRequiredCaption(lciTenGoi);
                SetRequiredCaption(lciNgayDangKy);
                SetRequiredCaption(lciDoiTuongTT);
                SetRequiredCaption(lciTrangThai);

                // Tên gói (PACKAGE_NAME VARCHAR2(200)): bắt buộc + chặn quá 200 ký tự (đếm UTF-8 byte)
                AddMaxLengthRequiredRule(txtTenGoi, 200);
                txtTenGoi.Properties.MaxLength = 200;
                // Ngày đăng ký / Đối tượng TT / Trạng thái: bắt buộc nhập — kiểm tra trong ValidateBeforeSave
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Đổi màu tiêu đề (caption) của 1 LayoutControlItem sang maroon — đánh dấu trường bắt buộc.
        /// </summary>
        private void SetRequiredCaption(DevExpress.XtraLayout.LayoutControlItem item)
        {
            if (item == null) return;
            item.AppearanceItemCaption.ForeColor = Color.Maroon;
            item.AppearanceItemCaption.Options.UseForeColor = true;
        }

        /// <summary>
        /// Gắn rule bắt buộc nhập + chặn số ký tự (theo độ dài cột, đếm UTF-8 byte) cho ô text.
        /// Dùng thư viện ControlMaxLengthValidationRule (Inventec.Desktop.Common.Controls.ValidationRule).
        /// </summary>
        private void AddMaxLengthRequiredRule(DevExpress.XtraEditors.TextEdit editor, int maxLength)
        {
            if (editor == null) return;
            Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule rule =
                new Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule();
            rule.editor = editor;
            rule.maxLength = maxLength;
            rule.IsRequired = true;
            rule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            dxValidationProvider.SetValidationRule(editor, rule);
        }

        /// <summary>
        /// Khi mở form từ chức năng khác có truyền vào bệnh nhân / gói:
        ///   - Hiển thị ngay thông tin hành chính BN (không tìm kiếm lại)
        ///   - Nếu có HIS_PATIENT_PACKAGE: chuyển sang chế độ Sửa, load lên các control
        /// </summary>
        private void ApplyInputData()
        {
            try
            {
                if (inputPatient != null)
                {
                    BindPatientInfo(inputPatient);
                    // Ẩn vùng tìm kiếm vì đã có sẵn bệnh nhân
                    txtMaBenhNhan.EditValue = inputPatient.PATIENT_CODE;
                    txtMaBenhNhan.Properties.ReadOnly = true;
                    btnTimKiem.Enabled = false;
                    SetBottomButtonsEnabled(true);
                }
                else
                {
                    // Mở từ menu: chưa có BN → disable nút dưới cùng đến khi tìm thấy
                    SetBottomButtonsEnabled(false);
                }

                if (isEditMode && inputPatientPackage != null)
                {
                    this.Text = "Sửa gói dịch vụ";
                    BindPatientPackageInfo(inputPatientPackage);
                }
                else
                {
                    this.Text = "Đăng ký gói dịch vụ";
                    dteNgayDangKy.EditValue = DateTime.Today;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Pad mã bệnh nhân bằng số 0 phía trước đủ 10 ký tự (chỉ khi là chuỗi số).
        /// VD: "123" → "0000000123".
        /// </summary>
        private string PadPatientCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return code;
            string trimmed = code.Trim();
            // Chỉ pad nếu là chuỗi số thuần và độ dài < 10
            long _;
            if (long.TryParse(trimmed, out _) && trimmed.Length < 10)
            {
                return trimmed.PadLeft(10, '0');
            }
            return trimmed;
        }

        /// <summary>
        /// Gọi API tìm bệnh nhân theo PATIENT_CODE (đã pad).
        /// Tìm thấy: hiển thị thông tin + enable nút dưới cùng.
        /// Không thấy: xóa thông tin + disable nút dưới cùng.
        /// </summary>
        private void SearchPatient()
        {
            try
            {
                string rawCode = txtMaBenhNhan.EditValue == null ? "" : txtMaBenhNhan.EditValue.ToString();
                if (string.IsNullOrWhiteSpace(rawCode))
                {
                    ClearPatientInfo();
                    SetBottomButtonsEnabled(false);
                    return;
                }

                string paddedCode = PadPatientCode(rawCode);
                txtMaBenhNhan.EditValue = paddedCode;

                CommonParam param = new CommonParam();
                HisPatientFilter filter = new HisPatientFilter();
                filter.PATIENT_CODE = paddedCode;

                WaitingManager.Show();
                List<HIS_PATIENT> patients = new BackendAdapter(param)
                    .Get<List<HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GET, ApiConsumers.MosConsumer, filter, param);
                WaitingManager.Hide();

                HIS_PATIENT patient = patients != null ? patients.FirstOrDefault() : null;
                if (patient != null)
                {
                    inputPatient = patient;
                    BindPatientInfo(patient);
                    SetBottomButtonsEnabled(true);
                }
                else
                {
                    inputPatient = null;
                    ClearPatientInfo();
                    SetBottomButtonsEnabled(false);
                    XtraMessageBox.Show("Không tìm thấy bệnh nhân với mã: " + paddedCode, "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Xóa toàn bộ thông tin hành chính + reset các control khác về rỗng.
        /// </summary>
        private void ClearPatientInfo()
        {
            try
            {
                lblHoTen.Text = "";
                lblNgaySinh.Text = "";
                lblGioiTinh.Text = "";
                lblCCCD.Text = "";
                lblDienThoai.Text = "";
                lblDiaChi.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Enable / Disable các control phụ thuộc trạng thái "đã tìm thấy bệnh nhân":
        /// nút Danh sách gói, Phí gói + 3 nút dưới cùng (In phiếu, Hủy bỏ, Lưu).
        /// Chỉ enable khi đã có HIS_PATIENT (tìm kiếm ra hoặc truyền vào từ chức năng khác).
        /// </summary>
        private void SetBottomButtonsEnabled(bool enabled)
        {
            try
            {
                // Danh sách gói chỉ load theo patient_id sau khi đã tìm thấy bệnh nhân
                btnDanhSachGoi.Enabled = enabled;
                // Phí gói: bật khi đã có bệnh nhân (không cần chọn mẫu gói / package_id)
                btnPhiGoi.Enabled = enabled;
                btnInPhieu.Enabled = enabled;
                btnHuyBo.Enabled = enabled;
                btnLuu.Enabled = enabled;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Đẩy thông tin hành chính bệnh nhân lên các label.
        /// Ngày sinh xử lý theo IS_HAS_NO_DAY_DOB / IS_HAS_NO_MONTH_DOB:
        ///   - cả 2 = 0 (đủ): dd/MM/yyyy
        ///   - IS_HAS_NO_DAY_DOB = 1:  MM/yyyy
        ///   - IS_HAS_NO_MONTH_DOB = 1: yyyy
        /// </summary>
        private void BindPatientInfo(HIS_PATIENT patient)
        {
            try
            {
                if (patient == null) return;

                lblHoTen.Text = patient.VIR_PATIENT_NAME ?? "";
                lblNgaySinh.Text = FormatDob(patient);
                // TODO: Tra GENDER_NAME từ HIS_GENDER theo patient.GENDER_ID (qua BackendDataWorker)
                lblGioiTinh.Text = patient.GENDER_ID > 0 ? BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o=>o.ID == patient.GENDER_ID).GENDER_NAME : "";
                lblCCCD.Text = !string.IsNullOrEmpty(patient.CCCD_NUMBER) ? patient.CCCD_NUMBER : (patient.CMND_NUMBER ?? "");
                lblDienThoai.Text = patient.PHONE ?? "";
                lblDiaChi.Text = !string.IsNullOrEmpty(patient.VIR_ADDRESS) ? patient.VIR_ADDRESS : (patient.ADDRESS ?? "");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Format ngày sinh từ HIS_PATIENT.DOB (yyyyMMddHHmmss) có xét cờ
        /// IS_HAS_NO_DAY_DOB và IS_HAS_NO_MONTH_DOB.
        /// </summary>
        private string FormatDob(HIS_PATIENT patient)
        {
            try
            {
                if (patient == null || patient.DOB <= 0) return "";

                string raw = patient.DOB.ToString();
                // Đảm bảo đủ độ dài để cắt năm/tháng/ngày
                if (raw.Length < 4) return "";

                string yyyy = raw.Substring(0, 4);

                // Không có tháng → chỉ năm
                if (patient.IS_HAS_NOT_DAY_DOB == 1)
                {
                    return yyyy;
                }

                // Đầy đủ → dd/MM/yyyy
                return Inventec.Common.DateTime.Convert.TimeNumberToDateString(patient.DOB) ?? yyyy;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }

        /// <summary>
        /// Đẩy thông tin gói (HIS_PATIENT_PACKAGE) lên các control khi xem/sửa:
        /// mẫu gói (PACKAGE_ID), tên gói, ngày đăng ký, đối tượng TT, trạng thái, ghi chú,
        /// đồng thời nạp danh sách dịch vụ trong gói (HIS_PATIENT_PACKAGE_DT) và danh mục dịch vụ.
        /// Chống treo: chặn reload danh mục khi gán nhiều control rồi chỉ nạp lại 1 lần.
        /// </summary>
        private void BindPatientPackageInfo(HIS_PATIENT_PACKAGE patientPackage)
        {
            if (patientPackage == null) return;

            bool prevLoading = isFormLoading;
            try
            {
                WaitingManager.Show();
                // Chặn cascade LoadServiceCatalog trong khi gán nhiều control
                isFormLoading = true;

                cboMauGoi.EditValue = patientPackage.PACKAGE_ID.HasValue ? (object)patientPackage.PACKAGE_ID.Value : null;
                txtTenGoi.EditValue = patientPackage.PACKAGE_NAME;
                dteNgayDangKy.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientPackage.REGISTER_DATE);
                cboDoiTuongTT.EditValue = patientPackage.PATIENT_TYPE_ID;
                cboTrangThai.EditValue = patientPackage.STATUS_CODE;
                memGhiChu.EditValue = patientPackage.NOTE;

                // Dịch vụ trong gói (HIS_PATIENT_PACKAGE_DT)
                LoadPackageDetailServices(patientPackage.ID);
                // Chế độ sửa → hiện cột "SL đã dùng"
                RefreshPackageDetailGridMode();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                isFormLoading = prevLoading;
                WaitingManager.Hide();
            }

            // Khi chọn từ popup (form đã load xong): nạp lại danh mục dịch vụ 1 lần theo thông tin gói.
            // Khi đang load form: frmPatientPackageRegister_Load sẽ tự gọi LoadServiceCatalog sau đó.
            if (!prevLoading)
            {
                LoadServiceCatalog();
            }
        }

        /// <summary>
        /// Nạp danh sách dịch vụ thuộc gói bệnh nhân (V_HIS_PATIENT_PACKAGE_DT theo PATIENT_PACKAGE_ID)
        /// vào lưới "Dịch vụ trong gói". Dùng BindingList nên Clear/Add tự cập nhật lưới + tổng cộng.
        /// </summary>
        private void LoadPackageDetailServices(long patientPackageId)
        {
            try
            {
                selectedPackageServices.Clear();
                originalPackageDetails.Clear();

                if (patientPackageId > 0)
                {
                    CommonParam param = new CommonParam();
                    HisPatientPackageDtViewFilter filter = new HisPatientPackageDtViewFilter();
                    filter.PATIENT_PACKAGE_ID = patientPackageId;
                    filter.IS_ACTIVE = 1;

                    // TODO: endpoint api/VHisPatientPackageDt/Get cần backend bổ sung — confirm khi schema sẵn sàng.
                    List<V_HIS_PATIENT_PACKAGE_DT> details = new BackendAdapter(param)
                        .Get<List<V_HIS_PATIENT_PACKAGE_DT>>("api/HisPatientPackageDt/GetView", ApiConsumers.MosConsumer, filter, param);

                    if (details != null)
                    {
                        foreach (V_HIS_PATIENT_PACKAGE_DT dt in details)
                        {
                            PackageServiceADO ado = new PackageServiceADO
                            {
                                DT_ID = dt.ID,
                                // Phí gói lấy id từ NONE_MEDI_SERVICE_ID; dịch vụ thường lấy SERVICE_ID
                                SERVICE_ID = dt.IS_NONE_SERVICE == 1 ? (dt.NONE_MEDI_SERVICE_ID ?? 0) : (dt.SERVICE_ID ?? 0),
                                SERVICE_CODE = dt.SV_SERVICE_CODE,
                                SERVICE_NAME = !string.IsNullOrEmpty(dt.SERVICE_NAME) ? dt.SERVICE_NAME : dt.SV_SERVICE_NAME,
                                PRICE = dt.UNIT_PRICE,
                                AMOUNT = dt.AMOUNT,
                                TOTAL_PRICE = dt.UNIT_PRICE * dt.AMOUNT,
                                IS_NONE_SERVICE = dt.IS_NONE_SERVICE,
                                AMOUNT_USED = dt.AMOUNT_USED,
                                AMOUNT_PREPAID = dt.AMOUNT_PREPAID
                            };
                            selectedPackageServices.Add(ado);

                            // Lưu bản gốc để tính diff khi lưu (giữ AMOUNT_USED/PREPAID/PREPAID_USED)
                            originalPackageDetails.Add(new HIS_PATIENT_PACKAGE_DT
                            {
                                ID = dt.ID,
                                PATIENT_PACKAGE_ID = dt.PATIENT_PACKAGE_ID,
                                SERVICE_ID = dt.SERVICE_ID,
                                NONE_MEDI_SERVICE_ID = dt.NONE_MEDI_SERVICE_ID,
                                SERVICE_NAME = dt.SERVICE_NAME,
                                AMOUNT = dt.AMOUNT,
                                AMOUNT_USED = dt.AMOUNT_USED,
                                AMOUNT_PREPAID = dt.AMOUNT_PREPAID,
                                AMOUNT_PREPAID_USED = dt.AMOUNT_PREPAID_USED,
                                UNIT_PRICE = dt.UNIT_PRICE,
                                IS_NONE_SERVICE = dt.IS_NONE_SERVICE,
                                IS_ACTIVE = dt.IS_ACTIVE
                            });
                        }
                    }
                }

                UpdateTotalAmount();
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.PatientPackageRegister.Resources.Lang", typeof(frmPatientPackageRegister).Assembly);
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
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            try
            {
                SearchPatient();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMaBenhNhan_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    SearchPatient();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDanhSachGoi_Click(object sender, EventArgs e)
        {
            try
            {
                if (inputPatient == null)
                {
                    XtraMessageBox.Show("Vui lòng tìm kiếm bệnh nhân trước.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                CommonParam param = new CommonParam();
                HisPatientPackageFilter filter = new HisPatientPackageFilter();
                filter.PATIENT_ID = inputPatient.ID;
                filter.IS_ACTIVE = 1;

                WaitingManager.Show();
                // TODO: Endpoint api/HisPatientPackage/Get cần backend bổ sung — confirm URI khi schema sẵn sàng.
                List<HIS_PATIENT_PACKAGE> packages = new BackendAdapter(param)
                    .Get<List<HIS_PATIENT_PACKAGE>>("api/HisPatientPackage/Get", ApiConsumers.MosConsumer, filter, param);
                WaitingManager.Hide();

                if (packages == null || packages.Count == 0)
                {
                    XtraMessageBox.Show("Bệnh nhân chưa có gói dịch vụ đang hoạt động.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                ShowPatientPackageMenu(packages);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Hiển thị danh sách gói đã đăng ký của bệnh nhân dưới dạng popup lưới (khung rộng)
        /// neo dưới nút btnDanhSachGoi: Tên gói / Ngày đăng ký / Đối tượng TT / Trạng thái /
        /// Tổng đã đóng / Tổng đã hoàn / Tổng đã chi trả. Danh sách sắp xếp giảm dần theo ngày
        /// đăng ký (mới nhất lên đầu). Click 1 dòng / Enter → load gói lên form (chế độ sửa).
        /// Click ra ngoài / Esc → đóng popup.
        /// </summary>
        private void ShowPatientPackageMenu(List<HIS_PATIENT_PACKAGE> packages)
        {
            try
            {
                // Lưới hiển thị danh sách gói
                GridControl grid = new GridControl();
                GridView view = new GridView(grid);
                grid.MainView = view;
                grid.DataSource = packages;

                view.OptionsBehavior.Editable = false;
                view.OptionsView.ShowGroupPanel = false;
                view.OptionsView.ColumnAutoWidth = false;
                view.OptionsView.RowAutoHeight = false;
                view.OptionsSelection.MultiSelect = false;
                view.OptionsSelection.EnableAppearanceFocusedCell = false;
                view.FocusRectStyle = DrawFocusRectStyle.RowFocus;
                view.OptionsBehavior.AutoPopulateColumns = false;
                view.Columns.Clear();

                GridColumn colName = view.Columns.AddVisible("PACKAGE_NAME", "Tên gói");
                colName.VisibleIndex = 0;
                colName.Width = 230;
                colName.OptionsColumn.AllowEdit = false;

                // Ngày đăng ký (REGISTER_DATE — số yyyyMMddHHmmss → hiển thị dd/MM/yyyy)
                GridColumn colRegDate = view.Columns.AddVisible("REGISTER_DATE", "Ngày đăng ký");
                colRegDate.VisibleIndex = 1;
                colRegDate.Width = 100;
                colRegDate.OptionsColumn.AllowEdit = false;
                colRegDate.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                // Sắp xếp danh sách gói giảm dần theo ngày đăng ký (mới nhất lên đầu)
                colRegDate.SortOrder = DevExpress.Data.ColumnSortOrder.Descending;

                // Tên đối tượng thanh toán (PATIENT_TYPE_ID → HIS_PATIENT_TYPE.PATIENT_TYPE_NAME)
                GridColumn colPatyName = view.Columns.AddVisible("PATIENT_TYPE_ID", "Đối tượng TT");
                colPatyName.VisibleIndex = 2;
                colPatyName.Width = 160;
                colPatyName.OptionsColumn.AllowEdit = false;

                GridColumn colStatus = view.Columns.AddVisible("STATUS_CODE", "Trạng thái");
                colStatus.VisibleIndex = 3;
                colStatus.Width = 110;
                colStatus.OptionsColumn.AllowEdit = false;

                GridColumn colPaid = view.Columns.AddVisible("TOTAL_PAID", "Tổng đã đóng");
                colPaid.VisibleIndex = 4;
                colPaid.Width = 120;
                colPaid.OptionsColumn.AllowEdit = false;
                colPaid.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                colPaid.DisplayFormat.FormatString = "n0";
                colPaid.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

                GridColumn colRefunded = view.Columns.AddVisible("TOTAL_REFUNDED", "Tổng đã hoàn");
                colRefunded.VisibleIndex = 5;
                colRefunded.Width = 120;
                colRefunded.OptionsColumn.AllowEdit = false;
                colRefunded.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                colRefunded.DisplayFormat.FormatString = "n0";
                colRefunded.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

                GridColumn colUsed = view.Columns.AddVisible("TOTAL_USED", "Tổng đã chi trả");
                colUsed.VisibleIndex = 6;
                colUsed.Width = 120;
                colUsed.OptionsColumn.AllowEdit = false;
                colUsed.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                colUsed.DisplayFormat.FormatString = "n0";
                colUsed.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

                // Dịch mã trạng thái sang tiếng Việt khi hiển thị
                view.CustomColumnDisplayText += gvPatientPackageList_CustomColumnDisplayText;

                SetGridHeaderBold(view);

                // Form popup không viền, khung rộng, neo ngay dưới nút Danh sách gói
                Form popup = new Form();
                popup.FormBorderStyle = FormBorderStyle.None;
                popup.StartPosition = FormStartPosition.Manual;
                popup.ShowInTaskbar = false;
                popup.MinimizeBox = false;
                popup.MaximizeBox = false;
                popup.TopMost = true;
                popup.KeyPreview = true;
                popup.Size = new Size(990, 320);
                popup.Location = btnDanhSachGoi.PointToScreen(new Point(0, btnDanhSachGoi.Height));

                grid.Dock = DockStyle.Fill;
                popup.Controls.Add(grid);

                // Đóng popup khi click ra ngoài hoặc nhấn Esc
                popup.Deactivate += (s, ev) => { try { popup.Close(); } catch { } };
                popup.KeyDown += (s, ev) =>
                {
                    if (ev.KeyCode == Keys.Escape) popup.Close();
                };

                // Chọn gói: click 1 lần vào dòng (hoặc Enter)
                view.RowClick += (s, ev) =>
                {
                    if (ev.Button != System.Windows.Forms.MouseButtons.Left || ev.RowHandle < 0) return;
                    HIS_PATIENT_PACKAGE pkg = view.GetRow(ev.RowHandle) as HIS_PATIENT_PACKAGE;
                    if (pkg == null) return;
                    popup.Close();
                    ApplySelectedPatientPackage(pkg);
                };
                grid.KeyDown += (s, ev) =>
                {
                    if (ev.KeyCode == Keys.Enter)
                    {
                        HIS_PATIENT_PACKAGE pkg = view.GetFocusedRow() as HIS_PATIENT_PACKAGE;
                        if (pkg == null) return;
                        ev.Handled = true;
                        popup.Close();
                        ApplySelectedPatientPackage(pkg);
                    }
                };

                popup.Show(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Khi người dùng chọn 1 gói trong popup danh sách: chuyển form sang chế độ Sửa
        /// và bind dữ liệu của gói đó lên các control.
        /// </summary>
        private void ApplySelectedPatientPackage(HIS_PATIENT_PACKAGE pkg)
        {
            try
            {
                if (pkg == null) return;

                inputPatientPackage = pkg;
                isEditMode = true;
                this.Text = "Sửa gói dịch vụ";
                BindPatientPackageInfo(pkg);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Hiển thị trên popup danh sách gói: trạng thái (STATUS_CODE) → tiếng Việt,
        /// ngày đăng ký (REGISTER_DATE số) → dd/MM/yyyy, đối tượng TT (PATIENT_TYPE_ID) → tên.
        /// </summary>
        private void gvPatientPackageList_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.Column == null) return;

                if (e.Column.FieldName == "STATUS_CODE")
                {
                    e.DisplayText = GetStatusName(e.Value == null ? "" : e.Value.ToString());
                }
                else if (e.Column.FieldName == "REGISTER_DATE")
                {
                    e.DisplayText = FormatRegisterDate(e.Value);
                }
                else if (e.Column.FieldName == "PATIENT_TYPE_ID")
                {
                    long id;
                    e.DisplayText = (e.Value != null && long.TryParse(e.Value.ToString(), out id) && patientTypeNameById.ContainsKey(id))
                        ? patientTypeNameById[id]
                        : "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Format ngày đăng ký gói để hiển thị (dd/MM/yyyy). Hỗ trợ cả kiểu số yyyyMMddHHmmss
        /// (REGISTER_DATE hiện tại) lẫn DateTime để an toàn nếu kiểu thay đổi.
        /// </summary>
        private string FormatRegisterDate(object value)
        {
            try
            {
                if (value == null) return "";
                if (value is DateTime) return ((DateTime)value).ToString("dd/MM/yyyy");
                long t;
                if (long.TryParse(value.ToString(), out t) && t > 0)
                {
                    return Inventec.Common.DateTime.Convert.TimeNumberToDateString(t) ?? "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }

        /// <summary>
        /// Đổi mã trạng thái gói sang tên tiếng Việt.
        /// </summary>
        private string GetStatusName(string statusCode)
        {
            switch (statusCode)
            {
                case STATUS_CODE__REGISTERED: return "Đăng ký";
                case STATUS_CODE__IN_USE: return "Đang sử dụng";
                case STATUS_CODE__LOCKED: return "Đã khóa";
                default: return statusCode;
            }
        }

        private void cboMauGoi_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                long packageId = GetLongEditValue(cboMauGoi.EditValue);
                if (packageId > 0)
                {
                    // Lấy entity HIS_PACKAGE đã chọn từ DataSource (đã load sẵn)
                    List<HIS_PACKAGE> source = cboMauGoi.Properties.DataSource as List<HIS_PACKAGE>;
                    HIS_PACKAGE selected = source != null ? source.FirstOrDefault(p => p.ID == packageId) : null;
                    if (selected != null)
                    {
                        // Gợi ý các trường để bind tự động khi chọn mẫu
                        txtTenGoi.EditValue = selected.PACKAGE_NAME;
                    }
                }

                // Đổi gói → đổi package_id của chính sách giá → nạp lại danh mục dịch vụ
                if (isFormLoading) return;
                LoadServiceCatalog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Load danh sách dịch vụ thuộc 1 mẫu gói (HIS_PACKAGE) vào grdDichVuTrongGoi.
        /// TODO: Khi backend bổ sung endpoint api/HisPackageService/Get hoặc tương đương,
        /// thay đổi tên URI và filter cho phù hợp.
        /// </summary>
        private void LoadPackageServices(long packageId)
        {
            try
            {
                // TODO: Thay bằng endpoint thực tế khi schema HIS_PACKAGE_SERVICE sẵn sàng.
                // VD:
                //     HisPackageServiceFilter filter = new HisPackageServiceFilter();
                //     filter.PACKAGE_ID = packageId;
                //     filter.IS_ACTIVE = 1;
                //     var services = new BackendAdapter(param)
                //         .Get<List<HIS_PACKAGE_SERVICE>>("api/HisPackageService/Get", ApiConsumers.MosConsumer, filter, param);
                //     grdDichVuTrongGoi.DataSource = services;
                //     gvDichVuTrongGoi.PopulateColumns();
                grdDichVuTrongGoi.DataSource = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDoiTuongTT_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (isFormLoading) return;
                // Đổi đối tượng thanh toán → chính sách giá khác → nạp lại danh mục dịch vụ
                LoadServiceCatalog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTrangThai_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                // TODO: Cập nhật UI theo trạng thái
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTimKiemDV_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                // Không lọc khi gõ (tránh lag) — chỉ lọc khi nhấn Enter (xem txtTimKiemDV_KeyDown)
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Chỉ lọc danh mục dịch vụ khi nhấn Enter trong ô từ khóa (tránh lọc liên tục gây lag).
        /// </summary>
        private void txtTimKiemDV_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    ApplyCatalogSearchFilter();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoaiDV_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (isFormLoading) return;
                // Chọn loại dịch vụ (hoặc "Tất cả") → nạp lại lưới danh mục theo loại
                LoadServiceCatalog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPhiGoi_Click(object sender, EventArgs e)
        {
            try
            {
                // Phí gói không cần chọn mẫu gói / package_id — chỉ cần đã có bệnh nhân (nút đã bật theo đó)
                List<NoneMediServiceADO> list = LoadNoneMediServiceList();
                if (list.Count == 0)
                {
                    XtraMessageBox.Show("Không có dịch vụ phí gói (HIS_NONE_MEDI_SERVICE).", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                ShowNoneMediServicePopup(list);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Lấy danh sách dịch vụ phí gói (HIS_NONE_MEDI_SERVICE) qua API, tra cứu tên đơn vị tính
        /// (HIS_SERVICE_UNIT) và loại (HIS_GOODS_TYPE) từ BackendDataWorker để hiển thị.
        /// </summary>
        private List<NoneMediServiceADO> LoadNoneMediServiceList()
        {
            List<NoneMediServiceADO> result = new List<NoneMediServiceADO>();
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                HisNoneMediServiceFilter filter = new HisNoneMediServiceFilter();
                filter.IS_ACTIVE = 1;
                List<HIS_NONE_MEDI_SERVICE> services = new BackendAdapter(param)
                    .Get<List<HIS_NONE_MEDI_SERVICE>>("api/HisNoneMediService/Get", ApiConsumers.MosConsumer, filter, param);
                WaitingManager.Hide();

                if (services == null) return result;

                // Tra cứu tên bảng phụ theo _ID (cache RAM)
                List<HIS_SERVICE_UNIT> units = BackendDataWorker.Get<HIS_SERVICE_UNIT>() ?? new List<HIS_SERVICE_UNIT>();
                List<HIS_GOODS_TYPE> goodsTypes = BackendDataWorker.Get<HIS_GOODS_TYPE>() ?? new List<HIS_GOODS_TYPE>();
                Dictionary<long, string> unitNames = units.GroupBy(o => o.ID).ToDictionary(g => g.Key, g => g.First().SERVICE_UNIT_NAME);
                Dictionary<long, string> goodsTypeNames = goodsTypes.GroupBy(o => o.ID).ToDictionary(g => g.Key, g => g.First().GOODS_TYPE_NAME);

                foreach (HIS_NONE_MEDI_SERVICE s in services)
                {
                    result.Add(new NoneMediServiceADO
                    {
                        SERVICE_ID = s.ID,
                        SERVICE_CODE = s.NONE_MEDI_SERVICE_CODE,
                        SERVICE_NAME = s.NONE_MEDI_SERVICE_NAME,
                        PRICE = s.PRICE ?? 0,
                        VAT_RATIO = s.VAT_RATIO ?? 0,
                        SERVICE_UNIT_NAME = unitNames.ContainsKey(s.SERVICE_UNIT_ID) ? unitNames[s.SERVICE_UNIT_ID] : "",
                        GOODS_TYPE_NAME = (s.GOODS_TYPE_ID.HasValue && goodsTypeNames.ContainsKey(s.GOODS_TYPE_ID.Value)) ? goodsTypeNames[s.GOODS_TYPE_ID.Value] : ""
                    });
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Cửa sổ nhỏ chọn dịch vụ phí gói: lưới đa chọn (checkbox) gồm Mã / Tên / Giá tiền / VAT /
        /// Đơn vị tính / Loại. Bấm "Thêm" → đưa các dịch vụ đã chọn vào "Dịch vụ trong gói".
        /// </summary>
        private void ShowNoneMediServicePopup(List<NoneMediServiceADO> list)
        {
            try
            {
                GridControl grid = new GridControl();
                GridView view = new GridView(grid);
                grid.MainView = view;
                grid.DataSource = list;

                view.OptionsBehavior.Editable = false;
                view.OptionsView.ShowGroupPanel = false;
                view.OptionsView.ColumnAutoWidth = false;
                view.OptionsSelection.MultiSelect = true;
                view.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
                view.OptionsBehavior.AutoPopulateColumns = false;
                view.Columns.Clear();

                view.Columns.AddVisible("SERVICE_CODE", "Mã").Width = 90;
                view.Columns.AddVisible("SERVICE_NAME", "Tên dịch vụ").Width = 260;
                GridColumn colPrice = view.Columns.AddVisible("PRICE", "Giá tiền");
                colPrice.Width = 100;
                colPrice.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                colPrice.DisplayFormat.FormatString = "n0";
                colPrice.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                GridColumn colVat = view.Columns.AddVisible("VAT_RATIO", "VAT");
                colVat.Width = 60;
                colVat.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                colVat.DisplayFormat.FormatString = "n2";
                view.Columns.AddVisible("SERVICE_UNIT_NAME", "Đơn vị tính").Width = 110;
                view.Columns.AddVisible("GOODS_TYPE_NAME", "Loại").Width = 130;

                SetGridHeaderBold(view);

                Form popup = new Form();
                popup.Text = "Chọn phí gói";
                popup.StartPosition = FormStartPosition.CenterParent;
                popup.FormBorderStyle = FormBorderStyle.FixedDialog;
                popup.MinimizeBox = false;
                popup.MaximizeBox = false;
                popup.ShowInTaskbar = false;
                popup.ClientSize = new Size(820, 460);

                grid.Location = new Point(0, 0);
                grid.Size = new Size(820, 460 - 40);
                grid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

                SimpleButton btnAdd = new SimpleButton();
                btnAdd.Text = "Thêm";
                btnAdd.Size = new Size(90, 28);
                btnAdd.Location = new Point(820 - 90 - 8, 460 - 28 - 6);
                btnAdd.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

                popup.Controls.Add(grid);
                popup.Controls.Add(btnAdd);
                popup.AcceptButton = btnAdd;

                btnAdd.Click += (s, ev) =>
                {
                    int[] rows = view.GetSelectedRows();
                    if (rows == null || rows.Length == 0)
                    {
                        XtraMessageBox.Show("Vui lòng chọn ít nhất 1 dịch vụ.", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    foreach (int rowHandle in rows)
                    {
                        NoneMediServiceADO ado = view.GetRow(rowHandle) as NoneMediServiceADO;
                        AddNoneMediServiceToPackage(ado);
                    }
                    popup.DialogResult = DialogResult.OK;
                    popup.Close();
                };

                popup.ShowDialog(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Thêm 1 dịch vụ phí gói vào "Dịch vụ trong gói": đơn giá = PRICE × (1 + VAT_RATIO),
        /// đánh dấu IS_NONE_SERVICE = 1. Tránh trùng theo (IS_NONE_SERVICE, SERVICE_ID).
        /// </summary>
        private void AddNoneMediServiceToPackage(NoneMediServiceADO src)
        {
            try
            {
                if (src == null) return;
                if (selectedPackageServices.Any(o => o.IS_NONE_SERVICE == 1 && o.SERVICE_ID == src.SERVICE_ID)) return;

                decimal priceVat = src.PRICE * (1 + src.VAT_RATIO);
                PackageServiceADO ado = new PackageServiceADO
                {
                    SERVICE_ID = src.SERVICE_ID,
                    SERVICE_CODE = src.SERVICE_CODE,
                    SERVICE_NAME = src.SERVICE_NAME,
                    PRICE = priceVat,
                    AMOUNT = 1,
                    TOTAL_PRICE = priceVat,
                    IS_NONE_SERVICE = 1
                };
                selectedPackageServices.Add(ado);
                UpdateTotalAmount();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnInPhieu_Click(object sender, EventArgs e)
        {
            try
            {
                // Chỉ in khi gói đã lưu (có ID) → in đúng dữ liệu mới nhất đã lưu
                if (inputPatient == null || inputPatientPackage == null || inputPatientPackage.ID <= 0)
                {
                    XtraMessageBox.Show("Vui lòng lưu gói dịch vụ trước khi in.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Tải template phiếu in theo mã loại phiếu, rồi gọi delegate sinh dữ liệu + in
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(
                    ApiConsumers.SarConsumer,
                    HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR,
                    LanguageManager.GetLanguage(),
                    HIS.Desktop.LocalStorage.LocalData.GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PRINT_TYPE_CODE__MPS000514, DelegatePrintMps000514);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool DelegatePrintMps000514(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                if (printCode == PRINT_TYPE_CODE__MPS000514)
                {
                    InPhieuGoiDichVu(printCode, fileName, ref result);
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// In phiếu gói dịch vụ (MPS000514). Luôn LẤY DỮ LIỆU MỚI NHẤT từ server theo ID gói vừa lưu
        /// (HIS_PATIENT_PACKAGE + HIS_PATIENT_PACKAGE_DT) để đảm bảo in ra đúng dữ liệu vừa lưu.
        /// </summary>
        private void InPhieuGoiDichVu(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                // Gói (dữ liệu mới nhất theo ID)
                HisPatientPackageFilter pkgFilter = new HisPatientPackageFilter();
                pkgFilter.ID = inputPatientPackage.ID;
                List<HIS_PATIENT_PACKAGE> pkgs = new BackendAdapter(param)
                    .Get<List<HIS_PATIENT_PACKAGE>>("api/HisPatientPackage/Get", ApiConsumers.MosConsumer, pkgFilter, param);
                HIS_PATIENT_PACKAGE pkg = pkgs != null ? pkgs.FirstOrDefault() : null;

                // Chi tiết dịch vụ trong gói (dữ liệu mới nhất)
                HisPatientPackageDtFilter dtFilter = new HisPatientPackageDtFilter();
                dtFilter.PATIENT_PACKAGE_ID = inputPatientPackage.ID;
                dtFilter.IS_ACTIVE = 1;
                List<HIS_PATIENT_PACKAGE_DT> details = new BackendAdapter(param)
                    .Get<List<HIS_PATIENT_PACKAGE_DT>>("api/HisPatientPackageDt/Get", ApiConsumers.MosConsumer, dtFilter, param);

                if (pkg == null)
                {
                    WaitingManager.Hide();
                    XtraMessageBox.Show("Không lấy được dữ liệu gói để in.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                MPS.Processor.Mps000514.PDO.Mps000514PDO pdo = new MPS.Processor.Mps000514.PDO.Mps000514PDO(
                    inputPatient,
                    pkg,
                    details ?? new List<HIS_PATIENT_PACKAGE_DT>());

                string printerName = "";
                var dicPrinter = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicPrinter;
                if (dicPrinter != null && dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = dicPrinter[printTypeCode];
                }

                WaitingManager.Hide();

                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(
                    printTypeCode, fileName, pdo,
                    MPS.ProcessorBase.PrintConfig.PreviewType.Show,
                    printerName));
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnHuyBo_Click(object sender, EventArgs e)
        {
            try
            {
                ResetToNewMode();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Hủy bỏ: xóa toàn bộ thông tin gói + dịch vụ trong gói, đưa form về chế độ thêm mới
        /// (giữ nguyên bệnh nhân đang chọn). Chặn reload danh mục nhiều lần để không treo,
        /// chỉ nạp lại danh mục dịch vụ 1 lần ở cuối.
        /// </summary>
        private void ResetToNewMode()
        {
            bool prevLoading = isFormLoading;
            try
            {
                isFormLoading = true;

                // Về chế độ thêm mới
                inputPatientPackage = null;
                isEditMode = false;
                this.Text = "Đăng ký gói dịch vụ";

                // Xóa thông tin gói
                cboMauGoi.EditValue = null;
                txtTenGoi.EditValue = "";
                dteNgayDangKy.EditValue = DateTime.Today;
                memGhiChu.EditValue = "";
                cboTrangThai.EditValue = STATUS_CODE__REGISTERED;
                SetDefaultDoiTuongTT();

                // Xóa dịch vụ trong gói
                selectedPackageServices.Clear();
                originalPackageDetails.Clear();
                UpdateTotalAmount();

                // Thêm mới: ẩn cột "SL đã dùng" (nút Phí gói vẫn bật vì giữ nguyên bệnh nhân)
                RefreshPackageDetailGridMode();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                isFormLoading = prevLoading;
            }

            // Nạp lại danh mục dịch vụ 1 lần (theo đối tượng TT/ngày hiện tại, không có package_id)
            LoadServiceCatalog();
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                // Commit ô đang sửa trên lưới (tên dịch vụ / SL / thành tiền) để lấy đúng giá trị mới nhất
                gvDichVuTrongGoi.PostEditor();
                gvDichVuTrongGoi.UpdateCurrentRow();

                if (!ValidateBeforeSave()) return;

                CommonParam param = new CommonParam();
                PatientPackageResultSDO result = null;

                WaitingManager.Show();
                if (isEditMode && inputPatientPackage != null)
                {
                    // SỬA: HIS_PATIENT_PACKAGE + 3 danh sách chi tiết (sửa / xóa / thêm mới)
                    List<HIS_PATIENT_PACKAGE_DT> updates = new List<HIS_PATIENT_PACKAGE_DT>();
                    List<HIS_PATIENT_PACKAGE_DT> deletes = new List<HIS_PATIENT_PACKAGE_DT>();
                    List<HIS_PATIENT_PACKAGE_DT> creates = new List<HIS_PATIENT_PACKAGE_DT>();
                    BuildUpdateDetailLists(updates, deletes, creates);

                    PatientPackageUpdateSDO sdo = new PatientPackageUpdateSDO();
                    sdo.PatientPackage = BuildPatientPackageFromForm(inputPatientPackage);
                    sdo.PatientPackageDtUpdates = updates;
                    sdo.PatientPackageDtDeletes = deletes;
                    sdo.PatientPackageDtCreates = creates;

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                    result = new BackendAdapter(param)
                        .Post<PatientPackageResultSDO>("api/HisPatientPackage/UpdateSdo", ApiConsumers.MosConsumer, sdo, param);

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));
                    WaitingManager.Hide();
                }
                else
                {
                    // TẠO MỚI: HIS_PATIENT_PACKAGE + toàn bộ chi tiết
                    PatientPackageCreateSDO sdo = new PatientPackageCreateSDO();
                    sdo.PatientPackage = BuildPatientPackageFromForm(null);
                    sdo.PatientPackageDts = selectedPackageServices.Select(o => BuildDtFromAdo(o, 0)).ToList();

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                    result = new BackendAdapter(param)
                        .Post<PatientPackageResultSDO>("api/HisPatientPackage/CreateSdo", ApiConsumers.MosConsumer, sdo, param);

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));
                    WaitingManager.Hide();
                }

                if (result != null && result.PatientPackage != null)
                {
                    // Chuyển sang chế độ sửa với dữ liệu vừa lưu (đồng bộ ID chi tiết cho lần lưu sau)
                    ApplySelectedPatientPackage(result.PatientPackage);
                }

                MessageManager.Show(this, param, result != null && result.PatientPackage != null);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Kiểm tra điều kiện tối thiểu trước khi lưu gói.
        /// </summary>
        private bool ValidateBeforeSave()
        {
            if (inputPatient == null)
            {
                XtraMessageBox.Show("Vui lòng tìm kiếm bệnh nhân trước.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            // Tên gói (PACKAGE_NAME): bắt buộc + chặn số ký tự bằng thư viện ControlMaxLengthValidationRule
            // (hiển thị lỗi ngay tại ô nhập — xem SetupRequiredValidation).
            if (!dxValidationProvider.Validate())
            {
                txtTenGoi.Focus();
                return false;
            }
            // Đối tượng TT (PATIENT_TYPE_ID — NOT NULL)
            if (GetLongEditValue(cboDoiTuongTT.EditValue) <= 0)
            {
                XtraMessageBox.Show("Vui lòng chọn đối tượng thanh toán.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                cboDoiTuongTT.Focus();
                return false;
            }
            // Ngày đăng ký (REGISTER_DATE — NOT NULL)
            if (!(dteNgayDangKy.EditValue is DateTime))
            {
                XtraMessageBox.Show("Vui lòng nhập ngày đăng ký.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                dteNgayDangKy.Focus();
                return false;
            }
            // Trạng thái (STATUS_CODE — NOT NULL)
            if (cboTrangThai.EditValue == null || string.IsNullOrEmpty(cboTrangThai.EditValue.ToString()))
            {
                XtraMessageBox.Show("Vui lòng chọn trạng thái gói.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                cboTrangThai.Focus();
                return false;
            }

            if (selectedPackageServices.Count == 0)
            {
                XtraMessageBox.Show("Gói chưa có dịch vụ nào.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Dựng HIS_PATIENT_PACKAGE từ thông tin trên form. existing = null khi tạo mới
        /// (giữ TOTAL_*, ID... khi sửa). Gói mới luôn ở trạng thái Đăng ký.
        /// </summary>
        private HIS_PATIENT_PACKAGE BuildPatientPackageFromForm(HIS_PATIENT_PACKAGE existing)
        {
            // Sửa: existing (đã có ID) → giữ nguyên ID bản ghi. Tạo mới: new HIS_PATIENT_PACKAGE (ID = 0) → không truyền ID
            HIS_PATIENT_PACKAGE pkg = existing ?? new HIS_PATIENT_PACKAGE();
            pkg.PATIENT_ID = inputPatient.ID;
            long pkgId = GetLongEditValue(cboMauGoi.EditValue);
            pkg.PACKAGE_ID =  pkgId > 0 ? (long?)pkgId : null;
            pkg.PACKAGE_NAME = txtTenGoi.EditValue == null ? "" : txtTenGoi.EditValue.ToString();
            if (dteNgayDangKy.EditValue is DateTime)
            {
                // Chỉ lấy phần ngày → yyyyMMdd000000 (bỏ giờ phút giây)
                pkg.REGISTER_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteNgayDangKy.DateTime.Date) ?? 0;
            }
            pkg.PATIENT_TYPE_ID = GetLongEditValue(cboDoiTuongTT.EditValue);
            pkg.NOTE = memGhiChu.EditValue == null ? null : memGhiChu.EditValue.ToString();
            pkg.STATUS_CODE = cboTrangThai.EditValue == null ? existing.STATUS_CODE : cboTrangThai.EditValue.ToString();
            return pkg;
        }

        /// <summary>
        /// Dựng 1 dòng HIS_PATIENT_PACKAGE_DT từ ADO. Dịch vụ phí gói (IS_NONE_SERVICE=1) để
        /// SERVICE_ID = null (không phải FK HIS_SERVICE).
        /// </summary>
        private HIS_PATIENT_PACKAGE_DT BuildDtFromAdo(PackageServiceADO ado, long patientPackageId)
        {
            HIS_PATIENT_PACKAGE_DT dt = new HIS_PATIENT_PACKAGE_DT();
            // Sửa (bản ghi đã có DT_ID) → truyền ID; Thêm mới → KHÔNG truyền ID (giữ 0 để backend tự sinh)
            dt.ID = ado.DT_ID > 0 ? ado.DT_ID : 0;
            if (patientPackageId > 0) dt.PATIENT_PACKAGE_ID = patientPackageId;
            // Phí gói (IS_NONE_SERVICE=1): lưu vào NONE_MEDI_SERVICE_ID (FK HIS_NONE_MEDI_SERVICE), SERVICE_ID = null.
            // Dịch vụ kỹ thuật thường: lưu SERVICE_ID (FK HIS_SERVICE), NONE_MEDI_SERVICE_ID = null.
            if (ado.IS_NONE_SERVICE == 1)
            {
                dt.SERVICE_ID = null;
                dt.NONE_MEDI_SERVICE_ID = ado.SERVICE_ID;
            }
            else
            {
                dt.SERVICE_ID = ado.SERVICE_ID;
                dt.NONE_MEDI_SERVICE_ID = null;
            }
            dt.SERVICE_NAME = ado.SERVICE_NAME;
            dt.AMOUNT = ado.AMOUNT;
            dt.UNIT_PRICE = ado.PRICE;
            dt.IS_NONE_SERVICE = (short)ado.IS_NONE_SERVICE;
            dt.AMOUNT_USED = ado.AMOUNT_USED;
            dt.AMOUNT_PREPAID = ado.AMOUNT_PREPAID;
            dt.IS_ACTIVE = 1;
            return dt;
        }

        /// <summary>
        /// Tính 3 danh sách chi tiết khi sửa: thêm mới (chưa có DT_ID), sửa (đã tồn tại — giữ
        /// nguyên dữ liệu gốc, ghi đè SL + đơn giá), xóa (dòng gốc đã bị bỏ khỏi lưới).
        /// </summary>
        private void BuildUpdateDetailLists(List<HIS_PATIENT_PACKAGE_DT> updates, List<HIS_PATIENT_PACKAGE_DT> deletes, List<HIS_PATIENT_PACKAGE_DT> creates)
        {
            long packageId = inputPatientPackage.ID;
            HashSet<long> currentIds = new HashSet<long>();

            foreach (PackageServiceADO ado in selectedPackageServices)
            {
                if (ado.DT_ID <= 0)
                {
                    // Thêm mới
                    creates.Add(BuildDtFromAdo(ado, packageId));
                }
                else
                {
                    // Sửa: lấy bản gốc (giữ AMOUNT_USED/PREPAID/PREPAID_USED), ghi đè SL + đơn giá
                    currentIds.Add(ado.DT_ID);
                    HIS_PATIENT_PACKAGE_DT dt = originalPackageDetails.FirstOrDefault(o => o.ID == ado.DT_ID);
                    if (dt == null) dt = BuildDtFromAdo(ado, packageId);
                    dt.PATIENT_PACKAGE_ID = packageId;
                    dt.AMOUNT = ado.AMOUNT;
                    dt.UNIT_PRICE = ado.PRICE;
                    updates.Add(dt);
                }
            }

            // Xóa: dòng gốc không còn trong lưới
            foreach (HIS_PATIENT_PACKAGE_DT orig in originalPackageDetails)
            {
                if (!currentIds.Contains(orig.ID)) deletes.Add(orig);
            }
        }

        private void barButtonItemLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnLuu_Click(sender, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItemInPhieu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnInPhieu_Click(sender, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
