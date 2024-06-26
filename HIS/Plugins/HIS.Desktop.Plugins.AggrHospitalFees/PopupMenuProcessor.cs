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
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AggrHospitalFees
{
    delegate void TransactionMouseRightClick(object sender, ItemClickEventArgs e);
    class PopupMenuProcessor
    {
        V_HIS_TRANSACTION _Transaction = null;
        BarManager _BarManager = null;
        PopupMenu _PopupMenu = null;
        TransactionMouseRightClick _MouseRightClick;
        Inventec.Desktop.Common.Modules.Module CurrentModule;

        internal enum ItemType
        {
            
            HuyHoaDon
        }

        internal PopupMenuProcessor(V_HIS_TRANSACTION transaction, BarManager barmanager, TransactionMouseRightClick mouseRightClick, Inventec.Desktop.Common.Modules.Module currentModule)
        {
            this._Transaction = transaction;
            this._MouseRightClick = mouseRightClick;
            this._BarManager = barmanager;
            this.CurrentModule = currentModule;
        }

        internal void InitMenu()
        {
            try
            {
                if (this._Transaction == null || this._BarManager == null || this._MouseRightClick == null)
                    return;
                //if (this._Transaction.IS_CANCEL == 1)
                //{
                //    Inventec.Common.Logging.LogSystem.Info("giao dich da bi huy: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this._Transaction), this._Transaction));
                //    return;
                //}
                if (this._PopupMenu == null)
                    this._PopupMenu = new PopupMenu(this._BarManager);
                this._PopupMenu.ItemLinks.Clear();


                BarButtonItem bbtnPhieuThuThanhToan = new BarButtonItem(this._BarManager, "Phiếu yêu cầu hủy hóa đơn", 0);
                bbtnPhieuThuThanhToan.Tag = ItemType.HuyHoaDon;
                bbtnPhieuThuThanhToan.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._PopupMenu.AddItems(new BarItem[] { bbtnPhieuThuThanhToan });

                this._PopupMenu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
