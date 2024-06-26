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
using HIS.UC.HisBloodTypeInStock.ADO;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExpMestDepaCreate.ADO
{
    public class MediMateTypeADO
    {
        public ExpMaterialTypeSDO ExpMaterial { get; set; }
        public ExpMedicineTypeSDO ExpMedicine { get; set; }
        public ExpBloodTypeSDO ExpBlood{get;set;}

        public long SERVICE_ID { get; set; }
        public long MEDI_MATE_TYPE_ID { get; set; }
        public string MEDI_MATE_TYPE_CODE { get; set; }
        public string MEDI_MATE_TYPE_NAME { get; set; }
        public string MEDI_CONCEN_TRA { get; set; }
     
        public long? BLOOD_ID_ABO {get;set;}
        public long? BLOOD_ID_RH { get; set; }
        public long BLOOD_TYPE_ID{get;set;}

        public string SERVICE_UNIT_NAME { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public string NATIONAL_NAME { get; set; }
        public string MANUFACTURER_NAME { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public decimal? AVAILABLE_AMOUNT { get; set; }

        public decimal VOLUME { get; set; }

        public decimal EXP_AMOUNT { get; set; }
        public string NOTE { get; set; }

        public bool IsMedicine { get; set; }

        public bool IsHoaChat { get; set; }

        public bool IsBlood { get; set; }

        public MediMateTypeADO() { }

        public MediMateTypeADO(HisMedicineTypeInStockSDO medicine)
        {
            try
            {
                if (medicine != null)
                {
                    this.IsMedicine = true;
                    this.IsBlood = false;
                    this.SERVICE_ID = medicine.ServiceId;
                    this.MEDI_MATE_TYPE_ID = medicine.Id;
                    this.MEDI_MATE_TYPE_CODE = medicine.MedicineTypeCode;
                    this.MEDI_MATE_TYPE_NAME = medicine.MedicineTypeName;
                    this.MEDI_CONCEN_TRA =     medicine.Concentra;
                    this.SERVICE_UNIT_NAME = medicine.ServiceUnitName;
                    this.NATIONAL_NAME = medicine.NationalName;
                    this.MANUFACTURER_NAME = medicine.ManufacturerName;
                    this.AVAILABLE_AMOUNT = medicine.AvailableAmount;
                    this.REGISTER_NUMBER = medicine.RegisterNumber;
                    this.ExpMedicine = new ExpMedicineTypeSDO();
                    this.ExpMedicine.MedicineTypeId = medicine.Id;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public MediMateTypeADO(HisMaterialTypeInStockSDO material)
        {
            try
            {
                if (material != null)
                {
                    this.IsMedicine = false;
                    this.IsBlood = false;
                    this.SERVICE_ID = material.ServiceId;
                    this.MEDI_MATE_TYPE_ID = material.Id;
                    this.MEDI_MATE_TYPE_CODE = material.MaterialTypeCode;
                    this.MEDI_MATE_TYPE_NAME = material.MaterialTypeName;
                    this.MEDI_CONCEN_TRA = material.Concentra;
                    this.SERVICE_UNIT_NAME = material.ServiceUnitName;
                    this.NATIONAL_NAME = material.NationalName;
                    this.MANUFACTURER_NAME = material.ManufacturerName;
                    this.AVAILABLE_AMOUNT = material.AvailableAmount;
                    this.ExpMaterial = new ExpMaterialTypeSDO();
                    this.ExpMaterial.MaterialTypeId = material.Id;
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
                    this.IsMedicine = false;
                    this.SERVICE_ID = blood.ServiceId;
                    this.MEDI_MATE_TYPE_ID = blood.Id;
                    this.MEDI_MATE_TYPE_CODE = blood.BloodTypeCode;
                    this.MEDI_MATE_TYPE_NAME = blood.BloodTypeName;
                    this.SERVICE_UNIT_NAME = "Túi";
                    this.VOLUME=blood.Volume;
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
        public MediMateTypeADO(HisBloodTypeInStockADO blood)
        {
            try
            {
                if (blood != null)
                {
                    this.IsBlood = true;
                    this.IsMedicine = false;
                    this.SERVICE_ID = blood.ServiceId ?? 0;
                    this.MEDI_MATE_TYPE_ID = blood.Id ?? 0;
                    this.MEDI_MATE_TYPE_CODE = blood.BloodTypeCode;
                    this.MEDI_MATE_TYPE_NAME = blood.BloodTypeName;
                    this.SERVICE_UNIT_NAME = "Túi";
                    this.VOLUME = blood.Volume;
                    this.AVAILABLE_AMOUNT = blood.Amount;
                    this.ExpBlood = new ExpBloodTypeSDO();
                    this.ExpBlood.BloodTypeId = blood.Id ?? 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
