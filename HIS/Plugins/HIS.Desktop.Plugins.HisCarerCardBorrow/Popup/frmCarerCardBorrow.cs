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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisCarerCardBorrow.Popup
{
	public partial class frmCarerCardBorrow : Form
	{
		private V_HIS_CARER_CARD_BORROW currentCarerCard { get; set; }
		public frmCarerCardBorrow(V_HIS_CARER_CARD_BORROW obj)
		{
			InitializeComponent();
			try
			{
				currentCarerCard = obj;
				string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
				this.Icon = Icon.ExtractAssociatedIcon(iconPath);
			}
			catch (Exception ex)
			{

				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void frmCarerCardBorrow_Load(object sender, EventArgs e)
		{
			try
			{
				if(currentCarerCard!=null)
				{
					lblTreatmentCode.Text = currentCarerCard.TREATMENT_CODE;
					lblPatientName.Text = currentCarerCard.TDL_PATIENT_NAME;
					lblPatientGenderName.Text = currentCarerCard.TDL_PATIENT_GENDER_NAME;
					if (currentCarerCard.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
						lblPatientDob.Text = currentCarerCard.TDL_PATIENT_DOB.ToString().Substring(0, 4);
					else
						lblPatientDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentCarerCard.TDL_PATIENT_DOB);
					lblPatientAddresss.Text = currentCarerCard.TDL_PATIENT_ADDRESS;

					if (!string.IsNullOrEmpty(currentCarerCard.TDL_PATIENT_CMND_NUMBER))
						lblPatientCCCD.Text = currentCarerCard.TDL_PATIENT_CMND_NUMBER;
					else if(!string.IsNullOrEmpty(currentCarerCard.TDL_PATIENT_CCCD_NUMBER))
						lblPatientCCCD.Text = currentCarerCard.TDL_PATIENT_CCCD_NUMBER;
					else if (!string.IsNullOrEmpty(currentCarerCard.TDL_PATIENT_CCCD_NUMBER))
						lblPatientCCCD.Text = currentCarerCard.TDL_PATIENT_CCCD_NUMBER;

					if (!string.IsNullOrEmpty(currentCarerCard.TDL_PATIENT_MOBILE))
						lblPatientPhone.Text = currentCarerCard.TDL_PATIENT_MOBILE;
					else if (!string.IsNullOrEmpty(currentCarerCard.TDL_PATIENT_PHONE))
						lblPatientPhone.Text = currentCarerCard.TDL_PATIENT_PHONE;

					lblPatientEthenic.Text = currentCarerCard.TDL_PATIENT_ETHNIC_NAME;
					lblPatientCarrer.Text = currentCarerCard.TDL_PATIENT_CAREER_NAME;

					if (!string.IsNullOrEmpty(currentCarerCard.TDL_PATIENT_WORK_PLACE))
						lblPatientWorkplace.Text = currentCarerCard.TDL_PATIENT_WORK_PLACE;
					else if (!string.IsNullOrEmpty(currentCarerCard.TDL_PATIENT_WORK_PLACE_NAME))
						lblPatientWorkplace.Text = currentCarerCard.TDL_PATIENT_WORK_PLACE_NAME;

					lblPatientPosition.Text = currentCarerCard.POSITION_NAME;
					lblPatientMilitary.Text = currentCarerCard.TDL_PATIENT_MILITARY_RANK_NAME;
					lblPatientType.Text = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && currentCarerCard.TDL_PATIENT_TYPE_ID  == o.ID).First().PATIENT_TYPE_NAME;

					lblPatientHeinCard.Text = currentCarerCard.TDL_HEIN_CARD_NUMBER;
					lblPatientClassify.Text = currentCarerCard.PATIENT_CLASSIFY_NAME;
					lblBorrowTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(currentCarerCard.BORROW_TIME);
					lblPatientGiveBack.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(currentCarerCard.GIVE_BACK_TIME ?? 0);
					lblGivingUserr.Text = currentCarerCard.GIVING_LOGINNAME + " - "+ currentCarerCard.GIVING_USERNAME;
					lblReceivingUser.Text = currentCarerCard.RECEIVING_LOGINNAME  + " - "+ currentCarerCard.RECEIVING_USERNAME; 
					chkIsLost.Checked = currentCarerCard.IS_LOST == 1;
					chkIsLost.Enabled = false;

				}	
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
	}
}
