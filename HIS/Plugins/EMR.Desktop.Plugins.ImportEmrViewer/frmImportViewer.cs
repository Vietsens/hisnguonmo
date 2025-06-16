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
using ACS.EFMODEL.DataModels;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using EMR.Desktop.Plugins.ImportEmrViewer.ADO;
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using EMR.SDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Common.DateTime;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EMR.Desktop.Plugins.ImportEmrViewer
{
    public partial class frmImportViewer : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module _Module { get; set; }
        RefeshReference delegateRefresh;
        List<ADO.EmrViewerAdo> _ImportAdos;
        List<ADO.EmrViewerAdo> _CurrentAdos;
        List<EMR_VIEWER> _ListViewer { get; set; }
        List<EMR_TREATMENT> _ListTreatment { get; set; }

        public frmImportViewer()
        {
            InitializeComponent();
        }

        public frmImportViewer(Inventec.Desktop.Common.Modules.Module _module)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                this._Module = _module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmImportViewer(Inventec.Desktop.Common.Modules.Module _module, RefeshReference _delegateRefresh)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                this._Module = _module;
                this.delegateRefresh = _delegateRefresh;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmImportViewer_Load(object sender, EventArgs e)
        {
            try
            {
                if (this._Module != null)
                {
                    this.Text = this._Module.text;
                    btnShowLineError.Enabled = false;
                    btnImport.Enabled = false;
                }
                SetIcon();
                LoadCurrentData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCurrentData()
        {
            try
            {


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDownLoadFile_Click(object sender, EventArgs e)
        {
            try
            {

                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_EMR_VIEWER.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog.Title = "Save File";
                    saveFileDialog.FileName = "IMPORT_EMR_VIEWER";
                    saveFileDialog.DefaultExt = "xlsx";
                    saveFileDialog.Filter = "Excel files (*.xlsx)|All files (*.*)";
                    saveFileDialog.FilterIndex = 2;
                    saveFileDialog.RestoreDirectory = true;

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(fileName, saveFileDialog.FileName);
                        MessageManager.Show(this.ParentForm, param, true);
                        if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn mở file ngay?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(saveFileDialog.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnChooseFile_Click(object sender, EventArgs e)
        {
            try
            {
                btnShowLineError.Text = "Dòng lỗi";

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    WaitingManager.Show();

                    var import = new Inventec.Common.ExcelImport.Import();
                    if (import.ReadFileExcel(ofd.FileName))
                    {
                        var emrViewerImport = import.GetWithCheck<ADO.EmrViewerAdo>(0);
                        if (emrViewerImport != null && emrViewerImport.Count > 0)
                        {

                            WaitingManager.Hide();

                            this._CurrentAdos = emrViewerImport.Where(p => checkNull(p)).ToList();

                            if (this._CurrentAdos != null && this._CurrentAdos.Count > 0)
                            {
                                btnShowLineError.Enabled = true;
                                this._ImportAdos = new List<ADO.EmrViewerAdo>();

                                _ListViewer = new List<EMR_VIEWER>();
                                EMR.Filter.EmrViewerFilter filter = new Filter.EmrViewerFilter();
                                //filter.REQUEST_USERNAME = _CurrentAdos.Select(o => o.TREATMENT_CODE).ToList();
                                _ListViewer = new BackendAdapter(new CommonParam()).Get<List<EMR_VIEWER>>("api/EmrViewer/Get", ApiConsumers.EmrConsumer, filter, null);


                                EMR_TREATMENT treatment = new EMR_TREATMENT();
                                EmrTreatmentFilter emrfilter = new EmrTreatmentFilter();
                                emrfilter.TREATMENT_CODEs = _CurrentAdos.Select(o => o.TREATMENT_CODE).ToList();
                                _ListTreatment = new BackendAdapter(new CommonParam()).Get<List<EMR_TREATMENT>>("api/EmrTreatment/Get", ApiConsumers.EmrConsumer, emrfilter, null);
                                EMR.Filter.EmrDocumentViewFilter documentFilter = new EMR.Filter.EmrDocumentViewFilter();

                                //LIST DEPARTMENT
                                var _ListDepartment = new List<HIS_DEPARTMENT>();
                                HisDepartmentFilter departmentFilter = new HisDepartmentFilter();
                                _ListDepartment = new BackendAdapter(new CommonParam())
                                    .Get<List<HIS_DEPARTMENT>>("api/HisDepartment/Get", ApiConsumers.MosConsumer, departmentFilter, null);

                                // ACS_USER



                                HIS_EMPLOYEE employ = new HIS_EMPLOYEE();
                                HisEmployeeFilter employFilter = new HisEmployeeFilter();

                                addServiceToProcessList(_CurrentAdos, ref this._ImportAdos, _ListTreatment, _ListDepartment);
                                SetDataSource(this._ImportAdos);
                            }

                            //btnSave.Enabled = true;
                        }
                        else
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Import thất bại");
                        }
                    }
                    else
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không đọc được file");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool checkNull(ADO.EmrViewerAdo data)
        {
            bool result = true;
            try
            {
                if (data != null)
                {
                    if (string.IsNullOrEmpty(data.REQUEST_LOGINNAME)
                        && !data.FINISH_TIME.HasValue
                        && string.IsNullOrEmpty(data.TREATMENT_CODE)

                        )
                    {
                        result = false;
                    }
                }
                else
                    result = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void addServiceToProcessList(List<ADO.EmrViewerAdo> _service, ref List<ADO.EmrViewerAdo> _importAdoRef, List<EMR_TREATMENT> _ListTreatment, List<HIS_DEPARTMENT> _ListDepartment)
        {
            try
            {
                _importAdoRef = new List<ADO.EmrViewerAdo>();
                long i = 0;
                foreach (var item in _service)
                {
                    i++;
                    string error = "";
                    var serAdo = new ADO.EmrViewerAdo();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.EmrViewerAdo>(serAdo, item);


                 
                    if (!string.IsNullOrEmpty(item.REQUEST_LOGINNAME))
                    {
                        if (item.REQUEST_LOGINNAME.Length > 50)
                        {
                            error += string.Format(Message.ResourceLanguageManager.Maxlength, item.REQUEST_LOGINNAME);
                        }
                        var hisEmployeedatas = BackendDataWorker.Get<HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == item.REQUEST_LOGINNAME);



                        if (hisEmployeedatas == null)
                        {
                            error += string.Format(Message.ResourceLanguageManager.DaTonTai, item.REQUEST_LOGINNAME);
                        }
                        else
                        {
                            serAdo.REQUEST_USERNAME = hisEmployeedatas.TDL_USERNAME; // hoặc employee.REQUEST_USERNAME nếu đúng tên property
                        }
                    }
                    else
                    {
                        error += string.Format(Message.ResourceLanguageManager.ThieuTruongDL, "Tên đăng nhập");
                    }


                    if (!string.IsNullOrEmpty(item.DEPARTMENT_CODE))
                    {
                        if (item.DEPARTMENT_CODE.Length > 4)
                        {
                            error += string.Format(Message.ResourceLanguageManager.Maxlength, item.DEPARTMENT_CODE);
                        }



                        var department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.DEPARTMENT_CODE == item.DEPARTMENT_CODE);

                        if (department != null)
                        {
                            serAdo.DEPARTMENT_CODE = department.DEPARTMENT_CODE;
                            serAdo.DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.ResourceLanguageManager.KhongThayKhoa, item.DEPARTMENT_CODE);
                        }
                    }
                    else
                    {
                        error += string.Format(Message.ResourceLanguageManager.ThieuTruongDL, "Khoa yêu cầu");
                    }

                    if (!string.IsNullOrEmpty(item.REASON))
                    {
                        if (Inventec.Common.String.CountVi.Count(item.REASON) > 100)
                        {
                            error += string.Format(Message.ResourceLanguageManager.Maxlength, item.REASON);
                        }
                    }

                    if (item.FINISH_TIME.HasValue)
                    {
                        string finishTimeStr = item.FINISH_TIME.Value.ToString("yyyyMMddHHmmss");
                        finishTimeStr = finishTimeStr.Trim();
                        if (finishTimeStr.Length != 14)
                        {
                            error += string.Format("Thời gian được duyệt không hợp lệ (phải là số 14 ký tự: yyyyMMddHHmmss).");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.ResourceLanguageManager.ThieuTruongDL, "Thời gian được duyệt");
                    }


                    var treatment = _ListTreatment.FirstOrDefault(x => x.ID == item.TREATMENT_ID);
                    if (treatment != null)
                    {
                        item.PATIENT_CODE = treatment.PATIENT_CODE;
                        item.VIR_PATIENT_NAME = treatment.VIR_PATIENT_NAME;
                        item.HEIN_CARD_NUMBER = treatment.HEIN_CARD_NUMBER;
                        item.ICD_CODE = treatment.ICD_CODE;
                    }


                    if (!string.IsNullOrWhiteSpace(item.TREATMENT_CODE))
                    {
                        if (Inventec.Common.String.CountVi.Count(item.TREATMENT_CODE) > 12)
                        {
                            error += string.Format(Message.ResourceLanguageManager.Maxlength, item.TREATMENT_CODE);
                        }
                        // Kiểm tra tồn tại mã điều trị trong danh sách (giả sử bạn có _ListTreatment)
                        var treatmentZ = _ListTreatment.FirstOrDefault(x => x.TREATMENT_CODE == item.TREATMENT_CODE);
                        if (treatmentZ == null)
                        {
                            error += string.Format("Mã điều trị không tồn tại: {0}", item.TREATMENT_CODE);
                        }
                    }
                    else
                    {
                        error += string.Format(Message.ResourceLanguageManager.ThieuTruongDL, "Mã điều trị");
                    }
                    if (!string.IsNullOrWhiteSpace(item.PATIENT_CODE))
                    {
                        if (Inventec.Common.String.CountVi.Count(item.PATIENT_CODE) > 10)
                        {
                            error += string.Format(Message.ResourceLanguageManager.Maxlength, item.PATIENT_CODE);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(item.HEIN_CARD_NUMBER))
                    {
                        if (Inventec.Common.String.CountVi.Count(item.HEIN_CARD_NUMBER) > 15)
                        {
                            error += string.Format(Message.ResourceLanguageManager.Maxlength, item.HEIN_CARD_NUMBER);
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(item.ICD_CODE))
                    {
                        if (Inventec.Common.String.CountVi.Count(item.ICD_CODE) > 5)
                        {
                            error += string.Format(Message.ResourceLanguageManager.Maxlength, item.ICD_CODE);
                        }
                    }



                    serAdo.ERROR = error;
                    serAdo.ID = i;

                    _importAdoRef.Add(serAdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        private void SetDataSource(List<ADO.EmrViewerAdo> dataSource)
        {
            try
            {
                gridControlData.BeginUpdate();
                gridControlData.DataSource = null;
                gridControlData.DataSource = dataSource;
                gridControlData.EndUpdate();
                CheckErrorLine(null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckErrorLine(List<ADO.EmrViewerAdo> dataSource)
        {
            try
            {
                var checkError = this._ImportAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
                if (!checkError)
                {
                    btnImport.Enabled = true;
                    btnShowLineError.Enabled = false;
                }
                else
                {
                    btnShowLineError.Enabled = true;
                    btnImport.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnShowLineError_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnShowLineError.Text == "Dòng lỗi")
                {
                    btnShowLineError.Text = "Dòng không lỗi";
                    var errorLine = this._ImportAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = this._ImportAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit2_Delete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (ADO.EmrViewerAdo)gridViewData.GetFocusedRow();
                if (row != null)
                {
                    if (this._ImportAdos != null && this._ImportAdos.Count > 0)
                    {
                        this._ImportAdos.Remove(row);
                        var dataCheck = this._ImportAdos.Where(p => p.REQUEST_LOGINNAME == row.REQUEST_LOGINNAME).ToList();
                        if (dataCheck != null && dataCheck.Count == 1)
                        {
                            if (!string.IsNullOrEmpty(dataCheck[0].ERROR))
                            {
                                string erro = string.Format(Message.ResourceLanguageManager.FileImportDaTonTai, dataCheck[0].REQUEST_LOGINNAME);
                              
                                dataCheck[0].ERROR = dataCheck[0].ERROR.Replace(erro, "");
                            }

                        }
                        SetDataSource(this._ImportAdos);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit1_error_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (ADO.EmrViewerAdo)gridViewData.GetFocusedRow();
                if (row != null && !string.IsNullOrEmpty(row.ERROR))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(row.ERROR, "Thông báo");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewData_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ADO.EmrViewerAdo pData = (ADO.EmrViewerAdo)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnImport.Enabled) return;
                btnImport.Focus();
                var listData = (List<ADO.EmrViewerAdo>)gridControlData.DataSource;
                if (listData == null || listData.Count <= 0) return;
                if (listData.Exists(o => !String.IsNullOrWhiteSpace(o.ERROR))) return;

                bool success = false;
                WaitingManager.Show();

              
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                string userName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();

                List<EMR_VIEWER> listViewer = new List<EMR_VIEWER>();

                foreach (var item in listData)
                {
                   
                    var treatment = _ListTreatment.FirstOrDefault(x => x.TREATMENT_CODE == item.TREATMENT_CODE);
                    if (treatment != null)
                    {
                        item.TREATMENT_ID = treatment.ID;

                    }

                    EMR_VIEWER viewer = new EMR_VIEWER();

                    viewer.REQUEST_LOGINNAME = item.REQUEST_LOGINNAME;
                    viewer.REQUEST_USERNAME = item.REQUEST_USERNAME;
                    viewer.DEPARTMENT_CODE = item.DEPARTMENT_CODE;
                    viewer.DEPARTMENT_NAME = item.DEPARTMENT_NAME;

                    viewer.REASON = item.REASON;
                    viewer.FINISH_TIME = item.FINISH_TIME;



                    viewer.TREATMENT_ID = item.TREATMENT_ID;
                    viewer.REQUEST_FINISH_TIME = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    viewer.APPROVAL_TIME = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    viewer.APPROVAL_LOGINNAME = loginName;
                    viewer.APPROVAL_USERNAME = userName;


                    listViewer.Add(viewer);
                }

                CommonParam param = new CommonParam();
                if (listViewer != null && listViewer.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("listViewer input:", listViewer));

                    var rs = new BackendAdapter(param).Post<List<EMR_VIEWER>>(
                        EMR.URI.EmrViewer.CREATE_LIST,
                        ApiConsumers.EmrConsumer,
                        listViewer,

                        param);

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("rs input:", rs));

                    if (rs != null)
                    {
                        success = true;
                        btnImport.Enabled = false;
                    }
                }

                WaitingManager.Hide();

                #region Hiển thị message thông báo
                MessageManager.Show(this, param, success);
                #endregion

                #region Nếu phiên làm việc bị mất, phần mềm tự động logout và trở về trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnImport.Enabled)
                    btnImport_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewData_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    //BedRoomADO data = (BedRoomADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    string error = (gridViewData.GetRowCellValue(e.RowHandle, "ERROR") ?? "").ToString();
                    if (e.Column.FieldName == "ERROR_")
                    {
                        if (!string.IsNullOrEmpty(error))
                        {
                            e.RepositoryItem = repositoryItemButtonEdit1_error;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridControlData_Click(object sender, EventArgs e)
        {

        }
        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {

            if (keyData == (Keys.Control | Keys.S))
            {
                btnImport.PerformClick();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        // Bổ sung xử lý hiển thị giá trị các cột như patient_code, icd_code,... trên grid theo EMR_TREATMENT lấy theo treatment_code

        private void gridViewData_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            try
            {


                var gridView = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (gridView == null || _ListTreatment == null) return;

                int rowHandle = e.ListSourceRowIndex;
                if (rowHandle < 0) return;

                var row = gridView.GetRow(rowHandle) as ADO.EmrViewerAdo;
                if (row == null) return;

                // Lấy treatment theo treatment_code
                var treatment = _ListTreatment.FirstOrDefault(x => x.TREATMENT_CODE == row.TREATMENT_CODE);

                if (treatment == null) return;

                switch (e.Column.FieldName)
                {
                    case "PATIENT_CODE":
                        e.DisplayText = treatment.PATIENT_CODE;
                        break;
                    case "VIR_PATIENT_NAME":
                        e.DisplayText = treatment.VIR_PATIENT_NAME;
                        break;
                    case "HEIN_CARD_NUMBER":
                        e.DisplayText = treatment.HEIN_CARD_NUMBER;
                        break;
                    case "ICD_CODE":
                        e.DisplayText = treatment.ICD_CODE;
                        break;
                    case "REQUEST_USERNAME":
                        e.DisplayText = row.REQUEST_USERNAME;
                        break;
                    case "DEPARTMENT_NAME":
                        e.DisplayText = row.DEPARTMENT_NAME;
                        break;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // Đăng ký event này trong hàm khởi tạo hoặc frmImportViewer_Load
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            try
            {
                gridViewData.CustomColumnDisplayText += gridViewData_CustomColumnDisplayText;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
