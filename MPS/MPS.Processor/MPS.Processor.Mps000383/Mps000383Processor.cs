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
using MPS.Processor.Mps000383.ADO;
using MPS.Processor.Mps000383.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000383
{
    public class Mps000383Processor : AbstractProcessor
    {
        private List<HeinServiceTypeADO> heinServiceTypeADOs { get; set; }

        Mps000383PDO rdo;
        public Mps000383Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000383PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));

                DataInputProcess();
                ProcessSingleKey();
                SetBarcodeKey();

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "HeinServiceType", heinServiceTypeADOs);

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void SetBarcodeKey()
        {
            try
            {
                if (rdo != null && rdo.Treatment != null && !String.IsNullOrWhiteSpace(rdo.Treatment.TREATMENT_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.Treatment.TREATMENT_CODE);
                    barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeTreatment.IncludeLabel = false;
                    barcodeTreatment.Width = 120;
                    barcodeTreatment.Height = 40;
                    barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeTreatment.IncludeLabel = true;

                    dicImage.Add(Mps000383ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatment);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessSingleKey()
        {
            try
            {
                if (rdo.CurrentPatyAlter != null)
                {
                    int? typeIndex = null;
                    string typeName;

                    switch (rdo.CurrentPatyAlter.TREATMENT_TYPE_ID)
                    {
                        case IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM:
                            typeIndex = 1;
                            typeName = "KHÁM BỆNH";
                            break;
                        case IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU:
                            typeIndex = 2;
                            typeName = "ĐIỀU TRỊ NGOẠI TRÚ";
                            break;
                        case IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU:
                            typeIndex = 3;
                            typeName = "ĐIỀU TRỊ NỘI TRÚ";
                            break;
                        default:
                            typeIndex = null;
                            typeName = "(KHÔNG XÁC ĐỊNH ĐƯỢC ĐỐI TƯỢNG)";
                            break;
                    }
                    SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.TYPE_INDEX, typeIndex));
                    SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.TYPE_NAME, typeName));

                    if (rdo.CurrentPatyAlter.FREE_CO_PAID_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.FREE_CO_PAID_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.CurrentPatyAlter.FREE_CO_PAID_TIME.Value)));
                    }
                    if (rdo.CurrentPatyAlter.HEIN_CARD_FROM_TIME.HasValue)
                        SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.STR_HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.CurrentPatyAlter.HEIN_CARD_FROM_TIME ?? 0)));
                    if (rdo.CurrentPatyAlter.HEIN_CARD_TO_TIME.HasValue)
                        SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.STR_HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.CurrentPatyAlter.HEIN_CARD_TO_TIME ?? 0)));
                    if (rdo.CurrentPatyAlter.JOIN_5_YEAR_TIME.HasValue)
                        SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.JOIN_5_YEAR_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.CurrentPatyAlter.JOIN_5_YEAR_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.RATIO_STR, DataRawProcess.GetDefaultHeinRatioForView(rdo.CurrentPatyAlter.HEIN_CARD_NUMBER, rdo.CurrentPatyAlter.HEIN_TREATMENT_TYPE_CODE, rdo.CurrentPatyAlter.LEVEL_CODE, rdo.CurrentPatyAlter.RIGHT_ROUTE_CODE)));
                    SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.LIVE_AREA_CODE, rdo.CurrentPatyAlter.LIVE_AREA_CODE));

                    PatyAlterBhytADO patyAlterBhytADO = DataRawProcess.PatyAlterBHYTRawToADO(rdo.CurrentPatyAlter);
                    if (patyAlterBhytADO != null)
                    {
                        SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.IS_HEIN, "X"));
                        SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.HEIN_CARD_ADDRESS, patyAlterBhytADO.ADDRESS));
                        if (patyAlterBhytADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                        {
                            if (patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                            {
                                SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                                SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                                SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                            }
                            else if (patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                            {
                                SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                                SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                                SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                            }
                            else
                            {
                                SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                                SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                                SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                                if (rdo.Treatment.MEDI_ORG_CODE != rdo.CurrentPatyAlter.HEIN_MEDI_ORG_CODE)
                                    SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.THONG_TUYEN, "X"));
                            }
                        }
                        else if (patyAlterBhytADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                        {
                            SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                        }

                        AddObjectKeyIntoListkey<PatyAlterBhytADO>(patyAlterBhytADO, false);
                    }
                    else
                        SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.IS_NOT_HEIN, "X"));
                }
                else
                    SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.IS_NOT_HEIN, "X"));

                if (rdo.Treatment != null)
                {
                    if (rdo.Treatment.CLINICAL_IN_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.CLINICAL_IN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.Treatment.CLINICAL_IN_TIME.Value)));
                    }

                    if (rdo.Treatment.OUT_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.Treatment.OUT_TIME.Value)));
                    }

                    if (rdo.Treatment.END_DEPARTMENT_ID.HasValue)
                    {
                        HIS_DEPARTMENT department = rdo.Departments.FirstOrDefault(o => o.ID == rdo.Treatment.END_DEPARTMENT_ID.Value);
                        if (department != null)
                        {
                            SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.DEPARTMENT_BHYT_CODE, department.BHYT_CODE));
                            SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.END_DEPARTMENT_CODE, department.DEPARTMENT_CODE));
                            SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.END_DEPARTMENT_NAME, department.DEPARTMENT_NAME));
                        }
                    }

                    int? genderIndex = null;
                    string genderName;
                    switch (rdo.Treatment.TDL_PATIENT_GENDER_ID)
                    {
                        case IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE:
                            genderIndex = 1;
                            genderName = rdo.Treatment.TDL_PATIENT_GENDER_NAME;
                            break;
                        case IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE:
                            genderIndex = 2;
                            genderName = rdo.Treatment.TDL_PATIENT_GENDER_NAME;
                            break;
                        default:
                            genderIndex = 3;
                            genderName = "Không xác định";
                            break;
                    }

                    SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.GENDER_INDEX, genderIndex));
                    SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.GENDER_NAME, genderName));

                    //Tình trạng ra viện
                    if (rdo.Treatment.TREATMENT_RESULT_ID.HasValue || rdo.Treatment.TREATMENT_RESULT_ID.HasValue)
                    {
                        int treatmentResultIndex = 2;
                        if (rdo.Treatment.TREATMENT_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__DO
                            || rdo.Treatment.TREATMENT_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__KHOI
                            || rdo.Treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN
                            || rdo.Treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN
                            || rdo.Treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CTCV)
                        {
                            treatmentResultIndex = 1;
                            if ((rdo.Treatment.TREATMENT_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__DO
                            || rdo.Treatment.TREATMENT_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__KHOI) &&
                                (rdo.Treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN))
                            {
                                treatmentResultIndex = 2;
                            }
                        }
                        SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.TREATMENT_RESULT_INDEX, treatmentResultIndex));
                    }

                    if (rdo.Treatment.OUT_TIME.HasValue)
                    {
                        long inTime = 0;
                        //nội trú, ngoại trú
                        if (rdo.Treatment.CLINICAL_IN_TIME.HasValue)
                        {
                            inTime = rdo.Treatment.CLINICAL_IN_TIME.Value;
                        }
                        else //khám, điều trị bạn ngày
                        {
                            inTime = rdo.Treatment.IN_TIME;
                        }

                        System.DateTime? dateBefore = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(inTime);
                        System.DateTime? dateAfter = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(rdo.Treatment.OUT_TIME.Value);
                        if (dateBefore != null && dateAfter != null)
                        {
                            TimeSpan difference = dateAfter.Value - dateBefore.Value;
                            // lớn hơn 24h thì ngày ra - ngày vào + 1;
                            if (difference.Days > 1 || (difference.Days == 1 && (difference.Hours >= 1 || difference.Minutes >= 1 || difference.Seconds >= 1)))
                            {
                                if (rdo.Treatment.TDL_TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                                {
                                    DateTime before = new DateTime(dateBefore.Value.Year, dateBefore.Value.Month, dateBefore.Value.Day);
                                    DateTime after = new DateTime(dateAfter.Value.Year, dateAfter.Value.Month, dateAfter.Value.Day);
                                    int diffDay = (int)(after - before).TotalDays + 1;
                                    SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.TREATMENT_DAY_COUNT_6556, diffDay));
                                }
                            }
                            else if (rdo.Treatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                            {
                                SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.TREATMENT_DAY_COUNT_6556, 1));
                            }
                        }
                    }

                    decimal totalPrice = 0;
                    decimal totalHeinPrice = 0;
                    decimal totalPatientPrice = 0;
                    decimal totalDeposit = 0;
                    decimal totalBill = 0;
                    decimal totalBillTransferAmount = 0;
                    decimal totalRepay = 0;
                    decimal exemption = 0;
                    decimal depositPlus = 0;
                    decimal total_obtained_price = 0;

                    totalPrice = rdo.Treatment.TOTAL_PRICE ?? 0; // tong tien
                    totalHeinPrice = rdo.Treatment.TOTAL_HEIN_PRICE ?? 0;
                    totalPatientPrice = rdo.Treatment.TOTAL_PATIENT_PRICE ?? 0; // bn tra
                    totalDeposit = rdo.Treatment.TOTAL_DEPOSIT_AMOUNT ?? 0;
                    totalBill = rdo.Treatment.TOTAL_BILL_AMOUNT ?? 0;
                    totalBillTransferAmount = rdo.Treatment.TOTAL_BILL_TRANSFER_AMOUNT ?? 0;
                    exemption = 0;// HospitalFeeSum[0].TOTAL_EXEMPTION ?? 0;
                    totalRepay = rdo.Treatment.TOTAL_REPAY_AMOUNT ?? 0;
                    total_obtained_price = (totalDeposit + totalBill - totalBillTransferAmount - totalRepay + exemption);//Da thu benh nhan
                    decimal transfer = totalPatientPrice - total_obtained_price;//Phai thu benh nhan
                    depositPlus = transfer;//Nop them

                    SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.TREATMENT_FEE_TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalPrice, 0)));
                    SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.TREATMENT_FEE_TOTAL_PATIENT_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalPatientPrice, 0)));
                    SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.TREATMENT_FEE_TOTAL_OBTAINED_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(total_obtained_price, 0)));
                    SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.TREATMENT_FEE_TRANSFER, Inventec.Common.Number.Convert.NumberToStringRoundAuto(transfer, 0)));
                    SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(rdo.Treatment.TOTAL_DEPOSIT_AMOUNT ?? 0, 0)));
                    AddObjectKeyIntoListkey<V_HIS_TREATMENT_FEE>(rdo.Treatment, false);
                }

                decimal thanhtoan_tong = 0;
                decimal bhytthanhtoan_tong = 0;
                decimal nguonkhac_tong = 0;
                decimal bnthanhtoan_tong = 0;
                decimal tongtienbenhnhantutra = 0;

                if (heinServiceTypeADOs != null && heinServiceTypeADOs.Count > 0)
                {
                    thanhtoan_tong = heinServiceTypeADOs.Sum(o => o.VIR_TOTAL_PRICE) ?? 0;
                    bhytthanhtoan_tong = heinServiceTypeADOs.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    bnthanhtoan_tong = heinServiceTypeADOs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT) ?? 0;
                    nguonkhac_tong = heinServiceTypeADOs.Sum(o => o.OTHER_SOURCE_PRICE) ?? 0;
                    tongtienbenhnhantutra = heinServiceTypeADOs.Sum(o => o.TOTAL_PRICE_PATIENT_SELF ?? 0);
                }

                SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.TOTAL_PRICE_PATIENT_SELF, Inventec.Common.Number.Convert.NumberToStringRoundAuto(tongtienbenhnhantutra, 0)));
                SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.TOTAL_PRICE_OTHER, Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguonkhac_tong, 0)));
                SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.TOTAL_PRICE_PATIENT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000383ExtendSingleKey.TOTAL_PRICE_OTHER_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(nguonkhac_tong).ToString())));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DataInputProcess()
        {
            try
            {
                List<HIS_TRANSACTION> listTranDirect = new List<HIS_TRANSACTION>();
                if (rdo.ListTransaction != null && rdo.ListTransaction.Count > 0)
                {
                    listTranDirect = rdo.ListTransaction.Where(o => o.IS_DIRECTLY_BILLING == 1).ToList();
                }

                List<HIS_SERE_SERV_BILL> listSSDirectly = new List<HIS_SERE_SERV_BILL>();
                if (rdo.ListSereServBill != null && rdo.ListSereServBill.Count > 0)
                {
                    listSSDirectly = rdo.ListSereServBill.Where(o => listTranDirect.Select(s => s.ID).Contains(o.BILL_ID)).ToList();
                }

                List<HIS_SERE_SERV> ListSS = new List<HIS_SERE_SERV>();
                if (rdo.ListSereServ != null && rdo.ListSereServ.Count > 0)
                {
                    ListSS = rdo.ListSereServ.Where(o => !listSSDirectly.Select(s => s.SERE_SERV_ID).Contains(o.ID)).ToList();
                }

                var sereServGroupHeinType = ListSS.Where(o =>
                        o.AMOUNT > 0
                        && o.IS_NO_EXECUTE != 1
                        && o.IS_EXPEND != 1).GroupBy(o => o.TDL_HEIN_SERVICE_TYPE_ID).ToList();

                heinServiceTypeADOs = new List<HeinServiceTypeADO>();

                foreach (var groupHein in sereServGroupHeinType)
                {
                    var heinServiceType = rdo.HeinServiceTypes.FirstOrDefault(o => o.ID == groupHein.Key);
                    HeinServiceTypeADO ado = new HeinServiceTypeADO();
                    ado.HEIN_SERVICE_TYPE_CODE = heinServiceType != null ? heinServiceType.HEIN_SERVICE_TYPE_CODE : "";
                    ado.HEIN_SERVICE_TYPE_NAME = heinServiceType != null ? heinServiceType.HEIN_SERVICE_TYPE_NAME : "Khác";
                    ado.NUM_ORDER = heinServiceType != null ? heinServiceType.NUM_ORDER : 99999;

                    ado.OTHER_SOURCE_PRICE = groupHein.Sum(o => o.OTHER_SOURCE_PRICE ?? 0);
                    ado.VIR_TOTAL_HEIN_PRICE = groupHein.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);
                    ado.VIR_TOTAL_PATIENT_PRICE = groupHein.Sum(o => o.VIR_TOTAL_PATIENT_PRICE ?? 0);
                    ado.VIR_TOTAL_PATIENT_PRICE_BHYT = groupHein.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0);
                    ado.VIR_TOTAL_PATIENT_PRICE_NO_DC = groupHein.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_NO_DC ?? 0);
                    ado.VIR_TOTAL_PATIENT_PRICE_TEMP = groupHein.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_TEMP ?? 0);
                    ado.VIR_TOTAL_PRICE = groupHein.Sum(o => o.VIR_TOTAL_PRICE ?? 0);
                    ado.VIR_TOTAL_PRICE_NO_ADD_PRICE = groupHein.Sum(o => o.VIR_TOTAL_PRICE_NO_ADD_PRICE ?? 0);
                    ado.VIR_TOTAL_PRICE_NO_EXPEND = groupHein.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0);
                    ado.TOTAL_PRICE_PATIENT_SELF = ado.VIR_TOTAL_PRICE - ado.VIR_TOTAL_HEIN_PRICE - ado.OTHER_SOURCE_PRICE;

                    heinServiceTypeADOs.Add(ado);
                }

                heinServiceTypeADOs = heinServiceTypeADOs.OrderBy(o => o.NUM_ORDER ?? 9999).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
