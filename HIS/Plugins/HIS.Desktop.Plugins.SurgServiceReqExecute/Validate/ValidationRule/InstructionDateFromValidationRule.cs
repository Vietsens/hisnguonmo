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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LibraryMessage;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute.Validate.ValidationRule
{
    public class InstructionDateFromValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.DateEdit dateFromEdit;
        internal DevExpress.XtraEditors.DateEdit dateToEdit;
        internal DevExpress.XtraLayout.LayoutControlItem lci;
        internal long? inTime;
        internal long? outTime;
        internal TimeSpanEdit timeSpan;
        internal bool isRequired = false;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (lci == null || lci.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    if (dateFromEdit == null || dateToEdit == null) return valid;

                    if (isRequired && (dateFromEdit.EditValue == null || dateFromEdit.DateTime == DateTime.MinValue || dateToEdit.EditValue == null || dateToEdit.DateTime == DateTime.MinValue))
                    {
                        this.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                        this.ErrorType = ErrorType.Warning;
                        return valid;
                    }
                    if (dateToEdit.DateTime.Date < dateFromEdit.DateTime.Date)
                    {
                        this.ErrorText = "Ngày từ phải nhỏ hơn ngày đến";
                        this.ErrorType = ErrorType.Warning;
                        return valid;
                    }
                    if(inTime.HasValue && Int64.Parse(dateFromEdit.DateTime.ToString("yyyyMMdd")) < Int64.Parse(inTime.ToString().Substring(0,8)))
                    {
                        this.ErrorText = "Ngày từ nhỏ hơn ngày vào viện";
                        this.ErrorType = ErrorType.Warning;
                        return valid;
                    }

                    if (outTime.HasValue && Int64.Parse(dateToEdit.DateTime.ToString("yyyyMMdd") ) > Int64.Parse(outTime.ToString().Substring(0, 8)))
                    {
                        this.ErrorText = "Ngày đến lớn hơn ngày ra viện";
                        this.ErrorType = ErrorType.Warning;
                        return valid;
                    }
                }
                valid = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
    }
}
