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
using MPS.Processor.Mps000238.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPS.Processor.Mps000238
{
    public class Mps000238Processor: AbstractProcessor
    {
        Mps000238PDO rdo;
        private List<ExpMestMedicineSDO> expMestMedicinesTYPE;

        public Mps000238Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000238PDO)rdoBase;
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
                        barcodeTreatmentCode.RotateFlipType = System.Drawing.RotateFlipType.Rotate180FlipXY;
                        barcodeTreatmentCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeTreatmentCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeTreatmentCode.IncludeLabel = true;

                        dicImage.Add(Mps000238ExtendSingleKey.TREATMENT_CODE_BARCODE, barcodeTreatmentCode);
                    }

                    if (!String.IsNullOrEmpty(rdo.Mps000238ADO.EXP_MEST_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode expMestCodeBarCode = new Inventec.Common.BarcodeLib.Barcode(rdo.Mps000238ADO.EXP_MEST_CODE);
                        expMestCodeBarCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        expMestCodeBarCode.IncludeLabel = false;
                        expMestCodeBarCode.Width = 120;
                        expMestCodeBarCode.Height = 40;
                        expMestCodeBarCode.RotateFlipType = System.Drawing.RotateFlipType.Rotate180FlipXY;
                        expMestCodeBarCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        expMestCodeBarCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        expMestCodeBarCode.IncludeLabel = true;

                        dicImage.Add(Mps000238ExtendSingleKey.EXP_MEST_CODE_BARCODE, expMestCodeBarCode);
                    }

                    if (!String.IsNullOrEmpty(rdo.vHisPrescription5.TDL_PATIENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodePatient = new Inventec.Common.BarcodeLib.Barcode(rdo.vHisPrescription5.TDL_PATIENT_CODE);
                        barcodePatient.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodePatient.IncludeLabel = false;
                        barcodePatient.Width = 120;
                        barcodePatient.Height = 40;
                        barcodePatient.RotateFlipType = System.Drawing.RotateFlipType.Rotate180FlipXY;
                        barcodePatient.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodePatient.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodePatient.IncludeLabel = true;

                        dicImage.Add(Mps000238ExtendSingleKey.PATIENT_CODE_BARCODE, barcodePatient);
                    }

                    if (!String.IsNullOrEmpty(rdo.vHisPrescription5.SERVICE_REQ_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeServiceReq = new Inventec.Common.BarcodeLib.Barcode(rdo.vHisPrescription5.SERVICE_REQ_CODE);
                        barcodeServiceReq.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeServiceReq.IncludeLabel = false;
                        barcodeServiceReq.Width = 120;
                        barcodeServiceReq.Height = 40;
                        barcodeServiceReq.RotateFlipType = System.Drawing.RotateFlipType.Rotate180FlipXY;
                        barcodeServiceReq.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeServiceReq.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeServiceReq.IncludeLabel = true;
                        dicImage.Add(Mps000238ExtendSingleKey.SERVICE_REQ_CODE_BAR, barcodeServiceReq);
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
                if (rdo.Mps000238ADO != null)
                {
                    AddObjectKeyIntoListkey<Mps000238ADO>(rdo.Mps000238ADO, false);
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
                        SetSingleKey(new KeyValue(Mps000238ExtendSingleKey.TOTAL_PRICE_PRESCRIPTION, tong));

                        //nếu không có use_time_to trong V_HIS_PRESCRIPTION thì lấy trong ds thuốc 
                        if (!rdo.vHisPrescription5.USE_TIME_TO.HasValue)
                        {
                            var maxUseTimeTo = rdo.expMestMedicines.Where(o => o.USE_TIME_TO.HasValue).ToList();
                            if (maxUseTimeTo != null && maxUseTimeTo.Count > 0)
                            {
                                var useTimeTo = maxUseTimeTo.Max(m => m.USE_TIME_TO).Value;
                                SetSingleKey(new KeyValue(Mps000238ExtendSingleKey.USE_TIME_TO_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(useTimeTo)));
                            }
                        }
                        else
                            SetSingleKey(new KeyValue(Mps000238ExtendSingleKey.USE_TIME_TO_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPrescription5.USE_TIME_TO ?? 0)));

                        if (rdo.vHisPrescription5.USE_TIME.HasValue)
                        {
                            SetSingleKey(new KeyValue(Mps000238ExtendSingleKey.USER_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPrescription5.USE_TIME ?? 0)));
                        }
                        else
                        {
                            SetSingleKey(new KeyValue(Mps000238ExtendSingleKey.USER_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPrescription5.INTRUCTION_TIME)));
                        }

                        SetSingleKey(new KeyValue(Mps000238ExtendSingleKey.INTRUCTION_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPrescription5.INTRUCTION_TIME)));
                        SetSingleKey(new KeyValue(Mps000238ExtendSingleKey.INTRUCTION_TIME_FULL_SRT, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo.vHisPrescription5.INTRUCTION_TIME)));
                    }

                    SetSingleKey((new KeyValue(Mps000238ExtendSingleKey.AGE, Inventec.Common.DateTime.Calculation.AgeCaption(rdo.vHisPrescription5.TDL_PATIENT_DOB))));
                    SetSingleKey((new KeyValue(Mps000238ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPrescription5.TDL_PATIENT_DOB))));
                    SetSingleKey((new KeyValue(Mps000238ExtendSingleKey.D_O_B, rdo.vHisPrescription5.TDL_PATIENT_DOB.ToString().Substring(0, 4))));

                    SetSingleKey(new KeyValue(Mps000238ExtendSingleKey.NATIONAL_NAME, rdo.vHisPrescription5.TDL_PATIENT_NATIONAL_NAME));
                    SetSingleKey(new KeyValue(Mps000238ExtendSingleKey.WORK_PLACE, rdo.vHisPrescription5.TDL_PATIENT_WORK_PLACE_NAME));
                    SetSingleKey(new KeyValue(Mps000238ExtendSingleKey.ADDRESS, rdo.vHisPrescription5.TDL_PATIENT_ADDRESS));
                    SetSingleKey(new KeyValue(Mps000238ExtendSingleKey.CAREER_NAME, rdo.vHisPrescription5.TDL_PATIENT_CAREER_NAME));
                    SetSingleKey(new KeyValue(Mps000238ExtendSingleKey.PATIENT_CODE, rdo.vHisPrescription5.TDL_PATIENT_CODE));
                    SetSingleKey(new KeyValue(Mps000238ExtendSingleKey.DISTRICT_CODE, rdo.vHisPrescription5.TDL_PATIENT_DISTRICT_CODE));
                    SetSingleKey(new KeyValue(Mps000238ExtendSingleKey.GENDER_NAME, rdo.vHisPrescription5.TDL_PATIENT_GENDER_NAME));
                    SetSingleKey(new KeyValue(Mps000238ExtendSingleKey.MILITARY_RANK_NAME, rdo.vHisPrescription5.TDL_PATIENT_MILITARY_RANK_NAME));
                    SetSingleKey(new KeyValue(Mps000238ExtendSingleKey.DOB, rdo.vHisPrescription5.TDL_PATIENT_DOB));
                    SetSingleKey(new KeyValue(Mps000238ExtendSingleKey.VIR_ADDRESS, rdo.vHisPrescription5.TDL_PATIENT_ADDRESS));
                    SetSingleKey(new KeyValue(Mps000238ExtendSingleKey.VIR_PATIENT_NAME, rdo.vHisPrescription5.TDL_PATIENT_NAME));
                }

                if (rdo.PatyAlterBhyt != null)
                {
                    if (!String.IsNullOrEmpty(rdo.PatyAlterBhyt.HEIN_CARD_NUMBER))
                    {
                        SetSingleKey((new KeyValue(Mps000238ExtendSingleKey.HEIN_CARD_NUMBER_SEPARATE, HeinCardHelper.SetHeinCardNumberDisplayByNumber(rdo.PatyAlterBhyt.HEIN_CARD_NUMBER))));
                        SetSingleKey((new KeyValue(Mps000238ExtendSingleKey.IS_HEIN, "X")));
                        SetSingleKey((new KeyValue(Mps000238ExtendSingleKey.IS_VIENPHI, "")));
                        SetSingleKey((new KeyValue(Mps000238ExtendSingleKey.HEIN_CARD_NUMBER_1, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(0, 2))));
                        SetSingleKey((new KeyValue(Mps000238ExtendSingleKey.HEIN_CARD_NUMBER_2, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(2, 1))));
                        SetSingleKey((new KeyValue(Mps000238ExtendSingleKey.HEIN_CARD_NUMBER_3, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(3, 2))));
                        SetSingleKey((new KeyValue(Mps000238ExtendSingleKey.HEIN_CARD_NUMBER_4, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(5, 2))));
                        SetSingleKey((new KeyValue(Mps000238ExtendSingleKey.HEIN_CARD_NUMBER_5, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(7, 3))));
                        SetSingleKey((new KeyValue(Mps000238ExtendSingleKey.HEIN_CARD_NUMBER_6, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(10, 5))));
                        SetSingleKey(new KeyValue(Mps000238ExtendSingleKey.STR_HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.PatyAlterBhyt.HEIN_CARD_FROM_TIME ?? 0)));
                        SetSingleKey(new KeyValue(Mps000238ExtendSingleKey.STR_HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.PatyAlterBhyt.HEIN_CARD_TO_TIME ?? 0)));
                    }
                    else
                    {
                        SetSingleKey((new KeyValue(Mps000238ExtendSingleKey.IS_HEIN, "")));
                        SetSingleKey((new KeyValue(Mps000238ExtendSingleKey.IS_VIENPHI, "X")));
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
                AddObjectKeyIntoListkey<HIS_SERVICE_REQ>(rdo.vHisPrescription5, false);
                AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo.PatyAlterBhyt);
                if (rdo.HisTreatment != null)
                {
                    AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.HisTreatment, false);
                }

                if (rdo.hisServiceReq_Exam != null)
                {
                    SetSingleKey((new KeyValue(Mps000238ExtendSingleKey.PART_EXAM_EYE_STR, ProcessDataEye(rdo.hisServiceReq_Exam.PART_EXAM_EYE))));
                    SetSingleKey((new KeyValue(Mps000238ExtendSingleKey.PART_EXAM_EYE_TENSION_LEFT_STR, ProcessDataEye(rdo.hisServiceReq_Exam.PART_EXAM_EYE_TENSION_LEFT))));
                    SetSingleKey((new KeyValue(Mps000238ExtendSingleKey.PART_EXAM_EYE_TENSION_RIGHT_STR, ProcessDataEye(rdo.hisServiceReq_Exam.PART_EXAM_EYE_TENSION_RIGHT))));
                    SetSingleKey((new KeyValue(Mps000238ExtendSingleKey.PART_EXAM_EYESIGHT_LEFT_STR, ProcessDataEye(rdo.hisServiceReq_Exam.PART_EXAM_EYESIGHT_LEFT))));
                    SetSingleKey((new KeyValue(Mps000238ExtendSingleKey.PART_EXAM_EYESIGHT_RIGHT_STR, ProcessDataEye(rdo.hisServiceReq_Exam.PART_EXAM_EYESIGHT_RIGHT))));
                    SetSingleKey((new KeyValue(Mps000238ExtendSingleKey.PART_EXAM_EYESIGHT_GLASS_LEFT_STR, ProcessDataEye(rdo.hisServiceReq_Exam.PART_EXAM_EYESIGHT_GLASS_LEFT))));
                    SetSingleKey((new KeyValue(Mps000238ExtendSingleKey.PART_EXAM_EYESIGHT_GLASS_RIGHT_STR, ProcessDataEye(rdo.hisServiceReq_Exam.PART_EXAM_EYESIGHT_GLASS_RIGHT))));
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

                SetSingleKey((new KeyValue(Mps000238ExtendSingleKey.TT_SERVICE_NAME, ttServiceNames)));
                SetSingleKey((new KeyValue(Mps000238ExtendSingleKey.CLS_SERVICE_NAME, clsServiceNames)));
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

                SetBarcodeKey();
                SetSingleKey();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "type", expMestMedicinesTYPE);
                objectTag.AddObjectData(store, "type2", expMestMedicinesTYPE);
                objectTag.AddObjectData(store, "type3", expMestMedicinesTYPE);
                objectTag.AddObjectData(store, "MedicineExpmest", rdo.expMestMedicines);
                objectTag.AddObjectData(store, "list2", rdo.expMestMedicines);
                objectTag.AddObjectData(store, "list3", rdo.expMestMedicines);
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
                if (!String.IsNullOrWhiteSpace(heinCardNumber) && heinCardNumber.Length == 15)
                {
                    string separateSymbol = "-";
                    result = new StringBuilder().Append(heinCardNumber.Substring(0, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(2, 1)).Append(separateSymbol).Append(heinCardNumber.Substring(3, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(5, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(7, 3)).Append(separateSymbol).Append(heinCardNumber.Substring(10, 5)).ToString();
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
