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
using HIS.Desktop.Plugins.HisHoldReturn.ADO;
using HIS.Desktop.Plugins.HisHoldReturn.Resources;
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

namespace HIS.Desktop.Plugins.HisHoldReturn.HoldReturn
{
    public partial class UCHoldReturn : UserControlBase
    {

        private void ProcessAddClick()
        {
            try
            {
                WaitingManager.Show();
                bool valid = true;
                this.positionHandleControl = -1;
                HIS_HOLD_RETURN holdReturn = null;
                CommonParam param = new CommonParam();
                valid = valid && ValidData(param);
                valid = valid && ValidRowHoreDhtySelected(param);
                valid = this.dxValidationProviderControl.Validate() && valid;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("UCHoldReturn.ProcessAddClick.valid", valid));
                if (valid)
                {
                    bool success = false;
                    this.ChangeLockButtonWhileProcess(false);

                    switch (this.actionType)
                    {
                        case GlobalVariables.ActionAdd:
                            HisHoldReturnCreateSDO holdReturnCreate = new HisHoldReturnCreateSDO();
                            UpdateDataFormToCreateSdo(ref holdReturnCreate);
                            Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => holdReturnCreate), holdReturnCreate));
                            holdReturn = new BackendAdapter(param).Post<HIS_HOLD_RETURN>(RequestUriStore.HIS_HOLD_RETURN_CREATE, ApiConsumers.MosConsumer, holdReturnCreate, ProcessLostToken, param);
                           
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("holdReturnCreate", holdReturnCreate) + "____" + Inventec.Common.Logging.LogUtil.TraceData("holdReturn", holdReturn) + "____" + Inventec.Common.Logging.LogUtil.TraceData("success", success));
                            break;
                        case GlobalVariables.ActionEdit:
                            HisHoldReturnUpdateSDO holdReturnUpdate = new HisHoldReturnUpdateSDO();
                            UpdateDataFormToUpdateSdo(ref holdReturnUpdate);
                            Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => holdReturnUpdate), holdReturnUpdate));
                            holdReturn = new BackendAdapter(param).Post<HIS_HOLD_RETURN>(RequestUriStore.HIS_HOLD_RETURN_UPDATE, ApiConsumers.MosConsumer, holdReturnUpdate, ProcessLostToken, param);

                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("holdReturnUpdate", holdReturnUpdate) + "____" + Inventec.Common.Logging.LogUtil.TraceData("holdReturn", holdReturn) + "____" + Inventec.Common.Logging.LogUtil.TraceData("success", success));
                            break;
                        default:
                            break;
                    }

                    if (holdReturn != null)
                    {
                        this.actionType = GlobalVariables.ActionView;
                        this.SetEnableButtonControl(this.actionType);
                        this.FillDataToGridHoldReturn();
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
                }
                else
                {
                    WaitingManager.Hide();
                    if (param.Messages != null && param.Messages.Count > 0)
                    {
                        MessageManager.Show(this.ParentForm, param, null);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                this.ChangeLockButtonWhileProcess(true);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void UpdateDataFormToCreateSdo(ref HisHoldReturnCreateSDO holdReturnCreate)
        {
            try
            {
                holdReturnCreate = holdReturnCreate == null ? new HisHoldReturnCreateSDO() : holdReturnCreate;
                holdReturnCreate.HeinCardNumber = lblHeinCardNumber.Text;
                holdReturnCreate.WorkingRoomId = this.requestRoom.ID;
                //holdReturnCreate.PatientId = this.currentPatientId;
                holdReturnCreate.TreatmentId = this.currentTreatment.ID;
                holdReturnCreate.DocHoldTypeIds = (from m in this.currentDocHoldTypeSelecteds
                                                   select m.ID).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void UpdateDataFormToUpdateSdo(ref HisHoldReturnUpdateSDO holdReturnUpdate)
        {
            try
            {
                holdReturnUpdate = holdReturnUpdate == null ? new HisHoldReturnUpdateSDO() : holdReturnUpdate;
                holdReturnUpdate.HeinCardNumber = this.lblHeinCardNumber.Text;
                holdReturnUpdate.WorkingRoomId = this.requestRoom.ID;
                //holdReturnUpdate.PatientId = this.currentPatientId;
                holdReturnUpdate.TreatmentId = this.currentTreatment.ID;
                holdReturnUpdate.Id = this.currentHoldReturn.ID;
                holdReturnUpdate.HoldTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dateHoldTime.DateTime) ?? 0;
                holdReturnUpdate.DocHoldTypeIds = (from m in this.currentDocHoldTypeSelecteds
                                                   select m.ID).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool ValidData(CommonParam param)
        {
            bool valid = true;
            try
            {
                //if (String.IsNullOrEmpty(this.lblHeinCardNumber.Text))
                //{
                //    throw new ArgumentNullException("HeinCardNumber");
                //}
                if (requestRoom == null || requestRoom.ID == 0)
                {
                    throw new ArgumentNullException("requestRoom");
                }
                if (currentPatientId == 0)
                {
                    throw new ArgumentNullException("currentPatientId");
                }
                if (this.actionType == GlobalVariables.ActionEdit && (currentHoldReturn == null || currentHoldReturn.ID == 0))
                {
                    throw new ArgumentNullException("currentHoldReturn");
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
                param.Messages.Add(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc));
            }

            return valid;
        }

        bool ValidRowHoreDhtySelected(CommonParam param)
        {
            bool valid = this.currentDocHoldTypeSelecteds != null && this.currentDocHoldTypeSelecteds.Count > 0;
            if (!valid)
            {
                param.Messages.Add("Chưa chọn loại giấy tờ");
            }
            return valid;
        }

        private void SetEnableButtonControl(int actionType)
        {
            try
            {
                this.btnSave.Enabled = (this.actionType == GlobalVariables.ActionAdd);
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

                this.btnSave.Enabled = isLock;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
