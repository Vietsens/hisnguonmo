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

namespace MPS.Processor.Mps000262
{
    class Mps000262CommonInfoADO : V_HIS_TREATMENT
    {
        public string ADD_LOGINNAME { get; set; }
        public long ADD_TIME { get; set; }
        public string ADD_USERNAME { get; set; }
        public string BED_CODE { get; set; }
        public long? BED_ID { get; set; }
        public string BED_NAME { get; set; }
        public string BED_ROOM_CODE { get; set; }
        public long BED_ROOM_ID { get; set; }
        public string BED_ROOM_NAME { get; set; }
        public long? CO_TREATMENT_ID { get; set; }
        public string REMOVE_LOGINNAME { get; set; }
        public long? REMOVE_TIME { get; set; }
        public string REMOVE_USERNAME { get; set; }

        public long? AGGR_EXP_MEST_ID { get; set; }
        public long? AGGR_USE_TIME { get; set; }
        public long? BILL_ID { get; set; }
        public string CASHIER_LOGINNAME { get; set; }
        public string CASHIER_USERNAME { get; set; }
        public long CREATE_DATE { get; set; }
        public string DESCRIPTION { get; set; }
        public decimal? DISCOUNT { get; set; }
        public long? DISPENSE_ID { get; set; }
        public string EXP_MEST_CODE { get; set; }
        public string EXP_MEST_REASON_CODE { get; set; }
        public long? EXP_MEST_REASON_ID { get; set; }
        public string EXP_MEST_REASON_NAME { get; set; }
        public string EXP_MEST_STT_CODE { get; set; }
        public long EXP_MEST_STT_ID { get; set; }
        public string EXP_MEST_STT_NAME { get; set; }
        public string EXP_MEST_TYPE_CODE { get; set; }
        public long EXP_MEST_TYPE_ID { get; set; }
        public string EXP_MEST_TYPE_NAME { get; set; }
        public long? FINISH_DATE { get; set; }
        public long? FINISH_TIME { get; set; }
        public long? IMP_MEDI_STOCK_ID { get; set; }
        public short? IS_CABINET { get; set; }
        public short? IS_EXPORT_EQUAL_APPROVE { get; set; }
        public short? IS_EXPORT_EQUAL_REQUEST { get; set; }
        public short? IS_HTX { get; set; }
        public short? IS_NOT_TAKEN { get; set; }
        public string LAST_EXP_LOGINNAME { get; set; }
        public long? LAST_EXP_TIME { get; set; }
        public string LAST_EXP_USERNAME { get; set; }
        public long? MANU_IMP_MEST_ID { get; set; }
        public string MEDI_STOCK_CODE { get; set; }
        public long MEDI_STOCK_ID { get; set; }
        public string MEDI_STOCK_NAME { get; set; }
        public string NATIONAL_EXP_MEST_CODE { get; set; }
        public long? PRESCRIPTION_ID { get; set; }
        public string REQ_DEPARTMENT_CODE { get; set; }
        public long REQ_DEPARTMENT_ID { get; set; }
        public string REQ_DEPARTMENT_NAME { get; set; }
        public string REQ_LOGINNAME { get; set; }
        public string REQ_ROOM_CODE { get; set; }
        public long REQ_ROOM_ID { get; set; }
        public string REQ_ROOM_NAME { get; set; }
        public string REQ_USERNAME { get; set; }
        public long? SALE_PATIENT_TYPE_ID { get; set; }
        public long? SERVICE_REQ_ID { get; set; }
        public long? SUPPLIER_ID { get; set; }
        public string TDL_AGGR_EXP_MEST_CODE { get; set; }
        public string TDL_DISPENSE_CODE { get; set; }
        public long? TDL_INTRUCTION_DATE { get; set; }
        public long? TDL_INTRUCTION_TIME { get; set; }
        public string TDL_MANU_IMP_MEST_CODE { get; set; }
        public long? TDL_PATIENT_ID { get; set; }
        public string TDL_PRESCRIPTION_CODE { get; set; }
        public string TDL_PRESCRIPTION_REQ_LOGINNAME { get; set; }
        public string TDL_PRESCRIPTION_REQ_USERNAME { get; set; }
        public string TDL_SERVICE_REQ_CODE { get; set; }
        public decimal? TDL_TOTAL_PRICE { get; set; }
        public string TDL_TREATMENT_CODE { get; set; }
        public long? TDL_TREATMENT_ID { get; set; }
        public string TDL_XBTT_EXP_MEST_CODE { get; set; }
        public long? VACCINATION_ID { get; set; }
        public long? XBTT_EXP_MEST_ID { get; set; }

