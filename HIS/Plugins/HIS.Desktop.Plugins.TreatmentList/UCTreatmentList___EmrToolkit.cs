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
                // ----- Hành chính bệnh nhân (lấy từ hồ sơ đang chọn nếu có) -----
                model.LoaiKy = 0;
                model.MaBenhNhan = treatment.PERSON_CODE;
                model.MaYTe = treatment.TDL_PATIENT_CODE;
                model.HoVaTenBenhNhan = treatment.TDL_PATIENT_NAME;
                model.HoTenBN = treatment.TDL_PATIENT_NAME;
                model.NgaySinh = ToDateTime(treatment.TDL_PATIENT_DOB);
                model.Tuoi = "24";
                model.GioiTinh = (int)treatment.TDL_PATIENT_GENDER_ID;
                model.CCCD = treatment.TDL_PATIENT_CCCD_NUMBER;
                model.DanToc = "Kinh";
                model.NgoaiKieu = "Việt Nam";
                model.NgheNghiep = "Học sinh";
                model.NoiLamViec = "Trường THPT Thái Bình";
                model.SoNha = "Số 10";
                model.ThonPho = "Phố Lê Lợi";
                model.TenPhuongXa = "Phường Lê Hồng Phong";
                model.MaPhuongXa = "12345";
                model.TenQuanHuyen = "Thành phố Thái Bình";
                model.MaQuanHuyen = "336";
                model.TenTinhThanh = "Thái Bình";
                model.MaTinhThanh = "34";
                model.DiaChi = "Số 10, Phố Lê Lợi, Phường Lê Hồng Phong, TP Thái Bình";

                // ----- BHYT -----
                model.DoiTuong = 0;
                model.SoTheBHYT = "GD4343434343434";
                model.SoThe = "GD4343434343434";
                model.BatDauBHYT = new DateTime(2026, 1, 1);
                model.KetThucBHYT = new DateTime(2026, 12, 31);
                model.NgayHetHanBHYT = new DateTime(2026, 12, 31);

                // ----- Người nhà / liên lạc -----
                model.HoTenDiaChiNguoiNha = "Nguyễn Văn B - Số 10, Phố Lê Lợi, TP Thái Bình";
                model.SoDienThoaiNguoiNha = "0986111222";
                model.SoDienThoaiLienLac = "0986111222";

                // ----- Điều trị / khoa phòng -----
                model.MaKhoaLamBenhAn = "K01";
                model.TenKhoaLamBenhAn = "Khoa Nội tổng hợp";
                model.Buong = "B001";
                model.Giuong = "G001";
                model.NgayVaoVien = new DateTime(2026, 6, 20, 8, 0, 0);
                model.NgayRaVien = new DateTime(2026, 6, 30, 9, 0, 0);
                model.SoNhapVien = treatment.TREATMENT_CODE;
                model.Ma_LK = treatment.TREATMENT_CODE;
                model.MA_LK = treatment.TREATMENT_CODE;
                model.DaDieuTriTai = "Bệnh viện Đa khoa tỉnh Thái Bình";
                model.NgayBDDieuTri = new DateTime(2026, 6, 20);
                model.NgayKTDieuTri = new DateTime(2026, 6, 30);
                model.KinhGui = "Bệnh viện Bạch Mai";
                model.SoLuuTru = "LT-2026-0001";
                model.SoGiayCV = "GCT-2026-0001";
                model.DanhSachChuoiKy = new List<object>();
                model.ID = 0;

                // ----- Tóm tắt bệnh án -----
                model.DauHieuLamSan = "Sốt cao liên tục, ho, khó thở";
                model.ChanDoan = "Viêm phổi nặng";
                model.ChanDoanND = "Viêm phổi nặng biến chứng suy hô hấp";
                model.ChanDoanNgay = new DateTime(2026, 6, 29);
                model.ThuocKhac = "Kháng sinh Ceftriaxone, hỗ trợ hô hấp";

                // ----- Tình trạng lúc chuyển viện -----
                model.MachTCV = "95";
                model.HuyetApcmHgTCV = "110/70";
                model.NhipThoTCV = "22";
                model.SPO2TCV = "94";
                model.LiDoCV = "Vượt quá khả năng điều trị của tuyến dưới";
                model.PTVanChuyen = "Xe cứu thương";
                model.HoTenNDD = "Điều dưỡng Trần Thị C";
                model.NgayDuaDi = new DateTime(2026, 6, 30, 9, 30, 0);
                model.CVNgay = new DateTime(2026, 6, 30);
                model.CVGio = "9";
                model.CVPhut = "30";

                // ----- Người ký / mẫu phiếu -----
                model.HTBacSyDieuTri = "BS. Lê Văn D";
                model.HTGiamDocBV = "GĐ. Phạm Văn E";
                model.TenMauPhieu = "Giấy Chuyển Viện";
                model.MaQuanLy = 0;
                // IDMauPhieu + MaCoSoKhamChuaBenh để trống → thư viện tự lấy theo HisConfig
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
    }
}
