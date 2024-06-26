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
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentList
{
    class LaunchChrome
    {
        internal LaunchChrome() { }

        internal void Launch(string code, string apiDomain)
        {
            var chromePath = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
            var found = System.IO.File.Exists(chromePath);
            if (!found)
            {
                chromePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
                found = System.IO.File.Exists(chromePath);
            }
            if (!found)
            {
                using (var openFileDialog = new System.Windows.Forms.OpenFileDialog
                {
                    Filter = "Chrome browser|chrome.exe",
                    CheckPathExists = true,
                    Title = "Hãy chọn trình duyệt chrome"
                })
                {
                    if (openFileDialog.ShowDialog() ==
                    System.Windows.Forms.DialogResult.OK)
                    {
                        chromePath = openFileDialog.FileName;
                        found = true;
                    }
                }
            }
            if (!found)
                return;
            var url = String.Format("{0}{1}{2}", apiDomain, "/admin/hssk4210?cardCode=", code);// "http://ybadientu.vn:60380/admin/hssk4210?cardCode=";
            System.Diagnostics.Process.Start(chromePath, url);
        }

        internal void Launch1(string maThe, string successUrl, string apiDomain)
        {
            var chromePath = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
            var found = File.Exists(chromePath);
            if (!found)
            {
                chromePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
                found = File.Exists(chromePath);
            }

            if (!found)
            {
                using (var openFileDialog = new OpenFileDialog { Filter = "Chrome browser|chrome.exe", CheckPathExists = true, Title = "Hãy chọn trình duyệt chrome" })
                {
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        chromePath = openFileDialog.FileName;
                        found = true;
                    }
                }
            }

            if (!found)
                return;

            //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => maThe), maThe) + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => successUrl), successUrl) + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiDomain), apiDomain));

            var secretKey = GetSecretKey(maThe);
            var clearText = maThe + "\n" + secretKey + "\n" + successUrl;
            var p = Convert.ToBase64String(Encoding.UTF8.GetBytes(clearText));

            Process.Start(chromePath, "-incognito " + apiDomain + "/redirect2.html?p=" + p);
        }

        string GetSecretKey(string maThe)
        {
            // return "abcdef";
            // {time}.salt.{hash({mã thẻ}.{time}.{private})}
            var tick = DateTime.Now.Ticks.ToString();
            var sha256 = SHA256.Create();
            var privateString = "VietSens_Siten_2019";
            var salt = new Random().Next().ToString();
            return tick + "." + salt + "." + Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(maThe + "." + tick + "." + salt + "." + privateString)));
        }
    }
}
