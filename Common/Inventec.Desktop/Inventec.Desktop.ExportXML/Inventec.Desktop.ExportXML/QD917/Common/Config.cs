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
namespace Inventec.Desktop.ExportXML.QD917.Common
{
    public class Config
    {
        //cấu hình giới tính theo BHYT(HIS_GENDER)
        public long Nam { get; set; }
        public long Nu { get; set; }

        //cấu hình mã tai nạn thương tích theo BHYT (HIS_ACCIDENT_HURT)
        public long KhongBiTaiNan { get; set; } //không bị tai nạn
        public long TaiNanGiaoThong { get; set; } //tai nạn giao thông
        public long TaiNanLaoDong { get; set; } //tai nạn lao động
        public long DuoiNuoc { get; set; } //tai nạn đuối nước
        public long Bong { get; set; } //bị bỏng
        public long BaoLucGiaDinhXaHoi { get; set; } //bạo lực gia đình
        public long TuTu { get; set; } //tự tử
        public long NgoDoc { get; set; } //ngộ độc
        public long TaiNanKhac { get; set; } //tai nạn khác

        //cấu hình kết quả điều trị theo BHYT (HIS_TREATMENT_RESULT)
        public long Khoi { get; set; } //khỏi
        public long Do { get; set; } //đỡ
        public long KhongThayDoi { get; set; } //không thay đổi
        public long NangHon { get; set; } //nặng hơn
        public long TuVong { get; set; } //tử vong

        //cấu hình tình trạng ra viện theo BHYT (HIS_TREATMENT_END_TYPE)
        public long RaVien { get; set; } //ra viện
        public long ChuyenVien { get; set; } //chuyển viện
        public long TronVien { get; set; } //trốn viện
        public long XinRaVien { get; set; } //xin ra viện

        //cấu hình hình thức khám chữa bệnh theo BHYT (HIS_TREATMENT_TYPE)
        public long Kham { get; set; } //khám
        public long NgoaiTru { get; set; } //ngoại trú
        public long NoiTru { get; set; } //nội trú

        //cấu hình loại dịch vụ (HIS_HEIN_SERVICE_TYPE)
        public long XetNghiem { get; set; }//xét nghiệm
        public long ChuanDoanHinhAnh { get; set; }//chuẩn đoán hình ảnh
        public long ThamDoChucNang { get; set; }//thăm dò chức năng
        public long ThuocTrongDanhMucBhyt { get; set; }//thuốc trong danh mục BHYT
        public long ThuocDieuTriUngThuChongThaiGhepNgoaiDanhMuc { get; set; }//thuốc điều trị ung thư, chống thải ghép ngoài danh mục
        public long ThuocThanhToanTheoTyLe { get; set; }//thuốc thanh toán theo tỷ lên
        public long MauVaChePhamMau { get; set; }//máu và chế phẩm máu
        public long ThuThuatPhauThuat { get; set; }//thủ thuật, phẫu thuật
        public long DvktThanhToanTheoTyLe { get; set; }//dịch vụ kỹ thuật thanh toán theo tỷ lệ
        public long VatTuTrongDanhMucBhyt { get; set; }//vật tư y tế trong danh mục BHYT
        public long VatTuThanhToanTheoTyLe { get; set; }//vật tư y tế thanh toán theo tỷ lệ
        public long VanChuyen { get; set; }//vận chuyển
        public long KhamBenh { get; set; }//khám bệnh
        public long NgayGiuongNgoaiTru { get; set; }//giường điều trị ngoại trú
        public long NgayGiuongNoiTru { get; set; }//giường điều trị nội trú

