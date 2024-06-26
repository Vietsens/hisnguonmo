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

namespace MPS.Core.Mps000176
{
    /// <summary>
    /// In bang ke thanh toan ra vien ngoai tru va noi tru.
    /// </summary>
    public class Mps000176RDO : RDOBase
    {
        internal V_HIS_PATIENT patient { get; set; }
        internal V_HIS_PATY_ALTER_BHYT patyAlterBHYT { get; set; }
        internal List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans { get; set; }
        internal List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE> treatmentFees { get; set; }
        internal List<V_HIS_SERE_SERV> sereServs { get; set; }
        internal V_HIS_TREATMENT treatment { get; set; }
        internal V_HIS_TRAN_PATI tranPati { get; set; }
        internal PatientADO patientADO { get; set; }
        internal PatyAlterBhytADO patyAlterBHYTADO { get; set; }
        internal List<MPS.ADO.Bordereau.SereServADO> sereServADOs { get; set; }
        internal V_HIS_SERE_SERV sereServPTTT { get; set; }
        internal List<MPS.ADO.Bordereau.SereServADO> heinServiceTypes { get; set; }
        internal List<MPS.ADO.Bordereau.SereServADO> requestRooms { get; set; }
        internal HeinServiceTypeCFG heinServiceTypeCfg { get; set; }
        internal ServiceTypeCFG serviceTypeCfg { get; set; }
        internal BordereauSingleValue BodereauSingleValue { get; set; }

