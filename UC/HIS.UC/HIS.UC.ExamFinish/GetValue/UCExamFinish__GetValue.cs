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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using MOS.EFMODEL.DataModels;
using HIS.UC.ExamFinish.ADO;
using MOS.SDO;
using MOS.Filter;
using Inventec.Common.Adapter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using HIS.UC.ExamFinish.Config;

namespace HIS.UC.ExamFinish.Run
{
    public partial class UCExamFinish : UserControl
    {
        public ExamFinishADO GetValue()
        {
            ExamFinishADO examFinishADO = null;
            try
            {
                this.positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                    return examFinishADO;
                ////Check co hoa don hay khong
                //if (CheckPrescriptionExist())
                //{
                //    DialogResult dialogResult = DevExpress.XtraEditors.XtraMessageBox.Show("Bệnh nhân đã được kê thuốc. Bạn có muốn cho bệnh nhân nhập viện không?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                //    if (dialogResult == DialogResult.No)
                //        return null;
                //}

                examFinishADO = new ExamFinishADO();

                if (!HisConfig.IsUsingServerTime)
                    examFinishADO.FinishTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtFinishTime.DateTime) ?? 0;
                examFinishADO.TreatmentId = examFinishInitADO.TreatmentId.Value;

                if (chkAppointment.Enabled && chkAppointment.Checked)
                {
                    examFinishADO.IsAppointment = true;
                }

                if (chkPrintAppointment.Enabled && chkPrintAppointment.Checked)
                {
                    examFinishADO.IsPrintAppointment = true;
                }

                if (dtAppointment.Enabled && dtAppointment.EditValue != null && dtAppointment.DateTime != DateTime.MinValue)
                {
                    examFinishADO.AppointmentTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtAppointment.DateTime) ?? 0;
                }

                if (txtAdvise.Enabled)
                {
                    examFinishADO.Advise = txtAdvise.Text;
                }
                examFinishADO.RoomApointmentId = cboRoomAppointment.EditValue != null ? (long?)Inventec.Common.TypeConvert.Parse.ToInt64(cboRoomAppointment.EditValue.ToString()) : null;

                examFinishADO.ServiceApointmentId = cboServiceAppointment.EditValue != null ? (long?)Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceAppointment.EditValue.ToString()) : null;
                examFinishADO.Note = memNote.Text.Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return examFinishADO;
        }
    }
}