        //cấu hình mã khoa theo phụ lục bảng 7 BHYT (HIS_DEPARTMENT)
        public string KhoaKhamBenh { get; set; }//khoa khám bệnh
        public string KhoaHoiSucCapCuu { get; set; }//khoa hồi sức cấp cứu
        public string KhoaNoiTongHop { get; set; }//khoa nội tổng hợp
        public string KhoaNoiTimMach { get; set; }//khoa nội tim mạch
        public string KhoaNoiTieuHoa { get; set; }//khoa nội tiêu hóa
        public string KhoaNoiCoXuongKhop { get; set; }//khoa nội cơ xương khớp
        public string KhoaNoiThanTietNieu { get; set; }//khoa nội thân tiết niệu
        public string KhoaNoiTiet { get; set; }//khoa nội tiết
        public string KhoaDiUng { get; set; }//khoa dị ứng
        public string KhoaHuyetHocLamSang { get; set; }//khoa huyết học lâm sàng
        public string KhoaTruyenNhiem { get; set; }//khoa truyền nhiễm
        public string KhoaLao { get; set; }//khoa lao
        public string KhoaDaLieu { get; set; }//khoa da liễu
        public string KhoaThanKinh { get; set; }//khoa thần kinh
        public string KhoaTamThan { get; set; }//khoa tâm thần
        public string KhoaYHocCoTruyen { get; set; }//khoa y học cổ truyền
        public string KhoaLaoHoc { get; set; }//khoa lao học
        public string KhoaNhi { get; set; }//khoa nhi
        public string KhoaNgoaiTongHop { get; set; }//khoa ngoại tổng hợp
        public string KhoaNgoaiThanKinh { get; set; }//khoa ngoại thần kinh
        public string KhoaNgoaiLongNguc { get; set; }//khoa ngoại lồng ngực
        public string KhoaNgoaiTieuHoa { get; set; }//khoa ngoại tiêu hóa
        public string KhoaNgoaiThanTietNieu { get; set; }//khoa ngoại thần kinh tiết niệu
        public string KhoaChanThuongChinhHinh { get; set; }//khoa chấn thương chỉnh hình
        public string KhoaBong { get; set; }//khoa bỏng
        public string KhoaPhauThuatGayMeHoiSuc { get; set; }//khoa phẫu thuật gây mê hồi sức
        public string KhoaPhuSan { get; set; }//khoa phụ sản
        public string KhoaTaiMuiHong { get; set; }//khoa tai mũi họng
        public string KhoaRangHamMat { get; set; }//khoa răng hàm mặt
        public string KhoaMat { get; set; }//khoa mắt
        public string KhoaVatLyTriLieuPhucHoiChucNang { get; set; }//khoa vật lý trị liệu - phục hồi chức năng
        public string KhoaYHocHatNhan { get; set; }//khoa y học hạt nhân
        public string KhoaUngBuou { get; set; }//khoa ung bướu
        public string KhoaTruyenMau { get; set; }//khoa truyền máu
        public string KhoaLocMau { get; set; }//khoa lọc máu
        public string KhoaHuyetHoc { get; set; }//khoa huyết học
        public string KhoaSinhHoa { get; set; }//khoa sinh hóa
        public string KhoaViSinh { get; set; }//khoa vi sinh
        public string KhoaChuanDoanHinhAnh { get; set; }//khoa chuẩn đoán hình ảnh
        public string KhoaThamDoChucNang { get; set; }//khoa thăm dò chức năng
        public string KhoaNoiSoi { get; set; }//khoa nội soi
        public string KhoaGiaiPhauBenh { get; set; }//khoa giải phẫu bệnh
        public string KhoaChongNhiemKhuan { get; set; }//khoa chống nhiễm khuẩn
        public string KhoaDuoc { get; set; }//khoa dược
        public string KhoaDinhDuong { get; set; }//khoa dinh dưỡng
        public string KhoaSinhHocPhanTu { get; set; }//khoa sinh học phân tử
        public string KhoaXetNghiem { get; set; }//khoa xét nghiệm
    }

    internal class XmlType
    {
        internal const string XML1 = "XML1";
        internal const string XML2 = "XML2";
        internal const string XML3 = "XML3";
        internal const string XML4 = "XML4";
        internal const string XML5 = "XML5";
    }

    internal class AccidentHurtCodeBhyt
    {
        public const int KhongBiTaiNan = 0; //không bị tai nạn
        public const int TaiNanGiaoThong = 1; //tai nạn giao thông
        public const int TaiNanLaoDong = 2; //tai nạn lao động
        public const int DuoiNuoc = 3; //tai nạn đuối nước
        public const int Bong = 4; //bị bỏng
        public const int BaoLucGiaDinhXaHoi = 5; //bạo lực gia đình
        public const int TuTu = 6; //tự tử
        public const int NgoDoc = 7; //ngộ độc
        public const int TaiNanKhac = 8; //tai nạn khác
    }

    internal class TreatmentResultBhyt
    {
        public const int Khoi = 1; //khỏi
        public const int Do = 2; //đỡ
        public const int KhongThayDoi = 3; //không thay đổi
        public const int NangHon = 4; //nặng hơn
        public const int TuVong = 5; //tử vong
    }

    public class TreatmentEndTypeBhyt
    {
        public const int RaVien = 1;
        public const int ChuyenVien = 2; //chuyển viện
        public const int TronVien = 3; //trốn viện
        public const int XinRaVien = 4; //xin ra viện
    }

    public class TreatmentTypeBhyt
    {
        public const int Kham = 1; //khám
        public const int NgoaiTru = 2; //ngoại trú
        public const int NoiTru = 3; //nội trú
    }

