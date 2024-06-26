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
using LIS.EFMODEL.DataModels;
using System.Reflection;
using HIS.Desktop.Plugins.SampleCollectionRoom.ADO;

namespace HIS.Desktop.Plugins.SampleCollectionRoom
{
    public partial class SampleCollectionRoomUC : HIS.Desktop.Utility.UserControlBase
    {
        private string callPatientFormName = "";

        public void LoadCallPatientByThread(object param)
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataToControlCallPatientThread));
            try
            {
                thread.Start(param);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                thread.Abort();
            }
        }

        internal void LoadDataToControlCallPatientThread(object param)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { CallPatient(param); }));
                }
                else
                {
                    CallPatient(param);
                }
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
                if (param is SampleListViewADO)
                {
                    var data = param as SampleListViewADO;
                    if (data != null)
                    {
                        CallPatientByNumOder(data.LAST_NAME + " " + data.FIRST_NAME, data.CALL_SAMPLE_ORDER ?? 0, data.SAMPLE_ROOM_NAME);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void CallPatientByNumOder(string patientName, long numOder, string examRoomName)
        {
            try
            {
                Inventec.Speech.SpeechPlayer.TypeSpeechCFG = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("Inventec.Speech.TypeSpeechCFG");
                string moiBenhNhanStr = HisConfigs.Get<string>(SdaConfigKeys.CALL_PATIENT_MOI_BENH_NHAN);
                string coSoSttStr = HisConfigs.Get<string>(SdaConfigKeys.CALL_PATIENT_CO_STT);
                string denStr = HisConfigs.Get<string>(SdaConfigKeys.CALL_PATIENT_DEN);

                Inventec.Speech.SpeechPlayer.SpeakSingle(moiBenhNhanStr);
                Inventec.Speech.SpeechPlayer.Speak(patientName);
                Inventec.Speech.SpeechPlayer.SpeakSingle(coSoSttStr);
                Inventec.Speech.SpeechPlayer.Speak(numOder);
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

        async void CallPatient()
        {
            try
            {
                if (!btnCallPatient.Enabled)
                    return;
                if (String.IsNullOrWhiteSpace(txtGateNumber.Text) || String.IsNullOrEmpty(txtStepNumber.Text))
                    return;
                string configKeyCallPatientCPA = ConfigApplicationWorker.Get<string>(AppConfigKey.CONFIG_KEY__DANG_KY_TIEP_DON__GOI_BENH_NHAN_BANG_PHAN_MEM_CPA);
                if (configKeyCallPatientCPA == "1")
                {
                    if (this.clienttManager == null)
                        this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager();
                    int[] nums = await this.clienttManager.AsyncCallNumOrderPlusString(txtGateNumber.Text, int.Parse(this.txtStepNumber.Text));
                    if (nums != null && nums.Length > 0)
                    {
                        await this.CallModuleCallPatientNumOrder(nums);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateDicCallPatient(SampleListViewADO lisSample)
        {
            try
            {
                if (lisSample != null)
                {

                    if (!CallPatientDataWorker.DicCallPatient.ContainsKey(this.currentModule.RoomId))
                    {
                        CallPatientDataWorker.DicCallPatient.Add(this.currentModule.RoomId, new List<ServiceReq1ADO>());
                    }

                    foreach (var dic in HIS.Desktop.LocalStorage.BackendData.CallPatientDataWorker.DicCallPatient)
                    {
                        if (dic.Key == this.currentModule.RoomId)
                        {
                            ServiceReq1ADO ado = dic.Value != null ? dic.Value.FirstOrDefault(o => !String.IsNullOrWhiteSpace(o.SERVICE_REQ_CODE) && o.SERVICE_REQ_CODE.Contains(lisSample.SERVICE_REQ_CODE)) : null; ;

                            if (ado == null)
                            {
                                ServiceReq1ADO serviceReq1ADO = new ServiceReq1ADO();
                                serviceReq1ADO.ID = lisSample.ID;
                                serviceReq1ADO.SERVICE_REQ_CODE = lisSample.SERVICE_REQ_CODE;
                                serviceReq1ADO.TDL_PATIENT_CODE = lisSample.PATIENT_CODE;
                                serviceReq1ADO.TDL_PATIENT_NAME = (lisSample.LAST_NAME ?? "" + " " + lisSample.FIRST_NAME ?? "").Trim();
                                serviceReq1ADO.NUM_ORDER = lisSample.CALL_SAMPLE_ORDER;
                                serviceReq1ADO.TDL_PATIENT_DOB = lisSample.DOB ?? 0;
                                dic.Value.Add(serviceReq1ADO);
                            }

                            Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dic.Value), dic.Value));

                            foreach (var item in dic.Value)
                            {
                                if (!String.IsNullOrWhiteSpace(item.SERVICE_REQ_CODE) && (item.SERVICE_REQ_CODE == lisSample.SERVICE_REQ_CODE || item.SERVICE_REQ_CODE.Contains(lisSample.SERVICE_REQ_CODE)))
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

        private void ButtonEdit_CallPatient_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var currentHisServiceReq = (SampleListViewADO)gridViewSample.GetFocusedRow();
                if (currentHisServiceReq != null)
                {
                    UpdateDicCallPatient(currentHisServiceReq);
                    LoadCallPatientByThread(currentHisServiceReq);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task CallModuleCallPatientNumOrder(int[] num)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(callPatientFormName))
                {
                    callPatientFormName = "WAITING_PATIENT_SAMPLE_" + this.room.ROOM_CODE;
                }
                Form waitingForm = null;
                if (Application.OpenForms != null && Application.OpenForms.Count > 0)
                {
                    for (int i = 0; i < Application.OpenForms.Count; i++)
                    {
                        Form f = Application.OpenForms[i];
                        if (f.Name == callPatientFormName)
                        {
                            waitingForm = f;
                        }
                    }
                }
                if (waitingForm != null)
                {
                    MethodInfo theMethod = waitingForm.GetType().GetMethod("CallNumOrder");
                    if (theMethod != null)
                    {
                        object[] param = new object[] { num.FirstOrDefault(), num.LastOrDefault() };
                        theMethod.Invoke(waitingForm, param);
                    }
                }
                else
                {
                    LogSystem.Warn("Nguoi dung chua mo man hinh cho CALL_PATIENT_NUM_ORDER");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThreadReCallPatient()
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(ReacallCallPatientNewThread));
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


        private void ReacallCallPatientNewThread()
        {
            try
            {
                this.ReCallPatient();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async void ReCallPatient()
        {
            try
            {
                if (!btnCallPatient.Enabled)
                    return;
                if (String.IsNullOrEmpty(txtStepNumber.Text) || String.IsNullOrEmpty(txtGateNumber.Text))
                    return;
                string configKeyCallPatientCPA = ConfigApplicationWorker.Get<string>(AppConfigKey.CONFIG_KEY__DANG_KY_TIEP_DON__GOI_BENH_NHAN_BANG_PHAN_MEM_CPA);
                if (configKeyCallPatientCPA == "1")
                {
                    if (this.clienttManager == null)
                        this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager();
                    int[] nums = await this.clienttManager.AsyncRecallNumOrderPlusString(txtGateNumber.Text.Trim(), int.Parse(txtStepNumber.Text));
                    if (nums != null && nums.Length > 0)
                    {
                        await this.CallModuleCallPatientNumOrder(nums);
                    }
                }
                else
                {
                    if (this.clienttManager == null)
                        this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager();
                    this.clienttManager.RecallNumOrder(int.Parse(txtGateNumber.Text), int.Parse(txtStepNumber.Text));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
