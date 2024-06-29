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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.DHST.ADO;
using HIS.UC.DHST.Base;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.UC.DHST.Validate;

namespace HIS.UC.DHST.Run
{
    public partial class UCDHST : UserControl
    {
        public void ValidateControl()
        {
            try
            {
                if (dhstInit == null) return;

                if (dhstInit.IsRequired)
                {
                    layoutControlItem1.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciCanNang.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciChieuCao.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciHuyetAp.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciMach.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciNhietDo.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciNhipTho.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciVongBung.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciVongNguc.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lblSPO2.AppearanceItemCaption.ForeColor = Color.Maroon;

                    ValidateSingleControl(dtExecuteTime);
                    ValidateControlSpinEdit(spinPulse);
                    ValidateControlSpinBloodPressure(spinBloodPressureMax);
                    ValidateControlSpinBloodPressure(spinBloodPressureMin);
                    ValidateControlSpinEdit(spinWeight);
                    ValidateControlSpinEdit(spinHeight);
                    ValidateControlSpinEdit(spinTemperature);
                    ValidateControlSpinEdit(spinBreathRate);
                    ValidateControlSpinEdit(spinChest);
                    ValidateControlSpinEdit(spinBelly);
                    ValidateControlSpinEdit(spinSPO2, IsValidControlSPO2, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.NguoiDungNhapDuLieuKhongHopLe));
                    ValidateControlMaxLength(txtNote, 4000);
                    ValidateControlSpinEdit(spinUrine);
                    ValidateControlSpinEdit(spinCapillaryBloodGlucose);
                }
                else if (dhstInit.IsRequiredWeight)
                {
                    lciCanNang.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidateControlSpinEdit(spinWeight);
                }

                //Cấu hình bắt buộc mạch huyết áp ở xử lý khám
                string requiredPulseBloodPressure = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigKeys.REQUIRED_PULSE_BLOOD_PRESSURE);
                if (requiredPulseBloodPressure == "1"
                    || (requiredPulseBloodPressure == "2" && dhstInit.IsThan16YearOld))
                {
                    lciMach.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciHuyetAp.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidateControlSpinEdit(spinPulse);
                    ValidateControlSpinBloodPressure(spinBloodPressureMax);
                    ValidateControlSpinBloodPressure(spinBloodPressureMin);
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

        private void ValidateControlSpinEdit(DevExpress.XtraEditors.SpinEdit control)
        {
            try
            {
                ControlEditValidationRule controlEdit = new ControlEditValidationRule();
                controlEdit.editor = control;
                controlEdit.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                controlEdit.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(control, controlEdit);
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
                this.dxValidationProvider1.SetValidationRule(control, controlEdit);
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
                this.dxValidationProvider1.SetValidationRule(control, controlEdit);
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
                this.dxValidationProvider1.SetValidationRule(control, controlEdit);
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
                this.dxValidationProvider1.SetValidationRule(control, controlEdit);
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
                this.dxValidationProvider1.SetValidationRule(spinBloodPressureMax, controlEdit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
