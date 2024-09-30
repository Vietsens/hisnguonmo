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

namespace HIS.Desktop.Plugins.MedicineSaleBill.ADO
{
    public class MediMateTypeADO
    {
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
        public string DESCRIPTION { get; set; }

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

        public string EXP_MEST_CODE { get; set; }
        public string TDl_SERVICE_REQ_CODE { get; set; }
        public long EXP_MEST_ID { get; set; }
        public long? BILL_ID { get; set; }

        public bool Check { get; set; }

        public decimal? IMP_VAT_RATIO { get; set; }

        public MediMateTypeADO() { }

        public MediMateTypeADO(V_HIS_EXP_MEST_MEDICINE medicine, V_HIS_EXP_MEST exp)
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
                this.REGISTER_NUMBER = medicine.REGISTER_NUMBER;
                this.EXP_VAT_RATIO = medicine.VAT_RATIO;
                this.EXP_AMOUNT = (medicine.AMOUNT - (medicine.TH_AMOUNT ?? 0));
                this.ADVISORY_PRICE = medicine.PRICE;
                this.ADVISORY_TOTAL_PRICE = (this.EXP_AMOUNT * (this.ADVISORY_PRICE ?? 0) * (1 + (EXP_VAT_RATIO ?? 0)));
                this.DESCRIPTION = medicine.DESCRIPTION;
                this.DISCOUNT = medicine.DISCOUNT;
                this.EXP_MEST_ID = exp.ID;
                this.EXP_MEST_CODE = String.Format("{0}({1})", exp.TDL_SERVICE_REQ_CODE ?? "", exp.EXP_MEST_CODE);
                this.BILL_ID = exp.BILL_ID;
                this.IMP_VAT_RATIO = medicine.IMP_VAT_RATIO;
            }
        }

        public MediMateTypeADO(V_HIS_EXP_MEST_MATERIAL material, V_HIS_EXP_MEST exp)
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
                this.ADVISORY_PRICE = material.PRICE;
                this.EXP_AMOUNT = (material.AMOUNT - (material.TH_AMOUNT ?? 0));
                this.EXP_VAT_RATIO = material.VAT_RATIO;
                this.ADVISORY_TOTAL_PRICE = (this.EXP_AMOUNT * (this.ADVISORY_PRICE ?? 0) * (1 + (EXP_VAT_RATIO ?? 0)));
                this.DESCRIPTION = material.DESCRIPTION;
                this.DISCOUNT = material.DISCOUNT;
                this.EXP_MEST_ID = exp.ID;
                this.EXP_MEST_CODE = String.Format("{0}({1})", exp.TDL_SERVICE_REQ_CODE ?? "", exp.EXP_MEST_CODE);
                this.BILL_ID = exp.BILL_ID;
                this.IMP_VAT_RATIO = material.IMP_VAT_RATIO;
            }
        }

        public MediMateTypeADO(List<V_HIS_EXP_MEST_MEDICINE> medicines, V_HIS_EXP_MEST exp)
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

                this.EXP_AMOUNT = medicines.Sum(s => (s.AMOUNT - (s.TH_AMOUNT ?? 0)));
                this.NOTE = medicines.First().DESCRIPTION;
                this.DISCOUNT = medicines.Sum(s => s.DISCOUNT ?? 0);
                this.EXP_PRICE = medicines.First().PRICE;
                this.EXP_VAT_RATIO = medicines.First().VAT_RATIO;
                this.TUTORIAL = medicines.First().TUTORIAL;

                this.ADVISORY_PRICE = medicines.First().PRICE;
                this.ADVISORY_TOTAL_PRICE = (this.EXP_AMOUNT * (this.ADVISORY_PRICE ?? 0) * (1 + (EXP_VAT_RATIO ?? 0)));
                this.EXP_MEST_ID = exp.ID;
                this.EXP_MEST_CODE = String.Format("{0}({1})", exp.TDL_SERVICE_REQ_CODE ?? "", exp.EXP_MEST_CODE);
                this.BILL_ID = exp.BILL_ID;
                this.IMP_VAT_RATIO = medicines.First().IMP_VAT_RATIO;
            }
        }

        public MediMateTypeADO(List<V_HIS_EXP_MEST_MATERIAL> materials, V_HIS_EXP_MEST exp)
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

                this.EXP_AMOUNT = materials.Sum(s => (s.AMOUNT - (s.TH_AMOUNT ?? 0)));
                this.NOTE = materials.First().DESCRIPTION;
                this.DISCOUNT = materials.Sum(s => s.DISCOUNT ?? 0);
                this.EXP_PRICE = materials.First().PRICE;
                this.EXP_VAT_RATIO = materials.First().VAT_RATIO;
                this.ADVISORY_PRICE = materials.First().PRICE;
                this.ADVISORY_TOTAL_PRICE = (this.EXP_AMOUNT * (this.ADVISORY_PRICE ?? 0) * (1 + (EXP_VAT_RATIO ?? 0)));
                this.EXP_MEST_ID = exp.ID;
                this.EXP_MEST_CODE = String.Format("{0}({1})", exp.TDL_SERVICE_REQ_CODE ?? "", exp.EXP_MEST_CODE);
                this.BILL_ID = exp.BILL_ID;
                this.IMP_VAT_RATIO = materials.First().IMP_VAT_RATIO;
            }
        }
    }
    public class ItemConfig
    {
        public string BANK { get; set; }
        //public List<string> VALUE { get; set; }
        public string VALUE { get; set; }
    }
}
