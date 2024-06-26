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
using Inventec.Common.Logging;
using Inventec.Common.XmlConfig;
using Inventec.Desktop.Common.LanguageManager;
using System;

namespace HIS.Desktop.LocalStorage.ConfigApplication
{
    public class Load
    {
        #region Variable
        static XmlApplicationConfig applicationConfig;
        #endregion

        #region Default value
        internal const int DEFAULT_NUM_PAGESIZE = 100;
        #endregion

        //public static void Init()
        //{
        //    try
        //    {
        //        string filePath = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath;
        //        string pathXmlFileConfig = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filePath), @"ConfigApplication.xml");
        //        string lang = (LanguageManager.GetLanguage() == LanguageManager.GetLanguageVi() ? "Vi" : "En");
        //        applicationConfig = new Inventec.Common.XmlConfig.XmlApplicationConfig(pathXmlFileConfig, lang);
        //        if (applicationConfig != null)
        //        {
        //            ConfigApplications.NumPageSize = GetLongConfigApplication(Keys.CONFIG_KEY__NUM_PAGESIZE);
        //            ConfigApplications.PatientTypeCodeDefault = GetStringConfigApplication(Keys.CONFIG_KEY__DEFAULT_CONFIG_PATIENT_TYPE_CODE);
        //            ConfigApplications.CheDoInChoCacChucNangTrongPhanMem = GetLongConfigApplication(Keys.CONFIG_KEY__CHE_DO_IN_CHO_CAC_CHUC_NANG_TRONG_PHAN_MEM);


        //            try
        //            {
        //                ConfigApplications.HIS_PAY_FORM_CODE__DEFAULT = GetStringConfigApplication(Keys.HFS_KEY__PAY_FORM_CODE);

