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
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.PrintBordereau;
using HIS.Desktop.Plugins.Library.PrintBordereau.ADO;
using HIS.Desktop.Plugins.TransactionList.Base;
using HIS.Desktop.Plugins.TransactionList.Config;
using Inventec.Common.Adapter;
using Inventec.Common.LocalStorage.SdaConfig;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TransactionList
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
            InHoaDonBanThuoc,
            BangKe,
            InPhieuChotNo,
            InPhieuThuNo,
            MPS373_BKChiTietThuVP,
            InPhieuHuyTamUng,
            XuatHoaDonDienTu,
            TransactionEInvoice,
            ChuyenDoiHoaDonDienTu,
            Mps000439_BienBanDieuChinhThongTinHanhChinhHoaDon__,
            Mps000440_BienBanDieuChinhTangGiamTrenHoaDon__,
            HuyHoaDonDienTu,
            ThayThe
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

                if (this._Transaction.SALE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SALE_TYPE.ID__SALE_EXP && this._Transaction.IS_CANCEL != 1)
                {
                    List<BarItem> items = new List<BarItem>();

                    BarButtonItem btnHoaDonBanThuoc = new BarButtonItem(this._BarManager, "Hóa đơn bán thuốc", 1);
                    btnHoaDonBanThuoc.Tag = ItemType.InHoaDonBanThuoc;
                    btnHoaDonBanThuoc.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    items.Add(btnHoaDonBanThuoc);

                    BarButtonItem btnHoaDonXuatBan = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_HOA_DON_XUAT_BAN", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 2);
                    btnHoaDonXuatBan.Tag = ItemType.InHoaDonXuatBan;
                    btnHoaDonXuatBan.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    items.Add(btnHoaDonXuatBan);

                    if (String.IsNullOrWhiteSpace(this._Transaction.INVOICE_CODE)
                         && this._Transaction.IS_CANCEL != 1
                         && (this._Transaction.AMOUNT - (this._Transaction.EXEMPTION ?? 0) - (this._Transaction.TDL_BILL_FUND_AMOUNT ?? 0) > 0))
                    {
                        //xuất hóa đơn điện tủ
                        BarButtonItem btnHoaDonDienTu = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_HOADONDIENTU", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 5);
                        btnHoaDonDienTu.Tag = ItemType.XuatHoaDonDienTu;
                        btnHoaDonDienTu.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                        items.Add(btnHoaDonDienTu);

                        //Thông tin hóa 
                        BarButtonItem btnEInvoice = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_THONGTINHOADONDIENTU", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 5);
                        btnEInvoice.Tag = ItemType.TransactionEInvoice;
                        btnEInvoice.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                        items.Add(btnEInvoice);
                    }

                    this._PopupMenu.AddItems(items.ToArray());
                }
                else if (this._Transaction.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT && this._Transaction.IS_CANCEL == 1)
                {
                    if (Config.HisConfigCFG.TransactionBillSelect == "1"
                        && !String.IsNullOrWhiteSpace(this._Transaction.ALL_TRANS_CODES_IN_INVOICE)
                        && !IsGiaoDichGop(this._Transaction.ALL_TRANS_CODES_IN_INVOICE))
                    {
                        BarButtonItem bbtnThayThe = new BarButtonItem(this._BarManager, "Thay thế", 0);
                        bbtnThayThe.Tag = ItemType.ThayThe;
                        bbtnThayThe.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                        this._PopupMenu.AddItems(new BarItem[] { bbtnThayThe });
                    }
                    BarButtonItem bbtnPhieuHuyThanhToan = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_PHIEUHUYTHANHTOAN", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 0);
                    bbtnPhieuHuyThanhToan.Tag = ItemType.InBienLaiHuy;
                    bbtnPhieuHuyThanhToan.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    this._PopupMenu.AddItems(new BarItem[] { bbtnPhieuHuyThanhToan });
                }
                else if (this._Transaction.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT)
                {
                    //Phiếu thu thanh toán
                    BarButtonItem bbtnPhieuThuThanhToan = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_PHIEUTHUTHANHTOAN", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 0);
                    bbtnPhieuThuThanhToan.Tag = ItemType.PhieuThuThanhToan;
                    bbtnPhieuThuThanhToan.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    this._PopupMenu.AddItems(new BarItem[] { bbtnPhieuThuThanhToan });

                    //Hóa đơn thanh toán theo yêu cầu dịch vụ
                    BarButtonItem bbtnHoaDonTTTheoYeuCauDichVu = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_HOADONTHANHTOANTHEOYEUCAUDICHVU", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 1);
                    bbtnHoaDonTTTheoYeuCauDichVu.Tag = ItemType.HoaDonTTTheoYeuCauDichVu;
                    bbtnHoaDonTTTheoYeuCauDichVu.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    this._PopupMenu.AddItems(new BarItem[] { bbtnHoaDonTTTheoYeuCauDichVu });

                    //Hóa đơn thanh toán chi tiết dịch vụ
                    BarButtonItem bbtnHoaDonTTChiTietDichVu = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_HOADONTHANHTOANCHITIETDICHVU", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 2);
                    bbtnHoaDonTTChiTietDichVu.Tag = ItemType.HoaDonTTChiTietDichVu;
                    bbtnHoaDonTTChiTietDichVu.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    this._PopupMenu.AddItems(new BarItem[] { bbtnHoaDonTTChiTietDichVu });

                    //Biên lai thu phí lệ phí
                    BarButtonItem bbtnBienLaiThuPhiLePhi = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_BIENLAITHUPHILEPHI", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 3);
                    bbtnBienLaiThuPhiLePhi.Tag = ItemType.BienLaiPhiLePhi;
                    bbtnBienLaiThuPhiLePhi.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    this._PopupMenu.AddItems(new BarItem[] { bbtnBienLaiThuPhiLePhi });

                    //Phiếu chỉ định
                    BarButtonItem bbtnPhieuChiDinh = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_PHIEUCHIDINH", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 4);
                    bbtnPhieuChiDinh.Tag = ItemType.PhieuChiDinh;
                    bbtnPhieuChiDinh.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    this._PopupMenu.AddItems(new BarItem[] { bbtnPhieuChiDinh });

                    if (!String.IsNullOrEmpty(this._Transaction.INVOICE_CODE) && !String.IsNullOrEmpty(this._Transaction.INVOICE_SYS))
                    {
                        BarButtonItem btnHoaDonDienTu = new BarButtonItem(this._BarManager, "In hóa đơn điện tử", 1);
                        btnHoaDonDienTu.Tag = ItemType.HoaDonDienTu;
                        btnHoaDonDienTu.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                        this._PopupMenu.AddItems(new BarItem[] { btnHoaDonDienTu });

                        if (this._Transaction.EINVOICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__VNPT || 
                            (this._Transaction.EINVOICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__VIETEL && HisConfigCFG.autoPrintType == "1") ||
                            (this._Transaction.EINVOICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__CYBERBILL && HisConfigCFG.autoPrintType == "1"))
                        {
                            BarButtonItem btnChuyenDoiHoaDonDienTu = new BarButtonItem(this._BarManager, "Chuyển đổi hóa đơn điện tử", 1);
                            btnChuyenDoiHoaDonDienTu.Tag = ItemType.ChuyenDoiHoaDonDienTu;
                            btnChuyenDoiHoaDonDienTu.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                            this._PopupMenu.AddItems(new BarItem[] { btnChuyenDoiHoaDonDienTu });
                        }
                    }

                    //Bảng kê chi tiết thu viện phí theo hóa đơn
                    BarButtonItem bbtnBangKeChiTietThuPhi_Mps373 = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_bbtnBangKeChiTietThuPhi_Mps373", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 2);
                    bbtnBangKeChiTietThuPhi_Mps373.Tag = ItemType.MPS373_BKChiTietThuVP;
                    bbtnBangKeChiTietThuPhi_Mps373.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    this._PopupMenu.AddItems(new BarItem[] { bbtnBangKeChiTietThuPhi_Mps373 });

                    //Phiếu thu phí dịch vụ
                    BarButtonItem bbtnPhieuThuPhiDichVu = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_PHIEUTHUPHIDICHVU", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 5);
                    bbtnPhieuThuPhiDichVu.Tag = ItemType.PhieuThuPhiDichVu;
                    bbtnPhieuThuPhiDichVu.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    //this._PopupMenu.AddItems(new BarItem[] { });

                    //(HIS_TRANSACTION có INVOICE_CODE null và IS_CANCEL null và TRANSACTION_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT)
                    //có AMOUNT - EXEMPTION - TDL_BILL_FUND_AMOUNT >0
                    if (String.IsNullOrWhiteSpace(this._Transaction.INVOICE_CODE)
                        && this._Transaction.IS_CANCEL != 1
                        && (this._Transaction.AMOUNT - (this._Transaction.EXEMPTION ?? 0) - (this._Transaction.TDL_BILL_FUND_AMOUNT ?? 0) > 0))
                    {
                        //xuất hóa đơn điện tủ
                        BarButtonItem btnHoaDonDienTu = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_HOADONDIENTU", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 5);
                        btnHoaDonDienTu.Tag = ItemType.XuatHoaDonDienTu;
                        btnHoaDonDienTu.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                        this._PopupMenu.AddItem(btnHoaDonDienTu);

                        //Thông tin hóa 
                        BarButtonItem btnEInvoice = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_THONGTINHOADONDIENTU", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 5);
                        btnEInvoice.Tag = ItemType.TransactionEInvoice;
                        btnEInvoice.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                        this._PopupMenu.AddItem(btnEInvoice);
                    }

                    if (!string.IsNullOrEmpty(this._Transaction.INVOICE_CODE) && this._Transaction.IS_CANCEL_EINVOICE != 1 && HisConfigCFG.Cancel_Option == "1" && frmTransactionList.controlAcs != null && frmTransactionList.controlAcs.Exists(o => o.CONTROL_CODE == "HIS000040"))
                    {
                        BarButtonItem btnHuyHoaDonDienTu = new BarButtonItem(this._BarManager, "Hủy hóa đơn điện tử", 1);
                        btnHuyHoaDonDienTu.Tag = ItemType.HuyHoaDonDienTu;
                        btnHuyHoaDonDienTu.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                        this._PopupMenu.AddItems(new BarItem[] { btnHuyHoaDonDienTu });
                    }

                }
                else if (this._Transaction.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU)
                {
                    //Phiếu tạm ứng
                    BarButtonItem bbtnPhieuTamUng = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_PHIEUTAMUNG", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 0);
                    bbtnPhieuTamUng.Tag = ItemType.PhieuTamUng;
                    bbtnPhieuTamUng.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    //Phiếu tạm ứng dịch vụ
                    //BarButtonItem bbtnPhieuTamUngDichVu = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_PHIEUTAMUNGDICHVU", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 1);
                    //bbtnPhieuTamUngDichVu.Tag = ItemType.PhieuTamUngDichVu;
                    //bbtnPhieuTamUngDichVu.ItemClick += new ItemClickEventHandler(this._MouseRightClick);


                    //Phiếu tạm ứng dịch vụ chi tiết
                    BarButtonItem bbtnChiTietPhieuTamUngDichVu = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_PHIEUTAMUNGDICHVUCHITIET", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 2);
                    bbtnChiTietPhieuTamUngDichVu.Tag = ItemType.PhieuTamUngDichVuChiTiet;
                    bbtnChiTietPhieuTamUngDichVu.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    //Phiếu tạm ứng và giữ thẻ
                    BarButtonItem bbtnPhieuTamUngVaGiuThe = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_PHIEUTAMUNGVAGIUTHE", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 3);
                    bbtnPhieuTamUngVaGiuThe.Tag = ItemType.PhieuTamUngGiuThe;
                    bbtnPhieuTamUngVaGiuThe.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    //Phiếu tạm ứng và giữ thẻ
                    BarButtonItem bbtnPhieuTamUngTongTinBhyt = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_PHIEUTAMUNGCOTHONGTINBHYT", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 4);
                    bbtnPhieuTamUngTongTinBhyt.Tag = ItemType.PhieuTamUngThongTinBhyt;
                    bbtnPhieuTamUngTongTinBhyt.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    this._PopupMenu.AddItems(new BarItem[] { bbtnPhieuTamUng, bbtnChiTietPhieuTamUngDichVu, bbtnPhieuTamUngVaGiuThe, bbtnPhieuTamUngTongTinBhyt });
                }
                else if (this._Transaction.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__HU)
                {
                    //Phiếu tạm ứng
                    BarButtonItem bbtnPhieuHoanUng = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_PHIEUHOANUNG", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 0);
                    bbtnPhieuHoanUng.Tag = ItemType.PhieuHoanUng;
                    bbtnPhieuHoanUng.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    //Phiếu hoàn ứng dịch vụ
                    BarButtonItem bbtnPhieuHoanUngDichVu = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_PHIEUHOANUNGDICHVU", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 1);
                    bbtnPhieuHoanUngDichVu.Tag = ItemType.PhieuHoanUngDichVu;
                    bbtnPhieuHoanUngDichVu.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    this._PopupMenu.AddItems(new BarItem[] { bbtnPhieuHoanUng, bbtnPhieuHoanUngDichVu });
                }

                if (HisConfigCFG.ALLOW_EDIT_NUM_ORDER == "1"
                    && (!this._Transaction.IS_CANCEL.HasValue || this._Transaction.IS_CANCEL != 1)
                    && this._Transaction.IS_NOT_GEN_TRANSACTION_ORDER == 1
                    && this.IsUseModuleEditSoBienLai())
                {
                    BarButtonItem bbtnSuaSoBienLai = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_LIST__POPUP_MENU__ITEM_SUASOBIENLAI", Base.ResourceLangManager.LanguageFrmTransactionList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 1);
                    bbtnSuaSoBienLai.Tag = ItemType.SuaSoBienLai;
                    bbtnSuaSoBienLai.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    this._PopupMenu.AddItem(bbtnSuaSoBienLai);
                }

                //gọi hàm init danh sách bảng kê tương ứng hồ sơ đtri
                //Đối với giao dịch đã bị hủy thì không cho phép in bảng kê.

                if (_Transaction.TREATMENT_ID.HasValue && _Transaction.IS_CANCEL != 1)
                {
                    BarButtonItem btnBangKe = new BarButtonItem(this._BarManager, "Bảng kê thanh toán", 1);
                    btnBangKe.Tag = ItemType.BangKe;
                    btnBangKe.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    this._PopupMenu.AddItems(new BarItem[] { btnBangKe });
                }

                if (_Transaction.TREATMENT_ID.HasValue && _Transaction.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__NO && _Transaction.IS_CANCEL != 1)
                {
                    BarButtonItem btnPhieuChotNo = new BarButtonItem(this._BarManager, "In phiếu chốt nợ", 1);
                    btnPhieuChotNo.Tag = ItemType.InPhieuChotNo;
                    btnPhieuChotNo.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    this._PopupMenu.AddItems(new BarItem[] { btnPhieuChotNo });
                }

                if (_Transaction.TREATMENT_ID.HasValue && _Transaction.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT && _Transaction.IS_DEBT_COLLECTION == 1)
                {
                    BarButtonItem btnPhieuThuNo = new BarButtonItem(this._BarManager, "In phiếu thu nợ", 1);
                    btnPhieuThuNo.Tag = ItemType.InPhieuThuNo;
                    btnPhieuThuNo.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    this._PopupMenu.AddItems(new BarItem[] { btnPhieuThuNo });
                }

                if (_Transaction.TREATMENT_ID.HasValue && _Transaction.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU && _Transaction.IS_CANCEL == 1)
                {
                    BarButtonItem btnPhieuHuyTamUng = new BarButtonItem(this._BarManager, "Phiếu hủy giao dịch tạm ứng", 1);
                    btnPhieuHuyTamUng.Tag = ItemType.InPhieuHuyTamUng;
                    btnPhieuHuyTamUng.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    this._PopupMenu.AddItems(new BarItem[] { btnPhieuHuyTamUng });
                }

                if (_Transaction.TREATMENT_ID.HasValue && _Transaction.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT)
                {
                    BarButtonItem btnBienBanDieuChinhThongtinHanhChinhHoaDon = new BarButtonItem(this._BarManager, "Biên bản điều chỉnh thông tin hành chính hóa đơn", 1);
                    btnBienBanDieuChinhThongtinHanhChinhHoaDon.Tag = ItemType.Mps000439_BienBanDieuChinhThongTinHanhChinhHoaDon__;
                    btnBienBanDieuChinhThongtinHanhChinhHoaDon.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    this._PopupMenu.AddItems(new BarItem[] { btnBienBanDieuChinhThongtinHanhChinhHoaDon });

                    BarButtonItem btnBienBanDieuChinhTangGiamTrenHoaDon = new BarButtonItem(this._BarManager, "Biên bản điều chỉnh tăng giảm trên hóa đơn", 1);
                    btnBienBanDieuChinhTangGiamTrenHoaDon.Tag = ItemType.Mps000440_BienBanDieuChinhTangGiamTrenHoaDon__;
                    btnBienBanDieuChinhTangGiamTrenHoaDon.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    this._PopupMenu.AddItems(new BarItem[] { btnBienBanDieuChinhTangGiamTrenHoaDon });
                }

                this._PopupMenu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool IsGiaoDichGop(string allTransCodeInInvoice)
        {
            bool result = false;
            try
            {
                if (String.IsNullOrWhiteSpace(allTransCodeInInvoice))
                    return false;
                string[] listTransCode = allTransCodeInInvoice.Split(',');
                listTransCode = listTransCode != null ? listTransCode.Where(o => !String.IsNullOrWhiteSpace(o)).ToArray() : null;
                if (listTransCode != null && listTransCode.Count() > 1)
                    result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
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
