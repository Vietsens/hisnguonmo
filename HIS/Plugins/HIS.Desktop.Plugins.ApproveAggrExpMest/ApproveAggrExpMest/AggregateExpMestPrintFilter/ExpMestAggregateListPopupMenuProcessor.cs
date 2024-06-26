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
using HIS.Desktop.Plugins.ApproveAggrExpMest.Resources;
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ApproveAggrExpMest.ApproveAggrExpMest.AggregateExpMestPrintFilter
{
    delegate void ExpMestAggregateMouseRight_Click(object sender, ItemClickEventArgs e);

    internal class ExpMestAggregateListPopupMenuProcessor
    {
        MOS.EFMODEL.DataModels.V_HIS_AGGR_EXP_MEST currentTreatmentSDO;
        ExpMestAggregateMouseRight_Click expMestAggregatePrintClick;
        BarManager barManager;
        PopupMenu menu;

        internal enum PrintType
        {
            InTraDoiThuoc,
            InPhieuTongHop,
            InPhieuLinhThuocGayNghienHuongTT,
            InPhieuLinhThuoc,
        }

        internal ExpMestAggregateListPopupMenuProcessor(MOS.EFMODEL.DataModels.V_HIS_AGGR_EXP_MEST currentTreatment, ExpMestAggregateMouseRight_Click aggregatePrintClick, BarManager barManager)
        {
            try
            {
                this.currentTreatmentSDO = currentTreatment;
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

                BarButtonItem itemInTraDoiThuoc = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXP_MEST_AGGREGATE__IN_TRA_DOI_THUOC", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), 1);
                itemInTraDoiThuoc.Tag = PrintType.InTraDoiThuoc;
                itemInTraDoiThuoc.ItemClick += new ItemClickEventHandler(expMestAggregatePrintClick);

                BarButtonItem itemInPhieuTonHop = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXP_MEST_AGGREGATE__IN_PHIEU_TONG_HOP", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), 2);
                itemInPhieuTonHop.Tag = PrintType.InPhieuTongHop;
                itemInPhieuTonHop.ItemClick += new ItemClickEventHandler(expMestAggregatePrintClick);

                //BarButtonItem itemInPhieuLinhThuocGayNghienHuongTT = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXP_MEST_AGGREGATE__IN_PHIEU_LINH_THUOC_GAY_NGHIEN_HUONG_TAM_THAN", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), 3);
                //itemInPhieuLinhThuocGayNghienHuongTT.Tag = PrintType.InPhieuLinhThuocGayNghienHuongTT;
                //itemInPhieuLinhThuocGayNghienHuongTT.ItemClick += new ItemClickEventHandler(expMestAggregatePrintClick);

                BarButtonItem itemInPhieuLinhThuoc = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXP_MEST_AGGREGATE__IN_PHIEU_LINH_THUOC", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), 4);
                itemInPhieuLinhThuoc.Tag = PrintType.InPhieuLinhThuoc;
                itemInPhieuLinhThuoc.ItemClick += new ItemClickEventHandler(expMestAggregatePrintClick);

                menu.AddItems(new BarItem[] { itemInTraDoiThuoc, itemInPhieuTonHop, itemInPhieuLinhThuoc });
                menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
