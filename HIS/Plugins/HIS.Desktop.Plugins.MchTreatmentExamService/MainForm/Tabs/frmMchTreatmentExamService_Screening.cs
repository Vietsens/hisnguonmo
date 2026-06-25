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

                // Tab Sàng lọc (loại 5) CHỈ lưu MCH_SCREENING, KHÔNG lưu MCH_CHILD
                // (dữ liệu theo dõi trẻ em dưới 6 tuổi được lưu riêng ở loại 6 - xem GetDataFromTab8)
                _child = null;
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
                // Dữ liệu trẻ em (MCH_CHILD) được fill ở tab Trẻ em dưới 6 tuổi - xem FillDataToTab8
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Tab Trẻ em dưới 6 tuổi (loại 6)

        /// <summary>
        /// Lấy dữ liệu từ tab Trẻ em dưới 6 tuổi (loại 6): header (Ngày khám/Người khám/Trình độ)
        /// + MCH_CHILD. KHÔNG lưu MCH_SCREENING.
        /// </summary>
        private void GetDataFromTab8()
        {
            try
            {
                if (_examService == null) _examService = new MCH_EXAM_SERVICE();

                // MCH_EXAM_SERVICE - header riêng của tab Trẻ em dưới 6 tuổi
                _examService.EXECUTE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteExam8.DateTime) ?? 0;
                _examService.EXECUTE_LOGINNAME = GetComboValue(cboUser8);
                _examService.EXECUTE_USERNAME = GetUserNameByLoginName(_examService.EXECUTE_LOGINNAME);
                _examService.EXECUTE_TYPE = GetComboValue(cboDiploma8);

                // Tab Trẻ em dưới 6 tuổi CHỈ lưu MCH_CHILD, KHÔNG lưu MCH_SCREENING
                _screening = null;

                string weight = GetSpinEditStringValue(spnW1);
                string height = GetSpinEditStringValue(spnH1);
                string headCircum = GetSpinEditStringValue(spnHC1);
                string cccdNumber = GetTextEditValue(txtCccd1);
                var mentalStatusValue = GetRadioGroupValue("MentalStatus");
                var motionStatusValue = GetRadioGroupValue("MotionStatus");

                if (_child == null) _child = new MCH_CHILD();
                _child.MENTAL_STATUS = mentalStatusValue.HasValue ? (mentalStatusValue.Value + 1).ToString() : null;
                _child.MOTION_STATUS = motionStatusValue.HasValue ? (motionStatusValue.Value + 1).ToString() : null;
                _child.WEIGHT = weight;
                _child.HEIGHT = height;
                _child.HEAD_CIRCUM = headCircum;
                _child.CCCD_NUMBER = cccdNumber;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Fill dữ liệu MCH_CHILD + header vào tab Trẻ em dưới 6 tuổi
        /// </summary>
        private void FillDataToTab8()
        {
            try
            {
                if (_examService != null)
                {
                    SetComboValue(cboUser8, _examService.EXECUTE_LOGINNAME);
                    SetComboValue(cboDiploma8, _examService.EXECUTE_TYPE);
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

        /// <summary>
        /// Kiểm tra MCH_CHILD có dữ liệu theo dõi trẻ em hay không
        /// </summary>
        private bool HasChildData()
        {
            try
            {
                if (_child == null) return false;
                return !string.IsNullOrEmpty(_child.WEIGHT)
                    || !string.IsNullOrEmpty(_child.HEIGHT)
                    || !string.IsNullOrEmpty(_child.HEAD_CIRCUM)
                    || !string.IsNullOrEmpty(_child.CCCD_NUMBER)
                    || !string.IsNullOrEmpty(_child.MENTAL_STATUS)
                    || !string.IsNullOrEmpty(_child.MOTION_STATUS);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        #endregion
    }
}
