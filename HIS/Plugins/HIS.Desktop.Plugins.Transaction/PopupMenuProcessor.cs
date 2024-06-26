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
using HIS.Desktop.Plugins.Transaction.ADO;
using HIS.Desktop.Plugins.Transaction.Config;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Transaction
{
    delegate void TransactionMouseRightClick(object sender, ItemClickEventArgs e);
    class PopupMenuProcessor
    {
        BarManager _BarManager = null;
        PopupMenu _Menu = null;
        TransactionMouseRightClick _MouseRightClick;
        V_HIS_TREATMENT_FEE currentTreatment;
        PopupItemStatusAdo PopupItemStatusAdo;
        internal PopupMenuProcessor(BarManager barManager, TransactionMouseRightClick mouseRightClick, V_HIS_TREATMENT_FEE _currentTreatment, PopupItemStatusAdo _PopupItemStatusAdo)
        {
            this._BarManager = barManager;
            this._MouseRightClick = mouseRightClick;
            this.currentTreatment = _currentTreatment;
            this.PopupItemStatusAdo = _PopupItemStatusAdo;
        }

        internal enum ItemType
        {
            InBangKe,
            LichSuGiaoDich,
            InPhieuGiuThe,
            InThanhToan,
            Khoa,
            MoKhoa,
            TamKhoa,
            MoTamKhoa,
            HoaDonDo,
            GiaoDich,
            TamUngNoiTru,
            HoanUngNoiTru,
            ThanhToan,
            TamThuDichVu,
            HoanUngDichVu,
            LichSuTacDongHoSo,
            MienGiam,
            ThanhToanKhac,
            CheckInfoBHYT,
            ChotNo,
            ThuNo,
            ThuTrucTiep,
            InTrichSao,
            HoaDonDienTu,
            CLS,
            InPhieuThanhToan,
            YCTU
        }

        internal void InitMenu(bool allowUnlock, string loginname)
        {
            try
            {
                if (this._BarManager == null || this._MouseRightClick == null)
                    return;
                if (this._Menu == null)
                    this._Menu = new PopupMenu(this._BarManager);

                //in bảng kê chi phí khám chưa bệnh => bảng kê
                BarButtonItem bbtnInBangKeChiPhiKCB = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__POPUP_MENU__ITEM_BANG_KE_CHI_PHI_KCB", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 0);
                bbtnInBangKeChiPhiKCB.Tag = ItemType.InBangKe;
                bbtnInBangKeChiPhiKCB.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItems(new BarItem[] { bbtnInBangKeChiPhiKCB });

                //In phiếu giữ thẻ BHYT
                BarButtonItem bbtnPhieuGiuThe = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__POPUP_MENU__ITEM_PHIEU_GIU_THE", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 1);
                bbtnPhieuGiuThe.Tag = ItemType.InPhieuGiuThe;
                bbtnPhieuGiuThe.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItems(new BarItem[] { bbtnPhieuGiuThe });

                //In phiếu giữ thẻ BHYT
                BarButtonItem bbtnPhieuThuThanhToan = new BarButtonItem(this._BarManager, "In phiếu thanh toán", 1);
                bbtnPhieuThuThanhToan.Tag = ItemType.InPhieuThanhToan;
                bbtnPhieuThuThanhToan.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItems(new BarItem[] { bbtnPhieuThuThanhToan });


                if (this.PopupItemStatusAdo.LockStt)
                {
                    //Khóa/mở khóa
                    if (this.currentTreatment.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        BarButtonItem bbtnKhoa = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__POPUP_MENU__ITEM_KHOA", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 2);
                        bbtnKhoa.Tag = ItemType.Khoa;
                        bbtnKhoa.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                        this._Menu.AddItems(new BarItem[] { bbtnKhoa });
                    }
                    else
                    {
                        if (HisConfigCFG.UNLOCK_FEE_OPTION == "2"
                            || allowUnlock
                            || this.currentTreatment.FEE_LOCK_LOGINNAME == loginname)
                        {
                            BarButtonItem bbtnMoKhoa = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__POPUP_MENU__ITEM_MO_KHOA", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 3);
                            bbtnMoKhoa.Tag = ItemType.MoKhoa;
                            bbtnMoKhoa.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                            this._Menu.AddItems(new BarItem[] { bbtnMoKhoa });
                        }
                    }
                }

                if (this.PopupItemStatusAdo.TemporaryLockStt)
                {
                    //tạm khóa/mở tạm khóa
                    if (this.currentTreatment.IS_TEMPORARY_LOCK == 1)
                    {
                        BarButtonItem bbtnMoTamKhoa = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__POPUP_MENU__ITEM_MO_TAM_KHOA", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 3);
                        bbtnMoTamKhoa.Tag = ItemType.MoTamKhoa;
                        bbtnMoTamKhoa.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                        this._Menu.AddItems(new BarItem[] { bbtnMoTamKhoa });
                    }
                    else
                    {
                        BarButtonItem bbtnTamKhoa = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__POPUP_MENU__ITEM_TAM_KHOA", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 2);
                        bbtnTamKhoa.Tag = ItemType.TamKhoa;
                        bbtnTamKhoa.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                        this._Menu.AddItems(new BarItem[] { bbtnTamKhoa });
                    }
                }

                //hóa đơn đỏ
                BarButtonItem bbtnHoaDonDo = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__POPUP_MENU__ITEM_HOA_DON_DO", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 4);
                bbtnHoaDonDo.Tag = ItemType.HoaDonDo;
                bbtnHoaDonDo.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItems(new BarItem[] { bbtnHoaDonDo });

                //Xem lịch sửa giao dịch
                BarButtonItem bbtnLichSuGiaoDich = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__POPUP_MENU__ITEM_TRANSACTION_LIST", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 5);
                bbtnLichSuGiaoDich.Tag = ItemType.LichSuGiaoDich;
                bbtnLichSuGiaoDich.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItems(new BarItem[] { bbtnLichSuGiaoDich });

                //Lịch sử tác động hồ sơ
                BarButtonItem bbtnLichSuTacDongHoSo = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__POPUP_MENU__ITEM_LICH_SU_TAC_DONG_HO_SO", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 11);
                bbtnLichSuTacDongHoSo.Tag = ItemType.LichSuTacDongHoSo;
                bbtnLichSuTacDongHoSo.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItems(new BarItem[] { bbtnLichSuTacDongHoSo });

                //Lịch sử tác động hồ sơ
                BarButtonItem bbtnCheckInfoBHYT = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__POPUP_MENU__ITEM_CheckInfoBHYT", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 10);
                bbtnCheckInfoBHYT.Tag = ItemType.CheckInfoBHYT;
                bbtnCheckInfoBHYT.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItems(new BarItem[] { bbtnCheckInfoBHYT });
                //Duyệt y lệnh CLS
                BarButtonItem bbtnDuyetCLS = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__POPUP_MENU__ITEM_CLS", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 5);
                bbtnDuyetCLS.Tag = ItemType.CLS;
                bbtnDuyetCLS.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItems(new BarItem[] { bbtnDuyetCLS });

                //In Phiếu hoàn ứng thành toán ra viện
                if (this.currentTreatment.IS_PAUSE == 1 && this.currentTreatment.TOTAL_BILL_AMOUNT > 0)
                {
                    BarButtonItem bbtnPhieuThanhToan = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__POPUP_MENU__ITEM_PHIEU_THANH_TOAN_RA_VIEN", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 11);
                    bbtnPhieuThanhToan.Tag = ItemType.InThanhToan;
                    bbtnPhieuThanhToan.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    this._Menu.AddItems(new BarItem[] { bbtnPhieuThanhToan });
                }

                //In Phiếu Trích sao chi phí điều trị
                if (this.currentTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU || this.currentTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
                {
                    BarButtonItem bbtnTrichSao = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__POPUP_MENU__ITEM_TRICH_SAO_CHI_PHI", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 11);
                    bbtnTrichSao.Tag = ItemType.InTrichSao;
                    bbtnTrichSao.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    this._Menu.AddItems(new BarItem[] { bbtnTrichSao });
                }

                //có thanh toán thì mới hiển thị
                if (this.currentTreatment.TOTAL_BILL_AMOUNT.HasValue)
                {
                    //in bảng kê chi phí khám chưa bệnh => bảng kê
                    BarButtonItem btnHoaDonDienTu = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__POPUP_MENU__ITEM_HOA_DON_DIEN_TU", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 0);
                    btnHoaDonDienTu.Tag = ItemType.HoaDonDienTu;
                    btnHoaDonDienTu.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    this._Menu.AddItems(new BarItem[] { btnHoaDonDienTu });
                }

                #region Giao dịch
                BarSubItem sub1 = new BarSubItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__POPUP_MENU__ITEM_TRANSACTION", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 4);

                if (this.PopupItemStatusAdo.TransactionDepositStt)
                {
                    //hoàn ứng nội trú
                    BarButtonItem bbtnHoanUngNoiTru = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__POPUP_MENU__ITEM_HOAN_UNG_NOI_TRU", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 7);
                    bbtnHoanUngNoiTru.Tag = ItemType.HoanUngNoiTru;
                    bbtnHoanUngNoiTru.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    sub1.ItemLinks.Add(bbtnHoanUngNoiTru);
                }

                if (this.PopupItemStatusAdo.TransactionAllStt)
                {
                    // tam ung noi tru
                    BarButtonItem bbtnTamUngNoiTru = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__POPUP_MENU__ITEM_TAM_UNG_NOI_TRU", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 6);
                    bbtnTamUngNoiTru.Tag = ItemType.TamUngNoiTru;
                    bbtnTamUngNoiTru.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    sub1.ItemLinks.Add(bbtnTamUngNoiTru);

                    //Thanh toán
                    if (HisConfigCFG.MustFinishedForBilling != "3"
                        || this.currentTreatment.IS_PAUSE == (short)1)
                    {
                        BarButtonItem bbtnThanhToan = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__POPUP_MENU__ITEM_THANH_TOAN", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 8);
                        bbtnThanhToan.Tag = ItemType.ThanhToan;
                        bbtnThanhToan.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                        sub1.ItemLinks.Add(bbtnThanhToan);
                    }

                    //Tạm thu dịch vụ
                    BarButtonItem bbtnTamThuDichVu = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__POPUP_MENU__ITEM_TAM_THU_DICH_VU", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 9);
                    bbtnTamThuDichVu.Tag = ItemType.TamThuDichVu;
                    bbtnTamThuDichVu.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    sub1.ItemLinks.Add(bbtnTamThuDichVu);

                    //Thu trực tiếp
                    BarButtonItem bbtnThuTrucTiep = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__POPUP_MENU__ITEM_THU_TRUC_TIEP", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 8);
                    bbtnThuTrucTiep.Tag = ItemType.ThuTrucTiep;
                    bbtnThuTrucTiep.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    if (HisConfigCFG.DirectlyBillingOption == "2" && !this.currentTreatment.IS_PAUSE.HasValue && this.currentTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                    {
                        sub1.ItemLinks.Add(bbtnThuTrucTiep);
                    }
                    else
                    {
                        sub1.ItemLinks.Add(bbtnThuTrucTiep);
                    }

                    //Hoàn ứng dịch vụ
                    BarButtonItem bbtnHoanUngDichVu = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__POPUP_MENU__ITEM_HOAN_UNG_DICH_VU", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 10);
                    bbtnHoanUngDichVu.Tag = ItemType.HoanUngDichVu;
                    bbtnHoanUngDichVu.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    sub1.ItemLinks.Add(bbtnHoanUngDichVu);

                    //Chốt nợ
                    BarButtonItem bbtnChotNo = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__POPUP_MENU__ITEM_CHOT_NO", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 10);
                    bbtnChotNo.Tag = ItemType.ChotNo;
                    bbtnChotNo.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    sub1.ItemLinks.Add(bbtnChotNo);

                    //Thu nợ
                    BarButtonItem bbtnThuNo = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__POPUP_MENU__ITEM_THU_NO", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 9);
                    bbtnThuNo.Tag = ItemType.ThuNo;
                    bbtnThuNo.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    sub1.ItemLinks.Add(bbtnThuNo);

                    //Thanh toán khác
                    BarButtonItem btnThanhToanKhac = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__POPUP_MENU__ITEM_THANH_TON_KHAC", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 10);
                    btnThanhToanKhac.Tag = ItemType.ThanhToanKhac;
                    btnThanhToanKhac.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    sub1.ItemLinks.Add(btnThanhToanKhac);
                }
                else if ((this.currentTreatment.IS_ACTIVE != 0 || (this.currentTreatment.IS_LOCK_HEIN == 1 && this.currentTreatment.IS_ACTIVE == 0)))
                {
                    //Thanh toán
                    BarButtonItem bbtnThanhToan = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__POPUP_MENU__ITEM_THANH_TOAN", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 8);
                    bbtnThanhToan.Tag = ItemType.ThanhToan;
                    bbtnThanhToan.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    sub1.ItemLinks.Add(bbtnThanhToan);

                }

                if (this.currentTreatment.IS_LOCK_FEE != 1)
                {
                    // Miễn giảm
                    BarButtonItem bbtnMienGiam = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__POPUP_MENU__ITEM_MIEN_GIAM", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 6);
                    bbtnMienGiam.Tag = ItemType.MienGiam;
                    bbtnMienGiam.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    sub1.ItemLinks.Add(bbtnMienGiam);
                }



                this._Menu.AddItems(new BarItem[] { sub1 });
                #endregion

                //Yêu cầu tạm ứng
                BarButtonItem bbtnYctu= new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__POPUP_MENU__ITEM_YCTU", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 5);
                bbtnYctu.Tag = ItemType.YCTU;
                bbtnYctu.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItems(new BarItem[] { bbtnYctu });
                this._Menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
