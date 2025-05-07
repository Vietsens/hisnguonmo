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
using HIS.Desktop.ApiConsumer;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using LIS.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.Processor.Mps000096.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000096
{
    public class Mps000096Processor : AbstractProcessor
    {
        Mps000096PDO rdo;
        public Mps000096Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000096PDO)rdoBase;
        }
        NumberStyles style = NumberStyles.Any;
        List<TestLisResultADO> ListTestParent = new List<TestLisResultADO>();
        List<TestLisResultADO> ListTestChild = new List<TestLisResultADO>();
        List<V_HIS_SERVICE> ListServiceParent = new List<V_HIS_SERVICE>();
        List<TestLisResultADO> ListTestParentService = new List<TestLisResultADO>();


        public void SetBarcodeKey()
        {
            try
            {
                if (rdo.currentTreatment != null)
                {
                    Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.currentTreatment.TDL_PATIENT_CODE);
                    barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodePatientCode.IncludeLabel = false;
                    barcodePatientCode.Width = 120;
                    barcodePatientCode.Height = 40;
                    barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodePatientCode.IncludeLabel = true;

                    dicImage.Add(Mps000096ExtendSingleKey.BARCODE_PATIENT_CODE_STR, barcodePatientCode);

                    Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.currentTreatment.TREATMENT_CODE);
                    barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeTreatment.IncludeLabel = false;
                    barcodeTreatment.Width = 120;
                    barcodeTreatment.Height = 40;
                    barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeTreatment.IncludeLabel = true;

                    dicImage.Add(Mps000096ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
                }

                if (rdo.currentServiceReq != null)
                {

                    Inventec.Common.BarcodeLib.Barcode barcodeServiceReqCode = new Inventec.Common.BarcodeLib.Barcode(rdo.currentServiceReq.SERVICE_REQ_CODE);
                    barcodeServiceReqCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeServiceReqCode.IncludeLabel = false;
                    barcodeServiceReqCode.Width = 120;
                    barcodeServiceReqCode.Height = 40;
                    barcodeServiceReqCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeServiceReqCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeServiceReqCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeServiceReqCode.IncludeLabel = true;

                    dicImage.Add(Mps000096ExtendSingleKey.BARCODE_SERVICE_REQ_CODE_STR, barcodeServiceReqCode);
                }

                if (rdo.currentSample != null && !String.IsNullOrWhiteSpace(this.rdo.currentSample.BARCODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barBarcode = new Inventec.Common.BarcodeLib.Barcode(rdo.currentSample.BARCODE);
                    barBarcode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barBarcode.IncludeLabel = false;
                    barBarcode.Width = 120;
                    barBarcode.Height = 40;
                    barBarcode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barBarcode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barBarcode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barBarcode.IncludeLabel = true;

                    dicImage.Add(Mps000096ExtendSingleKey.BAR_BARCODE_STR, barBarcode);
                }

                var ismeetisos = this.ListTestChild != null ? this.ListTestChild.Where(o => o.IS_MEET_ISO_STANDARD == (short)1 && o.MACHINE_ID.HasValue && !String.IsNullOrWhiteSpace(o.ISO_LOGO_URL)).ToList() : null;

                if (ismeetisos != null && ismeetisos.Count > 0)
                {
                    List<string> logos = ismeetisos.Select(s => s.ISO_LOGO_URL).Distinct().ToList();

                    int index = 0;
                    foreach (string url in logos)
                    {
                        try
                        {
                            MemoryStream imgLogo = Inventec.Fss.Client.FileDownload.GetFile(url);
                            if (imgLogo != null && imgLogo.Length > 0)
                            {
                                string key = "";
                                if (index == 0)
                                    key = Mps000096ExtendSingleKey.IMAGE_ISO_LOGO_STR;
                                else
                                {
                                    key = String.Format("{0}_{1}", Mps000096ExtendSingleKey.IMAGE_ISO_LOGO_STR, index);
                                    index++;
                                }
                                imgLogo.Position = 0;
                                SetSingleKey(new KeyValue(key, imgLogo.ToArray()));
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetSingleKey()
        {
            try
            {
                AddObjectKeyIntoListkey<MLCTADO>(rdo.mLCTADOs, false);
                if (rdo.currentSample != null && String.IsNullOrWhiteSpace(rdo.currentSample.SERVICE_REQ_CODE))
                {
                    SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.TDL_PATIENT_NAME, rdo.currentSample.LAST_NAME + " " + rdo.currentSample.FIRST_NAME));
                    SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.PATIENT_CONDITION_CODE, rdo.currentSample.PATIENT_CONDITION_CODE));
                    SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.PATIENT_CONDITION_NAME, rdo.currentSample.PATIENT_CONDITION_NAME));
                    if (rdo.currentSample.GENDER_CODE == "01")
                    {
                        SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.GENDER_NAME, "Nữ"));
                    }
                    else if (rdo.currentSample.GENDER_CODE == "02")
                    {
                        SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.GENDER_NAME, "Nam"));
                    }
                    else if (rdo.currentSample.GENDER_CODE == "03")
                    {
                        SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.GENDER_NAME, "Không xác định"));
                    }
                    else
                    {
                        SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.GENDER_NAME, ""));
                    }

                    SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.TDL_PATIENT_DOB, rdo.currentSample.DOB));
                    SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.VIR_ADDRESS, rdo.currentSample.ADDRESS));
                    SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.IS_HEIN, ""));
                    SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.IS_NOT_HEIN, ""));
                    SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.IS_RIGHT_ROUTE, ""));
                    SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.IS_NOT_RIGHT_ROUTE, ""));
                    SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.HEIN_CARD_NUMBER, ""));
                    SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.ICD_MAIN_TEXT, ""));
                    SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.ICD_NAME, ""));
                    SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                }
                if (rdo.PatientTypeAlter != null && !String.IsNullOrWhiteSpace(rdo.PatientTypeAlter.HEIN_CARD_NUMBER))
                {
                    SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.HEIN_CARD_NUMBER, rdo.PatientTypeAlter.HEIN_CARD_NUMBER));
                    SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.PatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.PatientTypeAlter.HEIN_CARD_TO_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.HEIN_MEDI_ORG_CODE, rdo.PatientTypeAlter.HEIN_MEDI_ORG_CODE));
                    SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.HEIN_ADDRESS, rdo.PatientTypeAlter.ADDRESS));
                    if (rdo.PatientTypeAlter.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)
                    {
                        SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                    }
                    SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.IS_HEIN, "X"));
                    if (rdo.PatientTypeAlter.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.IS_RIGHT_ROUTE, "X"));
                    }
                    else
                    {
                        SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.IS_NOT_RIGHT_ROUTE, "X"));
                    }
                }
                else
                {
                    SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.IS_NOT_HEIN, "X"));
                }

                if (rdo.currentTreatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.currentTreatment.IN_TIME)));
                    if (rdo.currentTreatment.OUT_TIME.HasValue)
                        SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.currentTreatment.OUT_TIME.Value)));
                    SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo.currentTreatment.TDL_PATIENT_DOB)));

                    HIS_MEDICINE_TYPE vaccine = GetVaccineFromVaccineId(rdo.currentTreatment.VACCINE_ID);
                    SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.VACCINE_CODE, vaccine != null ? vaccine.MEDICINE_TYPE_CODE : null));
                    SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.VACCINE_NAME, vaccine != null ? vaccine.MEDICINE_TYPE_NAME : null));
                }

                if (rdo.ServiceParent != null)
                {
                    SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.SERVICE_NAME_PARENT, rdo.ServiceParent.SERVICE_NAME));
                }

                SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.LOGIN_USER_NAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));
                if (rdo.currentTreatBedRoom != null)
                {
                    SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.BED_CODE, rdo.currentTreatBedRoom.BED_CODE));
                    SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.BED_NAME, rdo.currentTreatBedRoom.BED_NAME));
                }
                AddObjectKeyIntoListkey<V_LIS_SAMPLE>(rdo.currentSample, false);
                if (rdo.currentServiceReq != null)
                    AddObjectKeyIntoListkey<HIS_SERVICE_REQ>(rdo.currentServiceReq, false);
                if (rdo.currentTreatment != null)
                    AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.currentTreatment, false);
                Inventec.Common.Logging.LogSystem.Debug("currentSample.SERVICE_REQ_CODE: " + rdo.currentSample.SERVICE_REQ_CODE);
                if (ListTestParent != null && ListTestParent.Count > 0)
                {
                    var sampleTypeName = ListTestParent.Where(o => o.SAMPLE_TYPE_NAME != null).Select(o => o.SAMPLE_TYPE_NAME).Distinct().ToList();
                    if (sampleTypeName != null && sampleTypeName.Count > 0)
                        SetSingleKey(new KeyValue(Mps000096ExtendSingleKey.SAMPLE_TYPE_NAMEs, String.Join(", ", sampleTypeName)));
                }
                if (rdo.currentPatient != null)
                {
                    AddObjectKeyIntoListkey<HIS_PATIENT>(rdo.currentPatient, false);
                }
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HIS_MEDICINE_TYPE GetVaccineFromVaccineId(long? vaccineId)
        {
            HIS_MEDICINE_TYPE result = null;
            try
            {
                if (vaccineId == null || vaccineId <= 0)
                    return result;
                CommonParam param = new CommonParam();
                HisMedicineTypeFilter filter = new HisMedicineTypeFilter();
                filter.ID = vaccineId;
                result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_MEDICINE_TYPE>>("api/HisMedicineType/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX_RANGE GetTestIndexRange(long dob, long? genderId, string testIndexCode, ref List<V_HIS_TEST_INDEX_RANGE> testIndexRanges)
        {
            MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX_RANGE testIndexRange = new V_HIS_TEST_INDEX_RANGE();
            try
            {
                if (testIndexRanges != null && testIndexRanges.Count > 0)
                {
                    long? now = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                    int age = 0;

                    List<V_HIS_TEST_INDEX_RANGE> query = new List<V_HIS_TEST_INDEX_RANGE>();

                    foreach (var item in testIndexRanges)
                    {
                        if (item.TEST_INDEX_CODE == testIndexCode)
                        {
                            if (item.AGE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__YEAR)
                            {
                                age = Inventec.Common.DateTime.Calculation.DifferenceTime(dob, now ?? 0, Inventec.Common.DateTime.Calculation.UnitDifferenceTime.DAY) / 365;
                            }
                            else if (item.AGE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__MONTH)
                            {
                                age = Inventec.Common.DateTime.Calculation.DifferenceMonth(dob, now ?? 0);
                            }
                            else if (item.AGE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__DAY)
                            {
                                age = Inventec.Common.DateTime.Calculation.DifferenceDate(dob, now ?? 0);
                            }
                            else if (item.AGE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__HOUR)
                            {
                                age = Inventec.Common.DateTime.Calculation.DifferenceTime(dob, now ?? 0, Inventec.Common.DateTime.Calculation.UnitDifferenceTime.HOUR);
                            }

                            if (((item.AGE_FROM.HasValue && item.AGE_FROM.Value <= age) || !item.AGE_FROM.HasValue)
                            && ((item.AGE_TO.HasValue && item.AGE_TO.Value >= age) || !item.AGE_TO.HasValue))
                            {
                                query.Add(item);
                            }
                        }
                    }
                    if (genderId == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE)
                    {
                        query = query.Where(o => o.IS_MALE == 1).ToList();
                    }
                    else if (genderId == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
                    {
                        query = query.Where(o => o.IS_FEMALE == 1).ToList();
                    }

                    testIndexRange = query.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                testIndexRange = new V_HIS_TEST_INDEX_RANGE();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return testIndexRange;
        }

        private void ProcessMaxMixValue(TestLisResultADO ti, string description)
        {
            try
            {
                if (ti != null && !String.IsNullOrWhiteSpace(description))
                {
                    string[] values = description.Split('(', ' ', '-', ')');
                    values = values != null ? values.Where(o => !String.IsNullOrWhiteSpace(o)).ToArray() : null;
                    Decimal minValue, maxValue;

                    if (values != null && values.Length > 1 && Decimal.TryParse((values[0] ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out minValue) && Decimal.TryParse((values[1] ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out maxValue))
                    {
                        ti.MAX_VALUE = maxValue;
                        ti.MIN_VALUE = minValue;
                    }
                    else
                    {
                        ti.MIN_VALUE = null;
                        ti.MAX_VALUE = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessMaxMixValue(TestLisResultADO ti, V_HIS_TEST_INDEX_RANGE testIndexRange)
        {
            try
            {
                Decimal minValue = 0, maxValue = 0, value = 0, warMax = 0, warMin = 0;
                if (ti != null && testIndexRange != null)
                {
                    if (!String.IsNullOrWhiteSpace(testIndexRange.MIN_VALUE))
                    {
                        if (Decimal.TryParse((testIndexRange.MIN_VALUE ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out minValue))
                        {
                            ti.MIN_VALUE = minValue;

                        }
                        else
                        {
                            ti.MIN_VALUE = null;
                        }
                    }
                    if (!String.IsNullOrWhiteSpace(testIndexRange.MAX_VALUE))
                    {
                        if (Decimal.TryParse((testIndexRange.MAX_VALUE ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out maxValue))
                        {
                            ti.MAX_VALUE = maxValue;
                        }
                        else
                        {
                            ti.MAX_VALUE = null;
                        }
                    }
                    if (!String.IsNullOrWhiteSpace(testIndexRange.WARNING_MIN_VALUE))
                    {
                        if (Decimal.TryParse((testIndexRange.WARNING_MIN_VALUE ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out warMin))
                        {
                            ti.WARNING_MIN_VALUE = warMin;

                        }
                        else
                        {
                            ti.WARNING_MIN_VALUE = null;
                        }
                    }
                    if (!String.IsNullOrWhiteSpace(testIndexRange.WARNING_MAX_VALUE))
                    {
                        if (Decimal.TryParse((testIndexRange.WARNING_MAX_VALUE ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out warMax))
                        {
                            ti.WARNING_MAX_VALUE = warMax;

                        }
                        else
                        {
                            ti.WARNING_MAX_VALUE = null;
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(ti.VALUE_STR))
                    {
                        if (Decimal.TryParse((ti.VALUE_STR ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out value))
                        {
                            ti.VALUE = value;
                        }
                        else
                        {
                            ti.VALUE = null;
                        }
                    }

                    ti.VALUE_HL = ti.VALUE_STR;

                    this.ProcessHighLowValue(ti, testIndexRange);
                    this.ProcessHighLowWarningValue(ti, testIndexRange);
                    ti.VALUE_HL_NEW = !String.IsNullOrEmpty(ti.VALUE_HL) ? ti.VALUE_HL.Replace(',', '.') : "";
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessHighLowValue(TestLisResultADO ti, V_HIS_TEST_INDEX_RANGE testIndexRange)
        {
            if (!String.IsNullOrEmpty(testIndexRange.NORMAL_VALUE))
            {
                ti.DESCRIPTION = testIndexRange.NORMAL_VALUE;
                if (!String.IsNullOrWhiteSpace(ti.DESCRIPTION) && !String.IsNullOrWhiteSpace(ti.VALUE_STR) && ti.VALUE_STR.ToUpper() == ti.DESCRIPTION.ToUpper())
                {
                    ti.HIGH_OR_LOW = "";
                }
                else
                {
                    ti.HIGH_OR_LOW = " ";
                }
            }
            else
            {
                ti.DESCRIPTION = "";

                if (testIndexRange.IS_ACCEPT_EQUAL_MIN == 1 && testIndexRange.IS_ACCEPT_EQUAL_MAX == null)
                {
                    if (testIndexRange.MIN_VALUE != null)
                    {
                        ti.DESCRIPTION += testIndexRange.MIN_VALUE + "<= ";
                    }

                    ti.DESCRIPTION += "X";

                    if (testIndexRange.MAX_VALUE != null)
                    {
                        ti.DESCRIPTION += " < " + testIndexRange.MAX_VALUE;
                    }

                    if (ti.VALUE != null && ti.MIN_VALUE != null && ti.MIN_VALUE <= ti.VALUE && ti.MAX_VALUE != null && ti.VALUE < ti.MAX_VALUE)
                    {
                        ti.HIGH_OR_LOW = "";
                        ti.VALUE_HL = ti.VALUE + "";
                    }
                    else if (ti.VALUE != null && ti.MIN_VALUE != null && ti.VALUE < ti.MIN_VALUE)
                    {
                        ti.HIGH_OR_LOW = "L";
                        ti.VALUE_HL = ti.VALUE + "L";
                    }
                    else if (ti.VALUE != null && ti.MAX_VALUE != null && ti.MAX_VALUE <= ti.VALUE)
                    {
                        ti.HIGH_OR_LOW = "H";
                        ti.VALUE_HL = ti.VALUE + "H";
                    }
                    else
                    {
                        ti.HIGH_OR_LOW = "";
                        ti.VALUE_HL = ti.VALUE_STR + "";
                    }
                }
                else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == 1 && testIndexRange.IS_ACCEPT_EQUAL_MAX == 1)
                {
                    if (testIndexRange.MIN_VALUE != null)
                    {
                        ti.DESCRIPTION += testIndexRange.MIN_VALUE + "<= ";
                    }

                    ti.DESCRIPTION += "X";

                    if (testIndexRange.MAX_VALUE != null)
                    {
                        ti.DESCRIPTION += " <= " + testIndexRange.MAX_VALUE;
                    }

                    if (ti.VALUE != null && ti.MIN_VALUE != null && ti.MIN_VALUE <= ti.VALUE && ti.MAX_VALUE != null && ti.VALUE <= ti.MAX_VALUE)
                    {
                        ti.HIGH_OR_LOW = "";
                        ti.VALUE_HL = ti.VALUE + "";
                    }
                    else if (ti.VALUE != null && ti.MIN_VALUE != null && ti.VALUE < ti.MIN_VALUE)
                    {
                        ti.HIGH_OR_LOW = "L";
                        ti.VALUE_HL = ti.VALUE + "L";
                    }
                    else if (ti.VALUE != null && ti.MAX_VALUE != null && ti.MAX_VALUE < ti.VALUE)
                    {
                        ti.HIGH_OR_LOW = "H";
                        ti.VALUE_HL = ti.VALUE + "H";
                    }
                    else
                    {
                        ti.HIGH_OR_LOW = "";
                        ti.VALUE_HL = ti.VALUE_STR + "";
                    }
                }
                else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == null && testIndexRange.IS_ACCEPT_EQUAL_MAX == 1)
                {
                    if (testIndexRange.MIN_VALUE != null)
                    {
                        ti.DESCRIPTION += testIndexRange.MIN_VALUE + "< ";
                    }

                    ti.DESCRIPTION += "X";

                    if (testIndexRange.MAX_VALUE != null)
                    {
                        ti.DESCRIPTION += " <= " + testIndexRange.MAX_VALUE;
                    }

                    if (ti.VALUE != null && ti.MIN_VALUE != null && ti.MIN_VALUE < ti.VALUE && ti.MAX_VALUE != null && ti.VALUE <= ti.MAX_VALUE)
                    {
                        ti.HIGH_OR_LOW = "";
                        ti.VALUE_HL = ti.VALUE + "";
                    }
                    else if (ti.VALUE != null && ti.MIN_VALUE != null && ti.VALUE < ti.MIN_VALUE)
                    {
                        ti.HIGH_OR_LOW = "L";
                        ti.VALUE_HL = ti.VALUE + "L";
                    }
                    else if (ti.VALUE != null && ti.MAX_VALUE != null && ti.MAX_VALUE < ti.VALUE)
                    {
                        ti.HIGH_OR_LOW = "H";
                        ti.VALUE_HL = ti.VALUE + "H";
                    }
                    else
                    {
                        ti.HIGH_OR_LOW = "";
                        ti.VALUE_HL = ti.VALUE_STR + "";
                    }
                }
                else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == null && testIndexRange.IS_ACCEPT_EQUAL_MAX == null)
                {
                    if (testIndexRange.MIN_VALUE != null)
                    {
                        ti.DESCRIPTION += testIndexRange.MIN_VALUE + "< ";
                    }

                    ti.DESCRIPTION += "X";

                    if (testIndexRange.MAX_VALUE != null)
                    {
                        ti.DESCRIPTION += " < " + testIndexRange.MAX_VALUE;
                    }

                    if (ti.VALUE != null && ti.MIN_VALUE != null && ti.MIN_VALUE < ti.VALUE && ti.MAX_VALUE != null && ti.VALUE < ti.MAX_VALUE)
                    {
                        ti.HIGH_OR_LOW = "";
                        ti.VALUE_HL = ti.VALUE + "";
                    }
                    else if (ti.VALUE != null && ti.MIN_VALUE != null && ti.VALUE <= ti.MIN_VALUE)
                    {
                        ti.HIGH_OR_LOW = "L";
                        ti.VALUE_HL = ti.VALUE + "L";
                    }
                    else if (ti.VALUE != null && ti.MAX_VALUE != null && ti.MAX_VALUE <= ti.VALUE)
                    {
                        ti.HIGH_OR_LOW = "H";
                        ti.VALUE_HL = ti.VALUE + "H";
                    }
                    else
                    {
                        ti.HIGH_OR_LOW = "";
                        ti.VALUE_HL = ti.VALUE_STR + "";
                    }
                }
            }

            ti.DESCRIPTION_NEW = !String.IsNullOrEmpty(ti.DESCRIPTION) ? ti.DESCRIPTION.Replace(',', '.') : "";
        }

        private void ProcessHighLowWarningValue(TestLisResultADO ti, V_HIS_TEST_INDEX_RANGE testIndexRange)
        {
            ti.WARNING_DESCRIPTION = "";

            if (testIndexRange.IS_ACCEPT_EQUAL_WARNING_MIN == 1 && testIndexRange.IS_ACCEPT_EQUAL_WARNING_MAX == null)
            {
                if (testIndexRange.WARNING_MIN_VALUE != null)
                {
                    ti.WARNING_DESCRIPTION += testIndexRange.WARNING_MIN_VALUE + "<= ";
                }

                //ti.WARNING_DESCRIPTION += " ";

                if (testIndexRange.WARNING_MAX_VALUE != null)
                {
                    ti.WARNING_DESCRIPTION += " < " + testIndexRange.WARNING_MAX_VALUE;
                }

                if (ti.VALUE != null && ti.WARNING_MIN_VALUE != null && ti.VALUE < ti.WARNING_MIN_VALUE)
                {
                    ti.WARNING_NOTE = "Báo động";
                }
                else if (ti.VALUE != null && ti.WARNING_MAX_VALUE != null && ti.WARNING_MAX_VALUE <= ti.VALUE)
                {
                    ti.WARNING_NOTE = "Báo động";
                }
            }
            else if (testIndexRange.IS_ACCEPT_EQUAL_WARNING_MIN == 1 && testIndexRange.IS_ACCEPT_EQUAL_WARNING_MAX == 1)
            {
                if (testIndexRange.WARNING_MIN_VALUE != null)
                {
                    ti.WARNING_DESCRIPTION += testIndexRange.WARNING_MIN_VALUE + "<= ";
                }

                //ti.WARNING_DESCRIPTION += " ";

                if (testIndexRange.WARNING_MAX_VALUE != null)
                {
                    ti.WARNING_DESCRIPTION += " <= " + testIndexRange.WARNING_MAX_VALUE;
                }

                if (ti.VALUE != null && ti.WARNING_MIN_VALUE != null && ti.VALUE < ti.WARNING_MIN_VALUE)
                {
                    ti.WARNING_NOTE = "Báo động";
                }
                else if (ti.VALUE != null && ti.WARNING_MAX_VALUE != null && ti.WARNING_MAX_VALUE < ti.VALUE)
                {
                    ti.WARNING_NOTE = "Báo động";
                }
            }
            else if (testIndexRange.IS_ACCEPT_EQUAL_WARNING_MIN == null && testIndexRange.IS_ACCEPT_EQUAL_WARNING_MAX == 1)
            {
                if (testIndexRange.WARNING_MIN_VALUE != null)
                {
                    ti.WARNING_DESCRIPTION += testIndexRange.WARNING_MIN_VALUE + "< ";
                }

                //ti.WARNING_DESCRIPTION += " ";

                if (testIndexRange.WARNING_MAX_VALUE != null)
                {
                    ti.WARNING_DESCRIPTION += " <= " + testIndexRange.WARNING_MAX_VALUE;
                }

                if (ti.VALUE != null && ti.WARNING_MIN_VALUE != null && ti.VALUE < ti.WARNING_MIN_VALUE)
                {
                    ti.WARNING_NOTE = "Báo động";
                }
                else if (ti.VALUE != null && ti.WARNING_MAX_VALUE != null && ti.WARNING_MAX_VALUE < ti.VALUE)
                {
                    ti.WARNING_NOTE = "Báo động";
                }
            }
            else if (testIndexRange.IS_ACCEPT_EQUAL_WARNING_MIN == null && testIndexRange.IS_ACCEPT_EQUAL_WARNING_MAX == null)
            {
                if (testIndexRange.WARNING_MIN_VALUE != null)
                {
                    ti.WARNING_DESCRIPTION += testIndexRange.WARNING_MIN_VALUE + "< ";
                }

                //ti.WARNING_DESCRIPTION += " ";

                if (testIndexRange.WARNING_MAX_VALUE != null)
                {
                    ti.WARNING_DESCRIPTION += " < " + testIndexRange.WARNING_MAX_VALUE;
                }

                if (ti.VALUE != null && ti.WARNING_MIN_VALUE != null && ti.VALUE <= ti.WARNING_MIN_VALUE)
                {
                    ti.WARNING_NOTE = "Báo động";
                }
                else if (ti.VALUE != null && ti.WARNING_MAX_VALUE != null && ti.WARNING_MAX_VALUE <= ti.VALUE)
                {
                    ti.WARNING_NOTE = "Báo động";
                }
            }
        }
        private void SetListData2()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("SetListData2__ 1");
                int dem = 1;
                CommonParam param = new CommonParam();
                List<V_HIS_TEST_INDEX> testIndexs = null;
                List<V_HIS_TEST_INDEX_RANGE> testIndexRanges = null;
                if (rdo.lstLisResult != null && rdo.lstLisResult.Count > 0 && rdo.lstTestIndex != null && rdo.lstTestIndex.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("rdo.lstLisResult: ", rdo.lstLisResult));
                    testIndexs = new List<V_HIS_TEST_INDEX>();
                    var serviceCodes = rdo.lstLisResult.Select(o => o.SERVICE_CODE).Distinct().ToList();
                    testIndexs = rdo.lstTestIndex.Where(o => serviceCodes.Contains(o.SERVICE_CODE)).ToList();
                    if (testIndexs != null && testIndexs.Count > 0 && rdo.lstTestIndex != null && rdo.lstTestIndex.Count > 0)
                    {
                        var testIndexCodes = testIndexs.Select(o => o.TEST_INDEX_CODE).Distinct().ToList();
                        testIndexRanges = rdo.testIndexRangeAll.Where(o => testIndexCodes.Contains(o.TEST_INDEX_CODE)).ToList();
                    }
                }

                if (testIndexs != null && testIndexs.Count > 0)
                {
                    rdo.lstLisResult = rdo.lstLisResult.OrderBy(o => o.ID).ThenBy(o => o.TEST_INDEX_NAME).ToList();

                    var groupListResult = rdo.lstLisResult.GroupBy(o => new { o.SERVICE_CODE, o.MACHINE_ID }).ToList();

                    foreach (var group in groupListResult)
                    {
                        var firstItem = group.FirstOrDefault();
                        TestLisResultADO hisSereServTeinSDO = new TestLisResultADO();
                        hisSereServTeinSDO.IS_PARENT = 1;
                        hisSereServTeinSDO.IS_HAS_ONE_CHILD = 0;
                        hisSereServTeinSDO.ID = firstItem.ID;
                        hisSereServTeinSDO.IS_NO_EXECUTE = firstItem.IS_NO_EXECUTE;
                        hisSereServTeinSDO.SERVICE_NAME = firstItem.SERVICE_NAME;
                        hisSereServTeinSDO.SERVICE_CODE = firstItem.SERVICE_CODE;
                        hisSereServTeinSDO.TEST_INDEX_RANGE = "";
                        hisSereServTeinSDO.TEST_INDEX_UNIT_NAME = "";
                        hisSereServTeinSDO.VALUE = null;
                        hisSereServTeinSDO.PARENT_ID = ".";
                        hisSereServTeinSDO.MODIFIER = "";
                        hisSereServTeinSDO.CHILD_ID = firstItem.ID + ".";
                        hisSereServTeinSDO.SERVICE_NUM_ORDER = firstItem.SERVICE_NUM_ORDER;
                        hisSereServTeinSDO.RESULT_DESCRIPTION = firstItem.RESULT_DESCRIPTION;
                        hisSereServTeinSDO.ISO_PROCESS_CODE = firstItem.TDL_ISO_PROCESS_CODE;
                        hisSereServTeinSDO.IS_MEET_ISO_STANDARD = firstItem.TDL_IS_MEET_ISO_STANDARD;

                        V_HIS_SERVICE service = rdo.ListTestService != null ? rdo.ListTestService.FirstOrDefault(o => o.SERVICE_CODE == group.Key.SERVICE_CODE) : null;
                        V_HIS_SERVICE parent = null;
                        if (service != null)
                        {
                            hisSereServTeinSDO.SERVICE_ORDER = service.NUM_ORDER ?? -1;
                            if (service.PARENT_ID.HasValue)
                            {
                                parent = rdo.ListTestService != null ? rdo.ListTestService.FirstOrDefault(o => o.ID == service.PARENT_ID.Value) : null;
                                if (parent != null)
                                {
                                    hisSereServTeinSDO.SERVICE_CODE = parent.SERVICE_CODE;
                                    hisSereServTeinSDO.SERVICE_PARENT_ORDER = parent.NUM_ORDER ?? -1;
                                }
                            }
                            if (rdo.ListSampleType != null && rdo.ListSampleType.Count > 0)
                            {
                                var sampleType = rdo.ListSampleType.FirstOrDefault(o => o.SAMPLE_TYPE_CODE == service.SAMPLE_TYPE_CODE);
                                if (sampleType != null)
                                {
                                    hisSereServTeinSDO.SAMPLE_TYPE_CODE = sampleType.SAMPLE_TYPE_CODE;
                                    hisSereServTeinSDO.SAMPLE_TYPE_NAME = sampleType.SAMPLE_TYPE_NAME;
                                }
                            }
                            else if (rdo.ListTestSampleType != null && rdo.ListTestSampleType.Count > 0)
                            {
                                var testSampleType = rdo.ListTestSampleType.FirstOrDefault(o => o.TEST_SAMPLE_TYPE_CODE == service.SAMPLE_TYPE_CODE);
                                if (testSampleType != null)
                                {
                                    hisSereServTeinSDO.SAMPLE_TYPE_CODE = testSampleType.TEST_SAMPLE_TYPE_CODE;
                                    hisSereServTeinSDO.SAMPLE_TYPE_NAME = testSampleType.TEST_SAMPLE_TYPE_NAME;
                                }
                            }
                        }
                        else
                        {
                            hisSereServTeinSDO.SERVICE_ORDER = -1;
                            hisSereServTeinSDO.SERVICE_PARENT_ORDER = -1;
                        }

                        HIS_SERE_SERV sereServ = rdo.ListSereServ != null ? rdo.ListSereServ.FirstOrDefault(o => o.TDL_SERVICE_CODE == group.Key.SERVICE_CODE) : null;
                        if (sereServ != null)
                        {
                            hisSereServTeinSDO.PRIMARY_PRICE = sereServ.PRIMARY_PRICE;
                        }

                        //Lay machine_id
                        var lstResultItem = group.ToList();
                        lstResultItem = lstResultItem.OrderBy(o => o.ID).ThenBy(p => p.SERVICE_NAME).ToList();
                        if (rdo.lstLisResult != null && rdo.lstLisResult.Count > 0 && lstResultItem != null && lstResultItem.Count > 0)
                        {
                            var machineByLisResult = rdo.lstLisResult.FirstOrDefault(p => p.TEST_INDEX_CODE == lstResultItem[0].TEST_INDEX_CODE && p.SERVICE_CODE == lstResultItem[0].SERVICE_CODE);
                            if (machineByLisResult != null)
                            {
                                hisSereServTeinSDO.MACHINE_ID = machineByLisResult.MACHINE_ID;
                                hisSereServTeinSDO.MACHINE_NAME = machineByLisResult.MACHINE_NAME;
                            }
                        }

                        //SERVICE_PARENT_ID luôn có giá trị để gom nhóm.
                        var testIndFist = testIndexs.FirstOrDefault(o => o.TEST_INDEX_CODE == lstResultItem[0].TEST_INDEX_CODE);
                        if (testIndFist != null && rdo.ListTestService != null)
                        {
                            var parentService = rdo.ListTestService.FirstOrDefault(o => testIndFist.TEST_SERVICE_TYPE_ID == o.ID);
                            hisSereServTeinSDO.SERVICE_PARENT_ID = parentService != null ? parentService.PARENT_ID ?? 0 : 0;
                        }

                        if (lstResultItem != null && lstResultItem.Count == 1 && testIndFist != null && testIndFist.IS_NOT_SHOW_SERVICE == 1)
                        {

                            Inventec.Common.Logging.LogSystem.Debug("SetListData2__ 2");

                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("lstResultItem[0]: ", lstResultItem[0]));
                            hisSereServTeinSDO.MODIFIER = lstResultItem[0].MODIFIER;
                            hisSereServTeinSDO.TEST_INDEX_CODE = lstResultItem[0].TEST_INDEX_CODE;
                            hisSereServTeinSDO.TEST_INDEX_NAME = lstResultItem[0].TEST_INDEX_NAME;
                            hisSereServTeinSDO.IS_NO_EXECUTE = lstResultItem[0].IS_NO_EXECUTE;
                            hisSereServTeinSDO.TEST_INDEX_UNIT_NAME = testIndFist.TEST_INDEX_UNIT_NAME;
                            hisSereServTeinSDO.IS_IMPORTANT = testIndFist.IS_IMPORTANT;
                            hisSereServTeinSDO.VALUE_STR = lstResultItem[0].VALUE;
                            hisSereServTeinSDO.VALUE_HL = lstResultItem[0].VALUE;
                            hisSereServTeinSDO.ID = lstResultItem[0].ID;
                            hisSereServTeinSDO.NOTE = lstResultItem[0].DESCRIPTION;

                            // them con 
                            TestLisResultADO hisSereServTeinSDOChild = new TestLisResultADO();
                            AutoMapper.Mapper.CreateMap<TestLisResultADO, TestLisResultADO>();
                            hisSereServTeinSDOChild = AutoMapper.Mapper.Map<TestLisResultADO>(hisSereServTeinSDO);
                            hisSereServTeinSDOChild.IS_HAS_ONE_CHILD = 1;
                            hisSereServTeinSDOChild.PARENT_ID = hisSereServTeinSDO.CHILD_ID;
                            hisSereServTeinSDOChild.CHILD_ID = "";
                            hisSereServTeinSDOChild.SERVICE_NUM_ORDER = hisSereServTeinSDO.SERVICE_NUM_ORDER;
                            hisSereServTeinSDOChild.SERVICE_ORDER = hisSereServTeinSDO.SERVICE_ORDER;
                            hisSereServTeinSDOChild.SERVICE_PARENT_ORDER = hisSereServTeinSDO.SERVICE_PARENT_ORDER;

                            this.ListTestChild.Add(hisSereServTeinSDOChild);
                        }

                        hisSereServTeinSDO.VALUE_HL_NEW = !String.IsNullOrEmpty(hisSereServTeinSDO.VALUE_HL) ? hisSereServTeinSDO.VALUE_HL.Replace(',', '.') : "";

                        this.ListTestParent.Add(hisSereServTeinSDO);

                        if (lstResultItem != null && (lstResultItem.Count > 1 || (lstResultItem.Count == 1 && testIndFist != null && testIndFist.IS_NOT_SHOW_SERVICE != 1)))
                        {
                            Inventec.Common.Logging.LogSystem.Debug("SetListData2__ 3");

                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("lstResultItem: ", lstResultItem));
                            foreach (var ssTein in lstResultItem)
                            {
                                var testIndChild = testIndexs.FirstOrDefault(o => o.TEST_INDEX_CODE == ssTein.TEST_INDEX_CODE);
                                TestLisResultADO hisSereServTein = new TestLisResultADO();
                                if (testIndChild != null)
                                {
                                    hisSereServTein.IS_IMPORTANT = testIndChild.IS_IMPORTANT;
                                    hisSereServTein.TEST_INDEX_UNIT_NAME = testIndChild.TEST_INDEX_UNIT_NAME;
                                    hisSereServTein.NUM_ORDER = testIndChild.NUM_ORDER;
                                }
                                else
                                {
                                    hisSereServTein.NUM_ORDER = null;
                                }

                                hisSereServTein.IS_HAS_ONE_CHILD = 0;
                                hisSereServTein.CHILD_ID = ssTein.ID + "." + ssTein.ID;
                                hisSereServTein.ID = ssTein.ID;
                                hisSereServTein.PARENT_ID = hisSereServTeinSDO.CHILD_ID;
                                hisSereServTein.MODIFIER = ssTein.MODIFIER;
                                hisSereServTein.TEST_INDEX_CODE = ssTein.TEST_INDEX_CODE;
                                hisSereServTein.TEST_INDEX_NAME = ssTein.TEST_INDEX_NAME;
                                hisSereServTein.SERVICE_NAME = ssTein.TEST_INDEX_NAME;
                                hisSereServTein.SERVICE_CODE = firstItem.SERVICE_CODE;
                                hisSereServTein.IS_NO_EXECUTE = ssTein.IS_NO_EXECUTE;
                                hisSereServTein.VALUE_HL = ssTein.VALUE;
                                hisSereServTein.VALUE_STR = ssTein.VALUE;
                                hisSereServTein.SERVICE_NUM_ORDER = ssTein.SERVICE_NUM_ORDER;
                                hisSereServTein.ISO_PROCESS_CODE = ssTein.TDL_ISO_PROCESS_CODE;
                                hisSereServTein.IS_MEET_ISO_STANDARD = ssTein.TDL_IS_MEET_ISO_STANDARD;
                                hisSereServTein.RESULT_DESCRIPTION = ssTein.RESULT_DESCRIPTION;
                                if (service != null)
                                {
                                    hisSereServTein.SERVICE_ORDER = service.NUM_ORDER;
                                }
                                else
                                {
                                    hisSereServTein.SERVICE_ORDER = -1;
                                }
                                if (parent != null)
                                {
                                    hisSereServTeinSDO.SERVICE_PARENT_ORDER = parent.NUM_ORDER ?? -1;
                                }
                                else
                                {
                                    hisSereServTeinSDO.SERVICE_PARENT_ORDER = -1;
                                }
                                hisSereServTein.MACHINE_NAME = ssTein.MACHINE_NAME;
                                hisSereServTein.MACHINE_ID = ssTein.MACHINE_ID;
                                hisSereServTein.ISO_LOGO_URL = ssTein.ISO_LOGO_URL;

                                hisSereServTein.VALUE_HL_NEW = !String.IsNullOrEmpty(hisSereServTein.VALUE_HL) ? hisSereServTein.VALUE_HL.Replace(',', '.') : "";

                                this.ListTestChild.Add(hisSereServTein);
                            }
                        }

                        dem++;
                    }

                    if (ListTestParent != null && ListTestParent.Count > 0 && rdo.ListTestService != null)
                    {
                        var grServiceParent = ListTestParent.GroupBy(o => o.SERVICE_PARENT_ID).ToList();
                        foreach (var item in grServiceParent)
                        {
                            var parent = rdo.ListTestService.FirstOrDefault(o => item.Key == o.ID);
                            if (parent != null)
                            {
                                ListServiceParent.Add(parent);
                            }
                            else
                            {
                                ListServiceParent.Add(new V_HIS_SERVICE());
                            }
                        }
                    }
                }

                var testIndexRangeAll = rdo.testIndexRangeAll;
                foreach (var hisSereServTeinSDO in this.ListTestChild)
                {
                    V_HIS_TEST_INDEX_RANGE testIndexRange = new V_HIS_TEST_INDEX_RANGE();
                    testIndexRange = GetTestIndexRange(rdo.currentTreatment.TDL_PATIENT_DOB, rdo.genderId, hisSereServTeinSDO.TEST_INDEX_CODE, ref testIndexRangeAll);
                    if (testIndexRange != null)
                    {
                        ProcessMaxMixValue(hisSereServTeinSDO, testIndexRange);
                    }
                }

                foreach (var hisSereServTeinSDO in this.ListTestParent)
                {
                    V_HIS_TEST_INDEX_RANGE testIndexRange = new V_HIS_TEST_INDEX_RANGE();
                    if (rdo.currentTreatment != null)
                        testIndexRange = GetTestIndexRange(rdo.currentTreatment.TDL_PATIENT_DOB, rdo.genderId, hisSereServTeinSDO.TEST_INDEX_CODE, ref testIndexRangeAll);
                    else
                        testIndexRange = GetTestIndexRange(rdo.currentSample.DOB ?? 0, rdo.genderId, hisSereServTeinSDO.TEST_INDEX_CODE, ref testIndexRangeAll);
                    if (testIndexRange != null)
                    {
                        ProcessMaxMixValue(hisSereServTeinSDO, testIndexRange);
                    }
                }

                this.ListTestParent = this.ListTestParent != null && this.ListTestParent.Count > 0
                    ? this.ListTestParent.OrderByDescending(o => o.SERVICE_PARENT_ORDER).ThenByDescending(o => o.SERVICE_ORDER).ThenBy(o => o.SERVICE_NUM_ORDER).ThenBy(p => p.SERVICE_NAME).ToList()
                    : this.ListTestParent;

                this.ListTestParentService = this.ListTestParent.GroupBy(o => o.SERVICE_CODE).Select(s => s.First()).ToList();

                this.ListTestChild = this.ListTestChild != null && this.ListTestChild.Count > 0
                    ? this.ListTestChild.OrderByDescending(o => o.NUM_ORDER)
                    .ThenBy(p => p.TEST_INDEX_NAME).ToList()
                    : this.ListTestChild;

                this.ListServiceParent = ListServiceParent != null && ListServiceParent.Count() > 0
                    ? this.ListServiceParent.OrderByDescending(o => o.NUM_ORDER).ThenBy(p => p.SERVICE_NAME).ToList()
                    : this.ListServiceParent;

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

                this.SetListData2();
                this.SetSingleKey();
                //this.GetTestIndexRanges();
                this.SetBarcodeKey();
                SetTreatmentQrCodeBase();

                this.SetSignatureKeyImageByCFG();

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "ListTestParent", this.ListTestParent);
                objectTag.AddObjectData(store, "ListTestChild", this.ListTestChild);
                objectTag.AddObjectData(store, "ServiceParent", this.ListServiceParent);
                objectTag.AddObjectData(store, "TestParentService", this.ListTestParentService);
                objectTag.AddRelationship(store, "ListTestParent", "ListTestChild", "CHILD_ID", "PARENT_ID");
                objectTag.AddRelationship(store, "ServiceParent", "ListTestParent", "ID", "SERVICE_PARENT_ID");

                objectTag.AddRelationship(store, "TestParentService", "ListTestChild", "SERVICE_CODE", "SERVICE_CODE");
                objectTag.AddRelationship(store, "ServiceParent", "TestParentService", "ID", "SERVICE_PARENT_ID");
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                log = LogDataServiceReq(rdo.currentTreatment.TREATMENT_CODE, rdo.currentServiceReq.SERVICE_REQ_CODE, "In phiếu trả kết quả xét nghiệm");
            }
            catch (Exception ex)
            {
                log = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return log;
        }

        public override string ProcessUniqueCodeData()
        {
            string result = "";
            try
            {
                if (rdo != null && rdo.currentSample != null && rdo.currentServiceReq != null && rdo.currentTreatment != null)
                {
                    var groupedServices = rdo.lstLisResult
                        .Select(sereServ => rdo.ListTestService.FirstOrDefault(o => o.SERVICE_CODE == sereServ.SERVICE_CODE))
                        .Where(service => service != null && service.PARENT_ID.HasValue)
                        .GroupBy(service => service.PARENT_ID.Value)
                        .ToList();

                    foreach (var group in groupedServices)
                    {
                        var parentService = rdo.ListTestService.FirstOrDefault(o => o.ID == group.Key);
                        string serviceCode = parentService != null ? parentService.SERVICE_CODE : null;

                        string currentResult = string.Format("{0} {1} {2} {3}",
                            this.printTypeCode,
                            string.Format("TREATMENT_CODE:{0}", rdo.currentTreatment.TREATMENT_CODE),
                            string.Format("SERVICE_REQ_CODE:{0}", rdo.currentServiceReq.SERVICE_REQ_CODE),
                            string.Format("BARCODE:{0}", rdo.currentSample.BARCODE));


                        if (!string.IsNullOrEmpty(serviceCode))
                        {
                            currentResult += string.Format(" SERVICE_CODE:{0}", serviceCode);
                        }
                        result = currentResult;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
