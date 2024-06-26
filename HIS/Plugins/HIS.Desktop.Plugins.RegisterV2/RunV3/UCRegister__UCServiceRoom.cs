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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using HIS.UC.ServiceRoom;
using HIS.UC.ServiceRoom.ADO;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Linq;
using System.Windows.Forms;
using HIS.UC.UCServiceRoomInfo;
using HIS.UC.UCServiceRoomInfo.ADO;
using MOS.SDO;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using System.Collections.Generic;
using MOS.Filter;
using HIS.Desktop.ApiConsumer;

namespace HIS.Desktop.Plugins.RegisterV2.Run2
{
    public partial class UCRegister : UserControlBase
    {
        void InitExamServiceRoom()
        {
            try
            {
                ServiceRoomInitADO serviceRoomInitADO = new ServiceRoomInitADO();
                serviceRoomInitADO.RoomId = this.currentModule.RoomId;
                serviceRoomInitADO.dlgGetPatientTypeId = GetPatientTypeId;
                serviceRoomInitADO.DelegateFocusNextUserControl = FoucusMoveOutServiceRoomInfo;
                serviceRoomInitADO.IsFocusCombo = HisConfigCFG.IsByPassTextBoxRoomCode;
                if (this.ucPatientRaw1.GetValue().PATIENTTYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                {
                    serviceRoomInitADO.RegisterPatientWithRightRouteBHYT = ProcessRegisterPatientWithRightRouteBHYT;
                    serviceRoomInitADO.ChangeRoomNotEmergency = ProcessChangeRoomNotEmergency;
                }
                serviceRoomInitADO.dlgGetIntructionTime = GetIntructionTime;
                this.ucServiceRoomInfo1.InitForm(serviceRoomInitADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void InitExamServiceRoomByAppoimentTime(HisPatientSDO patientSDO)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("InitExamServiceRoomByAppoimentTime.1");
                if (patientSDO != null && patientSDO.AppointmentTime.HasValue && patientSDO.AppointmentExamServiceId.HasValue && patientSDO.AppointmentExamRoomIds != null && patientSDO.AppointmentExamRoomIds.Count > 0)
                {
                    V_HIS_SERE_SERV sereServExamForAppoiment = new V_HIS_SERE_SERV();
                    sereServExamForAppoiment.SERVICE_ID = patientSDO.AppointmentExamServiceId.Value;
                    sereServExamForAppoiment.TDL_EXECUTE_ROOM_ID = patientSDO.AppointmentExamRoomIds.First();

                    this.ucServiceRoomInfo1.InitExamServiceRoomByAppoiment(sereServExamForAppoiment, patientSDO);

                    if (Inventec.Common.DateTime.Convert.TimeNumberToDateString(patientSDO.AppointmentTime.Value) != Inventec.Common.DateTime.Convert.SystemDateTimeToDateString(DateTime.Now))
                    {
                        var executeRooms = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>();
                        var executeRoom = (executeRooms != null && executeRooms.Count > 0) ? executeRooms.Where(t => t.ROOM_ID == sereServExamForAppoiment.TDL_EXECUTE_ROOM_ID).FirstOrDefault() : null;
                        string message = String.Format(ResourceMessage.BenhNhanCoHenKhamVaoNgayTaiPhongKhamY, Inventec.Common.DateTime.Convert.TimeNumberToDateString(patientSDO.AppointmentTime.Value), (executeRoom != null ? executeRoom.EXECUTE_ROOM_NAME : ""));
                        Inventec.Common.Logging.LogSystem.Debug(message);
                        XtraMessageBox.Show(message);
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("InitExamServiceRoomByAppoimentTime.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessRegisterPatientWithRightRouteBHYT()
        {
            try
            {
                if (this.ucHeinInfo1 != null)
                    this.ucHeinInfo1.RightRouteEmergencyWhenRegisterOutTime(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessChangeRoomNotEmergency()
        {
            try
            {
                if (this.ucHeinInfo1 != null)
                    this.ucHeinInfo1.ChangeRoomNotEmergency();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void FoucusMoveOutServiceRoomInfo()
        {
            try
            {
                if (HisConfigCFG.IsAutoFocusToSavePrintAfterChoosingExam)
                {
                    this.focusToBtnSaveAndPrint();
                }
                else
                {
                    this.focusToBtnSave();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ReloadExamServiceRoom()
        {
            try
            {
                ServiceRoomInitADO serviceRoomInitADO = new ServiceRoomInitADO();
                serviceRoomInitADO.dlgGetPatientTypeId = GetPatientTypeId;
                serviceRoomInitADO.RoomId = this.currentModule.RoomId;
                serviceRoomInitADO.DelegateFocusNextUserControl = FoucusMoveOutServiceRoomInfo;
                serviceRoomInitADO.IsFocusCombo = HisConfigCFG.IsByPassTextBoxRoomCode;
                if (this.ucPatientRaw1.GetValue().PATIENTTYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                {
                    serviceRoomInitADO.RegisterPatientWithRightRouteBHYT = ProcessRegisterPatientWithRightRouteBHYT;
                }

                serviceRoomInitADO.dlgGetIntructionTime = GetIntructionTime;
                this.ucServiceRoomInfo1.ReloadExamServiceRoom(serviceRoomInitADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void FillDataToExamServiceReqByPatient(HisPatientSDO data)
        {
            try
            {
                if (data == null) throw new ArgumentNullException("FillDataToExamServiceReqByPatient. Get HisPatientSDO is null");
                if (AppConfigs.IsAutoFillDataRecentServiceRoom == "1")
                {
                    if (this.ucServiceRoomInfo1 != null && this.ucServiceRoomInfo1 != null)
                    {
                        V_HIS_PATIENT vPatient = new V_HIS_PATIENT();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_PATIENT>(vPatient, data);

                        this.ucServiceRoomInfo1.SetValueExamServiceRoom(vPatient);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DeleteServiceRoomInfo(object LayoutControlItemName)
        {
            bool result = false;
            try
            {
                if (LayoutControlItemName != null)
                {
                    foreach (Control item in this.ucHeinInfo1.Controls)
                    {
                        if (item != null && (item is UserControl || item is XtraUserControl) && item.Name == (string)LayoutControlItemName)
                        {
                            this.ucHeinInfo1.Controls.Remove(item);
                            item.Dispose();
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            GC.Collect();
                            result = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }
    }
}
