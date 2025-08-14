using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors.Repository;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.MaterialTypeCreate.ADO;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;

namespace HIS.Desktop.Plugins.MaterialTypeCreate.MaterialTypeCreate
{
    public partial class frmDepartmentPatientType : Form
    {
        #region declare 
        int deptRowCount = 0;
        int deptDataTotal = 0;
        int deptStart = 0;

        int patientTypeRowCount = 0;
        int patientTypeDataTotal = 0;
        int patientTypeStart = 0;
        private string selectionMode = "DEPARTMENT";
        private bool isHeaderDeptChecked = false;
        private bool isHeaderPatientTypeChecked = false;
        public bool isCalledApi = false;
        public bool isClickPick = false; 
        private List<HIS_DEPA_PATIENT_TYPE> depaPatientTypes = new List<HIS_DEPA_PATIENT_TYPE>();
        private List<DepartmentADO> selectedDepartments = new List<DepartmentADO>();
        private List<PatientTypeADO> selectedPatientTypes = new List<PatientTypeADO>();
        private List<PatientTypeADO> unSelectedPatientTypes = new List<PatientTypeADO>();
        private List<DepartmentADO> unSelectedDepartments = new List<DepartmentADO>();
        private long? serviceId;


        public delegate void DepaPatientTypeSaved(List<HIS_DEPA_PATIENT_TYPE> depaPatientTypes, bool isCalledApi, bool isClickPick);
        public DepaPatientTypeSaved OnDepaPatientTypeSaved;
        #endregion

        public frmDepartmentPatientType()
        {
            string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
            this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            InitializeComponent();
        }
        public frmDepartmentPatientType(long? serviceId, List<HIS_DEPA_PATIENT_TYPE> depaPatientTypes, bool isCalledApi, bool isClickPick)
        {
            string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
            this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            this.serviceId = serviceId;
            this.depaPatientTypes = depaPatientTypes;
            this.isCalledApi = isCalledApi;
            this.isClickPick = isClickPick; 
            InitializeComponent();

        }

        public frmDepartmentPatientType(List<HIS_DEPA_PATIENT_TYPE> depaPatientTypes)
        {
            string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
            this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            this.depaPatientTypes = depaPatientTypes;
            InitializeComponent();
        }

