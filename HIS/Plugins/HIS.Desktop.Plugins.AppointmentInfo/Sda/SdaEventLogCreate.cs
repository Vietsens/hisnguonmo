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
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Core;
using SDA.SDO;
using System;

namespace HIS.Desktop.Plugins.AppointmentInfo.Sda.SdaEventLogCreate
{
    internal partial class SdaEventLogCreate : BusinessBase
    {
        public SdaEventLogCreate()
            : base()
        {

        }

        public SdaEventLogCreate(CommonParam paramCreate)
            : base(paramCreate)
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
                Create(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                //Khong set param.HasException - log event la viec phu, khong duoc lam fail nghiep vu chinh
            }
        }

        string GetIpLocal()
        {
            string ip = "";
            try
            {
                System.Net.IPAddress[] localIPs = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName());
                if (localIPs != null && localIPs.Length > 0)
                {
                    foreach (var item in localIPs)
                    {
                        if (item.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            ip = item.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return ip;
        }

        private bool Create(SdaEventLogSDO data)
        {
            bool result = false;
            try
            {
                var aro = new BackendAdapter(param).Post<bool>("/api/SdaEventLog/Create", HIS.Desktop.ApiConsumer.ApiConsumers.SdaConsumer, data, param);
                if (aro)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
    }
}
