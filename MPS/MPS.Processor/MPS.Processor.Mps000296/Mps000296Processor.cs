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
using MPS.Processor.Mps000296.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000296
{
    class Mps000296Processor: AbstractProcessor
    {
        List<ExpMestMedicineSDO> ExpMests { get; set; }
        Mps000296PDO rdo;

        public Mps000296Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000296PDO)rdoBase;
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
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                this.ExpMestMedicineGroup();
                ProcessBarcodeKey();
                ProcessSingleKey();

                if (store.ReadTemplate(System.IO.Path.GetFullPath(fileName)))
                {
                    singleTag.ProcessData(store, singleValueDictionary);
                    barCodeTag.ProcessData(store, dicImage);

                    if (this.ExpMests == null)
                        return false;

                    this.ExpMests = this.ExpMests.OrderBy(o => o.ID).ToList();

                    objectTag.AddObjectData(store, "MedicineExpmest", rdo.expMestMedicines);
                    objectTag.AddObjectData(store, "ExpMest", this.ExpMests);
                    objectTag.AddRelationship(store, "ExpMest", "MedicineExpmest", "EXP_MEST_ID", "EXP_MEST_ID");
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void ExpMestMedicineGroup()
        {
            try
            {
                ExpMests = new List<ExpMestMedicineSDO>();
                if (rdo.expMestMedicines == null || rdo.expMestMedicines.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("rdo.expMestMedicines == null");
                    return;
                }

                var expMestMedicineGroups = rdo.expMestMedicines.GroupBy(o => o.EXP_MEST_ID);
                foreach (var item in expMestMedicineGroups)
                {
                    ExpMests.Add(item.First());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessSingleKey()
        {
            try
            {
                if (rdo.Mps000296ADO != null)
                {
                    AddObjectKeyIntoListkey<Mps000296ADO>(rdo.Mps000296ADO, false);
                }
                if (rdo.hisDhst != null)
                {
                    AddObjectKeyIntoListkey<HIS_DHST>(rdo.hisDhst, false);
                }

                if (rdo.HisPrescription != null)
                {
                    if (rdo.expMestMedicines != null && rdo.expMestMedicines.Count > 0)
                    {
                        decimal tong = 0;
                        foreach (var item in rdo.expMestMedicines)
                        {
                            tong += item.AMOUNT * ((item.PRICE ?? 0) * (1 + (item.VAT_RATIO ?? 0)));
                        }
                        SetSingleKey(new KeyValue(Mps000296ExtendSingleKey.TOTAL_PRICE_PRESCRIPTION, tong));

                        //nếu không có use_time_to trong V_HIS_PRESCRIPTION thì lấy trong ds thuốc 
                        if (rdo.HisPrescription != null && !rdo.HisPrescription.USE_TIME_TO.HasValue)
                        {
                            var maxUseTimeTo = rdo.expMestMedicines.Where(o => o.USE_TIME_TO.HasValue).ToList();
                            if (maxUseTimeTo != null && maxUseTimeTo.Count > 0)
                            {
                                var useTimeTo = maxUseTimeTo.Max(m => m.USE_TIME_TO).Value;
                                SetSingleKey(new KeyValue(Mps000296ExtendSingleKey.USE_TIME_TO_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(useTimeTo)));
                            }
                        }
                        else
                            SetSingleKey(new KeyValue(Mps000296ExtendSingleKey.USE_TIME_TO_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.HisPrescription.USE_TIME_TO ?? 0)));

                        if (rdo.HisPrescription != null && rdo.HisPrescription.USE_TIME.HasValue)
                        {
                            SetSingleKey(new KeyValue(Mps000296ExtendSingleKey.USER_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.HisPrescription.USE_TIME ?? 0)));
                        }
                        else
                        {
                            SetSingleKey(new KeyValue(Mps000296ExtendSingleKey.USER_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.HisPrescription.INTRUCTION_TIME)));
                        }
                    }

                    SetSingleKey(new KeyValue(Mps000296ExtendSingleKey.INTRUCTION_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.HisPrescription.INTRUCTION_TIME)));
                    SetSingleKey(new KeyValue(Mps000296ExtendSingleKey.INTRUCTION_TIME_FULL_SRT, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo.HisPrescription.INTRUCTION_TIME)));

                    SetSingleKey((new KeyValue(Mps000296ExtendSingleKey.AGE, Inventec.Common.DateTime.Calculation.AgeCaption(rdo.HisPrescription.TDL_PATIENT_DOB))));
                    SetSingleKey((new KeyValue(Mps000296ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.HisPrescription.TDL_PATIENT_DOB))));
                    SetSingleKey((new KeyValue(Mps000296ExtendSingleKey.D_O_B, rdo.HisPrescription.TDL_PATIENT_DOB.ToString().Substring(0, 4))));

                    SetSingleKey(new KeyValue(Mps000296ExtendSingleKey.NATIONAL_NAME, rdo.HisPrescription.TDL_PATIENT_NATIONAL_NAME));
                    SetSingleKey(new KeyValue(Mps000296ExtendSingleKey.WORK_PLACE, rdo.HisPrescription.TDL_PATIENT_WORK_PLACE_NAME));
                    SetSingleKey(new KeyValue(Mps000296ExtendSingleKey.ADDRESS, rdo.HisPrescription.TDL_PATIENT_ADDRESS));
                    SetSingleKey(new KeyValue(Mps000296ExtendSingleKey.CAREER_NAME, rdo.HisPrescription.TDL_PATIENT_CAREER_NAME));
                    SetSingleKey(new KeyValue(Mps000296ExtendSingleKey.PATIENT_CODE, rdo.HisPrescription.TDL_PATIENT_CODE));
                    SetSingleKey(new KeyValue(Mps000296ExtendSingleKey.DISTRICT_CODE, rdo.HisPrescription.TDL_PATIENT_DISTRICT_CODE));
                    SetSingleKey(new KeyValue(Mps000296ExtendSingleKey.GENDER_NAME, rdo.HisPrescription.TDL_PATIENT_GENDER_NAME));
                    SetSingleKey(new KeyValue(Mps000296ExtendSingleKey.MILITARY_RANK_NAME, rdo.HisPrescription.TDL_PATIENT_MILITARY_RANK_NAME));
                    SetSingleKey(new KeyValue(Mps000296ExtendSingleKey.DOB, rdo.HisPrescription.TDL_PATIENT_DOB));
                    SetSingleKey(new KeyValue(Mps000296ExtendSingleKey.VIR_ADDRESS, rdo.HisPrescription.TDL_PATIENT_ADDRESS));
                    SetSingleKey(new KeyValue(Mps000296ExtendSingleKey.VIR_PATIENT_NAME, rdo.HisPrescription.TDL_PATIENT_NAME));
                }

                if (rdo.vHisPatientTypeAlter != null)
                {
                    if (!String.IsNullOrEmpty(rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER))
                    {
                        SetSingleKey((new KeyValue(Mps000296ExtendSingleKey.HEIN_CARD_NUMBER_SEPARATE, HeinCardHelper.SetHeinCardNumberDisplayByNumber(rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER))));
                        SetSingleKey((new KeyValue(Mps000296ExtendSingleKey.IS_HEIN, "X")));
                        SetSingleKey((new KeyValue(Mps000296ExtendSingleKey.IS_VIENPHI, "")));
                        SetSingleKey((new KeyValue(Mps000296ExtendSingleKey.HEIN_CARD_NUMBER_1, rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER.Substring(0, 2))));
                        SetSingleKey((new KeyValue(Mps000296ExtendSingleKey.HEIN_CARD_NUMBER_2, rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER.Substring(2, 1))));
                        SetSingleKey((new KeyValue(Mps000296ExtendSingleKey.HEIN_CARD_NUMBER_3, rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER.Substring(3, 2))));
                        SetSingleKey((new KeyValue(Mps000296ExtendSingleKey.HEIN_CARD_NUMBER_4, rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER.Substring(5, 2))));
                        SetSingleKey((new KeyValue(Mps000296ExtendSingleKey.HEIN_CARD_NUMBER_5, rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER.Substring(7, 3))));
                        SetSingleKey((new KeyValue(Mps000296ExtendSingleKey.HEIN_CARD_NUMBER_6, rdo.vHisPatientTypeAlter.HEIN_CARD_NUMBER.Substring(10, 5))));
                        SetSingleKey(new KeyValue(Mps000296ExtendSingleKey.STR_HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0)));
                        SetSingleKey(new KeyValue(Mps000296ExtendSingleKey.STR_HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.vHisPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0)));
                    }
                    else
                    {
                        SetSingleKey((new KeyValue(Mps000296ExtendSingleKey.IS_HEIN, "")));
                        SetSingleKey((new KeyValue(Mps000296ExtendSingleKey.IS_VIENPHI, "X")));
                    }
                }

                AddObjectKeyIntoListkey<HIS_SERVICE_REQ>(rdo.HisPrescription, false);
                AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo.vHisPatientTypeAlter);
                //AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo.vHisPatient);
                if (rdo.hisServiceReq_Exam != null)
                {
                    AddObjectKeyIntoListkey<HIS_SERVICE_REQ>(rdo.hisServiceReq_Exam, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ProcessBarcodeKey()
        {
            try
            {
                if (rdo.HisPrescription != null)
                {
                    Inventec.Common.BarcodeLib.Barcode barcodeTreatmentCode = new Inventec.Common.BarcodeLib.Barcode(rdo.HisPrescription.TDL_TREATMENT_CODE);
                    barcodeTreatmentCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeTreatmentCode.IncludeLabel = false;
                    barcodeTreatmentCode.Width = 120;
                    barcodeTreatmentCode.Height = 40;
                    barcodeTreatmentCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeTreatmentCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeTreatmentCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeTreatmentCode.IncludeLabel = true;

                    dicImage.Add(Mps000296ExtendSingleKey.TREATMENT_CODE_BARCODE, barcodeTreatmentCode);

                    Inventec.Common.BarcodeLib.Barcode expMestCodeBarCode = new Inventec.Common.BarcodeLib.Barcode(rdo.Mps000296ADO.EXP_MEST_CODE);
                    expMestCodeBarCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    expMestCodeBarCode.IncludeLabel = false;
                    expMestCodeBarCode.Width = 120;
                    expMestCodeBarCode.Height = 40;
                    expMestCodeBarCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    expMestCodeBarCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    expMestCodeBarCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    expMestCodeBarCode.IncludeLabel = true;

                    dicImage.Add(Mps000296ExtendSingleKey.EXP_MEST_CODE_BARCODE, expMestCodeBarCode);
                    Inventec.Common.BarcodeLib.Barcode barcodePatient = new Inventec.Common.BarcodeLib.Barcode(rdo.HisPrescription.TDL_PATIENT_CODE);
                    barcodePatient.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodePatient.IncludeLabel = false;
                    barcodePatient.Width = 120;
                    barcodePatient.Height = 40;
                    barcodePatient.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodePatient.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodePatient.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodePatient.IncludeLabel = true;

                    dicImage.Add(Mps000296ExtendSingleKey.PATIENT_CODE_BARCODE, barcodePatient);
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
