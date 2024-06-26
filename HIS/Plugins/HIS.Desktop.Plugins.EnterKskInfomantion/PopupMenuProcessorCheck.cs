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
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System.Windows.Forms;
namespace HIS.Desktop.Plugins.EnterKskInfomantion
{
    delegate void MouseRightClick(object sender, ItemClickEventArgs e);
    class PopupMenuProcessorCheck
    {
        internal enum ItemType
        {
            InMps449
        }
        BarManager _BarManager = null;
        MouseRightClick _MouseRightClick;
        PopupMenu _PopupMenu = null;
        List<ADO.ServiceReqADO> ListAdos;

        internal PopupMenuProcessorCheck(BarManager barmanager, MouseRightClick mouseRightClick, List<ADO.ServiceReqADO> listAdos)
        {
            this._MouseRightClick = mouseRightClick;
            this._BarManager = barmanager;
            this.ListAdos = listAdos;
        }

        internal void InitMenu()
        {
            try
            {
                if (this.ListAdos == null || this.ListAdos.Count == 0 || this._BarManager == null || this._MouseRightClick == null)
                    return;

                if (this._PopupMenu == null)
                    this._PopupMenu = new PopupMenu(this._BarManager);
                this._PopupMenu.ItemLinks.Clear();

                List<BarItem> barItems = new List<BarItem>();

              
                BarButtonItem bbtInKemKetQua = new BarButtonItem(this._BarManager, Resources.ResourceMessage.InPhieu, 0);
                bbtInKemKetQua.Tag = ItemType.InMps449;
                bbtInKemKetQua.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                barItems.Add(bbtInKemKetQua);


                this._PopupMenu.AddItems(barItems.ToArray());
                this._PopupMenu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
