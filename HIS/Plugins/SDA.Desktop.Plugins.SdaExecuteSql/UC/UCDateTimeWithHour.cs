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
using DevExpress.XtraEditors;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.Utility;
using SDA.EFMODEL.DataModels;
using DevExpress.XtraEditors.ViewInfo;

namespace SDA.Desktop.Plugins.SdaExecuteSql.UC
{
    public partial class UCDateTimeWithHour : UserControlBase
    {
        SDA_SQL_PARAM Data = new SDA_SQL_PARAM();
        int positionHandleControl = -1;

        public UCDateTimeWithHour(SDA_SQL_PARAM data)
        {
            InitializeComponent();
            this.Data = data;
        }

        private void UCDateTimeWithHour_Load(object sender, EventArgs e)
        {
            try
            {
                this.lbNote.Text = this.Data.NOTE;
                this.lciDateTimeWithHour.Text = this.Data.SQL_PARAM_NAME + ':';
                this.lciDateTimeWithHour.OptionsToolTip.ToolTip = this.Data.SQL_PARAM_NAME;
                if (this.Data.IS_REQUIRED == 1)
                {
                    this.lciDateTimeWithHour.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    ValidateControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateControl()
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = dateDateTimeWithHour;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(dateDateTimeWithHour, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal valueADO GetValue()
        {
            valueADO data = new valueADO();
            data.ID = this.Data.ID;
            data.value = Inventec.Common.TypeConvert.Parse.ToInt64(dateDateTimeWithHour.DateTime.ToString("yyyyMMddHHmmss"));
            return data;
        }

        internal bool validate()
        {
            positionHandleControl = -1;
            return dxValidationProviderEditorInfo.Validate();
        }

        protected void ValidationSingleControl(BaseEdit control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
        }

        private void dxValidationProviderEditorInfo_ValidationFailed(object sender, ValidationFailedEventArgs e)
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
                        edit.Focus();
                        edit.SelectAll();
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
