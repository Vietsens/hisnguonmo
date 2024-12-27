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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.ADO;
using HIS.Desktop.Utility;
using DevExpress.XtraEditors;
using HIS.Desktop.Plugins.SurgServiceReqExecute.Base;
using DevExpress.XtraGrid.Columns;
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.ApiConsumer;
using ACS.EFMODEL.DataModels;
using Inventec.Common.Controls.EditorLoader;
using MOS.Filter;
using Inventec.Common.Adapter;
using AutoMapper;
using ACS.Filter;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Plugins.SurgServiceReqExecute.Config;
namespace HIS.Desktop.Plugins.SurgServiceReqExecute
{
    public partial class UCEkipUser : UserControlBase
    {
        List<AcsUserADO> AcsUserADOList { get; set; }
        List<HIS_DEPARTMENT> departmentClinic { get; set; }
        long? DepartmentId;
        List<HIS_EXECUTE_ROLE_USER> executeRoleUsers { get; set; }

        public UCEkipUser()
        {
            InitializeComponent();
            this.SetCaptionByLanguageKey();
            AcsUserADOList = ProcessAcsUser();
            LoadDataToComboDepartment();
            ComboAcsUser();
            ComboExecuteRole();
            LoadExecuteRoleUser();

        }

        public void FillDataToGrid(List<HisEkipUserADO> lst)
        {
            try
            {
                if (lst != null && lst.Count > 0)
                {
                    int index = 0;
                    lst.ForEach(o =>
                    {
                        o.IsMinus = true;
                        if (index == 0)
                            o.IsPlus = true;
                        else
                            o.IsPlus = false;
                        index++;
                    });
                }
                grdControlInformationSurg.DataSource = new List<HisEkipUserADO>();
                grdControlInformationSurg.DataSource = lst;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FillDataToGridDepartment()
        {
            try
            {
                if (this.DepartmentId != null && this.DepartmentId > 0)
                {
                    var dataEkipList = (List<HisEkipUserADO>)grdControlInformationSurg.DataSource;
                    if (dataEkipList != null && dataEkipList.Count > 0)
                    {
                        Parallel.ForEach(dataEkipList.Where(f => f.ID >= 0), l => l.DEPARTMENT_ID = this.DepartmentId);
                    }

                    grdViewInformationSurg.BeginDataUpdate();
                    dataEkipList.ForEach(o => o.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                    dataEkipList.LastOrDefault().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                    FillDataToGrid(dataEkipList);
                    grdViewInformationSurg.EndDataUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FillDataToInformationSurgFromSereServLast(List<HisEkipUserADO> ekipUserAdos, HIS_SERE_SERV sereServLast)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisEkipUserViewFilter hisEkipUserFilter = new HisEkipUserViewFilter();
                hisEkipUserFilter.EKIP_ID = sereServLast.EKIP_ID;
                hisEkipUserFilter.ORDER_DIRECTION = "ASC";
                hisEkipUserFilter.ORDER_FIELD = "ID";
                var lst = new BackendAdapter(param)
        .Get<List<V_HIS_EKIP_USER>>(HisRequestUriStore.HIS_EKIP_USER_GETVIEW, ApiConsumers.MosConsumer, hisEkipUserFilter, param);
                lst = lst.Where(o => o.IS_ACTIVE == 1).ToList();
                if (lst.Count > 0)
                {
                    foreach (var item in lst)
                    {
                        var dataCheck = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().FirstOrDefault(p => p.ID == item.EXECUTE_ROLE_ID && p.IS_ACTIVE == 1);
                        if (dataCheck == null || dataCheck.ID == 0)
                            continue;
                        Mapper.CreateMap<V_HIS_EKIP_USER, HisEkipUserADO>();
                        var HisEkipUserProcessing = Mapper.Map<V_HIS_EKIP_USER, HisEkipUserADO>(item);
                        SetDepartment(HisEkipUserProcessing);
                        ekipUserAdos.Add(HisEkipUserProcessing);
                    }
                    FillDataToGrid(ekipUserAdos);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FillDataToInformationSurgFromServiceReqEkipPlan(List<HisEkipUserADO> ekipUserAdos, V_HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisEkipPlanUserViewFilter hisEkipPlanUserFilter = new HisEkipPlanUserViewFilter();
                hisEkipPlanUserFilter.EKIP_PLAN_ID = serviceReq.EKIP_PLAN_ID;
                var lst = new BackendAdapter(param)
        .Get<List<V_HIS_EKIP_PLAN_USER>>("api/HisEkipPlanUser/GetView", ApiConsumers.MosConsumer, hisEkipPlanUserFilter, param);

                if (lst.Count > 0)
                {
                    foreach (var item in lst)
                    {
                        var dataCheck = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().FirstOrDefault(p => p.ID == item.EXECUTE_ROLE_ID && p.IS_ACTIVE == 1);
                        if (dataCheck == null || dataCheck.ID == 0)
                            continue;
                        Mapper.CreateMap<V_HIS_EKIP_PLAN_USER, HisEkipUserADO>();
                        var HisEkipUserProcessing = Mapper.Map<V_HIS_EKIP_PLAN_USER, HisEkipUserADO>(item);
                        //if (item != lst[0])
                        //{
                        //    HisEkipUserProcessing.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                        //}
                        HisEkipUserProcessing.DEPARTMENT_ID = null;
                        SetDepartment(HisEkipUserProcessing);
                        ekipUserAdos.Add(HisEkipUserProcessing);
                    }
                    FillDataToGrid(ekipUserAdos);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FillDataToInformationSurg(bool? isClick = null)
        {
            try
            {
                var lstX = grdControlInformationSurg.DataSource as List<HisEkipUserADO>;
                if (grdControlInformationSurg == null
              || grdControlInformationSurg.DataSource == null
               || lstX == null
               || lstX.Count <= 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("FillDataToInformationSurg_____________1");
                    List<HisEkipUserADO> ekipUserAdoTemps = new List<HisEkipUserADO>();
                    //ekipUserAdoTemps.ForEach(o => o.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                    //ekipUserAdoTemps.LastOrDefault().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                    string executeRoleDefault = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeys.HIS_DESKTOP_SURGSERVICEREQEXECUTE_EXECUTE_ROLE_DEFAULT);
                    List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE> executeRoles = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE>();
                    ekipUserAdoTemps = ekipUserAdoTemps.Where(o => o.IS_ACTIVE == 1).ToList();
                    if (!String.IsNullOrEmpty(executeRoleDefault))
                    {
                        string[] str = executeRoleDefault.Split(',');
                        List<string> executeRoleCodes = new List<string>(str);

                        foreach (var item in executeRoleCodes)
                        {
                            var executeRoleCheck = executeRoles.FirstOrDefault(o => o.EXECUTE_ROLE_CODE == item);
                            if (executeRoleCheck == null)
                                continue;

                            var dataCheck = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().FirstOrDefault(p => p.ID == executeRoleCheck.ID && p.IS_ACTIVE == 1);
                            if (dataCheck == null || dataCheck.ID == 0)
                                continue;
                            HisEkipUserADO ekipUserAdoTemp = new HisEkipUserADO();
                            ekipUserAdoTemp.EXECUTE_ROLE_ID = executeRoleCheck.ID;

                            SetDepartment(ekipUserAdoTemp);
                            ekipUserAdoTemps.Add(ekipUserAdoTemp);
                        }
                    }
                    else
                    {
                        HisEkipUserADO ekipUserAdoTemp = new HisEkipUserADO();
                        ekipUserAdoTemps.Add(ekipUserAdoTemp);
                    }
                    if (ekipUserAdoTemps == null || ekipUserAdoTemps.Count == 0)
                    {
                        HisEkipUserADO ekipUserAdoTemp = new HisEkipUserADO();
                        ekipUserAdoTemps.Add(ekipUserAdoTemp);
                    }
                    Inventec.Common.Logging.LogSystem.Warn("FillDataToInformationSurg_____________2");
                    if (isClick == null || isClick.Value == false)
                        FillDataToGrid(ekipUserAdoTemps);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public List<HisEkipUserADO> GetDataSource()
        {
            try
            {
                return grdControlInformationSurg.DataSource as List<HisEkipUserADO>;
            }
            catch (Exception ex)
            {
                return new List<HisEkipUserADO>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void LoadGridEkipUserFromTemp(long ekipTempId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisEkipTempUserFilter filter = new HisEkipTempUserFilter();
                filter.EKIP_TEMP_ID = ekipTempId;
                List<HIS_EKIP_TEMP_USER> ekipTempUsers = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_EKIP_TEMP_USER>>("api/HisEkipTempUser/Get", ApiConsumers.MosConsumer, filter, param);

                if (ekipTempUsers != null && ekipTempUsers.Count > 0)
                {
                    List<HisEkipUserADO> ekipUserAdoTemps = new List<HisEkipUserADO>();
                    List<string> loginNames = ekipTempUsers.Select(o => o.LOGINNAME).ToList();
                    List<ACS.EFMODEL.DataModels.ACS_USER> isActive = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => loginNames.Exists(p => p == o.LOGINNAME)).ToList();
                    List<string> isActiveLoginName = isActive.Where(o => o.IS_ACTIVE == 1).Select(i => i.LOGINNAME).ToList();
                    foreach (var ekipTempUser in ekipTempUsers)
                    {
                        var dataCheck = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().FirstOrDefault(p => p.ID == ekipTempUser.EXECUTE_ROLE_ID && p.IS_ACTIVE == 1);
                        if (dataCheck == null || dataCheck.ID == 0)
                            continue;
                        HisEkipUserADO ekipUserAdoTemp = new HisEkipUserADO();
                        ekipUserAdoTemp.EXECUTE_ROLE_ID = ekipTempUser.EXECUTE_ROLE_ID;
                        ekipUserAdoTemp.LOGINNAME = ekipTempUser.LOGINNAME;
                        ekipUserAdoTemp.USERNAME = ekipTempUser.USERNAME;
                        ekipUserAdoTemp.DEPARTMENT_ID = ekipTempUser.DEPARTMENT_ID;
                        //AcsUserFilter acsFilter = new AcsUserFilter();
                        //acsFilter.LOGINNAME = ekipUserAdoTemp.LOGINNAME;
                        //List<ACS.EFMODEL.DataModels.ACS_USER> isActive = new BackendAdapter(param).Get<List<ACS.EFMODEL.DataModels.ACS_USER>>("api/AcsUser/Get", ApiConsumers.AcsConsumer, acsFilter, param);
                        //isActive = isActive.Where(o => o.IS_ACTIVE == 1).ToList();
                        //if (isActive != null && isActive.Count > 0)
                        //{
                        if (isActiveLoginName.Contains(ekipTempUser.LOGINNAME))
                        {
                            SetDepartment(ekipUserAdoTemp);
                            ekipUserAdoTemps.Add(ekipUserAdoTemp);
                        }

                        //}
                        //SetDepartment(ekipUserAdoTemp);                      
                        //ekipUserAdoTemps.Add(ekipUserAdoTemp);
                    }
                    int index = 0;
                    ekipUserAdoTemps.ForEach(o =>
                    {
                        o.IsMinus = true;
                        if (index == 0)
                            o.IsPlus = true;
                        else
                            o.IsPlus = false;
                        index++;
                    });
                    grdControlInformationSurg.DataSource = ekipUserAdoTemps;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetColorTitle(bool isRed)
        {
            try
            {
                if (!isRed)
                {
                    lciInformationSurg.AppearanceItemCaption.ForeColor = Color.Black;
                }
                else
                {
                    lciInformationSurg.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetEnableControl(bool enable)
        {
            try
            {
                grdViewInformationSurg.OptionsBehavior.ReadOnly = enable;
                grdViewInformationSurg.OptionsCustomization.AllowFilter = !enable;
                grdViewInformationSurg.OptionsCustomization.AllowSort = !enable;
                grdViewInformationSurg.OptionsBehavior.Editable = !enable;
                btnAdd.ReadOnly = enable;
                btnDelete.ReadOnly = enable;
                grdViewInformationSurg.RefreshData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void EnableControl(bool enable)
        {
            try
            {

                lciInformationSurg.Enabled = enable;


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task ComboExecuteRole()
        {
            try
            {
                List<HIS_EXECUTE_ROLE> datas = null;
                if (BackendDataWorker.IsExistsKey<HIS_EXECUTE_ROLE>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EXECUTE_ROLE>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_EXECUTE_ROLE>>("api/HisExecuteRole/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(HIS_EXECUTE_ROLE), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                if (datas != null && datas.Count > 0)
                {
                    datas = datas.Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && p.IS_DISABLE_IN_EKIP != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROLE_NAME", "ID", columnInfos, false, 250);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboPosition, datas, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grdViewInformationSurg_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                var data = (HisEkipUserADO)grdViewInformationSurg.GetFocusedRow();

                if (e.Column.FieldName == "LOGINNAME")
                {
                    //this.grdControlInformationSurg.RefreshDataSource();
                    SetDepartment(data);
                    this.grdControlInformationSurg.RefreshDataSource();
                }else if(e.Column.FieldName == "EXECUTE_ROLE_ID")
                {
                    int visibleIndex = grdViewInformationSurg.FocusedColumn.VisibleIndex;
                    int newVisibleIndex = visibleIndex + 1;
                    grdViewInformationSurg.FocusedColumn = grdViewInformationSurg.VisibleColumns[newVisibleIndex];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grdViewInformationSurg_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "BtnAdd")
                {
                    int rowSelected = Convert.ToInt32(e.RowHandle);
                    bool IsPlus = Inventec.Common.TypeConvert.Parse.ToBoolean((grdViewInformationSurg.GetRowCellValue(e.RowHandle, "IsPlus") ?? "").ToString());
                    if (IsPlus)
                    {
                        e.RepositoryItem = btnAdd;
                    }
                }
                else if (e.Column.FieldName == "BtnDelete")
                {
                    int rowSelected = Convert.ToInt32(e.RowHandle);
                    bool IsPlus = Inventec.Common.TypeConvert.Parse.ToBoolean((grdViewInformationSurg.GetRowCellValue(e.RowHandle, "IsMinus") ?? "").ToString());
                    if (IsPlus)
                    {
                        e.RepositoryItem = btnDelete;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grdViewInformationSurg_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "USERNAME")
                    {
                        try
                        {
                            string status = (view.GetRowCellValue(e.ListSourceRowIndex, "LOGINNAME") ?? "").ToString();
                            ACS.EFMODEL.DataModels.ACS_USER data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS_USER>().SingleOrDefault(o => o.LOGINNAME == status && o.IS_ACTIVE == 1);
                            e.Value = data.USERNAME;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi hien thi gia tri cot USERNAME", ex);
                        }
                    }
                    //if (e.Column.FieldName == "DOB")
                    //{
                    //    string status = (view.GetRowCellValue(e.ListSourceRowIndex, "LOGINNAME") ?? "").ToString();

                    //    MOS.EFMODEL.DataModels.HIS_EMPLOYEE dataa = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMPLOYEE>().SingleOrDefault(o => o.LOGINNAME == status && o.IS_ACTIVE == 1);
                    //    e.Value = dataa.DOB;
                    //}
                    //if (e.Column.FieldName == "DIPLOMA ")
                    //{
                    //    string status = (view.GetRowCellValue(e.ListSourceRowIndex, "LOGINNAME") ?? "").ToString();

                    //    MOS.EFMODEL.DataModels.HIS_EMPLOYEE dataa_ = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMPLOYEE>().SingleOrDefault(o => o.LOGINNAME == status && o.IS_ACTIVE == 1);
                    //    e.Value = dataa_.DIPLOMA;
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grdViewInformationSurg_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            try
            {
                if (e.FocusedColumn.FieldName == "LOGINNAME")
                {
                    grdViewInformationSurg.ShowEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grdViewInformationSurg_ShownEditor(object sender, EventArgs e)
        {

            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                HisEkipUserADO data = view.GetFocusedRow() as HisEkipUserADO;
                if (view.FocusedColumn.FieldName == "LOGINNAME" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    LoadDataToUser(data, editor);
                    grdViewInformationSurg.RefreshData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToUser(HisEkipUserADO data, GridLookUpEdit editor)
        {
            try
            {
                List<string> loginNames = new List<string>();


                if (data != null && data.EXECUTE_ROLE_ID > 0)
                {

                    var executeRoleUserTemps = executeRoleUsers != null ? executeRoleUsers.Where(o => o.EXECUTE_ROLE_ID == data.EXECUTE_ROLE_ID).ToList() : null;
                    if (executeRoleUserTemps != null && executeRoleUserTemps.Count > 0)
                    {
                        loginNames = executeRoleUserTemps.Select(o => o.LOGINNAME).Distinct().ToList();
                    }
                    if (data.LOGINNAME != null)
                    {
                        if (HisConfigCFG.SURG_SERVICE_REQ_EXECUTE_ROLE_USER_OPTION == "1")
                        {
                            if (loginNames.Contains(data.LOGINNAME))
                            {
                                editor.EditValue = data.LOGINNAME;
                            }
                            else
                            {
                                editor.EditValue = null;
                                data.LOGINNAME = null;
                            }
                        }
                        else
                        {
                            editor.EditValue = data.LOGINNAME;
                        }
                    }

                }
                ComboAcsUser(editor, loginNames);


                SetDepartment(data);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetDepartmentID(long? id)
        {
            try
            {
                this.DepartmentId = id;
                Inventec.Common.Logging.LogSystem.Warn("SetDepartmentID_____________" + DepartmentId);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetDepartment(HisEkipUserADO data)
        {
            try
            {
                if (data == null)
                    return;

                if (data.DEPARTMENT_ID.HasValue && data.DEPARTMENT_ID.Value > 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("data.DEPARTMENT_ID.HasValue_____________" + data.DEPARTMENT_ID);
                    //if (!departmentClinic.Select(o => o.ID.ToString()).ToList().Contains(data.DEPARTMENT_ID.ToString()))
                    //{
                    //    Inventec.Common.Logging.LogSystem.Warn("data.DEPARTMENT_ID.HasValue _____________ !CONTAINS");
                    //    data.DEPARTMENT_ID = null;
                    //    data.DEPARTMENT_NAME = "";
                    //}

                    return;
                }
                Inventec.Common.Logging.LogSystem.Warn("SetDepartment_____________" + DepartmentId);
                if (DepartmentId != null && DepartmentId > 0)
                {
                    data.DEPARTMENT_ID = DepartmentId;
                    // data.DEPARTMENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboDepartment.EditValue.ToString());

                }
                else
                {
                    //var employee = BackendDataWorker.Get<HIS_EMPLOYEE>().FirstOrDefault(o => !String.IsNullOrWhiteSpace(data.LOGINNAME) && o.LOGINNAME.ToLower() == data.LOGINNAME.ToLower());
                    //if (employee != null)
                    //{
                    //    data.DEPARTMENT_ID = employee.DEPARTMENT_ID;
                    //    var department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == employee.DEPARTMENT_ID);
                    //    data.DEPARTMENT_NAME = department != null ? department.DEPARTMENT_NAME : "";

                    //}
                    //else
                    //{
                    data.DEPARTMENT_ID = null;
                    data.DEPARTMENT_NAME = "";

                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ComboAcsUser(GridLookUpEdit cbo, List<string> loginNames)
        {
            try
            {
                List<AcsUserADO> acsUserAlows = new List<AcsUserADO>();
                //BackendDataWorker.Get<HIS_EMPLOYEE>().FirstOrDefault(o => o.ID == this.serviceReq.EXECUTE_DEPARTMENT_ID);
                if (loginNames != null && loginNames.Count > 0)
                {
                    acsUserAlows = this.AcsUserADOList.Where(o => loginNames.Contains(o.LOGINNAME) && o.IS_ACTIVE == 1).ToList();
                }
                else if (HisConfigCFG.SURG_SERVICE_REQ_EXECUTE_ROLE_USER_OPTION == "1")
                {
                    acsUserAlows = null;
                }
                else
                {
                    acsUserAlows = this.AcsUserADOList.Where(o => o.IS_ACTIVE == 1).ToList();
                }

                cbo.Properties.DataSource = acsUserAlows;
                cbo.Properties.DisplayMember = "USERNAME";
                cbo.Properties.ValueMember = "LOGINNAME";

                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("LOGINNAME");
                aColumnCode.Caption = "Tài khoản";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cbo.Properties.View.Columns.AddField("USERNAME");
                aColumnName.Caption = "Họ tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;

                GridColumn aColumDOB = cbo.Properties.View.Columns.AddField("DOB");
                aColumDOB.Caption = "Ngày sinh";
                aColumDOB.Visible = true;
                aColumDOB.VisibleIndex = 3;
                aColumDOB.Width = 100;

                GridColumn aColumnDIPLOMA = cbo.Properties.View.Columns.AddField("DIPLOMA");
                aColumnDIPLOMA.Caption = "CCHN";
                aColumnDIPLOMA.Visible = true;
                aColumnDIPLOMA.VisibleIndex = 4;
                aColumnDIPLOMA.Width = 100;

                GridColumn aColumnDepartment = cbo.Properties.View.Columns.AddField("DEPARTMENT_NAME");
                aColumnDepartment.Caption = "Tên khoa";
                aColumnDepartment.Visible = true;
                aColumnDepartment.VisibleIndex = 5;
                aColumnDepartment.Width = 200;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<AcsUserADO> ProcessAcsUser()
        {
            List<AcsUserADO> AcsUserADOList = null;
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> datas = null;
                List<V_HIS_EMPLOYEE> employeeList = null;

                CommonParam paramCommon = new CommonParam();
                dynamic filter = new System.Dynamic.ExpandoObject();
                //datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                datas = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<ACS.EFMODEL.DataModels.ACS_USER>>("api/AcsUser/Get", ApiConsumers.AcsConsumer, filter, paramCommon);
                if (datas != null) BackendDataWorker.UpdateToRam(typeof(ACS.EFMODEL.DataModels.ACS_USER), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                employeeList = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE>>("api/HisEmployee/GetView", ApiConsumers.MosConsumer, filter, paramCommon);
                if (employeeList != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE), employeeList, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                var departmentList = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == 1 && o.IS_CLINICAL == 1).ToList();
                AcsUserADOList = new List<AcsUserADO>();

                foreach (var item in datas)
                {
                    if (item.LOGINNAME == "anhnp")
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item));
                    }
                    AcsUserADO user = new AcsUserADO();
                    user.ID = item.ID;
                    user.LOGINNAME = item.LOGINNAME;
                    user.USERNAME = item.USERNAME;
                    user.MOBILE = item.MOBILE;
                    user.PASSWORD = item.PASSWORD;
                    //user.IS_ACTIVE = item.IS_ACTIVE;

                    var check = employeeList.FirstOrDefault(o => o.LOGINNAME == item.LOGINNAME && o.IS_ACTIVE == 1);
                    if (check != null)
                    {
                        user.IS_ACTIVE = check.IS_ACTIVE;
                        user.DOB = Inventec.Common.DateTime.Convert.TimeNumberToDateString(check.DOB ?? 0);

                        user.DIPLOMA = check.DIPLOMA;
                        var checkDepartment = departmentList.FirstOrDefault(o => o.ID == check.DEPARTMENT_ID);

                        if (checkDepartment != null)
                        {
                            user.DEPARTMENT_NAME = checkDepartment.DEPARTMENT_NAME;

                        }
                    }
                    AcsUserADOList.Add(user);
                }

                AcsUserADOList = AcsUserADOList.OrderBy(o => o.USERNAME).ToList();
            }
            catch (Exception ex)
            {
                AcsUserADOList = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return AcsUserADOList;
        }

        private async Task ComboAcsUser()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                columnInfos.Add(new ColumnInfo("DOB", "", 100, 3));
                columnInfos.Add(new ColumnInfo("DIPLOMA", "", 100, 4));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 200, 5));

                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 800);
                ControlEditorLoader.Load(GridLookupEdit_UserName, this.AcsUserADOList, controlEditorADO);

                repositoryItemSearchLookUpEdit1.DataSource = this.AcsUserADOList;
                repositoryItemSearchLookUpEdit1.DisplayMember = "USERNAME";
                repositoryItemSearchLookUpEdit1.ValueMember = "LOGINNAME";

                repositoryItemSearchLookUpEdit1.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                repositoryItemSearchLookUpEdit1.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                repositoryItemSearchLookUpEdit1.ImmediatePopup = true;
                repositoryItemSearchLookUpEdit1.View.Columns.Clear();

                GridColumn aColumnCode = repositoryItemSearchLookUpEdit1.View.Columns.AddField("LOGINNAME");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = repositoryItemSearchLookUpEdit1.View.Columns.AddField("USERNAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;

                GridColumn aColumnDOB = repositoryItemSearchLookUpEdit1.View.Columns.AddField("DOB");
                aColumnDOB.Caption = "DOB";
                aColumnDOB.Visible = true;
                aColumnDOB.VisibleIndex = 3;
                aColumnDOB.Width = 100;

                GridColumn aColumnDIPLOMA = repositoryItemSearchLookUpEdit1.View.Columns.AddField("DIPLOMA");
                aColumnDOB.Caption = "CCHN";
                aColumnDOB.Visible = true;
                aColumnDOB.VisibleIndex = 4;
                aColumnDOB.Width = 100;



                GridColumn aColumnDepartment = repositoryItemSearchLookUpEdit1.View.Columns.AddField("DEPARTMENT_NAME");
                aColumnDepartment.Caption = "Khoa";
                aColumnDepartment.Visible = true;
                aColumnDepartment.VisibleIndex = 5;
                aColumnDepartment.Width = 200;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToComboDepartment()
        {
            try
            {
                departmentClinic = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_CLINICAL == 1 && o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 400);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(GridLookUpEdit_Department, departmentClinic, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadExecuteRoleUser()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Error("---S");
                HisExecuteRoleUserFilter filter = new HisExecuteRoleUserFilter();
                executeRoleUsers = BackendDataWorker.Get<HIS_EXECUTE_ROLE_USER>();
                Inventec.Common.Logging.LogSystem.Error("---F");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAdd_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                List<HisEkipUserADO> ekipUserAdoTemps = new List<HisEkipUserADO>();
                var ekipUsers = grdControlInformationSurg.DataSource as List<HisEkipUserADO>;

                HisEkipUserADO participant = new HisEkipUserADO();
                ekipUsers.Add(participant);
                grdControlInformationSurg.DataSource = null;
                int index = 0;
                ekipUsers.ForEach(o =>
                {
                    o.IsMinus = true;
                    if (index == 0)
                        o.IsPlus = true;
                    else
                        o.IsPlus = false;
                    index++;
                });
                grdControlInformationSurg.DataSource = ekipUsers;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var ekipUsers = grdControlInformationSurg.DataSource as List<HisEkipUserADO>;
                var ekipUser = (HisEkipUserADO)grdViewInformationSurg.GetFocusedRow();
                if (ekipUser != null)
                {
                    if (ekipUsers.Count > 1)
                    {
                        ekipUsers.Remove(ekipUser);                       
                    }
                    else if (ekipUsers.Count == 1)
                    {
                        ekipUsers = new List<HisEkipUserADO>();
                        ekipUsers.Add(new HisEkipUserADO());
                    }
                    grdControlInformationSurg.DataSource = null;
                    int index = 0;
                    ekipUsers.ForEach(o =>
                    {
                        o.IsMinus = true;
                        if (index == 0)
                            o.IsPlus = true;
                        else
                            o.IsPlus = false;
                        index++;
                    });
                    grdControlInformationSurg.DataSource = ekipUsers;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GridLookupEdit_UserName_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {

                grdViewInformationSurg.FocusedRowHandle = DevExpress.XtraGrid.GridControl.NewItemRowHandle;
                grdViewInformationSurg.FocusedColumn = grdViewInformationSurg.VisibleColumns[2];
                var loginName = grdViewInformationSurg.GetFocusedRowCellValue("LOGINNAME");
                var data = AcsUserADOList.Where(o => o.LOGINNAME.Equals(loginName)).ToList();
                if (data != null && data.Count > 0)
                {

                    if (!string.IsNullOrEmpty(data.FirstOrDefault().DEPARTMENT_NAME))
                    {
                        var departmentId = departmentClinic.Where(o => o.DEPARTMENT_NAME.Equals(data.FirstOrDefault().DEPARTMENT_NAME)).ToList();
                        if (departmentId != null && departmentId.Count > 0)
                        {

                            grdViewInformationSurg.SetFocusedRowCellValue("DEPARTMENT_ID", departmentId.FirstOrDefault().ID);
                        }
                        else
                        {
                            grdViewInformationSurg.SetFocusedRowCellValue("DEPARTMENT_ID", null);
                        }
                    }
                }
                else
                {
                    grdViewInformationSurg.SetFocusedRowCellValue("DEPARTMENT_ID", null);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grdControlInformationSurg_ProcessGridKey(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (grdViewInformationSurg.FocusedColumn.VisibleIndex != 0)
                {
                    if (grdViewInformationSurg.FocusedColumn.VisibleIndex == grdViewInformationSurg.VisibleColumns.Count - 1)
                    {
                        var data = (HisEkipUserADO)grdViewInformationSurg.GetFocusedRow();
                        if (data.IsPlus)
                        {

                            btnAdd_ButtonClick(null, null);
                            grdViewInformationSurg.FocusedRowHandle = grdViewInformationSurg.DataRowCount - 1;
                            int visibleIndex = grdViewInformationSurg.FocusedColumn.VisibleIndex;
                            int newVisibleIndex = visibleIndex + 1;
                            if (newVisibleIndex == grdViewInformationSurg.VisibleColumns.Count)
                                newVisibleIndex = 0;
                            grdViewInformationSurg.FocusedColumn = grdViewInformationSurg.VisibleColumns[newVisibleIndex];
                        }
                    }
                    else if (grdViewInformationSurg.FocusedColumn.VisibleIndex == grdViewInformationSurg.VisibleColumns.Count - 2)
                    {
                        btnDelete_ButtonClick(null, null);
                    }
                    else
                    {
                        int visibleIndex = grdViewInformationSurg.FocusedColumn.VisibleIndex;
                        int newVisibleIndex = visibleIndex + 1;
                        if (newVisibleIndex == grdViewInformationSurg.VisibleColumns.Count)
                            newVisibleIndex = 0;
                        grdViewInformationSurg.FocusedColumn = grdViewInformationSurg.VisibleColumns[newVisibleIndex];
                    }
                    //grdViewInformationSurg.FocusedColumn = grdViewInformationSurg.MoveNext(grdViewInformationSurg.FocusedColumn);
                }

            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCEkipUser
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource__UCEkipUser = new ResourceManager("HIS.Desktop.Plugins.SurgServiceReqExecute.Resources.Lang", typeof(UCEkipUser).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCEkipUser.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource__UCEkipUser, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCEkipUser.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource__UCEkipUser, LanguageManager.GetCulture());
                this.cboPosition.NullText = Inventec.Common.Resource.Get.Value("UCEkipUser.cboPosition.NullText", Resources.ResourceLanguageManager.LanguageResource__UCEkipUser, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCEkipUser.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource__UCEkipUser, LanguageManager.GetCulture());
                this.GridLookupEdit_UserName.NullText = Inventec.Common.Resource.Get.Value("UCEkipUser.GridLookupEdit_UserName.NullText", Resources.ResourceLanguageManager.LanguageResource__UCEkipUser, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCEkipUser.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource__UCEkipUser, LanguageManager.GetCulture());
                this.GridLookUpEdit_Department.NullText = Inventec.Common.Resource.Get.Value("UCEkipUser.GridLookUpEdit_Department.NullText", Resources.ResourceLanguageManager.LanguageResource__UCEkipUser, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCEkipUser.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource__UCEkipUser, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCEkipUser.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource__UCEkipUser, LanguageManager.GetCulture());
                this.repositoryItemGridLookUpEditUsername.NullText = Inventec.Common.Resource.Get.Value("UCEkipUser.repositoryItemGridLookUpEditUsername.NullText", Resources.ResourceLanguageManager.LanguageResource__UCEkipUser, LanguageManager.GetCulture());
                this.repositoryItemSearchLookUpEdit1.NullText = Inventec.Common.Resource.Get.Value("UCEkipUser.repositoryItemSearchLookUpEdit1.NullText", Resources.ResourceLanguageManager.LanguageResource__UCEkipUser, LanguageManager.GetCulture());
                this.lciInformationSurg.Text = Inventec.Common.Resource.Get.Value("UCEkipUser.lciInformationSurg.Text", Resources.ResourceLanguageManager.LanguageResource__UCEkipUser, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPosition_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                LookUpEdit edit = sender as LookUpEdit;
                if (edit == null) return;
                if (edit.EditValue != null)
                {
                    if ((edit.EditValue ?? 0).ToString() != (edit.OldEditValue ?? 0).ToString())
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = grdViewInformationSurg as DevExpress.XtraGrid.Views.Grid.GridView;
                        var data = (HisEkipUserADO)grdViewInformationSurg.GetFocusedRow();
                        data.EXECUTE_ROLE_ID = Convert.ToInt64(edit.EditValue);
                        GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                        if (editor != null)
                        {
                            LoadDataToUser(data, editor);
                        }
                        else
                        {
                            GridLookUpEdit newEditor = new GridLookUpEdit();
                            LoadDataToUser(data, newEditor);
                        }
                        if (data.LOGINNAME == null && (HisConfigCFG.SURG_SERVICE_REQ_EXECUTE_ROLE_USER_OPTION == "1"))
                        {
                            view.SetRowCellValue(view.FocusedRowHandle, view.Columns["LOGINNAME"], null);
                        }

                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
