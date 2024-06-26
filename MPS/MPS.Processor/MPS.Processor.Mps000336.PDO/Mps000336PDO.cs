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
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000336.PDO
{
    public class Mps000336PDO : RDOBase
    {
        public V_HIS_MEDI_STOCK MediStock { get; set; }
        public List<HisMaterialTypeView1SDO> MaterialTypeSdos { get; set; }
        public V_HIS_MATERIAL_TYPE ParentMaterialType { get; set; }
        public List<HIS_MATERIAL> Materials { get; set; }
        public List<V_HIS_MATERIAL_PATY> MaterialPaties { get; set; }
        public List<HIS_SUPPLIER> ListSupplier { get; set; }
        public List<HIS_MANUFACTURER> ListManufacturer { get; set; }
        public List<HisMaterialTypeView1SDO> medicineTypeIsLeafs { get; set; }
        public List<HIS_SALE_PROFIT_CFG> saleProfitCfgs { get; set; }
        public List<V_HIS_MATERIAL_TYPE> TotalMaterialType { get; set; }

        public Mps000336PDO() { }

        public Mps000336PDO(V_HIS_MEDI_STOCK mediStock, V_HIS_MATERIAL_TYPE parent, List<HisMaterialTypeView1SDO> materialTypeSdos)
        {
            this.MediStock = mediStock;
            this.ParentMaterialType = parent;
            this.MaterialTypeSdos = materialTypeSdos;
        }

        public Mps000336PDO(V_HIS_MEDI_STOCK mediStock, V_HIS_MATERIAL_TYPE parent, List<HisMaterialTypeView1SDO> materialTypeSdos, List<HIS_MATERIAL> _Materials, List<V_HIS_MATERIAL_PATY> _MedicinePaties, List<HIS_SUPPLIER> _ListSupplier, List<HIS_MANUFACTURER> _ListManufacturer, List<HIS_SALE_PROFIT_CFG> _saleProfitCfgs)
        {
            this.MediStock = mediStock;
            this.ParentMaterialType = parent;
            this.MaterialTypeSdos = materialTypeSdos;
            this.Materials = _Materials;
            this.MaterialPaties = _MedicinePaties;
            this.ListSupplier = _ListSupplier;
            this.ListManufacturer = _ListManufacturer;
            this.saleProfitCfgs = _saleProfitCfgs;
        }

        public Mps000336PDO(V_HIS_MEDI_STOCK mediStock, V_HIS_MATERIAL_TYPE parent, List<HisMaterialTypeView1SDO> materialTypeSdos, List<HIS_MATERIAL> _Materials, List<V_HIS_MATERIAL_PATY> _MedicinePaties, List<HIS_SUPPLIER> _ListSupplier, List<HIS_MANUFACTURER> _ListManufacturer, List<HisMaterialTypeView1SDO> _MedicineTypeIsLeafs, List<HIS_SALE_PROFIT_CFG> _saleProfitCfgs)
        {
            this.MediStock = mediStock;
            this.ParentMaterialType = parent;
            this.MaterialTypeSdos = materialTypeSdos;
            this.Materials = _Materials;
            this.MaterialPaties = _MedicinePaties;
            this.ListSupplier = _ListSupplier;
            this.ListManufacturer = _ListManufacturer;
            this.medicineTypeIsLeafs = _MedicineTypeIsLeafs;
            this.saleProfitCfgs = _saleProfitCfgs;
        }

    }
}
