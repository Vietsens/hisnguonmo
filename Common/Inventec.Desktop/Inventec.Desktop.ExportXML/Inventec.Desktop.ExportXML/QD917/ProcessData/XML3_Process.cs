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
using Inventec.Desktop.ExportXML.QD917.ADO.XML3_ADO;
using Inventec.Desktop.ExportXML.QD917.Common;

namespace Inventec.Desktop.ExportXML.QD917.ProcessData
{
    public class XML3_Process
    {
        private CommonQD917 _commonQd917 = new CommonQD917();
        private Config _config;

        public ResultReturn CreateXml3(InputData inputData)
        {
            ResultReturn result;
            try
            {
                var chitietDichVuKyThuat = ChiTietDvkt(inputData);
                if (chitietDichVuKyThuat.Obj == null || chitietDichVuKyThuat.Succeeded == false)
                    return new ResultReturn { Obj = null, Succeeded = false, Result = "" };
                var dsachChiTietDvkt = new XML3_DSACH_CHI_TIET_DVKT_ADO
                {
                    CHI_TIET_DVKT = chitietDichVuKyThuat.Obj as List<XML3_CHI_TIET_DVKT_ADO>
                };
                result = new ResultReturn { Obj = dsachChiTietDvkt, Succeeded = true, Result = "" };
            }
            catch (Exception ex)
            {
                result = new ResultReturn { Obj = null, Succeeded = false, Result = ex.Message };
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private ResultReturn ChiTietDvkt(InputData inputData)
        {
            ResultReturn result;
            try
            {
                var listChiTietDvkts = new List<XML3_CHI_TIET_DVKT_ADO>();
                var listChiTietVtyts = new List<XML3_CHI_TIET_DVKT_ADO>();
                var listHeinServiceType = new List<long>
                {
                    GlobalStore.HeinServiceType_Id__BedIn,
                    GlobalStore.HeinServiceType_Id__BenOut,
                    GlobalStore.HeinServiceType_Id__Diim,
                    GlobalStore.HeinServiceType_Id__Exam,
                    GlobalStore.HeinServiceType_Id__Fuex,
                    GlobalStore.HeinServiceType_Id__HighTech,                    
                    GlobalStore.HeinServiceType_Id__SurgMisu,
                    GlobalStore.HeinServiceType_Id__Test,
                    GlobalStore.HeinServiceType_Id__Tran
                };

                var listHeinServiceTypeMaterial = new List<long>
                {
                    GlobalStore.HeinServiceType_Id__MaterialIn,
                    GlobalStore.HeinServiceType_Id__MaterialOut,
                    GlobalStore.HeinServiceType_Id__MaterialRatio,
                    GlobalStore.HeinServiceType_Id__MaterialVttt
                };

                var listServeservs = inputData.L_V_HIS_SERE_SERV.Where(s => s.HEIN_SERVICE_TYPE_ID.HasValue && s.AMOUNT > 0 && s.PRICE > 0 && s.IS_EXPEND != IMSys.DbConfig.HIS_RS.HIS_SERE_SERV.IS_EXPEND__TRUE && (listHeinServiceType.Contains(s.HEIN_SERVICE_TYPE_ID.Value) || listHeinServiceTypeMaterial.Contains(s.HEIN_SERVICE_BHYT_ID.Value))).ToList();//lấy các dịch vụ không phải là thuốc, vật tư và có tỷ lệ chi trả tiền, các dịch vụ được hưởng BHYT
                foreach (var hisSereServ in listServeservs)
                {
                    string maVatTu = "";
                    long? maNhom = null;
                    string maDichVu = "";
                    string maBenh = "";

                    var xml3 = new XML3_CHI_TIET_DVKT_ADO();
                    if (listHeinServiceTypeMaterial.Contains(hisSereServ.HEIN_SERVICE_BHYT_ID.Value))
                    {
                        listChiTietVtyts.Add(xml3);

                        maVatTu = hisSereServ.HEIN_SERVICE_BHYT_CODE;
                        if (hisSereServ.HEIN_SERVICE_BHYT_ID.Value == GlobalStore.HeinServiceType_Id__MaterialIn)
                        {
                            maNhom = HeinServiceTypeBhyt.VatTuTrongDanhMucBhyt;
                        }
                        else if (hisSereServ.HEIN_SERVICE_BHYT_ID.Value == GlobalStore.HeinServiceType_Id__MaterialOut || hisSereServ.HEIN_SERVICE_BHYT_ID.Value == GlobalStore.HeinServiceType_Id__MaterialRatio)
                        {
                            maNhom = HeinServiceTypeBhyt.VatTuThanhToanTheoTyLe;
                        }
                    }
                    else
                    {
                        listChiTietDvkts.Add(xml3);
                        maDichVu = hisSereServ.HEIN_SERVICE_BHYT_CODE;
                        if (hisSereServ.HEIN_SERVICE_BHYT_ID.Value == GlobalStore.HeinServiceType_Id__BedIn)
                        {
                            maNhom = HeinServiceTypeBhyt.NgayGiuongNoiTru;
                        }
                        else if (hisSereServ.HEIN_SERVICE_BHYT_ID.Value == GlobalStore.HeinServiceType_Id__BenOut)
                        {
                            maNhom = HeinServiceTypeBhyt.NgayGiuongNgoaiTru;
                        }
                        else if (hisSereServ.HEIN_SERVICE_BHYT_ID.Value == GlobalStore.HeinServiceType_Id__Diim)
                        {
                            maNhom = HeinServiceTypeBhyt.ChuanDoanHinhAnh;
                        }
                        else if (hisSereServ.HEIN_SERVICE_BHYT_ID.Value == GlobalStore.HeinServiceType_Id__Exam)
                        {
                            maNhom = HeinServiceTypeBhyt.KhamBenh;
                        }
                        else if (hisSereServ.HEIN_SERVICE_BHYT_ID.Value == GlobalStore.HeinServiceType_Id__Fuex)
                        {
                            maNhom = HeinServiceTypeBhyt.ThamDoChucNang;
                        }
                        else if (hisSereServ.HEIN_SERVICE_BHYT_ID.Value == GlobalStore.HeinServiceType_Id__HighTech)
                        {
                            maNhom = HeinServiceTypeBhyt.DvktThanhToanTheoTyLe;
                        }
                        else if (hisSereServ.HEIN_SERVICE_BHYT_ID.Value == GlobalStore.HeinServiceType_Id__SurgMisu)
                        {
                            maNhom = HeinServiceTypeBhyt.ThuThuatPhauThuat;
                        }
                        else if (hisSereServ.HEIN_SERVICE_BHYT_ID.Value == GlobalStore.HeinServiceType_Id__Test)
                        {
                            maNhom = HeinServiceTypeBhyt.XetNghiem;
                        }
                        else if (hisSereServ.HEIN_SERVICE_BHYT_ID.Value == GlobalStore.HeinServiceType_Id__Tran)
                        {
                            maNhom = HeinServiceTypeBhyt.VanChuyen;
                        }
                    }

                    if (hisSereServ.ICD_ID.HasValue && GlobalStore.DicIcd.ContainsKey(hisSereServ.ICD_ID.Value))
                    {
                        maBenh = GlobalStore.DicIcd[hisSereServ.ICD_ID.Value].ICD_CODE;
                    }

                    xml3.MA_LK = _commonQd917.ConverToXmlDocument(!string.IsNullOrEmpty(inputData.V_HIS_HEIN_APPROVAL_BHYT.HEIN_APPROVAL_CODE) ? inputData.V_HIS_HEIN_APPROVAL_BHYT.HEIN_APPROVAL_CODE : "");//lấy mã BHYT làm mã liên kết trong toàn bộ file XML
                    xml3.STT = 1;
                    xml3.MA_DICH_VU = _commonQd917.ConverToXmlDocument(maDichVu);
                    xml3.MA_VAT_TU = _commonQd917.ConverToXmlDocument(maVatTu);
                    xml3.MA_NHOM = _commonQd917.ConverToXmlDocument(maNhom.Value.ToString());
                    xml3.TEN_DICH_VU = _commonQd917.ConverToXmlDocument(!string.IsNullOrEmpty(hisSereServ.HEIN_SERVICE_BHYT_NAME) ? hisSereServ.HEIN_SERVICE_BHYT_NAME : "");
                    xml3.DON_VI_TINH = _commonQd917.ConverToXmlDocument(!string.IsNullOrEmpty(hisSereServ.SERVICE_UNIT_NAME) ? hisSereServ.SERVICE_UNIT_NAME : "");
                    xml3.SO_LUONG = Math.Round(hisSereServ.AMOUNT, 2);
                    xml3.DON_GIA = Math.Round(hisSereServ.ORIGINAL_PRICE, 2);
                    decimal tyle = 0;
                    if (hisSereServ.ORIGINAL_PRICE > 0)
                    {
                        tyle = hisSereServ.HEIN_LIMIT_PRICE.HasValue ? (hisSereServ.HEIN_LIMIT_PRICE.Value / hisSereServ.ORIGINAL_PRICE) * 100 : (hisSereServ.PRICE / hisSereServ.ORIGINAL_PRICE) * 100;
                    }
                    xml3.TYLE_TT = Math.Round(tyle, 0);
                    xml3.THANH_TIEN = Math.Round(xml3.SO_LUONG * xml3.DON_GIA.Value * (xml3.TYLE_TT / 100), 2);
                    if (GlobalStore.DicDepartment.ContainsKey(hisSereServ.REQUEST_DEPARTMENT_ID))
                    {
                        xml3.MA_KHOA = _commonQd917.ConverToXmlDocument(GlobalStore.DicDepartment[hisSereServ.REQUEST_DEPARTMENT_ID].BHYT_CODE ?? "");//mã hóa khoa theo mã khoa chuẩn của BHYT
                    }
                    else
                    {
                        xml3.MA_KHOA = _commonQd917.ConverToXmlDocument("");
                    }
                    xml3.MA_BAC_SI = _commonQd917.ConverToXmlDocument("");//TO DO - chưa có nghiệp vụ quản lý nhân viện bệnh viện
                    xml3.MA_BENH = _commonQd917.ConverToXmlDocument(maBenh);
                    xml3.NGAY_YL = _commonQd917.ConverToXmlDocument(hisSereServ.INTRUCTION_TIME.ToString().Substring(0, 12));
                    xml3.NGAY_KQ = _commonQd917.ConverToXmlDocument(hisSereServ.FINISH_TIME.HasValue
                        ? hisSereServ.FINISH_TIME.ToString().Substring(0, 12)
                        : "");
                    xml3.MA_PTTT = _commonQd917.ConverToXmlDocument("");//TO 
                }
                result = new ResultReturn { Obj = new object[] { listChiTietDvkts, listChiTietVtyts }, Succeeded = true, Result = "" };
            }
            catch (Exception ex)
            {
                result = new ResultReturn { Obj = null, Succeeded = false, Result = ex.Message };
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private long? EncodeHeinServiceType(long value)
        {
            try
            {
                if (value == _config.XetNghiem)
                    return HeinServiceTypeBhyt.XetNghiem;
                if (value == _config.ChuanDoanHinhAnh)
                    return HeinServiceTypeBhyt.ChuanDoanHinhAnh;
                if (value == _config.ThamDoChucNang)
                    return HeinServiceTypeBhyt.ThamDoChucNang;
                if (value == _config.ThuocTrongDanhMucBhyt)
                    return HeinServiceTypeBhyt.ThuocTrongDanhMucBhyt;
                if (value == _config.ThuocDieuTriUngThuChongThaiGhepNgoaiDanhMuc)
                    return HeinServiceTypeBhyt.ThuocDieuTriUngThuChongThaiGhepNgoaiDanhMuc;
                if (value == _config.ThuocThanhToanTheoTyLe)
                    return HeinServiceTypeBhyt.ThuocThanhToanTheoTyLe;
                if (value == _config.MauVaChePhamMau)
                    return HeinServiceTypeBhyt.MauVaChePhamMau;
                if (value == _config.ThuThuatPhauThuat)
                    return HeinServiceTypeBhyt.ThuThuatPhauThuat;
                if (value == _config.DvktThanhToanTheoTyLe)
                    return HeinServiceTypeBhyt.DvktThanhToanTheoTyLe;
                if (value == _config.VatTuTrongDanhMucBhyt)
                    return HeinServiceTypeBhyt.VatTuTrongDanhMucBhyt;
                if (value == _config.VatTuThanhToanTheoTyLe)
                    return HeinServiceTypeBhyt.VatTuThanhToanTheoTyLe;
                if (value == _config.VanChuyen)
                    return HeinServiceTypeBhyt.VanChuyen;
                if (value == _config.KhamBenh)
                    return HeinServiceTypeBhyt.KhamBenh;
                if (value == _config.NgayGiuongNgoaiTru)
                    return HeinServiceTypeBhyt.NgayGiuongNgoaiTru;
                if (value == _config.NgayGiuongNoiTru)
                    return HeinServiceTypeBhyt.NgayGiuongNoiTru;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }
    }
}
