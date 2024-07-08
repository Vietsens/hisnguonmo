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
using His.UC.UCHein.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace His.UC.UCHein.Design.TemplateHeinBHYT1.ValidationRule
{
    class TemplateHeinBHYT1__Icd__ValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtMaChanDoanTD;
        internal DevExpress.XtraEditors.GridLookUpEdit cboChanDoanTD;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtMaChanDoanTD == null || cboChanDoanTD == null) return valid;

                if (!String.IsNullOrEmpty(txtMaChanDoanTD.Text))
                {
                    if (cboChanDoanTD.EditValue == null)
                    {
                        this.ErrorText = His.UC.UCHein.Base.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                        return valid;
                    }

                    var searchResult = ((DataStore.Icds != null && DataStore.Icds.Count > 0) ? DataStore.Icds.FirstOrDefault(o => o.ICD_CODE.ToUpper() == txtMaChanDoanTD.Text.ToUpper()) : null);
                    if (searchResult == null)
                    {
                        this.ErrorText = ResourceMessage.MaBenhChinhKhongHopLe;
                        return valid;
                    }
                    else if (searchResult.ID != Inventec.Common.TypeConvert.Parse.ToInt64((cboChanDoanTD.EditValue ?? "0").ToString())
                        || searchResult.ICD_NAME != cboChanDoanTD.Text)
                    {
                        this.ErrorText = String.Format(ResourceMessage.MaBenhKhongKhopVoiTenBenh, txtMaChanDoanTD.Text, cboChanDoanTD.Text);
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