        //            }
        //            catch (Exception ex)
        //            {
        //                Inventec.Common.Logging.LogSystem.Error(ex);
        //            }
        //            ConfigApplications.AlertExpriedTimeHeinCardBhyt = GetLongConfigApplication(Keys.CONFIG_KEY__ALERT_EXPRIED_TIME_HEIN_CARD_BHYT);
        //            ConfigApplications.CheDoHienThiCacManHinhChiDinhDichVuGopCu = GetLongConfigApplication(Keys.CONFIG_KEY__MODULE_CHI_DINH_DICH_VU__AN_HIEN_TAB);
        //            ConfigApplications.ChiDinhDichVuAnHienCotChiPhiNgoaiGoi = GetLongConfigApplication(Keys.CONFIG_KEY__MODULE_CHI_DINH_DICH_VU__AN_HIEN_CP_NGOAI_GOI);
        //            ConfigApplications.ChiDinhDichVuAnHienCotHaoPhi = GetLongConfigApplication(Keys.CONFIG_KEY__MODULE_CHI_DINH_DICH_VU__AN_HIEN_HAO_PHI);
        //            ConfigApplications.ChiDinhDichVuAnHienCotGiaGoi = GetLongConfigApplication(Keys.CONFIG_KEY__MODULE_CHI_DINH_DICH_VU__AN_HIEN_GIA_GOI);
        //            ConfigApplications.CheDoHienThiNoiLamViecManHinhDangKyTiepDon = GetLongConfigApplication(Keys.CONFIG_KEY__HIEN_THI_NOI_LAM_VIEC_THEO_DINH_DANG_MAN_HINH_DANG_KY);
        //            ConfigApplications.CheDoInPhieuDangKyDichVuKhamBenh = GetLongConfigApplication(Keys.CONFIG_KEY__CHE_DO_IN_PHIEU_DANG_KY_DICH_VU_KHAM_BENH);
        //            ConfigApplications.CheDoHienThiCacYeuCauDichVu = GetLongConfigApplication(Keys.CONFIG_KEY__CHE_DO_HIEN_THI_CAC_YEU_CAU_DICH_VU);
        //            ConfigApplications.CheDoInChoCacChucNangTrongPhanMem = GetLongConfigApplication(Keys.CONFIG_KEY__CHE_DO_IN_CHO_CAC_CHUC_NANG_TRONG_PHAN_MEM);
        //            ConfigApplications.TuDongInKhiLuuManHinhTiepDon = GetLongConfigApplication(Keys.CONFIG_KEY__IN_SAU_KHI_LUU_MAN_HINH_TIEP_DON);
        //            ConfigApplications.TiepDon_HienThiMotSoThongTinThemBenhNhan = GetLongConfigApplication(Keys.CONFIG_KEY__TIEP_DON_HIEN_THI_THONG_TIN_THEM);
        //            ConfigApplications.CheDoTuDongFillDuLieuDiaChiGhiTrenTheVaoODiaChiBenhNhanHayKhong = GetLongConfigApplication(Keys.CONFIG_KEY__FILL_DU_LIEU_TU_DONG_VAO_O_DIA_CHI_BENH_NHAN_CHIP_THE_MAN_HINH_DANG_KY);
        //            ConfigApplications.ChiDinhNhanhThuocVatTu = GetLongConfigApplication(Keys.CONFIG_KEY__CHI_DINH_NHANH_THUOC_VAT_TU);
        //            ConfigApplications.DuongDanChayFileVideo = GetStringConfigApplication(Keys.CONFIG_KEY__DUONG_DAN_CHAY_FILE_VIDEO);
        //            ConfigApplications.SoBenhNhanTrenDanhSachChoKhamVaCLS = GetLongConfigApplication(Keys.CONFIG_KEY__SO_BENH_NHAN_TREN_DANH_SACH_CHO_KHAM_VA_CLS);
        //            ConfigApplications.DirectoryAdvertisement = (string)GetStringConfigApplication(Keys.CONFIG_KEY__DIRECTORY_ADVERTISEMENT);
        //            ConfigApplications.CheDoKeDonThuocMotHoacNhieuKho = GetLongConfigApplication(Keys.CONFIG_KEY__CHE_DO_KE_DON_THUOC__MOT_HOAC_NHIEU_KHO);
        //            ConfigApplications.CheDoInPhieuThuocGayNghienHuongTamThan = GetLongConfigApplication(Keys.CONFIG_KEY__CHE_DO_PHIEU_LINH_THUOC_GAY_NGHIEN_HUONG_TAM_THAN);
        //            ConfigApplications.DangKyTiepDonThuTienSau = GetStringConfigApplication(Keys.CONFIG_KEY__DANG_KY_TIEP_DON__THU_TIEN_SAU);
        //            ConfigApplications.DangKyTiepDonThoiGianLoadDanhSachPhongKham = GetLongConfigApplication(Keys.CONFIG_KEY__DANG_KY_TIEP_DON__THOI_GIAN_LOAD_DANH_SACH_PHONG_KHAM);
        //            ConfigApplications.DangKyTiepDonGoiBenhNhanBangCPA = GetStringConfigApplication(Keys.CONFIG_KEY__DANG_KY_TIEP_DON__GOI_BENH_NHAN_BANG_PHAN_MEM_CPA);
        //            ConfigApplications.DangKyTiepDonHienThiThongBaoTimDuocBenhNhan = GetLongConfigApplication(Keys.CONFIG_KEY__DANG_KY_TIEP_DON__HIEN_THI_THONG_BAO_TIM_THAY_BN_THEO_THONG_TIN_NHAP);
        //            ConfigApplications.CheDoTuDongLogoutKhiTatPhanMem = GetLongConfigApplication(Keys.CONFIG_KEY__HIS_DESKTOP__AUTO_TOKEN_LOGOUT_WHILE_CLOSE_APPLICATION);
        //            ConfigApplications.ConfigureHideTabsForTreatmentInformation = GetLongConfigApplication(Keys.CONFIG_KEY__EXAM_SERVICE_REQ_EXCUTE_HIDE_TABS_INFOMATION__APPLICATION);
        //            ConfigApplications.IsVisibleRemedyCount = GetLongConfigApplication(Keys.CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__ISVISIBLE_REMEDY_COUNT);
        //            ConfigApplications.AssignPresscription__IsShowPrintPreview = GetLongConfigApplication(Keys.CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__IS_PRINT_NOW);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        #region Utils
        static long GetLongConfigApplication(string key)
        {
            try
            {
                return Inventec.Common.TypeConvert.Parse.ToInt64(GetStringConfigApplication(key));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => key), key), ex);
            }
            return 0;
        }

        static decimal GetDecimalConfigApplication(string key)
        {
            try
            {
                return Inventec.Common.TypeConvert.Parse.ToDecimal(GetStringConfigApplication(key));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => key), key), ex);
            }
            return 0;
        }

        static int GetIntConfigApplication(string key)
        {
            try
            {
                return Inventec.Common.TypeConvert.Parse.ToInt32(GetStringConfigApplication(key));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => key), key), ex);
            }
            return 0;
        }

        static string GetStringConfigApplication(string key)
        {
            try
            {
                if (applicationConfig == null)
                {
                    throw new ArgumentNullException("Khong khoi tao doi tuong applicationConfig.");
                }
                return (applicationConfig.GetKeyValue(key) ?? "").ToString();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => key), key), ex);
            }
            return "";
        }
        #endregion

    }
}
