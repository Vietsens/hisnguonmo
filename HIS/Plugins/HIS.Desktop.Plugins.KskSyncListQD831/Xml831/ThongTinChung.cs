/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using System.Collections.Generic;
using System.Xml.Serialization;

namespace HIS.Desktop.Plugins.KskSyncListQD831.Xml831
{
    /// <summary>&lt;THONGTINCHUNG&gt; — hành chính bệnh nhân + danh sách quan hệ gia đình.</summary>
    public class ThongTinChung
    {
        [XmlElement("MADINHDANH")]
        public string MaDinhDanh { get; set; }

        [XmlElement("VNEID")]
        public string VneId { get; set; }

        [XmlElement("MABHXH")]
        public string MaBhxh { get; set; }

        [XmlElement("MATHE")]
        public string MaThe { get; set; }

        [XmlElement("MAHO_GIADINH")]
        public string MaHoGiaDinh { get; set; }

        [XmlElement("MA_NHANKHAU")]
        public string MaNhanKhau { get; set; }

        [XmlElement("HOTEN")]
        public string HoTen { get; set; }

        [XmlElement("QUANHE_CHUHO")]
        public string QuanHeChuHo { get; set; }

        [XmlElement("GIOITINH")]
        public string GioiTinh { get; set; }

        [XmlElement("NHOMMAU_HEABO")]
        public string NhomMauHeAbo { get; set; }

        [XmlElement("NHOMMAU_HERH")]
        public string NhomMauHeRh { get; set; }

        [XmlElement("NGAYSINH")]
        public string NgaySinh { get; set; }

        [XmlElement("MATINH_KHAISINH")]
        public string MaTinhKhaiSinh { get; set; }

        [XmlElement("MADANTOC")]
        public string MaDanToc { get; set; }

        [XmlElement("MAQUOCTICH")]
        public string MaQuocTich { get; set; }

        [XmlElement("MATONGIAO")]
        public string MaTonGiao { get; set; }

        [XmlElement("MANGHENGHIEP")]
        public string MaNgheNghiep { get; set; }

        [XmlElement("SOCMND")]
        public string SoCmnd { get; set; }

        [XmlElement("NGAYCAP")]
        public string NgayCap { get; set; }

        [XmlElement("NOICAP")]
        public string NoiCap { get; set; }

        [XmlElement("DIACHI_THUONGTRU")]
        public string DiaChiThuongTru { get; set; }

        [XmlElement("MATINH_THUONGTRU")]
        public string MaTinhThuongTru { get; set; }

        [XmlElement("MAHUYEN_THUONGTRU")]
        public string MaHuyenThuongTru { get; set; }

        [XmlElement("MAXA_THUONGTRU")]
        public string MaXaThuongTru { get; set; }

        [XmlElement("MATHONXOM_THUONGTRU")]
        public string MaThonXomThuongTru { get; set; }

        [XmlElement("DIACHI_HIENTAI")]
        public string DiaChiHienTai { get; set; }

        [XmlElement("MATINH_HIENTAI")]
        public string MaTinhHienTai { get; set; }

        [XmlElement("MAHUYEN_HIENTAI")]
        public string MaHuyenHienTai { get; set; }

        [XmlElement("MAXA_HIENTAI")]
        public string MaXaHienTai { get; set; }

        [XmlElement("MATHONXOM_HIENTAI")]
        public string MaThonXomHienTai { get; set; }

        [XmlElement("DIENTHOAI_CD")]
        public string DienThoaiCd { get; set; }

        [XmlElement("DIENTHOAI_DD")]
        public string DienThoaiDd { get; set; }

        [XmlElement("EMAIL")]
        public string Email { get; set; }

        [XmlElement("QUANHE_GIADINH")]
        public List<QuanHeGiaDinh> QuanHeGiaDinh { get; set; }
    }
}
