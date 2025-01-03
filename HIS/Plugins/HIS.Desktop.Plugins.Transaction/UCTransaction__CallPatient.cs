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
using HIS.Desktop.Utility;
using System.Threading;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using System.Reflection;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Speech;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;

namespace HIS.Desktop.Plugins.Transaction
{
    public partial class UCTransaction : UserControlBase
    {
        string[] listReplace = new string[] { "(", ")" };
        Thread threadCallPatient;
        private string callPatientFormName = "";
        List<HIS_REGISTER_GATE> _RegisterGates { get; set; }
        private List<long> RegisterReqIds = new List<long>();
        private Dictionary<string, List<HIS_REGISTER_REQ>> dicRegisterReq = new Dictionary<string, List<HIS_REGISTER_REQ>>();
        private long RegisterGateId;
        private int bFrom = 0;
        private int bTo = 0;
        string numSttNow = "0";
        string gateCode = "";

        string numTotal;
        string txtNumberPer = "";
        int count = 0;
        private async Task GATE()
        {
            try
            {
                _RegisterGates = new List<HIS_REGISTER_GATE>();
                MOS.Filter.HisRegisterGateFilter filter = new MOS.Filter.HisRegisterGateFilter();
                _RegisterGates = new BackendAdapter(null).Get<List<HIS_REGISTER_GATE>>("api/HisRegisterGate/Get", ApiConsumers.MosConsumer, filter, null);

                int timeSyncAll = ConfigApplicationWorker.Get<int>(AppConfigKey.CONFIG_KEY__HIS_DESKTOP__REGISTER__TIME__AUTO___CALL_REGISTER_REQ);
                if (timeSyncAll > 0)
                {
                    System.Windows.Forms.Timer timerSyncAll = new System.Windows.Forms.Timer();
                    timerSyncAll.Interval = timeSyncAll;
                    timerSyncAll.Enabled = true;
                    RegisterTimer(currentModule.ModuleLink, "timerSyncAll" + count, timerSyncAll.Interval, timerCall_Tick);
                    StartTimer(currentModule.ModuleLink, "timerSyncAll" + count);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void timerCall_Tick()
        {
            CreateCallTransaction();
        }
        private void CreateCallTransaction()
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(TransactionNewThread));
            try
            {
                thread.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                thread.Abort();
            }
        }
        private void TransactionNewThread()
        {
            try
            {
                this.Transaction();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private async void Transaction()
        {
            try
            {
                if (String.IsNullOrEmpty(this.txtGateNumber.Text))
                    return;
                if (this.clienttManager == null)
                    this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager();
                GetSttNowAndCallApi();
                txtNumberPer = !string.IsNullOrEmpty(numTotal) ? numSttNow + "/" + numTotal : numSttNow;
                this.lblSTT.Invoke(new MethodInvoker(delegate () { lblSTT.Text = txtNumberPer; }));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void CreateThreadCallPatient()
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(CallPatientNewThread));
            // thread.Priority = ThreadPriority.Highest;
            try
            {
                thread.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                thread.Abort();
            }
        }

        private void CallPatientNewThread()
        {
            try
            {
                this.CallPatient();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async void CallPatient()
        {
            try
            {
                if (!this.btnCallPatient.Enabled)
                    return;
                if (String.IsNullOrEmpty(this.txtStepNumber.Text) || String.IsNullOrEmpty(this.txtGateNumber.Text))
                    return;
                if (configKeyCallPatientCPA == "1")
                {
                    if (this.clienttManager == null)
                        this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager();
                    string gateCode = this.txtGateNumber.Text.Trim().Split(':')[0];
                    int[] nums = await this.clienttManager.AsyncCallNumOrderPlus(int.Parse(gateCode), int.Parse(this.txtStepNumber.Text));
                    if (nums != null && nums.Length > 0)
                    {
                        await this.CallModuleCallPatientNumOrder(nums.LastOrDefault(), int.Parse(gateCode));
                    }
                }
                else
                {
                    CallPatientNotCPA(false, true);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void CallPatientNotCPA(bool IsReCall, bool isCall)
        {
            try
            {
                string GateNum = txtGateNumber.Text.Trim();
                if (String.IsNullOrEmpty(GateNum))
                    return;
                GateNum = GateNum.Split(':')[0];
                long? Step = null;
                if (!string.IsNullOrEmpty(txtStepNumber.Text))
                    Step = Int64.Parse(txtStepNumber.Text);
                CommonParam param = new CommonParam();
                MOS.SDO.RegisterGateCallSDO sdo = new MOS.SDO.RegisterGateCallSDO();
                sdo.RegisterGateId = RegisterGateId;
                sdo.CallPlace = GateNum;
                sdo.CallStep = Step;
                List<HIS_REGISTER_REQ> apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post
                   <List<HIS_REGISTER_REQ>>
                   (IsReCall ? "api/HisRegisterGate/ReCall" : "api/HisRegisterGate/Call",
                   ApiConsumer.ApiConsumers.MosConsumer, param, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken);
                if (apiResult != null && apiResult.Count > 0)
                {
                    bFrom = (int)apiResult.Min(o => o.NUM_ORDER);
                    bTo = (int)apiResult.Max(o => o.NUM_ORDER);
                    txtFromNumber.Text = bFrom.ToString();
                    txtToNumber.Text = bTo.ToString();
                    numSttNow = txtToNumber.Text;
                    if (bFrom == bTo)
                    {
                        CallModuleCallPatientNumOrderRegisterV2(bFrom.ToString());
                    }
                    else
                    {
                        CallModuleCallPatientNumOrderRegisterV2(bFrom.ToString() + " - " + bTo.ToString());
                    }
                }
                else
                {
                    isCall = false;
                    if (IsReCall)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(
                               "Không tìm thấy số thứ tự trước đó. Vui lòng thử lại sau",
                               "Thông báo");
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(
                               "Hiện tại không có số thứ tự tiếp theo. Vui lòng thử lại sau",
                               "Thông báo");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private async Task CallModuleCallPatientNumOrderRegisterV2(string num)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(callPatientFormName))
                {
                    V_HIS_ROOM room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
                    callPatientFormName = "WAITING_NUM_ORDER_" + room.ROOM_CODE;
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
                    MethodInfo theMethod = waitingForm.GetType().GetMethod("SetNumOrder");
                    if (theMethod != null)
                    {
                        object[] param = new object[] { num };
                        theMethod.Invoke(waitingForm, param);
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Warn("Nguoi dung chua mo man hinh cho CALL_PATIENT_NUM_ORDER");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private async Task CallModuleCallPatientNumOrder(int num, int counters)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(callPatientFormName))
                {
                    V_HIS_ROOM room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
                    callPatientFormName = "CALL_PATIENT_CASHIER_" + room.ROOM_CODE;
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
                    MethodInfo theMethod = waitingForm.GetType().GetMethod("SetNumOrder");
                    if (theMethod != null)
                    {
                        object[] param = new object[] { num, counters };
                        theMethod.Invoke(waitingForm, param);
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Warn("Nguoi dung chua mo man hinh cho CALL_PATIENT_CASHIER_");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThreadRecallCallPatient()
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(ReacallCallPatientNewThread));
            //thread.Priority = ThreadPriority.Highest;
            try
            {
                thread.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                if (configKeyCallPatientCPA == "1")
                {
                    if (this.clienttManager == null)
                        this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager();

                    string gateCode = this.txtGateNumber.Text.Trim().Split(':')[0];

                    int[] nums = await this.clienttManager.AsyncRecallNumOrderPlus(int.Parse(gateCode), int.Parse(txtStepNumber.Text));
                    if (nums != null && nums.Length > 0)
                    {
                        await this.CallModuleCallPatientNumOrder(nums.LastOrDefault(), int.Parse(gateCode));
                    }
                }
                else
                {
                    CallPatientNotCPA(true, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void LoadCallPatientByThread(object param)
        {
            Inventec.Common.Logging.LogSystem.Debug("LoadCallPatientByThread. 1");
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
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadCallPatient.Abort();
            }
            Inventec.Common.Logging.LogSystem.Debug("LoadCallPatientByThread. 2");

        }
        internal void LoadDataToControlCallPatientThread(object param)
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
        internal void CallPatient(object param)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("CallPatient. 1");
                if (param is V_HIS_TREATMENT_FEE)
                {
                    var data = param as V_HIS_TREATMENT_FEE;
                    if (data != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("CallPatient. 2");
                        V_HIS_ROOM executeRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
                        string executeRoomName = executeRoom != null ? executeRoom.ROOM_NAME : null;
                        Inventec.Common.Logging.LogSystem.Debug(executeRoomName);
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
        internal void CallPatientByNumOder(V_HIS_TREATMENT_FEE data, String examRoomName)
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
            if (CallPatientDataWorker.DicDelegateCallingPatient != null && CallPatientDataWorker.DicDelegateCallingPatient.ContainsKey(this.currentModuleBase.RoomId))
            {
                DelegateSelectData nhapNhay = CallPatientDataWorker.DicDelegateCallingPatient[this.currentModuleBase.RoomId];
                if (nhapNhay != null) nhapNhay(data);
            }

            string moiBenhNhanStr = HisConfigs.Get<string>(SdaConfigKeys.CALL_PATIENT_MOI_BENH_NHAN);

            bool isCallPatientName = HisConfigs.Get<string>(SdaConfigKeys.CALL_PATIENT_NAME__DISABLE) != "1" ? true : false;


            SpeechPlayer.TypeSpeechCFG = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("Inventec.Speech.TypeSpeechCFG");

            string callPatientFormat = Config.HisConfigCFG.CallPatientFormat;
            List<string> KEY_SINGLE = new List<string>() { "PATIENT_NAME", "ROOM_NAME", "GATE_NUMBER", "GATE_NAME" };
            var strCallsplit = callPatientFormat.Split(new string[] { "<#", ";>" }, System.StringSplitOptions.RemoveEmptyEntries);
            if (strCallsplit.ToList().Count > 0)
            {
                string gateCode = !string.IsNullOrEmpty(this.txtGateNumber.Text) ? this.txtGateNumber.Text.Trim().Split(':')[0] : "";
                foreach (var word in strCallsplit)
                {
                    var checkKey = KEY_SINGLE.FirstOrDefault(o => o == word.ToUpper());
                    if (checkKey == null || checkKey.Count() == 0)
                    {
                        var strWordsplit = word.Split(new string[] { ",", ";", ".", "-", ":", "/" }, System.StringSplitOptions.RemoveEmptyEntries);
                        foreach (var item in strWordsplit)
                        {
                            if (configKeyCallPatientByCPA == "1")
                            {
                                clienttManager.CallTextData(gateCode, item.Trim());
                            }
                            else
                                Inventec.Speech.SpeechPlayer.SpeakSingle(item.Trim());
                        }
                    }
                    else
                    {
                        switch (word)
                        {
                            case "PATIENT_NAME":
                                if (configKeyCallPatientByCPA == "1")
                                {
                                    clienttManager.CallTextData(gateCode, patientName.Trim());
                                }
                                else
                                    Inventec.Speech.SpeechPlayer.Speak(patientName.Trim());
                                break;
                            case "ROOM_NAME":
                                if (!string.IsNullOrEmpty(examRoomName))
                                {
                                    if (configKeyCallPatientByCPA == "1")
                                    {
                                        clienttManager.CallTextData(gateCode, examRoomName.Trim());
                                    }
                                    else
                                        Inventec.Speech.SpeechPlayer.SpeakSingle(examRoomName.Trim());
                                }
                                break;
                            case "GATE_NUMBER":
                                if (configKeyCallPatientByCPA == "1")
                                {
                                    clienttManager.CallTextData(gateCode, txtGateNumber.Text);
                                }
                                else
                                    Inventec.Speech.SpeechPlayer.SpeakSingle(txtGateNumber.Text);
                                break;
                            case "GATE_NAME":
                                if (!string.IsNullOrEmpty(txtGateNumber.Text.Trim()))
                                {
                                    if (configKeyCallPatientByCPA == "1")
                                    {
                                        clienttManager.CallTextData(gateCode, "Quầy thu ngân số");
                                        clienttManager.CallTextData(gateCode, txtGateNumber.Text);
                                    }
                                    else
                                    {
                                        Inventec.Speech.SpeechPlayer.SpeakSingle("Quầy thu ngân số");
                                        Inventec.Speech.SpeechPlayer.SpeakSingle(txtGateNumber.Text);
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

        }

        private void CallModuleCallPatientNumOrder(V_HIS_TREATMENT_FEE hisTreatmentFee)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(callPatientFormName))
                {
                    V_HIS_ROOM room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
                    callPatientFormName = "CALL_PATIENT_CASHIER_TWO_" + (room != null ? room.ROOM_CODE : "");
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

                string num = null;
                if (!String.IsNullOrWhiteSpace(txtGateNumber.Text))
                    num = txtGateNumber.Text;


                if (waitingForm != null)
                {
                    MethodInfo theMethod = waitingForm.GetType().GetMethod("SetPatient");
                    if (theMethod != null)
                    {
                        if (num != null)
                        {
                            object[] param = new object[] { hisTreatmentFee.TDL_PATIENT_NAME, hisTreatmentFee.TDL_PATIENT_DOB, hisTreatmentFee.TREATMENT_CODE, int.Parse(num) };
                            theMethod.Invoke(waitingForm, param);
                        }
                        else
                        {
                            object[] param = new object[] { hisTreatmentFee.TDL_PATIENT_NAME, hisTreatmentFee.TDL_PATIENT_DOB, hisTreatmentFee.TREATMENT_CODE, null };
                            theMethod.Invoke(waitingForm, param);
                        }
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Không có SetPatient ");
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Warn("Nguoi dung chua mo man hinh cho CALL_PATIENT_CASHIER_TWO_");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void GetSttNowAndCallApi()
        {
            try
            {
                string gateNum = this.txtGateNumber.Text.Trim();
                gateCode = "";
                if (gateNum.Contains(":"))
                {
                    gateNum = gateNum.Split(':').First();
                    gateCode = this.txtGateNumber.Text.Trim().Split(':').Last();
                }
                else
                {
                    gateCode = gateNum;
                }
                var data = _RegisterGates.FirstOrDefault(p => p.REGISTER_GATE_CODE == gateCode);
                if (data != null)
                {
                    RegisterGateId = data.ID;
                    MOS.Filter.HisRegisterReqFilter filter = new MOS.Filter.HisRegisterReqFilter();
                    filter.REGISTER_GATE_ID = data.ID;
                    filter.REGISTER_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd") + "000000");
                    filter.REGISTER_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd") + "235959");
                    var datas = new BackendAdapter(null).Get<List<HIS_REGISTER_REQ>>("api/HisRegisterReq/Get", ApiConsumers.MosConsumer, filter, null);
                    if (datas != null && datas.Count > 0)
                    {
                        var rs = datas.OrderByDescending(p => p.REGISTER_TIME).ThenByDescending(o => o.NUM_ORDER).FirstOrDefault();
                        numTotal = rs.NUM_ORDER.ToString();
                        var numStts = datas.Where(o => o.CALL_TIME != null).OrderByDescending(p => p.CALL_TIME).ThenByDescending(o => o.NUM_ORDER).FirstOrDefault();
                        if (numStts != null)
                            numSttNow = numStts.NUM_ORDER.ToString();
                        else
                            numSttNow = "0";
                        this.Invoke((MethodInvoker)delegate
                        {
                            if (!dicRegisterReq.ContainsKey(gateNum))
                                dicRegisterReq[gateNum] = new List<HIS_REGISTER_REQ>();
                            dicRegisterReq[gateNum] = datas;
                        });
                    }
                    else
                    {
                        numTotal = "";
                        numSttNow = "0";
                    }
                }
                else
                {
                    numTotal = "";
                    numSttNow = "0";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
