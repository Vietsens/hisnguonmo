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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisImportPatient.PopUp
{
	public partial class frmWaiting : Form
	{
		int total;
		public frmWaiting(int total)
		{
			this.total = total;
			InitializeComponent();
			string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
			this.Icon = Icon.ExtractAssociatedIcon(iconPath);
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			try
			{
				
				progressBar1.Value = UpdateIndex.currentIndex;
				lblStatus.Text = "Đã xử lý: " + UpdateIndex.currentIndex + "/" + total;
				if (UpdateIndex.currentIndex == total)
				{
					btnClose.Enabled = true;
					timer1.Stop();
				}	
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void frmWaiting_Load(object sender, EventArgs e)
		{
			try
			{
				progressBar1.Maximum = total;
				timer1.Start();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			try
			{
				this.Close();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
	}
}
