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
using HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using System;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.Edit.MaterialTypeTSD
{
    class MaterialTypeTSDRowEditBehavior : EditAbstract, IEdit
    {
        internal MaterialTypeTSDRowEditBehavior(CommonParam param,
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
            this.IS_SUB_PRES = frmAssignPrescription.currentMedicineTypeADOForEdit.IS_SUB_PRES;
            if (!String.IsNullOrEmpty(frmAssignPrescription.txtPreviousUseDay.Text) && Inventec.Common.TypeConvert.Parse.ToInt64(frmAssignPrescription.txtPreviousUseDay.Text) > 0)
                this.PREVIOUS_USING_COUNT = Inventec.Common.TypeConvert.Parse.ToInt64(frmAssignPrescription.txtPreviousUseDay.Text);
            else
                this.PREVIOUS_USING_COUNT = null;
        }

        bool IEdit.Run()
        {
            bool success = false;
            try
            {
                if (this.ValidSerialNumber())
                {
                    this.medicineTypeSDO = frmAssignPrescription.mediMatyTypeADOs.FirstOrDefault(o => o.PrimaryKey == PrimaryKey);
                    this.CreateADO();
                    this.UpdatePatientTypeInDataRow(this.medicineTypeSDO);
                    this.UpdateExpMestReasonInDataRow(this.medicineTypeSDO);
                    this.SetValidPatientTypeError();

                    this.SaveDataAndRefesh();
                    frmAssignPrescription.ReloadDataAvaiableMediBeanInCombo();
                    success = true;
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

        bool ValidSerialNumber()
        {
            if (String.IsNullOrEmpty(this.SeriNumber))
            {
                Param.Messages.Add("Thiếu trường dữ liệu bắt buộc");
                throw new ArgumentNullException("Edit materialTSD check SerialNumber valid fail => edit material fail");
            }

            return true;
        }
    }
}
