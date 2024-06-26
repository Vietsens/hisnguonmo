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
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using HIS.UC.SecondaryIcd;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Logging;
using HIS.Desktop.Plugins.ApprovalSurgery.ADO;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ApprovalSurgery.CreateCalendar.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.Plugins.ApprovalSurgery.Validate.ValidationRule;
using HIS.Desktop.Utility;
using HIS.Desktop.Plugins.ApprovalSurgery.Validate.ValidateRule;

namespace HIS.Desktop.Plugins.ApprovalSurgery
{
    public partial class UCApprovalSurgery : UserControlBase
    {

        private void ValidateControl()
        {
            try
            {
                ValidationPlanTimeFrom();
                ValidationPlanTimeTo();
                ValidationSingleControl(cboRoom, dxValidationProvider1);
                ValidationControlMaxLength(txtPlanningRequest, 4000,false);
                ValidationControlMaxLength(txtSurgeryNote, 4000,false);
                ValidationControlMaxLength(txtMANNER, 3000, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
			
        }

        private void ValidationPlanTimeFrom()
        {
            try
            {
                TimePlanFromValidationRule mainRule = new TimePlanFromValidationRule();
                mainRule.timePlanFrom = dtPlanTimeFrom;
                mainRule.timePlanTo = dtPlanTimeTo;
                if (ptttCalendar != null)
                {
                    mainRule.timeFrom = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ptttCalendar.TIME_FROM);
                    mainRule.timeTo = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ptttCalendar.TIME_TO);
                }
                
                mainRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(dtPlanTimeFrom, mainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationPlanTimeTo()
        {
            try
            {
                TimePlanToValidationRule mainRule = new TimePlanToValidationRule();
                mainRule.timePlanFrom = dtPlanTimeFrom;
                mainRule.timePlanTo = dtPlanTimeTo;
                if (ptttCalendar != null)
                {
                    mainRule.timeFrom = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ptttCalendar.TIME_FROM);
                    mainRule.timeTo = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ptttCalendar.TIME_TO);
                }

                mainRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(dtPlanTimeTo, mainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlMaxLength(BaseEdit control, int? maxLength, bool isRequired)
        {
            ValidateMaxLength validate = new ValidateMaxLength();
            validate.textEdit = control;
            validate.maxLength = maxLength;
            validate.isRequired = isRequired;
            this.dxValidationProvider1.SetValidationRule(control, validate);
        }
    }
}
