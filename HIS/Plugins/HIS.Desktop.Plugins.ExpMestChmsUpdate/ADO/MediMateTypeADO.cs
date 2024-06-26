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
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExpMestChmsUpdate.ADO
{
    public class MediMateTypeADO
    {
        public ExpMaterialTypeSDO ExpMaterial { get; set; }
        public ExpMedicineTypeSDO ExpMedicine { get; set; }
        public ExpBloodTypeSDO ExpBlood { get; set; }

        public long SERVICE_ID { get; set; }
        public long MEDI_MATE_TYPE_ID { get; set; }
        public string MEDI_MATE_TYPE_CODE { get; set; }
        public string MEDI_MATE_TYPE_NAME { get; set; }

        public long? MEDI_MATE_REQ_ID { get; set; }

        public string SERVICE_UNIT_NAME { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public string NATIONAL_NAME { get; set; }
        public string MANUFACTURER_NAME { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public decimal? AVAILABLE_AMOUNT { get; set; }

        public long MEDICINE_ID { get; set; }
        public decimal? EXPIRED_DATE { get; set; }
        public long MATERIAL_ID { get; set; }
        public bool IsPackage { get; set; }

        public bool IsSupplement { get; set; }


        public decimal EXP_AMOUNT { get; set; }
        public string NOTE { get; set; }
        public string DESCRIPTION { get; set; }

        public bool IsMedicine { get; set; }
        public long? NUM_ORDER { get; set; }

        public bool IsBlood { get; set; }
        public long? BLOOD_RH_ID { get; set; }
        public long? BLOOD_ABO_ID { get; set; }
        public decimal VOLUME { get; set; }
        public string BLOOD_TYPE_HEIN_NAME { get; set; }

        public MediMateTypeADO() { }

        public MediMateTypeADO(HisMedicineInStockSDO medicine)
        {
            try
            {
                if (medicine != null)
                {
                    this.IsMedicine = true;
                    this.SERVICE_ID = medicine.SERVICE_ID;
                    this.MEDI_MATE_TYPE_ID = medicine.MEDICINE_TYPE_ID;
                    this.MEDI_MATE_TYPE_CODE = medicine.MEDICINE_TYPE_CODE;
                    this.MEDI_MATE_TYPE_NAME = medicine.MEDICINE_TYPE_NAME;
                    this.SERVICE_UNIT_NAME = medicine.SERVICE_UNIT_NAME;
                    this.NATIONAL_NAME = medicine.NATIONAL_NAME;
                    this.MANUFACTURER_NAME = medicine.MANUFACTURER_NAME;
                    this.AVAILABLE_AMOUNT = medicine.AvailableAmount;
                    this.REGISTER_NUMBER = medicine.REGISTER_NUMBER;
                    this.ExpMedicine = new ExpMedicineTypeSDO();
                    this.ExpMedicine.MedicineTypeId = medicine.MEDICINE_TYPE_ID;
                    this.MEDICINE_ID = medicine.ID;
                    this.PACKAGE_NUMBER = medicine.PACKAGE_NUMBER;
                    this.EXPIRED_DATE = medicine.EXPIRED_DATE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public MediMateTypeADO(HisMaterialInStockSDO material)
        {
            try
            {
                if (material != null)
                {
                    this.IsMedicine = false;
                    this.SERVICE_ID = material.SERVICE_ID;
                    this.MEDI_MATE_TYPE_ID = material.MATERIAL_TYPE_ID;
                    this.MEDI_MATE_TYPE_CODE = material.MATERIAL_TYPE_CODE;
                    this.MEDI_MATE_TYPE_NAME = material.MATERIAL_TYPE_NAME;
                    this.SERVICE_UNIT_NAME = material.SERVICE_UNIT_NAME;
                    this.NATIONAL_NAME = material.NATIONAL_NAME;
                    this.MANUFACTURER_NAME = material.MANUFACTURER_NAME;
                    this.AVAILABLE_AMOUNT = material.AvailableAmount;
                    this.ExpMaterial = new ExpMaterialTypeSDO();
                    this.ExpMaterial.MaterialTypeId = material.MATERIAL_TYPE_ID;
                    this.MATERIAL_ID = material.ID;
                    this.PACKAGE_NUMBER = material.PACKAGE_NUMBER;
                    this.EXPIRED_DATE = material.EXPIRED_DATE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public MediMateTypeADO(HisBloodTypeInStockSDO blood)
        {
            try
            {
                if (blood != null)
                {
                    this.IsBlood = true;
                    this.SERVICE_ID = blood.ServiceId;
                    this.MEDI_MATE_TYPE_ID = blood.Id;
                    this.MEDI_MATE_TYPE_CODE = blood.BloodTypeCode;
                    this.MEDI_MATE_TYPE_NAME = blood.BloodTypeName;
                    this.BLOOD_TYPE_HEIN_NAME = blood.BloodTypeHeinName;
                    this.VOLUME = blood.Volume;
                    this.AVAILABLE_AMOUNT = blood.Amount;
                    this.ExpBlood = new ExpBloodTypeSDO();
                    this.ExpBlood.BloodTypeId = blood.Id;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
