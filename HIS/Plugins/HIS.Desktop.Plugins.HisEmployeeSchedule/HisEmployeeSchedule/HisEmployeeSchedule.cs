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
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisEmployeeSchedule.ADO;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisEmployeeSchedule
{
    public partial class frmHisEmployeeSchedule : FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.HIS_EMPLOYEE_SCHEDULE currentData;
        Inventec.Desktop.Common.Modules.Module moduleData;
        List<HIS_EMPLOYEE> employeeSelecteds;
        List<HIS_EMPLOYEE> listEmployees;
        internal List<DateTime?> dateScheduleSelecteds = new List<DateTime?>();
        List<int> scheduleDateSelecteds;

        internal List<DateTime?> dateSearchSelecteds = new List<DateTime?>();
        List<int> dtSearchSelecteds = new List<int>();
        #endregion

        #region Construct
        public frmHisEmployeeSchedule(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;

                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Methods
        private void frmHisEmployeeSchedule_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void MeShow()
        {
            try
            {
                //Gan gia tri mac dinh
                GetListEmplyee();
                SetDefaultValue();
                FillDataToComboAccount();
                InitCboEmployee();


                //Set enable control default
                EnableControlChanged(this.ActionType);

                //Load du lieu
                FillDataToGridControl();

                // Reset Controls
                ResetFormData();

                //Fill data into datasource combo
                FillDataToControlsForm(null);

                //Load ngon ngu label control
                SetCaptionByLanguageKey();


                //Set validate rule
                ValidateForm();

                //Focus default
                SetDefaultFocus();
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
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisEmployeeSchedule.Resources.Lang", typeof(frmHisEmployeeSchedule).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("HisEmployeeSchedule.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("HisEmployeeSchedule.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("HisEmployeeSchedule.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("HisEmployeeSchedule.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("HisEmployeeSchedule.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("HisEmployeeSchedule.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboAccount.Properties.NullText = Inventec.Common.Resource.Get.Value("HisEmployeeSchedule.cboAccount.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("HisEmployeeSchedule.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboEmployee.Properties.NullText = Inventec.Common.Resource.Get.Value("HisEmployeeSchedule.cboEmployee.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("HisEmployeeSchedule.btnReset.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("HisEmployeeSchedule.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("HisEmployeeSchedule.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("HisEmployeeSchedule.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("HisEmployeeSchedule.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("HisEmployeeSchedule.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("HisEmployeeSchedule.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnSTT.Caption = Inventec.Common.Resource.Get.Value("HisEmployeeSchedule.gridColumnSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("HisEmployeeSchedule.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnLoginName.Caption = Inventec.Common.Resource.Get.Value("HisEmployeeSchedule.gridColumnLoginName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnUserName.Caption = Inventec.Common.Resource.Get.Value("HisEmployeeSchedule.gridColumnUserName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnScheduleDate.Caption = Inventec.Common.Resource.Get.Value("HisEmployeeSchedule.gridColumnScheduleDate.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnTimeFrom.Caption = Inventec.Common.Resource.Get.Value("HisEmployeeSchedule.gridColumnTimeFrom.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnTimeTo.Caption = Inventec.Common.Resource.Get.Value("HisEmployeeSchedule.gridColumnTimeTo.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnCreatTime.Caption = Inventec.Common.Resource.Get.Value("HisEmployeeSchedule.gridColumnCreatTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnCreator.Caption = Inventec.Common.Resource.Get.Value("HisEmployeeSchedule.gridColumnCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEditTime.Caption = Inventec.Common.Resource.Get.Value("HisEmployeeSchedule.gridColumnEditTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEditor.Caption = Inventec.Common.Resource.Get.Value("HisEmployeeSchedule.gridColumnEditor.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtAccount.Text = Inventec.Common.Resource.Get.Value("HisEmployeeSchedule.txtAccount.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("HisEmployeeSchedule.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("HisEmployeeSchedule.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("HisEmployeeSchedule.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                cboAccount.EditValue = null;
                btnScheduleSearch.Text = null;
                cboEmployee.EditValue = null;
                dateSchedule.EditValue = null;
                dateSchedule.Text = null;
                timeFrom.EditValue = null;
                timeTo.EditValue = null;
                employeeSelecteds = null;
                dateSearchSelecteds = null;
                scheduleDateSelecteds = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void GetListEmplyee()
        {
            try
            {
                HisEmployeeFilter filter = new HisEmployeeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                listEmployees = new BackendAdapter(new CommonParam()).Get<List<HIS_EMPLOYEE>>("api/HisEmployee/Get", ApiConsumers.MosConsumer, filter, null);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableControlChanged(int action)
        {
            try
            {
                btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
                btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
                this.btnDateSchedule.Visible = (action == GlobalVariables.ActionAdd);
                this.dateSchedule.Visible = (action == GlobalVariables.ActionEdit);
                this.txtEmployee.Visible = (action == GlobalVariables.ActionEdit);
                this.txtEmployee.Enabled = (action != GlobalVariables.ActionEdit);
                this.cboEmployee.Visible = (action == GlobalVariables.ActionAdd);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        void FillDataToComboAccount()
        {
            try
            {
                if (listEmployees != null && listEmployees.Count > 0)
                {
                    List<EmpADO> listEmpADO = new List<EmpADO>();
                    foreach (var item in listEmployees)
                    {
                        EmpADO emp = new EmpADO();
                        emp.LOGINNAME = item.LOGINNAME;
                        emp.USERNAME = item.TDL_USERNAME;
                        listEmpADO.Add(emp);
                    }
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("LOGINNAME", "", 100, 1));
                    columnInfos.Add(new ColumnInfo("USERNAME", "", 200, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("DisplayName", "LOGINNAME", columnInfos, false, 300);
                    controlEditorADO.ImmediatePopup = true;
                    ControlEditorLoader.Load(cboAccount, listEmpADO, controlEditorADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        private void InitCboEmployee()
        {
            var lstEmp = listEmployees.Where(o => o.IS_LIMIT_SCHEDULE == 1);
            InitCheck(cboEmployee, SelectionGrid__cboEmployee);
            InitCombo(cboEmployee, lstEmp != null ? lstEmp.ToList() : null, "TDL_USERNAME", "LOGINNAME");
        }

        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;

                DevExpress.XtraGrid.Columns.GridColumn col1 = cbo.Properties.View.Columns.AddField(ValueMember);
                col1.VisibleIndex = 1;
                col1.Width = 150;
                col1.Caption = " ";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);
                col2.VisibleIndex = 2;
                col2.Width = 250;
                col2.Caption = "Tất cả";
                cbo.Properties.PopupFormWidth = 400;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                cbo.Properties.ImmediatePopup = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SelectionGrid__cboEmployee(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_EMPLOYEE> sgSelectedNews = new List<HIS_EMPLOYEE>();
                    foreach (MOS.EFMODEL.DataModels.HIS_EMPLOYEE rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append("; "); }
                            sb.Append(rv.TDL_USERNAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this.employeeSelecteds = new List<HIS_EMPLOYEE>();
                    this.employeeSelecteds.AddRange(sgSelectedNews);
                }

                this.cboEmployee.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGridControl()
        {
            try
            {
                WaitingManager.Show();
                int numPageSize = 0;
                if (ucPaging.pagingGrid != null)
                {
                    numPageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPaging, param, numPageSize, this.gridControlEmpSchedule);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_EMPLOYEE_SCHEDULE>> apiResult = null;
                HisEmployeeScheduleFilter filter = new HisEmployeeScheduleFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                gridViewEmpSchedule.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_EMPLOYEE_SCHEDULE>>("api/HisEmployeeSchedule/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.HIS_EMPLOYEE_SCHEDULE>)apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gridViewEmpSchedule.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridViewEmpSchedule.GridControl.DataSource = null;
                        rowCount = 0;
                        dataTotal = 0;
                    }
                }
                else
                {
                    gridViewEmpSchedule.GridControl.DataSource = null;
                    rowCount = 0;
                    dataTotal = 0;
                }
                gridViewEmpSchedule.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HisEmployeeScheduleFilter filter)
        {
            try
            {
                filter.LOGINNAME__EXACT = cboAccount.EditValue != null ? cboAccount.EditValue.ToString() : "";
                filter.SCHEDULE_DATES = dtSearchSelecteds;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void ResetFormData()
        {
            try
            {
                if (!lcEditorInfo.IsInitialized) return;
                lcEditorInfo.BeginUpdate();
                try
                {
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditorInfo.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;

                            fomatFrm.ResetText();

                            fomatFrm.EditValue = null;
                        }
                    }
                    cboEmployee.Text = null;
                    dateSchedule.EditValue = null;
                    dateSchedule.Text = null;
                    timeFrom.EditValue = null;
                    timeTo.EditValue = null;
                    employeeSelecteds = null;
                    scheduleDateSelecteds = null;
                    btnDateSchedule.Text = null;
                    GridCheckMarksSelection gridCheckMark = cboEmployee.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboEmployee.Properties.View);
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    lcEditorInfo.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToControlsForm(MOS.EFMODEL.DataModels.HIS_EMPLOYEE_SCHEDULE data)
        {
            try
            {
                if (data != null)
                {
                    txtEmployee.Text = data.USERNAME;
                    if (data.SCHEDULE_DATE != 0)
                    {
                        dateSchedule.EditValue = System.DateTime.ParseExact(data.SCHEDULE_DATE.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);
                    }
                    else
                        dateSchedule.EditValue = null;
                    if (!String.IsNullOrWhiteSpace(data.TIME_FROM) && data.TIME_FROM.Length == 4)
                    {
                        timeFrom.EditValue = String.Format("{0:00}:{1:00}", data.TIME_FROM.Substring(0, 2), data.TIME_FROM.Substring(2, 2));
                    }

                    else
                        timeFrom.EditValue = null;

                    if (!String.IsNullOrWhiteSpace(data.TIME_TO) && data.TIME_TO.Length == 4)
                        timeTo.EditValue = String.Format("{0:00}:{1:00}", data.TIME_TO.Substring(0, 2), data.TIME_TO.Substring(2, 2));
                    else
                        timeTo.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateForm()
        {
            try
            {
                ValidationTime();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidationTime()
        {
            try
            {
                ValidationTime validateTimeFromAndTimeTo = new ValidationTime();
                validateTimeFromAndTimeTo.tmFrom = timeFrom;
                validateTimeFromAndTimeTo.tmTo = timeTo;
                dxValidationProviderEditorInfo.SetValidationRule(timeFrom, validateTimeFromAndTimeTo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ValidationRequired validRule = new ValidationRequired();
                validRule.control = control;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultFocus()
        {
            try
            {
                cboAccount.Focus();
                cboAccount.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }
        #endregion
        #region ButtonHandle

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!btnEdit.Enabled && !btnAdd.Enabled)
                    return;

                positionHandle = -1;
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    dxValidationProviderEditorInfo.SetValidationRule(dateSchedule, null);
                    ValidationSingleControl(cboEmployee);
                    ValidationSingleControl(btnDateSchedule);
                    if (!dxValidationProviderEditorInfo.Validate())
                        return;

                    WaitingManager.Show();
                    List<HIS_EMPLOYEE_SCHEDULE> listUpdateDTO = new List<HIS_EMPLOYEE_SCHEDULE>();
                    CreateDTOFromDataForm(ref listUpdateDTO);
                    var resultData = new BackendAdapter(param).Post<List<HIS_EMPLOYEE_SCHEDULE>>("api/HisEmployeeSchedule/CreateList", ApiConsumers.MosConsumer, listUpdateDTO, param);
                    if (resultData != null && resultData.Count > 0)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                    }
                }
                else
                {
                    dxValidationProviderEditorInfo.SetValidationRule(btnDateSchedule, null);
                    dxValidationProviderEditorInfo.SetValidationRule(cboEmployee, null);
                    ValidationSingleControl(dateSchedule);
                    if (!dxValidationProviderEditorInfo.Validate())
                    {

                        IList<Control> invalidControls = this.dxValidationProviderEditorInfo.GetInvalidControls();
                        for (int i = invalidControls.Count - 1; i >= 0; i--)
                        {
                            Inventec.Common.Logging.LogSystem.Debug((i == 0 ? "InvalidControls:" : "") + "" + invalidControls[i].Name + ",");
                        }
                        return;
                    }

                    WaitingManager.Show();
                    HIS_EMPLOYEE_SCHEDULE updateDTO = new HIS_EMPLOYEE_SCHEDULE();

                    if (this.currentData != null && this.currentData.ID > 0)
                    {
                        LoadCurrent(this.currentData.ID, ref updateDTO);
                    }
                    UpdateDTOFromDataForm(ref updateDTO);
                    var resultData = new BackendAdapter(param).Post<HIS_EMPLOYEE_SCHEDULE>("api/HisEmployeeSchedule/Update", ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                    }
                }
                if (success)
                {
                    BackendDataWorker.Reset<HIS_EMPLOYEE_SCHEDULE>();
                }

                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void UpdateDTOFromDataForm(ref HIS_EMPLOYEE_SCHEDULE empScheduleDTO)
        {
            try
            {
                empScheduleDTO.SCHEDULE_DATE = Inventec.Common.TypeConvert.Parse.ToInt32(Convert.ToDateTime(dateSchedule.EditValue.ToString()).ToString("yyyyMMdd"));
                timeFrom.DeselectAll();
                if (timeFrom.EditValue != null)
                    empScheduleDTO.TIME_FROM = String.Format("{0:00}{1:00}", timeFrom.TimeSpan.Hours, timeFrom.TimeSpan.Minutes);
                else
                    empScheduleDTO.TIME_FROM = "0000";

                timeTo.DeselectAll();
                if (timeTo.EditValue != null)
                    empScheduleDTO.TIME_TO = String.Format("{0:00}{1:00}", timeTo.TimeSpan.Hours, timeTo.TimeSpan.Minutes);
                else
                    empScheduleDTO.TIME_TO = "2359";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void CreateDTOFromDataForm(ref List<HIS_EMPLOYEE_SCHEDULE> listEmpSchedule)
        {
            try
            {
                if (employeeSelecteds != null && employeeSelecteds.Count > 0 && scheduleDateSelecteds != null && scheduleDateSelecteds.Count > 0)
                {
                    string strTimeFrom, strTimeTo;
                    timeFrom.DeselectAll();
                    if (timeFrom.EditValue != null)
                        strTimeFrom = String.Format("{0:00}{1:00}", timeFrom.TimeSpan.Hours, timeFrom.TimeSpan.Minutes);
                    else
                        strTimeFrom = "0000";

                    timeTo.DeselectAll();
                    if (timeTo.EditValue != null)
                        strTimeTo = String.Format("{0:00}{1:00}", timeTo.TimeSpan.Hours, timeTo.TimeSpan.Minutes);
                    else
                        strTimeTo = "2359";


                    foreach (var emp in employeeSelecteds)
                    {
                        foreach (var date in scheduleDateSelecteds)
                        {
                            HIS_EMPLOYEE_SCHEDULE empSchedule = new HIS_EMPLOYEE_SCHEDULE();
                            empSchedule.LOGINNAME = emp.LOGINNAME;
                            empSchedule.USERNAME = emp.TDL_USERNAME;
                            empSchedule.SCHEDULE_DATE = date;
                            empSchedule.TIME_FROM = strTimeFrom;
                            empSchedule.TIME_TO = strTimeTo;
                            listEmpSchedule.Add(empSchedule);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref HIS_EMPLOYEE_SCHEDULE currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisEmployeeScheduleFilter filter = new HisEmployeeScheduleFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_EMPLOYEE_SCHEDULE>>("api/HisEmployeeSchedule/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                this.currentData = null;
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                ResetFormData();
                cboAccount.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnEdit.Enabled == false)
                    return;
                btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnAdd.Enabled == false)
                    return;
                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnReset_Click(null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        private void gridViewEmpSchedule_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.HIS_EMPLOYEE_SCHEDULE)gridViewEmpSchedule.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rowData), rowData));
                    ChangedDataRow(rowData);
                }
                Inventec.Common.Logging.LogSystem.Warn("Log 1");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangedDataRow(HIS_EMPLOYEE_SCHEDULE data)
        {
            try
            {
                if (data != null)
                {
                    ResetFormData();
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);
                    btnEdit.Enabled = data.SCHEDULE_DATE >= Int32.Parse(DateTime.Today.ToString("yyyyMMdd"));
                    FillDataToControlsForm(data);
                    this.currentData = data;
                    positionHandle = -1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.HIS_EMPLOYEE_SCHEDULE)gridViewEmpSchedule.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    if (rowData != null)
                    {
                        bool success = false;
                        CommonParam param = new CommonParam();
                        success = new BackendAdapter(param).Post<bool>("api/HisEmployeeSchedule/Delete", ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            BackendDataWorker.Reset<HIS_EMPLOYEE_SCHEDULE>();
                            FillDataToGridControl();
                            btnReset_Click(null, null);


                        }
                        MessageManager.Show(this, param, success);
                    }
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewEmpSchedule_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.HIS_EMPLOYEE_SCHEDULE pData = (MOS.EFMODEL.DataModels.HIS_EMPLOYEE_SCHEDULE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.CREATE_TIME) ?? "";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFIER_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.MODIFY_TIME) ?? "";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }

                    else if (e.Column.FieldName == "SCHEDULE_DATE_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)pData.SCHEDULE_DATE) ?? "";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "TIME_FROM_STR")
                    {
                        try
                        {
                            if (!String.IsNullOrWhiteSpace(pData.TIME_FROM) && pData.TIME_FROM.Length == 4)
                                e.Value = String.Format("{0:00}:{1:00}", pData.TIME_FROM.Substring(0, 2), pData.TIME_FROM.Substring(2, 2));
                            else
                                e.Value = "";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "TIME_TO_STR")
                    {
                        try
                        {
                            if (!String.IsNullOrWhiteSpace(pData.TIME_TO) && pData.TIME_TO.Length == 4)
                                e.Value = String.Format("{0:00}:{1:00}", pData.TIME_TO.Substring(0, 2), pData.TIME_TO.Substring(2, 2));
                            else
                                e.Value = "";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEmployee_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null || gridCheckMark.Selection == null || gridCheckMark.Selection.Count == 0)
                {
                    e.DisplayText = "";
                    return;
                }
                foreach (MOS.EFMODEL.DataModels.HIS_EMPLOYEE rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append("; "); }

                    sb.Append(rv.TDL_USERNAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnScheduleSearch_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Glyph)
                {
                    frmMultiDate frmChooseDate = new frmMultiDate(dateSearchSelecteds, SelectMultiDateSearch);
                    frmChooseDate.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectMultiDateSearch(List<DateTime?> datas)
        {
            try
            {
                if (datas != null)
                {
                    this.dateSearchSelecteds = datas as List<DateTime?>;
                    string strTimeDisplay = "";
                    int num = 0;
                    this.dateSearchSelecteds = this.dateSearchSelecteds.OrderBy(o => o.Value).ToList();
                    foreach (var item in this.dateSearchSelecteds)
                    {
                        if (item != null && item.Value != DateTime.MinValue)
                        {
                            strTimeDisplay += (num == 0 ? "" : "; ") + item.Value.ToString("dd/MM");
                            num++;
                        }
                    }
                    if (this.btnScheduleSearch.Text != strTimeDisplay)
                    {
                        this.btnScheduleSearch.Text = strTimeDisplay;
                    }
                    this.dtSearchSelecteds = this.dateSearchSelecteds.Select(o => Inventec.Common.TypeConvert.Parse.ToInt32(o.Value.ToString("yyyyMMdd"))).OrderByDescending(o => o).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDateSchedule_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Glyph)
                {
                    frmMultiDate frmChooseDate = new frmMultiDate(dateScheduleSelecteds, SelectMultiDate);
                    frmChooseDate.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectMultiDate(List<DateTime?> datas)
        {
            try
            {
                if (datas != null)
                {
                    this.dateScheduleSelecteds = datas as List<DateTime?>;
                    string strTimeDisplay = "";
                    int num = 0;
                    this.dateScheduleSelecteds = this.dateScheduleSelecteds.OrderBy(o => o.Value).ToList();
                    foreach (var item in this.dateScheduleSelecteds)
                    {
                        if (item != null && item.Value != DateTime.MinValue)
                        {
                            strTimeDisplay += (num == 0 ? "" : "; ") + item.Value.ToString("dd/MM");
                            num++;
                        }
                    }
                    if (this.btnDateSchedule.Text != strTimeDisplay)
                    {
                        this.btnDateSchedule.Text = strTimeDisplay;
                    }
                    this.scheduleDateSelecteds = this.dateScheduleSelecteds.Select(o => Inventec.Common.TypeConvert.Parse.ToInt32(o.Value.ToString("yyyyMMdd"))).OrderByDescending(o => o).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEmployee_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (btnDateSchedule.Visible)
                    {
                        btnDateSchedule.Focus();
                        btnDateSchedule.SelectAll();
                    }
                    else
                    {
                        dateSchedule.Focus();
                        dateSchedule.SelectAll();
                        dateSchedule.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDateSchedule_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    timeFrom.Focus();
                    timeFrom.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dateSchedule_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    timeFrom.Focus();
                    timeFrom.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccount_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboAccount.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAccount_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboAccount.EditValue == null)
                {
                    cboAccount.Properties.Buttons[1].Visible = false;
                }
                else
                {
                    cboAccount.Properties.Buttons[1].Visible = true;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timeFrom_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    timeFrom.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timeTo_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    timeTo.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timeTo_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (timeTo.EditValue == null)
                {
                    timeTo.Properties.Buttons[1].Visible = false;
                }
                else
                {
                    timeTo.Properties.Buttons[1].Visible = true;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timeFrom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (timeFrom.EditValue == null)
                {
                    timeFrom.Properties.Buttons[1].Visible = false;
                }
                else
                {
                    timeFrom.Properties.Buttons[1].Visible = true;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewEmpSchedule_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    HIS_EMPLOYEE_SCHEDULE data = (HIS_EMPLOYEE_SCHEDULE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "DELETE")
                    {
                        try
                        {
                            if (data.SCHEDULE_DATE < Int32.Parse(DateTime.Today.ToString("yyyyMMdd")))
                                e.RepositoryItem = btnGDeleteDisable;
                            else
                                e.RepositoryItem = btnGDelete;

                        }
                        catch (Exception ex)
                        {

                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnScheduleSearch_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    btnScheduleSearch.Text = null;
                    dateSearchSelecteds = null;
                    dtSearchSelecteds = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
