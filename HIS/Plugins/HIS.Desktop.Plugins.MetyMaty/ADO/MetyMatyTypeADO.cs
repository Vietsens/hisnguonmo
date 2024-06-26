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
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MetyMaty.ADO
{
    public class MetyMatyTypeADO : HisMedicineTypeInStockSDO
    {
        public string ADDITION_INFO { get; set; }
        public decimal YCD_AMOUNT { get; set; }

        public MetyMatyTypeADO()
        {
        }

        public MetyMatyTypeADO(HisMedicineTypeInStockSDO _data)
        {
            if (_data != null)
            {
                System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<HisMedicineTypeInStockSDO>();

                foreach (var item in pi)
                {
                    item.SetValue(this, (item.GetValue(_data)));
                }
            }
        }

        public MetyMatyTypeADO(HisMaterialTypeInStockSDO _data)
        {
            if (_data != null)
            {
                this.AvailableAmount = _data.AvailableAmount;
                this.Concentra = _data.Concentra;
                this.ExportedTotalAmount = _data.ExportedTotalAmount;
                this.Id = _data.Id;
                this.ImpStockAvailableAmount = _data.ImpStockAvailableAmount;
                this.ImpStockTotalAmount = _data.ImpStockTotalAmount;
                this.IsActive = _data.IsActive;
                this.IsBusiness = _data.IsBusiness;
                this.IsGoodsRestrict = _data.IsGoodsRestrict;
                this.IsLeaf = _data.IsLeaf;
                this.ManufacturerCode = _data.ManufacturerCode;
                this.ManufacturerName = _data.ManufacturerName;
                this.MedicineTypeCode = _data.MaterialTypeCode;
                this.MedicineTypeName = _data.MaterialTypeName;
                this.MediStockCode = _data.MediStockCode;
                this.MediStockId = _data.MediStockId;
                this.MediStockName = _data.MediStockName;
                this.NationalCode = _data.NationalCode;
                this.NationalName = _data.NationalName;
                this.NumOrder = _data.NumOrder;
                this.ParentId = _data.ParentId;
                this.ServiceId = _data.ServiceId;
                this.ServiceUnitCode = _data.ServiceUnitCode;
                this.ServiceUnitId = _data.ServiceUnitId;
                this.ServiceUnitName = _data.ServiceUnitName;
                this.ServiceUnitSymbol = _data.ServiceUnitSymbol;
                this.TotalAmount = _data.TotalAmount;
            }
        }
    }
}
