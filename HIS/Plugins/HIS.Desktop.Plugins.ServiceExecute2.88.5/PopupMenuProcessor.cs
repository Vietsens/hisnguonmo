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
using HIS.Desktop.Common;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServiceExecute
{
    delegate void ServiceMouseRightClick(object sender, ItemClickEventArgs e);
    class PopupMenuProcessor
    {
        BarManager _BarManager = null;
        PopupMenu _Menu = null;
        ServiceMouseRightClick _MouseRightClick;
        ADO.ServiceADO _ServicePoppupPrint;
        RefeshReference BtnRefreshPhimTat;
        internal PopupMenuProcessor(ADO.ServiceADO currentServiceADO, BarManager barManager, ServiceMouseRightClick mouseRightClick, RefeshReference _BtnRefreshPhimTat)
        {
            this._BarManager = barManager;
            this._MouseRightClick = mouseRightClick;
            this._ServicePoppupPrint = currentServiceADO;
            this.BtnRefreshPhimTat = _BtnRefreshPhimTat;
        }

        internal enum ItemType
        {           
            EkipExecute,
            PhieuKeKhaiThuocVatTuTieuHao,
            PhieuPTTT
        }

        internal void InitMenu()
        {
            try
            {
                if (this._BarManager == null || this._MouseRightClick == null)
                    return;
                if (this._Menu == null)
                    this._Menu = new PopupMenu(this._BarManager);

                //if (this._ServicePoppupPrint.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA
                //    || this._ServicePoppupPrint.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS
                //    || this._ServicePoppupPrint.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA
                //    || this._ServicePoppupPrint.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN)
                //{
                    BarButtonItem bbtnEkipExecute = new BarButtonItem(this._BarManager, "Thông tin thực hiện CLS", 1);
                    bbtnEkipExecute.Tag = ItemType.EkipExecute;
                    bbtnEkipExecute.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    this._Menu.AddItems(new BarItem[] { bbtnEkipExecute });

                    BarButtonItem bbtnPhieuKeKhaiThuocVatTu = new BarButtonItem(this._BarManager, "Phiếu kê khai thuốc vật tư hao phí", 1);
                    bbtnPhieuKeKhaiThuocVatTu.Tag = ItemType.PhieuKeKhaiThuocVatTuTieuHao;
                    bbtnPhieuKeKhaiThuocVatTu.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    this._Menu.AddItems(new BarItem[] { bbtnPhieuKeKhaiThuocVatTu });

                    BarButtonItem bbtnPhieuPTTT = new BarButtonItem(this._BarManager, "Phiếu phẫu thuật thủ thuật", 1);
                    bbtnPhieuPTTT.Tag = ItemType.PhieuPTTT;
                    bbtnPhieuPTTT.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    this._Menu.AddItems(new BarItem[] { bbtnPhieuPTTT });
                    
                    this._Menu.ShowPopup(Cursor.Position);
                //}              
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
