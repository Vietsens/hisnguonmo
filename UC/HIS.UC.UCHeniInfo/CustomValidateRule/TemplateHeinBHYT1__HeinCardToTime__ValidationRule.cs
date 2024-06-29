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
    class TemplateHeinBHYT1__HeinCardToTime__ValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.ButtonEdit txtHeinCardFromTime;
        internal DevExpress.XtraEditors.ButtonEdit txtHeinCardToTime;
        internal DevExpress.XtraEditors.CheckEdit checkKhongKTHSD;
        internal string isShowCheckKhongKTHSD;
        internal bool IsEdit;
        internal long PatientTypeId;
        public override bool Validate(Control control, object value)
        {
            bool valid = true;
            try
            {
                valid = valid && (txtHeinCardToTime != null && txtHeinCardFromTime != null && PatientTypeId > 0);
                if (valid)
                {
                    if (string.IsNullOrEmpty(txtHeinCardToTime.Text))
                    {
                        this.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                        return false;
                    }
                    var patientType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == PatientTypeId && o.ID == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT);
                    if (txtHeinCardToTime.Enabled == false || (patientType == null)) return true;

                    var dateToTime = HeinUtils.ConvertDateStringToSystemDate(txtHeinCardToTime.Text);
                    var dateFromTime = HeinUtils.ConvertDateStringToSystemDate(txtHeinCardFromTime.Text);
                    valid = valid && (dateFromTime != null && dateFromTime.Value != DateTime.MinValue);
                    valid = valid && (dateToTime != null && dateToTime.Value != DateTime.MinValue);
                    if (valid && dateFromTime != null && dateFromTime.Value > dateToTime.Value)
                    {
                        this.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.NguoiDungNhapHanTheTuPhaiNhoHonHanTheDen);
                        valid = false;
                    }
                    else if (valid && dateToTime.Value.Date < DateTime.Now.Date)
                    {
                        if ((isShowCheckKhongKTHSD != "1" || !checkKhongKTHSD.Checked) && !IsEdit)
                        {
                            this.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.NguoiDungNhapHanTheBhytDaHetHanSuDung);
                            valid = false;
                        }
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
