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
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ProcessorBase.Core;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000120.PDO;
using FlexCel.Report;
using MPS.ProcessorBase;
using MPS.Processor.Mps000120.ADO;

namespace MPS.Processor.Mps000120
{
    public partial class Mps000120Processor : AbstractProcessor
    {
        private PatientADO patientADO { get; set; }
        private List<PatyAlterBhytADO> patyAlterBHYTADOs { get; set; }
        private List<SereServADO> sereServADOs { get; set; }
        private List<HeinServiceTypeADO> heinServiceTypeADOs { get; set; }
        private List<MedicineLineADO> medicineLineADOs { get; set; }

        Mps000120PDO rdo;
        public Mps000120Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000120PDO)rdoBase;
        }

        internal void SetBarcodeKey()
        {
            try
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

                dicImage.Add(Mps000120ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatment);
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

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                DataInputProcess();
                HeinServiceTypeProcess();
                ProcessSingleKey();
                SetBarcodeKey();
                if (sereServADOs == null || sereServADOs.Count == 0)
                    return false;

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "HeinServiceType", heinServiceTypeADOs);
                objectTag.AddObjectData(store, "Service", sereServADOs);
                objectTag.AddRelationship(store, "HeinServiceType", "Service", "ID", "TDL_HEIN_SERVICE_TYPE_ID");
                objectTag.AddObjectData(store, "PatyAlterBHYT", patyAlterBHYTADOs);
                objectTag.AddObjectData(store, "MedicineLine", medicineLineADOs);
                objectTag.AddRelationship(store, "HeinServiceType", "MedicineLine", "ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "MedicineLine", "Service", "ID", "MEDICINE_LINE_ID");

                objectTag.SetUserFunction(store, "ReplaceValue", new ReplaceValueFunction());

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        class ReplaceValueFunction : FlexCel.Report.TFlexCelUserFunction
        {
            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length <= 0)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");

                try
                {
                    string value = parameters[0] + "";
                    if (!String.IsNullOrEmpty(value))
                    {
                        value = value.Replace(';', '/');
                    }
                    return value;
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    return parameters[0];
                }
            }
        }

        void ProcessSingleKey()
        {
            try
            {

                SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.RATIO_STR, rdo.SingleKeyValue.ratio));
                SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.TREATMENT_CODE, rdo.Treatment.TREATMENT_CODE));
                SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.USERNAME_RETURN_RESULT, rdo.SingleKeyValue.userNameReturnResult));
                SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.STATUS_TREATMENT_OUT, rdo.SingleKeyValue.statusTreatmentOut));

                if (patyAlterBHYTADOs != null && patyAlterBHYTADOs.Count > 0)
                {
                    string heinMediOrgCode = null;
                    string heinMediOrgName = null;
                    foreach (var item in patyAlterBHYTADOs)
                    {
                        heinMediOrgCode += (item.HEIN_MEDI_ORG_CODE + (patyAlterBHYTADOs.IndexOf(item) < patyAlterBHYTADOs.Count() - 1 ? "; " : ""));
                        heinMediOrgName += (item.HEIN_MEDI_ORG_NAME + (patyAlterBHYTADOs.IndexOf(item) < patyAlterBHYTADOs.Count() - 1 ? "; " : ""));
                    }

                    SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.HEIN_MEDI_ORG_CODE, heinMediOrgCode));
                    SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.HEIN_MEDI_ORG_NAME, heinMediOrgName));


                    PatyAlterBhytADO patyAlterBhytADO = patyAlterBHYTADOs.OrderBy(o => o.LOG_TIME).FirstOrDefault(o => !String.IsNullOrEmpty(o.HEIN_CARD_NUMBER));
                    if (patyAlterBhytADO != null)
                    {
                        SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.IS_HEIN, "X"));
                        SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.HEIN_CARD_ADDRESS, patyAlterBhytADO.ADDRESS));
                        if (patyAlterBhytADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                        {
                            if (patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                            {
                                SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                                SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                                SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                            }
                            else if (patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                            {
                                SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                                SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                                SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                            }
                            else
                            {
                                SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                                SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                                SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                            }
                        }
                        else if (patyAlterBhytADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                        {
                            SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                        }
                    }
                    else
                        SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.IS_NOT_HEIN, "X"));
                }
                else
                    SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.IS_NOT_HEIN, "X"));

                if (rdo.DepartmentTrans != null && rdo.DepartmentTrans.Count > 0)
                {
                    if (rdo.DepartmentTrans[0].DEPARTMENT_IN_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.DepartmentTrans[0].DEPARTMENT_IN_TIME ?? 0)));
                    }
                    if (rdo.DepartmentTrans[rdo.DepartmentTrans.Count - 1] != null && rdo.DepartmentTrans.Count > 1)
                    {
                        SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.DEPARTMENT_NAME_CLOSE_TREATMENT, rdo.DepartmentTrans[rdo.DepartmentTrans.Count - 1].DEPARTMENT_NAME));
                    }

                }

                if (!String.IsNullOrEmpty(rdo.SingleKeyValue.roomName))
                {
                    SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.ROOM_NAME, rdo.SingleKeyValue.roomName));
                }

                if (rdo.Treatment != null)
                {
                    if (rdo.Treatment.CLINICAL_IN_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.CLINICAL_IN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.Treatment.CLINICAL_IN_TIME.Value)));
                    }

                    if (rdo.Treatment.OUT_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.Treatment.OUT_TIME.Value)));
                    }
                }

                SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.TOTAL_DAY, rdo.SingleKeyValue.today));

                if (rdo.Treatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.TRAN_PATI_MEDI_ORG_CODE, rdo.Treatment.TRANSFER_IN_MEDI_ORG_CODE));
                    SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.TRAN_PATI_MEDI_ORG_NAME, rdo.Treatment.TRANSFER_IN_MEDI_ORG_NAME));
                }

                SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.CURRENT_DATE_SEPARATE_FULL_STR, rdo.SingleKeyValue.currentDateSeparateFullTime));

                string executeRoomExam = "";
                string executeRoomExamFirst = "";
                string executeRoomExamLast = "";

                decimal thanhtien_tong = 0;
                decimal bhytthanhtoan_tong = 0;
                decimal nguonkhac_tong = 0;
                decimal bnthanhtoan_tong = 0;
                long totalNumberFilm = 0;
                decimal tongtienbenhnhantutra = 0;

                if (sereServADOs != null && sereServADOs.Count > 0)
                {
                    var sereServExamADOs = sereServADOs.Where(o => o.HEIN_SERVICE_TYPE_ID == rdo.HeinServiceTypeCFG.HEIN_SERVICE_TYPE__EXAM_ID).OrderBy(o => o.CREATE_TIME).ToList();

                    if (sereServExamADOs != null && sereServExamADOs.Count > 0)
                    {
                        executeRoomExamFirst = sereServExamADOs[0].EXECUTE_ROOM_NAME;
                        executeRoomExamLast = sereServExamADOs[sereServExamADOs.Count - 1].EXECUTE_ROOM_NAME;

                        foreach (var sereServExamADO in sereServExamADOs)
                        {
                            executeRoomExam += sereServExamADO.EXECUTE_ROOM_NAME + ", ";
                        }
                    }

                    thanhtien_tong = sereServADOs.Sum(o => o.TOTAL_PRICE_BHYT);
                    bhytthanhtoan_tong = sereServADOs.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    bnthanhtoan_tong = sereServADOs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT) ?? 0;
                    nguonkhac_tong = 0;
                    tongtienbenhnhantutra = sereServADOs.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);

                    totalNumberFilm = (long) sereServADOs.Sum(o => ((o.NUMBER_OF_FILM ?? 0) * o.AMOUNT));
                    if (totalNumberFilm > 0)
                    {
                        SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.TOTAL_NUMBER_FILM, totalNumberFilm));
                        SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.TOTAL_NUMBER_FILM_STR, String.Format("Bệnh nhân đã nhận đủ số phim . Số phim {0}", totalNumberFilm)));
                    }
                }

                SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.EXECUTE_ROOM_EXAM, executeRoomExam));
                SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.FIRST_EXAM_ROOM_NAME, executeRoomExamFirst));
                SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.LAST_EXAM_ROOM_NAME, executeRoomExamLast));


                if (!String.IsNullOrEmpty(rdo.SingleKeyValue.departmentName))
                {
                    SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.DEPARTMENT_NAME, rdo.SingleKeyValue.departmentName));
                }

                SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong, 0)));
                SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.TOTAL_PRICE_PATIENT_SELF, Inventec.Common.Number.Convert.NumberToStringRoundAuto(tongtienbenhnhantutra, 0)));
                SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.TOTAL_PRICE_OTHER, Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguonkhac_tong, 0)));
                SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.TOTAL_PRICE_PATIENT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.TOTAL_PRICE_OTHER_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(nguonkhac_tong).ToString())));

                if (rdo.TreatmentFees != null)
                {
                    SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(rdo.TreatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0, 0)));

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

                    totalPrice = rdo.TreatmentFees[0].TOTAL_PRICE ?? 0; // tong tien
                    totalHeinPrice = rdo.TreatmentFees[0].TOTAL_HEIN_PRICE ?? 0;
                    totalPatientPrice = rdo.TreatmentFees[0].TOTAL_PATIENT_PRICE ?? 0; // bn tra
                    totalDeposit = rdo.TreatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0;
                    totalBill = rdo.TreatmentFees[0].TOTAL_BILL_AMOUNT ?? 0;
                    totalBillTransferAmount = rdo.TreatmentFees[0].TOTAL_BILL_TRANSFER_AMOUNT ?? 0;
                    exemption = 0;// HospitalFeeSum[0].TOTAL_EXEMPTION ?? 0;
                    totalRepay = rdo.TreatmentFees[0].TOTAL_REPAY_AMOUNT ?? 0;
                    total_obtained_price = (totalDeposit + totalBill - totalBillTransferAmount - totalRepay + exemption);//Da thu benh nhan
                    decimal transfer = totalPatientPrice - total_obtained_price;//Phai thu benh nhan
                    depositPlus = transfer;//Nop them

                    SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.TREATMENT_FEE_TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalPrice, 0)));
                    SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.TREATMENT_FEE_TOTAL_PATIENT_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalPatientPrice, 0)));
                    SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.TREATMENT_FEE_TOTAL_OBTAINED_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(total_obtained_price, 0)));
                    SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.TREATMENT_FEE_TRANSFER, Inventec.Common.Number.Convert.NumberToStringRoundAuto(transfer, 0)));
                    AddObjectKeyIntoListkey<V_HIS_TREATMENT_FEE>(rdo.TreatmentFees[0], false);
                }
                else
                {
                    SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, "0"));
                }

                SetSingleKey(new KeyValue(Mps000120ExtendSingleKey.CREATOR_USERNAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));

                AddObjectKeyIntoListkey<PatientADO>(patientADO);
                AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.Treatment, false);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