    public class HeinServiceTypeBhyt
    {
        public const long XetNghiem = 1;//xét nghiệm
        public const long ChuanDoanHinhAnh = 2;//chuẩn đoán hình ảnh
        public const long ThamDoChucNang = 3;//thăm dò chức năng
        public const long ThuocTrongDanhMucBhyt = 4;//thuốc trong danh mục BHYT
        public const long ThuocDieuTriUngThuChongThaiGhepNgoaiDanhMuc = 5;//thuốc điều trị ung thư, chống thải ghép ngoài danh mục
        public const long ThuocThanhToanTheoTyLe = 6;//thuốc thanh toán theo tỷ lên
        public const long MauVaChePhamMau = 7;//máu và chế phẩm máu
        public const long ThuThuatPhauThuat = 8;//thủ thuật, phẫu thuật
        public const long DvktThanhToanTheoTyLe = 9;//dịch vụ kỹ thuật thanh toán theo tỷ lệ
        public const long VatTuTrongDanhMucBhyt = 10;//vật tư y tế trong danh mục BHYT
        public const long VatTuThanhToanTheoTyLe = 11;//vật tư y tế thanh toán theo tỷ lệ
        public const long VanChuyen = 12;//vận chuyển
        public const long KhamBenh = 13;//khám bệnh
        public const long NgayGiuongNgoaiTru = 14;//giường điều trị ngoại trú
        public const long NgayGiuongNoiTru = 15;//giường điều trị nội trú
    }

    public class DepartmentBhyt
    {
        internal const string KhoaKhamBenh = "K01";//khoa khám bệnh
        internal const string KhoaHoiSucCapCuu = "K02";//khoa hồi sức cấp cứu
        internal const string KhoaNoiTongHop = "K03";//khoa nội tổng hợp
        internal const string KhoaNoiTimMach = "K04";//khoa nội tim mạch
        internal const string KhoaNoiTieuHoa = "K05";//khoa nội tiêu hóa
        internal const string KhoaNoiCoXuongKhop = "K06";//khoa nội cơ xương khớp
        internal const string KhoaNoiThanTietNieu = "K07";//khoa nội thân tiết niệu
        internal const string KhoaNoiTiet = "K08";//khoa nội tiết
        internal const string KhoaDiUng = "K09";//khoa dị ứng
        internal const string KhoaHuyetHocLamSang = "K10";//khoa huyết học lâm sàng
        internal const string KhoaTruyenNhiem = "K11";//khoa truyền nhiễm
        internal const string KhoaLao = "K12";//khoa lao
        internal const string KhoaDaLieu = "K13";//khoa da liễu
        internal const string KhoaThanKinh = "K14";//khoa thần kinh
        internal const string KhoaTamThan = "K15";//khoa tâm thần
        internal const string KhoaYHocCoTruyen = "K16";//khoa y học cổ truyền
        internal const string KhoaLaoHoc = "K17";//khoa lao học
        internal const string KhoaNhi = "K18";//khoa nhi
        internal const string KhoaNgoaiTongHop = "K19";//khoa ngoại tổng hợp
        internal const string KhoaNgoaiThanKinh = "K20";//khoa ngoại thần kinh
        internal const string KhoaNgoaiLongNguc = "K21";//khoa ngoại lồng ngực
        internal const string KhoaNgoaiTieuHoa = "K22";//khoa ngoại tiêu hóa
        internal const string KhoaNgoaiThanTietNieu = "K23";//khoa ngoại thần kinh tiết niệu
        internal const string KhoaChanThuongChinhHinh = "K24";//khoa chấn thương chỉnh hình
        internal const string KhoaBong = "K25";//khoa bỏng
        internal const string KhoaPhauThuatGayMeHoiSuc = "K26";//khoa phẫu thuật gây mê hồi sức
        internal const string KhoaPhuSan = "K27";//khoa phụ sản
        internal const string KhoaTaiMuiHong = "K28";//khoa tai mũi họng
        internal const string KhoaRangHamMat = "K29";//khoa răng hàm mặt
        internal const string KhoaMat = "K30";//khoa mắt
        internal const string KhoaVatLyTriLieuPhucHoiChucNang = "K31";//khoa vật lý trị liệu - phục hồi chức năng
        internal const string KhoaYHocHatNhan = "K32";//khoa y học hạt nhân
        internal const string KhoaUngBuou = "K33";//khoa ung bướu
        internal const string KhoaTruyenMau = "K34";//khoa truyền máu
        internal const string KhoaLocMau = "K35";//khoa lọc máu
        internal const string KhoaHuyetHoc = "K36";//khoa huyết học
        internal const string KhoaSinhHoa = "K37";//khoa sinh hóa
        internal const string KhoaViSinh = "K38";//khoa vi sinh
        internal const string KhoaChuanDoanHinhAnh = "K39";//khoa chuẩn đoán hình ảnh
        internal const string KhoaThamDoChucNang = "K40";//khoa thăm dò chức năng
        internal const string KhoaNoiSoi = "K41";//khoa nội soi
        internal const string KhoaGiaiPhauBenh = "K42";//khoa giải phẫu bệnh
        internal const string KhoaChongNhiemKhuan = "K43";//khoa chống nhiễm khuẩn
        internal const string KhoaDuoc = "K44";//khoa dược
        internal const string KhoaDinhDuong = "K45";//khoa dinh dưỡng
        internal const string KhoaSinhHocPhanTu = "K46";//khoa sinh học phân tử
        internal const string KhoaXetNghiem = "K47";//khoa xét nghiệm
    }
}
