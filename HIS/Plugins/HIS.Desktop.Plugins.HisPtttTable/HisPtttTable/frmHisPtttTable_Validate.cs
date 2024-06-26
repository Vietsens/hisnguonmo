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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Plugins.HisPtttTable.Validate;
using Inventec.Common.Logging;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors;
using Inventec.Desktop.Common.Controls.ValidationRule;

namespace HIS.Desktop.Plugins.HisPtttTable.HisPtttTable
{
    public partial class frmHisPtttTable : HIS.Desktop.Utility.FormBase
    {
        private void Validate()
        {
            try
            {
                ValidatetxtDocHoldTypeName();
                ValidatetxtDocHoldTypeCode();
                ValidationControlTextEdit(txtExcuteRoomID);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void ValidatetxtDocHoldTypeName()
        {
            try
            {
                ValidateMaxLength_TypeName validate = new ValidateMaxLength_TypeName();
                validate.txtcontrol = txtPtttTableName;
                validate.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validate.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtPtttTableName, validate);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void ValidatetxtDocHoldTypeCode()
        {
            try
            {
                ValiDateMaxLength_TypeCode validate = new ValiDateMaxLength_TypeCode();
                validate.txtcontrol = txtPtttTableCode;
                validate.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validate.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtPtttTableCode, validate);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void ValidationControlTextEdit(TextEdit control)
        {
            try
            {
                ControlEditValidationRule validate = new ControlEditValidationRule();
                validate.editor = control;
                validate.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validate.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}
