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
using System.Configuration;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.LocalStorage.Location;

namespace Inventec.Desktop.Plugins.Deverloper
{
    public partial class frmDeverloper : DevExpress.XtraEditors.XtraForm
    {
        string strPhatTrien = ConfigurationSettings.AppSettings["Inventec.Common.UcDeverloper.Deverloper"] ?? "";
        string strDonVi = ConfigurationSettings.AppSettings["Inventec.Common.UcDeverloper.GroupName"] ?? "";
        string strImage = ConfigurationSettings.AppSettings["Inventec.Common.UcDeverloper.Image.FileName"] ?? "";

        public frmDeverloper()
        {
            InitializeComponent();
        }

        private void frmDeverloper_Load(object sender, EventArgs e)
        {
            try
            {
                Inventec.UC.Development.MainDevelopment UCDeverloper = new Inventec.UC.Development.MainDevelopment();
                var formConfig = UCDeverloper.Init(Inventec.UC.Development.MainDevelopment.EmumTemp.TEMPLATE1, strDonVi, strPhatTrien);
                UCDeverloper.SetImage(formConfig, strImage);
                formConfig.Dock = DockStyle.Fill;
                this.Controls.Add(formConfig);
                try
                {
                    this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
