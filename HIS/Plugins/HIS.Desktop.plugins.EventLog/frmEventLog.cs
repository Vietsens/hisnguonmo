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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.EventLog
{
    public partial class frmEventLog : Form
    {
        UCEventLog uCEventLog;
        //private UC.EventLogControl.ProcessHasException exceptionApi;
        KeyCodeADO keyCodeADO = null;

        public frmEventLog()
        {
            InitializeComponent();
        }

        public frmEventLog(KeyCodeADO ado)
        {
            InitializeComponent();
            // TODO: Complete member initialization
            keyCodeADO = ado;
        }

        private void frmEventLog_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                if (keyCodeADO != null)
                {
                    this.uCEventLog = new UCEventLog(keyCodeADO);
                }
                else
                {
                    this.uCEventLog = new UCEventLog();
                }
                if (this.uCEventLog != null)
                {
                    this.panelControl1.Controls.Add(this.uCEventLog);
                    this.uCEventLog.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
