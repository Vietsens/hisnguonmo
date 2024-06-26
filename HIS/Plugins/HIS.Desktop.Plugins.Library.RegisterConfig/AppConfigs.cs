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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.RegisterConfig
{
    public class AppConfigs
    {
        private const string CONFIG_KEY__CONFIG_KEY__HIS_DESKTOP__CHANGE_ETHNIC = "CONFIG_KEY__HIS_DESKTOP__CHANGE_ETHNIC";
        private const string CONFIG_KEY__TIEP_DON_HIEN_THI_THONG_TIN_THEM = "CONFIG_KEY__TIEP_DON_HIEN_THI_THONG_TIN_THEM";
        private const string CONFIG_KEY__HIEN_THI_NOI_LAM_VIEC_THEO_DINH_DANG_MAN_HINH_DANG_KY = "CONFIG_KEY__HIEN_THI_NOI_LAM_VIEC_THEO_DINH_DANG_MAN_HINH_DANG_KY";
        private const string CONFIG_KEY__CHE_DO_IN_PHIEU_DANG_KY_DICH_VU_KHAM_BENH = "CONFIG_KEY__CHE_DO_IN_PHIEU_DANG_KY_DICH_VU_KHAM_BENH";
        private const string CONFIG_KEY__DEFAULT_CONFIG_PATIENT_TYPE_CODE = "CONFIG_KEY__DEFAULT_CONFIG_PATIENT_TYPE_CODE";
        private const string CONFIG_KEY__FILL_DU_LIEU_TU_DONG_VAO_O_DIA_CHI_BENH_NHAN_CHIP_THE_MAN_HINH_DANG_KY = "CONFIG_KEY__FILL_DU_LIEU_TU_DONG_VAO_O_DIA_CHI_BENH_NHAN_CHIP_THE_MAN_HINH_DANG_KY";
        private const string CONFIG_KEY__ALERT_EXPRIED_TIME_HEIN_CARD_BHYT = "CONFIG_KEY__ALERT_EXPRIED_TIME_HEIN_CARD_BHYT";
        private const string CONFIG_KEY__DANG_KY_TIEP_DON__THU_TIEN_SAU = "CONFIG_KEY__DEFAULT_CONFIG_IS_NOT_REQUIRE_FEE";
        private const string CONFIG_KEY__DANG_KY_TIEP_DON__THOI_GIAN_LOAD_DANH_SACH_PHONG_KHAM = "CONFIG_KEY__DANG_KY_TIEP_DON__THOI_GIAN_LOAD_DANH_SACH_PHONG_KHAM";
        private const string CONFIG_KEY__DANG_KY_TIEP_DON__GOI_BENH_NHAN_BANG_PHAN_MEM_CPA = "CONFIG_KEY__DANG_KY_TIEP_DON__GOI_BENH_NHAN_BANG_CPA";
        private const string CONFIG_KEY__DANG_KY_TIEP_DON__HIEN_THI_THONG_BAO_TIM_THAY_BN_THEO_THONG_TIN_NHAP = "CONFIG_KEY__DANG_KY_TIEP_DON__HIEN_THI_THONG_BAO_TIM_THAY_BN_THEO_THONG_TIN_NHAP";
        private const string CONFIG_KEY__HIS_DESKTOP__REGISTER__OWE_TYPE_DEFAULT = "CONFIG_KEY__HIS_DESKTOP__REGISTER__OWE_TYPE_DEFAULT";
        private const string CONFIG_KEY__HIS_DESKTOP__PLUGINS_AUTO_CHECK_HEIN_DATE_TO = "CONFIG_KEY__HIS_DESKTOP__PLUGINS_AUTO_CHECK_HEIN_DATE_TO";
        private const string CONFIG_KEY__IS_AUTO_FILL_DATA_RECENT_SERVICE_ROOM = "HIS.IS_AUTO_FILL_DATA_RECENT_SERVICE_ROOM";
        private const string CONFIG_KEY__IS_DANG_KY_QUA_TONG_DAI = "HIS.IS_DANG_KY_QUA_TONG_DAI";
        private const string CONFIG_KEY__INSURANCE_EXPERTISE__CHECK_HEIN_CONFIG = "CONFIG_KEY__INSURANCEEXPERTISE_CHECKHEINCONFIG";
        private const string CONFIG_KEY__IS_USE_HID_SYNC = "CONFIG_KEY__IS_USE_HID_SYNC";
        private const string CONFIG_KEY__HIS_DESKTOP__REGISTER__SHOW_DEPOSIT_SERVICE = "CONFIG_KEY__HIS_DESKTOP__REGISTER__SHOW_DEPOSIT_SERVICE";

        private const string CONFIG_KEY__HIS_DESKTOP__REGISTER__SHOW_LINE_FIRST_ADDRESS = "CONFIG_KEY__HIS_DESKTOP__REGISTER__SHOW_LINE_FIRST_ADDRESS";

        private const string CONFIG_KEY__HIS_DESKTOP__REGISTER__TIME__AUTO___CALL_REGISTER_REQ = "CONFIG_KEY__HIS_DESKTOP__REGISTER__TIME__AUTO___CALL_REGISTER_REQ";

        public static MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE PatientTypeDefault { get; set; }
        public static long PatientTypeID_Default { get; set; }
        public static string PatientTypeCodeDefault { get; set; }
        public static long AlertExpriedTimeHeinCardBhyt { get; set; }
        public static long CheDoHienThiNoiLamViecManHinhDangKyTiepDon { get; set; }
        public static long TiepDon_HienThiMotSoThongTinThemBenhNhan { get; set; }
        public static string DangKyTiepDonThuTienSau { get; set; }
        public static long DangKyTiepDonThoiGianLoadDanhSachPhongKham { get; set; }
        public static long DangKyTiepDonHienThiThongBaoTimDuocBenhNhan { get; set; }
        public static string DangKyTiepDonGoiBenhNhanBangCPA { get; set; }
        public static long CheDoTuDongFillDuLieuDiaChiGhiTrenTheVaoODiaChiBenhNhanHayKhong { get; set; }

        public static int ThoiGianTuDongGoiLaySTTMoiNhat { get; set; }

        public static string InsuranceExpertiseCheckHeinConfig { get; set; }

        /// <summary>
        /// Đặt là 1 nếu chọn chế độ hiển thị để xem xong rồi in, đặt là 2 nếu chọn chế độ in ngay không cần xem
        /// </summary>
        public static long CheDoInPhieuDangKyDichVuKhamBenh { get; set; }
        public static string OweTypeDefault { get; set; }
        public static long CheDoTuDongCheckThongTinTheBHYT { get; set; }

        /// <summary>
        /// Cấu hình tự động fill yêu cầu - phòng khám gần nhất khi tìm bệnh nhân cũ
        /// Đặt 1 là tự động fill
        /// Mặc định là không tự động
        /// </summary>
        public static string IsAutoFillDataRecentServiceRoom { get; set; }

        public static string IsDangKyQuaTongDai { get; set; }

        public static bool IsVisibleSomeControl { get; set; }
        public static List<string> PatientCodeIsNotRequireExamFee;
        public static List<long> PatientIdIsNotRequireExamFee;

        public static long IsShowDepositService { get; set; }
        public static long IsShowLineFirstAddress { get; set; }
        /// <summary>
        /// Thay đổi trường dân tộc.
        /// - Mặc định để 0 như hiện tại
        /// - khác 0: Đưa trường dân tộc lên sau trường nghề nghiệp
        /// </summary>
        public static long ChangeEthnic { get; set; }

        public static void LoadConfig()
        {
            try
            {
                ChangeEthnic = ConfigApplicationWorker.Get<long>(CONFIG_KEY__CONFIG_KEY__HIS_DESKTOP__CHANGE_ETHNIC);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            try
            {
                ThoiGianTuDongGoiLaySTTMoiNhat = ConfigApplicationWorker.Get<int>(CONFIG_KEY__HIS_DESKTOP__REGISTER__TIME__AUTO___CALL_REGISTER_REQ);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            try
            {
                IsShowLineFirstAddress = ConfigApplicationWorker.Get<long>(CONFIG_KEY__HIS_DESKTOP__REGISTER__SHOW_LINE_FIRST_ADDRESS);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            try
            {
                IsShowDepositService = ConfigApplicationWorker.Get<long>(CONFIG_KEY__HIS_DESKTOP__REGISTER__SHOW_DEPOSIT_SERVICE);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            try
            {
                IsVisibleSomeControl = (ConfigApplicationWorker.Get<string>(CONFIG_KEY__TIEP_DON_HIEN_THI_THONG_TIN_THEM) == GlobalVariables.CommonStringTrue);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            try
            {
                IsDangKyQuaTongDai = ConfigApplicationWorker.Get<string>(CONFIG_KEY__IS_DANG_KY_QUA_TONG_DAI);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            try
            {
                InsuranceExpertiseCheckHeinConfig = ConfigApplicationWorker.Get<string>(CONFIG_KEY__INSURANCE_EXPERTISE__CHECK_HEIN_CONFIG);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            try
            {
                IsAutoFillDataRecentServiceRoom = ConfigApplicationWorker.Get<string>(CONFIG_KEY__IS_AUTO_FILL_DATA_RECENT_SERVICE_ROOM);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            try
            {
                PatientTypeCodeDefault = ConfigApplicationWorker.Get<string>(CONFIG_KEY__DEFAULT_CONFIG_PATIENT_TYPE_CODE);

                PatientTypeDefault = GetDeaultPatientType();
                PatientTypeID_Default = PatientTypeDefault.ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            try
            {
                OweTypeDefault = ConfigApplicationWorker.Get<string>(CONFIG_KEY__HIS_DESKTOP__REGISTER__OWE_TYPE_DEFAULT);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            try
            {
                AlertExpriedTimeHeinCardBhyt = ConfigApplicationWorker.Get<long>(CONFIG_KEY__ALERT_EXPRIED_TIME_HEIN_CARD_BHYT);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            try
            {
                CheDoHienThiNoiLamViecManHinhDangKyTiepDon = ConfigApplicationWorker.Get<long>(CONFIG_KEY__HIEN_THI_NOI_LAM_VIEC_THEO_DINH_DANG_MAN_HINH_DANG_KY);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            try
            {
                TiepDon_HienThiMotSoThongTinThemBenhNhan = ConfigApplicationWorker.Get<long>(CONFIG_KEY__TIEP_DON_HIEN_THI_THONG_TIN_THEM);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            try
            {
                PatientCodeIsNotRequireExamFee = new List<string>();
                PatientIdIsNotRequireExamFee = new List<long>();
                string patientCodeIsNotRequireExamFeeCFG = ConfigApplicationWorker.Get<string>(CONFIG_KEY__DANG_KY_TIEP_DON__THU_TIEN_SAU);
                if (!String.IsNullOrEmpty(patientCodeIsNotRequireExamFeeCFG))
                {
                    string[] patientCodeIsNotRequireExamFeeArr = patientCodeIsNotRequireExamFeeCFG.Split('|');
                    if (patientCodeIsNotRequireExamFeeArr != null && patientCodeIsNotRequireExamFeeArr.Length > 0)
                    {
                        foreach (var item in patientCodeIsNotRequireExamFeeArr)
                        {
                            HIS_PATIENT_TYPE hisPatientType = GetPatientTypeByCode(item);
                            if (hisPatientType != null)
                            {
                                PatientIdIsNotRequireExamFee.Add(hisPatientType.ID);
                                PatientCodeIsNotRequireExamFee.Add(hisPatientType.PATIENT_TYPE_CODE);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            try
            {
                DangKyTiepDonThoiGianLoadDanhSachPhongKham = ConfigApplicationWorker.Get<long>(CONFIG_KEY__DANG_KY_TIEP_DON__THOI_GIAN_LOAD_DANH_SACH_PHONG_KHAM);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            try
            {
                DangKyTiepDonHienThiThongBaoTimDuocBenhNhan = ConfigApplicationWorker.Get<long>(CONFIG_KEY__DANG_KY_TIEP_DON__HIEN_THI_THONG_BAO_TIM_THAY_BN_THEO_THONG_TIN_NHAP);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            try
            {
                DangKyTiepDonGoiBenhNhanBangCPA = ConfigApplicationWorker.Get<string>(CONFIG_KEY__DANG_KY_TIEP_DON__GOI_BENH_NHAN_BANG_PHAN_MEM_CPA);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            try
            {
                CheDoTuDongFillDuLieuDiaChiGhiTrenTheVaoODiaChiBenhNhanHayKhong = ConfigApplicationWorker.Get<long>(CONFIG_KEY__FILL_DU_LIEU_TU_DONG_VAO_O_DIA_CHI_BENH_NHAN_CHIP_THE_MAN_HINH_DANG_KY);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            try
            {
                CheDoInPhieuDangKyDichVuKhamBenh = ConfigApplicationWorker.Get<long>(CONFIG_KEY__CHE_DO_IN_PHIEU_DANG_KY_DICH_VU_KHAM_BENH);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            try
            {
                CheDoTuDongCheckThongTinTheBHYT = ConfigApplicationWorker.Get<long>(CONFIG_KEY__HIS_DESKTOP__PLUGINS_AUTO_CHECK_HEIN_DATE_TO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        static MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE GetDeaultPatientType()
        {
            MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType = null;
            try
            {
                if (String.IsNullOrEmpty(HisConfigCFG.PatientTypeCode__BHYT))
                {
                    HisConfigCFG.PatientTypeCode__BHYT = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
                }

                if (!String.IsNullOrEmpty(AppConfigs.PatientTypeCodeDefault))
                {
                    patientType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == AppConfigs.PatientTypeCodeDefault);
                    if (patientType == null)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Phan mem HIS da duoc cau hinh doi tuong benh nhan mac dinh, tuy nhien ma doi tuong cau hinh khong ton tai trong danh muc doi tuong benh nhan, he thong tu dong lay doi tuong mac dinh la doi tuong BHYT. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => AppConfigs.PatientTypeCodeDefault), AppConfigs.PatientTypeCodeDefault));

                        patientType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == HisConfigCFG.PatientTypeCode__BHYT);
                    }
                }

                if (patientType == null)
                    patientType = new HIS_PATIENT_TYPE();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return patientType;
        }

        static MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE GetPatientTypeByCode(string code)
        {
            MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE result = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
            try
            {
                result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE.ToLower() == code.Trim().ToLower());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result ?? new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
        }
    }
}
