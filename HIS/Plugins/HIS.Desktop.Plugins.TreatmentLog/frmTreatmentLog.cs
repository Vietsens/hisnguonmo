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
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentLog
{
    public partial class frmTreatmentLog : HIS.Desktop.Utility.FormBase
    {
        long TreatmentId = 0;
        Inventec.Desktop.Common.Modules.Module module;
        long currentRoomId = 0;
        public frmTreatmentLog(Inventec.Desktop.Common.Modules.Module _Module, long _treatmentId, long _currentRoomId)
		:base(_Module)
        {
            InitializeComponent();
            TreatmentId = _treatmentId;
            module = _Module;
            this.currentRoomId = _currentRoomId;
            UCTreatmentProcessPartial UCtreatmentProcessPartial = new UCTreatmentProcessPartial(module, TreatmentId, currentRoomId);
            this.xtraUserControl1.Controls.Add(UCtreatmentProcessPartial);
            UCtreatmentProcessPartial.Dock = DockStyle.Fill;
            if (module != null)
            {
                this.Text = module.text;
            }
        }

        private void frmTreatmentLog_Load(object sender, EventArgs e)
        {
            SetIcon();
        }
        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
    }
}
