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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ImpMestAggregate
{
    delegate void ImpMestAggregateMouseRight_Click(object sender, ItemClickEventArgs e);

    internal class ImpMestAggregateListPopupMenuProcessor
    {
        MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2 currentTreatmentSDO;
        ImpMestAggregateMouseRight_Click impMestAggregatePrintClick;
        BarManager barManager;
        PopupMenu menu;
        List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2> CheckedImpMest;
        List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2> listCurrentTreatment;

        internal enum PrintType
        {
            InTraDoiThuoc = 1,
            InPhieuTraTongHop = 2,
            InPhieuTraThuocGayNghienHuongTT = 3,
            InPhieuTraThuoc = 4,
            TheoBenhNhan = 5,
            InTraDoiThuocTongHop = 6
        }

        internal ImpMestAggregateListPopupMenuProcessor(MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2 currentTreatment, List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2> checkedImpMest, ImpMestAggregateMouseRight_Click aggregatePrintClick, BarManager barManager)
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
                else if (this.CheckedImpMest == null)
                {
                    BarButtonItem itemInTraDoiThuoc = new BarButtonItem(barManager, "In phiếu trả tra đổi thuốc", 1);
                    itemInTraDoiThuoc.Tag = PrintType.InTraDoiThuoc;
                    itemInTraDoiThuoc.ItemClick += new ItemClickEventHandler(impMestAggregatePrintClick);

                    BarButtonItem itemInPhieuTonHop = new BarButtonItem(barManager, "In phiếu trả thuốc tổng hợp", 2);
                    itemInPhieuTonHop.Tag = PrintType.InPhieuTraTongHop;
                    itemInPhieuTonHop.ItemClick += new ItemClickEventHandler(impMestAggregatePrintClick);

                    BarButtonItem itemInPhieuTraThuoscgayNghien = new BarButtonItem(barManager, "In phiếu trả thuốc gây nghiện, hướng thần", 3);
                    itemInPhieuTraThuoscgayNghien.Tag = PrintType.InPhieuTraThuocGayNghienHuongTT;
                    itemInPhieuTraThuoscgayNghien.ItemClick += new ItemClickEventHandler(impMestAggregatePrintClick);

                    BarButtonItem itemPhieuTraChiTiet = new BarButtonItem(barManager, "In phiếu trả thuốc, vật tư", 4);
                    itemPhieuTraChiTiet.Tag = PrintType.InPhieuTraThuoc;
                    itemPhieuTraChiTiet.ItemClick += new ItemClickEventHandler(impMestAggregatePrintClick);

                    BarButtonItem itemPhieuTraTheoBenhNhan = new BarButtonItem(barManager, "In phiếu trả theo bệnh nhân", 5);
                    itemPhieuTraTheoBenhNhan.Tag = PrintType.TheoBenhNhan;
                    itemPhieuTraTheoBenhNhan.ItemClick += new ItemClickEventHandler(impMestAggregatePrintClick);

                    menu.AddItems(new BarItem[] { itemInPhieuTonHop, itemInTraDoiThuoc, itemInPhieuTraThuoscgayNghien, itemPhieuTraChiTiet, itemPhieuTraTheoBenhNhan });
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