        public Mps000176RDO(
            V_HIS_PATIENT _patient,
            V_HIS_PATY_ALTER_BHYT _patyAlterBhyt,
            List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> _departmentTrans,
            List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE> _treatmentFees,
            HeinServiceTypeCFG _heinServiceTypeCfg,
            List<V_HIS_SERE_SERV> _sereServ,
            V_HIS_SERE_SERV _sereServPTTT,
            V_HIS_TREATMENT _treatment,
            V_HIS_TRAN_PATI _tranPati,
            BordereauSingleValue _BodereauSingleValue
            )
        {
            try
            {
                this.patient = _patient;
                this.patyAlterBHYT = _patyAlterBhyt;
                this.sereServs = _sereServ;
                this.treatment = _treatment;
                this.tranPati = _tranPati;
                this.departmentTrans = _departmentTrans;
                this.treatmentFees = _treatmentFees;
                this.heinServiceTypeCfg = _heinServiceTypeCfg;
                this.sereServPTTT = _sereServPTTT;
                this.BodereauSingleValue = _BodereauSingleValue;
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

                //BHYT ADO
                patyAlterBHYTADO = DataRawProcess.PatyAlterBHYTRawToADO(patyAlterBHYT);

                sereServADOs = new List<ADO.Bordereau.SereServADO>();
                //SereServADO
                var sereServADOTemps = new List<ADO.Bordereau.SereServADO>();
                sereServADOTemps.AddRange(from r in sereServs
                                          select new ADO.Bordereau.SereServADO(r, patyAlterBHYT));

                //sereServ la bhyt, gom nhom
                var sereServGroups = sereServADOTemps
                    .Where(o =>
                        o.AMOUNT > 0
                        && o.PARENT_ID == sereServPTTT.ID
                        && (o.MEDICINE_ID != null || o.MATERIAL_ID != null)
                        && o.IS_EXPEND == 1
                        && o.IS_NO_EXECUTE == null)
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999)
                    .GroupBy(o => new
                    {
                        o.SERVICE_ID,
                        o.TOTAL_HEIN_PRICE_ONE_AMOUNT,
                        o.IS_EXPEND
                    }).ToList();

                foreach (var sereServGroup in sereServGroups)
                {
                    ADO.Bordereau.SereServADO sereServ = sereServGroup.FirstOrDefault();
                    sereServ.AMOUNT = sereServGroup.Sum(o => o.AMOUNT);
                    sereServ.VIR_TOTAL_HEIN_PRICE = sereServGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                    sereServ.VIR_TOTAL_PRICE_NO_EXPEND = sereServGroup
                        .Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                    sereServADOs.Add(sereServ);
                }


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
                keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.RATIO_STR, BodereauSingleValue.ratio));
                keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.TREATMENT_CODE, treatment.TREATMENT_CODE));
                keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.USERNAME_RETURN_RESULT, BodereauSingleValue.userNameReturnResult));
                keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.STATUS_TREATMENT_OUT, BodereauSingleValue.statusTreatmentOut));
                keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.MEDI_STOCK_NAME, BodereauSingleValue.mediStockName));

                if (patyAlterBHYTADO != null)
                {
                    if (patyAlterBHYTADO.IS_HEIN != null)
                        keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.IS_HEIN, "X"));
                    else
                        keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.IS_NOT_HEIN, "X"));
                    if (patyAlterBHYTADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (patyAlterBHYTADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                        {
                            keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (patyAlterBHYTADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (patyAlterBHYTADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }

                    //Dia chi the
                    keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.HEIN_CARD_ADDRESS, patyAlterBHYTADO.ADDRESS));
                }
                else
                    keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.IS_NOT_HEIN, "X"));

                if (departmentTrans != null && departmentTrans.Count > 0)
                {
                    keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(departmentTrans[0].LOG_TIME)));
                    if (departmentTrans[departmentTrans.Count - 1] != null && departmentTrans.Count > 1)
                    {
                        keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.DEPARTMENT_NAME_CLOSE_TREATMENT, departmentTrans[departmentTrans.Count - 1].DEPARTMENT_NAME));
                    }
                }

                if (treatment != null)
                {
                    if (treatment.CLINICAL_IN_TIME.HasValue)
                    {
                        keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.CLINICAL_IN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(treatment.CLINICAL_IN_TIME.Value)));
                    }

                    if (treatment.OUT_TIME.HasValue)
                    {
                        keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(treatment.OUT_TIME.Value)));
                    }
                }
                keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.TOTAL_DAY, BodereauSingleValue.today));


                if (tranPati != null)
                {
                    keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.TRAN_PATI_MEDI_ORG_CODE, tranPati.MEDI_ORG_CODE));
                    keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.TRAN_PATI_MEDI_ORG_NAME, tranPati.MEDI_ORG_NAME));
                }

                keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.CURRENT_DATE_SEPARATE_FULL_STR, BodereauSingleValue.currentDateSeparateFullTime));

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

                    thanhtien_tong = sereServADOs.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND) ?? 0;
                    bhytthanhtoan_tong = sereServADOs.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    bnthanhtoan_tong = sereServADOs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT) ?? 0;
                    nguonkhac_tong = 0;
                }

                keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.EXECUTE_ROOM_EXAM, executeRoomExam));
                keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.FIRST_EXAM_ROOM_NAME, executeRoomExamFirst));
                keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.LAST_EXAM_ROOM_NAME, executeRoomExamLast));


                if (!String.IsNullOrEmpty(BodereauSingleValue.departmentName))
                {
                    keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.DEPARTMENT_NAME, BodereauSingleValue.departmentName));
                }

                keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong, 0)));
                keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong, 0)));
                keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.TOTAL_PRICE_OTHER, Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguonkhac_tong, 0)));
                keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong).ToString())));
                keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.TOTAL_PRICE_PATIENT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong).ToString())));
                keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.TOTAL_PRICE_OTHER_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(nguonkhac_tong).ToString())));

                if (sereServPTTT != null)
                {
                    decimal maxExpend = sereServPTTT.MAX_EXPEND.HasValue ? sereServPTTT.MAX_EXPEND.Value : 0;
                    decimal priceDifferences = (thanhtien_tong - maxExpend) < 0 ? 0 : (thanhtien_tong - maxExpend);
                    keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.PRICE_MAX_EXPEND, Inventec.Common.Number.Convert.NumberToStringRoundAuto(maxExpend, 0)));
                    keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.PRICE_DIFFERENCES, Inventec.Common.Number.Convert.NumberToStringRoundAuto(priceDifferences, 0)));
                    keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.PTTT_EXECUTE_ROOM_NAME, sereServPTTT.EXECUTE_ROOM_NAME));
                    keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.PTTT_EXECUTE_DEPARTMENT_NAME, sereServPTTT.EXECUTE_DEPARTMENT_NAME));
                    keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.PRICE_DIFFERENCES_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(priceDifferences).ToString())));
                }

                if (treatmentFees != null)
                {
                    keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0, 0)));
                }
                else
                {
                    keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, "0"));
                }

                keyValues.Add(new KeyValue(Mps000176ExtendSingleKey.CREATOR_USERNAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));

                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(patientADO, keyValues);
                GlobalQuery.AddObjectKeyIntoListkey<PatyAlterBhytADO>(patyAlterBHYTADO, keyValues, false);
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

                    heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_BHYT);
                    heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE.Value);
                    heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup
                        .Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT.Value);

                    heinServiceTypes.Add(heinServiceType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //internal void HeinServiceTypeProcess()
        //{
        //    try
        //    {
        //        heinServiceTypes = new List<MPS.ADO.Bordereau.SereServADO>();

        //        var sereServBHYTGroups = sereServADOs.OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999)
        //            .GroupBy(o => o.HEIN_SERVICE_TYPE_ID).ToList();

        //        foreach (var sereServBHYTGroup in sereServBHYTGroups)
        //        {
        //            MPS.ADO.Bordereau.SereServADO heinServiceType = new MPS.ADO.Bordereau.SereServADO();

        //            V_HIS_SERE_SERV sereServBHYT = sereServBHYTGroup.FirstOrDefault();
        //            if (sereServBHYT.HEIN_SERVICE_TYPE_ID.HasValue)
        //            {
        //                heinServiceType.HEIN_SERVICE_TYPE_ID = sereServBHYT.HEIN_SERVICE_TYPE_ID.Value;
        //                heinServiceType.HEIN_SERVICE_TYPE_NAME = sereServBHYT.HEIN_SERVICE_TYPE_NAME;
        //            }
        //            else
        //            {
        //                heinServiceType.HEIN_SERVICE_TYPE_NAME = "Khác";
        //            }

        //            heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.TOTAL_PRICE_BHYT);
        //            heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE.Value);
        //            heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = sereServBHYTGroup
        //                .Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT.Value);

        //            heinServiceTypes.Add(heinServiceType);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}
    }

}
