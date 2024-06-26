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
using MOS.SDO;
using HIS.UC.PlusInfo.ADO;

namespace HIS.Desktop.Plugins.RegisterVaccination.Run3
{
    public partial class UCRegister : UserControlBase
    {
        private void FillDataIntoUCPlusInfo(HisPatientSDO _currentPatientSDO)
        {
            try
            {
                //HIS.UC.PlusInfo.ADO.PatientInformationADO dataPatient = new UC.PlusInfo.ADO.PatientInformationADO();
                UCPlusInfoADO dataPatient = new UCPlusInfoADO();
                dataPatient.ETHNIC_NAME = _currentPatientSDO.ETHNIC_NAME;
                dataPatient.ETHNIC_CODE = _currentPatientSDO.ETHNIC_CODE;
                dataPatient.MILITARYRANK_ID = _currentPatientSDO.MILITARY_RANK_ID;
                dataPatient.NATIONAL_NAME = _currentPatientSDO.NATIONAL_NAME;
                dataPatient.NATIONAL_CODE = _currentPatientSDO.NATIONAL_CODE;
                dataPatient.PROGRAM_CODE = _currentPatientSDO.PatientProgramCode;
                dataPatient.workPlaceADO = new WorkPlaceADO();
                dataPatient.workPlaceADO.WORK_PLACE = _currentPatientSDO.WORK_PLACE;
                dataPatient.workPlaceADO.WORK_PLACE_ID = _currentPatientSDO.WORK_PLACE_ID;
                dataPatient.PATIENT_ID = _currentPatientSDO.ID;
                dataPatient.EMAIL = _currentPatientSDO.EMAIL;
                dataPatient.PATIENT_STORE_CODE = _currentPatientSDO.PATIENT_STORE_CODE;
                dataPatient.PHONE_NUMBER = _currentPatientSDO.PHONE;
                dataPatient.PROVINCE_OfBIRTH_CODE = _currentPatientSDO.BORN_PROVINCE_CODE;
                dataPatient.PROVINCE_OfBIRTH_NAME = _currentPatientSDO.BORN_PROVINCE_NAME;
                dataPatient.HT_COMMUNE_NAME = _currentPatientSDO.HT_COMMUNE_NAME;
                dataPatient.HT_DISTRICT_NAME = _currentPatientSDO.HT_DISTRICT_NAME;
                dataPatient.HT_PROVINCE_NAME = _currentPatientSDO.HT_PROVINCE_NAME;
                dataPatient.HT_ADDRESS = _currentPatientSDO.HT_ADDRESS;
                dataPatient.BLOOD_ABO_CODE = _currentPatientSDO.BLOOD_ABO_CODE;
                dataPatient.BLOOD_RH_CODE = _currentPatientSDO.BLOOD_RH_CODE;
                dataPatient.MOTHER_NAME = _currentPatientSDO.MOTHER_NAME;
                dataPatient.FATHER_NAME = _currentPatientSDO.FATHER_NAME;
                if (_currentPatientSDO.CMND_DATE != null)
                    dataPatient.CMND_DATE = _currentPatientSDO.CMND_DATE;
                else
                    dataPatient.CMND_DATE = _currentPatientSDO.CCCD_DATE;
                if (!String.IsNullOrEmpty(_currentPatientSDO.CMND_NUMBER))
                    dataPatient.CMND_NUMBER = _currentPatientSDO.CMND_NUMBER;
                else
                    dataPatient.CMND_NUMBER = _currentPatientSDO.CCCD_NUMBER;
                if (!String.IsNullOrEmpty(_currentPatientSDO.CMND_PLACE))
                    dataPatient.CMND_PLACE = _currentPatientSDO.CMND_PLACE;
                else
                    dataPatient.CMND_PLACE = _currentPatientSDO.CCCD_PLACE;
                dataPatient.HOUSEHOLD_CODE = _currentPatientSDO.HOUSEHOLD_CODE;
                dataPatient.HOUSEHOLD_RELATION_NAME = _currentPatientSDO.HOUSEHOLD_RELATION_NAME;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
