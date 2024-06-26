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
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Plugins.InfantInformation;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
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
//using HIS.Desktop.Plugins.InfantInformation.Validate;

namespace HIS.Desktop.Plugins.InfantInformation
{
    public partial class frmInfantInformation : HIS.Desktop.Utility.FormBase
    {
        private void ValidControls()
        {
            try
            {
               
                validMalength(this.txtNoicap, 100);
                validMalength(this.txtFather, 100);
                validMalength(this.txtInfantMidwife1, 100);
                validMalength(this.txtInfantMidwife2, 100);
                validMalength(this.txtInfantMidwife3, 100);
                validMalength(this.txtInfantName, 100);
                validInfantMonth();
                validSoTuan();
               // cmtvalidate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void validInfantMonth()
        {
            try
            {

                MonthValidationRule1 icdExam1 = new MonthValidationRule1();
                icdExam1.txtTextEdit = txtInfantMonth;
                icdExam1.ErrorText = String.Format(ResourceMessage.SoThangPhaiNhoHon9);
                icdExam1.ErrorType = ErrorType.Warning;
                this.dxValidationProviderEditorInfo.SetValidationRule(txtInfantMonth, icdExam1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void validMalength(BaseEdit control, int? maxLength)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxLength;
                //validate.IsRequired = true;
                validate.ErrorText = "Nhập quá kí tự cho phép";
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void validSoTuan()
        {
            try
            {
                WeekValidationRule2 icsotuan = new WeekValidationRule2();
                icsotuan.txtTextEdit = txtInfantWeek;
                icsotuan.ErrorText = String.Format(ResourceMessage.SoTuanPhaiNhoHon40);
                icsotuan.ErrorType = ErrorType.Warning;
                this.dxValidationProviderEditorInfo.SetValidationRule(txtInfantWeek, icsotuan);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void cmtvalidate()

        {
            try
            {
                Validate.CMTvalidation cmt = new Validate.CMTvalidation();
                cmt.txtTextEdit = txtCMT;
                cmt.ErrorType = ErrorType.Warning;
                this.dxValidationProviderEditorInfo.SetValidationRule(txtCMT, cmt);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        


    }
}






