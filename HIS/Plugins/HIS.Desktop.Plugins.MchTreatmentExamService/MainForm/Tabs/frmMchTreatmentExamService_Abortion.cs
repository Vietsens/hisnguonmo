using DevExpress.XtraEditors;
using MCH.EFMODEL.DataModels;
using System;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.MainForm
{
    public partial class UCMchTreatmentExamService : HIS.Desktop.Utility.FormBase
    {
        #region Tab 5: Phá thai

        private void GetDataFromTab5()
        {
            try
            {
                if (_examService == null) _examService = new MCH_EXAM_SERVICE();
                if (_abortion == null) _abortion = new MCH_ABORTION();
                // MCH_EXAM_SERVICE
                _examService.EXECUTE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteExam5.DateTime) ?? 0;
                _examService.EXECUTE_LOGINNAME = GetComboValue(cboUser5);
                _examService.EXECUTE_USERNAME = GetUserNameByLoginName(_examService.EXECUTE_LOGINNAME);
                _examService.EXECUTE_TYPE = GetComboValue(cboDiploma5);
                
                // MCH_ABORTION
                _abortion.GESTATIONAL_WEEKS = GetSpinEditStringValue(spnGestationalWeeks5);
                _abortion.ABORTION_METHOD = GetComboValue(cboAbortionMethod5);
                _abortion.TISSUE_EXAMINATION_RESULT = GetComboValue(cboTissueExaminationResult5);
                
                var abortionComplicationValue = GetRadioGroupValue("AbortionComplication");
                _abortion.ABORTION_COMPLICATION = abortionComplicationValue.HasValue ? abortionComplicationValue.Value.ToString() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToTab5()
        {
            try
            {
                if (_examService != null)
                {
                    SetComboValue(cboUser5, _examService.EXECUTE_LOGINNAME);
                    SetComboValue(cboDiploma5, _examService.EXECUTE_TYPE);
                }

                if (_abortion != null)
                {
                    SetSpinEditStringValue(spnGestationalWeeks5, _abortion.GESTATIONAL_WEEKS);
                    SetComboValue(cboAbortionMethod5, _abortion.ABORTION_METHOD);
                    SetComboValue(cboTissueExaminationResult5, _abortion.TISSUE_EXAMINATION_RESULT);
                    
                    if (!string.IsNullOrEmpty(_abortion.ABORTION_COMPLICATION))
                        SetRadioGroupValue("AbortionComplication", short.Parse(_abortion.ABORTION_COMPLICATION));
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
