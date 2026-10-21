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
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.UC.Login.Base;
using SDA.SDO;
using System;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TestDeviceSample.Worker
{
    /// <summary>
    /// Copy of HIS.Desktop.Plugins.EmpUser.SdaEventLogCreate: writes one line to the SDA event log.
    /// </summary>
    internal class SdaEventLogCreate : BusinessBase
    {
        internal const string URI_SDA_EVENT_LOG_CREATE = "/api/SdaEventLog/Create";

        public SdaEventLogCreate()
            : base()
        {
        }

        public void Create(string loginName, long? eventLogTypeId, bool? isSuccess, string message)
        {
            try
            {
                SdaEventLogSDO data = new SdaEventLogSDO();
                data.EventLogTypeId = eventLogTypeId;
                data.IsSuccess = isSuccess;
                data.Description = message;
                data.Ip = GetIpLocal();
                data.LogginName = loginName;
                data.EventTime = Inventec.Common.DateTime.Get.Now();
                data.AppCode = GlobalVariables.APPLICATION_CODE;
                new BackendAdapter(param).Post<bool>(URI_SDA_EVENT_LOG_CREATE, HIS.Desktop.ApiConsumer.ApiConsumers.SdaConsumer, data, param);
            }
            catch (Exception ex)
            {
                // Event logging is secondary: never break the scan flow because of it
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetIpLocal()
        {
            string ip = "";
            try
            {
                System.Net.IPAddress[] localIPs = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName());
                if (localIPs != null)
                {
                    foreach (System.Net.IPAddress item in localIPs)
                    {
                        if (item.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            ip = item.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return ip;
        }
    }

    /// <summary>
    /// Writes the port login audit trail without blocking the UI thread (one HTTP call per line).
    /// </summary>
    internal static class EventLogWorker
    {
        internal static void Log(string message)
        {
            try
            {
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                Task.Factory.StartNew(() => new SdaEventLogCreate().Create(loginName, null, true, message));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
