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
using HIS.Desktop.LocalStorage.Location;
using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisImpMestMediMate.HisImpMestMediMate
{
    public partial class frmHisImpMestMediMate : HIS.Desktop.Utility.FormBase
    {
        private UCHisImpMestMediMate uc;

        public frmHisImpMestMediMate(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                SetIcon();

                if (module != null && !string.IsNullOrWhiteSpace(module.text))
                {
                    this.Text = module.text;
                }

                // Ho tro Ctrl+Shift+S (tuy chinh ten nut & phim tat) tu FormBase
                this.AddBarManager(this.barManager1);

                uc = new UCHisImpMestMediMate(module);
                uc.Dock = DockStyle.Fill;
                this.panelControl1.Controls.Add(uc);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmHisImpMestMediMate()
        {
            InitializeComponent();
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(
                    ApplicationStoreLocation.ApplicationDirectory,
                    ConfigurationManager.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barBtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (uc != null) uc.Search();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barBtnExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (uc != null) uc.btnExport_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
