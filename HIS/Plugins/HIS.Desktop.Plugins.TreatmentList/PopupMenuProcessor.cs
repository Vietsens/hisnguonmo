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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Library.FormMedicalRecord;
using HIS.Desktop.Plugins.Library.FormOtherTreatment;
using HIS.Desktop.Plugins.TreatmentList.Resources;
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
        internal PopupMenu _Menu = null;
        TreatmentMouseRightClick _MouseRightClick;
        V_HIS_TREATMENT_4 _TreatmentPoppupPrint;
        HIS_TREATMENT _treament;
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
            CertificateOfTBTreatment,
            TreatmentBedRoomList,
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
            AssignPan,
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
            DongboEMR,
            GiayRaVien,
            ViewHSSKCN,
            GiayPTTT,
            GiayTT,
            _PhieuHenKham,
            _PhieuHenMo,
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

            MRRegulationslist,
            DebateDiagnostic,
            DaSuaBenhAn,
            CareSlipList,
            MediReactSum,
            AccidentHurt,
            InfusionSumByTreatment,
            BloodTransfusion,
            BenhAnNgoaiTruDayMat,
            BenhAnNgoaiTruGlaucoma,
            deathdiagnosis,
            severeillness,
            SendOldSystemIntegration,
            SendTreatmentOfOldSystem,
            PublicServices_NT,
            CheckInfoBHYT,
            EndTypeExt,
            PublicServices_NT_ByDay,
            TomTatBenhAn330or331,
            CheckingTreatmentEmr,
            PhieuDKSDThuocDVKTNgoaiBHYT,
            BanGiaoBNTruocPTTT,
            PhieuXNDuongMauMaoMach,
            PhieuSangLocDinhDuongNguoiBenh,
            PhieuCongKhaiThuocTheoNgay,
            AppointmentInfo,
            PhieuHuyThuocVatTu,
            HoSoGiayToDinhKem,
            ChiTietBenhAn,
            ThongTinDichTe,
            InDonThuoc,
            ChanDoanTuVong,
            InDonThuocPTTT,
            BenhNangXinVe,
            TuVong,
            HivTreatment,
            MedicalAssessment,
            TuberclusisTreatment

            //ThongTinCungChiTra
        }
        internal void InitMenu(MediRecordMenuPopupProcessor emrMenuPopupProcessor, long roomId)
        {
            try
            {
                if (this._BarManager == null || this._MouseRightClick == null)
                    return;
                if (this._Menu == null)
                    this._Menu = new PopupMenu(this._BarManager);
                this._Menu.ItemLinks.Clear();

                //Vỏ bệnh án
                HIS.Desktop.Plugins.Library.FormMedicalRecord.Base.EmrInputADO emrInputAdo = new Library.FormMedicalRecord.Base.EmrInputADO();
                emrInputAdo.TreatmentId = this._TreatmentPoppupPrint.ID;
                emrInputAdo.PatientId = this._TreatmentPoppupPrint.PATIENT_ID;
                var data = BackendDataWorker.Get<HIS_EMR_COVER_CONFIG>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                    && o.ROOM_ID == roomId && o.TREATMENT_TYPE_ID == this._TreatmentPoppupPrint.TDL_TREATMENT_TYPE_ID).ToList();
                if (this._TreatmentPoppupPrint.EMR_COVER_TYPE_ID != null)
                {
                    emrInputAdo.EmrCoverTypeId = this._TreatmentPoppupPrint.EMR_COVER_TYPE_ID.Value;
                }
                else
                {
                    if (data != null && data.Count > 0)
                    {
                        if (data.Count == 1)
                        {
                            emrInputAdo.EmrCoverTypeId = data.FirstOrDefault().EMR_COVER_TYPE_ID;
                        }
                        else
                        {
                            emrInputAdo.lstEmrCoverTypeId = new List<long>();
                            emrInputAdo.lstEmrCoverTypeId = data.Select(o => o.EMR_COVER_TYPE_ID).ToList();
                        }
                    }
                    else
                    {
                        var DepartmentID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == roomId).DepartmentId;

                        var DataConfig = BackendDataWorker.Get<HIS_EMR_COVER_CONFIG>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                    && o.DEPARTMENT_ID == DepartmentID && o.TREATMENT_TYPE_ID == this._TreatmentPoppupPrint.TDL_TREATMENT_TYPE_ID).ToList();

                        if (DataConfig != null && DataConfig.Count > 0)
                        {
                            if (DataConfig.Count == 1)
                            {
                                emrInputAdo.EmrCoverTypeId = DataConfig.FirstOrDefault().EMR_COVER_TYPE_ID;
                            }
                            else
                            {
                                emrInputAdo.lstEmrCoverTypeId = new List<long>();
                                emrInputAdo.lstEmrCoverTypeId = DataConfig.Select(o => o.EMR_COVER_TYPE_ID).ToList();
                            }
                        }
                    }
                }

                emrInputAdo.roomId = roomId;
                emrInputAdo.DepartmentId = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == roomId).DepartmentId; 
                emrInputAdo.TreatmentTypeId = _TreatmentPoppupPrint.TDL_TREATMENT_TYPE_ID;

                var treatment = new HIS_TREATMENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(treatment, _TreatmentPoppupPrint);
                treatment.ID = _TreatmentPoppupPrint.ID;
                emrInputAdo.Treatment = treatment;
                emrMenuPopupProcessor.InitMenu(this._Menu, this._BarManager, emrInputAdo);

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
                // Thông tin dịch tễ
                BarButtonItem bbtnEpidemiologyInfo = new BarButtonItem(this._BarManager, "Thông tin dịch tễ", 1);
                bbtnEpidemiologyInfo.Tag = ItemType.ThongTinDichTe;
                bbtnEpidemiologyInfo.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subBenhNhan.AddItem(bbtnEpidemiologyInfo);
                this._Menu.AddItems(new BarItem[] { subBenhNhan });

                BarSubItem subCongKhai = new BarSubItem(this._BarManager, "Công khai thuốc, dịch vụ", 2);

                //Công khia thuốc
                BarButtonItem bbtnPublicMedicineByPhased = new BarButtonItem(this._BarManager, "Công khai thuốc theo giai đoạn", 2);
                bbtnPublicMedicineByPhased.Tag = ItemType.PublicMedicineByPhased;
                bbtnPublicMedicineByPhased.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subCongKhai.AddItem(bbtnPublicMedicineByPhased);

                //Công khia thuốc theo giai đoạn
                BarButtonItem bbtnPublicMedicineByDate = new BarButtonItem(this._BarManager, "Công khai thuốc theo ngày", 2);
                bbtnPublicMedicineByDate.Tag = ItemType.PhieuCongKhaiThuocTheoNgay;
                bbtnPublicMedicineByDate.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subCongKhai.AddItem(bbtnPublicMedicineByDate);

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

                //Giấy xác nhận điều trị bệnh lao
                BarButtonItem bbtnCertificateOfTBTreatment = new BarButtonItem(this._BarManager, "Giấy xác nhận điều trị bệnh lao", 2);
                bbtnCertificateOfTBTreatment.Tag = ItemType.CertificateOfTBTreatment;
                bbtnCertificateOfTBTreatment.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItem(bbtnCertificateOfTBTreatment).BeginGroup = true;

                BarSubItem subDieuTri = new BarSubItem(this._BarManager, "Hồ sơ điều trị", 2);
                //Lịch sử buồng bệnh    
                BarButtonItem bbtnTreatmentBedRoomList = new BarButtonItem(this._BarManager, "Lịch sử buồng bệnh", 7);
                bbtnTreatmentBedRoomList.Tag = ItemType.TreatmentBedRoomList;
                bbtnTreatmentBedRoomList.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subDieuTri.ItemLinks.Add(bbtnTreatmentBedRoomList);
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
                //Giám định y khoa
                BarButtonItem bbtnMedicalAssessment = new BarButtonItem(this._BarManager, "Giám định y khoa", 1);
                bbtnMedicalAssessment.Tag = ItemType.MedicalAssessment;
                bbtnMedicalAssessment.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subDieuTri.ItemLinks.Add(bbtnMedicalAssessment);
                //Thông tin điều trị HIV/AIDS
                BarButtonItem bbtnHivTreatment = new BarButtonItem(this._BarManager, "Thông tin điều trị HIV/AIDS", 1);
                bbtnHivTreatment.Tag = ItemType.HivTreatment;
                bbtnHivTreatment.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subDieuTri.ItemLinks.Add(bbtnHivTreatment);

                #region ----- Thông tin điều trị bệnh lao
                BarButtonItem itemBenhLao = new BarButtonItem(_BarManager, "Thông tin điều trị bệnh lao", 8);
                itemBenhLao.Tag = ItemType.TuberclusisTreatment;
                itemBenhLao.ItemClick += new ItemClickEventHandler(_MouseRightClick);
                subDieuTri.AddItem(itemBenhLao);
                #endregion
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
                if (this._TreatmentPoppupPrint != null && _TreatmentPoppupPrint.APPROVAL_STORE_STT_ID == 2)
                {
                    BarButtonItem DaSuaBenhAnSub = new BarButtonItem(_BarManager, "Đã sửa bệnh án", 9);
                    DaSuaBenhAnSub.Tag = ItemType.DaSuaBenhAn;
                    DaSuaBenhAnSub.ItemClick += new ItemClickEventHandler(_MouseRightClick);
                    subBenhAn.AddItem(DaSuaBenhAnSub);
                }

                BarButtonItem itemMRRegulationslist = new BarButtonItem(_BarManager, "Bảng kiểm thực hiện quy chế hồ sơ bệnh án", 9);
                itemMRRegulationslist.Tag = ItemType.MRRegulationslist;
                itemMRRegulationslist.ItemClick += new ItemClickEventHandler(_MouseRightClick);
                subBenhAn.AddItem(itemMRRegulationslist);

                BarButtonItem itemDebateDiagnostic = new BarButtonItem(_BarManager, "Danh sách biên bản hội chẩn", 9);
                itemDebateDiagnostic.Tag = ItemType.DebateDiagnostic;
                itemDebateDiagnostic.ItemClick += new ItemClickEventHandler(_MouseRightClick);
                subBenhAn.AddItem(itemDebateDiagnostic);

                //BarButtonItem itemCareSlipList = new BarButtonItem(_BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__CHAM_SOC", Base.ResourceLangManager.LanguageUCTreatmentList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 9);
                BarButtonItem itemCareSlipList = new BarButtonItem(_BarManager, "Chăm sóc", 9);
                itemCareSlipList.Tag = ItemType.CareSlipList;
                itemCareSlipList.ItemClick += new ItemClickEventHandler(_MouseRightClick);
                subBenhAn.AddItem(itemCareSlipList);

                // chi tiet benh an

                BarButtonItem itemPatientDetail = new BarButtonItem(_BarManager, "Chi tiết bệnh án", 9);
                itemPatientDetail.Tag = ItemType.ChiTietBenhAn;
                itemPatientDetail.ItemClick += new ItemClickEventHandler(_MouseRightClick);
                subBenhAn.AddItem(itemPatientDetail);

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

                //Bệnh án ngoại trú đáy mắt (BMK)
                BarButtonItem itemBenhAnNgoaiTruDayMat = new BarButtonItem(_BarManager, "Bệnh án ngoại trú đáy mắt (BMK)", 9);
                itemBenhAnNgoaiTruDayMat.Tag = ItemType.BenhAnNgoaiTruDayMat;
                itemBenhAnNgoaiTruDayMat.ItemClick += new ItemClickEventHandler(_MouseRightClick);
                subBenhAn.AddItem(itemBenhAnNgoaiTruDayMat);
                

                //Bệnh án ngoại trú Glaucoma (BMK)
                BarButtonItem itemBenhAnNgoaiTruGlaucoma = new BarButtonItem(_BarManager, "Bệnh án ngoại trú Glaucoma (BMK)", 9);
                itemBenhAnNgoaiTruGlaucoma.Tag = ItemType.BenhAnNgoaiTruGlaucoma;
                itemBenhAnNgoaiTruGlaucoma.ItemClick += new ItemClickEventHandler(_MouseRightClick);
                subBenhAn.AddItem(itemBenhAnNgoaiTruGlaucoma);
                
                
                

                
                
                if (_TreatmentPoppupPrint.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__XINRAVIEN && _TreatmentPoppupPrint.TREATMENT_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__NANG)
                {
                    //Phiếu tóm tắt thông tin bệnh nặng xin về
                    BarButtonItem itemsevereillness = new BarButtonItem(_BarManager, "Phiếu tóm tắt thông tin bệnh nặng xin về", 9);
                    itemsevereillness.Tag = ItemType.severeillness;
                    itemsevereillness.ItemClick += new ItemClickEventHandler(_MouseRightClick);
                    subBenhAn.AddItem(itemsevereillness);
                }
                
                else if (_TreatmentPoppupPrint.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET)
                    {
                        //Phiếu chuẩn đoán nguyên nhân tử vong
                            BarButtonItem itemdeathdiagnosis = new BarButtonItem(_BarManager, "Phiếu chẩn đoán nguyên nhân tử vong", 9);
                            itemdeathdiagnosis.Tag = ItemType.deathdiagnosis;
                            itemdeathdiagnosis.ItemClick += new ItemClickEventHandler(_MouseRightClick);
                            subBenhAn.AddItem(itemdeathdiagnosis);
                    }
                

                
                
                

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

                bool begin = true;
                //Thong tin hen kham
                if (this._TreatmentPoppupPrint.TREATMENT_END_TYPE_ID.HasValue &&
                    (this._TreatmentPoppupPrint.TREATMENT_END_TYPE_ID.Value == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN
                    || this._TreatmentPoppupPrint.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN
                    || this._TreatmentPoppupPrint.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__XINRAVIEN))
                {
                    begin = false;
                    BarButtonItem bbtnAppointmentInfo = new BarButtonItem(this._BarManager, ResourceMessage.ThongTinHenKham, 0);
                    bbtnAppointmentInfo.Tag = ItemType.AppointmentInfo;
                    bbtnAppointmentInfo.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    this._Menu.AddItem(bbtnAppointmentInfo).BeginGroup = true;
                }
                // Hồ sơ giấy tờ đính kèm
                BarButtonItem btnHoSoGiayToDinhKem = new BarButtonItem(this._BarManager, "Hồ sơ giấy tờ đính kèm", 1);
                btnHoSoGiayToDinhKem.Tag = ItemType.HoSoGiayToDinhKem;
                btnHoSoGiayToDinhKem.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItems(new BarItem[] { btnHoSoGiayToDinhKem });

                //Dịch vụ hẹn khám
                BarButtonItem bbtnAppointmentService = new BarButtonItem(this._BarManager, "Dịch vụ hẹn khám", 0);
                bbtnAppointmentService.Tag = ItemType.AppointmentService;
                bbtnAppointmentService.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItem(bbtnAppointmentService).BeginGroup = begin;

                //Chỉ định dịch vụ
                BarButtonItem bbtnAssignService = new BarButtonItem(this._BarManager, "Chỉ định dịch vụ", 0);
                bbtnAssignService.Tag = ItemType.AssignService;
                bbtnAssignService.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItem(bbtnAssignService).BeginGroup = true;

                //Chỉ định giải phẫu bệnh
                BarButtonItem bbtnAssignPan = new BarButtonItem(this._BarManager, "Chỉ định giải phẫu bệnh", 0);
                bbtnAssignPan.Tag = ItemType.AssignPan;
                bbtnAssignPan.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                this._Menu.AddItem(bbtnAssignPan);

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

                BarSubItem otherForm = new BarSubItem(this._BarManager, "Biểu mẫu khác", 1);

                BarButtonItem _barSDThuocDVKTNgoaiBHYT = new BarButtonItem(this._BarManager, "Giấy ĐK sử dụng thuốc/dvkt ngoài bhyt", 1);
                _barSDThuocDVKTNgoaiBHYT.Tag = ItemType.PhieuDKSDThuocDVKTNgoaiBHYT;
                _barSDThuocDVKTNgoaiBHYT.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                otherForm.AddItem(_barSDThuocDVKTNgoaiBHYT);

                BarButtonItem _barBanGiaoBNTruocPTTT = new BarButtonItem(this._BarManager, "Bàn giao người bệnh trước PTTT", 1);
                _barBanGiaoBNTruocPTTT.Tag = ItemType.BanGiaoBNTruocPTTT;
                _barBanGiaoBNTruocPTTT.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                otherForm.AddItem(_barBanGiaoBNTruocPTTT);

                BarButtonItem _barPhieuXNDuongMauMaoMach = new BarButtonItem(this._BarManager, "Phiếu XN đường máu mao mạch", 1);
                _barPhieuXNDuongMauMaoMach.Tag = ItemType.PhieuXNDuongMauMaoMach;
                _barPhieuXNDuongMauMaoMach.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                otherForm.AddItem(_barPhieuXNDuongMauMaoMach);

                BarButtonItem _barPhieuSangLocDinhDuongNguoiBenh = new BarButtonItem(this._BarManager, "Phiếu sàng lọc dinh dưỡng người bệnh", 1);
                _barPhieuSangLocDinhDuongNguoiBenh.Tag = ItemType.PhieuSangLocDinhDuongNguoiBenh;
                _barPhieuSangLocDinhDuongNguoiBenh.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                otherForm.AddItem(_barPhieuSangLocDinhDuongNguoiBenh);


                BarButtonItem _barChanDoanTuVong = new BarButtonItem(this._BarManager, "Phiếu chẩn đoán nguyên nhân tử vong", 1);
                _barChanDoanTuVong.Tag = ItemType.ChanDoanTuVong;
                _barChanDoanTuVong.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                otherForm.AddItem(_barChanDoanTuVong);

                subInAn.AddItems(new BarItem[] { otherForm });

                BarButtonItem _barGiayKhamBenhVaoVien = new BarButtonItem(this._BarManager, "Giấy khám bệnh vào viện", 1);
                _barGiayKhamBenhVaoVien.Tag = ItemType._GiayKhamBenhVaoVien;
                _barGiayKhamBenhVaoVien.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subInAn.AddItem(_barGiayKhamBenhVaoVien);

                BarButtonItem _barBenhAnNgoaiTru = new BarButtonItem(this._BarManager, "Bệnh án ngoại trú", 1);
                _barBenhAnNgoaiTru.Tag = ItemType._BenhAnNgoaiTru;
                _barBenhAnNgoaiTru.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subInAn.AddItem(_barBenhAnNgoaiTru);

                BarButtonItem _barGiayTHXN = new BarButtonItem(this._BarManager, "Tóm tắt kết quả CLS (Giấy tổng hợp kết quả xét nghiệm)", 1);
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
                    else if (this._TreatmentPoppupPrint.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN || this._TreatmentPoppupPrint.APPOINTMENT_TIME != null)
                    {
                        BarButtonItem _barPhieuhenKham = new BarButtonItem(this._BarManager, "Phiếu hẹn khám", 1);
                        _barPhieuhenKham.Tag = ItemType._PhieuHenKham;
                        _barPhieuhenKham.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                        subInAn.AddItem(_barPhieuhenKham);
                    }
                    if (this._TreatmentPoppupPrint.TREATMENT_END_TYPE_EXT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__HEN_MO)
                    {
                        BarButtonItem _barPhieuhenMo = new BarButtonItem(this._BarManager, "Phiếu hẹn mổ", 1);
                        _barPhieuhenMo.Tag = ItemType._PhieuHenMo;
                        _barPhieuhenMo.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                        subInAn.AddItem(_barPhieuhenMo);
                    }
                    if (this._TreatmentPoppupPrint.TREATMENT_END_TYPE_ID.HasValue
                        && (this._TreatmentPoppupPrint.TDL_TREATMENT_TYPE_ID ?? 0) != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM
                        )
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

                        BarButtonItem _barGiayTT = new BarButtonItem(this._BarManager, "Giấy chứng nhận thủ thuật", 1);
                        _barGiayTT.Tag = ItemType.GiayTT;
                        _barGiayTT.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                        subInAn.AddItem(_barGiayTT);
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

                BarButtonItem _barPhieuHuyThuocVatTu = new BarButtonItem(this._BarManager, "Phiếu hủy thuốc/vật tư", 1);
                _barPhieuHuyThuocVatTu.Tag = ItemType.PhieuHuyThuocVatTu;
                _barPhieuHuyThuocVatTu.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subInAn.AddItem(_barPhieuHuyThuocVatTu);

                 if (this._TreatmentPoppupPrint.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
				{
                    BarButtonItem _barPhieuInDonThuoc = new BarButtonItem(this._BarManager, "In đơn thuốc", 1);
                    _barPhieuInDonThuoc.Tag = ItemType.InDonThuoc;
                    _barPhieuInDonThuoc.ItemClick += new ItemClickEventHandler(_MouseRightClick);
                    subInAn.AddItem(_barPhieuInDonThuoc);
                }

                BarButtonItem _barPhieuInDonThuocPTTT = new BarButtonItem(this._BarManager, "Tóm tắt y lệnh phẫu thuật thủ thuật và đơn thuốc", 1);
                _barPhieuInDonThuocPTTT.Tag = ItemType.InDonThuocPTTT;
                _barPhieuInDonThuocPTTT.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                subInAn.AddItem(_barPhieuInDonThuocPTTT);

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
                BarButtonItem bbtnSarprintList = new BarButtonItem(this._BarManager, "Biểu mẫu khác hồ sơ điều trị", 1);
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
                //Yêu cầu tạm ứng
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

                //Đồng bộ lại EMR (iss 29253 )
                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HAS_CONNECTION_EMR") == "1")
                {
                    BarButtonItem bbtnDongboEMR = new BarButtonItem(this._BarManager, "Đồng bộ lại EMR", 1);
                    bbtnDongboEMR.Tag = ItemType.DongboEMR;
                    bbtnDongboEMR.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    this._Menu.AddItems(new BarItem[] { bbtnDongboEMR });

                    BarButtonItem bbtnCheckEMR = new BarButtonItem(this._BarManager, "Tra soát hố sơ bệnh án", 1);
                    bbtnCheckEMR.Tag = ItemType.CheckingTreatmentEmr;
                    bbtnCheckEMR.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    this._Menu.AddItems(new BarItem[] { bbtnCheckEMR });
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

                //if (treatData.FUND_ID.HasValue)
                //{
                //    BarButtonItem bbtCct = new BarButtonItem(this._BarManager, "Thông tin đơn vị cùng chi trả", 1);
                //    bbtCct.Tag = ItemType.ThongTinCungChiTra;
                //    bbtCct.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                //    this._Menu.AddItems(new BarItem[] { bbtCct });
                //}

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
