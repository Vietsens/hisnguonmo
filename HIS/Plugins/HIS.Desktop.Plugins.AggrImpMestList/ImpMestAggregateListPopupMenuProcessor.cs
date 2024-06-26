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

namespace HIS.Desktop.Plugins.AggrImpMestList
{
    delegate void ImpMestAggregateMouseRight_Click(object sender, ItemClickEventArgs e);

    internal class ImpMestAggregateListPopupMenuProcessor
    {
        MOS.EFMODEL.DataModels.V_HIS_IMP_MEST currentTreatmentSDO;
        ImpMestAggregateMouseRight_Click impMestAggregatePrintClick;
        BarManager barManager;
        PopupMenu menu;
        List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST> CheckedImpMest;

        internal enum PrintType
        {
            InTraDoiThuocTongHop = 7
        }

        internal ImpMestAggregateListPopupMenuProcessor(MOS.EFMODEL.DataModels.V_HIS_IMP_MEST currentTreatment, List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST> checkedImpMest, ImpMestAggregateMouseRight_Click aggregatePrintClick, BarManager barManager)
        {
            try
            {
                this.currentTreatmentSDO = currentTreatment;
                this.impMestAggregatePrintClick = aggregatePrintClick;
                this.CheckedImpMest = checkedImpMest;
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

                if (this.CheckedImpMest != null && this.CheckedImpMest.Count > 0 && this.CheckedImpMest.Exists(o => o.ID == this.currentTreatmentSDO.ID))
                {
                    BarButtonItem itemPhieuTraTongHop = new BarButtonItem(barManager, "In phiếu trả thuốc/vt tổng hợp", 1);
                    itemPhieuTraTongHop.Tag = PrintType.InTraDoiThuocTongHop;
                    itemPhieuTraTongHop.ItemClick += new ItemClickEventHandler(impMestAggregatePrintClick);

                    menu.AddItems(new BarItem[] { itemPhieuTraTongHop });
                    menu.ShowPopup(Cursor.Position);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
