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
using MPS.Processor.Mps000008.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000008
{
    public class Mps000008Processor : AbstractProcessor
    {
        Mps000008PDO rdo;

        public Mps000008Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000008PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = true;
            try
            {
                SetBarcodeKey();
                SetSingleKey();

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                objectTag.AddObjectData(store, "listEkipUser", rdo.listEkipUser);
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void SetBarcodeKey()
        {
            try
            {
                if (rdo.Patient != null && !String.IsNullOrEmpty(rdo.Patient.PATIENT_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.Patient.PATIENT_CODE);
                    barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodePatientCode.IncludeLabel = false;
                    barcodePatientCode.Width = 120;
                    barcodePatientCode.Height = 40;
                    barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodePatientCode.IncludeLabel = true;

                    dicImage.Add(Mps000008ExtendSingleKey.PATIENT_CODE_BAR, barcodePatientCode);
                }

                if (rdo.currentTreatment != null && !String.IsNullOrEmpty(rdo.currentTreatment.TREATMENT_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.currentTreatment.TREATMENT_CODE);
                    barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodeTreatment.IncludeLabel = false;
                    barcodeTreatment.Width = 120;
                    barcodeTreatment.Height = 40;
                    barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    barcodeTreatment.IncludeLabel = true;

                    dicImage.Add(Mps000008ExtendSingleKey.TREATMENT_CODE_BAR, barcodeTreatment);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetSingleKey()
        {
            try
            {
                if (rdo.currentTreatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000008ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.currentTreatment.IN_TIME)));
                    if (rdo.currentTreatment.OUT_TIME.HasValue)
                        SetSingleKey(new KeyValue(Mps000008ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.currentTreatment.OUT_TIME.Value)));
                    SetSingleKey(new KeyValue(Mps000008ExtendSingleKey.METHOD, rdo.currentTreatment.TREATMENT_METHOD));

                    if (rdo.currentTreatment.TDL_PATIENT_DOB.ToString().Length > 4)
                    {
                        SetSingleKey(new KeyValue(Mps000008ExtendSingleKey.DOB, rdo.currentTreatment.TDL_PATIENT_DOB));
                        SetSingleKey(new KeyValue(Mps000008ExtendSingleKey.AGE, Inventec.Common.DateTime.Calculation.AgeCaption(rdo.currentTreatment.TDL_PATIENT_DOB)));
                        SetSingleKey(new KeyValue(Mps000008ExtendSingleKey.STR_YEAR, rdo.currentTreatment.TDL_PATIENT_DOB.ToString().Substring(0, 4)));
                        SetSingleKey(new KeyValue(Mps000008ExtendSingleKey.STR_DOB, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.currentTreatment.TDL_PATIENT_DOB)));
                    }

                    SetSingleKey(new KeyValue(Mps000008ExtendSingleKey.ADDRESS, rdo.currentTreatment.TDL_PATIENT_ADDRESS));
                    SetSingleKey(new KeyValue(Mps000008ExtendSingleKey.VIR_ADDRESS, rdo.currentTreatment.TDL_PATIENT_ADDRESS));
                    SetSingleKey(new KeyValue(Mps000008ExtendSingleKey.CAREER_NAME, rdo.currentTreatment.TDL_PATIENT_CAREER_NAME));
                    SetSingleKey(new KeyValue(Mps000008ExtendSingleKey.PATIENT_CODE, rdo.currentTreatment.TDL_PATIENT_CODE));
                    SetSingleKey(new KeyValue(Mps000008ExtendSingleKey.GENDER_NAME, rdo.currentTreatment.TDL_PATIENT_GENDER_NAME));
                    SetSingleKey(new KeyValue(Mps000008ExtendSingleKey.MILITARY_RANK_NAME, rdo.currentTreatment.TDL_PATIENT_MILITARY_RANK_NAME));
                    SetSingleKey(new KeyValue(Mps000008ExtendSingleKey.VIR_PATIENT_NAME, rdo.currentTreatment.TDL_PATIENT_NAME));
                    SetSingleKey(new KeyValue(Mps000008ExtendSingleKey.NATIONAL_NAME, rdo.currentTreatment.TDL_PATIENT_NATIONAL_NAME));
                    SetSingleKey(new KeyValue(Mps000008ExtendSingleKey.WORK_PLACE, rdo.currentTreatment.TDL_PATIENT_WORK_PLACE));
                    SetSingleKey(new KeyValue(Mps000008ExtendSingleKey.WORK_PLACE_NAME, rdo.currentTreatment.TDL_PATIENT_WORK_PLACE_NAME));
                    SetSingleKey(new KeyValue(Mps000008ExtendSingleKey.END_DEPARTMENT_HEAD_LOGINNAME, rdo.currentTreatment.END_DEPARTMENT_HEAD_LOGINNAME));
                    SetSingleKey(new KeyValue(Mps000008ExtendSingleKey.END_DEPARTMENT_HEAD_USERNAME, rdo.currentTreatment.END_DEPARTMENT_HEAD_USERNAME));
                    SetSingleKey(new KeyValue(Mps000008ExtendSingleKey.HOSPITAL_DIRECTOR_LOGINNAME, rdo.currentTreatment.HOSPITAL_DIRECTOR_LOGINNAME));
                    SetSingleKey(new KeyValue(Mps000008ExtendSingleKey.HOSPITAL_DIRECTOR_USERNAME, rdo.currentTreatment.HOSPITAL_DIRECTOR_USERNAME));
                    SetSingleKey(new KeyValue(Mps000008ExtendSingleKey.END_DEPT_SUBS_HEAD_LOGINNAME, rdo.currentTreatment.END_DEPT_SUBS_HEAD_LOGINNAME));
                    SetSingleKey(new KeyValue(Mps000008ExtendSingleKey.END_DEPT_SUBS_HEAD_USERNAME, rdo.currentTreatment.END_DEPT_SUBS_HEAD_USERNAME));
                    SetSingleKey(new KeyValue(Mps000008ExtendSingleKey.HOSP_SUBS_DIRECTOR_LOGINNAME, rdo.currentTreatment.HOSP_SUBS_DIRECTOR_LOGINNAME));
                    SetSingleKey(new KeyValue(Mps000008ExtendSingleKey.HOSP_SUBS_DIRECTOR_USERNAME, rdo.currentTreatment.HOSP_SUBS_DIRECTOR_USERNAME));
                }

                if (rdo.listEkipUser != null && rdo.listEkipUser.Count() > 0)
                {

                    List<string> ekipUserNames = rdo.listEkipUser.Where(o => o.IS_SURG_MAIN == (short)1).OrderBy(p => p
                        .USERNAME).Select(o => o.USERNAME).Distinct().ToList();

                    var groupEkipUser = rdo.listEkipUser.GroupBy(o => o.EXECUTE_ROLE_ID).ToList();

                    foreach (var group in groupEkipUser)
                    {
                        string key = "SURG_EXECUTER_ROLE_CODE_" + group.FirstOrDefault().EXECUTE_ROLE_CODE;

                        SetSingleKey(new KeyValue(key, String.Join("; ", group.Select(o => o.USERNAME))));
                    }

                    SetSingleKey(new KeyValue(Mps000008ExtendSingleKey.SURG_DOCTOR_NAME, string.Join(",", ekipUserNames)));
                }
                SetSingleKey(new KeyValue(Mps000008ExtendSingleKey.TIME_IN_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.timeIn)));
                SetSingleKey(new KeyValue(Mps000008ExtendSingleKey.TIME_IN_FULL_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo.timeIn)));

                HIS_APPOINTMENT_PERIOD appPeriod = new HIS_APPOINTMENT_PERIOD();
                if (rdo.currentTreatment.APPOINTMENT_PERIOD_ID.HasValue && rdo.ListAppointmentPeriod != null && rdo.ListAppointmentPeriod.Count > 0)
                {
                    appPeriod = rdo.ListAppointmentPeriod.FirstOrDefault(o => o.ID == rdo.currentTreatment.APPOINTMENT_PERIOD_ID);
                }

                AddObjectKeyIntoListkey<PatientADO>(rdo.Patient);
                //AddObjectKeyIntoListkey<V_HIS_TREATMENT_OUT>(rdo.treatmentOut, false);
                AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.currentTreatment, true);
                AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo.PatyAlterBhyt, false);
                AddObjectKeyIntoListkey<Mps000008ADO>(rdo.Mps000008ADO, false);
                AddObjectKeyIntoListkey<HIS_APPOINTMENT_PERIOD>(appPeriod, false);

                if (rdo.HisTracking != null)
                {
                    SetSingleKey(new KeyValue("TRACKING_EYE_TENSION_LEFT", rdo.HisTracking.EYE_TENSION_LEFT));
                    SetSingleKey(new KeyValue("TRACKING_EYE_TENSION_RIGHT", rdo.HisTracking.EYE_TENSION_RIGHT));
                    SetSingleKey(new KeyValue("TRACKING_EYESIGHT_LEFT", rdo.HisTracking.EYESIGHT_LEFT));
                    SetSingleKey(new KeyValue("TRACKING_EYESIGHT_RIGHT", rdo.HisTracking.EYESIGHT_RIGHT));
                    SetSingleKey(new KeyValue("TRACKING_EYESIGHT_GLASS_LEFT", rdo.HisTracking.EYESIGHT_GLASS_LEFT));
                    SetSingleKey(new KeyValue("TRACKING_EYESIGHT_GLASS_RIGHT", rdo.HisTracking.EYESIGHT_GLASS_RIGHT));
                }

                if (rdo.PatyAlterBhyt != null)
                {
                    if (!String.IsNullOrEmpty(rdo.PatyAlterBhyt.HEIN_CARD_NUMBER))
                    {
                        SetSingleKey((new KeyValue(Mps000008ExtendSingleKey.HEIN_CARD_NUMBER_SEPARATE, HeinCardHelper.SetHeinCardNumberDisplayByNumber(rdo.PatyAlterBhyt.HEIN_CARD_NUMBER))));
                        SetSingleKey((new KeyValue(Mps000008ExtendSingleKey.IS_HEIN, "X")));
                        SetSingleKey((new KeyValue(Mps000008ExtendSingleKey.IS_VIENPHI, "")));
                        SetSingleKey((new KeyValue(Mps000008ExtendSingleKey.HEIN_CARD_NUMBER_1, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(0, 2))));
                        SetSingleKey((new KeyValue(Mps000008ExtendSingleKey.HEIN_CARD_NUMBER_2, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(2, 1))));
                        SetSingleKey((new KeyValue(Mps000008ExtendSingleKey.HEIN_CARD_NUMBER_3, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(3, 2))));
                        SetSingleKey((new KeyValue(Mps000008ExtendSingleKey.HEIN_CARD_NUMBER_4, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(5, 2))));
                        SetSingleKey((new KeyValue(Mps000008ExtendSingleKey.HEIN_CARD_NUMBER_5, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(7, 3))));
                        SetSingleKey((new KeyValue(Mps000008ExtendSingleKey.HEIN_CARD_NUMBER_6, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(10, 5))));
                        SetSingleKey(new KeyValue(Mps000008ExtendSingleKey.STR_HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.PatyAlterBhyt.HEIN_CARD_FROM_TIME ?? 0)));
                        SetSingleKey(new KeyValue(Mps000008ExtendSingleKey.STR_HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.PatyAlterBhyt.HEIN_CARD_TO_TIME ?? 0)));
                    }
                    else
                    {
                        SetSingleKey((new KeyValue(Mps000008ExtendSingleKey.IS_HEIN, "")));
                        SetSingleKey((new KeyValue(Mps000008ExtendSingleKey.IS_VIENPHI, "X")));
                    }

                    SetSingleKey(new KeyValue(Mps000008ExtendSingleKey.HEIN_CARD_ADDRESS, rdo.PatyAlterBhyt.ADDRESS));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                if (rdo != null && rdo.currentTreatment != null)
                {
                    string treatmentCode = rdo.currentTreatment.TREATMENT_CODE;
                    log = String.Format("HIS_TREATMENT: {0}", treatmentCode);
                }
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
                if (rdo != null && rdo.currentTreatment != null && rdo.Mps000008ADO != null)
                    result = String.Format("PRINT_TYPE_CODE:{0} TREATMENT_CODE:{1} END_CODE:{2}", printTypeCode, rdo.currentTreatment.TREATMENT_CODE, rdo.currentTreatment.END_CODE);
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }

    public class HeinCardHelper
    {
        public static string SetHeinCardNumberDisplayByNumber(string heinCardNumber)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrWhiteSpace(heinCardNumber) && heinCardNumber.Length == 15)
                {
                    string separateSymbol = "-";
                    result = new StringBuilder().Append(heinCardNumber.Substring(0, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(2, 1)).Append(separateSymbol).Append(heinCardNumber.Substring(3, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(5, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(7, 3)).Append(separateSymbol).Append(heinCardNumber.Substring(10, 5)).ToString();
                }
                else
                {
                    result = heinCardNumber;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = heinCardNumber;
            }
            return result;
        }

        public static string TrimHeinCardNumber(string chucodau)
        {
            string result = "";
            try
            {
                result = System.Text.RegularExpressions.Regex.Replace(chucodau, @"[-,_ ]|[_]{2}|[_]{3}|[_]{4}|[_]{5}", "").ToUpper();
            }
            catch (Exception ex)
            {

            }

            return result;
        }
    }
}
