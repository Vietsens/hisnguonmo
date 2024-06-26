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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AllergyCardCreate
{
    public partial class frmAllergyCardCreate : HIS.Desktop.Utility.FormBase
    {
        UC_AllergyCardCreate uc;
        V_HIS_ALLERGY_CARD currentAllergyCard;
        Inventec.Desktop.Common.Modules.Module currentModule;
        long treatmentId;

        public frmAllergyCardCreate()
        {
            InitializeComponent();
        }

        public frmAllergyCardCreate(Inventec.Desktop.Common.Modules.Module module, long _treatmentId)
            : base(module)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                this.Text = module.text;
                this.treatmentId = _treatmentId;
                this.currentModule = module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public frmAllergyCardCreate(Inventec.Desktop.Common.Modules.Module module, V_HIS_ALLERGY_CARD data)
            : base(module)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                this.Text = "Sửa thẻ dị ứng";
                this.currentAllergyCard = data;
                this.currentModule = module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void frmAllergyCardCreate_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.currentAllergyCard != null)
                {
                    uc = new UC_AllergyCardCreate(this.currentModule, this.currentAllergyCard);
                    uc.Dock = DockStyle.Fill;
                    panelControlAllergyCardCreate.Controls.Add(uc);
                }
                else
                {
                    uc = new UC_AllergyCardCreate(this.currentModule, this.treatmentId);
                    uc.Dock = DockStyle.Fill;
                    panelControlAllergyCardCreate.Controls.Add(uc);
                }
                //this.Size = new Size(884, 398);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

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

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (uc != null)
                {
                    uc.Add();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (uc != null)
                {
                    uc.Save();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (uc != null)
                {
                    uc.Print();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (uc != null)
                {
                    uc.RefreshForm();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
