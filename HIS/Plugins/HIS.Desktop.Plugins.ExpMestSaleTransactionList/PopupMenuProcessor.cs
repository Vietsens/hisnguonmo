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

namespace HIS.Desktop.Plugins.ExpMestSaleTransactionList
{
    delegate void TransactionMouseRightClick(object sender, ItemClickEventArgs e);
    class PopupMenuProcessor
    {
        DHisTransExpSDO _Transaction = null;
        BarManager _BarManager = null;
        PopupMenu _PopupMenu = null;
        TransactionMouseRightClick _MouseRightClick;
        internal enum ItemType
        {
            PhieuThuThanhToan,
            PhieuTamUng,
            //PhieuTamUngDichVu,
            PhieuTamUngDichVuChiTiet,
            PhieuHoanUng,
            HoaDonTTTheoYeuCauDichVu,
            HoaDonTTChiTietDichVu,
            PhieuChiDinh,
            BienLaiPhiLePhi,
            PhieuThuPhiDichVu,
            PhieuHoanUngDichVu,
            PhieuTamUngGiuThe,
            PhieuTamUngThongTinBhyt,
            HoaDonDienTu,
            SuaSoBienLai,
            InBienLaiHuy,
            InHoaDonXuatBan,
            KhoiPhucBienLai,
            HuyBienLai,
            HuyPhieuXuat,
            TransactionEInvoice
        }

        internal PopupMenuProcessor(DHisTransExpSDO transaction, BarManager barmanager, TransactionMouseRightClick mouseRightClick)
        {
            this._Transaction = transaction;
            this._MouseRightClick = mouseRightClick;
            this._BarManager = barmanager;
        }

