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

namespace HIS.Desktop.Plugins.HisMediRecordType.Validates
{
    class ValiDateMaxLength_TypeCode : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
         internal DevExpress.XtraEditors.TextEdit txtcontrol;
        public override bool Validate(Control control, object value)
        {
            bool valid = true;
            try
            {
                valid = valid && (txtcontrol != null);
                if(valid)
                {
                    string strError="";
                    string DocHoldType=txtcontrol.Text.Trim();
                    int? CoutLength=Inventec.Common.String.CountVi.Count(DocHoldType);
                    if (String.IsNullOrEmpty(DocHoldType))
                    {
                        valid = false;
                        strError = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    }
                    else
                    {
                        if (CoutLength > 2)
                        {
                            valid = false;
                            strError += ((!string.IsNullOrEmpty(strError) ? "\r\n" : "") + String.Format(ResourcesMassage.MaLoaiVuotQuaMaxLength, 2));
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
