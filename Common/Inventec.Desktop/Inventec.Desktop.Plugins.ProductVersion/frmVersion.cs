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
using Inventec.Desktop.Common.LocalStorage.Location;
using System;
using System.Configuration;
using System.Drawing;

namespace Inventec.Desktop.Plugins.ProductVersion
{
    public partial class frmVersion : DevExpress.XtraEditors.XtraForm
    {
        public frmVersion()
        {
            InitializeComponent();
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void frmVersion_Load(object sender, EventArgs e)
        {
            try
            {
                txtVersion.Text = System.IO.File.ReadAllText("readme.txt");
            }
            catch (Exception ex)
            {
                LogSystem.Warn("Khong tim thay file readme.txt hoac du lieu khong hop le. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }

        }
    }
}
