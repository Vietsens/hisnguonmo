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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Plugins.UpdateExamServiceReq.Validate;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.UpdateExamServiceReq
{
    public partial class frmUpdateExamServiceReq : HIS.Desktop.Utility.FormBase
    {
        public void InitValidate()
        {
            try
            {
                UpdateExamServiceReqExamValidate();
                ServiceValidate();
                PatientTypeValidate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateExamServiceReqExamValidate()
        {
            try
            {
                GridLookupEditWithTextEditValidationRule roomVali = new GridLookupEditWithTextEditValidationRule();
                roomVali.txtTextEdit = txtRoomCode;
                roomVali.cbo = cboRoom;
                roomVali.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                roomVali.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(txtRoomCode, roomVali);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ServiceValidate()
        {
            try
            {
                GridLookupEditWithTextEditValidationRule serviceVali = new GridLookupEditWithTextEditValidationRule();
                serviceVali.txtTextEdit = txtServiceCode;
                serviceVali.cbo = cboExamServiceReq;
                serviceVali.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                serviceVali.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(txtServiceCode, serviceVali);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PatientTypeValidate()
        {
            try
            {
                GridLookupEditWithTextEditValidationRule patientTypeVali = new GridLookupEditWithTextEditValidationRule();
                patientTypeVali.txtTextEdit = txtPatientTypeCode;
                patientTypeVali.cbo = cboPatientType;
                patientTypeVali.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                patientTypeVali.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(txtPatientTypeCode, patientTypeVali);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DateEditValidate()
        {
            try
            {
                DateEditValidationRule dateEditVali = new DateEditValidationRule();
                dateEditVali.dateEdit = dtInstructionTime;
                if (treatment!=null)
                    dateEditVali.inTime = treatment.IN_TIME;
                dateEditVali.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(dtInstructionTime, dateEditVali);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
