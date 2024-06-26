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
using HIS.UC.TreeSereServ7V2;
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
using HIS.Desktop.Utility;
using HIS.Desktop.Plugins.ExecuteRoom.ADO;
using HIS.Desktop.Common;

namespace HIS.Desktop.Plugins.ExecuteRoom
{

    public partial class UCExecuteRoom : UserControlBase
    {
        string[] listReplace = new string[] { "(", ")" };

        Thread threadCallPatient;
        public void LoadCallPatientByThread(object param)
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
                LogSystem.Error(ex);
                threadCallPatient.Abort();
            }
            Inventec.Common.Logging.LogSystem.Debug("LoadCallPatientByThread. 2");
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
                Inventec.Common.Logging.LogSystem.Debug("CallPatient. 1");
                if (param is L_HIS_SERVICE_REQ)
                {
                    var data = param as L_HIS_SERVICE_REQ;
                    if (data != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("CallPatient. 2");
                        var executeRoom = lstExecuteRoom.FirstOrDefault(o => o.ROOM_ID == roomId);
                        string executeRoomName = executeRoom != null ? executeRoom.EXECUTE_ROOM_NAME : null;
                        CallPatientByNumOder(data, executeRoom);
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

        internal void CallPatientByNumOder(L_HIS_SERVICE_REQ data, V_HIS_EXECUTE_ROOM examRoom)
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

                string callPatientFormat = HisConfigs.Get<string>(SdaConfigKeys.CALL_PATIENT_FORMAT);

                //string moiBenhNhanStr = HisConfigs.Get<string>(SdaConfigKeys.CALL_PATIENT_MOI_BENH_NHAN);
                //string coSoSttStr = HisConfigs.Get<string>(SdaConfigKeys.CALL_PATIENT_CO_STT);
                //string denStr = HisConfigs.Get<string>(SdaConfigKeys.CALL_PATIENT_DEN);
                //bool isCallPatientName = HisConfigs.Get<string>(SdaConfigKeys.CALL_PATIENT_NAME__DISABLE) != "1" ? true : false;
                //string sinhNamStr = HisConfigs.Get<string>(SdaConfigKeys.CALL_PATIENT_DOB_YEAR);
                //bool isCallPatientDob = HisConfigs.Get<string>(SdaConfigKeys.IS_CALL_PATIENT_DOB_YEAR) == "1" ? true : false;

                Inventec.Speech.SpeechPlayer.TypeSpeechCFG = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("Inventec.Speech.TypeSpeechCFG");

                // 1: Gọi theo Tên bệnh nhân và số thứ tự
                //2: Gọi theo Tên bệnh nhân và năm sinh
                //3: Gọi theo Tên bệnh nhân và cả số thứ tự và năm sinh
                //MẶc định : 1
                Inventec.Common.Logging.LogSystem.Debug("CallPatientByNumOder. 2");
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientName), patientName)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => callPatientFormat), callPatientFormat)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Inventec.Speech.SpeechPlayer.TypeSpeechCFG), Inventec.Speech.SpeechPlayer.TypeSpeechCFG)
                    );
                List<string> KEY_SINGLE = new List<string>() { "ROOM_NAME", "ROOM_ADDRESS", "PATIENT_NAME", "YOB", "NUM_ORDER", "YOB_STR", "NUM_ORDER_STR", "ROOM_ADDRESS_SPLIT" };
                var strCallsplit = callPatientFormat.Split(new string[] { "<#", ";>" }, System.StringSplitOptions.RemoveEmptyEntries);
                if (strCallsplit.ToList().Count > 0)
                {
                    foreach (var word in strCallsplit)
                    {
                        var checkKey = KEY_SINGLE.FirstOrDefault(o => o == word.ToUpper());
                        if (checkKey == null || checkKey.Count() == 0)
                        {
                            var strWordsplit = word.Split(new string[] { ",", ";", ".", "-", ":", "/" }, System.StringSplitOptions.RemoveEmptyEntries);
                            foreach (var item in strWordsplit)
                            {
                                Inventec.Speech.SpeechPlayer.SpeakSingle(item.Trim());
                            }
                        }
                        else
                        {
                            switch (word)
                            {
                                case "ROOM_NAME":
                                    Inventec.Speech.SpeechPlayer.SpeakSingle(examRoom.EXECUTE_ROOM_NAME);
                                    break;
                                case "ROOM_ADDRESS":
                                    Inventec.Speech.SpeechPlayer.SpeakSingle(examRoom.ADDRESS);
                                    break;
                                case "PATIENT_NAME":
                                    Inventec.Speech.SpeechPlayer.Speak(patientName);
                                    break;
                                case "YOB":
                                    long year = Int64.Parse(data.TDL_PATIENT_DOB.ToString().Substring(0, 4));
                                    Inventec.Speech.SpeechPlayer.Speak(year);
                                    break;
                                case "NUM_ORDER":
                                    if (data.NUM_ORDER.HasValue)
                                        Inventec.Speech.SpeechPlayer.Speak(data.NUM_ORDER.Value);
                                    break;
                                case "YOB_STR":
                                    long yob = Int64.Parse(data.TDL_PATIENT_DOB.ToString().Substring(0, 4));
                                    Inventec.Speech.SpeechPlayer.Speak(Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(yob.ToString()));
                                    break;
                                case "NUM_ORDER_STR":
                                    if (data.NUM_ORDER.HasValue)
                                        Inventec.Speech.SpeechPlayer.Speak(Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(data.NUM_ORDER.ToString()));
                                    break;
                                case "ROOM_ADDRESS_SPLIT":
                                    Inventec.Speech.SpeechPlayer.Speak(examRoom.ADDRESS);
                                    break;
                                default:
                                    break;
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

        private void CreateThreadCallPatientCPA(List<ServiceReqADO> listServiceReq)
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(CallPatientCPANewThread));
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("CreateThreadCallPatientCPA_____   " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listServiceReq), listServiceReq));
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
                Inventec.Common.Logging.LogSystem.Debug("CallPatientCPANewThread ____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listServiceReq), listServiceReq));
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
                if (String.IsNullOrWhiteSpace(txtGateNumber.Text) || String.IsNullOrEmpty(txtStepNumber.Text))
                    return;
                string configKeyCallPatientCPA = ConfigApplicationWorker.Get<string>(AppConfigKey.CONFIG_KEY__DANG_KY_TIEP_DON__GOI_BENH_NHAN_BANG_PHAN_MEM_CPA);
                if (configKeyCallPatientCPA == "1")
                {
                    if (CheckListCPA != null && CheckListCPA.Count > 0)
                    {
                        ThreadCallPatientRefresh(CheckListCPA);
                    }
                    if (this.clienttManager == null)
                        this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager();
                    this.clienttManager.CallNumOrderString(txtGateNumber.Text.Trim(), int.Parse(txtStepNumber.Text));
                    long[] id = this.clienttManager.GetCurrentPatientCall(txtGateNumber.Text.Trim(), false);
                    Inventec.Common.Logging.LogSystem.Info("This.clienttManager.GetCurrentPatientCall ____Goi F6: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => id), id));
                    txtCallPatientCPA.ForeColor = Color.Black;
                    lcCkhCPA.AppearanceItemCaption.ForeColor = Color.Black;
                    foreach (var item in id)
                    {
                        var data = serviceReqs.FirstOrDefault(o => o.ID == item);
                        if (data != null)
                        {
                            if (data.CALL_COUNT.HasValue)
                            {
                                if (data.CALL_COUNT >= 1)
                                {
                                    data.status = 13;
                                }
                                else
                                {
                                    data.status = 11;
                                }
                            }
                            else
                            {
                                data.status = 11;
                            }
                        }
                        gridControlServiceReq.RefreshDataSource();
                    }
                    CreateThreadCallPatientCountService(id);
                    CheckListCPA = id.ToList();
                    //ThreadCallPatientRefresh(id.ToList());
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadCallPatientRefresh(List<long> id)
        {

            try
            {
                List<ServiceReqADO> apiResult = new List<ServiceReqADO>();
                Thread t2 = new Thread(delegate()
                {
                    Thread.Sleep(2000);
                    this.BeginInvoke(new MethodInvoker(delegate
                    {
                        MOS.Filter.HisServiceReqLViewFilter hisServiceReqFilter = new HisServiceReqLViewFilter();
                        hisServiceReqFilter.IDs = id.ToList();
                        CommonParam param = new CommonParam();
                        apiResult = new BackendAdapter(param).Get<List<ServiceReqADO>>("api/HisServiceReq/GetLView", ApiConsumers.MosConsumer, hisServiceReqFilter, param);
                        foreach (var item in apiResult)
                        {
                            var data_ = serviceReqs.Where(o => o.ID == item.ID);
                            // FillDataToGridControl();
                            //if (currentHisServiceReq != null)
                            //{
                            //    if (item.CALL_COUNT.HasValue)
                            //    {
                            //        if (item.CALL_COUNT >= 1)
                            //        {
                            //            item.status = 13;
                            //        }
                            //        else
                            //        {
                            //            item.status = 11;
                            //        }
                            //    }
                            //    else
                            //    {
                            //        item.status = 11;
                            //    }
                            //}
                            gridControlServiceReq.RefreshDataSource();
                            foreach (var item_ in data_)
                            {
                                if (item_.CALL_COUNT.HasValue)
                                {
                                    if (item_.CALL_COUNT >= 1)
                                    {
                                        item_.status = 12;
                                    }
                                    else
                                    {
                                        item_.status = 14;
                                    }
                                }
                                else
                                {
                                    item_.status = 14;
                                }
                            }
                        }
                        gridControlServiceReq.RefreshDataSource();
                    }));
                });
                t2.Start();
                gridControlServiceReq.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void CreateThreadCallPatientCountServiceGrid(long[] id)
        {
            Thread thread = new Thread(() => CallPatientCountServiceGrid(id));
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

        public void CallPatientCountServiceGrid(long[] id)
        {
            try
            {
                Thread thread;
                CommonParam paramK = new CommonParam();
                bool success = false;

                if (id != null && id.Count() > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("_______Đ" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => id), id));
                    foreach (var item in id)
                    {
                        var resend = new BackendAdapter(paramK).Post<HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_CALL, ApiConsumers.MosConsumer, item, paramK);
                        if (resend != null)
                        {
                            var req = serviceReqs.FirstOrDefault(o => o.ID == resend.ID);
                            if (req != null)
                            {
                                req.CALL_COUNT = resend.CALL_COUNT;
                            }

                            success = true;
                        }
                        //string name = "";
                        //if (serviceReqs != null && serviceReqs.Count() > 0)
                        //{
                        //    name = serviceReqs.FirstOrDefault(o => o.ID == item).TDL_PATIENT_NAME;
                        //    txtCallPatientCPA.Invoke(new MethodInvoker(delegate { txtCallPatientCPA.Text = name; }));
                        //}
                    }
                }
                WaitingManager.Hide();
                #region Show message
                if (success == false)
                {
                    MessageManager.Show(paramK, success);
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramK);
                }
                #endregion
                Refesh_();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void CreateThreadCallPatientCountService(long[] id)
        {
            Thread thread = new Thread(() => CallPatientCountService(id));
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

        public void CallPatientCountService(long[] id)
        {
            try
            {
                Thread thread;
                CommonParam paramK = new CommonParam();
                bool success = false;

                if (id != null && id.Count() > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("_______Đ" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => id), id));
                    foreach (var item in id)
                    {
                        var resend = new BackendAdapter(paramK).Post<HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_CALL, ApiConsumers.MosConsumer, item, paramK);
                        if (resend != null)
                        {
                            var req = serviceReqs.FirstOrDefault(o => o.ID == resend.ID);
                            if (req != null)
                            {
                                req.CALL_COUNT = resend.CALL_COUNT;
                            }

                            success = true;
                        }
                        string name = "";
                        if (serviceReqs != null && serviceReqs.Count() > 0)
                        {
                            name = serviceReqs.FirstOrDefault(o => o.ID == item).TDL_PATIENT_NAME;
                            txtCallPatientCPA.Invoke(new MethodInvoker(delegate { txtCallPatientCPA.Text = name; }));
                        }
                    }
                }
                WaitingManager.Hide();
                #region Show message
                if (success == false)
                {
                    MessageManager.Show(paramK, success);
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramK);
                }
                #endregion
                Refesh_();
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
                Inventec.Common.Logging.LogSystem.Debug("data_____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                string configKeyCallPatientCPA = ConfigApplicationWorker.Get<string>(AppConfigKey.CONFIG_KEY__DANG_KY_TIEP_DON__GOI_BENH_NHAN_BANG_PHAN_MEM_CPA);

                List<ServiceReqADO> listServiceReq = new List<ServiceReqADO>();
                listServiceReq = data as List<ServiceReqADO>;
                //Inventec.Common.Mapper.DataObjectMapper.Map<ServiceReqADO>(listServiceReq, data);
                Inventec.Common.Logging.LogSystem.Debug("data_ _1 ____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listServiceReq), listServiceReq));

                Inventec.Common.Logging.LogSystem.Debug("data_ ____configKeyCallPatientCPA" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => configKeyCallPatientCPA), configKeyCallPatientCPA));
                if (configKeyCallPatientCPA == "1" && listServiceReq != null && listServiceReq.Count > 0)
                {
                    if (this.clienttManager == null)
                        this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager();
                    List<string> listPatientName = new List<string>();
                    foreach (var item in listServiceReq)
                    {
                        string namSinh = item.TDL_PATIENT_DOB > 0 ? item.TDL_PATIENT_DOB.ToString().Substring(0, 4) : "";
                        string Note = "";
                        string ExecuteTime = "";
                        string PriorityNote = "";
                        if (item.IS_WAIT_CHILD == null && item.HAS_CHILD == 1)
                        {
                            Note = "Có KQ";
                            ExecuteTime = item.RESULTING_TIME.ToString() ?? "0";
                            // }
                        }
                        else
                        {
                            Note = "";
                            ExecuteTime = item.INTRUCTION_TIME.ToString();

                        }
                        if (item.PRIORITY_TYPE_ID.HasValue)
                        {
                            //TH ưu tiên: <Tên đối tượng ưu tiên>"
                            PriorityNote = lstPatientPrioty.FirstOrDefault(o => o.ID == item.PRIORITY_TYPE_ID).PRIORITY_TYPE_NAME;
                        }
                        string patient = String.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}", item.TDL_PATIENT_NAME, namSinh, item.NUM_ORDER, item.ID, Note, ExecuteTime, PriorityNote);

                        listPatientName.Add(patient);
                        string name = "";
                        if (serviceReqs != null && serviceReqs.Count() > 0)
                        {
                            name = serviceReqs.FirstOrDefault(o => o.ID == item.ID).TDL_PATIENT_NAME;
                            txtCallPatientCPA.Invoke(new MethodInvoker(delegate { txtCallPatientCPA.Text = name; }));
                        }
                    }
                    Inventec.Common.Logging.LogSystem.Debug("data_ ___ " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listPatientName), listPatientName));
                    this.clienttManager.CallPatientAndExecuteRoom(int.Parse(txtGateNumber.Text.Trim()), listPatientName);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateDicCallPatient(L_HIS_SERVICE_REQ serviceReq1)
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
                                Inventec.Common.Mapper.DataObjectMapper.Map<ServiceReq1ADO>(serviceReq1ADO, serviceReq1);
                                dic.Value.Add(serviceReq1ADO);
                            }


                            foreach (var item in dic.Value)
                            {
                                if (item.ID == serviceReq1.ID)
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

        private void CallPatientChkCPA(V_HIS_EXECUTE_ROOM examRoom, List<ServiceReqADO> data_) // ServiceReqADO data, 
        {


            try
            {
                if (data_ != null && data_.Count() > 0)
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
                            foreach (var item in data_)
                            {
                                if (!serviceReqIds.Contains(item.ID))
                                {
                                    ServiceReq1ADO serviceReq1ADO = new ServiceReq1ADO();
                                    Inventec.Common.Mapper.DataObjectMapper.Map<ServiceReq1ADO>(serviceReq1ADO, item);
                                    dic.Value.Add(serviceReq1ADO);
                                }
                            }
                            foreach (var item in dic.Value)
                            {
                                var CallPatientData = data_.Where(o => o.ID == item.ID);
                                if (CallPatientData != null && CallPatientData.Count() > 0)
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



                            // Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data_), data_) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => roomId), roomId) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dic.Value), dic.Value));
                            var dic_Value_o_IsCalling = dic.Value.Where(o => o.IsCalling == true && o.CallPatientSTT == true);
                            Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dic_Value_o_IsCalling), dic_Value_o_IsCalling));
                        }
                    }
                }
                //////////////////////////////////////////////////////////////

                foreach (var data in data_)
                {


                    if (data != null)
                    {
                        if (data.CALL_COUNT.HasValue)
                        {
                            if (data.CALL_COUNT >= 1)
                            {
                                data.status = 13;
                            }
                            else
                            {
                                data.status = 11;
                            }
                        }
                        else
                        {
                            data.status = 11;
                        }
                    }
                    gridControlServiceReq.RefreshDataSource();

                    string name = "";
                    if (serviceReqs != null && serviceReqs.Count() > 0)
                    {
                        name = serviceReqs.FirstOrDefault(o => o.ID == data.ID).TDL_PATIENT_NAME;
                        txtCallPatientCPA.Invoke(new MethodInvoker(delegate { txtCallPatientCPA.Text = name; }));
                        Thread.Sleep(2000);
                    }
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

                    string callPatientFormat = HisConfigs.Get<string>(SdaConfigKeys.CALL_PATIENT_FORMAT);

                    //string moiBenhNhanStr = HisConfigs.Get<string>(SdaConfigKeys.CALL_PATIENT_MOI_BENH_NHAN);
                    //string coSoSttStr = HisConfigs.Get<string>(SdaConfigKeys.CALL_PATIENT_CO_STT);
                    //string denStr = HisConfigs.Get<string>(SdaConfigKeys.CALL_PATIENT_DEN);
                    //bool isCallPatientName = HisConfigs.Get<string>(SdaConfigKeys.CALL_PATIENT_NAME__DISABLE) != "1" ? true : false;
                    //string sinhNamStr = HisConfigs.Get<string>(SdaConfigKeys.CALL_PATIENT_DOB_YEAR);
                    //bool isCallPatientDob = HisConfigs.Get<string>(SdaConfigKeys.IS_CALL_PATIENT_DOB_YEAR) == "1" ? true : false;

                    Inventec.Speech.SpeechPlayer.TypeSpeechCFG = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("Inventec.Speech.TypeSpeechCFG");

                    // 1: Gọi theo Tên bệnh nhân và số thứ tự
                    //2: Gọi theo Tên bệnh nhân và năm sinh
                    //3: Gọi theo Tên bệnh nhân và cả số thứ tự và năm sinh
                    //MẶc định : 1
                    Inventec.Common.Logging.LogSystem.Debug("CallPatientByNumOder. 2");
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientName), patientName)
                        + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => callPatientFormat), callPatientFormat)
                        + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Inventec.Speech.SpeechPlayer.TypeSpeechCFG), Inventec.Speech.SpeechPlayer.TypeSpeechCFG)
                        );
                    List<string> KEY_SINGLE = new List<string>() { "ROOM_NAME", "ROOM_ADDRESS", "PATIENT_NAME", "YOB", "NUM_ORDER" };
                    var strCallsplit = callPatientFormat.Split(new string[] { "<#", ";>" }, System.StringSplitOptions.RemoveEmptyEntries);
                    if (strCallsplit.ToList().Count > 0)
                    {
                        foreach (var word in strCallsplit)
                        {
                            var checkKey = KEY_SINGLE.FirstOrDefault(o => o == word.ToUpper());
                            if (checkKey == null || checkKey.Count() == 0)
                            {
                                var strWordsplit = word.Split(new string[] { ",", ";", ".", "-", ":", "/" }, System.StringSplitOptions.RemoveEmptyEntries);
                                foreach (var item in strWordsplit)
                                {
                                    Inventec.Speech.SpeechPlayer.SpeakSingle(item.Trim());
                                }
                            }
                            else
                            {
                                switch (word)
                                {
                                    case "ROOM_NAME":
                                        Inventec.Speech.SpeechPlayer.SpeakSingle(examRoom.EXECUTE_ROOM_NAME);
                                        break;
                                    case "ROOM_ADDRESS":
                                        Inventec.Speech.SpeechPlayer.SpeakSingle(examRoom.ADDRESS);
                                        break;
                                    case "PATIENT_NAME":
                                        Inventec.Speech.SpeechPlayer.Speak(patientName);
                                        break;
                                    case "YOB":
                                        string year = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                                        Inventec.Speech.SpeechPlayer.Speak(year);
                                        break;
                                    case "NUM_ORDER":
                                        if (data.NUM_ORDER.HasValue)
                                            Inventec.Speech.SpeechPlayer.Speak(data.NUM_ORDER.Value);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Gọi bệnh nhân từ danh sách y lệnh (gridViewServiceReq_RowCellClick)
        private void CallPatient_gridViewServiceReq_ButtonClick()
        {
            try
            {
                LogTheadInSessionInfo(CallPatient_gridViewServiceReq_ButtonClick_Action, "CallPatient_gridViewServiceReq_ButtonClick");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CallPatient_gridViewServiceReq_ButtonClick_Action()
        {
            try
            {
                this.currentHisServiceReq = (ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                if (this.currentHisServiceReq != null)
                {

                    Inventec.Common.Logging.LogSystem.Debug("gridViewServiceReq_RowCellClick. 1");
                    List<ServiceReqADO> serviceReqTemps = new List<ServiceReqADO>();
                    int[] selectRows = gridViewServiceReq.GetSelectedRows();
                    if (selectRows != null && selectRows.Count() > 0)
                    {
                        for (int i = 0; i < selectRows.Count(); i++)
                        {
                            serviceReqTemps.Add((ServiceReqADO)gridViewServiceReq.GetRow(selectRows[i]));
                        }
                    }

                    if (serviceReqTemps != null && serviceReqTemps.Count > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("gridViewServiceReq_RowCellClick. 2");
                        if (!serviceReqTemps.Contains(this.currentHisServiceReq))
                            return;
                        CreateThreadCallPatientCPA(serviceReqTemps);
                        long[] id_ = new long[serviceReqTemps.Count];
                        for (int runs = 0; runs < serviceReqTemps.Count; runs++)
                        {
                            id_[runs] = serviceReqTemps[runs].ID;
                        }
                        if (CheckListCPA != null && CheckListCPA.Count > 0)
                        {
                            Thread t2 = new Thread(delegate()
                            {
                                Thread.Sleep(10000);
                                ThreadCallPatientRefresh(CheckListCPA);
                            });

                        }
                        CreateThreadCallPatientCountServiceGrid(id_);
                        CheckListCPA = id_.ToList();
                        foreach (var item_ in id_)
                        {
                            var data_ = serviceReqs.Where(o => o.ID != item_);
                            // FillDataToGridControl();
                            var choosedata = serviceReqs.FirstOrDefault(o => o.ID == item_);
                            if (choosedata != null)
                            {
                                if (choosedata.CALL_COUNT.HasValue)
                                {
                                    if (choosedata.CALL_COUNT >= 1)
                                    {
                                        choosedata.status = 13;
                                    }
                                    else
                                    {
                                        choosedata.status = 11;
                                    }
                                }
                                else
                                {
                                    choosedata.status = 11;
                                }
                            }
                            gridControlServiceReq.RefreshDataSource();
                            foreach (var item in data_)
                            {
                                if (item.CALL_COUNT.HasValue)
                                {
                                    if (item.CALL_COUNT >= 1)
                                    {
                                        item.status = 12;
                                    }
                                    else
                                    {
                                        item.status = 14;
                                    }
                                }
                                else
                                {
                                    item.status = 14;
                                }
                            }

                            gridControlServiceReq.RefreshDataSource();
                        }


                        gridViewServiceReq.FocusedColumn = gridViewServiceReq.Columns[1];
                        Inventec.Common.Logging.LogSystem.Debug("gridViewServiceReq_RowCellClick. 3");
                    }
                    else
                    {

                        string name = "";
                        if (serviceReqs != null && serviceReqs.Count() > 0)
                        {
                            name = serviceReqs.FirstOrDefault(o => o.ID == this.currentHisServiceReq.ID).TDL_PATIENT_NAME;
                            txtCallPatientCPA.Invoke(new MethodInvoker(delegate { txtCallPatientCPA.Text = name; }));
                        }
                        Inventec.Common.Logging.LogSystem.Debug("gridViewServiceReq_RowCellClick. 4");
                        UpdateDicCallPatient(this.currentHisServiceReq);
                        LoadCallPatientByThread(this.currentHisServiceReq);
                        Inventec.Common.Logging.LogSystem.Debug("gridViewServiceReq_RowCellClick. 5");

                        long[] id_ = new long[1];
                        id_[0] = currentHisServiceReq.ID;
                        CreateThreadCallPatientCountService(id_);
                        List<ServiceReqADO> data = new List<ServiceReqADO>();

                        var data_ = serviceReqs.Where(o => o.ID != currentHisServiceReq.ID);
                        // FillDataToGridControl();
                        if (currentHisServiceReq != null)
                        {
                            if (currentHisServiceReq.CALL_COUNT.HasValue)
                            {
                                if (currentHisServiceReq.CALL_COUNT >= 1)
                                {
                                    currentHisServiceReq.status = 13;
                                }
                                else
                                {
                                    currentHisServiceReq.status = 11;
                                }
                            }
                            else
                            {
                                currentHisServiceReq.status = 11;
                            }
                        }
                        gridControlServiceReq.RefreshDataSource();
                        foreach (var item in data_)
                        {
                            if (item.CALL_COUNT.HasValue)
                            {
                                if (item.CALL_COUNT >= 1)
                                {
                                    item.status = 12;
                                }
                                else
                                {
                                    item.status = 14;
                                }
                            }
                            else
                            {
                                item.status = 14;
                            }
                        }

                        gridControlServiceReq.RefreshDataSource();
                    }

                }

                gridViewServiceReq.FocusedColumn = gridViewServiceReq.Columns[1];
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //Nhấn nút gọi nhỡ từ danh sách y lệnh (gridViewServiceReq_RowCellClick)
        private void PatientMissed_gridViewServiceReq_ButtonClick()
        {
            try
            {
                LogTheadInSessionInfo(PatientMissed_gridViewServiceReq_ButtonClick_Action, "PatientMissed_gridViewServiceReq_ButtonClick");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PatientMissed_gridViewServiceReq_ButtonClick_Action()
        {
            try
            {
                WaitingManager.Show();

                var data_ = (HIS.Desktop.Plugins.ExecuteRoom.ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                CPA.WCFClient.CallPatientClient.ADO.CallPatientInfoADO CallPatientInfoADO_ = new CPA.WCFClient.CallPatientClient.ADO.CallPatientInfoADO();
                CallPatientInfoADO_.Dob = data_.TDL_PATIENT_DOB;
                CallPatientInfoADO_.NumOrder = data_.NUM_ORDER ?? 0;
                CallPatientInfoADO_.ServiceReqId = data_.ID;
                CallPatientInfoADO_.PatientName = data_.TDL_PATIENT_NAME;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Gọi bệnh nhân______" + Inventec.Common.Logging.LogUtil.GetMemberName(() => CallPatientInfoADO_), CallPatientInfoADO_));
                this.clienttManager.UpdatePatientToMissingCall(txtGateNumber.Text.Trim(), CallPatientInfoADO_);
                string name = "";
                if (serviceReqs != null && serviceReqs.Count() > 0)
                {
                    name = serviceReqs.FirstOrDefault(o => o.ID == data_.ID).TDL_PATIENT_NAME;
                    txtCallPatientCPA.Invoke(new MethodInvoker(delegate { txtCallPatientCPA.Text = name; }));
                }
                CommonParam paramK = new CommonParam();
                bool success = false;


                Inventec.Common.Logging.LogSystem.Debug("_______Đ" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data_.ID), data_.ID));
                var resend = new BackendAdapter(paramK).Post<HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_CALL, ApiConsumers.MosConsumer, data_.ID, paramK);
                if (resend != null)
                {
                    success = true;
                }

                WaitingManager.Hide();
                //#region Show message
                //MessageManager.Show(paramK, success);
                //#endregion

                //#region Process has exception
                //HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramK);
                //#endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
