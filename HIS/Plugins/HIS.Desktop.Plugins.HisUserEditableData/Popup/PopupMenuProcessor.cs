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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraBars;
using System.Windows.Forms;
namespace HIS.Desktop.Plugins.HisUserEditableData.Popup
{
    delegate void MouseRightClick(object sender, ItemClickEventArgs e);

    class PopupMenuProcessor
    {
        BarManager _Barmanager = null;
        DevExpress.XtraBars.PopupMenu _Menu = null;
        MouseRightClick _MouseRightClick;
        internal PopupMenuProcessor(BarManager barManager, MouseRightClick mouseRightClick)
        {
            this._Barmanager = barManager;
            this._MouseRightClick = mouseRightClick;
        }
        public enum ItemType
        { 
            Copy,Paste
        }
        internal void InitMenu()
        {
            try
            { 
                if(this._Barmanager == null || this._MouseRightClick == null)
                    return;
                if(this._Menu ==null)
                    this._Menu = new PopupMenu(this._Barmanager);

                //barBtn copy
                BarButtonItem btnCopy = new BarButtonItem(this._Barmanager,"Copy",0);
                btnCopy.Tag = ItemType.Copy;
                btnCopy.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItems(new BarItem[]{btnCopy});

                //bar btn paste
                BarButtonItem btnPaste = new BarButtonItem(this._Barmanager,"Paste",1);
                btnPaste.Tag = ItemType.Paste;
                btnPaste.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItems(new BarItem[] {btnPaste});

                this._Menu.ShowPopup(Cursor.Position);
            }
            catch(Exception e)
            {
                Inventec.Common.Logging.LogSystem.Error(e);
            }
        }
    }
}
