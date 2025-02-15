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

namespace HIS.Desktop.Plugins.HisTrackingList.Config
{
    class HisConfigCFG
    {
        internal static string Config_TrackingCreate_BloodPresOption
        {
            get
            {
                var result = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKING_CREATE_BLOOD_PRES_OPTION);
                return result;
            }
        }

        internal static string Config_TrackingList_CheckServiceReqWhenDeleteTracking
        {
            get
            {
                var result = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyss.DB_CODE__HIS_DESKTOP_PLUGINS_HIS_TRACKING_LIST_CHECK_SERVICE_REQ_WHEN_DELETE_TRACKING);
                return result;
            }
        }
    }
}
