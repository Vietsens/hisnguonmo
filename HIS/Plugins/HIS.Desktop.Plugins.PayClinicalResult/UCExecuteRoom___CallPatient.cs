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
using System.Collections;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using Inventec.Common.Logging;
using HIS.Desktop.Controls.Session;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.UC.TreeSereServ7;
using MOS.SDO;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.RichEditor.Base;
using HIS.Desktop.ADO;
using System.Threading;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;

namespace HIS.Desktop.Plugins.PayClinicalResult
{

    public partial class UCExecuteRoom : HIS.Desktop.Utility.UserControlBase
    {
        string[] listReplace = new string[] { "(", ")" };

        Thread threadCallPatient;
        public void LoadCallPatientByThread(object param)
        {
            if (threadCallPatient != null && (threadCallPatient.ThreadState == ThreadState.WaitSleepJoin || threadCallPatient.ThreadState == ThreadState.Running))
            {
                MessageBox.Show("Đang gọi bệnh nhân", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            threadCallPatient = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataToControlCallPatientThread));
            //threadCallPatient.Priority = ThreadPriority.Highest;
            try
            {
                threadCallPatient.Start(param);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                threadCallPatient.Abort();
            }
        }

        internal void LoadDataToControlCallPatientThread(object param)
        {
            try
            {
                //if (this.InvokeRequired)
                //{
                //    this.Invoke(new MethodInvoker(delegate { CallPatient(param); }));
                //}
                //else
                //{
                CallPatient(param);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void CallPatient(object param)
        {
            try
            {
                if (param is HIS_SERVICE_REQ)
                {
                    var data = param as HIS_SERVICE_REQ;
                    if (data != null)
                    {
                        HIS_EXECUTE_ROOM executeRoom = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().FirstOrDefault(o => o.ROOM_ID == roomId);
                        string executeRoomName = executeRoom != null ? executeRoom.EXECUTE_ROOM_NAME : null;
                        CallPatientByNumOder(data, executeRoomName);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void CallPatientByNumOder(HIS_SERVICE_REQ data, string examRoomName)
        {
            try
            {
                if (data == null)
                {
                    throw new Exception("data is " + data);
                }
                string patientName = data.TDL_PATIENT_NAME;
                for (int i = 0; i < listReplace.Length; i++)
                {
                    patientName = patientName.Replace(listReplace[i], "");
                }

                long callPatientFormat = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>(SdaConfigKeys.CALL_PATIENT_FORMAT));

                string moiBenhNhanStr = HisConfigs.Get<string>(SdaConfigKeys.CALL_PATIENT_MOI_BENH_NHAN);
                string coSoSttStr = HisConfigs.Get<string>(SdaConfigKeys.CALL_PATIENT_CO_STT);
                string denStr = HisConfigs.Get<string>(SdaConfigKeys.CALL_PATIENT_DEN);
                bool isCallPatientName = HisConfigs.Get<string>(SdaConfigKeys.CALL_PATIENT_NAME__DISABLE) != "1" ? true : false;
                string sinhNamStr = HisConfigs.Get<string>(SdaConfigKeys.CALL_PATIENT_DOB_YEAR);
                bool isCallPatientDob = HisConfigs.Get<string>(SdaConfigKeys.IS_CALL_PATIENT_DOB_YEAR) == "1" ? true : false;

                Inventec.Speech.SpeechPlayer.TypeSpeechCFG = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("Inventec.Speech.TypeSpeechCFG");

                // 1: Gọi theo Tên bệnh nhân và số thứ tự
                //2: Gọi theo Tên bệnh nhân và năm sinh
                //3: Gọi theo Tên bệnh nhân và cả số thứ tự và năm sinh
                //MẶc định : 1

                Inventec.Speech.SpeechPlayer.SpeakSingle(moiBenhNhanStr);
                if (isCallPatientName)
                    Inventec.Speech.SpeechPlayer.Speak(patientName);

                if (callPatientFormat == 1)
                {
                    Inventec.Speech.SpeechPlayer.SpeakSingle(coSoSttStr);
                    if (data.NUM_ORDER.HasValue)
                        Inventec.Speech.SpeechPlayer.Speak(data.NUM_ORDER.Value);
                }
                else if (callPatientFormat == 2)
                {
                    if (isCallPatientDob)
                    {
                        Inventec.Speech.SpeechPlayer.SpeakSingle(sinhNamStr);
                        string year = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                        Inventec.Speech.SpeechPlayer.Speak(year);
                    }
                }
                else
                {
                    if (isCallPatientDob)
                    {
                        Inventec.Speech.SpeechPlayer.SpeakSingle(sinhNamStr);
                        string year = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                        Inventec.Speech.SpeechPlayer.Speak(year);
                    }

                    Inventec.Speech.SpeechPlayer.SpeakSingle(coSoSttStr);
                    if (data.NUM_ORDER.HasValue)
                        Inventec.Speech.SpeechPlayer.Speak(data.NUM_ORDER.Value);
                }

                Inventec.Speech.SpeechPlayer.SpeakSingle(denStr);
                Inventec.Speech.SpeechPlayer.SpeakSingle(examRoomName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CreateThreadCallPatient()
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(CallPatientNewThread));
            //thread.Priority = ThreadPriority.Highest;
            try
            {
                thread.Start();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                thread.Abort();
            }
        }

        private void CreateThreadCallPatientCPA(List<HIS_SERVICE_REQ> listServiceReq)
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(CallPatientCPANewThread));
            //thread.Priority = ThreadPriority.Highest;
            try
            {
                thread.Start(listServiceReq);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                thread.Abort();
            }
        }

        private void CallPatientNewThread()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { CallPatient(); }));
                }
                else
                {
                    CallPatient();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CallPatientCPANewThread(object listServiceReq)
        {
            try
            {
                //if (this.InvokeRequired)
                //{
                //    this.Invoke(new MethodInvoker(delegate { CallPatientCPA(listServiceReq); }));
                //}
                //else
                //{
                CallPatientCPA(listServiceReq);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void CallPatient()
        {
            try
            {
                if (!btnCallPatient.Enabled)
                    return;
                if (String.IsNullOrEmpty(txtGateNumber.Text) || String.IsNullOrEmpty(txtStepNumber.Text))
                    return;
                string configKeyCallPatientCPA = ConfigApplicationWorker.Get<string>(AppConfigKey.CONFIG_KEY__DANG_KY_TIEP_DON__GOI_BENH_NHAN_BANG_PHAN_MEM_CPA);
                if (configKeyCallPatientCPA == "1")
                {
                    if (this.clienttManager == null)
                        this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager();
                    this.clienttManager.CallNumOrder(int.Parse(txtGateNumber.Text), int.Parse(txtStepNumber.Text));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void CallPatientCPA(object data)
        {
            try
            {
                string configKeyCallPatientCPA = ConfigApplicationWorker.Get<string>(AppConfigKey.CONFIG_KEY__DANG_KY_TIEP_DON__GOI_BENH_NHAN_BANG_PHAN_MEM_CPA);

                List<HIS_SERVICE_REQ> listServiceReq = data as List<HIS_SERVICE_REQ>;
                if (configKeyCallPatientCPA == "1" && listServiceReq != null && listServiceReq.Count > 0)
                {
                    if (this.clienttManager == null)
                        this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager();
                    List<string> listPatientName = new List<string>();
                    foreach (var item in listServiceReq)
                    {
                        string namSinh = item.TDL_PATIENT_DOB > 0 ? item.TDL_PATIENT_DOB.ToString().Substring(0, 4) : "";
                        string patient = String.Format("{0}/{1}/{2}", item.TDL_PATIENT_NAME, namSinh, item.NUM_ORDER);
                        listPatientName.Add(patient);
                    }
                    this.clienttManager.CallPatientAndExecuteRoom(0, listPatientName);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateDicCallPatient(HIS_SERVICE_REQ serviceReq1)
        {
            try
            {
                if (serviceReq1 != null)
                {

                    if (!CallPatientDataWorker.DicCallPatient.ContainsKey(roomId))
                    {
                        CallPatientDataWorker.DicCallPatient.Add(roomId, new List<ServiceReq1ADO>());
                    }

                    foreach (var dic in HIS.Desktop.LocalStorage.BackendData.CallPatientDataWorker.DicCallPatient)
                    {
                        if (dic.Key == roomId)
                        {
                            List<long> serviceReqIds = dic.Value != null ? dic.Value.Select(o => o.ID).ToList() : new List<long>();

                            if (!serviceReqIds.Contains(serviceReq1.ID))
                            {
                                ServiceReq1ADO serviceReq1ADO = new ServiceReq1ADO();
                                AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ, ServiceReq1ADO>();
                                serviceReq1ADO = AutoMapper.Mapper.Map<HIS_SERVICE_REQ, ServiceReq1ADO>(serviceReq1);
                                dic.Value.Add(serviceReq1ADO);
                            }

                            Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dic.Value), dic.Value));

                            foreach (var item in dic.Value)
                            {
                                if (item.ID == serviceReq1.ID)
                                {
                                    item.CallPatientSTT = true;
                                }
                                else
                                {
                                    item.CallPatientSTT = false;
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
    }
}
