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

namespace HIS.Desktop.Plugins.HistoryMedicine
{
    class MedicineADO : V_HIS_MEDICINE_TYPE
    {
        public string MEDICINE_TYPE_NAME_UNSIGN { get; set; }

        public MedicineADO(V_HIS_MEDICINE_TYPE data)
        {
            if (data != null)
            {
                //Inventec.Common.Mapper.DataObjectMapper.Map<MedicineADO>(this, data);

                this.ACTIVE_INGR_BHYT_CODE = data.ACTIVE_INGR_BHYT_CODE;
                this.ACTIVE_INGR_BHYT_NAME = data.ACTIVE_INGR_BHYT_NAME;
                this.ALERT_EXPIRED_DATE = data.ALERT_EXPIRED_DATE;
                this.ALERT_MAX_IN_PRESCRIPTION = data.ALERT_MAX_IN_PRESCRIPTION;
                this.ALERT_MAX_IN_TREATMENT = data.ALERT_MAX_IN_TREATMENT;
                this.ALERT_MIN_IN_STOCK = data.ALERT_MIN_IN_STOCK;
                this.APP_CREATOR = data.APP_CREATOR;
                this.APP_MODIFIER = data.APP_MODIFIER;
                this.BILL_OPTION = data.BILL_OPTION;
                this.BYT_NUM_ORDER = data.BYT_NUM_ORDER;
                this.CONCENTRA = data.CONCENTRA;
                this.CONTRAINDICATION = data.CONTRAINDICATION;
                this.CONVERT_RATIO = data.CONVERT_RATIO;
                this.CONVERT_UNIT_CODE = data.CONVERT_UNIT_CODE;
                this.CONVERT_UNIT_NAME = data.CONVERT_UNIT_NAME;
                this.CREATE_TIME = data.CREATE_TIME;
                this.CREATOR = data.CREATOR;
                this.DESCRIPTION = data.DESCRIPTION;
                this.GROUP_CODE = data.GROUP_CODE;
                this.HEIN_LIMIT_PRICE = data.HEIN_LIMIT_PRICE;
                this.HEIN_LIMIT_PRICE_IN_TIME = data.HEIN_LIMIT_PRICE_IN_TIME;
                this.HEIN_LIMIT_PRICE_INTR_TIME = data.HEIN_LIMIT_PRICE_INTR_TIME;
                this.HEIN_LIMIT_PRICE_OLD = data.HEIN_LIMIT_PRICE_OLD;
                this.HEIN_LIMIT_RATIO = data.HEIN_LIMIT_RATIO;
                this.HEIN_LIMIT_RATIO_OLD = data.HEIN_LIMIT_RATIO_OLD;
                this.HEIN_ORDER = data.HEIN_ORDER;
                this.HEIN_SERVICE_BHYT_CODE = data.HEIN_SERVICE_BHYT_CODE;
                this.HEIN_SERVICE_BHYT_NAME = data.HEIN_SERVICE_BHYT_NAME;
                this.HEIN_SERVICE_TYPE_CODE = data.HEIN_SERVICE_TYPE_CODE;
                this.HEIN_SERVICE_TYPE_ID = data.HEIN_SERVICE_TYPE_ID;
                this.HEIN_SERVICE_TYPE_NAME = data.HEIN_SERVICE_TYPE_NAME;
                this.ID = data.ID;
                this.IMP_PRICE = data.IMP_PRICE;
                this.IMP_VAT_RATIO = data.IMP_VAT_RATIO;
                this.INTERNAL_PRICE = data.INTERNAL_PRICE;
                this.IS_ACTIVE = data.IS_ACTIVE;
                this.IS_ALLOW_EXPORT_ODD = data.IS_ALLOW_EXPORT_ODD;
                this.IS_ALLOW_ODD = data.IS_ALLOW_ODD;
                this.IS_AUTO_EXPEND = data.IS_AUTO_EXPEND;
                this.IS_BUSINESS = data.IS_BUSINESS;
                this.IS_CHEMICAL_SUBSTANCE = data.IS_CHEMICAL_SUBSTANCE;
                this.IS_DELETE = data.IS_DELETE;
                this.IS_FUNCTIONAL_FOOD = data.IS_FUNCTIONAL_FOOD;
                this.IS_KIDNEY = data.IS_KIDNEY;
                this.IS_LEAF = data.IS_LEAF;
                this.IS_MUST_PREPARE = data.IS_MUST_PREPARE;
                this.IS_NO_HEIN_LIMIT_FOR_SPECIAL = data.IS_NO_HEIN_LIMIT_FOR_SPECIAL;
                this.IS_OTHER_SOURCE_PAID = data.IS_OTHER_SOURCE_PAID;
                this.IS_OUT_PARENT_FEE = data.IS_OUT_PARENT_FEE;
                this.IS_RAW_MEDICINE = data.IS_RAW_MEDICINE;
                this.IS_REQUIRE_HSD = data.IS_REQUIRE_HSD;
                this.IS_SALE_EQUAL_IMP_PRICE = data.IS_SALE_EQUAL_IMP_PRICE;
                this.IS_STAR_MARK = data.IS_STAR_MARK;
                this.IS_STOP_IMP = data.IS_STOP_IMP;
                this.IS_TCMR = data.IS_TCMR;
                this.IS_VACCINE = data.IS_VACCINE;
                this.IS_VITAMIN_A = data.IS_VITAMIN_A;
                this.LAST_EXP_PRICE = data.LAST_EXP_PRICE;
                this.LAST_EXP_VAT_RATIO = data.LAST_EXP_VAT_RATIO;
                this.LAST_IMP_PRICE = data.LAST_IMP_PRICE;
                this.LAST_IMP_VAT_RATIO = data.LAST_IMP_VAT_RATIO;
                this.MANUFACTURER_CODE = data.MANUFACTURER_CODE;
                this.MANUFACTURER_ID = data.MANUFACTURER_ID;
                this.MANUFACTURER_NAME = data.MANUFACTURER_NAME;
                this.MEDICINE_GROUP_CODE = data.MEDICINE_GROUP_CODE;
                this.MEDICINE_GROUP_ID = data.MEDICINE_GROUP_ID;
                this.MEDICINE_GROUP_NAME = data.MEDICINE_GROUP_NAME;
                this.MEDICINE_LINE_CODE = data.MEDICINE_LINE_CODE;
                this.MEDICINE_LINE_ID = data.MEDICINE_LINE_ID;
                this.MEDICINE_LINE_NAME = data.MEDICINE_LINE_NAME;
                this.MEDICINE_LINE_NUM_ORDER = data.MEDICINE_LINE_NUM_ORDER;
                this.MEDICINE_NATIONAL_CODE = data.MEDICINE_NATIONAL_CODE;
                this.MEDICINE_TYPE_CODE = data.MEDICINE_TYPE_CODE;
                this.MEDICINE_TYPE_NAME = data.MEDICINE_TYPE_NAME;
                this.MEDICINE_TYPE_PROPRIETARY_NAME = data.MEDICINE_TYPE_PROPRIETARY_NAME;
                this.MEDICINE_USE_FORM_CODE = data.MEDICINE_USE_FORM_CODE;
                this.MEDICINE_USE_FORM_ID = data.MEDICINE_USE_FORM_ID;
                this.MEDICINE_USE_FORM_NAME = data.MEDICINE_USE_FORM_NAME;
                this.MEMA_GROUP_ID = data.MEMA_GROUP_ID;
                this.MODIFIER = data.MODIFIER;
                this.MODIFY_TIME = data.MODIFY_TIME;
                this.NATIONAL_NAME = data.NATIONAL_NAME;
                this.NUM_ORDER = data.NUM_ORDER;
                this.PACKING_TYPE_ID__DELETE = data.PACKING_TYPE_ID__DELETE;
                this.PACKING_TYPE_NAME = data.PACKING_TYPE_NAME;
                this.PARENT_ID = data.PARENT_ID;
                this.RANK = data.RANK;
                this.REGISTER_NUMBER = data.REGISTER_NUMBER;
                this.SERVICE_ID = data.SERVICE_ID;
                this.SERVICE_TYPE_ID = data.SERVICE_TYPE_ID;
                this.SERVICE_UNIT_CODE = data.SERVICE_UNIT_CODE;
                this.SERVICE_UNIT_ID = data.SERVICE_UNIT_ID;
                this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                this.TCY_NUM_ORDER = data.TCY_NUM_ORDER;
                this.TDL_GENDER_ID = data.TDL_GENDER_ID;
                this.TDL_SERVICE_UNIT_ID = data.TDL_SERVICE_UNIT_ID;
                this.TUTORIAL = data.TUTORIAL;
                this.USE_ON_DAY = data.USE_ON_DAY;


                this.MEDICINE_TYPE_NAME_UNSIGN = Inventec.Common.String.Convert.UnSignVNese(data.MEDICINE_TYPE_NAME);
            }
        }
    }
}
