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
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Plugins.VaccinationExam.Base;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.VaccinationExam
{
    public partial class UCVaccinationExam : UserControlBase
    {
        Thread threadCallPatient;
        string[] listReplace = new string[] { "(", ")" };

        private void UpdateDicCallPatient(V_HIS_VACCINATION_EXAM currentVaccination)
        {
            try
            {
                if (currentVaccination != null)
                {
                    if (!CallPatientDataWorker.DicCallPatient.ContainsKey(roomId))
                    {
                        CallPatientDataWorker.DicCallPatient.Add(roomId, new List<ServiceReq1ADO>());
                    }

                    foreach (var dic in HIS.Desktop.LocalStorage.BackendData.CallPatientDataWorker.DicCallPatient)
                    {
                        if (dic.Key == roomId)
                        {
                            List<long> vaccinationIds = dic.Value != null ? dic.Value.Select(o => o.VaccinationId ?? 0).ToList() : new List<long>();

                            if (!vaccinationIds.Contains(currentVaccination.ID))
                            {
                                ServiceReq1ADO serviceReq1ADO = new ServiceReq1ADO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<ServiceReq1ADO>(serviceReq1ADO, currentVaccination);
                                serviceReq1ADO.VaccinationId = currentVaccination.ID;
                                serviceReq1ADO.ID = 0;
                                dic.Value.Add(serviceReq1ADO);
                            }

                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentVaccination), currentVaccination) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => roomId), roomId) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dic.Value), dic.Value));

                            foreach (var item in dic.Value)
                            {
                                if (item.VaccinationId == currentVaccination.ID)
                                {
                                    item.CallPatientSTT = true;
                                    item.IsCalling = true;
                                }
                                else
                                {
                                    item.CallPatientSTT = false;
                                    item.IsCalling = false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCallPatientByThread(object param)
        {
            Inventec.Common.Logging.LogSystem.Debug("LoadCallPatientByThread. 1");
            if (threadCallPatient != null && (threadCallPatient.ThreadState == ThreadState.WaitSleepJoin || threadCallPatient.ThreadState == ThreadState.Running))
            {
                MessageBox.Show("Đang gọi bệnh nhân", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            threadCallPatient = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataToControlCallPatientThread));
            try
            {
                threadCallPatient.Start(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadCallPatient.Abort();
            }
            Inventec.Common.Logging.LogSystem.Debug("LoadCallPatientByThread. 2");
        }

        private void LoadDataToControlCallPatientThread(object param)
        {
            try
            {
                CallPatient(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CallPatient(object param)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("CallPatient. 1");
                if (param is V_HIS_VACCINATION_EXAM)
                {
                    var data = param as V_HIS_VACCINATION_EXAM;
                    if (data != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("CallPatient. 2");
                        HIS_EXECUTE_ROOM executeRoom = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().FirstOrDefault(o => o.ROOM_ID == roomId);
                        string executeRoomName = executeRoom != null ? executeRoom.EXECUTE_ROOM_NAME : null;
                        CallPatientByNumOder(data, executeRoomName);
                        Inventec.Common.Logging.LogSystem.Debug("CallPatient. 3");
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("CallPatient. 4");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CallPatientByNumOder(V_HIS_VACCINATION_EXAM data, string examRoomName)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("CallPatientByNumOder. 1");
                if (data == null)
                {
                    throw new Exception("data is " + data);
                }
                string patientName = data.TDL_PATIENT_NAME;
                for (int i = 0; i < listReplace.Length; i++)
                {
                    patientName = patientName.Replace(listReplace[i], "");
                }

                if (CallPatientDataWorker.DicDelegateCallingPatient != null && CallPatientDataWorker.DicDelegateCallingPatient.ContainsKey(this.currentModuleBase.RoomId))
                {

                    DelegateSelectData nhapNhay = CallPatientDataWorker.DicDelegateCallingPatient[this.currentModuleBase.RoomId];
                    if (nhapNhay != null) nhapNhay(data);
                }

                long callPatientFormat = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>(AppConfigKeys.CALL_PATIENT_FORMAT));

                string moiBenhNhanStr = HisConfigs.Get<string>(AppConfigKeys.CALL_PATIENT_MOI_BENH_NHAN);
                string coSoSttStr = HisConfigs.Get<string>(AppConfigKeys.CALL_PATIENT_CO_STT);
                string denStr = HisConfigs.Get<string>(AppConfigKeys.CALL_PATIENT_DEN);
                bool isCallPatientName = HisConfigs.Get<string>(AppConfigKeys.CALL_PATIENT_NAME__DISABLE) != "1" ? true : false;
                string sinhNamStr = HisConfigs.Get<string>(AppConfigKeys.CALL_PATIENT_DOB_YEAR);
                bool isCallPatientDob = HisConfigs.Get<string>(AppConfigKeys.IS_CALL_PATIENT_DOB_YEAR) == "1" ? true : false;

                Inventec.Speech.SpeechPlayer.TypeSpeechCFG = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("Inventec.Speech.TypeSpeechCFG");

                // 1: Gọi theo Tên bệnh nhân và số thứ tự
                //2: Gọi theo Tên bệnh nhân và năm sinh
                //3: Gọi theo Tên bệnh nhân và cả số thứ tự và năm sinh
                //MẶc định : 1
                Inventec.Common.Logging.LogSystem.Debug("CallPatientByNumOder. 2");
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => moiBenhNhanStr), moiBenhNhanStr)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => moiBenhNhanStr), moiBenhNhanStr)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientName), patientName)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => coSoSttStr), coSoSttStr)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => callPatientFormat), callPatientFormat)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sinhNamStr), sinhNamStr)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isCallPatientDob), isCallPatientDob)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Inventec.Speech.SpeechPlayer.TypeSpeechCFG), Inventec.Speech.SpeechPlayer.TypeSpeechCFG)
                    );
                Inventec.Speech.SpeechPlayer.SpeakSingle(moiBenhNhanStr);
                Inventec.Common.Logging.LogSystem.Debug("CallPatientByNumOder. 2.1");
                if (isCallPatientName)
                    Inventec.Speech.SpeechPlayer.Speak(patientName);
                Inventec.Common.Logging.LogSystem.Debug("CallPatientByNumOder. 2.2");
                if (callPatientFormat == 1)
                {
                    Inventec.Common.Logging.LogSystem.Debug("CallPatientByNumOder. 2.3");
                    Inventec.Speech.SpeechPlayer.SpeakSingle(coSoSttStr);
                    if (data.NUM_ORDER.HasValue)
                        Inventec.Speech.SpeechPlayer.Speak(data.NUM_ORDER.Value);
                }
                else if (callPatientFormat == 2)
                {
                    if (isCallPatientDob)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("CallPatientByNumOder. 2.4");
                        Inventec.Speech.SpeechPlayer.SpeakSingle(sinhNamStr);
                        string year = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                        Inventec.Speech.SpeechPlayer.Speak(year);
                    }
                }
                else
                {
                    if (isCallPatientDob)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("CallPatientByNumOder. 2.5");
                        Inventec.Speech.SpeechPlayer.SpeakSingle(sinhNamStr);
                        string year = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                        Inventec.Speech.SpeechPlayer.Speak(year);
                    }
                    Inventec.Common.Logging.LogSystem.Debug("CallPatientByNumOder. 2.6");
                    Inventec.Speech.SpeechPlayer.SpeakSingle(coSoSttStr);
                    if (data.NUM_ORDER.HasValue)
                        Inventec.Speech.SpeechPlayer.Speak(data.NUM_ORDER.Value);
                    Inventec.Common.Logging.LogSystem.Debug("CallPatientByNumOder. 2.7");
                }

                Inventec.Speech.SpeechPlayer.SpeakSingle(denStr);
                Inventec.Common.Logging.LogSystem.Debug("CallPatientByNumOder. 2.8");
                Inventec.Speech.SpeechPlayer.SpeakSingle(examRoomName);
                Inventec.Common.Logging.LogSystem.Debug("CallPatientByNumOder. 3");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
