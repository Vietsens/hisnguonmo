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
using HIS.UC.UCOtherServiceReqInfo.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.UCOtherServiceReqInfo.Valid
{
    class IntructionTime__ValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtIntructionTime;

        public override bool Validate(Control control, object value)
        {
            bool valid = true;
            try
            {
                valid = valid && (txtIntructionTime != null);
                if (valid)
                {
                    string strError = "";
                    if (string.IsNullOrEmpty(this.txtIntructionTime.Text))
                    {
                        return false;
                    }
                    else
                    {
                        DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(this.txtIntructionTime.Text);
                        if (dt == null || dt.Value == DateTime.MinValue)
                        {
                            valid = false;
                            strError = ResourceMessage.NhapNgayThangKhongDungDinhDang;
                        }
                        else if (this.txtIntructionTime.Text.ToString().Substring(6, 1) == "0")
                        {
                            valid = false;
                            strError = ResourceMessage.NhapNgayThangKhongDungDinhDang;
                        }
                        else
                            try
                            {
                                DateTime.ParseExact(this.txtIntructionTime.Text, "dd/MM/yyyy HH:mm", null);
                            }
                            catch (Exception ex)
                            {
                                valid = false;
                                strError = ResourceMessage.NhapNgayThangKhongDungDinhDang;
                                Inventec.Common.Logging.LogSystem.Error(ex);
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
