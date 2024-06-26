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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Core.Mps000193
{
    /// <summary>
    /// In bang ke thanh toan ra vien ngoai tru va noi tru.
    /// </summary>
    public class Mps000193RDO : RDOBase
    {
        internal V_HIS_PATIENT patient { get; set; }
        internal V_HIS_PATIENT_TYPE_ALTER patyAlter { get; set; }
        internal string departmentName { get; set; }
        internal List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans { get; set; }
        internal List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE> treatmentFees { get; set; }
        internal List<V_HIS_SERE_SERV> sereServKTCs { get; set; }
        internal List<V_HIS_SERE_SERV> sereServs { get; set; }
        internal V_HIS_TREATMENT treatment { get; set; }
        internal long today { get; set; }
        internal V_HIS_TRAN_PATI tranPati { get; set; }
        internal List<HIS_EXECUTE_GROUP> executeGroups { get; set; }
        internal List<V_HIS_SERVICE_PATY_PRPO> servicePatyPrpos { get; set; }
        internal long departmentId;
        internal PatientADO patientADO { get; set; }
        internal PatyAlterBhytADO patyAlterBHYTADO { get; set; }
        internal List<MPS.ADO.Bordereau.SereServADO> sereServKTCADOs { get; set; }
        internal List<MPS.ADO.Bordereau.SereServADO> sereServADOs { get; set; }
        internal ServiceTypeCFG ServiceTypeCfg { get; set; }
        internal List<MPS.ADO.Bordereau.SereServADO> ktcFeeGroups { get; set; }
        internal List<MPS.ADO.Bordereau.SereServADO> sereServExecuteGroups { get; set; }
        internal List<MPS.ADO.Bordereau.SereServADO> heinServiceTypes { get; set; }
        internal string currentDateSeparateFullTime { get; set; }
        internal string userNameReturnResult { get; set; }
        internal string statusTreatmentOut { get; set; }

        public Mps000193RDO(
            V_HIS_PATIENT _patient,
            V_HIS_PATIENT_TYPE_ALTER _patyAlter,
            long _departmentId,
            string _departmentName,
            List<V_HIS_SERE_SERV> _sereServKTCs,
            List<V_HIS_SERE_SERV> _sereServs,
            List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> _departmentTrans,
            string _currentDateSeparateFullTime,
            V_HIS_TREATMENT _treatment,
            V_HIS_TRAN_PATI _tranPati,
            List<HIS_EXECUTE_GROUP> _executeGroups,
            long _today,
            List<V_HIS_SERVICE_PATY_PRPO> _servicePatyPrpos,
            ServiceTypeCFG _erviceTypeCfg,
            string _userNameReturnResult,
            string _statusTreatmentOut
            )
        {
            try
            {
                this.patient = _patient;
                this.sereServKTCs = _sereServKTCs;
                this.sereServs = _sereServs;
                this.treatment = _treatment;
                this.today = _today;
                this.patyAlter = _patyAlter;
                this.tranPati = _tranPati;
                this.servicePatyPrpos = _servicePatyPrpos;
                this.ServiceTypeCfg = _erviceTypeCfg;
                this.departmentName = _departmentName;
                this.departmentTrans = _departmentTrans;
                this.executeGroups = _executeGroups;
                this.currentDateSeparateFullTime = _currentDateSeparateFullTime;
                this.departmentId = _departmentId;
                this.userNameReturnResult = _userNameReturnResult;
                this.statusTreatmentOut = _statusTreatmentOut;
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
                patyAlterBHYTADO = DataRawProcess.PatyAlterBHYTRawToADO(patyAlter);

                sereServADOs = new List<ADO.Bordereau.SereServADO>();
                sereServKTCADOs = new List<ADO.Bordereau.SereServADO>();

                //SereServKTCADO
                var sereServKTCADOTemps = new List<ADO.Bordereau.SereServADO>();
                sereServKTCADOTemps.AddRange(from r in sereServKTCs
                                             select new ADO.Bordereau.SereServADO(r, servicePatyPrpos));
                var sereServKTCGroups = sereServKTCADOTemps
                    .Where(o => o.IS_NO_EXECUTE == null
                    && (o.VIR_PRICE_NO_EXPEND > 0 || o.PRICE_POLICY > 0)
                    && o.AMOUNT > 0)
                    .OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999)
                    .GroupBy(o => new
                    {
                        o.SERVICE_ID,
                        o.TOTAL_HEIN_PRICE_ONE_AMOUNT,
                        o.REQUEST_DEPARTMENT_ID,
                        o.IS_OUT_PARENT_FEE,
                        o.PATIENT_TYPE_ID,
                        o.PRICE_POLICY_ID,
                        o.IS_EXPEND,
                        o.PARENT_ID
                    });

                foreach (var sereServKTCGroup in sereServKTCGroups)
                {
                    ADO.Bordereau.SereServADO sereServ = sereServKTCGroup.FirstOrDefault();
                    sereServ.AMOUNT = sereServKTCGroup.Sum(o => o.AMOUNT);
                    sereServ.VIR_TOTAL_PRICE_NO_EXPEND = sereServKTCGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND);
                    sereServ.VIR_TOTAL_HEIN_PRICE = sereServKTCGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                    sereServKTCADOs.Add(sereServ);
                }

                //SereServADO
                var sereServADOTemps = new List<ADO.Bordereau.SereServADO>();
                sereServADOTemps.AddRange(from r in sereServs
                                          select new ADO.Bordereau.SereServADO(r, patyAlterBHYTADO));

                //sereServkhong phai la dịch vu ky thuat cao
                var sereServGroups = sereServADOTemps
                    .Where(o =>
                        (sereServKTCs != null ? !sereServKTCs.Select(p => p.ID).Contains(o.ID) : true)
                        && o.AMOUNT > 0
                        && o.PRICE_BHYT > 0
                        && o.IS_NO_EXECUTE == null)
                    .OrderBy(o => o.IS_OUT_PARENT_FEE ?? 99999)
                    .ThenBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999)
                    .ThenBy(o => o.SERVICE_NAME)
                    .GroupBy(o => new
                    {
                        o.PRICE_BHYT,
                        o.SERVICE_ID,
                        o.VIR_PRICE_NO_EXPEND,
                        o.REQUEST_DEPARTMENT_ID,
                        o.IS_OUT_PARENT_FEE,
                        o.IS_EXPEND,
                        o.PARENT_ID
                    });

                foreach (var sereServGroup in sereServGroups)
                {
                    ADO.Bordereau.SereServADO sereServ = sereServGroup.FirstOrDefault();
                    if (sereServ.PARENT_ID == null && (sereServ.IS_OUT_PARENT_FEE != 1 || sereServ.IS_OUT_PARENT_FEE == null)
                        || (sereServKTCADOs != null && sereServKTCADOs.Where(o => o.ID == sereServ.PARENT_ID).Count() == 0))
                    {
                        sereServ.IS_OUT_PARENT_FEE = 1;
                    }

                    sereServ.AMOUNT = sereServGroup.Sum(o => o.AMOUNT);
                    sereServ.TOTAL_PRICE_BHYT = sereServGroup.Sum(o => o.TOTAL_PRICE_BHYT);
                    sereServ.VIR_TOTAL_HEIN_PRICE = sereServGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                    sereServ.VIR_TOTAL_PATIENT_PRICE_BHYT = sereServGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT);
                    sereServADOs.Add(sereServ);
                }
                sereServADOs = sereServADOs.OrderBy(o => o.SERVICE_NAME).ToList();

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
                ktcFeeGroups = new List<ADO.Bordereau.SereServADO>();
                sereServExecuteGroups = new List<ADO.Bordereau.SereServADO>();
                heinServiceTypes = new List<ADO.Bordereau.SereServADO>();

                //Nhom chi phi trong, ngoai goi theo dich vu KTC
                var sereServKTCGroups = sereServADOs.GroupBy(o => o.IS_OUT_PARENT_FEE).ToList();
                foreach (var sereServKTCGroup in sereServKTCGroups)
                {
                    List<ADO.Bordereau.SereServADO> sereServADOTemps = sereServKTCGroup.ToList<ADO.Bordereau.SereServADO>();
                    ADO.Bordereau.SereServADO ktcFeeGroup = sereServKTCGroup.First();
                    if (ktcFeeGroup.IS_OUT_PARENT_FEE != 1 || ktcFeeGroup.IS_OUT_PARENT_FEE == null)
                    {
                        ktcFeeGroup.KTC_FEE_GROUP_NAME = "CHI PHÍ TRONG GÓI PHẪU THUẬT";
                    }
                    else
                    {
                        ktcFeeGroup.KTC_FEE_GROUP_NAME = "CHI PHÍ NGOÀI GÓI PHẪU THUẬT";
                    }

                    ktcFeeGroups.Add(ktcFeeGroup);

                    //Nhom Execute
                    var sereServExecuteGroupTemps = sereServADOTemps.GroupBy(o => o.EXECUTE_GROUP_ID).ToList();
                    foreach (var sereServExecuteGroupTemp in sereServExecuteGroupTemps)
                    {
                        ADO.Bordereau.SereServADO executeGroup = sereServExecuteGroupTemp.First();
                        if (executeGroup.EXECUTE_GROUP_ID == null)
                            executeGroup.EXECUTE_GROUP_NAME = "    ";
                        else
                            executeGroup.EXECUTE_GROUP_NAME = executeGroups.FirstOrDefault(o => o.ID == executeGroup.EXECUTE_GROUP_ID).EXECUTE_GROUP_NAME;
                        sereServExecuteGroups.Add(executeGroup);

                        //Nhom HeinServiceType
                        var heinServiceTypeGroups = sereServExecuteGroupTemp.OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 99999999999)
                            .GroupBy(o => o.HEIN_SERVICE_TYPE_ID).ToList();
                        foreach (var heinServiceTypeGroup in heinServiceTypeGroups)
                        {
                            ADO.Bordereau.SereServADO heinServiceType = heinServiceTypeGroup.First();
                            heinServiceType.HEIN_SERVICE_TYPE_NAME = heinServiceType.HEIN_SERVICE_TYPE_ID.HasValue ? heinServiceType.HEIN_SERVICE_TYPE_NAME : "Khác";

                            heinServiceType.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE = heinServiceTypeGroup
                                .Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                            heinServiceType.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE = heinServiceTypeGroup
                                             .Sum(o => o.TOTAL_PRICE_BHYT);
                            heinServiceType.TOTAL_PATIENT_PRICE_BHYT_HEIN_SERVICE_TYPE = heinServiceTypeGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT);

                            heinServiceTypes.Add(heinServiceType);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void ProcesTotalPrice()
        {
            try
            {
                #region TONG TIEN THEO GOI
                foreach (var ktcFee in ktcFeeGroups)
                {
                    List<ADO.Bordereau.SereServADO> sereServKTCTemps = sereServADOs.Where(o => o.IS_OUT_PARENT_FEE == ktcFee.IS_OUT_PARENT_FEE).ToList();

                    ktcFee.TOTAL_HEIN_PRICE_FEE_GROUP = sereServKTCTemps.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                    ktcFee.TOTAL_PRICE_BHYT_FEE_GROUP = sereServKTCTemps.Sum(o => o.TOTAL_PRICE_BHYT);
                    ktcFee.TOTAL_PATIENT_PRICE_BHYT_FEE_GROUP = sereServKTCTemps.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT);


                    if (sereServKTCTemps != null && sereServKTCTemps.Count > 0)
                    {
                        foreach (var execute in sereServExecuteGroups)
                        {
                            List<ADO.Bordereau.SereServADO> sereServExecuteTemps = sereServKTCTemps.Where(o => o.EXECUTE_GROUP_ID == execute.EXECUTE_GROUP_ID).ToList();
                            if (sereServExecuteTemps != null && sereServExecuteTemps.Count > 0)
                            {
                                execute.TOTAL_HEIN_PRICE_EXECUTE_GROUP = sereServExecuteTemps.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                                execute.TOTAL_PRICE_BHYT_EXECUTE_GROUP = sereServKTCTemps.Sum(o => o.TOTAL_PRICE_BHYT);
                                execute.TOTAL_PATIENT_PRICE_BHYT_EXECUTE_GROUP = sereServKTCTemps.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT);
                            }
                        }
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal override void SetSingleKey()
        {
            try
            {

                keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.TREATMENT_CODE, treatment.TREATMENT_CODE));
                keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.USERNAME_RETURN_RESULT, userNameReturnResult));
                keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.STATUS_TREATMENT_OUT, statusTreatmentOut));
                if (patyAlterBHYTADO != null)
                {
                    if (patyAlterBHYTADO.IS_HEIN != null)
                        keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.IS_HEIN, "X"));
                    else
                        keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.IS_NOT_HEIN, "X"));
                    if (patyAlterBHYTADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (patyAlterBHYTADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                        {
                            keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (patyAlterBHYTADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (patyAlterBHYTADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }

                    keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.HEIN_CARD_ADDRESS, patyAlterBHYTADO.ADDRESS));
                }
                else
                    keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.IS_NOT_HEIN, "X"));

                if (departmentTrans != null && departmentTrans.Count > 0)
                {
                    keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(departmentTrans[0].LOG_TIME)));
                    if (departmentTrans[departmentTrans.Count - 1] != null && departmentTrans.Count > 1)
                    {
                        keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.DEPARTMENT_NAME_CLOSE_TREATMENT, departmentTrans[departmentTrans.Count - 1].DEPARTMENT_NAME));
                    }


                    //Thời gian vào khoa
                    if (departmentId > 0)
                    {
                        List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTranTemps = new List<V_HIS_DEPARTMENT_TRAN>();
                        foreach (var departmentTran in departmentTrans)
                        {
                            if (departmentTran != null)
                                departmentTranTemps.Add(departmentTran);

                        }

                        var departmentIns = departmentTranTemps.Where(o => o.IN_OUT == 1 && o.DEPARTMENT_ID == departmentId).OrderByDescending(o => o.LOG_TIME).ToList();
                        if (departmentIns != null && departmentIns.Count > 0)
                        {
                            var timeDepartmentIn = departmentIns[0].LOG_TIME;
                            keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.TIME_DEPARTMENT_IN, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(timeDepartmentIn)));

                            var departmentOuts = departmentTrans.Where(o => o.IN_OUT == 2 && o.DEPARTMENT_ID == departmentId).OrderByDescending(o => o.LOG_TIME).ToList();
                            if (departmentOuts != null && departmentOuts.Count > 0 && departmentOuts.Count == departmentIns.Count)
                            {
                                var timeDepartmentOut = departmentOuts[0].LOG_TIME;

                                keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.TIME_DEPARTMENT_OUT, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(timeDepartmentOut)));
                            }
                        }
                    }
                }

                if (treatment != null)
                {
                    if (treatment.CLINICAL_IN_TIME.HasValue)
                    {
                        keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.CLINICAL_IN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(treatment.CLINICAL_IN_TIME.Value)));
                    }

                    if (treatment.OUT_TIME.HasValue)
                    {
                        keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(treatment.OUT_TIME.Value)));
                    }
                }

                keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.TOTAL_DAY, today));

                if (tranPati != null)
                {
                    keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.TRAN_PATI_MEDI_ORG_CODE, tranPati.MEDI_ORG_CODE));
                    keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.TRAN_PATI_MEDI_ORG_NAME, tranPati.MEDI_ORG_NAME));
                }

                keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.CURRENT_DATE_SEPARATE_FULL_STR, currentDateSeparateFullTime));

                if (!String.IsNullOrEmpty(departmentName))
                {
                    keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.DEPARTMENT_NAME, departmentName));
                }

                decimal bhytthanhtoan_tong = 0;
                decimal nguonkhac_tong = 0;

                decimal thanhtien_tong_bhyt = 0;
                decimal bnthanhtoan_tong_bhyt = 0;

                if (sereServADOs != null && sereServADOs.Count > 0)
                {
                    bhytthanhtoan_tong = sereServADOs.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    nguonkhac_tong = 0;

                    thanhtien_tong_bhyt = sereServADOs.Sum(o => o.TOTAL_PRICE_BHYT);
                    bnthanhtoan_tong_bhyt = sereServADOs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0);
                }

                keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.TOTAL_PRICE_BHYT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong_bhyt, 0)));
                keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.TOTAL_PRICE_PATIENT_BHYT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong_bhyt, 0)));
                keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.TOTAL_PRICE_OTHER, Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguonkhac_tong, 0)));
                keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.TOTAL_PRICE_BHYT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong_bhyt).ToString())));
                keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.TOTAL_PRICE_PATIENT_BHYT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong_bhyt).ToString())));
                keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.TOTAL_PRICE_OTHER_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(nguonkhac_tong).ToString())));




                if (treatmentFees != null)
                {
                    keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0, 0)));
                }
                else
                {
                    keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, "0"));
                }

                keyValues.Add(new KeyValue(Mps000193ExtendSingleKey.CREATOR_USERNAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));

                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(patientADO, keyValues);
                GlobalQuery.AddObjectKeyIntoListkey<PatyAlterBhytADO>(patyAlterBHYTADO, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TREATMENT>(treatment, keyValues, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
