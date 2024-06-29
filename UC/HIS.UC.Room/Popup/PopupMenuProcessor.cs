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

namespace HIS.UC.Room.Popup
{
    delegate void TransactionMouseRightClick(object sender, ItemClickEventArgs e);
    public class PopupMenuProcessor
    {
        BarManager _BarManager = null;
        DevExpress.XtraBars.PopupMenu _Menu = null;
        TransactionMouseRightClick _MouseRightClick;
        internal PopupMenuProcessor(BarManager barManager, TransactionMouseRightClick mouseRightClick)
        {
            this._BarManager = barManager;
            this._MouseRightClick = mouseRightClick;
        }

        public enum ItemType
        {
            CopyPhongSangPhong,
            PastePhongSangPhong
        }

        internal void InitMenu()
        {
            try
            {
                if (this._BarManager == null || this._MouseRightClick == null)
                    return;
                if (this._Menu == null)
                    this._Menu = new PopupMenu(this._BarManager);

                //Copy phòng sang phòng
                BarButtonItem bbtnCopyPhongSangPhong = new BarButtonItem(this._BarManager, "Copy", 0);
                bbtnCopyPhongSangPhong.Tag = ItemType.CopyPhongSangPhong;
                bbtnCopyPhongSangPhong.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItems(new BarItem[] { bbtnCopyPhongSangPhong });

                //Dán phòng sang phòng
                BarButtonItem bbtnDanPhongSangPhong = new BarButtonItem(this._BarManager, "Paste", 1);
                bbtnDanPhongSangPhong.Tag = ItemType.PastePhongSangPhong;
                bbtnDanPhongSangPhong.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItems(new BarItem[] { bbtnDanPhongSangPhong });

                this._Menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
