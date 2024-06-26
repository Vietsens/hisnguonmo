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
using DevExpress.Utils.Menu;
using HIS.Desktop.Plugins.HisTrackingList.ADO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisTrackingList
{
    public partial class frmEmrDocument : Form
    {
        public UcEmrDocument.UcViewEmrDocument ucViewEmrDocument1 { get; set; }
        public frmEmrDocument(List<DocumentTrackingADO> documents, DXPopupMenu menuTemplate)
        {
            InitializeComponent();
            SetIconFrm();
            LoadDropDown(menuTemplate);
            InitUcEmrDocument(documents);
        }

        private void LoadDropDown(DXPopupMenu menuTemplate)
        {
            btnDropDown.DropDownControl = menuTemplate;
        }

        private void InitUcEmrDocument(List<DocumentTrackingADO> documents)
        {
            try
            {
                ucViewEmrDocument1 = new UcEmrDocument.UcViewEmrDocument(false);
                ucViewEmrDocument1.Dock = DockStyle.Fill;
                this.panel1.Controls.Add(ucViewEmrDocument1);
                ucViewEmrDocument1.ReloadDocument(documents, documents != null && documents.Count > 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDropDown_Click(object sender, EventArgs e)
        {
            btnDropDown.ShowDropDown();
        }
    }
}
