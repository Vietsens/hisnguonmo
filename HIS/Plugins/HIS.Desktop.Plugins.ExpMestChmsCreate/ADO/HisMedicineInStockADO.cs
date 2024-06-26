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

namespace HIS.Desktop.Plugins.ExpMestChmsCreate.ADO
{
    public class HisMedicineInStockADO : HisMedicineIn2StockSDO
    {
        public bool IsCheck { get; set; }

        public decimal? ExportedTotalAmount { get; set; }
        public decimal? ImpStockAvailableAmount { get; set; }
        public decimal? ImpStockTotalAmount { get; set; }
        public decimal? EXP_AMOUNT { get; set; }
        public string EXPIRED_DATE_STRS { get; set; }
        public virtual ICollection<HisMedicineInStockADO> MedicineInStocks { get; set; }

        public HisMedicineInStockADO()
        { }

        public HisMedicineInStockADO(HisMedicineTypeInStockSDO item)
        {
            this.AvailableAmount = item.AvailableAmount;
            this.CONCENTRA = item.Concentra;
            this.ExportedTotalAmount = item.ExportedTotalAmount;
            this.ImpStockAvailableAmount = item.ImpStockAvailableAmount;
            this.ImpStockTotalAmount = item.ImpStockTotalAmount;
            this.IS_ACTIVE = item.IsActive;
            this.IS_LEAF = item.IsLeaf;
            this.IsCheck = true;
            this.MANUFACTURER_NAME = item.ManufacturerName;
            this.MEDICINE_TYPE_CODE = item.MedicineTypeCode;
            this.MEDICINE_TYPE_ID = item.Id;
            this.MEDICINE_TYPE_NAME = item.MedicineTypeName;
            this.MEDI_STOCK_ID = item.MediStockId;
            this.NATIONAL_NAME = item.NationalName;
            this.PARENT_ID = item.ParentId;
            this.SERVICE_ID = item.ServiceId;
            this.SERVICE_UNIT_CODE = item.ServiceUnitCode;
            this.SERVICE_UNIT_NAME = item.ServiceUnitName;
            this.TotalAmount = item.TotalAmount;
            this.EXP_AMOUNT = Math.Min(item.AvailableAmount ?? 0, item.ExportedTotalAmount ?? 0);
        }

        public HisMedicineInStockADO(HisMedicineIn2StockSDO item, List<HisMedicineIn2StockSDO> lstItem)
        {
            if (item != null)
            {
                this.ACTIVE_INGR_BHYT_CODE = item.ACTIVE_INGR_BHYT_CODE;
                this.ACTIVE_INGR_BHYT_NAME = item.ACTIVE_INGR_BHYT_NAME;
                this.ALERT_EXPIRED_DATE = item.ALERT_EXPIRED_DATE;
                this.ALERT_MIN_IN_STOCK = item.ALERT_MIN_IN_STOCK;
                this.TotalAmount = item.TotalAmount;
                this.AvailableAmount = item.AvailableAmount;
                this.SecondTotalAmount = item.SecondTotalAmount;
                this.SecondAvailableAmount = item.SecondAvailableAmount;
                //this.BaseAmount = item.BaseAmount;
                this.BID_NUMBER = item.BID_NUMBER;
                this.CONCENTRA = item.CONCENTRA;
                this.EXPIRED_DATE = item.EXPIRED_DATE;

                this.PACKAGE_NUMBER = item.PACKAGE_NUMBER;

                if (lstItem != null && lstItem.Count > 0)
                {
                    List<string> lstExpiredDate = new List<string>();
                    this.MedicineInStocks = new List<HisMedicineInStockADO>();

                    foreach (var i in lstItem)
                    {
                        if (i.EXPIRED_DATE.HasValue)
                        {
                            lstExpiredDate.Add(Inventec.Common.DateTime.Convert.TimeNumberToDateString(i.EXPIRED_DATE.ToString()));
                        }

                        HisMedicineInStockADO ado = new HisMedicineInStockADO();
                        ado.AvailableAmount = i.AvailableAmount;
                        ado.TotalAmount = i.TotalAmount;
                        ado.EXPIRED_DATE = i.EXPIRED_DATE;
                        ado.PACKAGE_NUMBER = i.PACKAGE_NUMBER;
                        this.MedicineInStocks.Add(ado);
                    }

                    this.EXPIRED_DATE_STRS = String.Join(", ", lstExpiredDate.Distinct());
                    this.PACKAGE_NUMBER = null;
                }

                this.ID = item.ID;
                this.IMP_PRICE = item.IMP_PRICE;
                this.IsCheck = true;
                this.IMP_TIME = item.IMP_TIME;
                this.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                this.IS_ACTIVE = item.IS_ACTIVE;
                this.IS_LEAF = item.IS_LEAF;
                this.isTypeNode = item.isTypeNode;
                this.MANUFACTURER_NAME = item.MANUFACTURER_NAME;
                this.MEDI_STOCK_ID = item.MEDI_STOCK_ID;
                this.MEDICINE_TYPE_CODE = item.MEDICINE_TYPE_CODE;
                this.MEDICINE_TYPE_HEIN_NAME = item.MEDICINE_TYPE_HEIN_NAME;
                this.MEDICINE_TYPE_ID = item.MEDICINE_TYPE_ID;
                this.MEDICINE_TYPE_IS_ACTIVE = item.MEDICINE_TYPE_IS_ACTIVE;
                this.MEDICINE_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                this.NATIONAL_NAME = item.NATIONAL_NAME;
                this.NodeId = item.NodeId;
                this.NUM_ORDER = item.NUM_ORDER;
                this.PARENT_ID = item.PARENT_ID;
                this.ParentNodeId = item.ParentNodeId;
                //this.RealBaseAmount = item.RealBaseAmount;
                this.REGISTER_NUMBER = item.REGISTER_NUMBER;
                this.SERVICE_ID = item.SERVICE_ID;
                this.SERVICE_UNIT_CODE = item.SERVICE_UNIT_CODE;
                this.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                this.SUPPLIER_CODE = item.SUPPLIER_CODE;
                this.SUPPLIER_ID = item.SUPPLIER_ID;
                this.SUPPLIER_NAME = item.SUPPLIER_NAME;
                this.TotalAmount = item.TotalAmount;
            }
        }
    }
}
