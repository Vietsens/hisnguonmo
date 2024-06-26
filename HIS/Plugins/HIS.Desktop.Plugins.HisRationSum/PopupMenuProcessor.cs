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

namespace HIS.Desktop.Plugins.HisRationSum
{
    delegate void TransactionMouseRightClick(object sender, ItemClickEventArgs e);
    class PopupMenuProcessor
    {
        BarManager _BarManager = null;
        PopupMenu _Menu = null;
        TransactionMouseRightClick _MouseRightClick;
        V_HIS_TREATMENT_FEE currentTreatment;
        internal PopupMenuProcessor(BarManager barManager, TransactionMouseRightClick mouseRightClick, V_HIS_TREATMENT_FEE _currentTreatment)
        {
            this._BarManager = barManager;
            this._MouseRightClick = mouseRightClick;
            this.currentTreatment = _currentTreatment;
        }

        internal enum ItemType
        {
            InTongHop,
            InChiTiet,
            ChiaSuatAn
        }

        internal void InitMenu()
        {
            try
            {
                if (this._BarManager == null || this._MouseRightClick == null)
                    return;
                if (this._Menu == null)
                    this._Menu = new PopupMenu(this._BarManager);

                //in tổng hợp
                BarButtonItem bbtnInBangKeChiPhiKCB = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_RATION_SUM__POPUP_MENU__ITEM_PHIEU_TONG_HOP", Base.ResourceLangManager.LanguageUCHisRationSum, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 0);
                bbtnInBangKeChiPhiKCB.Tag = ItemType.InTongHop;
                bbtnInBangKeChiPhiKCB.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItems(new BarItem[] { bbtnInBangKeChiPhiKCB });

                //In chi tiết
                BarButtonItem bbtnPhieuGiuThe = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_RATION_SUM__POPUP_MENU__ITEM_PHIEU_CHI_TIET", Base.ResourceLangManager.LanguageUCHisRationSum, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 1);
                bbtnPhieuGiuThe.Tag = ItemType.InChiTiet;
                bbtnPhieuGiuThe.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItems(new BarItem[] { bbtnPhieuGiuThe });

                //BarButtonItem bbtnPhieuChiaSuatAn = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_RATION_SUM__POPUP_MENU__ITEM_CHIA_SUAT_AN", Base.ResourceLangManager.LanguageUCHisRationSum, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 1);
                //bbtnPhieuChiaSuatAn.Tag = ItemType.ChiaSuatAn;
                //bbtnPhieuChiaSuatAn.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                //this._Menu.AddItems(new BarItem[] { bbtnPhieuChiaSuatAn });

                this._Menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
