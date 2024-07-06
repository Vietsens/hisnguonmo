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
using MOS.EFMODEL.DataModels;
using MPS.ADO;
using MPS.ADO.Bordereau;
using MPS.Old.ADO.Bordereau;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Core.Mps000196
{
    /// <summary>
    /// In bang ke thanh toan ra vien ngoai tru va noi tru.
    /// </summary>
    public class Mps000196RDO : RDOBase
    {
        internal V_HIS_PATIENT patient { get; set; }
        internal List<V_HIS_DEPARTMENT_TRAN> departmentTrans { get; set; }
        internal List<V_HIS_TREATMENT_FEE> treatmentFees { get; set; }
        internal List<V_HIS_SERE_SERV> sereServs { get; set; }
        internal V_HIS_TREATMENT treatment { get; set; }
        internal V_HIS_TRAN_PATI tranPati { get; set; }
        internal PatientADO patientADO { get; set; }
        internal List<MPS.ADO.Bordereau.SereServADO> sereServADOs { get; set; }
        internal List<MPS.ADO.Bordereau.SereServADO> heinServiceTypes { get; set; }
        internal PatientTypeCFG patientType { get; set; }
        internal BordereauSingleValue bordereauSingleValue { get; set; }
        internal List<V_HIS_PRESCRIPTION_1> prescriptions { get; set; }
        internal List<HIS_HEIN_SERVICE_TYPE> HeinServiceTypes { get; set; }
        internal HeinServiceTypeCFG heinServiceTypeCfg { get; set; }


        public Mps000196RDO(
            V_HIS_PATIENT _patient,
            List<V_HIS_SERE_SERV> _sereServ,
            V_HIS_TREATMENT _treatment,
            V_HIS_TRAN_PATI _tranPati,
            List<V_HIS_PRESCRIPTION_1> _prescriptions,
            List<HIS_HEIN_SERVICE_TYPE> _heinServiceTypes,
            List<V_HIS_TREATMENT_FEE> _treatmentFees,
            List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> _departmentTrans,
            HeinServiceTypeCFG _heinServiceType,
            PatientTypeCFG _patientType,
            BordereauSingleValue _bordereauSingleValue
            )
        {
            try
            {
                this.patient = _patient;
                this.sereServs = _sereServ;
                this.treatment = _treatment;
                this.tranPati = _tranPati;
                this.heinServiceTypeCfg = _heinServiceType;
                this.patientType = _patientType;
                this.departmentTrans = _departmentTrans;
                this.treatmentFees = _treatmentFees;
                this.bordereauSingleValue = _bordereauSingleValue;
                this.prescriptions = _prescriptions;
                this.HeinServiceTypes = _heinServiceTypes;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void DataInputProcess()
        {
            try
            {
                //Patient ADO
                patientADO = DataRawProcess.PatientRawToADO(patient);

                sereServADOs = new List<ADO.Bordereau.SereServADO>();
                //SereServADO
                var sereServADOTemps = new List<ADO.Bordereau.SereServADO>();
                sereServADOTemps.AddRange(from r in sereServs
                                          select new ADO.Bordereau.SereServADO(r));

                //sereServ Vien Phi
                var sereServVPGroups = sereServADOTemps
                    .Where(o => o.PATIENT_TYPE_CODE != patientType.PATIENT_TYPE_BHYT_CODE
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
                    ADO.Bordereau.SereServADO sereServ = sereServVPGroup.FirstOrDefault();
                    sereServ.AMOUNT = sereServVPGroup.Sum(o => o.AMOUNT);
                    sereServ.VIR_TOTAL_PRICE = sereServVPGroup.Sum(o => o.VIR_TOTAL_PRICE);
                    sereServ.VIR_TOTAL_PATIENT_PRICE = sereServVPGroup
                        .Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                    sereServ.VIR_TOTAL_HEIN_PRICE = sereServVPGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                    sereServADOs.Add(sereServ);
                }

                //sereServ dong chi tra
                var sereServBHYTGroups = sereServADOTemps
                    .Where(o =>
                        o.PRICE_CO_PAYMENT > 0
                        && o.IS_EXPEND == null
                        && o.IS_NO_EXECUTE == null)
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
                    ADO.Bordereau.SereServADO sereServ = sereServBHYTGroup.FirstOrDefault();
                    sereServ.AMOUNT = sereServBHYTGroup.Sum(o => o.AMOUNT);
                    sereServ.VIR_TOTAL_PRICE = sereServ.PRICE_CO_PAYMENT * sereServ.AMOUNT;
                    sereServ.VIR_TOTAL_PATIENT_PRICE = sereServ.PRICE_CO_PAYMENT * sereServ.AMOUNT;
                    sereServ.VIR_TOTAL_HEIN_PRICE = 0;
                    sereServ.VIR_PRICE = sereServ.PRICE_CO_PAYMENT;
                    sereServADOs.Add(sereServ);
                }

                prescriptions = prescriptions != null ? prescriptions.Where(o => o.IS_CABINET == 1).ToList() : null;
                if (prescriptions != null && prescriptions.Count > 0)
                {
                    foreach (var item in sereServADOs)
                    {
                        V_HIS_PRESCRIPTION_1 prescription = prescriptions.FirstOrDefault(o => o.SERVICE_REQ_ID == item.SERVICE_REQ_ID);
                        if (prescription != null)
                        {
                            item.HEIN_SERVICE_TYPE_ID = heinServiceTypeCfg.MEDI_MATE_FROM_CABINET_ID;
                            var heinServiceType = HeinServiceTypes.FirstOrDefault(o => o.ID == heinServiceTypeCfg.MEDI_MATE_FROM_CABINET_ID);
                            item.HEIN_SERVICE_TYPE_NAME = heinServiceType != null ? heinServiceType.HEIN_SERVICE_TYPE_NAME : null;
                        }

                    }
                }

                sereServADOs = sereServADOs.OrderBy(o => o.SERVICE_NAME).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal override void SetSingleKey()
        {
            try
            {
                keyValues.Add(new KeyValue(Mps000196ExtendSingleKey.TREATMENT_CODE, treatment.TREATMENT_CODE));
                keyValues.Add(new KeyValue(Mps000196ExtendSingleKey.USERNAME_RETURN_RESULT, bordereauSingleValue.userNameReturnResult));
                keyValues.Add(new KeyValue(Mps000196ExtendSingleKey.STATUS_TREATMENT_OUT, bordereauSingleValue.statusTreatmentOut));

                if (departmentTrans != null && departmentTrans.Count > 0)
                {
                    keyValues.Add(new KeyValue(Mps000196ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(departmentTrans[0].LOG_TIME)));
                    if (departmentTrans[departmentTrans.Count - 1] != null && departmentTrans.Count > 1)
                    {
                        keyValues.Add(new KeyValue(Mps000196ExtendSingleKey.DEPARTMENT_NAME_CLOSE_TREATMENT, departmentTrans[departmentTrans.Count - 1].DEPARTMENT_NAME));
                    }
                }

                if (treatment != null)
                {
                    if (treatment.CLINICAL_IN_TIME.HasValue)
                    {
                        keyValues.Add(new KeyValue(Mps000196ExtendSingleKey.CLINICAL_IN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(treatment.CLINICAL_IN_TIME.Value)));
                    }

                    if (treatment.OUT_TIME.HasValue)
                    {
                        keyValues.Add(new KeyValue(Mps000196ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(treatment.OUT_TIME.Value)));
                    }
                }
                keyValues.Add(new KeyValue(Mps000196ExtendSingleKey.TOTAL_DAY, bordereauSingleValue.today));

                if (tranPati != null)
                {
                    keyValues.Add(new KeyValue(Mps000196ExtendSingleKey.TRAN_PATI_MEDI_ORG_CODE, tranPati.MEDI_ORG_CODE));
                    keyValues.Add(new KeyValue(Mps000196ExtendSingleKey.TRAN_PATI_MEDI_ORG_NAME, tranPati.MEDI_ORG_NAME));
                }

                if (!String.IsNullOrEmpty(bordereauSingleValue.departmentName))
                {
                    keyValues.Add(new KeyValue(Mps000196ExtendSingleKey.DEPARTMENT_NAME, bordereauSingleValue.departmentName));
                }

                string executeRoomExam = "";
                string executeRoomExamFirst = "";
                string executeRoomExamLast = "";
                decimal thanhtien_tong = 0;
                decimal bhytthanhtoan_tong = 0;
                decimal nguonkhac_tong = 0;
                decimal bnthanhtoan_tong = 0;

                if (sereServADOs != null && sereServADOs.Count > 0)
                {
                    var sereServExamADOs = sereServADOs.Where(o => o.SERVICE_TYPE_CODE == heinServiceTypeCfg.EXAM_CODE).OrderBy(o => o.CREATE_TIME).ToList();

                    if (sereServExamADOs != null && sereServExamADOs.Count > 0)
                    {
                        executeRoomExamFirst = sereServExamADOs[0].EXECUTE_ROOM_NAME;
                        executeRoomExamLast = sereServExamADOs[sereServExamADOs.Count - 1].EXECUTE_ROOM_NAME;
                        foreach (var sereServExamADO in sereServExamADOs)
                        {
                            executeRoomExam += sereServExamADO.EXECUTE_ROOM_NAME + ", ";
                        }
                    }

                    thanhtien_tong = sereServADOs.Sum(o => o.VIR_TOTAL_PRICE) ?? -999;
                    bhytthanhtoan_tong = sereServADOs.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    bnthanhtoan_tong = sereServADOs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
                    nguonkhac_tong = 0;
                }

                keyValues.Add(new KeyValue(Mps000196ExtendSingleKey.EXECUTE_ROOM_EXAM, executeRoomExam));
                keyValues.Add(new KeyValue(Mps000196ExtendSingleKey.FIRST_EXAM_ROOM_NAME, executeRoomExamFirst));
                keyValues.Add(new KeyValue(Mps000196ExtendSingleKey.LAST_EXAM_ROOM_NAME, executeRoomExamLast));

                keyValues.Add(new KeyValue(Mps000196ExtendSingleKey.TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong, 0)));
                keyValues.Add(new KeyValue(Mps000196ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                keyValues.Add(new KeyValue(Mps000196ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong, 0)));
                keyValues.Add(new KeyValue(Mps000196ExtendSingleKey.TOTAL_PRICE_OTHER, Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguonkhac_tong, 0)));
                keyValues.Add(new KeyValue(Mps000196ExtendSingleKey.TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong).ToString())));
                keyValues.Add(new KeyValue(Mps000196ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                keyValues.Add(new KeyValue(Mps000196ExtendSingleKey.TOTAL_PRICE_PATIENT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong).ToString())));
                keyValues.Add(new KeyValue(Mps000196ExtendSingleKey.TOTAL_PRICE_OTHER_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(nguonkhac_tong).ToString())));

                if (treatmentFees != null)
                {
                    keyValues.Add(new KeyValue(Mps000196ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0, 0)));

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

                    totalPrice = treatmentFees[0].TOTAL_PRICE ?? 0; // tong tien
                    totalHeinPrice = treatmentFees[0].TOTAL_HEIN_PRICE ?? 0;
                    totalPatientPrice = treatmentFees[0].TOTAL_PATIENT_PRICE ?? 0; // bn tra
                    totalDeposit = treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0;
                    totalBill = treatmentFees[0].TOTAL_BILL_AMOUNT ?? 0;
                    totalBillTransferAmount = treatmentFees[0].TOTAL_BILL_TRANSFER_AMOUNT ?? 0;
                    exemption = 0;// HospitalFeeSum[0].TOTAL_EXEMPTION ?? 0;
                    totalRepay = treatmentFees[0].TOTAL_REPAY_AMOUNT ?? 0;
                    total_obtained_price = (totalDeposit + totalBill - totalBillTransferAmount - totalRepay + exemption);//Da thu benh nhan
                    decimal transfer = totalPatientPrice - total_obtained_price;//Phai thu benh nhan
                    depositPlus = transfer;//Nop them

                    keyValues.Add(new KeyValue(Mps000196ExtendSingleKey.TREATMENT_FEE_TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalPrice, 0)));
                    keyValues.Add(new KeyValue(Mps000196ExtendSingleKey.TREATMENT_FEE_TOTAL_PATIENT_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalPatientPrice, 0)));
                    keyValues.Add(new KeyValue(Mps000196ExtendSingleKey.TREATMENT_FEE_TOTAL_OBTAINED_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(total_obtained_price, 0)));
                    keyValues.Add(new KeyValue(Mps000196ExtendSingleKey.TREATMENT_FEE_TRANSFER, Inventec.Common.Number.Convert.NumberToStringRoundAuto(transfer, 0)));
                    GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TREATMENT_FEE>(treatmentFees[0], keyValues, false);
                }
                else
                {
                    keyValues.Add(new KeyValue(Mps000196ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, "0"));
                }

                keyValues.Add(new KeyValue(Mps000196ExtendSingleKey.CREATOR_USERNAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));

                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(patientADO, keyValues);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TREATMENT>(treatment, keyValues, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void HeinServiceTypeProcess()
        {
            try
            {
                heinServiceTypes = new List<MPS.ADO.Bordereau.SereServADO>();

                var sereServBHYTGroups = sereServADOs.OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999)
                    .GroupBy(o => o.HEIN_SERVICE_TYPE_ID).ToList();

                foreach (var sereServBHYTGroup in sereServBHYTGroups)
                {
                    MPS.ADO.Bordereau.SereServADO heinServiceType = new MPS.ADO.Bordereau.SereServADO();

                    V_HIS_SERE_SERV sereServBHYT = sereServBHYTGroup.FirstOrDefault();
                    if (sereServBHYT.HEIN_SERVICE_TYPE_ID.HasValue)
                    {
                        heinServiceType.HEIN_SERVICE_TYPE_ID = sereServBHYT.HEIN_SERVICE_TYPE_ID.Value;
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
                    heinServiceTypes.Add(heinServiceType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}