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
using MOS.SDO;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.RegisterVaccination.Run3
{
    public partial class UCRegister : UserControlBase
    {
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
                //patientByCard.CMND_DATE = cardSDO.CmndDate;
                //patientByCard.CMND_NUMBER = cardSDO.CmndNumber;
                //patientByCard.CMND_PLACE = cardSDO.CmndPlace;
                patientByCard.COMMUNE_NAME = cardSDO.CommuneName;
                patientByCard.DISTRICT_NAME = cardSDO.DistrictName;
                patientByCard.PROVINCE_NAME = cardSDO.ProvinceName;
                patientByCard.DOB = cardSDO.Dob;
                patientByCard.EMAIL = cardSDO.Email;
                patientByCard.ETHNIC_NAME = cardSDO.EthnicName;
                if (cardSDO.Dob > 0 && cardSDO.Dob.ToString().Length == 4)
                    patientByCard.IS_HAS_NOT_DAY_DOB = 1;
                else
                    patientByCard.IS_HAS_NOT_DAY_DOB = 0;
                patientByCard.PHONE = cardSDO.Phone;
                //patientByCard.RECENT_ROOM_ID = cardSDO.ReligionName;//TODO
                //patientByCard.RECENT_SERVICE_ID = cardSDO.Address;//TODO
                patientByCard.RELIGION_NAME = cardSDO.ReligionName;
                patientByCard.VIR_ADDRESS = cardSDO.VirAddress;
                patientByCard.VIR_PATIENT_NAME = patientByCard.LAST_NAME + " " + patientByCard.FIRST_NAME;
                //patientByCard.GENDER_CODE = cardSDO.GenderCode;
                //patientByCard.GENDER_NAME = cardSDO.GenderName;
                //var geneder = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().SingleOrDefault(o => o.ID == cardSDO.GenderId);
                patientByCard.GENDER_ID = cardSDO.GenderId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToHeinCardControlByCardSDO(HisCardSDO cardSDO)
        {
            try
            {
                if (!String.IsNullOrEmpty(cardSDO.HeinCardNumber))
                {
                    if (new MOS.LibraryHein.Bhyt.BhytHeinProcessor().IsValidHeinCardNumber(cardSDO.HeinCardNumber))
                    {
                        HIS_PATIENT_TYPE_ALTER patientTypeALter = new HIS_PATIENT_TYPE_ALTER();
                        patientTypeALter.HEIN_CARD_NUMBER = cardSDO.HeinCardNumber;
                        patientTypeALter.HEIN_CARD_FROM_TIME = cardSDO.HeinCardFromTime;
                        patientTypeALter.HEIN_CARD_TO_TIME = cardSDO.HeinCardToTime;
                        patientTypeALter.HEIN_MEDI_ORG_CODE = cardSDO.HeinOrgCode;
                        patientTypeALter.HEIN_MEDI_ORG_NAME = cardSDO.HeinOrgName;
                        patientTypeALter.ADDRESS = cardSDO.HeinAddress;
                        patientTypeALter.JOIN_5_YEAR = cardSDO.Join5Year;
                        patientTypeALter.PAID_6_MONTH = cardSDO.Paid6Month;
                        patientTypeALter.LEVEL_CODE = cardSDO.LevelCode;
                        patientTypeALter.LIVE_AREA_CODE = cardSDO.LiveAreaCode;
                        patientTypeALter.RIGHT_ROUTE_CODE = cardSDO.RightRouteCode;
                        //if (this.mainHeinProcessor != null && ucHeinBHYT != null)
                        //    this.mainHeinProcessor.FillDataHeinInsuranceInfoByPatientTypeAlter(this.ucHeinBHYT, patientTypeALter);

                        // Gọi delegate sang ucHein
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("So the bhyt (tu du lieu tra ve khi quet the thong minh vao dau doc) khong hop le. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => cardSDO), cardSDO));
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
