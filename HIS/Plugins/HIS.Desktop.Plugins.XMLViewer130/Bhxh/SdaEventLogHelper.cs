/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using SDA.SDO;
using System;
using System.Net;
using System.Net.Sockets;

namespace HIS.Desktop.Plugins.XMLViewer130.Bhxh
{
    internal static class SdaEventLogHelper
    {
        /// <summary>
        /// Tạo log audit trail vào SDA_EVENT_LOG qua api/SdaEventLog/Create.
        /// </summary>
        internal static bool Create(string loginName, long? eventLogTypeId, bool? isSuccess, string message)
        {
            bool result = false;
            try
            {
                var data = new SdaEventLogSDO
                {
                    EventLogTypeId = eventLogTypeId,
                    IsSuccess = isSuccess,
                    Description = message,
                    Ip = GetIpLocal(),
                    LogginName = loginName,
                    EventTime = Inventec.Common.DateTime.Get.Now(),
                    AppCode = GlobalVariables.APPLICATION_CODE
                };

                CommonParam param = new CommonParam();
                result = new BackendAdapter(param).Post<bool>(
                    "/api/SdaEventLog/Create",
                    ApiConsumers.SdaConsumer,
                    data,
                    param);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private static string GetIpLocal()
        {
            string ip = "";
            try
            {
                IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
                if (localIPs != null && localIPs.Length > 0)
                {
                    foreach (var item in localIPs)
                    {
                        if (item.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ip = item.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return ip;
        }
    }
}
