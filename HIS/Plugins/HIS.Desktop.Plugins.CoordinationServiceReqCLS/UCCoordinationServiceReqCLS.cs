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
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.CoordinationServiceReqCLS.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CoordinationServiceReqCLS
{
    public partial class UCCoordinationServiceReqCLS : UserControlBase
    {
        #region Declare
        internal Inventec.Desktop.Common.Modules.Module currentModule { get; set; }
        string loginName = null;

        /// <summary>Toàn bộ y lệnh CLS trả về từ API (chưa gom nhóm).</summary>
        List<HisServiceReqGetServiceReqCLSSDO> listAllServiceReq;

        /// <summary>Danh sách bệnh nhân sau khi gom theo mã điều trị (nguồn của lưới trái).</summary>
        List<CoordinationPatientADO> listPatient;

        int dataTotal = 0;
        int start = 0;

        /// <summary>Cờ chặn nạp chi tiết khi grid tự động focus trong lúc load/tìm (tránh auto-focus từ bảng trên).</summary>
        bool isLoadingData = false;

        const string moduleLink = "HIS.Desktop.Plugins.CoordinationServiceReqCLS";
        #endregion

        #region Construct
        public UCCoordinationServiceReqCLS()
            : base(null)
        {
            InitializeComponent();
        }

        public UCCoordinationServiceReqCLS(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
                this.loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager(
                    "HIS.Desktop.Plugins.CoordinationServiceReqCLS.Resources.Lang",
                    typeof(UCCoordinationServiceReqCLS).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Load
        private void UCCoordinationServiceReqCLS_Load(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
                SetCaptionByLanguageKey();
                InitControlState();
                // Tự động tải danh sách trong ngày hiện tại khi mở chức năng (mục 5.2)
                FillDataToGrid();
                RunTimerLoadCPA();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Giá trị mặc định: khoảng ngày = ngày hiện tại, số giây auto-refresh mặc định.</summary>
        private void SetDefaultValue()
        {
            try
            {
                this.dtIntructionDateFrom.DateTime = DateTime.Now.Date;
                this.dtIntructionDateTo.DateTime = DateTime.Now.Date;
                this.spnAutoReloadSeconds.Value = 60;
                this.chkAutoReload.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Set caption đa ngôn ngữ cho các control (mục 13 ui_rules).</summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                var res = Resources.ResourceLanguageManager.LanguageResource;
                var culture = LanguageManager.GetCulture();

                this.lciIntructionDateFrom.Text = Inventec.Common.Resource.Get.Value("UCCoordinationServiceReqCLS.lciIntructionDateFrom.Text", res, culture);
                this.lciIntructionDateTo.Text = Inventec.Common.Resource.Get.Value("UCCoordinationServiceReqCLS.lciIntructionDateTo.Text", res, culture);
                this.lciTreatmentCode.Text = Inventec.Common.Resource.Get.Value("UCCoordinationServiceReqCLS.lciTreatmentCode.Text", res, culture);
                this.lciPatientName.Text = Inventec.Common.Resource.Get.Value("UCCoordinationServiceReqCLS.lciPatientName.Text", res, culture);
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("UCCoordinationServiceReqCLS.btnFind.Text", res, culture);
                this.btnClear.Text = Inventec.Common.Resource.Get.Value("UCCoordinationServiceReqCLS.btnClear.Text", res, culture);
                this.chkAutoReload.Properties.Caption = Inventec.Common.Resource.Get.Value("UCCoordinationServiceReqCLS.chkAutoReload.Text", res, culture);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Filter
        /// <summary>Tự pad '0' cho mã điều trị đủ 12 số và hiển thị lại đầy đủ trong ô nhập.</summary>
        private void NormalizeTreatmentCodeDisplay()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(this.txtTreatmentCode.Text))
                {
                    string code = this.txtTreatmentCode.Text.Trim();
                    if (code.Length < 12)
                        code = code.PadLeft(12, '0');
                    if (this.txtTreatmentCode.Text != code)
                        this.txtTreatmentCode.Text = code;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Build filter từ điều kiện tìm kiếm trên form.</summary>
        private void GetFilter(ref HisServiceReqViewFilterQuery filter)
        {
            try
            {
                if (this.dtIntructionDateFrom.EditValue != null)
                    filter.INTRUCTION_DATE_FROM = Int64.Parse(this.dtIntructionDateFrom.DateTime.ToString("yyyyMMdd000000"));

                if (this.dtIntructionDateTo.EditValue != null)
                    filter.INTRUCTION_DATE_TO = Int64.Parse(this.dtIntructionDateTo.DateTime.ToString("yyyyMMdd235959"));

                // Mã điều trị: tự pad '0' cho đủ 12 số
                if (!string.IsNullOrWhiteSpace(this.txtTreatmentCode.Text))
                {
                    string treatmentCode = this.txtTreatmentCode.Text.Trim();
                    if (treatmentCode.Length < 12)
                        treatmentCode = treatmentCode.PadLeft(12, '0');
                    filter.TREATMENT_CODE = treatmentCode;
                }
                filter.PATIENT_NAME = string.IsNullOrWhiteSpace(this.txtPatientName.Text) ? null : this.txtPatientName.Text.Trim();
                filter.REQUEST_ROOM_ID = this.currentModule != null ? (long?)this.currentModule.RoomId : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Kiểm tra điều kiện tìm kiếm bắt buộc.</summary>
        private bool ValidateFilter()
        {
            try
            {
                if (this.dtIntructionDateFrom.EditValue == null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        Resources.ResourceMessage.ThoiGianTuKhongDuocBoTrong,
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (this.dtIntructionDateFrom.EditValue != null && this.dtIntructionDateTo.EditValue != null
                    && this.dtIntructionDateFrom.DateTime.Date > this.dtIntructionDateTo.DateTime.Date)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        Resources.ResourceMessage.ThoiGianTuPhaiNhoHonDen,
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }
        #endregion

        #region Load data
        /// <summary>Gọi API GetServiceReqCLS 1 lần → gom nhóm bệnh nhân → khởi tạo phân trang lưới trái.</summary>
        private void FillDataToGrid()
        {
            try
            {
                if (!ValidateFilter()) return;

                isLoadingData = true;

                // Tự pad '0' đủ 12 số và hiển thị mã đầy đủ ngay trong ô trước khi gọi API
                NormalizeTreatmentCodeDisplay();

                LoadAllDataFromApi();

                int pageSize = ucPaging1.pagingGrid != null ? ucPaging1.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                if (pageSize <= 0) pageSize = 50;

                FillPatientGridTransaction(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = pageSize;
                param.Count = dataTotal;
                ucPaging1.Init(FillPatientGridTransaction, param, pageSize, this.gridControlPatient);

                // Bỏ focus dòng mặc định (DevExpress 15.2 tự focus row 0 ở pha layout sau bind).
                // Đặt deferred để chạy SAU cùng, không bị grid ghi đè → không tự chọn dòng, lưới chi tiết trống.
                if (this.IsHandleCreated)
                    this.BeginInvoke((MethodInvoker)(() => ClearPatientFocus()));
                else
                    ClearPatientFocus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Chuyển focus bàn phím về ô Mã điều trị (để nhập điều kiện tìm tiếp).</summary>
        private void FocusTreatmentCode()
        {
            try
            {
                this.txtTreatmentCode.Focus();
                this.txtTreatmentCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Bỏ focus dòng trên lưới bệnh nhân (không tự chọn dòng nào) và xóa lưới chi tiết.</summary>
        private void ClearPatientFocus()
        {
            try
            {
                gridViewPatient.FocusedRowHandle = DevExpress.XtraGrid.GridControl.InvalidRowHandle;

                gridControlServiceReq.BeginUpdate();
                try
                {
                    gridControlServiceReq.DataSource = null;
                }
                finally
                {
                    gridControlServiceReq.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                // Hết giai đoạn load — từ giờ click/di chuyển dòng mới nạp chi tiết
                isLoadingData = false;
            }
        }

        /// <summary>Chọn dòng bệnh nhân đầu tiên và nạp chi tiết y lệnh sang lưới phải.</summary>
        private void SelectFirstPatientAndLoadDetail()
        {
            try
            {
                if (gridViewPatient.RowCount > 0)
                {
                    this.gridControlPatient.Focus();
                    // Ép đổi focus -1 -> 0 để chắc chắn FocusedRowChanged kích hoạt (luồng render đã chạy đúng khi đổi dòng)
                    gridViewPatient.FocusedRowHandle = -1;
                    gridViewPatient.FocusedRowHandle = 0;
                    // Nạp chi tiết trực tiếp làm dự phòng
                    CoordinationPatientADO firstRow = gridViewPatient.GetRow(0) as CoordinationPatientADO;
                    FillServiceReqGrid(firstRow);
                }
                else
                {
                    FillServiceReqGrid(null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Gọi API và gom danh sách y lệnh theo mã điều trị.</summary>
        private void LoadAllDataFromApi()
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                HisServiceReqViewFilterQuery filter = new HisServiceReqViewFilterQuery();
                GetFilter(ref filter);

                Inventec.Common.Logging.LogSystem.Debug(
                    Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));

                var result = new BackendAdapter(param).Get<List<HisServiceReqGetServiceReqCLSSDO>>(
                    RequestUriStore.HIS_SERVICE_REQ_GET_SERVICE_REQ_CLS,
                    ApiConsumers.MosConsumer, filter, param);

                listAllServiceReq = result ?? new List<HisServiceReqGetServiceReqCLSSDO>();
                BuildPatientList();

                WaitingManager.Hide();
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                listAllServiceReq = new List<HisServiceReqGetServiceReqCLSSDO>();
                listPatient = new List<CoordinationPatientADO>();
                dataTotal = 0;
            }
        }

        /// <summary>Gom y lệnh theo TREATMENT_CODE → dòng bệnh nhân, tính WARNING cao nhất + pre-compute hiển thị.</summary>
        private void BuildPatientList()
        {
            try
            {
                listPatient = new List<CoordinationPatientADO>();
                if (listAllServiceReq == null || listAllServiceReq.Count == 0)
                {
                    dataTotal = 0;
                    return;
                }

                var groups = listAllServiceReq
                    .GroupBy(o => o.TREATMENT_CODE ?? "");

                foreach (var g in groups)
                {
                    var first = g.First();
                    CoordinationPatientADO ado = new CoordinationPatientADO();
                    ado.TREATMENT_CODE = first.TREATMENT_CODE;
                    ado.BED_NAME = first.BED_NAME;
                    ado.PATIENT_NAME = first.PATIENT_NAME;
                    ado.PATIENT_DOB = first.PATIENT_DOB;
                    ado.PATIENT_ADDRESS = first.PATIENT_ADDRESS;
                    ado.PATIENT_GENDER_NAME = first.PATIENT_GENDER_NAME;
                    ado.PATIENT_TYPE_NAME = first.PATIENT_TYPE_NAME;
                    ado.ServiceReqs = g.ToList();

                    // WARNING đại diện = mức cao nhất trong các y lệnh
                    ado.WARNING = g.Where(o => o.WARNING.HasValue).Select(o => o.WARNING).DefaultIfEmpty(null).Max();
                    ado.WarningDisplay = GetWarningDisplay(ado.WARNING);

                    // Trạng thái đại diện = mức thấp nhất (ít hoàn thành nhất) trong các y lệnh
                    ado.SERVICE_REQ_STT_ID = g.Where(o => o.SERVICE_REQ_STT_ID.HasValue).Select(o => o.SERVICE_REQ_STT_ID).DefaultIfEmpty(null).Min();
                    ado.StatusDisplay = GetStatusDisplay(ado.SERVICE_REQ_STT_ID);

                    // Nếu có ít nhất 1 y lệnh có SOLUTION_DES → "Đã xử lý", ngược lại "Chưa xử lý"
                    var solved = g.FirstOrDefault(o => !string.IsNullOrWhiteSpace(o.SOLUTION_DES));
                    ado.SOLUTION_DES = solved != null ? solved.SOLUTION_DES : null;
                    ado.SolutionDesDisplay = (solved != null)
                        ? Resources.ResourceMessage.DaXuLy
                        : Resources.ResourceMessage.ChuaXuLy;

                    ado.PatientDobStr = (ado.PATIENT_DOB.HasValue && ado.PATIENT_DOB.Value > 0)
                        ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(ado.PATIENT_DOB.Value)
                        : "";

                    listPatient.Add(ado);
                }

                dataTotal = listPatient.Count;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                listPatient = new List<CoordinationPatientADO>();
                dataTotal = 0;
            }
        }

        /// <summary>Map mức cảnh báo (WARNING) sang chữ hiển thị: 1 Bình thường / 2 Bất thường / 3 Vượt ngưỡng.</summary>
        private string GetWarningDisplay(long? warning)
        {
            try
            {
                if (!warning.HasValue) return "";
                if (warning.Value == (long)EnumCoordinationWarning.OverThreshold)
                    return Resources.ResourceMessage.CanhBaoVuotNguong;
                if (warning.Value == (long)EnumCoordinationWarning.Abnormal)
                    return Resources.ResourceMessage.CanhBaoBatThuong;
                if (warning.Value == (long)EnumCoordinationWarning.Normal)
                    return Resources.ResourceMessage.CanhBaoBinhThuong;
                return "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }

        /// <summary>Map trạng thái tổng hợp CLS (SERVICE_REQ_STT_ID) sang chữ hiển thị.</summary>
        private string GetStatusDisplay(long? sttId)
        {
            try
            {
                if (!sttId.HasValue) return "";
                if (sttId.Value == (long)EnumCoordinationServiceReqStt.Completed)
                    return Resources.ResourceMessage.TrangThaiDuKetQua;
                if (sttId.Value == (long)EnumCoordinationServiceReqStt.Executing)
                    return Resources.ResourceMessage.TrangThaiDangThucHien;
                if (sttId.Value == (long)EnumCoordinationServiceReqStt.NotExecuted)
                    return Resources.ResourceMessage.TrangThaiChuaThucHien;
                return "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }

        /// <summary>Callback phân trang lưới trái — cắt trang client-side từ listPatient.</summary>
        private void FillPatientGridTransaction(object param)
        {
            try
            {
                start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                if (limit <= 0) limit = (int)ConfigApplications.NumPageSize;

                List<CoordinationPatientADO> page = (listPatient == null)
                    ? new List<CoordinationPatientADO>()
                    : listPatient.Skip(start).Take(limit).ToList();

                gridControlPatient.BeginUpdate();
                try
                {
                    gridControlPatient.DataSource = page;
                }
                finally
                {
                    gridControlPatient.EndUpdate();
                }

                // Xóa lưới chi tiết khi (re)bind — người dùng tự chọn dòng để xem chi tiết
                gridControlServiceReq.BeginUpdate();
                try
                {
                    gridControlServiceReq.DataSource = null;
                }
                finally
                {
                    gridControlServiceReq.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Đổ chi tiết y lệnh của bệnh nhân được chọn sang lưới phải (mục 5.4).</summary>
        private void FillServiceReqGrid(CoordinationPatientADO patient)
        {
            try
            {
                // Người xem + hướng giải quyết hiển thị nguyên theo dữ liệu API trả về (VIEW_LOGINNAME, SOLUTION_DES).
                // Gán list MỚI mỗi lần (shallow copy) để DevExpress luôn refresh — tránh bỏ qua khi trùng tham chiếu.
                List<HisServiceReqGetServiceReqCLSSDO> data = (patient != null && patient.ServiceReqs != null)
                    ? new List<HisServiceReqGetServiceReqCLSSDO>(patient.ServiceReqs)
                    : new List<HisServiceReqGetServiceReqCLSSDO>();

                gridControlServiceReq.BeginUpdate();
                try
                {
                    gridControlServiceReq.DataSource = data;
                }
                finally
                {
                    gridControlServiceReq.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Grid events
        private void gridViewPatient_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "STT" && e.IsGetData)
                {
                    int currentPage = ucPaging1.pagingGrid == null ? 1 : ucPaging1.pagingGrid.CurrentPage;
                    int pageSize = ucPaging1.pagingGrid == null ? 0 : ucPaging1.pagingGrid.PageSize;
                    e.Value = e.ListSourceRowIndex + 1 + ((currentPage - 1) * pageSize);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Tô màu dòng theo WARNING (mục 5.5): 2 vàng, 3 đỏ, còn lại trắng.</summary>
        private void gridViewPatient_RowStyle(object sender, RowStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0) return;
                CoordinationPatientADO data = gridViewPatient.GetRow(e.RowHandle) as CoordinationPatientADO;
                if (data == null || !data.WARNING.HasValue) return;

                if (data.WARNING.Value == (long)EnumCoordinationWarning.OverThreshold)
                {
                    e.Appearance.BackColor = Color.FromArgb(255, 200, 200);
                    e.Appearance.BackColor2 = Color.FromArgb(255, 200, 200);
                }
                else if (data.WARNING.Value == (long)EnumCoordinationWarning.Abnormal)
                {
                    e.Appearance.BackColor = Color.FromArgb(255, 250, 190);
                    e.Appearance.BackColor2 = Color.FromArgb(255, 250, 190);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPatient_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            try
            {
                // Bỏ qua khi đang load/tìm (grid tự focus row 0) — chỉ nạp chi tiết khi người dùng tự chọn dòng
                if (isLoadingData) return;

                CoordinationPatientADO data = gridViewPatient.GetRow(e.FocusedRowHandle) as CoordinationPatientADO;
                FillServiceReqGrid(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Click vào dòng — nạp chi tiết kể cả khi click lại đúng dòng đang chọn (FocusedRowChanged không fire).</summary>
        private void gridViewPatient_RowClick(object sender, RowClickEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0) return;
                CoordinationPatientADO data = gridViewPatient.GetRow(e.RowHandle) as CoordinationPatientADO;
                FillServiceReqGrid(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Khi nhập "Hướng giải quyết" cho 1 y lệnh → tự điền "Người xem" = tài khoản đang đăng nhập
        /// (thể hiện đang xử lý) ngay trên lưới, trước khi bấm Lưu / gọi API.
        /// </summary>
        private void gridViewServiceReq_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column == colSrSolution && e.RowHandle >= 0)
                {
                    HisServiceReqGetServiceReqCLSSDO data = gridViewServiceReq.GetRow(e.RowHandle) as HisServiceReqGetServiceReqCLSSDO;
                    if (data != null && string.IsNullOrWhiteSpace(data.VIEW_LOGINNAME))
                    {
                        data.VIEW_LOGINNAME = this.loginName;
                        gridViewServiceReq.SetRowCellValue(e.RowHandle, colSrViewLoginName, this.loginName);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceReq_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (!e.IsGetData) return;
                HisServiceReqGetServiceReqCLSSDO data = e.Row as HisServiceReqGetServiceReqCLSSDO;
                if (data == null) return;

                if (e.Column.FieldName == "STT")
                {
                    e.Value = e.ListSourceRowIndex + 1;
                }
                else if (e.Column.FieldName == "INTRUCTION_DATE_STR")
                {
                    e.Value = (data.INTRUCTION_TIME.HasValue && data.INTRUCTION_TIME.Value > 0)
                        ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.INTRUCTION_TIME.Value)
                        : "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Button events
        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Rời ô Mã điều trị → tự pad '0' đủ 12 số và hiển thị đầy đủ.</summary>
        private void txtTreatmentCode_Leave(object sender, EventArgs e)
        {
            NormalizeTreatmentCodeDisplay();
        }

        /// <summary>Nhấn Enter trong ô lọc (Mã điều trị / Họ tên) → tìm kiếm luôn, không cần chuột.</summary>
        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Phím tắt Ctrl+F (gọi từ KeyboardWorker) — tìm kiếm.</summary>
        public void FindShortcut()
        {
            try
            {
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Làm mới: đưa điều kiện về mặc định (ngày hiện tại) và tải lại danh sách.</summary>
        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                this.txtTreatmentCode.Text = "";
                this.txtPatientName.Text = "";
                this.dtIntructionDateFrom.DateTime = DateTime.Now.Date;
                this.dtIntructionDateTo.DateTime = DateTime.Now.Date;
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
    }
}
