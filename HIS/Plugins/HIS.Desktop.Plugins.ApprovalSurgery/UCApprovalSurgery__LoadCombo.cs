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
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using HIS.UC.SecondaryIcd;
using Inventec.Core;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.ApprovalSurgery
{
    public partial class UCApprovalSurgery : UserControlBase
    {
        private void LoadCombo()
        {
            try
            {
                LoadComboExecuteDepartment();
                LoadComboEkipTemp();
                LoadComboExecuteRole();
                LoadComboAcsUser();
                LoadComboRoom();
                LoadComboMethod();
                LoadComboEmotionless();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboRoom()
        {
            try
            {
                var dataFocus = (List<V_HIS_SERE_SERV_13>)gridViewService.DataSource;
                if (dataFocus != null && dataFocus.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceRoomViewFilter filter = new HisServiceRoomViewFilter();
                    filter.SERVICE_ID = dataFocus.FirstOrDefault().SERVICE_ID;

                    var ServiceRooms = new BackendAdapter(param).Get<List<V_HIS_SERVICE_ROOM>>(
                                        "api/HisServiceRoom/GetView",
                                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                    filter,
                                    param);

                    //var DepartmentID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == roomId).DepartmentId;

                    //List<V_HIS_ROOM> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                    MOS.Filter.HisRoomViewFilter filterRoom = new HisRoomViewFilter();
                    filterRoom.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                    if (ServiceRooms != null && ServiceRooms.Count > 0)
                    {
                        filterRoom.IDs = ServiceRooms.Select(o => o.ROOM_ID).ToList();
                    }

                    var data = new BackendAdapter(param).Get<List<V_HIS_ROOM>>(
                                        "api/HisRoom/GetView",
                                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                                    filterRoom,
                                    param);


                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("ROOM_CODE", "", 150, 1));
                    columnInfos.Add(new ColumnInfo("ROOM_NAME", "", 250, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_NAME", "ID", columnInfos, false, 250);
                    ControlEditorLoader.Load(cboRoom, data, controlEditorADO);

                    cboRoom.EditValue = dataFocus.FirstOrDefault().EXECUTE_ROOM_ID;

                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void LoadComboAcsUser()
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 250);
                ControlEditorLoader.Load(repositoryItemCustomGridLookUpEditUsername, data, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void LoadComboEkipTemp()
        {
            try
            {
                string logginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                CommonParam param = new CommonParam();
                HisEkipTempFilter filter = new HisEkipTempFilter();
                ekipTemps = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_EKIP_TEMP>>("/api/HisEkipTemp/Get", ApiConsumers.MosConsumer, filter, param).OrderByDescending(o => o.CREATE_TIME).ToList();

                if (ekipTemps != null)
                {
                    ekipTemps = ekipTemps.Where(o => o.IS_PUBLIC == 1 || o.CREATOR == logginName).ToList();
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EKIP_TEMP_NAME", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EKIP_TEMP_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboEkipTemp, ekipTemps, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void LoadComboExecuteRole()
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_DISABLE_IN_EKIP != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROLE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(repositoryItemCustomGridLookUpPosition, data, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboExecuteDepartment()
        {
            try
            {
                List<HIS_DEPARTMENT> departments = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>()
                    .Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboDepartment, departments.Where(o => o.BRANCH_ID == BranchDataWorker.GetCurrentBranchId()).ToList(), controlEditorADO);
                ControlEditorLoader.Load(cboDepartmentServiceReq, departments, controlEditorADO);
                V_HIS_ROOM room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.roomId);
                if (room != null)
                {
                    if (departments != null && departments.Count > 0)
                    {
                        HIS_DEPARTMENT department = departments.FirstOrDefault(o => o.ID == room.DEPARTMENT_ID);
                        if (department != null)
                        {
                            txtDepartmentCode.Text = department.DEPARTMENT_CODE;
                            cboDepartment.EditValue = department.ID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboMethod()
        {
            try
            {

                List<HIS_PTTT_METHOD> methods = BackendDataWorker.Get<HIS_PTTT_METHOD>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PTTT_METHOD_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("PTTT_METHOD_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PTTT_METHOD_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboMethod, methods, controlEditorADO);

                var dataFocus = (V_HIS_SERE_SERV_13)gridViewService.GetFocusedRow();
                if (dataFocus != null)
                {
                    CommonParam param = new CommonParam();
                    HisSereServPtttFilter filter = new HisSereServPtttFilter();
                    filter.SERE_SERV_ID = dataFocus.ID;
                    List<HIS_SERE_SERV_PTTT> hisSereServPttts = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_PTTT>>("api/HisSereServPttt/Get", ApiConsumers.MosConsumer, filter, param);
                    if (hisSereServPttts != null && hisSereServPttts.Count > 0 && methods.Exists(o => o.ID == hisSereServPttts.FirstOrDefault().PTTT_METHOD_ID))
                    {
                        var ptttMethodId = hisSereServPttts.FirstOrDefault().PTTT_METHOD_ID;
                        cboMethod.EditValue = ptttMethodId;
                        txtMethod.Text = methods.FirstOrDefault(o => o.ID == ptttMethodId).PTTT_METHOD_CODE;
                    }
                    else
                    {
                        var service = BackendDataWorker.Get<HIS_SERVICE>();
                        var methodId = service.FirstOrDefault(o => o.ID == dataFocus.SERVICE_ID).PTTT_METHOD_ID; ;
                        if (methods.Exists(o => o.ID == methodId))
                        {
                            cboMethod.EditValue = methodId;
                            if (cboMethod.EditValue != null)
                                txtMethod.Text = methods.Where(o => o.ID == methodId).FirstOrDefault().PTTT_METHOD_CODE;
                            else
                                txtMethod.Text = "";
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboEmotionless()
        {
            try
            {
                var dataFocus = (V_HIS_SERE_SERV_13)gridViewServiceReq.GetFocusedRow();
                if (dataFocus != null)
                {
                    List<HIS_EMOTIONLESS_METHOD> emotionless = BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>()
   .Where(o => o.IS_ACTIVE == 1).ToList();
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("EMOTIONLESS_METHOD_CODE", "", 150, 1));
                    columnInfos.Add(new ColumnInfo("EMOTIONLESS_METHOD_NAME", "", 250, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("EMOTIONLESS_METHOD_NAME", "ID", columnInfos, false, 250);
                    ControlEditorLoader.Load(cboEmotionless, emotionless, controlEditorADO);

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataFocus), dataFocus));
                    CommonParam param = new CommonParam();
                    HisSereServPtttFilter filter = new HisSereServPtttFilter();
                    filter.SERE_SERV_ID = dataFocus.ID;
                    List<HIS_SERE_SERV_PTTT> hisSereServPttts = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_PTTT>>("api/HisSereServPttt/Get", ApiConsumers.MosConsumer, filter, param);
                    if (hisSereServPttts != null && hisSereServPttts.Count > 0)
                    {
                        cboEmotionless.EditValue = hisSereServPttts.FirstOrDefault().EMOTIONLESS_METHOD_ID;
                        txtEmotionless.Text = emotionless.Where(o => o.ID == hisSereServPttts.FirstOrDefault().EMOTIONLESS_METHOD_ID).FirstOrDefault().EMOTIONLESS_METHOD_CODE;

                    }
                    else
                    {
                        cboEmotionless.EditValue = null;
                        txtEmotionless.Text = "";
                    }



                }
                else
                {
                    List<HIS_EMOTIONLESS_METHOD> emotionless = BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>()
                        .Where(o => o.IS_ACTIVE == 1).ToList();
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("EMOTIONLESS_METHOD_CODE", "", 150, 1));
                    columnInfos.Add(new ColumnInfo("EMOTIONLESS_METHOD_NAME", "", 250, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("EMOTIONLESS_METHOD_NAME", "ID", columnInfos, false, 250);
                    ControlEditorLoader.Load(cboEmotionless, emotionless, controlEditorADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboEmotionlessDefault()
        {
            try
            {

                List<HIS_EMOTIONLESS_METHOD> emotionless = BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>()
                    .Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EMOTIONLESS_METHOD_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("EMOTIONLESS_METHOD_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EMOTIONLESS_METHOD_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboEmotionless, emotionless, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
