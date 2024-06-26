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
using HIS.UC.SecondaryIcd;
using MOS.EFMODEL.DataModels;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Plugins.ApprovalSurgery.ADO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using MOS.SDO;
using HIS.Desktop.Plugins.ApprovalSurgery.CreateCalendar;
using System.Collections;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.ApiConsumer;
using ACS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.ApprovalSurgery.ApprovalInfo;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.ApprovalSurgery
{
    public partial class UCApprovalSurgery : UserControlBase
    {
        public enum ACTION_APPROVAL_CALENDAR
        {
            APPROVAL,
            UNAPPROVAL
        }

        public enum CALENDAR_ACTION
        {
            ADD,
            REMOVE
        }

        public enum APPROVAL_PLAN_ACTION
        {
            APPROVAL,
            UNAPPROVAL,
            REJECT,
            UNREJECT
        }

        private void ApprovalProcess(ACTION_APPROVAL_CALENDAR action)
        {
            try
            {
                V_HIS_PTTT_CALENDAR ptttCalendar = gridViewPtttCalendar.GetFocusedRow() as V_HIS_PTTT_CALENDAR;
                if (ptttCalendar != null)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    bool success = false;
                    HisPtttCalendarSDO hisPtttCalendarSDO = new HisPtttCalendarSDO();
                    hisPtttCalendarSDO.Id = ptttCalendar.ID;
                    hisPtttCalendarSDO.WorkingRoomId = this.roomId;
                    hisPtttCalendarSDO.TimeFrom = ptttCalendar.TIME_FROM;
                    hisPtttCalendarSDO.TimeTo = ptttCalendar.TIME_TO;
                    string requestUri = action == ACTION_APPROVAL_CALENDAR.APPROVAL ?
                        "api/HisPtttCalendar/Approve" : "api/HisPtttCalendar/Unapprove";
                    var result = new BackendAdapter(param)
                    .Post<MOS.EFMODEL.DataModels.HIS_PTTT_CALENDAR>(requestUri, ApiConsumers.MosConsumer, hisPtttCalendarSDO, param);
                    if (result != null)
                    {
                        FillDataToGridPtttCalendar();
                        success = true;
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<EkipSDO> GetEkipPlanUser()
        {
            List<EkipSDO> result = new List<EkipSDO>();
            try
            {
                List<EkipPlanUserADO> ekipPlanUsers = gridControlEkipPlanUser.DataSource as List<EkipPlanUserADO>;
                if (ekipPlanUsers != null && ekipPlanUsers.Count > 0)
                {
                    foreach (var item in ekipPlanUsers)
                    {
                        EkipSDO sdo = new EkipSDO();
                        sdo.ExecuteRoleId = item.EXECUTE_ROLE_ID;
                        sdo.LoginName = item.LOGINNAME;
                        ACS_USER acsUser = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS_USER>()
                            .FirstOrDefault(o => o.LOGINNAME == item.LOGINNAME);
                        sdo.UserName = acsUser != null ? acsUser.USERNAME : "";
                        result.Add(sdo);
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void CalendarAddRemoveProccess(CALENDAR_ACTION action)
        {
            try
            {
                if (sereServ13Choose == null || sereServ13Choose.Count == 0)
                {
                    MessageBox.Show("Không có yêu cầu phẫu thuật nào được chọn");
                    return;
                }

                HisServiceReqCalendarSDO sereServ13CalendarSDO = new HisServiceReqCalendarSDO();
                sereServ13CalendarSDO.PtttCalendarId = ptttCalendar.ID;
                sereServ13CalendarSDO.ServiceReqIds = sereServ13Choose.Select(o => o.SERVICE_REQ_ID ?? 0).ToList();
                sereServ13CalendarSDO.WorkingRoomId = this.roomId;
                string requestUri = "";
                if (action == CALENDAR_ACTION.ADD)
                {
                    requestUri = "api/HisServiceReq/SurgCalendarAdd";
                }
                else if (action == CALENDAR_ACTION.REMOVE)
                {
                    requestUri = "api/HisServiceReq/SurgCalendarRemove";
                }

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                var result = new BackendAdapter(param)
                        .Post<bool>(requestUri, ApiConsumers.MosConsumer, sereServ13CalendarSDO, param);
                WaitingManager.Hide();
                if (result)
                {
                    rdoGroupCalendar.SelectedIndex = 0;
                    FillDataToGridServiceReq();
                    sereServ13Choose = new List<V_HIS_SERE_SERV_13>();
                }
                MessageManager.Show(this.ParentForm, param, result);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ApprovalPlanProcess(V_HIS_SERE_SERV_13 sereServ13Temp, APPROVAL_PLAN_ACTION action)
        {
            try
            {
                if (sereServ13Temp != null && CheckApprovalPlan(sereServ13Temp, action))
                {

                    int proccessIndex = 0;
                    if (action == APPROVAL_PLAN_ACTION.APPROVAL)
                    {
                        proccessIndex = 1;
                    }
                    else if (action == APPROVAL_PLAN_ACTION.UNAPPROVAL)
                    {
                        proccessIndex = 2;
                    }
                    else if (action == APPROVAL_PLAN_ACTION.REJECT)
                    {
                        proccessIndex = 3;
                    }
                    else if (action == APPROVAL_PLAN_ACTION.UNREJECT)
                    {
                        proccessIndex = 4;
                    }
                    frmApprovalInfo frm = new frmApprovalInfo(proccessIndex, RefeshApprovalInfoResult);
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action">1: APPROVAL, 2 :UNPROVAL, 3: REJECT</param>
        /// <param name="time"></param>
        /// <param name="user"></param>
        private void RefeshApprovalInfoResult(int action, long time, ACS_USER user)
        {
            bool success = false;
            try
            {
                V_HIS_SERE_SERV_13 sereServ13Temp = gridViewServiceReq.GetFocusedRow() as V_HIS_SERE_SERV_13;
                if (sereServ13Temp != null)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    HisServiceReqPlanApproveSDO hisServiceReqPlanApproveSDO = new HisServiceReqPlanApproveSDO();
                    hisServiceReqPlanApproveSDO.WorkingRoomId = this.roomId;
                    hisServiceReqPlanApproveSDO.ServiceReqId = sereServ13Temp.SERVICE_REQ_ID ?? 0;
                    hisServiceReqPlanApproveSDO.Time = time;
                    hisServiceReqPlanApproveSDO.Loginname = user.LOGINNAME;
                    hisServiceReqPlanApproveSDO.Username = user.USERNAME;
                    string stringUri = "";
                    switch (action)
                    {
                        case 1:
                            stringUri = "api/HisServiceReq/SurgPlanApprove";
                            break;
                        case 2:
                            stringUri = "api/HisServiceReq/SurgPlanUnapprove";
                            break;
                        case 3:
                            stringUri = "api/HisServiceReq/SurgPlanReject";
                            break;
                        case 4:
                            stringUri = "api/HisServiceReq/SurgPlanUnreject";
                            break;
                        default:
                            break;
                    }

                    var result = new BackendAdapter(param)
                    .Post<HIS_SERVICE_REQ>(stringUri, ApiConsumers.MosConsumer, hisServiceReqPlanApproveSDO, param);
                    if (result != null)
                    {
                        success = true;
                        FillDataToGridServiceReq();
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
