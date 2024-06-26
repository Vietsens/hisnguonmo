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
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.LocalStorage.SdaConfig;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExecuteRoom
{
    delegate void TransactionMouseRightClick(object sender, ItemClickEventArgs e);
    class PopupMenuProcessor
    {
        V_HIS_SERE_SERV_6 _sereServRowMenu = null;
        BarManager _BarManager = null;
        PopupMenu _PopupMenu = null;
        TransactionMouseRightClick _MouseRightClick;
        ADO.ServiceReqADO currentHisServiceReq;
        internal enum ItemType
        {
            NhapThongTinPhim,
            KeThuocVatTu,
            ChonMayXuLy,
            DoiDichVu
        }

        internal PopupMenuProcessor(V_HIS_SERE_SERV_6 ss, BarManager barmanager, TransactionMouseRightClick mouseRightClick, ADO.ServiceReqADO currentHisServiceReq)
        {
            this._sereServRowMenu = ss;
            this._MouseRightClick = mouseRightClick;
            this._BarManager = barmanager;
            this.currentHisServiceReq = currentHisServiceReq;
        }

        internal void InitMenu()
        {
            try
            {
                //string _LoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                if (this._sereServRowMenu == null || this._BarManager == null || this._MouseRightClick == null)
                    return;
                if (this._PopupMenu == null)
                    this._PopupMenu = new PopupMenu(this._BarManager);
                this._PopupMenu.ItemLinks.Clear();

                //if (this._sereServRowMenu.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA)
                //{
                //    BarButtonItem btnNhapThongTinPhim = new BarButtonItem(this._BarManager, "Nhập thông tin phim", 0);
                //    btnNhapThongTinPhim.Tag = ItemType.NhapThongTinPhim;
                //    btnNhapThongTinPhim.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                //    this._PopupMenu.AddItems(new BarItem[] { btnNhapThongTinPhim });
                //}
                if (this._sereServRowMenu.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS
                    || this._sereServRowMenu.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA
                    || this._sereServRowMenu.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN
                    || this._sereServRowMenu.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN
                    || this._sereServRowMenu.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA
                    )
                {
                    if (currentHisServiceReq != null && currentHisServiceReq.SERVICE_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                    {
                        BarButtonItem btnKeDonThuoc = new BarButtonItem(this._BarManager, "Kê đơn cận lâm sàng", 0);
                        btnKeDonThuoc.Tag = ItemType.KeThuocVatTu;
                        btnKeDonThuoc.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                        this._PopupMenu.AddItems(new BarItem[] { btnKeDonThuoc });

                        if (this.currentHisServiceReq != null && currentHisServiceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                        {
                            BarButtonItem btnDoiDichVu = new BarButtonItem(this._BarManager, "Yêu cầu đổi dịch vụ", 0);
                            btnDoiDichVu.Tag = ItemType.DoiDichVu;
                            btnDoiDichVu.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                            this._PopupMenu.AddItems(new BarItem[] { btnDoiDichVu });
                        }
                    }
                    BarButtonItem btnChonmayxl = new BarButtonItem(this._BarManager, "Chọn máy xử lý cho các dịch vụ được chọn", 0);
                    btnChonmayxl.Tag = ItemType.ChonMayXuLy;
                    btnChonmayxl.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    this._PopupMenu.AddItems(new BarItem[] { btnChonmayxl });
                }
                this._PopupMenu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
