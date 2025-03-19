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
using Inventec.Desktop.ExportXML.QD917.ADO.XML1_ADO;
using Inventec.Desktop.ExportXML.QD917.Common;
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.ExportXML.QD917.ADO.XML2_ADO;
using Inventec.Desktop.ExportXML.QD917.ADO.XML3_ADO;

namespace Inventec.Desktop.ExportXML.QD917.ProcessData
{
    public class XML1_Process
    {
        private CommonQD917 _commonQd917 = new CommonQD917();
        //private Config _config;

        internal ResultReturn CreateXml1(InputData inputData, List<XML2_CHI_TIET_THUOC_ADO> listXml2, List<XML3_CHI_TIET_DVKT_ADO> listXml3VatTu)
        {
            //this._config = inputData.CONFIG;
            ResultReturn result;
            try
            {
                XML1_ADO xml1 = new XML1_ADO();
                if (inputData == null)
                {
                    return new ResultReturn { Obj = null, Succeeded = false, Result = "XML1: Input Data = null" };
                }
                if (inputData.V_HIS_HEIN_APPROVAL_BHYT == null)
                {
                    return new ResultReturn { Obj = null, Succeeded = false, Result = "XML1: HeinApprovalBhyt = null" };
                }
                if (inputData.V_HIS_TREATMENT == null)
                {
                    return new ResultReturn { Obj = null, Succeeded = false, Result = "XML1: Treatment = null" };
                }
                if (inputData.L_V_HIS_SERE_SERV == null || inputData.L_V_HIS_SERE_SERV.Count == 0)
                {
                    return new ResultReturn { Obj = null, Succeeded = false, Result = "XML1: ListSereServ = null or empty" };
                }

                if (String.IsNullOrEmpty(inputData.V_HIS_HEIN_APPROVAL_BHYT.RIGHT_ROUTE_CODE))
                {
                    return new ResultReturn { Obj = null, Succeeded = false, Result = "XML1: Khong Co RightRouteCode" };
                }

                if (!inputData.V_HIS_TREATMENT.TREATMENT_RESULT_ID.HasValue)
                {
                    return new ResultReturn { Obj = null, Succeeded = false, Result = "TreatmentResultId = null" };
                }

                if (!inputData.V_HIS_TREATMENT.TREATMENT_END_TYPE_ID.HasValue)
                {
                    return new ResultReturn { Obj = null, Succeeded = false, Result = "TreatmentEndTypeId=null" };
                }

                if (inputData.V_HIS_HEIN_APPROVAL_BHYT.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    xml1.MA_LYDO_VVIEN = 1;
                else if (inputData.V_HIS_HEIN_APPROVAL_BHYT.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)
                    xml1.MA_LYDO_VVIEN = 2;
                else if (inputData.V_HIS_HEIN_APPROVAL_BHYT.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)
                    xml1.MA_LYDO_VVIEN = 3;
                //chuyển đổi mã tai nạn thương thích theo quy định BHYT
                if (inputData.V_HIS_ACCIDENT_HURT != null)
                {
                    if (inputData.V_HIS_ACCIDENT_HURT.ACCIDENT_HURT_TYPE_ID == GlobalStore.AccidentHurtType_Id__Traffic)
                        xml1.MA_TAI_NAN = AccidentHurtCodeBhyt.TaiNanGiaoThong;
                    else if (inputData.V_HIS_ACCIDENT_HURT.ACCIDENT_HURT_TYPE_ID == GlobalStore.AccidentHurtType_Id__Labor)
                        xml1.MA_TAI_NAN = AccidentHurtCodeBhyt.TaiNanLaoDong;
                    else if (inputData.V_HIS_ACCIDENT_HURT.ACCIDENT_HURT_TYPE_ID == GlobalStore.AccidentHurtType_Id__Underwater)
                        xml1.MA_TAI_NAN = AccidentHurtCodeBhyt.DuoiNuoc;
                    else if (inputData.V_HIS_ACCIDENT_HURT.ACCIDENT_HURT_TYPE_ID == GlobalStore.AccidentHurtType_Id__Burn)
                        xml1.MA_TAI_NAN = AccidentHurtCodeBhyt.Bong;
                    else if (inputData.V_HIS_ACCIDENT_HURT.ACCIDENT_HURT_TYPE_ID == GlobalStore.AccidentHurtType_Id__Violence)
                        xml1.MA_TAI_NAN = AccidentHurtCodeBhyt.BaoLucGiaDinhXaHoi;
                    else if (inputData.V_HIS_ACCIDENT_HURT.ACCIDENT_HURT_TYPE_ID == GlobalStore.AccidentHurtType_Id__Suicidal)
                        xml1.MA_TAI_NAN = AccidentHurtCodeBhyt.TuTu;
                    else if (inputData.V_HIS_ACCIDENT_HURT.ACCIDENT_HURT_TYPE_ID == GlobalStore.AccidentHurtType_Id__Poisoning)
                        xml1.MA_TAI_NAN = AccidentHurtCodeBhyt.NgoDoc;
                    else if (inputData.V_HIS_ACCIDENT_HURT.ACCIDENT_HURT_TYPE_ID == GlobalStore.AccidentHurtType_Id__Animal || inputData.V_HIS_ACCIDENT_HURT.ACCIDENT_HURT_TYPE_ID == GlobalStore.AccidentHurtType_Id__Fall || inputData.V_HIS_ACCIDENT_HURT.ACCIDENT_HURT_TYPE_ID == GlobalStore.AccidentHurtType_Id__Life || inputData.V_HIS_ACCIDENT_HURT.ACCIDENT_HURT_TYPE_ID == GlobalStore.AccidentHurtType_Id__Poisoning || inputData.V_HIS_ACCIDENT_HURT.ACCIDENT_HURT_TYPE_ID == GlobalStore.AccidentHurtType_Id__Other)
                        xml1.MA_TAI_NAN = AccidentHurtCodeBhyt.TaiNanKhac;
                }
                //tính số ngày điều trị của bệnh nhân lấy thời gian ra viện - thời gian nhập viện điều trị nội trú
                //mã hóa kết quả điều trị theo BHYT
                if (inputData.V_HIS_TREATMENT.CLINICAL_IN_TIME.HasValue && inputData.V_HIS_TREATMENT.OUT_TIME.HasValue)
                {
                    xml1.SO_NGAY_DTRI = (int)HIS.Treatment.DateTime.Calculation.DayOfTreatment(inputData.V_HIS_TREATMENT.CLINICAL_IN_TIME.Value, inputData.V_HIS_TREATMENT.OUT_TIME.Value);
                }
                //Kết quả điều trị
                if (inputData.V_HIS_TREATMENT.TREATMENT_RESULT_ID == GlobalStore.TreatmentResult_Id__Cured)
                    xml1.KET_QUA_DTRI = TreatmentResultBhyt.Khoi;
                else if (inputData.V_HIS_TREATMENT.TREATMENT_RESULT_ID == GlobalStore.TreatmentResult_Id__Reduce)
                    xml1.KET_QUA_DTRI = TreatmentResultBhyt.Do;
                else if (inputData.V_HIS_TREATMENT.TREATMENT_RESULT_ID == GlobalStore.TreatmentResult_Id__Constant)
                    xml1.KET_QUA_DTRI = TreatmentResultBhyt.KhongThayDoi;
                else if (inputData.V_HIS_TREATMENT.TREATMENT_RESULT_ID == GlobalStore.TreatmentResult_Id__Heavier)
                    xml1.KET_QUA_DTRI = TreatmentResultBhyt.NangHon;
                else if (inputData.V_HIS_TREATMENT.TREATMENT_RESULT_ID == GlobalStore.TreatmentResult_Id__Death)
                    xml1.KET_QUA_DTRI = TreatmentResultBhyt.TuVong;

                //tình trạng ra viện
                if (inputData.V_HIS_TREATMENT.TREATMENT_END_TYPE_ID == GlobalStore.TreatmentEndType_Id__ChuyenVien)
                    xml1.TINH_TRANG_RV = TreatmentEndTypeBhyt.ChuyenVien;
                else if (inputData.V_HIS_TREATMENT.TREATMENT_END_TYPE_ID == GlobalStore.TreatmentEndType_Id__TronVien)
                    xml1.TINH_TRANG_RV = TreatmentEndTypeBhyt.TronVien;
                else if (inputData.V_HIS_TREATMENT.TREATMENT_END_TYPE_ID == GlobalStore.TreatmentEndType_Id__XinRaVien)
                    xml1.TINH_TRANG_RV = TreatmentEndTypeBhyt.XinRaVien;
                else
                    xml1.TINH_TRANG_RV = TreatmentEndTypeBhyt.RaVien;
                //tính mức hưởng BHYT
                xml1.MUC_HUONG = GetDefaultHeinRatioForView(inputData.V_HIS_HEIN_APPROVAL_BHYT.HEIN_CARD_NUMBER, inputData.V_HIS_HEIN_APPROVAL_BHYT.HEIN_TREATMENT_TYPE_CODE, inputData.V_HIS_HEIN_APPROVAL_BHYT.LEVEL_CODE, inputData.V_HIS_HEIN_APPROVAL_BHYT.RIGHT_ROUTE_CODE);
                //tính tiền thuốc
                decimal tientThuoc = 0;
                if (listXml2 != null && listXml2.Count > 0)
                {
                    foreach (var xml2 in listXml2)
                    {
                        if (xml2.THANH_TIEN.HasValue)
                            tientThuoc += Math.Round(xml2.THANH_TIEN.Value, 0);
                    }
                }
                xml1.T_THUOC = Math.Round(tientThuoc, 0);
                //tính tiền vật tư y tế
                var tienVatTuYTe = inputData.L_V_HIS_SERE_SERV.Where(s => s.MATERIAL_ID.HasValue && !s.IS_CANCEL.HasValue).Sum(s => s.HEIN_PRICE);
                decimal tientVTYT = 0;
                if (listXml3VatTu != null && listXml3VatTu.Count > 0)
                {
                    foreach (var xml3 in listXml3VatTu)
                    {
                        if (xml3.THANH_TIEN.HasValue)
                            tientVTYT += Math.Round(xml3.THANH_TIEN.Value, 0);
                    }
                }
                xml1.T_VTYT = Math.Round(tientVTYT, 0);

                //tính tổng chi của bệnh nhân
                xml1.T_TONGCHI = Math.Round(inputData.L_V_HIS_SERE_SERV.Sum(s => s.VIR_TOTAL_PRICE ?? 0));
                //tính số tiền bệnh nhân phải thanh toán
                xml1.T_BNTT = Math.Round(inputData.L_V_HIS_SERE_SERV.Sum(s => s.VIR_TOTAL_PATIENT_PRICE ?? 0));
                var bhytTra = Math.Round(inputData.L_V_HIS_SERE_SERV.Sum(s => s.VIR_TOTAL_HEIN_PRICE ?? 0));
                //kiểm tra có phải chi phí ngoài định suất (nếu là định suất thì số tiền = T_BHTT và T_BNTT = 0, ngược lại T_NGOAIDS = 0)(kiểm tra trên mã bệnh chính)
                if (CheckBhytNsd(GlobalStore.ListIcdCode_Nds, GlobalStore.ListIcdCode_Nds_Te,
                    inputData.V_HIS_TREATMENT, inputData.V_HIS_HEIN_APPROVAL_BHYT))
                {
                    xml1.T_NGOAIDS = Math.Round(bhytTra, 0);
                }
                else
                {
                    xml1.T_BHTT = Math.Round(bhytTra, 0);
                }
                //thời gian đề nghị thanh toán (theo thời gian duyệt BHYT)
                if (inputData.V_HIS_HEIN_APPROVAL_BHYT.EXECUTE_TIME.HasValue)
                {
                    xml1.NGAY_TTOAN = _commonQd917.ConverToXmlDocument(inputData.V_HIS_HEIN_APPROVAL_BHYT.EXECUTE_TIME.Value.ToString().Substring(0, 12));
                    xml1.NAM_QT = Convert.ToInt32(inputData.V_HIS_HEIN_APPROVAL_BHYT.EXECUTE_TIME.Value.ToString().Substring(0, 4));
                    xml1.THANG_QT = Convert.ToInt32(inputData.V_HIS_HEIN_APPROVAL_BHYT.EXECUTE_TIME.Value.ToString().Substring(4, 2));
                }
                //Chuyển đổi hình thức khám chữa bệnh theo BHYT
                if (inputData.V_HIS_HEIN_APPROVAL_BHYT.TREATMENT_TYPE_ID == GlobalStore.TreatmentType_Id__Exam)
                    xml1.MA_LOAI_KCB = TreatmentTypeBhyt.Kham;
                else if (inputData.V_HIS_HEIN_APPROVAL_BHYT.TREATMENT_TYPE_ID == GlobalStore.TreatmentType_Id__TreatOut)
                    xml1.MA_LOAI_KCB = TreatmentTypeBhyt.NgoaiTru;
                else if (inputData.V_HIS_HEIN_APPROVAL_BHYT.TREATMENT_TYPE_ID == GlobalStore.TreatmentType_Id__TreatIn)
                    xml1.MA_LOAI_KCB = TreatmentTypeBhyt.NoiTru;

                ////mã phẫu thuật thủ thuật quốc thế
                var listIcdIds = inputData.L_V_HIS_SERE_SERV.Where(s => s.HEIN_SERVICE_TYPE_ID == GlobalStore.HeinServiceType_Id__SurgMisu).Select(s => s.ICD_ID).ToList();
                if (listIcdIds != null && listIcdIds.Count > 0)
                {
                    var listIcdCode = GlobalStore.ListIcd.Where(s => listIcdIds.Contains(s.ID)).Select(s => s.ICD_CODE).ToArray();
                    xml1.MA_PTTT_QT = _commonQd917.ConverToXmlDocument(string.Join(";", listIcdCode));
                }

                //lấy cân nặng với trường hợp là trẻ em dưới 1 tuổi
                var tinhTuoi = Inventec.Common.DateTime.Calculation.Age(inputData.V_HIS_TREATMENT.DOB);
                if (tinhTuoi == 0 && inputData.HIS_DHST != null)
                {
                    xml1.CAN_NANG = inputData.HIS_DHST.WEIGHT;
                }
                //----------------------------------------------------------------------------------------------------------------------------------------------------
                //var xml1 = new XML1_ADO
                //{
                xml1.MA_LK = _commonQd917.ConverToXmlDocument(inputData.V_HIS_HEIN_APPROVAL_BHYT.HEIN_APPROVAL_CODE);//lấy mã BHYT làm mã liên kết trong toàn bộ file XML
                xml1.STT = 1;//mỗi lần chỉ xuất 1 thẻ BHYT của 1 bệnh nhân nên số lượng luôn là 1
                xml1.MA_BN = _commonQd917.ConverToXmlDocument(inputData.V_HIS_TREATMENT.PATIENT_CODE);
                xml1.HO_TEN = _commonQd917.ConverToXmlDocument(inputData.V_HIS_TREATMENT.VIR_PATIENT_NAME.ToLower());
                xml1.NGAY_SINH = _commonQd917.ConverToXmlDocument(inputData.V_HIS_TREATMENT.DOB.ToString().Substring(2, 8));
                xml1.GIOI_TINH = inputData.V_HIS_TREATMENT.GENDER_ID == GlobalStore.Gender_Id__Female ? 1 : 2;//mã hóa theo yêu câu BHYT (1 nam, 2 nữ)
                xml1.DIA_CHI = _commonQd917.ConverToXmlDocument(!String.IsNullOrEmpty(inputData.V_HIS_HEIN_APPROVAL_BHYT.ADDRESS) ? inputData.V_HIS_HEIN_APPROVAL_BHYT.ADDRESS : "");
                xml1.MA_THE = _commonQd917.ConverToXmlDocument(!string.IsNullOrEmpty(inputData.V_HIS_HEIN_APPROVAL_BHYT.HEIN_CARD_NUMBER) ? inputData.V_HIS_HEIN_APPROVAL_BHYT.HEIN_CARD_NUMBER : "");
                xml1.MA_DKBD = _commonQd917.ConverToXmlDocument(!string.IsNullOrEmpty(inputData.V_HIS_HEIN_APPROVAL_BHYT.HEIN_MEDI_ORG_CODE) ? inputData.V_HIS_HEIN_APPROVAL_BHYT.HEIN_MEDI_ORG_CODE : "");
                xml1.GT_THE_TU = _commonQd917.ConverToXmlDocument(inputData.V_HIS_HEIN_APPROVAL_BHYT.HEIN_CARD_FROM_TIME.ToString().Substring(0, 8));//thời điển thẻ BHYT bắt đầu có giá trị
                xml1.GT_THE_DEN = _commonQd917.ConverToXmlDocument(inputData.V_HIS_HEIN_APPROVAL_BHYT.HEIN_CARD_TO_TIME.ToString().Substring(0, 8));//thời điểm thẻ BHYT hết giá trị sử dụng
                xml1.TEN_BENH = _commonQd917.ConverToXmlDocument(!string.IsNullOrEmpty(inputData.V_HIS_TREATMENT.ICD_NAME) ? inputData.V_HIS_TREATMENT.ICD_NAME : "");//tên bệnh theo bảng ICD 10
                xml1.MA_BENH = _commonQd917.ConverToXmlDocument(!string.IsNullOrEmpty(inputData.V_HIS_TREATMENT.ICD_CODE) ? inputData.V_HIS_TREATMENT.ICD_CODE : "");//mã bệnh theo bảng ICD 10
                xml1.MA_BENHKHAC = _commonQd917.ConverToXmlDocument("");//để trống vì chưa quản lý mã bệnh phụ kèm theo
                xml1.MA_NOI_CHUYEN = _commonQd917.ConverToXmlDocument(!string.IsNullOrEmpty(inputData.V_HIS_TRAN_PATI.MEDI_ORG_CODE) ? inputData.V_HIS_TRAN_PATI.MEDI_ORG_CODE : "");
                if (inputData.V_HIS_TREATMENT.CLINICAL_IN_TIME.HasValue)
                {
                    xml1.NGAY_VAO = _commonQd917.ConverToXmlDocument(inputData.V_HIS_TREATMENT.CLINICAL_IN_TIME.Value.ToString().Substring(0, 12));
                }
                else
                {
                    xml1.NGAY_VAO = _commonQd917.ConverToXmlDocument(inputData.V_HIS_TREATMENT.IN_TIME.ToString().Substring(0, 12));
                }

                if (inputData.V_HIS_TREATMENT.OUT_TIME.HasValue)
                {
                    xml1.NGAY_RA = _commonQd917.ConverToXmlDocument(inputData.V_HIS_TREATMENT.OUT_TIME.Value.ToString().Substring(0, 12));
                }
                string maKhoa = "";
                if (inputData.V_HIS_TREATMENT.END_DEPARTMENT_ID.HasValue)
                {
                    if (GlobalStore.DicDepartment.ContainsKey(inputData.V_HIS_TREATMENT.END_DEPARTMENT_ID.Value))
                    {
                        maKhoa = GlobalStore.DicDepartment[inputData.V_HIS_TREATMENT.END_DEPARTMENT_ID.Value].BHYT_CODE;
                    }
                }
                xml1.MA_KHOA = _commonQd917.ConverToXmlDocument(maKhoa);
                string maKcbdb = "";
                if (GlobalStore.Branch != null && !String.IsNullOrEmpty(GlobalStore.Branch.HEIN_MEDI_ORG_CODE))
                {
                    maKcbdb = GlobalStore.Branch.HEIN_MEDI_ORG_CODE;
                }
                xml1.MA_CSKCB = _commonQd917.ConverToXmlDocument(GlobalStore.Branch.HEIN_MEDI_ORG_CODE);
                xml1.MA_KHUVUC = _commonQd917.ConverToXmlDocument(inputData.V_HIS_HEIN_APPROVAL_BHYT.LIVE_AREA_CODE ?? "");
                if (xml1 == null)
                    return new ResultReturn { Obj = null, Succeeded = false, Result = "Không tạo được XML1_ADO" };
                result = new ResultReturn { Obj = xml1, Succeeded = true, Result = "" };
            }
            catch (Exception ex)
            {
                result = new ResultReturn { Obj = null, Succeeded = false, Result = ex.Message };
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        //tính mức hưởng BHYT
        private static int GetDefaultHeinRatioForView(string heinCardNumber, string treatmentTypeCode, string levelCode, string rightRouteCode)
        {
            decimal result = 0;
            try
            {
                result = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(treatmentTypeCode, heinCardNumber, levelCode, rightRouteCode) ?? 0) * 100);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return (int)result;
        }

        //kiểm tra chi phí ngoài định suất
        private static bool CheckBhytNsd(List<string> listIcdCode, List<string> listIcdCodeTe, V_HIS_TREATMENT hisTreatment, V_HIS_HEIN_APPROVAL_BHYT hisHeinApprovalBhyt)
        {
            var result = false;
            try
            {
                if (listIcdCode.Contains(hisTreatment.ICD_CODE))
                    result = true;
                else if (!string.IsNullOrEmpty(hisTreatment.ICD_CODE))
                    if (hisHeinApprovalBhyt.HEIN_CARD_NUMBER.Substring(0, 2).Equals("TE") && listIcdCodeTe.Contains(hisTreatment.ICD_CODE.Substring(0, 3)))
                        result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
