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
using HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using System;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.Add.MaterialType
{
    class MaterialTypeTSDRowAddBehavior : AddAbstract, IAdd
    {
        long expMestId;
        internal MaterialTypeTSDRowAddBehavior(CommonParam param,
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
            this.IS_OUT_HOSPITAL = frmAssignPrescription.currentMedicineTypeADOForEdit.IS_OUT_HOSPITAL;
            this.SERVICE_CONDITION_ID = frmAssignPrescription.currentMedicineTypeADOForEdit.SERVICE_CONDITION_ID;
            this.SERVICE_CONDITION_NAME = frmAssignPrescription.currentMedicineTypeADOForEdit.SERVICE_CONDITION_NAME;
            this.Amount = 1;
            this.frmAssignPrescription.PresAmount = 1;
            this.IsExpend = frmAssignPrescription.currentMedicineTypeADOForEdit.IsExpend;
            if (!String.IsNullOrEmpty(frmAssignPrescription.txtPreviousUseDay.Text) && Inventec.Common.TypeConvert.Parse.ToInt64(frmAssignPrescription.txtPreviousUseDay.Text) > 0)
                this.PREVIOUS_USING_COUNT = Inventec.Common.TypeConvert.Parse.ToInt64(frmAssignPrescription.txtPreviousUseDay.Text);
            else
                this.PREVIOUS_USING_COUNT = null;
            this.IsStent = frmAssignPrescription.currentMedicineTypeADOForEdit.IsStent;
        }

        bool IAdd.Run()
        {
            bool success = false;
            try
            {
                if (ValidSerialNumber() && this.CheckPatientTypeHasValue() && frmAssignPrescription.CheckValidMaterial())
                {
                    this.CreateADO();
                    this.UpdatePatientTypeInDataRow(this.medicineTypeSDO);
                    this.UpdateExpMestReasonInDataRow(this.medicineTypeSDO);
                    this.SetValidPatientTypeError();
                    {
                        this.medicineTypeSDO.PrimaryKey = this.medicineTypeSDO.SERVICE_ID + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString();
                        if (TakeOrReleaseBeanWorker.TakeForCreateBeanTSD(this.expMestId, this.medicineTypeSDO, false, Param,ref this.frmAssignPrescription.lstOutPatientPres,this.frmAssignPrescription.UseTimeSelecteds))
                        {
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.medicineTypeSDO), this.medicineTypeSDO));
                            success = true;
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
                    }
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

            if (frmAssignPrescription.mediMatyTypeADOs != null && frmAssignPrescription.mediMatyTypeADOs.Any(o => o.SERIAL_NUMBER == this.SeriNumber))
            {
                Param.Messages.Add("Vật tư tái sử dụng " + this.Name + " (" + this.SeriNumber + ") đã được kê, không thể kê tiếp");
                throw new ArgumentNullException("Add materialTSD check SerialNumber exists in list => add material fail");
            }

            return true;
        }
    }
}
