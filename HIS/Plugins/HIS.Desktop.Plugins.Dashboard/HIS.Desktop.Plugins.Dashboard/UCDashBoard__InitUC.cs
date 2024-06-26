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
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using MOS.SDO;
using DevExpress.XtraCharts;
using System.Globalization;
using HIS.Desktop.LocalStorage.ConfigApplication;
using System.Runtime.InteropServices;
using HIS.Desktop.Plugins.Dashboard.Base;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Plugins.Dashboard.UCFilter;

namespace HIS.Desktop.Plugins.Dashboard
{
    public partial class UCDashBoard : UserControl
    {
        UCFilterByDay ucFilterByDay { get; set; }
        UCFilterByWeek ucFilterByWeek { get; set; }
        UCFilterByMonth ucFilterByMonth { get; set; }


        private void InitUcFilterByDay()
        {
            try
            {
                ucFilterByDay = new UCFilterByDay();
                panelFilter.Controls.Clear();
                ucFilterByDay.Dock = DockStyle.Fill;
                lciPanel.Size = new Size(518,24);
                panelFilter.Controls.Add(ucFilterByDay);
                btnThongKe.Visible = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitUcFilterByWeek()
        {
            try
            {
                ucFilterByWeek = new UCFilterByWeek();
                panelFilter.Controls.Clear();
                ucFilterByWeek.Dock = DockStyle.Fill;
                lciPanel.Size = new Size(390, 24);
                panelFilter.Controls.Add(ucFilterByWeek);
                btnThongKe.Visible = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitUcFilterByMonth()
        {
            try
            {
                ucFilterByMonth = new UCFilterByMonth();
                panelFilter.Controls.Clear();
                ucFilterByMonth.Dock = DockStyle.Fill;
                lciPanel.Size = new Size(390, 24);
                panelFilter.Controls.Add(ucFilterByMonth);
                btnThongKe.Visible = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
