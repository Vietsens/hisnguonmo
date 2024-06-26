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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.MetyMaty.ADO;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MetyMaty
{
    delegate void MouseRight_Click(object sender, ItemClickEventArgs e);

    class PopupMenuProcessor
    {
        MaterialTypeADO _Data;
        MouseRight_Click _MouseRightClick;
        BarManager barManager;
        PopupMenu menu;
        internal enum ModuleType
        {
            T1,

        }
        internal ModuleType moduleType { get; set; }

        internal PopupMenuProcessor(MaterialTypeADO _data, MouseRight_Click MouseRightClick, BarManager barManager)
        {
            this._Data = _data;
            this._MouseRightClick = MouseRightClick;
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
                BarButtonItem itemT1 = new BarButtonItem(barManager, "Đổi chế phẩm", 1);
                itemT1.Tag = ModuleType.T1;
                itemT1.ItemClick += new ItemClickEventHandler(_MouseRightClick);
                menu.AddItems(new BarItem[] { itemT1 });

                menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
