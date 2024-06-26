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

using MPS;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ADO;
using MOS.SDO;

namespace MPS.Core.Mps000102
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000102RDO : RDOBase
    {
        internal PatientADO patientADO { get; set; }
        internal PatyAlterBhytADO patyAlterBhytADO { get; set; }
        internal string departmentName;
        internal List<SereServGroupPlusADO> sereServNotHiTechs { get; set; }
        internal List<SereServGroupPlusADO> sereServHitechs { get; set; }
        internal List<SereServGroupPlusADO> sereServHitechADOs { get; set; }
        internal List<SereServGroupPlusADO> sereServVTTTs { get; set; }
        internal List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans;
        internal V_HIS_TREATMENT currentHisTreatment;

        internal V_HIS_SERVICE_REQ hisServiceReq;

        internal List<HIS_HEIN_SERVICE_TYPE> ServiceReports { get; set; }

        internal List<SereServGroupPlusADO> ServiceGroups { get; set; }
        internal List<SereServGroupPlusADO> DepartmentGroups { get; set; }
        internal List<SereServGroupPlusADO> HightTechDepartmentGroups { get; set; }
        internal List<SereServGroupPlusADO> ServiceVTTTDepartmentGroup { get; set; }
        internal List<V_HIS_DERE_DETAIL> dereDetails { get; set; }

        MOS.EFMODEL.DataModels.V_HIS_TRAN_PATI hisTranPati { get; set; }
        MOS.EFMODEL.DataModels.V_HIS_DEPOSIT deposit { get; set; }
        long totalDay { get; set; }
        string ratio { get; set; }
        decimal thuChenhLech, thuDongChiTra;
        public Mps000102RDO(
            PatientADO patientADO,
            PatyAlterBhytADO patyAlterBhytADO,
            string departmentName,

            List<SereServGroupPlusADO> sereServNotHiTechs,
            List<SereServGroupPlusADO> sereServHitechs,
            List<SereServGroupPlusADO> sereServVTTTs,

            List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans,
            V_HIS_TREATMENT currentHisTreatment,

            List<HIS_HEIN_SERVICE_TYPE> ServiceReports,

            MOS.EFMODEL.DataModels.V_HIS_TRAN_PATI hisTranPati,
            MOS.EFMODEL.DataModels.V_HIS_DEPOSIT HisDeposit,

            List<MOS.EFMODEL.DataModels.V_HIS_DERE_DETAIL> _dereDetails,
            long totalDay,
            string ratio,
            V_HIS_SERVICE_REQ hisServiceReq
            )
        {
            try
            {
                this.patientADO = patientADO;
                this.patyAlterBhytADO = patyAlterBhytADO;
                this.departmentName = departmentName;
                this.sereServNotHiTechs = sereServNotHiTechs;
                this.sereServHitechs = sereServHitechs;
                this.sereServVTTTs = sereServVTTTs;
                this.departmentTrans = departmentTrans;
                this.currentHisTreatment = currentHisTreatment;
                this.ServiceReports = ServiceReports;
                this.hisTranPati = hisTranPati;
                this.totalDay = totalDay;
                this.deposit = HisDeposit;
                this.dereDetails = _dereDetails;
                this.ratio = ratio;
                this.hisServiceReq = hisServiceReq;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //- Thu chênh lệch = (VIR_PRICE - (HEIN_LIMIT_PRICE ?? 0)) * AMOUNT
        //- Thu đồng chi trả = DEPOSIT_AMOUNT - thu chênh lệnh
        internal void processDereDetail()
        {
            try
            {
                thuChenhLech = 0;
                thuDongChiTra = 0;
                foreach (var item in this.dereDetails)
                {
                    thuChenhLech += ((item.VIR_PRICE ?? 0) - (item.HEIN_LIMIT_PRICE ?? (item.VIR_PRICE ?? 0))) * item.AMOUNT;
                    thuDongChiTra += item.DEPOSIT_AMOUNT - (((item.VIR_PRICE ?? 0) - (item.HEIN_LIMIT_PRICE ?? (item.VIR_PRICE ?? 0))) * item.AMOUNT);
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
                if (departmentTrans != null && departmentTrans.Count > 0)
                {
                    keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(departmentTrans[0].LOG_TIME)));
                    if (departmentTrans[departmentTrans.Count - 1] != null && departmentTrans.Count > 1)
                    {
                        keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(departmentTrans[departmentTrans.Count - 1].LOG_TIME)));
                        //Số ngày điều trị
                        keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.TOTAL_DAY, totalDay));
                    }
                }

                if (this.hisServiceReq != null)
                {
                    keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.FIRST_EXAM_ROOM_NAME, this.hisServiceReq.EXECUTE_ROOM_NAME));
                }

                keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.RATIO, this.ratio));

                if (departmentName != null)
                {
                    keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.DEPARTMENT_NAME, departmentName));
                }

                if (patyAlterBhytADO != null)
                {
                    if (patyAlterBhytADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                        {
                            keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (patyAlterBhytADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }

                    if (patyAlterBhytADO.IS_HEIN != null)
                        keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.IS_HEIN, "X"));
                    else
                        keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.IS_NOT_HEIN, "X"));
                }


                if (hisTranPati != null)
                    keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.TRAN_PATI_MEDI_ORG_CODE, hisTranPati.MEDI_ORG_CODE));

                //keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.CURRENT_DATE_SEPARATE_FULL_STR, currentDateSeparateFullTime));


                decimal thanhtien_tong = 0;
                decimal bhytthanhtoan_tong = 0;
                decimal nguonkhac_tong = 0;
                decimal bnthanhtoan_tong = 0;

                decimal thanhtien_tong_vttt = 0;
                decimal bhytthanhtoan_tong_vttt = 0;
                decimal nguonkhac_tong_vttt = 0;
                decimal bnthanhtoan_tong_vttt = 0;

                decimal thanhtien_tong_dvc = 0;
                decimal bhytthanhtoan_tong_dvc = 0;
                decimal nguonkhac_tong_dvc = 0;
                decimal bnthanhtoan_tong_dvc = 0;

                if (sereServNotHiTechs != null && sereServNotHiTechs.Count > 0)
                {
                    if (sereServVTTTs != null && sereServVTTTs.Count > 0)
                    {
                        thanhtien_tong_vttt = sereServVTTTs.Sum(o => (o.VIR_TOTAL_PRICE)) ?? 0;
                        bhytthanhtoan_tong_vttt = sereServVTTTs.Sum(o => (o.VIR_TOTAL_HEIN_PRICE)) ?? 0;
                        nguonkhac_tong_vttt = 0;
                        bnthanhtoan_tong_vttt = sereServVTTTs.Sum(o => (o.VIR_TOTAL_PATIENT_PRICE)) ?? 0;
                    }
                    if (sereServHitechs != null && sereServHitechs.Count > 0)
                    {
                        thanhtien_tong_dvc = sereServHitechs.Sum(o => (o.VIR_TOTAL_PRICE)) ?? 0;
                        bhytthanhtoan_tong_dvc = sereServHitechs.Sum(o => (o.VIR_TOTAL_HEIN_PRICE)) ?? 0;
                        nguonkhac_tong_dvc = 0;
                        bnthanhtoan_tong_dvc = sereServHitechs.Sum(o => (o.VIR_TOTAL_PATIENT_PRICE)) ?? 0;
                    }

                    thanhtien_tong = (thanhtien_tong_dvc + thanhtien_tong_vttt + sereServNotHiTechs.Sum(o => (o.VIR_TOTAL_PRICE))) ?? 0;
                    bhytthanhtoan_tong = (bhytthanhtoan_tong_dvc + bhytthanhtoan_tong_vttt + sereServNotHiTechs.Sum(o => (o.VIR_TOTAL_HEIN_PRICE))) ?? 0;
                    bnthanhtoan_tong = (bnthanhtoan_tong_dvc + bnthanhtoan_tong_vttt + sereServNotHiTechs.Sum(o => (o.VIR_TOTAL_PATIENT_PRICE))) ?? 0;
                    nguonkhac_tong = 0;

                }

                keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.VIR_TOTAL_PRICE_HEIN_LIMIT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(thuChenhLech, 0)));
                keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.VIR_DEPOSIT_AMOUNT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(thuDongChiTra, 0)));

                keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong, 0)));
                keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong, 0)));
                keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.TOTAL_PRICE_OTHER, Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguonkhac_tong, 0)));
                keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong).ToString())));
                keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.TOTAL_PRICE_PATIENT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong).ToString())));
                keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.TOTAL_PRICE_OTHER_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(nguonkhac_tong).ToString())));

                keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.VIR_TOTAL_PRICE_HEIN_LIMIT_TO_VN, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thuChenhLech).ToString())));
                keyValues.Add(new KeyValue(Mps000102ExtendSingleKey.VIR_DEPOSIT_AMOUNT_TO_VN, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thuDongChiTra).ToString())));

                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_DEPOSIT>(this.deposit, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<PatyAlterBhytADO>(patyAlterBhytADO, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TREATMENT>(currentHisTreatment, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(patientADO, keyValues);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(hisServiceReq, keyValues, false);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void ProcessTotalPriceGroup()
        {
            try
            {
                decimal totalPriceHightTech = 0;
                decimal totalPriceNotHightTech = 0;
                decimal totalPriceVTTT = 0;

                foreach (var ServiceGroup in ServiceGroups)
                {
                    totalPriceHightTech = sereServHitechs.Where(o => o.HEIN_SERVICE_TYPE_ID == ServiceGroup.HEIN_SERVICE_TYPE_ID).Sum(o => o.VIR_TOTAL_PRICE) ?? 0;
                    totalPriceVTTT = sereServVTTTs.Where(o => o.SERE_SERV__GROUP_SERVICE_REPORT == ServiceGroup.HEIN_SERVICE_TYPE_ID).Sum(o => o.VIR_TOTAL_PRICE) ?? 0;
                    totalPriceNotHightTech = sereServNotHiTechs.Where(o => o.HEIN_SERVICE_TYPE_ID == ServiceGroup.HEIN_SERVICE_TYPE_ID).Sum(o => o.VIR_TOTAL_PRICE) ?? 0;


                    ServiceGroup.TOTAL_PRICE_SERVICE_GROUP = totalPriceHightTech + totalPriceVTTT + totalPriceNotHightTech;

                    totalPriceHightTech = sereServHitechs.Where(o => o.HEIN_SERVICE_TYPE_ID == ServiceGroup.HEIN_SERVICE_TYPE_ID).Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    totalPriceVTTT = sereServVTTTs.Where(o => o.SERE_SERV__GROUP_SERVICE_REPORT == ServiceGroup.HEIN_SERVICE_TYPE_ID).Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    totalPriceNotHightTech = sereServNotHiTechs.Where(o => o.HEIN_SERVICE_TYPE_ID == ServiceGroup.HEIN_SERVICE_TYPE_ID).Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;

                    ServiceGroup.TOTAL_HEIN_PRICE_SERVICE_GROUP = totalPriceHightTech + totalPriceVTTT + totalPriceNotHightTech;

                    totalPriceHightTech = sereServHitechs.Where(o => o.HEIN_SERVICE_TYPE_ID == ServiceGroup.HEIN_SERVICE_TYPE_ID).Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
                    totalPriceVTTT = sereServVTTTs.Where(o => o.SERE_SERV__GROUP_SERVICE_REPORT == ServiceGroup.HEIN_SERVICE_TYPE_ID).Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
                    totalPriceNotHightTech = sereServNotHiTechs.Where(o => o.HEIN_SERVICE_TYPE_ID == ServiceGroup.HEIN_SERVICE_TYPE_ID).Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;

                    ServiceGroup.TOTAL_PATIENT_PRICE_SERVICE_GROUP = totalPriceHightTech + totalPriceVTTT + totalPriceNotHightTech;
                }

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
                DepartmentGroups = new List<SereServGroupPlusADO>();
                HightTechDepartmentGroups = new List<SereServGroupPlusADO>();
                ServiceVTTTDepartmentGroup = new List<SereServGroupPlusADO>();

                //Group Service Not Hitech
                var sServceReportGroups = sereServNotHiTechs.GroupBy(o => o.HEIN_SERVICE_TYPE_ID).ToList();
                foreach (var sServceReportGroup in sServceReportGroups)
                {
                    List<SereServGroupPlusADO> subSServiceReportGroup = sServceReportGroup.ToList<SereServGroupPlusADO>();

                    SereServGroupPlusADO itemSServiceReportGroup = subSServiceReportGroup.First();
                    if (itemSServiceReportGroup.HEIN_SERVICE_TYPE_ID != null)
                        itemSServiceReportGroup.HEIN_SERVICE_TYPE_NAME = ServiceReports.Where(o => o.ID == subSServiceReportGroup.First().HEIN_SERVICE_TYPE_ID).SingleOrDefault().HEIN_SERVICE_TYPE_NAME;
                    else
                        itemSServiceReportGroup.HEIN_SERVICE_TYPE_NAME = "Khác";

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

                //Group ServiceVTTT Department
                var ServiceVTTTDepartmentGroups = sereServVTTTs.OrderBy(o => o.REQUEST_DEPARTMENT_ID).GroupBy(o => o.REQUEST_DEPARTMENT_ID).ToList();
                foreach (var ServiceVTTTDepartment in ServiceVTTTDepartmentGroups)
                {
                    List<SereServGroupPlusADO> subServiceVTTTDepartmentGroups = ServiceVTTTDepartment.ToList<SereServGroupPlusADO>();
                    SereServGroupPlusADO itemServiceVTTTDepartmentGroup = subServiceVTTTDepartmentGroups.First();
                    itemServiceVTTTDepartmentGroup.DEPARTMENT__GROUP_SERVICE_REPORT = sereServHitechs.First().HEIN_SERVICE_TYPE_ID ?? 0;

                    itemServiceVTTTDepartmentGroup.TOTAL_PRICE_DEPARTMENT_GROUP = sereServVTTTs.Where(o => o.REQUEST_DEPARTMENT_ID == itemServiceVTTTDepartmentGroup.REQUEST_DEPARTMENT_ID).Sum(o => o.VIR_TOTAL_PRICE) ?? 0;
                    itemServiceVTTTDepartmentGroup.TOTAL_PATIENT_PRICE_DEPARTMENT_GROUP = sereServVTTTs.Where(o => o.REQUEST_DEPARTMENT_ID == itemServiceVTTTDepartmentGroup.REQUEST_DEPARTMENT_ID).Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
                    itemServiceVTTTDepartmentGroup.TOTAL_HEIN_PRICE_DEPARTMENT_GROUP = sereServVTTTs.Where(o => o.REQUEST_DEPARTMENT_ID == itemServiceVTTTDepartmentGroup.REQUEST_DEPARTMENT_ID).Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;

                    foreach (var sereServVTTT in sereServVTTTs)
                    {
                        sereServVTTT.SERE_SERV__GROUP_SERVICE_REPORT = sereServHitechs.First().HEIN_SERVICE_TYPE_ID ?? 0;
                    }

                    ServiceVTTTDepartmentGroup.Add(itemServiceVTTTDepartmentGroup);

                }

                //Group HightTech Department
                var sHightTechDepartmentGroups = sereServHitechs.OrderBy(o => o.EXECUTE_DEPARTMENT_ID).GroupBy(o => o.EXECUTE_DEPARTMENT_ID).ToList();
                foreach (var sHightTechDepartmentGroup in sHightTechDepartmentGroups)
                {
                    List<SereServGroupPlusADO> subHightTechDepartmentGroups = sHightTechDepartmentGroup.ToList<SereServGroupPlusADO>();
                    SereServGroupPlusADO itemHightTechDepartmentGroup = subHightTechDepartmentGroups.First();
                    HightTechDepartmentGroups.Add(itemHightTechDepartmentGroup);

                    itemHightTechDepartmentGroup.TOTAL_PRICE_DEPARTMENT_GROUP = sHightTechDepartmentGroup.Where(o => o.EXECUTE_DEPARTMENT_ID == itemHightTechDepartmentGroup.EXECUTE_DEPARTMENT_ID).Sum(o => o.VIR_TOTAL_PRICE) ?? 0;
                    itemHightTechDepartmentGroup.TOTAL_PATIENT_PRICE_DEPARTMENT_GROUP = sHightTechDepartmentGroup.Where(o => o.EXECUTE_DEPARTMENT_ID == itemHightTechDepartmentGroup.EXECUTE_DEPARTMENT_ID).Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
                    itemHightTechDepartmentGroup.TOTAL_HEIN_PRICE_DEPARTMENT_GROUP = sHightTechDepartmentGroup.Where(o => o.EXECUTE_DEPARTMENT_ID == itemHightTechDepartmentGroup.EXECUTE_DEPARTMENT_ID).Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;


                    var checkExitServiceGroup = ServiceGroups.Where(o => o.EXECUTE_DEPARTMENT_ID == itemHightTechDepartmentGroup.EXECUTE_DEPARTMENT_ID).ToList();
                    if (checkExitServiceGroup.Count == 0)
                    {
                        itemHightTechDepartmentGroup.HEIN_SERVICE_TYPE_NAME = ServiceReports.Where(o => o.ID == subHightTechDepartmentGroups.First().HEIN_SERVICE_TYPE_ID).SingleOrDefault().HEIN_SERVICE_TYPE_NAME;
                        ServiceGroups.Add(itemHightTechDepartmentGroup);
                    }
                }

                ////Sắp xếp lại nhóm Service Group
                //ServiceGroups = ServiceGroups.OrderBy(o => o.HEIN_SERVICE_TYPE_NUM_ORDER ?? 9999).ToList();

            }

            //}
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


    }
}
