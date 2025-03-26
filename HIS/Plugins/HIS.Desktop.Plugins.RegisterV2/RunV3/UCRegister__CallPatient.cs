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
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using System.Reflection;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Core;
using SDA.SDO;
using HIS.Desktop.LocalStorage.LocalData;

namespace HIS.Desktop.Plugins.RegisterV2.Run2
{
	public partial class UCRegister : UserControlBase
	{
		private string callPatientFormName = "";
		private int nFrom = 0;
		private int nTo = 0;
		private int bFrom = 0;
		private int bTo = 0;
		private List<long> RegisterReqIds = new List<long>();
		private Dictionary<string, List<HIS_REGISTER_REQ>> dicRegisterReq = new Dictionary<string, List<HIS_REGISTER_REQ>>();
		public const string CALL_PATIENT_MOI_BENH_NHAN = "EXE.CALL_PATIENT.MOI_BENH_NHAN";
		public const string CALL_PATIENT_CO_STT = "EXE.CALL_PATIENT.CO_STT";
		public const string CALL_PATIENT_DEN = "EXE.CALL_PATIENT.DEN";
		public const string CALL_PATIENT_CONG = "EXE.CALL_PATIENT.CONG";
		List<string> KEY_SINGLE = new List<string>() { "NUM_ORDER_STR", "NUM_ORDER", "GATE_NAME", "REGISTER_GATE_CODE", "REGISTER_GATE_NAME" };
		private long RegisterGateId;
        private string RegisterGateCode;
        string gateCode = "";
		string gateName = "";
		bool IsFirstCallNotCPA = true;
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
				this.CallPatient();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void CeateThreadGetPatient()
		{
			Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(CallPatientSDOThread));
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

