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
using MOS.EFMODEL.DataModels;
using Inventec.Common.Controls.EditorLoader;
using HIS.UC.ExamTreatmentFinish.Run.Validate;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors;
using Inventec.Common.Logging;

namespace HIS.UC.ExamTreatmentFinish.Run
{
    public partial class UCExamTreatmentFinish : UserControl
    {
        private void ValidateForm()
        {
            ValidationTimeFinish();
            //ValidationTimeAppointment();
            ValidationComboProgram();
            ValidateCombo(cboTreatmentEndType);
            ValidateCombo(cboTreatmentResult);
            ValidationICD(10, 500, !this.isAllowNoIcd);
            ValidateComboCareer();
            ValidationControlMaxLength(memNote, 2000);
            
        }

        private void ValidationControlMaxLength(BaseEdit control, int? maxLength)
        {
            ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
            validate.editor = control;
            validate.maxLength = maxLength;
            validate.ErrorText = String.Format(Resources.ResourceMessage.TruongDuLieuVuotQuaKyTu, maxLength);
            validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            this.dxValidationProvider1.SetValidationRule(control, validate);
        }

        private void ValidateComboCareer()
        {
            try
            {
                GridLookupEditValidationRule validationRule = new GridLookupEditValidationRule();
                validationRule.cbo = cboCareer;
                //validationRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validationRule.ErrorText = "Bắt buộc nhập thông tin nghề nghiệp";
                validationRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(cboCareer, validationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidateSignHead()
        {

            try
            {
                GridLookupEditValidationRule validationRule = new GridLookupEditValidationRule();
                validationRule.cbo = cboEndDeptSubs;
                validationRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                //validationRule.ErrorText = "Bắt buộc nhập thông tin nghề nghiệp";
                validationRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(cboEndDeptSubs, validationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateSignDirect()
        {

            try
            {
                GridLookupEditValidationRule validationRule = new GridLookupEditValidationRule();
                validationRule.cbo = cboHospSubs;
                validationRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                //validationRule.ErrorText = "Bắt buộc nhập thông tin nghề nghiệp";
                validationRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(cboHospSubs, validationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidationTimeFinish()
        {
            try
            {
                DateEditValidationRule vali = new DateEditValidationRule();
                vali.dtDateEdit = dtEndTime;
                vali.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                vali.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(dtEndTime, vali);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationComboProgram()
        {
            try
            {
                CboProgramValidationRule vali = new CboProgramValidationRule();
                vali.cboProgram = cboProgram;
                vali.chkDataStore = chkCapSoLuuTruBA;
                //vali.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                vali.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(cboProgram, vali);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void ValidationTimeAppointment()
        //{
        //    try
        //    {
        //        TGHenKhamValidationRule vali = new TGHenKhamValidationRule();
        //        vali.dtTGHenKham = dtTimeAppointment;
        //        vali.dtTGRaVien = dtEndTime;
        //        vali.cboTET = cboTreatmentEndType;
        //        vali.ErrorType = ErrorType.Warning;
        //        dxValidationProvider1.SetValidationRule(dtTimeAppointment, vali);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void ValidateCombo(GridLookUpEdit editor)
        {
            try
            {
                GridLookupEditValidationRule validate = new GridLookupEditValidationRule();
                validate.cbo = editor;
                validate.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validate.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(editor, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public bool ValidateControl(bool IsNotCheckValidateIcdUC)
        {
            bool valid = true;
            try
            {
                this.positionHandle = -1;
                if (!IsNotCheckValidateIcdUC && (Config.HisConfig.CheckIcdWhenSave == "1" || Config.HisConfig.CheckIcdWhenSave == "2"))
                {
                    string messErr = null;
                    if (!checkIcdManager.ProcessCheckIcd(txtIcdCode.Text.Trim(), icdSubCodeScreeen, ref messErr, Config.HisConfig.CheckIcdWhenSave == "1" || Config.HisConfig.CheckIcdWhenSave == "2"))
                    {
                        if (Config.HisConfig.CheckIcdWhenSave == "1")
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(messErr + ". Bạn có muốn tiếp tục?", "Cảnh báo",
                         MessageBoxButtons.YesNo) == DialogResult.No) valid = false;
                        }
                        else
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(messErr, "Cảnh báo",
                         MessageBoxButtons.OK);
                            valid = false;
                        }
                    }
                }
                valid = dxValidationProvider1.Validate() && valid;
                Inventec.Common.Logging.LogSystem.Debug("cboTreatmentEndTypeExt" + cboTreatmentEndTypeExt.EditValue + "___UCExamTreatmentFinish.ValidateControl.valid1=" + valid);
                if (sickSdoResult == null && cboTreatmentEndTypeExt.EditValue != null && (Inventec.Common.TypeConvert.Parse.ToInt64(cboTreatmentEndTypeExt.EditValue.ToString()) == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM
                        || Inventec.Common.TypeConvert.Parse.ToInt64(cboTreatmentEndTypeExt.EditValue.ToString()) == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_DUONG_THAI))
				{
                    valid = false;
                    sickSdoResult = null;
                    frmPopUpSick frm = new frmPopUpSick(sickInitADO, ActionGetSdoSickResult);
                    frm.ShowDialog();
                }                    
                //if (sickProcessor != null && ucSick != null && cboTreatmentEndTypeExt.EditValue!=null)
                //{
                //    valid = sickProcessor.ValidControl(ucSick) && valid;
                //    Inventec.Common.Logging.LogSystem.Debug("UCExamTreatmentFinish.ValidateControl.valid2=" + valid);
                //}

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
    }
}
