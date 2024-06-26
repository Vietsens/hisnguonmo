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
using HIS.Desktop.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpMestSaleCreate
{
    public partial class frmExpMestSaleCreate : FormBase
    {
        private long roomTypeId { get; set; }
        private long roomId { get; set; }
        private UCExpMestSaleCreate ucExpMestSaleCreate { get; set; }
        private long? expMestId { get; set; }

        public frmExpMestSaleCreate()
        {
            InitializeComponent();
        }

        public frmExpMestSaleCreate(long roomTypeId, long roomId, long? expMestId)
        {
            InitializeComponent();
            try
            {
                this.roomTypeId = roomTypeId;
                this.roomId = roomId;
                this.expMestId = expMestId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmExpMestSaleCreate_Load(object sender, EventArgs e)
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(Inventec.Desktop.Common.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                //ucExpMestSaleCreate = new UCExpMestSaleCreate(roomTypeId, roomId, expMestId);
                //ucExpMestSaleCreate.Dock = DockStyle.Fill;
                //panelExpMestSaleCreate.Controls.Add(ucExpMestSaleCreate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmExpMestSaleCreate_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (ucExpMestSaleCreate != null)
                {
                    ucExpMestSaleCreate.FromClosingEvent();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
