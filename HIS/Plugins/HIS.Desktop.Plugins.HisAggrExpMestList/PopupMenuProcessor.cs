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
using HIS.Desktop.Plugins.HisAggrExpMestList.Resources;
using Inventec.Common.LocalStorage.SdaConfig;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisAggrExpMestList
{
    delegate void AggrExpMestMouseRightClick(object sender, ItemClickEventArgs e);
    class PopupMenuProcessor
    {
        V_HIS_EXP_MEST_3 _ExpMest = null;
        BarManager _BarManager = null;
        PopupMenu _PopupMenu = null;
        AggrExpMestMouseRightClick _MouseRightClick;
        List<V_HIS_EXP_MEST_3> CheckedExpMest;

        internal enum ItemType
        {
            PhieuCongKhaiTheoBN,
            InPhieuLinhTongHop,
            InTraDoiThuocTongHop

        }

        internal PopupMenuProcessor(V_HIS_EXP_MEST_3 expMest, List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_3> checkedExpMest, BarManager barmanager, AggrExpMestMouseRightClick mouseRightClick)
        {
            this._ExpMest = expMest;
            this._MouseRightClick = mouseRightClick;
            this._BarManager = barmanager;
            this.CheckedExpMest = checkedExpMest;
        }

        internal void InitMenu()
        {
            try
            {
                if (this._ExpMest == null || this._BarManager == null || this._MouseRightClick == null)
                    return;
                if (this._PopupMenu == null)
                    this._PopupMenu = new PopupMenu(this._BarManager);
                this._PopupMenu.ItemLinks.Clear();

                if (this.CheckedExpMest != null && this.CheckedExpMest.Count > 0 && this.CheckedExpMest.Exists(o => o.ID == this._ExpMest.ID))
                {
                    BarButtonItem itemInPhieuTongHop = new BarButtonItem(_BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_AGGREXMEST__POPUP_MENU__IN_PHIEU_LINH_TONG_HOP", ResourceLanguageManager.LanguageUCHisAggrExpMestList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 1);
                    itemInPhieuTongHop.Tag = ItemType.InPhieuLinhTongHop;
                    itemInPhieuTongHop.ItemClick += new ItemClickEventHandler(_MouseRightClick);

                    BarButtonItem itemInTraDoiThuoc = new BarButtonItem(_BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_AGGREXMEST__POPUP_MENU__IN_TRA_DOI_THUOC_TONG_HOP", ResourceLanguageManager.LanguageUCHisAggrExpMestList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 2);
                    itemInTraDoiThuoc.Tag = ItemType.InTraDoiThuocTongHop;
                    itemInTraDoiThuoc.ItemClick += new ItemClickEventHandler(_MouseRightClick);
                    _PopupMenu.AddItems(new BarItem[] { itemInPhieuTongHop, itemInTraDoiThuoc });
                    _PopupMenu.ShowPopup(Cursor.Position);
                }
                else if (this.CheckedExpMest == null)
                {
                    //Phiếu thu thanh toán
                    BarButtonItem bbtnPhieuCongKhaiBN = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_AGGREXMEST__POPUP_MENU__ITEM_PHIEU_CONG_KHAI_THEO_BN", ResourceLanguageManager.LanguageUCHisAggrExpMestList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 0);
                    bbtnPhieuCongKhaiBN.Tag = ItemType.PhieuCongKhaiTheoBN;
                    bbtnPhieuCongKhaiBN.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    this._PopupMenu.AddItems(new BarItem[] { bbtnPhieuCongKhaiBN });
                    this._PopupMenu.ShowPopup(Cursor.Position);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
