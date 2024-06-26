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
using HIS.Desktop.Plugins.PharmacyCashier.Util;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.PharmacyCashier.ADO
{
    public class MetyMatyInStockADO
    {
        public string ActiveIngrBhytCode { get; set; }
        public string ActiveIngrBhytName { get; set; }
        public decimal? AvailableAmount { get; set; }
        public string Concentra { get; set; }
        public decimal? ExportedTotalAmount { get; set; }
        public long Id { get; set; }
        public decimal? ImpStockAvailableAmount { get; set; }
        public decimal? ImpStockTotalAmount { get; set; }
        public short? IsActive { get; set; }
        public short? IsBusiness { get; set; }
        public short? IsGoodsRestrict { get; set; }
        public short? IsLeaf { get; set; }
        public string ManufacturerCode { get; set; }
        public string ManufacturerName { get; set; }
        public string MedicineTypeCode { get; set; }
        public string MedicineTypeName { get; set; }
        public string MediStockCode { get; set; }
        public long? MediStockId { get; set; }
        public string MediStockName { get; set; }
        public string NationalCode { get; set; }
        public string NationalName { get; set; }
        public long? NumOrder { get; set; }
        public long? ParentId { get; set; }
        public string RegisterNumber { get; set; }
        public long ServiceId { get; set; }
        public string ServiceUnitCode { get; set; }
        public long? ServiceUnitId { get; set; }
        public string ServiceUnitName { get; set; }
        public string ServiceUnitSymbol { get; set; }
        public decimal? TotalAmount { get; set; }
        public string MaterialTypeHeinName { get; set; }
        public bool IsMedicine { get; set; }

        public string MedicineTypeCodeUnsign { get; set; }
        public string MedicineTypeNameUnsign { get; set; }
        public string ActiveIngrBhytNameUnsign { get; set; }

        public MetyMatyInStockADO()
        {
        }

        public MetyMatyInStockADO(HisMedicineTypeInStockSDO sdo)
        {
            this.IsMedicine = true;
            this.ActiveIngrBhytCode = sdo.ActiveIngrBhytCode;
            this.ActiveIngrBhytName = sdo.ActiveIngrBhytName;
            this.AvailableAmount = sdo.AvailableAmount;
            this.Concentra = sdo.Concentra;
            this.ExportedTotalAmount = sdo.ExportedTotalAmount;
            this.Id = sdo.Id;
            this.ImpStockAvailableAmount = sdo.ImpStockAvailableAmount;
            this.ImpStockTotalAmount = sdo.ImpStockTotalAmount;
            this.IsActive = sdo.IsActive;
            this.IsBusiness = sdo.IsBusiness;
            this.IsGoodsRestrict = sdo.IsGoodsRestrict;
            this.IsLeaf = sdo.IsLeaf;
            this.ManufacturerCode = sdo.ManufacturerCode;
            this.ManufacturerName = sdo.ManufacturerName;
            this.MedicineTypeCode = sdo.MedicineTypeCode;
            this.MedicineTypeName = sdo.MedicineTypeName;
            this.MediStockCode = sdo.MediStockCode;
            this.MediStockId = sdo.MediStockId;
            this.MediStockName = sdo.MediStockName;
            this.NationalCode = sdo.NationalCode;
            this.NationalName = sdo.NationalName;
            this.NumOrder = sdo.NumOrder;
            this.ParentId = sdo.ParentId;
            this.RegisterNumber = sdo.RegisterNumber;
            this.ServiceId = sdo.ServiceId;
            this.ServiceUnitCode = sdo.ServiceUnitCode;
            this.ServiceUnitId = sdo.ServiceUnitId;
            this.ServiceUnitName = sdo.ServiceUnitName;
            this.ServiceUnitSymbol = sdo.ServiceUnitSymbol;
            this.TotalAmount = sdo.TotalAmount;
            this.ActiveIngrBhytNameUnsign = StringUtil.ConvertToUnSign3(this.ActiveIngrBhytName) + this.ActiveIngrBhytName;
            this.MedicineTypeCodeUnsign = StringUtil.ConvertToUnSign3(this.MedicineTypeCode) + this.MedicineTypeCode;
            this.MedicineTypeNameUnsign = StringUtil.ConvertToUnSign3(this.MedicineTypeName) + this.MedicineTypeName;
        }

        public MetyMatyInStockADO(HisMaterialTypeInStockSDO sdo)
        {
            this.IsMedicine = false;
            this.AvailableAmount = sdo.AvailableAmount;
            this.Concentra = sdo.Concentra;
            this.ExportedTotalAmount = sdo.ExportedTotalAmount;
            this.Id = sdo.Id;
            this.ImpStockAvailableAmount = sdo.ImpStockAvailableAmount;
            this.ImpStockTotalAmount = sdo.ImpStockTotalAmount;
            this.IsActive = sdo.IsActive;
            this.IsBusiness = sdo.IsBusiness;
            this.IsGoodsRestrict = sdo.IsGoodsRestrict;
            this.IsLeaf = sdo.IsLeaf;
            this.ManufacturerCode = sdo.ManufacturerCode;
            this.ManufacturerName = sdo.ManufacturerName;
            this.MedicineTypeCode = sdo.MaterialTypeCode;
            this.MedicineTypeName = sdo.MaterialTypeName;
            this.MaterialTypeHeinName = sdo.MaterialTypeHeinName;
            this.MediStockCode = sdo.MediStockCode;
            this.MediStockId = sdo.MediStockId;
            this.MediStockName = sdo.MediStockName;
            this.NationalCode = sdo.NationalCode;
            this.NationalName = sdo.NationalName;
            this.NumOrder = sdo.NumOrder;
            this.ParentId = sdo.ParentId;
            this.ServiceId = sdo.ServiceId;
            this.ServiceUnitCode = sdo.ServiceUnitCode;
            this.ServiceUnitId = sdo.ServiceUnitId;
            this.ServiceUnitName = sdo.ServiceUnitName;
            this.ServiceUnitSymbol = sdo.ServiceUnitSymbol;
            this.TotalAmount = sdo.TotalAmount;
            this.MedicineTypeCodeUnsign = StringUtil.ConvertToUnSign3(this.MedicineTypeCode) + this.MedicineTypeCode;
            this.MedicineTypeNameUnsign = StringUtil.ConvertToUnSign3(this.MedicineTypeName) + this.MedicineTypeName;
        }
    }
}
