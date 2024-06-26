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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Plugins.AccidentHurt.Resources;
using HIS.Desktop.Plugins.AccidentHurt.Validation;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AccidentHurt
{
    public partial class frmAccidentHurt : HIS.Desktop.Utility.FormBase
    {
        internal void ValidationControl()
        {
            try
            {
                //ValidationAccidentLocaltion();
                //ValidationAccidentBodyPart();
                //ValidationAccidentCare();
                //ValidationAccidentHelmet();
                ValidationAccidentHurtType();
                //ValidationAccidentPoison();
                //ValidationAccidentResult();
                //ValidationAccidentVehicle();
                ValidationtxtCdChinh();
              
                ValidationMaxLength(txtTreatmentInfo, 4000);
                ValidationMaxLength(txtStatusIn, 2000);
                ValidationMaxLength(txtStatusOut, 2000);

                ValidationMaxLengthCMND_CCCD(txtCMNDNo);
                ValidationMaxLength(txtCMNDPlace, 100);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
        }

        private void ValidationAccidentLocaltion()
        {
            try
            {
                ComboWithTextEditValidationRule icdMainRule = new ComboWithTextEditValidationRule();
                icdMainRule.txtTextEdit = txtAccidentLocaltion;
                icdMainRule.cbo = cboAccidentLocaltion;
                icdMainRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                icdMainRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(txtAccidentLocaltion, icdMainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationAccidentBodyPart()
        {
            try
            {
                ComboWithTextEditValidationRule icdMainRule = new ComboWithTextEditValidationRule();
                icdMainRule.txtTextEdit = txtAccidentBodyPart;
                icdMainRule.cbo = cboAccidentBodyPart;
                icdMainRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                icdMainRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(txtAccidentBodyPart, icdMainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationAccidentCare()
        {
            try
            {
                ComboWithTextEditValidationRule icdMainRule = new ComboWithTextEditValidationRule();
                icdMainRule.txtTextEdit = txtAccidentCare;
                icdMainRule.cbo = cboAccidentCare;
                icdMainRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                icdMainRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(txtAccidentCare, icdMainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationAccidentHelmet()
        {
            try
            {
                ComboWithTextEditValidationRule icdMainRule = new ComboWithTextEditValidationRule();
                icdMainRule.txtTextEdit = txtHelmet;
                icdMainRule.cbo = cboHelmet;
                icdMainRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                icdMainRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(txtHelmet, icdMainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationAccidentHurtType()
        {
            try
            {
                ComboWithTextEditValidationRule icdMainRule = new ComboWithTextEditValidationRule();
                icdMainRule.txtTextEdit = txtAccidentHurtType;
                icdMainRule.cbo = cboAccidentHurtType;
                icdMainRule.ErrorText = String.Format(ResourceMessage.TruongDuLieuBatBuocPhaiNhap);
                icdMainRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(txtAccidentHurtType, icdMainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationMaxLengthCMND_CCCD(DevExpress.XtraEditors.BaseControl txtEdit)
        {
            try
            {
                ValidateMaxlengthCMNDCCCD cmndCCCD = new ValidateMaxlengthCMNDCCCD();
                cmndCCCD.textEdit = txtCMNDNo;
                this.dxValidationProvider.SetValidationRule(txtEdit, cmndCCCD);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationMaxLength(DevExpress.XtraEditors.BaseControl baseControl, int maxLength)
        {
            try
            {
                ValidateMaxLength maxlenth = new ValidateMaxLength();
                maxlenth.textEdit = baseControl;
                maxlenth.maxLength = maxLength;
                this.dxValidationProvider.SetValidationRule(baseControl, maxlenth);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationAccidentPoison()
        {
            try
            {
                ComboWithTextEditValidationRule icdMainRule = new ComboWithTextEditValidationRule();
                icdMainRule.txtTextEdit = txtAccidentPoison;
                icdMainRule.cbo = cboAccidentPoison;
                icdMainRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                icdMainRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(txtAccidentPoison, icdMainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationAccidentResult()
        {
            try
            {
                ComboWithTextEditValidationRule icdMainRule = new ComboWithTextEditValidationRule();
                icdMainRule.txtTextEdit = txtAccidentResult;
                icdMainRule.cbo = cboAccidentResult;
                icdMainRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                icdMainRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(txtAccidentResult, icdMainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationAccidentVehicle()
        {
            try
            {
                ComboWithTextEditValidationRule icdMainRule = new ComboWithTextEditValidationRule();
                icdMainRule.txtTextEdit = txtAccidentVehicle;
                icdMainRule.cbo = cboAccidentVehicle;
                icdMainRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                icdMainRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(txtAccidentVehicle, icdMainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
