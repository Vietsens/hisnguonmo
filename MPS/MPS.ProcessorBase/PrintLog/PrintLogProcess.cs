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
using Inventec.Core;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.ProcessorBase.PrintLog
{
    public class PrintLogProcess
    {
        private static List<SAR.SDO.SarPrintLogSDO> ListPrintLog;
        private static bool IsCloseApp;

        private static int RepeatTime
        {
            get
            {
                int time = 0;
                string stringTime = System.Configuration.ConfigurationManager.AppSettings["MPS.PrintLog.RepeatTime"] ?? "0";
                bool check = int.TryParse(stringTime, out time);

                if (check && time > 0)
                    return time;
                else
                    return 0;
            }
        }

        public static bool Add(SAR.SDO.SarPrintLogSDO data)
        {
            bool result = false;
            try
            {
                if (data != null && !String.IsNullOrWhiteSpace(data.UniqueCode))
                {
                    if (ListPrintLog == null) ListPrintLog = new List<SAR.SDO.SarPrintLogSDO>();

                    lock (ListPrintLog)
                    {
                        ListPrintLog.Add(data);
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public static void SendSarPrintLog()
        {
            //System.Threading.Thread Create = new System.Threading.Thread(CreateRepeatThreadSendLog);
            //try
            //{
            //    Create.Start();
            //}
            //catch (Exception ex)
            //{
            //    Create.Abort();
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private static void CreateRepeatThreadSendLog()
        {
            try
            {
                while (!IsCloseApp && RepeatTime > 0)
                {
                    CreateSarPrintLog(IsCloseApp);
                    System.Threading.Thread.Sleep(RepeatTime);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Khi tắt pm sẽ gọi hàm và truyền vào true
        /// </summary>
        /// <param name="isCloseApp"></param>
        public static void CreateSarPrintLog(bool isCloseApp)
        {
            try
            {
                IsCloseApp = isCloseApp;

                if (ListPrintLog != null && ListPrintLog.Count > 0)
                {
                    List<SAR.SDO.SarPrintLogSDO> listSend = new List<SAR.SDO.SarPrintLogSDO>();
                    lock (ListPrintLog)
                    {
                        listSend.AddRange(ListPrintLog);
                        ListPrintLog = new List<SAR.SDO.SarPrintLogSDO>();
                    }

                    foreach (var log in listSend)
                    {
                        Inventec.Core.ApiResultObject<SAR.SDO.SarPrintLogSDO> aro = null;
                        try
                        {
                            aro = ApiConsumerStore.SarConsumer.Post<Inventec.Core.ApiResultObject<SAR.SDO.SarPrintLogSDO>>("/api/SarPrintLog/Create", new CommonParam(), log);
                        }
                        catch (Exception) { }

                        if (aro == null || !aro.Success)
                        {
                            Add(log);
                            Inventec.Common.Logging.LogSystem.Info("Co loi khi ghi log lich su in____" + "Input:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => log), log) + "____Output:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => aro), aro));
                        }
                    }
                }

                if (isCloseApp && ListPrintLog != null && ListPrintLog.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListPrintLog), ListPrintLog));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
