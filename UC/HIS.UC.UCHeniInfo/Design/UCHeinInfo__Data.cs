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
using HIS.UC.UCHeniInfo.ADO;
using MOS.SDO;
using HIS.Desktop.Utility;
using HIS.UC.UCHeniInfo.Utils;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using HIS.UC.UCHeniInfo.Data;
using Inventec.Common.Adapter;
using HIS.UC.UCHeniInfo.ControlProcess;
using MOS.EFMODEL.DataModels;

namespace HIS.UC.UCHeniInfo
{
	public partial class UCHeinInfo : UserControlBase
	{
		public HisPatientProfileSDO GetValue()
		{
			HisPatientProfileSDO patientProfileSDO = new HisPatientProfileSDO();
			try
			{
				if (patientProfileSDO.HisPatientTypeAlter == null)
					patientProfileSDO.HisPatientTypeAlter = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER();
				if (chkHasCardTemp.Checked && !string.IsNullOrEmpty(this.txtSoThe.Text) && this.isTempQN == false && HeinUtils.TrimHeinCardNumber(this.txtSoThe.Text).Length == 10)
				{
					this.txtSoThe.Text = "TE1" + CodeProvince + HeinUtils.TrimHeinCardNumber(this.txtSoThe.Text);
				}
				patientProfileSDO.HisPatientTypeAlter.HEIN_CARD_NUMBER = HeinUtils.TrimHeinCardNumber(this.txtSoThe.Text);

				this.dtHeinCardFromTime.EditValue = HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardFromTime.Text);
				if (this.dtHeinCardFromTime.EditValue != null)
					patientProfileSDO.HisPatientTypeAlter.HEIN_CARD_FROM_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(this.dtHeinCardFromTime.DateTime.ToString("yyyyMMdd") + "000000");
				else
					patientProfileSDO.HisPatientTypeAlter.HEIN_CARD_FROM_TIME = null;
				this.ChangeDataByCard = true;
				this.dtHeinCardToTime.EditValue = HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardToTime.Text);
				this.ChangeDataByCard = false;
				if (dtHeinCardToTime.EditValue != null)
					patientProfileSDO.HisPatientTypeAlter.HEIN_CARD_TO_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(this.dtHeinCardToTime.DateTime.ToString("yyyyMMdd") + "000000");
				else
					patientProfileSDO.HisPatientTypeAlter.HEIN_CARD_TO_TIME = null;
				this.dtDu5Nam.EditValue = HeinUtils.ConvertDateStringToSystemDate(this.txtDu5Nam.Text);

