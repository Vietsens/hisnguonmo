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

namespace HIS.Desktop.Plugins.Register.ValidationRule
{
    class PatientName__ValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtPatientName;
        public override bool Validate(Control control, object value)
        {
            bool valid = true;
            try
            {
                valid = valid && (txtPatientName != null);
                if (valid)
                {
                    string strError = "";
                    string patientName = txtPatientName.Text.Trim();
                    if (String.IsNullOrEmpty(patientName))
                    {
                        valid = false;
                        strError = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    }
                    else
                    {
                        string firstName = "";
                        string lastName = "";
                        int idx = patientName.LastIndexOf(" ");
                        if (idx > -1)
                        {
                            firstName = patientName.Substring(idx).Trim();
                            lastName = patientName.Substring(0, idx).Trim();
                        }
                        else
                        {
                            firstName = patientName;
                            lastName = "";
                        }
                        if (!String.IsNullOrEmpty(firstName) && firstName.Length > 30)
                        {
                            valid = false;
                            strError += ((!String.IsNullOrEmpty(strError) ? "\r\n" : "") + String.Format(ResourceMessage.TenBNVuotQuaMaxLength, 30));
                        }
                        if (!String.IsNullOrEmpty(lastName) && lastName.Length > 70)
                        {
                            valid = false;
                            strError += ((!String.IsNullOrEmpty(strError) ? "\r\n" : "") + String.Format(ResourceMessage.HoDemBNVuotQuaMaxLength, 70));
                        }
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
    }
}