		private void CallPatientSDOThread()
		{
			try
			{
				this.CallPatientSDO();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void CallPatientNotPCA(bool IsReCall,ref bool isCall)
		{
			try
			{
				long? Step =  null;
				if(!string.IsNullOrEmpty(txtStepNumber.Text))
					Step = Int64.Parse(txtStepNumber.Text);

                string GateNum = txtGateNumber.Text.Trim();
                if (txtGateNumber.Text.Contains(":"))
                    GateNum = txtGateNumber.Text.Trim().Split(':').First();
                else
                {
					if (GateNum != RegisterGateCode)
					{
                        if (!IsReCall && bFrom != 0 && bTo != 0 && ((Step == 1 && bFrom == 1 && bTo == 1) || (Step != 1 && bFrom == 1 && bTo == (1 + Step))))
                        {
                            IsFirstCallNotCPA = false;
                        }
                        isCall = !IsFirstCallNotCPA;
                        if (!IsReCall)
						{
                            nFrom = bFrom;
                            nTo = bTo;
                            if (Step == 1)
							{
								bFrom = bTo += (int)(Step ?? 0);
							}
							else
							{
                                bFrom = bTo + 1;
                                bTo = bFrom + (int)(Step ?? 0);                              
							}
							txtFrom.Text = bFrom.ToString();
							txtTo.Text = bTo.ToString();
                        }
                        numSttNow = txtTo.Text;
                        return;
					}
                }

                CommonParam param = new CommonParam();
				MOS.SDO.RegisterGateCallSDO sdo = new MOS.SDO.RegisterGateCallSDO();
				sdo.CallPlace = GateNum;
				sdo.CallStep = Step;
				sdo.RegisterGateId = RegisterGateId;
				List<HIS_REGISTER_REQ> apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post
				   <List<HIS_REGISTER_REQ>>
				   (IsReCall ? "api/HisRegisterGate/ReCall" : "api/HisRegisterGate/Call",
				   ApiConsumer.ApiConsumers.MosConsumer, param, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken);
				if (apiResult != null && apiResult.Count > 0)
				{
					bFrom = (int)apiResult.Min(o => o.NUM_ORDER);
					bTo = (int)apiResult.Max(o => o.NUM_ORDER);
					txtFrom.Text = bFrom.ToString();
					txtTo.Text = bTo.ToString();
                    nFrom = bFrom;
                    nTo = bTo;
                    numSttNow = txtTo.Text;
                }
				else
				{
					isCall = false;
					if (IsReCall)
					{
						DevExpress.XtraEditors.XtraMessageBox.Show(
							   "Không tìm thấy số thứ tự trước đó. Vui lòng thử lại sau",
							   ResourceMessage.TieuDeCuaSoThongBaoLaCanhBao);
					}
					else
					{
						DevExpress.XtraEditors.XtraMessageBox.Show(
							   "Hiện tại không có số thứ tự tiếp theo. Vui lòng thử lại sau",
							   ResourceMessage.TieuDeCuaSoThongBaoLaCanhBao);
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private async void CallPatientSDO()
		{
			try
			{ 
				RegisterReqIds = new List<long>();
				string GateNum = txtGateNumber.Text.Trim();
				if (txtGateNumber.Text.Contains(":"))
					GateNum = txtGateNumber.Text.Trim().Split(':').First();
				long Step = Int64.Parse(txtStepNumber.Text);
				//long[] id = this.clienttManager.GetCurrentPatientCall(GateNum, false);
				long Number = Int64.Parse(numSttNow) + (Step > 1 ? 0 : Step);
				List<long> lstNumber = new List<long>();
				//if (id != null && id.Length > 0)
				//{
				//	Number = id.Last();
				//	lstNumber = id.ToList();
				//}
				long SplitStep = 1;

				
				if (Step>1)
				{
					while (true)
					{
						var NumberTemp = Number + SplitStep;
						SplitStep++;
						lstNumber.Add(NumberTemp);
						if (NumberTemp >= Int64.Parse(numTotal) || (SplitStep - 1) == Step)
							break;
					}
					
					if (dicRegisterReq!=null && dicRegisterReq.ContainsKey(GateNum) && lstNumber!=null && lstNumber.Count > 0)
					{
						foreach (var item in dicRegisterReq[GateNum])
						{
							if(lstNumber.Where(o=>o == item.NUM_ORDER) != null && lstNumber.Where(o => o == item.NUM_ORDER).ToList().Count > 0)
							{
								RegisterReqIds.Add(item.ID);
							}	
						}
					}
				}
				else
				{
					if (dicRegisterReq != null && dicRegisterReq.ContainsKey(GateNum))
					{
						foreach (var item in dicRegisterReq[GateNum])
						{
							if (item.NUM_ORDER == Number)
							{
								RegisterReqIds.Add(item.ID);
							}
						}
					}
				}	


				if (RegisterReqIds == null || RegisterReqIds.Count <= 0)
					return;
				CommonParam param = new CommonParam();
				MOS.SDO.CallPatientSDO sdo = new MOS.SDO.CallPatientSDO();
				sdo.CallPlace = GateNum;
				sdo.RegisterReqIds = RegisterReqIds;
				
				bool apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post
				   <bool>
				   ("api/HisRegisterReq/CallPatient",
				   ApiConsumer.ApiConsumers.MosConsumer, param, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken);
				
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		public string GetIpLocal()
		{
			string ip = "";
			try
			{
				// get local IP addresses
				System.Net.IPAddress[] localIPs = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName());
				if (localIPs != null && localIPs.Length > 0)
				{
					foreach (var item in localIPs)
					{
						if (item.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
						{
							ip = item.ToString();
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
				//param.HasException = true; //Khong set param o day ma chi logging do viec log event la 1 viec phu khong qua quan trong
			}

			return ip;
		}

		private bool Create(SdaEventLogSDO data)
		{
			bool result = false;
			try
			{
				CommonParam param = new CommonParam();
				//Inventec.Core.ApiResultObject<bool> aro = ApiConsumerStore.SdaConsumer.Post<Inventec.Core.ApiResultObject<bool>>("/api/SdaEventLog/Create", param, data);
				var aro = new BackendAdapter(param).Post<bool>("/api/SdaEventLog/Create", ApiConsumer.ApiConsumers.SdaConsumer, data, param);
				Inventec.Common.Logging.LogSystem.Info("Du lieu dau ra SdaEventLog/Create:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => aro), aro));
				if (aro)
				{
					result = true;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
				//param.HasException = true; //Khong set param o day ma chi logging do viec log event la 1 viec phu khong qua quan trong
				result = false;
			}
			return result;
		}

		private async void CallPatient()
		{
			try
			{
				if (!this.btnCallPatient.Enabled)
					return;
				if (String.IsNullOrEmpty(this.txtGateNumber.Text))
					return;
				if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.DangKyTiepDonGoiBenhNhanBangCPA == "1")
				{
					if (String.IsNullOrEmpty(this.txtStepNumber.Text))
						return;
					string txtGate = txtGateNumber.Text.Trim();
					if (txtGateNumber.Text.Contains(":"))
						txtGate = txtGateNumber.Text.Trim().Split(':').First();
					if (this.clienttManager == null)
						this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager();

					var numCheck = Int64.Parse(numSttNow) + Int64.Parse(txtStepNumber.Text);
					if ((!string.IsNullOrEmpty(numTotal) && (numCheck <= Int64.Parse(numTotal))) || string.IsNullOrEmpty(numTotal))
					{
						if (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.CallCpaOption == 2)
						{
							int[] nums = await this.clienttManager.AsyncCallNumOrderPlusString(txtGate, int.Parse(this.txtStepNumber.Text));
							if (nums != null && nums.Length > 0)
							{
								await this.CallModuleCallPatientNumOrder(nums.LastOrDefault().ToString());
							}
						}
						else
						{
							this.clienttManager.CallNumOrderString(txtGate, int.Parse(this.txtStepNumber.Text));

						}

						CeateThreadGetPatient();
					}
					else if (!string.IsNullOrEmpty(numTotal))
					{
						if (Int64.Parse(numSttNow) < Int64.Parse(numTotal))
						{
							var numSend = Int64.Parse(numTotal) - Int64.Parse(numSttNow);
							this.clienttManager.CallNumOrderString(txtGate, (int)numSend);

							CeateThreadGetPatient();
						}
						else if (Int64.Parse(numSttNow) >= Int64.Parse(numTotal))
						{
							DevExpress.XtraEditors.XtraMessageBox.Show(
							   "Hiện tại đã hết số đăng ký. Vui lòng thử lại sau.",
							   ResourceMessage.TieuDeCuaSoThongBaoLaCanhBao);
							return;
						}
					}



				}
				else
				{
					bool isCall = true;
					CallPatientNotPCA(false,ref isCall);
					if (isCall)
					{
						CallPatientByFromTo();
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void UpNumber()
		{
			try
			{
				nFrom = Int32.Parse(txtTo.Text) + 1;
				txtFrom.Text = nFrom.ToString();
				nTo = Int32.Parse(txtTo.Text) + Int32.Parse(txtStepNumber.Text);
				txtTo.Text = nTo.ToString();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void CallPatientByFromTo()
		{
			try
            {
                Inventec.Speech.SpeechPlayer.TypeSpeechCFG = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("Inventec.Speech.TypeSpeechCFG");
				Inventec.Common.Logging.LogSystem.Debug(CallConfigString);
				var strCallsplit = CallConfigString.Split(new string[] { "<#", ";>" }, System.StringSplitOptions.RemoveEmptyEntries);
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
								case "NUM_ORDER_STR":
									if (nFrom == nTo)
									{
										var NumOrderStr = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(nFrom.ToString()).Trim();
										Inventec.Speech.SpeechPlayer.SpeakSingle(NumOrderStr);										
                                    }
                                    else
                                    {
										var NumOrderFromStr = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(nFrom.ToString()).Trim();
										Inventec.Speech.SpeechPlayer.SpeakSingle(NumOrderFromStr.ToString().Trim());
										
										Inventec.Speech.SpeechPlayer.SpeakSingle(HisConfigs.Get<string>(CALL_PATIENT_DEN));
										var NumOrderToStr = Inventec.Common.String.Convert.CurrencyToVneseStringNoUpcase(nTo.ToString());
										Inventec.Speech.SpeechPlayer.SpeakSingle(NumOrderToStr.ToString().Trim());									
									}
									break;
								case "NUM_ORDER":
									if (nFrom == nTo)
									{
										Inventec.Speech.SpeechPlayer.Speak(nFrom);
									}
									else
									{
										Inventec.Speech.SpeechPlayer.Speak(nFrom);
										Inventec.Speech.SpeechPlayer.SpeakSingle(HisConfigs.Get<string>(CALL_PATIENT_DEN));
										Inventec.Speech.SpeechPlayer.Speak(nTo);
									}
									break;
								case "GATE_NAME":
									Inventec.Speech.SpeechPlayer.SpeakSingle(this.txtGateNumber.Text.Trim().Split(':').First());
									break;
								case "REGISTER_GATE_CODE":
									Inventec.Speech.SpeechPlayer.SpeakSingle(gateCode);
									break;
								case "REGISTER_GATE_NAME":
									Inventec.Speech.SpeechPlayer.SpeakSingle(gateName);
									break;
								default:
									break;
							}
						}
					}
				}
				if (nFrom == nTo)
                {
					CallModuleCallPatientNumOrder(nFrom.ToString());
                }
                else
                {
					CallModuleCallPatientNumOrder(nFrom.ToString() + " - " + nTo.ToString());
				}
				#region Old
					//if (nFrom == nTo)
					//{
					//	Inventec.Speech.SpeechPlayer.SpeakSingle(HisConfigs.Get<string>(CALL_PATIENT_MOI_BENH_NHAN));
					//	Inventec.Speech.SpeechPlayer.SpeakSingle(HisConfigs.Get<string>(CALL_PATIENT_CO_STT));
					//	foreach (var item in nFrom.ToString())
					//	{
					//		Inventec.Speech.SpeechPlayer.SpeakSingle(item.ToString());
					//	}
					//	Inventec.Speech.SpeechPlayer.SpeakSingle("tới");
					//	Inventec.Speech.SpeechPlayer.SpeakSingle(HisConfigs.Get<string>(CALL_PATIENT_CONG));
					//	string gate = txtGateNumber.Text;
					//	if (txtGateNumber.Text.Contains(":"))
					//		gate = txtGateNumber.Text.Split(':')[0];
					//	foreach (var item in gate)
					//	{
					//		Inventec.Speech.SpeechPlayer.SpeakSingle(item.ToString());
					//	}
					//	CallModuleCallPatientNumOrder(nFrom.ToString());
					//}
					//else if (nFrom < nTo)
					//{
					//	Inventec.Speech.SpeechPlayer.SpeakSingle(HisConfigs.Get<string>(CALL_PATIENT_MOI_BENH_NHAN));
					//	Inventec.Speech.SpeechPlayer.SpeakSingle(HisConfigs.Get<string>(CALL_PATIENT_CO_STT));
					//	Inventec.Speech.SpeechPlayer.SpeakSingle("từ");
					//	foreach (var item in nFrom.ToString())
					//	{
					//		Inventec.Speech.SpeechPlayer.SpeakSingle(item.ToString());
					//	}
					//	Inventec.Speech.SpeechPlayer.SpeakSingle(HisConfigs.Get<string>(CALL_PATIENT_DEN));
					//	foreach (var item in nTo.ToString())
					//	{
					//		Inventec.Speech.SpeechPlayer.SpeakSingle(item.ToString());
					//	}
					//	Inventec.Speech.SpeechPlayer.SpeakSingle("tới");
					//	Inventec.Speech.SpeechPlayer.SpeakSingle(HisConfigs.Get<string>(CALL_PATIENT_CONG));
					//	string gate = txtGateNumber.Text;
					//	if (txtGateNumber.Text.Contains(":"))
					//		gate = txtGateNumber.Text.Split(':')[0];
					//	foreach (var item in gate)
					//	{
					//		Inventec.Speech.SpeechPlayer.SpeakSingle(item.ToString());
					//	}
					//	CallModuleCallPatientNumOrder(nFrom.ToString() + " - " + nTo.ToString());
					//}
					#endregion
			}
            catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private async Task CallModuleCallPatientNumOrder(string num)
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
					LogSystem.Warn("Nguoi dung chua mo man hinh cho CALL_PATIENT_NUM_ORDER");
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
				if (String.IsNullOrEmpty(txtGateNumber.Text))
					return;
				if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.DangKyTiepDonGoiBenhNhanBangCPA == "1")
				{
					if (String.IsNullOrEmpty(txtStepNumber.Text))
						return;
					string txtGate = txtGateNumber.Text.Trim();
					if (txtGateNumber.Text.Contains(":"))
						txtGate = txtGateNumber.Text.Trim().Split(':').First();
					if (this.clienttManager == null)
						this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager();
					if (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.CallCpaOption == 2)
					{

						int[] nums = await this.clienttManager.AsyncRecallNumOrderPlusString(txtGate, int.Parse(txtStepNumber.Text));
						if (nums != null && nums.Length > 0)
						{
							await this.CallModuleCallPatientNumOrder(nums.LastOrDefault().ToString());
						}
					}
					else
					{
						this.clienttManager.RecallNumOrderString(txtGate, int.Parse(txtStepNumber.Text));
					}

					CeateThreadGetPatient();
				}
				else
				{
					bool isCall = true;
					CallPatientNotPCA(true,ref isCall);
					if (isCall)
					{
						CallPatientByFromTo();
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		List<HIS_REGISTER_GATE> _RegisterGates { get; set; }

		//private void GATE()
		//{
		//    try
		//    {
		//        _RegisterGates = new List<HIS_REGISTER_GATE>();
		//        MOS.Filter.HisRegisterGateFilter filter = new MOS.Filter.HisRegisterGateFilter();
		//        _RegisterGates = new BackendAdapter(null).Get<List<HIS_REGISTER_GATE>>("api/HisRegisterGate/Get", ApiConsumers.MosConsumer, filter, null);

		//        int timeSyncAll = AppConfigs.ThoiGianTuDongGoiLaySTTMoiNhat;
		//        if (timeSyncAll > 0)
		//        {
		//            lciRegisterNumOrder.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
		//            System.Windows.Forms.Timer timerSyncAll = new System.Windows.Forms.Timer();
		//            timerSyncAll.Interval = timeSyncAll;
		//            timerSyncAll.Enabled = true;
		//            timerSyncAll.Tick += timerCall_Tick;
		//            timerSyncAll.Start();
		//        }
		//    }
		//    catch (Exception ex)
		//    {
		//        Inventec.Common.Logging.LogSystem.Error(ex);
		//    }
		//}
		int count = 0;
		private async Task GATE()
		{
			try
			{
				_RegisterGates = new List<HIS_REGISTER_GATE>();
				MOS.Filter.HisRegisterGateFilter filter = new MOS.Filter.HisRegisterGateFilter();
				_RegisterGates = new BackendAdapter(null).Get<List<HIS_REGISTER_GATE>>("api/HisRegisterGate/Get", ApiConsumers.MosConsumer, filter, null);

				int timeSyncAll = AppConfigs.ThoiGianTuDongGoiLaySTTMoiNhat;
				if (timeSyncAll > 0)
				{
					lciRegisterNumOrder.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
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
			CreateCallRegisterReq();
		}

		private void CreateCallRegisterReq()
		{
			Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(RegisterReqNewThread));
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

		private void RegisterReqNewThread()
		{
			try
			{
				this.RegisterReq();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void GetSttNowAndCallApi(bool IsCPA, ref string numSttTo)
		{
			try
			{
				string gateNum = this.txtGateNumber.Text.Trim();
				gateCode = "";
				gateName = "";
				if (gateNum.Contains(":"))
				{
					gateNum = gateNum.Split(':').First();
					gateCode = this.txtGateNumber.Text.Trim().Split(':').Last();
                }
                else
                {
					gateCode = gateNum;
				}				
				if (IsCPA)
				{
					long[] id = this.clienttManager.GetCurrentPatientCall(gateNum, false);
					if (id != null && id.Length > 0)
					{
						numSttNow = id.Last().ToString();
						numSttTo = id.First().ToString();
					}
				}
				var data = _RegisterGates.FirstOrDefault(p => p.REGISTER_GATE_CODE == gateCode);
				if (data != null)
				{
					gateName = data.REGISTER_GATE_NAME;
					RegisterGateId = data.ID;
					RegisterGateCode = data.REGISTER_GATE_CODE;
					MOS.Filter.HisRegisterReqFilter filter = new MOS.Filter.HisRegisterReqFilter();
					filter.REGISTER_GATE_ID = data.ID;
					filter.REGISTER_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd") + "000000");
					var datas = new BackendAdapter(null).Get<List<HIS_REGISTER_REQ>>("api/HisRegisterReq/Get", ApiConsumers.MosConsumer, filter, null);
					if (datas != null && datas.Count > 0)
					{
						var rs = datas.OrderByDescending(p => p.REGISTER_TIME).ThenByDescending(o=>o.NUM_ORDER).FirstOrDefault();
						numTotal = rs.NUM_ORDER.ToString();
						if (!IsCPA)
							numSttNow = datas.Where(o=>o.CALL_TIME != null).OrderByDescending(p => p.CALL_TIME).ThenByDescending(o => o.NUM_ORDER).FirstOrDefault().NUM_ORDER.ToString();
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
					}
				}
				else
				{
					numTotal = "";
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private async void RegisterReq()
		{
			try
			{
				if (String.IsNullOrEmpty(this.txtGateNumber.Text))
					return;
				if (this.clienttManager == null)
					this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager();
				bool isCPA = false;
				if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.DangKyTiepDonGoiBenhNhanBangCPA == "1")
				{
					isCPA = true;
				}
				string numSttTo = "0";
				GetSttNowAndCallApi(isCPA,ref numSttTo);
				txtNumberPer = !string.IsNullOrEmpty(numTotal) ? numSttNow + "/" + numTotal : numSttNow;
				this.lblRegisterNumOrder.Invoke(new MethodInvoker(delegate () { lblRegisterNumOrder.Text = txtNumberPer; }));
				if (isCPA)
				{
					this.txtFrom.Invoke(new MethodInvoker(delegate () { txtFrom.Text = numSttTo; }));
					this.txtTo.Invoke(new MethodInvoker(delegate () { txtTo.Text = numSttNow; }));
				}

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

	}
}
