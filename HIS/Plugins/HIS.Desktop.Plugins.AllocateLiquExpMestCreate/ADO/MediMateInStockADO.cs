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

namespace HIS.Desktop.Plugins.AllocateLiquExpMestCreate.ADO
{
    public class MediMateInStockADO
    {
        public long MEDI_MATE_ID { get; set; }
        public string MEDI_MATE_TYPE_CODE { get; set; }
        public string MEDI_MATE_TYPE_NAME { get; set; }
        public decimal EXP_AMOUNT { get; set; }
        public long ID_ROW { get; set; }
        public long MEDI_MATE_TYPE_ID { get; set; }

        public string ACTIVE_INGR_BHYT_CODE { get; set; }
        public string ACTIVE_INGR_BHYT_NAME { get; set; }
        public long? ALERT_EXPIRED_DATE { get; set; }
        public decimal? ALERT_MIN_IN_STOCK { get; set; }
        public decimal? AvailableAmount { get; set; }
        public string BID_NUMBER { get; set; }
        public decimal? EXPIRED_DATE { get; set; }
        public decimal IMP_PRICE { get; set; }
        public long? IMP_TIME { get; set; }
        public decimal IMP_VAT_RATIO { get; set; }
        public short? IS_ACTIVE { get; set; }
        public short? IS_LEAF { get; set; }
        public bool isTypeNode { get; set; }
        public long? MEDI_STOCK_ID { get; set; }
        public string MEDICINE_TYPE_HEIN_NAME { get; set; }
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
        public string MANUFACTURER_NAME { get; set; }
        public decimal? ExpPrice { get; set; }
        public decimal? ExpVatRatio { get; set; }

        public bool IsMedicine { get; set; }


        public MediMateInStockADO() { }

        public MediMateInStockADO(HisMedicineInStockSDO medicine)
        {
            try
            {
                if (medicine != null)
                {
                    this.IsMedicine = true;
                    this.ACTIVE_INGR_BHYT_CODE = medicine.ACTIVE_INGR_BHYT_CODE;
                    this.ACTIVE_INGR_BHYT_NAME = medicine.ACTIVE_INGR_BHYT_NAME;
                    this.ALERT_EXPIRED_DATE = medicine.ALERT_EXPIRED_DATE;
                    this.ALERT_MIN_IN_STOCK = medicine.ALERT_MIN_IN_STOCK;
                    this.AvailableAmount = medicine.AvailableAmount;
                    this.BID_NUMBER = medicine.BID_NUMBER;
                    this.EXPIRED_DATE = medicine.EXPIRED_DATE;
                    this.IMP_PRICE = medicine.IMP_PRICE;
                    this.IMP_TIME = medicine.IMP_TIME;
                    this.IMP_VAT_RATIO = medicine.IMP_VAT_RATIO;
                    this.IS_ACTIVE = medicine.IS_ACTIVE;
                    this.IS_LEAF = medicine.IS_LEAF;
                    this.isTypeNode = medicine.isTypeNode;
                    this.MEDI_STOCK_ID = medicine.MEDI_STOCK_ID;
                    this.MEDI_MATE_TYPE_CODE = medicine.MEDICINE_TYPE_CODE;
                    this.MEDICINE_TYPE_HEIN_NAME = medicine.MEDICINE_TYPE_HEIN_NAME;
                    this.MEDI_MATE_ID = medicine.ID;
                    this.MEDI_MATE_TYPE_ID = medicine.MEDICINE_TYPE_ID;
                    this.MEDI_MATE_TYPE_NAME = medicine.MEDICINE_TYPE_NAME;
                    this.NATIONAL_NAME = medicine.NATIONAL_NAME;
                    this.NodeId = medicine.NodeId;
                    this.NUM_ORDER = medicine.NUM_ORDER;
                    this.PACKAGE_NUMBER = medicine.PACKAGE_NUMBER;
                    this.PARENT_ID = medicine.PARENT_ID;
                    this.ParentNodeId = medicine.ParentNodeId;
                    this.REGISTER_NUMBER = medicine.REGISTER_NUMBER;
                    this.SERVICE_ID = medicine.SERVICE_ID;
                    this.SERVICE_UNIT_CODE = medicine.SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = medicine.SERVICE_UNIT_NAME;
                    this.SUPPLIER_CODE = medicine.SUPPLIER_CODE;
                    this.SUPPLIER_ID = medicine.SUPPLIER_ID;
                    this.SUPPLIER_NAME = medicine.SUPPLIER_NAME;
                    this.TotalAmount = medicine.TotalAmount;
                    this.MANUFACTURER_NAME = medicine.MANUFACTURER_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public MediMateInStockADO(HisMaterialInStockSDO material)
        {
            try
            {
                if (material != null)
                {
                    this.IsMedicine = false;
                    this.ALERT_EXPIRED_DATE = material.ALERT_EXPIRED_DATE;
                    this.ALERT_MIN_IN_STOCK = material.ALERT_MIN_IN_STOCK;
                    this.AvailableAmount = material.AvailableAmount;
                    this.BID_NUMBER = material.BID_NUMBER;
                    this.EXPIRED_DATE = material.EXPIRED_DATE;
                    this.IMP_PRICE = material.IMP_PRICE;
                    this.IMP_TIME = material.IMP_TIME;
                    this.IMP_VAT_RATIO = material.IMP_VAT_RATIO;
                    this.IS_ACTIVE = material.IS_ACTIVE;
                    this.IS_LEAF = material.IS_LEAF;
                    this.isTypeNode = material.isTypeNode;
                    this.MEDI_STOCK_ID = material.MEDI_STOCK_ID;
                    this.MEDI_MATE_TYPE_CODE = material.MATERIAL_TYPE_CODE;
                    this.MEDI_MATE_ID = material.ID;
                    this.MEDI_MATE_TYPE_ID = material.MATERIAL_TYPE_ID;
                    this.MEDI_MATE_TYPE_NAME = material.MATERIAL_TYPE_NAME;
                    this.NATIONAL_NAME = material.NATIONAL_NAME;
                    this.NodeId = material.NodeId;
                    this.NUM_ORDER = material.NUM_ORDER;
                    this.PACKAGE_NUMBER = material.PACKAGE_NUMBER;
                    this.PARENT_ID = material.PARENT_ID;
                    this.ParentNodeId = material.ParentNodeId;
                    this.REGISTER_NUMBER = material.REGISTER_NUMBER;
                    this.SERVICE_ID = material.SERVICE_ID;
                    this.SERVICE_UNIT_CODE = material.SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = material.SERVICE_UNIT_NAME;
                    this.SUPPLIER_CODE = material.SUPPLIER_CODE;
                    this.SUPPLIER_ID = material.SUPPLIER_ID;
                    this.SUPPLIER_NAME = material.SUPPLIER_NAME;
                    this.TotalAmount = material.TotalAmount;
                    this.MANUFACTURER_NAME = material.MANUFACTURER_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