        public string KEY_GROUP { get; set; }

        public Mps000262CommonInfoADO() { }

        public Mps000262CommonInfoADO(V_HIS_TREATMENT data, V_HIS_TREATMENT_BED_ROOM bedRoom, V_HIS_EXP_MEST expMest)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<Mps000262CommonInfoADO>(this, data);
            }

            if (bedRoom != null)
            {
                this.ADD_LOGINNAME = bedRoom.ADD_LOGINNAME;
                this.ADD_TIME = bedRoom.ADD_TIME;
                this.ADD_USERNAME = bedRoom.ADD_USERNAME;
                this.BED_CODE = bedRoom.BED_CODE;
                this.BED_ID = bedRoom.BED_ID;
                this.BED_NAME = bedRoom.BED_NAME;
                this.BED_ROOM_CODE = bedRoom.BED_ROOM_CODE;
                this.BED_ROOM_ID = bedRoom.BED_ROOM_ID;
                this.BED_ROOM_NAME = bedRoom.BED_ROOM_NAME;
                this.CO_TREATMENT_ID = bedRoom.CO_TREATMENT_ID;
                this.REMOVE_LOGINNAME = bedRoom.REMOVE_LOGINNAME;
                this.REMOVE_TIME = bedRoom.REMOVE_TIME;
                this.REMOVE_USERNAME = bedRoom.REMOVE_USERNAME;
            }

