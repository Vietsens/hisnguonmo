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
using Inventec.Common.Adapter;
using Inventec.Core;
using SDA.EFMODEL.DataModels;
using SDA.Filter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Notify
{
    public partial class Alert : Form
    {
        string message = "";
        int timeClose;

        public Alert()
        {
            InitializeComponent();
            try
            {
                this.Opacity = 0;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void Alert_Load(object sender, EventArgs e)
        {
            try
            {


                this.ShowInTaskbar = false;
                string sdaBaseUri = "";
                string applicationCode = "";
                string tokenCode = "";

                ProcessCmd(ref sdaBaseUri, ref tokenCode, ref applicationCode);
                var s = sdaBaseUri + "/";

                Inventec.Common.WebApiClient.ApiConsumer sdaConsumer = new Inventec.Common.WebApiClient.ApiConsumer(s, applicationCode);

                CommonParam param = new CommonParam();
                SdaNotifyFilter filter = new SdaNotifyFilter();
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";

                var today = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);

                var sdaNotify = new BackendAdapter(param).Get<List<SDA_NOTIFY>>("/api/SdaNotify/Get", sdaConsumer, filter, param);

                if (sdaNotify != null && sdaNotify.Count > 0)
                {
                    var data = sdaNotify.Where(o => o.FROM_TIME < today && o.TO_TIME > today).ToList();
                    if (data != null && data.Count > 0)
                    {
                        foreach (var item in data)
                        {
                            message += item.CONTENT + ". ";
                        }
                    }
                }

                lblNotify.Text = message;

                
                this.Top = Screen.PrimaryScreen.Bounds.Height - this.Height - 45;
                this.Left = Screen.PrimaryScreen.Bounds.Width;

                Show.Start();
                timeOut.Start();

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        private void ProcessCmd(ref string uri, ref string token, ref string application)
        {
            try
            {
                string cmdLn = "SdaBaseUri|http://sda.12c.vn|ApplicationCode|HIS";
                //foreach (string arg in Environment.GetCommandLineArgs())
                //{
                //    cmdLn += arg;
                //}
                //if (cmdLn.IndexOf('|') == -1)
                //{
                //    return;
                //}

                string[] tmpCmd = cmdLn.Split('|');
                for (int i = 0; i < tmpCmd.GetLength(0); i++)
                {
                    if (tmpCmd[i] == "SdaBaseUri") uri = tmpCmd[i + 1];
                    if (tmpCmd[i] == "TokenCode") token = tmpCmd[i + 1];
                    if (tmpCmd[i] == "ApplicationCode") application = tmpCmd[i + 1];
                    i++;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            try
            {
                Close.Start();
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        private void timeOut_Tick(object sender, EventArgs e)
        {
            try
            {
                timeClose += 1000;
                if (timeClose == 4000)
                {
                    Close.Start();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        private void Show_Tick(object sender, EventArgs e)
        {
            try
            {
                if (this.Left > (Screen.PrimaryScreen.Bounds.Width - this.Width - 15))
                {
                    this.Left -= 5;
                    if (this.Opacity < 100)
                        this.Opacity += 0.01;
                }
                else
                {
                    Show.Stop();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        private void Close_Tick(object sender, EventArgs e)
        {
            try
            {
                if (this.Opacity > 0)
                {
                    this.Opacity -= 0.1;
                }
                else
                {
                    Show.Stop();
                    timeOut.Stop();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        private void Alert_MouseHover(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
