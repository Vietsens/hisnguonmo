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
using HIS.Desktop.Common.BankQrCode;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000181.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000181
{
    public class Mps000181Processor : AbstractProcessor
    {
        Mps000181PDO rdo;
        private List<ExpMestMedicineSDO> expMestMedicinesTYPE;
        private List<ExpMestMedicineSDO> expMestMedicines_Sort;
        private List<ServiceReqSDO> serviceReqSdo;
        private List<ExpMestMedicineSDO> expMestMedicineReq;

        public Mps000181Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000181PDO)rdoBase;
        }

        void SetBarcodeKey()
        {
            try
            {
                if (rdo.vHisPrescription5 != null)
                {
                    if (!String.IsNullOrEmpty(rdo.vHisPrescription5.TDL_TREATMENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeTreatmentCode = new Inventec.Common.BarcodeLib.Barcode(rdo.vHisPrescription5.TDL_TREATMENT_CODE);
                        barcodeTreatmentCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeTreatmentCode.IncludeLabel = false;
                        barcodeTreatmentCode.Width = 120;
                        barcodeTreatmentCode.Height = 40;
                        barcodeTreatmentCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeTreatmentCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeTreatmentCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeTreatmentCode.IncludeLabel = true;

                        dicImage.Add(Mps000181ExtendSingleKey.TREATMENT_CODE_BARCODE, barcodeTreatmentCode);
                    }

                    if (!String.IsNullOrEmpty(rdo.Mps000181ADO.EXP_MEST_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode expMestCodeBarCode = new Inventec.Common.BarcodeLib.Barcode(rdo.Mps000181ADO.EXP_MEST_CODE);
                        expMestCodeBarCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        expMestCodeBarCode.IncludeLabel = false;
                        expMestCodeBarCode.Width = 120;
                        expMestCodeBarCode.Height = 40;
                        expMestCodeBarCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        expMestCodeBarCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        expMestCodeBarCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        expMestCodeBarCode.IncludeLabel = true;

                        dicImage.Add(Mps000181ExtendSingleKey.EXP_MEST_CODE_BARCODE, expMestCodeBarCode);
                    }

                    if (!String.IsNullOrEmpty(rdo.vHisPrescription5.TDL_PATIENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodePatient = new Inventec.Common.BarcodeLib.Barcode(rdo.vHisPrescription5.TDL_PATIENT_CODE);
                        barcodePatient.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodePatient.IncludeLabel = false;
                        barcodePatient.Width = 120;
                        barcodePatient.Height = 40;
                        barcodePatient.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodePatient.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodePatient.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodePatient.IncludeLabel = true;

                        dicImage.Add(Mps000181ExtendSingleKey.PATIENT_CODE_BARCODE, barcodePatient);
                    }

                    if (!String.IsNullOrEmpty(rdo.vHisPrescription5.SERVICE_REQ_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeServiceReq = new Inventec.Common.BarcodeLib.Barcode(rdo.vHisPrescription5.SERVICE_REQ_CODE);
                        barcodeServiceReq.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeServiceReq.IncludeLabel = false;
                        barcodeServiceReq.Width = 120;
                        barcodeServiceReq.Height = 40;
                        barcodeServiceReq.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeServiceReq.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeServiceReq.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeServiceReq.IncludeLabel = true;
                        dicImage.Add(Mps000181ExtendSingleKey.SERVICE_REQ_CODE_BAR, barcodeServiceReq);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetSingleKey()
        {
            try
            {
                if (rdo.Mps000181ADO != null)
                {
                    AddObjectKeyIntoListkey<Mps000181ADO>(rdo.Mps000181ADO, false);
                }

                if (rdo.hisDhst != null)
                {
                    AddObjectKeyIntoListkey<HIS_DHST>(rdo.hisDhst, false);
                }

                if (rdo.vHisPrescription5 != null)
                {
                    if (rdo.expMestMedicines != null && rdo.expMestMedicines.Count > 0)
                    {
                        decimal tong = 0;
                        foreach (var item in rdo.expMestMedicines)
                        {
                            tong += item.AMOUNT * ((item.PRICE ?? 0) * (1 + (item.VAT_RATIO ?? 0)));
                        }
                        SetSingleKey(new KeyValue(Mps000181ExtendSingleKey.TOTAL_PRICE_PRESCRIPTION, tong));

                        //nếu không có use_time_to trong V_HIS_PRESCRIPTION thì lấy trong ds thuốc 
                        long useTimeTo = 0;
                        var maxUseTimeTo = rdo.expMestMedicines.Where(o => o.USE_TIME_TO.HasValue).ToList();
                        if (maxUseTimeTo != null && maxUseTimeTo.Count > 0)
                        {
                            useTimeTo = maxUseTimeTo.Max(m => m.USE_TIME_TO).Value;
                        }

                        if (!rdo.vHisPrescription5.USE_TIME_TO.HasValue && useTimeTo > 0)
                        {
                            SetSingleKey(new KeyValue(Mps000181ExtendSingleKey.USE_TIME_TO_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(useTimeTo)));
                        }
                        else
                            SetSingleKey(new KeyValue(Mps000181ExtendSingleKey.USE_TIME_TO_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPrescription5.USE_TIME_TO ?? 0)));

                        if (useTimeTo > 0)
                        {
                            SetSingleKey(new KeyValue(Mps000181ExtendSingleKey.DETAIL_MAX_USE_TIME_TO_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(useTimeTo)));
                        }

                        if (rdo.vHisPrescription5.USE_TIME.HasValue)
                        {
                            SetSingleKey(new KeyValue(Mps000181ExtendSingleKey.USER_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPrescription5.USE_TIME ?? 0)));
                        }
                        else
                        {
                            SetSingleKey(new KeyValue(Mps000181ExtendSingleKey.USER_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPrescription5.INTRUCTION_TIME)));
                        }

                        SetSingleKey(new KeyValue(Mps000181ExtendSingleKey.INTRUCTION_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPrescription5.INTRUCTION_TIME)));
                        SetSingleKey(new KeyValue(Mps000181ExtendSingleKey.INTRUCTION_TIME_FULL_SRT, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo.vHisPrescription5.INTRUCTION_TIME)));
                    }

                    SetSingleKey((new KeyValue(Mps000181ExtendSingleKey.AGE, AgeCaption(rdo.vHisPrescription5.TDL_PATIENT_DOB))));
                    SetSingleKey((new KeyValue(Mps000181ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPrescription5.TDL_PATIENT_DOB))));
                    SetSingleKey((new KeyValue(Mps000181ExtendSingleKey.D_O_B, rdo.vHisPrescription5.TDL_PATIENT_DOB.ToString().Substring(0, 4))));

                    SetSingleKey(new KeyValue(Mps000181ExtendSingleKey.NATIONAL_NAME, rdo.vHisPrescription5.TDL_PATIENT_NATIONAL_NAME));
                    SetSingleKey(new KeyValue(Mps000181ExtendSingleKey.WORK_PLACE, rdo.vHisPrescription5.TDL_PATIENT_WORK_PLACE_NAME));
                    SetSingleKey(new KeyValue(Mps000181ExtendSingleKey.ADDRESS, rdo.vHisPrescription5.TDL_PATIENT_ADDRESS));
                    SetSingleKey(new KeyValue(Mps000181ExtendSingleKey.CAREER_NAME, rdo.vHisPrescription5.TDL_PATIENT_CAREER_NAME));
                    SetSingleKey(new KeyValue(Mps000181ExtendSingleKey.PATIENT_CODE, rdo.vHisPrescription5.TDL_PATIENT_CODE));
                    SetSingleKey(new KeyValue(Mps000181ExtendSingleKey.DISTRICT_CODE, rdo.vHisPrescription5.TDL_PATIENT_DISTRICT_CODE));
                    SetSingleKey(new KeyValue(Mps000181ExtendSingleKey.GENDER_NAME, rdo.vHisPrescription5.TDL_PATIENT_GENDER_NAME));
                    SetSingleKey(new KeyValue(Mps000181ExtendSingleKey.MILITARY_RANK_NAME, rdo.vHisPrescription5.TDL_PATIENT_MILITARY_RANK_NAME));
                    SetSingleKey(new KeyValue(Mps000181ExtendSingleKey.DOB, rdo.vHisPrescription5.TDL_PATIENT_DOB));
                    SetSingleKey(new KeyValue(Mps000181ExtendSingleKey.VIR_ADDRESS, rdo.vHisPrescription5.TDL_PATIENT_ADDRESS));
                    SetSingleKey(new KeyValue(Mps000181ExtendSingleKey.VIR_PATIENT_NAME, rdo.vHisPrescription5.TDL_PATIENT_NAME));

                    if (rdo.vHisPrescription5 != null)
                    {
                        // Cấu trúc HIS thường dùng trường ADVISE cho lời dặn
                        SetSingleKey(new KeyValue("ADVISE", rdo.vHisPrescription5.ADVISE));

                        // Nếu lời dặn lưu ở trường NOTE, hãy dùng:
                        // SetSingleKey(new KeyValue("NOTE", rdo.vHisPrescription5.NOTE)); 
                    }
                }

                if (rdo.PatyAlterBhyt != null)
                {
                    if (!String.IsNullOrEmpty(rdo.PatyAlterBhyt.HEIN_CARD_NUMBER))
                    {
                        SetSingleKey((new KeyValue(Mps000181ExtendSingleKey.HEIN_CARD_NUMBER_SEPARATE, HeinCardHelper.SetHeinCardNumberDisplayByNumber(rdo.PatyAlterBhyt.HEIN_CARD_NUMBER))));
                        SetSingleKey((new KeyValue(Mps000181ExtendSingleKey.IS_HEIN, "X")));
                        SetSingleKey((new KeyValue(Mps000181ExtendSingleKey.IS_VIENPHI, "")));
                        SetSingleKey((new KeyValue(Mps000181ExtendSingleKey.HEIN_CARD_NUMBER_1, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(0, 2))));
                        SetSingleKey((new KeyValue(Mps000181ExtendSingleKey.HEIN_CARD_NUMBER_2, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(2, 1))));
                        SetSingleKey((new KeyValue(Mps000181ExtendSingleKey.HEIN_CARD_NUMBER_3, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(3, 2))));
                        SetSingleKey((new KeyValue(Mps000181ExtendSingleKey.HEIN_CARD_NUMBER_4, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(5, 2))));
                        SetSingleKey((new KeyValue(Mps000181ExtendSingleKey.HEIN_CARD_NUMBER_5, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(7, 3))));
                        SetSingleKey((new KeyValue(Mps000181ExtendSingleKey.HEIN_CARD_NUMBER_6, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(10))));
                        SetSingleKey(new KeyValue(Mps000181ExtendSingleKey.STR_HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.PatyAlterBhyt.HEIN_CARD_FROM_TIME ?? 0)));
                        SetSingleKey(new KeyValue(Mps000181ExtendSingleKey.STR_HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.PatyAlterBhyt.HEIN_CARD_TO_TIME ?? 0)));
                    }
                    else
                    {
                        SetSingleKey((new KeyValue(Mps000181ExtendSingleKey.IS_HEIN, "")));
                        SetSingleKey((new KeyValue(Mps000181ExtendSingleKey.IS_VIENPHI, "X")));
                    }
                }

                if (rdo.hisServiceReq_Exam != null && rdo.vHisPrescription5 != null)
                {
                    System.Reflection.PropertyInfo[] pis = typeof(HIS_SERVICE_REQ).GetProperties();
                    if (pis != null && pis.Length > 0)
                    {
                        foreach (var pi in pis)
                        {
                            if (pi.GetGetMethod().IsVirtual) continue;
                            try
                            {
                                if (pi.GetValue(rdo.vHisPrescription5) == null)
                                {
                                    pi.SetValue(rdo.vHisPrescription5, pi.GetValue(rdo.hisServiceReq_Exam));
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                    //AddObjectKeyIntoListkey<HIS_SERVICE_REQ>(rdo.hisServiceReq_Exam, false);
                }

                if (rdo.HisTreatment != null)
                {
                    SetSingleKey((new KeyValue("APPOINTMENT_CODE", rdo.HisTreatment.APPOINTMENT_CODE)));
                    SetSingleKey((new KeyValue("APPOINTMENT_DATE", rdo.HisTreatment.APPOINTMENT_DATE)));
                    SetSingleKey((new KeyValue("APPOINTMENT_DESC", rdo.HisTreatment.APPOINTMENT_DESC)));
                    SetSingleKey((new KeyValue("APPOINTMENT_SURGERY", rdo.HisTreatment.APPOINTMENT_SURGERY)));
                    SetSingleKey((new KeyValue("APPOINTMENT_TIME", rdo.HisTreatment.APPOINTMENT_TIME)));
                    SetSingleKey((new KeyValue("APPOINTMENT_EXAM_ROOM_IDS", rdo.HisTreatment.APPOINTMENT_EXAM_ROOM_IDS)));
                }
                if(rdo.lstHisServiceReq != null && rdo.lstHisServiceReq.Count > 0)
                    AddObjectKeyIntoListkey<HIS_SERVICE_REQ>(rdo.lstHisServiceReq.First(), false);
                AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo.PatyAlterBhyt);
                if (rdo.HisTreatment != null)
                {
                    AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.HisTreatment, false);
                }

                if (rdo.hisServiceReq_Exam != null)
                {
                    SetSingleKey((new KeyValue(Mps000181ExtendSingleKey.PART_EXAM_EYE_STR, ProcessDataEye(rdo.hisServiceReq_Exam.PART_EXAM_EYE))));
                    SetSingleKey((new KeyValue(Mps000181ExtendSingleKey.PART_EXAM_EYE_TENSION_LEFT_STR, ProcessDataEye(rdo.hisServiceReq_Exam.PART_EXAM_EYE_TENSION_LEFT))));
                    SetSingleKey((new KeyValue(Mps000181ExtendSingleKey.PART_EXAM_EYE_TENSION_RIGHT_STR, ProcessDataEye(rdo.hisServiceReq_Exam.PART_EXAM_EYE_TENSION_RIGHT))));
                    SetSingleKey((new KeyValue(Mps000181ExtendSingleKey.PART_EXAM_EYESIGHT_LEFT_STR, ProcessDataEye(rdo.hisServiceReq_Exam.PART_EXAM_EYESIGHT_LEFT))));
                    SetSingleKey((new KeyValue(Mps000181ExtendSingleKey.PART_EXAM_EYESIGHT_RIGHT_STR, ProcessDataEye(rdo.hisServiceReq_Exam.PART_EXAM_EYESIGHT_RIGHT))));
                    SetSingleKey((new KeyValue(Mps000181ExtendSingleKey.PART_EXAM_EYESIGHT_GLASS_LEFT_STR, ProcessDataEye(rdo.hisServiceReq_Exam.PART_EXAM_EYESIGHT_GLASS_LEFT))));
                    SetSingleKey((new KeyValue(Mps000181ExtendSingleKey.PART_EXAM_EYESIGHT_GLASS_RIGHT_STR, ProcessDataEye(rdo.hisServiceReq_Exam.PART_EXAM_EYESIGHT_GLASS_RIGHT))));
                }

                string ttServiceNames = "";
                string clsServiceNames = "";
                if (rdo.ListSereServCls != null && rdo.ListSereServCls.Count > 0)
                {
                    var listTt = rdo.ListSereServCls.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT && o.IS_NO_EXECUTE != 1).ToList();
                    var listOther = rdo.ListSereServCls.Where(o => o.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT && o.IS_NO_EXECUTE != 1).ToList();

                    if (listTt != null && listTt.Count > 0)
                    {
                        ttServiceNames = string.Join(",", listTt.Select(s => s.TDL_SERVICE_NAME).Distinct().ToList());
                    }

                    if (listOther != null && listOther.Count > 0)
                    {
                        clsServiceNames = string.Join(",", listOther.Select(s => s.TDL_SERVICE_NAME).Distinct().ToList());
                    }
                }

                SetSingleKey((new KeyValue(Mps000181ExtendSingleKey.TT_SERVICE_NAME, ttServiceNames)));
                SetSingleKey((new KeyValue(Mps000181ExtendSingleKey.CLS_SERVICE_NAME, clsServiceNames)));

                if (rdo.HisExpMest != null)
                {
                    AddObjectKeyIntoListkey(rdo.HisExpMest, false);
                    string title = "c";
                    var expMestMedicine = rdo.expMestMedicines != null && rdo.expMestMedicines.Count() > 0 ? rdo.expMestMedicines.FirstOrDefault() : null;
                    if (expMestMedicine != null && expMestMedicine.IS_NEUROLOGICAL == 1) title = "h";
                    else if (expMestMedicine != null && expMestMedicine.IS_ADDICTIVE == 1) title = "n";
                    string serviceReqCode = rdo.vHisPrescription5 != null ? (rdo.vHisPrescription5.SERVICE_REQ_CODE) : null;
                    string electronicExpMestCode = string.Format("{0}{1}-{2}", MPS.ProcessorBase.PrintConfig.MediOrgCode, HIS.ERXConnect.ERXCode.Encode(Convert.ToInt64(serviceReqCode)), title);
                    SetSingleKey(new KeyValue(Mps000181ExtendSingleKey.ELECTRONIC_EXP_MEST_CODE, electronicExpMestCode));
                }
                string icdCode = null;
                string icdName = null;
                string icdSubCode = null;
                string icdText = null;

                if (rdo.lstHisServiceReq != null && rdo.lstHisServiceReq.Count > 0)
                {
                    var lastReq = rdo.lstHisServiceReq.Last();

                    if (lastReq != null)
                    {
                        icdCode = lastReq.ICD_CODE;
                        icdName = lastReq.ICD_NAME;
                        icdSubCode = lastReq.ICD_SUB_CODE;
                        icdText = lastReq.ICD_TEXT;
                    }
                }
                else if (rdo.hisServiceReq_Exam != null)
                {
                    icdCode = rdo.hisServiceReq_Exam.ICD_CODE;
                    icdName = rdo.hisServiceReq_Exam.ICD_NAME;
                    icdSubCode = rdo.hisServiceReq_Exam.ICD_SUB_CODE;
                    icdText = rdo.hisServiceReq_Exam.ICD_TEXT;
                }
                else if (rdo.vHisPrescription5 != null)
                {
                    icdCode = rdo.vHisPrescription5.ICD_CODE;
                    icdName = rdo.vHisPrescription5.ICD_NAME;
                    icdSubCode = rdo.vHisPrescription5.ICD_SUB_CODE;
                    icdText = rdo.vHisPrescription5.ICD_TEXT;
                }
                if (!string.IsNullOrWhiteSpace(icdCode))
                    SetSingleKey(new KeyValue(Mps000181ExtendSingleKey.REQ_ICD_CODE, icdCode));

                if (!string.IsNullOrWhiteSpace(icdName))
                    SetSingleKey(new KeyValue(Mps000181ExtendSingleKey.REQ_ICD_NAME, icdName));

                if (!string.IsNullOrWhiteSpace(icdSubCode))
                    SetSingleKey(new KeyValue(Mps000181ExtendSingleKey.REQ_ICD_SUB_CODE, icdSubCode));

                if (!string.IsNullOrWhiteSpace(icdText))
                    SetSingleKey(new KeyValue(Mps000181ExtendSingleKey.REQ_ICD_TEXT, icdText));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetQrCode()
        {
            try
            {
                if (rdo.TransReq != null && rdo.ListHisConfigPaymentQrCode != null && rdo.ListHisConfigPaymentQrCode.Count > 0)
                {
                    var data = QrCodeProcessor.CreateQrImage(rdo.TransReq, rdo.ListHisConfigPaymentQrCode);
                    if (data != null && data.Count > 0)
                    {
                        foreach (var item in data)
                        {
                            SetSingleKey(new KeyValue(item.Key, item.Value));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public string AgeCaption(long dob)
        {
            string empty = string.Empty;
            try
            {
                string arg = "tháng tuổi";
                string arg2 = "giờ tuổi";
                if (dob > 0)
                {
                    System.DateTime value = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dob).Value;
                    if (value == System.DateTime.MinValue)
                    {
                        throw new ArgumentNullException("dtNgSinh");
                    }

                    TimeSpan timeSpan = System.DateTime.Now - value;
                    TimeSpan timeSpan2 = System.DateTime.Now.Date - value.Date;
                    double totalHours = timeSpan.TotalHours;
                    if (totalHours < 24.0)
                    {
                        return totalHours + " " + arg2;
                    }

                    long ticks = timeSpan2.Ticks;
                    System.DateTime dateTime = new System.DateTime(ticks);
                    int num = dateTime.Year * 12 + dateTime.Month - 1;
                    if (num < 72)
                    {
                        return num + " " + arg;
                    }

                    int num2 = System.DateTime.Now.Year - value.Year;
                    return num2 + " ";
                }

                return empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        //Lọc thuốc theo thứ tự
        private void MedicinesSort()
        {
            try
            {
                expMestMedicines_Sort = new List<ExpMestMedicineSDO>();
                if (rdo.expMestMedicines != null && rdo.expMestMedicines.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("MPS000181____________" + rdo.KeyUseForm);
                    if (rdo.KeyUseForm == 1)
                    {
                        expMestMedicines_Sort = rdo.expMestMedicines.OrderByDescending(p => p.MEDICINE_USE_FORM_NUM_ORDER).ThenBy(o => o.NUM_ORDER).ToList();
                    }
                    else if (rdo.KeyUseForm == 2)
                    {
                        expMestMedicines_Sort = rdo.expMestMedicines.OrderBy(p => p.MEDICINE_USE_FORM_NUM_ORDER).ThenBy(o => o.NUM_ORDER).ToList();
                    }
                    else
                    {
                        expMestMedicines_Sort = rdo.expMestMedicines.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string ProcessDataEye(string p)
        {
            string result = p;
            try
            {
                if (!String.IsNullOrEmpty(p))
                {
                    bool addText = true;
                    foreach (var item in p)
                    {
                        if (Char.IsLetter(item))
                        {
                            addText = false;
                            break;
                        }
                    }

                    if (addText)
                    {
                        result += "/10";
                    }
                }
            }
            catch (Exception ex)
            {
                result = p;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                LogPhase("ProcessData bat dau");

                SetBarcodeKey();
                SetSingleKey();
                ProcessListData();
                SetQrCode();
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                this.SetSignatureKeyImageByCFG();
                MedicinesSort();
                LogPhase("Template dang doc: " + System.IO.Path.GetFullPath(fileName));
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "type", expMestMedicinesTYPE);
                objectTag.AddObjectData(store, "type2", expMestMedicinesTYPE);
                objectTag.AddObjectData(store, "type3", expMestMedicinesTYPE);
                objectTag.AddObjectData(store, "MedicineExpmest", expMestMedicines_Sort);
                objectTag.AddObjectData(store, "list2", expMestMedicines_Sort);
                objectTag.AddObjectData(store, "list3", expMestMedicines_Sort);

                //AddObjectData nuot ArgumentNullException va chi log Warn, nen phai bat gia tri tra ve
                bool addPhase = objectTag.AddObjectData(store, "Phase", serviceReqSdo);
                bool addPhase1 = objectTag.AddObjectData(store, "Phase1", serviceReqSdo);
                bool addMedicine = objectTag.AddObjectData(store, "Medicine", expMestMedicineReq);
                bool addMedicine1 = objectTag.AddObjectData(store, "Medicine1", expMestMedicineReq);
                LogPhase(string.Format("AddTable Phase={0}, Phase1={1}, Medicine={2}, Medicine1={3} | so dot={4}, so dong thuoc={5}",
                    addPhase, addPhase1, addMedicine, addMedicine1,
                    serviceReqSdo == null ? "null" : serviceReqSdo.Count.ToString(),
                    expMestMedicineReq == null ? "null" : expMestMedicineReq.Count.ToString()));

                objectTag.AddRelationship(store, "Phase", "Medicine", "INTRUCTION_DATE", "TDL_INTRUCTION_DATE");
                objectTag.AddRelationship(store, "Phase1", "Medicine1", "INTRUCTION_DATE", "TDL_INTRUCTION_DATE");

                objectTag.AddRelationship(store, "type", "MedicineExpmest", "PATIENT_TYPE_NAME", "PATIENT_TYPE_NAME");
                objectTag.AddRelationship(store, "type2", "list2", "PATIENT_TYPE_NAME", "PATIENT_TYPE_NAME");
                objectTag.AddRelationship(store, "type3", "list3", "PATIENT_TYPE_NAME", "PATIENT_TYPE_NAME");
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        public override string ProcessUniqueCodeData()
        {
            string result = "";
            try
            {
                if (rdo != null && rdo.vHisPrescription5 != null)
                {
                    string treatmentCode = "TREATMENT_CODE:" + rdo.vHisPrescription5.TDL_TREATMENT_CODE;
                    string serviceReqCode = "SERVICE_REQ_CODE:" + rdo.vHisPrescription5.SERVICE_REQ_CODE;
                    string serviceCode = "";
                    if (rdo.expMestMedicines != null && rdo.expMestMedicines.Count > 0)
                    {
                        var serviceFirst = rdo.expMestMedicines.OrderBy(o => o.MEDICINE_TYPE_CODE).First();
                        serviceCode = "SERVICE_CODE:" + serviceFirst.MEDICINE_TYPE_CODE;
                    }

                    result = String.Format("{0} {1} {2} {3}", printTypeCode, treatmentCode, serviceReqCode, serviceCode);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        //Log lần vết băng Phase/Medicine. Ghi mức Error vì appender chặn Debug/Warn ở nhiều cấu hình.
        //Stamp trong chuỗi để biết DLL đang chạy đã đúng bản chưa
        private void LogPhase(string message)
        {
            Inventec.Common.Logging.LogSystem.Error("MPS000181[phase-v3-theo-y-lenh] " + message);
        }

        private void ProcessListData()
        {
            try
            {
                if (rdo.expMestMedicines != null && rdo.expMestMedicines.Count > 0)
                {
                    var groupType = rdo.expMestMedicines.GroupBy(o => o.PATIENT_TYPE_ID).ToList();
                    expMestMedicinesTYPE = new List<ExpMestMedicineSDO>();
                    foreach (var item in groupType)
                    {
                        if (item.First().PATIENT_TYPE_ID > 0)
                        {
                            ExpMestMedicineSDO ado = new ExpMestMedicineSDO();
                            ado.PATIENT_TYPE_ID = item.First().PATIENT_TYPE_ID;
                            ado.PATIENT_TYPE_NAME = item.First().PATIENT_TYPE_NAME;
                            expMestMedicinesTYPE.Add(ado);
                        }
                    }

                    expMestMedicinesTYPE = expMestMedicinesTYPE.OrderBy(o => o.PATIENT_TYPE_ID).ToList();
                    if (rdo.expMestMedicines.Exists(o => o.PATIENT_TYPE_ID == 0))
                    {
                        ExpMestMedicineSDO ado = new ExpMestMedicineSDO();
                        ado.PATIENT_TYPE_ID = rdo.expMestMedicines.First(o => o.PATIENT_TYPE_ID == 0).PATIENT_TYPE_ID;
                        ado.PATIENT_TYPE_NAME = rdo.expMestMedicines.First(o => o.PATIENT_TYPE_ID == 0).PATIENT_TYPE_NAME;
                        expMestMedicinesTYPE.Add(ado);
                    }
                }

                //Luôn khởi tạo để bảng Phase/Medicine vẫn được AddTable, tránh FlexCel báo "DataTable not defined"
                serviceReqSdo = new List<ServiceReqSDO>();
                expMestMedicineReq = new List<ExpMestMedicineSDO>();
                var allMedicines = rdo.expMestMedicines ?? new List<ExpMestMedicineSDO>();

                //Luồng in đơn lẻ không truyền lstHisServiceReq, dựng 1 đợt từ chính y lệnh đang in 
                List<HIS_SERVICE_REQ> lstServiceReq = rdo.lstHisServiceReq;
                if ((lstServiceReq == null || lstServiceReq.Count == 0) && rdo.vHisPrescription5 != null)
                {
                    lstServiceReq = new List<HIS_SERVICE_REQ>() { rdo.vHisPrescription5 };
                }

                LogPhase(string.Format("Nguon du lieu: rdo.lstHisServiceReq={0}, rdo.vHisPrescription5.ID={1}, expMestMedicines={2}, lstServiceReq sau fallback={3}",
                    rdo.lstHisServiceReq == null ? "null" : rdo.lstHisServiceReq.Count.ToString(),
                    rdo.vHisPrescription5 == null ? "null" : rdo.vHisPrescription5.ID.ToString(),
                    allMedicines.Count,
                    lstServiceReq == null ? "null" : lstServiceReq.Count.ToString()));

                if (rdo.lstHisServiceReq == null || rdo.lstHisServiceReq.Count == 0)
                {
                    LogPhase("Khong co lstHisServiceReq -> fallback 1 dot tu vHisPrescription5. Luong in nay moi ban in chi mang 1 y lenh nen khong the ra 2 dot.");
                }

                if (lstServiceReq != null)
                {
                    foreach (var req in lstServiceReq)
                    {
                        LogPhase(string.Format("  y lenh ID={0}, CODE={1}, INTRUCTION_DATE={2}, USE_TIME={3}, USE_TIME_TO={4}",
                            req.ID, req.SERVICE_REQ_CODE, req.INTRUCTION_DATE, req.USE_TIME, req.USE_TIME_TO));
                    }
                }

                if (lstServiceReq != null && lstServiceReq.Count > 0)
                {
                    var group = lstServiceReq
                        .GroupBy(req => (long?)req.ID)
                        .OrderBy(g => g.Min(o => o.USE_TIME.HasValue && o.USE_TIME.Value > 0 ? o.USE_TIME.Value : o.INTRUCTION_TIME))
                        .ToList();

                    LogPhase(string.Format("Gom duoc {0} dot (moi y lenh mot dot)", group.Count));

                    foreach (var item in group)
                    {
                        bool isKey = false;
                        foreach (var req in item)
                        {
                            var expMestMedicine = allMedicines
                                                .Where(o => o.TDL_SERVICE_REQ_ID == req.ID) 
                                                .ToList();
                            //Đơn lẻ: thuốc không gắn TDL_SERVICE_REQ_ID khớp y lệnh thì lấy trọn danh sách đang in
                            if (expMestMedicine.Count == 0 && lstServiceReq.Count == 1)
                            {
                                expMestMedicine = allMedicines.ToList();
                                LogPhase(string.Format("Y lenh ID={0} khong khop TDL_SERVICE_REQ_ID nao, lay tron {1} dong thuoc", req.ID, expMestMedicine.Count));
                            }
                            LogPhase(string.Format("Y lenh ID={0}, INTRUCTION_DATE={1}: {2} dong thuoc", req.ID, req.INTRUCTION_DATE, expMestMedicine.Count));
                            foreach (var mediMest in expMestMedicine)
                            {
                                isKey = true;
                                ExpMestMedicineSDO mediSDO = new ExpMestMedicineSDO();
                                mediSDO.MEDICINE_TYPE_NAME = mediMest.MEDICINE_TYPE_NAME ?? "";
                                mediSDO.AMOUNT = mediMest.AMOUNT > 0 ? mediMest.AMOUNT : 0;
                                mediSDO.TUTORIAL = mediMest.TUTORIAL ?? "";
                                mediSDO.TDL_SERVICE_REQ_ID = mediMest.TDL_SERVICE_REQ_ID ?? 0;
                                mediSDO.TDL_INTRUCTION_DATE = item.Key.HasValue ? item.Key.Value : 0;
                                expMestMedicineReq.Add(mediSDO);
                            }
                            //var serviceReqMety = rdo.lstServiceReqMety.Where(o=>o.SERVICE_REQ_ID == req.ID).ToList();
                            //foreach (var serviceMety in serviceReqMety)
                            //{
                            //    isKey = true;
                            //    ExpMestMedicineSDO mediSDO = new ExpMestMedicineSDO();
                            //    mediSDO.MEDICINE_TYPE_CODE = serviceMety.MEDICINE_TYPE_NAME != null ? serviceMety.MEDICINE_TYPE_NAME : "";
                            //    mediSDO.AMOUNT = serviceMety.AMOUNT > 0 ? serviceMety.AMOUNT : 0;
                            //    mediSDO.TUTORIAL = serviceMety.TUTORIAL != null ? serviceMety.TUTORIAL : "";
                            //    mediSDO.TDL_INTRUCTION_DATE = item.Key.HasValue ? item.Key.Value : 0;
                            //    expMestMedicineReq.Add(mediSDO);
                            //}
                        }
                        if (isKey)
                        {
                            ServiceReqSDO sdo = new ServiceReqSDO();
                            sdo.USE_TIME = item.Min(o => o.USE_TIME.HasValue && o.USE_TIME.Value > 0 ? o.USE_TIME.Value : o.INTRUCTION_TIME);
                            sdo.USE_TIME_TO = item.Max(o => o.USE_TIME_TO ?? 0);
                            //y lệnh chưa có USE_TIME_TO thì lấy mốc lớn nhất trong danh sách thuốc
                            if ((sdo.USE_TIME_TO ?? 0) == 0)
                            {
                                var mediUseTimeTo = allMedicines.Where(o => o.USE_TIME_TO.HasValue).ToList();
                                if (mediUseTimeTo.Count > 0)
                                {
                                    sdo.USE_TIME_TO = mediUseTimeTo.Max(o => o.USE_TIME_TO.Value);
                                }
                            }
                            sdo.INTRUCTION_DATE = item.Key.HasValue ? item.Key.Value : 0;
                            serviceReqSdo.Add(sdo);
                            LogPhase(string.Format("Them dot INTRUCTION_DATE={0}, USE_TIME={1}, USE_TIME_TO={2}", sdo.INTRUCTION_DATE, sdo.USE_TIME, sdo.USE_TIME_TO));
                        }
                        else
                        {
                            LogPhase(string.Format("Bo qua dot INTRUCTION_DATE={0} vi khong co dong thuoc nao", item.Key));
                        }


                        //expMestMedicineReq = new List<ExpMestMedicineSDO>();
                        //ExpMestMedicineSDO mediSDO = new ExpMestMedicineSDO();
                        //var expMestMedicine = rdo.expMestMedicines.Where(o=>o.TDL_SERVICE_REQ_ID == item.ID).ToList();
                        //foreach(var mediMest in expMestMedicine)
                        //{            
                        //    mediSDO.MEDICINE_TYPE_NAME = mediMest.MEDICINE_TYPE_NAME != null ? mediMest.MEDICINE_TYPE_NAME : "";
                        //    mediSDO.AMOUNT = mediMest.AMOUNT > 0 ? mediMest.AMOUNT : 0;
                        //    mediSDO.TUTORIAL = mediMest.TUTORIAL != null ? mediMest.TUTORIAL : "";
                        //    mediSDO.TDL_SERVICE_REQ_ID = mediMest.TDL_SERVICE_REQ_ID ?? 0;
                        //    expMestMedicineReq.Add(mediSDO);
                        //}

                        //var serviceReqMety = rdo.lstServiceReqMety.ToList();
                        //foreach(var serviceMety in serviceReqMety)
                        //{            
                        //    mediSDO.MEDICINE_TYPE_CODE = serviceMety.MEDICINE_TYPE_NAME != null ? serviceMety.MEDICINE_TYPE_NAME : "";
                        //    //mediSDO.CONCENTRA = serviceReqMety.CONCENTRA != null ? expMestMedicine.CONCENTRA : "";
                        //    mediSDO.AMOUNT = serviceMety.AMOUNT > 0 ? serviceMety.AMOUNT : 0;
                        //    mediSDO.TUTORIAL = serviceMety.TUTORIAL != null ? serviceMety.TUTORIAL : "";
                        //    //mediSDO.SERVICE_UNIT_NAME = serviceMety.SERVICE_UNIT_NAME != null ? serviceMety.SERVICE_UNIT_NAME : "";
                        //    expMestMedicineReq.Add(mediSDO);
                        //}
                    }

                }

                LogPhase(string.Format("Ket qua ProcessListData: Phase={0} dot, Medicine={1} dong, type={2}",
                    serviceReqSdo.Count,
                    expMestMedicineReq.Count,
                    expMestMedicinesTYPE == null ? "null" : expMestMedicinesTYPE.Count.ToString()));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }

    public class HeinCardHelper
    {
        public static string SetHeinCardNumberDisplayByNumber(string heinCardNumber)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrWhiteSpace(heinCardNumber) && (heinCardNumber.Length == 15 || heinCardNumber.Length == 17))
                {
                    string separateSymbol = "-";
                    result = heinCardNumber.Length == 17 ? new StringBuilder().Append(heinCardNumber.Substring(0, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(2, 1)).Append(separateSymbol).Append(heinCardNumber.Substring(3, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(5)).ToString() : new StringBuilder().Append(heinCardNumber.Substring(0, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(2, 1)).Append(separateSymbol).Append(heinCardNumber.Substring(3, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(5, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(7, 3)).Append(separateSymbol).Append(heinCardNumber.Substring(10, 5)).ToString();
                }
                else
                {
                    result = heinCardNumber;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = heinCardNumber;
            }
            return result;
        }

        public static string TrimHeinCardNumber(string chucodau)
        {
            string result = "";
            try
            {
                result = System.Text.RegularExpressions.Regex.Replace(chucodau, @"[-,_ ]|[_]{2}|[_]{3}|[_]{4}|[_]{5}", "").ToUpper();
            }
            catch (Exception ex)
            {

            }

            return result;
        }
    }
}
