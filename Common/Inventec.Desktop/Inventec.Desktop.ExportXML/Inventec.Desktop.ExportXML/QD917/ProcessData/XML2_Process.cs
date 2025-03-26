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
using Inventec.Desktop.ExportXML.QD917.ADO.XML2_ADO;
using Inventec.Desktop.ExportXML.QD917.Common;

namespace Inventec.Desktop.ExportXML.QD917.ProcessData
{
    public class XML2_Process
    {
        private CommonQD917 _commonQd917 = new CommonQD917();
        //private Config _config;

        public ResultReturn CreateXml2(InputData inputData)
        {
            //this._config = inputData.CONFIG;
            ResultReturn result;
            try
            {
                var chiTietThuoc = ChiTietThuoc(inputData);
                if (chiTietThuoc.Succeeded == false || chiTietThuoc.Obj == null)
                    return chiTietThuoc;
                var dsachChiTietThuoc = new XML2_DSACH_CHI_TIET_THUOC_ADO
                {
                    CHI_TIET_THUOC = chiTietThuoc.Obj as List<XML2_CHI_TIET_THUOC_ADO>
                };
                result = new ResultReturn { Obj = dsachChiTietThuoc, Succeeded = true, Result = "" };
            }
            catch (Exception ex)
            {
                result = new ResultReturn { Obj = null, Succeeded = false, Result = ex.Message };
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public ResultReturn ChiTietThuoc(InputData inputData)
        {
            ResultReturn result;
            try
            {
                var listChiTietThuoc = new List<XML2_CHI_TIET_THUOC_ADO>();
                var listHeinServiceType = new List<long>
                {
                    GlobalStore.HeinServiceType_Id__MedicineCancer,
                    GlobalStore.HeinServiceType_Id__MedicineIn,
                    GlobalStore.HeinServiceType_Id__MedicineOut,
                    GlobalStore.HeinServiceType_Id__MedicineRatio,
                    GlobalStore.HeinServiceType_Id__Blood
                };
                var hisSereServs = inputData.L_V_HIS_SERE_SERV.Where(s => s.HEIN_SERVICE_TYPE_ID.HasValue && !s.IS_CANCEL.HasValue && s.AMOUNT > 0 && s.PRICE > 0 && s.IS_EXPEND != IMSys.DbConfig.HIS_RS.HIS_SERE_SERV.IS_EXPEND__TRUE &&
                    listHeinServiceType.Contains(s.HEIN_SERVICE_TYPE_ID.Value)).ToList();//lấy các dịch vụ là thuốc, vật tư và không phải hao phí
                foreach (var hisSereServ in hisSereServs)
                {
                    string maThuoc = "";
                    long? maNhomThuoc = null;
                    string hamLuong = "";
                    string lieuDung = "";
                    string duongDung = "";
                    string soDangKy = "";
                    if (hisSereServ.HEIN_SERVICE_BHYT_ID.Value == GlobalStore.HeinServiceType_Id__Blood)
                    {
                        maThuoc = hisSereServ.HEIN_SERVICE_BHYT_CODE;
                        maNhomThuoc = HeinServiceTypeBhyt.MauVaChePhamMau;
                    }
                    else
                    {
                        if (hisSereServ.HEIN_SERVICE_BHYT_ID.Value == GlobalStore.HeinServiceType_Id__MedicineCancer)
                        {
                            maNhomThuoc = HeinServiceTypeBhyt.ThuocDieuTriUngThuChongThaiGhepNgoaiDanhMuc;
                        }
                        else if (hisSereServ.HEIN_SERVICE_BHYT_ID.Value == GlobalStore.HeinServiceType_Id__MedicineIn)
                        {
                            maNhomThuoc = HeinServiceTypeBhyt.ThuocTrongDanhMucBhyt;
                        }
                        else if (hisSereServ.HEIN_SERVICE_BHYT_ID.Value == GlobalStore.HeinServiceType_Id__MedicineOut || hisSereServ.HEIN_SERVICE_BHYT_ID.Value == GlobalStore.HeinServiceType_Id__MedicineRatio)
                        {
                            maNhomThuoc = HeinServiceTypeBhyt.ThuocThanhToanTheoTyLe;
                        }
                        if (GlobalStore.DicMedicineType.ContainsKey(hisSereServ.SERVICE_ID))
                        {
                            var medicine = GlobalStore.DicMedicineType[hisSereServ.SERVICE_ID];
                            if (medicine != null)
                            {
                                maThuoc = medicine.ACTIVE_INGR_BHYT_CODE ?? "";
                                hamLuong = medicine.CONCENTRA ?? "";
                                duongDung = medicine.MEDICINE_USE_FORM_NAME ?? "";
                                soDangKy = medicine.REGISTER_NUMBER ?? "";
                                lieuDung = medicine.TUTORIAL ?? "";
                            }
                        }

                    }

                    var xml2 = new XML2_CHI_TIET_THUOC_ADO();
                    xml2.MA_LK = _commonQd917.ConverToXmlDocument(!string.IsNullOrEmpty(inputData.V_HIS_HEIN_APPROVAL_BHYT.HEIN_APPROVAL_CODE) ? inputData.V_HIS_HEIN_APPROVAL_BHYT.HEIN_APPROVAL_CODE : "");//lấy mã BHYT làm mã liên kết trong toàn bộ file XML
                    xml2.STT = 1;
                    xml2.MA_THUOC = _commonQd917.ConverToXmlDocument(!string.IsNullOrEmpty(maThuoc) ? maThuoc : "");
                    xml2.MA_NHOM = _commonQd917.ConverToXmlDocument(maNhomThuoc.HasValue ? maNhomThuoc.Value.ToString() : "");
                    xml2.TEN_THUOC = _commonQd917.ConverToXmlDocument(!string.IsNullOrEmpty(hisSereServ.HEIN_SERVICE_BHYT_NAME) ? hisSereServ.HEIN_SERVICE_BHYT_NAME : "");
                    xml2.DON_VI_TINH = _commonQd917.ConverToXmlDocument(!string.IsNullOrEmpty(hisSereServ.SERVICE_UNIT_NAME) ? hisSereServ.SERVICE_UNIT_NAME : "");
                    xml2.HAM_LUONG = _commonQd917.ConverToXmlDocument(hamLuong);
                    xml2.DUONG_DUNG = _commonQd917.ConverToXmlDocument(duongDung);
                    xml2.LIEU_DUNG = _commonQd917.ConverToXmlDocument(lieuDung);
                    xml2.SO_DANG_KY = _commonQd917.ConverToXmlDocument(soDangKy);
                    xml2.SO_LUONG = Math.Round(hisSereServ.AMOUNT, 2);
                    xml2.DON_GIA = Math.Round(hisSereServ.ORIGINAL_PRICE, 2);
                    xml2.TYLE_TT = Math.Round(hisSereServ.ORIGINAL_PRICE > 0 ? (hisSereServ.HEIN_LIMIT_PRICE.HasValue ? (hisSereServ.HEIN_LIMIT_PRICE.Value / hisSereServ.ORIGINAL_PRICE) * 100 : (hisSereServ.PRICE / hisSereServ.ORIGINAL_PRICE) * 100) : 0, 0);
                    xml2.THANH_TIEN = Math.Round(xml2.SO_LUONG * xml2.DON_GIA.Value * (xml2.TYLE_TT.Value / 100), 2);
                    if (GlobalStore.DicDepartment.ContainsKey(hisSereServ.REQUEST_DEPARTMENT_ID))
                    {
                        xml2.MA_KHOA = _commonQd917.ConverToXmlDocument(GlobalStore.DicDepartment[hisSereServ.REQUEST_DEPARTMENT_ID].BHYT_CODE ?? "");//mã hóa khoa theo mã khoa chuẩn của BHYT
                    }
                    else
                    {
                        xml2.MA_KHOA = _commonQd917.ConverToXmlDocument("");
                    }
                    xml2.MA_BAC_SI = _commonQd917.ConverToXmlDocument("");//TO DO - phầm mềm chưa quản lý
                    if (hisSereServ.ICD_ID.HasValue)
                    {
                        if (GlobalStore.DicIcd.ContainsKey(hisSereServ.ICD_ID.Value))
                        {
                            xml2.MA_BENH = _commonQd917.ConverToXmlDocument(GlobalStore.DicIcd[hisSereServ.ICD_ID.Value].ICD_CODE);
                        }
                        else
                        {
                            xml2.MA_BENH = _commonQd917.ConverToXmlDocument("");
                        }
                    }
                    else
                    {
                        xml2.MA_BENH = _commonQd917.ConverToXmlDocument("");
                    }
                    xml2.NGAY_YL = _commonQd917.ConverToXmlDocument(hisSereServ.INTRUCTION_TIME.ToString().Substring(0, 12));
                    xml2.MA_PTTT = _commonQd917.ConverToXmlDocument("");//TO DO - phần mềm chưa có nghiệp vụ
                    listChiTietThuoc.Add(xml2);
                }
                if (listChiTietThuoc == null)
                    return new ResultReturn { Obj = null, Succeeded = false, Result = "XML2: Danh sách chi tiết thuốc trống" };
                result = new ResultReturn { Obj = listChiTietThuoc, Succeeded = true, Result = "" };
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