				if (this.chkHasCardTemp.Checked)
				{
					if (this.isTempQN == false)
						patientProfileSDO.HisPatientTypeAlter.HAS_BIRTH_CERTIFICATE = MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.TRUE;
					else
						patientProfileSDO.HisPatientTypeAlter.HAS_BIRTH_CERTIFICATE = MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.FALSE;
				}
				else
				{
					patientProfileSDO.HisPatientTypeAlter.HAS_BIRTH_CERTIFICATE = MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.FALSE;
					if (this.dtDu5Nam.EditValue != null)
						patientProfileSDO.HisPatientTypeAlter.JOIN_5_YEAR_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(this.dtDu5Nam.DateTime.ToString("yyyyMMdd") + "000000");
					else
						patientProfileSDO.HisPatientTypeAlter.JOIN_5_YEAR_TIME = null;
				}
				patientProfileSDO.HisPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE = (this.cboHeinRightRoute.EditValue ?? "").ToString();
				if (patientProfileSDO.HisPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)
				{
					string mediOrgCode = (this.cboDKKCBBD.EditValue ?? "").ToString();
					if (!string.IsNullOrEmpty(mediOrgCode) && (HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.MEDI_ORG_VALUE__CURRENT == mediOrgCode
						|| (!String.IsNullOrWhiteSpace(HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.SYS_MEDI_ORG_CODE) && HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.SYS_MEDI_ORG_CODE.Contains(mediOrgCode))
						|| this.IsMediOrgRightRouteByCurrent(mediOrgCode)))
					{
						patientProfileSDO.HisPatientTypeAlter.RIGHT_ROUTE_CODE = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE;
						this.cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE;
					}
					else if (MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.DISTRICT == HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT
			  || MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.COMMUNE == HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT)
					{
						patientProfileSDO.HisPatientTypeAlter.RIGHT_ROUTE_CODE = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE;
					}
					else
						patientProfileSDO.HisPatientTypeAlter.RIGHT_ROUTE_CODE = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE;
					patientProfileSDO.HisPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE = null;
				}
				else if (patientProfileSDO.HisPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
				{
					patientProfileSDO.HisPatientTypeAlter.RIGHT_ROUTE_CODE = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE;
					patientProfileSDO.HisPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE = null;
				}
				else
					patientProfileSDO.HisPatientTypeAlter.RIGHT_ROUTE_CODE = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE;
				patientProfileSDO.HisPatientTypeAlter.JOIN_5_YEAR = (string)(this.chkJoin5Year.Checked == true ? MOS.LibraryHein.Bhyt.HeinJoin5Year.HeinJoin5YearCode.TRUE : MOS.LibraryHein.Bhyt.HeinJoin5Year.HeinJoin5YearCode.FALSE);
				patientProfileSDO.HisPatientTypeAlter.PAID_6_MONTH = (string)(this.chkPaid6Month.Checked == true ? MOS.LibraryHein.Bhyt.HeinPaid6Month.HeinPaid6MonthCode.TRUE : MOS.LibraryHein.Bhyt.HeinPaid6Month.HeinPaid6MonthCode.FALSE);
				patientProfileSDO.HisPatientTypeAlter.HEIN_MEDI_ORG_CODE = (this.cboDKKCBBD.EditValue ?? "").ToString();
				patientProfileSDO.HisPatientTypeAlter.HEIN_MEDI_ORG_NAME = this.cboDKKCBBD.Text;
				patientProfileSDO.HisPatientTypeAlter.LEVEL_CODE = HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT;
				patientProfileSDO.HisPatientTypeAlter.TREATMENT_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM;
				patientProfileSDO.HisPatientTypeAlter.LIVE_AREA_CODE = (this.cboNoiSong.EditValue ?? "").ToString();
				patientProfileSDO.HisPatientTypeAlter.IS_NEWBORN = (short)(this.chkSs.Checked ? 1 : 0);
				if (isTempQN && this.chkHasCardTemp.Checked)
					patientProfileSDO.HisPatientTypeAlter.IS_TEMP_QN = 1;
				else
					patientProfileSDO.HisPatientTypeAlter.IS_TEMP_QN = null;
				//Xử lý luôn luôn fix là đúng tuyến với các trường hợp cơ sở kcbbd là đúng tuyến/thông tuyến/tuyến dưới
				this.ProcessCaseWrongRoute(patientProfileSDO.HisPatientTypeAlter.HEIN_MEDI_ORG_CODE, patientProfileSDO.HisPatientTypeAlter.LIVE_AREA_CODE);
				string inputDate = this.txtFreeCoPainTime.Text.Trim();
				if (inputDate.Length == 8)
					inputDate = inputDate.Substring(0, 2) + "/" + inputDate.Substring(2, 2) + "/" + inputDate.Substring(4, 4);
				this.dtFreeCoPainTime.EditValue = HeinUtils.ConvertDateStringToSystemDate(inputDate);
				if (!String.IsNullOrEmpty(inputDate) && this.dtFreeCoPainTime.EditValue != null)
				{
					this.txtFreeCoPainTime.Text = inputDate;
					patientProfileSDO.HisPatientTypeAlter.FREE_CO_PAID_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(this.dtFreeCoPainTime.DateTime.ToString("yyyyMMdd"));
				}
				else
					patientProfileSDO.HisPatientTypeAlter.FREE_CO_PAID_TIME = null;
				patientProfileSDO.HisPatientTypeAlter.ADDRESS = this.txtAddress.Text.Trim();
				patientProfileSDO.HisPatientTypeAlter.IS_NO_CHECK_EXPIRE = ((this.lciKhongKTHSD.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && this.chkKhongKTHSD.Checked) ? (short?)1 : null);

				patientProfileSDO.HisPatientTypeAlter.HAS_WORKING_LETTER = this.lciHasWorkingLetter.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && this.chkHasWorkingLetter.Checked ? (short?)1 : null;
				patientProfileSDO.HisPatientTypeAlter.HAS_ABSENT_LETTER = this.lciHasAbsentLetter.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && this.chkHasAbsentLetter.Checked ? (short?)1 : null;
				patientProfileSDO.HisPatientTypeAlter.IS_TT46 = this.lciIsTt46.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && this.chkIsTt46.Checked ? (short?)1 : null;
				patientProfileSDO.HisPatientTypeAlter.TT46_NOTE = this.lciNote.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always ? this.txtNote.Text : null;

				//có thông tin thẻ bhyt thì trả ra trạng thái
				if (this.lciIsBhytHolded.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && (this.chkHasCardTemp.Checked || !string.IsNullOrWhiteSpace(patientProfileSDO.HisPatientTypeAlter.HEIN_CARD_NUMBER)))
				{
					if (patientProfileSDO.HisTreatment == null)
					{
						patientProfileSDO.HisTreatment = new HIS_TREATMENT();
					}

					patientProfileSDO.HisTreatment.IS_BHYT_HOLDED = this.chkIsBhytHolded.Checked ? (short?)1 : null;
				}

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			return patientProfileSDO;
		}
		/// <summary>
		/// hàm lấy thông tin thẻ. tránh trường hợp khi thay đổi control ở patient raw thì check lại thẻ
		/// </summary>
		/// <returns></returns>
		public HisPatientProfileSDO GetValuePatientTypeAlter()
        {
			HisPatientProfileSDO patientProfileSDO = new HisPatientProfileSDO();
			try
            {
				if (patientProfileSDO.HisPatientTypeAlter == null)
					patientProfileSDO.HisPatientTypeAlter = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER();
				if (chkHasCardTemp.Checked && !string.IsNullOrEmpty(this.txtSoThe.Text) && this.isTempQN == false && HeinUtils.TrimHeinCardNumber(this.txtSoThe.Text).Length == 10)
				{
					this.txtSoThe.Text = "TE1" + CodeProvince + HeinUtils.TrimHeinCardNumber(this.txtSoThe.Text);
				}
				patientProfileSDO.HisPatientTypeAlter.HEIN_CARD_NUMBER = HeinUtils.TrimHeinCardNumber(this.txtSoThe.Text);


			}
			catch (Exception ex)
            {

				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			return patientProfileSDO;
		}

		public void SetValue(HisPatientSDO patientsdo)
		{
			try
			{
				if (patientsdo == null || patientsdo.ID == 0) throw new ArgumentNullException("Du lieu dau vao khong hop le => patientsdo is null");

				this.isPatientOld = true;

				this.dxErrorProviderControl.ClearErrors();//xuandv clear error

				this.currentPatientSdo = patientsdo;

				CommonParam param = new CommonParam();
				MOS.Filter.HisPatientTypeAlterFilter patientTypeAlterFilter = new MOS.Filter.HisPatientTypeAlterFilter();
				patientTypeAlterFilter.TDL_PATIENT_ID = patientsdo.ID;
				patientTypeAlterFilter.PATIENT_TYPE_ID = HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT;
				List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER> patyAlters = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER>>(RequestUriStore.HIS_PATIENT_TYPE_ALTER__GET, ApiConsumers.MosConsumer, patientTypeAlterFilter, param);

				//Nếu không có dữ liệu thẻ bhytn theo bệnh nhân => không xử lý gì
				if (patyAlters == null || patyAlters.Count == 0) throw new ArgumentNullException("Khong tim thay du lieu HIS_PATIENT_TYPE_ALTER theo benh nhan co PatientId = " + patientsdo.ID + " => patyAlters is null");

				patyAlters = patyAlters.OrderByDescending(o => o.LOG_TIME).ToList();
				var patientTypeAlters = patyAlters.GroupBy(it => new { it.HEIN_CARD_NUMBER, it.HEIN_CARD_FROM_TIME, it.HEIN_CARD_TO_TIME, it.HEIN_MEDI_ORG_CODE, it.JOIN_5_YEAR, it.PAID_6_MONTH, it.RIGHT_ROUTE_CODE, it.RIGHT_ROUTE_TYPE_CODE }).Select(group => group.First()).Distinct().ToList();

				if (patientTypeAlters != null)
					patientTypeAlters = patientTypeAlters.OrderByDescending(o => o.LOG_TIME).ToList();

				//Load lại dữ liệu combo chọn thẻ bhyt
				HeinCardProcess.LoadDataToCombo(patientTypeAlters, cboSoThe);

				//Nếu dữ liệu bệnh nhân truyền vào có thông tin số thẻ bhyt thì lọc tiếp theo số thẻ bhyt
				if (!String.IsNullOrEmpty(patientsdo.HeinCardNumber))
					patientTypeAlters = patientTypeAlters.Where(o => o.HEIN_CARD_NUMBER == patientsdo.HeinCardNumber).ToList();
				if (!String.IsNullOrEmpty(patientsdo.AppointmentCode))
					patientTypeAlters = patientTypeAlters.Where(o => o.HEIN_MEDI_ORG_CODE == patientsdo.HeinMediOrgCode
						&& o.JOIN_5_YEAR == patientsdo.Join5Year
						&& o.PAID_6_MONTH == patientsdo.Paid6Month
						&& o.RIGHT_ROUTE_CODE == patientsdo.RightRouteCode
						&& o.RIGHT_ROUTE_TYPE_CODE == patientsdo.RightRouteTypeCode).ToList();

				//Nếu dữ liệu thẻ bhyt đã có thẻ bhyt, hiển thị mặc định thẻ gần nhất của BN => fill dữ liệu thẻ vào form
				if (patientTypeAlters != null && patientTypeAlters.Count > 0)
				{
					if (patientTypeAlters.Count > 1)
						patientTypeAlters = patientTypeAlters.OrderByDescending(o => o.LOG_TIME).ToList();
					this.cboSoThe.EditValue = patientTypeAlters[0].ID;
					this.HeinCardSelectRowHandler(patientTypeAlters[0]);
					Inventec.Common.Logging.LogSystem.Debug("FillDataToHeinInsuranceInfoByOldPatient => Benh nhan co the bhyt, tu dong lay the gan nhat (so the: " + patientTypeAlters[0].HEIN_CARD_NUMBER + ")");
				}
				else
				{
					Inventec.Common.Logging.LogSystem.Debug("FillDataToHeinInsuranceInfoByOldPatient => Khong tim thay thong tin the bhyt cua BN, PatientId = " + patientsdo.ID);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		public void SetValueByPatientTypeAlter(HIS_PATIENT_TYPE_ALTER hispatientTypeAlter)
		{
			try
			{
				this.cboSoThe.EditValue = hispatientTypeAlter.HEIN_CARD_NUMBER;
				this.HeinCardSelectRowHandler(hispatientTypeAlter);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn("Tiep don: UCHeinInfo/SetValueByPatientTypeAlter:\n" + ex);
			}
		}

		/// <summary>
		/// Fill dữ liệu thẻ bhyt & thông tin chuyển tuyến vào form
		/// </summary>
		/// <param name="patientTypeAlterBHYT"></param>
		private void HeinCardSelectRowHandler(MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER patientTypeAlter)
		{
			try
			{
				if (patientTypeAlter != null)
				{
					this.ChangeDataHeinInsuranceInfoByPatientTypeAlter(patientTypeAlter);
					//if (this.updateTranPatiDataByPatientOld != null)
					//    this.updateTranPatiDataByPatientOld(patientTypeAlter);
				}
				else
				{
					this.dtHeinCardFromTime.Focus();
					this.dtHeinCardFromTime.SelectAll();
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		public bool HeinRightRouteTypeIsPresent()
		{
			bool isPresent = false;
			try
			{
				isPresent = (this.cboHeinRightRoute.EditValue.ToString() == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			return isPresent;
		}

		public bool HeinRightRouteTypeIsPresentAndAppointment()
		{
			bool isPresent = false;
			try
			{
				isPresent = (this.cboHeinRightRoute.EditValue.ToString() == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.APPOINTMENT);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
			return isPresent;
		}

	}
}
