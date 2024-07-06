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

namespace HIS.Desktop.Plugins.PatientUpdate
{
    class Valid_PatientName_Control : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
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

    class Valid_PatientDob_Control : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {

        internal DevExpress.XtraEditors.DateEdit dtPatientDob;
        internal DevExpress.XtraEditors.ButtonEdit txtPatientDob;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtPatientDob == null || dtPatientDob == null)
                    return valid;
                if (string.IsNullOrEmpty(txtPatientDob.Text) || dtPatientDob.EditValue == null)
                    return valid;
                if (dtPatientDob.DateTime == DateTime.MinValue)
                {
                    this.ErrorText = ResourceMessage.NhapNgaySinhKhongDungDinhDang;
                    return valid;
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

    class Valid_Career_Control : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.LookUpEdit cboCareer;
        internal DevExpress.XtraEditors.TextEdit txtCareer;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtCareer == null || cboCareer == null)
                    return valid;
                if (string.IsNullOrEmpty(txtCareer.Text) || cboCareer.EditValue == null || Inventec.Common.TypeConvert.Parse.ToInt64(cboCareer.EditValue.ToString()) == 0)
                    return valid;
                valid = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
    }

    class Valid_PatientType_Control : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.GridLookUpEdit cboPatientType;
        internal DevExpress.XtraEditors.TextEdit txtPatientType;
        internal bool TD3;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (TD3)
                    return true;
                if (txtPatientType == null || cboPatientType == null)
                    return valid;
                if (string.IsNullOrEmpty(txtPatientType.Text) || cboPatientType.EditValue == null)
                    return valid;
                valid = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
    }

    class Valid_GenderCode_Control : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.GridLookUpEdit cboGenderCode;
        //internal DevExpress.XtraEditors.TextEdit txtGenderCode;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (cboGenderCode == null)
                    return valid;
                if (cboGenderCode.EditValue == null)
                    return valid;
                valid = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
    }

    class Valid_Text_Age_Control : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtAge;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtAge != null && !string.IsNullOrEmpty(txtAge.Text))
                {
                    if (txtAge.Text != "0" && Inventec.Common.TypeConvert.Parse.ToInt64(txtAge.Text) == 0)
                    {
                        this.ErrorText = "Sai định dạng giờ";
                        return valid;
                    }
                    if (Inventec.Common.TypeConvert.Parse.ToInt64(txtAge.Text) > 23 || Inventec.Common.TypeConvert.Parse.ToInt64(txtAge.Text) < 0)
                    {
                        this.ErrorText = "Sai định dạng giờ";
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
