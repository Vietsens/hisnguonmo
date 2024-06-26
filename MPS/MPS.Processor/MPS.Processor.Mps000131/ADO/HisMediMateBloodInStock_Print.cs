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
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000131.ADO
{
    public class HisMediMateBloodInStock_Print
    {
        public string ACTIVE_INGR_BHYT_CODE { get; set; }
        public string ACTIVE_INGR_BHYT_NAME { get; set; }
        public long? ALERT_EXPIRED_DATE { get; set; }
        public decimal? ALERT_MIN_IN_STOCK { get; set; }
        public decimal? AvailableAmount { get; set; }
        public string BID_NUMBER { get; set; }
        public string CONCENTRA { get; set; }
        public decimal? EXPIRED_DATE { get; set; }
        public long ID { get; set; }
        public decimal? IMP_PRICE { get; set; }
        public long? IMP_TIME { get; set; }
        public decimal? IMP_VAT_RATIO { get; set; }
        public short? IS_ACTIVE { get; set; }
        public short? IS_LEAF { get; set; }
        public bool isTypeNode { get; set; }
        public string MANUFACTURER_NAME { get; set; }
        public long? MEDI_STOCK_ID { get; set; }
        public string MEDICINE_TYPE_CODE { get; set; }
        public string MEDICINE_TYPE_HEIN_NAME { get; set; }
        public long MEDICINE_TYPE_ID { get; set; }
        public short? MEDICINE_TYPE_IS_ACTIVE { get; set; }
        public string MEDICINE_TYPE_NAME { get; set; }
        public string NATIONAL_NAME { get; set; }
        public string NodeId { get; set; }
        public long? NUM_ORDER { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public long? PARENT_ID { get; set; }
        public string ParentNodeId { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public long SERVICE_ID { get; set; }
        public string SERVICE_UNIT_CODE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public long? SUPPLIER_ID { get; set; }
        public string SUPPLIER_NAME { get; set; }
        public decimal? TotalAmount { get; set; }

        public string EXPIRED_DATE_STR { get; set; }
        public decimal? PRICE_WITH_VAT { get; set; }

        public HisMediMateBloodInStock_Print() { }

        public HisMediMateBloodInStock_Print(HisMedicineInStockSDO sdo, bool isParent)
        {

            this.ACTIVE_INGR_BHYT_CODE = sdo.ACTIVE_INGR_BHYT_CODE;
            this.ACTIVE_INGR_BHYT_NAME = sdo.ACTIVE_INGR_BHYT_NAME;
            this.ALERT_EXPIRED_DATE = sdo.ALERT_EXPIRED_DATE;
            this.ALERT_MIN_IN_STOCK = sdo.ALERT_MIN_IN_STOCK;

            this.CONCENTRA = sdo.CONCENTRA;

            this.ID = sdo.ID;

            this.IS_ACTIVE = sdo.IS_ACTIVE;
            this.IS_LEAF = sdo.IS_LEAF;
            this.isTypeNode = sdo.isTypeNode;
            this.MANUFACTURER_NAME = sdo.MANUFACTURER_NAME;
            this.MEDI_STOCK_ID = sdo.MEDI_STOCK_ID;
            this.MEDICINE_TYPE_CODE = sdo.MEDICINE_TYPE_CODE;
            this.MEDICINE_TYPE_HEIN_NAME = sdo.MEDICINE_TYPE_HEIN_NAME;
            this.MEDICINE_TYPE_ID = sdo.MEDICINE_TYPE_ID;
            this.MEDICINE_TYPE_IS_ACTIVE = sdo.MEDICINE_TYPE_IS_ACTIVE;
            this.MEDICINE_TYPE_NAME = sdo.MEDICINE_TYPE_NAME;
            this.NATIONAL_NAME = sdo.NATIONAL_NAME;
            this.NodeId = sdo.NodeId;
            this.NUM_ORDER = sdo.NUM_ORDER;

            this.PARENT_ID = sdo.PARENT_ID;
            this.ParentNodeId = sdo.ParentNodeId;

            this.SERVICE_ID = sdo.SERVICE_ID;
            this.SERVICE_UNIT_CODE = sdo.SERVICE_UNIT_CODE;
            this.SERVICE_UNIT_NAME = sdo.SERVICE_UNIT_NAME;

            if (!isParent)
            {
                this.AvailableAmount = sdo.AvailableAmount;
                this.BID_NUMBER = sdo.BID_NUMBER;
                this.EXPIRED_DATE = sdo.EXPIRED_DATE;
                this.IMP_PRICE = sdo.IMP_PRICE;
                this.IMP_TIME = sdo.IMP_TIME;
                this.PACKAGE_NUMBER = sdo.PACKAGE_NUMBER;
                this.REGISTER_NUMBER = sdo.REGISTER_NUMBER;
                this.IMP_VAT_RATIO = sdo.IMP_VAT_RATIO;
                this.SUPPLIER_CODE = sdo.SUPPLIER_CODE;
                this.SUPPLIER_ID = sdo.SUPPLIER_ID;
                this.SUPPLIER_NAME = sdo.SUPPLIER_NAME;
                this.TotalAmount = sdo.TotalAmount;
                this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(Convert.ToInt64(this.EXPIRED_DATE ?? 0));
            }
        }

        public HisMediMateBloodInStock_Print(HisMaterialInStockSDO sdo, bool isParent)
        {

            this.ALERT_EXPIRED_DATE = sdo.ALERT_EXPIRED_DATE;
            this.ALERT_MIN_IN_STOCK = sdo.ALERT_MIN_IN_STOCK;

            this.CONCENTRA = sdo.CONCENTRA;

            this.ID = sdo.ID;

            this.IS_ACTIVE = sdo.IS_ACTIVE;
            this.IS_LEAF = sdo.IS_LEAF;
            this.isTypeNode = sdo.isTypeNode;
            this.MANUFACTURER_NAME = sdo.MANUFACTURER_NAME;
            this.MEDI_STOCK_ID = sdo.MEDI_STOCK_ID;
            this.MEDICINE_TYPE_CODE = sdo.MATERIAL_TYPE_CODE;
            this.MEDICINE_TYPE_ID = sdo.MATERIAL_TYPE_ID;
            this.MEDICINE_TYPE_IS_ACTIVE = sdo.MATERIAL_TYPE_IS_ACTIVE;
            this.MEDICINE_TYPE_NAME = sdo.MATERIAL_TYPE_NAME;
            this.NATIONAL_NAME = sdo.NATIONAL_NAME;
            this.NodeId = sdo.NodeId;
            this.NUM_ORDER = sdo.NUM_ORDER;

            this.PARENT_ID = sdo.PARENT_ID;
            this.ParentNodeId = sdo.ParentNodeId;

            this.SERVICE_ID = sdo.SERVICE_ID;
            this.SERVICE_UNIT_CODE = sdo.SERVICE_UNIT_CODE;
            this.SERVICE_UNIT_NAME = sdo.SERVICE_UNIT_NAME;

            if (!isParent)
            {
                this.AvailableAmount = sdo.AvailableAmount;
                this.BID_NUMBER = sdo.BID_NUMBER;
                this.EXPIRED_DATE = sdo.EXPIRED_DATE;
                this.IMP_PRICE = sdo.IMP_PRICE;
                this.IMP_TIME = sdo.IMP_TIME;
                this.IMP_VAT_RATIO = sdo.IMP_VAT_RATIO;
                this.PACKAGE_NUMBER = sdo.PACKAGE_NUMBER;
                this.REGISTER_NUMBER = sdo.REGISTER_NUMBER;
                this.SUPPLIER_CODE = sdo.SUPPLIER_CODE;
                this.SUPPLIER_ID = sdo.SUPPLIER_ID;
                this.SUPPLIER_NAME = sdo.SUPPLIER_NAME;
                this.TotalAmount = sdo.TotalAmount;
                this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(Convert.ToInt64(this.EXPIRED_DATE ?? 0));
            }
        }
    }
}
