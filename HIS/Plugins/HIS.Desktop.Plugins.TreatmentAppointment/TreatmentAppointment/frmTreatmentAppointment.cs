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
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.TreatmentAppointment.ADO;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentAppointment
{
    public partial class frmTreatmentAppointment : FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        Inventec.Desktop.Common.Modules.Module moduleData;
        List<HIS_DEPARTMENT> endDepartmentSelecteds;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        bool isInit;
        #endregion

        #region Construct
        public frmTreatmentAppointment(Inventec.Desktop.Common.Modules.Module moduleData)
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
        private void frmTreatmentAppointment_Load(object sender, EventArgs e)
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
                SetCaptionByLanguageKey();
                InitCombobox();
                SetDefaultValue();
                InitControlState();
                FillDataToGridControl();
                SetDefaultFocus();
                ShowFormInExtendMonitor(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        internal static void ShowFormInExtendMonitor(frmTreatmentAppointment control)
        {
            try
            {
                Screen[] sc;
                sc = Screen.AllScreens;
                if (sc.Length <= 1)
                {
                    control.Show();
                }
                else
                {
                    Screen secondScreen = sc.FirstOrDefault(o => o != Screen.PrimaryScreen);
                    control.StartPosition = FormStartPosition.Manual;
                    control.Location = new Point(
                        secondScreen.Bounds.Left + (secondScreen.Bounds.Width - control.Width) / 2,
                        secondScreen.Bounds.Top + (secondScreen.Bounds.Height - control.Height) / 2
                    );
                    control.Show();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.TreatmentAppointment.Resources.Lang", typeof(frmTreatmentAppointment).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtPatientCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.txtPatientCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtTreatmentCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.txtTreatmentCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboEndDepartment.Properties.NullText = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.cboEndDepartment.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboAppointmentTimeOption.Properties.NullText = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.cboAppointmentTimeOption.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsAppointmentReminded.Properties.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.chkIsAppointmentReminded.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkNotAppointmentAttended.Properties.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.chkNotAppointmentAttended.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkNotAppointmentReminded.Properties.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.chkNotAppointmentReminded.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsAppointmentAttended.Properties.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.chkIsAppointmentAttended.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.txtSearch.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnSTT.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.gridColumnSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnRemind.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.gridColumnRemind.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnStatus.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.gridColumnStatus.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnPatientCode.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.gridColumnPatientCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnTreatmentCode.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.gridColumnTreatmentCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnPatientName.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.gridColumnPatientName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnGender.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.gridColumnGender.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnDob.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.gridColumnDob.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnAddress.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.gridColumnAddress.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnPhone.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.gridColumnPhone.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnAppointDay.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.gridColumnAppointDay.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnInTime.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.gridColumnInTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnIcdName.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.gridColumnIcdName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDay.Text = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.lciDay.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEmptySpace.Text = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.lciEmptySpace.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAppointmentTimeTo.Text = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.lciAppointmentTimeTo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmTreatmentAppointment.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitCombobox()
        {
            InitComboEndDepartment();
            InitComboAppointmentTimeOption();
        }
        private void InitComboEndDepartment()
        {
            InitCheck(cboEndDepartment, SelectionGrid__cboEndDepartment);
            InitCombo(cboEndDepartment, BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList(), "DEPARTMENT_NAME");
        }
        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);
                col2.VisibleIndex = 2;
                col2.Width = 200;
                col2.Caption = Resources.ResourceMessageLang.TatCa;
                cbo.Properties.PopupFormWidth = 250;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
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

        private void SelectionGrid__cboEndDepartment(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_DEPARTMENT> sgSelectedNews = new List<HIS_DEPARTMENT>();
                    foreach (HIS_DEPARTMENT rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.DEPARTMENT_NAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this.endDepartmentSelecteds = new List<HIS_DEPARTMENT>();
                    this.endDepartmentSelecteds.AddRange(sgSelectedNews);
                }
                this.cboEndDepartment.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ProcessSelectEndDepartment()
        {
            try
            {
                long departmentId = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetWorkPlace(this.moduleData).DepartmentId;
                GridCheckMarksSelection gridCheckMark = cboEndDepartment.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboEndDepartment.Properties.View);
                }
                if (cboEndDepartment.Properties.Tag != null)
                {
                    List<HIS_DEPARTMENT> ds = cboEndDepartment.Properties.DataSource as List<HIS_DEPARTMENT>;

                    HIS_DEPARTMENT row = ds != null ? ds.FirstOrDefault(o => o.ID == departmentId) : null;
                    if (row != null)
                    {
                        List<HIS_DEPARTMENT> selects = new List<HIS_DEPARTMENT>();
                        selects.Add(row);
                        gridCheckMark.SelectAll(selects);
                    }
                }
                else
                {
                    cboEndDepartment.EditValue = null;
                    GridCheckMarksSelection gridCheckMarkBusinessCodes = cboEndDepartment.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMarkBusinessCodes != null)
                    {
                        gridCheckMarkBusinessCodes.ClearSelection(cboEndDepartment.Properties.View);
                    }
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void InitComboAppointmentTimeOption()
        {
            try
            {
                List<FilterTypeADO> ListOptionAll = new List<FilterTypeADO>();
                FilterTypeADO denNgayHenKhamTrong = new FilterTypeADO(0, Resources.ResourceMessageLang.DenNgayHenKhamTrong);
                ListOptionAll.Add(denNgayHenKhamTrong);

                FilterTypeADO daQuaThoiGianHenKham = new FilterTypeADO(1, Resources.ResourceMessageLang.DaQuaThoiGianHenKham);
                ListOptionAll.Add(daQuaThoiGianHenKham);

                FilterTypeADO thoiGianHenKhamTrongKhoang = new FilterTypeADO(2, Resources.ResourceMessageLang.ThoiGianHenKhamTrongKhoang);
                ListOptionAll.Add(thoiGianHenKhamTrongKhoang);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("Name", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("Name", "id", columnInfos, false, 250);
                ControlEditorLoader.Load(cboAppointmentTimeOption, ListOptionAll, controlEditorADO);
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
                chkNotAppointmentAttended.Checked = true;
                chkNotAppointmentReminded.Checked = true;
                ProcessSelectEndDepartment();
                cboAppointmentTimeOption.EditValue = 0;
                txtTreatmentCode.Text = "";
                txtPatientCode.Text = "";
                txtSearch.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitControlState()
        {
            try
            {
                isInit = true;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData("HIS.Desktop.Plugins.TreatmentAppointment");
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == spnAppointmentDay.Name)
                        {
                            spnAppointmentDay.Value = !String.IsNullOrWhiteSpace(item.VALUE) ? Convert.ToInt64(item.VALUE) : 0;
                        }
                    }
                }
                isInit = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                ucPaging.Init(LoadPaging, param, numPageSize, this.gridControlTreatmentAppointment);
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
                Inventec.Core.ApiResultObject<List<HIS_TREATMENT>> apiResult = null;
                HisTreatmentFilter filter = new HisTreatmentFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                gridViewTreatmentAppointment.BeginUpdate();
                gridViewTreatmentAppointment.GridControl.DataSource = null;
                apiResult = new BackendAdapter(paramCommon).GetRO<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<HIS_TREATMENT>)apiResult.Data;
                    gridViewTreatmentAppointment.GridControl.DataSource = data;
                    rowCount = (data == null ? 0 : data.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                }
                gridViewTreatmentAppointment.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HisTreatmentFilter filter)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtTreatmentCode.Text.Trim()))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    try
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                    txtTreatmentCode.Text = code;
                    filter.TREATMENT_CODE__EXACT = code;
                }
                if (!String.IsNullOrEmpty(txtPatientCode.Text.Trim()))
                {
                    string code = txtPatientCode.Text.Trim();
                    try
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }

                    txtPatientCode.Text = code;
                    filter.TDL_PATIENT_CODE__EXACT = code;
                }
                filter.IS_APPOINTMENT_ATTENDED = chkIsAppointmentAttended.Checked;
                filter.IS_APPOINTMENT_REMINDED = chkIsAppointmentReminded.Checked;

                if (this.endDepartmentSelecteds != null && this.endDepartmentSelecteds.Count > 0)
                    filter.END_DEPARTMENT_IDs = this.endDepartmentSelecteds.Select(o => o.ID).ToList();

                if (!String.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    filter.KEY_WORD = txtSearch.Text.Trim();
                }
                if (cboAppointmentTimeOption.EditValue != null && (int)cboAppointmentTimeOption.EditValue == 0)//Đến ngày hẹn khám trong
                {
                    DateTime newTime = DateTime.Now.AddDays((double)spnAppointmentDay.Value);
                    filter.APPOINTMENT_TIME_FROM = Convert.ToInt64(DateTime.Now.ToString("yyyyMMdd") + "000000");
                    filter.APPOINTMENT_TIME_TO = Convert.ToInt64(newTime.ToString("yyyyMMdd") + "235959");
                }
                else if (cboAppointmentTimeOption.EditValue != null && (int)cboAppointmentTimeOption.EditValue == 1)//Đã quá thời gian hẹn khám
                {
                    filter.APPOINTMENT_TIME_TO = Convert.ToInt64(DateTime.Now.ToString("yyyyMMdd") + "000000");
                }
                else if (cboAppointmentTimeOption.EditValue != null && (int)cboAppointmentTimeOption.EditValue == 2)//Thời gian hẹn khám trong khoảng
                {
                    filter.APPOINTMENT_TIME_FROM = Convert.ToInt64(dtAppointmentTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                    filter.APPOINTMENT_TIME_TO = Convert.ToInt64(dtAppointmentTimeTo.DateTime.ToString("yyyyMMdd") + "235959");
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("filter__:", filter));
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetDefaultFocus()
        {
            try
            {
                txtSearch.Focus();
                txtSearch.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }
        #endregion
        #region ButtonHandle

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

        private void btnAppointmentRemind_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_TREATMENT result = new HIS_TREATMENT();
            bool success = false;
            try
            {

                HIS_TREATMENT data = (HIS_TREATMENT)gridViewTreatmentAppointment.GetFocusedRow();
                WaitingManager.Show();
                result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TREATMENT>("api/HisTreatment/AppointmentRemind", ApiConsumers.MosConsumer, data.ID, param);
                WaitingManager.Hide();
                if (result != null)
                {
                    success = true;
                    FillDataToGridControl();
                }

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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCancelAppointmentRemind_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_TREATMENT result = new HIS_TREATMENT();
            bool success = false;
            try
            {

                HIS_TREATMENT data = (HIS_TREATMENT)gridViewTreatmentAppointment.GetFocusedRow();
                WaitingManager.Show();
                result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TREATMENT>("api/HisTreatment/AppointmentUnremind", ApiConsumers.MosConsumer, data.ID, param);
                WaitingManager.Hide();
                if (result != null)
                {
                    success = true;
                    FillDataToGridControl();
                }

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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void gridViewTreatmentAppointment_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    short isAppointmentAttended = Inventec.Common.TypeConvert.Parse.ToInt16((gridViewTreatmentAppointment.GetRowCellValue(e.RowHandle, "IS_APPOINTMENT_REMINDED") ?? "").ToString());
                    if (e.Column.FieldName == "REMIND")
                    {
                        e.RepositoryItem = isAppointmentAttended == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnCancelAppointmentRemind : btnAppointmentRemind;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTreatmentAppointment_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HIS_TREATMENT pData = (MOS.EFMODEL.DataModels.HIS_TREATMENT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    else if (e.Column.FieldName == "STATUS")
                    {
                        if (pData.IS_APPOINTMENT_REMINDED == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        {
                            e.Value = imageListStatus.Images[0];
                        }
                        else
                        {
                            e.Value = imageListStatus.Images[1];
                        }
                    }
                    else if (e.Column.FieldName == "STATUS_STR")
                    {
                        if (pData.IS_APPOINTMENT_ATTENDED == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        {
                            e.Value = Resources.ResourceMessageLang.DaTaiKham;
                        }
                        else
                        {
                            e.Value = Resources.ResourceMessageLang.ChuaTaiKham;
                        }
                    }
                    else if (e.Column.FieldName == "TDL_PATIENT_DOB_STR")
                    {
                        try
                        {
                            if (pData.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                            {
                                e.Value = pData.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                            }
                            else
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)pData.TDL_PATIENT_DOB) ?? "";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "PATIENT_PHONE_STR")
                    {

                        e.Value = !String.IsNullOrEmpty(pData.TDL_PATIENT_PHONE) ? pData.TDL_PATIENT_PHONE : pData.TDL_PATIENT_MOBILE;
                    }
                    else if (e.Column.FieldName == "APPOINTMENT_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)(pData.APPOINTMENT_TIME ?? 0)) ?? "";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "IN_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.IN_TIME) ?? "";
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
        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEndDepartment_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
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
                foreach (HIS_DEPARTMENT rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }

                    sb.Append(rv.DEPARTMENT_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnAppointmentDay_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAppointmentTimeOption_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboAppointmentTimeOption.EditValue != null && cboAppointmentTimeOption.EditValue != cboAppointmentTimeOption.OldEditValue)
                {
                    if (Convert.ToInt16(cboAppointmentTimeOption.EditValue.ToString()) == 0)//Đến ngày hẹn khám trong
                    {
                        this.lciDay.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        this.lciEmptySpace.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                        this.lciAppointmentTimeFrom.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        this.lciAppointmentTimeTo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                    else if (Convert.ToInt16(cboAppointmentTimeOption.EditValue.ToString()) == 1)//Đã quá thời gian hẹn khám
                    {
                        this.lciDay.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        this.lciEmptySpace.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                        this.lciAppointmentTimeFrom.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        this.lciAppointmentTimeTo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                    else if (Convert.ToInt16(cboAppointmentTimeOption.EditValue.ToString()) == 2)//Thời gian hẹn khám trong khoảng
                    {
                        this.lciDay.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        this.lciEmptySpace.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                        this.lciAppointmentTimeFrom.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        this.lciAppointmentTimeTo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnAppointmentDay_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (isInit)
                {
                    return;
                }
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == spnAppointmentDay.Name && o.MODULE_LINK == "HIS.Desktop.Plugins.TreatmentAppointment").FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = spnAppointmentDay.Value.ToString();
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = spnAppointmentDay.Name;
                    csAddOrUpdate.VALUE = spnAppointmentDay.Value.ToString();
                    csAddOrUpdate.MODULE_LINK = "HIS.Desktop.Plugins.TreatmentAppointment";
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
