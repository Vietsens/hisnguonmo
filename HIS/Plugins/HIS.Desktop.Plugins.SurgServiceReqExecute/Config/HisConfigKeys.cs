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

namespace HIS.Desktop.Plugins.SurgServiceReqExecute.Config
{
    internal class HisConfigKeys
    {
        internal const string HIS_CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";//Doi tuong BHYT
        internal const string HIS_CONFIG_KEY__PATIENT_TYPE_CODE__VP = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE";//Doi tuong VP

        internal const string IS_CHECKING_PERMISSON = "MOS.HIS_SERE_SERV_PTTT.IS_CHECKING_PERMISSON";

        internal const string CHECKING_PERMISSON_OPTION = "HIS.Desktop.Plugins.SurgServiceReqExecute.CheckingPermissionOption";

        internal const string ALLOW_FINISH_WHEN_ACCOUNT_IS_DOCTOR = "MOS.HIS_SERVICE_REQ.ALLOW_FINISH_WHEN_ACCOUNT_IS_DOCTOR";
        internal const string HIS_CONFIG_KEY__CHECK_SIMULTANEITY_OPTION = "MOS.HIS_SERVICE_REQ.CHECK_SIMULTANEITY_OPTION";

        internal const string HIS_CONFIG_KEY__ASSIGN_SERVICE_SIMULTANEITY_OPTION = "MOS.HIS_SERVICE_REQ.ASSIGN_SERVICE_SIMULTANEITY_OPTION";

        internal static string CheckPermisson
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(IS_CHECKING_PERMISSON);
            }
        }

        internal static string allowFinishWhenAccountIsDoctor
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ALLOW_FINISH_WHEN_ACCOUNT_IS_DOCTOR);
            }
        }

        internal static string CheckPermissonOption
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CHECKING_PERMISSON_OPTION);
            }
        }

        //#19893
        internal static string IS_ALLOWING_PROCESSING_SUBCLINICAL_AFTER_LOCKING_TREATMENT
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_SERVICE_REQ.IS_ALLOWING_PROCESSING_SUBCLINICAL_AFTER_LOCKING_TREATMENT");
            }
        }

        internal static string CHECK_SIMULTANEITY_OPTION
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HIS_CONFIG_KEY__CHECK_SIMULTANEITY_OPTION);
            }
        }

        internal static string ASSIGN_SERVICE_SIMULTANEITY_OPTION
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HIS_CONFIG_KEY__ASSIGN_SERVICE_SIMULTANEITY_OPTION);
            }
        }
    }
}
