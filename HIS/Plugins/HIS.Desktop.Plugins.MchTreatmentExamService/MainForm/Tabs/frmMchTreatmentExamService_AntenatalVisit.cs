using DevExpress.XtraEditors;
using HIS.Desktop.Plugins.MchTreatmentExamService.ADO;
using HIS.Desktop.Utilities.Extensions;
using MCH.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.MainForm
{
    public partial class UCMchTreatmentExamService : HIS.Desktop.Utility.FormBase
    {
        #region Tab 2: Khám thai

        private void GetDataFromTab2()
        {
            try
            {
                if (_examService == null) _examService = new MCH_EXAM_SERVICE();
                if (_antenatalVisit == null) _antenatalVisit = new MCH_ANTENATAL_VISIT();
                // MCH_EXAM_SERVICE
                _examService.EXECUTE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteExam2.DateTime) ?? 0;
                _examService.EXECUTE_LOGINNAME = GetComboValue(cboUser2);
                _examService.EXECUTE_USERNAME = GetUserNameByLoginName(_examService.EXECUTE_LOGINNAME);
                _examService.EXECUTE_TYPE = GetComboValue(cboDiploma2);
                
                // MCH_ANTENATAL_VISIT
                _antenatalVisit.LAST_MENSTRUAL_PERIOD = ConvertDateToTimeNumber(dteLastMenstrualPeriod2.EditValue, true);
                _antenatalVisit.ESTIMATED_BIRTH_DATE = ConvertDateToTimeNumber(dteEstimatedBirthDate2.EditValue, true);
                _antenatalVisit.GESTATIONAL_AGE = GetSpinEditStringValue(spnGestationalAge2);
                _antenatalVisit.GRAVIDA = GetSpinEditStringValue(spnGravida2);
                _antenatalVisit.WEIGHT = GetSpinEditStringValue(spnWeight2);
                _antenatalVisit.HEIGHT = GetSpinEditStringValue(spnHeight2);
                _antenatalVisit.BLOOD_PRESSURE_SYSTOLIC = GetSpinEditStringValue(spnBloodPressureSystolic2);
                _antenatalVisit.BLOOD_PRESSURE_DIASTOLIC = GetSpinEditStringValue(spnBloodPressureDiastolic2);
                _antenatalVisit.FUNDAL_HEIGHT = GetSpinEditStringValue(spnFundalHeight2);
                _antenatalVisit.ABDOMINAL_CIRCUMFERENCE = GetSpinEditStringValue(spnAbdominalCircumference2);
                
                var pelvicMeasurementValue = GetRadioGroupValue("PelvicMeasurement");
                _antenatalVisit.PELVIC_MEASUREMENT = pelvicMeasurementValue.HasValue ? (pelvicMeasurementValue.Value + 1).ToString() : null;
                
                var anemiaStatusValue = GetRadioGroupValue("AnemiaStatus");
                _antenatalVisit.ANEMIA_STATUS = anemiaStatusValue.HasValue ? anemiaStatusValue.Value.ToString() : null;
                
                var urineProteinValue = GetRadioGroupValue("UrineProtein");
                _antenatalVisit.URINE_PROTEIN = urineProteinValue.HasValue ? urineProteinValue.Value.ToString() : null;
                
                var testHivValue = GetRadioGroupValue("TestHiv");
                _antenatalVisit.TEST_HIV = testHivValue.HasValue ? testHivValue.Value.ToString() : null;
                
                var testHepatitisBValue = GetRadioGroupValue("TestHepatitisB");
                _antenatalVisit.TEST_HEPATITIS_B = testHepatitisBValue.HasValue ? testHepatitisBValue.Value.ToString() : null;
                
                var testSyphilisValue = GetRadioGroupValue("TestSyphilis");
                _antenatalVisit.TEST_SYPHILIS = testSyphilisValue.HasValue ? testSyphilisValue.Value.ToString() : null;
                
                var testBloodGlucoseValue = GetRadioGroupValue("TestBloodGlucose");
                _antenatalVisit.TEST_BLOOD_GLUCOSE = testBloodGlucoseValue.HasValue ? testBloodGlucoseValue.Value.ToString() : null;
                
                var prenatalScreeningValue = GetRadioGroupValue("PrenatalScreening");
                _antenatalVisit.PRENATAL_SCREENING = prenatalScreeningValue.HasValue ? prenatalScreeningValue.Value.ToString() : null;
                
                var fetalHeartValue = GetRadioGroupValue("FetalHeart");
                _antenatalVisit.FETAL_HEART = fetalHeartValue.HasValue ? fetalHeartValue.Value.ToString() : null;
                
                var fetalPositionValue = GetRadioGroupValue("FetalPosition");
                _antenatalVisit.FETAL_POSITION = fetalPositionValue.HasValue ? fetalPositionValue.Value.ToString() : null;
                
                var birthPredictionValue = GetRadioGroupValue("BirthPrediction");
                _antenatalVisit.BIRTH_PREDICTION = birthPredictionValue.HasValue ? birthPredictionValue.Value.ToString() : null;

                _antenatalVisit.MEDICAL_HISTORY_INTERNAL = MedicalHistoryInternal2Selected != null && MedicalHistoryInternal2Selected.Count > 0 ? string.Join(";", MedicalHistoryInternal2Selected.Select(o => o.CODE).ToList()) : null;// GetComboValue(cboMedicalHistoryInternal2);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToTab2()
        {
            try
            {
                if (_examService != null)
                {
                    SetComboValue(cboUser2, _examService.EXECUTE_LOGINNAME);
                    SetComboValue(cboDiploma2, _examService.EXECUTE_TYPE);
                }

                if (_antenatalVisit != null)
                {
                    if (_antenatalVisit.LAST_MENSTRUAL_PERIOD.HasValue)
                    {
                        dteLastMenstrualPeriod2.EditValue = ConvertTimeNumberToDate(_antenatalVisit.LAST_MENSTRUAL_PERIOD.Value);
                    }
                    
                    if (_antenatalVisit.ESTIMATED_BIRTH_DATE.HasValue)
                    {
                        dteEstimatedBirthDate2.EditValue = ConvertTimeNumberToDate(_antenatalVisit.ESTIMATED_BIRTH_DATE.Value);
                    }
                    
                    SetSpinEditStringValue(spnGestationalAge2, _antenatalVisit.GESTATIONAL_AGE);
                    SetSpinEditStringValue(spnGravida2, _antenatalVisit.GRAVIDA);
                    SetSpinEditStringValue(spnWeight2, _antenatalVisit.WEIGHT);
                    SetSpinEditStringValue(spnHeight2, _antenatalVisit.HEIGHT);
                    SetSpinEditStringValue(spnBloodPressureSystolic2, _antenatalVisit.BLOOD_PRESSURE_SYSTOLIC);
                    SetSpinEditStringValue(spnBloodPressureDiastolic2, _antenatalVisit.BLOOD_PRESSURE_DIASTOLIC);
                    SetSpinEditStringValue(spnFundalHeight2, _antenatalVisit.FUNDAL_HEIGHT);
                    SetSpinEditStringValue(spnAbdominalCircumference2, _antenatalVisit.ABDOMINAL_CIRCUMFERENCE);
                    
                    if (!string.IsNullOrEmpty(_antenatalVisit.PELVIC_MEASUREMENT))
                        SetRadioGroupValue("PelvicMeasurement", (short?)(short.Parse(_antenatalVisit.PELVIC_MEASUREMENT) - 1));
                    
                    if (!string.IsNullOrEmpty(_antenatalVisit.ANEMIA_STATUS))
                        SetRadioGroupValue("AnemiaStatus", short.Parse(_antenatalVisit.ANEMIA_STATUS));
                    
                    if (!string.IsNullOrEmpty(_antenatalVisit.URINE_PROTEIN))
                        SetRadioGroupValue("UrineProtein", short.Parse(_antenatalVisit.URINE_PROTEIN));
                    
                    if (!string.IsNullOrEmpty(_antenatalVisit.TEST_HIV))
                        SetRadioGroupValue("TestHiv", short.Parse(_antenatalVisit.TEST_HIV));
                    
                    if (!string.IsNullOrEmpty(_antenatalVisit.TEST_HEPATITIS_B))
                        SetRadioGroupValue("TestHepatitisB", short.Parse(_antenatalVisit.TEST_HEPATITIS_B));
                    
                    if (!string.IsNullOrEmpty(_antenatalVisit.TEST_SYPHILIS))
                        SetRadioGroupValue("TestSyphilis", short.Parse(_antenatalVisit.TEST_SYPHILIS));
                    
                    if (!string.IsNullOrEmpty(_antenatalVisit.TEST_BLOOD_GLUCOSE))
                        SetRadioGroupValue("TestBloodGlucose", short.Parse(_antenatalVisit.TEST_BLOOD_GLUCOSE));
                    
                    if (!string.IsNullOrEmpty(_antenatalVisit.PRENATAL_SCREENING))
                        SetRadioGroupValue("PrenatalScreening", short.Parse(_antenatalVisit.PRENATAL_SCREENING));
                    
                    if (!string.IsNullOrEmpty(_antenatalVisit.FETAL_HEART))
                        SetRadioGroupValue("FetalHeart", short.Parse(_antenatalVisit.FETAL_HEART));
                    
                    if (!string.IsNullOrEmpty(_antenatalVisit.FETAL_POSITION))
                        SetRadioGroupValue("FetalPosition", short.Parse(_antenatalVisit.FETAL_POSITION));
                    
                    if (!string.IsNullOrEmpty(_antenatalVisit.BIRTH_PREDICTION))
                        SetRadioGroupValue("BirthPrediction", short.Parse(_antenatalVisit.BIRTH_PREDICTION));
                    
                    GridCheckMarksSelection gridCheckChiSo = cboMedicalHistoryInternal2.Properties.Tag as GridCheckMarksSelection;
                    if (_antenatalVisit.MEDICAL_HISTORY_INTERNAL != null && cboMedicalHistoryInternal2.Properties.Tag != null)
                    {
                        ProcessSelect(_antenatalVisit.MEDICAL_HISTORY_INTERNAL.ToString(), gridCheckChiSo);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessSelect(string p, GridCheckMarksSelection gridCheckMark)
        {
            try
            {
                List<KeyValueADO> ds = cboMedicalHistoryInternal2.Properties.DataSource as List<KeyValueADO>;
                string[] arrays = p.Split(';');
                if (arrays != null && arrays.Length > 0)
                {
                    List<KeyValueADO> selects = new List<KeyValueADO>();
                    foreach (var item in arrays)
                    {
                        var row = ds != null ? ds.FirstOrDefault(o => o.CODE.ToString() == item) : null;
                        if (row != null)
                        {
                            selects.Add(row);
                            MedicalHistoryInternal2Selected.Add(row);
                        }
                    }
                    gridCheckMark.SelectAll(selects);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
    }
}