        internal void InitMenu()
        {
            try
            {
                string _LoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
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

                if (!string.IsNullOrEmpty(this._Transaction.TRANSACTION_CODE) && !string.IsNullOrEmpty(this._Transaction.EXP_MEST_CODE) && this._Transaction.IS_CANCEL != 1)
                {
                    BarButtonItem btnHoaDonXuatBan = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_HOA_DON_XUAT_BAN", Base.ResourceLangManager.LanguageFrmExpMestSaleTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 0);
                    btnHoaDonXuatBan.Tag = ItemType.InHoaDonXuatBan;
                    btnHoaDonXuatBan.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    this._PopupMenu.AddItems(new BarItem[] { btnHoaDonXuatBan });

                    if (String.IsNullOrWhiteSpace(this._Transaction.INVOICE_CODE)
                        && this._Transaction.IS_CANCEL != 1
                        && (this._Transaction.AMOUNT - (this._Transaction.EXEMPTION ?? 0) - (this._Transaction.TDL_BILL_FUND_AMOUNT ?? 0) - (this._Transaction.KC_AMOUNT ?? 0) > 0))
                    {
                        //Thông tin hóa đơn điện tử
                        BarButtonItem btnEInvoice = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_THONGTINHOADONDIENTU", Base.ResourceLangManager.LanguageFrmExpMestSaleTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 5);
                        btnEInvoice.Tag = ItemType.TransactionEInvoice;
                        btnEInvoice.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                        this._PopupMenu.AddItem(btnEInvoice);
                    }
                }
                else if (this._Transaction.TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT && this._Transaction.IS_CANCEL == 1)
                {
                    BarButtonItem bbtnPhieuHuyThanhToan = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_PHIEUHUYTHANHTOAN", Base.ResourceLangManager.LanguageFrmExpMestSaleTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 0);
                    bbtnPhieuHuyThanhToan.Tag = ItemType.InBienLaiHuy;
                    bbtnPhieuHuyThanhToan.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    this._PopupMenu.AddItems(new BarItem[] { bbtnPhieuHuyThanhToan });
                }
                else if (this._Transaction.TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT)
                {
                    //Phiếu thu thanh toán
                    BarButtonItem bbtnPhieuThuThanhToan = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_PHIEUTHUTHANHTOAN", Base.ResourceLangManager.LanguageFrmExpMestSaleTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 0);
                    bbtnPhieuThuThanhToan.Tag = ItemType.PhieuThuThanhToan;
                    bbtnPhieuThuThanhToan.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    //Hóa đơn thanh toán theo yêu cầu dịch vụ
                    BarButtonItem bbtnHoaDonTTTheoYeuCauDichVu = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_HOADONTHANHTOANTHEOYEUCAUDICHVU", Base.ResourceLangManager.LanguageFrmExpMestSaleTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 1);
                    bbtnHoaDonTTTheoYeuCauDichVu.Tag = ItemType.HoaDonTTTheoYeuCauDichVu;
                    bbtnHoaDonTTTheoYeuCauDichVu.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    //Hóa đơn thanh toán chi tiết dịch vụ
                    BarButtonItem bbtnHoaDonTTChiTietDichVu = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_HOADONTHANHTOANCHITIETDICHVU", Base.ResourceLangManager.LanguageFrmExpMestSaleTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 2);
                    bbtnHoaDonTTChiTietDichVu.Tag = ItemType.HoaDonTTChiTietDichVu;
                    bbtnHoaDonTTChiTietDichVu.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    //Biên lai thu phí lệ phí
                    BarButtonItem bbtnBienLaiThuPhiLePhi = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_BIENLAITHUPHILEPHI", Base.ResourceLangManager.LanguageFrmExpMestSaleTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 3);
                    bbtnBienLaiThuPhiLePhi.Tag = ItemType.BienLaiPhiLePhi;
                    bbtnBienLaiThuPhiLePhi.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    //Phiếu chỉ định
                    BarButtonItem bbtnPhieuChiDinh = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_PHIEUCHIDINH", Base.ResourceLangManager.LanguageFrmExpMestSaleTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 4);
                    bbtnPhieuChiDinh.Tag = ItemType.PhieuChiDinh;
                    bbtnPhieuChiDinh.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    //Phiếu thu phí dịch vụ
                    BarButtonItem bbtnPhieuThuPhiDichVu = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_PHIEUTHUPHIDICHVU", Base.ResourceLangManager.LanguageFrmExpMestSaleTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 5);
                    bbtnPhieuThuPhiDichVu.Tag = ItemType.PhieuThuPhiDichVu;
                    bbtnPhieuThuPhiDichVu.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    this._PopupMenu.AddItems(new BarItem[] { bbtnPhieuThuThanhToan, bbtnHoaDonTTTheoYeuCauDichVu, bbtnHoaDonTTChiTietDichVu, bbtnBienLaiThuPhiLePhi, bbtnPhieuChiDinh });

                    if (String.IsNullOrWhiteSpace(this._Transaction.INVOICE_CODE)
                        && this._Transaction.IS_CANCEL != 1
                        && (this._Transaction.AMOUNT - (this._Transaction.EXEMPTION ?? 0) - (this._Transaction.TDL_BILL_FUND_AMOUNT ?? 0) > 0))
                    {
                        //Thông tin hóa đơn điện tử
                        BarButtonItem btnEInvoice = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_THONGTINHOADONDIENTU", Base.ResourceLangManager.LanguageFrmExpMestSaleTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 5);
                        btnEInvoice.Tag = ItemType.TransactionEInvoice;
                        btnEInvoice.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                        this._PopupMenu.AddItem(btnEInvoice);
                    }
                }
                else if (this._Transaction.TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU)
                {
                    //Phiếu tạm ứng
                    BarButtonItem bbtnPhieuTamUng = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_PHIEUTAMUNG", Base.ResourceLangManager.LanguageFrmExpMestSaleTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 0);
                    bbtnPhieuTamUng.Tag = ItemType.PhieuTamUng;
                    bbtnPhieuTamUng.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    //Phiếu tạm ứng dịch vụ chi tiết
                    BarButtonItem bbtnChiTietPhieuTamUngDichVu = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_PHIEUTAMUNGDICHVUCHITIET", Base.ResourceLangManager.LanguageFrmExpMestSaleTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 2);
                    bbtnChiTietPhieuTamUngDichVu.Tag = ItemType.PhieuTamUngDichVuChiTiet;
                    bbtnChiTietPhieuTamUngDichVu.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    //Phiếu tạm ứng và giữ thẻ
                    BarButtonItem bbtnPhieuTamUngVaGiuThe = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_PHIEUTAMUNGVAGIUTHE", Base.ResourceLangManager.LanguageFrmExpMestSaleTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 3);
                    bbtnPhieuTamUngVaGiuThe.Tag = ItemType.PhieuTamUngGiuThe;
                    bbtnPhieuTamUngVaGiuThe.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    //Phiếu tạm ứng và giữ thẻ
                    BarButtonItem bbtnPhieuTamUngTongTinBhyt = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_PHIEUTAMUNGCOTHONGTINBHYT", Base.ResourceLangManager.LanguageFrmExpMestSaleTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 4);
                    bbtnPhieuTamUngTongTinBhyt.Tag = ItemType.PhieuTamUngThongTinBhyt;
                    bbtnPhieuTamUngTongTinBhyt.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    this._PopupMenu.AddItems(new BarItem[] { bbtnPhieuTamUng, bbtnChiTietPhieuTamUngDichVu, bbtnPhieuTamUngVaGiuThe, bbtnPhieuTamUngTongTinBhyt });
                }
                else if (this._Transaction.TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__HU)
                {
                    //Phiếu tạm ứng
                    BarButtonItem bbtnPhieuHoanUng = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_PHIEUHOANUNG", Base.ResourceLangManager.LanguageFrmExpMestSaleTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 0);
                    bbtnPhieuHoanUng.Tag = ItemType.PhieuHoanUng;
                    bbtnPhieuHoanUng.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    //Phiếu hoàn ứng dịch vụ
                    BarButtonItem bbtnPhieuHoanUngDichVu = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_PHIEUHOANUNGDICHVU", Base.ResourceLangManager.LanguageFrmExpMestSaleTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 1);
                    bbtnPhieuHoanUngDichVu.Tag = ItemType.PhieuHoanUngDichVu;
                    bbtnPhieuHoanUngDichVu.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    this._PopupMenu.AddItems(new BarItem[] { bbtnPhieuHoanUng, bbtnPhieuHoanUngDichVu });
                }
                if (_Transaction.CASHIER_LOGINNAME == _LoginName
                                || HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(_LoginName))
                {
                    if (this._Transaction.IS_CANCEL == 1 && !string.IsNullOrEmpty(this._Transaction.TRANSACTION_CODE))
                    {
                        //KhoiPhucBienLai
                        BarButtonItem bbtnKhoiPhucBienLai = new BarButtonItem(this._BarManager, "Khôi phục biên lai", 1);
                        bbtnKhoiPhucBienLai.Tag = ItemType.KhoiPhucBienLai;
                        bbtnKhoiPhucBienLai.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                        this._PopupMenu.AddItems(new BarItem[] { bbtnKhoiPhucBienLai });
                    }
                    else if (!string.IsNullOrEmpty(this._Transaction.TRANSACTION_CODE))
                    {
                        //HuyBienLai
                        BarButtonItem bbtnHuyBienLai = new BarButtonItem(this._BarManager, "Hủy biên lai/Hóa đơn", 1);
                        bbtnHuyBienLai.Tag = ItemType.HuyBienLai;
                        bbtnHuyBienLai.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                        this._PopupMenu.AddItems(new BarItem[] { bbtnHuyBienLai });
                    }
                    if (string.IsNullOrEmpty(this._Transaction.TRANSACTION_CODE) || (_Transaction.IS_CANCEL == 1 && !string.IsNullOrEmpty(this._Transaction.TRANSACTION_CODE) && !string.IsNullOrEmpty(this._Transaction.EXP_MEST_CODE))
                        )
                    {
                        //HuyPhieuXuat
                        BarButtonItem bbtnHuyPhieuXuat = new BarButtonItem(this._BarManager, "Hủy phiếu xuất", 1);
                        bbtnHuyPhieuXuat.Tag = ItemType.HuyPhieuXuat;
                        bbtnHuyPhieuXuat.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                        this._PopupMenu.AddItems(new BarItem[] { bbtnHuyPhieuXuat });
                    }
                }

                this._PopupMenu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool IsUseModuleEditSoBienLai()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionNumOrderUpdate").FirstOrDefault();
                return moduleData != null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }
    }
}
