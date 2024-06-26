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
using DevExpress.XtraEditors.Controls;
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Debate
{
    delegate void PrintMedicine_Click(object sender, ItemClickEventArgs e);

    class PrintDebateMenuProcessor
    {
        PrintMedicine_Click PrintMouseClick;
        BarManager barManager;
        PopupMenu menu;
        internal enum ModuleType
        {
            BIEN_BAN_HOI_CHAN,
            BIEN_BAN_HOI_CHAN_THUOC_DAU_SAO,
            SO_BIEN_BAN_HOI_CHAN,
            HOI_CHAN_PTTT
        }

        internal PrintDebateMenuProcessor(PrintMedicine_Click PrintMouseClick, BarManager barManager)
        {
            this.PrintMouseClick = PrintMouseClick;
            this.barManager = barManager;
        }

        internal void InitMenu()
        {
            try
            {
                if (menu == null)
                    menu = new PopupMenu(barManager);
                // Add item and show
                menu.ItemLinks.Clear();

                Resources.ResourceLanguageManager.LanguagefrmDebate = new ResourceManager("HIS.Desktop.Plugins.Debate.Resources.Lang", typeof(HIS.Desktop.Plugins.Debate.frmDebate).Assembly);

                BarButtonItem BienBanHoiChan = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("frmDebate.Print.TrichBienBan", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture()), 1);
                BienBanHoiChan.Tag = ModuleType.BIEN_BAN_HOI_CHAN;
                BienBanHoiChan.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                menu.AddItems(new BarItem[] { BienBanHoiChan });


                BarButtonItem soBienBanHoiChanDauSao = new BarButtonItem(barManager,
                    Inventec.Common.Resource.Get.Value(
                    "frmDebate.Print.BienBanHoiChanThuocDauSao",
                    Resources.ResourceLanguageManager.LanguagefrmDebate,
                    LanguageManager.GetCulture()), 1);
                soBienBanHoiChanDauSao.Tag = ModuleType.BIEN_BAN_HOI_CHAN_THUOC_DAU_SAO;
                soBienBanHoiChanDauSao.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                menu.AddItems(new BarItem[] { soBienBanHoiChanDauSao });

                BarButtonItem SoBienBanHoiChan = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("frmDebate.Print.SoBienBan", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture()), 2);
                SoBienBanHoiChan.Tag = ModuleType.SO_BIEN_BAN_HOI_CHAN;
                SoBienBanHoiChan.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                menu.AddItems(new BarItem[] { SoBienBanHoiChan });

                BarButtonItem hoiChanPttt = new BarButtonItem(barManager, "Biên bản hội chẩn trước phẫu thuật", 2);
                hoiChanPttt.Tag = ModuleType.HOI_CHAN_PTTT;
                hoiChanPttt.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                menu.AddItems(new BarItem[] { hoiChanPttt });

                menu.ShowPopup(Cursor.Position);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
