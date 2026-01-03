using DevExpress.XtraEditors;
using MCH.EFMODEL.DataModels;
using System;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.MainForm
{
    public partial class UCMchTreatmentExamService : HIS.Desktop.Utility.FormBase
    {
        #region Tab 1: Sàng lọc ung thư cổ tử cung

        private void GetDataFromTab1()
        {
            try
            {
                if (_examService == null) _examService = new MCH_EXAM_SERVICE();
                if (_screening == null) _screening = new MCH_SCREENING();
                // MCH_EXAM_SERVICE
                _examService.EXECUTE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteExam1.DateTime) ?? 0;
                _examService.EXECUTE_LOGINNAME = GetComboValue(cboUser1);
                _examService.EXECUTE_USERNAME = GetUserNameByLoginName(_examService.EXECUTE_LOGINNAME);
                _examService.EXECUTE_TYPE = GetComboValue(cboDiploma1);
                
                // MCH_SCREENING
                var screenPurpose = GetRadioGroupValue("ScreenPurpose");
                _screening.SCREENING_PURPOSE = screenPurpose.HasValue ? (screenPurpose.Value + 1).ToString() : null;
                
                var gynecologyValue = GetRadioGroupValue("Gynecology");
                _screening.GYNECOLOGY_TREAT = gynecologyValue.HasValue ? gynecologyValue.Value.ToString() : null;
                
                var viaViliTestValue = GetRadioGroupValue("ViaViliTest");
                _screening.VIA_VILI_TEST = viaViliTestValue.HasValue ? viaViliTestValue.Value.ToString() : null;
                
                var cytologyTestValue = GetRadioGroupValue("CytologyTest");
                _screening.CYTOLOGY_TEST = cytologyTestValue.HasValue ? cytologyTestValue.Value.ToString() : null;
                
                var hpvTestValue = GetRadioGroupValue("HpvTest");
                _screening.HPV_TEST = hpvTestValue.HasValue ? hpvTestValue.Value.ToString() : null;
                
                var breastExamValue = GetRadioGroupValue("BreastExam");
                _screening.BREAST_EXAM = breastExamValue.HasValue ? breastExamValue.Value.ToString() : null;
                
                var breastUltraSoundValue = GetRadioGroupValue("BreastUltraSound");
                _screening.BREAST_ULTRASOUND = breastUltraSoundValue.HasValue ? breastUltraSoundValue.Value.ToString() : null;
                
                var mamographyValue = GetRadioGroupValue("Mamography");
                _screening.MAMMOGRAPHY = mamographyValue.HasValue ? mamographyValue.Value.ToString() : null;
                
                _screening.CERVICAL_CANCER_DX = GetComboValue(cboCervicalCancerDx1);
                _screening.PRE_CERVICAL_CANCER_TREAT = GetComboValue(cboPreCervicalCancerTreat1);
                
                // MCH_CHILD - Phát triển trẻ
                // Lấy dữ liệu trẻ em
                string weight = GetSpinEditStringValue(spnW1);
                string height = GetSpinEditStringValue(spnH1);
                string headCircum = GetSpinEditStringValue(spnHC1);
                string cccdNumber = GetTextEditValue(txtCccd1);
                var mentalStatusValue = GetRadioGroupValue("MentalStatus");
                var motionStatusValue = GetRadioGroupValue("MotionStatus");
                
                // Kiểm tra xem có ít nhất một thông tin trẻ em được nhập hay không
                bool hasChildInfo = !string.IsNullOrEmpty(weight) ||
                                   !string.IsNullOrEmpty(height) ||
                                   !string.IsNullOrEmpty(headCircum) ||
                                   !string.IsNullOrEmpty(cccdNumber) ||
                                   mentalStatusValue.HasValue ||
                                   motionStatusValue.HasValue;
                
                // Chỉ tạo và lưu _child khi có thông tin
                if (hasChildInfo)
                {
                    if (_child == null) _child = new MCH_CHILD();
                    
                    _child.MENTAL_STATUS = mentalStatusValue.HasValue ? (mentalStatusValue.Value + 1).ToString() : null;
                    _child.MOTION_STATUS = motionStatusValue.HasValue ? (motionStatusValue.Value + 1).ToString() : null;
                    _child.WEIGHT = weight;
                    _child.HEIGHT = height;
                    _child.HEAD_CIRCUM = headCircum;
                    _child.CCCD_NUMBER = cccdNumber;
                    
                    Inventec.Common.Logging.LogSystem.Debug("GetDataFromTab1: Child info detected, _child will be saved");
                }
                else
                {
                    // Không có thông tin trẻ em, set _child = null để không lưu
                    _child = null;
                    Inventec.Common.Logging.LogSystem.Debug("GetDataFromTab1: No child info, _child set to null");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToTab1()
        {
            try
            {
                if (_examService != null)
                {
                    SetComboValue(cboUser1, _examService.EXECUTE_LOGINNAME);
                    SetComboValue(cboDiploma1, _examService.EXECUTE_TYPE);
                }

                if (_screening != null)
                {
                    if (!string.IsNullOrEmpty(_screening.SCREENING_PURPOSE))
                    {
                        short index = (short)(short.Parse(_screening.SCREENING_PURPOSE) - 1);
                        SetRadioGroupValue("ScreenPurpose", index);
                    }
                    
                    if (!string.IsNullOrEmpty(_screening.GYNECOLOGY_TREAT))
                        SetRadioGroupValue("Gynecology", short.Parse(_screening.GYNECOLOGY_TREAT));
                    
                    if (!string.IsNullOrEmpty(_screening.VIA_VILI_TEST))
                        SetRadioGroupValue("ViaViliTest", short.Parse(_screening.VIA_VILI_TEST));
                    
                    if (!string.IsNullOrEmpty(_screening.CYTOLOGY_TEST))
                        SetRadioGroupValue("CytologyTest", short.Parse(_screening.CYTOLOGY_TEST));
                    
                    if (!string.IsNullOrEmpty(_screening.HPV_TEST))
                        SetRadioGroupValue("HpvTest", short.Parse(_screening.HPV_TEST));
                    
                    if (!string.IsNullOrEmpty(_screening.BREAST_EXAM))
                        SetRadioGroupValue("BreastExam", short.Parse(_screening.BREAST_EXAM));
                    
                    if (!string.IsNullOrEmpty(_screening.BREAST_ULTRASOUND))
                        SetRadioGroupValue("BreastUltraSound", short.Parse(_screening.BREAST_ULTRASOUND));
                    
                    if (!string.IsNullOrEmpty(_screening.MAMMOGRAPHY))
                        SetRadioGroupValue("Mamography", short.Parse(_screening.MAMMOGRAPHY));
                    
                    SetComboValue(cboCervicalCancerDx1, _screening.CERVICAL_CANCER_DX);
                    SetComboValue(cboPreCervicalCancerTreat1, _screening.PRE_CERVICAL_CANCER_TREAT);
                }
                
                if (_child != null)
                {
                    if (!string.IsNullOrEmpty(_child.MENTAL_STATUS))
                        SetRadioGroupValue("MentalStatus", (short?)(short.Parse(_child.MENTAL_STATUS) - 1));
                    
                    if (!string.IsNullOrEmpty(_child.MOTION_STATUS))
                        SetRadioGroupValue("MotionStatus", (short?)(short.Parse(_child.MOTION_STATUS) - 1));
                    
                    SetSpinEditStringValue(spnW1, _child.WEIGHT);
                    SetSpinEditStringValue(spnH1, _child.HEIGHT);
                    SetSpinEditStringValue(spnHC1, _child.HEAD_CIRCUM);
                    SetTextEditValue(txtCccd1, _child.CCCD_NUMBER);
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
