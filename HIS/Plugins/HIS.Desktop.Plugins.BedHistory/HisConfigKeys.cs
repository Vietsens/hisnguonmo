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

namespace HIS.Desktop.Plugins.BedHistory
{
    internal class HisConfigKeys
    {
        internal const string HIS_CONFIG__PATIENT_TYPE_CODE__BHYT = "HIS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";//Doi tuong BHYT

        internal const string HIS_CONFIG__PATIENT_TYPE_CODE__VP = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE";

        internal const string CONFIG_KEY__MOS_HIS_SERE_SERV_IS_SET_PRIMARY_PATIENT_TYPE = "MOS.HIS_SERE_SERV.IS_SET_PRIMARY_PATIENT_TYPE";

        internal static string BedServiceType_NotAllow_For_OutPatient = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.AssignService.BedServiceType_NotAllow_For_OutPatient");

        internal const string CONFIG_KEY__BHYT__EXCEED_DAY_ALLOW_FOR_IN_PATIENT = "MOS.BHYT.EXCEED_DAY_ALLOW_FOR_IN_PATIENT";
        internal const string CONFIG_KEY__MOS__HIS_SERE_SERV__IS__USING_BED_TEMP = "MOS.HIS_SERE_SERV.IS_USING_BED_TEMP";
        internal const string CONFIG_KEY__WarningOverTotalPatientPrice__IsCheck = "HIS.Desktop.WarningOverTotalPatientPrice__IsCheck";
        internal const string CONFIG_KEY__IsShareBedFeeOffAllPatients = "HIS.Desktop.Plugins.BedHistory.IsShareBedFeeOffAllPatients";
    }
}
