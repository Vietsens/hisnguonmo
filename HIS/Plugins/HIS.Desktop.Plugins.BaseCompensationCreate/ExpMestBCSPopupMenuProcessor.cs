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
using DevExpress.XtraBars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraBars;

namespace HIS.Desktop.Plugins.BaseCompensationCreate
{
    delegate void ExpMestAggregateMouseRight_Click(object sender, ItemClickEventArgs e);

    internal class ExpMestBCSPopupMenuProcessor
    {
        MOS.EFMODEL.DataModels.V_HIS_EXP_MEST _ExpMestMouseRight;
        ExpMestAggregateMouseRight_Click expMestAggregatePrintClick;
        BarManager barManager;
        PopupMenu menu;

        internal enum PrintType
        {
            InTraDoiThuoc,
            PhieuBuCoSo
            
        }

        internal ExpMestBCSPopupMenuProcessor(MOS.EFMODEL.DataModels.V_HIS_EXP_MEST _expMest, ExpMestAggregateMouseRight_Click aggregatePrintClick, BarManager barManager)
        {
            try
            {
                this._ExpMestMouseRight = _expMest;
                this.expMestAggregatePrintClick = aggregatePrintClick;
                this.barManager = barManager;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void InitMenu()
        {
            try
            {
                if (menu == null)
                    menu = new PopupMenu(barManager);

                menu.ItemLinks.Clear();

                BarButtonItem itemInTraDoiBCS = new BarButtonItem(barManager, "Phiếu tra đối bù cơ số", 2);
                itemInTraDoiBCS.Tag = PrintType.InTraDoiThuoc;
                itemInTraDoiBCS.ItemClick += new ItemClickEventHandler(expMestAggregatePrintClick);

                BarButtonItem itemInPhieuLinhThuoc = new BarButtonItem(barManager, "Phiếu bù cơ số", 4);
                itemInPhieuLinhThuoc.Tag = PrintType.PhieuBuCoSo;
                itemInPhieuLinhThuoc.ItemClick += new ItemClickEventHandler(expMestAggregatePrintClick);

                menu.AddItems(new BarItem[] {itemInTraDoiBCS, itemInPhieuLinhThuoc });
                menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
