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
using HIS.Desktop.Plugins.Library.FormOtherTreatment;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentList
{
    delegate void TreatmentMouseRightClick(object sender, ItemClickEventArgs e);
    class PopupMenuProcessor
    {
        BarManager _BarManager = null;
        PopupMenu _Menu = null;
        TreatmentMouseRightClick _MouseRightClick;
        V_HIS_TREATMENT_4 _TreatmentPoppupPrint;
        RefeshReference BtnRefreshPhimTat;
        internal PopupMenuProcessor(V_HIS_TREATMENT_4 currenttreatment, BarManager barManager, TreatmentMouseRightClick mouseRightClick, RefeshReference _BtnRefreshPhimTat)
        {
            this._BarManager = barManager;
            this._MouseRightClick = mouseRightClick;
            this._TreatmentPoppupPrint = currenttreatment;
            this.BtnRefreshPhimTat = _BtnRefreshPhimTat;
        }

        internal enum ItemType
        {
            #region --- ERM
            Erm,
            Erm2,
            Erm3,
            Erm4,
            Erm5,
            Erm6,
            Erm7,
            Erm8,
            Erm9,
            Erm10,
            Erm11,
            Erm12,
            Erm13,
            Erm14,
            Erm15,
            Erm16,
            Erm17,
            Erm18,
            Erm19,
            Erm20,
            Erm21,
            Erm22,
            Erm23,
            Erm24,
            Erm25,
            Erm26,
            Erm27,
            Erm30,
            Erm33,
            #endregion
            BedHistory,
            EventLog,
            Tracking,
            vi,
            Timeline,
            PatientUpdate,
            Finishtreat,
            OpenTreat,
            Bo,
            print,
            BornInfo,
            DeathInfo,
            AssignService,
            AppointmentService,
            ComminBed,
            fixTreatment,
            ViewPackge,
            patientInf,
            MargePatient,
            SarprintList,
            HistoryTreat,
            Feehop,
            RequestDeposit,
            TimelineTest,
            TranPatiOutInfo,
            TranPatiInInfo,
            PublicMedicineByPhased,
            HisAdr,
            AllergyCard,
            GiayRaVien,
            ViewHSSKCN,
            GiayPTTT,
            _PhieuHenKham,
            _PhieuChuyenVien,
            _GiayKhamBenhVaoVien,
            _BenhAnNgoaiTru,
            _GiayTHXN,
            _HSSKCN,
            _THE_BENH_NHAN,
            BangKiemTruocTiemChung,
            ExamAggr,
            OtherFormAssTreatment,
            HisDhst,
            PREPARE,
            HisDhstChart,
            SumaryTestResults,

            DebateDiagnostic,
            CareSlipList,
            MediReactSum,
            AccidentHurt,
            InfusionSumByTreatment,
            BloodTransfusion,
            SendOldSystemIntegration,
            SendTreatmentOfOldSystem,
            PublicServices_NT,
            CheckInfoBHYT,
            EndTypeExt,
            PublicServices_NT_ByDay,
            TomTatBenhAn330or331,
            ThongTinCungChiTra
        }

        internal void InitMenu()
        {
            try
            {
                if (this._BarManager == null || this._MouseRightClick == null)
                    return;
                if (this._Menu == null)
                    this._Menu = new PopupMenu(this._BarManager);

                BarSubItem sub1 = new BarSubItem(this._BarManager, "Vỏ bệnh án", 4);

                #region ------ Benh An Dien Tu -------
                BarButtonItem bbtnErm = new BarButtonItem(this._BarManager, "Nội khoa", 4);
                bbtnErm.Tag = ItemType.Erm;
                bbtnErm.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                sub1.ItemLinks.Add(bbtnErm);

                BarButtonItem bbtnNgoaiKhoa = new BarButtonItem(this._BarManager, "Ngoại khoa", 4);
                bbtnNgoaiKhoa.Tag = ItemType.Erm2;
                bbtnNgoaiKhoa.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                sub1.ItemLinks.Add(bbtnNgoaiKhoa);

                BarButtonItem bbtnDaLieu = new BarButtonItem(this._BarManager, "Da liễu", 4);
                bbtnDaLieu.Tag = ItemType.Erm3;
                bbtnDaLieu.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                sub1.ItemLinks.Add(bbtnDaLieu);

                BarButtonItem bbtnBong = new BarButtonItem(this._BarManager, "Bỏng", 4);
                bbtnBong.Tag = ItemType.Erm4;
                bbtnBong.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                sub1.ItemLinks.Add(bbtnBong);

                BarButtonItem bbtnHuyetHocTruyenMau = new BarButtonItem(this._BarManager, "Huyết học truyền máu", 4);
                bbtnHuyetHocTruyenMau.Tag = ItemType.Erm5;
                bbtnHuyetHocTruyenMau.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                sub1.ItemLinks.Add(bbtnHuyetHocTruyenMau);

                BarSubItem subMat = new BarSubItem(this._BarManager, "Mắt", 4);
                BarButtonItem bbtnMatBanPhanTruoc = new BarButtonItem(this._BarManager, "Mắt _ Bán phần trước", 4);
                bbtnMatBanPhanTruoc.Tag = ItemType.Erm6;
                bbtnMatBanPhanTruoc.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subMat.ItemLinks.Add(bbtnMatBanPhanTruoc);

                BarButtonItem bbtnMatChanThuong = new BarButtonItem(this._BarManager, "Mắt _ Chấn thương", 4);
                bbtnMatChanThuong.Tag = ItemType.Erm7;
                bbtnMatChanThuong.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subMat.ItemLinks.Add(bbtnMatChanThuong);

                BarButtonItem bbtnMatDayMat = new BarButtonItem(this._BarManager, "Mắt _ Đáy mắt", 4);
                bbtnMatDayMat.Tag = ItemType.Erm8;
                bbtnMatDayMat.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subMat.ItemLinks.Add(bbtnMatDayMat);

                BarButtonItem bbtnMatGloCom = new BarButtonItem(this._BarManager, "Mắt _ GloCom", 4);
                bbtnMatGloCom.Tag = ItemType.Erm9;
                bbtnMatGloCom.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subMat.ItemLinks.Add(bbtnMatGloCom);

                BarButtonItem bbtnMatLac = new BarButtonItem(this._BarManager, "Mắt - Mắt lác", 4);
                bbtnMatLac.Tag = ItemType.Erm10;
                bbtnMatLac.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subMat.ItemLinks.Add(bbtnMatLac);

                BarButtonItem bbtnMatTreEm = new BarButtonItem(this._BarManager, "Mắt _ Mắt trẻ em", 4);
                bbtnMatTreEm.Tag = ItemType.Erm11;
                bbtnMatTreEm.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subMat.ItemLinks.Add(bbtnMatTreEm);
                sub1.AddItems(new BarItem[] { subMat });

                BarSubItem subNgoaiTru = new BarSubItem(this._BarManager, "Ngoại trú", 4);
                BarButtonItem bbtnNgoaiTru = new BarButtonItem(this._BarManager, "Ngoại trú", 4);
                bbtnNgoaiTru.Tag = ItemType.Erm12;
                bbtnNgoaiTru.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subNgoaiTru.ItemLinks.Add(bbtnNgoaiTru);

                BarButtonItem bbtnNgoaiTruRangHamMat = new BarButtonItem(this._BarManager, "Ngoại trú _ Răng hàm mặt", 4);
                bbtnNgoaiTruRangHamMat.Tag = ItemType.Erm13;
                bbtnNgoaiTruRangHamMat.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subNgoaiTru.ItemLinks.Add(bbtnNgoaiTruRangHamMat);

                BarButtonItem bbtnNgoaiTruTaiMuiHong = new BarButtonItem(this._BarManager, "Ngoại trú _ Tai mũi họng", 4);
                bbtnNgoaiTruTaiMuiHong.Tag = ItemType.Erm14;
                bbtnNgoaiTruTaiMuiHong.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subNgoaiTru.ItemLinks.Add(bbtnNgoaiTruTaiMuiHong);

                BarButtonItem bbtnNgoaiTruYHCT = new BarButtonItem(this._BarManager, "Ngoại trú _ YHCT", 4);
                bbtnNgoaiTruYHCT.Tag = ItemType.Erm15;
                bbtnNgoaiTruYHCT.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subNgoaiTru.ItemLinks.Add(bbtnNgoaiTruYHCT);
                sub1.AddItems(new BarItem[] { subNgoaiTru });

                BarButtonItem bbtnNhiKhoa = new BarButtonItem(this._BarManager, "Nhi khoa", 4);
                bbtnNhiKhoa.Tag = ItemType.Erm16;
                bbtnNhiKhoa.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                sub1.ItemLinks.Add(bbtnNhiKhoa);

                BarButtonItem bbtnNoiTruYHCT = new BarButtonItem(this._BarManager, "Nội trú _ YHCT", 4);
                bbtnNoiTruYHCT.Tag = ItemType.Erm17;
                bbtnNoiTruYHCT.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                sub1.ItemLinks.Add(bbtnNoiTruYHCT);

                BarButtonItem bbtndieuTribanNgay = new BarButtonItem(this._BarManager, "Điều trị ban ngày", 4);
                bbtndieuTribanNgay.Tag = ItemType.Erm30;
                bbtndieuTribanNgay.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                sub1.ItemLinks.Add(bbtndieuTribanNgay);

                BarButtonItem bbtnPhucHoiChucNang = new BarButtonItem(this._BarManager, "Phục hồi chức năng", 4);
                bbtnPhucHoiChucNang.Tag = ItemType.Erm18;
                bbtnPhucHoiChucNang.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                sub1.ItemLinks.Add(bbtnPhucHoiChucNang);

                BarButtonItem bbtnPhuKhoa = new BarButtonItem(this._BarManager, "Phụ khoa", 4);
                bbtnPhuKhoa.Tag = ItemType.Erm19;
                bbtnPhuKhoa.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                sub1.ItemLinks.Add(bbtnPhuKhoa);

                BarButtonItem bbtnRangHamMat = new BarButtonItem(this._BarManager, "Răng hàm mặt", 4);
                bbtnRangHamMat.Tag = ItemType.Erm20;
                bbtnRangHamMat.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                sub1.ItemLinks.Add(bbtnRangHamMat);

                BarButtonItem bbtnSanKhoa = new BarButtonItem(this._BarManager, "Sản khoa", 4);
                bbtnSanKhoa.Tag = ItemType.Erm21;
                bbtnSanKhoa.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                sub1.ItemLinks.Add(bbtnSanKhoa);

                BarButtonItem bbtnSoSinh = new BarButtonItem(this._BarManager, "Sơ sinh", 4);
                bbtnSoSinh.Tag = ItemType.Erm22;
                bbtnSoSinh.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                sub1.ItemLinks.Add(bbtnSoSinh);

                BarButtonItem bbtnTaiMuiHong = new BarButtonItem(this._BarManager, "Tai mũi họng", 4);
                bbtnTaiMuiHong.Tag = ItemType.Erm23;
                bbtnTaiMuiHong.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                sub1.ItemLinks.Add(bbtnTaiMuiHong);

                BarButtonItem bbtnTamThan = new BarButtonItem(this._BarManager, "Tâm thần", 4);
                bbtnTamThan.Tag = ItemType.Erm24;
                bbtnTamThan.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                sub1.ItemLinks.Add(bbtnTamThan);

                BarButtonItem bbtnTruyenNhiem = new BarButtonItem(this._BarManager, "Truyền nhiễm", 4);
                bbtnTruyenNhiem.Tag = ItemType.Erm25;
                bbtnTruyenNhiem.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                sub1.ItemLinks.Add(bbtnTruyenNhiem);

                BarButtonItem bbtnUngBuou = new BarButtonItem(this._BarManager, "Ung bướu", 4);
                bbtnUngBuou.Tag = ItemType.Erm26;
                bbtnUngBuou.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                sub1.ItemLinks.Add(bbtnUngBuou);

                BarButtonItem bbtnXaPhuong = new BarButtonItem(this._BarManager, "Xã phường", 4);
                bbtnXaPhuong.Tag = ItemType.Erm27;
                bbtnXaPhuong.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                sub1.ItemLinks.Add(bbtnXaPhuong);

                BarButtonItem bbtnTim = new BarButtonItem(this._BarManager, "Tim", 4);
                bbtnTim.Tag = ItemType.Erm33;
                bbtnTim.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                sub1.ItemLinks.Add(bbtnTim);

                this._Menu.AddItems(new BarItem[] { sub1 });
                #endregion


                #region -------
                //Bảng kê thanh toán
                BarButtonItem bbtnBo = new BarButtonItem(this._BarManager, "Bảng kê thanh toán", 5);
                bbtnBo.Tag = ItemType.Bo;
                bbtnBo.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItem(bbtnBo).BeginGroup = true;

                BarSubItem subBenhNhan = new BarSubItem(this._BarManager, "Bệnh nhân", 1);
                //Sửa bệnh nhân
                BarButtonItem bbtnPatientUpdate = new BarButtonItem(this._BarManager, "Sửa bệnh nhân", 2);
                bbtnPatientUpdate.Tag = ItemType.PatientUpdate;
                bbtnPatientUpdate.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subBenhNhan.AddItem(bbtnPatientUpdate);
                //Ghép mã bệnh nhân
                BarButtonItem bbtnMargePatient = new BarButtonItem(this._BarManager, "Ghép mã bệnh nhân", 1);
                bbtnMargePatient.Tag = ItemType.MargePatient;
                bbtnMargePatient.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subBenhNhan.AddItem(bbtnMargePatient);
                this._Menu.AddItems(new BarItem[] { subBenhNhan });

                BarSubItem subCongKhai = new BarSubItem(this._BarManager, "Công khai thuốc, dịch vụ", 2);

                //Công khia thuốc
                BarButtonItem bbtnPublicMedicineByPhased = new BarButtonItem(this._BarManager, "Công khai thuốc theo giai đoạn", 2);
                bbtnPublicMedicineByPhased.Tag = ItemType.PublicMedicineByPhased;
                bbtnPublicMedicineByPhased.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subCongKhai.AddItem(bbtnPublicMedicineByPhased);

                //Công khia dich vu
                BarButtonItem bbtnPublicServices_NT = new BarButtonItem(this._BarManager, "Công khai dịch vụ khám, chữa bệnh nội trú", 2);
                bbtnPublicServices_NT.Tag = ItemType.PublicServices_NT;
                bbtnPublicServices_NT.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subCongKhai.AddItem(bbtnPublicServices_NT);

                //Công khia dich vu
                BarButtonItem bbtnPublicServices_NT_ByDay = new BarButtonItem(this._BarManager, "Công khai dịch vụ khám, chữa bệnh nội trú theo ngày", 2);
                bbtnPublicServices_NT_ByDay.Tag = ItemType.PublicServices_NT_ByDay;
                bbtnPublicServices_NT_ByDay.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subCongKhai.AddItem(bbtnPublicServices_NT_ByDay);

                this._Menu.AddItems(new BarItem[] { subCongKhai });


                //Dòng thời gian
                BarButtonItem bbtnTimeline = new BarButtonItem(this._BarManager, "Dòng thời gian", 1);
                bbtnTimeline.Tag = ItemType.Timeline;
                bbtnTimeline.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItem(bbtnTimeline).BeginGroup = true;

                //Tổng hợp đơn phòng khám
                BarButtonItem bbtnExamAggr = new BarButtonItem(this._BarManager, "Tổng hợp đơn phòng khám", 1);
                bbtnExamAggr.Tag = ItemType.ExamAggr;
                bbtnExamAggr.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItem(bbtnExamAggr).BeginGroup = true;

                //Danh sách yêu cầu
                BarButtonItem bbtnvi = new BarButtonItem(this._BarManager, "Danh sách y lệnh", 0);
                bbtnvi.Tag = ItemType.vi;
                bbtnvi.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItems(new BarItem[] { bbtnvi });
                #endregion

                BarButtonItem itemCheckInfoBHYT = new BarButtonItem(this._BarManager, "Kiểm tra thông tin thẻ BHYT", 7);
                itemCheckInfoBHYT.Tag = ItemType.CheckInfoBHYT;
                itemCheckInfoBHYT.ItemClick += new ItemClickEventHandler(_MouseRightClick);
                _Menu.AddItems(new BarItem[] { itemCheckInfoBHYT });

                #region ------Thong Tin ra Vien --------
                //BarSubItem sub2 = new BarSubItem(this._BarManager, "Thông tin ra viện", 7);

                //// this._Menu.AddItem(bbtnDeathInfo).BeginGroup = true;
                //this._Menu.AddItems(new BarItem[] { sub2 });
                #endregion

                #region -----
                BarButtonItem itemViewHSSKCN = new BarButtonItem(this._BarManager, "Xem thông tin hồ sơ sức khỏe cá nhân", 7);
                itemViewHSSKCN.Tag = ItemType.ViewHSSKCN;
                itemViewHSSKCN.ItemClick += new ItemClickEventHandler(_MouseRightClick);
                _Menu.AddItems(new BarItem[] { itemViewHSSKCN });

                //EVentLog
                BarButtonItem bbtnEventLog = new BarButtonItem(this._BarManager, "Lịch sử tác động", 2);
                bbtnEventLog.Tag = ItemType.EventLog;
                bbtnEventLog.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItem(bbtnEventLog).BeginGroup = true;

                BarSubItem subDieuTri = new BarSubItem(this._BarManager, "Hồ sơ điều trị", 2);
                //Lịch sử giường bệnh nhân
                BarButtonItem bbtnBedHistory = new BarButtonItem(this._BarManager, "Lịch sử giường bệnh nhân", 7);
                bbtnBedHistory.Tag = ItemType.BedHistory;
                bbtnBedHistory.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subDieuTri.ItemLinks.Add(bbtnBedHistory);
                //Thông tin Phan ung
                BarButtonItem bbtnHisAdr = new BarButtonItem(this._BarManager, "Thông tin phản ứng có hại của thuốc", 7);
                bbtnHisAdr.Tag = ItemType.HisAdr;
                bbtnHisAdr.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subDieuTri.ItemLinks.Add(bbtnHisAdr);
                //Thông tin chuyển đi
                BarButtonItem bbtnTranPatiOutInfo = new BarButtonItem(this._BarManager, "Thông tin chuyển đi", 7);
                bbtnTranPatiOutInfo.Tag = ItemType.TranPatiOutInfo;
                bbtnTranPatiOutInfo.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subDieuTri.ItemLinks.Add(bbtnTranPatiOutInfo);
                //Thông tin chuyển đến
                BarButtonItem bbtnTranPatiInInfo = new BarButtonItem(this._BarManager, "Thông tin chuyển đến", 7);
                bbtnTranPatiInInfo.Tag = ItemType.TranPatiInInfo;
                bbtnTranPatiInInfo.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subDieuTri.ItemLinks.Add(bbtnTranPatiInInfo);
                //Thông tin trẻ sơ sinh
                BarButtonItem bbtnBornInfo = new BarButtonItem(this._BarManager, "Thông tin trẻ sơ sinh", 7);
                bbtnBornInfo.Tag = ItemType.BornInfo;
                bbtnBornInfo.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subDieuTri.ItemLinks.Add(bbtnBornInfo);
                //Thông tin tử vong
                BarButtonItem bbtnDeathInfo = new BarButtonItem(this._BarManager, "Thông tin tử vong", 7);
                bbtnDeathInfo.Tag = ItemType.DeathInfo;
                bbtnDeathInfo.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subDieuTri.ItemLinks.Add(bbtnDeathInfo);
                //Sửa điều trị
                BarButtonItem bbtnfixTreatment = new BarButtonItem(this._BarManager, "Sửa hồ sơ điều trị", 2);
                bbtnfixTreatment.Tag = ItemType.fixTreatment;
                bbtnfixTreatment.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subDieuTri.AddItem(bbtnfixTreatment);
                //Lịch sử điều trị
                BarButtonItem bbtnHistoryTreat = new BarButtonItem(this._BarManager, "Lịch sử điều trị", 1);
                bbtnHistoryTreat.Tag = ItemType.HistoryTreat;
                bbtnHistoryTreat.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subDieuTri.AddItem(bbtnHistoryTreat);
                //Mở lại điều trị
                BarButtonItem bbtnOpenTreat = new BarButtonItem(this._BarManager, "Mở lại điều trị", 4);
                bbtnOpenTreat.Tag = ItemType.OpenTreat;
                bbtnOpenTreat.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subDieuTri.AddItem(bbtnOpenTreat);
                //Kết thúc điều trị
                BarButtonItem bbtnFinishtreat = new BarButtonItem(this._BarManager, "Kết thúc điều trị", 3);
                bbtnFinishtreat.Tag = ItemType.Finishtreat;
                bbtnFinishtreat.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subDieuTri.AddItem(bbtnFinishtreat);

                //Tờ điều trị
                BarButtonItem bbtnTracking = new BarButtonItem(this._BarManager, "Tờ điều trị", 0);
                bbtnTracking.Tag = ItemType.Tracking;
                bbtnTracking.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subDieuTri.AddItem(bbtnTracking);

                this._Menu.AddItems(new BarItem[] { subDieuTri });

                #region ----- BenhAn
                //BarSubItem subBenhAn = new BarSubItem(_BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__SUB_BENH_AN", Base.ResourceLangManager.LanguageUCTreatmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 9);
                BarSubItem subBenhAn = new BarSubItem(_BarManager, "Bệnh án", 9);

                //BarButtonItem itemDebateDiagnostic = new BarButtonItem(_BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__BIEN_BAN_HOI_CHAN", Base.ResourceLangManager.LanguageUCTreatmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 9);
                BarButtonItem itemDebateDiagnostic = new BarButtonItem(_BarManager, "Danh sách biên bản hội chẩn", 9);
                itemDebateDiagnostic.Tag = ItemType.DebateDiagnostic;
                itemDebateDiagnostic.ItemClick += new ItemClickEventHandler(_MouseRightClick);
                subBenhAn.AddItem(itemDebateDiagnostic);

                //BarButtonItem itemCareSlipList = new BarButtonItem(_BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__CHAM_SOC", Base.ResourceLangManager.LanguageUCTreatmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 9);
                BarButtonItem itemCareSlipList = new BarButtonItem(_BarManager, "Chăm sóc", 9);
                itemCareSlipList.Tag = ItemType.CareSlipList;
                itemCareSlipList.ItemClick += new ItemClickEventHandler(_MouseRightClick);
                subBenhAn.AddItem(itemCareSlipList);

                //BarButtonItem itemMediReactSum = new BarButtonItem(_BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__PHAN_UNG_THUOC", Base.ResourceLangManager.LanguageUCTreatmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 9);
                BarButtonItem itemMediReactSum = new BarButtonItem(_BarManager, "Phản ứng thuốc", 9);
                itemMediReactSum.Tag = ItemType.MediReactSum;
                itemMediReactSum.ItemClick += new ItemClickEventHandler(_MouseRightClick);
                subBenhAn.AddItem(itemMediReactSum);

                //BarButtonItem itemAccidentHurt = new BarButtonItem(_BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__THONG_TIN_TAI_NAN_THUONG_TICH", Base.ResourceLangManager.LanguageUCTreatmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 9);
                BarButtonItem itemAccidentHurt = new BarButtonItem(_BarManager, "Thông tin tai nạn thương tích", 9);
                itemAccidentHurt.Tag = ItemType.AccidentHurt;
                itemAccidentHurt.ItemClick += new ItemClickEventHandler(_MouseRightClick);
                subBenhAn.AddItem(itemAccidentHurt);

                //BarButtonItem itemInfusionSumByTreatment = new BarButtonItem(_BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__TRUYEN_DICH", Base.ResourceLangManager.LanguageUCTreatmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 9);
                BarButtonItem itemInfusionSumByTreatment = new BarButtonItem(_BarManager, "Truyền dịch", 9);
                itemInfusionSumByTreatment.Tag = ItemType.InfusionSumByTreatment;
                itemInfusionSumByTreatment.ItemClick += new ItemClickEventHandler(_MouseRightClick);
                subBenhAn.AddItem(itemInfusionSumByTreatment);

                //BarButtonItem itemBloodTransfusion = new BarButtonItem(_BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__BloodTransfusion", Base.ResourceLangManager.LanguageUCTreatmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 9);
                BarButtonItem itemBloodTransfusion = new BarButtonItem(_BarManager, "Truyền máu", 9);
                itemBloodTransfusion.Tag = ItemType.BloodTransfusion;
                itemBloodTransfusion.ItemClick += new ItemClickEventHandler(_MouseRightClick);
                subBenhAn.AddItem(itemBloodTransfusion);

                ////BarButtonItem itemHisDhst = new BarButtonItem(_BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__HisDhst", Base.ResourceLangManager.LanguageUCTreatmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 9);
                //BarButtonItem itemHisDhst = new BarButtonItem(_BarManager, "Theo dõi chức năng sống", 9);
                //itemHisDhst.Tag = ItemType.HisDhst;
                //itemHisDhst.ItemClick += new ItemClickEventHandler(_MouseRightClick);
                //subBenhAn.AddItem(itemHisDhst);

                ////BarButtonItem itemHisDhstChart = new BarButtonItem(_BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__HisDhstChart", Base.ResourceLangManager.LanguageUCTreatmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 9);
                //BarButtonItem itemHisDhstChart = new BarButtonItem(_BarManager, "Biểu đồ dấu hiệu sinh tồn", 9);
                //itemHisDhstChart.Tag = ItemType.HisDhstChart;
                //itemHisDhstChart.ItemClick += new ItemClickEventHandler(_MouseRightClick);
                //subBenhAn.AddItem(itemHisDhstChart);

                this._Menu.AddItems(new BarItem[] { subBenhAn });
                #endregion

                //Dịch vụ hẹn khám
                BarButtonItem bbtnAppointmentService = new BarButtonItem(this._BarManager, "Dịch vụ hẹn khám", 0);
                bbtnAppointmentService.Tag = ItemType.AppointmentService;
                bbtnAppointmentService.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItem(bbtnAppointmentService).BeginGroup = true;

                //Chỉ định dịch vụ
                BarButtonItem bbtnAssignService = new BarButtonItem(this._BarManager, "Chỉ định dịch vụ", 0);
                bbtnAssignService.Tag = ItemType.AssignService;
                bbtnAssignService.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItem(bbtnAssignService).BeginGroup = true;

                //Du tru theo benh nhan
                BarButtonItem btnPrepare = new BarButtonItem(this._BarManager, "Dự trù theo bệnh nhân", 0);
                btnPrepare.Tag = ItemType.PREPARE;
                btnPrepare.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItem(btnPrepare);

                //Chuyển vào buồng
                BarButtonItem bbtnComminBed = new BarButtonItem(this._BarManager, "Chuyển vào buồng", 1);
                bbtnComminBed.Tag = ItemType.ComminBed;
                bbtnComminBed.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItems(new BarItem[] { bbtnComminBed });

                //Xem gói dịch vụ
                BarButtonItem bbtnViewPackge = new BarButtonItem(this._BarManager, "Xem gói dịch vụ", 3);
                bbtnViewPackge.Tag = ItemType.ViewPackge;
                bbtnViewPackge.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItems(new BarItem[] { bbtnViewPackge });
                #endregion

                #region -- InAn ---
                BarSubItem subInAn = new BarSubItem(this._BarManager, "In ấn", 6);

                BarButtonItem _barGiayKhamBenhVaoVien = new BarButtonItem(this._BarManager, "Giấy khám bệnh vào viện", 1);
                _barGiayKhamBenhVaoVien.Tag = ItemType._GiayKhamBenhVaoVien;
                _barGiayKhamBenhVaoVien.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subInAn.AddItem(_barGiayKhamBenhVaoVien);

                BarButtonItem _barBenhAnNgoaiTru = new BarButtonItem(this._BarManager, "Bệnh án ngoại trú", 1);
                _barBenhAnNgoaiTru.Tag = ItemType._BenhAnNgoaiTru;
                _barBenhAnNgoaiTru.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subInAn.AddItem(_barBenhAnNgoaiTru);

                BarButtonItem _barGiayTHXN = new BarButtonItem(this._BarManager, "Giấy Tổng hợp kết quả xét nghiệm", 1);
                _barGiayTHXN.Tag = ItemType._GiayTHXN;
                _barGiayTHXN.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subInAn.AddItem(_barGiayTHXN);

                if (this._TreatmentPoppupPrint != null)
                {
                    if (this._TreatmentPoppupPrint.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN)
                    {
                        BarButtonItem _barPhieuChuyenVien = new BarButtonItem(this._BarManager, "Phiếu chuyển viện", 1);
                        _barPhieuChuyenVien.Tag = ItemType._PhieuChuyenVien;
                        _barPhieuChuyenVien.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                        subInAn.AddItem(_barPhieuChuyenVien);
                    }
                    else if (this._TreatmentPoppupPrint.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN)
                    {
                        BarButtonItem _barPhieuhenKham = new BarButtonItem(this._BarManager, "Phiếu hẹn khám", 1);
                        _barPhieuhenKham.Tag = ItemType._PhieuHenKham;
                        _barPhieuhenKham.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                        subInAn.AddItem(_barPhieuhenKham);
                    }
                    if (this._TreatmentPoppupPrint.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN
                        || this._TreatmentPoppupPrint.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CTCV
                        || this._TreatmentPoppupPrint.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN
                        || this._TreatmentPoppupPrint.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN
                        || this._TreatmentPoppupPrint.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__XINRAVIEN
                        || this._TreatmentPoppupPrint.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET
                        || this._TreatmentPoppupPrint.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__KHAC)
                    {
                        BarButtonItem _barGiayRaVien = new BarButtonItem(this._BarManager, "Phiếu ra viện", 1);
                        _barGiayRaVien.Tag = ItemType.GiayRaVien;
                        _barGiayRaVien.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                        subInAn.AddItem(_barGiayRaVien);
                    }
                    if (this._TreatmentPoppupPrint != null)
                    {
                        BarButtonItem _barGiayPTTT = new BarButtonItem(this._BarManager, "Giấy chứng nhận phẫu thuật", 1);
                        _barGiayPTTT.Tag = ItemType.GiayPTTT;
                        _barGiayPTTT.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                        subInAn.AddItem(_barGiayPTTT);
                    }

                    BarButtonItem itemInBangKiemTruocTiemChung = new BarButtonItem(this._BarManager, "Bảng kiểm trước tiêm chủng", 1);
                    itemInBangKiemTruocTiemChung.Tag = ItemType.BangKiemTruocTiemChung;
                    itemInBangKiemTruocTiemChung.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    subInAn.AddItem(itemInBangKiemTruocTiemChung);
                }
                BarButtonItem _TheBenhNhan = new BarButtonItem(this._BarManager, "Thẻ bệnh nhân", 1);
                _TheBenhNhan.Tag = ItemType._THE_BENH_NHAN;
                _TheBenhNhan.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subInAn.AddItem(_TheBenhNhan);

                BarButtonItem _barHSSKCN = new BarButtonItem(this._BarManager, "Hồ sơ quản lý sức khỏe cá nhân", 1);
                _barHSSKCN.Tag = ItemType._HSSKCN;
                _barHSSKCN.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subInAn.AddItem(_barHSSKCN);

                BarButtonItem _barTomTatBenhAn = new BarButtonItem(this._BarManager, "Tóm tắt bệnh án", 1);
                _barTomTatBenhAn.Tag = ItemType.TomTatBenhAn330or331;
                _barTomTatBenhAn.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subInAn.AddItem(_barTomTatBenhAn);

                this._Menu.AddItems(new BarItem[] { subInAn });
                #endregion

                //Theo dõi chức năng sống
                BarButtonItem bbtnHisDhst = new BarButtonItem(this._BarManager, "Theo dõi chức năng sống", 2);
                bbtnHisDhst.Tag = ItemType.HisDhst;
                bbtnHisDhst.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItems(new BarItem[] { bbtnHisDhst });

                //Biểu đồ dấu hiệu sinh tồn
                BarButtonItem bbtnHisDhstChart = new BarButtonItem(this._BarManager, "Biểu đồ dấu hiệu sinh tồn", 2);
                bbtnHisDhstChart.Tag = ItemType.HisDhstChart;
                bbtnHisDhstChart.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItems(new BarItem[] { bbtnHisDhstChart });

                //Biểu mẫu khác
                BarButtonItem bbtnSarprintList = new BarButtonItem(this._BarManager, "Biểu mẫu khác", 1);
                bbtnSarprintList.Tag = ItemType.OtherFormAssTreatment;
                bbtnSarprintList.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItems(new BarItem[] { bbtnSarprintList });

                //Thông tin bệnh án
                BarButtonItem bbtnpatientInf = new BarButtonItem(this._BarManager, "Thông tin bệnh án", 4);
                bbtnpatientInf.Tag = ItemType.patientInf;
                bbtnpatientInf.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItems(new BarItem[] { bbtnpatientInf });

                //Ket qua xet nghiem
                BarButtonItem bbtnSumaryTestResults = new BarButtonItem(this._BarManager, "Kết quả xét nghiệm", 4);
                bbtnSumaryTestResults.Tag = ItemType.SumaryTestResults;
                bbtnSumaryTestResults.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItems(new BarItem[] { bbtnSumaryTestResults });
                //Thông tin viện phí
                BarButtonItem bbtnFeehop = new BarButtonItem(this._BarManager, "Thông tin viện phí", 1);
                bbtnFeehop.Tag = ItemType.Feehop;
                bbtnFeehop.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItems(new BarItem[] { bbtnFeehop });
                //Thông tin viện phí
                BarButtonItem bbtnRequestDeposit = new BarButtonItem(this._BarManager, "Yêu cầu tạm ứng", 1);
                bbtnRequestDeposit.Tag = ItemType.RequestDeposit;
                bbtnRequestDeposit.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItems(new BarItem[] { bbtnRequestDeposit });
                //Thẻ dị ứng
                BarButtonItem bbtnTheDiUng = new BarButtonItem(this._BarManager, "Thẻ dị ứng", 1);
                bbtnTheDiUng.Tag = ItemType.AllergyCard;
                bbtnTheDiUng.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItems(new BarItem[] { bbtnTheDiUng });

                //Thông tin kết thúc khác
                if (_TreatmentPoppupPrint != null)
                {
                    //if (_TreatmentPoppupPrint.TREATMENT_END_TYPE_EXT_ID.HasValue || (_TreatmentPoppupPrint.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CTCV ||
                    //    _TreatmentPoppupPrint.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__KHAC || _TreatmentPoppupPrint.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__TRON))
                    //{
                    BarButtonItem bbtnEndTypeExt = new BarButtonItem(this._BarManager, "Thông tin kết thúc khác", 1);
                    bbtnEndTypeExt.Tag = ItemType.EndTypeExt;
                    bbtnEndTypeExt.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    this._Menu.AddItems(new BarItem[] { bbtnEndTypeExt });
                    //}
                }

                BarSubItem subFormOther = new BarSubItem(this._BarManager, "Form khác", 6);

                HIS_TREATMENT treatData = new HIS_TREATMENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(treatData, _TreatmentPoppupPrint);
                FormOtherTreatmentProcessor form = new FormOtherTreatmentProcessor(treatData, BtnRefreshPhimTat);
                var lstBar = form.GetBarButtonItem(this._BarManager);
                if (lstBar != null && lstBar.Count > 0)
                {
                    foreach (var item in lstBar)
                    {
                        subFormOther.AddItem(item);
                    }
                }
                this._Menu.AddItems(new BarItem[] { subFormOther });

                if (treatData.FUND_ID.HasValue)
                {
                    BarButtonItem bbtCct = new BarButtonItem(this._BarManager, "Thông tin đơn vị cùng chi trả", 1);
                    bbtCct.Tag = ItemType.ThongTinCungChiTra;
                    bbtCct.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    this._Menu.AddItems(new BarItem[] { bbtCct });
                }

                //Gửi thông tin hồ sơ sang hệ thống cũ
                if (HIS.Desktop.Plugins.TreatmentList.Config.HisConfigCFG.OldSystemIntegrationType == "1"
                    && IsAdmin.CheckLoginAdmin.IsAdmin(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()))
                {
                    BarButtonItem bbtnOldSystemIntegration = new BarButtonItem(this._BarManager, "Gửi thông tin hồ sơ sang hệ thống cũ", 1);
                    bbtnOldSystemIntegration.Tag = ItemType.SendOldSystemIntegration;
                    bbtnOldSystemIntegration.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    this._Menu.AddItems(new BarItem[] { bbtnOldSystemIntegration });

                    BarButtonItem bbtTreatmentOfOldSystem = new BarButtonItem(this._BarManager, "Gửi thông tin hồ sơ của BN cũ sang hệ thống cũ", 1);
                    bbtTreatmentOfOldSystem.Tag = ItemType.SendTreatmentOfOldSystem;
                    bbtTreatmentOfOldSystem.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    this._Menu.AddItems(new BarItem[] { bbtTreatmentOfOldSystem });
                }

                this._Menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
