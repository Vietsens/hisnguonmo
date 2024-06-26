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
using FlexCel.Report;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000050.PDO;
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
namespace MPS.Processor.Mps000050
{
    class Mps000050Processor : AbstractProcessor
    {
        Mps000050PDO rdo;
        private List<ExpMestMedicineSDO> expMestMedicinesTYPE;
        private List<ExpMestMedicineSDO> expMestMedicines_Sort;


        public Mps000050Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000050PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {
                if (rdo.vHisPrescription5 != null)
                {
                    if (!String.IsNullOrEmpty(rdo.vHisPrescription5.TDL_TREATMENT_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.vHisPrescription5.TDL_TREATMENT_CODE);
                        barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeTreatment.IncludeLabel = false;
                        barcodeTreatment.Width = 120;
                        barcodeTreatment.Height = 40;
                        barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeTreatment.IncludeLabel = true;
                        dicImage.Add(Mps000050ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
                    }

                    if (!String.IsNullOrEmpty(rdo.Mps000050ADO.EXP_MEST_CODE))
                    {
                        Inventec.Common.BarcodeLib.Barcode barcodeExpMest = new Inventec.Common.BarcodeLib.Barcode(rdo.Mps000050ADO.EXP_MEST_CODE);
                        barcodeExpMest.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                        barcodeExpMest.IncludeLabel = false;
                        barcodeExpMest.Width = 120;
                        barcodeExpMest.Height = 40;
                        barcodeExpMest.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                        barcodeExpMest.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                        barcodeExpMest.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                        barcodeExpMest.IncludeLabel = true;

                        dicImage.Add(Mps000050ExtendSingleKey.EXP_MEST_CODE_BAR, barcodeExpMest);
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
                        dicImage.Add(Mps000050ExtendSingleKey.PATIENT_CODE_BAR, barcodePatient);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                    Inventec.Common.Logging.LogSystem.Warn("MPS000050____________" + rdo.KeyUseForm);   
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
        /// <summary>
        /// Ham xu ly du lieu da qua xu ly
        /// Tao ra cac doi tuong du lieu xu dung trong thu vien xu ly file excel
        /// </summary>
        /// <returns></returns>
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                SetBarcodeKey();
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                SetSingleKey();
                ProcessListData();
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                this.SetSignatureKeyImageByCFG();
                MedicinesSort();
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                if (rdo.expMestMedicines != null && rdo.expMestMedicines.Count > 0)
                {
                    objectTag.AddObjectData(store, "type", expMestMedicinesTYPE);
                    objectTag.AddObjectData(store, "ServiceMedicines", expMestMedicines_Sort);
                    objectTag.AddRelationship(store, "type", "ServiceMedicines", "PATIENT_TYPE_NAME", "PATIENT_TYPE_NAME");
                }

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

        void SetSingleKey()
        {
            try
            {
                if (rdo.vHisPrescription5 != null)
                {
                    SetSingleKey(new KeyValue(Mps000050ExtendSingleKey.USE_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPrescription5.USE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000050ExtendSingleKey.USE_DATE_FROM_1, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPrescription5.INTRUCTION_TIME)));
                    SetSingleKey(Mps000050ExtendSingleKey.USE_DATE_TO_1, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPrescription5.USE_TIME ?? 0));

                    if (rdo.expMestMedicines != null && rdo.expMestMedicines.Count > 0)
                    {

                        decimal tong = 0;
                        foreach (var item in rdo.expMestMedicines)
                        {
                            tong += item.AMOUNT * ((item.PRICE ?? 0) * (1 + (item.VAT_RATIO ?? 0)));
                        }
                        SetSingleKey(new KeyValue(Mps000050ExtendSingleKey.TOTAL_PRICE_PRESCRIPTION, tong));

                        if (!rdo.vHisPrescription5.USE_TIME_TO.HasValue)
                        {
                            var maxUseTimeTo = rdo.expMestMedicines.Where(o => o.USE_TIME_TO.HasValue).ToList();
                            if (maxUseTimeTo != null && maxUseTimeTo.Count > 0)
                            {
                                var useTimeTo = maxUseTimeTo.Max(m => m.USE_TIME_TO).Value;
                                SetSingleKey(new KeyValue(Mps000050ExtendSingleKey.USE_TIME_TO_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(useTimeTo)));
                            }
                        }
                        else
                            SetSingleKey(new KeyValue(Mps000050ExtendSingleKey.USE_TIME_TO_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPrescription5.USE_TIME_TO ?? 0)));

                        if (rdo.vHisPrescription5.USE_TIME.HasValue)
                        {
                            SetSingleKey(new KeyValue(Mps000050ExtendSingleKey.USER_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPrescription5.USE_TIME ?? 0)));
                        }
                        else
                        {
                            SetSingleKey(new KeyValue(Mps000050ExtendSingleKey.USER_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPrescription5.INTRUCTION_TIME)));
                        }
                    }

                    SetSingleKey(new KeyValue(Mps000050ExtendSingleKey.INTRUCTION_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPrescription5.INTRUCTION_TIME)));
                    SetSingleKey(new KeyValue(Mps000050ExtendSingleKey.INTRUCTION_TIME_FULL_SRT, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo.vHisPrescription5.INTRUCTION_TIME)));

