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
using System;
using System.Collections.Generic;
using HIS.Desktop.Plugins.Library.EmrToolkitImport;
using HIS.Desktop.Plugins.Library.EmrToolkitImport.Models;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;

namespace HIS.Desktop.Plugins.TreatmentList
{
    /// <summary>
    /// Tính năng TEST: gửi dữ liệu "Giấy chuyển viện" (JSON mẫu) qua EMRTOOLKIT
    /// và hiển thị kết quả. Toàn bộ logic gọi API nằm trong thư viện riêng
    /// HIS.Desktop.Plugins.Library.EmrToolkitImport để chỗ khác cùng tái sử dụng.
    /// </summary>
    public partial class UCTreatmentList : UserControlBase
    {
        /// <summary>
        /// Handler menu chuột phải "Gửi Giấy chuyển viện qua EMRTOOLKIT (Test)".
        /// </summary>
        private void GuiGiayChuyenVienEmrToolkitClick()
        {
            try
            {
                if (this.currentTreatment == null)
                    return;

                EmrImportModel model = BuildSampleGiayChuyenVienModel(this.currentTreatment);

                EmrToolkitImportProcessor processor = new EmrToolkitImportProcessor();

                // Bọc WaitingManager quanh phần gọi mạng (đồng bộ)
                WaitingManager.Show();
                EmrToolkitImportResult result = processor.ImportEmr(model);
                WaitingManager.Hide();

                // Log trace (KHÔNG log dữ liệu nhạy cảm BN — chỉ trạng thái + mã hồ sơ)
                Inventec.Common.Logging.LogSystem.Info(string.Format(
                    "EMRTOOLKIT GuiGiayChuyenVien. TreatmentCode={0}, MaQuanLy={1}, Success={2}, Step={3}, Message={4}",
                    model.MA_LK, model.MaQuanLy,
                    result != null ? result.Success : false,
                    result != null ? result.Step.ToString() : "-",
                    result != null ? result.Message : "-"));

                // Hiển thị cửa sổ kết quả (JSON gửi/nhận)
                processor.ShowResult(result, this.ParentForm);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Dựng JSON mẫu Giấy Chuyển Viện theo file PDF "Giấy Chuyển Tuyến".
        /// Lấy 1 vài trường từ hồ sơ đang chọn, phần còn lại là dữ liệu test cố định.
        /// IDMauPhieu + Mã CSKCB để trống → thư viện tự lấy theo cấu hình.
        /// </summary>
        private EmrImportModel BuildSampleGiayChuyenVienModel(V_HIS_TREATMENT_4 treatment)
        {
            EmrImportModel model = new EmrImportModel();
            try
            {
                // ----- Hành chính bệnh nhân (lấy từ hồ sơ đang chọn) -----
                model.LoaiKy = 0;
                model.MaBenhNhan = treatment.TDL_PATIENT_CODE;   // mã bệnh nhân (PATIENT_CODE)
                model.MaYTe = treatment.TDL_PATIENT_CODE;
                model.HoVaTenBenhNhan = treatment.TDL_PATIENT_NAME;
                model.HoTenBN = treatment.TDL_PATIENT_NAME;
                model.NgaySinh = ToDateTime(treatment.TDL_PATIENT_DOB);
                model.Tuoi = CalcTuoi(treatment.TDL_PATIENT_DOB);
                // EMRTOOLKIT quy ước 1 = Nam, 2 = Nữ; so theo hằng số HIS_GENDER trong DbConfig
                model.GioiTinh = treatment.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE ? 2 : 1;
                model.CCCD = treatment.TDL_PATIENT_CCCD_NUMBER;
                model.DanToc = treatment.TDL_PATIENT_ETHNIC_NAME;
                model.NgoaiKieu = treatment.TDL_PATIENT_NATIONAL_NAME;
                model.NgheNghiep = treatment.TDL_PATIENT_CAREER_NAME;
                model.NoiLamViec = FirstNotEmpty(treatment.TDL_PATIENT_WORK_PLACE_NAME, treatment.WORK_PLACE_NAME, treatment.TDL_PATIENT_WORK_PLACE);
                model.TenPhuongXa = treatment.TDL_PATIENT_COMMUNE_NAME;
                model.MaPhuongXa = treatment.TDL_PATIENT_COMMUNE_CODE;
                model.TenQuanHuyen = treatment.TDL_PATIENT_DISTRICT_NAME;
                model.MaQuanHuyen = treatment.TDL_PATIENT_DISTRICT_CODE;
                model.TenTinhThanh = treatment.TDL_PATIENT_PROVINCE_NAME;
                model.MaTinhThanh = treatment.TDL_PATIENT_PROVINCE_CODE;
                model.DiaChi = treatment.TDL_PATIENT_ADDRESS;
                // SoNha, ThonPho: V_HIS_TREATMENT_4 chỉ có địa chỉ đầy đủ → để trống
                model.SoNha = null;
                model.ThonPho = null;

                // ----- BHYT -----
                model.DoiTuong = 0;
                model.SoTheBHYT = treatment.TDL_HEIN_CARD_NUMBER;
                model.SoThe = treatment.TDL_HEIN_CARD_NUMBER;
                model.BatDauBHYT = ToDateTime(treatment.TDL_HEIN_CARD_FROM_TIME);
                model.KetThucBHYT = ToDateTime(treatment.TDL_HEIN_CARD_TO_TIME);
                model.NgayHetHanBHYT = ToDateTime(treatment.TDL_HEIN_CARD_TO_TIME);

                // ----- Người nhà / liên lạc -----
                model.HoTenDiaChiNguoiNha = JoinNameAddress(treatment.TDL_PATIENT_RELATIVE_NAME, treatment.TDL_PATIENT_RELATIVE_ADDRESS);
                model.SoDienThoaiNguoiNha = FirstNotEmpty(treatment.TDL_PATIENT_RELATIVE_MOBILE, treatment.TDL_PATIENT_RELATIVE_PHONE);
                model.SoDienThoaiLienLac = FirstNotEmpty(treatment.TDL_PATIENT_MOBILE, treatment.TDL_PATIENT_PHONE);

                // ----- Điều trị / khoa phòng -----
                model.TenKhoaLamBenhAn = treatment.HOPITALIZE_DEPARTMENT_NAME;
                // MaKhoaLamBenhAn, Buong, Giuong: không có trong V_HIS_TREATMENT_4 → để trống
                model.MaKhoaLamBenhAn = null;
                model.Buong = null;
                model.Giuong = null;
                model.NgayVaoVien = ToDateTime(treatment.IN_TIME);
                model.NgayRaVien = ToDateTime(treatment.OUT_TIME);
                model.SoNhapVien = treatment.TREATMENT_CODE;
                model.Ma_LK = treatment.TREATMENT_CODE;
                model.MA_LK = treatment.TREATMENT_CODE;
                model.NgayBDDieuTri = ToDateTime(treatment.IN_TIME);
                model.NgayKTDieuTri = ToDateTime(treatment.OUT_TIME);
                model.KinhGui = treatment.MEDI_ORG_NAME;              // nơi chuyển đến
                model.MaCoSoKhamChuaBenh = treatment.MEDI_ORG_CODE;   // mã CSKCB nơi chuyển đến
                model.SoLuuTru = treatment.STORE_CODE;
                model.DanhSachChuoiKy = new List<object>();
                model.ID = 0;
                // DaDieuTriTai (cơ sở đang điều trị của chính viện) → không có trên
                // treatment, để thư viện lấy theo HisConfig
                model.DaDieuTriTai = null;

                // ----- Tóm tắt bệnh án -----
                model.DauHieuLamSan = treatment.CLINICAL_SIGNS;
                model.ChanDoan = treatment.ICD_NAME;
                model.ChanDoanND = FirstNotEmpty(treatment.ICD_TEXT, treatment.ICD_NAME);
                model.ThuocKhac = treatment.USED_MEDICINE;
                // ChanDoanNgay: không có cột riêng trên treatment → để trống
                model.ChanDoanNgay = null;

                // ----- Tình trạng lúc chuyển viện -----
                model.PTVanChuyen = treatment.TRANSPORT_VEHICLE;
                model.HoTenNDD = treatment.TRANSPORTER;
                // Sinh hiệu lúc chuyển (Mạch/HA/Nhịp thở/SPO2), Số giấy CV, giờ/ngày chuyển,
                // ngày đưa đi, lý do CV: không lưu trên V_HIS_TREATMENT_4 → để trống
                model.MachTCV = null;
                model.HuyetApcmHgTCV = null;
                model.NhipThoTCV = null;
                model.SPO2TCV = null;
                model.LiDoCV = null;
                model.NgayDuaDi = null;
                model.CVNgay = null;
                model.CVGio = null;
                model.CVPhut = null;
                model.SoGiayCV = null;

                // ----- Người ký / mẫu phiếu -----
                model.HTBacSyDieuTri = treatment.DOCTOR_USERNAME;
                model.HTGiamDocBV = treatment.HOSPITAL_DIRECTOR_USERNAME;
                model.TenMauPhieu = "Giấy Chuyển Viện";
                model.MaQuanLy = treatment.ID;   // mã hồ sơ điều trị
                // IDMauPhieu để trống → thư viện tự lấy theo HisConfig
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return model;
        }

        /// <summary>Chuyển ngày kiểu long (yyyyMMddHHmmss) sang DateTime?.</summary>
        private DateTime? ToDateTime(long? timeNumber)
        {
            try
            {
                if (timeNumber == null || timeNumber.Value <= 0)
                    return null;
                return Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(timeNumber.Value);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>Tính tuổi (số năm) từ ngày sinh kiểu long (yyyyMMddHHmmss).</summary>
        private string CalcTuoi(long? dob)
        {
            try
            {
                DateTime? birth = ToDateTime(dob);
                if (birth == null)
                    return null;
                DateTime now = DateTime.Now;
                int age = now.Year - birth.Value.Year;
                if (birth.Value.Date > now.AddYears(-age))
                    age--;
                return age >= 0 ? age.ToString() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>Trả về giá trị đầu tiên không rỗng trong danh sách.</summary>
        private string FirstNotEmpty(params string[] values)
        {
            if (values == null)
                return null;
            foreach (string value in values)
            {
                if (!string.IsNullOrWhiteSpace(value))
                    return value;
            }
            return null;
        }

        /// <summary>Ghép "Họ tên - Địa chỉ", bỏ qua phần rỗng.</summary>
        private string JoinNameAddress(string name, string address)
        {
            bool hasName = !string.IsNullOrWhiteSpace(name);
            bool hasAddress = !string.IsNullOrWhiteSpace(address);
            if (hasName && hasAddress)
                return name + " - " + address;
            if (hasName)
                return name;
            if (hasAddress)
                return address;
            return null;
        }
    }
}
