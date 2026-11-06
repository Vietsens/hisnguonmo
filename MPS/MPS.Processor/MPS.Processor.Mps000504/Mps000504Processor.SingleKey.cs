using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000504.ADO;
using MPS.Processor.Mps000504.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MPS.Processor.Mps000504
{
    // 56810: single keys copied from Mps000512.ProcessSingleKey - same data as 512, only the services are
    // limited to the time filter. Totals are summed from the filtered sereServADOs.
    public partial class Mps000504Processor
    {
        private void SetSingleKeyAsMps000512(decimal totalSurcharge)
        {
            try
            {
                V_HIS_TREATMENT treatment = rdo.TreatmentView;
                SingleKeyValue singleKeyValue = rdo.SingleKeyValue ?? new SingleKeyValue();

                if (rdo.ServiceReqs != null)
                {
                    HIS_SERVICE_REQ firstExam = rdo.ServiceReqs
                        .Where(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH && o.START_TIME != null)
                        .OrderBy(o => o.START_TIME ?? 0).FirstOrDefault();
                    if (firstExam != null)
                        SetSingleKey(new KeyValue("SERVICE_REQ_START_TIME_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(firstExam.START_TIME ?? 0)));
                }
                SetSingleKey(new KeyValue("RATIO_STR", singleKeyValue.RATIO_STR));
                SetSingleKey(new KeyValue("USERNAME_RETURN_RESULT", singleKeyValue.USERNAME_RETURN_RESULT));
                SetSingleKey(new KeyValue("STATUS_TREATMENT_OUT", singleKeyValue.STATUS_TREATMENT_OUT));
                if (treatment != null)
                {
                    SetSingleKey(new KeyValue("TREATMENT_CODE", treatment.TREATMENT_CODE));
                    if (treatment.IN_TIME > 0)
                        SetSingleKey(new KeyValue("IN_TIME_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(treatment.IN_TIME)));
                }

                if (rdo.CurrentPatyAlter != null)
                {
                    int? typeIndex = null;
                    string typeName = "";
                    HIS_TREATMENT_TYPE treatmentType = rdo.TreatmentTypes != null ? rdo.TreatmentTypes.FirstOrDefault(o => o.ID == rdo.CurrentPatyAlter.TREATMENT_TYPE_ID) : null;
                    if (treatmentType != null)
                    {
                        typeIndex = (int)treatmentType.ID;
                        typeName = (treatmentType.TREATMENT_TYPE_NAME ?? "").ToUpper();
                    }
                    else
                    {
                        switch (rdo.CurrentPatyAlter.TREATMENT_TYPE_ID)
                        {
                            case IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM:
                                typeIndex = 1;
                                typeName = "KHÁM BỆNH";
                                break;
                            case IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU:
                                typeIndex = 2;
                                typeName = "ĐIỀU TRỊ NGOẠI TRÚ";
                                break;
                            case IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU:
                                typeIndex = 3;
                                typeName = "ĐIỀU TRỊ NỘI TRÚ";
                                break;
                            case IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__NHANTHUOC:
                                typeIndex = 6;
                                typeName = "NHẬN THUỐC THEO HẸN (KHÔNG KHÁM BỆNH)";
                                break;
                            case IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__TYTXA:
                                typeIndex = 5;
                                typeName = "ĐIỀU TRỊ LƯU TẠI TYT XÃ, PKĐKKV";
                                break;
                            default:
                                typeName = "(KHÔNG XÁC ĐỊNH ĐƯỢC ĐỐI TƯỢNG)";
                                break;
                        }
                    }
                    SetSingleKey(new KeyValue("TYPE_INDEX", typeIndex));
                    SetSingleKey(new KeyValue("TYPE_NAME", typeName));
                    if (rdo.CurrentPatyAlter.FREE_CO_PAID_TIME.HasValue)
                        SetSingleKey(new KeyValue("FREE_CO_PAID_TIME_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.CurrentPatyAlter.FREE_CO_PAID_TIME.Value)));
                }

                SetRightRouteKeys(treatment);

                if (rdo.DepartmentTrans != null && rdo.DepartmentTrans.Count > 0)
                {
                    if (rdo.DepartmentTrans[0].DEPARTMENT_IN_TIME.HasValue)
                        SetSingleKey(new KeyValue("OPEN_TIME_SEPARATE_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.DepartmentTrans[0].DEPARTMENT_IN_TIME ?? 0)));
                    if (rdo.DepartmentTrans.Count > 1 && rdo.DepartmentTrans[rdo.DepartmentTrans.Count - 1] != null)
                        SetSingleKey(new KeyValue("DEPARTMENT_NAME_CLOSE_TREATMENT", rdo.DepartmentTrans[rdo.DepartmentTrans.Count - 1].DEPARTMENT_NAME));
                }

                if (!String.IsNullOrEmpty(singleKeyValue.roomName))
                    SetSingleKey(new KeyValue("ROOM_NAME", singleKeyValue.roomName));

                if (treatment != null)
                    SetTreatmentKeys(treatment);

                SetSingleKey(new KeyValue("TOTAL_DAY", singleKeyValue.TOTAL_DAY));
                SetSingleKey(new KeyValue("CURRENT_DATE_SEPARATE_FULL_STR", singleKeyValue.CURRENT_DATE_SEPARATE_FULL_STR));

                if (rdo.PatientTypeAlterAlls != null && rdo.CurrentPatyAlter != null)
                {
                    HIS_PATIENT_TYPE_ALTER currentAlter = rdo.PatientTypeAlterAlls.FirstOrDefault(o => o.ID == rdo.CurrentPatyAlter.ID);
                    if (currentAlter != null)
                        SetSingleKey(new KeyValue("CO_PAID_ACCUMULATE_AMOUNT", currentAlter.CO_PAID_ACCUMULATE_AMOUNT));
                }

                SetTotalKeys(totalSurcharge);

                if (!String.IsNullOrEmpty(singleKeyValue.departmentName))
                    SetSingleKey(new KeyValue("DEPARTMENT_NAME", singleKeyValue.departmentName));

                SetDepositKeys();

                SetSingleKey(new KeyValue("CREATOR_USERNAME", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));

                if (patientADO != null)
                    AddObjectKeyIntoListkey<PatientADO>(patientADO);
                if (treatment != null)
                    AddObjectKeyIntoListkey<V_HIS_TREATMENT>(treatment, false);

                if (rdo.CurrentPatyAlter != null && treatment != null)
                {
                    AddObjectKeyIntoListkey<PatyAlterBhytADO>(DataRawProcess.PatyAlterBHYTRawToADO(rdo.CurrentPatyAlter, rdo.Branch, rdo.TreatmentTypes, treatment), false);
                    if (rdo.CurrentPatyAlter.HEIN_CARD_FROM_TIME.HasValue)
                        SetSingleKey(new KeyValue(Mps000504ExtendSingleKey.STR_HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.CurrentPatyAlter.HEIN_CARD_FROM_TIME ?? 0)));
                    if (rdo.CurrentPatyAlter.HEIN_CARD_TO_TIME.HasValue)
                        SetSingleKey(new KeyValue(Mps000504ExtendSingleKey.STR_HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.CurrentPatyAlter.HEIN_CARD_TO_TIME ?? 0)));
                    if (rdo.CurrentPatyAlter.JOIN_5_YEAR_TIME.HasValue)
                        SetSingleKey(new KeyValue("JOIN_5_YEAR_TIME_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.CurrentPatyAlter.JOIN_5_YEAR_TIME ?? 0)));
                    SetSingleKey(new KeyValue("RATIO_STR", DataRawProcess.GetDefaultHeinRatioForView(rdo.CurrentPatyAlter.HEIN_CARD_NUMBER, rdo.CurrentPatyAlter.HEIN_TREATMENT_TYPE_CODE, rdo.CurrentPatyAlter.LEVEL_CODE, rdo.CurrentPatyAlter.RIGHT_ROUTE_CODE, rdo.CurrentPatyAlter.FACILITY_CLASS, rdo.CurrentPatyAlter.FORMER_LEVEL_CODE, (long)(rdo.CurrentPatyAlter.CLASSIFY_POINT ?? 0), treatment.CLINICAL_IN_TIME ?? 0)));
                    SetSingleKey(new KeyValue("LIVE_AREA_CODE", rdo.CurrentPatyAlter.LIVE_AREA_CODE));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetRightRouteKeys(V_HIS_TREATMENT treatment)
        {
            try
            {
                // Always registered (also for non-BHYT patients) so the template never hits "Report Variable ... is not defined"
                string rightRouteCC = "";
                string rightRoute = "";
                string notRightRoute = "";
                string rightRouteTT = "";

                PatyAlterBhytADO patyAlterBhytADO = null;
                if (patyAlterBHYTADOs != null && patyAlterBHYTADOs.Count > 0)
                {
                    SetSingleKey(new KeyValue("HEIN_MEDI_ORG_CODE", string.Join(";", patyAlterBHYTADOs.Select(s => s.HEIN_MEDI_ORG_CODE).Distinct().OrderBy(o => o))));
                    SetSingleKey(new KeyValue("HEIN_MEDI_ORG_NAME", string.Join(";", patyAlterBHYTADOs.Select(s => s.HEIN_MEDI_ORG_NAME).Distinct().OrderBy(o => o))));

                    string keyPaty = "";
                    HIS_SERE_SERV check = rdo.SereServs != null ? rdo.SereServs.OrderByDescending(o => o.ID).FirstOrDefault(o => !String.IsNullOrEmpty(o.HEIN_CARD_NUMBER) && o.IS_EXPEND != 1 && o.IS_NO_EXECUTE != 1) : null;
                    if (check != null && treatment != null)
                        PatientTypeAlterProcessor.GetPatientTypeAlter(check, rdo.PatientTypeCFG, treatment.TDL_TREATMENT_TYPE_ID ?? 0, ref keyPaty);

                    patyAlterBhytADO = patyAlterBHYTADOs.OrderByDescending(o => o.LOG_TIME).FirstOrDefault(o => !String.IsNullOrEmpty(o.HEIN_CARD_NUMBER) && (String.IsNullOrWhiteSpace(keyPaty) || o.KEY == keyPaty));
                }

                if (patyAlterBhytADO != null)
                {
                    HIS_BRANCH branch = rdo.Branch ?? new HIS_BRANCH();
                    string province = !String.IsNullOrWhiteSpace(patyAlterBhytADO.HEIN_MEDI_ORG_CODE) && patyAlterBhytADO.HEIN_MEDI_ORG_CODE.Length >= 2 ? patyAlterBhytADO.HEIN_MEDI_ORG_CODE.Substring(0, 2) : "";
                    HIS_MEDI_ORG mediOrg = rdo.ListMediOrg != null ? rdo.ListMediOrg.FirstOrDefault(o => o.MEDI_ORG_CODE == patyAlterBhytADO.HEIN_MEDI_ORG_CODE) : null;

                    SetSingleKey(new KeyValue("IS_HEIN", "X"));
                    SetSingleKey(new KeyValue("HEIN_CARD_ADDRESS", patyAlterBhytADO.ADDRESS));

                    if (patyAlterBhytADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)
                            rightRouteCC = "X";
                        else if (patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.OVER)
                            rightRouteTT = "X";
                        else if (patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)
                            rightRoute = "X";
                        else if (patyAlterBhytADO.HEIN_MEDI_ORG_CODE == branch.HEIN_MEDI_ORG_CODE || (!string.IsNullOrWhiteSpace(branch.ACCEPT_HEIN_MEDI_ORG_CODE) && branch.ACCEPT_HEIN_MEDI_ORG_CODE.Contains(patyAlterBhytADO.HEIN_MEDI_ORG_CODE ?? "")))
                            rightRoute = "X";
                        else if (branch.HEIN_LEVEL_CODE == MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.DISTRICT || branch.HEIN_LEVEL_CODE == MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.COMMUNE)
                        {
                            if (province == branch.HEIN_PROVINCE_CODE && mediOrg != null && (mediOrg.LEVEL_CODE == MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.DISTRICT || mediOrg.LEVEL_CODE == MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.COMMUNE))
                                rightRouteTT = "X";
                            else
                                notRightRoute = "X";
                        }
                        else
                        {
                            rightRoute = "X";
                            if (treatment != null && rdo.CurrentPatyAlter != null && treatment.MEDI_ORG_CODE != rdo.CurrentPatyAlter.HEIN_MEDI_ORG_CODE)
                                SetSingleKey(new KeyValue("THONG_TUYEN", "X"));
                        }
                    }
                    else if (patyAlterBhytADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)
                    {
                        notRightRoute = "X";
                    }
                }
                else
                {
                    SetSingleKey(new KeyValue("IS_NOT_HEIN", "X"));
                }

                SetSingleKey(new KeyValue("RIGHT_ROUTE_TYPE_NAME_CC", rightRouteCC));
                SetSingleKey(new KeyValue("RIGHT_ROUTE_TYPE_NAME", rightRoute));
                SetSingleKey(new KeyValue("NOT_RIGHT_ROUTE_TYPE_NAME", notRightRoute));
                SetSingleKey(new KeyValue("RIGHT_ROUTE_TYPE_NAME_TT", rightRouteTT));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetTreatmentKeys(V_HIS_TREATMENT treatment)
        {
            try
            {
                if (treatment.CLINICAL_IN_TIME.HasValue)
                    SetSingleKey(new KeyValue("CLINICAL_IN_TIME_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(treatment.CLINICAL_IN_TIME.Value)));
                if (treatment.OUT_TIME.HasValue)
                    SetSingleKey(new KeyValue("CLOSE_TIME_SEPARATE_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(treatment.OUT_TIME.Value)));

                if (treatment.END_DEPARTMENT_ID.HasValue && rdo.Departments != null)
                {
                    HIS_DEPARTMENT department = rdo.Departments.FirstOrDefault(o => o.ID == treatment.END_DEPARTMENT_ID.Value);
                    if (department != null)
                    {
                        SetSingleKey(new KeyValue("DEPARTMENT_BHYT_CODE", department.BHYT_CODE));
                        SetSingleKey(new KeyValue("END_DEPARTMENT_CODE", department.DEPARTMENT_CODE));
                        SetSingleKey(new KeyValue("END_DEPARTMENT_NAME", department.DEPARTMENT_NAME));
                    }
                }

                int genderIndex;
                string genderName;
                switch (treatment.TDL_PATIENT_GENDER_ID)
                {
                    case IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE:
                        genderIndex = 1;
                        genderName = treatment.TDL_PATIENT_GENDER_NAME;
                        break;
                    case IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE:
                        genderIndex = 2;
                        genderName = treatment.TDL_PATIENT_GENDER_NAME;
                        break;
                    default:
                        genderIndex = 3;
                        genderName = "Không xác định";
                        break;
                }
                SetSingleKey(new KeyValue("GENDER_INDEX", genderIndex));
                SetSingleKey(new KeyValue(Mps000504ExtendSingleKey.GENDER_NAME, genderName));

                SetSingleKey(new KeyValue("TRAN_PATI_MEDI_ORG_CODE", treatment.TRANSFER_IN_MEDI_ORG_CODE));
                SetSingleKey(new KeyValue("TRAN_PATI_MEDI_ORG_NAME", treatment.TRANSFER_IN_MEDI_ORG_NAME));

                if (treatment.TREATMENT_RESULT_ID.HasValue)
                {
                    int treatmentResultIndex = 2;
                    if (treatment.TREATMENT_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__DO
                        || treatment.TREATMENT_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__KHOI
                        || treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN
                        || treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN
                        || treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CTCV)
                    {
                        treatmentResultIndex = 1;
                        if ((treatment.TREATMENT_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__DO
                            || treatment.TREATMENT_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__KHOI)
                            && treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN)
                        {
                            treatmentResultIndex = 2;
                        }
                    }
                    SetSingleKey(new KeyValue("TREATMENT_RESULT_INDEX", treatmentResultIndex));
                }

                if (treatment.OUT_TIME.HasValue)
                {
                    long inTime = treatment.CLINICAL_IN_TIME.HasValue ? treatment.CLINICAL_IN_TIME.Value : treatment.IN_TIME;
                    System.DateTime? dateBefore = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(inTime);
                    System.DateTime? dateAfter = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(treatment.OUT_TIME.Value);
                    if (dateBefore != null && dateAfter != null)
                    {
                        TimeSpan difference = dateAfter.Value - dateBefore.Value;
                        // over 24h: out day - in day + 1
                        if (difference.Days > 1 || (difference.Days == 1 && (difference.Hours >= 1 || difference.Minutes >= 1 || difference.Seconds >= 1)))
                        {
                            if (treatment.TDL_TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                            {
                                int diffDay = (int)(dateAfter.Value.Date - dateBefore.Value.Date).TotalDays + 1;
                                SetSingleKey(new KeyValue("TREATMENT_DAY_COUNT_6556", diffDay));
                            }
                        }
                        else if (treatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                        {
                            SetSingleKey(new KeyValue("TREATMENT_DAY_COUNT_6556", 1));
                        }
                    }
                }

                if (rdo.ListTransactionBill != null && rdo.ListTransactionBill.Count > 0 && treatment.IS_PAUSE == 1)
                {
                    List<HIS_TRANSACTION> validTrans = rdo.ListTransactionBill.Where(o => o.IS_DIRECTLY_BILLING != 1 && o.IS_CANCEL != 1).ToList();
                    HIS_TRANSACTION lastTran = validTrans.OrderByDescending(o => o.TRANSACTION_TIME).FirstOrDefault();
                    if (lastTran != null)
                    {
                        if (lastTran.TREATMENT_DEPOSIT_AMOUNT.HasValue)
                            SetSingleKey(new KeyValue("TRANSACTION_TREATMENT_DEPOSIT_AMOUNT", Inventec.Common.Number.Convert.NumberToStringRoundAuto(lastTran.TREATMENT_DEPOSIT_AMOUNT ?? 0, 0)));
                        if (lastTran.TREATMENT_REPAY_AMOUNT.HasValue)
                            SetSingleKey(new KeyValue("TRANSACTION_TREATMENT_REPAY_AMOUNT", Inventec.Common.Number.Convert.NumberToStringRoundAuto(lastTran.TREATMENT_REPAY_AMOUNT ?? 0, 0)));
                        if (lastTran.TREATMENT_PATIENT_PRICE.HasValue)
                            SetSingleKey(new KeyValue("TRANSACTION_TREATMENT_PATIENT_PRICE", Inventec.Common.Number.Convert.NumberToStringRoundAuto(lastTran.TREATMENT_PATIENT_PRICE ?? 0, 0)));
                        if (lastTran.TREATMENT_BILL_AMOUNT.HasValue)
                            SetSingleKey(new KeyValue("TRANSACTION_TREATMENT_BILL_AMOUNT", Inventec.Common.Number.Convert.NumberToStringRoundAuto(lastTran.TREATMENT_BILL_AMOUNT ?? 0, 0)));

                        HIS_TRANSACTION lastBill = validTrans.Where(o => o.SALE_TYPE_ID == null).OrderByDescending(o => o.TRANSACTION_TIME).FirstOrDefault();
                        SetSingleKey(new KeyValue("LAST_TRANSACTION_AMOUNT", Inventec.Common.Number.Convert.NumberToStringRoundAuto(lastBill != null ? lastBill.AMOUNT : 0, 0)));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetTotalKeys(decimal totalSurcharge)
        {
            try
            {
                List<SereServADO> services = sereServADOs ?? new List<SereServADO>();

                List<SereServADO> exams = (rdo.HeinServiceTypeCFG != null)
                    ? services.Where(o => o.HEIN_SERVICE_TYPE_ID == rdo.HeinServiceTypeCFG.HEIN_SERVICE_TYPE__EXAM_ID).OrderBy(o => o.CREATE_TIME).ToList()
                    : new List<SereServADO>();
                SetSingleKey(new KeyValue("EXECUTE_ROOM_EXAM", string.Concat(exams.Select(o => o.EXECUTE_ROOM_NAME + ", "))));
                SetSingleKey(new KeyValue("FIRST_EXAM_ROOM_NAME", exams.Count > 0 ? exams[0].EXECUTE_ROOM_NAME : ""));
                SetSingleKey(new KeyValue("LAST_EXAM_ROOM_NAME", exams.Count > 0 ? exams[exams.Count - 1].EXECUTE_ROOM_NAME : ""));

                decimal thanhtienBH_tong = services.Sum(o => o.TOTAL_PRICE_BHYT);
                decimal thanhtien_tong = services.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0) + totalSurcharge;
                decimal bhytthanhtoan_tong = services.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);
                decimal bnthanhtoan_tong = services.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0);
                decimal nguonkhac_tong = services.Sum(o => o.OTHER_SOURCE_PRICE ?? 0);
                decimal tongtienbenhnhantutra = services.Sum(o => o.TOTAL_PRICE_PATIENT_SELF) + totalSurcharge;
                decimal thanhtien_tong_new = services.Sum(o => o.TOTAL_PRICE_VP);
                decimal tongtienbenhnhantutra_new = services.Sum(o => o.TOTAL_PATIENT_PRICE_LEFT);
                decimal tongTienBenhNhan = services.Sum(o => o.VIR_TOTAL_PATIENT_PRICE ?? 0);
                long totalNumberFilm = (long)services.Sum(o => (o.NUMBER_OF_FILM ?? 0) * o.AMOUNT);

                // 512 keeps separate lists for these (same rows, only grouped differently) - sums are equal
                List<SereServADO> noExamZero = services.Where(o => o.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH || o.VIR_PRICE > 0).ToList();
                decimal thanhtien_tong_NoExamZero = noExamZero.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0);
                decimal tongtienbenhnhantutra_NoExamZero = noExamZero.Sum(o => o.TOTAL_PRICE_PATIENT_SELF);
                decimal tongTienSuatAn = services.Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN).Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0);

                SetSingleKey(new KeyValue("TOTAL_NUMBER_FILM", totalNumberFilm > 0 ? (object)totalNumberFilm : ""));
                SetSingleKey(new KeyValue("TOTAL_NUMBER_FILM_STR", totalNumberFilm > 0 ? String.Format("Bệnh nhân đã nhận đủ số phim . Số phim {0}", totalNumberFilm) : ""));

                SetSingleKey(new KeyValue("SURCHARGE_SECTION_NO", (heinServiceTypeADOs != null ? heinServiceTypeADOs.Count : 0) + 1));
                SetSingleKey(new KeyValue("TOTAL_PRICE", Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong, 0)));
                SetSingleKey(new KeyValue("TOTAL_PRICE_BHYT", Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtienBH_tong, 0)));
                SetSingleKey(new KeyValue("TOTAL_PRICE_HEIN", Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue("TOTAL_PRICE_PATIENT", Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong, 0)));
                SetSingleKey(new KeyValue("TOTAL_PRICE_PATIENT_SELF", Inventec.Common.Number.Convert.NumberToStringRoundAuto(tongtienbenhnhantutra, 0)));
                SetSingleKey(new KeyValue("TOTAL_PRICE_OTHER", Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguonkhac_tong, 0)));
                SetSingleKey(new KeyValue("TOTAL_PRICE_TEXT", Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong).ToString())));
                SetSingleKey(new KeyValue("TOTAL_PRICE_HEIN_TEXT", Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue("TOTAL_PRICE_PATIENT_TEXT", Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong).ToString())));
                SetSingleKey(new KeyValue("TOTAL_PRICE_OTHER_TEXT", Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(nguonkhac_tong).ToString())));
                SetSingleKey(new KeyValue("TOTAL_PRICE_PATIENT_VIR", Inventec.Common.Number.Convert.NumberToStringRoundAuto(tongTienBenhNhan, 0)));
                SetSingleKey(new KeyValue("TOTAL_PRICE_PATIENT_VIR_TEXT", Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(tongTienBenhNhan).ToString())));
                SetSingleKey(new KeyValue("TOTAL_SA", Inventec.Common.Number.Convert.NumberToStringRoundAuto(tongTienSuatAn, 0)));

                SetSingleKey(new KeyValue("TOTAL_PRICE_NEW", Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong_new, 0)));
                SetSingleKey(new KeyValue("TOTAL_PATIENT_PRICE_LEFT", Inventec.Common.Number.Convert.NumberToStringRoundAuto(tongtienbenhnhantutra_new, 0)));
                SetSingleKey(new KeyValue("TOTAL_PRICE_NEW_TEXT", Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong_new).ToString())));
                // not in 512 - hospital template wraps it in FlFuncSpeechNumberToString, so it is a number (as Mps000508)
                SetSingleKey(new KeyValue("TOTAL_PATIENT_PRICE_LEFT_TEXT", Inventec.Common.Number.Convert.NumberToStringRoundAuto(tongtienbenhnhantutra_new, 2)));

                SetSingleKey(new KeyValue("TOTAL_PRICE_NO_EXAM_ZERO", Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong_NoExamZero, 0)));
                SetSingleKey(new KeyValue("TOTAL_PRICE_NO_EXAM_ZERO_TEXT", Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong_NoExamZero).ToString())));
                SetSingleKey(new KeyValue("TOTAL_PATIENT_PRICE_LEFT_NO_EXAM_ZERO", Inventec.Common.Number.Convert.NumberToStringRoundAuto(tongtienbenhnhantutra_NoExamZero, 0)));

                // 512 takes these two from V_HIS_TREATMENT_FEE (whole treatment); here they follow the time filter
                SetSingleKey(new KeyValue("TOTAL_HEIN_PRICE", bhytthanhtoan_tong));
                SetSingleKey(new KeyValue("TOTAL_PATIENT_PRICE_BHYT", bnthanhtoan_tong));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDepositKeys()
        {
            try
            {
                decimal depositService = rdo.ListSereServDeposits != null ? rdo.ListSereServDeposits.Where(o => o.IS_CANCEL != 1).Sum(s => s.AMOUNT) : 0;
                decimal depositOther = 0;

                V_HIS_TREATMENT_FEE fee = rdo.TreatmentFees != null ? rdo.TreatmentFees.FirstOrDefault() : null;
                if (fee != null)
                {
                    decimal totalDeposit = fee.TOTAL_DEPOSIT_AMOUNT ?? 0;
                    decimal totalPatientPrice = fee.TOTAL_PATIENT_PRICE ?? 0;
                    decimal totalObtainedPrice = totalDeposit + (fee.TOTAL_BILL_AMOUNT ?? 0) - (fee.TOTAL_BILL_TRANSFER_AMOUNT ?? 0) - (fee.TOTAL_REPAY_AMOUNT ?? 0);

                    SetSingleKey(new KeyValue("TOTAL_DEPOSIT_AMOUNT", Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalDeposit, 0)));
                    SetSingleKey(new KeyValue("TREATMENT_FEE_TOTAL_PRICE", Inventec.Common.Number.Convert.NumberToStringRoundAuto(fee.TOTAL_PRICE ?? 0, 0)));
                    SetSingleKey(new KeyValue("TREATMENT_FEE_TOTAL_PATIENT_PRICE", Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalPatientPrice, 0)));
                    SetSingleKey(new KeyValue("TREATMENT_FEE_TOTAL_OBTAINED_PRICE", Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalObtainedPrice, 0)));
                    SetSingleKey(new KeyValue("TREATMENT_FEE_TRANSFER", Inventec.Common.Number.Convert.NumberToStringRoundAuto(totalPatientPrice - totalObtainedPrice, 0)));
                    AddObjectKeyIntoListkey<V_HIS_TREATMENT_FEE>(fee, false);

                    depositOther = totalDeposit - depositService;
                }
                else
                {
                    SetSingleKey(new KeyValue("TOTAL_DEPOSIT_AMOUNT", 0));
                }

                SetSingleKey(new KeyValue("TOTAL_DEPOSIT_SERVICE_AMOUNT", depositService));
                SetSingleKey(new KeyValue("TOTAL_DEPOSIT_OTHER_AMOUNT", depositOther));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
