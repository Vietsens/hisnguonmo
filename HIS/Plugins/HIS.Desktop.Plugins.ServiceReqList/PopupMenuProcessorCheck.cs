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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.IsAdmin;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServiceReqList
{
    class PopupMenuProcessorCheck 
    {
        internal enum ItemType
        {
            In,
            Xoa,
            ChuyenPhong, 
            InKemKetQua,
            KetQuaHeThongBenhAnhDienTu
        }

        BarManager _BarManager = null;
        MouseRightClick _MouseRightClick;
        PopupMenu _PopupMenu = null;
        List<ADO.ServiceReqADO> ListAdos;
        V_HIS_ROOM currentRoom;
        string loginName = null;
        List<ADO.ServiceReqADO> lstSerSelected = null;

        internal PopupMenuProcessorCheck(BarManager barmanager, MouseRightClick mouseRightClick, List<ADO.ServiceReqADO> listAdos, string loginname, V_HIS_ROOM currentRoom)
        {
            this._MouseRightClick = mouseRightClick;
            this._BarManager = barmanager;
            this.ListAdos = listAdos;
            this.loginName = loginname;
            this.currentRoom = currentRoom;
        }
        internal PopupMenuProcessorCheck(BarManager barmanager, MouseRightClick mouseRightClick, List<ADO.ServiceReqADO> listAdos, string loginname, V_HIS_ROOM currentRoom, List<ADO.ServiceReqADO> _lstSerSelected)
        {
            this._MouseRightClick = mouseRightClick;
            this._BarManager = barmanager;
            this.ListAdos = listAdos;
            this.loginName = loginname;
            this.currentRoom = currentRoom;
            this.lstSerSelected = _lstSerSelected;
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

                //in tất cả 
                BarButtonItem bbtIn = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("ServiceReq.RightMenu.PrintSelectedItem", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture()), 0);
                bbtIn.Tag = ItemType.In;
                bbtIn.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                barItems.Add(bbtIn);

                List<HIS_SERVICE_REQ> ServiceReqSttId = new List<HIS_SERVICE_REQ>();

                if (lstSerSelected != null && lstSerSelected.Count() > 0)
                {
                    HisServiceReqFilter filter = new HisServiceReqFilter();
                    filter.IDs = lstSerSelected.Select(o => o.ID).ToList();
                    ServiceReqSttId = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>(RequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, filter, new Inventec.Core.CommonParam());
                }
                var check = ServiceReqSttId.Where(o => o.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL);
                if (check.Count() == 0 )
                {
                    BarButtonItem KetQuaHeThongBenhAnhDienTu_ = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("ServiceReq.RightMenu.KetQuaHeThongBenhAnhDienTu", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture()), 0);
                    KetQuaHeThongBenhAnhDienTu_.Tag = ItemType.KetQuaHeThongBenhAnhDienTu;
                    KetQuaHeThongBenhAnhDienTu_.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    barItems.Add(KetQuaHeThongBenhAnhDienTu_);
                }

                bool isDelete = true;
                foreach (var ado in ListAdos)
                {
                    if (!((ado.CREATOR == this.loginName || ado.REQUEST_LOGINNAME == this.loginName || CheckLoginAdmin.IsAdmin(loginName) || (this.currentRoom != null && ado.REQUEST_DEPARTMENT_ID == this.currentRoom.DEPARTMENT_ID && ado.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH))
                           && (ado.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)))
                    {
                        isDelete = false;
                        break;
                    }
                }

                if (isDelete)
                {
                    //xóa tất cả 
                    BarButtonItem bbtXoa = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("ServiceReq.RightMenu.DeleteSelectedItem", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture()), 0);
                    bbtXoa.Tag = ItemType.Xoa;
                    bbtXoa.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    barItems.Add(bbtXoa);
                }
                BarButtonItem bbtInKemKetQua = new BarButtonItem(this._BarManager, "In phiếu chỉ định kèm phiếu kết quả", 0);
                bbtInKemKetQua.Tag = ItemType.InKemKetQua;
                bbtInKemKetQua.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                barItems.Add(bbtInKemKetQua);


                BarButtonItem bbtChangeRoom = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("ServiceReq.RightMenu.ChangeRoom", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture()), 0);
                bbtChangeRoom.Tag = ItemType.ChuyenPhong;
                bbtChangeRoom.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                barItems.Add(bbtChangeRoom);





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