            if (expMest != null)
            {
                this.AGGR_EXP_MEST_ID = expMest.AGGR_EXP_MEST_ID;
                this.AGGR_USE_TIME = expMest.AGGR_USE_TIME;
                this.BILL_ID = expMest.BILL_ID;
                this.CASHIER_LOGINNAME = expMest.CASHIER_LOGINNAME;
                this.CASHIER_USERNAME = expMest.CASHIER_USERNAME;
                this.CREATE_DATE = expMest.CREATE_DATE;
                this.DESCRIPTION = expMest.DESCRIPTION;
                this.DISCOUNT = expMest.DISCOUNT;
                this.DISPENSE_ID = expMest.DISPENSE_ID;
                this.EXP_MEST_CODE = expMest.EXP_MEST_CODE;
                this.EXP_MEST_REASON_CODE = expMest.EXP_MEST_REASON_CODE;
                this.EXP_MEST_REASON_ID = expMest.EXP_MEST_REASON_ID;
                this.EXP_MEST_REASON_NAME = expMest.EXP_MEST_REASON_NAME;
                this.EXP_MEST_STT_CODE = expMest.EXP_MEST_STT_CODE;
                this.EXP_MEST_STT_ID = expMest.EXP_MEST_STT_ID;
                this.EXP_MEST_STT_NAME = expMest.EXP_MEST_STT_NAME;
                this.EXP_MEST_TYPE_CODE = expMest.EXP_MEST_TYPE_CODE;
                this.EXP_MEST_TYPE_ID = expMest.EXP_MEST_TYPE_ID;
                this.EXP_MEST_TYPE_NAME = expMest.EXP_MEST_TYPE_NAME;
                this.FINISH_DATE = expMest.FINISH_DATE;
                this.FINISH_TIME = expMest.FINISH_TIME;
                this.IMP_MEDI_STOCK_ID = expMest.IMP_MEDI_STOCK_ID;
                this.IS_CABINET = expMest.IS_CABINET;
                this.IS_EXPORT_EQUAL_APPROVE = expMest.IS_EXPORT_EQUAL_APPROVE;
                this.IS_EXPORT_EQUAL_REQUEST = expMest.IS_EXPORT_EQUAL_REQUEST;
                this.IS_HTX = expMest.IS_HTX;
                this.IS_NOT_TAKEN = expMest.IS_NOT_TAKEN;
                this.LAST_EXP_LOGINNAME = expMest.LAST_EXP_LOGINNAME;
                this.LAST_EXP_TIME = expMest.LAST_EXP_TIME;
                this.LAST_EXP_USERNAME = expMest.LAST_EXP_USERNAME;
                this.MANU_IMP_MEST_ID = expMest.MANU_IMP_MEST_ID;
                this.MEDI_STOCK_CODE = expMest.MEDI_STOCK_CODE;
                this.MEDI_STOCK_ID = expMest.MEDI_STOCK_ID;
                this.MEDI_STOCK_NAME = expMest.MEDI_STOCK_NAME;
                this.NATIONAL_EXP_MEST_CODE = expMest.NATIONAL_EXP_MEST_CODE;
                this.PRESCRIPTION_ID = expMest.PRESCRIPTION_ID;
                this.REQ_DEPARTMENT_CODE = expMest.REQ_DEPARTMENT_CODE;
                this.REQ_DEPARTMENT_ID = expMest.REQ_DEPARTMENT_ID;
                this.REQ_DEPARTMENT_NAME = expMest.REQ_DEPARTMENT_NAME;
                this.REQ_LOGINNAME = expMest.REQ_LOGINNAME;
                this.REQ_ROOM_CODE = expMest.REQ_ROOM_CODE;
                this.REQ_ROOM_ID = expMest.REQ_ROOM_ID;
                this.REQ_ROOM_NAME = expMest.REQ_ROOM_NAME;
                this.REQ_USERNAME = expMest.REQ_USERNAME;
                this.SALE_PATIENT_TYPE_ID = expMest.SALE_PATIENT_TYPE_ID;
                this.SERVICE_REQ_ID = expMest.SERVICE_REQ_ID;
                this.SUPPLIER_ID = expMest.SUPPLIER_ID;
                this.TDL_AGGR_EXP_MEST_CODE = expMest.TDL_AGGR_EXP_MEST_CODE;
                this.TDL_DISPENSE_CODE = expMest.TDL_DISPENSE_CODE;
                this.TDL_INTRUCTION_DATE = expMest.TDL_INTRUCTION_DATE;
                this.TDL_INTRUCTION_TIME = expMest.TDL_INTRUCTION_TIME;
                this.TDL_MANU_IMP_MEST_CODE = expMest.TDL_MANU_IMP_MEST_CODE;
                this.TDL_PATIENT_ID = expMest.TDL_PATIENT_ID;
                this.TDL_PRESCRIPTION_CODE = expMest.TDL_PRESCRIPTION_CODE;
                this.TDL_PRESCRIPTION_REQ_LOGINNAME = expMest.TDL_PRESCRIPTION_REQ_LOGINNAME;
                this.TDL_PRESCRIPTION_REQ_USERNAME = expMest.TDL_PRESCRIPTION_REQ_USERNAME;
                this.TDL_SERVICE_REQ_CODE = expMest.TDL_SERVICE_REQ_CODE;
                this.TDL_TOTAL_PRICE = expMest.TDL_TOTAL_PRICE;
                this.TDL_TREATMENT_CODE = expMest.TDL_TREATMENT_CODE;
                this.TDL_TREATMENT_ID = expMest.TDL_TREATMENT_ID;
                this.TDL_XBTT_EXP_MEST_CODE = expMest.TDL_XBTT_EXP_MEST_CODE;
                this.VACCINATION_ID = expMest.VACCINATION_ID;
                this.XBTT_EXP_MEST_ID = expMest.XBTT_EXP_MEST_ID;
            }
        }
    }
}
