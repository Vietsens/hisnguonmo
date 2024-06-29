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
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.LocalData;
using MOS.Filter;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.Data;
using HIS.Desktop.Controls.Session;
using Inventec.Desktop.Common.LanguageManager;
using HIS.UC.TreatmentFinish;
using HIS.UC.TreatmentFinish.ADO;
using HIS.UC.TreatmentFinish.Run;
using HIS.UC.TreatmentFinish.Reload;
using MOS.SDO;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.ADO;
using HIS.UC.TreatmentFinish.CloseTreatment;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.Utility;
using HIS.UC.TreatmentFinish.Resources;
using Inventec.Desktop.CustomControl;

namespace HIS.UC.TreatmentFinish.Run
{
    public partial class UCTreatmentFinish : UserControl
    {
        public bool GetValidate()
        {
            bool result = true;
            try
            {
                if (chkAutoTreatmentFinish.Checked)
                {
                    this.positionHandle = -1;
                    if (!dxValidationProvider1.Validate())
                        return false;
                    if (treatmentFinishSDO != null && Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentEndType.EditValue ?? "0").ToString()) == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN && (string.IsNullOrEmpty(treatmentFinishSDO.ClinicalNote) || string.IsNullOrEmpty(treatmentFinishSDO.TreatmentDirection) || string.IsNullOrEmpty(treatmentFinishSDO.TreatmentMethod) && string.IsNullOrEmpty(treatmentFinishSDO.TransportVehicle) || (string.IsNullOrEmpty(treatmentFinishSDO.TransporterLoginnames) && string.IsNullOrEmpty(treatmentFinishSDO.Transporter)) || string.IsNullOrEmpty(treatmentFinishSDO.TransferOutMediOrgCode) || !treatmentFinishSDO.TranPatiReasonId.HasValue || !treatmentFinishSDO.TranPatiFormId.HasValue))
                    {
                        XtraMessageBox.Show("Thiếu thông tin chuyển viện","Thông báo");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        public bool GetValidateWithMessage(List<string> errorEmpty, List<string> errorOther)
        {
            bool result = true;
            try
            {
                if (chkAutoTreatmentFinish.Checked)
                {
                    this.positionHandle = -1;
                    if (!dxValidationProvider1.Validate())
                    {
                        CommonParam param = new CommonParam();
                        GetMessageErrorControlInvalidProcess getMessageErrorControlInvalidProcess = new GetMessageErrorControlInvalidProcess();
                        getMessageErrorControlInvalidProcess.Run(this, dxValidationProvider1, null, errorEmpty, errorOther);

                        return false;
                    }

                    long treatmentEndTypeExtId = Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentEndTypeExt.EditValue ?? "0").ToString());
                    long treatmentEndTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentEndType.EditValue ?? "0").ToString());
                    if (treatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN
                        && (treatmentFinishSDO == null || treatmentFinishSDO.AppointmentTime == null || treatmentFinishSDO.AppointmentTime <= 0))
                    {
                        errorOther.Add(ResourceMessage.ChuaNhapThoiGianHenKham);
                        return false;
                    }
                    else if (treatmentEndTypeExtId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM
                        && (treatmentFinishSDO == null || String.IsNullOrEmpty(treatmentFinishSDO.SickLoginname) || (String.IsNullOrEmpty(treatmentFinishSDO.PatientWorkPlace) && (treatmentFinishSDO.WorkPlaceId == null || treatmentFinishSDO.WorkPlaceId == 0))))
                    {
                        errorOther.Add(ResourceMessage.ChuaNhapThongTinBatBuocKhiChonNGhiViecHuongBHXH);
                        return false;
                    }
                    if (treatmentFinishSDO != null && Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentEndType.EditValue ?? "0").ToString()) == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN && (string.IsNullOrEmpty(treatmentFinishSDO.ClinicalNote) || string.IsNullOrEmpty(treatmentFinishSDO.TreatmentDirection) || string.IsNullOrEmpty(treatmentFinishSDO.TreatmentMethod) && string.IsNullOrEmpty(treatmentFinishSDO.TransportVehicle) || (string.IsNullOrEmpty(treatmentFinishSDO.TransporterLoginnames) && string.IsNullOrEmpty(treatmentFinishSDO.Transporter)) || string.IsNullOrEmpty(treatmentFinishSDO.TransferOutMediOrgCode) || !treatmentFinishSDO.TranPatiReasonId.HasValue || !treatmentFinishSDO.TranPatiFormId.HasValue))
                    {
                        errorOther.Add("Thiếu thông tin chuyển viện");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }


        private void ValidateForm()
        {
            ValidateLookupWithTextEdit(cboTreatmentEndType, txtTreatmentEndTypeCode);
            //SetMaxlength(this.txtAdsive, 500);
            ValidationSingleControl(dtEndTime, IsValidTreatmentEnd, HIS.UC.TreatmentFinish.Resources.ResourceMessage.ThoiGianKetThucKhongDuocNhoHonThoiGianYLenhCuaHoSo);
            ValidationSingleControl(cboCareer);
        }

        bool IsValidTreatmentEnd()
        {
            bool valid = true;
            try
            {
                if (chkAutoTreatmentFinish.Checked)
                {
                    var finishTime = Convert.ToInt64(dtEndTime.DateTime.ToString("yyyyMMddHHmmss"));
                    this.dataInputADO = this.getDateADO();
                    if (finishTime < this.dataInputADO.IntructionTime)
                    {
                        valid = false;
                        Inventec.Common.Logging.LogSystem.Debug(HIS.UC.TreatmentFinish.Resources.ResourceMessage.ThoiGianKetThucKhongDuocNhoHonThoiGianYLenhCuaHoSo + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => finishTime), finishTime) + Inventec.Common.Logging.LogUtil.TraceData("InstructionTIme", this.dataInputADO.IntructionTime));
                        //MessageManager.Show(HIS.UC.TreatmentFinish.Resources.ResourceMessage.ThoiGianKetThucKhongDuocNhoHonThoiGianYLenhCuaHoSo);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private void ValidateLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control, IsValidControl isValidControl, string errorMess)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.isValidControl = isValidControl;
                validRule.isUseOnlyCustomValidControl = isValidControl != null;
                validRule.ErrorText = errorMess;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void ValidCustomGridLookup(TextEdit txt, CustomGridLookUpEditWithFilterMultiColumn cbo)
        {

            try
            {
                GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = txt;
                validRule.cbo = cbo;
                validRule.ErrorText = "Trường dữ liệu bắt buộc";
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txt, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void SetMaxlength(BaseEdit control, int maxlenght)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxlenght;
                validate.IsRequired = false;
                validate.ErrorText = string.Format("Nhập quá kí tự cho phép", maxlenght);
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
