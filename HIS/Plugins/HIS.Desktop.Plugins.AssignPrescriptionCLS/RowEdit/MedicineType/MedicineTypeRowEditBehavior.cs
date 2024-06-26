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
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.AssignPrescription;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Resources;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Worker;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using System;

namespace HIS.Desktop.Plugins.AssignPrescriptionCLS.Edit.MedicineType
{
    class MedicineTypeRowEditBehavior : EditAbstract, IEdit
    {
        internal MedicineTypeRowEditBehavior(CommonParam param,
            frmAssignPrescription frmAssignPrescription,
            ValidAddRow validAddRow,
            HIS.Desktop.Plugins.AssignPrescriptionCLS.MediMatyCreateWorker.ChoosePatientTypeDefaultlService choosePatientTypeDefaultlService,
            HIS.Desktop.Plugins.AssignPrescriptionCLS.MediMatyCreateWorker.ChoosePatientTypeDefaultlServiceOther choosePatientTypeDefaultlServiceOther,
            CalulateUseTimeTo calulateUseTimeTo,
            ExistsAssianInDay existsAssianInDay,
            object dataRow)
            : base(param,
             frmAssignPrescription,
             validAddRow,
             choosePatientTypeDefaultlService,
             choosePatientTypeDefaultlServiceOther,
             calulateUseTimeTo,
             existsAssianInDay,
             dataRow)
        {
            this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM;
            this.Code = frmAssignPrescription.currentMedicineTypeADOForEdit.MEDICINE_TYPE_CODE;
            this.Name = frmAssignPrescription.currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME;
            this.ManuFacturerName = frmAssignPrescription.currentMedicineTypeADOForEdit.MANUFACTURER_NAME;
            this.ServiceUnitName = frmAssignPrescription.currentMedicineTypeADOForEdit.SERVICE_UNIT_NAME;
            this.NationalName = frmAssignPrescription.currentMedicineTypeADOForEdit.NATIONAL_NAME;
            this.ServiceId = frmAssignPrescription.currentMedicineTypeADOForEdit.SERVICE_ID;
            this.Concentra = frmAssignPrescription.currentMedicineTypeADOForEdit.CONCENTRA;
            this.HeinServiceTypeId = frmAssignPrescription.currentMedicineTypeADOForEdit.HEIN_SERVICE_TYPE_ID;
            this.ServiceTypeId = frmAssignPrescription.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID;
            this.ActiveIngrBhytCode = frmAssignPrescription.currentMedicineTypeADOForEdit.ACTIVE_INGR_BHYT_CODE;
            this.ActiveIngrBhytName = frmAssignPrescription.currentMedicineTypeADOForEdit.ACTIVE_INGR_BHYT_NAME;
            this.IsOutKtcFee = ((frmAssignPrescription.currentMedicineTypeADOForEdit.IS_OUT_PARENT_FEE ?? -1) == 1);

        }

        bool IEdit.Run()
        {
            bool success = false;
            try
            {
                if (this.CheckValidPre()
                    && MedicineAgeWorker.ValidThuocCoGioiHanTuoi(this.ServiceId, frmAssignPrescription.patientDob)
                    && HIS.Desktop.Plugins.AssignPrescriptionCLS.ValidAcinInteractiveWorker.ValidGrade(this.DataRow, MediMatyTypeADOs)
                    //&& this.CheckWarningOddConvertAmount()
                    && WarningOddConvertWorker.CheckWarningOddConvertAmount(frmAssignPrescription.currentMedicineTypeADOForEdit, this.Amount, frmAssignPrescription.ResetFocusMediMaty)
                    && ValidKidneyShift())
                {
                    this.CreateADO();
                    this.UpdateMedicineUseFormInDataRow(this.medicineTypeSDO);
                    this.UpdateUseTimeInDataRow(this.medicineTypeSDO);
                    this.SetValidAssianInDayError();
                    this.SetValidAmountError();
                    this.SetValidKidneyShiftError();

                    if (medicineTypeSDO.ErrorMessageMedicineUseForm == ResourceMessage.BenhNhanDoiTuongTTBhytBatBuocPhaiNhapDuongDung
                       && medicineTypeSDO.ErrorTypeMedicineUseForm == ErrorType.Warning)
                    {
                        MessageManager.Show(ResourceMessage.BenhNhanDoiTuongTTBhytBatBuocPhaiNhapDuongDung);
                        frmAssignPrescription.cboMedicineUseForm.Focus();
                        frmAssignPrescription.cboMedicineUseForm.ShowPopup();
                        success = false;
                    }
                    else
                    {
                        this.SaveDataAndRefesh();
                        success = true;
                    }
                }
                else
                {
                    LogSystem.Debug("IEdit.Run => CheckValidPre = false");
                }
            }
            catch (Exception ex)
            {
                success = false;
                MessageManager.Show(Param, success);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        protected bool ValidKidneyShift()
        {
            bool valid = true;

            if (this.IsKidneyShift.HasValue && this.IsKidneyShift.Value)
            {
                valid = this.KidneyShiftCount > 0;
            }
            if (!valid)
            {
                Param.Messages.Add(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc));
                throw new ArgumentNullException("KidneyShiftCount is null");
            }
            return valid;
        }

        protected void SetValidKidneyShiftError()
        {
            try
            {
                if (this.IsKidneyShift.HasValue && this.IsKidneyShift.Value)
                {
                    medicineTypeSDO.ErrorMessageMediMatyBean = Resources.ResourceMessage.ThuocChayThanKhongCapChoBNMangVeKeNgoaiKho;
                    medicineTypeSDO.ErrorTypeMediMatyBean = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                }
                else
                {
                    medicineTypeSDO.ErrorMessageMediMatyBean = "";
                    medicineTypeSDO.ErrorTypeMediMatyBean = DevExpress.XtraEditors.DXErrorProvider.ErrorType.None;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
