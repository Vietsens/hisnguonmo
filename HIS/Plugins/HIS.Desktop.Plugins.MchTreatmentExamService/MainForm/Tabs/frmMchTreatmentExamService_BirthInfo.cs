using DevExpress.XtraEditors;
using HIS.UC.SecondaryIcd.ADO;
using MCH.EFMODEL.DataModels;
using System;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.MainForm
{
    public partial class UCMchTreatmentExamService : HIS.Desktop.Utility.FormBase
    {
        #region Tab 3: Sinh đẻ (Mẹ)

        private void GetDataFromTab3Mother()
        {
            try
            {
                if (_examService == null) _examService = new MCH_EXAM_SERVICE();
                if (_birthInfo == null) _birthInfo = new MCH_BIRTH_INFO();
                // MCH_EXAM_SERVICE
                _examService.EXECUTE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteExam3.DateTime) ?? 0;
                _examService.EXECUTE_LOGINNAME = GetComboValue(cboUser3);
                _examService.EXECUTE_USERNAME = GetUserNameByLoginName(_examService.EXECUTE_LOGINNAME);
                _examService.EXECUTE_TYPE = GetComboValue(cboDiploma3);
                
                // MCH_BIRTH_INFO - Mẹ
                _birthInfo.GESTATIONAL_WEEKS = GetSpinEditStringValue(spnGestationalWeeks3);
                _birthInfo.BORN_TIME = ConvertDateToTimeNumber(dteBornTime3.EditValue, false);
                _birthInfo.BIRTHPLACE_TYPE = GetComboValue(cboBirthplaceType3);

                var ado = addressMother.GetValue();
                _birthInfo.BIRTH_PROVINCE_CODE = ado.Province_Code;
                _birthInfo.BIRTH_DISTRICT_CODE = ado.District_Code;
                _birthInfo.BIRTH_COMMUNE_CODE = ado.Commune_Code;
                _birthInfo.BIRTH_ADDRESS = ado.Address;
                _birthInfo.BIRTH_COMMUNE_NAME = ado.Commune_Name;
                _birthInfo.BIRTH_DISTRICT_NAME = ado.District_Name;
                _birthInfo.BIRTH_PROVINCE_NAME = ado.Province_Name;

                _birthInfo.BIRTH_ORDER = Int64.Parse(GetSpinEditStringValue(spnBirthOrder3));
                _birthInfo.TERM_BIRTHS = GetSpinEditStringValue(spnTermBirths3);
                _birthInfo.PRETERM_BIRTH = GetSpinEditStringValue(spnPretermBirth3);
                _birthInfo.CHILD_COUNT = GetSpinEditStringValue(spnChildCount3);
                _birthInfo.MISCARRIAGE = GetSpinEditStringValue(spnMiscarriage3);
                
                var antenatalVisitsValue = GetRadioGroupValue("AntenatalVisits");
                _birthInfo.ANTENATAL_VISITS = antenatalVisitsValue.HasValue ? antenatalVisitsValue.Value.ToString() : null;
                
                var testHivScreenValue = GetRadioGroupValue("TestHivScreen");
                _birthInfo.TEST_HIV_SCREEN = testHivScreenValue.HasValue ? testHivScreenValue.Value.ToString() : null;
                
                var testHivIntrapartumValue = GetRadioGroupValue("TestHivIntrapartum");
                _birthInfo.TEST_HIV_INTRAPARTUM = testHivIntrapartumValue.HasValue ? testHivIntrapartumValue.Value.ToString() : null;
                
                var testSyphilisScreenValue = GetRadioGroupValue("TestSyphilisScreen");
                _birthInfo.TEST_SYPHILIS_SCREEN = testSyphilisScreenValue.HasValue ? testSyphilisScreenValue.Value.ToString() : null;
                
                var testSyphilisIntrapartumValue = GetRadioGroupValue("TestSyphilisIntrapartum");
                _birthInfo.TEST_SYPHILIS_INTRAPARTUM = testSyphilisIntrapartumValue.HasValue ? testSyphilisIntrapartumValue.Value.ToString() : null;
                
                var testHepbScreenValue = GetRadioGroupValue("TestHepbScreen");
                _birthInfo.TEST_HEPB_SCREEN = testHepbScreenValue.HasValue ? testHepbScreenValue.Value.ToString() : null;
                
                var testHepbIntrapartumValue = GetRadioGroupValue("TestHepbIntrapartum");
                _birthInfo.TEST_HEPB_INTRAPARTUM = testHepbIntrapartumValue.HasValue ? testHepbIntrapartumValue.Value.ToString() : null;
                
                var testGlucoseIntrapartumValue = GetRadioGroupValue("TestGlucoseIntrapartum");
                _birthInfo.TEST_GLUCOSE_INTRAPARTUM = testGlucoseIntrapartumValue.HasValue ? testGlucoseIntrapartumValue.Value.ToString() : null;
                
                var diagnosisGdmValue = GetRadioGroupValue("DiagnosisGdm");
                _birthInfo.DIAGNOSIS_GDM = diagnosisGdmValue.HasValue ? diagnosisGdmValue.Value.ToString() : null;
                
                var fullTetanusDoseValue = GetRadioGroupValue("FullTetanusDose");
                _birthInfo.FULL_TETANUS_DOSE = fullTetanusDoseValue.HasValue ? fullTetanusDoseValue.Value.ToString() : null;
                
                _birthInfo.BIRTH_METHOD = GetComboValue(cboBirthMethod3);
                _birthInfo.MATERNAL_COMPLICATION = GetComboValue(cboMaternalComplication3);
                _birthInfo.NUMBER_NEWBORN_BIRTH = GetSpinEditStringValue(spnNumberNewbornBirth3);
                _birthInfo.NEWBORN_ALIVE = GetSpinEditStringValue(spnNewbornAlive3);
                
                var motherDeathValue = GetRadioGroupValue("MotherDeath");
                _birthInfo.MOTHER_DEATH = motherDeathValue.HasValue ? motherDeathValue.Value.ToString() : null;
                
                _birthInfo.MOTHER_DEATH_CAUSE = memMotherDeathCause3.Text;

                var icdValueSecond = subIcdProcessor.GetValue(this.ucSecondaryIcd);
                if (icdValueSecond != null)
                {
                    _birthInfo.ICD_SUB_CODE = ((SecondaryIcdDataADO)icdValueSecond).ICD_SUB_CODE;
                    _birthInfo.ICD_TEXT = ((SecondaryIcdDataADO)icdValueSecond).ICD_TEXT;
                }
                var firstWeekCareValue = GetRadioGroupValue("FirstWeekCare");
                _birthInfo.FIRST_WEEK_CARE = firstWeekCareValue.HasValue ? firstWeekCareValue.Value.ToString() : null;
                
                var week2To6CareValue = GetRadioGroupValue("Week2To6Care");
                _birthInfo.WEEK_2_TO_6_CARE = week2To6CareValue.HasValue ? week2To6CareValue.Value.ToString() : null;
                _birthInfo.NEWBORN_CONDITION = GetComboValue(cboNewbornCondition3);
                _birthInfo.MIDWIFE_TYPE = GetComboValue(cboDiploma3);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToTab3Mother()
        {
            try
            {
                if (_examService != null)
                {
                    SetComboValue(cboUser3, _examService.EXECUTE_LOGINNAME);
                    SetComboValue(cboDiploma3, _examService.EXECUTE_TYPE);
                }

                if (_birthInfo != null)
                {
                    SetSpinEditStringValue(spnGestationalWeeks3, _birthInfo.GESTATIONAL_WEEKS);
                    
                    if (_birthInfo.BORN_TIME.HasValue)
                    {
                        dteBornTime3.EditValue = ConvertTimeNumberToDate(_birthInfo.BORN_TIME.Value);
                    }

                    addressMother.SetValue(new ADO.UCAddressADO() { Address = _birthInfo.BIRTH_ADDRESS, Commune_Code = _birthInfo.BIRTH_COMMUNE_CODE, Commune_Name = _birthInfo.BIRTH_COMMUNE_NAME, District_Code = _birthInfo.BIRTH_DISTRICT_CODE, District_Name = _birthInfo.BIRTH_DISTRICT_NAME, Province_Code = _birthInfo.BIRTH_PROVINCE_CODE, Province_Name = _birthInfo.BIRTH_PROVINCE_NAME});
                    
                    SetComboValue(cboBirthplaceType3, _birthInfo.BIRTHPLACE_TYPE);
                    SetSpinEditStringValue(spnBirthOrder3, _birthInfo.BIRTH_ORDER != null ? _birthInfo.BIRTH_ORDER.ToString() : null);
                    SetSpinEditStringValue(spnTermBirths3, _birthInfo.TERM_BIRTHS);
                    SetSpinEditStringValue(spnPretermBirth3, _birthInfo.PRETERM_BIRTH);
                    SetSpinEditStringValue(spnChildCount3, _birthInfo.CHILD_COUNT);
                    SetSpinEditStringValue(spnMiscarriage3, _birthInfo.MISCARRIAGE);
                    
                    if (!string.IsNullOrEmpty(_birthInfo.ANTENATAL_VISITS))
                        SetRadioGroupValue("AntenatalVisits", short.Parse(_birthInfo.ANTENATAL_VISITS));
                    
                    if (!string.IsNullOrEmpty(_birthInfo.TEST_HIV_SCREEN))
                        SetRadioGroupValue("TestHivScreen", short.Parse(_birthInfo.TEST_HIV_SCREEN));
                    
                    if (!string.IsNullOrEmpty(_birthInfo.TEST_HIV_INTRAPARTUM))
                        SetRadioGroupValue("TestHivIntrapartum", short.Parse(_birthInfo.TEST_HIV_INTRAPARTUM));
                    
                    if (!string.IsNullOrEmpty(_birthInfo.TEST_SYPHILIS_SCREEN))
                        SetRadioGroupValue("TestSyphilisScreen", short.Parse(_birthInfo.TEST_SYPHILIS_SCREEN));
                    
                    if (!string.IsNullOrEmpty(_birthInfo.TEST_SYPHILIS_INTRAPARTUM))
                        SetRadioGroupValue("TestSyphilisIntrapartum", short.Parse(_birthInfo.TEST_SYPHILIS_INTRAPARTUM));
                    
                    if (!string.IsNullOrEmpty(_birthInfo.TEST_HEPB_SCREEN))
                        SetRadioGroupValue("TestHepbScreen", short.Parse(_birthInfo.TEST_HEPB_SCREEN));
                    
                    if (!string.IsNullOrEmpty(_birthInfo.TEST_HEPB_INTRAPARTUM))
                        SetRadioGroupValue("TestHepbIntrapartum", short.Parse(_birthInfo.TEST_HEPB_INTRAPARTUM));
                    
                    if (!string.IsNullOrEmpty(_birthInfo.TEST_GLUCOSE_INTRAPARTUM))
                        SetRadioGroupValue("TestGlucoseIntrapartum", short.Parse(_birthInfo.TEST_GLUCOSE_INTRAPARTUM));
                    
                    if (!string.IsNullOrEmpty(_birthInfo.DIAGNOSIS_GDM))
                        SetRadioGroupValue("DiagnosisGdm", short.Parse(_birthInfo.DIAGNOSIS_GDM));
                    
                    if (!string.IsNullOrEmpty(_birthInfo.FULL_TETANUS_DOSE))
                        SetRadioGroupValue("FullTetanusDose", short.Parse(_birthInfo.FULL_TETANUS_DOSE));
                    
                    SetComboValue(cboBirthMethod3, _birthInfo.BIRTH_METHOD);
                    SetComboValue(cboMaternalComplication3, _birthInfo.MATERNAL_COMPLICATION);
                    SetSpinEditStringValue(spnNumberNewbornBirth3, _birthInfo.NUMBER_NEWBORN_BIRTH);
                    SetSpinEditStringValue(spnNewbornAlive3, _birthInfo.NEWBORN_ALIVE);
                    
                    if (!string.IsNullOrEmpty(_birthInfo.MOTHER_DEATH))
                        SetRadioGroupValue("MotherDeath", short.Parse(_birthInfo.MOTHER_DEATH));
                    
                    memMotherDeathCause3.Text = _birthInfo.MOTHER_DEATH_CAUSE;
                    SecondaryIcdDataADO subIcd = new SecondaryIcdDataADO();
                    subIcd.ICD_SUB_CODE = _birthInfo.ICD_SUB_CODE;
                    subIcd.ICD_TEXT = _birthInfo.ICD_TEXT;
                    if (ucSecondaryIcd != null)
                    {
                        subIcdProcessor.Reload(ucSecondaryIcd, subIcd);
                    }

                    if (!string.IsNullOrEmpty(_birthInfo.FIRST_WEEK_CARE))
                        SetRadioGroupValue("FirstWeekCare", short.Parse(_birthInfo.FIRST_WEEK_CARE));
                    
                    if (!string.IsNullOrEmpty(_birthInfo.WEEK_2_TO_6_CARE))
                        SetRadioGroupValue("Week2To6Care", short.Parse(_birthInfo.WEEK_2_TO_6_CARE));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Tab 3: Sinh đẻ (Con)

        private void GetDataFromTab3Child()
        {
            try
            {
                // Lấy tất cả thông tin trẻ em trước
                var isDeathValue = GetRadioGroupValue("IsDeath");
                var abandonedChildValue = GetRadioGroupValue("AbandonedChild");
                string foundLocation = GetComboValue(cboFoundLocation3);
                // qtcode
                string skinToSkin = GetComboValue(cboSkinToSkin3);
                var liveBirthValue = GetRadioGroupValue("LiveBirth");
                string childStatus = GetComboValue(cboChildStatus3);
                string childName = txtChildName3.Text;
                string childGender = GetComboValue(cboChildGender3);
                string cccdNumber = txtCccdNumber3.Text;
                string weight = GetSpinEditStringValue(spnW3);
                string height = GetSpinEditStringValue(spnH3);
                string headCircum = GetSpinEditStringValue(spnVH3);
                var newbornScreeningValue = GetRadioGroupValue("NewbornScreening");
                var essentialNewbornCareValue = GetRadioGroupValue("EssentialNewbornCare");
                var earlyBreastfeedingValue = GetRadioGroupValue("EarlyBreastfeeding");
                var vitaminK1Value = GetRadioGroupValue("VitaminK1");
                var hepbVaccineValue = GetRadioGroupValue("HepbVaccine");
                var kangarooCareValue = GetRadioGroupValue("KangarooCare");
                var hasBirthCertificateValue = GetRadioGroupValue("HasBirthCertificate");
                string birthCertificateCode = txtBirthCertificateCode3.Text;
                long? birthCertificateDate = ConvertDateToTimeNumber(dteBirthCertificateDate3.EditValue, true);
                var birthCertificateRoundValue = GetRadioGroupValue("BirthCertificateRound");
                long? childBirthDate = ConvertDateToTimeNumber(dteChildBirthDate3.EditValue, false);
                string temporaryHeinCard = txtTemporaryHeinCardNumber3.Text;
                string ethnicCode = GetComboValue(cboEthnic3);
                var careWeek1Value = GetRadioGroupValue("CareWeek1");
                var careWeek2To6Value = GetRadioGroupValue("CareWeek2To6");
                string deliveryAssistant = GetComboValue(cboNewbornCondition3);
                
                var addressBabyAdo = addressBaby.GetValue();
                bool hasAddressBaby = !string.IsNullOrEmpty(addressBabyAdo.Address) ||
                                     !string.IsNullOrEmpty(addressBabyAdo.Province_Code) ||
                                     !string.IsNullOrEmpty(addressBabyAdo.District_Code) ||
                                     !string.IsNullOrEmpty(addressBabyAdo.Commune_Code);

                // Kiểm tra xem có ít nhất MỘT thông tin trẻ em được nhập hay không
                bool hasChildInfo = isDeathValue.HasValue ||
                                   abandonedChildValue.HasValue ||
                                   !string.IsNullOrEmpty(foundLocation) ||
                                   !string.IsNullOrEmpty(skinToSkin) ||
                                   liveBirthValue.HasValue ||
                                   !string.IsNullOrEmpty(childStatus) ||
                                   !string.IsNullOrEmpty(childName) ||
                                   !string.IsNullOrEmpty(childGender) ||
                                   !string.IsNullOrEmpty(cccdNumber) ||
                                   !string.IsNullOrEmpty(weight) ||
                                   !string.IsNullOrEmpty(height) ||
                                   !string.IsNullOrEmpty(headCircum) ||
                                   newbornScreeningValue.HasValue ||
                                   essentialNewbornCareValue.HasValue ||
                                   earlyBreastfeedingValue.HasValue ||
                                   vitaminK1Value.HasValue ||
                                   hepbVaccineValue.HasValue ||
                                   kangarooCareValue.HasValue ||
                                   hasBirthCertificateValue.HasValue ||
                                   !string.IsNullOrEmpty(birthCertificateCode) ||
                                   birthCertificateDate.HasValue ||
                                   birthCertificateRoundValue.HasValue ||
                                   hasAddressBaby ||
                                   childBirthDate.HasValue ||
                                   !string.IsNullOrEmpty(temporaryHeinCard) ||
                                   !string.IsNullOrEmpty(ethnicCode) ||
                                   careWeek1Value.HasValue ||
                                   careWeek2To6Value.HasValue ||
                                   !string.IsNullOrEmpty(deliveryAssistant);

                // Chỉ tạo và lưu _child khi có thông tin
                if (hasChildInfo)
                {
                    if (_child == null) _child = new MCH_CHILD();
                    
                    // MCH_CHILD
                    _child.IS_DEATH = isDeathValue;
                    _child.ABANDONED_CHILD = abandonedChildValue.HasValue ? abandonedChildValue.Value.ToString() : null;
                    _child.FOUND_LOCATION = foundLocation;
                    _child.SKIN_TO_SKIN = skinToSkin;
                    _child.LIVE_BIRTH = liveBirthValue.HasValue ? liveBirthValue.Value.ToString() : null;
                    _child.CHILD_STATUS = childStatus;
                    _child.CHILD_NAME = childName;
                    _child.CHILD_GENDER = childGender;
                    _child.CCCD_NUMBER = cccdNumber;
                    _child.WEIGHT = weight;
                    _child.HEIGHT = height;
                    _child.HEAD_CIRCUM = headCircum;
                    _child.NEWBORN_SCREENING = newbornScreeningValue.HasValue ? newbornScreeningValue.Value.ToString() : null;
                    _child.ESSENTIAL_NEWBORN_CARE = essentialNewbornCareValue.HasValue ? essentialNewbornCareValue.Value.ToString() : null;
                    _child.EARLY_BREASTFEEDING = earlyBreastfeedingValue.HasValue ? earlyBreastfeedingValue.Value.ToString() : null;
                    _child.VITAMIN_K1 = vitaminK1Value.HasValue ? vitaminK1Value.Value.ToString() : null;
                    _child.HEPB_VACCINE = hepbVaccineValue.HasValue ? hepbVaccineValue.Value.ToString() : null;
                    _child.KANGAROO_CARE = kangarooCareValue.HasValue ? kangarooCareValue.Value.ToString() : null;
                    _child.HAS_BIRTH_CERTIFICATE = hasBirthCertificateValue.HasValue ? hasBirthCertificateValue.Value.ToString() : null;
                    _child.BIRTH_CERTIFICATE_CODE = birthCertificateCode;
                    _child.BIRTH_CERTIFICATE_DATE = birthCertificateDate;
                    _child.BIRTH_CERTIFICATE_ROUND = birthCertificateRoundValue.HasValue ? birthCertificateRoundValue.Value.ToString() : null;

                    _child.BIRTH_PROVINCE_CODE = addressBabyAdo.Province_Code;
                    _child.BIRTH_DISTRICT_CODE = addressBabyAdo.District_Code;
                    _child.BIRTH_COMMUNE_CODE = addressBabyAdo.Commune_Code;
                    _child.BIRTH_ADDRESS = addressBabyAdo.Address;
                    _child.BIRTH_COMMUNE_NAME = addressBabyAdo.Commune_Name;
                    _child.BIRTH_DISTRICT_NAME = addressBabyAdo.District_Name;
                    _child.BIRTH_PROVINCE_NAME = addressBabyAdo.Province_Name;
                    _child.CHILD_BIRTH_DATE = childBirthDate;
                    _child.TEMPORARY_HEIN_CARD_NUMBER = temporaryHeinCard;
                    _child.ETHNIC_CODE = ethnicCode;
                    _child.ETHNIC_NAME = GetEthnicNameByCode(_child.ETHNIC_CODE);
                    _child.CARE_WEEK_1 = careWeek1Value.HasValue ? careWeek1Value.Value.ToString() : null;
                    _child.CARE_WEEK_2_TO_6 = careWeek2To6Value.HasValue ? careWeek2To6Value.Value.ToString() : null;
                    _child.DELIVERY_ASSISTANT = deliveryAssistant;
                    
                    Inventec.Common.Logging.LogSystem.Debug("GetDataFromTab3Child: Child info detected, _child will be saved");
                }
                else
                {
                    // Không có thông tin trẻ em, set _child = null để không lưu
                    _child = null;
                    Inventec.Common.Logging.LogSystem.Debug("GetDataFromTab3Child: No child info, _child set to null");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToTab3Child()
        {
            try
            {
                if (_child != null)
                {
                    if (_child.IS_DEATH.HasValue)
                        SetRadioGroupValue("IsDeath", _child.IS_DEATH ?? 0);
                    
                    if (!string.IsNullOrEmpty(_child.ABANDONED_CHILD))
                        SetRadioGroupValue("AbandonedChild", short.Parse(_child.ABANDONED_CHILD));
                    
                    SetComboValue(cboFoundLocation3, _child.FOUND_LOCATION);
                    
                    if (!string.IsNullOrEmpty(_child.LIVE_BIRTH))
                        SetRadioGroupValue("LiveBirth", short.Parse(_child.LIVE_BIRTH));
                    
                    SetComboValue(cboChildStatus3, _child.CHILD_STATUS);
                    SetComboValue(cboSkinToSkin3, _child.SKIN_TO_SKIN);
                    txtChildName3.Text = _child.CHILD_NAME;
                    SetComboValue(cboChildGender3, _child.CHILD_GENDER);
                    txtCccdNumber3.Text = _child.CCCD_NUMBER;
                    
                    SetSpinEditStringValue(spnW3, _child.WEIGHT);
                    SetSpinEditStringValue(spnH3, _child.HEIGHT);
                    SetSpinEditStringValue(spnVH3, _child.HEAD_CIRCUM);
                    
                    if (!string.IsNullOrEmpty(_child.NEWBORN_SCREENING))
                        SetRadioGroupValue("NewbornScreening", short.Parse(_child.NEWBORN_SCREENING));
                    
                    if (!string.IsNullOrEmpty(_child.ESSENTIAL_NEWBORN_CARE))
                        SetRadioGroupValue("EssentialNewbornCare", short.Parse(_child.ESSENTIAL_NEWBORN_CARE));
                    
                    if (!string.IsNullOrEmpty(_child.EARLY_BREASTFEEDING))
                        SetRadioGroupValue("EarlyBreastfeeding", short.Parse(_child.EARLY_BREASTFEEDING));
                    
                    if (!string.IsNullOrEmpty(_child.VITAMIN_K1))
                        SetRadioGroupValue("VitaminK1", short.Parse(_child.VITAMIN_K1));
                    
                    if (!string.IsNullOrEmpty(_child.HEPB_VACCINE))
                        SetRadioGroupValue("HepbVaccine", short.Parse(_child.HEPB_VACCINE));
                    
                    if (!string.IsNullOrEmpty(_child.KANGAROO_CARE))
                        SetRadioGroupValue("KangarooCare", short.Parse(_child.KANGAROO_CARE));
                    
                    if (!string.IsNullOrEmpty(_child.HAS_BIRTH_CERTIFICATE))
                        SetRadioGroupValue("HasBirthCertificate", short.Parse(_child.HAS_BIRTH_CERTIFICATE));
                    
                    txtBirthCertificateCode3.Text = _child.BIRTH_CERTIFICATE_CODE;
                    
                    if (_child.BIRTH_CERTIFICATE_DATE.HasValue)
                    {
                        dteBirthCertificateDate3.EditValue = ConvertTimeNumberToDate(_child.BIRTH_CERTIFICATE_DATE.Value);
                    }
                    
                    if (!string.IsNullOrEmpty(_child.BIRTH_CERTIFICATE_ROUND))
                        SetRadioGroupValue("BirthCertificateRound", short.Parse(_child.BIRTH_CERTIFICATE_ROUND));
                    
                    if (_child.CHILD_BIRTH_DATE.HasValue)
                    {
                        dteChildBirthDate3.EditValue = ConvertTimeNumberToDate(_child.CHILD_BIRTH_DATE.Value);
                    }
                    addressBaby.SetValue(new ADO.UCAddressADO() { Address = _child.BIRTH_ADDRESS, Commune_Code = _child.BIRTH_COMMUNE_CODE, Commune_Name = _child.BIRTH_COMMUNE_NAME, District_Code = _child.BIRTH_DISTRICT_CODE, District_Name = _child.BIRTH_DISTRICT_NAME, Province_Code = _child.BIRTH_PROVINCE_CODE, Province_Name = _child.BIRTH_PROVINCE_NAME });
                    txtTemporaryHeinCardNumber3.Text = _child.TEMPORARY_HEIN_CARD_NUMBER;
                    SetComboValue(cboEthnic3, _child.ETHNIC_CODE);
                    
                    if (!string.IsNullOrEmpty(_child.CARE_WEEK_1))
                        SetRadioGroupValue("CareWeek1", short.Parse(_child.CARE_WEEK_1));
                    
                    if (!string.IsNullOrEmpty(_child.CARE_WEEK_2_TO_6))
                        SetRadioGroupValue("CareWeek2To6", short.Parse(_child.CARE_WEEK_2_TO_6));
                    
                    SetComboValue(cboNewbornCondition3, _child.DELIVERY_ASSISTANT);
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
