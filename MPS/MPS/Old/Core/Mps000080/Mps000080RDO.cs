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

namespace MPS.Core.Mps000080
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000080RDO : RDOBase
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
        internal List<SereServGroupPlusADO> ServiceHitechGroups { get; set; }
        internal List<SereServGroupPlusADO> HitechDepartmentGroups { get; set; }

        MOS.EFMODEL.DataModels.V_HIS_TRAN_PATI hisTranPati { get; set; }
        List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE> treatmentFees { get; set; }
        long totalDay { get; set; }

        public Mps000080RDO(
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
            long totalDay
            
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
                    keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(departmentTrans[0].LOG_TIME)));
                    if (departmentTrans[departmentTrans.Count - 1] != null && departmentTrans.Count>1)
                    {
                        keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(departmentTrans[departmentTrans.Count - 1].LOG_TIME)));
                        //Số ngày điều trị
                        keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.TOTAL_DAY, totalDay));

                    }
                }

                if (departmentName != null)
                {
                    keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.DEPARTMENT_NAME, departmentName));
                }


                if (patyAlterBhytADO != null)
                {
                    if (patyAlterBhytADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                        {
                            keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (patyAlterBhytADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }

                    if (patyAlterBhytADO.IS_HEIN != null)
                        keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.IS_HEIN, "X"));
                    else
                        keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.IS_NOT_HEIN, "X"));
                }



                if (hisTranPati != null)
                {
                    keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.TRAN_PATI_MEDI_ORG_CODE, hisTranPati.MEDI_ORG_CODE));
                    keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.TRAN_PATI_MEDI_ORG_NAME, hisTranPati.MEDI_ORG_NAME));
                }

                //keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.CURRENT_DATE_SEPARATE_FULL_STR, currentDateSeparateFullTime));


                decimal thanhtien_tong = 0;
                decimal bhytthanhtoan_tong = 0;
                decimal nguonkhac_tong = 0;
                decimal bnthanhtoan_tong = 0;

                decimal thanhtien_tong_dvc = 0;
                decimal bhytthanhtoan_tran_tong_dvc = 0;
                decimal bhytthanhtoan_tong_dvc = 0;
                decimal nguonkhac_tong_dvc = 0;
                decimal bnthanhtoan_tong_dvc = 0;

                if (sereServNotHiTechs != null && sereServNotHiTechs.Count > 0)
                {
                    if (sereServVTTTs != null && sereServVTTTs.Count>0)
                        {
                            thanhtien_tong_dvc = sereServVTTTs.Sum(o => (o.VIR_TOTAL_PRICE_NO_ADD_PRICE ?? 0));
                            bhytthanhtoan_tran_tong_dvc = 48400000;
                            nguonkhac_tong_dvc = 0;
                            if (thanhtien_tong_dvc > bhytthanhtoan_tran_tong_dvc)
                                bnthanhtoan_tong_dvc = thanhtien_tong_dvc - bhytthanhtoan_tran_tong_dvc;
                            else
                                bnthanhtoan_tong_dvc = 0;
                        }

                    thanhtien_tong = sereServVTTTs.Sum(o => (o.VIR_TOTAL_PRICE_NO_ADD_PRICE ?? 0)) + sereServNotHiTechs.Sum(o => (o.VIR_TOTAL_PRICE_NO_ADD_PRICE ?? 0));

                    if (bnthanhtoan_tong_dvc > 0)
                        bhytthanhtoan_tong_dvc = bhytthanhtoan_tran_tong_dvc;
                    else
                        bhytthanhtoan_tong_dvc = sereServVTTTs.Sum(o => (o.VIR_TOTAL_HEIN_PRICE ?? 0));
                    bhytthanhtoan_tong = bhytthanhtoan_tong_dvc + sereServNotHiTechs.Sum(o => (o.VIR_TOTAL_HEIN_PRICE ?? 0));
                    bnthanhtoan_tong = bnthanhtoan_tong_dvc + sereServNotHiTechs.Sum(o => (o.VIR_TOTAL_PATIENT_PRICE ?? 0));
                    nguonkhac_tong = 0;
                        
                }



                keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong, 0)));
                keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong, 0)));
                keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.TOTAL_PRICE_OTHER, Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguonkhac_tong, 0)));
                keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong).ToString())));
                keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.TOTAL_PRICE_PATIENT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong).ToString())));
                keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.TOTAL_PRICE_OTHER_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(nguonkhac_tong).ToString())));

                if (treatmentFees != null)
                {
                    keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0, 0)));
                }
                else
                {
                    keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, "0"));
                }

                //
                decimal tongChiPhiBHYT = 0;
                decimal tienBHYTThanhToan = 0;
                decimal nguoiBenhCungChiTra = 0;
                decimal ngoaiDMChenhLech = 0;
                decimal tongThuBenhNhan = 0;
                decimal bnDaThanhToan = 0;
                decimal bnTamUng = 0;
                decimal bnHoanUng = 0;
                decimal cacQuyThanhToanToan = 0;
                decimal bnPhaiNop = 0;
                decimal bnNhanLai = 0;



                tongChiPhiBHYT = sereServVTTTs.Sum(o => o.PRICE_BHYT * o.AMOUNT)
                    + sereServHitechs.Sum(o => o.PRICE_BHYT * o.AMOUNT)
                    + sereServNotHiTechs.Sum(o => o.PRICE_BHYT * o.AMOUNT);

                tienBHYTThanhToan = sereServVTTTs.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0)
                    + sereServHitechs.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0)
                    + sereServNotHiTechs.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);

                nguoiBenhCungChiTra = tongChiPhiBHYT - tienBHYTThanhToan;

                //ngoaiDMChenhLech = (
                //    nguoiBenhCungChiTra 
                //    + sereServVTTTs.Where(o=>o.HEIN_LIMIT_PRICE==null).Sum(o=>o.VIR_TOTAL_PATIENT_PRICE)
                //    + sereServNotHiTechs.Where(o => o.HEIN_LIMIT_PRICE == null).Sum(o => o.VIR_TOTAL_PATIENT_PRICE)
                //    + sereServNotHiTechs.Where(o => o.HEIN_LIMIT_PRICE == null).Sum(o => o.VIR_TOTAL_PATIENT_PRICE)
                //    ) ?? 0;

                tongThuBenhNhan = (sereServVTTTs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE)
                    + sereServHitechs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE)
                    + sereServNotHiTechs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE)) ?? 0;

                ngoaiDMChenhLech = tongThuBenhNhan - nguoiBenhCungChiTra;
                //ngoaiDMChenhLech + nguoiBenhCungChiTra;
                if (treatmentFees != null)
                {
                    bnTamUng = treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0;
                    bnHoanUng = treatmentFees[0].TOTAL_REPAY_AMOUNT ?? 0;
                    bnDaThanhToan = (treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0) + (treatmentFees[0].TOTAL_BILL_AMOUNT ?? 0) - bnHoanUng;
                    cacQuyThanhToanToan = treatmentFees[0].TOTAL_BILL_FUND ?? 0;
                }
                else
                {
                    bnDaThanhToan = 0;
                    bnTamUng = 0;
                    cacQuyThanhToanToan = 0;
                    bnHoanUng = 0;
                }

                bnPhaiNop = tongThuBenhNhan - bnDaThanhToan;
                bnNhanLai = bnDaThanhToan - tongThuBenhNhan;

                if (bnPhaiNop > 0)
                    keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.SO_TIEN_BN_PHAI_NOP, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnPhaiNop, 0)));
                else
                    keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.SO_TIEN_BN_PHAI_NOP, ""));
                if (bnNhanLai > 0)
                    keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.SO_TIEN_BN_NHAN_LAI, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnNhanLai, 0)));
                else
                    keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.SO_TIEN_BN_NHAN_LAI, ""));

                keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.TONG_CP_BHYT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(tongChiPhiBHYT, 0)));
                keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.TIEN_BHYT_THANH_TOAN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(tienBHYTThanhToan, 0)));
                keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.TIEN_BN_CUNG_CHI_TRA, Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguoiBenhCungChiTra, 0)));
                keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.TIEN_NGOAI_DM_VA_CHENH_LECH, Inventec.Common.Number.Convert.NumberToStringRoundAuto(ngoaiDMChenhLech, 0)));
                keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.TONG_THU_BN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(tongThuBenhNhan, 0)));
                keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.BN_DA_THANH_TOAN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnDaThanhToan, 0)));
                keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.BN_DA_TAM_UNG, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnTamUng, 0)));
                keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.BN_HOAN_UNG, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnHoanUng, 0)));
                keyValues.Add(new KeyValue(Mps000080ExtendSingleKey.CAC_QUY_THANH_TOAN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(cacQuyThanhToanToan, 0)));

                GlobalQuery.AddObjectKeyIntoListkey<PatyAlterBhytADO>(patyAlterBhytADO, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TREATMENT>(currentHisTreatment, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(patientADO, keyValues);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void ProcessPriceTotalSereServ()
        {
            try
            {
                var sereServNotHiTechGroups = sereServNotHiTechs.GroupBy(o => new { o.SERVICE_ID, o.VIR_PRICE, o.REQUEST_DEPARTMENT_ID, o.PRICE_BHYT, o.IS_EXPEND, o.PATIENT_TYPE_ID, o.PRICE_POLICY_ID });
                sereServNotHiTechs = new List<SereServGroupPlusADO>();
                foreach (var sereServNotHiTechGroup in sereServNotHiTechGroups)
                {
                    SereServGroupPlusADO sereServPlus = new SereServGroupPlusADO();
                    sereServPlus = sereServNotHiTechGroup.FirstOrDefault();
                    sereServPlus.AMOUNT = sereServNotHiTechGroup.Sum(o=>o.AMOUNT);
                    sereServPlus.VIR_TOTAL_PRICE_NO_EXPEND = sereServNotHiTechGroup.Sum(o=>o.VIR_TOTAL_PRICE_NO_EXPEND) ?? 0;

                        
                    sereServPlus.VIR_TOTAL_PRICE = sereServNotHiTechGroup.Sum(o => o.VIR_TOTAL_PRICE) ?? 0;
                    sereServPlus.VIR_TOTAL_HEIN_PRICE = sereServNotHiTechGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    sereServPlus.VIR_TOTAL_PRICE_NO_EXPEND = sereServNotHiTechGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND) ?? 0;

                    sereServPlus.VIR_TOTAL_PRICE_NO_ADD_PRICE = sereServPlus.AMOUNT * sereServPlus.PRICE_BHYT;
                    sereServPlus.VIR_TOTAL_PATIENT_PRICE = sereServPlus.VIR_TOTAL_PRICE_NO_ADD_PRICE - sereServPlus.VIR_TOTAL_HEIN_PRICE;
                    if (sereServPlus.VIR_TOTAL_PATIENT_PRICE < 0)
                        sereServPlus.VIR_TOTAL_PATIENT_PRICE = 0;
                    sereServNotHiTechs.Add(sereServPlus);
                }

                var sereServVTTTGroups = sereServVTTTs.GroupBy(o => new { o.SERVICE_ID, o.REQUEST_DEPARTMENT_ID, o.VIR_PRICE, o.PRICE_BHYT, o.IS_EXPEND, o.PATIENT_TYPE_ID, o.PRICE_POLICY_ID });
                sereServVTTTs = new List<SereServGroupPlusADO>();
                foreach (var sereServVTTTGroup in sereServVTTTGroups)
                {
                    SereServGroupPlusADO sereServPlus = new SereServGroupPlusADO();
                    sereServPlus = sereServVTTTGroup.FirstOrDefault();
                    sereServPlus.AMOUNT = sereServVTTTGroup.Sum(o => o.AMOUNT);
                    sereServPlus.VIR_TOTAL_PRICE_NO_EXPEND = sereServVTTTGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND) ?? 0;

                    sereServPlus.VIR_TOTAL_PRICE = sereServVTTTGroup.Sum(o => o.VIR_TOTAL_PRICE) ?? 0;
                    sereServPlus.VIR_TOTAL_HEIN_PRICE = sereServVTTTGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    sereServPlus.VIR_TOTAL_PRICE_NO_EXPEND = sereServVTTTGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND) ?? 0;

                    sereServPlus.VIR_TOTAL_PRICE_NO_ADD_PRICE = sereServPlus.AMOUNT * sereServPlus.PRICE_BHYT;
                    sereServPlus.VIR_TOTAL_PATIENT_PRICE = sereServPlus.VIR_TOTAL_PRICE_NO_ADD_PRICE - sereServPlus.VIR_TOTAL_HEIN_PRICE;
                    if (sereServPlus.VIR_TOTAL_PATIENT_PRICE < 0)
                        sereServPlus.VIR_TOTAL_PATIENT_PRICE = 0;
                    sereServVTTTs.Add(sereServPlus);
                }

                var sereServHitechGroups = sereServHitechs.GroupBy(o => new { o.SERVICE_ID, o.REQUEST_DEPARTMENT_ID, o.VIR_PRICE, o.PRICE_BHYT, o.IS_EXPEND, o.PATIENT_TYPE_ID, o.PRICE_POLICY_ID });
                sereServHitechs = new List<SereServGroupPlusADO>();
                foreach (var sereServHitechGroup in sereServHitechGroups)
                {
                    SereServGroupPlusADO sereServPlus = new SereServGroupPlusADO();
                    sereServPlus = sereServHitechGroup.FirstOrDefault();
                    sereServPlus.AMOUNT = sereServHitechGroup.Sum(o => o.AMOUNT);
                    sereServPlus.VIR_TOTAL_PRICE_NO_EXPEND = sereServHitechGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND) ?? 0;

                    sereServPlus.VIR_TOTAL_PRICE = sereServHitechGroup.Sum(o => o.VIR_TOTAL_PRICE) ?? 0;
                    sereServPlus.VIR_TOTAL_HEIN_PRICE = sereServHitechGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    sereServPlus.VIR_TOTAL_PRICE_NO_EXPEND = sereServHitechGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND) ?? 0;

                    sereServPlus.VIR_TOTAL_PRICE_NO_ADD_PRICE = sereServPlus.AMOUNT * sereServPlus.PRICE_BHYT;
                    sereServPlus.VIR_TOTAL_PATIENT_PRICE = sereServPlus.VIR_TOTAL_PRICE_NO_ADD_PRICE - sereServPlus.VIR_TOTAL_HEIN_PRICE;
                    if (sereServPlus.VIR_TOTAL_PATIENT_PRICE < 0)
                        sereServPlus.VIR_TOTAL_PATIENT_PRICE = 0;
                    sereServHitechs.Add(sereServPlus);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void ProcessTotalPriceSereServHitech()
        {
            try
            {
                sereServHitechADOs = new List<SereServGroupPlusADO>();

                decimal totalPriceHight = 0;
                decimal totalHeinPriceHight = 0;
                decimal totalPatientPriceHight = 0;

                foreach (var sereServHitech in sereServHitechs)
                {
                    totalPriceHight = sereServVTTTs.Where(o => o.PARENT_ID == sereServHitech.ID).Sum(o => o.VIR_TOTAL_PRICE_NO_ADD_PRICE) ?? 0;
                    sereServHitech.VIR_TOTAL_PRICE_KTC = totalPriceHight;

                    totalHeinPriceHight = sereServVTTTs.Where(o => o.PARENT_ID == sereServHitech.ID).Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    sereServHitech.VIR_TOTAL_HEIN_PRICE_SUM = totalHeinPriceHight;

                    totalPatientPriceHight = sereServHitech.VIR_TOTAL_PRICE_KTC - sereServHitech.VIR_TOTAL_HEIN_PRICE_SUM;

                    //totalPatientPriceHight = sereServVTTTs.Where(o => o.PARENT_ID == sereServHitech.ID).Sum(o => o.VIR_TOTAL_PATIENT_PRICE)??0;

                    if (totalPatientPriceHight < 0)
                        totalPatientPriceHight = 0; 
                    sereServHitech.VIR_TOTAL_PATIENT_PRICE_SUM = totalPatientPriceHight;
                    sereServHitechADOs.Add(sereServHitech);
                }

                foreach (var sereServVTTT in sereServVTTTs)
                {
                    //sereServVTTT.VIR_TOTAL_PRICE = sereServVTTT.PRICE_BHYT * sereServVTTT.AMOUNT;
                    //sereServVTTT.VIR_TOTAL_PATIENT_PRICE = sereServVTTT.VIR_TOTAL_PRICE - sereServVTTT.VIR_TOTAL_HEIN_PRICE;
                    if (sereServVTTT.ID == sereServVTTT.PARENT_ID)
                    {
                        sereServVTTT.REQUEST_DEPARTMENT_ID = sereServVTTT.EXECUTE_DEPARTMENT_ID;
                        sereServVTTT.REQUEST_DEPARTMENT_NAME = sereServVTTT.EXECUTE_DEPARTMENT_NAME;
                    }
                    
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
                sereServHitechs = sereServHitechs.Where(o => o.IS_NO_EXECUTE == null).ToList(); ;
                sereServVTTTs = sereServVTTTs.Where(o => o.IS_NO_EXECUTE == null).ToList();

                ServiceGroups = new List<SereServGroupPlusADO>();
                DepartmentGroups = new List<SereServGroupPlusADO>();
                ServiceHitechGroups = new List<SereServGroupPlusADO>();
                HitechDepartmentGroups = new List<SereServGroupPlusADO>();

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

                        DepartmentGroups.Add(itemSDepartmentGroup);
                    }
                }

                //Group Hitech
                var sServceReportHitechGroups = sereServHitechADOs.GroupBy(o => o.HEIN_SERVICE_TYPE_ID).ToList();
                foreach (var sServceReportHitechGroup in sServceReportHitechGroups)
                {
                    List<SereServGroupPlusADO> subSServiceReportHitechGroup = sServceReportHitechGroup.ToList<SereServGroupPlusADO>();

                    SereServGroupPlusADO itemSServiceReportHitechGroup = subSServiceReportHitechGroup.First();

                    decimal totalPriceHightGroup = 0;
                    decimal totalHeinPriceHightGroup = 0;
                    decimal totalPatientPriceHightGroup = 0;

                    decimal totalPriceHight = 0;
                    decimal totalHeinPriceHight = 0;
                    decimal totalPatientPriceHight = 0;

                    foreach (var sereServHitech in sereServHitechADOs)
                    {
                        totalPriceHight = sereServHitech.VIR_TOTAL_PRICE_KTC;
                        totalPriceHightGroup += totalPriceHight;

                        totalHeinPriceHight = sereServHitech.VIR_TOTAL_HEIN_PRICE_SUM;
                        totalHeinPriceHightGroup += totalHeinPriceHight;

                        totalPatientPriceHight = sereServHitech.VIR_TOTAL_PATIENT_PRICE_SUM;
                        totalPatientPriceHightGroup += totalPatientPriceHight;
                    }

                    itemSServiceReportHitechGroup.ROW_POS = ServiceGroups.Count + 1;
                    itemSServiceReportHitechGroup.VIR_TOTAL_PRICE_KTC_HIGHTTECH_GROUP = totalPriceHightGroup;
                    itemSServiceReportHitechGroup.VIR_TOTAL_PATIENT_PRICE_HIGHTTECH_GROUP = totalPatientPriceHightGroup;
                    itemSServiceReportHitechGroup.VIR_TOTAL_HEIN_PRICE_HIGHTTECH_GROUP =  totalHeinPriceHightGroup;
                    itemSServiceReportHitechGroup.HEIN_SERVICE_TYPE_NAME = ServiceReports.Where(o => o.ID == subSServiceReportHitechGroup.First().HEIN_SERVICE_TYPE_ID).SingleOrDefault().HEIN_SERVICE_TYPE_NAME;
                    ServiceHitechGroups.Add(itemSServiceReportHitechGroup);

                }

                var sereServHitechGroups = sereServHitechADOs.GroupBy(o => o.ID);
                foreach (var sereServHitechGroup in sereServHitechGroups)
                {
                    List<SereServGroupPlusADO> subSereServHitechGroup = sereServHitechGroup.ToList<SereServGroupPlusADO>();

                    var sHitechDepartmentGroups = sereServVTTTs.Where(o => o.PARENT_ID == subSereServHitechGroup.First().ID).OrderBy(o => o.REQUEST_DEPARTMENT_ID).GroupBy(o => o.REQUEST_DEPARTMENT_ID).ToList();
                    foreach (var sHitechDepartmentGroup in sHitechDepartmentGroups)
                    {
                        List<SereServGroupPlusADO> subSHitechDepartmentGroups = sHitechDepartmentGroup.ToList<SereServGroupPlusADO>();
                        SereServGroupPlusADO itemSHitechDepartmentGroup = subSHitechDepartmentGroups.First();
                        itemSHitechDepartmentGroup.DEPARTMENT__GROUP_SERE_SERV = subSereServHitechGroup.First().PARENT_ID ?? 0;
                        HitechDepartmentGroups.Add(itemSHitechDepartmentGroup);
                    }

                }
               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


    }
}
