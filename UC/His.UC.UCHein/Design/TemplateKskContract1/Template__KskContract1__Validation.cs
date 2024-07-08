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
using His.UC.UCHein.Data;
using His.UC.UCHein;
using System.Globalization;
using His.UC.UCHein.Base;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
using Inventec.Common.Logging;
using DevExpress.Utils;
using Inventec.Core;
using His.UC.UCHein.ControlProcess;
using His.UC.UCHein.Resources;
using His.UC.UCHein.Design.TemplateKskContract1.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;

namespace His.UC.UCHein.Design.TemplateKskContract1
{
    public partial class Template__KskContract1 : UserControl
    {
        private void ValidControl()
        {
            try
            {
                validKskContract();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void validKskContract()
        {
            try
            {
                TemplateKskContract1__KskContract__ValidationRule oDobDateRule = new TemplateKskContract1__KskContract__ValidationRule();
                oDobDateRule.txtKskContractCode = txtKskContractCode;
                oDobDateRule.cboKskContract = cboKskContract;
                oDobDateRule.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                oDobDateRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(txtKskContractCode, oDobDateRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
