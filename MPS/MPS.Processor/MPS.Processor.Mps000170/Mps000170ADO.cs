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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000170
{
    class Mps000170ADO
    {
        public string MEDI_MATE_TYPE_NAME { get; set; }
        public string MEDI_MATE_TYPE_CODE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string MANUFACTURER_NAME { get; set; }
        public string NATIONAL_NAME { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public string EXPIRED_DATE_STR { get; set; }
        public decimal AMOUNT { get; set; }
        public decimal? PRICE { get; set; }
        public decimal? VAT_RATIO_100 { get; set; }
        public decimal IMP_VAT_RATIO_100 { get; set; }
        public decimal PRICE_AMOUNT { get; set; }

        public Mps000170ADO(V_HIS_IMP_MEST_MEDICINE medicine)
        {
            try
            {
                if (medicine != null)
                {
                    this.MEDI_MATE_TYPE_NAME = medicine.MEDICINE_TYPE_NAME;
                    this.MEDI_MATE_TYPE_CODE = medicine.MEDICINE_TYPE_CODE;
                    this.SERVICE_UNIT_NAME = medicine.SERVICE_UNIT_NAME;
                    this.REGISTER_NUMBER = medicine.REGISTER_NUMBER;
                    this.PACKAGE_NUMBER = medicine.PACKAGE_NUMBER;
                    this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(medicine.EXPIRED_DATE ?? 0);
                    this.AMOUNT = medicine.AMOUNT;
                    this.PRICE = medicine.PRICE;
                    this.MANUFACTURER_NAME = medicine.MANUFACTURER_NAME;
                    this.NATIONAL_NAME = medicine.NATIONAL_NAME;
                    this.VAT_RATIO_100 = medicine.VAT_RATIO * 100;
                    this.IMP_VAT_RATIO_100 = medicine.IMP_VAT_RATIO * 100;
                    this.PRICE_AMOUNT = medicine.AMOUNT * (medicine.PRICE ?? 0) * (1 + (medicine.IMP_VAT_RATIO));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000170ADO(V_HIS_IMP_MEST_MATERIAL material)
        {
            try
            {
                if (material != null)
                {
                    this.MEDI_MATE_TYPE_NAME = material.MATERIAL_TYPE_NAME;
                    this.MEDI_MATE_TYPE_CODE = material.MATERIAL_TYPE_CODE;
                    this.SERVICE_UNIT_NAME = material.SERVICE_UNIT_NAME;
                    //this.REGISTER_NUMBER = material.REGISTER_NUMBER;
                    this.PACKAGE_NUMBER = material.PACKAGE_NUMBER;
                    this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(material.EXPIRED_DATE ?? 0);
                    this.MANUFACTURER_NAME = material.MANUFACTURER_NAME;
                    this.NATIONAL_NAME = material.NATIONAL_NAME;
                    this.IMP_VAT_RATIO_100 = material.IMP_VAT_RATIO * 100;
                    this.VAT_RATIO_100 = material.VAT_RATIO * 100;
                    this.AMOUNT = material.AMOUNT;
                    this.PRICE = material.PRICE;
                    this.PRICE_AMOUNT = material.AMOUNT * (material.PRICE ?? 0) * (1 + (material.IMP_VAT_RATIO));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
