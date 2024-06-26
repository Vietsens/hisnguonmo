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
                SetBarcodeKey();

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                barCodeTag.ProcessData(store, dicImage);
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
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
