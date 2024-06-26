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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ServiceReqList.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServiceReqList
{
    delegate void MouseRightClick(object sender, ItemClickEventArgs e);
    class PopupMenuProcessorMedicine
    {
        public ListMedicineADO listMedicineAdo;
        BarManager _BarManager = null;
        PopupMenu _PopupMenu = null;
        MouseRightClick _MouseRightClick;
        WorkPlaceSDO _CurrentWorkPlace;

        internal enum ItemType
        {
            SuaHuongDanSuDung,
            AssignPres,
            AssignPresCabinet,
            PrintServiceReq,
            AssignInKip,
            AssignOutKip,
            AcceptNoExecute,
            UnacceptNoExecute,
            // KetQuaHeThongBenhAnhDienTu,
        }

        internal PopupMenuProcessorMedicine(ListMedicineADO data, BarManager barmanager, MouseRightClick mouseRightClick, WorkPlaceSDO currentWorkPlace)
        {
            this.listMedicineAdo = data;
            this._MouseRightClick = mouseRightClick;
            this._BarManager = barmanager;
            this._CurrentWorkPlace = currentWorkPlace;
        }

        internal void InitMenu()
        {
            try
            {
                if (this.listMedicineAdo == null || this._BarManager == null || this._MouseRightClick == null)
                    return;


                //14357 cho suwae hdsd với đơn ngoài kho bỏ check theo kind
                //if (this.listMedicineAdo.ExpMestMedicineId <= 0)
                //{
                //    return;
                //}
                //if (this.listMedicineAdo.type != 1)
                //    return;

                if (this._PopupMenu == null)
                    this._PopupMenu = new PopupMenu(this._BarManager);
                this._PopupMenu.ItemLinks.Clear();

                List<HIS_SERVICE_REQ> ServiceReqSttId = new List<HIS_SERVICE_REQ>();
                if (this.listMedicineAdo.SERVICE_REQ_ID != null)
                {
                    HisServiceReqFilter filter = new HisServiceReqFilter();
                    filter.ID = this.listMedicineAdo.SERVICE_REQ_ID;

                    ServiceReqSttId = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>(RequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, filter, new CommonParam());
                }

                List<BarItem> barItems = new List<BarItem>();



                if (this.listMedicineAdo.type == 1)
                {
                    //Phiếu thu thanh toán
                    BarButtonItem bbtSuaHuongDanSuDung = new BarButtonItem(this._BarManager, "Sửa thông tin chung", 0);
                    bbtSuaHuongDanSuDung.Tag = ItemType.SuaHuongDanSuDung;
                    bbtSuaHuongDanSuDung.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    barItems.Add(bbtSuaHuongDanSuDung);
                }

                if (this.listMedicineAdo.TDL_SERVICE_TYPE_ID != 0
                    && this.listMedicineAdo.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                    && this.listMedicineAdo.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                    && this.listMedicineAdo.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU
                    && this.listMedicineAdo.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC
                    && this.listMedicineAdo.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN
                    && this.listMedicineAdo.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G)
                {
                    BarButtonItem bbtAssignPres = new BarButtonItem(this._BarManager, "Kê đơn", 0);
                    bbtAssignPres.Tag = ItemType.AssignPres;
                    bbtAssignPres.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    barItems.Add(bbtAssignPres);

                    BarButtonItem bbtAssignPresCapinet = new BarButtonItem(this._BarManager, "Kê đơn tủ trực", 0);
                    bbtAssignPresCapinet.Tag = ItemType.AssignPresCabinet;
                    bbtAssignPresCapinet.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    barItems.Add(bbtAssignPresCapinet);
                }

                if (this.listMedicineAdo.TDL_SERVICE_TYPE_ID != 0
                    && this.listMedicineAdo.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                    && this.listMedicineAdo.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                    && this.listMedicineAdo.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU)
                {

                    BarButtonItem bbtPrintServiceReq = new BarButtonItem(this._BarManager, "In phiếu chỉ định", 0);
                    bbtPrintServiceReq.Tag = ItemType.PrintServiceReq;
                    bbtPrintServiceReq.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    barItems.Add(bbtPrintServiceReq);
                }

                //if (ServiceReqSttId.FirstOrDefault().SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT || ServiceReqSttId.FirstOrDefault().SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                //{
                //    BarButtonItem KetQuaHeThongBenhAnhDienTu_ = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("ServiceReq.RightMenu.KetQuaHeThongBenhAnhDienTu", Resources.ResourceLanguageManager.LanguagefrmServiceReqList, LanguageManager.GetCulture()), 0);
                //    KetQuaHeThongBenhAnhDienTu_.Tag = ItemType.KetQuaHeThongBenhAnhDienTu;
                //    KetQuaHeThongBenhAnhDienTu_.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                //    barItems.Add(KetQuaHeThongBenhAnhDienTu_);
                //}

                if (this.listMedicineAdo.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT || this.listMedicineAdo.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT)
                {
                    //Chỉ định cùng kíp
                    BarButtonItem bbtAsignInKip = new BarButtonItem(this._BarManager, "Chỉ định cùng kíp", 0);
                    bbtAsignInKip.Tag = ItemType.AssignInKip;
                    bbtAsignInKip.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    barItems.Add(bbtAsignInKip);

                    //Chỉ định khác kíp
                    BarButtonItem bbtAsignOutKip = new BarButtonItem(this._BarManager, "Chỉ định khác kíp", 0);
                    bbtAsignOutKip.Tag = ItemType.AssignOutKip;
                    bbtAsignOutKip.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    barItems.Add(bbtAsignOutKip);
                }

                var currentExecuteRoom = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().FirstOrDefault(o => o.ROOM_ID == this._CurrentWorkPlace.RoomId);

                if (this.listMedicineAdo.TDL_REQUEST_ROOM_ID == this._CurrentWorkPlace.RoomId
                     && currentExecuteRoom != null && currentExecuteRoom.IS_EXAM == 1
                     && this.listMedicineAdo.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                     && this.listMedicineAdo.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                     && this.listMedicineAdo.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU
                     && this.listMedicineAdo.IS_NO_EXECUTE != 1)
                {
                    if (this.listMedicineAdo.IS_ACCEPTING_NO_EXECUTE != 1 && ServiceReqSttId != null && ServiceReqSttId.Count > 0 && (ServiceReqSttId.FirstOrDefault().SERVICE_REQ_STT_ID == 1 || this.listMedicineAdo.IS_CONFIRM_NO_EXCUTE == 1))
                    {
                        BarButtonItem bbtAcceptNoExecute = new BarButtonItem(this._BarManager, "Cho phép không thực hiện", 0);
                        bbtAcceptNoExecute.Tag = ItemType.AcceptNoExecute;
                        bbtAcceptNoExecute.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                        barItems.Add(bbtAcceptNoExecute);
                    }
                    else if (this.listMedicineAdo.IS_ACCEPTING_NO_EXECUTE == 1)
                    {
                        BarButtonItem bbtUnacceptNoExecute = new BarButtonItem(this._BarManager, "Hủy cho phép không thực hiện", 0);
                        bbtUnacceptNoExecute.Tag = ItemType.UnacceptNoExecute;
                        bbtUnacceptNoExecute.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                        barItems.Add(bbtUnacceptNoExecute);
                    }
                }

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
