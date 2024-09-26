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
using DevExpress.XtraEditors;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Plugins.ServiceExecute.ADO;
using HIS.Desktop.Plugins.ServiceExecute.Config;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServiceExecute
{
    public partial class UCServiceExecute : UserControlBase
    {
        private List<HIS_EKIP_TEMP> ekipTemps { get; set; }
        private List<HIS_EXECUTE_ROLE_USER> executeRoleUsers { get; set; }
        private List<AcsUserADO> AcsUserADOList { get; set; }
        public bool IsDataEkipUser { get; private set; }

        private void LoadExecuteRoleUser()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("LoadExecuteRoleUser. 1");
                HisExecuteRoleUserFilter filter = new HisExecuteRoleUserFilter();
                executeRoleUsers = new BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE_USER>>("api/HisExecuteRoleUser/Get", ApiConsumers.MosConsumer, filter, new CommonParam());
                Inventec.Common.Logging.LogSystem.Debug("LoadExecuteRoleUser. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboDepartment(GridLookUpEdit cbo)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, lstDepartment, controlEditorADO);
                cbo.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToGridComboDepartment()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 400);
                ControlEditorLoader.Load(repositoryItemCboDepartment, lstDepartment, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ComboAcsUser()
        {
            try
            {
                this.AcsUserADOList = ProcessAcsUser();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 3));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 750);
                ControlEditorLoader.Load(repositoryItemCboUser, this.AcsUserADOList, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task ComboEkipTemp(GridLookUpEdit cbo)
        {
            try
            {
                var DepartmentID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.moduleData.RoomId).DepartmentId;

                string logginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                CommonParam param = new CommonParam();
                HisEkipTempFilter filter = new HisEkipTempFilter();
                ekipTemps = await new BackendAdapter(param).GetAsync<List<MOS.EFMODEL.DataModels.HIS_EKIP_TEMP>>("/api/HisEkipTemp/Get", ApiConsumers.MosConsumer, filter, param);
                if (ekipTemps != null && ekipTemps.Count > 0)
                {
                    ekipTemps = ekipTemps.Where(o => (o.IS_PUBLIC == 1 || o.CREATOR == logginName || (o.IS_PUBLIC_IN_DEPARTMENT == 1 && o.DEPARTMENT_ID == DepartmentID)) && o.IS_ACTIVE == 1).OrderByDescending(o => o.CREATE_TIME).ToList();
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EKIP_TEMP_NAME", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EKIP_TEMP_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, ekipTemps, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadGridEkipUserFromTemp(long ekipTempId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisEkipTempUserFilter filter = new HisEkipTempUserFilter();
                filter.EKIP_TEMP_ID = ekipTempId;
                List<HIS_EKIP_TEMP_USER> ekipTempUsers = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_EKIP_TEMP_USER>>("api/HisEkipTempUser/Get", ApiConsumers.MosConsumer, filter, param);
                if (ekipTempUsers != null && ekipTempUsers.Count > 0)
                {
                    List<HisEkipUserADO> ekipUserAdoTemps = new List<HisEkipUserADO>();
                    List<string> loginNames = ekipTempUsers.Select(o => o.LOGINNAME).ToList();
                    //AcsUserFilter acsFilter = new AcsUserFilter();
                    //acsFilter.LOGINNAMEs = loginNames;
                    var isActive = AcsUserADOList.Where(o => ekipTempUsers.Exists(p => p.LOGINNAME == o.LOGINNAME)).ToList();
                    List<string> isActiveLoginName = isActive.Where(o => o.IS_ACTIVE == 1).Select(i => i.LOGINNAME).ToList();
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
                        if (isActiveLoginName.Contains(ekipTempUser.LOGINNAME))
                        {
                            SetDepartment(ekipUserAdoTemp);
                            ekipUserAdoTemps.Add(ekipUserAdoTemp);
                        }
                    }

                    gridControlEkip.DataSource = ekipUserAdoTemps;
                }
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
                    return;

                if (cboEkipDepartment.EditValue != null)
                {
                    data.DEPARTMENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboEkipDepartment.EditValue.ToString());
                }
                else
                {
                    var employee = BackendDataWorker.Get<V_HIS_EMPLOYEE>().FirstOrDefault(o => !String.IsNullOrWhiteSpace(data.LOGINNAME) && o.LOGINNAME.ToLower() == data.LOGINNAME.ToLower());
                    if (employee != null)
                    {
                        data.DEPARTMENT_ID = employee.DEPARTMENT_ID;
                        var department = lstDepartment.FirstOrDefault(o => o.ID == employee.DEPARTMENT_ID);
                        data.DEPARTMENT_NAME = department != null ? department.DEPARTMENT_NAME : "";
                    }
                    else
                    {
                        data.DEPARTMENT_ID = null;
                        data.DEPARTMENT_NAME = "";
                    }
                }
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
                ControlEditorLoader.Load(repositoryItemCboPosition, lstExecuteRole, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<AcsUserADO> ProcessAcsUser()
        {
            List<AcsUserADO> result = null;
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> datas = null;
                List<V_HIS_EMPLOYEE> employeeList = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE>();

                CommonParam paramCommon = new CommonParam();
                dynamic filter = new System.Dynamic.ExpandoObject();
                datas = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => o.IS_ACTIVE == 1).ToList();

                var departmentList = lstDepartment;
                result = new List<AcsUserADO>();

                foreach (var item in datas)
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
                        var checkDepartment = departmentList.FirstOrDefault(o => o.ID == check.DEPARTMENT_ID);
                        if (checkDepartment != null)
                        {
                            user.DEPARTMENT_NAME = checkDepartment.DEPARTMENT_NAME;
                        }
                    }
                    result.Add(user);
                }

                result = result.OrderBy(o => o.USERNAME).ToList();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void FillDataToGridEkip()
        {
            try
            {
                ekipUserAdos = new List<HisEkipUserADO>();
                HisEkipUserADO ado = new HisEkipUserADO();

                bool IsDataKey = false;
                if (this.sereServ != null)
                {
                    CommonParam param = new CommonParam();
                    if (dicEkipUser.ContainsKey(this.sereServ.ID))
                    {
                        ekipUserAdos = new List<HisEkipUserADO>();
                        ekipUserAdos.AddRange(dicEkipUser[this.sereServ.ID]);
                        IsDataEkipUser = true;
                    }
                    else if (this.sereServ.EKIP_ID.HasValue)
                    {
                        HisEkipUserViewFilter hisEkipUserFilter = new HisEkipUserViewFilter();
                        hisEkipUserFilter.EKIP_ID = this.sereServ.EKIP_ID;
                        hisEkipUserFilter.IS_ACTIVE = 1;
                        var lstEkipUser = new BackendAdapter(param).Get<List<V_HIS_EKIP_USER>>(HisRequestUriStore.HIS_EKIP_USER_GETVIEW, ApiConsumers.MosConsumer, hisEkipUserFilter, param);
                        if (lstEkipUser != null && lstEkipUser.Count > 0)
                        {
                            ekipUserAdos = new List<HisEkipUserADO>();
                            foreach (var item in lstEkipUser)
                            {
                                var dataCheck = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().FirstOrDefault(p => p.ID == item.EXECUTE_ROLE_ID);
                                if (dataCheck == null || dataCheck.ID == 0)
                                    continue;

                                HisEkipUserADO ekipAdo = new HisEkipUserADO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<HisEkipUserADO>(ekipAdo, item);

                                if (ekipUserAdos.Count == 0)
                                {
                                    ekipAdo.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                                }
                                else
                                {
                                    ekipAdo.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                                }

                                SetDepartment(ekipAdo);
                                ekipUserAdos.Add(ekipAdo);
                                IsDataEkipUser = true;
                            }
                            AutoAddUser();
                            dicEkipUser[this.sereServ.ID] = ekipUserAdos;
                        }
                    }
                    else if (this.currentServiceReq.EKIP_PLAN_ID.HasValue) //tiennv
                    {
                        HisEkipPlanUserViewFilter hisEkipPlanUserFilter = new HisEkipPlanUserViewFilter();
                        hisEkipPlanUserFilter.EKIP_PLAN_ID = this.currentServiceReq.EKIP_PLAN_ID;
                        hisEkipPlanUserFilter.IS_ACTIVE = 1;
                        var lstEkipUser = new BackendAdapter(param).Get<List<V_HIS_EKIP_PLAN_USER>>("api/HisEkipPlanUser/GetView", ApiConsumers.MosConsumer, hisEkipPlanUserFilter, param);
                        if (lstEkipUser != null && lstEkipUser.Count > 0)
                        {
                            ekipUserAdos = new List<HisEkipUserADO>();
                            foreach (var item in lstEkipUser)
                            {
                                var dataCheck = lstExecuteRole.FirstOrDefault(p => p.ID == item.EXECUTE_ROLE_ID && p.IS_ACTIVE == 1);
                                if (dataCheck == null || dataCheck.ID == 0)
                                    continue;

                                HisEkipUserADO ekipAdo = new HisEkipUserADO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<HisEkipUserADO>(ekipAdo, item);

                                if (ekipUserAdos.Count == 0)
                                {
                                    ekipAdo.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                                }
                                else
                                {
                                    ekipAdo.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                                }

                                SetDepartment(ekipAdo);
                                ekipUserAdos.Add(ekipAdo);
                                IsDataEkipUser = true;
                            }
                            AutoAddUser();
                            dicEkipUser[this.sereServ.ID] = ekipUserAdos;
                        }
                    }
                    else
                    {
                        string key = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ServiceExecuteCFG.ExecuteRoleDefault);
                        if (!String.IsNullOrEmpty(key))
                        {
                            List<string> lstCode = key.Split(',').ToList();

                            if (lstCode != null && lstCode.Count > 0)
                            {
                                ekipUserAdos = new List<HisEkipUserADO>();
                                foreach (var item in lstCode)
                                {
                                    var executeRole = lstExecuteRole.Where(o => o.EXECUTE_ROLE_CODE == item && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).FirstOrDefault();
                                    if (executeRole != null)
                                    {
                                        HisEkipUserADO HisEkipUserADOAdd = new HisEkipUserADO();
                                        HisEkipUserADOAdd.EXECUTE_ROLE_CODE = executeRole.EXECUTE_ROLE_CODE;
                                        HisEkipUserADOAdd.EXECUTE_ROLE_NAME = executeRole.EXECUTE_ROLE_NAME;
                                        HisEkipUserADOAdd.EXECUTE_ROLE_ID = executeRole.ID;
                                        if (ekipUserAdos.Count == 0)
                                        {
                                            HisEkipUserADOAdd.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                                        }
                                        else
                                        {
                                            HisEkipUserADOAdd.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                                        }

                                        ekipUserAdos.Add(HisEkipUserADOAdd);
                                        IsDataEkipUser = true;
                                        IsDataKey = true;
                                    }
                                }
                                AutoAddUser();
                                if (ekipUserAdos.Count == 0)
                                {
                                    ekipUserAdos.Add(ado);
                                }
                            }
                        }
                    }
                }

                if (IsDataEkipUser && !IsDataKey)
                {
                    AutoAddUser();
                    if (ekipUserAdos == null || ekipUserAdos.Count == 0)
                    {
                        ekipUserAdos.Add(ado);
                    }
                    ekipUserAdos.ForEach(o => o.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                    ekipUserAdos.First().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                    gridControlEkip.DataSource = null;
                    gridControlEkip.DataSource = ekipUserAdos;
                }
                else
                {
                    SetDataSourceEkipUser();
                    if (!IsLoadFromPin)
                    {
                        AutoAddUser();
                        if (ekipUserAdos == null || ekipUserAdos.Count == 0)
                        {
                            ekipUserAdos.Add(ado);
                        }
                        ekipUserAdos.ForEach(o => o.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                        ekipUserAdos.First().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                        gridControlEkip.DataSource = null;
                        gridControlEkip.DataSource = ekipUserAdos;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AutoAddUser()
        {
            try
            {
                var loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                if (((ekipUserAdos != null && ekipUserAdos.Count > 0 && !ekipUserAdos.Exists(p => p.LOGINNAME.Equals(loginName))) || ekipUserAdos == null || ekipUserAdos.Count == 0) && !string.IsNullOrEmpty(AppConfigKeys.ServiceReqTypeCode) && AppConfigKeys.GetServiceReqTypeId() != null && AppConfigKeys.ServiceReqTypeId != null && AppConfigKeys.ServiceReqTypeId.Count > 0 && AppConfigKeys.ServiceReqTypeId.Contains(currentServiceReq.SERVICE_REQ_TYPE_ID) && executeRoleUsers != null && executeRoleUsers.Count > 0)
                {
                    var executeRoleUser = executeRoleUsers.Where(o => o.LOGINNAME.Equals(loginName));
                    if (executeRoleUser != null && executeRoleUser.Count() > 0)
                    {
                        var executeRole = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().Where(o => executeRoleUser.ToList().Exists(p => p.EXECUTE_ROLE_ID == o.ID) && o.IS_SUBCLINICAL_RESULT == 1);
                        if (executeRole != null && executeRole.Count() > 0)
                        {
                            var erole = executeRole.OrderByDescending(o => o.ID).First();

                            var department = BackendDataWorker.Get<V_HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME.Equals(executeRoleUser.First().LOGINNAME));
                            if (ekipUserAdos == null)
                                ekipUserAdos = new List<HisEkipUserADO>();
                            ekipUserAdos.Add(new HisEkipUserADO() { LOGINNAME = executeRoleUser.First().LOGINNAME, USERNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName(), EXECUTE_ROLE_ID = erole.ID, EXECUTE_ROLE_CODE = erole.EXECUTE_ROLE_CODE, EXECUTE_ROLE_NAME = erole.EXECUTE_ROLE_NAME, DEPARTMENT_ID = (department != null ? (long?)department.DEPARTMENT_ID : null) });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private string key = HisConfigs.Get<string>("HIS.Desktop.Plugins.SurgServiceReqExecute.ExecuteRoleUserOption");
        private void ComboAcsUser(GridLookUpEdit cbo, List<string> loginNames)
        {
            try
            {
                List<AcsUserADO> acsUserAlows = new List<AcsUserADO>();
                
                if (loginNames != null && loginNames.Count > 0)
                {
                    acsUserAlows = AcsUserADOList.Where(o => loginNames.Contains(o.LOGINNAME)).ToList();
                }
                else if(key == "1")
                {
                    acsUserAlows = null;
                }
                else
                {
                    acsUserAlows = AcsUserADOList;
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 3));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, acsUserAlows, controlEditorADO);
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RefeshDataEkipTemp()
        {
            try
            {
                this.ComboEkipTemp(cboEkipUserTemp);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckEkip()
        {
            bool result = false;
            try
            {
                var ekipUsers = gridControlEkip.DataSource as List<HisEkipUserADO>;

                if (ekipUsers != null && ekipUsers.Count > 0)
                {
                    List<string> messError = new List<string>();

                    var groupLoginname = ekipUsers.Where(o => !String.IsNullOrWhiteSpace(o.LOGINNAME)).GroupBy(o => o.LOGINNAME).ToList();
                    foreach (var item in groupLoginname)
                    {
                        if (item.Count() > 1)
                        {
                            var lstExeRole = lstExecuteRole.Where(o => item.Select(s => s.EXECUTE_ROLE_ID).Contains(o.ID)).ToList();

                            messError.Add(string.Format(ResourceMessage.TaiKhoanDuocGanNhieuVaiTro, item.Key, string.Join(",", lstExeRole.Select(s => s.EXECUTE_ROLE_NAME))));
                        }
                    }

                    if (messError.Count > 0)
                    {
                        XtraMessageBox.Show(string.Join("\n", messError), ResourceMessage.ThongBao);
                        result = true;
                    }
                }

                if (!result && keySubclinicalInfoOption != "1")
                {
                    bool valid = false;
                    if (ekipUsers != null && ekipUsers.Count > 0)
                    {
                        List<HisEkipUserADO> checkExist = ekipUsers.Where(o => (o.EXECUTE_ROLE_ID == 0 && (o.DEPARTMENT_ID == 0 || o.DEPARTMENT_ID == null) && string.IsNullOrEmpty(o.LOGINNAME)) || (o.EXECUTE_ROLE_ID == 0 && string.IsNullOrEmpty(o.LOGINNAME))).ToList();
                        if (checkExist != null && checkExist.Count > 0)
                        {
                            valid = true;
                        }
                    }
                    else
                    {
                        var service = lstService.FirstOrDefault(o => o.ID == this.sereServ.SERVICE_ID);
                        if (service != null && service.PTTT_GROUP_ID.HasValue)
                        {
                            valid = true;
                        }
                    }

                    WaitingManager.Hide();
                    if (valid && DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.BanChuaChonKipThucHien, ResourceMessage.ThongBao, MessageBoxButtons.OK) == DialogResult.OK)
                    {
                        xtraTabControl1.SelectedTabPage = xtraTabPageEkip;
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = true;
            }

            return result;
        }
    }
}
