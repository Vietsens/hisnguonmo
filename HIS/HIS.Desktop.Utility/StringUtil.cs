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
using HIS.Desktop.LocalStorage.Location;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Utility
{
    public class StringUtil
    {
        internal static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        internal static string Base64Encode(string dataEncode)
        {
            return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(dataEncode));
        }

        internal static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        internal static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        public static string GetIpLocal()
        {
            string ips = "";
            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
            if (localIPs != null && localIPs.Count() > 0)
            {
                ips = String.Join(",", localIPs.Where(k => k.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).Select(k => k.ToString()));
                if (ips.EndsWith(","))
                {
                    ips = ips.Substring(0, ips.Length - 2);
                }
            }
            return ips;
        }

        static string versionApp;
        public static string VersionApp
        {
            get
            {
                if (String.IsNullOrEmpty(versionApp))
                {
                    versionApp = File.Exists(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, "readme.txt")) ? System.IO.File.ReadAllText(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, "readme.txt")) : "";
                }
                return versionApp;
            }
            set
            {
                versionApp = value;
            }
        }

        static string customerCode;
        public static string CustomerCode
        {
            get
            {
                if (String.IsNullOrEmpty(customerCode))
                {
                    string customerInfo = LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.VPLUS_CUSTOMER_INFO");
                    if (!String.IsNullOrEmpty(customerInfo))
                    {
                        var cusInfoArr = customerInfo.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                        if (cusInfoArr != null && cusInfoArr.Length > 2)
                        {
                            customerCode = cusInfoArr[0];
                        }
                    }
                }
                return customerCode;
            }
            set
            {
                customerCode = value;
            }
        }

        static string stt_sản_phẩm;
        public static string Stt_sản_phẩm
        {
            get
            {
                if (String.IsNullOrEmpty(stt_sản_phẩm))
                {
                    string customerInfo = LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.VPLUS_CUSTOMER_INFO");
                    if (!String.IsNullOrEmpty(customerInfo))
                    {
                        var cusInfoArr = customerInfo.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                        if (cusInfoArr != null && cusInfoArr.Length > 2)
                        {
                            stt_sản_phẩm = cusInfoArr[2];
                        }
                    }
                }
                return stt_sản_phẩm;
            }
            set
            {
                stt_sản_phẩm = value;
            }
        }

        static string stt_phần_mềm;
        public static string Stt_phần_mềm
        {
            get
            {
                if (String.IsNullOrEmpty(stt_phần_mềm))
                {
                    string customerInfo = LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.VPLUS_CUSTOMER_INFO");
                    if (!String.IsNullOrEmpty(customerInfo))
                    {
                        var cusInfoArr = customerInfo.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                        if (cusInfoArr != null && cusInfoArr.Length > 2)
                        {
                            stt_phần_mềm = cusInfoArr[3];
                        }
                    }
                }
                return stt_phần_mềm;
            }
            set
            {
                stt_phần_mềm = value;
            }
        }
    }
}
