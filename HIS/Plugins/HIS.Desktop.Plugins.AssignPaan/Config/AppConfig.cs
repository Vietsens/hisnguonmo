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

namespace HIS.Desktop.Plugins.AssignPaan.Config
{
    internal class AppConfig
    {
        private const string Key__ShowRequestUser = "HIS.Desktop.Plugins.AssignConfig.ShowRequestUser";
        private const string CONFIG_KEY__IS_DEFAULT_TRACKING = "HIS.Desktop.Plugins.AssignPrescription.IsDefaultTracking";
        private const string CONFIG_KEY__OBLIGATE_ICD = "EXE.ASSIGN_SERVICE_REQUEST__OBLIGATE_ICD";
        internal const string CONFIG_KEY__IS_TRACKING_REQUIRED = "MOS.HIS_SERVICE_REQ.ASSIGN_SERVICES.IS_TRACKING_REQUIRED";

        private const string CONFIG_KEY__INTEGRATION_VERSION = "MOS.LIS.INTEGRATION_VERSION";
        internal const string CONFIG_KEY__INTEGRATE_OPTION = "MOS.LIS.INTEGRATE_OPTION";
        internal const string CONFIG_KEY__INTEGRATION_TYPE = "MOS.LIS.INTEGRATION_TYPE";
        /// <summary>
        /// Cấu hình để ẩn/hiện trường người chỉ định tai form chỉ định, kê đơn
        //- Giá trị mặc định (hoặc ko có cấu hình này) sẽ ẩn
        //- Nếu có cấu hình, đặt là 1 thì sẽ hiển thị
        /// </summary>
        private static string showRequestUser;
        public static string ShowRequestUser
        {
            get
            {
                showRequestUser = GetValue(Key__ShowRequestUser);
                return showRequestUser;
            }
            set
            {
                showRequestUser = value;
            }
        }

        private static string obligateIcd;
        public static string ObligateIcd
        {
            get
            {
                obligateIcd = GetValue(CONFIG_KEY__OBLIGATE_ICD);
                return obligateIcd;
            }
            set
            {
                obligateIcd = value;
            }
        }

        private static string isDefaultTracking;
        public static string IsDefaultTracking
        {
            get
            {
                isDefaultTracking = GetValue(CONFIG_KEY__IS_DEFAULT_TRACKING);
                return isDefaultTracking;
            }
            set
            {
                isDefaultTracking = value;
            }
        }

        private static string GetValue(string code)
        {
            string result = null;
            try
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(code) ?? "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
        internal static string IntegrationVersionValue
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__INTEGRATION_VERSION);
            }
        }
        internal static string IntegrationOptionValue
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__INTEGRATE_OPTION);
            }
        }
        internal static string IntegrationTypeValue
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__INTEGRATION_TYPE);
            }
        }

        internal static bool IsRequiredTracking
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__IS_TRACKING_REQUIRED) == "1";
            }
        }
    }
}
