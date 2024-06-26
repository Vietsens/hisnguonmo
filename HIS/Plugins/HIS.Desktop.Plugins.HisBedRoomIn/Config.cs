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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisBedRoomIn
{
    class Config
    {
        private const string SHOW_PRIMARY_PATIENT_TYPE = "MOS.HIS_SERE_SERV.IS_SET_PRIMARY_PATIENT_TYPE";
        private const string HIS_CONFIG__PATIENT_TYPE_CODE__BHYT = "HIS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";//Doi tuong BHYT
        private const string HIS_CONFIG__PATIENT_TYPE_CODE__VP = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE";
        private const string USING_BED_TEMP = "MOS.HIS_SERE_SERV.IS_USING_BED_TEMP";

        public static string IsPrimaryPatientType
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SHOW_PRIMARY_PATIENT_TYPE);
            }
        }

        public static string IsUsingBedTemp
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(USING_BED_TEMP);
            }
        }

        public static long PatientTypeId__BHYT
        {
            get
            {
                var patientType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HIS_CONFIG__PATIENT_TYPE_CODE__BHYT));
                if (patientType != null)
                {
                    return patientType.ID;
                }
                else
                    return -1;
            }
        }

        public static long PatientTypeId__VP
        {
            get
            {
                var patientType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HIS_CONFIG__PATIENT_TYPE_CODE__VP));
                if (patientType != null)
                {
                    return patientType.ID;
                }
                else
                    return -1;
            }
        }
    }
}
