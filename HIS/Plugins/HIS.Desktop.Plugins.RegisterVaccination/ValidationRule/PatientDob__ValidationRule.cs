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
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.LibraryMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.RegisterVaccination.ValidationRule
{
    class PatientDob__ValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.ButtonEdit txtDob;
        internal DevExpress.XtraEditors.DateEdit dtDob;
        public override bool Validate(Control control, object value)
        {
            bool valid = true;
            try
            {
                bool isChild = CheckIsChild();
                valid = valid && (txtDob != null && dtDob != null);
                if (valid)
                {
                    string strError = "";
                    HIS.Desktop.Plugins.RegisterVaccination.DateUtil.DateValidObject dateValidObject = DateUtil.ValidPatientDob(this.txtDob.Text);
                    valid = !String.IsNullOrEmpty(dateValidObject.OutDate);
                    if (!String.IsNullOrEmpty(dateValidObject.Message))
                    {
                        strError = dateValidObject.Message;
                        valid = false;
                    }

                    this.ErrorText = strError;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        bool CheckIsChild()
        {
            bool success = false;
            try
            {
                if (dtDob.EditValue != null && dtDob.DateTime != DateTime.MinValue)
                {
                    DateTime dtNgSinh = dtDob.DateTime;
                    success = MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(dtNgSinh);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return success;
        }
    }
}
