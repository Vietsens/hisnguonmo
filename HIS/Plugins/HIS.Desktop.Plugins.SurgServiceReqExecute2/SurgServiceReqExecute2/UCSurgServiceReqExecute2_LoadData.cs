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
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.SurgServiceReqExecute2.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute2
{
    public partial class UCSurgServiceReqExecute2 : UserControlBase
    {
        private void CreateThreadLoadDataAll()
        {
            try
            {
                List<Action> methods = new List<Action>();
                methods.Add(InitDataAcsUserCombo);
                methods.Add(InitDataServiceCombo);
                methods.Add(InitDataPtttMethodCombo);
                methods.Add(InitDataEmotionLessMethodCombo);
                methods.Add(InitDataPtttMethodRealCombo);
                methods.Add(InitDataPtttGroupCombo);
                methods.Add(InitDataExecuteRoleCombo);
                methods.Add(LoadExecuteRoleUser);
                methods.Add(InitDataEkipTempCombo);
                ThreadCustomManager.MultipleThreadWithJoin(methods);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void CreatThreadLoadDataInfor()
        {
            try
            {
                List<Action> methods = new List<Action>();
                methods.Add(LoadDataPttt);
                methods.Add(LoadDataEkipUser);
                methods.Add(LoadDataPatientTypeAlter);
                ThreadCustomManager.MultipleThreadWithJoin(methods);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        V_HIS_PATIENT_TYPE_ALTER patientTyleAlter { get; set; }
        private void LoadDataPatientTypeAlter()
        {

            try
            {
                CommonParam paramCommon = new CommonParam();
                HisPatientTypeAlterViewFilter filter = new HisPatientTypeAlterViewFilter();
                filter.TREATMENT_ID = currentRow.TDL_TREATMENT_ID;
                var lst = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (lst != null && lst.Count > 0)
                    patientTyleAlter = lst.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        List<HIS_EKIP_USER> ekData { get; set; }
        private void LoadDataEkipUser()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                if (currentRow.EKIP_ID.HasValue)
                {
                    HisEkipUserFilter ekUfilter = new HisEkipUserFilter();
                    ekUfilter.EKIP_ID = currentRow.EKIP_ID;
                    ekData = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_EKIP_USER>>("api/HisEkipUser/Get", ApiConsumers.MosConsumer, ekUfilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        V_HIS_SERE_SERV_PTTT sp { get; set; }
        private void LoadDataPttt()
        {

            try
            {
                CommonParam paramCommon = new CommonParam();
                if (currentRow.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL || currentRow.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                {
                    HisSereServPtttViewFilter spVfilter = new HisSereServPtttViewFilter();
                    spVfilter.SERE_SERV_ID = currentRow.ID;
                    var spData = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_SERE_SERV_PTTT>>("api/HisSereServPttt/GetView", ApiConsumers.MosConsumer, spVfilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    if (spData != null && spData.Count > 0)
                    {
                        sp = spData.First();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void LoadExecuteRoleUser()
        {
            try
            {
                executeRoleUsers = BackendDataWorker.Get<HIS_EXECUTE_ROLE_USER>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitDataAcsUserCombo()
        {
            AcsUserADOList = ProcessAcsUser();
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
                datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                employeeList = BackendDataWorker.Get<V_HIS_EMPLOYEE>();

                DepartmentList = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_CLINICAL == 1).ToList();
                AcsUserADOList = new List<AcsUserADO>();

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

                        user.DOB = Inventec.Common.DateTime.Convert.TimeNumberToDateString(check.DOB ?? 0);

                        user.DIPLOMA = check.DIPLOMA;
                        var checkDepartment = DepartmentList.FirstOrDefault(o => o.ID == check.DEPARTMENT_ID);

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
        private void InitDataEkipTempCombo()
        {
            try
            {
                var DepartmentID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.moduleData.RoomId).DepartmentId;

                string logginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                CommonParam param = new CommonParam();
                HisEkipTempFilter filter = new HisEkipTempFilter();
                EkipTempList = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_EKIP_TEMP>>("/api/HisEkipTemp/Get", ApiConsumers.MosConsumer, filter, param);
                if (EkipTempList != null && EkipTempList.Count > 0)
                {
                    EkipTempList = EkipTempList.Where(o => (o.IS_PUBLIC == 1 || o.CREATOR == logginName || (o.IS_PUBLIC_IN_DEPARTMENT == 1 && o.DEPARTMENT_ID == DepartmentID)) && o.IS_ACTIVE == 1).OrderByDescending(o => o.CREATE_TIME).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDataExecuteRoleCombo()
        {
            try
            {
                ExecuteRoleList = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_DISABLE_IN_EKIP != 1).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDataPtttGroupCombo()
        {
            try
            {
                PtttGroupList = BackendDataWorker.Get<HIS_PTTT_GROUP>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDataPtttMethodRealCombo()
        {

            try
            {
                PtttMethodRealList = BackendDataWorker.Get<HIS_PTTT_METHOD>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDataEmotionLessMethodCombo()
        {
            try
            {
                EmotionLessMethodList = BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && (o.IS_SECOND == 1 || (o.IS_FIRST != 1 && o.IS_SECOND != 1))).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDataPtttMethodCombo()
        {

            try
            {
                PtttMethodList = BackendDataWorker.Get<HIS_PTTT_METHOD>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void InitDataServiceCombo()
        {
            try
            {
                ServiceRoomViewList = BackendDataWorker.Get<V_HIS_SERVICE_ROOM>().Where(o => o.ROOM_ID == moduleData.RoomId).ToList();
                if(ServiceRoomViewList != null && ServiceRoomViewList.Count > 0)
                {
                    ServiceList = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && ServiceRoomViewList.Exists(p=>p.SERVICE_ID == o.ID)).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

    }
}
