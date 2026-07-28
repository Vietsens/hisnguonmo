/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
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
 *
 * BV HAGL — bọc UCAnticipateCreateV2 trong Form popup để thay thế màn "Sửa dự trù"
 * (HIS.Desktop.Plugins.AnticipateUpdate) khi mở từ Danh sách dự trù.
 */
using HIS.Desktop.LocalStorage.Location;
using MOS.EFMODEL.DataModels;
using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AnticipateCreateV2
{
    public partial class frmAnticipateCreateV2Edit : HIS.Desktop.Utility.FormBase
    {
        UCAnticipateCreateV2 uc;

        public frmAnticipateCreateV2Edit(Inventec.Desktop.Common.Modules.Module module, V_HIS_ANTICIPATE anticipate,
            HIS.Desktop.Common.DelegateRefreshData refresh)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
                if (module != null) this.Text = module.text;

                uc = new UCAnticipateCreateV2(module, module.RoomId, module.RoomTypeId);
                uc.Dock = DockStyle.Fill;
                this.Controls.Add(uc);
                uc.LoadExistingAnticipate(anticipate, refresh);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmAnticipateCreateV2Edit_Load(object sender, EventArgs e)
        {
        }
    }
}
