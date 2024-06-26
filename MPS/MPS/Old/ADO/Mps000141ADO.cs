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

namespace MPS.ADO
{
    public class Mps000141ADO
    {
        public string MEDI_MATE_TYPE_NAME { get; set; }
        public string MEDI_MATE_TYPE_CODE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public string EXPIRED_DATE_STR { get; set; }
        public decimal AMOUNT { get; set; }
        public decimal TOTAL_PRICE { get; set; }
        public decimal? PRICE { get; set; }
        // add new

        public string ACTIVE_INGR_BHYT_CODE { get; set; }
        public string ACTIVE_INGR_BHYT_NAME { get; set; }
        public string BID_NAME { get; set; }
        public string BID_NUMBER { get; set; }
        public string BYT_NUM_ORDER { get; set; }
        public string CONCENTRA { get; set; }
        public long? CREATE_TIME { get; set; }
        public string CREATOR { get; set; }
        public string IMP_MEST_CODE { get; set; }
        public decimal IMP_PRICE { get; set; }
        public long? IMP_TIME { get; set; }
        public decimal IMP_VAT_RATIO { get; set; }
        public decimal? INTERNAL_PRICE { get; set; }
        public short? IS_ADDICTIVE { get; set; }
        public short? IS_NEUROLOGICAL { get; set; }
        public short? IS_STAR_MARK { get; set; }
        public string MANUFACTURER_CODE { get; set; }
        public string MANUFACTURER_NAME { get; set; }
        public string MEDICINE_BYT_NUM_ORDER { get; set; }
        public short? MEDICINE_IS_STAR_MARK { get; set; }
        public string MEDICINE_REGISTER_NUMBER { get; set; }
        public string MEDICINE_TCY_NUM_ORDER { get; set; }
        public string MEDICINE_TYPE_CODE { get; set; }
        public string MEDICINE_TYPE_NAME { get; set; }
        public string MODIFIER { get; set; }
        public long? MODIFY_TIME { get; set; }
        public string NATIONAL_NAME { get; set; }
        public long? NUM_ORDER { get; set; }
        public string PACKING_TYPE_CODE { get; set; }
        public string PACKING_TYPE_NAME { get; set; }
        public string SERVICE_UNIT_CODE { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_NAME { get; set; }
        public string TCY_NUM_ORDER { get; set; }
        public decimal? VAT_RATIO { get; set; }


        public Mps000141ADO(V_HIS_IMP_MEST_MEDICINE medicine)
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
                    if (this.PRICE.HasValue)
                    {
                        this.TOTAL_PRICE += this.PRICE.Value * this.AMOUNT;
                    }

                    this.ACTIVE_INGR_BHYT_CODE = medicine.ACTIVE_INGR_BHYT_CODE;
                    this.ACTIVE_INGR_BHYT_CODE = medicine.ACTIVE_INGR_BHYT_CODE;
                    this.ACTIVE_INGR_BHYT_NAME = medicine.ACTIVE_INGR_BHYT_NAME;
                    this.BID_NAME = medicine.BID_NAME;
                    this.BID_NUMBER = medicine.BID_NUMBER;
                    this.BYT_NUM_ORDER = medicine.BYT_NUM_ORDER;
                    this.CONCENTRA = medicine.CONCENTRA;
                    this.CREATE_TIME = medicine.CREATE_TIME;
                    this.CREATOR = medicine.CREATOR;
                    this.IMP_MEST_CODE = medicine.IMP_MEST_CODE;
                    this.IMP_PRICE = medicine.IMP_PRICE;
                    this.IMP_TIME = medicine.IMP_TIME;
                    this.IMP_VAT_RATIO = medicine.IMP_VAT_RATIO;
                    this.INTERNAL_PRICE = medicine.INTERNAL_PRICE;
                    this.IS_STAR_MARK = medicine.IS_STAR_MARK;
                    this.MANUFACTURER_CODE = medicine.MANUFACTURER_CODE;
                    this.MANUFACTURER_NAME = medicine.MANUFACTURER_NAME;
                    this.MEDICINE_BYT_NUM_ORDER = medicine.MEDICINE_BYT_NUM_ORDER;
                    this.MEDICINE_IS_STAR_MARK = medicine.MEDICINE_IS_STAR_MARK;
                    this.MEDICINE_REGISTER_NUMBER = medicine.MEDICINE_REGISTER_NUMBER;
                    this.MEDICINE_TCY_NUM_ORDER = medicine.MEDICINE_TCY_NUM_ORDER;
                    this.MEDICINE_TYPE_CODE = medicine.MEDICINE_TYPE_CODE;
                    this.MEDICINE_TYPE_NAME = medicine.MEDICINE_TYPE_NAME;
                    this.MODIFIER = medicine.MODIFIER;
                    this.MODIFY_TIME = medicine.MODIFY_TIME;
                    this.NATIONAL_NAME = medicine.NATIONAL_NAME;
                    this.NUM_ORDER = medicine.NUM_ORDER;
                    //this.PACKING_TYPE_CODE = medicine.PACKING_TYPE_CODE;
                    this.PACKING_TYPE_NAME = medicine.PACKING_TYPE_NAME;
                    this.SERVICE_UNIT_CODE = medicine.SERVICE_UNIT_CODE;
                    this.SUPPLIER_CODE = medicine.SUPPLIER_CODE;
                    this.SUPPLIER_NAME = medicine.SUPPLIER_NAME;
                    this.TCY_NUM_ORDER = medicine.TCY_NUM_ORDER;
                    this.VAT_RATIO = medicine.VAT_RATIO;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000141ADO(V_HIS_IMP_MEST_MATERIAL material)
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
                    this.AMOUNT = material.AMOUNT;
                    this.PRICE = material.PRICE;
                    if (this.PRICE.HasValue)
                    {
                        this.TOTAL_PRICE += this.AMOUNT * this.PRICE.Value;
                    }
                    this.BID_NAME = material.BID_NAME;
                    this.BID_NUMBER = material.BID_NUMBER;
                    this.CREATE_TIME = material.CREATE_TIME;
                    this.CREATOR = material.CREATOR;
                    this.IMP_MEST_CODE = material.IMP_MEST_CODE;
                    this.IMP_PRICE = material.IMP_PRICE;
                    this.IMP_TIME = material.IMP_TIME;
                    this.IMP_VAT_RATIO = material.IMP_VAT_RATIO;
                    this.INTERNAL_PRICE = material.INTERNAL_PRICE;
                    this.MANUFACTURER_CODE = material.MANUFACTURER_CODE;
                    this.MANUFACTURER_NAME = material.MANUFACTURER_NAME;
                    this.MODIFIER = material.MODIFIER;
                    this.MODIFY_TIME = material.MODIFY_TIME;
                    this.NATIONAL_NAME = material.NATIONAL_NAME;
                    this.NUM_ORDER = material.NUM_ORDER;
                    //this.PACKING_TYPE_CODE = material.PACKING_TYPE_CODE;
                    this.PACKING_TYPE_NAME = material.PACKING_TYPE_NAME;
                    this.SERVICE_UNIT_CODE = material.SERVICE_UNIT_CODE;
                    this.SUPPLIER_CODE = material.SUPPLIER_CODE;
                    this.SUPPLIER_NAME = material.SUPPLIER_NAME;
                    this.VAT_RATIO = material.VAT_RATIO;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
