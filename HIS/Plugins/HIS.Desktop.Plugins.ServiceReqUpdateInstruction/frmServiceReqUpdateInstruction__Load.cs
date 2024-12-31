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
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LocalStorage.HisConfig;
using System;
using System.Linq;

namespace HIS.Desktop.Plugins.ServiceReqUpdateInstruction
{
    public partial class frmServiceReqUpdateInstruction : HIS.Desktop.Utility.FormBase
    {
        private void InitEnabledControl()
        {
            try
            {
                long currentDepartmentId = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == module.RoomId).DepartmentId;
                if (currentServiceReq == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("Khong tim thay currentServiceReq");
                    return;
                }

                if (currentServiceReq.REQUEST_DEPARTMENT_ID == currentDepartmentId)
                {
                    lciThoiGianYLenh.Enabled = true;
                    panelControlUcIcd.Enabled = true;
                    panelControlCDYHCT.Enabled = true;
                    panelControlICDSubYHCT.Enabled = true;
                    panelControlSubIcd.Enabled = true;
                    panelControlCauseIcd.Enabled = true;
                    btnSave.Enabled = true;
                }

                if (currentServiceReq.EXECUTE_DEPARTMENT_ID == currentDepartmentId)
                {
                    lciStartTime.Enabled = true;
                    lciEndTime.Enabled = true;
                    lciUserName.Enabled = true;
                    lciCboNguoiThucHien.Enabled = true;
                    btnSave.Enabled = true;
                }

                if (HisConfigs.Get<string>("HIS.Desktop.Plugins.AssignConfig.ShowRequestUser") == "1")
                {
                    cboRequestUser.Enabled = true;
                    txtRequestUser.Enabled = true;
                    btnSave.Enabled = true;
                }
                else
                {
                    cboRequestUser.Enabled = false;
                    txtRequestUser.Enabled = false;
                }

                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                if (CheckLoginAdmin.IsAdmin(loginName))
                {
                    lciThoiGianYLenh.Enabled = true;
                    lciStartTime.Enabled = true;
                    lciEndTime.Enabled = true;
                    lciUserName.Enabled = true;
                    lciCboNguoiThucHien.Enabled = true;
                    panelControlSubIcd.Enabled = true;
                    panelControlUcIcd.Enabled = true;
                    panelControlCauseIcd.Enabled = true;
                    panelControlCDYHCT.Enabled = true;
                    panelControlICDSubYHCT.Enabled = true;
                    btnSave.Enabled = true;
                }

                if (currentServiceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                {
                    lciStartTime.Enabled = false;
                    lciEndTime.Enabled = false;
                }
                else if (currentServiceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                {
                    lciStartTime.Enabled = true;
                    lciEndTime.Enabled = false;
                    btnSave.Enabled = true;
                }
                if (currentServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK
                    || currentServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM
                    || currentServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT
                    ||currentServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT)
                {
                    cboEndServiceReq.Enabled = false;
                    txtLoginname.Enabled = false;
                }
                if (currentServiceReq.CREATOR == LoggingName || CheckLoginAdmin.IsAdmin(loginName))
                {
                    mmNOTE.Enabled = true;
                }
                else
                {
                    mmNOTE.Enabled = false;
                }
                if (currentServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__AN && currentServiceReq.RATION_SUM_ID != null)
                {
                    lciThoiGianYLenh.Enabled = false;
                    dtTime.Enabled = false;
                }
                else
                {
                    lciThoiGianYLenh.Enabled = true;
                    dtTime.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
