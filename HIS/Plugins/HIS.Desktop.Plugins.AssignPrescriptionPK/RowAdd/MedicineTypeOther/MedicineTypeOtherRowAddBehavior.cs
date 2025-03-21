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
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription;
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

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.Add.MedicineTypeOther
{
    class MedicineTypeOtherRowAddBehavior : AddAbstract, IAdd
    {
        internal MedicineTypeOtherRowAddBehavior(CommonParam param,
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
            this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC;
            this.Name = frmAssignPrescription.txtMedicineTypeOther.Text.Trim();
            this.ServiceUnitName = frmAssignPrescription.txtUnitOther.Text.Trim();
            if (frmAssignPrescription.spinPrice.EditValue != null)
                this.Price = frmAssignPrescription.spinPrice.Value;
            this.DataRow = this.Name;
            if (!String.IsNullOrEmpty(frmAssignPrescription.txtPreviousUseDay.Text) && Inventec.Common.TypeConvert.Parse.ToInt64(frmAssignPrescription.txtPreviousUseDay.Text) > 0)
                this.PREVIOUS_USING_COUNT = Inventec.Common.TypeConvert.Parse.ToInt64(frmAssignPrescription.txtPreviousUseDay.Text);
            else
                this.PREVIOUS_USING_COUNT = null;
        }

        bool IAdd.Run()
        {
            bool success = false;
            try
            {
                if (this.CheckValidPre()
                    && ValidUnitName())
                {
                    this.CreateADO();
                    medicineTypeSDO.AMOUNT = this.Amount;
                    medicineTypeSDO.SERVICE_UNIT_NAME = this.ServiceUnitName;
                    medicineTypeSDO.MEDICINE_TYPE_NAME = this.Name;
                    medicineTypeSDO.PRICE = this.Price;
                    medicineTypeSDO.TotalPrice = (this.Amount * (medicineTypeSDO.PRICE ?? 0));
                    medicineTypeSDO.CONVERT_RATIO = null;
                    medicineTypeSDO.CONVERT_UNIT_CODE = null;
                    medicineTypeSDO.CONVERT_UNIT_NAME = null;

                    SetValidAssianInDayError();

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

        bool ValidUnitName()
        {
            bool valid = true;
            try
            {
                valid = !String.IsNullOrEmpty(this.ServiceUnitName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
    }
}
