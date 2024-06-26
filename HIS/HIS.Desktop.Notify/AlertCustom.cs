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
using HIS.Desktop.Notify.Properties;
using Inventec.Common.Adapter;
using Inventec.Common.WebApiClient;
using Inventec.Core;
using SDA.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Notify
{
    public partial class AlertCustom : UserControl
    {
        private bool IsPin { get; set; }

        private int x;
        private int y;
        public NotifyADO ado;
        public string branchName;
        public string loginName;
        private ApiConsumer sdaConsumer = null;
        CommonParam commonParam;
        Action<bool> DisposeAlert;
        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                SdaNotifySeenSDO sdo = new SdaNotifySeenSDO();
                sdo.Ids = new List<long>() { ado.ID };
                sdo.Loginname = loginName;
                Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                var result = new BackendAdapter(commonParam).Post<bool>("api/SdaNotify/NotifySeen", sdaConsumer, sdo, commonParam);
                Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));
                DisposeAlert(result);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public AlertCustom()
        {
            InitializeComponent();
        }
        public AlertCustom(NotifyADO ado, ApiConsumer sdaConsumer, CommonParam commonParam, string branchName, string loginname,Action<bool> DisposeAlert)
        {
            this.ado = ado;
            this.branchName = branchName;
            this.sdaConsumer = sdaConsumer;
            this.loginName = loginname;
            this.commonParam = commonParam;
            this.DisposeAlert = DisposeAlert;
            InitializeComponent();
            this.BackColor = Color.FromArgb(248, 248, 249);
        }

        public void createAlert(Action<string> dic)
        {
            try
            {
                int height = lblContent.Size.Height;

                this.lblBranchName.Text = string.Format(lblBranchName.Text, branchName);
                this.lblContent.Text = string.Format(lblContent.Text, ado.CONTENT);
                //if (ado.CONTENT.Length < 40)
                //{
                //    this.Width -= 120;
                //}
                this.lblCreator.Text = string.Format(lblCreator.Text, ado.CREATOR);
                this.lblTime.Text = string.Format(lblTime.Text, (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ado.FROM_TIME) ?? DateTime.Now).ToString("HH:mm dd/MM/yyyy"));

                if (lblContent.Size.Height > height)
                {
                    this.Height += lblContent.Size.Height;
                }
                else
                {
                    var countLine = ado.CONTENT.Length / 40;
                    this.Height += (lblContent.Size.Height - 3) * countLine;
                    Inventec.Common.Logging.LogSystem.Debug(countLine + "_" + lblContent.Size.Height);
                }
                Inventec.Common.Logging.LogSystem.Debug(Width + "_" + Height);
                dic(Width + "_" + Height);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void ShowAlert(int x, int y)
        {

            try
            {
                this.x = x;
                this.y = y;
                this.Location = new Point(x, y);

                this.Show();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
    }
}
