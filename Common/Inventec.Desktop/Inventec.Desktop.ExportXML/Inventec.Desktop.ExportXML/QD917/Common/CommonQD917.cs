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
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Inventec.Desktop.ExportXML.QD917.Common
{
    public class CommonQD917
    {
        public ResultReturn CreatedXmlFile<T>(T input, bool encode, bool displayNamspacess, bool saveFile, string path)
        {
            string xmlFile;
            try
            {
                var enc = Encoding.UTF8;
                using (var ms = new MemoryStream())
                {
                    var xmlNamespaces = new XmlSerializerNamespaces();
                    if (displayNamspacess)
                        xmlNamespaces.Add("xsd", "http://www.w3.org/2001/XMLSchema");
                    else
                        xmlNamespaces.Add("", "");

                    var xmlWriterSettings = new XmlWriterSettings
                    {
                        CloseOutput = false,
                        Encoding = enc,
                        OmitXmlDeclaration = false,
                        Indent = true
                    };
                    using (var xw = XmlWriter.Create(ms, xmlWriterSettings))
                    {
                        var s = new XmlSerializer(typeof(T));
                        s.Serialize(xw, input, xmlNamespaces);
                    }
                    xmlFile = enc.GetString(ms.ToArray());
                    if (saveFile)//kiểm tra lưu file không
                    {
                        using (var file = new StreamWriter(path))
                        {
                            file.Write(xmlFile);
                        }
                        return new ResultReturn { Obj = null, Succeeded = true, Result = string.Format("Lưu file XML vào ''{0}'' thành công", path) };
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResultReturn { Obj = null, Succeeded = false, Result = ex.Message };
            }
            if (encode)//kiểm tra nếu cần mã hóa file thì mã hóa sau đó trả lại cho ng dùng
            {
                var encodeXml = EncodeBase64(Encoding.UTF8, xmlFile);
                if (!string.IsNullOrEmpty(encodeXml))
                    return new ResultReturn { Obj = encodeXml, Succeeded = true, Result = "Mã hóa XML sang Base64 thành công" };
            }
            return new ResultReturn { Obj = xmlFile, Succeeded = true, Result = "" };//nếu không cần mã hóa file thì trả lại file string
        }

        private string EncodeBase64(Encoding encoding, string text)//Mã hóa file XML sang Base64
        {
            if (text == null)
                return null;
            byte[] textAsBytes = encoding.GetBytes(text);
            return Convert.ToBase64String(textAsBytes);
        }

        internal XmlCDataSection ConverToXmlDocument(string dataConvert)//chuyển đổi sang kiểu <![CDATA[string....]]>
        {
            XmlCDataSection result;
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml("<book genre='novel' ISBN='1-861001-57-5'>" + "<title>Pride And Prejudice</title>" + "</book>");
                result = doc.CreateCDataSection(dataConvert);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        internal string EncodeDepartment(string str, Config config)//mã hóa tên khoa sang mã chuẩn theo BHYT
        {
            try
            {
                if (str == config.KhoaKhamBenh)
                    return DepartmentBhyt.KhoaKhamBenh;
                if (str == config.KhoaHoiSucCapCuu)
                    return DepartmentBhyt.KhoaHoiSucCapCuu;
                if (str == config.KhoaNoiTongHop)
                    return DepartmentBhyt.KhoaNoiTongHop;
                if (str == config.KhoaNoiTimMach)
                    return DepartmentBhyt.KhoaNoiTimMach;
                if (str == config.KhoaNoiTieuHoa)
                    return DepartmentBhyt.KhoaNoiTieuHoa;
                if (str == config.KhoaNoiCoXuongKhop)
                    return DepartmentBhyt.KhoaNoiCoXuongKhop;
                if (str == config.KhoaNoiThanTietNieu)
                    return DepartmentBhyt.KhoaNoiThanTietNieu;
                if (str == config.KhoaNoiTiet)
                    return DepartmentBhyt.KhoaNoiTiet;
                if (str == config.KhoaDiUng)
                    return DepartmentBhyt.KhoaDiUng;
                if (str == config.KhoaHuyetHocLamSang)
                    return DepartmentBhyt.KhoaHuyetHocLamSang;
                if (str == config.KhoaTruyenNhiem)
                    return DepartmentBhyt.KhoaTruyenNhiem;
                if (str == config.KhoaLao)
                    return DepartmentBhyt.KhoaLao;
                if (str == config.KhoaDaLieu)
                    return DepartmentBhyt.KhoaDaLieu;
                if (str == config.KhoaThanKinh)
                    return DepartmentBhyt.KhoaThanKinh;
                if (str == config.KhoaTamThan)
                    return DepartmentBhyt.KhoaTamThan;
                if (str == config.KhoaYHocCoTruyen)
                    return DepartmentBhyt.KhoaYHocCoTruyen;
                if (str == config.KhoaLaoHoc)
                    return DepartmentBhyt.KhoaLaoHoc;
                if (str == config.KhoaNhi)
                    return DepartmentBhyt.KhoaNhi;
                if (str == config.KhoaNgoaiTongHop)
                    return DepartmentBhyt.KhoaNgoaiTongHop;
                if (str == config.KhoaNgoaiThanKinh)
                    return DepartmentBhyt.KhoaNgoaiThanKinh;
                if (str == config.KhoaNgoaiLongNguc)
                    return DepartmentBhyt.KhoaNgoaiLongNguc;
                if (str == config.KhoaNgoaiTieuHoa)
                    return DepartmentBhyt.KhoaNgoaiTieuHoa;
                if (str == config.KhoaNgoaiThanTietNieu)
                    return DepartmentBhyt.KhoaNgoaiThanTietNieu;
                if (str == config.KhoaChanThuongChinhHinh)
                    return DepartmentBhyt.KhoaChanThuongChinhHinh;
                if (str == config.KhoaBong)
                    return DepartmentBhyt.KhoaBong;
                if (str == config.KhoaPhauThuatGayMeHoiSuc)
                    return DepartmentBhyt.KhoaPhauThuatGayMeHoiSuc;
                if (str == config.KhoaPhuSan)
                    return DepartmentBhyt.KhoaPhuSan;
                if (str == config.KhoaTaiMuiHong)
                    return DepartmentBhyt.KhoaTaiMuiHong;
                if (str == config.KhoaRangHamMat)
                    return DepartmentBhyt.KhoaRangHamMat;
                if (str == config.KhoaMat)
                    return DepartmentBhyt.KhoaMat;
                if (str == config.KhoaVatLyTriLieuPhucHoiChucNang)
                    return DepartmentBhyt.KhoaVatLyTriLieuPhucHoiChucNang;
                if (str == config.KhoaYHocHatNhan)
                    return DepartmentBhyt.KhoaYHocHatNhan;
                if (str == config.KhoaUngBuou)
                    return DepartmentBhyt.KhoaUngBuou;
                if (str == config.KhoaTruyenMau)
                    return DepartmentBhyt.KhoaTruyenMau;
                if (str == config.KhoaLocMau)
                    return DepartmentBhyt.KhoaLocMau;
                if (str == config.KhoaHuyetHoc)
                    return DepartmentBhyt.KhoaHuyetHoc;
                if (str == config.KhoaSinhHoa)
                    return DepartmentBhyt.KhoaSinhHoa;
                if (str == config.KhoaViSinh)
                    return DepartmentBhyt.KhoaViSinh;
                if (str == config.KhoaChuanDoanHinhAnh)
                    return DepartmentBhyt.KhoaChuanDoanHinhAnh;
                if (str == config.KhoaThamDoChucNang)
                    return DepartmentBhyt.KhoaThamDoChucNang;
                if (str == config.KhoaNoiSoi)
                    return DepartmentBhyt.KhoaNoiSoi;
                if (str == config.KhoaGiaiPhauBenh)
                    return DepartmentBhyt.KhoaGiaiPhauBenh;
                if (str == config.KhoaChongNhiemKhuan)
                    return DepartmentBhyt.KhoaChongNhiemKhuan;
                if (str == config.KhoaDuoc)
                    return DepartmentBhyt.KhoaDuoc;
                if (str == config.KhoaDinhDuong)
                    return DepartmentBhyt.KhoaDinhDuong;
                if (str == config.KhoaSinhHocPhanTu)
                    return DepartmentBhyt.KhoaSinhHocPhanTu;
                if (str == config.KhoaXetNghiem)
                    return DepartmentBhyt.KhoaXetNghiem;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return "";
        }
    }
}
