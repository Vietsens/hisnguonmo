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
using Inventec.Common.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Inventec.Common.WebApiClient
{
    public class IpAddressUtils
    {
        static string localIP = "";
        public static void InitialIpAddressLocal()
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "ipconfig";
                process.StartInfo.Arguments = "/all";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                localIP = GetIPv4AddressFromOutput(output);
                localIP = LocalIPAddressReplace(localIP);

                if (!String.IsNullOrEmpty(localIP))
                {
                    try
                    {
                        string pathFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ipconfig.txt");
                        File.WriteAllText(pathFile, localIP);
                        LogSystem.Info(String.Format("InitialIpAddressLocal: IPv4 Address: {0}", localIP));
                    }
                    catch (Exception ex)
                    {
                        LogSystem.Info(String.Format("Error: {0}", ex.Message));
                    }
                }
            }
            catch (Exception exx)
            {
                LogSystem.Warn(exx);
            }
        }

        public static string GetIpAddressLocal()
        {
            try
            {
                if (String.IsNullOrEmpty(localIP))
                {
                    try
                    {
                        string pathFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ipconfig.txt");
                        if (File.Exists(pathFile))
                        {
                            LogSystem.Info("GetIpAddressLocal.1");
                            StreamReader sr = new StreamReader(pathFile);
                            localIP = sr.ReadToEnd();
                            sr.Close();
                        }
                        LogSystem.Info(String.Format("IPv4 Address: {0}", localIP));
                    }
                    catch (Exception ex)
                    {
                        LogSystem.Info(String.Format("Error: {0}", ex.Message));
                    }

                    if (String.IsNullOrEmpty(localIP))
                        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                        {
                            socket.Connect("8.8.8.8", 65530);
                            IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                            localIP = endPoint.Address.ToString();
                        }

                    if (String.IsNullOrEmpty(localIP))
                    {
                        IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
                        if (localIPs != null && localIPs.Count() > 0 && localIPs.Any(k => k.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork))
                        {
                            localIP = localIPs.Where(k => k.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).FirstOrDefault().ToString();
                        }
                    }

                    localIP = LocalIPAddressReplace(localIP);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }

            return localIP;
        }

        private static string LocalIPAddressReplace(string localIPAddress)
        {
            if (!String.IsNullOrEmpty(localIPAddress))
            {
                localIPAddress = localIPAddress.Replace("(Preferred)", "").Replace("(Duplicate)", "");
                int idex = localIPAddress.IndexOf("(");
                if (idex > 0)
                {
                    localIPAddress = localIPAddress.Substring(0, idex);
                }
            }
            return localIPAddress;
        }

        static string GetIPv4AddressFromOutput(string output)
        {
            // Sử dụng biểu thức chính quy để tìm địa chỉ IPv4 từ output của ipconfig
            Regex regex = new Regex(@"IPv4 Address[^\r\n:]*:\s*([^\s]+)");
            Match match = regex.Match(output);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return null;
        }
    }
}
