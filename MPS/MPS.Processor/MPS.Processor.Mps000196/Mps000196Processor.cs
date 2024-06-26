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
using MPS.Processor.Mps000196.PDO;
using FlexCel.Report;
using MPS.ProcessorBase;

namespace MPS.Processor.Mps000196
{
    public partial class Mps000196Processor : AbstractProcessor
    {
        Mps000196PDO rdo;
        public Mps000196Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000196PDO)rdoBase;
        }

        internal void SetBarcodeKey()
        {
            try
            {

                Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.treatment.TREATMENT_CODE);
                barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatment.IncludeLabel = false;
                barcodeTreatment.Width = 120;
                barcodeTreatment.Height = 40;
                barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatment.IncludeLabel = true;

                dicImage.Add(Mps000196ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatment);
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
                ProcessGroupSereServ();
                ProcessSingleKey();
                SetBarcodeKey();

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "HeinServiceType", rdo.heinServiceTypes);
                objectTag.AddObjectData(store, "Service", rdo.sereServADOs);
                objectTag.AddRelationship(store, "HeinServiceType", "Service", "TDL_HEIN_SERVICE_TYPE_ID", "TDL_HEIN_SERVICE_TYPE_ID");
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

        internal void DataInputProcess()
        {
            try
            {
                //Patient ADO
                rdo.patientADO = DataRawProcess.PatientRawToADO(rdo.patient);

                rdo.sereServADOs = new List<SereServADO>();
                //SereServADO
                var sereServADOTemps = new List<SereServADO>();
                sereServADOTemps.AddRange(from r in rdo.sereServs
                                          select new SereServADO(r,rdo.HeinServiceTypes,rdo.Rooms,rdo.Services));

                //sereServ Vien Phi
                var sereServVPGroups = sereServADOTemps
                    .Where(o => o.PATIENT_TYPE_ID != rdo.patientType.PATIENT_TYPE_THUPHI_ID
                    && o.IS_NO_EXECUTE == null
                    && o.VIR_PRICE > 0
                    && o.IS_EXPEND == null
                    && o.AMOUNT > 0)
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999)
                    .GroupBy(o => new
                    {
                        o.SERVICE_ID,
                        o.IS_EXPEND
                    }).ToList();

                foreach (var sereServVPGroup in sereServVPGroups)
                {
                    SereServADO sereServ = sereServVPGroup.FirstOrDefault();
                    sereServ.AMOUNT = sereServVPGroup.Sum(o => o.AMOUNT);
                    sereServ.VIR_TOTAL_PRICE = sereServVPGroup.Sum(o => o.VIR_TOTAL_PRICE);
                    sereServ.VIR_TOTAL_PATIENT_PRICE = sereServVPGroup
                        .Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                    sereServ.VIR_TOTAL_HEIN_PRICE = sereServVPGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                    rdo.sereServADOs.Add(sereServ);
                }

                //sereServ dong chi tra
                var sereServBHYTGroups = sereServADOTemps
                    .Where(o =>
                         o.PATIENT_TYPE_ID == rdo.patientType.PATIENT_TYPE_BHYT_ID
                        && o.PRICE_CO_PAYMENT > 0
                        && o.IS_EXPEND != 1
                        && o.IS_NO_EXECUTE != 1)
                    .GroupBy(o => new
                    {
                        o.SERVICE_ID,
                        o.VIR_TOTAL_PRICE,
                        o.TOTAL_HEIN_PRICE_ONE_AMOUNT,
                        o.HEIN_LIMIT_PRICE,
                        o.IS_EXPEND,
                        o.PRICE_CO_PAYMENT
                    }).ToList();

                foreach (var sereServBHYTGroup in sereServBHYTGroups)
                {
                    SereServADO sereServ = sereServBHYTGroup.FirstOrDefault();
                    sereServ.AMOUNT = sereServBHYTGroup.Sum(o => o.AMOUNT);
                    sereServ.VIR_TOTAL_PRICE = sereServ.PRICE_CO_PAYMENT * sereServ.AMOUNT;
                    sereServ.VIR_TOTAL_PATIENT_PRICE = sereServ.PRICE_CO_PAYMENT * sereServ.AMOUNT;
                    sereServ.VIR_TOTAL_HEIN_PRICE = 0;
                    sereServ.VIR_PRICE = sereServ.PRICE_CO_PAYMENT;
                    rdo.sereServADOs.Add(sereServ);
                }

                rdo.expMests = rdo.expMests != null ? rdo.expMests.Where(o => o.IS_CABINET == 1).ToList() : null;
                if (rdo.expMests != null && rdo.expMests.Count > 0)
                {
                    foreach (var item in rdo.sereServADOs)
                    {
                        V_HIS_EXP_MEST expMest = rdo.expMests.FirstOrDefault(o => o.SERVICE_REQ_ID == item.SERVICE_REQ_ID);
                        if (expMest != null)
                        {
                            item.TDL_HEIN_SERVICE_TYPE_ID = rdo.heinServiceTypeCfg.MEDI_MATE_FROM_CABINET_ID;
                            var heinServiceType = rdo.HeinServiceTypes.FirstOrDefault(o => o.ID == rdo.heinServiceTypeCfg.MEDI_MATE_FROM_CABINET_ID);
                            item.HEIN_SERVICE_TYPE_NAME = heinServiceType != null ? heinServiceType.HEIN_SERVICE_TYPE_NAME : null;
                        }

                    }
                }

                rdo.sereServADOs = rdo.sereServADOs.OrderBy(o => o.TDL_SERVICE_NAME).ToList();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void ProcessGroupSereServ()
        {
            try
            {
                rdo.heinServiceTypes = new List<SereServADO>();

                var sereServBHYTGroups = rdo.sereServADOs.OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999)
                    .GroupBy(o => o.TDL_HEIN_SERVICE_TYPE_ID).ToList();

                foreach (var sereServBHYTGroup in sereServBHYTGroups)
                {
                    SereServADO heinServiceType = new SereServADO();

                    SereServADO sereServBHYT = sereServBHYTGroup.FirstOrDefault();
                    if (sereServBHYT.TDL_HEIN_SERVICE_TYPE_ID.HasValue)
                    {
                        heinServiceType.TDL_HEIN_SERVICE_TYPE_ID = sereServBHYT.TDL_HEIN_SERVICE_TYPE_ID.Value;
                        heinServiceType.HEIN_SERVICE_TYPE_NAME = sereServBHYT.HEIN_SERVICE_TYPE_NAME;
                    }
                    else
                    {
                        heinServiceType.HEIN_SERVICE_TYPE_NAME = "Khác";
                    }

                    heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_PRICE) ?? -999;
                    heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE.Value);
                    heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup
                        .Sum(o => o.VIR_TOTAL_PATIENT_PRICE.Value);
                    rdo.heinServiceTypes.Add(heinServiceType);
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
                SetSingleKey(new KeyValue(Mps000196ExtendSingleKey.TREATMENT_CODE, rdo.treatment.TREATMENT_CODE));
                SetSingleKey(new KeyValue(Mps000196ExtendSingleKey.USERNAME_RETURN_RESULT, rdo.bordereauSingleValue.userNameReturnResult));
                SetSingleKey(new KeyValue(Mps000196ExtendSingleKey.STATUS_TREATMENT_OUT, rdo.bordereauSingleValue.statusTreatmentOut));

                if (rdo.departmentTrans != null && rdo.departmentTrans.Count > 0)
                {
                    if (rdo.departmentTrans[0].DEPARTMENT_IN_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000196ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.departmentTrans[0].DEPARTMENT_IN_TIME.Value)));
                    }
                    if (rdo.departmentTrans[rdo.departmentTrans.Count - 1] != null && rdo.departmentTrans.Count > 1)
                    {
                        SetSingleKey(new KeyValue(Mps000196ExtendSingleKey.DEPARTMENT_NAME_CLOSE_TREATMENT, rdo.departmentTrans[rdo.departmentTrans.Count - 1].DEPARTMENT_NAME));
                    }
                }

                if (rdo.treatment != null)
                {
                    if (rdo.treatment.CLINICAL_IN_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000196ExtendSingleKey.CLINICAL_IN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.treatment.CLINICAL_IN_TIME.Value)));
                    }

                    if (rdo.treatment.OUT_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000196ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.treatment.OUT_TIME.Value)));
                    }
                }
                SetSingleKey(new KeyValue(Mps000196ExtendSingleKey.TOTAL_DAY, rdo.bordereauSingleValue.today));

                if (rdo.treatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000196ExtendSingleKey.TRAN_PATI_MEDI_ORG_CODE, rdo.treatment.TRANSFER_IN_MEDI_ORG_CODE));
                    SetSingleKey(new KeyValue(Mps000196ExtendSingleKey.TRAN_PATI_MEDI_ORG_NAME, rdo.treatment.TRANSFER_IN_MEDI_ORG_NAME));
                }

                if (!String.IsNullOrEmpty(rdo.bordereauSingleValue.departmentName))
                {
                    SetSingleKey(new KeyValue(Mps000196ExtendSingleKey.DEPARTMENT_NAME, rdo.bordereauSingleValue.departmentName));
                }

                string executeRoomExam = "";
                string executeRoomExamFirst = "";
                string executeRoomExamLast = "";
                decimal thanhtien_tong = 0;
                decimal bhytthanhtoan_tong = 0;
                decimal nguonkhac_tong = 0;
                decimal bnthanhtoan_tong = 0;
                long totalNumberFilm = 0;

                if (rdo.sereServADOs != null && rdo.sereServADOs.Count > 0)
                {
                    var sereServExamADOs = rdo.sereServADOs.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID == rdo.heinServiceTypeCfg.EXAM_ID).OrderBy(o => o.CREATE_TIME).ToList();

                    if (sereServExamADOs != null && sereServExamADOs.Count > 0)
                    {
                        executeRoomExamFirst = sereServExamADOs[0].EXECUTE_ROOM_NAME;
                        executeRoomExamLast = sereServExamADOs[sereServExamADOs.Count - 1].EXECUTE_ROOM_NAME;
                        foreach (var sereServExamADO in sereServExamADOs)
                        {
                            executeRoomExam += sereServExamADO.EXECUTE_ROOM_NAME + ", ";
                        }
                    }

                    thanhtien_tong = rdo.sereServADOs.Sum(o => o.VIR_TOTAL_PRICE) ?? -999;
                    bhytthanhtoan_tong = rdo.sereServADOs.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    bnthanhtoan_tong = rdo.sereServADOs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
                    nguonkhac_tong = 0;

                    totalNumberFilm = rdo.sereServADOs.Sum(o => o.NUMBER_OF_FILM ?? 0);
                    if (totalNumberFilm > 0)
                    {
                        SetSingleKey(new KeyValue(Mps000196ExtendSingleKey.TOTAL_NUMBER_FILM, totalNumberFilm));
                        SetSingleKey(new KeyValue(Mps000196ExtendSingleKey.TOTAL_NUMBER_FILM_STR, String.Format("Bệnh nhân đã nhận đủ số phim . Số phim {0}", totalNumberFilm)));
                    }
                }

                SetSingleKey(new KeyValue(Mps000196ExtendSingleKey.EXECUTE_ROOM_EXAM, executeRoomExam));
                SetSingleKey(new KeyValue(Mps000196ExtendSingleKey.FIRST_EXAM_ROOM_NAME, executeRoomExamFirst));
                SetSingleKey(new KeyValue(Mps000196ExtendSingleKey.LAST_EXAM_ROOM_NAME, executeRoomExamLast));

                SetSingleKey(new KeyValue(Mps000196ExtendSingleKey.TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong, 0)));
                SetSingleKey(new KeyValue(Mps000196ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000196ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000196ExtendSingleKey.TOTAL_PRICE_OTHER, Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguonkhac_tong, 0)));
                SetSingleKey(new KeyValue(Mps000196ExtendSingleKey.TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000196ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000196ExtendSingleKey.TOTAL_PRICE_PATIENT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000196ExtendSingleKey.TOTAL_PRICE_OTHER_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(nguonkhac_tong).ToString())));

                if (rdo.treatmentFees != null)
                {
                    SetSingleKey(new KeyValue(Mps000196ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(rdo.treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0, 0)));

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

                    totalPrice = rdo.treatmentFees[0].TOTAL_PRICE ?? 0; // tong tien
                    totalHeinPrice = rdo.treatmentFees[0].TOTAL_HEIN_PRICE ?? 0;
                    totalPatientPrice = rdo.treatmentFees[0].TOTAL_PATIENT_PRICE ?? 0; // bn tra
                    totalDeposit = rdo.treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0;
                    totalBill = rdo.treatmentFees[0].TOTAL_BILL_AMOUNT ?? 0;
                    totalBillTransferAmount = rdo.treatmentFees[0].TOTAL_BILL_TRANSFER_AMOUNT ?? 0;
                    exemption = 0;// HospitalFeeSum[0].TOTAL_EXEMPTION ?? 0;
                    totalRepay = rdo.treatmentFees[0].TOTAL_REPAY_AMOUNT ?? 0;
                    total_obtained_price = (totalDeposit + totalBill - totalBillTransferAmount - totalRepay + exemption);//Da thu benh nhan
                    decimal transfer = totalPatientPrice - total_obtained_price;//Phai thu benh nhan
                    depositPlus = transfer;//Nop them

                    SetSingleKey(new KeyValue(Mps000196ExtendSingleKey.TREATMENT_FEE_TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalPrice, 0)));
                    SetSingleKey(new KeyValue(Mps000196ExtendSingleKey.TREATMENT_FEE_TOTAL_PATIENT_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalPatientPrice, 0)));
                    SetSingleKey(new KeyValue(Mps000196ExtendSingleKey.TREATMENT_FEE_TOTAL_OBTAINED_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(total_obtained_price, 0)));
                    SetSingleKey(new KeyValue(Mps000196ExtendSingleKey.TREATMENT_FEE_TRANSFER, Inventec.Common.Number.Convert.NumberToStringRoundAuto(transfer, 0)));
                    AddObjectKeyIntoListkey<V_HIS_TREATMENT_FEE>(rdo.treatmentFees[0], false);
                }
                else
                {
                    SetSingleKey(new KeyValue(Mps000196ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, "0"));
                }

                SetSingleKey(new KeyValue(Mps000196ExtendSingleKey.CREATOR_USERNAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));

                AddObjectKeyIntoListkey<PatientADO>(rdo.patientADO);
                AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.treatment, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
