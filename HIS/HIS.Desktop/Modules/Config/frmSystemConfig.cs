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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.Location;
using System.Configuration;
using Inventec.Common.Logging;

namespace HIS.Desktop.Modules.Config
{
    public partial class frmSystemConfig : DevExpress.XtraEditors.XtraForm
    {
        public frmSystemConfig()
        {
            InitializeComponent();
        }

        private void DelegateCloseForm()
        {
            //RAE.MANAGER.Token.TokenManager.ResetConsunmer();

            //Show form login
            // to start new instance of application
            System.Diagnostics.Process.Start(Application.ExecutablePath);

            //close this one
            System.Diagnostics.Process.GetCurrentProcess().Kill();

            this.Close();
        }

        private void frmSystemConfig_Load(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }

                Inventec.UC.ServerConfig.MainServerConfig UCSystemConfig = new Inventec.UC.ServerConfig.MainServerConfig();
                var formConfig = UCSystemConfig.Init(Inventec.UC.ServerConfig.MainServerConfig.EnumTemplate.TEMPLATE1, DelegateCloseForm);
                formConfig.Dock = DockStyle.Fill;
                this.Controls.Add(formConfig);
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
