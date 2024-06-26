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
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.Global.ADO;
using HIS.Desktop.LocalStorage.LocalData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Modules.CheckServerConnect
{
    public partial class frmCheckServerConnect : Form
    {
        public frmCheckServerConnect()
        {
            InitializeComponent();
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmCheckServerConnect_Load(object sender, EventArgs e)
        {
            try
            {
                gridControl1.DataSource = GlobalVariables.ListServerInfoADO;
                timer1.Enabled = true;
                timer1.Interval = 30000;
                timer1.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (((IList)((BaseView)sender).DataSource) != null && ((IList)((BaseView)sender).DataSource).Count > 0)
                    {
                        ServerInfoADO serverInfoADO = (ServerInfoADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                        if (serverInfoADO != null)
                        {
                            if (e.Column.FieldName == "IPStatusDisplay")
                            {
                                e.Value = serverInfoADO.IPStatus != null ? serverInfoADO.IPStatus.ToString() : "";
                            }
                            else if (e.Column.FieldName == "ServerAddressDisplay")
                            {
                                e.Value = !String.IsNullOrEmpty(serverInfoADO.ServerAddress) ? serverInfoADO.ServerAddress.Replace("http://", "") : "";
                            }
                            else if (e.Column.FieldName == "LastPingTimeDisplay")
                            {
                                e.Value = serverInfoADO.LastPingTime.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(serverInfoADO.LastPingTime.Value) : "";
                            }
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                var index = this.gridView1.GetDataSourceRowIndex(e.RowHandle);
                if (index < 0) return;

                var listDatas = this.gridControl1.DataSource as List<ServerInfoADO>;
                var dataRow = listDatas[index];
                if (dataRow != null && dataRow.IPStatus.HasValue)
                {
                    e.Appearance.ForeColor = (dataRow.IPStatus == IPStatus.Success ? System.Drawing.Color.Green : System.Drawing.Color.Red);
                    if (dataRow.IPStatus != IPStatus.Success)
                        e.Appearance.BackColor = System.Drawing.Color.Yellow;
                }
                else
                {
                    e.Appearance.ForeColor = Color.Black;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                gridControl1.DataSource = null;
                gridControl1.DataSource = GlobalVariables.ListServerInfoADO;              
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmCheckServerConnect_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                timer1.Stop();
                timer1.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
