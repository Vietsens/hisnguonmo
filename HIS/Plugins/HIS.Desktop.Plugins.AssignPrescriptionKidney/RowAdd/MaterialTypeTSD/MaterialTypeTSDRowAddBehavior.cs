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
using HIS.Desktop.Plugins.AssignPrescriptionKidney.AssignPrescription;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.Resources;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using System;

namespace HIS.Desktop.Plugins.AssignPrescriptionKidney.Add.MaterialType
{
    class MaterialTypeTSDRowAddBehavior : AddAbstract, IAdd
    {
        long expMestId;
        internal MaterialTypeTSDRowAddBehavior(CommonParam param,
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
            this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD;
            this.Code = frmAssignPrescription.currentMedicineTypeADOForEdit.MEDICINE_TYPE_CODE;
            this.Name = frmAssignPrescription.currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME;
            this.ManuFacturerName = frmAssignPrescription.currentMedicineTypeADOForEdit.MANUFACTURER_NAME;
            this.ServiceUnitName = frmAssignPrescription.currentMedicineTypeADOForEdit.SERVICE_UNIT_NAME;
            this.NationalName = frmAssignPrescription.currentMedicineTypeADOForEdit.NATIONAL_NAME;
            this.ServiceId = frmAssignPrescription.currentMedicineTypeADOForEdit.SERVICE_ID;
            this.Concentra = frmAssignPrescription.currentMedicineTypeADOForEdit.CONCENTRA;
            this.HeinServiceTypeId = frmAssignPrescription.currentMedicineTypeADOForEdit.HEIN_SERVICE_TYPE_ID;
            this.ServiceTypeId = frmAssignPrescription.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID;
            this.IsOutKtcFee = ((frmAssignPrescription.currentMedicineTypeADOForEdit.IS_OUT_PARENT_FEE ?? -1) == 1);
            this.expMestId = frmAssignPrescription.oldExpMestId;
            this.MaxReuseCount = frmAssignPrescription.currentMedicineTypeADOForEdit.MAX_REUSE_COUNT;//TODO
            this.UseRemainCount = frmAssignPrescription.currentMedicineTypeADOForEdit.USE_REMAIN_COUNT;//TODO
            this.UseCount = frmAssignPrescription.currentMedicineTypeADOForEdit.USE_COUNT;//TODO
            this.SeriNumber = frmAssignPrescription.currentMedicineTypeADOForEdit.SERIAL_NUMBER;
            this.MediStockId = frmAssignPrescription.currentMedicineTypeADOForEdit.MEDI_STOCK_ID;
            this.MediStockCode = frmAssignPrescription.currentMedicineTypeADOForEdit.MEDI_STOCK_CODE;
            this.MediStockName = frmAssignPrescription.currentMedicineTypeADOForEdit.MEDI_STOCK_NAME;
            this.Amount = 1;
        }

        bool IAdd.Run()
        {
            bool success = false;
            try
            {
                if (ValidSerialNumber() && this.CheckPatientTypeHasValue())
                {
                    this.CreateADO();
                    this.UpdatePatientTypeInDataRow(this.medicineTypeSDO);
                    this.SetValidPatientTypeError();

                    //if (medicineTypeSDO.ErrorMessageMedicineUseForm == ResourceMessage.BenhNhanDoiTuongTTBhytBatBuocPhaiNhapDuongDung
                    //   && medicineTypeSDO.ErrorTypeMedicineUseForm == ErrorType.Warning)
                    //{
                    //    MessageManager.Show(ResourceMessage.BenhNhanDoiTuongTTBhytBatBuocPhaiNhapDuongDung);
                    //    frmAssignPrescription.cboMedicineUseForm.Focus();
                    //    frmAssignPrescription.cboMedicineUseForm.ShowPopup();
                    //    success = false;
                    //}
                    //else
                    //{
                    if (TakeOrReleaseBeanWorker.TakeForCreateBeanTSD(this.expMestId, this.medicineTypeSDO, false, Param))
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.medicineTypeSDO), this.medicineTypeSDO));
                        success = true;
                        this.medicineTypeSDO.PrimaryKey = this.medicineTypeSDO.SERVICE_ID + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString();
                        this.SaveDataAndRefesh(this.medicineTypeSDO);
                        frmAssignPrescription.ReloadDataAvaiableMediBeanInCombo();
                    }
                    else
                    {
                        //Release stent
                        MessageManager.Show(Param, success);
                        this.medicineTypeSDO = null;
                        return success = false;
                    }
                    //}
                }
            }
            catch (Exception ex)
            {
                success = false;
                medicineTypeSDO = null;
                MessageManager.Show(Param, success);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        bool ValidSerialNumber()
        {
            if (String.IsNullOrEmpty(this.SeriNumber))
            {
                Param.Messages.Add("Thiếu trường dữ liệu bắt buộc");
                throw new ArgumentNullException("Add materialTSD check SerialNumber valid fail => add material fail");
            }

            return true;
        }
    }
}
