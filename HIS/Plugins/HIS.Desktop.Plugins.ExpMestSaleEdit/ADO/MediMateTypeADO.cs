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
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExpMestSaleEdit.ADO
{
    public class MediMateTypeADO
    {
        public HisMaterialTypeAmountWithPriceSDO ExpMaterial { get; set; }
        public HisMedicineTypeAmountWithPriceSDO ExpMedicine { get; set; }

        public long SERVICE_ID { get; set; }
        public long MEDI_MATE_TYPE_ID { get; set; }
        public string MEDI_MATE_TYPE_CODE { get; set; }
        public string MEDI_MATE_TYPE_NAME { get; set; }

        public string SERVICE_UNIT_NAME { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public string NATIONAL_NAME { get; set; }
        public string MANUFACTURER_NAME { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public decimal? AVAILABLE_AMOUNT { get; set; }

        public decimal EXP_AMOUNT { get; set; }
        public string NOTE { get; set; }
        public decimal? DISCOUNT { get; set; }
        public decimal? DISCOUNT_RATIO { get; set; }
        public decimal? EXP_PRICE { get; set; }
        public decimal? EXP_VAT_RATIO { get; set; }
        public string TUTORIAL { get; set; }
        public long? PATIENT_TYPE_ID { get; set; }

        public bool IsMedicine { get; set; }

        public bool IsNotHasMest { get; set; }
        public bool IsGreatAvailable { get; set; }


        public decimal? ADVISORY_PRICE { get; set; }
        public decimal? ADVISORY_TOTAL_PRICE { get; set; }

        public MediMateTypeADO() { }

        public MediMateTypeADO(HisMedicineTypeInStockSDO medicine)
        {
            try
            {
                if (medicine != null)
                {
                    this.IsMedicine = true;
                    this.SERVICE_ID = medicine.ServiceId;
                    this.MEDI_MATE_TYPE_ID = medicine.Id;
                    this.MEDI_MATE_TYPE_CODE = medicine.MedicineTypeCode;
                    this.MEDI_MATE_TYPE_NAME = medicine.MedicineTypeName;
                    this.SERVICE_UNIT_NAME = medicine.ServiceUnitName;
                    this.NATIONAL_NAME = medicine.NationalName;
                    this.MANUFACTURER_NAME = medicine.ManufacturerName;
                    this.REGISTER_NUMBER = medicine.RegisterNumber;
                    this.AVAILABLE_AMOUNT = medicine.AvailableAmount;
                    this.ExpMedicine = new HisMedicineTypeAmountWithPriceSDO();
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
                    this.SERVICE_ID = material.ServiceId;
                    this.MEDI_MATE_TYPE_ID = material.Id;
                    this.MEDI_MATE_TYPE_CODE = material.MaterialTypeCode;
                    this.MEDI_MATE_TYPE_NAME = material.MaterialTypeName;
                    this.SERVICE_UNIT_NAME = material.ServiceUnitName;
                    this.NATIONAL_NAME = material.NationalName;
                    this.MANUFACTURER_NAME = material.ManufacturerName;
                    this.AVAILABLE_AMOUNT = material.AvailableAmount;
                    this.ExpMaterial = new HisMaterialTypeAmountWithPriceSDO();
                    this.ExpMaterial.MaterialTypeId = material.Id;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public MediMateTypeADO(List<V_HIS_EXP_MEST_MEDICINE> medicines)
        {
            try
            {
                if (medicines != null)
                {
                    this.IsMedicine = true;
                    this.SERVICE_ID = medicines.First().SERVICE_ID;
                    this.MEDI_MATE_TYPE_ID = medicines.First().MEDICINE_TYPE_ID;
                    this.MEDI_MATE_TYPE_CODE = medicines.First().MEDICINE_TYPE_CODE;
                    this.MEDI_MATE_TYPE_NAME = medicines.First().MEDICINE_TYPE_NAME;
                    this.SERVICE_UNIT_NAME = medicines.First().SERVICE_UNIT_NAME;
                    this.NATIONAL_NAME = medicines.First().NATIONAL_NAME;
                    this.MANUFACTURER_NAME = medicines.First().MANUFACTURER_NAME;
                    this.REGISTER_NUMBER = medicines.First().REGISTER_NUMBER;

                    this.EXP_AMOUNT = medicines.Sum(s => s.AMOUNT);
                    this.NOTE = medicines.First().DESCRIPTION;
                    this.DISCOUNT = medicines.Sum(s => s.DISCOUNT ?? 0);
                    this.EXP_PRICE = medicines.First().PRICE;
                    this.EXP_VAT_RATIO = medicines.First().VAT_RATIO;
                    this.TUTORIAL = medicines.First().TUTORIAL;
                    if (this.EXP_AMOUNT > 0 && this.EXP_PRICE.HasValue && this.EXP_PRICE.Value > 0)
                    {
                        this.DISCOUNT_RATIO = (this.DISCOUNT ?? 0) / (this.EXP_AMOUNT * this.EXP_PRICE.Value);
                    }

                    this.ADVISORY_PRICE = medicines.First().PRICE;
                    this.ADVISORY_TOTAL_PRICE = (this.EXP_AMOUNT * (this.ADVISORY_PRICE ?? 0)) * (1 + (EXP_VAT_RATIO ?? 0));
                    this.ExpMedicine = new HisMedicineTypeAmountWithPriceSDO();
                    this.ExpMedicine.MedicineTypeId = medicines.First().MEDICINE_TYPE_ID;
                    this.ExpMedicine.Amount = this.EXP_AMOUNT;
                    this.ExpMedicine.Description = this.NOTE;
                    this.ExpMedicine.DiscountRatio = this.DISCOUNT_RATIO;
                    this.ExpMedicine.Price = this.EXP_PRICE;
                    this.ExpMedicine.VatRatio = this.EXP_VAT_RATIO;
                    this.ExpMedicine.Tutorial = this.TUTORIAL;
                    this.ExpMedicine.PatientTypeId = this.PATIENT_TYPE_ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public MediMateTypeADO(List<V_HIS_EXP_MEST_MATERIAL> materials)
        {
            try
            {
                if (materials != null)
                {
                    this.IsMedicine = false;
                    this.SERVICE_ID = materials.First().SERVICE_ID;
                    this.MEDI_MATE_TYPE_ID = materials.First().MATERIAL_TYPE_ID;
                    this.MEDI_MATE_TYPE_CODE = materials.First().MATERIAL_TYPE_CODE;
                    this.MEDI_MATE_TYPE_NAME = materials.First().MATERIAL_TYPE_NAME;
                    this.SERVICE_UNIT_NAME = materials.First().SERVICE_UNIT_NAME;
                    this.NATIONAL_NAME = materials.First().NATIONAL_NAME;
                    this.MANUFACTURER_NAME = materials.First().MANUFACTURER_NAME;

                    this.EXP_AMOUNT = materials.Sum(s => s.AMOUNT);
                    this.NOTE = materials.First().DESCRIPTION;
                    this.DISCOUNT = materials.Sum(s => s.DISCOUNT ?? 0);
                    this.EXP_PRICE = materials.First().PRICE;
                    this.EXP_VAT_RATIO = materials.First().VAT_RATIO;
                    if (this.EXP_AMOUNT > 0 && this.EXP_PRICE.HasValue && this.EXP_PRICE.Value > 0)
                    {
                        this.DISCOUNT_RATIO = (this.DISCOUNT ?? 0) / (this.EXP_AMOUNT * this.EXP_PRICE.Value);
                    }

                    this.ADVISORY_PRICE = materials.First().PRICE;
                    this.ADVISORY_TOTAL_PRICE = (this.EXP_AMOUNT * (this.ADVISORY_PRICE ?? 0)) * (1 + (EXP_VAT_RATIO ?? 0));
                    this.ExpMaterial = new HisMaterialTypeAmountWithPriceSDO();
                    this.ExpMaterial.MaterialTypeId = materials.First().MATERIAL_TYPE_ID;
                    this.ExpMaterial.Amount = this.EXP_AMOUNT;
                    this.ExpMaterial.Description = this.NOTE;
                    this.ExpMaterial.DiscountRatio = this.DISCOUNT_RATIO;
                    this.ExpMaterial.Price = this.EXP_PRICE;
                    this.ExpMaterial.VatRatio = this.EXP_VAT_RATIO;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
