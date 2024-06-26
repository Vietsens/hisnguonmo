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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.AssignPrescription;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignPrescriptionYHCT.Add.MedicineType
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
            this.DoNotRequiredUseForm = frmAssignPrescription.currentMedicineTypeADOForEdit.DO_NOT_REQUIRED_USE_FORM ?? -1;
            this.AtcCodes = frmAssignPrescription.currentMedicineTypeADOForEdit.ATC_CODES;
            this.AtcGroupCodes = frmAssignPrescription.currentMedicineTypeADOForEdit.ATC_GROUP_CODES;
        }

        bool IAdd.Run()
        {
            bool success = false;
            this.medicineTypeSDO__Category__SameMediAcin = null;
            try
            {
                if (this.CheckValidPre()
                    && MedicineAgeWorker.ValidThuocCoGioiHanTuoi(this.ServiceId, frmAssignPrescription.patientDob)
                    && this.ValidThuocDaKeTrongNgay()
                    && this.ValidKhaDungThuocTrongNhaThuoc()
                    && HIS.Desktop.Plugins.AssignPrescriptionYHCT.ValidAcinInteractiveWorker.Valid(this.DataRow, MediMatyTypeADOs, this.LstExpMestMedicine))
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
                    this.UpdateMedicineUseFormInDataRow(this.medicineTypeSDO);
                    this.UpdateUseTimeInDataRow(this.medicineTypeSDO);
                    this.SetValidAssianInDayError();
                    this.SetValidAmountError();

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
    }
}
