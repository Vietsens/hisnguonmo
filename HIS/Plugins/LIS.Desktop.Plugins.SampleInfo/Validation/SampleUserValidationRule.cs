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
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIS.Desktop.Plugins.SampleInfo.Validation
{
    class SampleUserValidationRule : ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtSampleLoginname;
        internal DevExpress.XtraEditors.GridLookUpEdit cboSampleUser;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (cboSampleUser == null || txtSampleLoginname == null) return valid;
                if (String.IsNullOrWhiteSpace(txtSampleLoginname.Text) || cboSampleUser.EditValue == null)
                {
                    ErrorText = MessageUtil.GetMessage(Message.Enum.TruongDuLieuBatBuoc);
                    ErrorType = ErrorType.Warning;
                    return valid;
                }
                if (cboSampleUser.EditValue != null)
                {
                    ACS_USER user = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == 1 && o.LOGINNAME.Equals(cboSampleUser.EditValue.ToString()));
                    if (user == null)
                    {
                        ErrorText = MessageUtil.GetMessage(Message.Enum.TruongDuLieuBatBuoc);
                        ErrorType = ErrorType.Warning;
                        return valid;
                    }
                }
                valid = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }
    }
}
