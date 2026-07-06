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
namespace HIS.Desktop.Plugins.HemodialysisDispensary
{
    /// <summary>
    /// URI các API dùng trong plugin — TẤT CẢ đều là API backend ĐÃ CÓ SẴN
    /// (không thêm backend mới). Nguồn dữ liệu bám theo màn Chạy thận (Hemodialysis).
    /// </summary>
    class RequestUriStore
    {
        /// <summary>Lưới trái — DS bệnh nhân chạy thận theo Phòng + Ngày + Ca (V_HIS_SERVICE_REQ_8).</summary>
        internal const string HIS_SERVICE_REQ_GETVIEW_8 = "api/HisServiceReq/GetView8";

        /// <summary>Lưới phải trên — Y lệnh đơn chạy thận BS, load theo bệnh nhân/cross-treatment (V_HIS_SERVICE_REQ_7).</summary>
        internal const string HIS_SERVICE_REQ_GETVIEW_7 = "api/HisServiceReq/GetView7";

        /// <summary>Lấy 1 y lệnh (HIS_SERVICE_REQ) theo ID — dựng ServiceReq cho AssignPrescriptionKidney.</summary>
        internal const string HIS_SERVICE_REQ_GET = "api/HisServiceReq/Get";

        /// <summary>Lưới phải dưới — chi tiết thuốc + KIDNEY_AMOUNT_LEFT (Còn lại) (V_HIS_SERVICE_REQ_METY).</summary>
        internal const string HIS_SERVICE_REQ_METY_GETVIEW = "api/HisServiceReqMety/GetView";
    }
}
