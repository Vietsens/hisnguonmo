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
using HIS.Desktop.LibraryMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors.DXErrorProvider;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;

namespace HIS.Desktop.Plugins.HisSourceMedicine
{
    class ValidateTextCode : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtSourceMedicineCode; 
        internal DevExpress.XtraEditors.TextEdit txtSourceMedicineName; 
        internal bool IsExist;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtSourceMedicineCode == null) return valid;
                if (String.IsNullOrEmpty(txtSourceMedicineCode.Text.Trim()))
                {
                    this.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    this.ErrorType = ErrorType.Warning;
                    return valid;
                }
                if (IsExist)
                {
                    //string existingName = BackendDataWorker.Get<HIS_SOURCE_MEDICINE>().FirstOrDefault(o => o.SOURCE_MEDICINE_CODE == txtSourceMedicineCode).txtSourceMedicineName;
                    string existingName = (txtSourceMedicineName.Text.Trim());
                    this.ErrorText = string.Format("Mã '{0}' đã được gán với tên '{1}'.", txtSourceMedicineCode.Text.Trim(), existingName);
                    this.ErrorType = ErrorType.Warning;
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
}
