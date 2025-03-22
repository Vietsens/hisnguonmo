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

namespace HIS.UC.PlusInfo.Validate
{
    class Valid_ProvinceOfBirth_Control : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.LookUpEdit cboProvinceOfBirth;
        internal DevExpress.XtraEditors.TextEdit txtProvinceOfBirth;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtProvinceOfBirth == null || cboProvinceOfBirth == null)
                {
                    this.ErrorText = HIS.UC.PlusInfo.Validate.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    return valid;
                }
                if (string.IsNullOrEmpty(txtProvinceOfBirth.Text) || cboProvinceOfBirth.EditValue == null)
                {
                    this.ErrorText = HIS.UC.PlusInfo.Validate.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
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

    class Validate_MaHN_Control : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtHNCode;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                string hncode = txtHNCode.Text.Trim();
                if (!txtHNCode.Enabled || (txtHNCode.Enabled && (String.IsNullOrEmpty(hncode))))
                    return true;

                //#18611
                //if (hncode.Length != 9 && hncode.Length != 12)
                //{
                //    this.ErrorText = ResourceMessage.MaHoNgheoKhongDungDinhDang;
                //    return valid;
                //}
                //else if (hncode.Length == 12 && !hncode.EndsWith("VCN"))
                //{
                // this.ErrorText = ResourceMessage.MaHoNgheoKhongDungDinhDang;
                //    return valid;
                //}

                if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(hncode, 20))
                {
                    this.ErrorText = string.Format(ResourceMessage.NhapQuaKyTuChoPhep, 20);
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

    class Validate_CMND_Control : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtCMND;
        internal bool IsRequired;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (!String.IsNullOrEmpty(txtCMND.Text))
                {
                    if (txtCMND.Text.Trim().Length >= 12)
                    {
                        try
                        {
                            var checkText = Int64.Parse(txtCMND.Text);
                            valid = true;
                        }
                        catch (Exception ex)
                        {
                            this.ErrorText = ResourceMessage.HoChieuKhongDungDinhDang;
                        }
                        return valid;
                    }

                    if (txtCMND.Text.Trim().Length == 9)
                    {
                        try
                        {
                            var checkText = Int64.Parse(txtCMND.Text);
                            valid = true;
                        }
                        catch (Exception ex)
                        {
                            valid = IsValid(txtCMND.Text.Trim());
                        }
                        return valid;
                    }

                    if (txtCMND.Text.Trim().Length > 9)
                    {
                        this.ErrorText = ResourceMessage.HoChieuKhongDungDinhDang;
                        return valid;
                    }
                    if (txtCMND.Text.Trim().Length < 9)
                    {
                        try
                        {
                            var checkText = Int64.Parse(txtCMND.Text);
                            this.ErrorText = ResourceMessage.HoChieuKhongDungDinhDang;
                        }
                        catch (Exception ex)
                        {
                            valid = IsValid(txtCMND.Text.Trim());
                        }
                        return valid;
                    }

                }
                else if(IsRequired)
                {
                    this.ErrorText = HIS.UC.PlusInfo.Validate.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
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

        private bool IsValid(string txtCMND)
        {
            bool valid = false;
            var txt = txtCMND;
            int countNumber = 0;
            int total = txt.Length;
            for (int i = 0; i < txt.Length; i++)
            {
                if (IsNumber(txt[i].ToString()))
                {
                    countNumber++;
                }
            }
            if (countNumber == 0)
            {
                this.ErrorText = ResourceMessage.HoChieuKhongDungDinhDang;
                valid = false;
            }
            else if (countNumber != 0 && countNumber < total)
            {
                valid = true;
            }
            return valid;
        }

        private bool IsNumber(string pValue)
        {
            foreach (Char c in pValue)
            {
                if (!Char.IsDigit(c))
                    return false;
            }
            return true;
        }
    }


    class Validate_CMNDDate_Control : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtCMNDDATE;
        internal DevExpress.XtraEditors.DateEdit dt;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {

                long startDay = Inventec.Common.TypeConvert.Parse.ToInt64((Inventec.Common.DateTime.Get.StartDay() ?? 0).ToString());
                if (dt.DateTime != null && dt.DateTime != DateTime.MinValue)
                {
                    if (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dt.DateTime) > startDay)
                    {
                        this.ErrorText = "Thời gian nhập lớn hơn thời gian hiện tại";
                        return valid;
                    }
                }
                if (!string.IsNullOrEmpty(txtCMNDDATE.Text))
                {

                    Inventec.Common.Logging.LogSystem.Error("VALID 1 " + txtCMNDDATE.Text);
                    DateTime? date = HIS.Desktop.Utility.DateTimeHelper.ConvertDateTimeStringToSystemTime(this.txtCMNDDATE.Text + " 00:00");
                    Inventec.Common.Logging.LogSystem.Error("VALID 1 + " + Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(date));
                    if (date != null && date.Value != DateTime.MinValue)
                    {
                        if (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(date) > startDay)
                        {
                            Inventec.Common.Logging.LogSystem.Error("VALID 2");
                            this.ErrorText = "Thời gian nhập lớn hơn thời gian hiện tại";
                            return valid;
                        }

                    }
                }
                Inventec.Common.Logging.LogSystem.Error("VALID 3");
                valid = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
    }

    class Valid_HrmKskCode_Control : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtHrmKskCode;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtHrmKskCode == null || string.IsNullOrEmpty(txtHrmKskCode.Text))
                {
                    this.ErrorText = HIS.UC.PlusInfo.Validate.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    return valid;
                }
                if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(txtHrmKskCode.Text, 20))
                {
                    this.ErrorText = "Trường dữ liệu vượt quá maxlength( " + 20 + " kí tự)";
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

    class Valid_Ethnic_Control : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtEthnic;
        internal DevExpress.XtraEditors.LookUpEdit cboEthnic;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtEthnic == null || cboEthnic == null)
                    return valid;
                if (string.IsNullOrEmpty(txtEthnic.Text) || cboEthnic.EditValue == null)
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

    class Valid_Number_Control : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtEdit;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtEdit != null && !string.IsNullOrEmpty(txtEdit.Text))
                {
                    foreach (Char c in txtEdit.Text)
                    {
                        if (!Char.IsDigit(c))
                            return false;
                    }
                    return true;
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
