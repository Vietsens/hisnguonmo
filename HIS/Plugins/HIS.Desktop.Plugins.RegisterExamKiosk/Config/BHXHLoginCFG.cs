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
using Inventec.Common.LocalStorage.SdaConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.RegisterExamKiosk
{
    class BHXHLoginCFG
    {
        private const string CONFIG_KEY = "HIS.CHECK_HEIN_CARD.BHXH.LOGIN.USER_PASS";
        private const string CONFIG_KEY_ADDRESS = "HIS.CHECK_HEIN_CARD.BHXH__ADDRESS";
        private const string CONFIG_KEY_ADDRESS_OPTION = "HIS.CHECK_HEIN_CARD.BHXH__ADDRESS__OPTION";
        private static string username;
        public static string USERNAME
        {
            get
            {
                if (String.IsNullOrEmpty(username))
                {
                    username = Get(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY), 0);
                }
                return username;
            }
        }

        private static string password;
        public static string PASSWORD
        {
            get
            {
                if (String.IsNullOrEmpty(password))
                {
                    password = Get(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY), 1);
                }
                return password;
            }
        }

        private static string address;
        public static string ADDRESS
        {
            get
            {
                if (String.IsNullOrEmpty(address))
                {
                    address = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY_ADDRESS);
                }
                return address;
            }
        }
        private static long address_option;
        public static long ADDRESS_OPTION
        {
            get
            {
                address_option = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>(CONFIG_KEY_ADDRESS_OPTION);
                return address_option;
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
                        user = data[index];
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
