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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000312.ADO;
using MPS.Processor.Mps000312.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000312
{
    public partial class Mps000312Processor : AbstractProcessor
    {
        private PatientADO patientADO { get; set; }
        private PatyAlterBhytADO patyAlterBHYTADO { get; set; }
        private List<SereServADO> sereServADOs { get; set; }
        private List<SereServADO> sereServPTTTADOs { get; set; }
        private List<SereServADO> sereServInPackageOutFeeADOs { get; set; }
        private List<HeinServiceTypeADO> heinServiceTypeADOs { get; set; }
        private List<DepartmentADO> departmentADOs { get; set; }
        private List<DepartmentADO> ptttDepartmentADOs { get; set; }
        private List<DepartmentADO> vtttDepartmentADOs { get; set; }

        Mps000312PDO rdo;

        public Mps000312Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000312PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                DataInputProcess();
                ProcessGroupSereServ();
                ProcessSingleKey();
                SetBarcodeKey();

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                objectTag.AddObjectData(store, "Services", sereServADOs);
                objectTag.AddObjectData(store, "HeinServiceType", heinServiceTypeADOs);
                objectTag.AddObjectData(store, "Departments", departmentADOs);
                objectTag.AddObjectData(store, "HightTechDepartments", ptttDepartmentADOs);
                //objectTag.AddObjectData(store, "ServiceVTTTDepartments", vtttDepartmentADOs);
                objectTag.AddObjectData(store, "ServiceHightTechs", sereServPTTTADOs);
                objectTag.AddObjectData(store, "ServiceVTTTs", sereServInPackageOutFeeADOs);

                objectTag.AddRelationship(store, "HeinServiceType", "Departments", "ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "Departments", "Services", "ID", "TDL_REQUEST_DEPARTMENT_ID");
                objectTag.AddRelationship(store, "HeinServiceType", "Services", "ID", "TDL_HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "HeinServiceType", "HightTechDepartments", "ID", "HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "HeinServiceType", "ServiceHightTechs", "ID", "TDL_HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "HightTechDepartments", "ServiceHightTechs", "ID", "TDL_EXECUTE_DEPARTMENT_ID");
                //objectTag.AddRelationship(store, "HightTechDepartments", "ServiceVTTTs", "ID", "TDL_REQUEST_DEPARTMENT_ID");
                //objectTag.AddRelationship(store, "HeinServiceType", "ServiceVTTTs", "ID", "TDL_HEIN_SERVICE_TYPE_ID");
                objectTag.AddRelationship(store, "ServiceHightTechs", "ServiceVTTTs", "ID", "PARENT_ID");

                objectTag.SetUserFunction(store, "ReplaceValue", new ReplaceValueFunction());

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        class ReplaceValueFunction : FlexCel.Report.TFlexCelUserFunction
        {
            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length <= 0)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");

                try
                {
                    string value = parameters[0] + "";
                    if (!String.IsNullOrEmpty(value))
                    {
                        value = value.Replace(';', '/');
                    }
                    return value;
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    return parameters[0];
                }
            }
        }

        private void ProcessSingleKey()
        {
            try
            {
                SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.RATIO_STR, rdo.SingleKeyValue.ratio));
                SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.USERNAME_RETURN_RESULT, rdo.SingleKeyValue.userNameReturnResult));
                SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.STATUS_TREATMENT_OUT, rdo.SingleKeyValue.statusTreatmentOut));

                HIS_PATIENT_TYPE_ALTER patyAlterBhytADO = null;
                if (rdo.PatientTypeAlterAlls != null && rdo.PatientTypeAlterAlls.Count > 0)
                {
                    patyAlterBhytADO = rdo.PatientTypeAlterAlls.OrderBy(o => o.LOG_TIME).FirstOrDefault(o => !String.IsNullOrEmpty(o.HEIN_CARD_NUMBER));
                }

                if (patyAlterBHYTADO != null)
                {
                    HIS_BRANCH branch = rdo.Branch != null ? rdo.Branch : new HIS_BRANCH();
                    string province = !String.IsNullOrWhiteSpace(patyAlterBhytADO.HEIN_MEDI_ORG_CODE) ? patyAlterBhytADO.HEIN_MEDI_ORG_CODE.Substring(0, 2) : "";
                    HIS_MEDI_ORG mediOrg = rdo.ListMediOrg != null && rdo.ListMediOrg.Count > 0 ? rdo.ListMediOrg.FirstOrDefault(o => o.MEDI_ORG_CODE == patyAlterBHYTADO.HEIN_MEDI_ORG_CODE) : new HIS_MEDI_ORG();

                    SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.IS_HEIN, "X"));
                    SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.HEIN_CARD_ADDRESS, patyAlterBHYTADO.ADDRESS));

                    string RIGHT_ROUTE_TYPE_NAME_CC = "";
                    string RIGHT_ROUTE_TYPE_NAME = "";
                    string NOT_RIGHT_ROUTE_TYPE_NAME = "";
                    string RIGHT_ROUTE_TYPE_NAME_TT = "";

                    if (patyAlterBhytADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                        {
                            RIGHT_ROUTE_TYPE_NAME_CC = "X";
                        }
                        else if (patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.OVER)
                        {
                            RIGHT_ROUTE_TYPE_NAME_TT = "X";
                        }
                        else if (patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)
                        {
                            RIGHT_ROUTE_TYPE_NAME = "X";
                        }
                        else if (patyAlterBhytADO.HEIN_MEDI_ORG_CODE == branch.HEIN_MEDI_ORG_CODE || (!string.IsNullOrWhiteSpace(branch.ACCEPT_HEIN_MEDI_ORG_CODE) && branch.ACCEPT_HEIN_MEDI_ORG_CODE.Contains(patyAlterBhytADO.HEIN_MEDI_ORG_CODE)))
                        {
                            RIGHT_ROUTE_TYPE_NAME = "X";
                        }
                        else if (branch.HEIN_LEVEL_CODE == MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.DISTRICT || branch.HEIN_LEVEL_CODE == MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.COMMUNE
                            )
                        {
                            if (province == branch.HEIN_PROVINCE_CODE && mediOrg != null && (mediOrg.LEVEL_CODE == MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.DISTRICT || mediOrg.LEVEL_CODE == MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.COMMUNE))
                            {
                                RIGHT_ROUTE_TYPE_NAME_TT = "X";
                            }
                            else
                            {
                                NOT_RIGHT_ROUTE_TYPE_NAME = "X";
                            }
                        }
                        else
                        {
                            RIGHT_ROUTE_TYPE_NAME = "X";
                        }
                    }
                    else if (patyAlterBhytADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        NOT_RIGHT_ROUTE_TYPE_NAME = "X";
                    }

                    SetSingleKey(new KeyValue("RIGHT_ROUTE_TYPE_NAME_CC", RIGHT_ROUTE_TYPE_NAME_CC));
                    SetSingleKey(new KeyValue("RIGHT_ROUTE_TYPE_NAME", RIGHT_ROUTE_TYPE_NAME));
                    SetSingleKey(new KeyValue("NOT_RIGHT_ROUTE_TYPE_NAME", NOT_RIGHT_ROUTE_TYPE_NAME));
                    SetSingleKey(new KeyValue("RIGHT_ROUTE_TYPE_NAME_TT", RIGHT_ROUTE_TYPE_NAME_TT));
                }
                else
                    SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.IS_NOT_HEIN, "X"));

                if (rdo.DepartmentTrans != null && rdo.DepartmentTrans.Count > 0)
                {
                    if (rdo.DepartmentTrans[0].DEPARTMENT_IN_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.DepartmentTrans[0].DEPARTMENT_IN_TIME ?? 0)));
                    }

                    if (rdo.DepartmentTrans[rdo.DepartmentTrans.Count - 1] != null && rdo.DepartmentTrans.Count > 1)
                    {
                        SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.DEPARTMENT_NAME_CLOSE_TREATMENT, rdo.DepartmentTrans[rdo.DepartmentTrans.Count - 1].DEPARTMENT_NAME));
                    }
                }

                if (rdo.Treatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.TREATMENT_CODE, rdo.Treatment.TREATMENT_CODE));
                    if (rdo.Treatment.CLINICAL_IN_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.CLINICAL_IN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.Treatment.CLINICAL_IN_TIME.Value)));
                    }

                    if (rdo.Treatment.OUT_TIME.HasValue)
                    {
                        SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.Treatment.OUT_TIME.Value)));
                    }
                }

                SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.TOTAL_DAY, rdo.SingleKeyValue.today));

                if (rdo.Treatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.TRAN_PATI_MEDI_ORG_CODE, rdo.Treatment.TRANSFER_IN_MEDI_ORG_CODE));
                    SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.TRAN_PATI_MEDI_ORG_NAME, rdo.Treatment.TRANSFER_IN_MEDI_ORG_NAME));
                }

                SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.CURRENT_DATE_SEPARATE_FULL_STR, rdo.SingleKeyValue.currentDateSeparateFullTime));

                if (!String.IsNullOrEmpty(rdo.SingleKeyValue.departmentName))
                {
                    SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.DEPARTMENT_NAME, rdo.SingleKeyValue.departmentName));
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
                    var sereServExamADOs = sereServADOs.Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__KH).OrderBy(o => o.CREATE_TIME).ToList();

                    if (sereServExamADOs != null && sereServExamADOs.Count > 0)
                    {
                        executeRoomExamFirst = sereServExamADOs[0].EXECUTE_ROOM_NAME;
                        executeRoomExamLast = sereServExamADOs[sereServExamADOs.Count - 1].EXECUTE_ROOM_NAME;
                        foreach (var sereServExamADO in sereServExamADOs)
                        {
                            executeRoomExam += sereServExamADO.EXECUTE_ROOM_NAME + ", ";
                        }
                    }

                    SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.EXECUTE_ROOM_EXAM, executeRoomExam));
                    SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.FIRST_EXAM_ROOM_NAME, executeRoomExamFirst));
                    SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.LAST_EXAM_ROOM_NAME, executeRoomExamLast));

                    thanhtien_tong = sereServInPackageOutFeeADOs.Sum(o => o.VIR_TOTAL_PRICE ?? 0)
                        + sereServPTTTADOs.Sum(o => o.VIR_TOTAL_PRICE ?? 0)
                        + sereServADOs.Sum(o => o.VIR_TOTAL_PRICE ?? 0);
                    bhytthanhtoan_tong = sereServInPackageOutFeeADOs.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0)
                        + sereServPTTTADOs.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0)
                        + sereServADOs.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);
                    bnthanhtoan_tong = sereServInPackageOutFeeADOs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE ?? 0)
                        + sereServPTTTADOs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE ?? 0)
                        + sereServADOs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE ?? 0);
                    nguonkhac_tong = sereServInPackageOutFeeADOs.Sum(o => o.OTHER_SOURCE_PRICE ?? 0)
                        + sereServPTTTADOs.Sum(o => o.OTHER_SOURCE_PRICE ?? 0)
                        + sereServADOs.Sum(o => o.OTHER_SOURCE_PRICE ?? 0);
                }

                SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong, 0)));
                SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.TOTAL_PRICE_OTHER, Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguonkhac_tong, 0)));
                SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.TOTAL_PRICE_PATIENT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.TOTAL_PRICE_OTHER_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(nguonkhac_tong).ToString())));

                if (rdo.TreatmentFees != null)
                {
                    SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(rdo.TreatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0, 0)));
                }
                else
                {
                    SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, "0"));
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
                decimal hoanUngSauThanhToan = 0;
                decimal hoanUngReason5NoiTru = 0;
                decimal hoanUngReason4NoiTru = 0;
                decimal tamUngNoiTru = 0;

                tongChiPhiBHYT = sereServADOs.Where(o => o.PATIENT_TYPE_ID == rdo.PatientTypeCFG.PATIENT_TYPE__BHYT).Sum(o => o.TOTAL_PRICE_BHYT);
                tienBHYTThanhToan = sereServADOs.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;

                nguoiBenhCungChiTra = sereServADOs.Where(o => o.PATIENT_TYPE_ID == rdo.PatientTypeCFG.PATIENT_TYPE__BHYT)
                    .Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0); //tongChiPhiBHYT - tienBHYTThanhToan;

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("input: SereServDeposit", rdo.SereServDeposit));

                if (rdo.SereServDeposit != null && rdo.SereServDeposit.Count > 0)
                {
                    rdo.SereServDeposit = rdo.SereServDeposit.Where(o => o.IS_CANCEL == 0 || o.IS_CANCEL == null).ToList();
                    tamUngDV = rdo.SereServDeposit.Sum(o => o.AMOUNT);
                    if (rdo.SeseDepoRepay != null && rdo.SeseDepoRepay.Count > 0)
                    {
                        rdo.SeseDepoRepay = rdo.SeseDepoRepay.Where(o => o.IS_CANCEL == 0 || o.IS_CANCEL == null).ToList();
                        List<long> sereServIdRepays = rdo.SeseDepoRepay.Select(o => o.SERE_SERV_DEPOSIT_ID).ToList();
                        tamUngDVChuaHoanThu = rdo.SereServDeposit.Where(o => !sereServIdRepays.Contains(o.SERE_SERV_ID)).Sum(o => o.AMOUNT);
                        hoanUngDV = rdo.SeseDepoRepay.Sum(o => o.AMOUNT);
                    }
                }

                tongThuBenhNhan = (sereServInPackageOutFeeADOs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE)
                    + sereServPTTTADOs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE)
                    + sereServADOs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE)) ?? 0;

                ngoaiDMChenhLech = tongThuBenhNhan - nguoiBenhCungChiTra;

                //ngoaiDMChenhLech + nguoiBenhCungChiTra;
                if (rdo.TreatmentFees != null)
                {
                    mienGiam = rdo.TreatmentFees[0].TOTAL_BILL_EXEMPTION ?? 0;
                    ketChuyen = rdo.TreatmentFees[0].TOTAL_BILL_TRANSFER_AMOUNT ?? 0;
                    cacQuyThanhToanToan = rdo.TreatmentFees[0].TOTAL_BILL_FUND ?? 0;
                    thanhToan = rdo.TreatmentFees[0].TOTAL_BILL_AMOUNT ?? 0;
                    tamUng = rdo.TreatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0;
                    hoanUng = rdo.TreatmentFees[0].TOTAL_REPAY_AMOUNT ?? 0;
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo.TreatmentFees), rdo.TreatmentFees));

                //Thanh toan cuoi cung
                HIS_TRANSACTION bill = rdo.Bills != null ? rdo.Bills.Where(o => o.TRANSACTION_TYPE_ID == rdo.transactionTypeCFG.TRANSACTION_TYPE_ID__BILL).OrderByDescending(o => o.TRANSACTION_TIME).FirstOrDefault() : null;
                if (bill != null)
                {
                    SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.CASHIER_LOGINNAME, bill.CASHIER_LOGINNAME));
                    SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.TRANSACTION_TIME, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(bill.CREATE_TIME.Value)));
                    SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.CASHIER_USERNAME, bill.CASHIER_USERNAME));

                    //Hoan ung sau thanh toan
                    List<HIS_TRANSACTION> repayAfterBills = rdo.Bills.Where(o => o.TRANSACTION_TYPE_ID == rdo.transactionTypeCFG.TRANSACTION_TYPE_ID__REPAY
                        && o.TRANSACTION_TIME >= bill.TRANSACTION_TIME).ToList();
                    if (repayAfterBills != null && repayAfterBills.Count > 0)
                    {
                        hoanUngSauThanhToan = repayAfterBills.Sum(o => o.AMOUNT);
                    }
                }

                List<HIS_TRANSACTION> transactionRepay5s = rdo.Bills != null ? rdo.Bills.Where(o => o.TRANSACTION_TYPE_ID == rdo.transactionTypeCFG.TRANSACTION_TYPE_ID__REPAY && o.REPAY_REASON_ID == 5 && o.IS_CANCEL != 1 && o.TDL_SESE_DEPO_REPAY_COUNT == null).ToList() : null;
                if (transactionRepay5s != null && transactionRepay5s.Count > 0)
                {
                    hoanUngReason5NoiTru = transactionRepay5s.Sum(o => o.AMOUNT);
                }

                List<HIS_TRANSACTION> transactionRepay4s = rdo.Bills != null ? rdo.Bills.Where(o => o.TRANSACTION_TYPE_ID == rdo.transactionTypeCFG.TRANSACTION_TYPE_ID__REPAY && o.REPAY_REASON_ID == 4 && o.IS_CANCEL != 1 && o.TDL_SESE_DEPO_REPAY_COUNT == null).ToList() : null;
                if (transactionRepay4s != null && transactionRepay4s.Count > 0)
                {
                    hoanUngReason4NoiTru = transactionRepay4s.Sum(o => o.AMOUNT);
                }

                List<HIS_TRANSACTION> transactionDeposits = rdo.Bills != null ? rdo.Bills.Where(o => o.TRANSACTION_TYPE_ID == rdo.transactionTypeCFG.TRANSACTION_TYPE_ID__DEPOSIT && o.IS_CANCEL != 1 && o.TDL_SERE_SERV_DEPOSIT_COUNT == null).ToList() : null;
                if (transactionDeposits != null && transactionDeposits.Count > 0)
                {
                    tamUngNoiTru = transactionDeposits.Sum(o => o.AMOUNT);
                }

                bnDaThanhToan = tamUngDV - hoanUngDV - hoanUngReason5NoiTru;
                bnTamUng = tamUngNoiTru - hoanUngReason4NoiTru;

                bnPhaiNop = tongThuBenhNhan - bnDaThanhToan - bnTamUng - cacQuyThanhToanToan;// +ketChuyen + hoanUngDV + hoanUngSauThanhToan;
                bnNhanLai = -bnPhaiNop;

                if (bnPhaiNop > 0)
                {
                    SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.SO_TIEN_BN_PHAI_NOP, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnPhaiNop, 0)));
                    SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.SO_TIEN_BN_PHAI_NOP_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnPhaiNop).ToString())));
                }

                if (bnNhanLai > 0)
                {
                    SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.SO_TIEN_BN_NHAN_LAI, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnNhanLai, 0)));
                    SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.SO_TIEN_BN_NHAN_LAI_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnNhanLai).ToString())));
                }

                SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.TONG_CP_BHYT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(tongChiPhiBHYT, 0)));
                SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.TIEN_BHYT_THANH_TOAN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(tienBHYTThanhToan, 0)));
                SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.TIEN_BN_CUNG_CHI_TRA, Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguoiBenhCungChiTra, 0)));
                SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.TIEN_NGOAI_DM_VA_CHENH_LECH, Inventec.Common.Number.Convert.NumberToStringRoundAuto(ngoaiDMChenhLech, 0)));
                SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.TONG_THU_BN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(tongThuBenhNhan, 0)));
                SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.BN_DA_THANH_TOAN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnDaThanhToan, 0)));
                SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.BN_DA_THANH_TOAN2, Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhToan, 0)));
                SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.BN_DA_TAM_UNG, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnTamUng, 0)));
                SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.BN_HOAN_UNG, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnHoanUng, 0)));
                SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.CAC_QUY_THANH_TOAN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(cacQuyThanhToanToan, 0)));

                SetSingleKey(new KeyValue(Mps000312ExtendSingleKey.CREATOR_USERNAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));

                AddObjectKeyIntoListkey<PatientADO>(patientADO);
                AddObjectKeyIntoListkey<PatyAlterBhytADO>(patyAlterBHYTADO);
                AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.Treatment, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.Treatment.TREATMENT_CODE);
                barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatment.IncludeLabel = false;
                barcodeTreatment.Width = 120;
                barcodeTreatment.Height = 40;
                barcodeTreatment.RotateFlipType = System.Drawing.RotateFlipType.Rotate180FlipXY;
                barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatment.IncludeLabel = true;

                dicImage.Add(Mps000312ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
