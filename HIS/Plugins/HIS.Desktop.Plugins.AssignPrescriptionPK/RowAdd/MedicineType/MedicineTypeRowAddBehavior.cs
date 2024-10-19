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
using HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using HIS.Desktop.Plugins.AssignPrescriptionPK.MessageBoxForm;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Worker;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.Add.MedicineType
{
    class MedicineTypeRowAddBehavior : AddAbstract, IAdd
    {
        internal MedicineTypeRowAddBehavior(CommonParam param,
            frmAssignPrescription frmAssignPrescription,
            ValidAddRow validAddRow,
            HIS.Desktop.Plugins.AssignPrescriptionPK.MediMatyCreateWorker.ChoosePatientTypeDefaultlService choosePatientTypeDefaultlService,
            HIS.Desktop.Plugins.AssignPrescriptionPK.MediMatyCreateWorker.ChoosePatientTypeDefaultlServiceOther choosePatientTypeDefaultlServiceOther,
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
            this.IS_SPLIT_COMPENSATION = frmAssignPrescription.currentMedicineTypeADOForEdit.IS_SPLIT_COMPENSATION;

            this.ATC_CODES = frmAssignPrescription.currentMedicineTypeADOForEdit.ATC_CODES;
            this.ATC_GROUP_CODES = frmAssignPrescription.currentMedicineTypeADOForEdit.ATC_GROUP_CODES;
            this.DESCRIPTION = frmAssignPrescription.currentMedicineTypeADOForEdit.DESCRIPTION;
            this.CONTRAINDICATION = frmAssignPrescription.currentMedicineTypeADOForEdit.CONTRAINDICATION;
            this.CONTRAINDICATION_IDS = frmAssignPrescription.currentMedicineTypeADOForEdit.CONTRAINDICATION_IDS;
            this.IS_OUT_HOSPITAL = frmAssignPrescription.currentMedicineTypeADOForEdit.IS_OUT_HOSPITAL;
            this.SERVICE_CONDITION_ID = frmAssignPrescription.currentMedicineTypeADOForEdit.SERVICE_CONDITION_ID;
            this.SERVICE_CONDITION_NAME = frmAssignPrescription.currentMedicineTypeADOForEdit.SERVICE_CONDITION_NAME;

            this.IsAllowOdd = frmAssignPrescription.currentMedicineTypeADOForEdit.IsAllowOdd;
            this.IsAllowOddAndExportOdd = frmAssignPrescription.currentMedicineTypeADOForEdit.IsAllowOddAndExportOdd;

            this.IsKidneyShift = frmAssignPrescription.chkPreKidneyShift.Checked;
            this.KidneyShiftCount = frmAssignPrescription.spinKidneyCount.Value;
            if (!String.IsNullOrEmpty(frmAssignPrescription.txtPreviousUseDay.Text) && Inventec.Common.TypeConvert.Parse.ToInt64(frmAssignPrescription.txtPreviousUseDay.Text) > 0)
                this.PREVIOUS_USING_COUNT = Inventec.Common.TypeConvert.Parse.ToInt64(frmAssignPrescription.txtPreviousUseDay.Text);
            else
                this.PREVIOUS_USING_COUNT = null;
        }

        bool IAdd.Run()
        {
            bool success = false;
            medicineTypeSDO__Category__SameMediAcin = null;
            try
            {
                if (this.CheckValidPre()
                    && this.ValidThuocDaKeTrongNgay()
                    && MedicineAgeWorker.ValidThuocCoGioiHanTuoi(this.ServiceId, frmAssignPrescription.patientDob)
                    && ValidAcinInteractiveWorker.ValidGrade(this.DataRow, MediMatyTypeADOs, ref frmAssignPrescription.txtInteractionReason, frmAssignPrescription)
                    && WarningOddConvertWorker.CheckWarningOddConvertAmount(frmAssignPrescription.currentMedicineTypeADOForEdit, this.Amount, frmAssignPrescription.ResetFocusMediMaty)
                    && ValidKidneyShift()
                    && this.ValidKhaDungThuocTrongNhaThuoc()
                    && ValidAcinInteractiveWorker.ValidSameAcin(MediMatyTypeADOs, frmAssignPrescription.currentMedicineTypeADOForEdit)
                    && ValidThuocWithContraindicaterWarningOption()
                    )
                {
                    //Nếu thuốc đã kê không đủ khả dụng trong kho, người dùng chọn lấy thuốc ngoài kho thay thế
                    //==> Lấy các thuốc ngoài kho + các thông tin số lượng, đường dùng, cách dùng, hướng dẫn sử dụng,.. đã chọn => tự động bổ sung vào danh sách thuốc đã chọn luôn
                    if (medicineTypeSDO__Category__SameMediAcin != null)
                    {
                        this.UpdateMediMatyByMedicineTypeCategory(medicineTypeSDO__Category__SameMediAcin);
                    }
                    //Nếu thuốc còn khả dụng trong kho
                    //==> Set các thông tin đối tượng mặc định, đường dùng, thời gian dùng, validate,...
                    else
                    {
                        this.CreateADO();
                    }
                    //119139 V+
                    if (frmAssignPrescription.intructionTimeSelected != null && frmAssignPrescription.intructionTimeSelected.Count < 2 && !frmAssignPrescription.IsSelectMultiPatient())
                    {
                        if (!frmAssignPrescription.GetOverReason(medicineTypeSDO))
                            return success;
                    }
                    this.medicineTypeSDO.UpdateAmountAutoRoundUpByAllowOddInDataRow(this.medicineTypeSDO);
                    this.medicineTypeSDO.UpdateAutoRoundUpByConvertUnitRatioInDataRow(this.medicineTypeSDO, frmAssignPrescription.VHistreatment);
                    this.UpdateMedicineUseFormInDataRow(this.medicineTypeSDO);
                    this.UpdateUseTimeInDataRow(this.medicineTypeSDO);
                    this.SetValidAssianInDayError();
                    this.SetValidAmountError();
                    this.SetValidKidneyShiftError();
                    this.medicineTypeSDO.PrimaryKey = (this.medicineTypeSDO.SERVICE_ID + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
                    if (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet ? frmAssignPrescription.CheckMaxInPrescriptionInBatchWithMultilPatient(medicineTypeSDO, medicineTypeSDO.AMOUNT) : frmAssignPrescription.CheckMaxInPrescriptionInBatch(medicineTypeSDO, medicineTypeSDO.AMOUNT))
                    {
                        if (!(GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet))
                            medicineTypeSDO.EXCEED_LIMIT_IN_BATCH_REASON = frmAssignPrescription.reasonMaxPrescriptionBatch;
                        this.SaveDataAndRefesh(this.medicineTypeSDO);
                        success = true;
                    }
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
