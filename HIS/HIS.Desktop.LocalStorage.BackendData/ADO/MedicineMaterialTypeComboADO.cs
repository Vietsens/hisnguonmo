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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.BackendData.ADO
{
    public class MedicineMaterialTypeComboADO : MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE
    {
        public const int THUOC = 1;
        public const int VATTU = 2;
        public const int THUOC_DM = 3;
        public const int VATTU_DM = 4;
        public const int THUOC_TUTUC = 5;
        public const int VATTU_TSD = 6;

        public MedicineMaterialTypeComboADO()
        {

        }
        public MedicineMaterialTypeComboADO(MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE data)
        {
            try
            {
                //Inventec.Common.Mapper.DataObjectMapper.Map<MedicineMaterialTypeComboADO>(this, data);//Hiệu năng không tốt -> bỏ thay bằng gán trực tiếp

                this.ACTIVE_INGR_BHYT_CODE = data.ACTIVE_INGR_BHYT_CODE;
                this.ACTIVE_INGR_BHYT_NAME = data.ACTIVE_INGR_BHYT_NAME;
                this.ALERT_EXPIRED_DATE = data.ALERT_EXPIRED_DATE;
                this.ALERT_MAX_IN_TREATMENT = data.ALERT_MAX_IN_TREATMENT;
                this.ALERT_MIN_IN_STOCK = data.ALERT_MIN_IN_STOCK;
                this.APP_CREATOR = data.APP_CREATOR;
                this.APP_MODIFIER = data.APP_MODIFIER;
                this.BILL_OPTION = data.BILL_OPTION;
                this.BYT_NUM_ORDER = data.BYT_NUM_ORDER;
                this.CONCENTRA = data.CONCENTRA;
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
                this.IS_DELETE = data.IS_DELETE;
                this.IS_FUNCTIONAL_FOOD = data.IS_FUNCTIONAL_FOOD;
                this.IS_LEAF = data.IS_LEAF;
                this.IS_OUT_PARENT_FEE = data.IS_OUT_PARENT_FEE;
                this.IS_RAW_MEDICINE = data.IS_RAW_MEDICINE;
                this.IS_REQUIRE_HSD = data.IS_REQUIRE_HSD;
                this.IS_SALE_EQUAL_IMP_PRICE = data.IS_SALE_EQUAL_IMP_PRICE;
                this.IS_STAR_MARK = data.IS_STAR_MARK;
                this.IS_STOP_IMP = data.IS_STOP_IMP;
                this.MANUFACTURER_CODE = data.MANUFACTURER_CODE;
                this.MANUFACTURER_ID = data.MANUFACTURER_ID;
                this.MANUFACTURER_NAME = data.MANUFACTURER_NAME;
                this.MEDICINE_GROUP_CODE = data.MEDICINE_GROUP_CODE;
                this.MEDICINE_GROUP_ID = data.MEDICINE_GROUP_ID;
                this.MEDICINE_GROUP_NAME = data.MEDICINE_GROUP_NAME;
                this.MEDICINE_LINE_CODE = data.MEDICINE_LINE_CODE;
                this.MEDICINE_LINE_ID = data.MEDICINE_LINE_ID;
                this.MEDICINE_LINE_NAME = data.MEDICINE_LINE_NAME;
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
                this.REGISTER_NUMBER = data.REGISTER_NUMBER;
                this.SERVICE_ID = data.SERVICE_ID;
                this.SERVICE_TYPE_ID = data.SERVICE_TYPE_ID;
                this.SERVICE_UNIT_CODE = data.SERVICE_UNIT_CODE;
                this.SERVICE_UNIT_ID = data.SERVICE_UNIT_ID;
                this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                this.TCY_NUM_ORDER = data.TCY_NUM_ORDER;
                this.TDL_SERVICE_UNIT_ID = data.TDL_SERVICE_UNIT_ID;
                this.TUTORIAL = data.TUTORIAL;
                this.USE_ON_DAY = data.USE_ON_DAY;
                this.ALERT_MAX_IN_PRESCRIPTION = data.ALERT_MAX_IN_PRESCRIPTION;
                this.IS_BLOCK_MAX_IN_PRESCRIPTION = data.IS_BLOCK_MAX_IN_PRESCRIPTION;
                this.ALERT_MAX_IN_DAY = data.ALERT_MAX_IN_DAY;
                this.IS_BLOCK_MAX_IN_DAY = data.IS_BLOCK_MAX_IN_DAY;
                this.TDL_GENDER_ID = data.TDL_GENDER_ID;

                this.DataType = THUOC_DM;
                this.IS_MUST_PREPARE = data.IS_MUST_PREPARE;
                this.MEDICINE_TYPE_CODE__UNSIGN = convertToUnSign3(data.MEDICINE_TYPE_CODE) + data.MEDICINE_TYPE_CODE;
                this.MEDICINE_TYPE_NAME__UNSIGN = convertToUnSign3(data.MEDICINE_TYPE_NAME) + data.MEDICINE_TYPE_NAME;
                this.ACTIVE_INGR_BHYT_NAME__UNSIGN = convertToUnSign3(data.ACTIVE_INGR_BHYT_NAME) + data.ACTIVE_INGR_BHYT_NAME;
                this.CONVERT_RATIO = data.CONVERT_RATIO;
                this.CONVERT_UNIT_CODE = data.CONVERT_UNIT_CODE;
                this.CONVERT_UNIT_NAME = data.CONVERT_UNIT_NAME;
                this.CONTRAINDICATION = data.CONTRAINDICATION;
                this.HTU_ID = data.HTU_ID;
                this.ODD_WARNING_CONTENT = data.ODD_WARNING_CONTENT;
                this.ATC_CODES = data.ATC_CODES;
                this.ATC_GROUP_CODES = data.ATC_GROUP_CODES;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public MedicineMaterialTypeComboADO(MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE data)
        {
            try
            {
                //Inventec.Common.Mapper.DataObjectMapper.Map<MedicineMaterialTypeComboADO>(this, data);//Hiệu năng không tốt -> bỏ thay bằng gán trực tiếp

                this.ALERT_EXPIRED_DATE = data.ALERT_EXPIRED_DATE;
                this.ALERT_MIN_IN_STOCK = data.ALERT_MIN_IN_STOCK;
                this.APP_CREATOR = data.APP_CREATOR;
                this.APP_MODIFIER = data.APP_MODIFIER;
                this.BILL_OPTION = data.BILL_OPTION;
                this.CONCENTRA = data.CONCENTRA;
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
                this.IS_DELETE = data.IS_DELETE;
                this.IS_LEAF = data.IS_LEAF;
                this.IS_OUT_PARENT_FEE = data.IS_OUT_PARENT_FEE;
                this.IS_REQUIRE_HSD = data.IS_REQUIRE_HSD;
                this.IS_SALE_EQUAL_IMP_PRICE = data.IS_SALE_EQUAL_IMP_PRICE;
                this.IS_STOP_IMP = data.IS_STOP_IMP;
                this.MANUFACTURER_CODE = data.MANUFACTURER_CODE;
                this.MANUFACTURER_ID = data.MANUFACTURER_ID;
                this.MANUFACTURER_NAME = data.MANUFACTURER_NAME;
                this.MEMA_GROUP_ID = data.MEMA_GROUP_ID;
                this.MODIFIER = data.MODIFIER;
                this.MODIFY_TIME = data.MODIFY_TIME;
                this.NATIONAL_NAME = data.NATIONAL_NAME;
                this.NUM_ORDER = data.NUM_ORDER;
                this.PACKING_TYPE_ID__DELETE = data.PACKING_TYPE_ID__DELETE;
                this.PACKING_TYPE_NAME = data.PACKING_TYPE_NAME;
                this.PARENT_ID = data.PARENT_ID;
                this.SERVICE_ID = data.SERVICE_ID;
                this.SERVICE_TYPE_ID = data.SERVICE_TYPE_ID;
                this.SERVICE_UNIT_CODE = data.SERVICE_UNIT_CODE;
                this.SERVICE_UNIT_ID = data.SERVICE_UNIT_ID;
                this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                this.TDL_SERVICE_UNIT_ID = data.TDL_SERVICE_UNIT_ID;
                this.ALERT_MAX_IN_PRESCRIPTION = data.ALERT_MAX_IN_PRESCRIPTION;
                this.ALERT_MAX_IN_DAY = data.ALERT_MAX_IN_DAY;
                this.TDL_GENDER_ID = data.TDL_GENDER_ID;
                this.MEDICINE_TYPE_CODE = data.MATERIAL_TYPE_CODE;
                this.MEDICINE_TYPE_NAME = data.MATERIAL_TYPE_NAME;
                this.IS_CHEMICAL_SUBSTANCE = data.IS_CHEMICAL_SUBSTANCE;
                this.DataType = VATTU_DM;
                this.IS_MUST_PREPARE = data.IS_MUST_PREPARE;
                this.MEDICINE_TYPE_CODE__UNSIGN = convertToUnSign3(data.MATERIAL_TYPE_CODE) + data.MATERIAL_TYPE_CODE;
                this.MEDICINE_TYPE_NAME__UNSIGN = convertToUnSign3(data.MATERIAL_TYPE_NAME) + data.MATERIAL_TYPE_NAME;
                this.CONVERT_RATIO = data.CONVERT_RATIO;
                this.CONVERT_UNIT_CODE = data.CONVERT_UNIT_CODE;
                this.CONVERT_UNIT_NAME = data.CONVERT_UNIT_NAME;                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public MedicineMaterialTypeComboADO(MOS.EFMODEL.DataModels.V_HIS_SERVICE data, MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType, bool isAssignDay)
        {
            try
            {
                //Inventec.Common.Mapper.DataObjectMapper.Map<MedicineMaterialTypeComboADO>(this, data);

                this.ACTIVE_INGR_BHYT_CODE = data.ACTIVE_INGR_BHYT_CODE;
                this.ACTIVE_INGR_BHYT_NAME = data.ACTIVE_INGR_BHYT_NAME;
                this.APP_CREATOR = data.APP_CREATOR;
                this.APP_MODIFIER = data.APP_MODIFIER;
                this.BILL_OPTION = data.BILL_OPTION;
                this.CONCENTRA = data.CONCENTRA;
                this.CREATE_TIME = data.CREATE_TIME;
                this.CREATOR = data.CREATOR;
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
                this.IS_ACTIVE = data.IS_ACTIVE;
                this.IS_DELETE = data.IS_DELETE;
                this.IS_LEAF = data.IS_LEAF;
                this.IS_OUT_PARENT_FEE = data.IS_OUT_PARENT_FEE;
                this.MODIFIER = data.MODIFIER;
                this.MODIFY_TIME = data.MODIFY_TIME;
                this.NUM_ORDER = data.NUM_ORDER;
                this.PARENT_ID = data.PARENT_ID;
                this.SERVICE_TYPE_ID = data.SERVICE_TYPE_ID;
                this.SERVICE_UNIT_CODE = data.SERVICE_UNIT_CODE;
                this.SERVICE_UNIT_ID = data.SERVICE_UNIT_ID;
                this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                
                this.AMOUNT = 1;
                this.IsExpend = false;
                this.IsKHBHYT = false;
                this.SERVICE_ID = data.ID;
                this.SERVICE_TYPE_ID = data.SERVICE_TYPE_ID;
                if (patientType != null)
                {
                    this.PATIENT_TYPE_ID = patientType.ID;
                    this.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                    this.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                }

                this.IsAssignDay = isAssignDay;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public string convertToUnSign3(string s)
        {
            string result = null;
            try
            {
                if (!String.IsNullOrEmpty(s))
                {
                    Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
                    string temp = s.Normalize(NormalizationForm.FormD);
                    result = regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        public int DataType { get; set; }
        public short? IS_CHEMICAL_SUBSTANCE { get; set; }
        public long? PATIENT_TYPE_ID { get; set; }
        public string PATIENT_TYPE_CODE { get; set; }
        public string PATIENT_TYPE_NAME { get; set; }
        public decimal? AMOUNT { get; set; }
        public decimal? PRE_AMOUNT { get; set; }
        public bool IsExpend { get; set; }
        public bool IsKHBHYT { get; set; }
        public bool IsOutParentFee { get; set; }
        public bool? IsOutKtcFee { get; set; }
        public bool IsAssignDay { get; set; }
        public decimal? AmountAlert { get; set; }
        public double? Sang { get; set; }
        public double? Trua { get; set; }
        public double? Chieu { get; set; }
        public double? Toi { get; set; }
        public long? HTU_ID { get; set; }
        public long? UseTimeTo { get; set; }
        public decimal? UseDays { get; set; }
        public decimal TotalPrice { get; set; }
        public long? MEDI_STOCK_ID { get; set; }
        public string MEDI_STOCK_CODE { get; set; }
        public string MEDI_STOCK_NAME { get; set; }
        public short? IN_EXECUTE { get; set; }
        public short? IN_REQUEST { get; set; }
        public string MEDICINE_TYPE_CODE__UNSIGN { get; set; }
        public string MEDICINE_TYPE_NAME__UNSIGN { get; set; }
        public string ACTIVE_INGR_BHYT_NAME__UNSIGN { get; set; }
        public bool IsExistAssignPres { get; set; }
        public long IdRow { get; set; }
    }

    
}
