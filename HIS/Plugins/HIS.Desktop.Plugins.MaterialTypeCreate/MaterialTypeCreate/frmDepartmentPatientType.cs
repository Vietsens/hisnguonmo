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
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton simpleButton2;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.TextEdit txtSearchPatientType;
        private DevExpress.XtraEditors.TextEdit txtSearchDepartment;
        private DevExpress.XtraGrid.GridControl gridControlPatientType;
        private GridView gridViewPatientType;
        private DevExpress.XtraGrid.GridControl gridControlDepartment;
        private GridView gridViewDepartment;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private Inventec.UC.Paging.UcPaging ucPaging2;
        private Inventec.UC.Paging.UcPaging ucPaging1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private RepositoryItemCheckEdit repositoryItemCheckEdit_DepartmentRad;
        private RepositoryItemCheckEdit repositoryItemCheckEdit2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.GridLookUpEdit cboChooseMode;
        private GridView gridLookUpEdit1View;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private RepositoryItemCheckEdit repositoryItemCheckEdit_PatientTypeRad;
        private RepositoryItemCheckEdit repositoryItemCheckEdit3;

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
                LoadSelectedPatientTypes();
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
                LoadSelectedDepartments(); 
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
                btnSave.Focus(); 
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

        private void InitializeComponent()
        {
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.cboChooseMode = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.ucPaging2 = new Inventec.UC.Paging.UcPaging();
            this.ucPaging1 = new Inventec.UC.Paging.UcPaging();
            this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.txtSearchPatientType = new DevExpress.XtraEditors.TextEdit();
            this.txtSearchDepartment = new DevExpress.XtraEditors.TextEdit();
            this.gridControlPatientType = new DevExpress.XtraGrid.GridControl();
            this.gridViewPatientType = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit_PatientTypeRad = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridControlDepartment = new DevExpress.XtraGrid.GridControl();
            this.gridViewDepartment = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit_DepartmentRad = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboChooseMode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchPatientType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchDepartment.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlPatientType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPatientType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit_PatientTypeRad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlDepartment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDepartment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit_DepartmentRad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.cboChooseMode);
            this.layoutControl1.Controls.Add(this.ucPaging2);
            this.layoutControl1.Controls.Add(this.ucPaging1);
            this.layoutControl1.Controls.Add(this.simpleButton2);
            this.layoutControl1.Controls.Add(this.simpleButton1);
            this.layoutControl1.Controls.Add(this.txtSearchPatientType);
            this.layoutControl1.Controls.Add(this.txtSearchDepartment);
            this.layoutControl1.Controls.Add(this.gridControlPatientType);
            this.layoutControl1.Controls.Add(this.gridControlDepartment);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1087, 543);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(994, 519);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(91, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 13;
            this.btnSave.Text = "Chọn(Ctrl T)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cboChooseMode
            // 
            this.cboChooseMode.EditValue = "";
            this.cboChooseMode.Location = new System.Drawing.Point(835, 519);
            this.cboChooseMode.Name = "cboChooseMode";
            this.cboChooseMode.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboChooseMode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboChooseMode.Properties.NullText = "";
            this.cboChooseMode.Properties.View = this.gridLookUpEdit1View;
            this.cboChooseMode.Size = new System.Drawing.Size(155, 20);
            this.cboChooseMode.StyleController = this.layoutControl1;
            this.cboChooseMode.TabIndex = 12;
            this.cboChooseMode.EditValueChanged += new System.EventHandler(this.cboChooseMode_EditValueChanged);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // ucPaging2
            // 
            this.ucPaging2.Location = new System.Drawing.Point(547, 490);
            this.ucPaging2.Name = "ucPaging2";
            this.ucPaging2.Size = new System.Drawing.Size(538, 25);
            this.ucPaging2.TabIndex = 11;
            // 
            // ucPaging1
            // 
            this.ucPaging1.Location = new System.Drawing.Point(2, 490);
            this.ucPaging1.Name = "ucPaging1";
            this.ucPaging1.Size = new System.Drawing.Size(541, 25);
            this.ucPaging1.TabIndex = 10;
            // 
            // simpleButton2
            // 
            this.simpleButton2.Location = new System.Drawing.Point(1001, 2);
            this.simpleButton2.Name = "simpleButton2";
            this.simpleButton2.Size = new System.Drawing.Size(84, 22);
            this.simpleButton2.StyleController = this.layoutControl1;
            this.simpleButton2.TabIndex = 9;
            this.simpleButton2.Text = "Tìm kiếm(Ctrl D)";
            this.simpleButton2.Click += new System.EventHandler(this.btnSearchPatient_Click);
            // 
            // simpleButton1
            // 
            this.simpleButton1.Location = new System.Drawing.Point(458, 2);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(85, 22);
            this.simpleButton1.StyleController = this.layoutControl1;
            this.simpleButton1.TabIndex = 8;
            this.simpleButton1.Text = "Tìm kiếm(Ctrl F)";
            this.simpleButton1.Click += new System.EventHandler(this.btnSearchDepartment_Click);
            // 
            // txtSearchPatientType
            // 
            this.txtSearchPatientType.Location = new System.Drawing.Point(547, 2);
            this.txtSearchPatientType.Name = "txtSearchPatientType";
            this.txtSearchPatientType.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtSearchPatientType.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtSearchPatientType.Size = new System.Drawing.Size(450, 20);
            this.txtSearchPatientType.StyleController = this.layoutControl1;
            this.txtSearchPatientType.TabIndex = 7;
            this.txtSearchPatientType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearchPatientType_KeyDown);
            // 
            // txtSearchDepartment
            // 
            this.txtSearchDepartment.Location = new System.Drawing.Point(2, 2);
            this.txtSearchDepartment.Name = "txtSearchDepartment";
            this.txtSearchDepartment.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtSearchDepartment.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtSearchDepartment.Size = new System.Drawing.Size(452, 20);
            this.txtSearchDepartment.StyleController = this.layoutControl1;
            this.txtSearchDepartment.TabIndex = 6;
            this.txtSearchDepartment.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearchDepartment_KeyDown);
            // 
            // gridControlPatientType
            // 
            this.gridControlPatientType.Location = new System.Drawing.Point(547, 28);
            this.gridControlPatientType.MainView = this.gridViewPatientType;
            this.gridControlPatientType.Name = "gridControlPatientType";
            this.gridControlPatientType.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit_PatientTypeRad,
            this.repositoryItemCheckEdit3});
            this.gridControlPatientType.Size = new System.Drawing.Size(538, 458);
            this.gridControlPatientType.TabIndex = 5;
            this.gridControlPatientType.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewPatientType});
            // 
            // gridViewPatientType
            // 
            this.gridViewPatientType.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn8});
            this.gridViewPatientType.GridControl = this.gridControlPatientType;
            this.gridViewPatientType.Name = "gridViewPatientType";
            this.gridViewPatientType.OptionsView.ShowGroupPanel = false;
            this.gridViewPatientType.OptionsView.ShowIndicator = false;
            this.gridViewPatientType.CustomDrawColumnHeader += new DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventHandler(this.gridViewPatientType_CustomDrawColumnHeader);
            this.gridViewPatientType.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewPatientType_CellValueChanging);
            this.gridViewPatientType.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewPatientType_CustomUnboundColumnData);
            this.gridViewPatientType.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridViewPatientType_MouseDown);
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "gridColumn5";
            this.gridColumn5.ColumnEdit = this.repositoryItemCheckEdit_PatientTypeRad;
            this.gridColumn5.FieldName = "IsRadioPatientTypeChecked_Str";
            this.gridColumn5.MaxWidth = 100;
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.OptionsColumn.ShowCaption = false;
            this.gridColumn5.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 0;
            this.gridColumn5.Width = 80;
            // 
            // repositoryItemCheckEdit_PatientTypeRad
            // 
            this.repositoryItemCheckEdit_PatientTypeRad.AutoHeight = false;
            this.repositoryItemCheckEdit_PatientTypeRad.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.repositoryItemCheckEdit_PatientTypeRad.Name = "repositoryItemCheckEdit_PatientTypeRad";
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "gridColumn6";
            this.gridColumn6.ColumnEdit = this.repositoryItemCheckEdit3;
            this.gridColumn6.FieldName = "IsCheckBoxPatientTypeChecked_Str";
            this.gridColumn6.MaxWidth = 100;
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.OptionsColumn.ShowCaption = false;
            this.gridColumn6.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 1;
            this.gridColumn6.Width = 80;
            // 
            // repositoryItemCheckEdit3
            // 
            this.repositoryItemCheckEdit3.AutoHeight = false;
            this.repositoryItemCheckEdit3.Name = "repositoryItemCheckEdit3";
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "Mã đối tượng";
            this.gridColumn7.FieldName = "PATIENT_TYPE_CODE";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 2;
            this.gridColumn7.Width = 178;
            // 
            // gridColumn8
            // 
            this.gridColumn8.Caption = "Tên đối tượng";
            this.gridColumn8.FieldName = "PATIENT_TYPE_NAME";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 3;
            this.gridColumn8.Width = 190;
            // 
            // gridControlDepartment
            // 
            this.gridControlDepartment.Location = new System.Drawing.Point(2, 28);
            this.gridControlDepartment.MainView = this.gridViewDepartment;
            this.gridControlDepartment.Name = "gridControlDepartment";
            this.gridControlDepartment.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit_DepartmentRad,
            this.repositoryItemCheckEdit2});
            this.gridControlDepartment.Size = new System.Drawing.Size(541, 458);
            this.gridControlDepartment.TabIndex = 4;
            this.gridControlDepartment.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewDepartment});
            // 
            // gridViewDepartment
            // 
            this.gridViewDepartment.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4});
            this.gridViewDepartment.GridControl = this.gridControlDepartment;
            this.gridViewDepartment.Name = "gridViewDepartment";
            this.gridViewDepartment.OptionsView.ShowGroupPanel = false;
            this.gridViewDepartment.OptionsView.ShowIndicator = false;
            this.gridViewDepartment.CustomDrawColumnHeader += new DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventHandler(this.gridViewDepartment_CustomDrawColumnHeader);
            this.gridViewDepartment.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewDepartment_CellValueChanging);
            this.gridViewDepartment.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewDepartment_CustomUnboundColumnData);
            this.gridViewDepartment.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridViewDepartment_MouseDown);
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "gridColumn1";
            this.gridColumn1.ColumnEdit = this.repositoryItemCheckEdit_DepartmentRad;
            this.gridColumn1.FieldName = "IsRadioDeptChecked_Str";
            this.gridColumn1.MaxWidth = 100;
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.ShowCaption = false;
            this.gridColumn1.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 80;
            // 
            // repositoryItemCheckEdit_DepartmentRad
            // 
            this.repositoryItemCheckEdit_DepartmentRad.AutoHeight = false;
            this.repositoryItemCheckEdit_DepartmentRad.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.repositoryItemCheckEdit_DepartmentRad.Name = "repositoryItemCheckEdit_DepartmentRad";
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "gridColumn2";
            this.gridColumn2.ColumnEdit = this.repositoryItemCheckEdit2;
            this.gridColumn2.FieldName = "IsCheckBoxDeptChecked_Str";
            this.gridColumn2.MaxWidth = 100;
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.ShowCaption = false;
            this.gridColumn2.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            this.gridColumn2.Width = 80;
            // 
            // repositoryItemCheckEdit2
            // 
            this.repositoryItemCheckEdit2.AutoHeight = false;
            this.repositoryItemCheckEdit2.Name = "repositoryItemCheckEdit2";
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Mã khoa";
            this.gridColumn3.FieldName = "DEPARTMENT_CODE";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            this.gridColumn3.Width = 180;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "Tên khoa";
            this.gridColumn4.FieldName = "DEPARTMENT_NAME";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 3;
            this.gridColumn4.Width = 207;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.layoutControlItem5,
            this.layoutControlItem6,
            this.layoutControlItem7,
            this.layoutControlItem8,
            this.layoutControlItem9,
            this.layoutControlItem10,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(1087, 543);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControlDepartment;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 26);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(545, 462);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.gridControlPatientType;
            this.layoutControlItem2.Location = new System.Drawing.Point(545, 26);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(542, 462);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.txtSearchDepartment;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(456, 26);
            this.layoutControlItem3.Text = "Tìm kiếm";
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.txtSearchPatientType;
            this.layoutControlItem4.Location = new System.Drawing.Point(545, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(454, 26);
            this.layoutControlItem4.Text = "Tìm kiếm:";
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.simpleButton1;
            this.layoutControlItem5.Location = new System.Drawing.Point(456, 0);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(89, 26);
            this.layoutControlItem5.Text = "Tìm kiếm(Ctrl F)";
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.simpleButton2;
            this.layoutControlItem6.Location = new System.Drawing.Point(999, 0);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(88, 26);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.ucPaging1;
            this.layoutControlItem7.Location = new System.Drawing.Point(0, 488);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(545, 29);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.ucPaging2;
            this.layoutControlItem8.Location = new System.Drawing.Point(545, 488);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(542, 29);
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextVisible = false;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.cboChooseMode;
            this.layoutControlItem9.Location = new System.Drawing.Point(773, 517);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(219, 26);
            this.layoutControlItem9.Text = "Chọn theo: ";
            this.layoutControlItem9.TextSize = new System.Drawing.Size(57, 13);
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.Control = this.btnSave;
            this.layoutControlItem10.Location = new System.Drawing.Point(992, 517);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Size = new System.Drawing.Size(95, 26);
            this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem10.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 517);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(773, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // frmDepartmentPatientType
            // 
            this.ClientSize = new System.Drawing.Size(1087, 543);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmDepartmentPatientType";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thiết lập khoa - đối tượng thanh toán";
            this.Load += new System.EventHandler(this.frmDepartmentPatientType_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmDepartmentPatientType_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboChooseMode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchPatientType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchDepartment.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlPatientType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPatientType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit_PatientTypeRad)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlDepartment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDepartment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit_DepartmentRad)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
