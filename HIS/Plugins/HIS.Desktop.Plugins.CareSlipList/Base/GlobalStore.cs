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
using HIS.Desktop.Plugins.CareSlipList.ADO;
using Inventec.Common.XmlConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.CareSlipList.Base
{
    public class GlobalStore
    {
        private static object syncRoot = new Object();

        public static XmlApplicationConfig ApplicationConfig { get; set; }
        public const int ActionAdd = 1;//1 -> Add
        public const int ActionEdit = 2;//2 -> Edit
        public const int ActionView = 3;//3 -> View
        public const int ActionViewForEdit = 4;//4 -> View for edit
        public static short HIS_SERE_SERV_IS_EXPEND = 1;
        public static int SERE_SERV_TYPE = 3;
        public static long SERVICE_ROOM_TYPE = 4;
        public static string AccountBookCodeForUser { get; set; }
        public static string PayFormCodeDefault { get; set; }
        public static long THUOC = 1;
        public static long VATTU = 2;
        public static long THUOC_DM = 3;
        public static long VATTU_DM = 4;
        public static long THUOC_TUTUC = 5;
        public const long HEIN_SERVICE_TYPE_ID_THUOC_VATTU_NGOAI_KHO = 20;
        public const long HEIN_SERVICE_TYPE_ORDER_THUOC_VATTU_NGOAI_KHO = 88;

        //Global
        public enum ModuleType
        {
            CREATE_EKIP
        }

        public static string TemnplatePathFolder = "PrintTemplate";
        public static long ROOM_ID_FOR_WAITING_SCREEN { get; set; }
        public static bool isLogouter = false;
        public static bool IsLostToken = false;
        public static string UserDelegateLoginName { get; set; }
        public static string CurrentTabPage { get; set; }
        public static int SelectedTabPageIndex { get; set; }
        public static int SelectedPaperKindIndex { get; set; }
        public static string ChooseRoomMultiChoseByCode = "";
        //public static List<EXE.UTILITY.Module> currentModules { get; set; }
        public static List<MOS.EFMODEL.DataModels.HIS_GENDER> ListGenders = new
        List<MOS.EFMODEL.DataModels.HIS_GENDER>();
        static long currentRoomType { get; set; }

        public static long CurrentRoomType
        {
            get
            {
                lock (syncRoot)
                    return currentRoomType;
            }
            set
            {
                lock (syncRoot)
                    currentRoomType = value;
            }
        }
        public static long NumPageSize { get; set; }
        public static long ExamServiceRequestRequestByRoomAutoLoadTimer { get; set; }//Thời gian tải lại danh sách yêu cầu xử lý dịch vụ                     
        public static string IsUseRoomCounter { get; set; }
        public static long CheckConnectionHostTimer { get; set; }
        public static string DirectoryAdvertisement { get; set; }
        public static long PrintFullTemplate { get; set; }
        public static long PrintSieuAmRemoveExecuteUse { get; set; }
        public static long Is_Print_Execute_UserName { get; set; }
        public static long ChonMauGiayChuyenTuyen { get; set; }
        public static long SoBenhNhanTrenDanhSachChoKhamVaCLS { get; set; }
        public static long ChonManHinhGoiBenhNhan { get; set; }
        public static string SelectedPaperKind { get; set; }
        public static long PrintFullAdviseDoctor { get; set; }
        public static decimal CanhBaoYeuCauBNTamUngKhiChiDinh { get; set; }
        public static long BordereauTemplateType { get; set; }
        public static long PrintGiayKhamSucKhoeCanBo { get; set; }
        public static long HAS_PRIORITY = 1;
        public static int SurgeryMember { get; set; }
        public static int SoDangKyKhamSua { get; set; }
        public static string PatientTypeCodeDefault { get; set; }
        public static string IsNotRequireFee { get; set; }
        public static object dataExxcute { get; set; }
        public static long AlertExpriedTimeHeinCardBhyt { get; set; }
        public static long TemplateMauPhieuThanhToanRaVien { get; set; }
        public static long CheDoTuDongFillDuLieuDiaChiGhiTrenTheVaoODiaChiBenhNhanHayKhong { get; set; }
        public static long CheDoHienThiNoiLamViecManHinhDangKyTiepDon { get; set; }
        public static long ExamServiceRequestNewestServiceReqNumLimit { get; set; }
        public static long CheDoHienThiCacYeuCauDichVu { get; set; }
        public static long CheDoInPhieuThuocGayNghienHuongTamThan { get; set; }
        //public static long CheDoHienThiCacManHinhChiDinhDichVuGopCu { get; set; }
        public static TreatmentCommonInfoADOCombo.ModuleType? ExaminationModuleType { get; set; }
        public static TreatmentCommonInfoADOCombo HisTreatmentProcessing { get; set; }
        public static TreatmentCommonLogADO HisTreatmentLog { get; set; }
        public static List<MOS.EFMODEL.DataModels.HIS_EMERGENCY_WTIME> ListEmergency = new List<MOS.EFMODEL.DataModels.HIS_EMERGENCY_WTIME>();
        public static MOS.EFMODEL.DataModels.V_HIS_PATIENT currentPatientInRegisterForm { get; set; }
        public static List<MOS.EFMODEL.DataModels.V_HIS_ROOM_COUNTER> RoomCounters = new List<MOS.EFMODEL.DataModels.V_HIS_ROOM_COUNTER>();
        public static List<MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX> ListTestIndex = new List<MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX>();
        public static List<MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX_RANGE> ListTestIndexRange = new List<MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX_RANGE>();
        public static List<MOS.EFMODEL.DataModels.HIS_DEPARTMENT> ListDepartment = new List<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>();
        public static string DIALOG_FILTER_STRING = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
        public const string APPLICATION_CODE = "EXE";
        public static string URI_API_MOS = "";
        public static string URI_API_SDA = "";
        public static string URI_API_SAR = "";
        public static string URI_API_ACS = "";
        public static string URI_API_MRS = "";

        public static string REPORT_TYPE_CODE_BC_PHIEU_CONG_KHAI_THUOC = "MRS00093";
        public static string REPORT_TYPE_CODE_BC_PHIEU_LIET_KE_VAT_TU_TIEU_HAO = "MRS00104";

        public static long CheDoInPhieuDangKyDichVuKhamBenh { get; set; }
        public static long CheDoInChoCacChucNangTrongPhanMem { get; set; }
        public static long TuDongInKhiLuuManHinhTiepDon { get; set; }
        public static long TiepDon_HienThiMotSoThongTinThemBenhNhan { get; set; }
        public static long ChiDinhNhanhThuocVatTu { get; set; }


        public static int Gio = 4;
    }
}
