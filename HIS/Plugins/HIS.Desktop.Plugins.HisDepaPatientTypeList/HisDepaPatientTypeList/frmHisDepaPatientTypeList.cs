using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.HisDepaPatientTypeList.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisDepaPatientTypeList.HisDepaPatientTypeList
{
    public partial class frmHisDepaPatientTypeList : FormBase
    {
        #region Declare
        private const string SELECTION_MODE_DEPARTMENT = "DEPARTMENT";
        private const string SELECTION_MODE_PATIENT_TYPE = "PATIENT_TYPE";

        // Event public — caller subscribe sau khi user nhan "Chon".
        public delegate void DepaPatientTypeSaved(List<HIS_DEPA_PATIENT_TYPE> depaPatientTypes, bool isCalledApi, bool isClickPick);
        public event DepaPatientTypeSaved OnDepaPatientTypeSaved;

        private Inventec.Desktop.Common.Modules.Module currentModule;
        private DelegateSelectData callBackDelegate;

        private long? serviceId;
        private bool isCalledApi;
        private bool isClickPick;
        private List<HIS_DEPA_PATIENT_TYPE> depaPatientTypes = new List<HIS_DEPA_PATIENT_TYPE>();

        /// <summary>True khi dang Form_Load — chan cboChooseMode_EditValueChanged tu fill grid lai.</summary>
        private bool isLoading = false;

        private int deptRowCount = 0;
        private int deptDataTotal = 0;
        private int deptStart = 0;

        private int patientTypeRowCount = 0;
        private int patientTypeDataTotal = 0;
        private int patientTypeStart = 0;

        private string selectionMode = SELECTION_MODE_DEPARTMENT;
        /// <summary>Mode detect duoc tu config da luu khi mo form. Dung de re-apply pre-check khi user quay lai mode nay.</summary>
        private string detectedConfigMode = null;
        private bool isHeaderDeptChecked = false;
        private bool isHeaderPatientTypeChecked = false;
        /// <summary>Chan re-entrancy khi mutex tu dong bo tich header hao phi con lai.</summary>
        private bool isUpdatingExpendCheckAll = false;

        private List<DepartmentADO> selectedDepartments = new List<DepartmentADO>();
        private List<PatientTypeADO> selectedPatientTypes = new List<PatientTypeADO>();
        private List<PatientTypeADO> unSelectedPatientTypes = new List<PatientTypeADO>();
        private List<DepartmentADO> unSelectedDepartments = new List<DepartmentADO>();
        #endregion

        #region Constructor
        public frmHisDepaPatientTypeList()
        {
            InitializeComponent();
            SetIcon();
        }

        /// <summary>Mo form khi co serviceId (cap nhat thuoc/vat tu da save).</summary>
        public frmHisDepaPatientTypeList(long? serviceId, List<HIS_DEPA_PATIENT_TYPE> depaPatientTypes, bool isCalledApi, bool isClickPick)
        {
            try
            {
                InitializeComponent();
                SetIcon();
                this.serviceId = serviceId.HasValue && serviceId.Value != 0 ? serviceId : null;
                // Clone list de form mutate khong anh huong caller khi user Cancel/Close khong Save.
                this.depaPatientTypes = depaPatientTypes != null
                    ? new List<HIS_DEPA_PATIENT_TYPE>(depaPatientTypes)
                    : new List<HIS_DEPA_PATIENT_TYPE>();
                this.isCalledApi = isCalledApi;
                this.isClickPick = isClickPick;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Mo form khi chua co serviceId (tao moi thuoc/vat tu lan dau).</summary>
        public frmHisDepaPatientTypeList(List<HIS_DEPA_PATIENT_TYPE> depaPatientTypes)
        {
            try
            {
                InitializeComponent();
                SetIcon();
                this.depaPatientTypes = depaPatientTypes != null
                    ? new List<HIS_DEPA_PATIENT_TYPE>(depaPatientTypes)
                    : new List<HIS_DEPA_PATIENT_TYPE>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Constructor cho Behavior (qua MEF) — giu lai du de tuong thich. Khong dung khi goi truc tiep.</summary>
        public frmHisDepaPatientTypeList(
            Inventec.Desktop.Common.Modules.Module moduleData,
            DepaPatientTypeInputADO input,
            DelegateSelectData callBack)
            : base()
        {
            try
            {
                InitializeComponent();
                SetIcon();
                this.currentModule = moduleData;
                this.callBackDelegate = callBack;
                if (input != null)
                {
                    this.serviceId = input.ServiceId.HasValue && input.ServiceId.Value != 0 ? input.ServiceId : null;
                    this.depaPatientTypes = input.DepaPatientTypes != null
                        ? new List<HIS_DEPA_PATIENT_TYPE>(input.DepaPatientTypes)
                        : new List<HIS_DEPA_PATIENT_TYPE>();
                    this.isCalledApi = input.IsCalledApi;
                    this.isClickPick = input.IsClickPick;
                }
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
                string iconPath = System.IO.Path.Combine(
                    HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath,
                    System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Load
        private void frmHisDepaPatientTypeList_Load(object sender, EventArgs e)
        {
            try
            {
                isLoading = true;
                WaitingManager.Show();
                SetCaptionByLanguageKey();
                SetDefaultValueMode();
                LoadComboChooseMode();      // Set EditValue truoc → trigger EditValueChanged nhung bi chan boi isLoading.
                UpdateGridControlState();   // Set mode column edit-state truoc khi fill grid.
                HookHeaderCheckEvents();    // Gan event ve checkbox header cot Tu dong/Khong hao phi.
                FillDataToGridDepartment();
                FillDataToGridPatientType();
                ApplyExistingConfigOnLoad(); // Hien thi thiet lap da luu (neu co) khi reopen form.
                this.KeyPreview = true;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                isLoading = false;
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager(
                    "HIS.Desktop.Plugins.HisDepaPatientTypeList.Resources.Lang",
                    typeof(frmHisDepaPatientTypeList).Assembly);

                this.Text = GetLangValue("frmHisDepaPatientTypeList.Text");
                this.txtSearchDepartment.Properties.NullValuePrompt = GetLangValue("frmHisDepaPatientTypeList.txtSearchDepartment.NullValuePrompt");
                this.txtSearchPatientType.Properties.NullValuePrompt = GetLangValue("frmHisDepaPatientTypeList.txtSearchPatientType.NullValuePrompt");
                this.btnSearchDepartment.Text = GetLangValue("frmHisDepaPatientTypeList.btnSearchDepartment.Text");
                this.btnSearchPatientType.Text = GetLangValue("frmHisDepaPatientTypeList.btnSearchPatientType.Text");
                this.btnSave.Text = GetLangValue("frmHisDepaPatientTypeList.btnSave.Text");
                this.lciChooseMode.Text = GetLangValue("frmHisDepaPatientTypeList.lciChooseMode.Text");

                this.gcDepartmentCode.Caption = GetLangValue("frmHisDepaPatientTypeList.gcDepartmentCode.Caption");
                this.gcDepartmentName.Caption = GetLangValue("frmHisDepaPatientTypeList.gcDepartmentName.Caption");
                this.gcDepartmentAutoExpend.Caption = GetLangValue("frmHisDepaPatientTypeList.gcDepartmentAutoExpend.Caption");
                this.gcDepartmentNotExpend.Caption = GetLangValue("frmHisDepaPatientTypeList.gcDepartmentNotExpend.Caption");

                this.gcPatientTypeCode.Caption = GetLangValue("frmHisDepaPatientTypeList.gcPatientTypeCode.Caption");
                this.gcPatientTypeName.Caption = GetLangValue("frmHisDepaPatientTypeList.gcPatientTypeName.Caption");
                this.gcPatientTypeAutoExpend.Caption = GetLangValue("frmHisDepaPatientTypeList.gcPatientTypeAutoExpend.Caption");
                this.gcPatientTypeNotExpend.Caption = GetLangValue("frmHisDepaPatientTypeList.gcPatientTypeNotExpend.Caption");

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetLangValue(string key)
        {
            try
            {
                return Inventec.Common.Resource.Get.Value(
                    key,
                    Resources.ResourceLanguageManager.LanguageResource,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }

        private void SetDefaultValueMode()
        {
            try
            {
                selectionMode = SELECTION_MODE_DEPARTMENT;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboChooseMode()
        {
            try
            {
                string deptLabel = GetLangValue("frmHisDepaPatientTypeList.ChooseModeDepartment");
                string patientTypeLabel = GetLangValue("frmHisDepaPatientTypeList.ChooseModePatientType");
                if (string.IsNullOrWhiteSpace(deptLabel)) deptLabel = "Khoa";
                if (string.IsNullOrWhiteSpace(patientTypeLabel)) patientTypeLabel = "ĐTTT";

                List<object> selectionModeList = new List<object>
                {
                    new { MODE_NAME = deptLabel, MODE_CODE = SELECTION_MODE_DEPARTMENT },
                    new { MODE_NAME = patientTypeLabel, MODE_CODE = SELECTION_MODE_PATIENT_TYPE }
                };
                List<ColumnInfo> columnInfos = new List<ColumnInfo>
                {
                    new ColumnInfo("MODE_NAME", "", 100, 1)
                };
                ControlEditorADO controlEditorADO = new ControlEditorADO("MODE_NAME", "MODE_CODE", columnInfos, false, 100);
                ControlEditorLoader.Load(cboChooseMode, selectionModeList, controlEditorADO);
                cboChooseMode.EditValue = SELECTION_MODE_DEPARTMENT;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Fill grids
        private void FillDataToGridDepartment()
        {
            try
            {
                int pagingSize = ucPagingDepartment.pagingGrid != null
                    ? ucPagingDepartment.pagingGrid.PageSize
                    : (int)ConfigApplications.NumPageSize;
                LoadDepartment(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = deptRowCount;
                param.Count = deptDataTotal;
                ucPagingDepartment.Init(LoadDepartment, param, pagingSize, this.gridControlDepartment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridPatientType()
        {
            try
            {
                int pagingSize = ucPagingPatientType.pagingGrid != null
                    ? ucPagingPatientType.pagingGrid.PageSize
                    : (int)ConfigApplications.NumPageSize;
                LoadPatientType(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = patientTypeRowCount;
                param.Count = patientTypeDataTotal;
                ucPagingPatientType.Init(LoadPatientType, param, pagingSize, this.gridControlPatientType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

                var departments = new BackendAdapter(paramCommon).GetRO<List<HIS_DEPARTMENT>>(
                    "api/HisDepartment/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (departments != null && departments.Data != null && departments.Data.Count > 0)
                {
                    listDepartment = departments.Data.Select(d => new DepartmentADO(d)).ToList();
                    deptRowCount = listDepartment.Count;
                    deptDataTotal = (departments.Param == null ? 0 : departments.Param.Count ?? 0);
                }

                gridControlDepartment.BeginUpdate();
                gridControlDepartment.DataSource = listDepartment;
                gridControlDepartment.EndUpdate();
                LoadSelectedDepartments();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
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

                var patientTypes = new BackendAdapter(paramCommon).GetRO<List<HIS_PATIENT_TYPE>>(
                    "api/HisPatientType/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (patientTypes != null && patientTypes.Data != null && patientTypes.Data.Count > 0)
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
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Mode change + state
        private void cboChooseMode_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (isLoading) return; // Tranh fill grid trung khi Form_Load set EditValue lan dau.

                var raw = cboChooseMode.EditValue;
                selectionMode = raw == null ? SELECTION_MODE_DEPARTMENT : raw.ToString();

                // Reset trang thai chon de tranh stale data tu mode cu.
                selectedDepartments = new List<DepartmentADO>();
                selectedPatientTypes = new List<PatientTypeADO>();
                unSelectedDepartments = new List<DepartmentADO>();
                unSelectedPatientTypes = new List<PatientTypeADO>();
                isHeaderDeptChecked = false;
                isHeaderPatientTypeChecked = false;
                ResetExpendCheckAll();

                UpdateGridControlState();
                FillDataToGridDepartment();
                FillDataToGridPatientType();
                ApplyPreCheckForMode(); // Re-apply config khi quay lai mode da luu (no-op neu khac mode).
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Mode DEPARTMENT: radio bên Khoa + checkbox bên ĐTTT + 2 cột expend bên ĐTTT bật.
        /// Mode PATIENT_TYPE: radio bên ĐTTT + checkbox bên Khoa + 2 cột expend bên Khoa bật.
        /// </summary>
        private void UpdateGridControlState()
        {
            try
            {
                // Pattern chuan HIS team (HIS.UC.ExecuteRoom):
                //   Repository disable: CheckStyle = Style2 + ReadOnly = true (visual gach xam)
                //   Repository enable:  CheckStyle = Standard / Radio
                var styleStandard = DevExpress.XtraEditors.Controls.CheckStyles.Standard;
                var styleRadio = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
                var styleDisabled = DevExpress.XtraEditors.Controls.CheckStyles.Style2;

                if (selectionMode == SELECTION_MODE_DEPARTMENT)
                {
                    // === Ben Khoa: Radio ENABLE; Checkbox + 2 hao phi DISABLE ===
                    // Truy cap truc tiep cot — KHONG dung gridView.Columns[name] vi indexer lookup theo FieldName, khong phai Name → null.
                    gcDepartmentRadio.OptionsColumn.AllowEdit = true;
                    gcDepartmentCheckBox.OptionsColumn.AllowEdit = false;
                    gcDepartmentAutoExpend.OptionsColumn.AllowEdit = false;
                    gcDepartmentNotExpend.OptionsColumn.AllowEdit = false;
                    repoChkDepartmentRadio.CheckStyle = styleRadio; repoChkDepartmentRadio.ReadOnly = false;
                    repoChkDepartmentCheckBox.CheckStyle = styleDisabled; repoChkDepartmentCheckBox.ReadOnly = true;
                    repoChkDepartmentAutoExpend.CheckStyle = styleDisabled; repoChkDepartmentAutoExpend.ReadOnly = true;
                    repoChkDepartmentNotExpend.CheckStyle = styleDisabled; repoChkDepartmentNotExpend.ReadOnly = true;

                    // === Ben DTTT: Radio DISABLE; Checkbox + 2 hao phi ENABLE ===
                    gcPatientTypeRadio.OptionsColumn.AllowEdit = false;
                    gcPatientTypeCheckBox.OptionsColumn.AllowEdit = true;
                    gcPatientTypeAutoExpend.OptionsColumn.AllowEdit = true;
                    gcPatientTypeNotExpend.OptionsColumn.AllowEdit = true;
                    repoChkPatientTypeRadio.CheckStyle = styleRadio; repoChkPatientTypeRadio.ReadOnly = true;
                    repoChkPatientTypeCheckBox.CheckStyle = styleStandard; repoChkPatientTypeCheckBox.ReadOnly = false;
                    repoChkPatientTypeAutoExpend.CheckStyle = styleStandard; repoChkPatientTypeAutoExpend.ReadOnly = false;
                    repoChkPatientTypeNotExpend.CheckStyle = styleStandard; repoChkPatientTypeNotExpend.ReadOnly = false;

                }
                else
                {
                    // === Ben Khoa: Radio DISABLE; Checkbox + 2 hao phi ENABLE ===
                    gcDepartmentRadio.OptionsColumn.AllowEdit = false;
                    gcDepartmentCheckBox.OptionsColumn.AllowEdit = true;
                    gcDepartmentAutoExpend.OptionsColumn.AllowEdit = true;
                    gcDepartmentNotExpend.OptionsColumn.AllowEdit = true;
                    repoChkDepartmentRadio.CheckStyle = styleRadio; repoChkDepartmentRadio.ReadOnly = true;
                    repoChkDepartmentCheckBox.CheckStyle = styleStandard; repoChkDepartmentCheckBox.ReadOnly = false;
                    repoChkDepartmentAutoExpend.CheckStyle = styleStandard; repoChkDepartmentAutoExpend.ReadOnly = false;
                    repoChkDepartmentNotExpend.CheckStyle = styleStandard; repoChkDepartmentNotExpend.ReadOnly = false;

                    // === Ben DTTT: Radio ENABLE; Checkbox + 2 hao phi DISABLE ===
                    gcPatientTypeRadio.OptionsColumn.AllowEdit = true;
                    gcPatientTypeCheckBox.OptionsColumn.AllowEdit = false;
                    gcPatientTypeAutoExpend.OptionsColumn.AllowEdit = false;
                    gcPatientTypeNotExpend.OptionsColumn.AllowEdit = false;
                    repoChkPatientTypeRadio.CheckStyle = styleRadio; repoChkPatientTypeRadio.ReadOnly = false;
                    repoChkPatientTypeCheckBox.CheckStyle = styleDisabled; repoChkPatientTypeCheckBox.ReadOnly = true;
                    repoChkPatientTypeAutoExpend.CheckStyle = styleDisabled; repoChkPatientTypeAutoExpend.ReadOnly = true;
                    repoChkPatientTypeNotExpend.CheckStyle = styleDisabled; repoChkPatientTypeNotExpend.ReadOnly = true;

                }

                gridControlDepartment.RefreshDataSource();
                gridControlPatientType.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Header checkbox custom draw + click
        private void gridViewDepartment_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column != null && e.Column.FieldName == gcDepartmentCheckBox.FieldName)
                {
                    DrawHeaderCheckBox(e, repoChkDepartmentCheckBox, isHeaderDeptChecked);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPatientType_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column != null && e.Column.FieldName == gcPatientTypeCheckBox.FieldName)
                {
                    DrawHeaderCheckBox(e, repoChkPatientTypeCheckBox, isHeaderPatientTypeChecked);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DrawHeaderCheckBox(DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e, RepositoryItemCheckEdit checkEdit, bool isChecked)
        {
            e.Info.InnerElements.Clear();
            e.Painter.DrawObject(e.Info);
            if (checkEdit != null)
            {
                int size = 16;
                int x = e.Bounds.X + (e.Bounds.Width - size) / 2;
                int y = e.Bounds.Y + (e.Bounds.Height - size) / 2;
                Rectangle rect = new Rectangle(x, y, size, size);

                var info = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.CreateViewInfo();
                var painter = (DevExpress.XtraEditors.Drawing.CheckEditPainter)checkEdit.CreatePainter();
                info.EditValue = isChecked;
                info.Bounds = rect;
                info.CalcViewInfo(e.Graphics);

                using (DevExpress.Utils.Drawing.GraphicsCache cache = new DevExpress.Utils.Drawing.GraphicsCache(e.Graphics))
                {
                    painter.Draw(new DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs(info, cache, rect));
                }
            }
            e.Handled = true;
        }

        private void gridViewDepartment_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view == null) return;
                GridHitInfo info = view.CalcHitInfo(e.Location);
                if (info.InColumn && info.Column.FieldName == gcDepartmentCheckBox.FieldName && selectionMode == SELECTION_MODE_PATIENT_TYPE)
                {
                    isHeaderDeptChecked = !isHeaderDeptChecked;

                    // Set thang ADO roi RefreshDataSource — KHONG dung view.Columns[Name] vi indexer tra theo FieldName -> null.
                    var dataSource = gridControlDepartment.DataSource as List<DepartmentADO>;
                    if (dataSource != null)
                    {
                        foreach (var item in dataSource) item.IsCheckBoxChecked = isHeaderDeptChecked;
                        selectedDepartments = dataSource.Where(d => d.IsCheckBoxChecked).ToList();
                        unSelectedDepartments = dataSource.Where(d => !d.IsCheckBoxChecked).ToList();
                        gridControlDepartment.RefreshDataSource();
                    }
                    view.InvalidateColumnHeader(gcDepartmentCheckBox);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPatientType_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view == null) return;
                GridHitInfo info = view.CalcHitInfo(e.Location);
                if (info.InColumn && info.Column.FieldName == gcPatientTypeCheckBox.FieldName && selectionMode == SELECTION_MODE_DEPARTMENT)
                {
                    isHeaderPatientTypeChecked = !isHeaderPatientTypeChecked;

                    // Set thang ADO roi RefreshDataSource — KHONG dung view.Columns[Name] vi indexer tra theo FieldName -> null.
                    var dataSource = gridControlPatientType.DataSource as List<PatientTypeADO>;
                    if (dataSource != null)
                    {
                        foreach (var item in dataSource) item.IsCheckBoxChecked = isHeaderPatientTypeChecked;
                        selectedPatientTypes = dataSource.Where(p => p.IsCheckBoxChecked).ToList();
                        unSelectedPatientTypes = dataSource.Where(p => !p.IsCheckBoxChecked).ToList();
                        gridControlPatientType.RefreshDataSource();
                    }
                    view.InvalidateColumnHeader(gcPatientTypeCheckBox);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Tich tat ca Tu dong/Khong hao phi tren header column

        private readonly System.Collections.Generic.Dictionary<DevExpress.XtraGrid.Columns.GridColumn, bool> headerCheckState
            = new System.Collections.Generic.Dictionary<DevExpress.XtraGrid.Columns.GridColumn, bool>();

        private void HookHeaderCheckEvents()
        {
            try
            {
                this.gridViewDepartment.CustomDrawColumnHeader += this.gridView_CustomDrawColumnHeader_ExpendAll;
                this.gridViewDepartment.MouseDown += this.gridViewDepartment_MouseDown_ExpendAll;
                this.gridViewPatientType.CustomDrawColumnHeader += this.gridView_CustomDrawColumnHeader_ExpendAll;
                this.gridViewPatientType.MouseDown += this.gridViewPatientType_MouseDown_ExpendAll;
                // Header tu gian chieu cao de caption "Tu dong hao phi"/"Khong hao phi" wrap du khi co checkbox.
                this.gridViewDepartment.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.True;
                this.gridViewPatientType.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.True;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private System.Drawing.Rectangle GetHeaderCheckBoxRect(System.Drawing.Rectangle headerBounds)
        {
            int size = 14;
            int left = headerBounds.Left + 4;
            int top = headerBounds.Top + (headerBounds.Height - size) / 2;
            return new System.Drawing.Rectangle(left, top, size, size);
        }

        private void gridView_CustomDrawColumnHeader_ExpendAll(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column == null) return;
                if (e.Column != this.gcDepartmentAutoExpend && e.Column != this.gcDepartmentNotExpend
                    && e.Column != this.gcPatientTypeAutoExpend && e.Column != this.gcPatientTypeNotExpend)
                    return;

                // Tam xoa caption goc khi ve nen header — vi caption KHONG nam trong InnerElements,
                // painter ve caption theo e.Info.Caption. Neu khong blank se bi caption goc + caption tu ve chong nhau.
                string captionBak = e.Info.Caption;
                e.Info.Caption = string.Empty;
                e.Info.InnerElements.Clear();
                e.Painter.DrawObject(e.Info);
                e.Info.Caption = captionBak;

                var cbRect = GetHeaderCheckBoxRect(e.Bounds);
                bool isChecked;
                if (!this.headerCheckState.TryGetValue(e.Column, out isChecked)) isChecked = false;

                // Ve checkbox theo skin DevExpress (dung repository cua cot) cho dong bo voi o trong luoi.
                RepositoryItemCheckEdit chkRepo = e.Column.ColumnEdit as RepositoryItemCheckEdit;
                if (chkRepo != null)
                {
                    var vi = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)chkRepo.CreateViewInfo();
                    var pt = (DevExpress.XtraEditors.Drawing.CheckEditPainter)chkRepo.CreatePainter();
                    vi.EditValue = isChecked;
                    vi.Bounds = cbRect;
                    vi.CalcViewInfo(e.Graphics);
                    using (DevExpress.Utils.Drawing.GraphicsCache cache = new DevExpress.Utils.Drawing.GraphicsCache(e.Graphics))
                    {
                        pt.Draw(new DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs(vi, cache, cbRect));
                    }
                }
                else
                {
                    System.Windows.Forms.ControlPaint.DrawCheckBox(e.Graphics, cbRect,
                        isChecked ? System.Windows.Forms.ButtonState.Checked : System.Windows.Forms.ButtonState.Normal);
                }

                var textRect = new System.Drawing.Rectangle(
                    cbRect.Right + 4,
                    e.Bounds.Top,
                    e.Bounds.Width - cbRect.Width - 8,
                    e.Bounds.Height);
                System.Windows.Forms.TextRenderer.DrawText(
                    e.Graphics,
                    e.Column.Caption ?? "",
                    e.Appearance.Font,
                    textRect,
                    e.Appearance.ForeColor,
                    System.Windows.Forms.TextFormatFlags.VerticalCenter | System.Windows.Forms.TextFormatFlags.Left | System.Windows.Forms.TextFormatFlags.WordBreak);

                e.Handled = true;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void gridViewDepartment_MouseDown_ExpendAll(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                var view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
                var hit = view.CalcHitInfo(e.Location);
                if (!hit.InColumn) return;
                if (hit.Column != this.gcDepartmentAutoExpend && hit.Column != this.gcDepartmentNotExpend) return;
                if (selectionMode != SELECTION_MODE_PATIENT_TYPE) return; // ben Khoa chi enable o mode DTTT
                if (isUpdatingExpendCheckAll) return;

                isUpdatingExpendCheckAll = true;
                try
                {
                    bool current;
                    if (!this.headerCheckState.TryGetValue(hit.Column, out current)) current = false;
                    bool newState = !current;
                    this.headerCheckState[hit.Column] = newState;

                    bool isAuto = (hit.Column == this.gcDepartmentAutoExpend);
                    ApplyExpendCheckAllDept(isAuto, newState);

                    if (newState)
                    {
                        var otherCol = isAuto ? this.gcDepartmentNotExpend : this.gcDepartmentAutoExpend;
                        this.headerCheckState[otherCol] = false;
                    }

                    view.InvalidateColumnHeader(this.gcDepartmentAutoExpend);
                    view.InvalidateColumnHeader(this.gcDepartmentNotExpend);
                }
                finally { isUpdatingExpendCheckAll = false; }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void gridViewPatientType_MouseDown_ExpendAll(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                var view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
                var hit = view.CalcHitInfo(e.Location);
                if (!hit.InColumn) return;
                if (hit.Column != this.gcPatientTypeAutoExpend && hit.Column != this.gcPatientTypeNotExpend) return;
                if (selectionMode != SELECTION_MODE_DEPARTMENT) return; // ben DTTT chi enable o mode Khoa
                if (isUpdatingExpendCheckAll) return;

                isUpdatingExpendCheckAll = true;
                try
                {
                    bool current;
                    if (!this.headerCheckState.TryGetValue(hit.Column, out current)) current = false;
                    bool newState = !current;
                    this.headerCheckState[hit.Column] = newState;

                    bool isAuto = (hit.Column == this.gcPatientTypeAutoExpend);
                    ApplyExpendCheckAllPatientType(isAuto, newState);

                    if (newState)
                    {
                        var otherCol = isAuto ? this.gcPatientTypeNotExpend : this.gcPatientTypeAutoExpend;
                        this.headerCheckState[otherCol] = false;
                    }

                    view.InvalidateColumnHeader(this.gcPatientTypeAutoExpend);
                    view.InvalidateColumnHeader(this.gcPatientTypeNotExpend);
                }
                finally { isUpdatingExpendCheckAll = false; }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Set IS_AUTO_EXPEND/IS_NOT_EXPEND cho TAT CA dong khoa tren trang hien tai (co mutex).</summary>
        private void ApplyExpendCheckAllDept(bool isAuto, bool isChecked)
        {
            var dataSource = gridControlDepartment.DataSource as List<DepartmentADO>;
            if (dataSource == null) return;
            foreach (var item in dataSource)
            {
                if (isAuto)
                {
                    item.IsAutoExpend = isChecked;
                    if (isChecked) item.IsNotExpend = false;
                }
                else
                {
                    item.IsNotExpend = isChecked;
                    if (isChecked) item.IsAutoExpend = false;
                }
            }
            gridControlDepartment.RefreshDataSource();
        }

        /// <summary>Set IS_AUTO_EXPEND/IS_NOT_EXPEND cho TAT CA dong DTTT tren trang hien tai (co mutex).</summary>
        private void ApplyExpendCheckAllPatientType(bool isAuto, bool isChecked)
        {
            var dataSource = gridControlPatientType.DataSource as List<PatientTypeADO>;
            if (dataSource == null) return;
            foreach (var item in dataSource)
            {
                if (isAuto)
                {
                    item.IsAutoExpend = isChecked;
                    if (isChecked) item.IsNotExpend = false;
                }
                else
                {
                    item.IsNotExpend = isChecked;
                    if (isChecked) item.IsAutoExpend = false;
                }
            }
            gridControlPatientType.RefreshDataSource();
        }

        /// <summary>Bo tich tat ca header check tren 4 cot Tu dong/Khong hao phi (khi doi mode).</summary>
        private void ResetExpendCheckAll()
        {
            try
            {
                isUpdatingExpendCheckAll = true;
                this.headerCheckState[this.gcDepartmentAutoExpend] = false;
                this.headerCheckState[this.gcDepartmentNotExpend] = false;
                this.headerCheckState[this.gcPatientTypeAutoExpend] = false;
                this.headerCheckState[this.gcPatientTypeNotExpend] = false;
                this.gridViewDepartment.InvalidateColumnHeader(this.gcDepartmentAutoExpend);
                this.gridViewDepartment.InvalidateColumnHeader(this.gcDepartmentNotExpend);
                this.gridViewPatientType.InvalidateColumnHeader(this.gcPatientTypeAutoExpend);
                this.gridViewPatientType.InvalidateColumnHeader(this.gcPatientTypeNotExpend);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            finally { isUpdatingExpendCheckAll = false; }
        }
        #endregion

        #region ShowingEditor — disable cell mutex (Tu dong hao phi / Khong hao phi)
        /// <summary>
        /// Disable per-cell theo rule mutex:
        /// - Cell "Tu dong hao phi" disable neu IsNotExpend = true (vi cai kia da check).
        /// - Cell "Khong hao phi" disable neu IsAutoExpend = true.
        /// Goi truoc khi DevExpress show editor → cancel = true thi cell khong vao edit mode.
        /// </summary>
        private void gridViewDepartment_ShowingEditor(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (gridViewDepartment.FocusedColumn == null) return;
                string colName = gridViewDepartment.FocusedColumn.FieldName;

                // Block click cot disable theo mode.
                if (selectionMode == SELECTION_MODE_DEPARTMENT)
                {
                    if (colName == gcDepartmentCheckBox.FieldName
                        || colName == gcDepartmentAutoExpend.FieldName
                        || colName == gcDepartmentNotExpend.FieldName)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else
                {
                    if (colName == gcDepartmentRadio.FieldName)
                    {
                        e.Cancel = true;
                        return;
                    }
                }

                // Mutex per-row: tick Auto roi → khong tick Not duoc. Va nguoc lai.
                var data = gridViewDepartment.GetFocusedRow() as DepartmentADO;
                if (data == null) return;
                if (colName == gcDepartmentAutoExpend.FieldName && data.IsNotExpend)
                    e.Cancel = true;
                else if (colName == gcDepartmentNotExpend.FieldName && data.IsAutoExpend)
                    e.Cancel = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPatientType_ShowingEditor(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (gridViewPatientType.FocusedColumn == null) return;
                string colName = gridViewPatientType.FocusedColumn.FieldName;

                // Block click cot disable theo mode.
                if (selectionMode == SELECTION_MODE_DEPARTMENT)
                {
                    if (colName == gcPatientTypeRadio.FieldName)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else
                {
                    if (colName == gcPatientTypeCheckBox.FieldName
                        || colName == gcPatientTypeAutoExpend.FieldName
                        || colName == gcPatientTypeNotExpend.FieldName)
                    {
                        e.Cancel = true;
                        return;
                    }
                }

                // Mutex per-row.
                var data = gridViewPatientType.GetFocusedRow() as PatientTypeADO;
                if (data == null) return;
                if (colName == gcPatientTypeAutoExpend.FieldName && data.IsNotExpend)
                    e.Cancel = true;
                else if (colName == gcPatientTypeNotExpend.FieldName && data.IsAutoExpend)
                    e.Cancel = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region CustomUnboundColumnData — đọc/ghi giá trị unbound
        private void gridViewDepartment_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = ((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex] as DepartmentADO;
                    if (data == null) return;

                    // Mode DEPARTMENT: ben Khoa Radio enable; Checkbox + 2 hao phi disable -> tra null de render gray dashes.
                    // Mode PATIENT_TYPE: ben Khoa Radio disable -> tra null.
                    bool isModeDepartment = selectionMode == SELECTION_MODE_DEPARTMENT;

                    if (e.Column.FieldName == gcDepartmentRadio.FieldName)
                        e.Value = isModeDepartment ? (object)data.IsRadioChecked : null;
                    else if (e.Column.FieldName == gcDepartmentCheckBox.FieldName)
                        e.Value = isModeDepartment ? (object)null : (object)data.IsCheckBoxChecked;
                    else if (e.Column.FieldName == gcDepartmentAutoExpend.FieldName)
                        e.Value = isModeDepartment ? (object)null : (object)data.IsAutoExpend;
                    else if (e.Column.FieldName == gcDepartmentNotExpend.FieldName)
                        e.Value = isModeDepartment ? (object)null : (object)data.IsNotExpend;
                }
                else if (e.IsSetData)
                {
                    var data = ((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex] as DepartmentADO;
                    if (data == null) return;

                    if (e.Column.FieldName == gcDepartmentRadio.FieldName && (bool)e.Value)
                    {
                        var dataSource = gridControlDepartment.DataSource as List<DepartmentADO>;
                        if (dataSource != null)
                        {
                            foreach (var item in dataSource) item.IsRadioChecked = false;
                            data.IsRadioChecked = true;
                            selectedDepartments = new List<DepartmentADO> { data };
                            gridControlDepartment.RefreshDataSource();
                        }
                    }
                    else if (e.Column.FieldName == gcDepartmentCheckBox.FieldName)
                    {
                        data.IsCheckBoxChecked = (bool)e.Value;
                        var dataSource = gridControlDepartment.DataSource as List<DepartmentADO>;
                        if (dataSource != null)
                        {
                            selectedDepartments = dataSource.Where(d => d.IsCheckBoxChecked).ToList();
                            unSelectedDepartments = dataSource.Where(d => !d.IsCheckBoxChecked).ToList();
                        }
                    }
                    else if (e.Column.FieldName == gcDepartmentAutoExpend.FieldName)
                    {
                        // ShowingEditor da chan click khi mutex → vao day chi khi cell con enable.
                        data.IsAutoExpend = (bool)e.Value;
                        // Refresh row de cell con lai cap nhat enabled/disabled state qua ShowingEditor.
                        int rh = gridViewDepartment.GetRowHandle(e.ListSourceRowIndex);
                        if (rh != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
                            gridViewDepartment.RefreshRow(rh);
                    }
                    else if (e.Column.FieldName == gcDepartmentNotExpend.FieldName)
                    {
                        data.IsNotExpend = (bool)e.Value;
                        int rh = gridViewDepartment.GetRowHandle(e.ListSourceRowIndex);
                        if (rh != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
                            gridViewDepartment.RefreshRow(rh);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPatientType_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = ((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex] as PatientTypeADO;
                    if (data == null) return;

                    // Mode DEPARTMENT: ben DTTT Radio disable -> null; Checkbox+hao phi enable -> bool.
                    // Mode PATIENT_TYPE: ben DTTT Radio enable -> bool; Checkbox+hao phi disable -> null.
                    bool isModeDepartment = selectionMode == SELECTION_MODE_DEPARTMENT;

                    if (e.Column.FieldName == gcPatientTypeRadio.FieldName)
                        e.Value = isModeDepartment ? (object)null : (object)data.IsRadioChecked;
                    else if (e.Column.FieldName == gcPatientTypeCheckBox.FieldName)
                        e.Value = isModeDepartment ? (object)data.IsCheckBoxChecked : null;
                    else if (e.Column.FieldName == gcPatientTypeAutoExpend.FieldName)
                        e.Value = isModeDepartment ? (object)data.IsAutoExpend : null;
                    else if (e.Column.FieldName == gcPatientTypeNotExpend.FieldName)
                        e.Value = isModeDepartment ? (object)data.IsNotExpend : null;
                }
                else if (e.IsSetData)
                {
                    var data = ((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex] as PatientTypeADO;
                    if (data == null) return;

                    if (e.Column.FieldName == gcPatientTypeRadio.FieldName && (bool)e.Value)
                    {
                        var dataSource = gridControlPatientType.DataSource as List<PatientTypeADO>;
                        if (dataSource != null)
                        {
                            foreach (var item in dataSource) item.IsRadioChecked = false;
                            data.IsRadioChecked = true;
                            selectedPatientTypes = new List<PatientTypeADO> { data };
                            gridControlPatientType.RefreshDataSource();
                        }
                    }
                    else if (e.Column.FieldName == gcPatientTypeCheckBox.FieldName)
                    {
                        data.IsCheckBoxChecked = (bool)e.Value;
                        var dataSource = gridControlPatientType.DataSource as List<PatientTypeADO>;
                        if (dataSource != null)
                        {
                            selectedPatientTypes = dataSource.Where(p => p.IsCheckBoxChecked).ToList();
                            unSelectedPatientTypes = dataSource.Where(p => !p.IsCheckBoxChecked).ToList();
                        }
                    }
                    else if (e.Column.FieldName == gcPatientTypeAutoExpend.FieldName)
                    {
                        data.IsAutoExpend = (bool)e.Value;
                        int rh = gridViewPatientType.GetRowHandle(e.ListSourceRowIndex);
                        if (rh != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
                            gridViewPatientType.RefreshRow(rh);
                    }
                    else if (e.Column.FieldName == gcPatientTypeNotExpend.FieldName)
                    {
                        data.IsNotExpend = (bool)e.Value;
                        int rh = gridViewPatientType.GetRowHandle(e.ListSourceRowIndex);
                        if (rh != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
                            gridViewPatientType.RefreshRow(rh);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region CellValueChanging — đồng bộ trạng thái sau khi click
        private void gridViewDepartment_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == gcDepartmentRadio.FieldName && selectionMode == SELECTION_MODE_DEPARTMENT)
                {
                    var department = (DepartmentADO)gridViewDepartment.GetRow(e.RowHandle);
                    if (department == null) return;
                    if (department.IsRadioChecked)
                    {
                        LoadSelectedPatientTypes();
                        repoChkDepartmentRadio.ReadOnly = true;
                        return;
                    }
                    var dataSource = gridControlDepartment.DataSource as List<DepartmentADO>;
                    if (dataSource != null)
                    {
                        foreach (var item in dataSource) item.IsRadioChecked = false;
                        department.IsRadioChecked = true;
                        selectedDepartments = new List<DepartmentADO> { department };
                        gridControlDepartment.RefreshDataSource();
                        LoadSelectedPatientTypes();
                    }
                }
                else if (e.Column.FieldName == gcDepartmentCheckBox.FieldName && selectionMode == SELECTION_MODE_PATIENT_TYPE)
                {
                    var department = (DepartmentADO)gridViewDepartment.GetRow(e.RowHandle);
                    if (department == null) return;
                    department.IsCheckBoxChecked = !department.IsCheckBoxChecked;
                    var dataSource = gridControlDepartment.DataSource as List<DepartmentADO>;
                    if (dataSource != null)
                    {
                        selectedDepartments = dataSource.Where(d => d.IsCheckBoxChecked).ToList();
                        gridControlDepartment.RefreshDataSource();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                repoChkDepartmentRadio.ReadOnly = false;
            }
        }

        private void gridViewPatientType_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == gcPatientTypeRadio.FieldName && selectionMode == SELECTION_MODE_PATIENT_TYPE)
                {
                    var patientType = (PatientTypeADO)gridViewPatientType.GetRow(e.RowHandle);
                    if (patientType == null) return;
                    if (patientType.IsRadioChecked)
                    {
                        LoadSelectedDepartments();
                        repoChkPatientTypeRadio.ReadOnly = true;
                        return;
                    }
                    var dataSource = gridControlPatientType.DataSource as List<PatientTypeADO>;
                    if (dataSource != null)
                    {
                        foreach (var item in dataSource) item.IsRadioChecked = false;
                        patientType.IsRadioChecked = true;
                        selectedPatientTypes = new List<PatientTypeADO> { patientType };
                        gridControlPatientType.RefreshDataSource();
                        LoadSelectedDepartments();
                    }
                }
                else if (e.Column.FieldName == gcPatientTypeCheckBox.FieldName && selectionMode == SELECTION_MODE_DEPARTMENT)
                {
                    var patientType = (PatientTypeADO)gridViewPatientType.GetRow(e.RowHandle);
                    if (patientType == null) return;
                    patientType.IsCheckBoxChecked = !patientType.IsCheckBoxChecked;
                    var dataSource = gridControlPatientType.DataSource as List<PatientTypeADO>;
                    if (dataSource != null)
                    {
                        selectedPatientTypes = dataSource.Where(p => p.IsCheckBoxChecked).ToList();
                        gridControlPatientType.RefreshDataSource();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                repoChkPatientTypeRadio.ReadOnly = false;
            }
        }
        #endregion

        #region Load lại trạng thái đã chọn khi đổi radio
        private void LoadSelectedDepartments()
        {
            try
            {
                if (selectionMode != SELECTION_MODE_PATIENT_TYPE) return;
                var selectedPatientType = selectedPatientTypes.FirstOrDefault(p => p.IsRadioChecked);
                if (selectedPatientType == null) return;

                EnsureDepaPatientTypeFromDb();

                // Build dictionary tu HIS_DEPA_PATIENT_TYPE theo DEPARTMENT_ID de tra cuu O(1).
                var dptDict = depaPatientTypes
                    .Where(dpt => dpt.PATIENT_TYPE_ID == selectedPatientType.ID)
                    .GroupBy(dpt => dpt.DEPARTMENT_ID)
                    .ToDictionary(g => g.Key, g => g.First());

                gridViewDepartment.ClearSelection();
                var dataSource = gridControlDepartment.DataSource as List<DepartmentADO>;
                if (dataSource == null) return;

                // Reset state truoc khi apply.
                foreach (var dept in dataSource)
                {
                    dept.IsCheckBoxChecked = false;
                    dept.IsAutoExpend = false;
                    dept.IsNotExpend = false;
                }

                // QUAN TRONG: selectedDepartments dung CHINH reference tu dataSource (khong tao ado moi).
                // Ly do: khi user thay doi co IS_AUTO_EXPEND/IS_NOT_EXPEND tren grid (qua CustomUnboundColumnData),
                // dataSource[i] reflect ngay → selectedDepartments[i] cung reference reflect → Save lay co dung.
                selectedDepartments = new List<DepartmentADO>();
                for (int i = 0; i < dataSource.Count; i++)
                {
                    HIS_DEPA_PATIENT_TYPE dpt;
                    if (dptDict.TryGetValue(dataSource[i].ID, out dpt))
                    {
                        dataSource[i].IsCheckBoxChecked = true;
                        ApplyExpendFromDb(dataSource[i], dpt);
                        selectedDepartments.Add(dataSource[i]); // SHARED reference
                        int rowHandle = gridViewDepartment.GetRowHandle(i);
                        if (rowHandle != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
                            gridViewDepartment.SelectRow(rowHandle);
                    }
                }
                gridControlDepartment.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadSelectedPatientTypes()
        {
            try
            {
                if (selectionMode != SELECTION_MODE_DEPARTMENT) return;
                var selectedDepartment = selectedDepartments.FirstOrDefault(d => d.IsRadioChecked);
                if (selectedDepartment == null) return;

                EnsureDepaPatientTypeFromDb();

                var dptDict = depaPatientTypes
                    .Where(dpt => dpt.DEPARTMENT_ID == selectedDepartment.ID)
                    .GroupBy(dpt => dpt.PATIENT_TYPE_ID)
                    .ToDictionary(g => g.Key, g => g.First());

                gridViewPatientType.ClearSelection();
                var dataSource = gridControlPatientType.DataSource as List<PatientTypeADO>;
                if (dataSource == null) return;

                foreach (var pt in dataSource)
                {
                    pt.IsCheckBoxChecked = false;
                    pt.IsAutoExpend = false;
                    pt.IsNotExpend = false;
                }

                // selectedPatientTypes dung SHARED reference voi dataSource — xem comment LoadSelectedDepartments.
                selectedPatientTypes = new List<PatientTypeADO>();
                for (int i = 0; i < dataSource.Count; i++)
                {
                    HIS_DEPA_PATIENT_TYPE dpt;
                    if (dptDict.TryGetValue(dataSource[i].ID, out dpt))
                    {
                        dataSource[i].IsCheckBoxChecked = true;
                        ApplyExpendFromDb(dataSource[i], dpt);
                        selectedPatientTypes.Add(dataSource[i]); // SHARED reference
                        int rowHandle = gridViewPatientType.GetRowHandle(i);
                        if (rowHandle != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
                            gridViewPatientType.SelectRow(rowHandle);
                    }
                }
                gridControlPatientType.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Nạp danh sách đã lưu DB theo serviceId nếu có — chỉ chạy 1 lần (isCalledApi=true).
        /// </summary>
        /// <summary>
        /// Khi mo lai form cho mot service da co thiet lap: load tu cache → detect mode →
        /// pre-check radio + checkbox + 2 co hao phi tren grid hien tai.
        /// </summary>
        private void ApplyExistingConfigOnLoad()
        {
            try
            {
                // Buoc 1: Dam bao depaPatientTypes co data (load tu cache neu trong).
                EnsureDepaPatientTypeFromDb();
                if (depaPatientTypes == null || depaPatientTypes.Count == 0) return;

                // Buoc 2: Detect mode + primary id dua tren count cua group.
                // Mode DEPARTMENT: 1 khoa - nhieu DTTT → group theo DEPARTMENT_ID, nhom dong nhat la khoa duoc chon.
                // Mode PATIENT_TYPE: 1 DTTT - nhieu khoa → group theo PATIENT_TYPE_ID, nhom dong nhat la DTTT duoc chon.
                var biggestByDept = depaPatientTypes
                    .Where(o => o.DEPARTMENT_ID > 0)
                    .GroupBy(o => o.DEPARTMENT_ID)
                    .OrderByDescending(g => g.Count())
                    .FirstOrDefault();
                var biggestByPt = depaPatientTypes
                    .Where(o => o.PATIENT_TYPE_ID > 0)
                    .GroupBy(o => o.PATIENT_TYPE_ID)
                    .OrderByDescending(g => g.Count())
                    .FirstOrDefault();

                int deptGroupSize = biggestByDept != null ? biggestByDept.Count() : 0;
                int ptGroupSize = biggestByPt != null ? biggestByPt.Count() : 0;

                if (deptGroupSize == 0 && ptGroupSize == 0) return;

                if (deptGroupSize >= ptGroupSize)
                {
                    detectedConfigMode = SELECTION_MODE_DEPARTMENT;
                }
                else
                {
                    detectedConfigMode = SELECTION_MODE_PATIENT_TYPE;
                }

                // Buoc 3: Set mode + cap nhat trang thai grid (chan EditValueChanged fill grid lai).
                bool prevLoading = isLoading;
                isLoading = true;
                selectionMode = detectedConfigMode;
                cboChooseMode.EditValue = selectionMode;
                isLoading = prevLoading;
                UpdateGridControlState();

                // Buoc 4: Pre-check theo mode hien tai.
                ApplyPreCheckForMode();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Pre-check radio + LoadSelected* dua tren selectionMode hien tai.
        /// Chi apply khi mode hien tai trung voi detectedConfigMode (mode khi data duoc luu).
        /// </summary>
        private void ApplyPreCheckForMode()
        {
            try
            {
                if (depaPatientTypes == null || depaPatientTypes.Count == 0) return;
                if (string.IsNullOrEmpty(detectedConfigMode) || selectionMode != detectedConfigMode) return;

                if (selectionMode == SELECTION_MODE_DEPARTMENT)
                {
                    var biggestByDept = depaPatientTypes
                        .Where(o => o.DEPARTMENT_ID > 0)
                        .GroupBy(o => o.DEPARTMENT_ID)
                        .OrderByDescending(g => g.Count())
                        .FirstOrDefault();
                    if (biggestByDept == null) return;
                    long primaryDeptId = biggestByDept.Key ?? 0;

                    var deptDataSource = gridControlDepartment.DataSource as List<DepartmentADO>;
                    if (deptDataSource == null) return;

                    var primaryDept = deptDataSource.FirstOrDefault(d => d.ID == primaryDeptId);
                    if (primaryDept == null) return; // Khoa khong nam tren trang hien tai.

                    foreach (var item in deptDataSource) item.IsRadioChecked = false;
                    primaryDept.IsRadioChecked = true;
                    selectedDepartments = new List<DepartmentADO> { primaryDept };
                    gridControlDepartment.RefreshDataSource();
                    LoadSelectedPatientTypes();
                }
                else
                {
                    var biggestByPt = depaPatientTypes
                        .Where(o => o.PATIENT_TYPE_ID > 0)
                        .GroupBy(o => o.PATIENT_TYPE_ID)
                        .OrderByDescending(g => g.Count())
                        .FirstOrDefault();
                    if (biggestByPt == null) return;
                    long primaryPtId = biggestByPt.Key ?? 0;

                    var ptDataSource = gridControlPatientType.DataSource as List<PatientTypeADO>;
                    if (ptDataSource == null) return;

                    var primaryPt = ptDataSource.FirstOrDefault(p => p.ID == primaryPtId);
                    if (primaryPt == null) return;

                    foreach (var item in ptDataSource) item.IsRadioChecked = false;
                    primaryPt.IsRadioChecked = true;
                    selectedPatientTypes = new List<PatientTypeADO> { primaryPt };
                    gridControlPatientType.RefreshDataSource();
                    LoadSelectedDepartments();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnsureDepaPatientTypeFromDb()
        {
            try
            {
                if (!this.serviceId.HasValue || this.isCalledApi) return;

                // Goi API truc tiep — KHONG dung BackendDataWorker.Get cho junction table nay
                // vi cache co the stale sau khi MaterialTypeCreate/MedicineTypeCreate Save.
                var filter = new HisDepaPatientTypeFilter();
                filter.SERVICE_ID = this.serviceId.Value;
                CommonParam apiParam = new CommonParam();
                var fromDb = new BackendAdapter(apiParam).Get<List<HIS_DEPA_PATIENT_TYPE>>(
                    "api/HisDepaPatientType/Get", ApiConsumers.MosConsumer, filter, apiParam);
                if (fromDb == null) return;

                var existKey = new HashSet<string>(depaPatientTypes
                    .Select(o => string.Format("{0}_{1}_{2}", o.DEPARTMENT_ID, o.PATIENT_TYPE_ID, o.SERVICE_ID)));

                foreach (var item in fromDb)
                {
                    string key = string.Format("{0}_{1}_{2}", item.DEPARTMENT_ID, item.PATIENT_TYPE_ID, item.SERVICE_ID);
                    if (!existKey.Contains(key))
                    {
                        // Reset ID = 0 — vi caller (MaterialTypeCreate/MedicineTypeCreate) trong Edit mode
                        // se DELETE old records (theo ID lay tu API) roi CREATE list nay. Neu giu ID cu
                        // backend CreateList co the conflict/reject silently → DB sach, ko co record moi.
                        item.ID = 0;
                        depaPatientTypes.Add(item);
                        existKey.Add(key);
                    }
                }
                this.isCalledApi = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Đọc IS_AUTO_EXPEND / IS_NOT_EXPEND từ HIS_DEPA_PATIENT_TYPE (kiểu long?) qua reflection
        /// vì EFMODEL có thể chưa có 2 property này khi backend chưa update.
        /// </summary>
        private void ApplyExpendFromDb(DepartmentADO ado, HIS_DEPA_PATIENT_TYPE dpt)
        {
            ado.IsAutoExpend = ReadLongFlag(dpt, "IS_AUTO_EXPEND") == 1;
            ado.IsNotExpend = ReadLongFlag(dpt, "IS_NOT_EXPEND") == 1;
        }

        private void ApplyExpendFromDb(PatientTypeADO ado, HIS_DEPA_PATIENT_TYPE dpt)
        {
            ado.IsAutoExpend = ReadLongFlag(dpt, "IS_AUTO_EXPEND") == 1;
            ado.IsNotExpend = ReadLongFlag(dpt, "IS_NOT_EXPEND") == 1;
        }

        private static long ReadLongFlag(object obj, string propName)
        {
            try
            {
                if (obj == null) return 0;
                var prop = obj.GetType().GetProperty(propName);
                if (prop == null) return 0;
                var val = prop.GetValue(obj, null);
                if (val == null) return 0;
                if (val is long) return (long)val;
                long parsed;
                if (long.TryParse(val.ToString(), out parsed)) return parsed;
                return 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return 0;
            }
        }

        private static void WriteLongFlag(object obj, string propName, long value)
        {
            try
            {
                if (obj == null) return;
                var prop = obj.GetType().GetProperty(propName);
                if (prop == null || !prop.CanWrite) return;

                if (prop.PropertyType == typeof(long))
                    prop.SetValue(obj, value, null);
                else if (prop.PropertyType == typeof(long?))
                    prop.SetValue(obj, (long?)value, null);
                else if (prop.PropertyType == typeof(short) || prop.PropertyType == typeof(short?))
                    prop.SetValue(obj, prop.PropertyType == typeof(short) ? (object)(short)value : (object)(short?)value, null);
                else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
                    prop.SetValue(obj, prop.PropertyType == typeof(int) ? (object)(int)value : (object)(int?)value, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Save (chọn)
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // KHONG set isClickPick = true tai day — phai sau khi validate pass va build xong danh sach.
                WaitingManager.Show();
                List<HIS_DEPA_PATIENT_TYPE> depaPatientTypeItems = new List<HIS_DEPA_PATIENT_TYPE>();

                if (selectionMode == SELECTION_MODE_DEPARTMENT)
                {
                    var department = selectedDepartments.FirstOrDefault(d => d.IsRadioChecked);
                    if (department == null)
                    {
                        WaitingManager.Hide();
                        MessageManager.Show(Resources.ResourceMessage.VuiLongChonMotKhoa);
                        return;
                    }

                    // Tinh lai tu trang thai grid hien tai — KHONG dua vao unSelected duoc cap nhat tang dan (de stale khi bo tick).
                    var ptDataSource = gridControlPatientType.DataSource as List<PatientTypeADO>;
                    if (ptDataSource != null)
                    {
                        selectedPatientTypes = ptDataSource.Where(p => p.IsCheckBoxChecked).ToList();
                        unSelectedPatientTypes = ptDataSource.Where(p => !p.IsCheckBoxChecked).ToList();
                    }

                    foreach (var pt in unSelectedPatientTypes)
                    {
                        depaPatientTypes.RemoveAll(x =>
                            x.DEPARTMENT_ID == department.ID &&
                            x.PATIENT_TYPE_ID == pt.ID &&
                            x.SERVICE_ID == this.serviceId);
                    }

                    depaPatientTypeItems = selectedPatientTypes
                        .Where(p => p.IsCheckBoxChecked)
                        .Select(pt =>
                        {
                            var rec = new HIS_DEPA_PATIENT_TYPE
                            {
                                DEPARTMENT_ID = department.ID,
                                PATIENT_TYPE_ID = pt.ID,
                                SERVICE_ID = this.serviceId
                            };
                            WriteLongFlag(rec, "IS_AUTO_EXPEND", pt.IsAutoExpend ? 1 : 0);
                            WriteLongFlag(rec, "IS_NOT_EXPEND", pt.IsNotExpend ? 1 : 0);
                            return rec;
                        })
                        .ToList();
                }
                else
                {
                    var patientType = selectedPatientTypes.FirstOrDefault(p => p.IsRadioChecked);
                    if (patientType == null)
                    {
                        WaitingManager.Hide();
                        MessageManager.Show(Resources.ResourceMessage.VuiLongChonMotDoiTuongThanhToan);
                        return;
                    }

                    // Tinh lai tu trang thai grid hien tai — KHONG dua vao unSelected duoc cap nhat tang dan (de stale khi bo tick).
                    var deptDataSource = gridControlDepartment.DataSource as List<DepartmentADO>;
                    if (deptDataSource != null)
                    {
                        selectedDepartments = deptDataSource.Where(d => d.IsCheckBoxChecked).ToList();
                        unSelectedDepartments = deptDataSource.Where(d => !d.IsCheckBoxChecked).ToList();
                    }

                    foreach (var dept in unSelectedDepartments)
                    {
                        depaPatientTypes.RemoveAll(x =>
                            x.PATIENT_TYPE_ID == patientType.ID &&
                            x.DEPARTMENT_ID == dept.ID &&
                            x.SERVICE_ID == this.serviceId);
                    }

                    depaPatientTypeItems = selectedDepartments
                        .Where(d => d.IsCheckBoxChecked)
                        .Select(dept =>
                        {
                            var rec = new HIS_DEPA_PATIENT_TYPE
                            {
                                DEPARTMENT_ID = dept.ID,
                                PATIENT_TYPE_ID = patientType.ID,
                                SERVICE_ID = this.serviceId
                            };
                            WriteLongFlag(rec, "IS_AUTO_EXPEND", dept.IsAutoExpend ? 1 : 0);
                            WriteLongFlag(rec, "IS_NOT_EXPEND", dept.IsNotExpend ? 1 : 0);
                            return rec;
                        })
                        .ToList();
                }

                // Build index cu de tim record exist nhanh O(1).
                var existDict = new Dictionary<string, HIS_DEPA_PATIENT_TYPE>();
                foreach (var o in depaPatientTypes)
                {
                    string k = string.Format("{0}_{1}_{2}", o.DEPARTMENT_ID, o.PATIENT_TYPE_ID, o.SERVICE_ID);
                    if (!existDict.ContainsKey(k)) existDict[k] = o;
                }
                foreach (var newItem in depaPatientTypeItems)
                {
                    string key = string.Format("{0}_{1}_{2}", newItem.DEPARTMENT_ID, newItem.PATIENT_TYPE_ID, newItem.SERVICE_ID);
                    HIS_DEPA_PATIENT_TYPE existing;
                    if (existDict.TryGetValue(key, out existing))
                    {
                        // Da ton tai → cap nhat co IS_AUTO_EXPEND/IS_NOT_EXPEND theo lua chon moi.
                        WriteLongFlag(existing, "IS_AUTO_EXPEND", ReadLongFlag(newItem, "IS_AUTO_EXPEND"));
                        WriteLongFlag(existing, "IS_NOT_EXPEND", ReadLongFlag(newItem, "IS_NOT_EXPEND"));
                    }
                    else
                    {
                        depaPatientTypes.Add(newItem);
                        existDict[key] = newItem;
                    }
                }

                this.isClickPick = true;

                // Edit mode (da co serviceId): luu thang DB → user khong can click Luu o form cha nua.
                // Add mode (chua co serviceId): khong the luu DB vi chua co SERVICE_ID → giu in-memory, form cha se luu khi click Luu.
                bool dbSaveOk = true;
                if (this.serviceId.HasValue && this.serviceId.Value > 0)
                {
                    dbSaveOk = SaveDepaPatientTypeToDb();
                    if (dbSaveOk)
                    {
                        // Khong de form cha redo: tat isClickPick → form cha skip depa ops khi click Luu.
                        this.isClickPick = false;
                        this.isCalledApi = true;
                    }
                }

                NotifyCaller();
                WaitingManager.Hide();
                CommonParam param = new CommonParam();
                MessageManager.Show(this, param, dbSaveOk);
                // Set DialogResult de ShowDialog tra ket qua, sau do Close.
                if (this.Modal) this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Luu thang HIS_DEPA_PATIENT_TYPE xuong DB cho serviceId hien tai.
        /// Strategy: DELETE all old + CREATE new from depaPatientTypes (clean swap).
        /// Tra ve true neu thanh cong, false neu fail (caller se bao loi va de in-memory).
        /// </summary>
        private bool SaveDepaPatientTypeToDb()
        {
            try
            {
                if (!this.serviceId.HasValue || this.serviceId.Value <= 0) return false;

                CommonParam param = new CommonParam();

                // Buoc 1: Lay records cu tu DB de delete.
                var filter = new HisDepaPatientTypeFilter();
                filter.SERVICE_ID = this.serviceId.Value;
                var oldRecords = new BackendAdapter(param).Get<List<HIS_DEPA_PATIENT_TYPE>>(
                    "api/HisDepaPatientType/Get", ApiConsumers.MosConsumer, filter, param);

                // Buoc 2: Delete old records (neu co).
                if (oldRecords != null && oldRecords.Count > 0)
                {
                    var deleteResult = new BackendAdapter(param).Post<bool>(
                        "api/HisDepaPatientType/DeleteList", ApiConsumers.MosConsumer,
                        oldRecords.Select(o => o.ID).ToList(), param);
                    if (!deleteResult) return false;
                }

                // Buoc 3: Create new records (neu co).
                if (depaPatientTypes != null && depaPatientTypes.Count > 0)
                {
                    foreach (var item in depaPatientTypes)
                    {
                        item.SERVICE_ID = this.serviceId.Value;
                        item.ID = 0; // backend tu sinh ID moi
                    }
                    var createResult = new BackendAdapter(param).Post<List<HIS_DEPA_PATIENT_TYPE>>(
                        "api/HisDepaPatientType/CreateList", ApiConsumers.MosConsumer,
                        depaPatientTypes, param);
                    if (createResult == null) return false;
                }

                // Buoc 4: Reset cache de cac plugin khac luon co data tuoi.
                BackendDataWorker.Reset<HIS_DEPA_PATIENT_TYPE>();
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private void NotifyCaller()
        {
            try
            {
                // Cach 1 — Event public (caller goi `new` truc tiep va += event).
                if (OnDepaPatientTypeSaved != null)
                {
                    OnDepaPatientTypeSaved.Invoke(this.depaPatientTypes, this.isCalledApi, this.isClickPick);
                }

                // Cach 2 — DelegateSelectData (khi goi qua Behavior/MEF — neu sau nay can).
                if (callBackDelegate != null)
                {
                    var result = new DepaPatientTypeResultADO
                    {
                        DepaPatientTypes = this.depaPatientTypes,
                        IsCalledApi = this.isCalledApi,
                        IsClickPick = this.isClickPick
                    };
                    callBackDelegate(result);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Search + keyboard
        private void btnSearchDepartment_Click(object sender, EventArgs e)
        {
            try { FillDataToGridDepartment(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void btnSearchPatientType_Click(object sender, EventArgs e)
        {
            try { FillDataToGridPatientType(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void txtSearchDepartment_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((e.Control && e.KeyCode == Keys.F) || e.KeyCode == Keys.Enter)
                {
                    btnSearchDepartment_Click(null, null);
                    e.Handled = true;
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void txtSearchPatientType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((e.Control && e.KeyCode == Keys.D) || e.KeyCode == Keys.Enter)
                {
                    btnSearchPatientType_Click(null, null);
                    e.Handled = true;
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void frmHisDepaPatientTypeList_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.T)
                {
                    btnSave.Focus();
                    btnSave_Click(null, null);
                    e.Handled = true;
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }
        #endregion
    }
}
