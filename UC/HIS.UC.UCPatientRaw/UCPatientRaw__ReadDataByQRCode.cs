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
using Inventec.Core;
using MOS.SDO;
using MOS.Filter;
using Inventec.Common.Adapter;
using Inventec.Common.QrCodeBHYT;
using HIS.UC.UCPatientRaw.Base;
using HIS.Desktop.ApiConsumer;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.LibraryHein.Bhyt;
using Inventec.Common.QrCodeCCCD;

namespace HIS.UC.UCPatientRaw
{
	public partial class UCPatientRaw : HIS.Desktop.Utility.UserControlBase
    {
		long patientId = 0;
		string patientCode = "";
		string employeeCode = "";
		// trả về thông tin thẻ nếu bệnh nhân có thẻ
		private object SearchByCode(string code, string type = null)
		{
			try
			{
				var data = new HisPatientSDO();
				if (String.IsNullOrEmpty(code)) throw new ArgumentNullException("code is null");
				// trường hợp là qrcode
				if (code.Length > 12 && code.Contains("|") && this.typeCodeFind == ResourceMessage.typeCodeFind__MaCMCC)
				{
					return GetDataQrCodeCccdCard(code);
				}else if (code.Trim().Length == 12 && !string.IsNullOrEmpty(txtPatientName.Text) && (!string.IsNullOrEmpty(txtPatientDob.Text) || dtPatientDob.EditValue != null) && this.typeCodeFind == ResourceMessage.typeCodeFind__MaCMCC)
				{
					CccdCardData cccd = new CccdCardData();
					cccd.CardData = code.Trim();
					if(!string.IsNullOrEmpty(txtPatientDob.Text))
						cccd.Dob = txtPatientDob.Text.Contains("/") ? txtPatientDob.Text : ProcessDate(txtPatientDob.Text);
					else
						cccd.Dob = dtPatientDob.Text;
					cccd.PatientName = txtPatientName.Text.Trim();
					return cccd;
				}					
				else if (code.Length > 10 && code.Contains("|"))
				{
                    Inventec.Common.Logging.LogSystem.Debug("Tiep don Ma BHYT__");
                    this.typeReceptionForm = ReceptionForm.TheBHYT;
					return GetDataQrCodeHeinCard(code);
				}
				else
				{
					CommonParam param = new CommonParam();
					HisPatientAdvanceFilter filter = new HisPatientAdvanceFilter();
                    if (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.ISALLOWPROGRAMPATIENTOLD == "1" && this.typeCodeFind == ResourceMessage.typeCodeFind__MaBA)
                    {
                        filter.STORE_CODE__EXACT = code;
                    }
					else if ((HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.ISALLOWPROGRAMPATIENTOLD == "1" && this.typeCodeFind != ResourceMessage.typeCodeFind__MaBA))
					{
                        if (!String.IsNullOrWhiteSpace(type))
                        {
                            if (this.typeCodeFind == ResourceMessage.typeCodeFind__MaHK)
                            {
                                filter.APPOINTMENT_CODE__EXACT = code;
                            }
                            else if (this.typeCodeFind == ResourceMessage.typeCodeFind__MaDT)
                            {
                                filter.TREATMENT_CODE__EXACT = code;
                            }
                            else if (this.typeCodeFind == ResourceMessage.typeCodeFind__MaTV)
                            {
                                filter.CONSULTATION_REG_CODE = code;
                            }
                            else //thêm filter lỗi để không trả về tất cả dữ liệu
                            {
                                filter.PATIENT_CODE__EXACT = "-1";
                            }
                        }
                        else
                        {
                            filter.PATIENT_CODE__EXACT = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        }
                    }
					else if (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.ISALLOWPROGRAMPATIENTOLD != "1")
					{
						if (!String.IsNullOrWhiteSpace(type))
						{
							if (this.typeCodeFind == ResourceMessage.typeCodeFind__MaHK)
							{
								filter.APPOINTMENT_CODE__EXACT = code;
							}
							else if (this.typeCodeFind == ResourceMessage.typeCodeFind__MaDT)
							{
								filter.TREATMENT_CODE__EXACT = code;
							}
							else if (this.typeCodeFind == ResourceMessage.typeCodeFind__MaTV)
							{
								filter.CONSULTATION_REG_CODE = code;
							}
							else //thêm filter lỗi để không trả về tất cả dữ liệu
							{
								filter.PATIENT_CODE__EXACT = "-1";
							}
						}
						else if (this.typeCodeFind == ResourceMessage.typeCodeFind__MaBA)
						{
							filter.STORE_CODE__EXACT = code;
						}
						else
						{
							filter.PATIENT_CODE__EXACT = string.Format("{0:0000000000}", Convert.ToInt64(code));
						}
					}
                    
					                  
					data = (new BackendAdapter(param).Get<List<HisPatientSDO>>(RequestUriStore.HIS_PATIENT_GETSDOADVANCE, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param)).SingleOrDefault();

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
					Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
					if (data != null)
					{
						CommonParam paramCommon = new CommonParam();
						HisTreatmentFilter filterTreatment = new HisTreatmentFilter();
						filterTreatment.TDL_PATIENT_CODE__EXACT = data.PATIENT_CODE;
						var lstTreatment = new BackendAdapter(paramCommon).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filterTreatment, paramCommon).ToList();
						if (lstTreatment != null && lstTreatment.Count > 0)
						{
							var treatment = lstTreatment.Where(o => o.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET).ToList();
							if (treatment != null && treatment.Count > 0)
							{
								Inventec.Desktop.Common.Message.WaitingManager.Hide();
								if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.BenhNhanDaTuVong, treatment.FirstOrDefault().TDL_PATIENT_NAME), ResourceMessage.TieuDeCuaSoThongBaoLaThongBao, System.Windows.Forms.MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK)
								{
									this.txtPatientCode.Focus();
									this.txtPatientCode.SelectAll();

									return null;
								}
							}
						}
					}
				}
				return data;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}

			return null;
		}

		private HeinCardData GetDataQrCodeHeinCard(string qrCode)
		{
			HeinCardData dataHein = null;
			try
			{
				if (this.dlgIsReadQrCode != null)
				{
					this.dlgIsReadQrCode(true);
				}
				//Lay thong tin tren th BHYT cua benh nhan khi quet the doc chuoi qrcode
				ReadQrCodeHeinCard readQrCode = new ReadQrCodeHeinCard();
				dataHein = readQrCode.ReadDataQrCode(qrCode);

				BhytHeinProcessor _BhytHeinProcessor = new BhytHeinProcessor();
				if (!_BhytHeinProcessor.IsValidHeinCardNumber(dataHein.HeinCardNumber))
				{
					//DevExpress.XtraEditors.XtraMessageBox.Show("Mã QR không hợp lệ. Vui lòng kiểm tra lại.", "Thông báo");
					return null;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}

			return dataHein;
		}

		private CccdCardData GetDataQrCodeCccdCard(string qrCode)
		{
			CccdCardData dataCccd = null;
			try
			{
				if (this.dlgIsReadQrCode != null)
				{
					this.dlgIsReadQrCode(true);
				}
				//ReadQrCodeCCCD readQrCode = new ReadQrCodeCCCD();
				dataCccd = ReadQrCodeCCCD.ReadDataQrCode(qrCode);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}

			return dataCccd;
		}

		private void SetPatientDTOFromCardSDO(HisCardSDO cardSDO, HisPatientSDO patientByCard)
		{
			try
			{
				if (cardSDO == null) throw new ArgumentNullException("cardSDO");
				if (patientByCard == null) throw new ArgumentNullException("patientByCard");

				patientByCard.ID = (cardSDO.PatientId ?? 0);
				patientByCard.PATIENT_CODE = cardSDO.PatientCode;
				patientByCard.FIRST_NAME = cardSDO.FirstName;
				patientByCard.LAST_NAME = cardSDO.LastName;
				patientByCard.ADDRESS = cardSDO.Address;
				patientByCard.CAREER_ID = cardSDO.CareerId;
				patientByCard.CMND_DATE = cardSDO.CmndDate;
				patientByCard.CMND_NUMBER = cardSDO.CmndNumber;
				patientByCard.CMND_PLACE = cardSDO.CmndPlace;
				patientByCard.COMMUNE_NAME = cardSDO.CommuneName;
				patientByCard.DISTRICT_NAME = cardSDO.DistrictName;
				patientByCard.PROVINCE_NAME = cardSDO.ProvinceName;
				patientByCard.NATIONAL_NAME = cardSDO.NationalName;

				patientByCard.DOB = cardSDO.Dob;
				patientByCard.EMAIL = cardSDO.Email;
				patientByCard.ETHNIC_NAME = cardSDO.EthnicName;
				if (cardSDO.Dob > 0 && cardSDO.Dob.ToString().Length == 4)
					patientByCard.IS_HAS_NOT_DAY_DOB = 1;
				else
					patientByCard.IS_HAS_NOT_DAY_DOB = 0;
				patientByCard.PHONE = cardSDO.Phone;
				patientByCard.RELIGION_NAME = cardSDO.ReligionName;
				patientByCard.VIR_ADDRESS = cardSDO.VirAddress;
				patientByCard.VIR_PATIENT_NAME = patientByCard.LAST_NAME + " " + patientByCard.FIRST_NAME;
				patientByCard.GENDER_ID = cardSDO.GenderId;
                patientByCard.PERSON_CODE = cardSDO.PersonCode;
                patientByCard.HeinAddress = cardSDO.HeinAddress;
				patientByCard.HeinCardFromTime = cardSDO.HeinCardFromTime;
				patientByCard.HeinCardNumber = cardSDO.HeinCardNumber;
				patientByCard.HeinCardToTime = cardSDO.HeinCardToTime;
				patientByCard.HeinMediOrgCode = cardSDO.HeinOrgCode;
				patientByCard.HeinMediOrgName = cardSDO.HeinOrgName;
				patientByCard.IS_HAS_NOT_DAY_DOB = cardSDO.IsHasNotDayDob;
				patientByCard.Join5Year = cardSDO.Join5Year;
				patientByCard.LiveAreaCode = cardSDO.LiveAreaCode;
				patientByCard.Paid6Month = cardSDO.Paid6Month;
				patientByCard.RightRouteCode = cardSDO.RightRouteCode;
				patientByCard.WORK_PLACE = cardSDO.WorkPlace;
				patientByCard.HT_COMMUNE_CODE = cardSDO.HtCommuneCode;
                patientByCard.HT_COMMUNE_NAME = cardSDO.HtCommuneName;
                patientByCard.HT_DISTRICT_CODE = cardSDO.HtDistrictCode;
                patientByCard.HT_DISTRICT_NAME = cardSDO.HtDistrictName;
                patientByCard.HT_PROVINCE_CODE = cardSDO.HtProvinceCode;
                patientByCard.HT_PROVINCE_NAME = cardSDO.HtProvinceName;
            }
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void FillDataPatientToControl(HisPatientSDO patientDTO, bool isCheckPeriosTreatment)
		{
			try
			{
				Inventec.Common.Logging.LogSystem.Debug("FillDataPatientToControl.1");
				if (patientDTO.ID > 0 && isCheckPeriosTreatment && !AlertTreatmentInOutInDayForTreatmentMessage(patientDTO))
				{
					Inventec.Common.Logging.LogSystem.Debug("FillDataPatientToControl.2");
					this.currentPatientSDO = null;
					this.dlgSendPatientSdo(currentPatientSDO);
					this.isReadQrCode = true;
					this.qrCodeBHYTHeinCardData = null;
					this._UCPatientRawADO = new ADO.UCPatientRawADO();
					return;
				}
				Inventec.Common.Logging.LogSystem.Debug("FillDataPatientToControl.3");
				if (patientDTO == null) throw new ArgumentNullException("patientDTO is null");
				if (this.btnCodeFind.Text == ResourceMessage.typeCodeFind__MaBN)
				{
					this.txtPatientCode.Text = patientDTO.PATIENT_CODE;
				}
				if (this.typeCodeFind == ResourceMessage.typeCodeFind__MaNV)
				{
					this.cboPatientType.EditValue = 42;
					patientId = patientDTO.ID;
					patientCode = patientDTO.PATIENT_CODE;
				}
				this.txtPatientName.Text = patientDTO.VIR_PATIENT_NAME;
				if (!this.isReadQrCode)
				{
					if (patientDTO.DOB > 0 && patientDTO.DOB.ToString().Length >= 6)
					{
						if (patientDTO.IS_HAS_NOT_DAY_DOB == 1)
							this.LoadNgayThangNamSinhBNToForm(patientDTO.DOB, true);
						else
							this.LoadNgayThangNamSinhBNToForm(patientDTO.DOB, false);
					}
				}

				MOS.EFMODEL.DataModels.HIS_GENDER gioitinh = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().SingleOrDefault(o => o.ID == patientDTO.GENDER_ID);
				if (gioitinh != null)
				{
					this.cboGender.EditValue = gioitinh.ID;
				}
				if (!String.IsNullOrEmpty(patientDTO.CAREER_CODE) && patientDTO.CAREER_ID != null && patientDTO.CAREER_ID > 0)
				{
					this.txtCareerCode.Text = patientDTO.CAREER_CODE;
                    this.cboCareer.EditValue = patientDTO.CAREER_ID;
                }
				else
				{
					MOS.EFMODEL.DataModels.HIS_CAREER career = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).SingleOrDefault(o => o.ID == patientDTO.CAREER_ID);
					if (career != null)
					{
						this.txtCareerCode.Text = career.CAREER_CODE;
                        this.cboCareer.EditValue = patientDTO.CAREER_ID;
                    }
					else if (!String.IsNullOrEmpty(patientDTO.HeinCardNumber))
					{
						this.FillDataCareerUnder6AgeByHeinCardNumber(patientDTO.HeinCardNumber);
					}
				}

				if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.ChangeEthnic != 0 && lciFortxtEthnicCode.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
				{
					this.cboEthnic.EditValue = patientDTO.ETHNIC_CODE;
					this.txtEthnicCode.Text = patientDTO.ETHNIC_CODE;
					if ((this.typeCodeFind == ResourceMessage.typeCodeFind__SoThe && String.IsNullOrEmpty(patientDTO.ETHNIC_CODE)) || dlgSendPatientSdo != null)
					{
						var ethnicCur = dataEthnic.Where(o => o.ETHNIC_NAME == patientDTO.ETHNIC_NAME).ToList();
						if (ethnicCur != null && ethnicCur.Count > 0)
						{
							this.cboEthnic.EditValue = ethnicCur.FirstOrDefault().ETHNIC_CODE;
							this.txtEthnicCode.Text = ethnicCur.FirstOrDefault().ETHNIC_CODE;
							patientDTO.ETHNIC_CODE = ethnicCur.FirstOrDefault().ETHNIC_CODE;
						}
					}
				}
				if (this.typeCodeFind == ResourceMessage.typeCodeFind__SoThe || dlgSendPatientSdo != null)
				{
					if (!string.IsNullOrEmpty(patientDTO.PHONE) && patientDTO.PHONE.StartsWith("84"))
					{
						patientDTO.PHONE = "0" + patientDTO.PHONE.Substring(2, patientDTO.PHONE.Length - 2);
					}
				}
				this.currentPatientSDO = patientDTO;
				this.dlgSendPatientSdo(currentPatientSDO);
				if (isCheckPeriosTreatment)
					this.PeriosTreatmentMessage();
				Inventec.Common.Logging.LogSystem.Debug("FillDataPatientToControl.4");
				popupControlContainer1.HidePopup();
				this.txtPosition.Text = "";
				this.txtMilitaryRank.Text = "";
				this.txtWorkPlace.Text = "";
				this.txtPatientClassify.Text = "";
				this.currentPosition = null;
				this.currentMilitaryRank = null;
				this.currentWorkPlace = null;
				this.currentPatientClassify = null;
				lciWorkPlaceNameNew.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
				lciMilitaryRankNew.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
				lciPositionNew.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
				if (lciPatientClassifyNew.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
				{
					currentPatientClassify = dataClassify.FirstOrDefault(o => o.ID == patientDTO.PATIENT_CLASSIFY_ID);
					txtPatientClassify.Text = dataClassify != null && dataClassify.Count > 0 && currentPatientClassify != null ? dataClassify.FirstOrDefault(o => o.ID == patientDTO.PATIENT_CLASSIFY_ID).PATIENT_CLASSIFY_NAME : "";
					ValidateOtherCpn();
				}
				if (lciPositionNew.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
				{
					currentPosition = dataPosition.FirstOrDefault(o => o.ID == patientDTO.POSITION_ID);
					txtPosition.Text = dataPosition != null && dataPosition.Count > 0 && currentPosition != null ? dataPosition.FirstOrDefault(o => o.ID == patientDTO.POSITION_ID).POSITION_NAME : "";
				}
				if (lciMilitaryRankNew.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
				{
					currentMilitaryRank = dataMilitaryRank.FirstOrDefault(o => o.ID == patientDTO.MILITARY_RANK_ID);
					txtMilitaryRank.Text = dataMilitaryRank != null && dataMilitaryRank.Count > 0 && currentMilitaryRank != null ? dataMilitaryRank.FirstOrDefault(o => o.ID == patientDTO.MILITARY_RANK_ID).MILITARY_RANK_NAME : "";
				}
				if (lciWorkPlaceNameNew.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
				{
					currentWorkPlace = dataWorkPlace.FirstOrDefault(o => o.ID == patientDTO.WORK_PLACE_ID);
					txtWorkPlace.Text = dataWorkPlace != null && dataWorkPlace.Count > 0 && currentWorkPlace != null ? dataWorkPlace.FirstOrDefault(o => o.ID == patientDTO.WORK_PLACE_ID).WORK_PLACE_NAME : "";
				}
				if (!string.IsNullOrEmpty(patientDTO.NOTE))
				{
					DevExpress.XtraEditors.XtraMessageBox.Show(patientDTO.NOTE, ResourceMessage.TieuDeCuaSoThongBaoLaThongBao, System.Windows.Forms.MessageBoxButtons.OK);
				}
				if (dlgEnableFindType != null && !string.IsNullOrEmpty(patientDTO.PERSON_CODE))
				{
                    dlgEnableFindType(true);
				}
                if (patientDTO != null && !string.IsNullOrEmpty(patientDTO.PATIENT_CODE))
                    DisableControlOldPatientInformationOption();
                else
                    DisableControlOldPatientInformationOption(true);
            }
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
	}
}
