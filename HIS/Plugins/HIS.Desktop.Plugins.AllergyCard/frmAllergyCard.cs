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

namespace HIS.Desktop.Plugins.AllergyCard
{
    public partial class frmAllergyCard : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        long treatmentID;
        V_HIS_PATIENT currentPatient;

        public frmAllergyCard()
        {
            InitializeComponent();
        }

        public frmAllergyCard(Inventec.Desktop.Common.Modules.Module module, long _treatmentId)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.Text = module.text;
                this.currentModule = module;
                this.treatmentID = _treatmentId;
                SetIcon();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
			
        }

        public frmAllergyCard(Inventec.Desktop.Common.Modules.Module module, V_HIS_PATIENT _patient)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.Text = module.text;
                this.currentModule = module;
                this.currentPatient = _patient;
                SetIcon();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void frmAllergyCard_Load(object sender, EventArgs e)
        {
            try
            {
                //this.Size = new Size(1018, 485);
                if (this.currentPatient == null)
                {
                    UC_AllergyCard uc = new UC_AllergyCard(this.currentModule, this.treatmentID);
                    uc.Dock = DockStyle.Fill;
                    panelControlAllergyCard.Controls.Add(uc);
                }
                else
                {
                    UC_AllergyCard uc = new UC_AllergyCard(this.currentModule, this.currentPatient);
                    uc.Dock = DockStyle.Fill;
                    panelControlAllergyCard.Controls.Add(uc);
                }
                //panelControlAllergyCard.Dock = DockStyle.Fill;
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
    }
}
