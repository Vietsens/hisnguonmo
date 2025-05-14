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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.BedRoomPartial.Key;
using HIS.Desktop.Plugins.Library.FormMedicalRecord;
using HIS.Desktop.Utility;
using Inventec.Desktop.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.BedRoomPartial
{
    delegate void BedRoomMouseRight_Click(object sender, ItemClickEventArgs e);

    class BedRoomPopupMenuProcessor : UserControlBase
    {
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        MOS.EFMODEL.DataModels.L_HIS_TREATMENT_BED_ROOM currentTreatmentBedRoom;
        BedRoomMouseRight_Click bedRoomMouseRightClick;
        BarManager barManager;
        PopupMenu menu;
        internal enum ModuleType
        {
            Care,
            SummaryInforTreatment,
            btnBedRoomIn,
            btnBedRoomOut,
            AssignService,
            AssignService__Plus,
            AssignBlood,
            TreatmentPatientUpdate,
            CareSlipList,
            MedicalInfomation,
            TranPatiToInfo,
            DebateDiagnostic,
            TrackingTreatment,
            InfusionSumByTreatment,
            AccidentHurt,
            MediReactSum,
            BedHistory,
            InfantInformation,
            TreatmentHistory,
            TreatmentFinish,
            TransDepartment,
            RequestDeposit,
            EmrDocument,
            Bordereau,
            AggrHospitalFees,
            ServiceReqList,
            SumaryTestResults,
            PublicMedicineByPhased,
            PublicMedicineByDate,
            OtherForms,
            ServicePackageView,
            TuTruc,
            TaoBienBanHoiChan,
            AssignPaan,
            GiayNamVien,
            SuaHSDT,
            BacSiDt,
            PublicService_NT,
            HisCoTreatmentCreate,
            HisCoTreatmentFinish,
            TreatmentList,
            HisAdr,
            AllergyCard,
            AssignNutrition,
            RationSchedule,
            BloodTransfusion,
            OtherFormAssTreatment,
            HisDhst,
            PREPARE,
            HisDhstChart,
            CheckInfoBHYT,
            TreatmentLog,
            HisPhieuSoKetTruocMo,
            DisApprovalFinish,
            ApprovalFinish,
            _GiayTHXN,
            CheckingTreatmentEmr,
            CamKetNamGiuongTuNguyen,
            BanGiaoBNTruocPTTT,
            PhieuKhaiThacTienSuDiUng,
            PhieuXNDuongMauMaoMach,
            HisSoKetBenhAnTruocPhauThuat,
            HisSoKetBenhAnTruocThuThuat,
            MedicinMaterialIUsed,
            HisTreatmentFile,
            PhanLoaiBenhNhan,
            Phieuchamsoc_vobenhan,
            CanTheoDoi,
            BoTheoDoi,
            HivTreatment,
            MedicalAssessment,
            TuberclusisTreatment,
            Specialty,
            Consultation,

        }
        internal ModuleType moduleType { get; set; }

        internal BedRoomPopupMenuProcessor(MOS.EFMODEL.DataModels.L_HIS_TREATMENT_BED_ROOM currentTreatmentBedRoom, BedRoomMouseRight_Click bedRoomMouseRightClick, BarManager barManager)
        {
            this.currentTreatmentBedRoom = currentTreatmentBedRoom;
            this.bedRoomMouseRightClick = bedRoomMouseRightClick;
            this.barManager = barManager;
        }

        internal BedRoomPopupMenuProcessor(Inventec.Desktop.Common.Modules.Module currentModule, MOS.EFMODEL.DataModels.L_HIS_TREATMENT_BED_ROOM currentTreatmentBedRoom, BedRoomMouseRight_Click bedRoomMouseRightClick, BarManager barManager)
            : base(currentModule)
        {
            this.currentModule = currentModule;
            this.currentTreatmentBedRoom = currentTreatmentBedRoom;
            this.bedRoomMouseRightClick = bedRoomMouseRightClick;
            this.barManager = barManager;
        }

        internal void InitMenu(MediRecordMenuPopupProcessor emrMenuPopupProcessor ,long roomId)
        {
            try
            {
                if (menu == null)
                    menu = new PopupMenu(barManager);
                // Add item and show
                menu.ItemLinks.Clear();

                long DepartmentID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == roomId).DepartmentId;
                #region -----Vỏ bệnh án
                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HAS_CONNECTION_EMR") == "1")
                {
                    HIS.Desktop.Plugins.Library.FormMedicalRecord.Base.EmrInputADO emrInputAdo = new Library.FormMedicalRecord.Base.EmrInputADO();
                    emrInputAdo.TreatmentId = this.currentTreatmentBedRoom.TREATMENT_ID;
                    emrInputAdo.PatientId = this.currentTreatmentBedRoom.PATIENT_ID;
                    if (this.currentTreatmentBedRoom.EMR_COVER_TYPE_ID != null)
                    {
                        emrInputAdo.EmrCoverTypeId = this.currentTreatmentBedRoom.EMR_COVER_TYPE_ID;
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<HIS_EMR_COVER_CONFIG>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                            && o.ROOM_ID == roomId
                        && o.TREATMENT_TYPE_ID == this.currentTreatmentBedRoom.TDL_TREATMENT_TYPE_ID
                        ).ToList();
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
                            var DataConfig = BackendDataWorker.Get<HIS_EMR_COVER_CONFIG>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                        && o.DEPARTMENT_ID == DepartmentID && o.TREATMENT_TYPE_ID == this.currentTreatmentBedRoom.TDL_TREATMENT_TYPE_ID).ToList();

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
                    emrInputAdo.DepartmentId = DepartmentID;
                    emrInputAdo.TreatmentTypeId = currentTreatmentBedRoom.TDL_TREATMENT_TYPE_ID;

                    var treatment = new HIS_TREATMENT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(treatment, currentTreatmentBedRoom);
                    treatment.ID = currentTreatmentBedRoom.TREATMENT_ID;
                    emrInputAdo.Treatment = treatment;
                    emrMenuPopupProcessor.InitMenu(this.menu, this.barManager, emrInputAdo);
                }
                #endregion

                //#region ----- Phiếu chăm sóc_Vỏ bệnh án
                //BarButtonItem itemEmrDocumentChamSoc = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__Phiếu chăm sóc_Vo", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 8);
                //itemEmrDocumentChamSoc.Tag = ModuleType.Phieuchamsoc_vobenhan;
                //itemEmrDocumentChamSoc.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                //menu.AddItems(new BarItem[] { itemEmrDocumentChamSoc });
                //#endregion

                #region ----- ChiTietThanhToan
                BarButtonItem itemEmrDocument = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__CHI_TIET_BENH_AN", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 8);
                itemEmrDocument.Tag = ModuleType.EmrDocument;
                itemEmrDocument.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                menu.AddItems(new BarItem[] { itemEmrDocument });
                #endregion

                #region ----- BangKeThanhToan
                BarButtonItem itemBordereau = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__BANG_KE", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 1);
                itemBordereau.Tag = ModuleType.Bordereau;
                itemBordereau.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                menu.AddItems(new BarItem[] { itemBordereau });
                #endregion

                #region ----- MoiKhamChuyenKhoa
                BarButtonItem itemSpecialty = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__MOI_KHAM_CHUYEN_KHOA", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 1);
                itemSpecialty.Tag = ModuleType.Specialty;
                itemSpecialty.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                menu.AddItems(new BarItem[] { itemSpecialty });
                #endregion

                #region ---MoiHoiChan
                BarButtonItem itemConsultation = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__MOI_HOI_CHAN", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 1);
                itemConsultation.Tag = ModuleType.Consultation;
                itemConsultation.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                menu.AddItems(new BarItem[] { itemConsultation });
                #endregion
                #region ----- CanTheodoi_BoTheoiDoi
                if (this.currentTreatmentBedRoom.TDL_OBSERVED_TIME_FROM ==  null || this.currentTreatmentBedRoom.TDL_OBSERVED_TIME_TO == null || this.currentTreatmentBedRoom.TDL_OBSERVED_TIME_TO < Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now))
                {
                    BarButtonItem itemCanTheodoi = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__CANTD", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 1);
                    itemCanTheodoi.Tag = ModuleType.CanTheoDoi;
                    itemCanTheodoi.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                    menu.AddItems(new BarItem[] { itemCanTheodoi });
                }
                if (this.currentTreatmentBedRoom.TDL_OBSERVED_TIME_FROM <= Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) && Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) <= this.currentTreatmentBedRoom.TDL_OBSERVED_TIME_TO)
                {
                    BarButtonItem itemBoTheodoi = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__BOTD", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 1);
                    itemBoTheodoi.Tag = ModuleType.BoTheoDoi;
                    itemBoTheodoi.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                    menu.AddItems(new BarItem[] { itemBoTheodoi });
                }
                   
                #endregion

                #region ----- ThuocVatTuBenhNhanDaDung
                BarButtonItem itemMedicinMaterialIsUsed = new BarButtonItem(barManager,Resources.ResourceMessage.ThuocVtBNDaDung, 1);
                itemMedicinMaterialIsUsed.Tag = ModuleType.MedicinMaterialIUsed;
                itemMedicinMaterialIsUsed.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                menu.AddItems(new BarItem[] { itemMedicinMaterialIsUsed });
                #endregion


                #region ----- CongKhaiThuocDichVu
                BarSubItem subCongKhai = new BarSubItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__SUB_CONG_KHAI_THUOC_DICH_VU", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 2);
                BarButtonItem itemPublicMedicineByPhased = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__CONG_KHAI_THUOC_THEO_GIAI_DOAN", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 2);
                itemPublicMedicineByPhased.Tag = ModuleType.PublicMedicineByPhased;
                itemPublicMedicineByPhased.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subCongKhai.AddItem(itemPublicMedicineByPhased);

                BarButtonItem itemPublicMedicineByDate = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__CONG_KHAI_THUOC_THEO_NGAY", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 2);
                itemPublicMedicineByDate.Tag = ModuleType.PublicMedicineByDate;
                itemPublicMedicineByDate.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subCongKhai.AddItem(itemPublicMedicineByDate);


                BarButtonItem itemPublicService_NT = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__CONG_KHAI_DICH_VU_KHAM_CHUA_BENH_NOT_TRU", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 2);
                itemPublicService_NT.Tag = ModuleType.PublicService_NT;
                itemPublicService_NT.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subCongKhai.AddItem(itemPublicService_NT);
                menu.AddItems(new BarItem[] { subCongKhai });
                #endregion

                #region ----- ThongTin
                BarSubItem subThongTin = new BarSubItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__SUB_THONG_TIN", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 3);

                #region ----- ThongTinPhaUngCoHaiThuoc
                BarButtonItem itemHisAdr = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__HIS_ADR", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 3);
                itemHisAdr.Tag = ModuleType.HisAdr;
                itemHisAdr.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subThongTin.AddItem(itemHisAdr);
                #endregion

                BarButtonItem itemAllergyCard = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__AllergyCard", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 3);
                itemAllergyCard.Tag = ModuleType.AllergyCard;
                itemAllergyCard.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subThongTin.AddItem(itemAllergyCard);

                BarButtonItem itemServiceReqList = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__DANH_SACH_YEU_CAU", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 3);
                itemServiceReqList.Tag = ModuleType.ServiceReqList;
                itemServiceReqList.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subThongTin.AddItem(itemServiceReqList);

                BarButtonItem itemSumaryTestResults = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__KET_QUA_XET_NGHIEM", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 3);
                itemSumaryTestResults.Tag = ModuleType.SumaryTestResults;
                itemSumaryTestResults.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subThongTin.AddItem(itemSumaryTestResults);

                BarButtonItem itemTreatmentHistory = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__Y_LENH_THEO_KHOA", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 3);
                itemTreatmentHistory.Tag = ModuleType.TreatmentHistory;
                itemTreatmentHistory.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subThongTin.AddItem(itemTreatmentHistory);

                BarButtonItem itemBedHistory = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__LICH_SU_GIUONG", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 3);
                itemBedHistory.Tag = ModuleType.BedHistory;
                itemBedHistory.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subThongTin.AddItem(itemBedHistory);

                BarButtonItem itemMedicalInfomation = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__THONG_TIN_BENH_AN", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 3);
                itemMedicalInfomation.Tag = ModuleType.MedicalInfomation;
                itemMedicalInfomation.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subThongTin.AddItem(itemMedicalInfomation);

                BarButtonItem itemTranPatiToInfo = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__THONG_TIN_CHUYEN_DEN", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 3);
                itemTranPatiToInfo.Tag = ModuleType.TranPatiToInfo;
                itemTranPatiToInfo.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subThongTin.AddItem(itemTranPatiToInfo);
                #region ----- Giam dinh y khoa
                BarButtonItem itemMedicalAssessment = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("GiamDinhYKhoa", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 8);
                itemMedicalAssessment.Tag = ModuleType.MedicalAssessment;
                itemMedicalAssessment.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subThongTin.AddItem(itemMedicalAssessment);
                #endregion
                #region ----- Thông tin điều trị HIV/AIDS
                BarButtonItem itemHivTreatment = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("UCTreeListService.btnHivTreatment.Text", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 8);
                itemHivTreatment.Tag = ModuleType.HivTreatment;
                itemHivTreatment.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subThongTin.AddItem(itemHivTreatment);
                #endregion

                #region ----- Thông tin điều trị bệnh lao
                BarButtonItem itemBenhLao = new BarButtonItem(barManager, "Thông tin điều trị bệnh lao", 8);
                itemBenhLao.Tag = ModuleType.TuberclusisTreatment;
                itemBenhLao.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subThongTin.AddItem(itemBenhLao);
                #endregion

                if (currentTreatmentBedRoom.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
                {
                    BarButtonItem itemInfantInformation = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__THONG_TIN_TRE_SO_SINH", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 2);
                    itemInfantInformation.Tag = ModuleType.InfantInformation;
                    itemInfantInformation.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                    subThongTin.AddItem(itemInfantInformation);
                }

                BarButtonItem itemAggrHospitalFees = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__VIEN_PHI", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 3);
                itemAggrHospitalFees.Tag = ModuleType.AggrHospitalFees;
                itemAggrHospitalFees.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subThongTin.AddItem(itemAggrHospitalFees);

                BarButtonItem itemRequestDeposit = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__YEU_CAU_TAM_UNG", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 3);
                itemRequestDeposit.Tag = ModuleType.RequestDeposit;
                itemRequestDeposit.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subThongTin.AddItem(itemRequestDeposit);

                BarButtonItem itemServicePackageView = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__XEM_GOI_DICH_VU", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 3);
                itemServicePackageView.Tag = ModuleType.ServicePackageView;
                itemServicePackageView.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subThongTin.AddItem(itemServicePackageView);

                menu.AddItems(new BarItem[] { subThongTin });
                #endregion

                #region ----- KeDonChiDinh
                BarSubItem subKeDonChiDinh = new BarSubItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__SUB_KE_DON_CHI_DINH", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 4);

                BarButtonItem itemTuTruc = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__TU_TRUC", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 4);
                itemTuTruc.Tag = ModuleType.TuTruc;
                itemTuTruc.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subKeDonChiDinh.AddItem(itemTuTruc);

                BarButtonItem itemAssignService = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__CHI_DINH", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 4);
                itemAssignService.Tag = ModuleType.AssignService;
                itemAssignService.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subKeDonChiDinh.AddItem(itemAssignService);

                BarButtonItem itemAssignNutrition = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__CHI_DINH_Dinh_DUONG", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 4);
                itemAssignNutrition.Tag = ModuleType.AssignNutrition;
                itemAssignNutrition.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subKeDonChiDinh.AddItem(itemAssignNutrition);
                if (currentTreatmentBedRoom.LAST_DEPARTMENT_ID == DepartmentID) 
                {
                    BarButtonItem itemRationSchedule = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__BAO_AN", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 4);
                    itemRationSchedule.Tag = ModuleType.RationSchedule;
                    itemRationSchedule.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                    subKeDonChiDinh.AddItem(itemRationSchedule);
                }
                BarButtonItem itemAssignPrescription = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__KE_DON_DUOC", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 4);
                itemAssignPrescription.Tag = ModuleType.AssignService__Plus;
                itemAssignPrescription.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subKeDonChiDinh.AddItem(itemAssignPrescription);

                BarButtonItem itemAssignBlood = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__KE_DON_MAU", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 4);
                itemAssignBlood.Tag = ModuleType.AssignBlood;
                itemAssignBlood.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subKeDonChiDinh.AddItem(itemAssignBlood);

                BarButtonItem itemAssignPaan = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__CHI_DINH_GIAI_PHAU_BENH_LY", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 4);
                itemAssignPaan.Tag = ModuleType.AssignPaan;
                itemAssignPaan.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subKeDonChiDinh.AddItem(itemAssignPaan);

                BarButtonItem itemPREPARE = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__DU_TRU_THUOC_THEO_BENH_NHAN", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 4);
                itemPREPARE.Tag = ModuleType.PREPARE;
                itemPREPARE.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subKeDonChiDinh.AddItem(itemPREPARE);

                menu.AddItems(new BarItem[] { subKeDonChiDinh });
                #endregion

                #region ----- KetThucDieuTri
                BarButtonItem itemTreatmentFinish = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__KET_THUC_DIEU_TRI", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 7);
                itemTreatmentFinish.Tag = ModuleType.TreatmentFinish;
                itemTreatmentFinish.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                #endregion

                #region ------ DieuTriKetHop
                if (this.currentTreatmentBedRoom != null)
                {
                    if (this.currentTreatmentBedRoom.CO_TREATMENT_ID != null)
                    {
                        BarButtonItem itemKetThucDieuTriKetHop = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__KET_THUC_DIEU_TRI_KET_HOP", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 7);
                        itemKetThucDieuTriKetHop.Tag = ModuleType.HisCoTreatmentFinish;
                        itemKetThucDieuTriKetHop.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                        menu.AddItems(new BarItem[] { itemKetThucDieuTriKetHop });
                    }
                    else
                    {
                        BarButtonItem itemDieuTriKetHop = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__CHUYEN_DIEU_TRI_KET_HOP", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 7);
                        itemDieuTriKetHop.Tag = ModuleType.HisCoTreatmentCreate;
                        itemDieuTriKetHop.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                        menu.AddItems(new BarItem[] { itemDieuTriKetHop });

                        menu.AddItems(new BarItem[] { itemTreatmentFinish });
                    }
                }
                #endregion

                #region ----- LuuChuyen
                BarSubItem subLuuChuyen = new BarSubItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__SUB_LUU_CHUYEN", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 6);
                if (this.currentTreatmentBedRoom != null && this.currentTreatmentBedRoom.CO_TREATMENT_ID == null)
                {
                    BarButtonItem itemTransDepartment = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__CHUYEN_KHOA", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 6);
                    itemTransDepartment.Tag = ModuleType.TransDepartment;
                    itemTransDepartment.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                    subLuuChuyen.AddItem(itemTransDepartment);
                }

                BarButtonItem itemBedRoomIn = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__CHUYEN_VAO_BUONG", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 6);
                itemBedRoomIn.Tag = ModuleType.btnBedRoomIn;
                itemBedRoomIn.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subLuuChuyen.AddItem(itemBedRoomIn);

                BarButtonItem itemBedRoomOut = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__CHUYEN_RA_BUONG", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 6);
                itemBedRoomOut.Tag = ModuleType.btnBedRoomOut;
                itemBedRoomOut.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subLuuChuyen.AddItem(itemBedRoomOut);
                menu.AddItems(new BarItem[] { subLuuChuyen });
                #endregion

                #region -----HoSoDieuTri
                BarButtonItem itemCheckInfoBHYT = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__CheckInfoBHYT", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 7);
                itemCheckInfoBHYT.Tag = ModuleType.CheckInfoBHYT;
                itemCheckInfoBHYT.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                menu.AddItems(new BarItem[] { itemCheckInfoBHYT });

                BarButtonItem itemHSDT = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__HO_SO_DIEU_TRI", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 7);
                itemHSDT.Tag = ModuleType.TreatmentList;
                itemHSDT.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                menu.AddItems(new BarItem[] { itemHSDT });

                BarButtonItem itemSuaHSDT = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__SUA_HSDT", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 7);
                itemSuaHSDT.Tag = ModuleType.SuaHSDT;
                itemSuaHSDT.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                menu.AddItems(new BarItem[] { itemSuaHSDT });

                BarButtonItem itemBacSiDt = new BarButtonItem(barManager, "Bác sĩ điều trị", 7);
                itemBacSiDt.Tag = ModuleType.BacSiDt;
                itemBacSiDt.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                menu.AddItems(new BarItem[] { itemBacSiDt });

                BarButtonItem itemDongThoiGian = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__DONG_THOI_GIAN", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 7);
                itemDongThoiGian.Tag = ModuleType.TreatmentLog;
                itemDongThoiGian.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                menu.AddItems(new BarItem[] { itemDongThoiGian });
                #endregion

                #region -----Duyet Du dieu kien
                if (currentTreatmentBedRoom != null && currentTreatmentBedRoom.IS_APPROVE_FINISH == 1)
                {
                    BarButtonItem itemDisApprovalFinish = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__DIS_APPROVE_FINISH", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 7);
                    itemDisApprovalFinish.Tag = ModuleType.DisApprovalFinish;
                    itemDisApprovalFinish.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                    menu.AddItems(new BarItem[] { itemDisApprovalFinish });
                }
                else
                {
                    BarButtonItem itemApprovalFinish = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__APPROVE_FINISH", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 7);
                    itemApprovalFinish.Tag = ModuleType.ApprovalFinish;
                    itemApprovalFinish.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                    menu.AddItems(new BarItem[] { itemApprovalFinish });
                }
                #endregion

                #region ----- InAn
                BarSubItem subBieuMau = new BarSubItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__SUB_IN_AN", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 8);

                BarButtonItem itemOtherForms = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__BIEU_MAU_KHAC", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 8);
                itemOtherForms.Tag = ModuleType.OtherForms;
                itemOtherForms.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subBieuMau.AddItem(itemOtherForms);

                BarButtonItem itemOtherFormAssTreatment = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__BIEU_MAU_KHAC_HSDT", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 8);
                itemOtherFormAssTreatment.Tag = ModuleType.OtherFormAssTreatment;
                itemOtherFormAssTreatment.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subBieuMau.AddItem(itemOtherFormAssTreatment);

                BarButtonItem itemGiayNamVien = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__GIAY_NAM_VIEN", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 8);
                itemGiayNamVien.Tag = ModuleType.GiayNamVien;
                itemGiayNamVien.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subBieuMau.AddItem(itemGiayNamVien);

                BarButtonItem item_GiayTHXN = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__GIAY_THXN", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 8);
                item_GiayTHXN.Tag = ModuleType._GiayTHXN;
                item_GiayTHXN.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subBieuMau.AddItem(item_GiayTHXN);

                BarButtonItem item_GiayCamKetNamGiuongTuNguyen = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__GIAY_CAM_KET_NAM_GIUONG_TU_NGUYEN", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 8);
                item_GiayCamKetNamGiuongTuNguyen.Tag = ModuleType.CamKetNamGiuongTuNguyen;
                item_GiayCamKetNamGiuongTuNguyen.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subBieuMau.AddItem(item_GiayCamKetNamGiuongTuNguyen);

                BarButtonItem item_BanGiaoBNTruocPTTT = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__BAN_GIAO_BN_TRUOC_PTTT", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 8);
                item_BanGiaoBNTruocPTTT.Tag = ModuleType.BanGiaoBNTruocPTTT;
                item_BanGiaoBNTruocPTTT.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subBieuMau.AddItem(item_BanGiaoBNTruocPTTT);

                BarButtonItem item_KhaiThacTienSuDiUng = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__PHIEU_KHAI_THAC_TIEN_SU_DI_UNG", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 8);
                item_KhaiThacTienSuDiUng.Tag = ModuleType.PhieuKhaiThacTienSuDiUng;
                item_KhaiThacTienSuDiUng.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subBieuMau.AddItem(item_KhaiThacTienSuDiUng);


                BarButtonItem item_PhieuXNDuongMauMaoMach = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__PHIEU_XN_DUONG_MAU_MAO_MACH", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 8);
                item_PhieuXNDuongMauMaoMach.Tag = ModuleType.PhieuXNDuongMauMaoMach;
                item_PhieuXNDuongMauMaoMach.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subBieuMau.AddItem(item_PhieuXNDuongMauMaoMach);

                menu.AddItems(new BarItem[] { subBieuMau });
                #endregion

                #region ----- BenhAn
                BarSubItem subBenhAn = new BarSubItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__SUB_BENH_AN", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 9);

                BarButtonItem itemDebateDiagnostic = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__BIEN_BAN_HOI_CHAN", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 9);
                itemDebateDiagnostic.Tag = ModuleType.DebateDiagnostic;
                itemDebateDiagnostic.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subBenhAn.AddItem(itemDebateDiagnostic);

                BarButtonItem itemCareSlipList = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__CHAM_SOC", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 9);
                itemCareSlipList.Tag = ModuleType.CareSlipList;
                itemCareSlipList.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subBenhAn.AddItem(itemCareSlipList);

                BarButtonItem itemMediReactSum = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__PHAN_UNG_THUOC", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 9);
                itemMediReactSum.Tag = ModuleType.MediReactSum;
                itemMediReactSum.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subBenhAn.AddItem(itemMediReactSum);

                BarButtonItem itemAccidentHurt = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__THONG_TIN_TAI_NAN_THUONG_TICH", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 9);
                itemAccidentHurt.Tag = ModuleType.AccidentHurt;
                itemAccidentHurt.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subBenhAn.AddItem(itemAccidentHurt);

                BarButtonItem itemInfusionSumByTreatment = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__TRUYEN_DICH", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 9);
                itemInfusionSumByTreatment.Tag = ModuleType.InfusionSumByTreatment;
                itemInfusionSumByTreatment.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subBenhAn.AddItem(itemInfusionSumByTreatment);

                BarButtonItem itemBloodTransfusion = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__BloodTransfusion", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 9);
                itemBloodTransfusion.Tag = ModuleType.BloodTransfusion;
                itemBloodTransfusion.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subBenhAn.AddItem(itemBloodTransfusion);

                BarButtonItem itemHisDhst = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__HisDhst", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 9);
                itemHisDhst.Tag = ModuleType.HisDhst;
                itemHisDhst.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subBenhAn.AddItem(itemHisDhst);

                BarButtonItem itemHisDhstChart = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__HisDhstChart", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 9);
                itemHisDhstChart.Tag = ModuleType.HisDhstChart;
                itemHisDhstChart.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subBenhAn.AddItem(itemHisDhstChart);

                BarButtonItem itemHisPhieuSoketTruocMo = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__HisPhieuSoketTruocMo", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 9);
                itemHisPhieuSoketTruocMo.Tag = ModuleType.HisPhieuSoKetTruocMo;
                itemHisPhieuSoketTruocMo.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subBenhAn.AddItem(itemHisPhieuSoketTruocMo);

                BarButtonItem itemHisSoKetBenhAnTruocPhauThuat = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__HisSoKetBenhAnTruocPhauThuat", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 9);
                itemHisSoKetBenhAnTruocPhauThuat.Tag = ModuleType.HisSoKetBenhAnTruocPhauThuat;
                itemHisSoKetBenhAnTruocPhauThuat.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subBenhAn.AddItem(itemHisSoKetBenhAnTruocPhauThuat);

                BarButtonItem itemHisSoKetBenhAnTruocThuThuat = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__HisSoKetBenhAnTruocThuThuat", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 9);
                itemHisSoKetBenhAnTruocThuThuat.Tag = ModuleType.HisSoKetBenhAnTruocThuThuat;
                itemHisSoKetBenhAnTruocThuThuat.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                subBenhAn.AddItem(itemHisSoKetBenhAnTruocThuThuat);

                menu.AddItems(new BarItem[] { subBenhAn });

                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HAS_CONNECTION_EMR") == "1")
                {
                    BarButtonItem bbtnCheckEMR = new BarButtonItem(barManager, Resources.ResourceMessage.TraSoatHoSoBenhAn, 7);
                    bbtnCheckEMR.Tag = ModuleType.CheckingTreatmentEmr;
                    bbtnCheckEMR.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                    menu.AddItems(new BarItem[] { bbtnCheckEMR });
                }
                #endregion

                BarButtonItem itemHisTreatmentFile = new BarButtonItem(barManager, Resources.ResourceMessage.HoSoGiayToDinhKem, 10);
                itemHisTreatmentFile.Tag = ModuleType.HisTreatmentFile;
                itemHisTreatmentFile.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                menu.AddItems(new BarItem[] { itemHisTreatmentFile });

                BarButtonItem itemPhanLoaiBenhNhan = new BarButtonItem(barManager, Resources.ResourceMessage.PhanLoaiBN, 11);
                itemPhanLoaiBenhNhan.Tag = ModuleType.PhanLoaiBenhNhan;
                itemPhanLoaiBenhNhan.ItemClick += new ItemClickEventHandler(bedRoomMouseRightClick);
                menu.AddItems(new BarItem[] { itemPhanLoaiBenhNhan });

                menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
