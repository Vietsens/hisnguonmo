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
using HIS.UC.UCPatientRaw.ADO;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Logging;
using HIS.Desktop.Utility;
using MOS.SDO;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.RegisterConfig;

namespace HIS.UC.UCPatientRaw
{
	public partial class UCPatientRaw : HIS.Desktop.Utility.UserControlBase
    {
		public UCPatientRawADO GetValue()
		{
			UCPatientRawADO dataGet = new UCPatientRawADO();
			try
			{
				popupControlContainer1.HidePopup();
				if (this.typeCodeFind == ResourceMessage.typeCodeFind__MaBN)
				{
					dataGet.EMPLOYEE_CODE = this.txtPatientCode.Text;
					dataGet.IS_FIND_BY_PATIENT_CODE = 1;
				}
				if (this.typeCodeFind == ResourceMessage.typeCodeFind__MaNV)
				{
					dataGet.PATIENT_ID = patientId;
					dataGet.EMPLOYEE_CODE = this.txtPatientCode.Text;
				}
                if (this.typeCodeFind == ResourceMessage.typeCodeFind__MaCMCC && isReadQrCode)
                {
					dataGet.IsReadQrCccd = true;
                }
                dataGet.PATIENT_NAME = this.txtPatientName.Text;
				if (this.cboCareer.EditValue != null)
				{
					dataGet.CARRER_ID = Inventec.Common.TypeConvert.Parse.ToInt64(this.cboCareer.EditValue.ToString());
				}
				dataGet.CARRER_NAME = this.cboCareer.Text;
				dataGet.CARRER_CODE = this.txtCareerCode.Text;

				if (lciFortxtEthnicCode.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
				{
					//dataGet.ETHNIC_ID = this.cboEthnic.EditValue == null ? null : (long?)Inventec.Common.TypeConvert.Parse.ToInt64(this.cboEthnic.EditValue.ToString());
					dataGet.ETHNIC_NAME = this.cboEthnic.Text;
					dataGet.ETHNIC_CODE = this.txtEthnicCode.Text;
				}

                if (this.dtPatientDob.EditValue != null)
				{
					string dateDob = this.dtPatientDob.DateTime.ToString("yyyyMMdd");
					string timeDob = "00";
					string minute = "00";
					if (this.txtAge.Enabled)
					{
						timeDob = string.Format("{0:00}", DateTime.Now.Hour - Inventec.Common.TypeConvert.Parse.ToInt32(this.txtAge.Text));
						minute = string.Format("{0:00}", DateTime.Now.Minute);
					}
					long dob = Inventec.Common.TypeConvert.Parse.ToInt64(dateDob + timeDob + minute + "00");
					dataGet.DOB = dob;
				}
				else
				{
					DateUtil.DateValidObject dateValidObject = DateUtil.ValidPatientDob(this.txtPatientDob.Text);
					if (dateValidObject != null && dateValidObject.HasNotDayDob)
					{
						this.dtPatientDob.EditValue = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(dateValidObject.OutDate);
						this.dtPatientDob.Update();
					}
				}

				dataGet.PATIENT_CODE = this.txtPatientCode.Text;
				dataGet.GENDER_ID = (this.cboGender.EditValue) == null ? 0 : Inventec.Common.TypeConvert.Parse.ToInt64(this.cboGender.EditValue.ToString());
				dataGet.PATIENTTYPE_ID = this.cboPatientType.EditValue == null ? 0 : Inventec.Common.TypeConvert.Parse.ToInt64(this.cboPatientType.EditValue.ToString());
				string patientName = this.txtPatientName.Text.Trim();
				int idx = patientName.LastIndexOf(" ");
				dataGet.PATIENT_FIRST_NAME = (idx > -1 ? patientName.Substring(idx).Trim() : patientName);
				dataGet.PATIENT_LAST_NAME = (idx > -1 ? patientName.Substring(0, idx).Trim() : "");
				if (this.isNotPatientDayDob)
				{
					dataGet.IS_HAS_NOT_DAY_DOB = 1;
				}
				dataGet.DOB_STR = txtPatientDob.Text;
				if (this.patientTD3 != null)
				{
					dataGet.PERSON_CODE = this.patientTD3.PERSON_CODE;
					dataGet.TREATMENT_TYPE_ID = this.patientTD3.TreatmentTypeId;
				}
				if (HisConfigCFG.IsSetPrimaryPatientType == "2" && cboPrimaryPatientType.EditValue != null)
				{
					dataGet.PRIMARY_PATIENT_TYPE_ID = Convert.ToInt64(cboPrimaryPatientType.EditValue);
				}

				if (currentPatientClassify != null && this.lciPatientClassifyNew.Visible)
				{
					dataGet.PATIENT_CLASSIFY_ID = currentPatientClassify.ID;
				}

				if (currentWorkPlace != null && this.lciWorkPlaceNameNew.Visible)
				{
					dataGet.WORK_PLACE_ID = currentWorkPlace.ID;

				}

				if (currentMilitaryRank != null && this.lciMilitaryRankNew.Visible)
				{
					dataGet.MILITARY_RANK_ID = currentMilitaryRank.ID;
				}

				if (currentPosition != null && this.lciPositionNew.Visible)
				{
					dataGet.POSITION_ID = currentPosition.ID;
				}
				dataGet.lstPreviousDebtTreatments = lstSend;
                dataGet.ReceptionForm = this.typeReceptionForm;
				if(cardSearch!= null)
                {
					dataGet.CardCode = cardSearch.CardCode;
					dataGet.CardServiceCode = cardSearch.ServiceCode;
					dataGet.BankCardCode = cardSearch.BankCardCode;
				}
				if (this.ResultDataADO != null && this.ResultDataADO.ResultHistoryLDO != null)
				{
					dataGet.SocialInsuranceNumberPatient = this.ResultDataADO.ResultHistoryLDO.maSoBHXH;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			return dataGet;
		}

		public UCPatientRawADO GetValueForCheckInfoBhyt()
		{
			UCPatientRawADO dataGet = new UCPatientRawADO();
			try
			{
				dataGet.PATIENT_NAME = this.txtPatientName.Text;
				if (this.dtPatientDob.EditValue != null)
				{
					string dateDob = this.dtPatientDob.DateTime.ToString("yyyyMMdd");
					string timeDob = "00";
					string minute = "00";
					if (this.txtAge.Enabled)
					{
						timeDob = string.Format("{0:00}", DateTime.Now.Hour - Inventec.Common.TypeConvert.Parse.ToInt32(this.txtAge.Text));
						minute = string.Format("{0:00}", DateTime.Now.Minute);
					}
					long dob = Inventec.Common.TypeConvert.Parse.ToInt64(dateDob + timeDob + minute + "00");
					dataGet.DOB = dob;
				}
				else
				{
					DateUtil.DateValidObject dateValidObject = DateUtil.ValidPatientDob(this.txtPatientDob.Text);
					if (dateValidObject != null && dateValidObject.HasNotDayDob)
					{
						this.dtPatientDob.EditValue = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(dateValidObject.OutDate);
						this.dtPatientDob.Update();
					}
				}		
				dataGet.GENDER_ID = (this.cboGender.EditValue) == null ? 0 : Inventec.Common.TypeConvert.Parse.ToInt64(this.cboGender.EditValue.ToString());							}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			return dataGet;
		}

		public bool IsChangeLayout()
		{
			try
			{
				return (this.currentPatientType != null);
			}
			catch { }
			return false;
		}

		public void SetPatientCodeAfterSavePatient(string patientCode)
		{
			try
			{
				if (String.IsNullOrEmpty(this.txtPatientCode.Text) && !String.IsNullOrEmpty(patientCode))
					this.txtPatientCode.Text = patientCode;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		public void SetPatientCodeAfterSavePatient(HisPatientVitaminASDO patient)
		{
			try
			{
				if (String.IsNullOrEmpty(this.txtPatientCode.Text) && !String.IsNullOrEmpty(patient.HisPatient.PATIENT_CODE))
					this.txtPatientCode.Text = patient.HisPatient.PATIENT_CODE;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		public void SetValue(UCPatientRawADO dataSet)
		{
			try
			{
				if (dataSet != null)
				{
					if (!String.IsNullOrEmpty(dataSet.PATIENT_CODE))
						this.txtPatientCode.Text = dataSet.PATIENT_CODE;
					else
						this.txtPatientCode.Text = "";

					this.txtPatientName.Text = dataSet.PATIENT_NAME == null ? dataSet.PATIENT_FIRST_NAME + " " + dataSet.PATIENT_LAST_NAME : dataSet.PATIENT_NAME;
					if (dataSet.DOB > 0 && dataSet.DOB.ToString().Length >= 6)
					{
						if (dataSet.IS_HAS_NOT_DAY_DOB == 1)
							this.LoadNgayThangNamSinhBNToForm(dataSet.DOB, true);
						else
							this.LoadNgayThangNamSinhBNToForm(dataSet.DOB, false);
					}

					MOS.EFMODEL.DataModels.HIS_GENDER gioitinh = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().SingleOrDefault(o => o.ID == dataSet.GENDER_ID);
					if (gioitinh != null)
					{
						this.cboGender.EditValue = gioitinh.ID;
					}

					if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.ChangeEthnic != 0 && lciFortxtEthnicCode.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
					{
						if (!String.IsNullOrEmpty(dataSet.ETHNIC_CODE))
						{
							var ethnicData = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>().FirstOrDefault(o => o.ETHNIC_CODE == dataSet.ETHNIC_CODE);
							this.cboEthnic.EditValue = (ethnicData != null ? ethnicData.ETHNIC_CODE : null);
							this.txtEthnicCode.Text = ethnicData != null ? ethnicData.ETHNIC_CODE : "";
						}
						else
						{
							var ethnicData = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>().FirstOrDefault(o => o.ETHNIC_NAME == dataSet.ETHNIC_NAME);
							this.cboEthnic.EditValue = (ethnicData != null ? ethnicData.ETHNIC_CODE : null);
							this.txtEthnicCode.Text = ethnicData != null ? ethnicData.ETHNIC_CODE : "";
						}
					}

					MOS.EFMODEL.DataModels.HIS_CAREER career = this.GetCareerByBhytWhiteListConfig(dataSet.HEIN_CARD_NUMBER);

					//Khi người dùng nhập thẻ BHYT, nếu đầu mã thẻ là TE1, thì tự động chọn giá trị của trường "Nghề nghiệp" là "Trẻ em dưới 6 tuổi"
					if (career != null)
					{
						this.FillDataCareerUnder6AgeByHeinCardNumber(dataSet.HEIN_CARD_NUMBER);
					}
					else
					{
						career = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).FirstOrDefault(o => o.ID == dataSet.CARRER_ID);
					}
					if (career != null)
					{
						// hiển thị nghề nghiệp theo thẻ (nganl yêu cầu ngày 21/06)
						this.txtCareerCode.Text = career.CAREER_CODE;
                        this.cboCareer.EditValue = career.ID;
                    }
					// nếu là trẻ em và có key cấu hình thì bắt buộc nhập thông tin người nhà.
					bool isTE = MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dataSet.DOB) ?? DateTime.Now);
					if (isTE && HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.MustHaveNCSInfoForChild == true && dlgSetValidation != null)
						this.dlgSetValidation(isTE);
					IsLoadFromSearchTxtCode = true;
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
					if (this.lciPatientClassifyNew.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
					{
						currentPatientClassify = dataClassify.FirstOrDefault(o => o.ID == dataSet.PATIENT_CLASSIFY_ID);
						txtPatientClassify.Text = dataClassify != null && dataClassify.Count > 0 && currentPatientClassify!=null  ? dataClassify.FirstOrDefault(o => o.ID == dataSet.PATIENT_CLASSIFY_ID).PATIENT_CLASSIFY_NAME : "";
						ValidateOtherCpn();
					}
					if (this.lciPositionNew.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
					{
						currentPosition = dataPosition.FirstOrDefault(o => o.ID == dataSet.POSITION_ID);
						txtPosition.Text = dataPosition != null && dataPosition.Count > 0 && currentPosition !=null? dataPosition.FirstOrDefault(o => o.ID == dataSet.POSITION_ID).POSITION_NAME : "";
					}
					if (this.lciMilitaryRankNew.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
					{
						currentMilitaryRank = dataMilitaryRank.FirstOrDefault(o => o.ID == dataSet.MILITARY_RANK_ID);
						txtMilitaryRank.Text = dataMilitaryRank != null && dataMilitaryRank.Count > 0 && currentMilitaryRank!=null? dataMilitaryRank.FirstOrDefault(o => o.ID == dataSet.MILITARY_RANK_ID).MILITARY_RANK_NAME : "";
					}
					if (this.lciWorkPlaceNameNew.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
					{
						currentWorkPlace = dataWorkPlace.FirstOrDefault(o => o.ID == dataSet.WORK_PLACE_ID);
						txtWorkPlace.Text = dataWorkPlace != null && dataWorkPlace.Count > 0 && currentWorkPlace != null ? dataWorkPlace.FirstOrDefault(o => o.ID == dataSet.WORK_PLACE_ID).WORK_PLACE_NAME : "";
					}
					IsLoadFromSearchTxtCode = false;
                    this.typeReceptionForm = dataSet.ReceptionForm;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		public void UpdateValueAfterCheckTT(UCPatientRawADO data)
		{
			try
			{
				if (data != null)
				{
					if (!String.IsNullOrEmpty(data.PATIENT_NAME) && data.PATIENT_NAME != this.txtPatientName.Text)
						this.txtPatientName.Text = data.PATIENT_NAME;

					if (data.DOB > 0)
					{
						this.isNotPatientDayDob = (data.IS_HAS_NOT_DAY_DOB == 1);
						this.LoadNgayThangNamSinhBNToForm(data.DOB, this.isNotPatientDayDob);
					}

					this._UCPatientRawADO = new UCPatientRawADO();
					this._UCPatientRawADO = data;

					if (data.GENDER_ID > 0)
					{
						var gender = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().SingleOrDefault(o => o.ID == (data.GENDER_ID == 1 ? 2 : 1));
						if (gender != null && gender.ID > 0)
						{
							this.cboGender.EditValue = gender.ID;

							this._UCPatientRawADO.GENDER_ID = gender.ID;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		public void SetValuePatientType(long patientTypeId)
		{
			try
			{
				var dataSource = cboPatientType.Properties.DataSource as List<HIS_PATIENT_TYPE>;

                var patientType = dataSource.FirstOrDefault(o => o.ID == patientTypeId);
				if (patientType != null && patientType.ID > 0)
				{
					IsLoadFromSearchTxtCode = true;
					this.txtPatientTypeCode.Text = patientType.PATIENT_TYPE_CODE;
					this.cboPatientType.EditValue = patientType.ID;
					IsLoadFromSearchTxtCode = false;
					Inventec.Common.Logging.LogSystem.Info("Co cau hinh UsingPatientTypeOfPreviousPatient =" + HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.UsingPatientTypeOfPreviousPatient
					   + " va tim duoc doi tuong benh nhan thoa man ==> gan doi tuong benh nhan mac dinh"
						+ Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientType), patientType));
				}
				else
				{
					Inventec.Common.Logging.LogSystem.Warn("Du lieu dau vao khong hop le, khong tim duoc doi tuong benh nhan de gan gia tri mac dinh:"
						+ Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientTypeId), patientTypeId)
						+ Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.UsingPatientTypeOfPreviousPatient), HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.UsingPatientTypeOfPreviousPatient)
						);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void LoadNgayThangNamSinhBNToForm(long dob, bool hasNotDayDob)
		{
			try
			{
				this.isNotPatientDayDob = hasNotDayDob;
				LogSystem.Debug("Bat dau gan du lieu nam sinh benh nhan len form. p1: tinh toan nam sinh");
				if (dob > 0)
				{
					DateTime dtNgSinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dob) ?? DateTime.MinValue;
					bool isGKS = MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(dtNgSinh);
					if (isGKS == true)
					{
						if (!this.TD3)
							this.isEnable(true, null);
					}
					else if (isGKS == false && this.isTemp_QN == true || (this.isGKS == true && this.isTemp_QN == true))
						this.isEnable(null, true);
					if (isGKS == true && (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.MustHaveNCSInfoForChild == true || HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.RelativesInforOption == "1" || HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.RelativesInforOption == "2"))
						this.dlgSetValidation(isGKS);
					this.dtPatientDob.EditValue = dtNgSinh;
                    string currentTxtPatientDob = this.txtPatientDob.Text ?? "";
					if (hasNotDayDob)
					{
						this.txtPatientDob.Text = dtNgSinh.ToString("yyyy");
					}
                    else if ((currentTxtPatientDob.Length == 6 || currentTxtPatientDob.Length == 7) 
                        && (currentTxtPatientDob == dtNgSinh.ToString("MMyyyy") || currentTxtPatientDob == dtNgSinh.ToString("MM/yyyy")))
					{
                        this.txtPatientDob.Text = dtNgSinh.ToString("MM/yyyy");
                    }
                    else
                    {
                        this.txtPatientDob.Text = dtNgSinh.ToString("dd/MM/yyyy");
                    }

					CalculatePatientAge.AgeObject ageObject = CalculatePatientAge.Calculate(dob);
					if (ageObject != null)
					{
						this.txtAge.EditValue = ageObject.OutDate;
						this.cboAge.EditValue = ageObject.AgeType;
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private HIS_CAREER GetCareerByBhytWhiteListConfig(string heinCardNumder)
		{
			HIS_CAREER result = null;
			try
			{
				if (!String.IsNullOrEmpty(heinCardNumder))
				{
					var bhytWhiteList = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BHYT_WHITELIST>().FirstOrDefault(o => !String.IsNullOrEmpty(heinCardNumder)
						&& o.BHYT_WHITELIST_CODE.ToUpper() == heinCardNumder.Substring(0, 3).ToUpper());
					if (bhytWhiteList != null && (bhytWhiteList.CAREER_ID ?? 0) > 0)
					{
						result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().Where(o=>o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).SingleOrDefault(o => o.ID == bhytWhiteList.CAREER_ID.Value);
						if (result == null)
						{
							Inventec.Common.Logging.LogSystem.Warn("GetCareerByBhytWhiteListConfig => Khong lay duoc nghe nghiep theo id = " + bhytWhiteList.CAREER_ID);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
			return result;
		}

		private void FillDataCareerUnder6AgeByHeinCardNumber(string heinCardNumder)
		{
			try
			{
				//Khi người dùng nhập thẻ BHYT, nếu đầu mã thẻ là TE1, thì tự động chọn giá trị của trường "Nghề nghiệp" là "Trẻ em dưới 6 tuổi"
				//27/10/2017 => sửa lại => Căn cứ vào đầu thẻ BHYT và dữ liệu cấu hình trong bảng HIS_BHYT_WHITELIST để tự động điền nghề nghiệp tương ứng
				MOS.EFMODEL.DataModels.HIS_CAREER career = GetCareerByBhytWhiteListConfig(heinCardNumder);
				if (career == null)
				{
					if (this.dtPatientDob.DateTime != DateTime.MinValue)
					{
						if (this.dtPatientDob.DateTime != DateTime.MinValue && MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(this.dtPatientDob.DateTime))
						{
							career = HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.CareerUnder6Age;
						}
						else if (DateTime.Now.Year - this.dtPatientDob.DateTime.Year <= 18)
						{
							career = HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.CareerHS;
						}
						else
						{
							career = HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.CareerBase;
						}
					}
					else
					{
						career = HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.CareerBase;
					}
				}
				if (career != null)
				{
					this.txtCareerCode.Text = career.CAREER_CODE;
                    this.cboCareer.EditValue = career.ID;
                }
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void SelectOnePatientProcess(HisPatientSDO patient)
		{
			try
			{
				if (!AlertTreatmentInOutInDayForTreatmentMessage(patient))
				{
					this.currentPatientSDO = null;
					this.dlgSendPatientSdo(currentPatientSDO);
					//Reset form TODO
					this.dlgResetRegisterForm();
					return;
				}

				LogSystem.Debug("SelectOnePatientProcess => t1. Gan thong tin cua benh nhan len form");
				this.enableLciBenhNhanMoi(true);

				//Nếu thực hiện tìm kiếm theo (họ tên + ngày sinh + giới tính) -> tìm ra BN cũ -> giữ nguyên ngày sinh đang nhập
				//if (this.txtPatientDob.Text.Length == 4)
				//    patient.IS_HAS_NOT_DAY_DOB = 1;
				//else
				//    patient.IS_HAS_NOT_DAY_DOB = null;
				this.txtPatientCode.Focus();
				//this.txtPatientCode.Text = patient.PATIENT_CODE;
				if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.DangKyTiepDonHienThiThongBaoTimDuocBenhNhan == 1)
				{
					DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.TimDuocMotBenhNhanTheoThongTinNguoiDungNhapNeuKhongPhaiBNCuVuiLongNhanNutBNMoi, ResourceMessage.TieuDeCuaSoThongBaoLaThongBao, DevExpress.Utils.DefaultBoolean.True);
				}
				DataResultADO dataResult = new DataResultADO();
				dataResult.HisPatientSDO = patient;
				dataResult.OldPatient = true;
				this.dlgSearchPatient1(dataResult);
				this.FillDataPatientToControl(patient, true);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
	}
}
