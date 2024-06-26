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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.UC.UCHeniInfo.Data;
using HIS.UC.UCHeniInfo.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.UCHeniInfo.CustomValidateRule
{
    class TemplateHeinBHYT1__HeinCardFromTime__ValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.ButtonEdit txtHeinCardFromTime;
        internal long PatientTypeId;
        public override bool Validate(Control control, object value)
        {
            bool valid = true;
            try
            {
                valid = valid && (txtHeinCardFromTime != null && PatientTypeId > 0);
                if (valid)
                {
                    if (string.IsNullOrEmpty(txtHeinCardFromTime.Text))
                    {
                        this.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                        return false;
                    }
                    var patientType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == PatientTypeId && o.ID == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT);
                    if (txtHeinCardFromTime.Enabled == false || (patientType == null)) return true;

                    var date = HeinUtils.ConvertDateStringToSystemDate(txtHeinCardFromTime.Text);
                    valid = valid && (date != null && date.Value != DateTime.MinValue);

                    if (valid && date.Value.Date > DateTime.Now.Date)
                    {
                        this.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.NguoiDungNhapNgayPhaiNhoHonNgayHienTai);
                        valid = false;
                    }
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
