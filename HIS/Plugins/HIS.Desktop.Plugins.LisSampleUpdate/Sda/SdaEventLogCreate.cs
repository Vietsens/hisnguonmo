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
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.UC.Login.Base;
using SDA.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.LisSampleUpdate.Sda.SdaEventLogCreate
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
                //param.HasException = true; //Khong set param o day ma chi logging do viec log event la 1 viec phu khong qua quan trong
            }
        }

        string GetIpLocal()
        {
            string ip = "";
            try
            {
                // get local IP addresses
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
                //param.HasException = true; //Khong set param o day ma chi logging do viec log event la 1 viec phu khong qua quan trong
            }

            return ip;
        }

        private bool Create(SdaEventLogSDO data)
        {
            bool result = false;
            try
            {
                //Inventec.Core.ApiResultObject<bool> aro = ApiConsumerStore.SdaConsumer.Post<Inventec.Core.ApiResultObject<bool>>("/api/SdaEventLog/Create", param, data);
                var aro = new BackendAdapter(param).Post<bool>("/api/SdaEventLog/Create", ApiConsumer.ApiConsumers.SdaConsumer, data, param);
                Inventec.Common.Logging.LogSystem.Info("Du lieu dau ra SdaEventLog/Create:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => aro), aro));
                if (aro)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                //param.HasException = true; //Khong set param o day ma chi logging do viec log event la 1 viec phu khong qua quan trong
                result = false;
            }
            return result;
        }
    }
}
