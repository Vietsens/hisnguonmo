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

namespace His.UC.UCHein.Design.TemplateHeinBHYT1.ValidationRule
{
    class TemplateHeinBHYT1__HeinCardToTime__ValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.ButtonEdit txtHeinCardFromTime;
        internal DevExpress.XtraEditors.ButtonEdit txtHeinCardToTime;
        internal DevExpress.XtraEditors.CheckEdit checkKhongKTHSD;
        internal string isShowCheckKhongKTHSD;
        internal bool IsEdit;
        internal long PatientTypeId;
        internal long ExceedDayAllow;
        public override bool Validate(Control control, object value)
        {
            bool valid = true;
            try
            {
                valid = valid && (txtHeinCardToTime != null && txtHeinCardFromTime != null && PatientTypeId > 0);
                if (valid)
                {
                    var patientType = His.UC.UCHein.Config.DataStore.PatientTypes.FirstOrDefault(o => o.ID == PatientTypeId && o.ID == Template__HeinBHYT1.PatientTypeIdBHYT);
                    if (txtHeinCardToTime.Enabled == false || (patientType == null)) return true;
                    var dateToTime = His.UC.UCHein.Utils.HeinUtils.ConvertDateStringToSystemDate(txtHeinCardToTime.Text);
                    var dateFromTime = His.UC.UCHein.Utils.HeinUtils.ConvertDateStringToSystemDate(txtHeinCardFromTime.Text);
                    valid = valid && (dateFromTime != null && dateFromTime.Value != DateTime.MinValue);
                    valid = valid && (dateToTime != null && dateToTime.Value != DateTime.MinValue);
                    if (valid && dateFromTime != null && dateFromTime.Value > dateToTime.Value)
                    {
                        this.ErrorText = His.UC.UCHein.Base.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.NguoiDungNhapHanTheTuPhaiNhoHonHanTheDen);
                        valid = false;
                    }
                    //#19024
                    //else if (valid)
                    //{
                    //    if ((isShowCheckKhongKTHSD != "1" || !checkKhongKTHSD.Checked) && !IsEdit)
                    //    {
                    //        var date = Inventec.Common.DateTime.Calculation.DifferenceDate(Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dateToTime) ?? 0, Inventec.Common.DateTime.Get.Now() ?? 0);
                    //        if (date > ExceedDayAllow)
                    //        {
                    //            this.ErrorText = His.UC.UCHein.Base.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.NguoiDungNhapHanTheBhytDaHetHanSuDung);
                    //            valid = false;
                    //        }
                    //    }
                    //}
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
