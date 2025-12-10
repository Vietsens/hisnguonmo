using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.Data;
using System.Resources;
using System.Collections;

using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using Inventec.Common.Adapter;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using HIS.Desktop.Plugins.Library.ConnectWhoCnd;
using HIS.Desktop.Utilities.Extensions;

namespace HIS.Desktop.Plugins.NcdWhoInformationList
{
    public partial class UCNcdWhoInformationList : UserControlBase
    {
        #region Khai báo biến
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int pageSize;
        Inventec.Desktop.Common.Modules.Module currentModule { get; set; }
        List<HIS_DEPARTMENT> listDepartment = null;
        #endregion

        #region Constructor & Load
        public UCNcdWhoInformationList(Inventec.Desktop.Common.Modules.Module module)
        {
            InitializeComponent();
            this.currentModule = module;
        }

        private void UCNcdWhoInformationList_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                SetDefaultControl();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Xử lý dữ liệu & Grid
        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.NcdWhoInformationList.Resources.Lang", typeof(UCNcdWhoInformationList).Assembly);
            }
            catch { }
        }

        private void FillDataToGrid()
        {
            try
            {
                WaitingManager.Show();
                if (ucPaging.pagingGrid != null) pageSize = ucPaging.pagingGrid.PageSize;
                else pageSize = (int)ConfigApplications.NumPageSize;

                btnDongBo.Enabled = false;

                LoadGridData(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadGridData, param, pageSize, gridControl1);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadGridData(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);

                ApiResultObject<List<HIS_TREATMENT>> apiResult = null;
                HisTreatmentFilter filter = new HisTreatmentFilter();
                SetFilter(ref filter);

                gridView1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<HIS_TREATMENT>>(
                    "api/HisTreatment/Get",
                    ApiConsumers.MosConsumer,
                    filter,
                    paramCommon);

                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    gridControl1.DataSource = (data != null && data.Count > 0) ? data : null;
                    rowCount = (data == null ? 0 : data.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                }
                else
                {
                    rowCount = 0;
                    dataTotal = 0;
                    gridControl1.DataSource = null;
                }
                gridView1.EndUpdate();
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void SetFilter(ref HisTreatmentFilter filter)
        {
            try
            {
                // 1. MÃ ĐIỀU TRỊ: Tự động điền đủ 12 ký tự
                if (!string.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    string treatmentCode = txtTreatmentCode.Text.Trim();
                    // Nếu độ dài < 12 thì thêm số 0 vào đầu
                    if (treatmentCode.Length < 12)
                    {
                        treatmentCode = treatmentCode.PadLeft(12, '0');
                        txtTreatmentCode.Text = treatmentCode; // Cập nhật lại lên ô nhập liệu cho người dùng thấy
                    }
                    filter.TREATMENT_CODE__EXACT = treatmentCode;
                }

                // 2. MÃ BỆNH NHÂN: Tự động điền đủ 10 ký tự
                if (!string.IsNullOrEmpty(txtPatientCode.Text))
                {
                    string patientCode = txtPatientCode.Text.Trim();
                    // Nếu độ dài < 10 thì thêm số 0 vào đầu
                    if (patientCode.Length < 10)
                    {
                        patientCode = patientCode.PadLeft(10, '0');
                        txtPatientCode.Text = patientCode; // Cập nhật lại lên ô nhập liệu
                    }
                    filter.TDL_PATIENT_CODE__EXACT = patientCode;
                }

                // 3. Tên bệnh nhân
                if (!string.IsNullOrEmpty(txtPatientName.Text))
                    filter.PATIENT_NAME = txtPatientName.Text.Trim();

                // 4. Ngày ra viện
                if (dtBornTimeFrom.EditValue != null && dtBornTimeFrom.DateTime != DateTime.MinValue)
                    filter.OUT_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtBornTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");

                if (dtBornTimeTo.EditValue != null && dtBornTimeTo.DateTime != DateTime.MinValue)
                    filter.OUT_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtBornTimeTo.EditValue).ToString("yyyyMMdd") + "235959");

                // 5. Trạng thái NCD
                if (cboSYNC_RESULT_TYPE.EditValue != null)
                {
                    switch (cboSYNC_RESULT_TYPE.SelectedIndex)
                    {
                        case 1: // Chưa đồng bộ
                            filter.HAS_NCD_WHO_RESULT = false;
                            break;
                        case 2: // Đã đồng bộ
                            filter.HAS_NCD_WHO_RESULT = true;
                            break;
                        case 3: // Thất bại
                            filter.NCD_WHO_RESULT = 2;
                            break;
                        default: // Tất cả
                            break;
                    }
                }

                // Sắp xếp
                filter.ORDER_FIELD = "OUT_TIME";
                filter.ORDER_DIRECTION = "DESC";
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void SetDefaultControl()
        {
            try
            {
                dtBornTimeFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.StartMonth() ?? 0));
                dtBornTimeTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.EndDay() ?? 0));
                listDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>();
                txtTreatmentCode.Text = "";
                txtPatientCode.Text = "";
                txtPatientName.Text = "";
                cboSYNC_RESULT_TYPE.SelectedIndex = 0;
                btnDongBo.Enabled = false;
                txtTreatmentCode.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Sự kiện (Events)
        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) { btnSearch_Click(null, null); }
        private void bbtnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) { btnRefresh_Click(null, null); }

        private void btnSearch_Click(object sender, EventArgs e) { FillDataToGrid(); }
        private void btnRefresh_Click(object sender, EventArgs e) { SetDefaultControl(); FillDataToGrid(); }

        private void Control_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if(e.KeyCode == Keys.Enter) btnSearch_Click(null, null); 
        }    
        
        private void txtPatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) { Control_PreviewKeyDown(sender, e); }
        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) { Control_PreviewKeyDown(sender, e); }
        private void btnSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) { Control_PreviewKeyDown(sender, e); }
        private void btnRefresh_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) { Control_PreviewKeyDown(sender, e); }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (HIS_TREATMENT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        // 1. Số thứ tự
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        // 2. Icon Trạng thái (Quan trọng: Logic dùng NCD_WHO_RESULT)
                        else if (e.Column.FieldName == "ICON_STR")
                        {
                            if (data.NCD_WHO_RESULT != null) e.Value = 1; // 1: Kích hoạt hiện ảnh
                            else e.Value = null; // Null: Ẩn ảnh
                        }
                        // 3. Lý do
                        else if (e.Column.FieldName == "REASON_STR")
                        {
                            if (data.NCD_WHO_RESULT == 2) e.Value = "Đồng bộ thất bại";
                            else if (data.NCD_WHO_RESULT != null) e.Value = "Đã đồng bộ";
                            else e.Value = "";
                        }
                        // 4. [SỬA] Thời gian đồng bộ
                        else if (e.Column.FieldName == "SYNC_TIME_STR")
                        {
                            // Logic: Nếu đã có kết quả (khác null) thì hiển thị thời gian sửa đổi
                            if (data.NCD_WHO_RESULT != null)
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                            }
                            else
                            {
                                e.Value = "";
                            }
                        }
                        // --- CÁC CỘT THÔNG TIN KHÁC (Khắc phục lỗi ô trắng) ---
                        else if (e.Column.FieldName == "IN_TIME_STR") // Ngày vào viện
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IN_TIME);
                        }
                        else if (e.Column.FieldName == "OUT_TIME_STR") // Ngày ra viện
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.OUT_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY") // Thời gian tạo
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY") // Thời gian sửa
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "CREATOR") e.Value = data.CREATOR;
                        else if (e.Column.FieldName == "MODIFIER") e.Value = data.MODIFIER;
                        else if (e.Column.FieldName == "TDL_PATIENT_DOB_STR") e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                        else if (e.Column.FieldName == "TDL_SOCIAL_INSURANCE_NUMBER_STR") e.Value = !string.IsNullOrEmpty(data.TDL_SOCIAL_INSURANCE_NUMBER) ? data.TDL_SOCIAL_INSURANCE_NUMBER : data.TDL_HEIN_CARD_NUMBER;
                        else if (e.Column.FieldName == "DEPARTMENT_NAME")
                        {
                            if (listDepartment != null && data.LAST_DEPARTMENT_ID.HasValue)
                            {
                                var dept = listDepartment.FirstOrDefault(o => o.ID == data.LAST_DEPARTMENT_ID.Value);
                                if (dept != null) e.Value = dept.DEPARTMENT_NAME;
                            }
                        }
                        else if (e.Column.FieldName == "CUSTOM_ADDRESS_STR") e.Value = data.TDL_PATIENT_ADDRESS;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        // --- HÀM 2: HIỂN THỊ ẢNH (Đã sửa FieldName chuẩn) ---
        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                // [ĐÃ SỬA CHUẨN]: Dùng ICON_STR
                if (e.Column.FieldName == "ICON_STR" && e.RowHandle >= 0)
                {
                    var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    var data = (HIS_TREATMENT)view.GetRow(e.RowHandle);

                    if (data != null)
                    {
                        if (data.NCD_WHO_RESULT == 2)
                        {
                            e.RepositoryItem = this.rep_tickdo; // Thất bại
                        }
                        else if (data.NCD_WHO_RESULT != null)
                        {
                            e.RepositoryItem = this.red_tickxanh; // Thành công
                        }
                        else
                        {
                            e.RepositoryItem = null; // Chưa làm
                        }
                    }
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void gridView1_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                btnDongBo.Enabled = (gridView1.GetSelectedRows().Count() > 0);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }
        private void btnDongBo_Click(object sender, EventArgs e)
        {
            try
            {
                var rowHandles = gridView1.GetSelectedRows();
                if (rowHandles == null || rowHandles.Count() == 0)
                {
                    XtraMessageBox.Show("Vui lòng chọn ít nhất một hồ sơ để đồng bộ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                WaitingManager.Show();
                int successCount = 0;
                int failCount = 0;
                List<string> listErrors = new List<string>();
                CommonParam param = new CommonParam();

                foreach (var i in rowHandles)
                {
                    var row = (HIS_TREATMENT)gridView1.GetRow(i);
                    if (row != null)
                    {
                        try
                        {
                            // [SỬA QUAN TRỌNG]: Dùng trực tiếp dữ liệu từ lưới (row) thay vì gọi API lại.
                            // Vì gọi API Get/ID đang bị lỗi trả về null.
                            var treatment = row;

                            if (treatment != null)
                            {
                                var dhst = GetDhst(treatment.ID, param);
                                var medicines = GetMedicines(treatment.ID, param);
                                var processor = new ConnectWhoCndProcessor(treatment, dhst, medicines);

                                if (processor.CheckData())
                                {
                                    processor.SendData();
                                    successCount++;
                                }
                                else
                                {
                                    failCount++;
                                 listErrors.Add($"Mã {row.TREATMENT_CODE}: Thiếu thông tin hoặc sai ICD.");
                                }
                            }
                        }
                        catch (Exception exRow)
                        {
                            failCount++;
                            listErrors.Add($"Mã {row.TREATMENT_CODE}: Lỗi hệ thống.");
                            Inventec.Common.Logging.LogSystem.Error("Lỗi đồng bộ HS: " + row.TREATMENT_CODE, exRow);
                        }
                    }
                }
                WaitingManager.Hide();

                string msg = string.Format("Kết quả xử lý:\n- Thành công: {0}\n- Thất bại: {1}", successCount, failCount);
                if (listErrors.Count > 0) msg += "\n\nChi tiết: \n" + string.Join("\n", listErrors);

                XtraMessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                XtraMessageBox.Show("Có lỗi xảy ra trong quá trình đồng bộ.", "Lỗi");
            }
        }

        private HIS_DHST GetDhst(long treatmentId, CommonParam param)
        {
            try
            {
                HisDhstFilter filter = new HisDhstFilter();
                filter.TREATMENT_ID = treatmentId;
                var list = new BackendAdapter(param).Get<List<HIS_DHST>>("api/HisDHST/Get", ApiConsumers.MosConsumer, filter, param);
                if (list != null && list.Count > 0)
                    return list.OrderByDescending(o => o.EXECUTE_TIME ?? 0).FirstOrDefault();
            }
            catch { }
            return null;
        }

        private List<V_HIS_EXP_MEST_MEDICINE> GetMedicines(long treatmentId, CommonParam param)
        {
            try
            {
                HisExpMestMedicineViewFilter filter = new HisExpMestMedicineViewFilter();
                filter.TDL_TREATMENT_ID = treatmentId;
                return new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, filter, param);
            }
            catch { }
            return new List<V_HIS_EXP_MEST_MEDICINE>();
        }     
        #endregion
    }
}