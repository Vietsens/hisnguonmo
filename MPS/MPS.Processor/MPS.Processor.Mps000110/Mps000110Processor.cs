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
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using MPS.Processor.Mps000110.PDO;
using MOS.EFMODEL.DataModels;

namespace MPS.Processor.Mps000110
{
    public class Mps000110Processor : AbstractProcessor
    {
        Mps000110PDO rdo;
        public Mps000110Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000110PDO)rdoBase;
        }

        private void SetBarcodeKey()
        {
            try
            {
                if (rdo.currentHisTreatment != null && rdo.currentHisTreatment.TREATMENT_CODE != null)
                {
                    Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.currentHisTreatment.TREATMENT_CODE);
                    barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeTreatment.IncludeLabel = false;
                    barcodeTreatment.Width = 120;
                    barcodeTreatment.Height = 40;
                    barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeTreatment.IncludeLabel = true;

                    dicImage.Add(Mps000110ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatment);
                }

                if (rdo.patientADO != null && rdo.patientADO.PATIENT_CODE != null)
                {
                    Inventec.Common.BarcodeLib.Barcode barcodePatient = new Inventec.Common.BarcodeLib.Barcode(rdo.patientADO.PATIENT_CODE);
                    barcodePatient.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodePatient.IncludeLabel = false;
                    barcodePatient.Width = 120;
                    barcodePatient.Height = 40;
                    barcodePatient.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodePatient.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodePatient.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodePatient.IncludeLabel = true;

                    dicImage.Add(Mps000110ExtendSingleKey.PATIENT_CODE_BAR, barcodePatient);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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
                rdo.executeRooms = new List<HIS_SESE_DEPO_REPAY>();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                List<SereServsADO> detail = new List<SereServsADO>();
                foreach (var item in rdo.dereDetails)
                {
                    detail.Add(new SereServsADO(item, rdo.ListSsDeposit, rdo.ListDeposit));
                }

                objectTag.AddObjectData(store, "SereServs", detail);
                objectTag.AddObjectData(store, "ExecuteRooms", rdo.executeRooms);

                objectTag.AddRelationship(store, "SereServs", "ExecuteRooms", "EXECUTE_ROOM_ID", "EXECUTE_ROOM_ID");

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
                if (rdo.departmentTrans != null && rdo.departmentTrans.Count > 0)
                {
                    SetSingleKey(new KeyValue(Mps000110ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.departmentTrans[0].DEPARTMENT_IN_TIME ?? 0)));
                    if (rdo.departmentTrans[rdo.departmentTrans.Count - 1] != null && rdo.departmentTrans.Count > 1)
                    {
                        SetSingleKey(new KeyValue(Mps000110ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.departmentTrans[rdo.departmentTrans.Count - 1].DEPARTMENT_IN_TIME ?? 0)));
                        //Số ngày điều trị
                        SetSingleKey(new KeyValue(Mps000110ExtendSingleKey.TOTAL_DAY, rdo.totalDay));
                    }
                }

                if (rdo.departmentName != null)
                {
                    SetSingleKey(new KeyValue(Mps000110ExtendSingleKey.DEPARTMENT_NAME, rdo.departmentName));
                }

                if (rdo.V_HIS_PATIENT_TYPE_ALTER != null)
                {
                    if (rdo.V_HIS_PATIENT_TYPE_ALTER.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (rdo.V_HIS_PATIENT_TYPE_ALTER.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                        {
                            SetSingleKey(new KeyValue(Mps000110ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            SetSingleKey(new KeyValue(Mps000110ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            SetSingleKey(new KeyValue(Mps000110ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (rdo.V_HIS_PATIENT_TYPE_ALTER.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            SetSingleKey(new KeyValue(Mps000110ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000110ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000110ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            SetSingleKey(new KeyValue(Mps000110ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000110ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000110ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (rdo.V_HIS_PATIENT_TYPE_ALTER.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        SetSingleKey(new KeyValue(Mps000110ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        SetSingleKey(new KeyValue(Mps000110ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        SetSingleKey(new KeyValue(Mps000110ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }

                    if (!String.IsNullOrEmpty(rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER))
                    {
                        SetSingleKey((new KeyValue(Mps000110ExtendSingleKey.HEIN_CARD_NUMBER_SEPARATE, HeinCardHelper.SetHeinCardNumberDisplayByNumber(rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER))));
                        SetSingleKey((new KeyValue(Mps000110ExtendSingleKey.IS_HEIN, "X")));
                        SetSingleKey((new KeyValue(Mps000110ExtendSingleKey.IS_VIENPHI, "")));
                        SetSingleKey((new KeyValue(Mps000110ExtendSingleKey.HEIN_CARD_NUMBER_1, rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER.Substring(0, 2))));
                        SetSingleKey((new KeyValue(Mps000110ExtendSingleKey.HEIN_CARD_NUMBER_2, rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER.Substring(2, 1))));
                        SetSingleKey((new KeyValue(Mps000110ExtendSingleKey.HEIN_CARD_NUMBER_3, rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER.Substring(3, 2))));
                        SetSingleKey((new KeyValue(Mps000110ExtendSingleKey.HEIN_CARD_NUMBER_4, rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER.Substring(5, 2))));
                        SetSingleKey((new KeyValue(Mps000110ExtendSingleKey.HEIN_CARD_NUMBER_5, rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER.Substring(7, 3))));
                        SetSingleKey((new KeyValue(Mps000110ExtendSingleKey.HEIN_CARD_NUMBER_6, rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_NUMBER.Substring(10, 5))));
                        SetSingleKey(new KeyValue(Mps000110ExtendSingleKey.STR_HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_FROM_TIME ?? 0)));
                        SetSingleKey(new KeyValue(Mps000110ExtendSingleKey.STR_HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.V_HIS_PATIENT_TYPE_ALTER.HEIN_CARD_TO_TIME ?? 0)));
                    }
                    else
                    {
                        SetSingleKey((new KeyValue(Mps000110ExtendSingleKey.IS_HEIN, "")));
                        SetSingleKey((new KeyValue(Mps000110ExtendSingleKey.IS_VIENPHI, "X")));
                    }
                    SetSingleKey(new KeyValue(Mps000110ExtendSingleKey.HEIN_CARD_ADDRESS, rdo.V_HIS_PATIENT_TYPE_ALTER.ADDRESS));
                }


                //if (rdo.hisTranPati != null)
                //SetSingleKey(new KeyValue(Mps000110ExtendSingleKey.TRAN_PATI_MEDI_ORG_CODE, rdo.hisTranPati.MEDI_ORG_CODE));

                //SetSingleKey(new KeyValue(Mps000082ExtendSingleKey.CURRENT_DATE_SEPARATE_FULL_STR, currentDateSeparateFullTime));


                decimal thanhtien_tong = 0;
                decimal bhytthanhtoan_tong = 0;
                decimal nguonkhac_tong = 0;
                decimal bnthanhtoan_tong = 0;

                if (rdo.dereDetails != null && rdo.dereDetails.Count > 0)
                {
                    thanhtien_tong = rdo.dereDetails.Sum(o => o.AMOUNT);
                    nguonkhac_tong = 0;
                }
                SetSingleKey(new KeyValue(Mps000110ExtendSingleKey.TOTAL_PRICE_NUM, thanhtien_tong));
                SetSingleKey(new KeyValue(Mps000110ExtendSingleKey.TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToString(thanhtien_tong, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator)));
                SetSingleKey(new KeyValue(Mps000110ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToString(bhytthanhtoan_tong, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator)));
                SetSingleKey(new KeyValue(Mps000110ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToString(bnthanhtoan_tong, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator)));
                SetSingleKey(new KeyValue(Mps000110ExtendSingleKey.TOTAL_PRICE_OTHER, Inventec.Common.Number.Convert.NumberToString(nguonkhac_tong, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator)));
                SetSingleKey(new KeyValue(Mps000110ExtendSingleKey.TOTAL_PRICE_TEXT, Inventec.Common.Number.Convert.NumberToString(thanhtien_tong, 0)));
                SetSingleKey(new KeyValue(Mps000110ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT,
Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000110ExtendSingleKey.TOTAL_PRICE_PATIENT_TEXT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000110ExtendSingleKey.TOTAL_PRICE_OTHER_TEXT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguonkhac_tong, 0)));
                AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo.V_HIS_PATIENT_TYPE_ALTER, false);
                AddObjectKeyIntoListkey<V_HIS_TREATMENT_1>(rdo.currentHisTreatment, false);
                AddObjectKeyIntoListkey<PatientADO>(rdo.patientADO, false);
                AddObjectKeyIntoListkey<V_HIS_DEPARTMENT_TRAN>(rdo.DepartmentTran, false);
                AddObjectKeyIntoListkey<V_HIS_TRANSACTION>(rdo.repay);
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
