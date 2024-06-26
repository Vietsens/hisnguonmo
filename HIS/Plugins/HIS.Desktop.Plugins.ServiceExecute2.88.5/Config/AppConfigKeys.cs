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

namespace HIS.Desktop.Plugins.ServiceExecute.Config
{
    class AppConfigKeys
    {
        internal const string CONFIG_KEY__MODULE_CAMERA__SHORTCUT_KEY = "CONFIG_KEY__MODULE_CAMERA__SHORTCUT_KEY";

        internal const string HIS_DESKTOP_SURGSERVICEREQEXECUTE_EXECUTE_ROLE_DEFAULT = "HIS.DESKTOP.PLUGINS.SURGSERVICEREQEXECUTE.EXECUTE_ROLE_DEFAULT";

        internal const string MOS__HIS_SERVICE_REQ__ALLOW_UPDATE_SURG_INFO_AFTER_LOCKING_TREATMENT = "MOS.HIS_SERVICE_REQ.ALLOW_UPDATE_SURG_INFO_AFTER_LOCKING_TREATMENT";

        internal const string IS_CHECKING_PERMISSON = "MOS.HIS_SERE_SERV_PTTT.IS_CHECKING_PERMISSON";

        internal const string CONFIG_KEY__Camera__IsSavingInLocal = "HIS.Desktop.Plugins.Camera.IsSavingInLocal";

        internal const string CONFIG_KEY__IsRequiredPtttPriority = "HIS.Desktop.Plugins.ServiceExecute.IsRequiredPtttPriority";
        internal const string CONFIG_KEY__IsInitCameraDefault = "HIS.Desktop.Plugins.ServiceExecute.IsInitCameraDefault";

        internal static bool IsInitCameraDefault
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__IsInitCameraDefault) == "1";
            }
        }

        internal static bool IsSavingInLocal
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__Camera__IsSavingInLocal) == "1";
            }
        }

        internal static string CheckPermisson
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(IS_CHECKING_PERMISSON);
            }
        }

        internal static bool IsRequiredPtttPriority
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__IsRequiredPtttPriority) == "1";
            }
        }
    }
}
