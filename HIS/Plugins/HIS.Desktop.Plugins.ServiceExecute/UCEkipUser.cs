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
using HIS.Desktop.Plugins.ServiceExecute.Base;
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
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.LocalStorage.HisConfig;

namespace HIS.Desktop.Plugins.ServiceExecute
{
    public partial class UCEkipUser : UserControlBase
    {
        List<AcsUserADO> AcsUserADOList { get; set; }
        List<HIS_DEPARTMENT> departmentClinic { get; set; }
        long? DepartmentId;
        List<HIS_EXECUTE_ROLE_USER> executeRoleUsers { get; set; }
        List<HIS_EXECUTE_ROLE> lstExecuteRole { get; set; }
        Action<int> Plus_Click;
        Action<int> Delete_Click;
        Action<int> NumExe;
        int num = 1;
        List<ACS.EFMODEL.DataModels.ACS_USER> datasAcsUser;
        public UCEkipUser(Action<int> Plus_Click, Action<int> Delete_Click, Action<int> NumExe, int num, List<HIS_EXECUTE_ROLE> lstExecuteRole)
        {
            InitializeComponent();
            this.SetCaptionByLanguageKey();
            this.Plus_Click = Plus_Click;
            this.Delete_Click = Delete_Click;
            this.NumExe = NumExe;
            this.num = num;
            this.lstExecuteRole = lstExecuteRole;
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
                    //Cập nhật lại Action
                    for (int i = 0; i < lst.Count; i++)
                    {
                        if (i == 0)
                            lst[i].Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                        else
                            lst[i].Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                    }
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
                        var dataCheck = lstExecuteRole.FirstOrDefault(p => p.ID == item.EXECUTE_ROLE_ID && p.IS_ACTIVE == 1);
                        if (dataCheck == null || dataCheck.ID == 0)
                            continue;
                        Mapper.CreateMap<V_HIS_EKIP_USER, HisEkipUserADO>();
                        var HisEkipUserProcessing = Mapper.Map<V_HIS_EKIP_USER, HisEkipUserADO>(item);
                        //if (item != lst[0])
                        //{
                        //    HisEkipUserProcessing.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                        //}
                        if (ekipUserAdos.Count == 0)
                        {
                            HisEkipUserProcessing.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                        }
                        else
                        {
                            HisEkipUserProcessing.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                        }
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
                        var dataCheck = lstExecuteRole.FirstOrDefault(p => p.ID == item.EXECUTE_ROLE_ID && p.IS_ACTIVE == 1);
                        if (dataCheck == null || dataCheck.ID == 0)
                            continue;
                        Mapper.CreateMap<V_HIS_EKIP_PLAN_USER, HisEkipUserADO>();
                        var HisEkipUserProcessing = Mapper.Map<V_HIS_EKIP_PLAN_USER, HisEkipUserADO>(item);
                        //if (item != lst[0])
                        //{
                        //    HisEkipUserProcessing.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                        //}
                        HisEkipUserProcessing.DEPARTMENT_ID = null;
                        if (ekipUserAdos.Count == 0)
                        {
                            HisEkipUserProcessing.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                        }
                        else
                        {
                            HisEkipUserProcessing.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                        }
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
                    List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE> executeRoles = lstExecuteRole;
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

                            var dataCheck = lstExecuteRole.FirstOrDefault(p => p.ID == executeRoleCheck.ID && p.IS_ACTIVE == 1);
                            if (dataCheck == null || dataCheck.ID == 0)
                                continue;
                            HisEkipUserADO ekipUserAdoTemp = new HisEkipUserADO();
                            ekipUserAdoTemp.EXECUTE_ROLE_ID = executeRoleCheck.ID;

                            SetDepartment(ekipUserAdoTemp);

                            if (ekipUserAdoTemps.Count == 0)
                            {
                                ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                            }
                            else
                            {
                                ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                            }
                            ekipUserAdoTemps.Add(ekipUserAdoTemp);
                        }
                    }
                    else
                    {
                        HisEkipUserADO ekipUserAdoTemp = new HisEkipUserADO();
                        ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                        ekipUserAdoTemps.Add(ekipUserAdoTemp);
                    }
                    if (ekipUserAdoTemps == null || ekipUserAdoTemps.Count == 0)
                    {
                        HisEkipUserADO ekipUserAdoTemp = new HisEkipUserADO();
                        ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                        ekipUserAdoTemps.Add(ekipUserAdoTemp);
                    }
                    this.NumExe(ekipUserAdoTemps.Count);
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
                    List<string> isActiveLoginName = datasAcsUser.Where(o => loginNames.Exists(p => p.Equals(o.LOGINNAME)) && o.IS_ACTIVE == 1).Select(i => i.LOGINNAME).ToList();
                    foreach (var ekipTempUser in ekipTempUsers)
                    {
                        var dataCheck = lstExecuteRole.FirstOrDefault(p => p.ID == ekipTempUser.EXECUTE_ROLE_ID && p.IS_ACTIVE == 1);
                        if (dataCheck == null || dataCheck.ID == 0)
                            continue;
                        HisEkipUserADO ekipUserAdoTemp = new HisEkipUserADO();
                        ekipUserAdoTemp.EXECUTE_ROLE_ID = ekipTempUser.EXECUTE_ROLE_ID;
                        ekipUserAdoTemp.LOGINNAME = ekipTempUser.LOGINNAME;
                        ekipUserAdoTemp.USERNAME = ekipTempUser.USERNAME;
                        ekipUserAdoTemp.DEPARTMENT_ID = ekipTempUser.DEPARTMENT_ID;
                        if (ekipUserAdoTemps.Count == 0)
                        {
                            ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                        }
                        else
                        {
                            ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                        }
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
                    this.NumExe(ekipUserAdoTemps.Count);
                    grdControlInformationSurg.DataSource = ekipUserAdoTemps;
                }
                FillDataToInformationSurg();
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
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROLE_NAME", "ID", columnInfos, false, 250);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboPosition, lstExecuteRole, controlEditorADO);
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
                var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.Column.FieldName == "LOGINNAME")
                {
                    //this.grdControlInformationSurg.RefreshDataSource();
                    SetDepartment(data);
                    this.grdControlInformationSurg.RefreshDataSource();
                }
                //if(e.Column.FieldName == "EXECUTE_ROLE_ID")
                //{
                //    if(data.LOGINNAME == null)
                //    {
                //        view.SetRowCellValue(e.RowHandle, view.Columns["LOGINNAME"], null);
                //    }
                //}
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
                if (e.Column.FieldName == "BtnDelete")
                {
                    int rowSelected = Convert.ToInt32(e.RowHandle);
                    int action = Inventec.Common.TypeConvert.Parse.ToInt32((grdViewInformationSurg.GetRowCellValue(e.RowHandle, "Action") ?? "").ToString());
                    if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                    {
                        e.RepositoryItem = btnAdd;
                    }
                    else
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
                        if (key == "1")
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
                            
                        
                        
                        //editor.Text = loginNames.Contains(data.LOGINNAME) ? data.LOGINNAME : null;
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
                    return;
                }
                Inventec.Common.Logging.LogSystem.Warn("SetDepartment_____________" + DepartmentId);
                if (DepartmentId != null && DepartmentId > 0)
                {
                    data.DEPARTMENT_ID = DepartmentId;
                }
                else
                {
                    data.DEPARTMENT_ID = null;
                    data.DEPARTMENT_NAME = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public string key = HisConfigs.Get<string>("HIS.Desktop.Plugins.SurgServiceReqExecute.ExecuteRoleUserOption");
        private void ComboAcsUser(GridLookUpEdit cbo, List<string> loginNames)
        {
            try
            {
                List<AcsUserADO> acsUserAlows = new List<AcsUserADO>();
                
                if (loginNames != null && loginNames.Count > 0)
                {

                    acsUserAlows = this.AcsUserADOList.Where(o => loginNames.Contains(o.LOGINNAME) && o.IS_ACTIVE == 1).ToList();

                }
                else if(key == "1")
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
                datasAcsUser = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<V_HIS_EMPLOYEE> employeeList = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_EMPLOYEE>();

                var departmentList = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == 1 && o.IS_CLINICAL == 1).ToList();
                AcsUserADOList = new List<AcsUserADO>();

                foreach (var item in datasAcsUser)
                {
                    AcsUserADO user = new AcsUserADO();
                    user.ID = item.ID;
                    user.LOGINNAME = item.LOGINNAME;
                    user.USERNAME = item.USERNAME;
                    user.MOBILE = item.MOBILE;
                    user.PASSWORD = item.PASSWORD;
                    user.IS_ACTIVE = item.IS_ACTIVE;

                    var check = employeeList.FirstOrDefault(o => o.LOGINNAME == item.LOGINNAME);
                    if (check != null)
                    {

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
                executeRoleUsers = new BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE_USER>>("api/HisExecuteRoleUser/Get", ApiConsumers.MosConsumer, filter, new CommonParam());
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
                if (ekipUsers == null || ekipUsers.Count < 1)
                {
                    HisEkipUserADO ekipUserAdoTemp = new HisEkipUserADO();
                    //ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                    ekipUserAdoTemps.Add(ekipUserAdoTemp);
                    ekipUserAdoTemps.ForEach(o => o.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                    ekipUserAdoTemps.LastOrDefault().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                    grdControlInformationSurg.DataSource = null;
                    grdControlInformationSurg.DataSource = ekipUserAdoTemps;
                }
                else
                {
                    HisEkipUserADO participant = new HisEkipUserADO();
                    //participant.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                    ekipUsers.Add(participant);
                    ekipUsers.ForEach(o => o.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                    ekipUsers.LastOrDefault().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                    grdControlInformationSurg.DataSource = null;
                    grdControlInformationSurg.DataSource = ekipUsers;
                }
                num++;
                this.Plus_Click(num);
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
                    if (ekipUsers.Count > 0)
                    {
                        ekipUsers.Remove(ekipUser);
                        ekipUsers.ForEach(o => o.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                        ekipUsers.LastOrDefault().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                        grdControlInformationSurg.DataSource = null;
                        grdControlInformationSurg.DataSource = ekipUsers;
                    }
                }
                num--;
                this.Delete_Click(num);
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
                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => departmentId), departmentId));
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
                if (grdViewInformationSurg.FocusedColumn.VisibleIndex == grdViewInformationSurg.VisibleColumns.Count - 1)
                {

                    var data = (HisEkipUserADO)grdViewInformationSurg.GetFocusedRow();

                    if (data.Action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                    {

                        btnAdd_ButtonClick(null, null);
                        grdViewInformationSurg.FocusedRowHandle = grdViewInformationSurg.DataRowCount - 1;
                        int visibleIndex = grdViewInformationSurg.FocusedColumn.VisibleIndex;
                        int newVisibleIndex = visibleIndex + 1;
                        if (newVisibleIndex == grdViewInformationSurg.VisibleColumns.Count)
                            newVisibleIndex = 0;
                        grdViewInformationSurg.FocusedColumn = grdViewInformationSurg.VisibleColumns[newVisibleIndex];
                    }
                    else
                    {
                        btnDelete_ButtonClick(null, null);
                    }

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


        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCEkipUser
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResourceUCEkipUser = new ResourceManager("HIS.Desktop.Plugins.ServiceExecute.Resources.Lang", typeof(UCEkipUser).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCEkipUser.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResourceUCEkipUser, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCEkipUser.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResourceUCEkipUser, LanguageManager.GetCulture());
                this.cboPosition.NullText = Inventec.Common.Resource.Get.Value("UCEkipUser.cboPosition.NullText", Resources.ResourceLanguageManager.LanguageResourceUCEkipUser, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCEkipUser.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResourceUCEkipUser, LanguageManager.GetCulture());
                this.GridLookupEdit_UserName.NullText = Inventec.Common.Resource.Get.Value("UCEkipUser.GridLookupEdit_UserName.NullText", Resources.ResourceLanguageManager.LanguageResourceUCEkipUser, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCEkipUser.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResourceUCEkipUser, LanguageManager.GetCulture());
                this.GridLookUpEdit_Department.NullText = Inventec.Common.Resource.Get.Value("UCEkipUser.GridLookUpEdit_Department.NullText", Resources.ResourceLanguageManager.LanguageResourceUCEkipUser, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCEkipUser.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResourceUCEkipUser, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCEkipUser.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResourceUCEkipUser, LanguageManager.GetCulture());
                this.repositoryItemGridLookUpEditUsername.NullText = Inventec.Common.Resource.Get.Value("UCEkipUser.repositoryItemGridLookUpEditUsername.NullText", Resources.ResourceLanguageManager.LanguageResourceUCEkipUser, LanguageManager.GetCulture());
                this.repositoryItemSearchLookUpEdit1.NullText = Inventec.Common.Resource.Get.Value("UCEkipUser.repositoryItemSearchLookUpEdit1.NullText", Resources.ResourceLanguageManager.LanguageResourceUCEkipUser, LanguageManager.GetCulture());
                this.lciInformationSurg.Text = Inventec.Common.Resource.Get.Value("UCEkipUser.lciInformationSurg.Text", Resources.ResourceLanguageManager.LanguageResourceUCEkipUser, LanguageManager.GetCulture());
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
                        if(data.LOGINNAME == null && (key == "1"))
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
