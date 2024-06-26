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
using DevExpress.Data;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using Inventec.Common.Adapter;
using Inventec.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;
using MOS.Filter;
using DevExpress.XtraEditors;
using HIS.Desktop.Plugins.ServiceExecuteGroup.Validation;
using Inventec.Desktop.Common.Controls.ValidationRule;

namespace HIS.Desktop.Plugins.ServiceExecuteGroup.Run
{
	public partial class frmServiceExecuteGroup
	{
        private void SetValidateForm()
        {
            try
            {
                //ValidationControlMaxLength(this.txtCccdCmnd, 12, true);
                ValidationControlMaxLength(this.memDescription, 4000);
                ValidationControlMaxLength(this.memConclude, 4000);
                ValidationControlMaxLength(this.memNote, 3000);
                ValidationDateTime(dteStart);
                ValidationDateTime(dteEnd);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidationControlMaxLength(BaseEdit control, int? maxLength, bool IsRequest = false)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxLength;
                validate.IsRequired = IsRequest;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationDateTime(DevExpress.XtraEditors.DateEdit control)
        {
            try
            {
                ExpiredDateValidationRule validate = new ExpiredDateValidationRule();
                validate.dtExpiredDate = control;
                validate.ErrorText = "Trường dữ liệu bắt buộc";
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