        private void frmDepartmentPatientType_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetDefaultValueMode();
                FillDataToGridDepartment();
                FillDataToGridPatientType();
                LoadComboChooseMode();
                UpdateGridControlState();
                this.KeyPreview = true;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridPatientType()
        {
            try
            {
                int pagingSize = ucPaging2.pagingGrid != null ? ucPaging2.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize; // xác định số dòng /trang
                LoadPatientType(new CommonParam(0, pagingSize)); // nạp dữ liệu cho trang đầu tiên
                CommonParam param = new CommonParam();
                param.Limit = patientTypeRowCount;  // giới hạn số dòng
                param.Count = patientTypeDataTotal; // tính tổng số trang trong phân trang
                ucPaging2.Init(LoadPatientType, param, pagingSize, this.gridControlPatientType); // khởi tạo phân trang 
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridDepartment()
        {
            try
            {
                int pagingSize = ucPaging1.pagingGrid != null ? ucPaging1.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize; // xác định số dòng /trang
                LoadDepartment(new CommonParam(0, pagingSize)); // nạp dữ liệu cho trang đầu tiên
                CommonParam param = new CommonParam();
                param.Limit = deptRowCount;  // giới hạn số dòng
                param.Count = deptDataTotal; // tính tổng số trang trong phân trang
                ucPaging1.Init(LoadDepartment, param, pagingSize, this.gridControlDepartment); // khởi tạo phân trang 
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPatientType(object param)
        {
            try
            {
                WaitingManager.Show();
                gridControlPatientType.DataSource = null;
                List<PatientTypeADO> listPatientType = new List<PatientTypeADO>();

                patientTypeStart = ((CommonParam)param).Start ?? 0;
                var limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(patientTypeStart, limit);
                HisPatientTypeFilter filter = new HisPatientTypeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.KEY_WORD = txtSearchPatientType.Text;
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                var patientTypes = new BackendAdapter(paramCommon).GetRO<List<HIS_PATIENT_TYPE>>("api/HisPatientType/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (patientTypes != null && patientTypes.Data.Count > 0)
                {
                    listPatientType = patientTypes.Data.Select(p => new PatientTypeADO(p)).ToList();
                    patientTypeRowCount = listPatientType.Count;
                    patientTypeDataTotal = (patientTypes.Param == null ? 0 : patientTypes.Param.Count ?? 0);
                }
                gridControlPatientType.BeginUpdate();
                gridControlPatientType.DataSource = listPatientType;
                gridControlPatientType.EndUpdate();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void LoadDepartment(object param)
        {
            try
            {
                WaitingManager.Show();
                gridControlDepartment.DataSource = null;
                List<DepartmentADO> listDepartment = new List<DepartmentADO>();
                deptStart = ((CommonParam)param).Start ?? 0;
                var limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(deptStart, limit);
                HisDepartmentFilter filter = new HisDepartmentFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.KEY_WORD = txtSearchDepartment.Text;
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                var departments = new BackendAdapter(paramCommon).GetRO<List<HIS_DEPARTMENT>>("api/HisDepartment/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                if (departments != null && departments.Data.Count > 0)
                {
                    listDepartment = departments.Data.Select(d => new DepartmentADO(d)).ToList();
                    deptRowCount = listDepartment.Count;
                    deptDataTotal = (departments.Param == null ? 0 : departments.Param.Count ?? 0);
                    //deptDataTotal = departments == null ? 0 : departments.Data.Count();
                }
                gridControlDepartment.BeginUpdate();
                gridControlDepartment.DataSource = listDepartment;
                gridControlDepartment.EndUpdate();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void LoadComboChooseMode()
        {
            try
            {
                List<object> selectionModeList = new List<object>
                {
                    new { MODE_NAME = "Khoa", MODE_CODE = "DEPARTMENT" },
                    new { MODE_NAME = "ĐTTT", MODE_CODE = "PATIENT_TYPE" }
                };
                List<ColumnInfo> columnInfos = new List<ColumnInfo>
                {
                    new ColumnInfo("MODE_NAME", "", 100, 1)
                };
                ControlEditorADO controlEditorADO = new ControlEditorADO("MODE_NAME", "MODE_CODE", columnInfos, false, 100);
                ControlEditorLoader.Load(cboChooseMode, selectionModeList, controlEditorADO);
                cboChooseMode.EditValue = "DEPARTMENT";
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueMode()
        {
            try
            {
                selectionMode = "DEPARTMENT";
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void cboChooseMode_EditValueChanged(object sender, EventArgs e)
        {
            selectionMode = cboChooseMode.EditValue?.ToString();
            UpdateGridControlState();
            FillDataToGridDepartment();
            FillDataToGridPatientType();
        }

        private void UpdateGridControlState()
        {
            try
            {
                RepositoryItemCheckEdit checkEditDept = gridViewDepartment.Columns["IsCheckBoxDeptChecked_Str"].ColumnEdit as RepositoryItemCheckEdit;
                RepositoryItemCheckEdit checkEditPatientType = gridViewPatientType.Columns["IsCheckBoxPatientTypeChecked_Str"].ColumnEdit as RepositoryItemCheckEdit;
                if (selectionMode == "DEPARTMENT")
                {
                    gridViewPatientType.Columns["IsRadioPatientTypeChecked_Str"].OptionsColumn.AllowEdit = false;
                    gridViewDepartment.Columns["IsRadioDeptChecked_Str"].OptionsColumn.AllowEdit = true;
                    gridViewDepartment.Columns["IsCheckBoxDeptChecked_Str"].OptionsColumn.AllowEdit = false;
                    checkEditDept.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Style2;
                    checkEditPatientType.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Standard;
                    gridViewPatientType.Columns["IsCheckBoxPatientTypeChecked_Str"].OptionsColumn.AllowEdit = true;
                }
                else
                {
                    gridViewPatientType.Columns["IsRadioPatientTypeChecked_Str"].OptionsColumn.AllowEdit = true;
                    gridViewDepartment.Columns["IsRadioDeptChecked_Str"].OptionsColumn.AllowEdit = false;
                    gridViewDepartment.Columns["IsCheckBoxDeptChecked_Str"].OptionsColumn.AllowEdit = true;
                    gridViewPatientType.Columns["IsCheckBoxPatientTypeChecked_Str"].OptionsColumn.AllowEdit = false;
                    checkEditPatientType.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Style2;
                    checkEditDept.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Standard;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewDepartment_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            if (e.Column != null && e.Column.FieldName == "IsCheckBoxDeptChecked_Str")
            {
                e.Info.InnerElements.Clear();
                e.Painter.DrawObject(e.Info);

                RepositoryItemCheckEdit checkEdit = e.Column.ColumnEdit as RepositoryItemCheckEdit;
                if (checkEdit != null)
                {
                    int size = 16;
                    int x = e.Bounds.X + (e.Bounds.Width - size) / 2;
                    int y = e.Bounds.Y + (e.Bounds.Height - size) / 2;
                    Rectangle rect = new Rectangle(x, y, size, size);

                    var info = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.CreateViewInfo();
                    var painter = (DevExpress.XtraEditors.Drawing.CheckEditPainter)checkEdit.CreatePainter();
                    info.EditValue = isHeaderDeptChecked;
                    info.Bounds = rect;
                    info.CalcViewInfo(e.Graphics);

                    using (DevExpress.Utils.Drawing.GraphicsCache cache = new DevExpress.Utils.Drawing.GraphicsCache(e.Graphics))
                    {
                        painter.Draw(new DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs(info, cache, rect));
                    }
                }
                e.Handled = true;
            }
        }

        private void gridViewDepartment_MouseDown(object sender, MouseEventArgs e) //click vào header của checkbox
        {
            GridView view = sender as GridView;
            GridHitInfo info = view.CalcHitInfo(e.Location);
            if (info.InColumn && info.Column.FieldName == "IsCheckBoxDeptChecked_Str" && selectionMode == "PATIENT_TYPE")
            {
                isHeaderDeptChecked = !isHeaderDeptChecked;
                for (int i = 0; i < view.RowCount; i++)
                {
                    view.SetRowCellValue(i, view.Columns["IsCheckBoxDeptChecked_Str"], isHeaderDeptChecked);
                }
                view.InvalidateColumnHeader(view.Columns["IsCheckBoxDeptChecked_Str"]);
            }
        }
        private void gridViewPatientType_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column != null && e.Column.FieldName == "IsCheckBoxPatientTypeChecked_Str")
                {
                    e.Info.InnerElements.Clear();
                    e.Painter.DrawObject(e.Info);
                    RepositoryItemCheckEdit checkEdit = e.Column.ColumnEdit as RepositoryItemCheckEdit;
                    if (checkEdit != null)
                    {
                        int size = 16;
                        int x = e.Bounds.X + (e.Bounds.Width - size) / 2;
                        int y = e.Bounds.Y + (e.Bounds.Height - size) / 2;
                        Rectangle rect = new Rectangle(x, y, size, size);
                        var info = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.CreateViewInfo();
                        var painter = (DevExpress.XtraEditors.Drawing.CheckEditPainter)checkEdit.CreatePainter();
                        info.EditValue = isHeaderPatientTypeChecked;
                        info.Bounds = rect;
                        info.CalcViewInfo(e.Graphics);
                        using (DevExpress.Utils.Drawing.GraphicsCache cache = new DevExpress.Utils.Drawing.GraphicsCache(e.Graphics))
                        {
                            painter.Draw(new DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs(info, cache, rect));
                        }
                    }
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void gridViewPatientType_MouseDown(object sender, MouseEventArgs e) // click header
        {
            try
            {
                GridView view = sender as GridView;
                GridHitInfo info = view.CalcHitInfo(e.Location);
                if (info.InColumn && info.Column.FieldName == "IsCheckBoxPatientTypeChecked_Str" && selectionMode == "DEPARTMENT")
                {
                    isHeaderPatientTypeChecked = !isHeaderPatientTypeChecked;
                    for (int i = 0; i < view.RowCount; i++)
                    {
                        view.SetRowCellValue(i, view.Columns["IsCheckBoxPatientTypeChecked_Str"], isHeaderPatientTypeChecked);
                    }
                    view.InvalidateColumnHeader(view.Columns["IsCheckBoxPatientTypeChecked_Str"]);
                    selectedPatientTypes = ((List<PatientTypeADO>)gridControlPatientType.DataSource)
                        .Where(p => p.IsCheckBoxChecked).ToList();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void gridViewDepartment_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (DepartmentADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "IsRadioDeptChecked_Str")
                        {
                            e.Value = data.IsRadioChecked;
                        }
                        else if (e.Column.FieldName == "IsCheckBoxDeptChecked_Str")
                        {
                            e.Value = data.IsCheckBoxChecked;
                        }
                    }
                }
                else if (e.IsSetData)
                {
                    var data = (DepartmentADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "IsRadioDeptChecked_Str" && (bool)e.Value)
                        {
                            var dataSource = (List<DepartmentADO>)gridControlDepartment.DataSource;
                            foreach (var item in dataSource)
                            {
                                item.IsRadioChecked = false;
                            }
                            data.IsRadioChecked = true;
                            selectedDepartments = new List<DepartmentADO> { data };
                            gridControlDepartment.RefreshDataSource();
                            //if (selectionMode == "DEPARTMENT")
                            //{
                            //    LoadSelectedPatientTypes();
                            //}
                        }
                        else if (e.Column.FieldName == "IsCheckBoxDeptChecked_Str")
                        {
                            data.IsCheckBoxChecked = (bool)e.Value;
                            selectedDepartments = ((List<DepartmentADO>)gridControlDepartment.DataSource)
                                .Where(d => d.IsCheckBoxChecked).ToList();
                            unSelectedDepartments = ((List<DepartmentADO>)gridControlDepartment.DataSource)
                                .Where(d => !d.IsCheckBoxChecked).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewPatientType_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (PatientTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "IsRadioPatientTypeChecked_Str")
                        {
                            e.Value = data.IsRadioChecked;
                        }
                        else if (e.Column.FieldName == "IsCheckBoxPatientTypeChecked_Str")
                        {
                            e.Value = data.IsCheckBoxChecked;
                        }
                    }
                }
                else if (e.IsSetData)
                {
                    var data = (PatientTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "IsRadioPatientTypeChecked_Str" && (bool)e.Value)
                        {
                            var dataSource = (List<PatientTypeADO>)gridControlPatientType.DataSource;
                            foreach (var item in dataSource)
                            {
                                item.IsRadioChecked = false;
                            }
                            data.IsRadioChecked = true;
                            selectedPatientTypes = new List<PatientTypeADO> { data };
                            gridControlPatientType.RefreshDataSource();
                            //if (selectionMode == "PATIENT_TYPE")
                            //{
                            //    LoadSelectedDepartments();
                            //}
                        }
                        else if (e.Column.FieldName == "IsCheckBoxPatientTypeChecked_Str")
                        {
                            data.IsCheckBoxChecked = (bool)e.Value;
                            selectedPatientTypes = ((List<PatientTypeADO>)gridControlPatientType.DataSource)
                                .Where(p => p.IsCheckBoxChecked).ToList();
                            unSelectedPatientTypes = ((List<PatientTypeADO>)gridControlPatientType.DataSource)
                                .Where(p => !p.IsCheckBoxChecked).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewDepartment_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "IsRadioDeptChecked_Str" && selectionMode == "DEPARTMENT")
                {
                    var department = (DepartmentADO)gridViewDepartment.GetRow(e.RowHandle);
                    if (department.IsRadioChecked)
                    {
                        LoadSelectedPatientTypes();
                        repositoryItemCheckEdit_DepartmentRad.ReadOnly = true;
                        return;
                    }
                    if (department != null)
                    {
                        var dataSource = (List<DepartmentADO>)gridControlDepartment.DataSource;
                        foreach (var item in dataSource)
                        {
                            item.IsRadioChecked = false;
                        }
                        department.IsRadioChecked = true;
                        selectedDepartments = new List<DepartmentADO> { department };
                        gridControlDepartment.RefreshDataSource();
                        LoadSelectedPatientTypes();
                    }
                }
                else if (e.Column.FieldName == "IsCheckBoxDeptChecked_Str" && selectionMode == "PATIENT_TYPE")
                {
                    var department = (DepartmentADO)gridViewDepartment.GetRow(e.RowHandle);
                    if (department != null)
                    {
                        department.IsCheckBoxChecked = !(department.IsCheckBoxChecked);
                        selectedDepartments = ((List<DepartmentADO>)gridControlDepartment.DataSource)
                            .Where(d => d.IsCheckBoxChecked).ToList();
                        gridControlDepartment.RefreshDataSource();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            finally
            {
                repositoryItemCheckEdit_DepartmentRad.ReadOnly = false;
            }
        }

        private void gridViewPatientType_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "IsRadioPatientTypeChecked_Str" && selectionMode == "PATIENT_TYPE")
                {
                    var patientType = (PatientTypeADO)gridViewPatientType.GetRow(e.RowHandle);
                    if (patientType.IsRadioChecked)
                    {
                        LoadSelectedDepartments();
                        repositoryItemCheckEdit_PatientTypeRad.ReadOnly = true;
                        return;
                    }
                    if (patientType != null)
                    {
                        var dataSource = (List<PatientTypeADO>)gridControlPatientType.DataSource;
                        foreach (var item in dataSource)
                        {
                            item.IsRadioChecked = false;
                        }
                        patientType.IsRadioChecked = true;
                        selectedPatientTypes = new List<PatientTypeADO> { patientType };
                        gridControlPatientType.RefreshDataSource();
                        LoadSelectedDepartments();
                    }
                }
                else if (e.Column.FieldName == "IsCheckBoxPatientTypeChecked_Str" && selectionMode == "DEPARTMENT")
                {
                    var patientType = (PatientTypeADO)gridViewPatientType.GetRow(e.RowHandle); //lấy giá trị hàng được click
                    if (patientType != null)
                    {
                        patientType.IsCheckBoxChecked = !patientType.IsCheckBoxChecked;
                        selectedPatientTypes = ((List<PatientTypeADO>)gridControlPatientType.DataSource)
                            .Where(p => p.IsCheckBoxChecked).ToList();
                        gridControlPatientType.RefreshDataSource();  //bỏ đi cũng được
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            finally
            {
                repositoryItemCheckEdit_PatientTypeRad.ReadOnly = false;
            }
        }

        private void LoadSelectedDepartments()
        {
            try
            {
                //if (selectionMode == "PATIENT_TYPE")
                //if (this.serviceId.HasValue)
                //{
                if (selectionMode == "PATIENT_TYPE")
                {
                    var selectedPatientType = selectedPatientTypes.FirstOrDefault(p => p.IsRadioChecked);
                    if (selectedPatientType != null)
                    {
                        if (this.serviceId != null && this.isCalledApi == false)
                        {
                            var deptPatientTypeFromDb = BackendDataWorker.Get<HIS_DEPA_PATIENT_TYPE>().Where(p => p.SERVICE_ID == this.serviceId).ToList();
                            //depaPatientTypes.AddRange(deptPatientTypeFromDb);
                            depaPatientTypes.AddRange(
                                deptPatientTypeFromDb.Where(newItem => !depaPatientTypes.Any(
                                    exist => exist.DEPARTMENT_ID == newItem.DEPARTMENT_ID
                                          && exist.PATIENT_TYPE_ID == newItem.PATIENT_TYPE_ID
                                          && exist.SERVICE_ID == newItem.SERVICE_ID))
                            );
                        }
                        selectedDepartments = depaPatientTypes
                                .Where(dpt => dpt.PATIENT_TYPE_ID == selectedPatientType.ID)
                                .Join(BackendDataWorker.Get<HIS_DEPARTMENT>(),
                                    dpt => dpt.DEPARTMENT_ID,
                                    dept => dept.ID,
                                    (dpt, dept) => new DepartmentADO(dept) { IsCheckBoxChecked = true })
                                .ToList();
                        gridViewDepartment.ClearSelection();
                        var dataSource = (List<DepartmentADO>)gridControlDepartment.DataSource;
                        foreach (var dept in dataSource)
                        {
                            dept.IsCheckBoxChecked = false;
                        }
                        foreach (var dept in selectedDepartments)
                        {
                            int rowHandle = gridViewDepartment.LocateByValue("ID", dept.ID);
                            if (rowHandle != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
                            {
                                gridViewDepartment.SelectRow(rowHandle);
                                dataSource.FirstOrDefault(d => d.ID == dept.ID).IsCheckBoxChecked = true;
                            }
                        }
                        gridControlDepartment.RefreshDataSource();
                    }
                }
                //}

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void LoadSelectedPatientTypes()
        {
            try
            {
                if (selectionMode == "DEPARTMENT")
                {
                    var allHisPatientTypes = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                    var selectedDepartment = selectedDepartments.FirstOrDefault(d => d.IsRadioChecked);
                    if (selectedDepartment != null)
                    {
                        if (this.serviceId != null && this.isCalledApi == false)
                        {
                            var deptPatientTypeFromDb = BackendDataWorker.Get<HIS_DEPA_PATIENT_TYPE>().Where(p => p.SERVICE_ID == this.serviceId).ToList();
                            //depaPatientTypes.AddRange(deptPatientTypeFromDb);
                            depaPatientTypes.AddRange(
                                deptPatientTypeFromDb.Where(newItem => !depaPatientTypes.Any(
                                    exist => exist.DEPARTMENT_ID == newItem.DEPARTMENT_ID
                                          && exist.PATIENT_TYPE_ID == newItem.PATIENT_TYPE_ID
                                          && exist.SERVICE_ID == newItem.SERVICE_ID))
                            );
                            this.isCalledApi = true;

                        }
                        selectedPatientTypes = depaPatientTypes
                            .Where(dpt => dpt.DEPARTMENT_ID == selectedDepartment.ID
                            )
                            .Join(allHisPatientTypes,
                                dpt => dpt.PATIENT_TYPE_ID,
                                pt => pt.ID,
                                (dpt, pt) => new PatientTypeADO(pt) { IsCheckBoxChecked = true })
                            .ToList();
                        gridViewPatientType.ClearSelection();
                        var dataSource = (List<PatientTypeADO>)gridControlPatientType.DataSource;

                        foreach (var dept in dataSource)
                        {
                            dept.IsCheckBoxChecked = false;
                        }
                        foreach (var pt in selectedPatientTypes)
                        {
                            int rowHandle = gridViewPatientType.LocateByValue("ID", pt.ID);
                            if (rowHandle != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
                            {
                                gridViewPatientType.SelectRow(rowHandle);
                                dataSource.FirstOrDefault(p => p.ID == pt.ID).IsCheckBoxChecked = true;
                            }
                        }
                        gridControlPatientType.RefreshDataSource();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.isClickPick = true; 
                WaitingManager.Show();
                //unSelectedPatientTypes = ((List<PatientTypeADO>)gridControlPatientType.DataSource)
                //                .Where(p => !p.IsCheckBoxChecked).ToList();
                List<HIS_DEPA_PATIENT_TYPE> depaPatientTypeItems = new List<HIS_DEPA_PATIENT_TYPE>();
                if (selectionMode == "DEPARTMENT")
                {
                    var department = selectedDepartments.FirstOrDefault(d => d.IsRadioChecked);
                    if (department == null)
                    {
                        MessageManager.Show("Vui lòng chọn một khoa!");
                        WaitingManager.Hide();
                        return;
                    }
                    //if (!selectedPatientTypes.Any(p => p.IsCheckBoxChecked))
                    //{
                    //    MessageManager.Show("Vui lòng chọn ít nhất một đối tượng thanh toán!");
                    //    WaitingManager.Hide();
                    //    return;
                    //}
                    foreach (var pt in unSelectedPatientTypes) // loại bỏ những thằng vừa chọn xong lại bỏ chọn, của cái department đó thôi
                    {
                        depaPatientTypes.RemoveAll(x =>
                            x.DEPARTMENT_ID == department.ID &&
                            x.PATIENT_TYPE_ID == pt.ID &&
                            x.SERVICE_ID == this.serviceId);
                    }
                    depaPatientTypeItems = selectedPatientTypes
                        .Where(p => p.IsCheckBoxChecked)
                        .Select(pt => new HIS_DEPA_PATIENT_TYPE
                        {
                            DEPARTMENT_ID = department.ID,
                            PATIENT_TYPE_ID = pt.ID,
                            SERVICE_ID = this.serviceId
                        })
                        .ToList();
                    //depaPatientTypes.AddRange(depaPatientTypeItems);
                    depaPatientTypes.AddRange(
                    depaPatientTypeItems.Where(newItem => !depaPatientTypes.Any(
                        exist => exist.DEPARTMENT_ID == newItem.DEPARTMENT_ID
                              && exist.PATIENT_TYPE_ID == newItem.PATIENT_TYPE_ID
                              && exist.SERVICE_ID == newItem.SERVICE_ID))
                                );
                }
                else
                {
                    var patientType = selectedPatientTypes.FirstOrDefault(p => p.IsRadioChecked);
                    if (patientType == null)
                    {
                        MessageManager.Show("Vui lòng chọn một đối tượng thanh toán!");
                        WaitingManager.Hide();
                        return;
                    }
                    //if (!selectedDepartments.Any(d => d.IsCheckBoxChecked))
                    //{
                    //    MessageManager.Show("Vui lòng chọn ít nhất một khoa!");
                    //    WaitingManager.Hide();
                    //    return;
                    //}
                    foreach (var dept in unSelectedDepartments) // loại bỏ những thằng vừa chọn xong lại bỏ chọn 
                    {
                        depaPatientTypes.RemoveAll(x =>
                            x.PATIENT_TYPE_ID == patientType.ID &&
                            x.DEPARTMENT_ID == dept.ID &&
                            x.SERVICE_ID == this.serviceId);
                    }
                    depaPatientTypeItems = selectedDepartments
                        .Where(d => d.IsCheckBoxChecked)
                        .Select(dept => new HIS_DEPA_PATIENT_TYPE
                        {
                            DEPARTMENT_ID = dept.ID,
                            PATIENT_TYPE_ID = patientType.ID,
                            SERVICE_ID = this.serviceId
                        })
                        .ToList();
                    //depaPatientTypes.AddRange(depaPatientTypeItems);
                    depaPatientTypes.AddRange(
                   depaPatientTypeItems.Where(newItem => !depaPatientTypes.Any(
                       exist => exist.DEPARTMENT_ID == newItem.DEPARTMENT_ID
                             && exist.PATIENT_TYPE_ID == newItem.PATIENT_TYPE_ID
                             ))
                               );
                }
                OnDepaPatientTypeSaved?.Invoke(this.depaPatientTypes, this.isCalledApi, this.isClickPick);
                WaitingManager.Hide();
                CommonParam param = new CommonParam();
                MessageManager.Show(this, param, true);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void btnSearchDepartment_Click(object sender, EventArgs e)
        {
            FillDataToGridDepartment();
        }

        private void btnSearchPatient_Click(object sender, EventArgs e)
        {
            FillDataToGridPatientType();
        }

        private void frmDepartmentPatientType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.T)
            {
                btnSave_Click(null, null);
                e.Handled = true;
            }
        }

        private void txtSearchDepartment_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.F) || e.KeyCode == Keys.Enter)
            {
                btnSearchDepartment_Click(null, null);
                e.Handled = true;
            }
        }

        private void txtSearchPatientType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.D || e.KeyCode == Keys.Enter)
            {
                btnSearchPatient_Click(null, null);
                e.Handled = true;
            }
        }

        private void btnSave_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Control && e.KeyCode == Keys.T)
            //{
            //    btnSave_Click(null, null);
            //    e.Handled = true;
            //}
        }
    }
}
