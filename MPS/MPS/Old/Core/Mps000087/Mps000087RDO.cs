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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Core.Mps000087
{
    /// <summary>
    /// In bang ke thanh toan ra vien ngoai tru va noi tru.
    /// </summary>
    public class Mps000087RDO : RDOBase
    {
        internal PatyAlterBhytADO PatyAlterBhyt { get; set; }
        internal List<SereServGroupPlusADO> sereServADOs { get; set; }
        internal string DepartmentName { get; set; }
        internal V_HIS_PATIENT Patient { get; set; }
        internal V_HIS_PATIENT_TYPE_ALTER PatientTypeAlter { get; set; }
        internal V_HIS_TREATMENT currentTreatment { get; set; }
        internal List<HIS_HEIN_SERVICE_TYPE> serviceReports { get; set; }
        internal string ratio;
        internal List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans { get; set; }
        MOS.EFMODEL.DataModels.V_HIS_TRAN_PATI hisTranPati { get; set; }
        List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE> treatmentFees { get; set; }
        string currentDateSeparateFullTime = "";
        string departmentName;
        internal List<SereServGroupPlusADO> ServiceGroups { get; set; }
        internal long totalDay { get; set; }


        public Mps000087RDO(
            V_HIS_PATIENT patient,
            PatyAlterBhytADO patyAlterBhyt,
            V_HIS_PATIENT_TYPE_ALTER PatientTypeAlter,
            List<SereServGroupPlusADO> sereServADOs,
            V_HIS_TREATMENT treatment,
            List<HIS_HEIN_SERVICE_TYPE> serviceReports,
            string ratio,
            List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans, MOS.EFMODEL.DataModels.V_HIS_TRAN_PATI hisTranPati,
            List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE> treatmentFees,
            string currentDateSeparateFullTime,
            string departmentName,
            long totalDay
            )
        {
            try
            {
                this.Patient = patient;
                this.PatyAlterBhyt = patyAlterBhyt;
                this.PatientTypeAlter = PatientTypeAlter;
                this.sereServADOs = sereServADOs;
                this.currentTreatment = treatment;
                this.serviceReports = serviceReports;
                this.departmentTrans = departmentTrans;
                this.hisTranPati = hisTranPati;
                this.treatmentFees = treatmentFees;
                this.ratio = ratio;
                this.currentDateSeparateFullTime = currentDateSeparateFullTime;
                this.departmentName = departmentName;
                this.totalDay = totalDay;
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
                //GlobalQuery.AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(ServiceReq, keyValues);
                keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.RATIO, ratio));
                keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.TREATMENT_CODE, currentTreatment.TREATMENT_CODE));

                if (PatyAlterBhyt != null)
                {
                    if (PatyAlterBhyt.IS_HEIN != null)
                        keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.IS_HEIN, "X"));
                    else
                        keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.IS_NOT_HEIN, "X"));
                    if (PatyAlterBhyt.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (PatyAlterBhyt.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                        {
                            keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (PatyAlterBhyt.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (PatyAlterBhyt.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }
                }
                else
                    keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.IS_NOT_HEIN, "X"));

                //keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.ICD_MAIN_CODE, currentTreatment.ICD_CODE));
                //keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.ICD_MAIN_TEXT, currentTreatment.ICD_NAME));

                if (departmentTrans != null && departmentTrans.Count > 0)
                {
                    keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(departmentTrans[0].LOG_TIME)));
                    if (departmentTrans[departmentTrans.Count - 1] != null && departmentTrans.Count > 1)
                    {
                        keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(departmentTrans[departmentTrans.Count - 1].LOG_TIME)));
                        //Số ngày điều trị
                        keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.TOTAL_DAY, totalDay));
                       
                    }

                    //Thời gian vào khoa
                    //Thời gian vào khoa
                    List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTranTemps = new List<V_HIS_DEPARTMENT_TRAN>();
                    foreach (var departmentTran in departmentTrans)
                    {
                        if (departmentTran != null)
                            departmentTranTemps.Add(departmentTran);

                    }

                    var departmentIns = departmentTranTemps.Where(o => o.IN_OUT == 1).OrderByDescending(o => o.LOG_TIME).ToList();
                    if (departmentIns != null && departmentIns.Count > 0)
                    {
                        var timeDepartmentIn = departmentIns[0].LOG_TIME;
                        keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.TIME_DEPARTMENT_IN, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(timeDepartmentIn)));
                    }

                    if (departmentTranTemps.Count == departmentTrans.Count)
                    {
                        var departmentOuts = departmentTrans.Where(o => o.IN_OUT == 2).OrderByDescending(o => o.LOG_TIME).ToList();
                        if (departmentOuts != null && departmentOuts.Count > 0)
                        {
                            var timeDepartmentOut = departmentOuts[0].LOG_TIME;

                            keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.TIME_DEPARTMENT_OUT, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(timeDepartmentOut)));
                        }
                    }
                }



                if (hisTranPati != null)
                {
                    keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.TRAN_PATI_MEDI_ORG_CODE, hisTranPati.MEDI_ORG_CODE));
                    keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.TRAN_PATI_MEDI_ORG_NAME, hisTranPati.MEDI_ORG_NAME));
                }

                keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.CURRENT_DATE_SEPARATE_FULL_STR, currentDateSeparateFullTime));


                decimal thanhtien_tong = 0;
                decimal bhytthanhtoan_tong = 0;
                decimal nguonkhac_tong = 0;
                decimal bnthanhtoan_tong = 0;

                if (sereServADOs != null && sereServADOs.Count > 0)
                {
                    thanhtien_tong = sereServADOs.Sum(o => o.VIR_TOTAL_PRICE_NO_ADD_PRICE) ?? 0;
                    bhytthanhtoan_tong = sereServADOs.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    bnthanhtoan_tong = sereServADOs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
                    nguonkhac_tong = 0;
                }

                keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong, 0)));
                keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong, 0)));
                keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.TOTAL_PRICE_OTHER, Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguonkhac_tong, 0)));
                keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong).ToString())));
                keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.TOTAL_PRICE_PATIENT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong).ToString())));
                keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.TOTAL_PRICE_OTHER_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(nguonkhac_tong).ToString())));

                if (treatmentFees != null)
                {
                    keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0, 0)));
                }
                else
                {
                    keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, "0"));
                }

                keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.CREATOR_USERNAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));
                keyValues.Add(new KeyValue(Mps000087ExtendSingleKey.DEPARTMENT_NAME,departmentName.ToUpper()));
                

                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_PATIENT>(Patient, keyValues);
                GlobalQuery.AddObjectKeyIntoListkey<PatyAlterBhytADO>(PatyAlterBhyt, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TREATMENT>(currentTreatment, keyValues, false);
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
                ServiceGroups = new List<SereServGroupPlusADO>();
                var sServceReportGroups = sereServADOs.OrderBy(o=>o.HEIN_SERVICE_TYPE_NUM_ORDER).GroupBy(o => o.HEIN_SERVICE_TYPE_ID).ToList();
                foreach (var sServceReportGroup in sServceReportGroups)
                {
                    List<SereServGroupPlusADO> subSServiceReportGroup = sServceReportGroup.ToList<SereServGroupPlusADO>();
                    SereServGroupPlusADO itemSServiceReportGroup = subSServiceReportGroup.First();
                    if (itemSServiceReportGroup.HEIN_SERVICE_TYPE_ID != null)
                        itemSServiceReportGroup.HEIN_SERVICE_TYPE_NAME = serviceReports.Where(o => o.ID == subSServiceReportGroup.First().HEIN_SERVICE_TYPE_ID).SingleOrDefault().HEIN_SERVICE_TYPE_NAME;
                    else
                        itemSServiceReportGroup.HEIN_SERVICE_TYPE_NAME = "Khác";
                    itemSServiceReportGroup.TOTAL_PRICE_SERVICE_GROUP = subSServiceReportGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_ADD_PRICE) ?? 0;
                    itemSServiceReportGroup.TOTAL_HEIN_PRICE_SERVICE_GROUP = subSServiceReportGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    itemSServiceReportGroup.TOTAL_PATIENT_PRICE_SERVICE_GROUP = subSServiceReportGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
                    ServiceGroups.Add(itemSServiceReportGroup);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
