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

namespace HIS.Desktop.Plugins.ApprovalPrescriptionPK.ADO
{
    public class PrescriptionADO : V_HIS_EXP_MEST_MEDICINE
    {
        public long type { get; set; } // 1:thuốc; 2: Vật tư
        public decimal TOTAL_PRICE { get; set; }
        public string DETAIL_CODE { get; set; }
        public string DETAIL_NAME { get; set; }

        public long? MATERIAL_TYPE_ID { get; set; }

        public bool IsCheck { get; set; }

        public long? SERVICE_REQ_ID { get; set; }
        public string REQUEST_LOGINNAME { get; set; }
        public string REQUEST_USERNAME { get; set; }
        public string REQUEST_ROOM_NAME { get; set; }
        public string SERVICE_REQ_CODE { get; set; }
        public long? INTRUCTION_TIME { get; set; }
        public string INTRUCTION_TIME_STR { get; set; }

        public PrescriptionADO(V_HIS_EXP_MEST_MATERIAL material)
        {
            try
            {
                if (material != null)
                {
                    this.AGGR_EXP_MEST_ID = material.AGGR_EXP_MEST_ID;
                    this.AMOUNT = material.AMOUNT;
                    this.APP_CREATOR = material.APP_CREATOR;
                    this.APP_MODIFIER = material.APP_MODIFIER;
                    this.APPROVAL_DATE = material.APPROVAL_DATE;
                    this.APPROVAL_LOGINNAME = material.APPROVAL_LOGINNAME;
                    this.APPROVAL_TIME = material.APPROVAL_TIME;
                    this.APPROVAL_USERNAME = material.APPROVAL_USERNAME;
                    this.BID_ID = material.BID_ID;
                    this.BID_NAME = material.BID_NAME;
                    this.BID_NUMBER = material.BID_NUMBER;
                    this.BK_AMOUNT = material.BK_AMOUNT;
                    this.CK_IMP_MEST_MEDICINE_ID = material.CK_IMP_MEST_MATERIAL_ID;
                    this.CREATE_TIME = material.CREATE_TIME;
                    this.CREATOR = material.CREATOR;
                    this.DESCRIPTION = material.DESCRIPTION;
                    this.DISCOUNT = material.DISCOUNT;
                    this.EXP_DATE = material.EXP_DATE;
                    this.EXP_LOGINNAME = material.EXP_LOGINNAME;
                    this.EXP_MEST_CODE = material.EXP_MEST_CODE;
                    this.EXP_MEST_ID = material.EXP_MEST_ID;
                    this.EXP_MEST_METY_REQ_ID = material.EXP_MEST_MATY_REQ_ID;
                    this.EXP_MEST_STT_ID = material.EXP_MEST_STT_ID;
                    this.EXP_MEST_TYPE_ID = material.EXP_MEST_TYPE_ID;
                    this.EXP_TIME = material.EXP_TIME;
                    this.EXP_USERNAME = material.EXP_USERNAME;
                    this.EXPIRED_DATE = material.EXPIRED_DATE;
                    this.GROUP_CODE = material.GROUP_CODE;
                    this.ID = material.ID;
                    this.IMP_PRICE = material.IMP_PRICE;
                    this.IMP_TIME = material.IMP_TIME;
                    this.IMP_VAT_RATIO = material.IMP_VAT_RATIO;
                    this.INTERNAL_PRICE = material.INTERNAL_PRICE;
                    this.IS_ACTIVE = material.IS_ACTIVE;
                    this.IS_DELETE = material.IS_DELETE;
                    this.IS_EXPEND = material.IS_EXPEND;
                    this.IS_EXPORT = material.IS_EXPORT;
                    this.IS_OUT_PARENT_FEE = material.IS_OUT_PARENT_FEE;
                    this.IS_USE_CLIENT_PRICE = material.IS_USE_CLIENT_PRICE;
                    this.MANUFACTURER_CODE = material.MANUFACTURER_CODE;
                    this.MANUFACTURER_ID = material.MANUFACTURER_ID;
                    this.MANUFACTURER_NAME = material.MANUFACTURER_NAME;
                    this.MATERIAL_NUM_ORDER = material.MATERIAL_NUM_ORDER;
                    this.MEDI_STOCK_ID = material.MEDI_STOCK_ID;
                    this.MEDI_STOCK_PERIOD_ID = material.MEDI_STOCK_PERIOD_ID;
                    this.MEDICINE_NUM_ORDER = material.MEDICINE_NUM_ORDER;
                    this.MEMA_GROUP_ID = material.MEMA_GROUP_ID;
                    this.MODIFIER = material.MODIFIER;
                    this.MODIFY_TIME = material.MODIFY_TIME;
                    this.NATIONAL_NAME = material.NATIONAL_NAME;
                    this.NUM_ORDER = material.NUM_ORDER;
                    this.PACKAGE_NUMBER = material.PACKAGE_NUMBER;
                    this.PATIENT_TYPE_CODE = material.PATIENT_TYPE_CODE;
                    this.PATIENT_TYPE_ID = material.PATIENT_TYPE_ID;
                    this.PATIENT_TYPE_NAME = material.PATIENT_TYPE_NAME;
                    this.PRICE = material.PRICE;
                    this.REQ_DEPARTMENT_ID = material.REQ_DEPARTMENT_ID;
                    this.REQ_ROOM_ID = material.REQ_ROOM_ID;
                    this.SERE_SERV_PARENT_ID = material.SERE_SERV_PARENT_ID;
                    this.SERVICE_ID = material.SERVICE_ID;
                    this.SERVICE_UNIT_CODE = material.SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_ID = material.SERVICE_UNIT_ID;
                    this.SERVICE_UNIT_NAME = material.SERVICE_UNIT_NAME;
                    this.SUPPLIER_CODE = material.SUPPLIER_CODE;
                    this.SUPPLIER_ID = material.SUPPLIER_ID;
                    this.SUPPLIER_NAME = material.SUPPLIER_NAME;
                    this.TDL_AGGR_EXP_MEST_ID = material.TDL_AGGR_EXP_MEST_ID;
                    this.TDL_MEDI_STOCK_ID = material.TDL_MEDI_STOCK_ID;
                    this.TDL_SERVICE_REQ_ID = material.TDL_SERVICE_REQ_ID;
                    this.TDL_TREATMENT_ID = material.TDL_TREATMENT_ID;
                    this.TH_AMOUNT = material.TH_AMOUNT;
                    this.VAT_RATIO = material.VAT_RATIO;
                    this.VIR_PRICE = material.VIR_PRICE;
                    this.MATERIAL_TYPE_ID = material.TDL_MATERIAL_TYPE_ID;

                    this.TUTORIAL = material.TUTORIAL;

                    //this.IsCheck = material.IS_EXPORT != 1;
                    this.IsCheck = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public PrescriptionADO(V_HIS_EXP_MEST_MEDICINE medicine)
        {
            try
            {
                if (medicine != null)
                {
                    this.ACTIVE_INGR_BHYT_CODE = medicine.ACTIVE_INGR_BHYT_CODE;
                    this.ACTIVE_INGR_BHYT_NAME = medicine.ACTIVE_INGR_BHYT_NAME;
                    this.AGGR_EXP_MEST_ID = medicine.AGGR_EXP_MEST_ID;
                    this.AMOUNT = medicine.AMOUNT;
                    this.APP_CREATOR = medicine.APP_CREATOR;
                    this.APP_MODIFIER = medicine.APP_MODIFIER;
                    this.APPROVAL_DATE = medicine.APPROVAL_DATE;
                    this.APPROVAL_LOGINNAME = medicine.APPROVAL_LOGINNAME;
                    this.APPROVAL_TIME = medicine.APPROVAL_TIME;
                    this.APPROVAL_USERNAME = medicine.APPROVAL_USERNAME;
                    this.BID_ID = medicine.BID_ID;
                    this.BID_NAME = medicine.BID_NAME;
                    this.BID_NUMBER = medicine.BID_NUMBER;
                    this.BK_AMOUNT = medicine.BK_AMOUNT;
                    this.BYT_NUM_ORDER = medicine.BYT_NUM_ORDER;
                    this.CK_IMP_MEST_MEDICINE_ID = medicine.CK_IMP_MEST_MEDICINE_ID;
                    this.CONCENTRA = medicine.CONCENTRA;
                    this.CREATE_TIME = medicine.CREATE_TIME;
                    this.CREATOR = medicine.CREATOR;
                    this.DESCRIPTION = medicine.DESCRIPTION;
                    this.DISCOUNT = medicine.DISCOUNT;
                    this.EXP_DATE = medicine.EXP_DATE;
                    this.EXP_LOGINNAME = medicine.EXP_LOGINNAME;
                    this.EXP_MEST_CODE = medicine.EXP_MEST_CODE;
                    this.EXP_MEST_ID = medicine.EXP_MEST_ID;
                    this.EXP_MEST_METY_REQ_ID = medicine.EXP_MEST_METY_REQ_ID;
                    this.EXP_MEST_STT_ID = medicine.EXP_MEST_STT_ID;
                    this.EXP_MEST_TYPE_ID = medicine.EXP_MEST_TYPE_ID;
                    this.EXP_TIME = medicine.EXP_TIME;
                    this.EXP_USERNAME = medicine.EXP_USERNAME;
                    this.EXPIRED_DATE = medicine.EXPIRED_DATE;
                    this.GROUP_CODE = medicine.GROUP_CODE;
                    this.ID = medicine.ID;
                    this.IMP_PRICE = medicine.IMP_PRICE;
                    this.IMP_TIME = medicine.IMP_TIME;
                    this.IMP_VAT_RATIO = medicine.IMP_VAT_RATIO;
                    this.INTERNAL_PRICE = medicine.INTERNAL_PRICE;
                    this.IS_ACTIVE = medicine.IS_ACTIVE;
                    this.IS_ALLOW_ODD = medicine.IS_ALLOW_ODD;
                    this.IS_DELETE = medicine.IS_DELETE;
                    this.IS_EXPEND = medicine.IS_EXPEND;
                    this.IS_EXPORT = medicine.IS_EXPORT;
                    this.IS_FUNCTIONAL_FOOD = medicine.IS_FUNCTIONAL_FOOD;
                    this.IS_OUT_PARENT_FEE = medicine.IS_OUT_PARENT_FEE;
                    this.IS_USE_CLIENT_PRICE = medicine.IS_USE_CLIENT_PRICE;
                    this.MANUFACTURER_CODE = medicine.MANUFACTURER_CODE;
                    this.MANUFACTURER_ID = medicine.MANUFACTURER_ID;
                    this.MANUFACTURER_NAME = medicine.MANUFACTURER_NAME;
                    this.MATERIAL_NUM_ORDER = medicine.MATERIAL_NUM_ORDER;
                    this.MEDI_STOCK_ID = medicine.MEDI_STOCK_ID;
                    this.MEDI_STOCK_PERIOD_ID = medicine.MEDI_STOCK_PERIOD_ID;
                    this.MEDICINE_BYT_NUM_ORDER = medicine.MEDICINE_BYT_NUM_ORDER;
                    this.MEDICINE_GROUP_ID = medicine.MEDICINE_GROUP_ID;
                    this.MEDICINE_ID = medicine.MEDICINE_ID;
                    this.MEDICINE_NUM_ORDER = medicine.MEDICINE_NUM_ORDER;
                    this.MEDICINE_REGISTER_NUMBER = medicine.MEDICINE_REGISTER_NUMBER;
                    this.MEDICINE_TCY_NUM_ORDER = medicine.MEDICINE_TCY_NUM_ORDER;
                    this.MEDICINE_TYPE_CODE = medicine.MEDICINE_TYPE_CODE;
                    this.MEDICINE_TYPE_ID = medicine.MEDICINE_TYPE_ID;
                    this.MEDICINE_TYPE_NAME = medicine.MEDICINE_TYPE_NAME;
                    this.MEDICINE_TYPE_NUM_ORDER = medicine.MEDICINE_TYPE_NUM_ORDER;
                    this.MEMA_GROUP_ID = medicine.MEMA_GROUP_ID;
                    this.MODIFIER = medicine.MODIFIER;
                    this.MODIFY_TIME = medicine.MODIFY_TIME;
                    this.NATIONAL_NAME = medicine.NATIONAL_NAME;
                    this.NUM_ORDER = medicine.NUM_ORDER;
                    this.PACKAGE_NUMBER = medicine.PACKAGE_NUMBER;
                    this.PATIENT_TYPE_CODE = medicine.PATIENT_TYPE_CODE;
                    this.PATIENT_TYPE_ID = medicine.PATIENT_TYPE_ID;
                    this.PATIENT_TYPE_NAME = medicine.PATIENT_TYPE_NAME;
                    this.PRICE = medicine.PRICE;
                    this.REGISTER_NUMBER = medicine.REGISTER_NUMBER;
                    this.REQ_DEPARTMENT_ID = medicine.REQ_DEPARTMENT_ID;
                    this.REQ_ROOM_ID = medicine.REQ_ROOM_ID;
                    this.SERE_SERV_PARENT_ID = medicine.SERE_SERV_PARENT_ID;
                    this.SERVICE_ID = medicine.SERVICE_ID;
                    this.SERVICE_UNIT_CODE = medicine.SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_ID = medicine.SERVICE_UNIT_ID;
                    this.SERVICE_UNIT_NAME = medicine.SERVICE_UNIT_NAME;
                    this.SUPPLIER_CODE = medicine.SUPPLIER_CODE;
                    this.SUPPLIER_ID = medicine.SUPPLIER_ID;
                    this.SUPPLIER_NAME = medicine.SUPPLIER_NAME;
                    this.TCY_NUM_ORDER = medicine.TCY_NUM_ORDER;
                    this.TDL_AGGR_EXP_MEST_ID = medicine.TDL_AGGR_EXP_MEST_ID;
                    this.TDL_MEDI_STOCK_ID = medicine.TDL_MEDI_STOCK_ID;
                    this.TDL_MEDICINE_TYPE_ID = medicine.TDL_MEDICINE_TYPE_ID;
                    this.TDL_SERVICE_REQ_ID = medicine.TDL_SERVICE_REQ_ID;
                    this.TDL_TREATMENT_ID = medicine.TDL_TREATMENT_ID;
                    this.TDL_VACCINATION_ID = medicine.TDL_VACCINATION_ID;
                    this.TUTORIAL = medicine.TUTORIAL;
                    this.TH_AMOUNT = medicine.TH_AMOUNT;
                    this.USE_TIME_TO = medicine.USE_TIME_TO;
                    this.VACCINATION_RESULT_ID = medicine.VACCINATION_RESULT_ID;
                    this.VAT_RATIO = medicine.VAT_RATIO;
                    this.VIR_PRICE = medicine.VIR_PRICE;

                    //this.IsCheck = medicine.IS_EXPORT != 1;
                    this.IsCheck = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
