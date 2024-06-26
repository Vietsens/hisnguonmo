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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.DelegateRegister
{
    public interface IRegisterAppConfig
    {       
        MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE GetPatientTypeDefault();
        long GetAlertExpriedTimeHeinCardBhyt();
        long GetCheDoHienThiNoiLamViecManHinhDangKyTiepDon();
        long GetTiepDon_HienThiMotSoThongTinThemBenhNhan();
        string GetDangKyTiepDonThuTienSau();
        long GetDangKyTiepDonThoiGianLoadDanhSachPhongKham();
        long GetDangKyTiepDonHienThiThongBaoTimDuocBenhNhan();
        string GetDangKyTiepDonGoiBenhNhanBangCPA();
        long GetCheDoTuDongFillDuLieuDiaChiGhiTrenTheVaoODiaChiBenhNhanHayKhong();
        string GetInsuranceExpertiseCheckHeinConfig();

        /// <summary>
        /// Đặt là 1 nếu chọn chế độ hiển thị để xem xong rồi in, đặt là 2 nếu chọn chế độ in ngay không cần xem
        /// </summary>
        long GetCheDoInPhieuDangKyDichVuKhamBenh();
        string GetOweTypeDefault();
        long GetCheDoTuDongCheckThongTinTheBHYT();

        /// <summary>
        /// Cấu hình tự động fill yêu cầu - phòng khám gần nhất khi tìm bệnh nhân cũ
        /// Đặt 1 là tự động fill
        /// Mặc định là không tự động
        /// </summary>
        string GetIsAutoFillDataRecentServiceRoom();

        string GetIsDangKyQuaTongDai();
        bool GetIsVisibleSomeControl();
    }
}
