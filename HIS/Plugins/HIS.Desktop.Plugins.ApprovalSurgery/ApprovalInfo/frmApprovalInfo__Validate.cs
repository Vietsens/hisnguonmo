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
using ACS.EFMODEL.DataModels;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.Plugins.ApprovalSurgery.ApprovalInfo.ValidationRule;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Desktop.Common.Controls.ValidationRule;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ApprovalSurgery.ApprovalInfo
{
    
    public partial class frmApprovalInfo : Form
    {
        private void ValidateControl()
        {
            try
            {
                ValidationLoginname();
                ValidateTimeApproval();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateTimeApproval()
        {
            try
            {
                ApprovalTimeValidationRule ruleMain = new ApprovalTimeValidationRule();
                ruleMain.time = dtTime;
                ruleMain.action = action;
                ruleMain.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(dtTime, ruleMain);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
			
        }

        private void ValidationLoginname()
        {
            try
            {
                GridLookupEditWithTextEditValidationRule ruleMain = new GridLookupEditWithTextEditValidationRule();
                ruleMain.txtTextEdit = txtLoginname;
                ruleMain.cbo = cboLoginname;
                ruleMain.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                ruleMain.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(txtLoginname, ruleMain);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
