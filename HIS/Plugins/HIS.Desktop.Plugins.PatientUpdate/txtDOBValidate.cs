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
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.LibraryMessage;
namespace HIS.Desktop.Plugins.PatientUpdate
{
    public class txtDOBValidate : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {

        internal DevExpress.XtraEditors.TextEdit txtdob;
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtdob == null) return valid;
                if (string.IsNullOrEmpty(txtdob.Text))
                {
                    ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);

                }
                string strDob = "";
                if (txtdob.Text.Length == 2 || txtdob.Text.Length == 1)
                {
                    int patientDob = Int32.Parse(txtdob.Text);
                    if (patientDob < 7)
                    {
                        ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.NguoiDungNhapNgaySinhKhongHopLe);
                        return valid;
                    }
                    else
                    {
                        txtdob.Text = (DateTime.Now.Year - patientDob).ToString();
                    }
                }
                else if (txtdob.Text.Length == 4)
                {
                    if (Inventec.Common.TypeConvert.Parse.ToInt64(txtdob.Text) <= DateTime.Now.Year)
                    {
                        strDob = "01/01/" + txtdob.Text;
                    }
                    else
                    {
                        ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.NguoiDungNhapNgaySinhKhongHopLe);
                        return valid;
                    }
                }
                else if (txtdob.Text.Length < 4)
                {
                    ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.NguoiDungNhapNgaySinhKhongHopLe);
                    return valid;
                }
                else if (txtdob.Text.Length == 8)
                {
                    strDob = txtdob.Text.Substring(0, 2) + "/" + txtdob.Text.Substring(2, 2) + "/" + txtdob.Text.Substring(4, 4);
                    if (HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(strDob).Value.Date <= DateTime.Now.Date)
                    {
                        txtdob.Text = strDob;
                    }
                    else
                    {
                        txtdob.Text = strDob;
                        ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.NguoiDungNhapNgaySinhKhongHopLe);
                        return valid;
                    }
                }
                else if (txtdob.Text.Length > 4 && txtdob.Text.Length < 8)
                {
                    ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.NguoiDungNhapNgaySinhKhongHopLe);
                    return valid;
                }
                valid = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
    }
}
