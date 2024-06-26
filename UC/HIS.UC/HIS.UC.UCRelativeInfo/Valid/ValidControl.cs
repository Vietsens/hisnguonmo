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
using HIS.UC.UCRelativeInfo.Resources;

namespace HIS.UC.UCRelativeInfo.Valid
{
    class Valid_HomePerson_Control : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtHomePerson;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtHomePerson == null) return valid;
                if (txtHomePerson.Enabled && (String.IsNullOrEmpty(txtHomePerson.Text.Trim())))
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

    class Valid_Correlated_Control : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtCorrelated;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtCorrelated == null) return valid;
                if (txtCorrelated.Enabled && (String.IsNullOrEmpty(txtCorrelated.Text.Trim())))
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

    class Valid_RelativeAddress_Control : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtRelativeAddress;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtRelativeAddress == null) return valid;
                if (txtRelativeAddress.Enabled && (String.IsNullOrEmpty(txtRelativeAddress.Text.Trim())))
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

    class Valid_RelativeCMNDNumber_Control : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtRelativeCMNDNumber;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtRelativeCMNDNumber == null)
                {
                    this.ErrorText = HIS.UC.UCRelativeInfo.Valid.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    return valid;
                }
                if (txtRelativeCMNDNumber.Enabled && (String.IsNullOrEmpty(txtRelativeCMNDNumber.Text.Trim())))
                {
                    this.ErrorText = HIS.UC.UCRelativeInfo.Valid.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    return valid;
                }
                if (!String.IsNullOrEmpty(txtRelativeCMNDNumber.Text))
                {
                    if (txtRelativeCMNDNumber.Text.Trim().Length == 9 || txtRelativeCMNDNumber.Text.Trim().Length == 12)
                        return valid = true;
                    else
                    {
                        this.ErrorText = ResourceMessage.ThongTinCMNDKhongDungDinhDang;
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

    class Validate_CMND_Control : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtCMND;
        internal bool isValid;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (isValid && String.IsNullOrEmpty(txtCMND.Text) )
                {
                     this.ErrorText = HIS.UC.UCRelativeInfo.Valid.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                     return valid;
                }
                if (!String.IsNullOrEmpty(txtCMND.Text) 
                    && (txtCMND.Text.Trim().Length != 9 
                    && txtCMND.Text.Trim().Length != 12))
                {
                    this.ErrorText = ResourceMessage.ThongTinCMNDKhongDungDinhDang;
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
    class ValidCustomMultilFeilName : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txt1;
        internal bool isValid;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txt1 == null) return valid;
                if (isValid && string.IsNullOrEmpty(txt1.Text.Trim()))
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
}
