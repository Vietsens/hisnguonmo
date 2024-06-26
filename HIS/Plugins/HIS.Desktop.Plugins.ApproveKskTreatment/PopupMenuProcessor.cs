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
using HIS.Desktop.Plugins.ApproveKskTreatment.Resources;
using Inventec.Common.LocalStorage.SdaConfig;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ApproveKskTreatment
{
    delegate void AggrExpMestMouseRightClick(object sender, ItemClickEventArgs e);
    class PopupMenuProcessor
    {
        V_HIS_TREATMENT_4 _ExpMest = null;
        BarManager _BarManager = null;
        PopupMenu _PopupMenu = null;
        AggrExpMestMouseRightClick _MouseRightClick;
        internal enum ItemType
        {
            PhieuCongKhaiTheoBN

        }

        internal PopupMenuProcessor(V_HIS_TREATMENT_4 expMest, BarManager barmanager, AggrExpMestMouseRightClick mouseRightClick)
        {
            this._ExpMest = expMest;
            this._MouseRightClick = mouseRightClick;
            this._BarManager = barmanager;
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

                //Phiếu thu thanh toán
                BarButtonItem bbtnPhieuCongKhaiBN = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_AGGREXMEST__POPUP_MENU__ITEM_PHIEU_CONG_KHAI_THEO_BN", ResourceLanguageManager.LanguageUCHisAggrExpMestList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 0);
                bbtnPhieuCongKhaiBN.Tag = ItemType.PhieuCongKhaiTheoBN;
                bbtnPhieuCongKhaiBN.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                this._PopupMenu.AddItems(new BarItem[] { bbtnPhieuCongKhaiBN });

                this._PopupMenu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
