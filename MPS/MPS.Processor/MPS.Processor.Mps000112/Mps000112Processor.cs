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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000112.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MPS.Processor.Mps000112
{
    public class Mps000112Processor : AbstractProcessor
    {
        Mps000112PDO rdo;
        List<Mps000112ServiceADO> serviceADOs = new List<Mps000112ServiceADO>();
        List<Mps000112PayformADO> payformADOs = new List<Mps000112PayformADO>();
        List<Mps000112DiscountADO> discountADOs = new List<Mps000112DiscountADO>();
        public Mps000112Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000112PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                SetSingleKey();
                SetSereServRequestKey();
                SetDepositReqKey();
                SetBarcodeKey();
                ProcessServiceADO();
                ProcessPayformADO();
                ProcessDiscountADO();

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                barCodeTag.ProcessData(store, dicImage);
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                objectTag.AddObjectData(store, Mps000112ExtendSingleKey.OBJECT_TAG__SERVICE, serviceADOs);
                objectTag.AddObjectData(store, Mps000112ExtendSingleKey.OBJECT_TAG__PAYFORM, payformADOs);
                objectTag.AddObjectData(store, Mps000112ExtendSingleKey.OBJECT_TAG__DISCOUNT, discountADOs);
                singleTag.ProcessData(store, singleValueDictionary);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void SetSingleKey()
        {
            try
            {
                if (rdo._Transaction != null)
                {
                    SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo._Transaction.TDL_PATIENT_DOB ?? 0)));

                    string temp = rdo._Transaction.TDL_PATIENT_DOB.ToString();
                    if (temp != null && temp.Length >= 8)
                    {
                        SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.YEAR_STR, temp.Substring(0, 4)));
                    }
                    SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.AGE_STR, AgeUtil.CalculateFullAge(rdo._Transaction.TDL_PATIENT_DOB ?? 0)));

                    SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.AMOUNT, Inventec.Common.Number.Convert.NumberToNumberRoundMax4(rdo._Transaction.AMOUNT)));
                    string amountStr = string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(rdo._Transaction.AMOUNT));
                    string amountText = Inventec.Common.String.Convert.CurrencyToVneseString(amountStr);
                    SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.AMOUNT_TEXT, amountText));
                    SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.AMOUNT_TEXT_UPPER_FIRST, amountText));
                    SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.CREATE_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo._Transaction.CREATE_TIME ?? 0)));

                    AddObjectKeyIntoListkey<V_HIS_TRANSACTION>(rdo._Transaction, false);
                }

                if (rdo._Patient != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo._Patient, false);
                }
                if (rdo._PatyalterBHYT != null)
                {
                    SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.HEIN_CARD_ADDRESS, rdo._PatyalterBHYT.ADDRESS));
                    AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo._PatyalterBHYT, false);
                }
                if (rdo._DepartmentTranAllByTreatment != null)
                {
                    var requestDepartment = rdo._DepartmentTranAllByTreatment
                        .Where(o => o.DEPARTMENT_IN_TIME.HasValue && o.DEPARTMENT_IN_TIME <= rdo._Transaction.TRANSACTION_TIME)
                        .OrderByDescending(o => o.DEPARTMENT_IN_TIME)
                        .ThenByDescending(o => o.ID).FirstOrDefault();
                    if (requestDepartment != null)
                    {
                        SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.REQUEST_DEPARTMENT_CODE, requestDepartment.DEPARTMENT_CODE));
                        SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.REQUEST_DEPARTMENT_NAME, requestDepartment.DEPARTMENT_NAME));
                    }

                    var currentDepartment = rdo._DepartmentTranAllByTreatment
                        .Where(o => o.CREATE_TIME <= rdo._Transaction.TRANSACTION_TIME)
                        .OrderByDescending(o => o.CREATE_TIME)
                        .ThenByDescending(o => o.ID).FirstOrDefault();
                    if (currentDepartment != null)
                    {
                        SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.CURRENT_DEPARTMENT_CODE, currentDepartment.DEPARTMENT_CODE));
                        SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.CURRENT_DEPARTMENT_NAME, currentDepartment.DEPARTMENT_NAME));
                        SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.CURRENT_PREVIOUS_DEPARTMENT_CODE, currentDepartment.PREVIOUS_DEPARTMENT_CODE));
                        SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.CURRENT_PREVIOS_DEPARTMENT_NAME, currentDepartment.PREVIOUS_DEPARTMENT_NAME));
                    }

                    //#31161
                    var nextDepartment = rdo._DepartmentTranAllByTreatment
                        .Where(o => !o.DEPARTMENT_IN_TIME.HasValue)
                        .OrderByDescending(p => p.CREATE_TIME)
                        .FirstOrDefault();
                    if (nextDepartment != null)
                    {
                        SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.NEXT_DEPARTMENT_CODE, nextDepartment.DEPARTMENT_CODE));
                        SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.NEXT_DEPARTMENT_NAME, nextDepartment.DEPARTMENT_NAME));
                    }
                }
                SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.RATIO, rdo.ratio));
                SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.RATIO_STR, (rdo.ratio * 100) + "%"));

                SetPayFormKey();

                if (rdo.MpsADO != null)
                {
                    AddObjectKeyIntoListkey<Mps000112ADO>(rdo.MpsADO, false);
                }

                if (rdo.treatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.PATIENT_CLASSIFY_NAME, rdo.treatment.TDL_PATIENT_CLASSIFY_NAME));
                    AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.treatment, false);
                    if (rdo._TreatmentType != null && rdo._TreatmentType.Count > 0)
                    {
                        if (rdo.treatment.IN_TREATMENT_TYPE_ID.HasValue && rdo._TreatmentType.First(o => o.ID == rdo.treatment.IN_TREATMENT_TYPE_ID) != null) {
                            string NameType = rdo._TreatmentType.First(o => o.ID == rdo.treatment.IN_TREATMENT_TYPE_ID).TREATMENT_TYPE_NAME;
                            SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.IN_TREATMENT_TYPE_NAME, NameType));
						}
                        else if (rdo.treatment.TDL_TREATMENT_TYPE_ID.HasValue && rdo._TreatmentType.First(o => o.ID == rdo.treatment.TDL_TREATMENT_TYPE_ID) != null)
                        {
                            string NameType = rdo._TreatmentType.First(o => o.ID == rdo.treatment.TDL_TREATMENT_TYPE_ID).TREATMENT_TYPE_NAME;
                            SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.IN_TREATMENT_TYPE_NAME, NameType));
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Khoa/phong YEU CAU (chi dinh) lay tu danh sach dich vu cua phieu (V_HIS_SERE_SERV).
        /// Cach lay giong ban in Mps000446 - Yeu cau thanh toan: chi set khi tat ca dich vu cung
        /// mot khoa/phong yeu cau, neu khac nhau thi de trong.
        /// Khac voi REQUEST_DEPARTMENT_* (lay tu HIS_DEPARTMENT_TRAN = khoa benh nhan dang nam
        /// tai thoi diem giao dich) - giu nguyen key cu de khong pha cac mau in dang dung.
        /// </summary>
        private void SetSereServRequestKey()
        {
            try
            {
                if (rdo._ListSereServ == null || rdo._ListSereServ.Count == 0)
                    return;

                var sereServs = rdo._ListSereServ.Where(o => o.IS_DELETE == 0).ToList();
                if (sereServs.Count == 0)
                    return;

                var departmentIds = sereServs.Select(o => o.TDL_REQUEST_DEPARTMENT_ID).Distinct().ToList();
                if (departmentIds.Count == 1)
                {
                    var sereServ = sereServs.First();
                    SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.SERE_SERV_REQUEST_DEPARTMENT_CODE, sereServ.REQUEST_DEPARTMENT_CODE));
                    SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.SERE_SERV_REQUEST_DEPARTMENT_NAME, sereServ.REQUEST_DEPARTMENT_NAME));
                }

                var roomIds = sereServs.Select(o => o.TDL_REQUEST_ROOM_ID).Distinct().ToList();
                if (roomIds.Count == 1)
                {
                    var sereServ = sereServs.First();
                    SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.SERE_SERV_REQUEST_ROOM_CODE, sereServ.REQUEST_ROOM_CODE));
                    SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.SERE_SERV_REQUEST_ROOM_NAME, sereServ.REQUEST_ROOM_NAME));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Khoa/phong YEU CAU lay tu YEU CAU TAM UNG (V_HIS_DEPOSIT_REQ) gan voi giao dich nay.
        /// Day chinh la nguon ma cot "Khoa yeu cau" tren man hinh Yeu cau tam ung dang hien thi
        /// (DEPARTMENT_NAME <- REQUEST_DEPARTMENT_ID, backend suy khoa tu phong tao phieu).
        /// Chi set khi tat ca yeu cau tam ung cua phieu thu cung mot khoa/phong, khac nhau thi de trong.
        /// </summary>
        private void SetDepositReqKey()
        {
            try
            {
                if (rdo._ListDepositReq == null || rdo._ListDepositReq.Count == 0)
                    return;

                var depositReqs = rdo._ListDepositReq.Where(o => o.IS_DELETE == 0).ToList();
                if (depositReqs.Count == 0)
                    return;

                var first = depositReqs.First();

                if (depositReqs.Select(o => o.DEPOSIT_REQ_CODE).Distinct().Count() == 1)
                {
                    SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.DEPOSIT_REQ_CODE, first.DEPOSIT_REQ_CODE));
                    SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.DEPOSIT_REQ_REQUEST_USERNAME, first.REQUEST_USERNAME));
                }

                if (depositReqs.Select(o => o.REQUEST_DEPARTMENT_ID).Distinct().Count() == 1)
                {
                    SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.DEPOSIT_REQ_DEPARTMENT_CODE, first.DEPARTMENT_CODE));
                    SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.DEPOSIT_REQ_DEPARTMENT_NAME, first.DEPARTMENT_NAME));
                }

                if (depositReqs.Select(o => o.REQUEST_ROOM_ID).Distinct().Count() == 1)
                {
                    SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.DEPOSIT_REQ_ROOM_CODE, first.ROOM_CODE));
                    SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.DEPOSIT_REQ_ROOM_NAME, first.ROOM_NAME));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Hinh thuc thanh toan cua giao dich.
        /// View giao dich da co san PAY_FORM_CODE/PAY_FORM_NAME, chi tra cuu danh muc khi view khong co du lieu.
        /// </summary>
        private void SetPayFormKey()
        {
            try
            {
                if (rdo._Transaction == null) return;

                string payFormCode = rdo._Transaction.PAY_FORM_CODE;
                string payFormName = rdo._Transaction.PAY_FORM_NAME;

                if ((String.IsNullOrWhiteSpace(payFormCode) || String.IsNullOrWhiteSpace(payFormName))
                    && rdo._PayForms != null && rdo._PayForms.Count > 0)
                {
                    var payForm = rdo._PayForms.FirstOrDefault(o => o.ID == rdo._Transaction.PAY_FORM_ID);
                    if (payForm != null)
                    {
                        if (String.IsNullOrWhiteSpace(payFormCode)) payFormCode = payForm.PAY_FORM_CODE;
                        if (String.IsNullOrWhiteSpace(payFormName)) payFormName = payForm.PAY_FORM_NAME;
                    }
                }

                SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.PAY_FORM_CODE, payFormCode));
                SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.PAY_FORM_NAME, payFormName));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Danh sach dich vu duoc tam ung boi phieu thu nay + cac key tong hop
        /// </summary>
        private void ProcessServiceADO()
        {
            try
            {
                if (rdo._ListSereServ != null && rdo._ListSereServ.Count > 0)
                {
                    long numOrder = 1;
                    foreach (var sereServ in rdo._ListSereServ.OrderBy(o => o.TDL_SERVICE_TYPE_ID).ThenBy(o => o.TDL_SERVICE_NAME))
                    {
                        Mps000112ServiceADO ado = new Mps000112ServiceADO(sereServ);
                        ado.NUM_ORDER = numOrder++;

                        if (rdo._ListSereServDeposit != null && rdo._ListSereServDeposit.Count > 0)
                        {
                            ado.DEPOSIT_AMOUNT = rdo._ListSereServDeposit
                                .Where(o => o.SERE_SERV_ID == sereServ.ID)
                                .Sum(o => o.AMOUNT);
                        }

                        serviceADOs.Add(ado);
                    }
                }

                SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.SERVICE_COUNT, serviceADOs.Count));
                SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.SERVICE_TOTAL_PRICE, serviceADOs.Sum(o => o.TOTAL_PRICE)));
                SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.SERVICE_TOTAL_HEIN_PRICE, serviceADOs.Sum(o => o.TOTAL_HEIN_PRICE)));
                SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.SERVICE_TOTAL_PATIENT_PRICE_BHYT, serviceADOs.Sum(o => o.TOTAL_PATIENT_PRICE_BHYT)));
                SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.SERVICE_TOTAL_PATIENT_PRICE, serviceADOs.Sum(o => o.TOTAL_PATIENT_PRICE)));
                SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.SERVICE_TOTAL_DEPOSIT_AMOUNT, serviceADOs.Sum(o => o.DEPOSIT_AMOUNT)));
                SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.SERVICE_TOTAL_DISCOUNT, serviceADOs.Sum(o => o.DISCOUNT)));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Bang hinh thuc thanh toan cua giao dich + cac key tong hop
        /// </summary>
        private void ProcessPayformADO()
        {
            try
            {
                if (rdo._ListTransactionPayform != null && rdo._ListTransactionPayform.Count > 0)
                {
                    long numOrder = 1;
                    foreach (var payform in rdo._ListTransactionPayform
                        .Where(o => o.IS_DELETE != 1)
                        .OrderBy(o => o.SORT_ORDER).ThenBy(o => o.ID))
                    {
                        Mps000112PayformADO ado = new Mps000112PayformADO(payform, rdo._PayForms, rdo._Banks);
                        ado.NUM_ORDER = numOrder++;
                        payformADOs.Add(ado);
                    }
                }
                else if (rdo._Transaction != null)
                {
                    //Giao dich thanh toan 1 hinh thuc (config MULTI_PAYFORM tat) khong co dong
                    //HIS_TRANSACTION_PAYFORM -> suy 1 dong tu chinh giao dich de bang khong bi rong
                    Mps000112PayformADO ado = new Mps000112PayformADO(rdo._Transaction);
                    ado.NUM_ORDER = 1;
                    payformADOs.Add(ado);
                }

                SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.PAYFORM_COUNT, payformADOs.Count));
                SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.PAYFORM_TOTAL_AMOUNT, payformADOs.Sum(o => o.AMOUNT)));
                SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.PAYFORM_TOTAL_SURCHARGE, payformADOs.Sum(o => o.SURCHARGE_AMOUNT)));
                SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.PAYFORM_TOTAL, payformADOs.Sum(o => o.TOTAL_AMOUNT)));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Bang chiet khau cua giao dich + cac key tong hop
        /// </summary>
        private void ProcessDiscountADO()
        {
            try
            {
                if (rdo._ListTransactionDiscount != null && rdo._ListTransactionDiscount.Count > 0)
                {
                    long numOrder = 1;
                    foreach (var discount in rdo._ListTransactionDiscount
                        .Where(o => o.IS_DELETE != 1)
                        .OrderBy(o => o.ID))
                    {
                        Mps000112DiscountADO ado = new Mps000112DiscountADO(discount);
                        ado.NUM_ORDER = numOrder++;
                        discountADOs.Add(ado);
                    }
                }

                SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.DISCOUNT_COUNT, discountADOs.Count));
                SetSingleKey(new KeyValue(Mps000112ExtendSingleKey.DISCOUNT_TOTAL, discountADOs.Sum(o => o.DISCOUNT)));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetBarcodeKey()
        {
            try
            {
                if (rdo._Transaction != null && !String.IsNullOrWhiteSpace(rdo._Transaction.TREATMENT_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcodeTreatmentCode = new Inventec.Common.BarcodeLib.Barcode(rdo._Transaction.TREATMENT_CODE);
                    barcodeTreatmentCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeTreatmentCode.IncludeLabel = false;
                    barcodeTreatmentCode.Width = 120;
                    barcodeTreatmentCode.Height = 40;
                    barcodeTreatmentCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeTreatmentCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeTreatmentCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeTreatmentCode.IncludeLabel = true;

                    dicImage.Add(Mps000112ExtendSingleKey.TREATMENT_CODE_BARCODE, barcodeTreatmentCode);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                log = LogDataTransaction(rdo._Transaction.TREATMENT_CODE, rdo._Transaction.TRANSACTION_CODE, "");
                log += "SoTien: " + rdo._Transaction.AMOUNT;
            }
            catch (Exception ex)
            {
                log = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return log;
        }

        public override string ProcessUniqueCodeData()
        {
            string result = "";
            try
            {
                if (rdo != null && rdo._Transaction != null)
                    result = String.Format("{0}_{1}_{2}_{3}", this.printTypeCode, rdo._Transaction.TREATMENT_CODE, rdo._Transaction.TRANSACTION_CODE, rdo._Transaction.ACCOUNT_BOOK_CODE);
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }

    public static class AgeUtil
    {
        public static string CalculateFullAge(long ageNumber)
        {
            string tuoi;
            string cboAge;
            try
            {
                DateTime dtNgSinh = Inventec.Common.TypeConvert.Parse.ToDateTime(Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ageNumber));
                TimeSpan diff = DateTime.Now - dtNgSinh;
                long tongsogiay = diff.Ticks;
                if (tongsogiay < 0)
                {
                    tuoi = "";
                    cboAge = "Tuổi";
                    return "";
                }
                DateTime newDate = new DateTime(tongsogiay);

                int nam = newDate.Year - 1;
                int thang = newDate.Month - 1;
                int ngay = newDate.Day - 1;
                int gio = newDate.Hour;
                int phut = newDate.Minute;
                int giay = newDate.Second;

                if (nam > 0)
                {
                    tuoi = nam.ToString();
                    cboAge = "Tuổi";
                }
                else
                {
                    if (thang > 0)
                    {
                        tuoi = thang.ToString();
                        cboAge = "Tháng";
                    }
                    else
                    {
                        if (ngay > 0)
                        {
                            tuoi = ngay.ToString();
                            cboAge = "ngày";
                        }
                        else
                        {
                            tuoi = "";
                            cboAge = "Giờ";
                        }
                    }
                }
                return tuoi + " " + cboAge;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }
    }
}
