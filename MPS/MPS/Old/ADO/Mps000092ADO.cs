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
    public class Mps000092ADO
    {
        public long TYPE_ID { get; set; }
        public long MEDI_MATE_TYPE_ID { get; set; }

        public string MEDI_MATE_TYPE_CODE { get; set; }
        public string MEDI_MATE_TYPE_NAME { get; set; }
        public string SERVICE_UNIT_CODE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string TUTORIAL { get; set; }
        public string EXPIRED_DATE_STR { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_NAME { get; set; }

        public decimal? DISCOUNT { get; set; }
        public decimal TOTAL_AMOUNT { get; set; }
        public decimal? PRICE { get; set; }
        public decimal IMP_PRICE { get; set; }
        public decimal? VAT_RATIO_100 { get; set; }
        public decimal SUM_TOTAL_PRICE { get; set; }

        //new
        public string ACTIVE_INGR_BHYT_CODE { get; set; }
        public string ACTIVE_INGR_BHYT_NAME { get; set; }
        public decimal AMOUNT { get; set; }
        public string BID_NAME { get; set; }
        public string BID_NUMBER { get; set; }
        public string BYT_NUM_ORDER { get; set; }
        public decimal? CAN_MOBA_AMOUNT { get; set; }
        public long? CREATE_TIME { get; set; }
        public string DESCRIPTION { get; set; }
        public string EXP_MEST_CODE { get; set; }
        public long? EXP_TIME { get; set; }
        public long? EXPIRED_DATE { get; set; }
        public long? IMP_TIME { get; set; }
        public decimal IMP_VAT_RATIO { get; set; }
        public short? IN_EXECUTE { get; set; }
        public short? IN_REQUEST { get; set; }
        public decimal? INTERNAL_PRICE { get; set; }
        public string MANUFACTURER_CODE { get; set; }
        public string MANUFACTURER_NAME { get; set; }
        public decimal? MEDICINE_BEAN_AMOUNT { get; set; }
        public short? MEDICINE_BEAN_IS_ACTIVE { get; set; }
        public string MEDICINE_BYT_NUM_ORDER { get; set; }
        public string MEDICINE_REGISTER_NUMBER { get; set; }
        public string MEDICINE_TCY_NUM_ORDER { get; set; }
        public long? MEDICINE_TYPE_NUM_ORDER { get; set; }
        public string MEDICINE_USE_FORM_CODE { get; set; }
        public string MEDICINE_USE_FORM_NAME { get; set; }
        public string NATIONAL_NAME { get; set; }
        public long? NUM_ORDER { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public string TCY_NUM_ORDER { get; set; }
        public long? USE_TIME_TO { get; set; }
        public decimal? VAT_RATIO { get; set; }

        public Mps000092ADO()
        {
        }

        public Mps000092ADO(List<V_HIS_EXP_MEST_MEDICINE> listMedicine)
        {
            try
            {
                if (listMedicine != null && listMedicine.Count > 0)
                {
                    this.TYPE_ID = 1;
                    this.MEDI_MATE_TYPE_CODE = listMedicine.First().MEDICINE_TYPE_CODE;
                    this.MEDI_MATE_TYPE_ID = listMedicine.First().MEDICINE_TYPE_ID;
                    this.MEDI_MATE_TYPE_NAME = listMedicine.First().MEDICINE_TYPE_NAME;
                    this.REGISTER_NUMBER = listMedicine.First().REGISTER_NUMBER;
                    this.SERVICE_UNIT_CODE = listMedicine.First().SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = listMedicine.First().SERVICE_UNIT_NAME;
                    this.SUPPLIER_CODE = listMedicine.First().SUPPLIER_CODE;
                    this.SUPPLIER_NAME = listMedicine.First().SUPPLIER_NAME;
                    this.PRICE = listMedicine.First().PRICE;
                    this.TUTORIAL = listMedicine.First().TUTORIAL;
                    this.IMP_PRICE = listMedicine.First().IMP_PRICE;
                    if (listMedicine.First().VAT_RATIO.HasValue)
                    {
                        this.VAT_RATIO_100 = listMedicine.First().VAT_RATIO.Value * 100;
                    }
                    this.DISCOUNT = listMedicine.Sum(o => o.DISCOUNT ?? 0);
                    this.TOTAL_AMOUNT = listMedicine.Sum(s => s.AMOUNT);
                    this.NUM_ORDER = listMedicine.First().NUM_ORDER;
                    if (this.PRICE.HasValue)
                    {
                        var vat = this.VAT_RATIO_100.HasValue ? this.VAT_RATIO_100.Value / 100 : 0;
                        this.SUM_TOTAL_PRICE = this.TOTAL_AMOUNT * this.PRICE.Value * (1 + vat);
                    }
                    ACTIVE_INGR_BHYT_CODE = listMedicine.First().ACTIVE_INGR_BHYT_CODE;
                    ACTIVE_INGR_BHYT_NAME = listMedicine.First().ACTIVE_INGR_BHYT_NAME;
                    AMOUNT = listMedicine.First().AMOUNT;
                    BID_NAME = listMedicine.First().BID_NAME;
                    BID_NUMBER = listMedicine.First().BID_NUMBER;
                    BYT_NUM_ORDER = listMedicine.First().BYT_NUM_ORDER;
                    CAN_MOBA_AMOUNT = listMedicine.First().CAN_MOBA_AMOUNT;
                    CREATE_TIME = listMedicine.First().CREATE_TIME;
                    DESCRIPTION = listMedicine.First().DESCRIPTION;
                    EXP_MEST_CODE = listMedicine.First().EXP_MEST_CODE;
                    EXP_TIME = listMedicine.First().EXP_TIME;
                    EXPIRED_DATE = listMedicine.First().EXPIRED_DATE;
                    IMP_TIME = listMedicine.First().IMP_TIME;
                    IMP_VAT_RATIO = listMedicine.First().IMP_VAT_RATIO;
                    IN_EXECUTE = listMedicine.First().IN_EXECUTE;
                    IN_REQUEST = listMedicine.First().IN_REQUEST;
                    INTERNAL_PRICE = listMedicine.First().INTERNAL_PRICE;
                    MANUFACTURER_CODE = listMedicine.First().MANUFACTURER_CODE;
                    MANUFACTURER_NAME = listMedicine.First().MANUFACTURER_NAME;
                    MEDICINE_BEAN_AMOUNT = listMedicine.First().MEDICINE_BEAN_AMOUNT;
                    MEDICINE_BEAN_IS_ACTIVE = listMedicine.First().MEDICINE_BEAN_IS_ACTIVE;
                    MEDICINE_BYT_NUM_ORDER = listMedicine.First().MEDICINE_BYT_NUM_ORDER;
                    MEDICINE_REGISTER_NUMBER = listMedicine.First().MEDICINE_REGISTER_NUMBER;
                    MEDICINE_TCY_NUM_ORDER = listMedicine.First().MEDICINE_TCY_NUM_ORDER;
                    MEDICINE_TYPE_NUM_ORDER = listMedicine.First().MEDICINE_TYPE_NUM_ORDER;
                    MEDICINE_USE_FORM_CODE = listMedicine.First().MEDICINE_USE_FORM_CODE;
                    MEDICINE_USE_FORM_NAME = listMedicine.First().MEDICINE_USE_FORM_NAME;
                    NATIONAL_NAME = listMedicine.First().NATIONAL_NAME;
                    NUM_ORDER = listMedicine.First().NUM_ORDER;
                    PACKAGE_NUMBER = listMedicine.First().PACKAGE_NUMBER;
                    TCY_NUM_ORDER = listMedicine.First().TCY_NUM_ORDER;
                    USE_TIME_TO = listMedicine.First().USE_TIME_TO;
                    VAT_RATIO = listMedicine.First().VAT_RATIO;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000092ADO(List<V_HIS_EXP_MEST_MATERIAL> listMaterial)
        {
            try
            {
                if (listMaterial != null && listMaterial.Count > 0)
                {
                    this.TYPE_ID = 1;
                    this.MEDI_MATE_TYPE_CODE = listMaterial.First().MATERIAL_TYPE_CODE;
                    this.MEDI_MATE_TYPE_ID = listMaterial.First().MATERIAL_TYPE_ID;
                    this.MEDI_MATE_TYPE_NAME = listMaterial.First().MATERIAL_TYPE_NAME;
                    this.SERVICE_UNIT_CODE = listMaterial.First().SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = listMaterial.First().SERVICE_UNIT_NAME;
                    this.SUPPLIER_CODE = listMaterial.First().SUPPLIER_CODE;
                    this.SUPPLIER_NAME = listMaterial.First().SUPPLIER_NAME;
                    this.IMP_PRICE = listMaterial.First().IMP_PRICE;
                    this.PRICE = listMaterial.First().PRICE;
                    if (listMaterial.First().VAT_RATIO.HasValue)
                    {
                        this.VAT_RATIO_100 = listMaterial.First().VAT_RATIO.Value * 100;
                    }
                    this.DISCOUNT = listMaterial.Sum(o => o.DISCOUNT ?? 0);
                    this.TOTAL_AMOUNT = listMaterial.Sum(s => s.AMOUNT);
                    this.NUM_ORDER = listMaterial.First().NUM_ORDER;
                    if (this.PRICE.HasValue)
                    {
                        var vat = this.VAT_RATIO_100.HasValue ? this.VAT_RATIO_100.Value / 100 : 0;
                        this.SUM_TOTAL_PRICE = this.TOTAL_AMOUNT * this.PRICE.Value * (1 + vat);
                    }
                    AMOUNT = listMaterial.First().AMOUNT;
                    BID_NAME = listMaterial.First().BID_NAME;
                    BID_NUMBER = listMaterial.First().BID_NUMBER;
                    CAN_MOBA_AMOUNT = listMaterial.First().CAN_MOBA_AMOUNT;
                    CREATE_TIME = listMaterial.First().CREATE_TIME;
                    DESCRIPTION = listMaterial.First().DESCRIPTION;
                    EXP_MEST_CODE = listMaterial.First().EXP_MEST_CODE;
                    EXP_TIME = listMaterial.First().EXP_TIME;
                    EXPIRED_DATE = listMaterial.First().EXPIRED_DATE;
                    IMP_TIME = listMaterial.First().IMP_TIME;
                    IMP_VAT_RATIO = listMaterial.First().IMP_VAT_RATIO;
                    IN_EXECUTE = listMaterial.First().IN_EXECUTE;
                    IN_REQUEST = listMaterial.First().IN_REQUEST;
                    INTERNAL_PRICE = listMaterial.First().INTERNAL_PRICE;
                    MANUFACTURER_CODE = listMaterial.First().MANUFACTURER_CODE;
                    MANUFACTURER_NAME = listMaterial.First().MANUFACTURER_NAME;
                    NATIONAL_NAME = listMaterial.First().NATIONAL_NAME;
                    NUM_ORDER = listMaterial.First().NUM_ORDER;
                    PACKAGE_NUMBER = listMaterial.First().PACKAGE_NUMBER;
                    VAT_RATIO = listMaterial.First().VAT_RATIO;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
