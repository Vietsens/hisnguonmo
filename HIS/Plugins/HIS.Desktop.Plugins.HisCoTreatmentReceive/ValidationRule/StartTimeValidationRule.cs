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

namespace HIS.Desktop.Plugins.HisCoTreatmentReceive.ValidationRule
{
    class StartTimeValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.DateEdit dtCancelTime;
        internal long TransactionTime;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (dtCancelTime == null)
                    return valid;
                if (String.IsNullOrEmpty(dtCancelTime.Text))
                {
                    ErrorText = HIS.Desktop.Plugins.HisCoTreatmentReceive.Resources.ResourceMessageLang.TruongDuLieuBatBuoc;
                    ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    return valid;
                }

                //if (dtCancelTime.EditValue != null && dtCancelTime.DateTime == DateTime.MinValue)
                //{
                //    ErrorText = HIS.Desktop.Plugins.TransactionCancel.Base.ResourceMessageLang.ThoiGianHuyGiaoDichLonHonThoiGianGiaoDich;
                //    ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                //    return valid;
                //}

                //if (dtCancelTime.EditValue != null && dtCancelTime.DateTime != DateTime.MinValue)
                //{
                //    long CancelTime = Inventec.Common.TypeConvert.Parse.ToInt64(
                //      Convert.ToDateTime(dtCancelTime.EditValue).ToString("yyyyMMddHHmm") + "00");
                //    if (CancelTime < TransactionTime)
                //    {
                //        ErrorText = HIS.Desktop.Plugins.TransactionCancel.Base.ResourceMessageLang.ThoiGianHuyGiaoDichLonHonThoiGianGiaoDich;
                //        ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                //        return valid;
                //    }
                //}
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
