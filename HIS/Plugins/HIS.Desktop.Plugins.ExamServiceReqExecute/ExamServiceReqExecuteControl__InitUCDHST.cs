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
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Base;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Config;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Validate.ValidateRule;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl : UserControlBase
    {
        private void UcDHSTFocusComtrol()
        {
            try
            {
                dtExecuteTime.Focus();
                dtExecuteTime.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public object UcDHSTGetValue()
        {
            object result = null;
            try
            {
                DHSTADO outPut = new DHSTADO();
                if (dtExecuteTime.EditValue != null)
                    outPut.EXECUTE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtExecuteTime.DateTime);
                if (spinBloodPressureMax.EditValue != null)
                    outPut.BLOOD_PRESSURE_MAX = Inventec.Common.TypeConvert.Parse.ToInt64(spinBloodPressureMax.Value.ToString());
                if (spinBloodPressureMin.EditValue != null)
                    outPut.BLOOD_PRESSURE_MIN = Inventec.Common.TypeConvert.Parse.ToInt64(spinBloodPressureMin.Value.ToString());
                if (spinBreathRate.EditValue != null)
                    outPut.BREATH_RATE = Inventec.Common.Number.Get.RoundCurrency(spinBreathRate.Value, 2);
                if (spinHeight.EditValue != null)
                    outPut.HEIGHT = Inventec.Common.Number.Get.RoundCurrency(spinHeight.Value, 2);
                if (spinChest.EditValue != null)
                    outPut.CHEST = Inventec.Common.Number.Get.RoundCurrency(spinChest.Value, 2);
                if (spinBelly.EditValue != null)
                    outPut.BELLY = Inventec.Common.Number.Get.RoundCurrency(spinBelly.Value, 2);
                if (spinPulse.EditValue != null)
                    outPut.PULSE = Inventec.Common.TypeConvert.Parse.ToInt64(spinPulse.Value.ToString());
                if (spinTemperature.EditValue != null)
                    outPut.TEMPERATURE = Inventec.Common.Number.Get.RoundCurrency(spinTemperature.Value, 2);
                if (spinWeight.EditValue != null)
                    outPut.WEIGHT = Inventec.Common.Number.Get.RoundCurrency(spinWeight.Value, 2);
                if (spinSPO2.EditValue != null)
                    outPut.SPO2 = Inventec.Common.Number.Get.RoundCurrency(spinSPO2.Value, 2) / 100;
                outPut.NOTE = txtNote.Text.Trim();

                outPut.IsVali = true;
                ////if (dhstInit.IsRequired || dhstInit.IsRequiredWeight)
                ////{
                //this.positionHandle = -1;
                //if (!dxValidationProvider1.Validate())
                //{
                //    outPut.IsVali = false;
                //}
                //// }

                if (!CheckDhst(outPut))//có dữ liệu mới tạo
                {
                    result = outPut;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckDhst(DHSTADO data)
        {
            bool result = true;
            try
            {
                result = result && !data.PULSE.HasValue;
                result = result && !data.BLOOD_PRESSURE_MAX.HasValue;
                result = result && !data.BLOOD_PRESSURE_MIN.HasValue;
                result = result && !data.TEMPERATURE.HasValue;
                result = result && !data.BREATH_RATE.HasValue;
                result = result && !data.HEIGHT.HasValue;
                result = result && !data.WEIGHT.HasValue;
                result = result && !data.CHEST.HasValue;
                result = result && !data.BELLY.HasValue;
                result = result && !data.CAPILLARY_BLOOD_GLUCOSE.HasValue;
                result = result && !data.SPO2.HasValue;
                //result = result && (dtExecuteTimeDhst.EditValue == null || dtExecuteTimeDhst.DateTime == DateTime.MinValue);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            Inventec.Common.Logging.LogSystem.Info("CheckDhst: " + result);
            return result;
        }

        public async Task UcDHSTValidateControl()
        {
            try
            {
                bool isRequired = false, isThan16YearOld = false, isRequiredWeight = false, lessthan16YearOld = false; ;
                if (this.requiredControl == 1 && (isChronic || this.treatment != null && this.treatment.IS_CHRONIC == 1))
                {
                    isRequired = true;
                }
                Inventec.Common.Logging.LogSystem.Debug("UcDHSTValidateControl." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isRequired), isRequired));
                if ((IsLessThan1YearOldOr6YearOld(this.HisServiceReqView.TDL_PATIENT_DOB)))
                {
                    CommonParam param = new CommonParam();
                    HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
                    filter.TreatmentId = treatmentId;
                    filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                    var currentHisPatientTypeAlter = await new BackendAdapter(param).GetAsync<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, filter, param);
                    if (currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT || currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__IS_FEE)
                        isRequiredWeight = true;
                }
                if (IsThan16YearOld(this.HisServiceReqView.TDL_PATIENT_DOB))
                {
                    isThan16YearOld = true;
                }
                if (IsThan16YearOldByTreatment())
                {
                    lessthan16YearOld = true;
                }
                ValidateControlSpinEditWeight(spinWeight, false, false);
                if (isRequired)
                {
                    lciExecuteTime.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciWeight.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciHeight.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciBloodPressure.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciPulse.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciTemperature.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciBreathRate.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciBelly.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciChest.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciSpo2.AppearanceItemCaption.ForeColor = Color.Maroon;

                    ValidateSingleControl(dtExecuteTime);
                    ValidateControlSpinEdit(spinPulse);
                    ValidateControlSpinBloodPressure(spinBloodPressureMax);
                    ValidateControlSpinBloodPressure(spinBloodPressureMin);
                    ValidateControlSpinEditWeight(spinWeight, false, true);
                    ValidateControlSpinEdit(spinHeight);
                    ValidateControlSpinEdit(spinTemperature);
                    ValidateControlSpinEdit(spinBreathRate);
                    ValidateControlSpinEdit(spinChest);
                    ValidateControlSpinEdit(spinBelly);
                    ValidateControlSpinEdit(spinSPO2, IsValidControlSPO2, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.NguoiDungNhapDuLieuKhongHopLe));
                    ValidateControlMaxLength(txtNote, 4000);
                }
                else if (isRequiredWeight)
                {
                    lciWeight.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidateControlSpinEditWeight(spinWeight, true, true);
                }


                //Cấu hình bắt buộc mạch huyết áp ở xử lý khám
                string requiredPulseBloodPressure = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.REQUIRED_PULSE_BLOOD_PRESSURE);
                if (requiredPulseBloodPressure == "1"
                    || (requiredPulseBloodPressure == "2" && isThan16YearOld))
                {
                    lciPulse.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciBloodPressure.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidateControlSpinEdit(spinPulse);
                    ValidateControlSpinBloodPressure(spinBloodPressureMax);
                    ValidateControlSpinBloodPressure(spinBloodPressureMin);
                }
                if (HisConfigCFG.IsRequiredTemperatureOption && lessthan16YearOld)
                {
                    lciTemperature.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool IsValidControlSPO2()
        {
            bool valid = false;
            try
            {
                if (spinSPO2.EditValue != null)
                {
                    valid = (spinSPO2.Value > 0 && spinSPO2.Value <= 100);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private void ValidateControlSpinEditWeight(DevExpress.XtraEditors.SpinEdit control, bool Option, bool IsRequired)
        {
            try
            {
                WeightValidaionRule controlEdit = new WeightValidaionRule();
                controlEdit.spn = control;
                controlEdit.IsRequired = IsRequired;
                controlEdit.IsRequiredWeightOption = Option ? IsRequiredWeightOption : 0;
                this.dxValidationProviderForLeftPanel.SetValidationRule(control, controlEdit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void ValidateControlSpinEdit(DevExpress.XtraEditors.SpinEdit control)
        {
            try
            {
                ControlEditValidationRule controlEdit = new ControlEditValidationRule();
                controlEdit.editor = control;
                controlEdit.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                controlEdit.ErrorType = ErrorType.Warning;
                this.dxValidationProviderForLeftPanel.SetValidationRule(control, controlEdit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateControlMaxLength(DevExpress.XtraEditors.BaseEdit control, int? maxLength)
        {
            try
            {
                ValidateMaxLength controlEdit = new ValidateMaxLength();
                controlEdit.textEdit = control;
                controlEdit.maxLength = maxLength;
                controlEdit.ErrorType = ErrorType.Warning;
                this.dxValidationProviderForLeftPanel.SetValidationRule(control, controlEdit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidateControlSpinEdit(DevExpress.XtraEditors.SpinEdit control, IsValidControl isValidControl, string message)
        {
            try
            {
                ControlEditValidationRule controlEdit = new ControlEditValidationRule();
                controlEdit.editor = control;
                controlEdit.isUseOnlyCustomValidControl = true;
                controlEdit.isValidControl = isValidControl;
                controlEdit.ErrorText = (String.IsNullOrEmpty(message) ? Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap) : message);
                controlEdit.ErrorType = ErrorType.Warning;
                this.dxValidationProviderForLeftPanel.SetValidationRule(control, controlEdit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateSingleControl(DevExpress.XtraEditors.BaseEdit control)
        {
            try
            {
                ControlEditValidationRule controlEdit = new ControlEditValidationRule();
                controlEdit.editor = control;
                controlEdit.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                controlEdit.ErrorType = ErrorType.Warning;
                this.dxValidationProviderForLeftPanel.SetValidationRule(control, controlEdit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateControlSpinBloodPressure(DevExpress.XtraEditors.SpinEdit control)
        {
            try
            {
                ControlEditValidationRule controlEdit = new ControlEditValidationRule();
                controlEdit.editor = control;
                controlEdit.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                controlEdit.ErrorType = ErrorType.Warning;
                this.dxValidationProviderForLeftPanel.SetValidationRule(control, controlEdit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateControlPressureMin()
        {
            try
            {
                ControlEditValidationRule controlEdit = new ControlEditValidationRule();
                controlEdit.editor = spinBloodPressureMin;
                controlEdit.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                controlEdit.ErrorType = ErrorType.Warning;
                this.dxValidationProviderForLeftPanel.SetValidationRule(spinBloodPressureMax, controlEdit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DHSTSetValue(HIS_DHST dhst)
        {
            try
            {
                if (dhst != null)
                {
                    isLoadedMLCT = true;
                    if (dhst.EXECUTE_TIME != null)
                        dtExecuteTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dhst.EXECUTE_TIME ?? 0) ?? DateTime.Now;
                    else
                        dtExecuteTime.EditValue = DateTime.Now;
                    spinBloodPressureMax.EditValue = dhst.BLOOD_PRESSURE_MAX;
                    spinBloodPressureMin.EditValue = dhst.BLOOD_PRESSURE_MIN;
                    spinBreathRate.EditValue = dhst.BREATH_RATE;
                    spinHeight.EditValue = dhst.HEIGHT;
                    spinChest.EditValue = dhst.CHEST;
                    spinBelly.EditValue = dhst.BELLY;
                    spinPulse.EditValue = dhst.PULSE;
                    spinTemperature.EditValue = dhst.TEMPERATURE;
                    spinWeight.EditValue = dhst.WEIGHT;
                    if (dhst.SPO2.HasValue)
                        spinSPO2.Value = (dhst.SPO2.Value * 100);
                    else
                        spinSPO2.EditValue = null;
                    txtNote.Text = dhst.NOTE;

                    //LoadMLCT();

                    isLoadedMLCT = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void DHSTSetValueHeightWeight(HIS_DHST dhst)
        {
            try
            {
                if (dhst != null)
                {
                    isLoadedMLCT = true;
                    spinHeight.EditValue = dhst.HEIGHT;
                    spinWeight.EditValue = dhst.WEIGHT;

                    //LoadMLCT();
                    isLoadedMLCT = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void DHSTFillDataToBmiAndLeatherArea()
        {
            try
            {
                decimal bmi = 0;
                if (spinHeight.Value != null && spinHeight.Value != 0)
                {
                    bmi = (spinWeight.Value) / ((spinHeight.Value / 100) * (spinHeight.Value / 100));
                }
                double leatherArea = 0.007184 * Math.Pow((double)spinHeight.Value, 0.725) * Math.Pow((double)spinWeight.Value, 0.425);
                lblBMI.Text = Math.Round(bmi, 2) + "";
                lblLeatherArea.Text = Math.Round(leatherArea, 2) + "";
                //if (bmi < 16)
                //{
                //    lblBmiDisplayText.Text = Inventec.Common.Resource.Get.Value("UCDHST.SKINNY.III", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                //}
                //else if (16 <= bmi && bmi < 17)
                //{
                //    lblBmiDisplayText.Text = Inventec.Common.Resource.Get.Value("UCDHST.SKINNY.II", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                //}
                //else if (17 <= bmi && bmi < (decimal)18.5)
                //{
                //    lblBmiDisplayText.Text = Inventec.Common.Resource.Get.Value("UCDHST.SKINNY.I", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                //}
                //else if ((decimal)18.5 <= bmi && bmi < 25)
                //{
                //    lblBmiDisplayText.Text = Inventec.Common.Resource.Get.Value("UCDHST.BMIDISPLAY.NORMAL", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                //}
                //else if (25 <= bmi && bmi < 30)
                //{
                //    lblBmiDisplayText.Text = Inventec.Common.Resource.Get.Value("UCDHST.BMIDISPLAY.OVERWEIGHT", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                //}
                //else if (30 <= bmi && bmi < 35)
                //{
                //    lblBmiDisplayText.Text = Inventec.Common.Resource.Get.Value("UCDHST.BMIDISPLAY.OBESITY.I", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                //}
                //else if (35 <= bmi && bmi < 40)
                //{
                //    lblBmiDisplayText.Text = Inventec.Common.Resource.Get.Value("UCDHST.BMIDISPLAY.OBESITY.II", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                //}
                //else if (40 < bmi)
                //{
                //    lblBmiDisplayText.Text = Inventec.Common.Resource.Get.Value("UCDHST.BMIDISPLAY.OBESITY.III", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DHSTLoadDataDefault()
        {
            try
            {

                UcDHSTValidateControl();//TODO
                                        //this.dhstProcessor.SetValidate(this.ucDHST, true);
                isLoadedMLCT = true;

                dtExecuteTime.DateTime = DateTime.Now;
                spinBloodPressureMin.EditValue = null;
                spinBloodPressureMax.EditValue = null;
                spinBreathRate.EditValue = null;
                spinHeight.EditValue = null;
                spinChest.EditValue = null;
                spinBelly.EditValue = null;
                spinPulse.EditValue = null;
                spinTemperature.EditValue = null;
                spinWeight.EditValue = null;
                spinSPO2.EditValue = null;
                txtNote.Text = "";
                isLoadedMLCT = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
