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
using Inventec.Desktop.ExportXML.QD917.ADO.HoSoBenhNhan_ADO;
using Inventec.Desktop.ExportXML.QD917.Common;

namespace Inventec.Desktop.ExportXML.QD917.ProcessData
{
    public class HoSoBenhNhan_Process
    {
        private InputData _inputData;
        private CommonQD917 _commonQd917 = new CommonQD917();

        internal ResultReturn GiamDinhHs(InputData inputData)
        {
            this._inputData = inputData;
            ResultReturn result;
            try
            {
                if (ThongTinDonVi().Succeeded == false || ThongTinDonVi().Obj == null)
                    return ThongTinDonVi();
                if (ThongTinHoSo().Succeeded == false || ThongTinHoSo().Obj == null)
                    return ThongTinHoSo();
                var giamDinhHs = new HS_GIAMDINHHS_ADO
                {
                    THONGTINDONVI = ThongTinDonVi().Obj != null ? ThongTinDonVi().Obj as HS_THONGTINDONVI_ADO : null,
                    THONGTINHOSO = ThongTinHoSo().Obj != null ? ThongTinHoSo().Obj as HS_THONGTINHOSO_ADO : null,
                    CHUKYDONVI = _commonQd917.ConverToXmlDocument(!String.IsNullOrEmpty(GlobalStore.Signature) ? GlobalStore.Signature : "")
                };
                result = new ResultReturn { Obj = giamDinhHs, Succeeded = true, Result = "" };
            }
            catch (Exception ex)
            {
                result = new ResultReturn { Obj = null, Succeeded = false, Result = ex.Message };
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private ResultReturn ThongTinDonVi()
        {
            ResultReturn result;
            try
            {
                var thongTinDonVi = new HS_THONGTINDONVI_ADO
                {
                    MACSKCB = _commonQd917.ConverToXmlDocument(GlobalStore.Branch != null ? (GlobalStore.Branch.HEIN_MEDI_ORG_CODE ?? "") : "")
                };
                result = new ResultReturn { Obj = thongTinDonVi, Succeeded = true, Result = "" };
            }
            catch (Exception ex)
            {
                result = new ResultReturn { Obj = null, Succeeded = false, Result = ex.Message };
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private ResultReturn ThongTinHoSo()
        {
            ResultReturn result;
            try
            {
                if (DanhSachHoSo().Succeeded == false || DanhSachHoSo().Obj == null)
                    return DanhSachHoSo();
                var thongTinHoSo = new HS_THONGTINHOSO_ADO
                {
                    NGAYLAP = _commonQd917.ConverToXmlDocument(DateTime.Now.ToString("yyyyMMMMdd")),
                    SOLUONGHOSO = 1,//mỗi mỗi file XML chỉ chứa 1 hồ sơ nên gán = 1 luôn
                    DANHSACHHOSO = DanhSachHoSo().Obj != null ? DanhSachHoSo().Obj as HS_DANHSACHHOSO_ADO : null
                };
                result = new ResultReturn { Obj = thongTinHoSo, Succeeded = true, Result = "" };
            }
            catch (Exception ex)
            {
                result = new ResultReturn { Obj = null, Succeeded = false, Result = ex.Message };
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private ResultReturn DanhSachHoSo()
        {
            ResultReturn result;
            try
            {
                if (HoSo().Succeeded == false || HoSo().Obj == null)
                    return HoSo();
                var danhSachHoSo = new HS_DANHSACHHOSO_ADO
                {
                    HOSO = HoSo().Obj != null ? HoSo().Obj as HS_HOSO_ADO : null
                };
                result = new ResultReturn { Obj = danhSachHoSo, Succeeded = true, Result = "" };
            }
            catch (Exception ex)
            {
                result = new ResultReturn { Obj = null, Succeeded = false, Result = ex.Message };
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private ResultReturn HoSo()
        {
            ResultReturn result;
            try
            {
                if (FileHoSo().Succeeded == false || FileHoSo().Obj == null)
                    return FileHoSo();
                var hoSo = new HS_HOSO_ADO
                {
                    FILEHOSO = FileHoSo().Obj != null ? FileHoSo().Obj as List<HS_FILEHOSO_ADO> : null
                };
                result = new ResultReturn { Obj = hoSo, Succeeded = true, Result = "" };
            }
            catch (Exception ex)
            {
                result = new ResultReturn { Obj = null, Succeeded = false, Result = ex.Message };
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private ResultReturn FileHoSo()
        {
            ResultReturn result;
            try
            {
                //tạo file XML1
                var noidungFileXml1 = NoiDungFile(XmlType.XML1);
                if (noidungFileXml1.Succeeded == false || noidungFileXml1.Obj == null)
                    return noidungFileXml1;
                var xml1 = new HS_FILEHOSO_ADO
                {
                    LOAIHOSO = XmlType.XML1,
                    NOIDUNGFILE = noidungFileXml1.Obj != null ? noidungFileXml1.Obj as string : ""
                };
                //tạo file XML2
                var noiDungFileXml2 = NoiDungFile(XmlType.XML2);
                if (noiDungFileXml2.Succeeded == false || noiDungFileXml2.Obj == null)
                    return noiDungFileXml2;
                var xml2 = new HS_FILEHOSO_ADO
                {
                    LOAIHOSO = XmlType.XML2,
                    NOIDUNGFILE = noiDungFileXml2.Obj != null ? noiDungFileXml2.Obj as string : ""
                };
                //tạo file XML3
                var noiDungFileXml3 = NoiDungFile(XmlType.XML3);
                if (noiDungFileXml3.Succeeded == false || noiDungFileXml3.Obj == null)
                    return noiDungFileXml3;
                var xml3 = new HS_FILEHOSO_ADO
                {
                    LOAIHOSO = XmlType.XML3,
                    NOIDUNGFILE = noiDungFileXml3.Obj != null ? noiDungFileXml3.Obj as string : ""
                };
                ////tạo file XML4
                //var noiDungFileXml4 = NoiDungFile(XmlType.XML4);
                //var xml4 = new HS_FILEHOSO_ADO
                //{
                //    LOAIHOSO = XmlType.XML4,
                //    NOIDUNGFILE = noiDungFileXml4.Obj != null ? noiDungFileXml4.Obj as string : ""
                //};
                ////tạo file XML5
                //var noiDungFileXml5 = NoiDungFile(XmlType.XML5);
                //var xml5 = new HS_FILEHOSO_ADO
                //{
                //    LOAIHOSO = XmlType.XML5,
                //    NOIDUNGFILE = noiDungFileXml5.Obj != null ? noiDungFileXml5.Obj as string : ""
                //};
                //list XML1, XML2, XML3, XML4, XML5
                var listFileHoSos = new List<HS_FILEHOSO_ADO>
                {
                    xml1,
                    xml2,
                    xml3,
                    //xml4,
                    //xml5
                };
                result = new ResultReturn { Obj = listFileHoSos, Succeeded = true, Result = "" };
            }
            catch (Exception ex)
            {
                result = new ResultReturn { Obj = null, Succeeded = false, Result = ex.Message };
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private ResultReturn NoiDungFile(string type)
        {
            ResultReturn result;
            try
            {
                string xmlToString = null;
                switch (type)
                {
                    case XmlType.XML1:
                        var xml1 = new XML1_Process().CreateXml1(_inputData);
                        if (xml1.Succeeded == false || xml1.Obj == null)
                            return xml1;
                        var resultCreateXml1 = _commonQd917.CreatedXmlFile(xml1.Obj, true, false, false, string.Empty);
                        if (resultCreateXml1.Succeeded == false || resultCreateXml1.Obj == null)
                            return resultCreateXml1;
                        xmlToString = resultCreateXml1.Obj as string;
                        break;
                    case XmlType.XML2:
                        var xml2 = new XML2_Process().CreateXml2(_inputData);
                        if (xml2.Succeeded == false || xml2.Obj == null)
                            return xml2;
                        var resultCreateXml2 = _commonQd917.CreatedXmlFile(xml2.Obj, true, false, false, string.Empty);
                        if (resultCreateXml2.Succeeded == false || resultCreateXml2.Obj == null)
                            return resultCreateXml2;
                        xmlToString = resultCreateXml2.Obj as string;
                        break;
                    case XmlType.XML3:
                        var xml3 = new XML3_Process().CreateXml3(_inputData);
                        if (xml3.Succeeded == false || xml3.Obj == null)
                            return xml3;
                        var resultCreateXml3 = _commonQd917.CreatedXmlFile(xml3.Obj, true, false, false, string.Empty);
                        if (resultCreateXml3.Succeeded == false || resultCreateXml3.Obj == null)
                            return resultCreateXml3;
                        xmlToString = resultCreateXml3.Obj as string;
                        break;
                    //case XmlType.XML4:
                    //    break;
                    //case XmlType.XML5:
                    //    break;
                }
                result = new ResultReturn { Obj = xmlToString, Succeeded = true, Result = "" };
            }
            catch (Exception ex)
            {
                result = new ResultReturn { Obj = null, Succeeded = false, Result = ex.Message };
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
