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
using HIS.Desktop.Plugins.RegisterV2.ADO;
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

namespace HIS.Desktop.Plugins.RegisterV2.Choice
{
    public partial class frmConfigCall : Form
    {
        const string moduleLink = "HIS.Desktop.Plugins.RegisterV2.frmConfigCall";
        const long PRIORITY_TRUE = 1;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
		Action<bool> IsReload;
	
		public frmConfigCall(Action<bool> IsReload)
        {
            InitializeComponent();
			this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
			this.IsReload = IsReload;
		}

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
			try
			{
				btnSave_Click(null,null);
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

        private void SetTextToGate(string obj)
        {
			try
			{
				txtConfig.Text += " " + obj;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

        private void btnSave_Click(object sender, EventArgs e)
        {
			try
			{
				HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == moduleLink && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
				if (csAddOrUpdate != null)
				{
					csAddOrUpdate.VALUE = txtConfig.Text;
				}
				else
				{
					csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
					csAddOrUpdate.KEY = moduleLink;
					csAddOrUpdate.VALUE = txtConfig.Text.Trim();
					csAddOrUpdate.MODULE_LINK = moduleLink;
					if (this.currentControlStateRDO == null)
						this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
					this.currentControlStateRDO.Add(csAddOrUpdate);
				}
				this.controlStateWorker.SetData(this.currentControlStateRDO);
				IsReload(true);
				this.Close();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

        private void frmConfigCall_Load(object sender, EventArgs e)
        {
            try
            {
				this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
				this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
				if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
				{
					foreach (var item in this.currentControlStateRDO)
					{
						if (item.KEY == moduleLink)
						{
							txtConfig.Text = item.VALUE;
						}
					}
				}
			}
			catch (Exception ex)
            {
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
        }

        private void txtConfig_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
				if(e.KeyCode == Keys.F1)
                {
					frmKeyConfig frm = new frmKeyConfig(SetTextToGate);
					frm.ShowDialog();
                }					
            }
            catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
        }

        private void frmConfigCall_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
				currentControlStateRDO = null;
				controlStateWorker = null;
				this.barButtonItem1.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
				this.btnSave.Click -= new System.EventHandler(this.btnSave_Click);
				this.txtConfig.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtConfig_PreviewKeyDown);
				this.Load -= new System.EventHandler(this.frmConfigCall_Load);
				barDockControlRight = null;
				barDockControlLeft = null;
				barDockControlBottom = null;
				barDockControlTop = null;
				barButtonItem1 = null;
				bar1 = null;
				barManager1 = null;
				emptySpaceItem1 = null;
				layoutControlItem2 = null;
				layoutControlItem1 = null;
				layoutControlGroup1 = null;
				txtConfig = null;
				btnSave = null;
				layoutControl1 = null;
			}
            catch (Exception ex)
            {
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
        }
    }
}