                    SetSingleKey((new KeyValue(Mps000050ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo.vHisPrescription5.TDL_PATIENT_DOB))));
                    SetSingleKey((new KeyValue(Mps000050ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPrescription5.TDL_PATIENT_DOB))));
                    SetSingleKey((new KeyValue(Mps000050ExtendSingleKey.D_O_B, rdo.vHisPrescription5.TDL_PATIENT_DOB.ToString().Substring(0, 4))));

                    SetSingleKey(new KeyValue(Mps000050ExtendSingleKey.NATIONAL_NAME, rdo.vHisPrescription5.TDL_PATIENT_NATIONAL_NAME));
                    SetSingleKey(new KeyValue(Mps000050ExtendSingleKey.WORK_PLACE, rdo.vHisPrescription5.TDL_PATIENT_WORK_PLACE_NAME));
                    SetSingleKey(new KeyValue(Mps000050ExtendSingleKey.ADDRESS, rdo.vHisPrescription5.TDL_PATIENT_ADDRESS));
                    SetSingleKey(new KeyValue(Mps000050ExtendSingleKey.CAREER_NAME, rdo.vHisPrescription5.TDL_PATIENT_CAREER_NAME));
                    SetSingleKey(new KeyValue(Mps000050ExtendSingleKey.PATIENT_CODE, rdo.vHisPrescription5.TDL_PATIENT_CODE));
                    SetSingleKey(new KeyValue(Mps000050ExtendSingleKey.DISTRICT_CODE, rdo.vHisPrescription5.TDL_PATIENT_DISTRICT_CODE));
                    SetSingleKey(new KeyValue(Mps000050ExtendSingleKey.GENDER_NAME, rdo.vHisPrescription5.TDL_PATIENT_GENDER_NAME));
                    SetSingleKey(new KeyValue(Mps000050ExtendSingleKey.MILITARY_RANK_NAME, rdo.vHisPrescription5.TDL_PATIENT_MILITARY_RANK_NAME));
                    SetSingleKey(new KeyValue(Mps000050ExtendSingleKey.DOB, rdo.vHisPrescription5.TDL_PATIENT_DOB));
                    SetSingleKey(new KeyValue(Mps000050ExtendSingleKey.VIR_ADDRESS, rdo.vHisPrescription5.TDL_PATIENT_ADDRESS));
                    SetSingleKey(new KeyValue(Mps000050ExtendSingleKey.VIR_PATIENT_NAME, rdo.vHisPrescription5.TDL_PATIENT_NAME));
                }

                if (rdo.expMestMedicines != null && rdo.expMestMedicines.Count > 0)
                {
                    SetSingleKey(new KeyValue(Mps000050ExtendSingleKey.USE_DATE_TO, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.expMestMedicines.Max(o => o.USE_TIME_TO ?? 0))));
                }

                if (rdo.vHisPatientTypeAlter != null)
                {
                    if (!String.IsNullOrEmpty(rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER))
                    {
                        SetSingleKey((new KeyValue(Mps000050ExtendSingleKey.HEIN_CARD_NUMBER_SEPARATE, HeinCardHelper.SetHeinCardNumberDisplayByNumber(rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER))));
                        SetSingleKey((new KeyValue(Mps000050ExtendSingleKey.IS_HEIN, "X")));
                        SetSingleKey((new KeyValue(Mps000050ExtendSingleKey.IS_VIENPHI, "")));
                        SetSingleKey((new KeyValue(Mps000050ExtendSingleKey.HEIN_CARD_NUMBER_1, rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER.Substring(0, 2))));
                        SetSingleKey((new KeyValue(Mps000050ExtendSingleKey.HEIN_CARD_NUMBER_2, rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER.Substring(2, 1))));
                        SetSingleKey((new KeyValue(Mps000050ExtendSingleKey.HEIN_CARD_NUMBER_3, rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER.Substring(3, 2))));
                        SetSingleKey((new KeyValue(Mps000050ExtendSingleKey.HEIN_CARD_NUMBER_4, rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER.Substring(5, 2))));
                        SetSingleKey((new KeyValue(Mps000050ExtendSingleKey.HEIN_CARD_NUMBER_5, rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER.Substring(7, 3))));
                        SetSingleKey((new KeyValue(Mps000050ExtendSingleKey.HEIN_CARD_NUMBER_6, rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER.Substring(10, 5))));
                        SetSingleKey(new KeyValue(Mps000050ExtendSingleKey.STR_HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0)));
                        SetSingleKey(new KeyValue(Mps000050ExtendSingleKey.STR_HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0)));
                    }
                    else
                    {
                        SetSingleKey((new KeyValue(Mps000050ExtendSingleKey.IS_HEIN, "")));
                        SetSingleKey((new KeyValue(Mps000050ExtendSingleKey.IS_VIENPHI, "X")));
                    }
                }

                if (rdo.hisTreatment != null)
                {
                    SetSingleKey((new KeyValue("APPOINTMENT_CODE", rdo.hisTreatment.APPOINTMENT_CODE)));
                    SetSingleKey((new KeyValue("APPOINTMENT_DATE", rdo.hisTreatment.APPOINTMENT_DATE)));
                    SetSingleKey((new KeyValue("APPOINTMENT_DESC", rdo.hisTreatment.APPOINTMENT_DESC)));
                    SetSingleKey((new KeyValue("APPOINTMENT_SURGERY", rdo.hisTreatment.APPOINTMENT_SURGERY)));
                    SetSingleKey((new KeyValue("APPOINTMENT_TIME", rdo.hisTreatment.APPOINTMENT_TIME)));
                    SetSingleKey((new KeyValue("APPOINTMENT_EXAM_ROOM_IDS", rdo.hisTreatment.APPOINTMENT_EXAM_ROOM_IDS)));
                }

                AddObjectKeyIntoListkey<HIS_SERVICE_REQ>(rdo.vHisPrescription5, false);
                if (rdo.Mps000050ADO != null)
                {
                    AddObjectKeyIntoListkey<Mps000050ADO>(rdo.Mps000050ADO, false);
                }
                AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo.vHisPatientTypeAlter, false);
                AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.hisTreatment, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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
