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

namespace MPS.Core.Mps000186
{
    /// <summary>
    /// In bang ke thanh toan ra vien ngoai tru va noi tru.
    /// </summary>
    public class Mps000186RDO : RDOBase
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
        internal List<MPS.ADO.Bordereau.SereServADO> sereServExecuteRoomADOs { get; set; }
        internal List<MPS.ADO.Bordereau.SereServADO> sereServRequestRoomADOs { get; set; }
        internal List<MPS.ADO.Bordereau.SereServADO> sereServServiceADOs { get; set; }
        internal V_HIS_SERE_SERV sereServPTTT { get; set; }
        internal List<MPS.ADO.Bordereau.SereServADO> heinServiceTypes { get; set; }
        internal List<MPS.ADO.Bordereau.SereServADO> heinServiceExecuteRoomTypes { get; set; }
        internal List<MPS.ADO.Bordereau.SereServADO> heinServiceRequestRoomTypes { get; set; }
        internal List<MPS.ADO.Bordereau.SereServADO> requestDepartments { get; set; }
        internal List<MPS.ADO.Bordereau.SereServADO> executeRooms { get; set; }
        internal List<MPS.ADO.Bordereau.SereServADO> RequestRooms { get; set; }
        internal HeinServiceTypeCFG heinServiceTypeCfg { get; set; }
        internal ServiceTypeCFG serviceTypeCfg { get; set; }
        internal PatientTypeCFG patienTypeCFG { get; set; }
        internal List<V_HIS_SERVICE_PATY> ServicePatys { get; set; }
        internal BordereauSingleValue BodereauSingleValue { get; set; }
        internal List<HIS_EXECUTE_ROOM> ExecuteRoomAlls { get; set; }
        internal List<V_HIS_ROOM> RoomAlls { get; set; }
        internal RoomTypeCFG RoomTypeCFG { get; set; }

        public Mps000186RDO(
            V_HIS_PATIENT _patient,
            V_HIS_PATY_ALTER_BHYT _patyAlterBhyt,
            List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> _departmentTrans,
            List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE> _treatmentFees,
            HeinServiceTypeCFG _heinServiceTypeCfg,
            PatientTypeCFG _patientTypeCFG,
            List<V_HIS_SERE_SERV> _sereServ,
            List<V_HIS_SERVICE_PATY> _servicePatys,
            V_HIS_TREATMENT _treatment,
            V_HIS_TRAN_PATI _tranPati,
            BordereauSingleValue _BodereauSingleValue,
            List<HIS_EXECUTE_ROOM> _ExecuteRoomAlls,
            List<V_HIS_ROOM> _RoomAlls,
            RoomTypeCFG _RoomTypeCFG
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
                this.patienTypeCFG = _patientTypeCFG;
                this.BodereauSingleValue = _BodereauSingleValue;
                this.ServicePatys = _servicePatys;
                this.ExecuteRoomAlls = _ExecuteRoomAlls;
                this.RoomAlls = _RoomAlls;
                this.RoomTypeCFG = _RoomTypeCFG;
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

                int treatmentNumber = sereServs != null ? sereServs.Select(o => o.INTRUCTION_TIME).Distinct().Count() : 0;

                sereServADOTemps.AddRange(from r in sereServs
                                          select new ADO.Bordereau.SereServADO(r, patyAlterBHYT, RoomAlls));

                #region Doi tuong thanh toan la dich vu
                var sereServServiceGroups = sereServADOTemps
                    .Where(o =>
                        o.AMOUNT > 0
                        && o.IS_EXPEND == null
                        && o.PATIENT_TYPE_ID == patienTypeCFG.PATIENT_TYPE_SERVICE_ID
                        && o.IS_NO_EXECUTE == null)
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999)
                    .GroupBy(o => new
                    {
                        o.SERVICE_ID,
                        o.TOTAL_HEIN_PRICE_ONE_AMOUNT,
                        o.IS_EXPEND,
                        o.REQUEST_DEPARTMENT_ID
                    }).ToList();

