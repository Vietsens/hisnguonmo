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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TestServiceExecute.PACS
{
    public class PacsIp
    {
        public string Ip { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }

    class PacsCFG
    {
        /// <summary>
        /// 0: Ip share folder
        /// 1: pass
        /// 2: user
        /// </summary>
        private const string BASE_URL = @"/C net use \\{0} {1} /user:{2}";
        private const string BASE_URL_2 = @"net use \\{0} {1} /user:{2}";
        public const string SOURCE = @"\\{0}\{1}";
        private const int TIME_OUT = 100;

        private const string PACS_IP_CFG = "HIS.PACS_IP";
        private const string PACS_ADDRESS_CFG = "MOS.PACS.ADDRESS";

        public class PacsAddress
        {
            public string RoomCode { get; set; }
            public string Address { get; set; }
            public int Port { get; set; }
        }

        private static List<PacsAddress> pacsAddress;
        internal static List<PacsAddress> PACS_ADDRESS
        {
            get
            {
                if (pacsAddress == null)
                {
                    pacsAddress = GetAddress(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(PACS_ADDRESS_CFG));
                }
                return pacsAddress;
            }
            set
            { pacsAddress = value; }
        }

        private static List<PacsAddress> GetAddress(string config)
        {
            List<PacsAddress> result = new List<PacsAddress>();
            try
            {
                if (String.IsNullOrWhiteSpace(config))
                {
                    throw new ArgumentNullException(config);
                }
                List<PacsAddress> adds = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PacsAddress>>(config);
                if (adds == null || adds.Count == 0)
                {
                    throw new AggregateException(config);
                }
                result.AddRange(adds);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new List<PacsAddress>();
            }
            return result;
        }

        private static Dictionary<string, PacsIp> dic_server_pacs = null;
        internal static Dictionary<string, PacsIp> DIC_SERVER_PACS //key: ip
        {
            get
            {
                if (dic_server_pacs == null || dic_server_pacs.Count == 0)
                {
                    dic_server_pacs = GetServer(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(PACS_IP_CFG));
                }
                return dic_server_pacs;
            }
            set
            {
                dic_server_pacs = value;
            }
        }

        private static Dictionary<string, PacsIp> GetServer(string ipConfig)
        {
            Dictionary<string, PacsIp> result = new Dictionary<string, PacsIp>();
            try
            {
                if (!string.IsNullOrWhiteSpace(ipConfig))
                {
                    Inventec.Common.Logging.LogSystem.Error("ipConfig:" + ipConfig);
                    List<PacsIp> PacsIps = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PacsIp>>(ipConfig);
                    if (PacsIps == null && PacsIps.Count == 0)
                    {
                        throw new Exception("Khong Json duoc cau hinh ip: " + ipConfig);
                    }

                    foreach (var item in PacsIps)
                    {
                        if (OpenConnect(item.Ip, item.User, item.Password) || OpenConnect2(item.Ip, item.User, item.Password) || (String.IsNullOrEmpty(item.Password) && String.IsNullOrEmpty(item.User)))
                        {
                            if (result.ContainsKey(item.Ip))
                                result[item.Ip] = item;
                            else
                                result.Add(item.Ip, item);
                        }
                        Inventec.Common.Logging.LogSystem.Error("OpenConnect:" + OpenConnect(item.Ip, item.User, item.Password));
                        Inventec.Common.Logging.LogSystem.Error("OpenConnect2:" + OpenConnect2(item.Ip, item.User, item.Password));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new Dictionary<string, PacsIp>();
            }
            return result;
        }

        private static bool OpenConnect(string ip, string user, string pass)
        {
            bool result = true;
            try
            {
                if (!string.IsNullOrEmpty(ip) && !String.IsNullOrEmpty(user))
                {
                    string url = String.Format(BASE_URL, ip, pass, user);
                    Inventec.Common.Logging.LogSystem.Info("url:" + url);
                    using (Process process = new Process())
                    {
                        process.StartInfo.FileName = "cmd.exe";
                        process.StartInfo.Arguments = url;
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.RedirectStandardError = true;
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                        process.Start();
                        if (process.WaitForExit(TIME_OUT))
                        {
                            // Process completed. Check process.ExitCode here.
                            if (process.ExitCode != 0)
                            {
                                result = false;
                            }
                        }
                        else
                        {
                            // Timed out.
                            result = false;
                        }

                        //StringBuilder output = new StringBuilder();
                        //StringBuilder error = new StringBuilder();

                        //using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                        //using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
                        //{
                        //    process.OutputDataReceived += (sender, e) =>
                        //    {
                        //        if (e.Data == null)
                        //        {
                        //            outputWaitHandle.Set();
                        //        }
                        //        else
                        //        {
                        //            output.AppendLine(e.Data);
                        //        }
                        //    };
                        //    process.ErrorDataReceived += (sender, e) =>
                        //    {
                        //        if (e.Data == null)
                        //        {
                        //            errorWaitHandle.Set();
                        //        }
                        //        else
                        //        {
                        //            error.AppendLine(e.Data);
                        //        }
                        //    };

                        //    process.Start();

                        //    process.BeginOutputReadLine();
                        //    process.BeginErrorReadLine();

                        //    if (process.WaitForExit(TIME_OUT) &&
                        //        outputWaitHandle.WaitOne(TIME_OUT) &&
                        //        errorWaitHandle.WaitOne(TIME_OUT))
                        //    {
                        //        // Process completed. Check process.ExitCode here.
                        //        if (process.ExitCode != 0)
                        //        {
                        //            result = false;
                        //        }
                        //    }
                        //    else
                        //    {
                        //        // Timed out.
                        //        result = false;
                        //    }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private static bool OpenConnect2(string ip, string user, string pass)
        {
            bool result = true;
            try
            {
                if (!string.IsNullOrEmpty(ip) && !String.IsNullOrEmpty(user))
                {
                    string url = String.Format(BASE_URL_2, ip, pass, user);
                    Inventec.Common.Logging.LogSystem.Info("url:" + url);
                    using (Process process = new Process())
                    {
                        process.StartInfo.FileName = "cmd.exe";
                        process.StartInfo.Arguments = url;
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.RedirectStandardError = true;
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                        process.Start();
                        if (process.WaitForExit(TIME_OUT))
                        {
                            // Process completed. Check process.ExitCode here.
                            if (process.ExitCode != 0)
                            {
                                result = false;
                            }
                        }
                        else
                        {
                            // Timed out.
                            result = false;
                        }

                        //StringBuilder output = new StringBuilder();
                        //StringBuilder error = new StringBuilder();

                        //using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                        //using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
                        //{
                        //    process.OutputDataReceived += (sender, e) =>
                        //    {
                        //        if (e.Data == null)
                        //        {
                        //            outputWaitHandle.Set();
                        //        }
                        //        else
                        //        {
                        //            output.AppendLine(e.Data);
                        //        }
                        //    };
                        //    process.ErrorDataReceived += (sender, e) =>
                        //    {
                        //        if (e.Data == null)
                        //        {
                        //            errorWaitHandle.Set();
                        //        }
                        //        else
                        //        {
                        //            error.AppendLine(e.Data);
                        //        }
                        //    };

                        //    process.Start();

                        //    process.BeginOutputReadLine();
                        //    process.BeginErrorReadLine();

                        //    if (process.WaitForExit(TIME_OUT) &&
                        //        outputWaitHandle.WaitOne(TIME_OUT) &&
                        //        errorWaitHandle.WaitOne(TIME_OUT))
                        //    {
                        //        // Process completed. Check process.ExitCode here.
                        //        if (process.ExitCode != 0)
                        //        {
                        //            result = false;
                        //        }
                        //    }
                        //    else
                        //    {
                        //        // Timed out.
                        //        result = false;
                        //    }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        internal static void Reload()
        {
            dic_server_pacs = null;
        }
    }
}
