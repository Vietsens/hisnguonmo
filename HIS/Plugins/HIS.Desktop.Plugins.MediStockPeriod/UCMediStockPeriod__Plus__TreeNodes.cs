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
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.Controls.Session;
using Inventec.Desktop.Common.Message;

namespace HIS.Desktop.Plugins.MediStockPeriod
{
    public partial class UCMediStockPeriod : UserControl
    {
        private void medicineType_NodeCellStyle(V_HIS_MEST_PERIOD_METY data, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                if (data != null)
                {
                    if (data.IN_AMOUNT < 0 || data.OUT_AMOUNT < 0 || data.BEGIN_AMOUNT < 0 || data.VIR_END_AMOUNT < 0 || data.INVENTORY_AMOUNT < 0 || data.VIR_END_AMOUNT != data.INVENTORY_AMOUNT)
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void materialType_NodeCellStyle(V_HIS_MEST_PERIOD_MATY data, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                if (data != null)
                {
                    if (data.IN_AMOUNT < 0 || data.OUT_AMOUNT < 0 || data.BEGIN_AMOUNT < 0 || data.VIR_END_AMOUNT < 0 || data.INVENTORY_AMOUNT < 0 || data.VIR_END_AMOUNT != data.INVENTORY_AMOUNT)
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bltyType_NodeCellStyle(V_HIS_MEST_PERIOD_BLTY data, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                if (data != null)
                {
                    if (data.IN_AMOUNT < 0 || data.OUT_AMOUNT < 0 || data.BEGIN_AMOUNT < 0 || data.VIR_END_AMOUNT < 0 || data.INVENTORY_AMOUNT < 0 || data.VIR_END_AMOUNT != data.INVENTORY_AMOUNT)
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
