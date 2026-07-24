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
    /// <summary>&lt;HOSOKHAMCHUABENH&gt; — 1 lượt khám: hành chính lượt khám, sinh hiệu, khám lâm sàng, CLS, kết luận.</summary>
    public class HoSoKhamChuaBenh
    {
        [XmlElement("MA_LK")]
        public string MaLk { get; set; }

        [XmlElement("NGAYKHAM")]
        public string NgayKham { get; set; }

        [XmlElement("NGAYBATDAU")]
        public string NgayBatDau { get; set; }

        [XmlElement("NGAYKETTHUC")]
        public string NgayKetThuc { get; set; }

        [XmlElement("MABACSI")]
        public string MaBacSi { get; set; }

        [XmlElement("BACSIKHAM")]
        public string BacSiKham { get; set; }

        [XmlElement("LYDOKHAM")]
        public string LyDoKham { get; set; }

        [XmlElement("BENHSU")]
        public string BenhSu { get; set; }

        [XmlElement("MACH")]
        public string Mach { get; set; }

        [XmlElement("NHIETDO")]
        public string NhietDo { get; set; }

        [XmlElement("HUYETAPTT")]
        public string HuyetApTt { get; set; }

        [XmlElement("HUYETAPTD")]
        public string HuyetApTd { get; set; }

        [XmlElement("NHIPTHO")]
        public string NhipTho { get; set; }

        [XmlElement("CHIEUCAO")]
        public string ChieuCao { get; set; }

        [XmlElement("CHISOBMI")]
        public string ChiSoBmi { get; set; }

        [XmlElement("CANNANG")]
        public string CanNang { get; set; }

        [XmlElement("VONGBUNG")]
        public string VongBung { get; set; }

        [XmlElement("SUDUNGKINH")]
        public string SuDungKinh { get; set; }

        [XmlElement("MATPHAI")]
        public string MatPhai { get; set; }

        [XmlElement("MATTRAI")]
        public string MatTrai { get; set; }

        [XmlElement("KHAMDA")]
        public string KhamDa { get; set; }

        [XmlElement("KHAMNIEMMAC")]
        public string KhamNiemMac { get; set; }

        [XmlElement("KHAMTOANTHANKHAC")]
        public string KhamToanThanKhac { get; set; }

        [XmlElement("KHAMTIMMACH")]
        public string KhamTimMach { get; set; }

        [XmlElement("KHAMHOHAP")]
        public string KhamHoHap { get; set; }

        [XmlElement("KHAMTIEUHOA")]
        public string KhamTieuHoa { get; set; }

        [XmlElement("KHAMTIETNIEU")]
        public string KhamTietNieu { get; set; }

        [XmlElement("KHAMCOXUONGKHOP")]
        public string KhamCoXuongKhop { get; set; }

        [XmlElement("KHAMNOITIET")]
        public string KhamNoiTiet { get; set; }

        [XmlElement("KHAMTHANKINH")]
        public string KhamThanKinh { get; set; }

        [XmlElement("KHAMTAMTHAN")]
        public string KhamTamThan { get; set; }

        [XmlElement("KHAMNGOAIKHOA")]
        public string KhamNgoaiKhoa { get; set; }

        [XmlElement("KHAMPHUKHOA")]
        public string KhamPhuKhoa { get; set; }

        [XmlElement("KHAMTAIMUIHONG")]
        public string KhamTaiMuiHong { get; set; }

        [XmlElement("KHAMRHM")]
        public string KhamRhm { get; set; }

        [XmlElement("KHAMMAT")]
        public string KhamMat { get; set; }

        [XmlElement("KHAMDALIEU")]
        public string KhamDaLieu { get; set; }

        [XmlElement("KHAMDINHDUONG")]
        public string KhamDinhDuong { get; set; }

        [XmlElement("KHAMVANDONG")]
        public string KhamVanDong { get; set; }

        [XmlElement("KHAC")]
        public string Khac { get; set; }

        [XmlElement("TC_TT_VD")]
        public string TcTtVd { get; set; }

        [XmlElement("TUVAN")]
        public string TuVan { get; set; }

        [XmlArray("CANLAMSANG")]
        [XmlArrayItem("DICHVU")]
        public List<DichVu> CanLamSang { get; set; }

        [XmlArray("CHANDOANKETLUAN")]
        [XmlArrayItem("CHANDOANBENH")]
        public List<ChanDoanBenh> ChanDoanKetLuan { get; set; }
    }
}
