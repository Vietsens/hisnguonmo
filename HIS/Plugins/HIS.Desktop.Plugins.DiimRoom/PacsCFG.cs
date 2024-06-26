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

namespace HIS.Desktop.Plugins.DiimRoom
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
        public const string SOURCE = @"\\{0}\{1}";
        private const int TIME_OUT = 5000;

        private const string PACS_IP_CFG = "HIS.PACS_IP";

        private static Dictionary<string, PacsIp> dic_server_pacs = null;
        internal static Dictionary<string, PacsIp> DIC_SERVER_PACS //key: ip
        {
            get
            {
                if (dic_server_pacs == null)
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
                    List<PacsIp> PacsIps = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PacsIp>>(ipConfig);
                    if (PacsIps == null && PacsIps.Count == 0)
                    {
                        throw new Exception("Khong Json duoc cau hinh ip: " + ipConfig);
                    }

                    foreach (var item in PacsIps)
                    {
                        if (OpenConnect(item.Ip, item.User, item.Password) || (String.IsNullOrEmpty(item.Password) && String.IsNullOrEmpty(item.User)))
                        {
                            if (result.ContainsKey(item.Ip))
                                result[item.Ip] = item;
                            else
                                result.Add(item.Ip, item);
                        }
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

        internal static bool OpenConnect(string ip, string user, string pass)
        {
            bool result = true;
            try
            {
                if (!string.IsNullOrEmpty(ip) && !String.IsNullOrEmpty(user))
                {
                    string url = String.Format(BASE_URL, ip, pass, user);

                    using (Process process = new Process())
                    {
                        process.StartInfo.FileName = "cmd.exe";
                        process.StartInfo.Arguments = url;
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.RedirectStandardError = true;
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                        StringBuilder output = new StringBuilder();
                        StringBuilder error = new StringBuilder();

                        using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                        using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
                        {
                            process.OutputDataReceived += (sender, e) =>
                            {
                                try
                                {
                                    if (e.Data == null)
                                    {
                                        outputWaitHandle.Set();
                                    }
                                    else
                                    {
                                        output.AppendLine(e.Data);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Inventec.Common.Logging.LogSystem.Error(ex);
                                }
                            };
                            process.ErrorDataReceived += (sender, e) =>
                            {
                                try
                                {
                                    if (e.Data == null)
                                    {
                                        errorWaitHandle.Set();
                                    }
                                    else
                                    {
                                        error.AppendLine(e.Data);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Inventec.Common.Logging.LogSystem.Error(ex);
                                }
                            };

                            process.Start();

                            process.BeginOutputReadLine();
                            process.BeginErrorReadLine();

                            if (process.WaitForExit(TIME_OUT) &&
                                outputWaitHandle.WaitOne(TIME_OUT) &&
                                errorWaitHandle.WaitOne(TIME_OUT))
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
                        }
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
    }
}
