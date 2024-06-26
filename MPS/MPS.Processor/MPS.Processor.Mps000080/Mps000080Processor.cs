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
using FlexCel.Report;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000080.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000080
{
    class Mps000080Processor : AbstractProcessor
    {
        Mps000080PDO rdo;
        public Mps000080Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000080PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodeTreatmentCode = new Inventec.Common.BarcodeLib.Barcode(rdo.currentHisTreatment.TREATMENT_CODE);
                barcodeTreatmentCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatmentCode.IncludeLabel = false;
                barcodeTreatmentCode.Width = 120;
                barcodeTreatmentCode.Height = 40;
                barcodeTreatmentCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTreatmentCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatmentCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatmentCode.IncludeLabel = true;

                dicImage.Add(Mps000080ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatmentCode);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Ham xu ly du lieu da qua xu ly
        /// Tao ra cac doi tuong du lieu xu dung trong thu vien xu ly file excel
        /// </summary>
        /// <returns></returns>
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                ProcessSingleKey();
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                objectTag.AddObjectData(store, "Services", rdo.sereServNotHiTechs);
                objectTag.AddObjectData(store, "rdo.rdo.rdo.ServiceGroups", rdo.ServiceGroups);
                objectTag.AddObjectData(store, "Departments", rdo.DepartmentGroups);

                objectTag.AddRelationship(store, "rdo.rdo.rdo.ServiceGroups", "Departments", "HEIN_SERVICE_TYPE_ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "Departments", "Services", "REQUEST_DEPARTMENT_ID", "REQUEST_DEPARTMENT_ID");
                objectTag.AddRelationship(store, "rdo.rdo.rdo.ServiceGroups", "Services", "HEIN_SERVICE_TYPE_ID", "HEIN_SERVICE_TYPE_ID");


                objectTag.AddObjectData(store, "HightServiceGroups", rdo.ServiceHitechGroups);
                objectTag.AddObjectData(store, "HightTechServices", rdo.sereServHitechADOs);
                objectTag.AddObjectData(store, "HightTechDepartments", rdo.HitechDepartmentGroups);
                objectTag.AddObjectData(store, "ServiceInHightTechVTTTs", rdo.sereServVTTTs);

                objectTag.AddRelationship(store, "HightServiceGroups", "HightTechServices", "HEIN_SERVICE_TYPE_ID", "HEIN_SERVICE_TYPE_ID");

                objectTag.AddRelationship(store, "HightTechServices", "HightTechDepartments", "ID", "DEPARTMENT__GROUP_SERE_SERV");
                objectTag.AddRelationship(store, "HightTechServices", "ServiceInHightTechVTTTs", "ID", "PARENT_ID");
                objectTag.AddRelationship(store, "HightTechDepartments", "ServiceInHightTechVTTTs", "REQUEST_DEPARTMENT_ID", "REQUEST_DEPARTMENT_ID");




                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }



        void ProcessSingleKey()
        {
            try
            {
                if (rdo.departmentTrans != null && rdo.departmentTrans.Count > 0)
                {
                    SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.departmentTrans[0].LOG_TIME)));
                    if (rdo.departmentTrans[rdo.departmentTrans.Count - 1] != null && rdo.departmentTrans.Count > 1)
                    {
                        SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.departmentTrans[rdo.departmentTrans.Count - 1].LOG_TIME)));
                        //Số ngày điều trị
                        SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.TOTAL_DAY, rdo.totalDay));

                    }
                }

                if (rdo.departmentName != null)
                {
                    SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.DEPARTMENT_NAME, rdo.departmentName));
                }


                if (rdo.patyAlterBhytADO != null)
                {
                    if (rdo.patyAlterBhytADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (rdo.patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                        {
                            SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (rdo.patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (rdo.patyAlterBhytADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }

                    if (rdo.patyAlterBhytADO.IS_HEIN != null)
                        SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.IS_HEIN, "X"));
                    else
                        SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.IS_NOT_HEIN, "X"));
                }



                if (rdo.hisTranPati != null)
                {
                    SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.TRAN_PATI_MEDI_ORG_CODE, rdo.hisTranPati.MEDI_ORG_CODE));
                    SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.TRAN_PATI_MEDI_ORG_NAME, rdo.hisTranPati.MEDI_ORG_NAME));
                }

                //SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.CURRENT_DATE_SEPARATE_FULL_STR, currentDateSeparateFullTime));


                decimal thanhtien_tong = 0;
                decimal bhytthanhtoan_tong = 0;
                decimal nguonkhac_tong = 0;
                decimal bnthanhtoan_tong = 0;

                decimal thanhtien_tong_dvc = 0;
                decimal bhytthanhtoan_tran_tong_dvc = 0;
                decimal bhytthanhtoan_tong_dvc = 0;
                decimal nguonkhac_tong_dvc = 0;
                decimal bnthanhtoan_tong_dvc = 0;

                if (rdo.sereServNotHiTechs != null && rdo.sereServNotHiTechs.Count > 0)
                {
                    if (rdo.sereServVTTTs != null && rdo.sereServVTTTs.Count > 0)
                    {
                        thanhtien_tong_dvc = rdo.sereServVTTTs.Sum(o => (o.VIR_TOTAL_PRICE_NO_ADD_PRICE ?? 0));
                        bhytthanhtoan_tran_tong_dvc = 48400000;
                        nguonkhac_tong_dvc = 0;
                        if (thanhtien_tong_dvc > bhytthanhtoan_tran_tong_dvc)
                            bnthanhtoan_tong_dvc = thanhtien_tong_dvc - bhytthanhtoan_tran_tong_dvc;
                        else
                            bnthanhtoan_tong_dvc = 0;
                    }

                    thanhtien_tong = rdo.sereServVTTTs.Sum(o => (o.VIR_TOTAL_PRICE_NO_ADD_PRICE ?? 0)) + rdo.sereServNotHiTechs.Sum(o => (o.VIR_TOTAL_PRICE_NO_ADD_PRICE ?? 0));

                    if (bnthanhtoan_tong_dvc > 0)
                        bhytthanhtoan_tong_dvc = bhytthanhtoan_tran_tong_dvc;
                    else
                        bhytthanhtoan_tong_dvc = rdo.sereServVTTTs.Sum(o => (o.VIR_TOTAL_HEIN_PRICE ?? 0));
                    bhytthanhtoan_tong = bhytthanhtoan_tong_dvc + rdo.sereServNotHiTechs.Sum(o => (o.VIR_TOTAL_HEIN_PRICE ?? 0));
                    bnthanhtoan_tong = bnthanhtoan_tong_dvc + rdo.sereServNotHiTechs.Sum(o => (o.VIR_TOTAL_PATIENT_PRICE ?? 0));
                    nguonkhac_tong = 0;

                }



                SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong, 0)));
                SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.TOTAL_PRICE_OTHER, Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguonkhac_tong, 0)));
                SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.TOTAL_PRICE_PATIENT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.TOTAL_PRICE_OTHER_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(nguonkhac_tong).ToString())));

                if (rdo.treatmentFees != null)
                {
                    SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(rdo.treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0, 0)));
                }
                else
                {
                    SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, "0"));
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



                tongChiPhiBHYT = rdo.sereServVTTTs.Sum(o => o.PRICE_BHYT * o.AMOUNT)
                    + rdo.sereServHitechs.Sum(o => o.PRICE_BHYT * o.AMOUNT)
                    + rdo.sereServNotHiTechs.Sum(o => o.PRICE_BHYT * o.AMOUNT);

                tienBHYTThanhToan = rdo.sereServVTTTs.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0)
                    + rdo.sereServHitechs.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0)
                    + rdo.sereServNotHiTechs.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);

                nguoiBenhCungChiTra = tongChiPhiBHYT - tienBHYTThanhToan;

                //ngoaiDMChenhLech = (
                //    nguoiBenhCungChiTra 
                //    + rdo.sereServVTTTs.Where(o=>o.HEIN_LIMIT_PRICE==null).Sum(o=>o.VIR_TOTAL_PATIENT_PRICE)
                //    + rdo.sereServNotHiTechs.Where(o => o.HEIN_LIMIT_PRICE == null).Sum(o => o.VIR_TOTAL_PATIENT_PRICE)
                //    + rdo.sereServNotHiTechs.Where(o => o.HEIN_LIMIT_PRICE == null).Sum(o => o.VIR_TOTAL_PATIENT_PRICE)
                //    ) ?? 0;

                tongThuBenhNhan = (rdo.sereServVTTTs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE)
                    + rdo.sereServHitechs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE)
                    + rdo.sereServNotHiTechs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE)) ?? 0;

                ngoaiDMChenhLech = tongThuBenhNhan - nguoiBenhCungChiTra;
                //ngoaiDMChenhLech + nguoiBenhCungChiTra;
                if (rdo.treatmentFees != null)
                {
                    bnTamUng = rdo.treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0;
                    bnHoanUng = rdo.treatmentFees[0].TOTAL_REPAY_AMOUNT ?? 0;
                    bnDaThanhToan = (rdo.treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0) + (rdo.treatmentFees[0].TOTAL_BILL_AMOUNT ?? 0) - bnHoanUng;
                    cacQuyThanhToanToan = rdo.treatmentFees[0].TOTAL_BILL_FUND ?? 0;
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
                    SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.SO_TIEN_BN_PHAI_NOP, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnPhaiNop, 0)));
                else
                    SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.SO_TIEN_BN_PHAI_NOP, ""));
                if (bnNhanLai > 0)
                    SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.SO_TIEN_BN_NHAN_LAI, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnNhanLai, 0)));
                else
                    SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.SO_TIEN_BN_NHAN_LAI, ""));

                SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.TONG_CP_BHYT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(tongChiPhiBHYT, 0)));
                SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.TIEN_BHYT_THANH_TOAN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(tienBHYTThanhToan, 0)));
                SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.TIEN_BN_CUNG_CHI_TRA, Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguoiBenhCungChiTra, 0)));
                SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.TIEN_NGOAI_DM_VA_CHENH_LECH, Inventec.Common.Number.Convert.NumberToStringRoundAuto(ngoaiDMChenhLech, 0)));
                SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.TONG_THU_BN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(tongThuBenhNhan, 0)));
                SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.BN_DA_THANH_TOAN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnDaThanhToan, 0)));
                SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.BN_DA_TAM_UNG, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnTamUng, 0)));
                SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.BN_HOAN_UNG, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnHoanUng, 0)));
                SetSingleKey(new KeyValue(Mps000080ExtendSingleKey.CAC_QUY_THANH_TOAN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(cacQuyThanhToanToan, 0)));

                AddObjectKeyIntoListkey<PatyAlterBhytADO>(rdo.patyAlterBhytADO, false);
                AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.currentHisTreatment, false);
                AddObjectKeyIntoListkey<PatientADO>(rdo.patientADO);

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
                var sereServNotHiTechGroups = rdo.sereServNotHiTechs.GroupBy(o => new { o.SERVICE_ID, o.VIR_PRICE, o.REQUEST_DEPARTMENT_ID, o.PRICE_BHYT, o.IS_EXPEND, o.PATIENT_TYPE_ID, o.PRICE_POLICY_ID });
                rdo.sereServNotHiTechs = new List<SereServGroupPlusADO>();
                foreach (var sereServNotHiTechGroup in sereServNotHiTechGroups)
                {
                    SereServGroupPlusADO sereServPlus = new SereServGroupPlusADO();
                    sereServPlus = sereServNotHiTechGroup.FirstOrDefault();
                    sereServPlus.AMOUNT = sereServNotHiTechGroup.Sum(o => o.AMOUNT);
                    sereServPlus.VIR_TOTAL_PRICE_NO_EXPEND = sereServNotHiTechGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND) ?? 0;


                    sereServPlus.VIR_TOTAL_PRICE = sereServNotHiTechGroup.Sum(o => o.VIR_TOTAL_PRICE) ?? 0;
                    sereServPlus.VIR_TOTAL_HEIN_PRICE = sereServNotHiTechGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    sereServPlus.VIR_TOTAL_PRICE_NO_EXPEND = sereServNotHiTechGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND) ?? 0;

                    sereServPlus.VIR_TOTAL_PRICE_NO_ADD_PRICE = sereServPlus.AMOUNT * sereServPlus.PRICE_BHYT;
                    sereServPlus.VIR_TOTAL_PATIENT_PRICE = sereServPlus.VIR_TOTAL_PRICE_NO_ADD_PRICE - sereServPlus.VIR_TOTAL_HEIN_PRICE;
                    if (sereServPlus.VIR_TOTAL_PATIENT_PRICE < 0)
                        sereServPlus.VIR_TOTAL_PATIENT_PRICE = 0;
                    rdo.sereServNotHiTechs.Add(sereServPlus);
                }

                var sereServVTTTGroups = rdo.sereServVTTTs.GroupBy(o => new { o.SERVICE_ID, o.REQUEST_DEPARTMENT_ID, o.VIR_PRICE, o.PRICE_BHYT, o.IS_EXPEND, o.PATIENT_TYPE_ID, o.PRICE_POLICY_ID });
                rdo.sereServVTTTs = new List<SereServGroupPlusADO>();
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
                    rdo.sereServVTTTs.Add(sereServPlus);
                }

                var sereServHitechGroups = rdo.sereServHitechs.GroupBy(o => new { o.SERVICE_ID, o.REQUEST_DEPARTMENT_ID, o.VIR_PRICE, o.PRICE_BHYT, o.IS_EXPEND, o.PATIENT_TYPE_ID, o.PRICE_POLICY_ID });
                rdo.sereServHitechs = new List<SereServGroupPlusADO>();
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
                    rdo.sereServHitechs.Add(sereServPlus);
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
                rdo.sereServHitechADOs = new List<SereServGroupPlusADO>();

                decimal totalPriceHight = 0;
                decimal totalHeinPriceHight = 0;
                decimal totalPatientPriceHight = 0;

                foreach (var sereServHitech in rdo.sereServHitechs)
                {
                    totalPriceHight = rdo.sereServVTTTs.Where(o => o.PARENT_ID == sereServHitech.ID).Sum(o => o.VIR_TOTAL_PRICE_NO_ADD_PRICE) ?? 0;
                    sereServHitech.VIR_TOTAL_PRICE_KTC = totalPriceHight;

                    totalHeinPriceHight = rdo.sereServVTTTs.Where(o => o.PARENT_ID == sereServHitech.ID).Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    sereServHitech.VIR_TOTAL_HEIN_PRICE_SUM = totalHeinPriceHight;

                    totalPatientPriceHight = sereServHitech.VIR_TOTAL_PRICE_KTC - sereServHitech.VIR_TOTAL_HEIN_PRICE_SUM;

                    //totalPatientPriceHight = rdo.sereServVTTTs.Where(o => o.PARENT_ID == sereServHitech.ID).Sum(o => o.VIR_TOTAL_PATIENT_PRICE)??0;

                    if (totalPatientPriceHight < 0)
                        totalPatientPriceHight = 0;
                    sereServHitech.VIR_TOTAL_PATIENT_PRICE_SUM = totalPatientPriceHight;
                    rdo.sereServHitechADOs.Add(sereServHitech);
                }

                foreach (var sereServVTTT in rdo.sereServVTTTs)
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
                rdo.sereServNotHiTechs = rdo.sereServNotHiTechs.Where(o => o.IS_NO_EXECUTE == null).ToList();
                rdo.sereServHitechs = rdo.sereServHitechs.Where(o => o.IS_NO_EXECUTE == null).ToList(); ;
                rdo.sereServVTTTs = rdo.sereServVTTTs.Where(o => o.IS_NO_EXECUTE == null).ToList();

                rdo.ServiceGroups = new List<SereServGroupPlusADO>();
                rdo.DepartmentGroups = new List<SereServGroupPlusADO>();
                rdo.ServiceHitechGroups = new List<SereServGroupPlusADO>();
                rdo.HitechDepartmentGroups = new List<SereServGroupPlusADO>();

                //Group Service Not Hitech
                var sServceReportGroups = rdo.sereServNotHiTechs.GroupBy(o => o.HEIN_SERVICE_TYPE_ID).ToList();
                foreach (var sServceReportGroup in sServceReportGroups)
                {
                    List<SereServGroupPlusADO> subSServiceReportGroup = sServceReportGroup.ToList<SereServGroupPlusADO>();

                    SereServGroupPlusADO itemSServiceReportGroup = subSServiceReportGroup.First();
                    if (itemSServiceReportGroup.HEIN_SERVICE_TYPE_ID != null)
                        itemSServiceReportGroup.HEIN_SERVICE_TYPE_NAME = rdo.ServiceReports.Where(o => o.ID == subSServiceReportGroup.First().HEIN_SERVICE_TYPE_ID).SingleOrDefault().HEIN_SERVICE_TYPE_NAME;
                    else
                        itemSServiceReportGroup.HEIN_SERVICE_TYPE_NAME = "Khác";
                    itemSServiceReportGroup.TOTAL_PRICE_SERVICE_GROUP = subSServiceReportGroup.Sum(o => o.VIR_TOTAL_PRICE_NO_ADD_PRICE) ?? 0;
                    itemSServiceReportGroup.TOTAL_HEIN_PRICE_SERVICE_GROUP = subSServiceReportGroup.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    itemSServiceReportGroup.TOTAL_PATIENT_PRICE_SERVICE_GROUP = subSServiceReportGroup.Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;

                    rdo.ServiceGroups.Add(itemSServiceReportGroup);

                    //Nhom Department
                    var sDepartmentGroups = subSServiceReportGroup.OrderBy(o => o.REQUEST_DEPARTMENT_ID).GroupBy(o => o.REQUEST_DEPARTMENT_ID).ToList();
                    foreach (var sDepartmentGroup in sDepartmentGroups)
                    {
                        List<SereServGroupPlusADO> subSDepartmentGroups = sDepartmentGroup.ToList<SereServGroupPlusADO>();
                        SereServGroupPlusADO itemSDepartmentGroup = subSDepartmentGroups.First();

                        rdo.DepartmentGroups.Add(itemSDepartmentGroup);
                    }
                }

                //Group Hitech
                var sServceReportHitechGroups = rdo.sereServHitechADOs.GroupBy(o => o.HEIN_SERVICE_TYPE_ID).ToList();
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

                    foreach (var sereServHitech in rdo.sereServHitechADOs)
                    {
                        totalPriceHight = sereServHitech.VIR_TOTAL_PRICE_KTC;
                        totalPriceHightGroup += totalPriceHight;

                        totalHeinPriceHight = sereServHitech.VIR_TOTAL_HEIN_PRICE_SUM;
                        totalHeinPriceHightGroup += totalHeinPriceHight;

                        totalPatientPriceHight = sereServHitech.VIR_TOTAL_PATIENT_PRICE_SUM;
                        totalPatientPriceHightGroup += totalPatientPriceHight;
                    }

                    itemSServiceReportHitechGroup.ROW_POS = rdo.ServiceGroups.Count + 1;
                    itemSServiceReportHitechGroup.VIR_TOTAL_PRICE_KTC_HIGHTTECH_GROUP = totalPriceHightGroup;
                    itemSServiceReportHitechGroup.VIR_TOTAL_PATIENT_PRICE_HIGHTTECH_GROUP = totalPatientPriceHightGroup;
                    itemSServiceReportHitechGroup.VIR_TOTAL_HEIN_PRICE_HIGHTTECH_GROUP = totalHeinPriceHightGroup;
                    itemSServiceReportHitechGroup.HEIN_SERVICE_TYPE_NAME = rdo.ServiceReports.Where(o => o.ID == subSServiceReportHitechGroup.First().HEIN_SERVICE_TYPE_ID).SingleOrDefault().HEIN_SERVICE_TYPE_NAME;
                    rdo.ServiceHitechGroups.Add(itemSServiceReportHitechGroup);

                }

                var sereServHitechGroups = rdo.sereServHitechADOs.GroupBy(o => o.ID);
                foreach (var sereServHitechGroup in sereServHitechGroups)
                {
                    List<SereServGroupPlusADO> subSereServHitechGroup = sereServHitechGroup.ToList<SereServGroupPlusADO>();

                    var sHitechDepartmentGroups = rdo.sereServVTTTs.Where(o => o.PARENT_ID == subSereServHitechGroup.First().ID).OrderBy(o => o.REQUEST_DEPARTMENT_ID).GroupBy(o => o.REQUEST_DEPARTMENT_ID).ToList();
                    foreach (var sHitechDepartmentGroup in sHitechDepartmentGroups)
                    {
                        List<SereServGroupPlusADO> subSHitechDepartmentGroups = sHitechDepartmentGroup.ToList<SereServGroupPlusADO>();
                        SereServGroupPlusADO itemSHitechDepartmentGroup = subSHitechDepartmentGroups.First();
                        itemSHitechDepartmentGroup.DEPARTMENT__GROUP_SERE_SERV = subSereServHitechGroup.First().PARENT_ID ?? 0;
                        rdo.HitechDepartmentGroups.Add(itemSHitechDepartmentGroup);
                    }

                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }

    class CustomerFuncRownumberData : TFlexCelUserFunction
    {
        public CustomerFuncRownumberData()
        {
        }
        public override object Evaluate(object[] parameters)
        {
            if (parameters == null || parameters.Length < 1)
                throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");

            long result = 0;
            try
            {
                long rownumber = Convert.ToInt64(parameters[0]);
                result = (rownumber + 1);
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }

            return result;
        }
    }
}