                if (sereServServiceGroups.Count>0)
                    Inventec.Common.Logging.LogSystem.Info("Doi tuong la thanh toan dich vu :" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patienTypeCFG.PATIENT_TYPE_SERVICE_ID), patienTypeCFG.PATIENT_TYPE_SERVICE_ID));
                    
                foreach (var sereServServiceGroup in sereServServiceGroups)
                {
                    ADO.Bordereau.SereServADO sereServ = sereServServiceGroup.FirstOrDefault();
                    sereServ.AMOUNT = sereServServiceGroup.Sum(o => o.AMOUNT);
                    sereServ.VIR_TOTAL_HEIN_PRICE = sereServServiceGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                    sereServ.VIR_TOTAL_PRICE = sereServServiceGroup.Sum(o => o.VIR_TOTAL_PRICE);
                    sereServ.VIR_TOTAL_PATIENT_PRICE = sereServServiceGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                    sereServADOs.Add(sereServ);
                }
                #endregion

                #region Dich vu co chenh lech ( bao hiem y te , thu phi )
                var sereServDifferenceGroups = sereServADOTemps
                    .Where(o =>
                        o.AMOUNT > 0
                        && o.IS_EXPEND == null
                        && o.PATIENT_TYPE_ID != patienTypeCFG.PATIENT_TYPE_SERVICE_ID
                        && o.PRICE_CO_PAYMENT > 0
                        && o.IS_NO_EXECUTE == null)
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999)
                    .GroupBy(o => new
                    {
                        o.SERVICE_ID,
                        o.TOTAL_HEIN_PRICE_ONE_AMOUNT,
                        o.REQUEST_DEPARTMENT_ID
                    }).ToList();

                if (sereServDifferenceGroups.Count > 0)
                    Inventec.Common.Logging.LogSystem.Info("Doi tuong la chenh lech thu phi, bhyt" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServDifferenceGroups), sereServDifferenceGroups));


                foreach (var sereServDifferenceGroup in sereServDifferenceGroups)
                {
                    decimal priceCoPayment = sereServDifferenceGroup.Sum(o => o.PRICE_CO_PAYMENT.Value);
                    ADO.Bordereau.SereServADO sereServ = sereServDifferenceGroup.FirstOrDefault();
                    sereServ.AMOUNT = sereServDifferenceGroup.Sum(o => o.AMOUNT);
                    sereServ.VIR_TOTAL_HEIN_PRICE = sereServDifferenceGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                    sereServ.VIR_PRICE = priceCoPayment;
                    sereServ.VIR_TOTAL_PRICE = priceCoPayment * sereServ.AMOUNT;
                    sereServ.VIR_TOTAL_PATIENT_PRICE = priceCoPayment * sereServ.AMOUNT;
                    sereServADOs.Add(sereServ);
                }
                #endregion

                #region Vuot dinh muc hao phi dich vu ky thuat
                var sereServDifferenceExpendGroups = sereServADOTemps
                    .Where(o =>
                        o.AMOUNT > 0
                        && o.IS_EXPEND == null
                        && o.PRICE_DIFFERENCES_EXPEND > 0
                        && o.IS_NO_EXECUTE == null)
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999)
                    .GroupBy(o => new
                    {
                        o.SERVICE_ID,
                        o.TOTAL_HEIN_PRICE_ONE_AMOUNT,
                        o.REQUEST_DEPARTMENT_ID
                    }).ToList();

                if (sereServDifferenceExpendGroups.Count > 0)
                    Inventec.Common.Logging.LogSystem.Info("Vuot muc hao phi" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServDifferenceExpendGroups), sereServDifferenceExpendGroups));

                foreach (var sereServDifferenceExpendGroup in sereServDifferenceExpendGroups)
                {
                    decimal priceDifferencesExpend = sereServDifferenceExpendGroup.Sum(o => o.PRICE_DIFFERENCES_EXPEND.Value);
                    ADO.Bordereau.SereServADO sereServ = sereServDifferenceExpendGroup.FirstOrDefault();
                    sereServ.AMOUNT = sereServDifferenceExpendGroup.Sum(o => o.AMOUNT);
                    sereServ.VIR_TOTAL_HEIN_PRICE = sereServDifferenceExpendGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                    sereServ.VIR_PRICE = priceDifferencesExpend;
                    sereServ.VIR_TOTAL_PRICE = priceDifferencesExpend * sereServ.AMOUNT;
                    sereServ.VIR_TOTAL_PATIENT_PRICE = priceDifferencesExpend * sereServ.AMOUNT;
                    sereServADOs.Add(sereServ);
                }
                #endregion

                List<ADO.Bordereau.SereServADO> sereServNoExams = new List<ADO.Bordereau.SereServADO>();
                sereServExecuteRoomADOs = (sereServADOs != null && sereServADOs.Count > 0) ? sereServADOs.Where(o => o.HEIN_SERVICE_TYPE_CODE == heinServiceTypeCfg.EXAM_CODE).ToList() : new List<ADO.Bordereau.SereServADO>();
                foreach (var item in sereServExecuteRoomADOs)
                {
                    item.EXECUTE_ROOM_ID = item.REQUEST_ROOM_ID;
                }
                sereServNoExams = (sereServADOs != null && sereServADOs.Count > 0) ? sereServADOs.Except(sereServExecuteRoomADOs).ToList() : null;

                sereServRequestRoomADOs = new List<ADO.Bordereau.SereServADO>();
                sereServServiceADOs = new List<ADO.Bordereau.SereServADO>();
                if (sereServNoExams != null && sereServNoExams.Count > 0)
                {
                    foreach (var sereServNoExam in sereServNoExams)
                    {
                        if (sereServNoExam.ROOM_TYPE_ID == RoomTypeCFG.ROOM_TYPE_ID__BED)
                            sereServServiceADOs.Add(sereServNoExam);
                        if (sereServNoExam.ROOM_TYPE_ID == RoomTypeCFG.ROOM_TYPE_ID__RECEPTION)
                            sereServRequestRoomADOs.Add(sereServNoExam);
                        if (sereServNoExam.ROOM_TYPE_ID == RoomTypeCFG.ROOM_TYPE_ID__EXECUTE)
                        {
                            if (ExecuteRoomAlls != null && ExecuteRoomAlls.Count > 0)
                            {
                                HIS_EXECUTE_ROOM executeRoom = ExecuteRoomAlls.FirstOrDefault(o => o.ROOM_ID == sereServNoExam.REQUEST_ROOM_ID);
                                if (executeRoom != null && executeRoom.IS_EXAM == 1)
                                {
                                    if (executeRoom.ROOM_ID == sereServNoExam.REQUEST_ROOM_ID)
                                    {
                                        sereServNoExam.EXECUTE_ROOM_ID = executeRoom.ID;
                                        sereServNoExam.EXECUTE_ROOM_NAME = executeRoom.EXECUTE_ROOM_NAME;
                                        sereServExecuteRoomADOs.Add(sereServNoExam);
                                    }
                                    else
                                        sereServRequestRoomADOs.Add(sereServNoExam);
                                }
                                else
                                    sereServServiceADOs.Add(sereServNoExam);
                            }
                        }
                    }
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
                keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.RATIO_STR, BodereauSingleValue.ratio));
                keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.TREATMENT_CODE, treatment.TREATMENT_CODE));
                keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.USERNAME_RETURN_RESULT, BodereauSingleValue.userNameReturnResult));
                keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.STATUS_TREATMENT_OUT, BodereauSingleValue.statusTreatmentOut));
                keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.MEDI_STOCK_NAME, BodereauSingleValue.mediStockName));

                if (patyAlterBHYTADO != null)
                {
                    if (patyAlterBHYTADO.IS_HEIN != null)
                        keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.IS_HEIN, "X"));
                    else
                        keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.IS_NOT_HEIN, "X"));
                    if (patyAlterBHYTADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (patyAlterBHYTADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                        {
                            keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (patyAlterBHYTADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (patyAlterBHYTADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }

                    //Dia chi the
                    keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.HEIN_CARD_ADDRESS, patyAlterBHYTADO.ADDRESS));
                }
                else
                    keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.IS_NOT_HEIN, "X"));

                if (departmentTrans != null && departmentTrans.Count > 0)
                {
                    keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(departmentTrans[0].LOG_TIME)));
                    if (departmentTrans[departmentTrans.Count - 1] != null && departmentTrans.Count > 1)
                    {
                        keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.DEPARTMENT_NAME_CLOSE_TREATMENT, departmentTrans[departmentTrans.Count - 1].DEPARTMENT_NAME));
                    }
                }

                if (treatment != null)
                {
                    if (treatment.CLINICAL_IN_TIME.HasValue)
                    {
                        keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.CLINICAL_IN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(treatment.CLINICAL_IN_TIME.Value)));
                    }

                    if (treatment.OUT_TIME.HasValue)
                    {
                        keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(treatment.OUT_TIME.Value)));
                    }
                }
                keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.TOTAL_DAY, BodereauSingleValue.today));


                if (tranPati != null)
                {
                    keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.TRAN_PATI_MEDI_ORG_CODE, tranPati.MEDI_ORG_CODE));
                    keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.TRAN_PATI_MEDI_ORG_NAME, tranPati.MEDI_ORG_NAME));
                }

                keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.CURRENT_DATE_SEPARATE_FULL_STR, BodereauSingleValue.currentDateSeparateFullTime));

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

                    thanhtien_tong = sereServADOs.Sum(o => o.VIR_TOTAL_PRICE) ?? 0;
                    bhytthanhtoan_tong = sereServADOs.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    bnthanhtoan_tong = sereServADOs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
                    nguonkhac_tong = 0;
                }

                keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.EXECUTE_ROOM_EXAM, executeRoomExam));
                keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.FIRST_EXAM_ROOM_NAME, executeRoomExamFirst));
                keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.LAST_EXAM_ROOM_NAME, executeRoomExamLast));


                if (!String.IsNullOrEmpty(BodereauSingleValue.departmentName))
                {
                    keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.DEPARTMENT_NAME, BodereauSingleValue.departmentName));
                }

                keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong, 0)));
                keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong, 0)));
                keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.TOTAL_PRICE_OTHER, Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguonkhac_tong, 0)));
                keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong).ToString())));
                keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.TOTAL_PRICE_PATIENT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong).ToString())));
                keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.TOTAL_PRICE_OTHER_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(nguonkhac_tong).ToString())));

                if (sereServPTTT != null)
                {
                    decimal maxExpend = sereServPTTT.MAX_EXPEND.HasValue ? sereServPTTT.MAX_EXPEND.Value : 0;
                    decimal priceDifferences = thanhtien_tong - maxExpend;
                    keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.PRICE_MAX_EXPEND, Inventec.Common.Number.Convert.NumberToStringRoundAuto(maxExpend, 0)));
                    keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.PRICE_DIFFERENCES, Inventec.Common.Number.Convert.NumberToStringRoundAuto(priceDifferences, 0)));
                    keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.PTTT_EXECUTE_ROOM_NAME, sereServPTTT.EXECUTE_ROOM_NAME));
                    keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.PTTT_EXECUTE_DEPARTMENT_NAME, sereServPTTT.EXECUTE_DEPARTMENT_NAME));
                    keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.PRICE_DIFFERENCES_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(priceDifferences).ToString())));
                }

                if (treatmentFees != null)
                {
                    keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0, 0)));
                }
                else
                {
                    keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, "0"));
                }

                keyValues.Add(new KeyValue(Mps000186ExtendSingleKey.CREATOR_USERNAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));

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
                heinServiceExecuteRoomTypes = new List<ADO.Bordereau.SereServADO>();
                heinServiceRequestRoomTypes = new List<ADO.Bordereau.SereServADO>();
                heinServiceTypes = new List<MPS.ADO.Bordereau.SereServADO>();
                executeRooms = new List<ADO.Bordereau.SereServADO>();
                RequestRooms = new List<ADO.Bordereau.SereServADO>();
                requestDepartments = new List<ADO.Bordereau.SereServADO>();

                if (sereServExecuteRoomADOs != null && sereServExecuteRoomADOs.Count > 0)
                {
                    var sereServExecuteRoomGroups = sereServExecuteRoomADOs.GroupBy(o => o.EXECUTE_ROOM_ID).ToList();
                    foreach (var sereServExecuteRoomGroup in sereServExecuteRoomGroups)
                    {
                        MPS.ADO.Bordereau.SereServADO executeRoom = sereServExecuteRoomGroup.FirstOrDefault();
                        executeRoom.TOTAL_PRICE_ROOM = sereServExecuteRoomGroup.Sum(o => o.VIR_TOTAL_PRICE);
                        executeRoom.TOTAL_PATIENT_PRICE_ROOM = sereServExecuteRoomGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                        executeRoom.TOTAL_HEIN_PRICE_ROOM = sereServExecuteRoomGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                        executeRooms.Add(executeRoom);
                        var sereServBHYTHeinServiceTypeGroups = sereServExecuteRoomGroup
                            .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 9999999999)
                            .GroupBy(o => o.HEIN_SERVICE_TYPE_ID).ToList();
                        foreach (var sereServBHYTHeinServiceTypeGroup in sereServBHYTHeinServiceTypeGroups)
                        {
                            MPS.ADO.Bordereau.SereServADO heinServiceType = sereServBHYTHeinServiceTypeGroup.First();
                            heinServiceType.HEIN_SERVICE_TYPE_NAME = heinServiceType.HEIN_SERVICE_TYPE_ID.HasValue ? heinServiceType.HEIN_SERVICE_TYPE_NAME : "Khác";
                            heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE = sereServBHYTHeinServiceTypeGroup.Sum(o => o.VIR_TOTAL_PRICE);
                            heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = sereServBHYTHeinServiceTypeGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                            heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = sereServBHYTHeinServiceTypeGroup
                                .Sum(o => o.VIR_TOTAL_PATIENT_PRICE.Value);
                            heinServiceExecuteRoomTypes.Add(heinServiceType);
                        }
                    }
                }

                if (sereServRequestRoomADOs != null && sereServRequestRoomADOs.Count > 0)
                {
                    var sereServRequestRoomGroups = sereServRequestRoomADOs.GroupBy(o => o.REQUEST_ROOM_ID).ToList();
                    foreach (var sereServRequestRoomGroup in sereServRequestRoomGroups)
                    {
                        MPS.ADO.Bordereau.SereServADO requestRoom = sereServRequestRoomGroup.FirstOrDefault();
                        requestRoom.TOTAL_PRICE_ROOM = sereServRequestRoomGroup.Sum(o => o.VIR_TOTAL_PRICE);
                        requestRoom.TOTAL_PATIENT_PRICE_ROOM = sereServRequestRoomGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                        requestRoom.TOTAL_HEIN_PRICE_ROOM = sereServRequestRoomGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                        RequestRooms.Add(requestRoom);
                        var sereServBHYTHeinServiceTypeGroups = sereServRequestRoomGroup
                            .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 9999999999)
                            .GroupBy(o => o.HEIN_SERVICE_TYPE_ID).ToList();
                        foreach (var sereServBHYTHeinServiceTypeGroup in sereServBHYTHeinServiceTypeGroups)
                        {
                            MPS.ADO.Bordereau.SereServADO heinServiceType = sereServBHYTHeinServiceTypeGroup.First();
                            heinServiceType.HEIN_SERVICE_TYPE_NAME = heinServiceType.HEIN_SERVICE_TYPE_ID.HasValue ? heinServiceType.HEIN_SERVICE_TYPE_NAME : "Khác";
                            heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE = sereServBHYTHeinServiceTypeGroup.Sum(o => o.VIR_TOTAL_PRICE);
                            heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = sereServBHYTHeinServiceTypeGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                            heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = sereServBHYTHeinServiceTypeGroup
                                .Sum(o => o.VIR_TOTAL_PATIENT_PRICE.Value);
                            heinServiceRequestRoomTypes.Add(heinServiceType);
                        }
                    }
                }

                if (sereServServiceADOs != null && sereServServiceADOs.Count > 0)
                {

                    var sereServBHYTDepartmentGroups = sereServServiceADOs.GroupBy(o => o.REQUEST_DEPARTMENT_ID).ToList();
                    foreach (var sereServBHYTDepartmentGroup in sereServBHYTDepartmentGroups)
                    {
                        MPS.ADO.Bordereau.SereServADO requestDepartment = sereServBHYTDepartmentGroup.FirstOrDefault();
                        requestDepartment.REQUEST_DEPARTMENT_ID = sereServBHYTDepartmentGroup.First().REQUEST_DEPARTMENT_ID;
                        requestDepartment.TOTAL_PRICE_DEPARTMENT = sereServBHYTDepartmentGroup.Sum(o => o.VIR_TOTAL_PRICE);
                        requestDepartment.TOTAL_PATIENT_PRICE_DEPARTMENT = sereServBHYTDepartmentGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                        requestDepartment.TOTAL_HEIN_PRICE_DEPARTMENT = sereServBHYTDepartmentGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                        requestDepartments.Add(requestDepartment);
                        var sereServBHYTHeinServiceTypeGroups = sereServBHYTDepartmentGroup
                            .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 9999999999)
                            .GroupBy(o => o.HEIN_SERVICE_TYPE_ID).ToList();
                        foreach (var sereServBHYTHeinServiceTypeGroup in sereServBHYTHeinServiceTypeGroups)
                        {
                            MPS.ADO.Bordereau.SereServADO heinServiceType = sereServBHYTHeinServiceTypeGroup.First();
                            heinServiceType.HEIN_SERVICE_TYPE_NAME = heinServiceType.HEIN_SERVICE_TYPE_ID.HasValue ? heinServiceType.HEIN_SERVICE_TYPE_NAME : "Khác";
                            heinServiceType.TOTAL_PRICE_HEIN_SERVICE_TYPE = sereServBHYTHeinServiceTypeGroup.Sum(o => o.VIR_TOTAL_PRICE);
                            heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = sereServBHYTHeinServiceTypeGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE.Value);
                            heinServiceType.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE = sereServBHYTHeinServiceTypeGroup
                                .Sum(o => o.VIR_TOTAL_PATIENT_PRICE.Value);

                            heinServiceTypes.Add(heinServiceType);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

}
