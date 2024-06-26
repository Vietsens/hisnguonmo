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

namespace MPS.Processor.Mps000335.PDO
{
    public class Mps000335PDO : RDOBase
    {
        public V_HIS_MEDI_STOCK MediStock { get; set; }
        public List<HisMedicineTypeView1SDO> MedicineTypeSdos { get; set; }
        public V_HIS_MEDICINE_TYPE ParentMedicineType { get; set; }
        public List<HIS_MEDICINE> Medicines { get; set; }
        public List<V_HIS_MEDICINE_PATY> MedicinePaties { get; set; }
        public List<HIS_SUPPLIER> ListSupplier { get; set; }
        public List<HIS_MANUFACTURER> ListManufacturer { get; set; }
        public List<HisMedicineTypeView1SDO> medicineTypeIsLeafs { get; set; }
        public List<HIS_SALE_PROFIT_CFG> saleProfitCfgs { get; set; }
        public List<V_HIS_MEDICINE_TYPE> TotalMedicineType { get; set; }

        public Mps000335PDO() { }

        public Mps000335PDO(V_HIS_MEDI_STOCK mediStock, V_HIS_MEDICINE_TYPE parent, List<HisMedicineTypeView1SDO> medicineTypeSdos, List<HIS_MEDICINE> _Medicines, List<V_HIS_MEDICINE_PATY> _MedicinePaties, List<HIS_SUPPLIER> _ListSupplier, List<HIS_MANUFACTURER> _ListManufacturer, List<HIS_SALE_PROFIT_CFG> _saleProfitCfgs)
        {
            this.MediStock = mediStock;
            this.ParentMedicineType = parent;
            this.MedicineTypeSdos = medicineTypeSdos;
            this.Medicines = _Medicines;
            this.MedicinePaties = _MedicinePaties;
            this.ListSupplier = _ListSupplier;
            this.ListManufacturer = _ListManufacturer;
            this.saleProfitCfgs = _saleProfitCfgs;
        }

        public Mps000335PDO(V_HIS_MEDI_STOCK mediStock, V_HIS_MEDICINE_TYPE parent, List<HisMedicineTypeView1SDO> medicineTypeSdos, List<HIS_MEDICINE> _Medicines, List<V_HIS_MEDICINE_PATY> _MedicinePaties, List<HIS_SUPPLIER> _ListSupplier, List<HIS_MANUFACTURER> _ListManufacturer, List<HisMedicineTypeView1SDO> _medicineTypeIsLeafs, List<HIS_SALE_PROFIT_CFG> _saleProfitCfgs)
        {
            this.MediStock = mediStock;
            this.ParentMedicineType = parent;
            this.MedicineTypeSdos = medicineTypeSdos;
            this.Medicines = _Medicines;
            this.MedicinePaties = _MedicinePaties;
            this.ListSupplier = _ListSupplier;
            this.ListManufacturer = _ListManufacturer;
            this.medicineTypeIsLeafs = _medicineTypeIsLeafs;
            this.saleProfitCfgs = _saleProfitCfgs;
        }

        public Mps000335PDO(V_HIS_MEDI_STOCK mediStock, V_HIS_MEDICINE_TYPE parent, List<HisMedicineTypeView1SDO> medicineTypeSdos)
        {
            this.MediStock = mediStock;
            this.ParentMedicineType = parent;
            this.MedicineTypeSdos = medicineTypeSdos;
        }
    }
}
