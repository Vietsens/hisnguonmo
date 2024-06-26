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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.KidneyShiftSchedule.ADO;
using HIS.Desktop.Plugins.KidneyShiftSchedule.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.KidneyShiftSchedule.KidneyShift
{
    public partial class UCKidneyShift : UserControlBase
    {

        private void ProcessAddClick()
        {
            try
            {
                WaitingManager.Show();
                bool valid = true;
                this.positionHandleControl = -1;
                TreatmentBedRoomADO treatmentBedRoomADO = null;
                CommonParam param = new CommonParam();
                valid = valid && ValidRowTreatmentSelected(ref treatmentBedRoomADO);
                valid = this.dxValidationProviderControl.Validate() && valid;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("UCKidneyShift.valid", valid));
                if (valid)
                {
                    bool success = false;
                    this.ChangeLockButtonWhileProcess(false);

                    HisServiceReqKidneyScheduleSDO serviceReqKidneyScheduleSDO = new HisServiceReqKidneyScheduleSDO();
                    serviceReqKidneyScheduleSDO.ExecuteTime = Inventec.Common.TypeConvert.Parse.ToInt64(dateDateForAdd.DateTime.ToString("yyyyMMddHHmmss"));
                    if (cboExpMestTemplateForAdd.EditValue != null)
                        serviceReqKidneyScheduleSDO.ExpMestTemplateId = (long)cboExpMestTemplateForAdd.EditValue;
                    serviceReqKidneyScheduleSDO.KidneyShift = (long)cboCaForAdd.EditValue;
                    serviceReqKidneyScheduleSDO.MachineId = (long)cboMarchineForAdd.EditValue;
                    serviceReqKidneyScheduleSDO.Note = txtNoteForAdd.Text;
                    serviceReqKidneyScheduleSDO.PatientTypeId = (long)cboPatientType.EditValue;
                    serviceReqKidneyScheduleSDO.WorkingRoomId = requestRoom.ID;
                    serviceReqKidneyScheduleSDO.RoomId = (long)cboExecuteRoom.EditValue;
                    serviceReqKidneyScheduleSDO.ServiceId = (long)cboServiceForAdd.EditValue;
                    serviceReqKidneyScheduleSDO.TreatmentId = treatmentBedRoomADO.TREATMENT_ID;
                    serviceReqKidneyScheduleSDO.RequestUsername = cboUser.Text;
                    serviceReqKidneyScheduleSDO.RequestLoginname = txtLoginName.Text;

                    this.serviceReqListResultSDO = new BackendAdapter(param).Post<HisServiceReqListResultSDO>(RequestUriStore.HIS_SERVICE_REQ__KIDNEYS_CHEDULE, ApiConsumers.MosConsumer, serviceReqKidneyScheduleSDO, ProcessLostToken, param);

                    if (this.serviceReqListResultSDO != null && this.serviceReqListResultSDO.ServiceReqs != null && this.serviceReqListResultSDO.ServiceReqs.Count > 0)
                    {
                        this.actionType = GlobalVariables.ActionView;
                        this.SetEnableButtonControl(this.actionType);
                        this.FillDataToGridServiceReqKidneyShift();
                        success = true;
                    }

                    this.ChangeLockButtonWhileProcess(true);
                    WaitingManager.Hide();

                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    SessionManager.ProcessTokenLost(param);
                    #endregion

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("serviceReqKidneyScheduleSDO", serviceReqKidneyScheduleSDO) + "____" + Inventec.Common.Logging.LogUtil.TraceData("serviceReqListResultSDO", serviceReqListResultSDO) + "____" + Inventec.Common.Logging.LogUtil.TraceData("success", success));
                }
                else
                    WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                this.ChangeLockButtonWhileProcess(true);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetEnableButtonControl(int actionType)
        {
            try
            {
                this.btnAddIntoSchedule.Enabled = (this.actionType == GlobalVariables.ActionAdd);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangeLockButtonWhileProcess(bool isLock)
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionView)
                    return;

                this.btnAddIntoSchedule.Enabled = isLock;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool ValidRowTreatmentSelected(ref TreatmentBedRoomADO treatmentBedRoomADO)
        {
            treatmentBedRoomADO = (TreatmentBedRoomADO)this.gridViewTreatmentBedRoom.GetFocusedRow();

            return treatmentBedRoomADO != null;
        }
    }
}
