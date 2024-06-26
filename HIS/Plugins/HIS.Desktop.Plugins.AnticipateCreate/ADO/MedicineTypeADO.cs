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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AnticipateCreate.ADO
{
    public class MedicineTypeADO : MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE
    {
        public double IdRow { get; set; }
        public decimal? AMOUNT { get; set; }
        public decimal? AllowAmount { get; set; }
        public long Type { get; set; }
        public long? SUPPLIER_ID { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_NAME { get; set; }
        public string BID_NUM_ORDER { get; set; }
        public string IS_MEDICINE { get; set; }
        public long? BID_ID { get; set; }
        public bool isDelete { get; set; }
        public bool isAmount { get; set; }
        public decimal? AMOUNT_EXPORT { get; set; }
        public decimal? AMOUNT_MEDI_STOCK { get; set; }
        public string BID_NUMBER { get; set; }
        public string BID_YEAR { get; set; }
        public bool IsNotSave { get; set; }

        public MedicineTypeADO() { }

        public MedicineTypeADO(MedicineTypeADO medicineType)
        {
            //set truc tiep cac truong trong doi tuong
            this.IS_BUSINESS = medicineType.IS_BUSINESS;
            this.IS_SALE_EQUAL_IMP_PRICE = medicineType.IS_SALE_EQUAL_IMP_PRICE;
            this.ACTIVE_INGR_BHYT_CODE = medicineType.ACTIVE_INGR_BHYT_CODE;
            this.ACTIVE_INGR_BHYT_NAME = medicineType.ACTIVE_INGR_BHYT_NAME;
            this.ALERT_EXPIRED_DATE = medicineType.ALERT_EXPIRED_DATE;
            this.ALERT_MAX_IN_TREATMENT = medicineType.ALERT_MAX_IN_TREATMENT;
            this.ALERT_MIN_IN_STOCK = medicineType.ALERT_MIN_IN_STOCK;
            this.APP_CREATOR = medicineType.APP_CREATOR;
            this.APP_MODIFIER = medicineType.APP_MODIFIER;
            this.BILL_OPTION = medicineType.BILL_OPTION;
            this.BYT_NUM_ORDER = medicineType.BYT_NUM_ORDER;
            this.CONCENTRA = medicineType.CONCENTRA;
            this.CREATE_TIME = medicineType.CREATE_TIME;
            this.CREATOR = medicineType.CREATOR;
            this.DESCRIPTION = medicineType.DESCRIPTION;
            this.GROUP_CODE = medicineType.GROUP_CODE;
            this.HEIN_ORDER = medicineType.HEIN_ORDER;
            this.HEIN_SERVICE_BHYT_CODE = medicineType.HEIN_SERVICE_BHYT_CODE;
            this.HEIN_SERVICE_BHYT_NAME = medicineType.HEIN_SERVICE_BHYT_NAME;
            this.HEIN_SERVICE_TYPE_CODE = medicineType.HEIN_SERVICE_TYPE_CODE;
            this.HEIN_SERVICE_TYPE_ID = medicineType.HEIN_SERVICE_TYPE_ID;
            this.HEIN_SERVICE_TYPE_NAME = medicineType.HEIN_SERVICE_TYPE_NAME;
            this.ID = medicineType.ID;
            this.IMP_PRICE = medicineType.IMP_PRICE;
            this.IMP_VAT_RATIO = medicineType.IMP_VAT_RATIO;
            this.INTERNAL_PRICE = medicineType.INTERNAL_PRICE;
            this.IS_ACTIVE = medicineType.IS_ACTIVE;
            this.IS_ALLOW_ODD = medicineType.IS_ALLOW_ODD;
            this.IS_DELETE = medicineType.IS_DELETE;
            this.IS_FUNCTIONAL_FOOD = medicineType.IS_FUNCTIONAL_FOOD;
            this.IS_LEAF = medicineType.IS_LEAF;
            this.IS_OUT_PARENT_FEE = medicineType.IS_OUT_PARENT_FEE;
            this.IS_REQUIRE_HSD = medicineType.IS_REQUIRE_HSD;
            this.IS_STAR_MARK = medicineType.IS_STAR_MARK;
            this.IS_STOP_IMP = medicineType.IS_STOP_IMP;
            this.IS_ALLOW_EXPORT_ODD = medicineType.IS_ALLOW_EXPORT_ODD;
            this.MEDICINE_GROUP_ID = medicineType.MEDICINE_GROUP_ID;
            this.MANUFACTURER_CODE = medicineType.MANUFACTURER_CODE;
            this.MANUFACTURER_ID = medicineType.MANUFACTURER_ID;
            this.MANUFACTURER_NAME = medicineType.MANUFACTURER_NAME;
            this.MEDICINE_LINE_CODE = medicineType.MEDICINE_LINE_CODE;
            this.MEDICINE_LINE_ID = medicineType.MEDICINE_LINE_ID;
            this.MEDICINE_LINE_NAME = medicineType.MEDICINE_LINE_NAME;
            this.MEDICINE_TYPE_CODE = medicineType.MEDICINE_TYPE_CODE;
            this.MEDICINE_TYPE_NAME = medicineType.MEDICINE_TYPE_NAME;
            this.MEDICINE_TYPE_PROPRIETARY_NAME = medicineType.MEDICINE_TYPE_PROPRIETARY_NAME;
            this.MEDICINE_USE_FORM_CODE = medicineType.MEDICINE_USE_FORM_CODE;
            this.MEDICINE_USE_FORM_ID = medicineType.MEDICINE_USE_FORM_ID;
            this.MEDICINE_USE_FORM_NAME = medicineType.MEDICINE_USE_FORM_NAME;
            this.MEMA_GROUP_ID = medicineType.MEMA_GROUP_ID;
            this.MODIFIER = medicineType.MODIFIER;
            this.MODIFY_TIME = medicineType.MODIFY_TIME;
            this.NATIONAL_NAME = medicineType.NATIONAL_NAME;
            this.NUM_ORDER = medicineType.NUM_ORDER;
            this.PACKING_TYPE_NAME = medicineType.PACKING_TYPE_NAME;
            this.PARENT_ID = medicineType.PARENT_ID;
            this.REGISTER_NUMBER = medicineType.REGISTER_NUMBER;
            this.SERVICE_ID = medicineType.SERVICE_ID;
            this.SERVICE_TYPE_ID = medicineType.SERVICE_TYPE_ID;
            this.SERVICE_UNIT_CODE = medicineType.SERVICE_UNIT_CODE;
            this.SERVICE_UNIT_ID = medicineType.SERVICE_UNIT_ID;
            this.SERVICE_UNIT_NAME = medicineType.SERVICE_UNIT_NAME;
            this.TCY_NUM_ORDER = medicineType.TCY_NUM_ORDER;
            this.TUTORIAL = medicineType.TUTORIAL;
            this.USE_ON_DAY = medicineType.USE_ON_DAY;
            this.HEIN_LIMIT_PRICE_OLD = medicineType.HEIN_LIMIT_PRICE_OLD;
            this.HEIN_LIMIT_PRICE = medicineType.HEIN_LIMIT_PRICE;
            this.HEIN_LIMIT_RATIO_OLD = medicineType.HEIN_LIMIT_RATIO_OLD;
            this.HEIN_LIMIT_RATIO = medicineType.HEIN_LIMIT_RATIO;
            this.HEIN_LIMIT_PRICE_IN_TIME = medicineType.HEIN_LIMIT_PRICE_IN_TIME;
            this.HEIN_LIMIT_PRICE_INTR_TIME = medicineType.HEIN_LIMIT_PRICE_INTR_TIME;
            
            this.SUPPLIER_ID = medicineType.SUPPLIER_ID;
            this.SUPPLIER_CODE = medicineType.SUPPLIER_CODE;
            this.SUPPLIER_NAME = medicineType.SUPPLIER_NAME;
            this.MEDICINE_GROUP_ID = medicineType.MEDICINE_GROUP_ID;
            this.Type = medicineType.Type;
          

        }

    }
}
