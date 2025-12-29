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
using HIS.Desktop.Plugins.Optometrist.UC;
using MOS.EFMODEL.DataModels;
using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Optometrist.Optometrist
{
    public partial class frmOptometrist : HIS.Desktop.Utility.FormBase
    {
        UCOptometrist uc;

        public frmOptometrist(Inventec.Desktop.Common.Modules.Module currentModule, HIS_SERE_SERV sereServ)
            : base(currentModule)
        {
            InitializeComponent();
            SetIcon();
            if (currentModule != null)
            {
                this.Text = currentModule.text;
            }
            uc = new UCOptometrist(currentModule, sereServ);
            uc.Dock = DockStyle.Fill;
            panelControl1.Controls.Add(uc);

            this.Load += frmOptometrist_Load_SetSize;
        }

        private void frmOptometrist_Load_SetSize(object sender, EventArgs e)
        {
            try
            {
                if (uc == null) return;

                Size ucSize = uc.Size;
                if (ucSize.Width > 0 && ucSize.Height > 0)
                {
                    // Tính toán kích thước form dựa trên UC
                    int formWidth = ucSize.Width + (this.Width - this.ClientSize.Width);
                    int formHeight = ucSize.Height + (this.Height - this.ClientSize.Height);
                    // Đảm bảo form không vượt quá kích thước màn hình
                    Rectangle screenBounds = Screen.FromControl(this).WorkingArea;
                    formWidth = Math.Min(formWidth, screenBounds.Width);
                    formHeight = Math.Min(formHeight, screenBounds.Height);
                    this.Size = new Size(formWidth + 45, formHeight + 50);
                    this.StartPosition = FormStartPosition.CenterScreen;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmOptometrist()
        {
            InitializeComponent();
        }

        private void frmHistory_Load(object sender, EventArgs e)
        {

        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barOptometristSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                uc.btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barOptometristPrintDon_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                uc.btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barOptometristPrintKham_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                uc.btnPrintPhieuKham_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
