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
using EMR_MAIN;
using EMR_MAIN.DATABASE.BenhAn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.FormMedicalRecord.Base
{
    public class EMR_ADO
    {
        public string KyDienTu_DiaChiACS = "";
        public string KyDienTu_DiaChiEMR = "";
        public string KyDienTu_DiaChiThuVienKy = "";
        public string KyDienTu_ApplicationCode = "HIS";
        public string KyDienTu_TREATMENT_CODE = "";

        public string KyDienTu_DiaChiMOS = "";
        public string KyDienTu_TokenCode = "";

        public string UserCodeLogin = "";
        public string MaPhieu = "";
        public string SoLuuTru = "";
        public string MaYTe = "";

        public long? TreatmentId = null;

        public long? TuyChonCanhBaoVanBanDaKy = null;

        public long? IdPhong = null;
        public long? IdLoaiPhong = null;
        public string MaPhong = null;
        public string MaLoaiPhong = null;
        public string jsonbenhan = "";

        public LoaiBenhAnEMR _LoaiBenhAnEMR_s { get; set; }
        public HanhChinhBenhNhan _HanhChinhBenhNhan_s { get; set; }
        public ThongTinDieuTri _ThongTinDieuTri_s { get; set; }

        public DauSinhTon _DauHieuSinhTonMoi_s { get; set; }
        public List<ThuocKhangSinh> _KhangSinh_s { get; set; }
        public bool IsDongBenhAn_s { get; set; }


    }
}
