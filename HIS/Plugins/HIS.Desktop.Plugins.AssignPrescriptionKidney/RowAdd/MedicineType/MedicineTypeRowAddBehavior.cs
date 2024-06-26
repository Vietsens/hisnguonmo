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
using HIS.Desktop.Plugins.AssignPrescriptionKidney.AssignPrescription;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.Worker;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using System;

namespace HIS.Desktop.Plugins.AssignPrescriptionKidney.Add.MedicineType
{
    class MedicineTypeRowAddBehavior : AddAbstract, IAdd
    {
        internal MedicineTypeRowAddBehavior(CommonParam param,
            frmAssignPrescription frmAssignPrescription,
            ValidAddRow validAddRow,
            GetPatientTypeBySeTy getPatientTypeBySeTy,
            CalulateUseTimeTo calulateUseTimeTo,
            ExistsAssianInDay existsAssianInDay,
            object dataRow)
            : base(param,
             frmAssignPrescription,
             validAddRow,
             getPatientTypeBySeTy,
             calulateUseTimeTo,
             existsAssianInDay,
             dataRow)
        {
            this.Id = frmAssignPrescription.currentMedicineTypeADOForEdit.ID;
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
            this.Speed = frmAssignPrescription.spinTocDoTruyen.EditValue != null ? (decimal?)frmAssignPrescription.spinTocDoTruyen.Value : null;

            this.IsKidneyShift = frmAssignPrescription.chkPreKidneyShift.Checked;
            this.KidneyShiftCount = frmAssignPrescription.spinKidneyCount.Value;
        }

        bool IAdd.Run()
        {
            bool success = false;
            try
            {
                if (this.CheckValidPre()
                    && this.ValidThuocDaKeTrongNgay()
                    && MedicineAgeWorker.ValidThuocCoGioiHanTuoi(this.ServiceId, frmAssignPrescription.patientDob)
                    && ValidAcinInteractiveWorker.Valid(this.DataRow, MediMatyTypeADOs)
                    && ValidKidneyShift())
                {
                    this.CreateADO();
                    this.UpdateMedicineUseFormInDataRow(this.medicineTypeSDO);
                    this.UpdateUseTimeInDataRow(this.medicineTypeSDO);
                    this.SetValidAssianInDayError();
                    this.SetValidAmountError();
                    this.SetValidKidneyShiftError();

                    medicineTypeSDO.PrimaryKey = (medicineTypeSDO.SERVICE_ID + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());

                    this.SaveDataAndRefesh(medicineTypeSDO);
                    //frmAssignPrescription.ReloadDataAvaiableMediBeanInCombo();
                    success = true;
                }
                else
                {
                    this.medicineTypeSDO = null;
                }
            }
            catch (Exception ex)
            {
                success = false;
                this.medicineTypeSDO = null;
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
