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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.PubSub.Config
{
    class HisConfigCFG
    {       
        private const string CONFIG_KEY__PUB_SUB_SERVER_INFO = "HIS.Desktop.PubSubServerInfo";
        private const string CONFIG_KEY__TIME_CHECK_CONNECTION = "HIS.Desktop.PubSub.TimeCheckConnection";
        internal static CCC_PUBSUB_INFO PUBSUB_INFO;
        internal static int TimeCheckConnection;
        internal static void LoadConfig()
        {
            try
            {
                PUBSUB_INFO = GetPubSubInfo(CONFIG_KEY__PUB_SUB_SERVER_INFO);
                TimeCheckConnection = GetValueNumber(CONFIG_KEY__TIME_CHECK_CONNECTION);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private static CCC_PUBSUB_INFO GetPubSubInfo(string key)
        {
            CCC_PUBSUB_INFO rs = null;
            try
            {
                string value = HisConfigs.Get<string>(key);
                if (String.IsNullOrWhiteSpace(value))
                    throw new Exception(" Value <=0 ");

                List<string> lst = value.Split('|').ToList();
                if (lst.Count >= 0)
                {
                    rs = new CCC_PUBSUB_INFO();
                    rs.Address = lst[0].Trim();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                rs = null;
            }
            return rs;
        }
        private static string GetValueString(string code)
        {
            string result = null;
            try
            {
                return HisConfigs.Get<string>(code);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                result = null;
            }
            return result;
        }
        private static int GetValueNumber(string code)
        {
            int result = 0;
            try
            {
                return HisConfigs.Get<int>(code);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                result = 0;
            }
            return result;
        }
    }
    class CCC_PUBSUB_INFO
    {
        public string Address { get; set; }
    }
}
