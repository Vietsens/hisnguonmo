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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Core;
using Inventec.Common.Logging;
using Inventec.Common.Adapter;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using ACS.SDO;
using Inventec.Desktop.Common.Message;

namespace Inventec.Desktop.Plugins.TrackingUser
{
    public partial class UCTrackingMonitor : UserControl
    {
        Inventec.Desktop.Common.Modules.Module moduleData;
        public UCTrackingMonitor()
        {
            InitializeComponent();
        }
        public UCTrackingMonitor(Inventec.Desktop.Common.Modules.Module moduledata)            
        {
            InitializeComponent();
            this.moduleData = moduledata;
        }

        private void UCTrackingMonitor_Load(object sender, EventArgs e)
        {
            try
            {
                this.gridColumn6.Caption = "Phiên bản " + TrackingConfig.AppCode;
                LoadData();

                timer1.Interval = 600000;//10 phút
                timer1.Enabled = true;
                timer1.Start();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public void Search()
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                WaitingManager.Show();
                CommonParam paramCommon = new CommonParam();
                List<AcsCredentialTrackingSDO> apiResult = null;
                List<AcsCredentialTrackingSDO> trackingTemp = new List<AcsCredentialTrackingSDO>();
                gridViewMonitor.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).Get<List<AcsCredentialTrackingSDO>>(AcsRequestUriStore.ACS_TOKEN__GETCREDENTIALTRACKING, TrackingConfig.AcsConsumer, TrackingConfig.AppCode, paramCommon);
                if (apiResult != null)
                {
                    if (!String.IsNullOrEmpty(txtKeyword.Text))
                        apiResult = apiResult.Where(o =>
                             (o.Email ?? " ").ToLower().Contains(txtKeyword.Text.ToLower())
                             || (o.LoginAddress ?? "").ToLower().Contains(txtKeyword.Text.ToLower())
                             || (o.LoginName ?? "").ToLower().Contains(txtKeyword.Text.ToLower())
                             || (o.MachineName ?? " ").ToLower().Contains(txtKeyword.Text.ToLower())
                             || (o.Mobile ?? " ").ToLower().Contains(txtKeyword.Text.ToLower())
                             || (o.UserName ?? "").ToLower().Contains(txtKeyword.Text.ToLower())
                             || (o.VersionApp ?? " ").ToLower().Contains(txtKeyword.Text.ToLower())
                             )
                             .ToList();

                    apiResult = apiResult.OrderByDescending(o => o.LastAccessTime)
                        .GroupBy(o => new { o.LoginName, o.MachineName })
                        .Select(o => o.First())
                        .OrderByDescending(o => o.LastAccessTime)
                        .ToList();
                }

                gridViewMonitor.GridControl.DataSource = apiResult;
                gridViewMonitor.EndUpdate();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void gridViewMonitor_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    AcsCredentialTrackingSDO data = (AcsCredentialTrackingSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "LastAccessTimeDisplay")
                        {
                            e.Value = data.LastAccessTime.ToString("dd/MM/yyyy HH:mm:ss");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                LoadData();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadData();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void txtKeyword_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
    }
}
