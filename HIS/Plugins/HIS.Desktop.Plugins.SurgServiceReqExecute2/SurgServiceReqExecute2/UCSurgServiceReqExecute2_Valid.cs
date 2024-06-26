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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraNavBar;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utilities;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using System.Security.Cryptography;
using HIS.Desktop.Plugins.SurgServiceReqExecute2.ADO;
using HIS.Desktop.Plugins.SurgServiceReqExecute2.EkipTemp;
using HIS.Desktop.ADO;
using ACS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.SurgServiceReqExecute2.Config;
using MOS.SDO;
using Inventec.Common.RichEditor.Base;
using Inventec.Common.ThreadCustom;
using HIS.Desktop.Plugins.SurgServiceReqExecute2.Validate;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute2
{
    public partial class UCSurgServiceReqExecute2 : UserControlBase
    {
        private void ValidForm()
        {

            try
            {
                this.layoutControlItem29.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                ValidationGridLookUpWithTextEdit(cboPtttGroup, txtPtttGroup);
                ValidationStartTime();
                ValidationFinishTime();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void ValidationGridLookUpWithTextEdit(GridLookUpEdit cboLoaiPT, TextEdit txtLoaiPT)
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validate = new GridLookupEditWithTextEditValidationRule();
                validate.cbo = cboLoaiPT;
                validate.txtTextEdit = txtLoaiPT;
                validate.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProviderEditorInfo.SetValidationRule(txtLoaiPT, validate);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidationStartTime()
        {
            try
            {
                StartTimeValidationRule mainRule = new StartTimeValidationRule();
                mainRule.startTime = dteStart;
                mainRule.finishTime = dteFinish;
                mainRule.instructionTime = currentRow.INTRUCTION_TIME;
                mainRule.keyCheck = this.isAllowEditInfo;
                mainRule.ErrorType = ErrorType.Warning;
                this.dxValidationProviderEditorInfo.SetValidationRule(dteStart, mainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidationFinishTime()
        {
            try
            {
                FinishTimeValidationRule mainRule = new FinishTimeValidationRule();
                mainRule.startTime = dteStart;
                mainRule.finishTime = dteFinish;
                mainRule.instructionTime = currentRow.INTRUCTION_TIME;
                mainRule.keyCheck = this.isAllowEditInfo;
                mainRule.ErrorType = ErrorType.Warning;
                this.dxValidationProviderEditorInfo.SetValidationRule(dteFinish, mainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
