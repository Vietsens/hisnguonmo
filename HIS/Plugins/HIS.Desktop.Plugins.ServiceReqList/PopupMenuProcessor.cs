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
using Inventec.Common.LocalStorage.SdaConfig;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServiceReqList
{
    delegate void TransactionMouseRightClick(object sender, ItemClickEventArgs e);
    class PopupMenuProcessor
    {
        V_HIS_TRANSACTION _Transaction = null;
        BarManager _BarManager = null;
        PopupMenu _PopupMenu = null;
        TransactionMouseRightClick _MouseRightClick;
        internal enum ItemType
        {
            PhieuThuThanhToan,
            PhieuTamUng,
            PhieuHoanUng,
            HoaDonTTTheoYeuCauDichVu,
            HoaDonTTChiTietDichVu,
            PhieuChiDinh,
            BienLaiPhiLePhi,
            PhieuThuPhiDichVu        }

        internal PopupMenuProcessor(V_HIS_TRANSACTION transaction, BarManager barmanager, TransactionMouseRightClick mouseRightClick)
        {
            this._Transaction = transaction;
            this._MouseRightClick = mouseRightClick;
            this._BarManager = barmanager;
        }

        internal void InitMenu()
        {
            try
            {
                if (this._Transaction == null || this._BarManager == null || this._MouseRightClick == null)
                    return;
                if (this._Transaction.IS_CANCEL == 1)//IMSys.DbConfig.HIS_RS.HIS_TRANSACTION.IS_CANCEL__TRUE)
                {
                    Inventec.Common.Logging.LogSystem.Info("giao dich da bi huy: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this._Transaction), this._Transaction));
                    return;
                }
                if (this._PopupMenu == null)
                    this._PopupMenu = new PopupMenu(this._BarManager);
                this._PopupMenu.ItemLinks.Clear();
                if (this._Transaction.TRANSACTION_TYPE_CODE == SdaConfigs.Get<string>(ConfigKeys.DBCODE__HIS_RS__HIS_TRANSACTION_TYPE__TRANSACTION_TYPE_CODE__BILL))
                {
                    //Phiếu thu thanh toán
                    BarButtonItem bbtnPhieuThuThanhToan = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_PHIEUTHUTHANHTOAN", Base.ResourceLangManager.LanguageFrmServiceReqList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 0);
                    bbtnPhieuThuThanhToan.Tag = ItemType.PhieuThuThanhToan;
                    bbtnPhieuThuThanhToan.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    //Hóa đơn thanh toán theo yêu cầu dịch vụ
                    BarButtonItem bbtnHoaDonTTTheoYeuCauDichVu = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_HOADONTHANHTOANTHEOYEUCAUDICHVU", Base.ResourceLangManager.LanguageFrmServiceReqList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 1);
                    bbtnHoaDonTTTheoYeuCauDichVu.Tag = ItemType.HoaDonTTTheoYeuCauDichVu;
                    bbtnHoaDonTTTheoYeuCauDichVu.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    //Hóa đơn thanh toán chi tiết dịch vụ
                    //BarButtonItem bbtnHoaDonTTChiTietDichVu = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_HOADONTHANHTOANCHITIETDICHVU", Base.ResourceLangManager.LanguageFrmServiceReqList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 2);
                    //bbtnHoaDonTTChiTietDichVu.Tag = ItemType.HoaDonTTChiTietDichVu;
                    //bbtnHoaDonTTChiTietDichVu.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    //Biên lai thu phí lệ phí
                    BarButtonItem bbtnBienLaiThuPhiLePhi = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_BIENLAITHUPHILEPHI", Base.ResourceLangManager.LanguageFrmServiceReqList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 3);
                    bbtnBienLaiThuPhiLePhi.Tag = ItemType.BienLaiPhiLePhi;
                    bbtnBienLaiThuPhiLePhi.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    //Phiếu chỉ định
                    BarButtonItem bbtnPhieuChiDinh = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_PHIEUCHIDINH", Base.ResourceLangManager.LanguageFrmServiceReqList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 4);
                    bbtnPhieuChiDinh.Tag = ItemType.PhieuChiDinh;
                    bbtnPhieuChiDinh.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    //Phiếu thu phí dịch vụ
                    //BarButtonItem bbtnPhieuThuPhiDichVu = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_PHIEUTHUPHIDICHVU", Base.ResourceLangManager.LanguageFrmServiceReqList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 5);
                    //bbtnPhieuThuPhiDichVu.Tag = ItemType.PhieuThuPhiDichVu;
                    //bbtnPhieuThuPhiDichVu.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    this._PopupMenu.AddItems(new BarItem[] { bbtnPhieuThuThanhToan, bbtnHoaDonTTTheoYeuCauDichVu, bbtnBienLaiThuPhiLePhi, bbtnPhieuChiDinh });
                    this._PopupMenu.ShowPopup(Cursor.Position);
                }
                else if (this._Transaction.TRANSACTION_TYPE_CODE == SdaConfigs.Get<string>(ConfigKeys.DBCODE__HIS_RS__HIS_TRANSACTION_TYPE__TRANSACTION_TYPE_CODE__DEPOSIT))
                {
                    //Phiếu tạm ứng
                    BarButtonItem bbtnPhieuTamUng = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_PHIEUTAMUNG", Base.ResourceLangManager.LanguageFrmServiceReqList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 0);
                    bbtnPhieuTamUng.Tag = ItemType.PhieuTamUng;
                    bbtnPhieuTamUng.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    ////Phiếu thu phí dịch vụ
                    //BarButtonItem bbtnPhieuThuPhiDichVu = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_PHIEUTHUPHIDICHVU", Base.ResourceLangManager.LanguageFrmServiceReqList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 1);
                    //bbtnPhieuThuPhiDichVu.Tag = ItemType.PhieuThuPhiDichVu;
                    //bbtnPhieuThuPhiDichVu.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    this._PopupMenu.AddItems(new BarItem[] { bbtnPhieuTamUng });
                    this._PopupMenu.ShowPopup(Cursor.Position);
                }
                else if (this._Transaction.TRANSACTION_TYPE_CODE == SdaConfigs.Get<string>(ConfigKeys.DBCODE__HIS_RS__HIS_TRANSACTION_TYPE__TRANSACTION_TYPE_CODE__REPAY))
                {
                    //Phiếu tạm ứng
                    BarButtonItem bbtnPhieuHoanUng = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_PHIEUHOANUNG", Base.ResourceLangManager.LanguageFrmServiceReqList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 0);
                    bbtnPhieuHoanUng.Tag = ItemType.PhieuTamUng;
                    bbtnPhieuHoanUng.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    //Phiếu thu phí dịch vụ
                    //BarButtonItem bbtnPhieuThuPhiDichVu = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_PHIEUTHUPHIDICHVU", Base.ResourceLangManager.LanguageFrmServiceReqList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 1);
                    //bbtnPhieuThuPhiDichVu.Tag = ItemType.PhieuThuPhiDichVu;
                    //bbtnPhieuThuPhiDichVu.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    this._PopupMenu.AddItems(new BarItem[] { bbtnPhieuHoanUng });
                    this._PopupMenu.ShowPopup(Cursor.Position);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
