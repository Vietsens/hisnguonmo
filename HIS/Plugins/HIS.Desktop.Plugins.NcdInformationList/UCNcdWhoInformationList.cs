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
using System.Threading;

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

        List<HIS_PATIENT> listPatients = null;
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

                gridView1.DoubleClick += gridView1_DoubleClick;

                gridView1.OptionsBehavior.Editable = true;
                gridView1.OptionsBehavior.ReadOnly = true;

                var columnsToCopy = new List<string> { "TREATMENT_CODE", "TDL_PATIENT_CODE","CCCD_HIENTHI_AO","TDL_PATIENT_CCCD_NUMBER","TDL_HEIN_CARD_NUMBER","TDL_SOCIAL_INSURANCE_NUMBER","TDL_SOCIAL_INSURANCE_NUMBER_STR"};

                foreach (DevExpress.XtraGrid.Columns.GridColumn col in gridView1.Columns)
                {
                    if (columnsToCopy.Contains(col.FieldName))
                    {
                        col.OptionsColumn.AllowEdit = true;
                        col.OptionsColumn.ReadOnly = true; // Chỉ đọc
                    }
                    else
                    {
                        col.OptionsColumn.AllowEdit = false;
                        col.OptionsColumn.ReadOnly = true;
                    }
                }

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
                    "api/HisTreatment/GetView",
                    ApiConsumers.MosConsumer,
                    filter,
                    paramCommon);

                if (apiResult != null)
                {
                    var data = apiResult.Data;

                    if (data != null && data.Count > 0)
                    {
                        if (cboSYNC_RESULT_TYPE.SelectedIndex == 1) // Chọn "Chưa đồng bộ"
                        {
                            data = data.Where(o => o.NCD_WHO_RESULT == null || o.NCD_WHO_RESULT == 0).ToList();
                        }
                        else if (cboSYNC_RESULT_TYPE.SelectedIndex == 2) // Chọn "Đã đồng bộ"
                        {
                            data = data.Where(o => o.NCD_WHO_RESULT == 1).ToList();
                        }
                        else if (cboSYNC_RESULT_TYPE.SelectedIndex == 3) // Chọn "Thất bại"
                        {
                            data = data.Where(o => o.NCD_WHO_RESULT == 2).ToList();
                        }
                    }
                    if (data != null && data.Count > 0)
                    {
                        try
                        {
                            var listPatientIds = data.Select(o => o.PATIENT_ID).Distinct().ToList();
                            if (listPatientIds.Count > 0)
                            {
                                HisPatientFilter patientFilter = new HisPatientFilter();
                                patientFilter.IDs = listPatientIds;
                                listPatients = new BackendAdapter(paramCommon).Get<List<HIS_PATIENT>>(
                                    "api/HisPatient/Get",
                                    ApiConsumers.MosConsumer,
                                    patientFilter,
                                    paramCommon);
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error("Lỗi tải thông tin bệnh nhân kèm theo: ", ex);
                        }
                    }
                    else
                    {
                        listPatients = null;
                    }

                    gridControl1.DataSource = (data != null && data.Count > 0) ? data : null;
                    rowCount = (data == null ? 0 : data.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                }
                else
                {
                    rowCount = 0;
                    dataTotal = 0;
                    gridControl1.DataSource = null;
                    listPatients = null;
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
                // 1. MÃ ĐIỀU TRỊ
                if (!string.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    string treatmentCode = txtTreatmentCode.Text.Trim();
                    if (treatmentCode.Length < 12)
                    {
                        treatmentCode = treatmentCode.PadLeft(12, '0');
                        txtTreatmentCode.Text = treatmentCode;
                    }
                    filter.TREATMENT_CODE__EXACT = treatmentCode;
                }

                // 2. MÃ BỆNH NHÂN
                if (!string.IsNullOrEmpty(txtPatientCode.Text))
                {
                    string patientCode = txtPatientCode.Text.Trim();
                    if (patientCode.Length < 10)
                    {
                        patientCode = patientCode.PadLeft(10, '0');
                        txtPatientCode.Text = patientCode;
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

                // 5. Trạng thái NCD (SỬA LẠI LOGIC CHUẨN)
                if (cboSYNC_RESULT_TYPE.EditValue != null)
                {
                    switch (cboSYNC_RESULT_TYPE.SelectedIndex)
                    {
                        case 1: // Chưa đồng bộ
                            filter.HAS_NCD_WHO_RESULT = false;
                            break;
                        case 2: // Đã đồng bộ
                            filter.NCD_WHO_RESULT = 1;
                            break;
                        case 3: // Thất bại
                            filter.NCD_WHO_RESULT = 2;
                            break;
                        default: // Tất cả
                            break;
                    }
                }

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
            if (e.KeyCode == Keys.Enter) btnSearch_Click(null, null);
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
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "ICON_STR")
                        {
                            if (data.NCD_WHO_RESULT != null) e.Value = 1;
                            else e.Value = null;
                        }
                        else if (e.Column.FieldName == "REASON_STR")
                        {
                            if (data.NCD_WHO_RESULT == 2)
                            {
                                e.Value = !string.IsNullOrEmpty(data.NCD_WHO_DESCRIPTION)
                                          ? data.NCD_WHO_DESCRIPTION
                                          : "Đồng bộ thất bại (Không rõ lý do)";
                            }
                            else if (data.NCD_WHO_RESULT == 1)
                            {
                                e.Value = "Đã đồng bộ";
                            }
                            else
                            {
                                e.Value = "";
                            }
                        }
                        else if (e.Column.FieldName == "SYNC_TIME_STR")
                        {
                            if (data.NCD_WHO_RESULT != null)
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                            else
                                e.Value = "";
                        }
                        else if (e.Column.FieldName == "IN_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IN_TIME);
                        }
                        else if (e.Column.FieldName == "OUT_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.OUT_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "CREATOR") e.Value = data.CREATOR;
                        else if (e.Column.FieldName == "MODIFIER") e.Value = data.MODIFIER;
                        else if (e.Column.FieldName == "TDL_PATIENT_DOB_STR")
                        {
                            if (data.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                            {
                                if (data.TDL_PATIENT_DOB > 0) e.Value = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                            }
                            else
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                            }
                        }
                        else if (e.Column.FieldName == "CCCD_HIENTHI_AO")
                        {
                            string cccd = data.TDL_PATIENT_CCCD_NUMBER;
                            string cmnd = data.TDL_PATIENT_CMND_NUMBER;
                            string finalVal = !string.IsNullOrEmpty(cccd) ? cccd : cmnd;

                            if (string.IsNullOrEmpty(finalVal) && listPatients != null)
                            {
                                var patient = listPatients.FirstOrDefault(o => o.ID == data.PATIENT_ID);
                                if (patient != null)
                                {
                                    finalVal = !string.IsNullOrEmpty(patient.CCCD_NUMBER)
                                               ? patient.CCCD_NUMBER
                                               : patient.CMND_NUMBER;
                                }
                            }
                            e.Value = finalVal;

                        }
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

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "ICON_STR" && e.RowHandle >= 0)
                {
                    var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    var data = (HIS_TREATMENT)view.GetRow(e.RowHandle);

                    if (data != null)
                    {
                        if (data.NCD_WHO_RESULT == 2) e.RepositoryItem = this.rep_tickdo;
                        else if (data.NCD_WHO_RESULT != null) e.RepositoryItem = this.red_tickxanh;
                        else e.RepositoryItem = null;
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

                int totalSelected = rowHandles.Length; // Tổng số đã chọn
                int successCount = 0;
                int failCount = 0; // Đếm số lượng hồ sơ NCD nhưng bị lỗi (thiếu data hoặc lỗi mạng)

                List<string> listIgnoredCodes = new List<string>();
                CommonParam param = new CommonParam();

                foreach (var i in rowHandles)
                {
                    var row = (HIS_TREATMENT)gridView1.GetRow(i);
                    if (row != null)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(row.TDL_PATIENT_CCCD_NUMBER) && listPatients != null)
                            {
                                var p = listPatients.FirstOrDefault(o => o.ID == row.PATIENT_ID);
                                if (p != null) row.TDL_PATIENT_CCCD_NUMBER = p.CCCD_NUMBER ?? p.CMND_NUMBER;
                            }
                            var dhst = GetDhst(row.ID, param);
                            var medicines = GetMedicines(row.ID, param);
                            var processor = new ConnectWhoCndProcessor(row, dhst, medicines);

                            processor.CheckData();

                            if (processor.isNotInIcd)
                            {
                                listIgnoredCodes.Add(row.TREATMENT_CODE);
                                continue;
                            }

                            if (!processor.HasData)
                            {
                                failCount++; // Tính vào thất bại
                                continue;
                            }

                            Inventec.Common.Logging.LogSystem.Info("Bắt đầu gửi hồ sơ: " + row.TREATMENT_CODE);
                            processor.SendDataForCND();

                            if (processor.HasData)
                            {
                                successCount++; 
                            }
                            else
                            {
                                failCount++;
                            }
                        }
                        catch (Exception exRow)
                        {
                            failCount++; 
                            Inventec.Common.Logging.LogSystem.Error("Lỗi đồng bộ HS: " + row.TREATMENT_CODE, exRow);
                        }
                    }
                }
                WaitingManager.Hide();

                string msg = string.Format("Kết quả xử lý {0} hồ sơ:\n- Thành công: {1}\n- Thất bại: {2}\n- Bỏ qua (Không có bệnh huyết áp hoặc đái tháo đường): {3}", totalSelected,successCount,failCount,listIgnoredCodes.Count);

                if (listIgnoredCodes.Count > 0)
                {
                    string strCodes = string.Join(", ", listIgnoredCodes);
                    msg += string.Format("\n\nCác hồ sơ {0} không có mã bệnh huyết áp hoặc đái tháo đường.", strCodes);
                }

                XtraMessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                System.Threading.Thread.Sleep(1000);
                FillDataToGrid();
                gridView1.RefreshData();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                XtraMessageBox.Show("Có lỗi xảy ra trong quá trình đồng bộ.", "Lỗi hệ thống");
            }
        }
        //private bool IsPotentialNcdWho(string icdMain, string icdSub)
        //{
        //    var keys = new List<string> { "E10", "E11", "E12", "E13", "E14", "I10", "I11", "I12", "I13", "I15" };
        //    foreach (var k in keys) if (icdMain.StartsWith(k)) return true;
        //    if (!string.IsNullOrEmpty(icdSub))
        //    {
        //        foreach (var k in keys) if (icdSub.Contains(k)) return true;
        //    }

        //    return false;
        //}

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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F))
            {
                btnSearch_Click(null, null);
                return true;
            }
            if (keyData == (Keys.Control | Keys.R))
            {
                btnRefresh_Click(null, null);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                var ea = e as DevExpress.Utils.DXMouseEventArgs;
                var info = view.CalcHitInfo(ea.Location);

                if (info.InRow || info.InRowCell)
                {
                    view.ShowEditor();
                    if (view.ActiveEditor is DevExpress.XtraEditors.TextEdit editor)
                    {
                        editor.SelectAll();
                    }
                }
            }
            catch (Exception ex) { }
        }
        #endregion
    }

    // Class SDO dùng cho việc update thủ công
    public class NcdWhoStatusSDO
    {
        public long TreatmentId { get; set; }
        public short NcdWhoResult { get; set; }
        public string NcdWhoDescription { get; set; }
    }
}