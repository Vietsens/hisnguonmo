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
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.BhxhApiSend.Config
{
    public class BhxhApiSendCFG
    {
        private const string CONFIG_KEY_BHXH_ADDRESS = "HIS.BHXH_API_SEND.ADDRESS";
        private const string CONFIG_KEY_BHXH_USER_PASS = "HIS.BHXH_API_SEND.USER_PASS";
        private const string CONFIG_KEY_BHXH_MA_TINH = "HIS.BHXH_API_SEND.MA_TINH";
        private const string CONFIG_KEY_BHXH_MA_CSKCB = "HIS.BHXH_API_SEND.MA_CSKCB";

        public static string ADDRESS;
        public static string USERNAME;
        public static string PASSWORD;
        public static string MA_TINH;
        public static string MA_CSKCB;

        public static void LoadConfig()
        {
            try
            {
                ADDRESS = GetValue(CONFIG_KEY_BHXH_ADDRESS);
                if (!string.IsNullOrEmpty(ADDRESS))
                {
                    ADDRESS = ADDRESS.Trim();
                }

                string userPass = GetValue(CONFIG_KEY_BHXH_USER_PASS);
                USERNAME = Get(userPass, 0);
                PASSWORD = Get(userPass, 1);

                MA_TINH = GetValue(CONFIG_KEY_BHXH_MA_TINH);
                if (!string.IsNullOrEmpty(MA_TINH))
                {
                    MA_TINH = MA_TINH.Trim();
                }

                MA_CSKCB = GetValue(CONFIG_KEY_BHXH_MA_CSKCB);
                if (!string.IsNullOrEmpty(MA_CSKCB))
                {
                    MA_CSKCB = MA_CSKCB.Trim();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private static string GetValue(string code)
        {
            string result = null;
            try
            {
                return HisConfigs.Get<string>(code);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        private static string Get(string value, int index)
        {
            string user = "";
            try
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var data = value.Split(':');
                    if (data != null && data.Length > index)
                    {
                        user = data[index].Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                user = "";
            }
            return user;
        }
    }
}
