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
using HIS.Desktop.ADO;
using HIS.Desktop.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AttachFileViewer.AttachFileViewer
{
    public partial class frmAttachFileViewer : FormBase
    {
        AttachFileADO attachFileADO;
        private const string BASE_URL = @"/C net use \\{0} {1} /user:{2}";
        private const string BASE_URL_2 = @"net use \\{0} {1} /user:{2}";
        const int TIME_OUT = 10000;

        public frmAttachFileViewer()
            : this(null, null)
        {

        }

        public frmAttachFileViewer(Inventec.Desktop.Common.Modules.Module module, AttachFileADO attachFileADO)
            : base(module)
        {
            InitializeComponent();
            this.attachFileADO = attachFileADO;
        }

        private void frmAttachFileViewer_Load(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.attachFileADO), this.attachFileADO));
                if (this.attachFileADO != null && !String.IsNullOrEmpty(this.attachFileADO.AttachFileUrl))
                {
                    ////var s = ConfigSystems.URI_API_PACS.Split('file://');
                    ////var ip = s[2].Split(':').FirstOrDefault();
                    if (FileCFG.DIC_SERVER_EMR_FILE != null && FileCFG.DIC_SERVER_EMR_FILE.Count > 0)// && FileCFG.DIC_SERVER_PACS.ContainsKey(ip)
                    {
                        Inventec.Common.Logging.LogSystem.Info("Connect to share computer emr integration with hisconfig key success");
                        string urlIPdf = this.attachFileADO.AttachFileUrl.Replace("file:", "").Replace("/", "\\");
                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => urlIPdf), urlIPdf));
                        pdfViewer1.LoadDocument(urlIPdf);
                    }
                    else
                    {
                        //samba_share
                        //123456
                        string ip = "172.251.109.6";
                        string user = "samba_share";
                        string password = "123456";
                        if (OpenConnect(ip, user, password) || OpenConnect2(ip, user, password))
                        {
                            Inventec.Common.Logging.LogSystem.Info("Connect to share computer emr integration with fix ip success____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ip), ip) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => user), user));

                            string urlIPdf = this.attachFileADO.AttachFileUrl.Replace("file:", "").Replace("/", "\\");
                            Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => urlIPdf), urlIPdf));
                            pdfViewer1.DetachStreamAfterLoadComplete = true;
                            pdfViewer1.LoadDocument(urlIPdf);

                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Info("OpenConnect fail");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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

        private void bbtnprint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.pdfViewer1.Print();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
