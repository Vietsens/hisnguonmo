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

namespace MPS.Core.Mps000082
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000082RDO : RDOBase
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

        internal List<HIS_HEIN_SERVICE_TYPE> ServiceReports { get; set; }

        internal List<SereServGroupPlusADO> ServiceGroups { get; set; }
        internal List<SereServGroupPlusADO> DepartmentGroups { get; set; }
        internal List<SereServGroupPlusADO> HightTechDepartmentGroups { get; set; }
        internal List<SereServGroupPlusADO> ServiceVTTTDepartmentGroup { get; set; }

        MOS.EFMODEL.DataModels.V_HIS_TRAN_PATI hisTranPati { get; set; }
        List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE> treatmentFees { get; set; }
        List<V_HIS_DERE_DETAIL> dereDetails { get; set; }

        long totalDay { get; set; }
        public Mps000082RDO(
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
            List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE> treatmentFees,

            long totalDay,
            List<V_HIS_DERE_DETAIL> dereDetails
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
                this.treatmentFees = treatmentFees;
                this.totalDay = totalDay;
                this.dereDetails = dereDetails;

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
                    keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(departmentTrans[0].LOG_TIME)));
                    if (departmentTrans[departmentTrans.Count - 1] != null && departmentTrans.Count > 1)
                    {
                        keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(departmentTrans[departmentTrans.Count - 1].LOG_TIME)));
                        //Số ngày điều trị
                        keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.TOTAL_DAY, totalDay));
                    }
                }

                if (departmentName != null)
                {
                    keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.DEPARTMENT_NAME, departmentName));
                }

                if (patyAlterBhytADO != null)
                {
                    if (patyAlterBhytADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                        {
                            keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (patyAlterBhytADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }

                    if (patyAlterBhytADO.IS_HEIN != null)
                        keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.IS_HEIN, "X"));
                    else
                        keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.IS_NOT_HEIN, "X"));
                }


                if (hisTranPati != null)
                {
                    keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.TRAN_PATI_MEDI_ORG_CODE, hisTranPati.MEDI_ORG_CODE));
                    keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.TRAN_PATI_MEDI_ORG_NAME, hisTranPati.MEDI_ORG_NAME));
                }

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



                keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong, 0)));
                keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong, 0)));
                keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.TOTAL_PRICE_OTHER, Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguonkhac_tong, 0)));
                keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong).ToString())));
                keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.TOTAL_PRICE_PATIENT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong).ToString())));
                keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.TOTAL_PRICE_OTHER_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(nguonkhac_tong).ToString())));

                if (treatmentFees != null)
                {
                    keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0, 0)));
                }
                else
                {
                    keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, "0"));
                }

                //
                decimal tongChiPhiBHYT = 0;
                decimal tienBHYTThanhToan = 0;
                decimal nguoiBenhCungChiTra = 0;
                decimal ngoaiDMChenhLech = 0;
                decimal tongThuBenhNhan = 0;
                decimal bnDaThanhToan = 0;
                decimal tamUng = 0;
                decimal hoanUng = 0;
                decimal bnTamUng = 0;
                decimal bnHoanUng = 0;
                decimal cacQuyThanhToanToan = 0;
                decimal bnPhaiNop = 0;
                decimal bnNhanLai = 0;
                decimal tamUngDV = 0;
                decimal mienGiam = 0;
                decimal ketChuyen = 0;
                decimal thanhToan = 0;
                decimal hoanUngDV = 0;
                decimal tamUngDVChuaHoanThu = 0;

                tongChiPhiBHYT = sereServVTTTs.Sum(o => o.PRICE_BHYT * o.AMOUNT)
                    + sereServHitechs.Sum(o => o.PRICE_BHYT * o.AMOUNT)
                    + sereServNotHiTechs.Sum(o => o.PRICE_BHYT * o.AMOUNT);

                tienBHYTThanhToan = sereServVTTTs.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0)
                    + sereServHitechs.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0)
                    + sereServNotHiTechs.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);

                nguoiBenhCungChiTra = tongChiPhiBHYT - tienBHYTThanhToan;

                if (dereDetails != null && dereDetails.Count > 0)
                {
                    tamUngDVChuaHoanThu = dereDetails.Where(o => o.REPAY_ID == null).Sum(o => o.DEPOSIT_AMOUNT);
                    hoanUngDV = dereDetails.Where(o => o.REPAY_ID != null).Sum(o => o.DEPOSIT_AMOUNT);
                    tamUngDV = dereDetails.Sum(o => o.DEPOSIT_AMOUNT);
                }

                tongThuBenhNhan = (sereServVTTTs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE)
                    + sereServHitechs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE)
                    + sereServNotHiTechs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE)) ?? 0;

                ngoaiDMChenhLech = tongThuBenhNhan - nguoiBenhCungChiTra;


                //ngoaiDMChenhLech + nguoiBenhCungChiTra;
                if (treatmentFees != null)
                {
                    mienGiam = treatmentFees[0].TOTAL_BILL_EXEMPTION ?? 0;
                    ketChuyen = treatmentFees[0].TOTAL_BILL_TRANSFER_AMOUNT ?? 0;
                    cacQuyThanhToanToan = treatmentFees[0].TOTAL_BILL_FUND ?? 0;
                    thanhToan = treatmentFees[0].TOTAL_BILL_AMOUNT ?? 0;

                    tamUng = treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0;
                    hoanUng = treatmentFees[0].TOTAL_REPAY_AMOUNT ?? 0;

                }

                bnDaThanhToan = tamUngDVChuaHoanThu + thanhToan - mienGiam - ketChuyen - cacQuyThanhToanToan;
                bnTamUng = (tamUng - tamUngDV) - (hoanUng - hoanUngDV);

                bnPhaiNop = tongThuBenhNhan - tamUng - thanhToan + ketChuyen + hoanUng;
                bnNhanLai = -bnPhaiNop;

                if (bnPhaiNop > 0)
                    keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.SO_TIEN_BN_PHAI_NOP, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnPhaiNop, 0)));
                else
                    keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.SO_TIEN_BN_PHAI_NOP, ""));
                if (bnNhanLai > 0)
                    keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.SO_TIEN_BN_NHAN_LAI, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnNhanLai, 0)));
                else
                    keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.SO_TIEN_BN_NHAN_LAI, ""));

                keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.TONG_CP_BHYT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(tongChiPhiBHYT, 0)));
                keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.TIEN_BHYT_THANH_TOAN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(tienBHYTThanhToan, 0)));
                keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.TIEN_BN_CUNG_CHI_TRA, Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguoiBenhCungChiTra, 0)));
                keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.TIEN_NGOAI_DM_VA_CHENH_LECH, Inventec.Common.Number.Convert.NumberToStringRoundAuto(ngoaiDMChenhLech, 0)));
                keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.TONG_THU_BN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(tongThuBenhNhan, 0)));
                keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.BN_DA_THANH_TOAN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnDaThanhToan, 0)));
                keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.BN_DA_TAM_UNG, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnTamUng, 0)));
                keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.BN_HOAN_UNG, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnHoanUng, 0)));
                keyValues.Add(new KeyValue(Mps000082ExtendSingleKey.CAC_QUY_THANH_TOAN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(cacQuyThanhToanToan, 0)));

                GlobalQuery.AddObjectKeyIntoListkey<PatyAlterBhytADO>(patyAlterBhytADO, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TREATMENT>(currentHisTreatment, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(patientADO, keyValues);

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
                sereServNotHiTechs = sereServNotHiTechs.Where(o=>o.IS_NO_EXECUTE==null).ToList();
                sereServHitechs = sereServHitechs.Where(o => o.IS_NO_EXECUTE == null).ToList();
                sereServVTTTs = sereServVTTTs.Where(o => o.IS_NO_EXECUTE == null).ToList();

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


                    var checkExitServiceGroup = ServiceGroups.Where(o => o.EXECUTE_DEPARTMENT_ID == itemHightTechDepartmentGroup.EXECUTE_DEPARTMENT_ID && o.HEIN_SERVICE_TYPE_ID == itemHightTechDepartmentGroup.HEIN_SERVICE_TYPE_ID).ToList();
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
