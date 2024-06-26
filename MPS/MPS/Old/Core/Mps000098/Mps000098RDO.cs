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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Core.Mps000098
{
    public class Mps000098RDO : RDOBase
    {
        internal PatientADO patientADO { get; set; }
        internal PatyAlterBhytADO patyAlterBhytADO { get; set; }
        internal string departmentName;
        internal List<SereServGroupPlusADO> sereServInKTCs { get; set; }
        internal List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans;
        internal V_HIS_TREATMENT currentHisTreatment;
        internal SereServGroupPlusADO sereServKTC;
        internal List<HIS_HEIN_SERVICE_TYPE> ServiceReports { get; set; }
        internal List<SereServGroupPlusADO> DepartmentGroups { get; set; }
        internal List<SereServGroupPlusADO> ServiceGroups { get; set; }
        long totalDay { get; set; }

        MOS.EFMODEL.DataModels.V_HIS_TRAN_PATI hisTranPati { get; set; }
        List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE> treatmentFees { get; set; }
        public Mps000098RDO(
            PatientADO patientADO,
            PatyAlterBhytADO patyAlterBhytADO,
            string departmentName,
            SereServGroupPlusADO sereServKTC,
            List<SereServGroupPlusADO> sereServInKTCs,

            List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans,
            V_HIS_TREATMENT currentHisTreatment,
            List<HIS_HEIN_SERVICE_TYPE> ServiceReports,
            MOS.EFMODEL.DataModels.V_HIS_TRAN_PATI hisTranPati,
            List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE> treatmentFees,
            long totalDay

            )
        {
            try
            {
                this.sereServKTC = sereServKTC;
                this.patientADO = patientADO;
                this.patyAlterBhytADO = patyAlterBhytADO;
                this.departmentName = departmentName;
                this.sereServInKTCs = sereServInKTCs;
                this.departmentTrans = departmentTrans;
                this.currentHisTreatment = currentHisTreatment;
                this.ServiceReports = ServiceReports;
                this.hisTranPati = hisTranPati;
                this.treatmentFees = treatmentFees;
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
                if (departmentTrans != null && departmentTrans.Count > 0)
                {
                    keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(departmentTrans[0].LOG_TIME)));
                    if (departmentTrans[departmentTrans.Count - 1] != null && departmentTrans.Count > 1)
                    {
                        keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(departmentTrans[departmentTrans.Count - 1].LOG_TIME)));
                        keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.TOTAL_DAY, totalDay));
                    }
                }

                if (departmentName != null)
                {
                    keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.DEPARTMENT_NAME, departmentName));
                }


                if (patyAlterBhytADO != null)
                {
                    if (patyAlterBhytADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                        {
                            keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (patyAlterBhytADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }

                    if (patyAlterBhytADO.IS_HEIN != null)
                        keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.IS_HEIN, "X"));
                    else
                        keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.IS_NOT_HEIN, "X"));
                }


                if (hisTranPati != null)
                {
                    keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.TRAN_PATI_MEDI_ORG_CODE, hisTranPati.MEDI_ORG_CODE));
                    keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.TRAN_PATI_MEDI_ORG_NAME, hisTranPati.MEDI_ORG_NAME));
                }

                decimal thanhtien_tong = 0;
                decimal bhytthanhtoan_tong = 0;
                decimal nguonkhac_tong = 0;
                decimal bnthanhtoan_tong = 0;

                if (sereServInKTCs != null && sereServInKTCs.Count > 0)
                {
                    thanhtien_tong = sereServInKTCs.Sum(o => o.VIR_TOTAL_PRICE_NO_ADD_PRICE) ?? 0;
                    bhytthanhtoan_tong = sereServInKTCs.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    bnthanhtoan_tong = sereServInKTCs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
                    nguonkhac_tong = 0;
                }

                keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong, 0)));
                keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong, 0)));
                keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.TOTAL_PRICE_OTHER, Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguonkhac_tong, 0)));
                keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong).ToString())));
                keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.TOTAL_PRICE_PATIENT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong).ToString())));
                keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.TOTAL_PRICE_OTHER_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(nguonkhac_tong).ToString())));

                if (treatmentFees != null)
                {
                    keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0, 0)));
                }
                else
                {
                    keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, "0"));
                }

                //Tên dịch vụ kỹ thuật cao
                if (sereServKTC != null)
                {
                    keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.SERVICE_HIGHTECH, sereServKTC.SERVICE_NAME));
                    keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.PRICE_POLICY, Inventec.Common.Number.Convert.NumberToStringRoundAuto(sereServKTC.PRICE_POLICY,0)));
                    keyValues.Add(new KeyValue(Mps000098ExtendSingleKey.PRICE_BHYT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(sereServKTC.PRICE_BHYT, 0)));
                }

                GlobalQuery.AddObjectKeyIntoListkey<PatyAlterBhytADO>(patyAlterBhytADO, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TREATMENT>(currentHisTreatment, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(patientADO, keyValues);
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
                sereServInKTCs = sereServInKTCs.Where(o => o.IS_NO_EXECUTE == null).ToList();

                ServiceGroups = new List<SereServGroupPlusADO>();
                DepartmentGroups = new List<SereServGroupPlusADO>();
                var sServceReportGroups = sereServInKTCs.GroupBy(o => o.HEIN_SERVICE_TYPE_ID).ToList();
                foreach (var sServceReportGroup in sServceReportGroups)
                {
                    List<SereServGroupPlusADO> subSServiceReportGroup = sServceReportGroup.ToList<SereServGroupPlusADO>();
                    SereServGroupPlusADO itemSServiceReportGroup = subSServiceReportGroup.First();
                    if (itemSServiceReportGroup.HEIN_SERVICE_TYPE_ID != null)
                        itemSServiceReportGroup.HEIN_SERVICE_TYPE_NAME = ServiceReports.Where(o => o.ID == subSServiceReportGroup.First().HEIN_SERVICE_TYPE_ID).SingleOrDefault().HEIN_SERVICE_TYPE_NAME;
                    else
                        itemSServiceReportGroup.HEIN_SERVICE_TYPE_NAME = "Khác";

                    itemSServiceReportGroup.TOTAL_PRICE_SERVICE_GROUP = subSServiceReportGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_ADD_PRICE) ?? 0;
                    itemSServiceReportGroup.TOTAL_HEIN_PRICE_SERVICE_GROUP = subSServiceReportGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    itemSServiceReportGroup.TOTAL_PATIENT_PRICE_SERVICE_GROUP = subSServiceReportGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
                    ServiceGroups.Add(itemSServiceReportGroup);


                    //Nhom Department
                    var sDepartmentGroups = subSServiceReportGroup.OrderBy(o => o.REQUEST_DEPARTMENT_ID).GroupBy(o => o.REQUEST_DEPARTMENT_ID).ToList();
                    foreach (var sDepartmentGroup in sDepartmentGroups)
                    {
                        List<SereServGroupPlusADO> subSDepartmentGroups = sDepartmentGroup.ToList<SereServGroupPlusADO>();
                        SereServGroupPlusADO itemSDepartmentGroup = subSDepartmentGroups.First();

                        itemSDepartmentGroup.TOTAL_PRICE_DEPARTMENT_GROUP = sDepartmentGroup.Where(o => o.REQUEST_DEPARTMENT_ID == itemSDepartmentGroup.REQUEST_DEPARTMENT_ID).Sum(o => o.VIR_TOTAL_PRICE) ?? 0;
                        itemSDepartmentGroup.TOTAL_PATIENT_PRICE_DEPARTMENT_GROUP = sDepartmentGroup.Where(o => o.REQUEST_DEPARTMENT_ID == itemSDepartmentGroup.REQUEST_DEPARTMENT_ID).Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
                        itemSDepartmentGroup.TOTAL_HEIN_PRICE_DEPARTMENT_GROUP = sDepartmentGroup.Where(o => o.REQUEST_DEPARTMENT_ID == itemSDepartmentGroup.REQUEST_DEPARTMENT_ID).Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                        DepartmentGroups.Add(itemSDepartmentGroup);

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
