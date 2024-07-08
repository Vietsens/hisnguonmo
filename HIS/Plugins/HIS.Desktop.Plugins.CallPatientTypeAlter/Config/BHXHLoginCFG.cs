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
using HIS.Desktop.LocalStorage.HisConfig;

namespace HIS.Desktop.Plugins.CallPatientTypeAlter.Config
{
    class BHXHLoginCFG
    {
        private const string CONFIG_KEY = "HIS.CHECK_HEIN_CARD.BHXH.LOGIN.USER_PASS";
        private const string CONFIG_KEY_ADDRESS = "HIS.CHECK_HEIN_CARD.BHXH__ADDRESS";

        public static string USERNAME;
        public static string PASSWORD;
        public static string ADDRESS;

        public static void LoadConfig()
        {
            try
            {
                USERNAME = Get(HisConfigs.Get<string>(CONFIG_KEY), 0);
                PASSWORD = Get(HisConfigs.Get<string>(CONFIG_KEY), 1);
                ADDRESS = HisConfigs.Get<string>(CONFIG_KEY_ADDRESS).Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static string Get(string value, int index)
        {
            string user = "";
            try
            {
                if (!String.IsNullOrEmpty(value))
                {
                    var data = value.Split(':');
                    if (data != null && data.Length >= index)
                    {
                        user = data[index].Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                user = "";
            }
            return user;
        }
    }
}
