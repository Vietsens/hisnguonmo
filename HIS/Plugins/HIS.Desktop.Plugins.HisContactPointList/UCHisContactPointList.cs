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
using ACS.Filter;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisContactPointList.Base;
//using HIS.Desktop.Plugins.HisContactPointList.Config;
using HIS.Desktop.Plugins.Library.FormMedicalRecord;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace HIS.Desktop.Plugins.HisContactPointList
{
    public partial class UCHisContactPointList : UserControlBase
    {
        int rowCount = 0;
        int dataTotal = 0;
        int start = 0;
        int lastRowHandle = -1;
        GridColumn lastColumn = null;
        ToolTipControlInfo lastInfo = null;
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        List<V_HIS_CONTACT_POINT> ListHisContactPoint = new List<V_HIS_CONTACT_POINT>();
        List<ACS_USER> ListAcsUser = new List<ACS_USER>();
        List<V_HIS_EMPLOYEE> ListHisEmployee = new List<V_HIS_EMPLOYEE>();
        List<HIS_GENDER> ListGender = new List<HIS_GENDER>();

        string loginName = null;

        List<HIS_DEPARTMENT> _EndDepartmentSelecteds;
        List<HIS_DEPARTMENT> listDepartment;
        bool isDelete = false;

        public UCHisContactPointList(Inventec.Desktop.Common.Modules.Module module, string treatmentCode)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.currentModule = module;
                Base.ResourceLangManager.InitResourceLanguageManager();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCHisContactPointList(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            this.loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            currentModule = module;
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCHisContactPointList_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetCaptionByLanguageKey();
                //InitTypeFind();
                lciFortxtPatientName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;//TODO
                GetDataCombo();
                InitCheck(cboDepartment, SelectionGrid__EndDepartment);
                InitCombo(cboDepartment, listDepartment, "DEPARTMENT_NAME", "ID");
                SetDefaultControl();
                FillDataToGrid();
                //LoadAcsControls();
                //HisConfigCFG.LoadConfig();

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataCombo()
        {
            try
            {
                ListGender = BackendDataWorker.Get<HIS_GENDER>();
                listDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SelectionGrid__EndDepartment(object sender, EventArgs e)
        {
            try
            {
                _EndDepartmentSelecteds = new List<HIS_DEPARTMENT>();
                foreach (HIS_DEPARTMENT rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _EndDepartmentSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);

                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = "Tất cả";
                cbo.Properties.PopupFormWidth = 200;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(cbo.Properties.DataSource);
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

        private void ResetCombo(GridLookUpEdit cbo)
        {
            try
            {
                cbo.EditValue = null;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(/*cbo.Properties.DataSource*/null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetDefaultControl()
        {
            try
            {
                ResetCombo(cboDepartment);
                rdNhanVien.Checked = false;
                rdBenhNhan.Checked = false;
                rdKhac.Checked = false;
                spinFFrom.EditValue = null;
                spinFTo.EditValue = null;
                txtKeyword.Text = "";
                cboDepartment.Text = null;
                _EndDepartmentSelecteds = new List<HIS_DEPARTMENT>();
                txtLoginname.Text = "";
                txtPatientName.Text = "";
                txtPatientCode.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FomatControlByTimePatientCode()
        {
            try
            {
                //ResetCombo(cboDepartment);//TODO
                //rdBenhNhan.Checked = true;
                //rdNhanVien.Checked = false;/
                //rdKhac.Checked = false;

                txtKeyword.Text = "";
                txtLoginname.Text = "";
                txtPatientName.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                int pageSize;
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }

                FillDataToGridContactPointList(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridContactPointList, param, pageSize, gridControlHisContactPointList);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void FillDataToGridContactPointList(object param)
        {
            try
            {
                start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);

                HisContactPointViewFilter filter = new HisContactPointViewFilter();

                if (!string.IsNullOrEmpty(txtLoginname.Text))
                {
                    filter.LOGINNAME__EXACT = txtLoginname.Text.Trim();
                }
                if (!string.IsNullOrEmpty(txtPatientCode.Text))
                {
                    string code = txtPatientCode.Text.Trim();
                    if (code.Length < 10)
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtPatientCode.Text = code;
                    }
                    filter.PATIENT_CODE__EXACT = code;//TODO
                    FomatControlByTimePatientCode();
                }

                if (this.rdNhanVien.Checked)
                {
                    filter.CONTACT_TYPE = IMSys.DbConfig.HIS_RS.HIS_CONTACT_POINT.CONTACT_TYPE__NV;
                }
                else if (this.rdBenhNhan.Checked)
                {
                    filter.CONTACT_TYPE = IMSys.DbConfig.HIS_RS.HIS_CONTACT_POINT.CONTACT_TYPE__BN;
                }
                else if (this.rdKhac.Checked)
                {
                    filter.CONTACT_TYPE = IMSys.DbConfig.HIS_RS.HIS_CONTACT_POINT.CONTACT_TYPE__KHAC;
                }

                if (_EndDepartmentSelecteds != null && _EndDepartmentSelecteds.Count > 0)
                {
                    //filter.DEPARTMENT_ID = _EndDepartmentSelecteds.Select(p => p.ID).FirstOrDefault();//TODO
                    filter.DEPARTMENT_IDs = _EndDepartmentSelecteds.Select(p => p.ID).ToList();//TODO
                }

                if (spinFFrom.EditValue != null)
                {
                    filter.CONTACT_LEVEL_FROM = (long)spinFFrom.Value;
                }
                if (spinFTo.EditValue != null)
                {
                    filter.CONTACT_LEVEL_TO = (long)spinFTo.Value;
                }

                //filter.PATIENT_NAME = txtPatientName.Text.Trim();//TODO
                filter.KEY_WORD = txtKeyword.Text != "" ? txtKeyword.Text : null;

                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));

                var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<V_HIS_CONTACT_POINT>>(HisRequestUriStore.HIS_CONTACT_POINT_GETVIEW, ApiConsumers.MosConsumer, filter, paramCommon);
                if (result != null)
                {
                    ListHisContactPoint = (List<V_HIS_CONTACT_POINT>)result.Data;

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListHisContactPoint), ListHisContactPoint));

                    CommonParam paramCommonUser = new CommonParam();
                    //ACS_USER data
                    ACS.Filter.AcsUserFilter filterUserSearch = new ACS.Filter.AcsUserFilter();
                    filterUserSearch.LOGINNAMEs = (ListHisContactPoint != null && ListHisContactPoint.Count > 0) ? ListHisContactPoint.Select(o => o.LOGINNAME).Distinct().ToList() : null;
                    filterUserSearch.ORDER_FIELD = "MODIFY_TIME";
                    filterUserSearch.ORDER_DIRECTION = "DESC";

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filterUserSearch), filterUserSearch));

                    this.ListAcsUser = new BackendAdapter(paramCommonUser).Get<List<ACS.EFMODEL.DataModels.ACS_USER>>
                        (HisRequestUriStore.ACS_USER_GET, HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, filterUserSearch, paramCommonUser);

                    int userCount = (this.ListAcsUser != null ? this.ListAcsUser.Count : 0);

                    if (this.ListAcsUser != null && this.ListAcsUser.Count > 0)
                    {

                        MOS.Filter.HisEmployeeFilter hisEmployeeFilter = new MOS.Filter.HisEmployeeFilter();
                        hisEmployeeFilter.LOGINNAMEs = (ListHisContactPoint != null && ListHisContactPoint.Count > 0) ? ListHisContactPoint.Select(o => o.LOGINNAME).Distinct().ToList() : null;
                        hisEmployeeFilter.ORDER_FIELD = "MODIFY_TIME";
                        hisEmployeeFilter.ORDER_DIRECTION = "DESC";

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisEmployeeFilter), hisEmployeeFilter));

                        paramCommonUser = new CommonParam();
                        this.ListHisEmployee = new BackendAdapter(paramCommonUser).Get<List<V_HIS_EMPLOYEE>>
                            (HisRequestUriStore.HIS_EMPLOYEE_GETVIEW, HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, hisEmployeeFilter, paramCommonUser);

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListHisEmployee), ListHisEmployee));
                        if (!String.IsNullOrEmpty(txtPatientName.Text))
                        {
                            this.ListAcsUser = this.ListAcsUser.Where(t => Inventec.Common.String.Convert.UnSignVNese(t.USERNAME.ToLower()).Contains(Inventec.Common.String.Convert.UnSignVNese(txtPatientName.Text.ToLower()))).ToList();
                        }
                        else if (!String.IsNullOrEmpty(txtKeyword.Text))
                        {
                            if (this.ListHisEmployee != null && this.ListHisEmployee.Count > 0)
                            {
                                var ListHisEmployees = this.ListHisEmployee.Where(t => t.DEPARTMENT_NAME != null && Inventec.Common.String.Convert.UnSignVNese(t.DEPARTMENT_NAME.ToLower()).Contains(Inventec.Common.String.Convert.UnSignVNese(txtKeyword.Text.ToLower()))).ToList();
                                if (ListHisEmployees != null && ListHisEmployees.Count > 0)
                                {
                                    this.ListAcsUser = this.ListAcsUser.Where(t => 
                                        (Inventec.Common.String.Convert.UnSignVNese(t.USERNAME.ToLower()).Contains(Inventec.Common.String.Convert.UnSignVNese(txtKeyword.Text.ToLower())))
                                        || (ListHisEmployees.Select(k => k.LOGINNAME).Contains(t.LOGINNAME))
                                        ).ToList();
                                }
                                else
                                {
                                    this.ListAcsUser = this.ListAcsUser.Where(t => Inventec.Common.String.Convert.UnSignVNese(t.USERNAME.ToLower()).Contains(Inventec.Common.String.Convert.UnSignVNese(txtKeyword.Text.ToLower()))).ToList();
                                }
                            }
                            else
                            {
                                this.ListAcsUser = this.ListAcsUser.Where(t => Inventec.Common.String.Convert.UnSignVNese(t.USERNAME.ToLower()).Contains(Inventec.Common.String.Convert.UnSignVNese(txtKeyword.Text.ToLower()))).ToList();
                            }
                        }

                        if (this.ListHisEmployee != null && this.ListHisEmployee.Count > 0)
                        {
                            if (_EndDepartmentSelecteds != null && _EndDepartmentSelecteds.Count > 0)
                            {
                                this.ListHisEmployee = this.ListHisEmployee.Where(t => t.DEPARTMENT_ID != null && _EndDepartmentSelecteds.Select(p => p.ID).Contains(t.DEPARTMENT_ID ?? 0)).ToList();

                                this.ListAcsUser = (this.ListHisEmployee != null && this.ListHisEmployee.Count > 0) ? this.ListAcsUser.Where(o => this.ListHisEmployee.Select(k => k.LOGINNAME).Contains(o.LOGINNAME)).ToList() : null;

                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _EndDepartmentSelecteds), _EndDepartmentSelecteds)
                                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListAcsUser), ListAcsUser)
                                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.ListHisEmployee), this.ListHisEmployee));
                            }
                        }
                    }

                    int userCountAfterFilter = (this.ListAcsUser != null ? this.ListAcsUser.Count : 0);

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => userCount), userCount)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => userCountAfterFilter), userCountAfterFilter));

                    ListHisContactPoint = ListHisContactPoint != null ? ListHisContactPoint.Where(o => (o.CONTACT_TYPE == IMSys.DbConfig.HIS_RS.HIS_CONTACT_POINT.CONTACT_TYPE__NV && this.ListAcsUser != null && this.ListAcsUser.Select(t => t.LOGINNAME).Contains(o.LOGINNAME)) || o.CONTACT_TYPE != IMSys.DbConfig.HIS_RS.HIS_CONTACT_POINT.CONTACT_TYPE__NV).ToList() : null;

                    gridControlHisContactPointList.BeginUpdate();
                    gridControlHisContactPointList.DataSource = null;
                    gridControlHisContactPointList.DataSource = ListHisContactPoint;
                    rowCount = (ListHisContactPoint == null ? 0 : (ListHisContactPoint.Count - (userCount - userCountAfterFilter)));
                    dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
                    gridControlHisContactPointList.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                gridControlHisContactPointList.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewHisContactPointList_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                //Inventec.Common.Logging.LogSystem.Debug("gridViewHisContactPointList_PopupMenuShowing.1");
                //GridHitInfo hi = e.HitInfo;
                //if (hi.InRowCell)
                //{
                //    Inventec.Common.Logging.LogSystem.Debug("gridViewHisContactPointList_PopupMenuShowing.2");
                //    int rowHandle = gridViewHisContactPointList.GetVisibleRowHandle(hi.RowHandle);

                //    this.currentTreatment = (V_HIS_TREATMENT_4)gridViewHisContactPointList.GetRow(rowHandle);

                //    gridViewHisContactPointList.OptionsSelection.EnableAppearanceFocusedCell = true;
                //    gridViewHisContactPointList.OptionsSelection.EnableAppearanceFocusedRow = true;
                //    if (barManager2 == null)
                //    {
                //        barManager2 = new BarManager();
                //        barManager2.Form = this;
                //    }
                //    Inventec.Common.Logging.LogSystem.Debug("gridViewHisContactPointList_PopupMenuShowing.3");
                //    if (this.emrMenuPopupProcessor == null) this.emrMenuPopupProcessor = new Library.FormMedicalRecord.MediRecordMenuPopupProcessor();
                //    popupMenuProcessor = new PopupMenuProcessor(this.currentTreatment, barManager2, Treatment_MouseRightClick, (RefeshReference)BtnSearch);//(RefeshReference)BtnSearch
                //    popupMenuProcessor.InitMenu(this.emrMenuPopupProcessor, this.currentModule.RoomId);
                //    Inventec.Common.Logging.LogSystem.Debug("gridViewHisContactPointList_PopupMenuShowing.4");
                //    Inventec.Common.Logging.LogSystem.Debug("e.Allow:" + e.Allow);
                //}
                //Inventec.Common.Logging.LogSystem.Debug("gridViewHisContactPointList_PopupMenuShowing.5");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewHisContactPointList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    //DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    var data = (V_HIS_CONTACT_POINT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + start;
                        }
                        else if (e.Column.FieldName == "DOB_ST")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)(data.DOB ?? 0));
                        }
                        else if (e.Column.FieldName == "GENDER_NAME")
                        {
                            var gender = (ListGender != null && data.GENDER_ID.HasValue) ? ListGender.FirstOrDefault(o => o.ID == data.GENDER_ID) : null;
                            e.Value = gender != null ? gender.GENDER_NAME : "";
                        }
                        else if (e.Column.FieldName == "CONTACT_LEVEL_DISPLAY")
                        {
                            if (data.CONTACT_LEVEL != null)
                            {
                                e.Value = String.Format("F{0}", data.CONTACT_LEVEL);
                            }
                        }
                        else if (e.Column.FieldName == "FULL_NAME_DISPLAY")
                        {
                            if (data.CONTACT_TYPE == IMSys.DbConfig.HIS_RS.HIS_CONTACT_POINT.CONTACT_TYPE__NV)
                            {
                                var user = this.ListAcsUser != null && this.ListAcsUser.Count > 0 ? this.ListAcsUser.FirstOrDefault(o => o.LOGINNAME == data.LOGINNAME) : null;
                                if (user != null)
                                {
                                    e.Value = user.USERNAME;
                                }
                            }
                            else
                            {
                                e.Value = data.FULL_NAME;
                            }
                        }
                        else if (e.Column.FieldName == "CONTACT_TYPE_NAME")
                        {
                            if (data.CONTACT_TYPE == IMSys.DbConfig.HIS_RS.HIS_CONTACT_POINT.CONTACT_TYPE__BN)
                            {
                                e.Value = "Bệnh nhân";
                            }
                            else if (data.CONTACT_TYPE == IMSys.DbConfig.HIS_RS.HIS_CONTACT_POINT.CONTACT_TYPE__NV)
                            {
                                e.Value = "Nhân viên";
                            }
                            else if (data.CONTACT_TYPE == IMSys.DbConfig.HIS_RS.HIS_CONTACT_POINT.CONTACT_TYPE__KHAC)
                            {
                                e.Value = data.CONTACT_POINT_OTHER_TYPE_NAME;
                            }
                        }
                        //else if (e.Column.FieldName == "CREATE_TIME_ST")
                        //{
                        //    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        //}
                        //else if (e.Column.FieldName == "MODIFY_TIME_ST")
                        //{
                        //    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        //}                       
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewHisContactPointList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                //DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    short isPause = Inventec.Common.TypeConvert.Parse.ToInt16((gridViewHisContactPointList.GetRowCellValue(e.RowHandle, "IS_PAUSE") ?? "").ToString());
                    short isAutoDiscount = Inventec.Common.TypeConvert.Parse.ToInt16((gridViewHisContactPointList.GetRowCellValue(e.RowHandle, "IS_AUTO_DISCOUNT") ?? "").ToString());

                    string departmentIds = (gridViewHisContactPointList.GetRowCellValue(e.RowHandle, "DEPARTMENT_IDS") ?? "").ToString();
                    //bool AssignService = false;
                    bool isfinishButton = false;
                    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    if (e.Column.FieldName == "Finish")
                    {
                        e.RepositoryItem = repositoryItembtnFinish;
                        //                        isfinishButton = (isPause != 1) && (CheckLoginAdmin.IsAdmin(loginName) || IsStayingDepartment(departmentIds));
                        //                        e.RepositoryItem = (isPause != 1) && (CheckLoginAdmin.IsAdmin(loginName) || IsStayingDepartment(departmentIds)) ? repositoryItembtnFinish :
                        //(isPause == 1 && (CheckLoginAdmin.IsAdmin(loginName) || IsStayingDepartment(departmentIds)) ? repositoryItembtnUnFinish :
                        //(isPause == 1 ? repositoryItembtnUnFinish_Disable : repositoryItembtnFinish_Disable));
                    }
                    else if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = BtnDelete_Enable;
                        isDelete = true;

                        //if (this.controlDelete != null)
                        //{
                        //    if (this.controlDelete.IS_ANONYMOUS == 1)
                        //    {
                        //        e.RepositoryItem = BtnDelete_Enable;
                        //        isDelete = true;
                        //    }
                        //    else
                        //    {
                        //        if (RoleUse != null && ControlRule != null && RoleUse.Count > 0 && ControlRule.Count > 0)
                        //        {
                        //            var CheckRole = this.RoleUse.Where(o => this.ControlRule.Select(p => p.ROLE_ID).Contains(o.ROLE_ID)).ToList();
                        //            if ((CheckRole != null && CheckRole.Count > 0))
                        //            {
                        //                e.RepositoryItem = BtnDelete_Enable;
                        //                isDelete = true;
                        //            }
                        //            else
                        //            {
                        //                e.RepositoryItem = BtnDelete_Disable;
                        //                isDelete = false;
                        //            }
                        //        }
                        //        else
                        //        {
                        //            e.RepositoryItem = BtnDelete_Disable;
                        //            isDelete = false;
                        //        }

                        //    }
                        //}
                    }
                    //else if (e.Column.FieldName == "IS_AUTO_DISCOUNT_DISPLAY")
                    //{
                    //    if (isAutoDiscount == 1)
                    //    {
                    //        e.RepositoryItem = ButtonEditIsAutoDiscount;
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void BtnFind(object sender, EventArgs e)
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnRefreshs()
        {
            try
            {
                if (btnRefresh.Enabled)
                {
                    BtnRefresh_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetDefaultControl();
                //txtPatientName.Text = "";
                //txtLoginname.Text = "";
                //txtPatientCode.Text = "";
                cboDepartment.ShowPopup();
                cboDepartment.ClosePopup();
                FillDataToGrid();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnSearch()
        {
            try
            {
                if (btnFind.Enabled)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FillDataToGrid();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatient_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnFind_Click(null, null);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnFind_Click(null, null);
            }
        }

        private void toolTipController1_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlHisContactPointList)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlHisContactPointList.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;

                            string text = "";
                            if (info.Column.FieldName == "FULL_NAME")
                            {
                                text = (view.GetRowCellValue(lastRowHandle, "FULL_NAME") ?? "").ToString();
                            }
                            else if (info.Column.FieldName == "ST_DEPLAY")
                            {
                                short status_ispause = Inventec.Common.TypeConvert.Parse.ToInt16((view.GetRowCellValue(lastRowHandle, "IS_PAUSE") ?? "-1").ToString());
                                decimal status_islock = Inventec.Common.TypeConvert.Parse.ToDecimal((view.GetRowCellValue(lastRowHandle, "IS_ACTIVE") ?? "-1").ToString());
                                short status_islockhein = Inventec.Common.TypeConvert.Parse.ToInt16((view.GetRowCellValue(lastRowHandle, "IS_LOCK_HEIN") ?? "-1").ToString());
                                //Status
                                //1- dang dieu tri
                                //2- da ket thuc
                                //3- khóa hồ sơ
                                //4- duyệt bhyt
                                if (status_islockhein != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                {
                                    if (status_islock == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                    {
                                        if (status_ispause != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                        {
                                            text = "Đang điều trị";
                                        }
                                        else
                                        {
                                            text = "Kết thúc điều trị";
                                        }
                                    }
                                    else
                                    {
                                        text = "Khóa hồ sơ";
                                    }
                                }
                                else
                                {
                                    text = "Duyệt bảo hiểm y tế";
                                }
                            }
                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEndDepartment_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string endDepartment = "";
                if (_EndDepartmentSelecteds != null && _EndDepartmentSelecteds.Count > 0)
                {
                    foreach (var item in _EndDepartmentSelecteds)
                    {
                        endDepartment += item.DEPARTMENT_NAME + ", ";
                    }
                }

                e.DisplayText = endDepartment;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboEndDepartment_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboDepartment.Enabled = false;
                cboDepartment.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewHisContactPointList_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (hi.InRowCell)
                    {
                        var treatmentData = (V_HIS_CONTACT_POINT)view.GetRow(hi.RowHandle);
                        string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        short isPause = Inventec.Common.TypeConvert.Parse.ToInt16((view.GetRowCellValue(hi.RowHandle, "IS_PAUSE") ?? "").ToString());
                        string departmentIds = (view.GetRowCellValue(hi.RowHandle, "DEPARTMENT_IDS") ?? "").ToString();
                        bool AssignService = false;
                        bool isfinishButton = false;

                        if (treatmentData != null)
                        {
                            if (hi.Column.FieldName == "Finish")
                            {
                                #region ----- Kết thúc điều trị -----
                                if (treatmentData != null)
                                {
                                    repositoryItembtnFinish_Click(treatmentData);
                                }
                                #endregion
                            }
                            if (hi.Column.FieldName == "Delete")
                            {
                                #region ----- Xóa -----
                                //if (treatmentData != null && CheckLoginAdmin.IsAdmin(loginName))
                                if (isDelete)
                                {
                                    BtnDelete_Enable_ButtonClick(treatmentData);
                                }
                                #endregion
                            }
                            else if (hi.Column.FieldName == "ServiceReqList")
                            {
                                #region ----- Danh sách yêu cầu -----
                                if (treatmentData != null)
                                {
                                    repositoryItembtnServiceReqList_Click(treatmentData);
                                }
                                #endregion
                            }
                            //else if (hi.Column.FieldName == "Edit")
                            //{
                            //    #region ----- Sửa hsdt -----
                            //    if (treatmentData != null)
                            //    {
                            //        repositoryItembtnEditTreatment_btnClick(treatmentData);
                            //    }
                            //    #endregion
                            //}
                            //else if (hi.Column.FieldName == "PaySereServ")
                            //{
                            //    #region ----- Bảng kê thanh toán -----
                            //    if (treatmentData != null)
                            //    {
                            //        repositoryItembtnPaySereServ_Click(treatmentData);
                            //    }
                            //    #endregion
                            //}
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboEndDepartment_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDepartment.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtLoginname_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void BtnDelete_Enable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            var row = (MOS.EFMODEL.DataModels.V_HIS_CONTACT_POINT)gridViewHisContactPointList.GetFocusedRow();
            if (row != null)
            {
                BtnDelete_Enable_ButtonClick(row);
            }
        }

        private void BtnDelete_Enable_ButtonClick(V_HIS_CONTACT_POINT data)
        {
            try
            {
                if (data == null) return;
                if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có chắc chắn muốn xóa dữ liệu?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    CommonParam param = new CommonParam();
                    //HIS_CONTACT_POINT treatment = new HIS_CONTACT_POINT();
                    //Inventec.Common.Mapper.DataObjectMapper.Map<HIS_CONTACT_POINT>(treatment, data);//TODO

                    WaitingManager.Show();

                    var rs = new BackendAdapter(param).Post<bool>("api/HisContactPoint/Delete", ApiConsumers.MosConsumer, data.ID, param);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data.ID), data.ID)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
                    if (rs)
                    {
                        FillDataToGrid();
                    }
                    WaitingManager.Hide();

                    MessageManager.Show(this.ParentForm, param, rs);
                    SessionManager.ProcessTokenLost(param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewHisContactPointList_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                try
                {
                    //if (e.RowHandle < 0)
                    //    return;
                    //int rowHandleSelected = gridViewHisContactPointList.GetVisibleRowHandle(e.RowHandle);
                    //var data = (V_HIS_TREATMENT_4)gridViewHisContactPointList.GetRow(rowHandleSelected);
                    //if (data != null && data.APPROVAL_STORE_STT_ID == 2)
                    //{
                    //    e.Appearance.ForeColor = Color.Red;
                    //}
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridControlHisContactPointList_MouseClick(object sender, MouseEventArgs e)
        {
            //if (e.Button.IsRight())
            //{                
            //    ContextMenuStrip mnu = new ContextMenuStrip();
            //    ToolStripMenuItem mnuCopy = new ToolStripMenuItem("Copy");
            //    ToolStripMenuItem mnuCut = new ToolStripMenuItem("Cut");
            //    ToolStripMenuItem mnuPaste = new ToolStripMenuItem("Paste");
            //    //Assign event handlers
            //    mnuCopy.Click += new EventHandler(mnuCopy_Click);
            //    mnuCut.Click += new EventHandler(mnuCut_Click);
            //    mnuPaste.Click += new EventHandler(mnuPaste_Click);
            //    //Add to main context menu
            //    mnu.Items.AddRange(new ToolStripItem[] { mnuCopy, mnuCut, mnuPaste });
            //    //Assign to datagridview
            //    mnu.Show(e.Location);
            //}
        }

        void LoadDefaultData()
        {
            try
            {
                //btnCodeFind.Text = typeCodeFind__KeyWork_InDate;
                //FormatDtIntructionDate();
                //FormatDtIntructionOutDate();
                //dtCreatefrom.Properties.VistaDisplayMode = DefaultBoolean.True;
                //dtCreatefrom.Properties.VistaEditTime = DefaultBoolean.True;
                //dtCreateTo.Properties.VistaDisplayMode = DefaultBoolean.True;
                //dtCreateTo.Properties.VistaEditTime = DefaultBoolean.True;
                //dtCreatefrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0);
                //dtCreateTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0);
                //dtInHospital.DateTime = DateTime.Now;
                //dtOutHospital.DateTime = null;
                //cboFind.SelectedIndex = 0;
                //cboTreatmentType.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinFFrom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //spinFTo.Properties.MaxValue = spinFFrom.Value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinFFrom_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void spinFTo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

    }
}
