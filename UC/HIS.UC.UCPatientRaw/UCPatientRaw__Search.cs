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
using HIS.Desktop.DelegateRegister;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Library.CheckHeinGOV;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using HIS.Desktop.Utility;
using HIS.UC.UCPatientRaw.ADO;
using HIS.UC.UCPatientRaw.Base;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Common.QrCodeBHYT;
using Inventec.Common.QrCodeCCCD;
using Inventec.Core;
using Inventec.Desktop.Common.LibraryMessage;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.UCPatientRaw
{
	public partial class UCPatientRaw : HIS.Desktop.Utility.UserControlBase
    {
		public HisPatientSDO patientTD3;
		DataResultADO dataResult = new DataResultADO();
		string hrmEmployeeCode = "";
        public string oldValue = "";
		public string oldTypeFind = ResourceMessage.typeCodeFind__MaBN;
        public async void SearchPatientByCodeOrQrCode(string strValue, string keyTypeFind = null)
        {
            oldTypeFind = this.typeCodeFind;
            try
			{
				this.isAlertTreatmentEndInDay = false;
				this.ResultDataADO = null;
				this.isReadQrCode = false;
				this.hrmEmployeeCode = "";
				this.dataResult = new DataResultADO();
				oldValue = strValue;
				if (!string.IsNullOrEmpty(keyTypeFind))
					typeCodeFind = keyTypeFind;
                if (!String.IsNullOrEmpty(strValue))
				{
					LogSystem.Debug("txtPatientCode_KeyDown");
					CommonParam param = new CommonParam();
					WaitingManager.Show();
                    if (strValue.Contains("|")) {
						var dataFirst = strValue.Split('|')[0];
						if (dataFirst.Length == 10 || dataFirst.Length == 15)
                        {
                            this.typeCodeFind = ResourceMessage.typeCodeFind__MaBN;

                        }else if(dataFirst.Length == 12)
                        {
                            this.typeCodeFind = ResourceMessage.typeCodeFind__MaCMCC;
                        }	
					}
					#region --- Trường hợp tìm kiếm BN theo mã BN hoặc QRCode
					if (this.typeCodeFind == ResourceMessage.typeCodeFind__MaBN)
					{
						WaitingManager.Hide();
						Inventec.Common.Logging.LogSystem.Debug("Ma BN________________________");
                        this.typeReceptionForm = ReceptionForm.MaBN;
						var data = await (ProcessSearchByCode(strValue, 1));
						if (data != null)
						{
							if (data is HisPatientSDO)
							{
								dataResult.HisPatientSDO = (HisPatientSDO)data;
								dataResult.OldPatient = true;
								this.currentPatientSDO = (HisPatientSDO)data;
								this.dlgSendPatientSdo(currentPatientSDO);
								this.patientTD3 = (HisPatientSDO)data;
								hrmEmployeeCode = currentPatientSDO.HRM_EMPLOYEE_CODE;
							}
							else if (data is HeinCardData)
							{
								this.patientTD3 = null;
								dataResult.HeinCardData = (HeinCardData)data;
								dataResult.OldPatient = false;
							}
							dataResult.SearchTypePatient = 1;
						}
						else
						{
							dataResult = null;
							this.patientTD3 = null;
						}
					}
					#endregion

					#region ---- TheThongMinh
					else if (this.typeCodeFind == ResourceMessage.typeCodeFind__SoThe)
					{
                        this.typeReceptionForm = ReceptionForm.TheKcbThongminh;
						var patientInRegisterSearchByCard = new BackendAdapter(param).Get<HisCardSDO>(RequestUriStore.HIS_CARD_GETVIEWBYSERVICECODE, ApiConsumers.MosConsumer, strValue, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
						WaitingManager.Hide();

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientInRegisterSearchByCard), patientInRegisterSearchByCard));
						if (patientInRegisterSearchByCard != null)
						{
                            await SetDataFromCardSDO(patientInRegisterSearchByCard);
						}
						else
						{
							WaitingManager.Hide();
							DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.SoTheKhongTontai + " '" + strValue + "'", MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
							this.txtPatientCode.Focus();
							this.txtPatientCode.SelectAll();
							this.patientTD3 = null;
							return;
						}
					}
					#endregion

					#region ----MaCT
					else if (this.typeCodeFind == ResourceMessage.typeCodeFind__MaCT)
					{
                        this.typeReceptionForm = ReceptionForm.MaChuongTrinh;
						var _PatientProgram = new BackendAdapter(param).Get<V_HIS_PATIENT_PROGRAM>(RequestUriStore.HIS_PATIEN_PROGRAM_GET, ApiConsumers.MosConsumer, strValue.Trim(), HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
						WaitingManager.Hide();
						if (_PatientProgram != null)
						{
							this.txtPatientCode.Text = strValue;
							dataResult.V_HIS_PATIENT_PROGRAM = _PatientProgram;
							var data = await this.ProcessSearchByCode(_PatientProgram.PATIENT_CODE, 3);
							if (data is HisCardSDO)
							{
								this.patientTD3 = null;
								dataResult.HisCardSDO = (HisCardSDO)data;
								dataResult.OldPatient = false;
							}
							else if (data is HisPatientSDO)
							{
								dataResult.HisPatientSDO = (HisPatientSDO)data;
								dataResult.OldPatient = true;
								this.currentPatientSDO = (HisPatientSDO)data;
								this.dlgSendPatientSdo(currentPatientSDO);
								this.patientTD3 = (HisPatientSDO)data;
								hrmEmployeeCode = currentPatientSDO.HRM_EMPLOYEE_CODE;
							}
							else
							{
								this.patientTD3 = null;
							}
							dataResult.SearchTypePatient = 3;
						}
						else
						{
							this.patientTD3 = null;
							DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.MaChuongTrinhKhongTontai + " '" + strValue + "'", MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
							this.txtPatientCode.Focus();
							this.txtPatientCode.SelectAll();
							return;
						}
					}
					#endregion

					#region ----MaHK
					else if (this.typeCodeFind == ResourceMessage.typeCodeFind__MaHK)
					{
                        this.typeReceptionForm = ReceptionForm.MaHenKham;
						string heinAddressOfPatient = "";
						int n;
						bool isNumeric = int.TryParse(this.txtPatientCode.Text, out n);
						if (!isNumeric)
						{
							this.patientTD3 = null;
							DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.MaHenKhamKhongTontai + " '" + this.txtPatientCode.Text + "'", MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
							this.txtPatientCode.Focus();
							this.txtPatientCode.SelectAll();
							return;
						}
						var codeFind = string.Format("{0:000000000000}", Convert.ToInt64(strValue));
						this.txtPatientCode.Text = codeFind;
						param = new CommonParam();
						var data = SearchByCode(codeFind, typeCodeFind);
						WaitingManager.Hide();
						if (data != null && data is HisPatientSDO)
						{
							HisPatientSDO _PatientSDO = data as HisPatientSDO;
							this.patientTD3 = _PatientSDO;
							dataResult.HisPatientSDO = _PatientSDO;
							dataResult.OldPatient = true;
							this.currentPatientSDO = _PatientSDO;
							this.dlgSendPatientSdo(currentPatientSDO);
							this.ProcessPatientCodeKeydown(_PatientSDO);
							dataResult.AppointmentCode = _PatientSDO.AppointmentCode;
							dataResult.TreatmnetIdByAppointmentCode = _PatientSDO.TreatmentId ?? 0;
							dataResult.SearchTypePatient = 2;
							dataResult.TreatmentTypeId = _PatientSDO.TreatmentTypeId;
							heinAddressOfPatient = _PatientSDO.HeinAddress;
							hrmEmployeeCode = _PatientSDO.HRM_EMPLOYEE_CODE;
							if (this.dlgFillDataPreviewForSearchByQrcodeInUCPatientRaw != null)
							{
								HeinCardData heinCardDataForCheckGOV = new HeinCardData();
								heinCardDataForCheckGOV = ConvertFromPatientData(dataResult.HisPatientSDO);
								this.dlgFillDataPreviewForSearchByQrcodeInUCPatientRaw(heinCardDataForCheckGOV);
								if (this.actInitExamServiceRoomByAppoimentTime != null)
									this.actInitExamServiceRoomByAppoimentTime(dataResult.HisPatientSDO);

								long patientTypeId = this.cboPatientType.EditValue == null ? 0 : Inventec.Common.TypeConvert.Parse.ToInt64(this.cboPatientType.EditValue.ToString());
								if (!this.TD3 && patientTypeId == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT)
								{
									HeinGOVManager heinGOVManager = new HeinGOVManager(ResourceMessage.GoiSangCongBHXHTraVeMaLoi);
									if ((HisConfigCFG.IsBlockingInvalidBhyt == ((int)HisConfigCFG.OptionKey.Option1).ToString() || HisConfigCFG.IsBlockingInvalidBhyt == ((int)HisConfigCFG.OptionKey.Option2).ToString()))
										heinGOVManager.SetDelegateHeinEnableButtonSave(dlgHeinEnableSave);
									this.ResultDataADO = await heinGOVManager.Check(heinCardDataForCheckGOV, null, false, heinAddressOfPatient, this.dlgGetIntructionTime(), isReadQrCode);
								}
								if (this.ResultDataADO != null && this.ResultDataADO.ResultHistoryLDO != null)
								{
									heinCardDataForCheckGOV.HeinCardNumber = this.ResultDataADO.IsUsedNewCard ? this.ResultDataADO.ResultHistoryLDO.maTheMoi : this.ResultDataADO.ResultHistoryLDO.maThe;
                                    //Trường hợp tìm kiếm BN theo qrocde & BN có số thẻ bhyt mới, cần tìm kiếm BN theo số thẻ mới này & người dùng chọn lấy thông tin thẻ mới => tìm kiếm Bn theo số thẻ mới
                                    if (!String.IsNullOrEmpty(heinCardDataForCheckGOV.HeinCardNumber))
									{
										if (this.ResultDataADO.IsShowQuestionWhileChangeHeinTime__Choose)
										{
                                            heinCardDataForCheckGOV = this.ResultDataADO.HeinCardData;
                                        }
                                        dataResult.HeinCardData = heinCardDataForCheckGOV;
                                    }
								}

								if (!String.IsNullOrEmpty(heinCardDataForCheckGOV.HeinCardNumber))
								{
									this.CheckPatientOldByHeinCard(heinCardDataForCheckGOV, false);//xuandv sua ve false// k check lan 2
								}
							}
						}
						else
						{
							this.patientTD3 = null;
							DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.MaHenKhamKhongTontai + " '" + codeFind + "'", MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
							this.txtPatientCode.Focus();
							this.txtPatientCode.SelectAll();
							return;
						}
					}
					#endregion

					#region --- MaNV
					else if (this.typeCodeFind == ResourceMessage.typeCodeFind__MaNV)
					{
						if (string.IsNullOrEmpty(strValue))
							return;
                        this.typeReceptionForm = ReceptionForm.MaNV;
						param = new CommonParam();
						HisPatientFilter filter = new HisPatientFilter();
						filter.HRM_EMPLOYEE_CODE__EXACT = strValue.Trim();
						var data = (new BackendAdapter(param).Get<List<HIS_PATIENT>>("/api/HisPatient/Get", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param));

						if (data != null && data.Count > 0)
						{
							HisPatientSDO _PatientSDOByHrm = new HisPatientSDO();
							Inventec.Common.Mapper.DataObjectMapper.Map<HisPatientSDO>(_PatientSDOByHrm, data.FirstOrDefault());
							dataResult.HisPatientSDO = _PatientSDOByHrm;
							this.ProcessPatientCodeKeydown(_PatientSDOByHrm);

							if (this.dlgFillDataPreviewForSearchByQrcodeInUCPatientRaw != null)
							{
								HeinCardData heinCardDataForCheckGOV = new HeinCardData();
								heinCardDataForCheckGOV = ConvertFromPatientData(_PatientSDOByHrm);
								this.dlgFillDataPreviewForSearchByQrcodeInUCPatientRaw(heinCardDataForCheckGOV);
							}
						}
						else
						{
							ProcessGetDataHrm(strValue.Trim());//GetDataHrm return _PatientSDOByHrm
						}
					}
					#endregion

					#region ---- MaDT
					else if (this.typeCodeFind == ResourceMessage.typeCodeFind__MaDT)
					{
						int n;
						bool isNumeric = int.TryParse(this.txtPatientCode.Text, out n);
						if (!isNumeric)
						{
							this.patientTD3 = null;
							DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.MaDieuTriKhongTontai + " '" + this.txtPatientCode.Text + "'", MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
							this.txtPatientCode.Focus();
							this.txtPatientCode.SelectAll();
							return;
						}
						var codeFind = string.Format("{0:000000000000}", Convert.ToInt64(strValue));
						this.txtPatientCode.Text = codeFind;
						var data = SearchByCode(codeFind, typeCodeFind);
						WaitingManager.Hide();
						if (data != null && data is HisPatientSDO)
						{
                            this.typeReceptionForm = ReceptionForm.MaDieuTri;
							HisPatientSDO _PatientSDO = data as HisPatientSDO;

							this.patientTD3 = _PatientSDO;
							dataResult.HisPatientSDO = _PatientSDO;
							dataResult.OldPatient = true;
							this.currentPatientSDO = _PatientSDO;
							this.dlgSendPatientSdo(currentPatientSDO);
							this.ProcessPatientCodeKeydown(_PatientSDO);
							dataResult.SearchTypePatient = 6;

							HeinCardData heinCardDataForCheckGOV = new HeinCardData();
							heinCardDataForCheckGOV = ConvertFromPatientData(_PatientSDO);
							hrmEmployeeCode = _PatientSDO.HRM_EMPLOYEE_CODE;

							long patientTypeId = this.cboPatientType.EditValue == null ? 0 : Inventec.Common.TypeConvert.Parse.ToInt64(this.cboPatientType.EditValue.ToString());
							if (!this.TD3 && patientTypeId == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT)
							{
								HIS.Desktop.Plugins.Library.CheckHeinGOV.HeinGOVManager heinGOVManager = new HIS.Desktop.Plugins.Library.CheckHeinGOV.HeinGOVManager(ResourceMessage.GoiSangCongBHXHTraVeMaLoi);
                                DateTime dtIntructionTime = DateTime.Now;
                                dtIntructionTime = this.dlgGetIntructionTime();
								if ((HisConfigCFG.IsBlockingInvalidBhyt == ((int)HisConfigCFG.OptionKey.Option1).ToString() || HisConfigCFG.IsBlockingInvalidBhyt == ((int)HisConfigCFG.OptionKey.Option2).ToString()))
									heinGOVManager.SetDelegateHeinEnableButtonSave(dlgHeinEnableSave);
								this.ResultDataADO = await heinGOVManager.Check(heinCardDataForCheckGOV, null, false, _PatientSDO.ADDRESS, dtIntructionTime, isReadQrCode);
							}

							if (this.ResultDataADO != null && this.ResultDataADO.ResultHistoryLDO != null)
							{
                                heinCardDataForCheckGOV.HeinCardNumber = this.ResultDataADO.IsUsedNewCard ? this.ResultDataADO.ResultHistoryLDO.maTheMoi : this.ResultDataADO.ResultHistoryLDO.maThe;
                                //Trường hợp tìm kiếm BN theo qrocde & BN có số thẻ bhyt mới, cần tìm kiếm BN theo số thẻ mới này & người dùng chọn lấy thông tin thẻ mới => tìm kiếm Bn theo số thẻ mới
                                if (!String.IsNullOrEmpty(heinCardDataForCheckGOV.HeinCardNumber))
								{
									if (this.ResultDataADO.IsShowQuestionWhileChangeHeinTime__Choose)
                                    {
                                        heinCardDataForCheckGOV = this.ResultDataADO.HeinCardData;
                                    }
                                    dataResult.HeinCardData = heinCardDataForCheckGOV;
                                }
							}
						}
						else
						{
							this.patientTD3 = null;
							DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.MaDieuTriKhongTontai + " '" + codeFind + "'", MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
							this.txtPatientCode.Focus();
							this.txtPatientCode.SelectAll();
							return;
						}
					}
					#endregion

					#region ---- CMND/CCCD
					else if (this.typeCodeFind == ResourceMessage.typeCodeFind__MaCMCC)
					{
						if (!((strValue.Trim().Length > 12 && strValue.Trim().Contains("|")) || (strValue.Trim().Length == 12 && !string.IsNullOrEmpty(txtPatientName.Text) && (!string.IsNullOrEmpty(txtPatientDob.Text) || dtPatientDob.EditValue != null))) || ((strValue.Trim().Length == 12 || strValue.Trim().Length == 9) && !strValue.Trim().Contains("|")))
						{
								param = new CommonParam();
								HisPatientAdvanceFilter filter = new HisPatientAdvanceFilter();
								if (strValue.Trim().Length == 9)
								{
									filter.CMND_NUMBER__EXACT = strValue;
								}
								else
								{
									filter.CCCD_NUMBER__EXACT = strValue;
								}

								var data = (new BackendAdapter(param).Get<List<HisPatientSDO>>(RequestUriStore.HIS_PATIENT_GETSDOADVANCE, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param));
								WaitingManager.Hide();
								if (data != null && data.Count > 0)
								{
									this.typeReceptionForm = ReceptionForm.SoCCCD;
									if (data.Count > 1)
									{
										LogSystem.Debug(data.Count + " benh nhan cu => mo form chon benh nhan => chon 1 => fill du lieu bn duoc chon.");
										frmPatientChoice frm = new frmPatientChoice(data, this.SelectOnePatientProcess, txtPatientDob.Text);
										frm.ShowDialog();
										this.isAlertTreatmentEndInDay = true;//set true để không tìm kiếm lại do đã thực hiện trong SelectOnePatientProcess
									}
									else
									{
										HisPatientSDO _PatientSDO = data.SingleOrDefault();
										this.patientTD3 = _PatientSDO;
										dataResult.HisPatientSDO = _PatientSDO;
										dataResult.OldPatient = true;
										this.currentPatientSDO = _PatientSDO;
									this.dlgSendPatientSdo(currentPatientSDO);
									this.ProcessPatientCodeKeydown(_PatientSDO);
										dataResult.SearchTypePatient = 1;
										hrmEmployeeCode = _PatientSDO.HRM_EMPLOYEE_CODE;
										HeinCardData heinCardDataForCheckGOV = new HeinCardData();
										heinCardDataForCheckGOV = ConvertFromPatientData(_PatientSDO);

										long patientTypeId = this.cboPatientType.EditValue == null ? 0 : Inventec.Common.TypeConvert.Parse.ToInt64(this.cboPatientType.EditValue.ToString());
										if (!this.TD3 && patientTypeId == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT)
										{
											HIS.Desktop.Plugins.Library.CheckHeinGOV.HeinGOVManager heinGOVManager = new HIS.Desktop.Plugins.Library.CheckHeinGOV.HeinGOVManager(ResourceMessage.GoiSangCongBHXHTraVeMaLoi);
                                        DateTime dtIntructionTime = DateTime.Now;
                                        dtIntructionTime = this.dlgGetIntructionTime();
											if ((HisConfigCFG.IsBlockingInvalidBhyt == ((int)HisConfigCFG.OptionKey.Option1).ToString() || HisConfigCFG.IsBlockingInvalidBhyt == ((int)HisConfigCFG.OptionKey.Option2).ToString()))
												heinGOVManager.SetDelegateHeinEnableButtonSave(dlgHeinEnableSave);
											this.ResultDataADO = await heinGOVManager.Check(heinCardDataForCheckGOV, null, false, _PatientSDO.ADDRESS, dtIntructionTime, isReadQrCode);
										}

										if (this.ResultDataADO != null && this.ResultDataADO.ResultHistoryLDO != null)
										{
											heinCardDataForCheckGOV.HeinCardNumber = this.ResultDataADO.IsUsedNewCard ? this.ResultDataADO.ResultHistoryLDO.maTheMoi : this.ResultDataADO.ResultHistoryLDO.maThe;
                                        //Trường hợp tìm kiếm BN theo qrocde & BN có số thẻ bhyt mới, cần tìm kiếm BN theo số thẻ mới này & người dùng chọn lấy thông tin thẻ mới => tìm kiếm Bn theo số thẻ mới
                                        if (!String.IsNullOrEmpty(heinCardDataForCheckGOV.HeinCardNumber))
											{
												if (this.ResultDataADO.IsShowQuestionWhileChangeHeinTime__Choose)
                                            {
                                                heinCardDataForCheckGOV = this.ResultDataADO.HeinCardData;
                                            }
                                            dataResult.HeinCardData = heinCardDataForCheckGOV;
                                        }
										}
									}
								}
								else
								{
									this.patientTD3 = null;
									DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.MaCmndCccdKhongTontai + " '" + strValue + "'", MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
									this.txtPatientCode.Focus();
									this.txtPatientCode.SelectAll();
									return;
								}
						}
						else
						{
                            this.typeReceptionForm = ReceptionForm.QrCCCD;
							CccdCardData cccdCard = null;
							string strTemp = strValue.Trim();
							if (strValue.Trim().Contains("|"))
							{
								cccdCard = GetDataQrCodeCccdCard(strValue.Trim());
								if (cccdCard != null)
								{
									strValue = cccdCard.CardData;
								}
							}
							if (strValue.Trim().Length == 12 && !string.IsNullOrEmpty(txtPatientName.Text) && (!string.IsNullOrEmpty(txtPatientDob.Text) || dtPatientDob.EditValue != null))
							{
								if (this.dlgIsReadQrCode != null)
								{
									this.dlgIsReadQrCode(true);
								}
							}
							if (strTemp.Contains("|") || (strValue.Trim().Length == 12 && !string.IsNullOrEmpty(txtPatientName.Text) && (!string.IsNullOrEmpty(txtPatientDob.Text) || dtPatientDob.EditValue != null)))
							{
								var dataQr = await (ProcessSearchByCode(strTemp, 1));
								if (dataQr != null)
								{
									Inventec.Common.Logging.LogSystem.Warn("1________");
									dataResult.HisPatientSDO = new HisPatientSDO();
									if (dataQr is HisPatientSDO)
									{
										Inventec.Common.Logging.LogSystem.Warn("2________");
										dataResult.HisPatientSDO = (HisPatientSDO)dataQr;
										dataResult.OldPatient = true;
										this.currentPatientSDO = (HisPatientSDO)dataQr;
										this.dlgSendPatientSdo(currentPatientSDO);
										this.patientTD3 = (HisPatientSDO)dataQr;
									}
									else if (dataQr is HeinCardData || dataQr is CccdCardData)
									{
										Inventec.Common.Logging.LogSystem.Warn("3________");
										this.patientTD3 = null;
										if(dataQr is HeinCardData)
											dataResult.HeinCardData = (HeinCardData)dataQr;
										else
                                        {
											dataResult.HeinCardData = dataHeinCardFromQrCccd != null ? dataHeinCardFromQrCccd : null;
										}											
										dataResult.OldPatient = false;
									}
									if (strTemp.Contains("|"))
									{
                                        long patientTypeId = this.cboPatientType.EditValue == null ? 0 : Inventec.Common.TypeConvert.Parse.ToInt64(this.cboPatientType.EditValue.ToString());
                                            dataResult.HisPatientSDO.ADDRESS = dataResult.HisPatientSDO.ADDRESS ?? (patientTypeId == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT && dataResult.HisPatientSDO.ID > 0 ? "" : cccdCard.Address);

                                        //171433 IVT - Sửa chức năng Tiếp đón. Bổ sung xử lý tách thông tin địa chỉ lấy thông tin Tỉnh, Huyện, Xã
                                        Inventec.Common.Address.AddressProcessor adProc = new Inventec.Common.Address.AddressProcessor(BackendDataWorker.Get<V_SDA_PROVINCE>(), BackendDataWorker.Get<V_SDA_DISTRICT>(), BackendDataWorker.Get<V_SDA_COMMUNE>());
                                        var data = adProc.SplitFromFullAddress(cccdCard.Address);
                                        if (dataResult.HisPatientSDO.ID <= 0 || (data != null && (dataResult.HisPatientSDO.PROVINCE_CODE != data.ProvinceCode || dataResult.HisPatientSDO.DISTRICT_CODE != data.DistrictCode || dataResult.HisPatientSDO.COMMUNE_CODE != data.CommuneCode)))
                                        {
                                            if (data != null)
                                            {
                                                dataResult.HisPatientSDO.PROVINCE_CODE = data.ProvinceCode;
                                                dataResult.HisPatientSDO.PROVINCE_NAME = data.ProvinceName;
                                                dataResult.HisPatientSDO.DISTRICT_CODE = data.DistrictCode;
                                                dataResult.HisPatientSDO.DISTRICT_NAME = data.DistrictName;
                                                dataResult.HisPatientSDO.COMMUNE_CODE = data.CommuneCode;
                                                dataResult.HisPatientSDO.COMMUNE_NAME = data.CommuneName;
                                            }
                                        }

                                        if (!string.IsNullOrEmpty(cccdCard.ReleaseDate))
											dataResult.HisPatientSDO.CMND_DATE = Int64.Parse(cccdCard.ReleaseDate.Split('/')[2] + cccdCard.ReleaseDate.Split('/')[1] + cccdCard.ReleaseDate.Split('/')[0] + "000000");
										if (cccdCard.CardData.Length == 9)
										{
											dataResult.HisPatientSDO.CMND_NUMBER = cccdCard.CardData;
											dataResult.HisPatientSDO.CCCD_NUMBER = null;
										}
										else
										{
											dataResult.HisPatientSDO.CMND_NUMBER = null;
											dataResult.HisPatientSDO.CCCD_NUMBER = cccdCard.CardData;
										}
                                    }
                                    else
                                    {
										dataResult.HisPatientSDO.CMND_NUMBER = null;
										dataResult.HisPatientSDO.CCCD_NUMBER = strValue.Trim();
									}

									if (dataHeinCardFromQrCccd != null)
									{
										dataResult.HisPatientSDO.HeinCardNumber = dataHeinCardFromQrCccd.HeinCardNumber;
										dataResult.HisPatientSDO.HeinMediOrgCode = dataHeinCardFromQrCccd.MediOrgCode;
										if (!string.IsNullOrEmpty(dataHeinCardFromQrCccd.FromDate))
											dataResult.HisPatientSDO.HeinCardFromTime = Int64.Parse(dataHeinCardFromQrCccd.FromDate.Split('/')[2] + dataHeinCardFromQrCccd.FromDate.Split('/')[1] + dataHeinCardFromQrCccd.FromDate.Split('/')[0] + "000000");
										else
											dataResult.HisPatientSDO.HeinCardFromTime = null;
										if (!string.IsNullOrEmpty(dataHeinCardFromQrCccd.ToDate))
											dataResult.HisPatientSDO.HeinCardToTime = Int64.Parse(dataHeinCardFromQrCccd.ToDate.Split('/')[2] + dataHeinCardFromQrCccd.ToDate.Split('/')[1] + dataHeinCardFromQrCccd.ToDate.Split('/')[0] + "000000");
										else
											dataResult.HisPatientSDO.HeinCardToTime = null;
										dataResult.HisPatientSDO.HeinAddress = dataHeinCardFromQrCccd.Address;
										dataResult.HeinCardData = dataHeinCardFromQrCccd;
									}
									Inventec.Common.Logging.LogSystem.Warn("END________");
									dataResult.SearchTypePatient = 1;
								}
								else
								{
									Inventec.Common.Logging.LogSystem.Warn("4________");
									dataResult = null;
									this.patientTD3 = null;
								}
								Inventec.Common.Logging.LogSystem.Warn("5________");
								this.txtPatientCode.Text = "";
							}						
						}

					}
					#endregion

					#region ---- SoDT
					else if (this.typeCodeFind == ResourceMessage.typeCodeFind__SoDT)
					{
						param = new CommonParam();
						HisPatientAdvanceFilter filter = new HisPatientAdvanceFilter();
						filter.PHONE__EXACT = strValue;
						var data = (new BackendAdapter(param).Get<List<HisPatientSDO>>(RequestUriStore.HIS_PATIENT_GETSDOADVANCE, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param));
						WaitingManager.Hide();
						if (data != null && data.Count > 0)
						{
                            this.typeReceptionForm = ReceptionForm.SDT;
							if (data.Count > 1)
							{
								frmPatientChoice frm = new frmPatientChoice(data, this.SelectOnePatientProcess, txtPatientDob.Text);
								frm.ShowDialog();
								this.isAlertTreatmentEndInDay = true;//set true để không tìm kiếm lại do đã thực hiện trong SelectOnePatientProcess
							}
							else
							{
								HisPatientSDO _PatientSDO = data.SingleOrDefault();
								this.patientTD3 = _PatientSDO;
								dataResult.HisPatientSDO = _PatientSDO;
								dataResult.OldPatient = true;
								this.currentPatientSDO = _PatientSDO;
								this.dlgSendPatientSdo(currentPatientSDO);
								this.ProcessPatientCodeKeydown(_PatientSDO);
								dataResult.SearchTypePatient = 9;
								hrmEmployeeCode = _PatientSDO.HRM_EMPLOYEE_CODE;
								HeinCardData heinCardDataForCheckGOV = new HeinCardData();
								heinCardDataForCheckGOV = ConvertFromPatientData(_PatientSDO);

								long patientTypeId = this.cboPatientType.EditValue == null ? 0 : Inventec.Common.TypeConvert.Parse.ToInt64(this.cboPatientType.EditValue.ToString());
								if (!this.TD3 && patientTypeId == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT)
								{
									HIS.Desktop.Plugins.Library.CheckHeinGOV.HeinGOVManager heinGOVManager = new HIS.Desktop.Plugins.Library.CheckHeinGOV.HeinGOVManager(ResourceMessage.GoiSangCongBHXHTraVeMaLoi);
                                    DateTime dtIntructionTime = DateTime.Now;
                                    dtIntructionTime = this.dlgGetIntructionTime();
									if ((HisConfigCFG.IsBlockingInvalidBhyt == ((int)HisConfigCFG.OptionKey.Option1).ToString() || HisConfigCFG.IsBlockingInvalidBhyt == ((int)HisConfigCFG.OptionKey.Option2).ToString()))
										heinGOVManager.SetDelegateHeinEnableButtonSave(dlgHeinEnableSave);
									this.ResultDataADO = await heinGOVManager.Check(heinCardDataForCheckGOV, null, false, _PatientSDO.ADDRESS, dtIntructionTime, isReadQrCode);
								}

								if (this.ResultDataADO != null && this.ResultDataADO.ResultHistoryLDO != null)
								{
									heinCardDataForCheckGOV.HeinCardNumber = this.ResultDataADO.ResultHistoryLDO.maThe;
									//Trường hợp tìm kiếm BN theo qrocde & BN có số thẻ bhyt mới, cần tìm kiếm BN theo số thẻ mới này & người dùng chọn lấy thông tin thẻ mới => tìm kiếm Bn theo số thẻ mới
									if (!String.IsNullOrEmpty(heinCardDataForCheckGOV.HeinCardNumber))
									{
										if (this.ResultDataADO.IsShowQuestionWhileChangeHeinTime__Choose)
                                        {
                                            heinCardDataForCheckGOV = this.ResultDataADO.HeinCardData;

                                        }
                                        dataResult.HeinCardData = heinCardDataForCheckGOV;
                                    }
								}
							}
						}
						else
						{
							this.patientTD3 = null;
							DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy bệnh nhân có số điện thoại" + " '" + strValue + "'", MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
							this.txtPatientCode.Focus();
							this.txtPatientCode.SelectAll();
							return;
						}
					}
                    #endregion

                    #region ---- MaBA
                    else if (this.typeCodeFind == ResourceMessage.typeCodeFind__MaBA)
					{
                        WaitingManager.Hide();
                        Inventec.Common.Logging.LogSystem.Debug("Ma BA________________________");
                        this.typeReceptionForm = ReceptionForm.MaBA;
                        var data = await (ProcessSearchByCode(strValue, 1));
                        if (data != null)
                        {
                            if (data is HisPatientSDO)
                            {
                                dataResult.HisPatientSDO = (HisPatientSDO)data;
                                dataResult.OldPatient = true;
                                this.currentPatientSDO = (HisPatientSDO)data;
                                this.dlgSendPatientSdo(currentPatientSDO);
                                this.patientTD3 = (HisPatientSDO)data;
                                hrmEmployeeCode = currentPatientSDO.HRM_EMPLOYEE_CODE;
                            }
                            else if (data is HeinCardData)
                            {
                                this.patientTD3 = null;
                                dataResult.HeinCardData = (HeinCardData)data;
                                dataResult.OldPatient = false;
                            }
                            dataResult.SearchTypePatient = 1;
                        }
                        else
                        {
                            dataResult = null;
                            this.patientTD3 = null;
                        }
                    }
                    #endregion

                    if (this.typeCodeFind != ResourceMessage.typeCodeFind__MaNV)
					{
						this.dlgShowControlHrmKskCodeNotValid(false);
					if (!string.IsNullOrEmpty(hrmEmployeeCode) && this.dlgShowControlHrmKskCodeNotValid != null)
							this.dlgShowControlHrmKskCodeNotValid(true);
					}

					if (this.typeCodeFind == ResourceMessage.typeCodeFind__MaTV)
					{
                        int n;
                        bool isNumeric = int.TryParse(this.txtPatientCode.Text, out n);
                        if (!isNumeric)
                        {
                            this.patientTD3 = null;
                            DevExpress.XtraEditors.XtraMessageBox.Show("Mã tư vấn không tồn tại" + " '" + this.txtPatientCode.Text + "'", MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                            this.txtPatientCode.Focus();
                            this.txtPatientCode.SelectAll();
                            return;
                        }
                        var codeFind = string.Format("{0:0000000}", Convert.ToInt64(strValue));
                        this.txtPatientCode.Text = codeFind;
                        this.typeReceptionForm = ReceptionForm.MaTV;
                        param = new CommonParam();
                        var data = SearchByCode(codeFind, typeCodeFind);
                        WaitingManager.Hide();
						if(data != null && data is HisPatientSDO)
						{
                            HisPatientSDO _PatientSDO = data as HisPatientSDO;
                            this.patientTD3 = _PatientSDO;
                            dataResult.HisPatientSDO = _PatientSDO;
                            dataResult.OldPatient = true;
                            this.currentPatientSDO = _PatientSDO;
                            this.dlgSendPatientSdo(currentPatientSDO);
                            this.ProcessPatientCodeKeydown(_PatientSDO);
                            dataResult.SearchTypePatient = 6;
                            hrmEmployeeCode = _PatientSDO.HRM_EMPLOYEE_CODE;
							if (dlgCheckExamOnline != null)
							{
								dlgCheckExamOnline(true);
                            }

                            HeinCardData heinCardDataForCheckGOV = new HeinCardData();
                            heinCardDataForCheckGOV = ConvertFromPatientData(_PatientSDO);

                            long patientTypeId = this.cboPatientType.EditValue == null ? 0 : Inventec.Common.TypeConvert.Parse.ToInt64(this.cboPatientType.EditValue.ToString());
                            if (!this.TD3 && patientTypeId == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT)
                            {
                                HIS.Desktop.Plugins.Library.CheckHeinGOV.HeinGOVManager heinGOVManager = new HIS.Desktop.Plugins.Library.CheckHeinGOV.HeinGOVManager(ResourceMessage.GoiSangCongBHXHTraVeMaLoi);
                                DateTime dtIntructionTime = DateTime.Now;
                                dtIntructionTime = this.dlgGetIntructionTime();
                                if ((HisConfigCFG.IsBlockingInvalidBhyt == ((int)HisConfigCFG.OptionKey.Option1).ToString() || HisConfigCFG.IsBlockingInvalidBhyt == ((int)HisConfigCFG.OptionKey.Option2).ToString()))
                                    heinGOVManager.SetDelegateHeinEnableButtonSave(dlgHeinEnableSave);
                                this.ResultDataADO = await heinGOVManager.Check(heinCardDataForCheckGOV, null, false, _PatientSDO.ADDRESS, dtIntructionTime, isReadQrCode);
                            }

                            if (this.ResultDataADO != null && this.ResultDataADO.ResultHistoryLDO != null)
                            {
                                heinCardDataForCheckGOV.HeinCardNumber = this.ResultDataADO.ResultHistoryLDO.maThe;
                                //Trường hợp tìm kiếm BN theo qrocde & BN có số thẻ bhyt mới, cần tìm kiếm BN theo số thẻ mới này & người dùng chọn lấy thông tin thẻ mới => tìm kiếm Bn theo số thẻ mới
                                if (!String.IsNullOrEmpty(heinCardDataForCheckGOV.HeinCardNumber))
                                {
                                    if (this.ResultDataADO.IsShowQuestionWhileChangeHeinTime__Choose)
                                    {
                                        heinCardDataForCheckGOV = this.ResultDataADO.HeinCardData;
                                    }
                                    dataResult.HeinCardData = heinCardDataForCheckGOV;
                                }
                            }
                        }
                        else
						{
                            this.patientTD3 = null;
                            DevExpress.XtraEditors.XtraMessageBox.Show("Mã tư vấn không tồn tại" + " '" + this.txtPatientCode.Text + "'", MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                            this.txtPatientCode.Focus();
                            this.txtPatientCode.SelectAll();
                        }
                    }
					WaitingManager.Hide();
					if(dataResult != null && ((this.typeCodeFind == ResourceMessage.typeCodeFind__MaCMCC && oldValue.Trim().Contains("|")) || (this.typeCodeFind == ResourceMessage.typeCodeFind__MaBN && oldValue.Trim().Contains("|"))))
                    {
						dataResult.IsReadQr = true;

					}
                    MapHeinCardToPatientSDO();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataResult), dataResult));
                    if (!this.isAlertTreatmentEndInDay && this.dlgSearchPatient1 != null)
						this.dlgSearchPatient1(dataResult);
				}
				else
				{
					this.txtPatientName.Focus();
					this.txtPatientName.SelectAll();
				}
				if(dataResult != null && dataResult.HisPatientSDO != null && !string.IsNullOrEmpty(dataResult.HisPatientSDO.PATIENT_CODE))
                    DisableControlOldPatientInformationOption();
				else
                    DisableControlOldPatientInformationOption(true);
            }
            catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			finally
			{
				typeCodeFind = oldTypeFind;
			}
		}
        private void MapHeinCardToPatientSDO()
        {

            try
            {
                if (dataResult.HeinCardData != null)
                {
                    dataResult.HisPatientSDO.HeinCardNumber = dataResult.HeinCardData.HeinCardNumber;
                    dataResult.HisPatientSDO.HeinMediOrgCode = dataResult.HeinCardData.MediOrgCode;
                    if (!string.IsNullOrEmpty(dataResult.HeinCardData.FromDate))
                        dataResult.HisPatientSDO.HeinCardFromTime = Int64.Parse(dataResult.HeinCardData.FromDate.Split('/')[2] + dataResult.HeinCardData.FromDate.Split('/')[1] + dataResult.HeinCardData.FromDate.Split('/')[0] + "000000");
                    else
                        dataResult.HisPatientSDO.HeinCardFromTime = null;
                    if (!string.IsNullOrEmpty(dataResult.HeinCardData.ToDate))
                        dataResult.HisPatientSDO.HeinCardToTime = Int64.Parse(dataResult.HeinCardData.ToDate.Split('/')[2] + dataResult.HeinCardData.ToDate.Split('/')[1] + dataResult.HeinCardData.ToDate.Split('/')[0] + "000000");
                    else
                        dataResult.HisPatientSDO.HeinCardToTime = null;
                    dataResult.HisPatientSDO.HeinAddress = dataResult.HeinCardData.Address;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private async Task SetDataFromCardSDO(HisCardSDO patientInRegisterSearchByCard)
        {
            try
            {
				string heinAddressOfPatient = "";
				dataResult.HisCardSDO = patientInRegisterSearchByCard;
				this.cardSearch = patientInRegisterSearchByCard;
				if (!String.IsNullOrEmpty(patientInRegisterSearchByCard.PatientCode))
				{
					var data = SearchByCode(patientInRegisterSearchByCard.PatientCode);
					if (data != null && data is HisPatientSDO)
					{
						dataResult.HisPatientSDO = (HisPatientSDO)data;
						currentPatientSDO = (HisPatientSDO)data;
						this.dlgSendPatientSdo(currentPatientSDO);
						this.patientTD3 = (HisPatientSDO)data;
						dataResult.OldPatient = true;
						this.FillDataPatientToControl(dataResult.HisPatientSDO, true);
						heinAddressOfPatient = dataResult.HisPatientSDO.HeinAddress;
						hrmEmployeeCode = currentPatientSDO.HRM_EMPLOYEE_CODE;
					}
					else
					{
						this.patientTD3 = null;
					}
				}
				else
				{
					this.Invoke(new MethodInvoker(delegate ()
					{
						//Benh nhan chua dang ky tren he thong benh vien, chua co thong tin ho so, fill thông tin bệnh nhân theo thẻ lên control
						HisPatientSDO patientByCard = new HisPatientSDO();
						this.SetPatientDTOFromCardSDO(patientInRegisterSearchByCard, patientByCard);
						dataResult.HisPatientSDO = patientByCard;
						this.patientTD3 = patientByCard;
						dataResult.OldPatient = false;
						this.FillDataPatientToControl(patientByCard, true);
					}));
				}
				dataResult.SearchTypePatient = 5;
				HeinCardData heinCardDataForCheckGOV = new HeinCardData();
				heinCardDataForCheckGOV = ConvertFromPatientData(dataResult.HisPatientSDO);

				////xuandv fill data hein truoc khi check
				if (this.dlgFillDataPreviewForSearchByQrcodeInUCPatientRaw != null)
					this.dlgFillDataPreviewForSearchByQrcodeInUCPatientRaw(heinCardDataForCheckGOV);

				long patientTypeId = this.cboPatientType.EditValue == null ? 0 : Inventec.Common.TypeConvert.Parse.ToInt64(this.cboPatientType.EditValue.ToString());
				if (!this.TD3 && patientTypeId == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT)
				{
					HeinGOVManager heinGOVManager = new HeinGOVManager(ResourceMessage.GoiSangCongBHXHTraVeMaLoi);
					if ((HisConfigCFG.IsBlockingInvalidBhyt == ((int)HisConfigCFG.OptionKey.Option1).ToString() || HisConfigCFG.IsBlockingInvalidBhyt == ((int)HisConfigCFG.OptionKey.Option2).ToString()))
						heinGOVManager.SetDelegateHeinEnableButtonSave(dlgHeinEnableSave);
					this.ResultDataADO = await heinGOVManager.Check(heinCardDataForCheckGOV, null, false, heinAddressOfPatient, this.dlgGetIntructionTime(), isReadQrCode);
				}
				if (this.ResultDataADO != null && this.ResultDataADO.ResultHistoryLDO != null)
				{
					heinCardDataForCheckGOV.HeinCardNumber = this.ResultDataADO.IsUsedNewCard ? this.ResultDataADO.ResultHistoryLDO.maTheMoi : this.ResultDataADO.ResultHistoryLDO.maThe;
                    //Trường hợp tìm kiếm BN theo qrocde & BN có số thẻ bhyt mới, cần tìm kiếm BN theo số thẻ mới này & người dùng chọn lấy thông tin thẻ mới => tìm kiếm Bn theo số thẻ mới
                    if (!String.IsNullOrEmpty(heinCardDataForCheckGOV.HeinCardNumber))
					{
						if (this.ResultDataADO.IsShowQuestionWhileChangeHeinTime__Choose)
						{
							heinCardDataForCheckGOV = this.ResultDataADO.HeinCardData;
                        }
                        dataResult.HeinCardData = heinCardDataForCheckGOV;
                    }
				}

				if (!String.IsNullOrEmpty(heinCardDataForCheckGOV.HeinCardNumber))
				{
					this.CheckPatientOldByHeinCard(heinCardDataForCheckGOV, false);//xuandv sua ve false// k check lan 2
				}
			}
            catch (Exception ex)
            {
				LogSystem.Warn(ex);
			}
        }
		private string ProcessDate(string date)
		{
			string result = "";
			try
			{
				if (!string.IsNullOrEmpty(date))
				{
					if (date.Length == 4)
					{
						result = date;
					}
					else if (date.Length == 6)
					{
						result = new StringBuilder().Append(date.Substring(0, 2)).Append("/").Append(date.Substring(2, 4))
							.ToString();
					}
					else if (date.Length == 8)
					{
						result = new StringBuilder().Append(date.Substring(0, 2)).Append("/").Append(date.Substring(2, 2))
							.Append("/")
							.Append(date.Substring(4, 4))
							.ToString();
					}
				}
			}
			catch (Exception ex)
			{
				LogSystem.Warn(ex);
			}

			return result;
		}

		public void GetDataBySearchPatient(DelegateSetDataRegisterBeforeSerachPatient _dlgSearchPatient, Action<HeinCardData> _dlgFillDataPreviewForSearchByQrcodeInUCPatientRaw, Action<HisPatientSDO> initExamServiceRoomByAppoimentTime = null)
		{
			try
			{
				if (_dlgSearchPatient != null)
					this.dlgSearchPatient1 = _dlgSearchPatient;
				if (_dlgFillDataPreviewForSearchByQrcodeInUCPatientRaw != null)
					this.dlgFillDataPreviewForSearchByQrcodeInUCPatientRaw = _dlgFillDataPreviewForSearchByQrcodeInUCPatientRaw;
				this.actInitExamServiceRoomByAppoimentTime = initExamServiceRoomByAppoimentTime;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		public void GetDataBySearchPatient(DelegateSetDataRegisterBeforeSerachPatient _dlgSearchPatient)
		{
			try
			{
				if (_dlgSearchPatient != null)
					this.dlgSearchPatient1 = _dlgSearchPatient;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		#region Search Patient by name, dob, gencer

		private void SearchPatientByFilterCombo()
		{
			try
			{
				string strDob = "";
				if (this.txtPatientDob.Text.Length == 4)
					strDob = "01/01/" + this.txtPatientDob.Text;
				else if (this.txtPatientDob.Text.Length == 8)
				{
					strDob = this.txtPatientDob.Text.Substring(0, 2) + "/" + this.txtPatientDob.Text.Substring(2, 2) + "/" + this.txtPatientDob.Text.Substring(4, 4);
				}
				else
					strDob = this.txtPatientDob.Text;
				this.dtPatientDob.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(strDob);
				this.dtPatientDob.Update();

				//Trường hợp chưa nhập đủ 3 thông tin: hộ tên, ngày sinh, giới tính thì bỏ qua không thưc hiện tìm kiếm
				if ((this.dtPatientDob.EditValue == null
					|| this.dtPatientDob.DateTime == DateTime.MinValue)
					|| this.cboGender.EditValue == null
					|| String.IsNullOrEmpty(this.txtPatientName.Text.Trim()))
				{
					return;
				}

				LogSystem.Debug("Bat dau tim kiem benh nhan theo filter.");
				string dateDob = this.dtPatientDob.DateTime.ToString("yyyyMMdd");
				string timeDob = "00";
				if (this.txtAge.Enabled)
					//&& this.cboAge.Enabled)
					timeDob = string.Format("{0:00}", DateTime.Now.Hour - Inventec.Common.TypeConvert.Parse.ToInt32(this.txtAge.Text));

				long dob = Inventec.Common.TypeConvert.Parse.ToInt64(dateDob + timeDob + "0000");
				short ismale = Convert.ToInt16(this.cboGender.EditValue);
				this.LoadDataSearchPatient("", this.txtPatientName.Text, dob, ismale, true);
				this.cardSearch = null;
				LogSystem.Debug("Ket thuc tim kiem benh nhan theo filter.");
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void LoadDataSearchPatient(string maBN, string hoten, long? dob, short? isMale, bool isSearchData)
		{
			try
			{
				LogSystem.Debug("LoadDataSearchPatient => t1");
				CommonParam param = new CommonParam();
				MOS.Filter.HisPatientAdvanceFilter hisPatientFilter = new MOS.Filter.HisPatientAdvanceFilter();
				hisPatientFilter.DOB = dob;
				hisPatientFilter.VIR_PATIENT_NAME__EXACT = hoten;
				if (!String.IsNullOrEmpty(maBN))
				{
					hisPatientFilter.PATIENT_CODE__EXACT = string.Format("{0:0000000000}", Inventec.Common.TypeConvert.Parse.ToInt64(maBN));
				}
				hisPatientFilter.GENDER_ID = isMale;
				this.currentSearchedPatients = new BackendAdapter(param).Get<List<HisPatientSDO>>(RequestUriStore.HIS_PATIENT_GETSDOADVANCE, ApiConsumers.MosConsumer, hisPatientFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisPatientFilter), hisPatientFilter));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentSearchedPatients), currentSearchedPatients));
				if (this.currentSearchedPatients != null && this.currentSearchedPatients.Count > 0)
				{
					LogSystem.Debug("LoadDataSearchPatient => t1.1. Tim thay benh nhan cu, hien thi cua so chon benh nhan");
					LogSystem.Debug("LoadDataSearchPatient => t1.3");
					frmPatientChoice frm = new frmPatientChoice(this.currentSearchedPatients, this.SelectOnePatientProcess, txtPatientDob.Text);
					frm.ShowDialog();
				}
				else
				{
					this.currentPatientSDO = null;
					this.dlgSendPatientSdo(currentPatientSDO);
					bool isGKS = MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(this.dtPatientDob.DateTime);
					if (this.isGKS == true || this.isGKS == true && this.isTemp_QN == true)
						this.isEnable(true, null);
					else if (this.isTemp_QN == true)
						this.isEnable(null, true);
					else
						this.isEnable(null, false);
				}
				LogSystem.Debug("LoadDataSearchPatient => t2");
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		#endregion
		List<string> lstSend = new List<string>();
		private void PeriosTreatmentMessage()
		{
			try
			{
				//- Kiểm tra cấu hình trên CCC: MOS.HIS_TREATMENT.IS_CHECK_PREVIOUS_PRESCRIPTION
				//1: Khi đăng ký tiếp đón, có kiểm tra xem đợt khám/điều trị trước đó của BN đã uống hết thuốc hay chưa 
				//0: Không kiểm tra
				LogSystem.Debug("Tiep don: Cau hinh co kiem tra dot dieu tri truoc cua BN con thuoc chua uong het hay khong: IsCheckPreviousPrescription = " + HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.IsCheckPreviousPrescription);
				string message = "";
				lstSend = new List<string>();
				if (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.IsCheckPreviousPrescription)
				{
					if (this.currentPatientSDO.PreviousPrescriptions != null
						&& this.currentPatientSDO.PreviousPrescriptions.Count > 0)
					{
						LogSystem.Debug("Tiep don: Du lieu benh nhan cu: " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentPatientSDO), this.currentPatientSDO));
						string treatmentPrevis = this.currentPatientSDO.TreatmentCode;
						string pressMessages = "";
						if (this.currentPatientSDO.PreviousPrescriptions.Count == 1)
							//pressMessages += String.Format(HIS.UC.UCPatientRaw.Base.ResourceMessage.ThuocCoThoiSuDungDen,
							//    (" - " + this.currentPatientSDO.PreviousPrescriptions[i].REQUEST_ROOM_NAME + " ")
							//    , Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentPatientSDO.PreviousPrescriptions[i].USE_TIME_TO ?? 0) + "\r\n");
							pressMessages += String.Format("Hồ sơ {0} còn thuốc của {1} có thuốc kê sử dụng đến {2}",
								this.currentPatientSDO.PreviousPrescriptions[0].TREATMENT_CODE,
							   this.currentPatientSDO.PreviousPrescriptions[0].REQUEST_ROOM_NAME
							   , Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentPatientSDO.PreviousPrescriptions[0].USE_TIME_TO ?? 0));
						else
						{
							var dataGroups = this.currentPatientSDO.PreviousPrescriptions.GroupBy(p => p.TREATMENT_CODE).Select(p => p.ToList()).ToList();
							foreach (var itemGr in dataGroups)
							{
								pressMessages += string.Format(" Hồ sơ {0} còn thuốc của: \r\n", itemGr[0].TREATMENT_CODE);
								for (int i = 0; i < itemGr.Count; i++)
								{
									pressMessages += String.Format(ResourceMessage.ThuocCoThoiSuDungDen,
							   (" - " + this.currentPatientSDO.PreviousPrescriptions[i].REQUEST_ROOM_NAME + " ")
							   , Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentPatientSDO.PreviousPrescriptions[i].USE_TIME_TO ?? 0) + "\r\n");
								}
							}
						}

						message += String.Format(ResourceMessage.DotKhamTruocCuaBenhNhanCoThuocChuaUongHet, treatmentPrevis, "\r\n", pressMessages, "");
					}
				}
				LogSystem.Debug("Tiep don: Cau hinh co kiem tra dot dieu tri truoc cua BN con no tien vien phi hay khong: IsCheckPreviousDebt = " + HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.IsCheckPreviousDebt);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentPatientSDO), this.currentPatientSDO));
				var dtPatientType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Find(o => o.PATIENT_TYPE_CODE == HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT"));
				if (this.dlgEnableSave != null)
					this.dlgEnableSave(true);
				if (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.IsCheckPreviousDebt == "1" || HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.IsCheckPreviousDebt == "3" || HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.IsCheckPreviousDebt == "4")
				{
					if (this.currentPatientSDO.PreviousDebtTreatments != null
						&& this.currentPatientSDO.PreviousDebtTreatments.Count > 0)
					{
						LogSystem.Debug("Tiep don: Du lieu benh nhan cu: " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentPatientSDO), this.currentPatientSDO));
						string treatmentPrevis = String.Join(",", this.currentPatientSDO.PreviousDebtTreatments.Distinct().ToList());
						if (!String.IsNullOrEmpty(message))
						{
							message += "\r\n";
						}
						if (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.IsCheckPreviousDebt == "4")
						{
							message += String.Format("Đợt khám/điều trị trước đó của bệnh nhân có số tiền phải trả lớn hơn 0  hoặc chưa duyệt khóa viện phí. Mã hồ sơ điều trị {0}. Bạn có muốn đăng ký tiếp đón không?", treatmentPrevis);
						}
						else
						{
							if (HisConfigCFG.IsCheckPreviousDebt == "1")
							{
								message += String.Format(ResourceMessage.DotKhamTruocCuaBenhNhanConNoTienVienPhi, treatmentPrevis);
							}							
						}
					}
					if (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.IsCheckPreviousDebt == "3" && this.currentPatientSDO.LastTreatmentFee != null)
					{
						if (this.currentPatientSDO.LastTreatmentFee.IS_ACTIVE == 1 || ((this.currentPatientSDO.LastTreatmentFee.TOTAL_PATIENT_PRICE ?? 0) - (this.currentPatientSDO.LastTreatmentFee.TOTAL_DEPOSIT_AMOUNT ?? 0) - (this.currentPatientSDO.LastTreatmentFee.TOTAL_BILL_AMOUNT ?? 0) +(this.currentPatientSDO.LastTreatmentFee.TOTAL_BILL_TRANSFER_AMOUNT ?? 0) + (this.currentPatientSDO.LastTreatmentFee.TOTAL_REPAY_AMOUNT ?? 0)) > 0)
						{
							lstSend = new List<string>() { this.currentPatientSDO.LastTreatmentFee.TREATMENT_CODE };
							message += String.Format(ResourceMessage.DotKhamTruocCuaBenhNhanConNoTienVienPhi, this.currentPatientSDO.LastTreatmentFee.TREATMENT_CODE);							
						}
					}
				}
				else if (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.IsCheckPreviousDebt == "2" && !IsEmergency && dtPatientType != null && this.currentPatientSDO.PreviousDebtTreatmentDetails != null
						&& this.currentPatientSDO.PreviousDebtTreatmentDetails.Count > 0)
				{
					var dtTreatmentDetails = this.currentPatientSDO.PreviousDebtTreatmentDetails.Where(o => o.PATIENT_TYPE_ID == dtPatientType.ID).ToList();
					if (dtTreatmentDetails != null && dtTreatmentDetails.Count > 0)
					{
						string treatmentPrevis = String.Join(",", dtTreatmentDetails.Select(o => o.TDL_TREATMENT_CODE).ToList());
						if (!String.IsNullOrEmpty(message))
						{
							message += "\r\n";
						}
						message += String.Format("Đợt khám/điều trị trước đó của bệnh nhân còn nợ viện phí hoặc chưa duyệt khóa viện phí. Mã hồ sơ điều trị {0}. Không cho phép tiếp đón", treatmentPrevis);
						if (this.dlgEnableSave != null)
							this.dlgEnableSave(false);
					}
				}				
				if (!String.IsNullOrEmpty(message))
				{
					if (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.IsCheckPreviousDebt == "4" || HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.IsCheckPreviousDebt == "3")
					{
						if(DevExpress.XtraEditors.XtraMessageBox.Show(message, MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),MessageBoxButtons.YesNo) != DialogResult.Yes)
						{
							if (this.dlgEnableSave != null)
								this.dlgEnableSave(false);
						}
					}
					else
					{
						MessageManager.Show(message);
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
		public bool IsStateEnableAgeOld = false;
		private void DisableControlOldPatientInformationOption(bool IsRefresh = false)
		{

			try
			{
				if (!IsRefresh && HisConfigCFG.EditOldPatientInformationOption && (this.typeCodeFind == ResourceMessage.typeCodeFind__MaBN || this.typeCodeFind == ResourceMessage.typeCodeFind__MaHK || this.typeCodeFind == ResourceMessage.typeCodeFind__MaDT || this.typeCodeFind == ResourceMessage.typeCodeFind__SoDT)) {
                    txtPatientName.Enabled = false;
                    cboGender.Enabled = false;
                    dtPatientDob.Enabled = false;
					txtPatientDob.Enabled = false;
					txtAge.Enabled = false;
                }
				else
				{
					txtPatientName.Enabled = true;
					cboGender.Enabled = true;
					dtPatientDob.Enabled = true;
                    txtPatientDob.Enabled = true;
					txtAge.Enabled = IsStateEnableAgeOld;
                }

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}

		}

    }
}
