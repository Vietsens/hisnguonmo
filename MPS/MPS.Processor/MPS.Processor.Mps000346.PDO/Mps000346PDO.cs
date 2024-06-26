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
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000346.PDO
{
    public partial class Mps000346PDO : RDOBase
    {
        public List<V_HIS_MEDI_STOCK> _MediStocks { get; set; }
        public List<V_HIS_MEDICINE_TYPE> _MedicineTypes { get; set; }
        public List<V_HIS_MATERIAL_TYPE> _MaterialTypes { get; set; }
        public List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReqs { get; set; }
        public List<HIS_EXP_MEST_MATY_REQ> _ExpMestMatyReqs { get; set; }
        public string _Tittle { get; set; }

        public Mps000346PDO(
            V_HIS_EXP_MEST chmsExpMest,
            List<V_HIS_EXP_MEST_MEDICINE> _expMestMedicine,
            List<V_HIS_EXP_MEST_MATERIAL> _expMestMaterial,
            List<HIS_EXP_MEST_METY_REQ> _expMestMetyReqs,
            List<HIS_EXP_MEST_MATY_REQ> _expMestMatyReqs,
            long _expMesttSttId__Approval,
            long _expMesttSttId__Export,
            List<V_HIS_MEDICINE_TYPE> _medicineTypes,
            List<V_HIS_MATERIAL_TYPE> _materialTypes,
            List<HIS_MEDICINE> _medicine,
            List<HIS_MATERIAL> _material,
            string _tittle
            )
        {
            this._ChmsExpMest = chmsExpMest;
            this._ExpMestMaterials = _expMestMaterial;
            this._ExpMestMedicines = _expMestMedicine;
            this._ExpMestMetyReqs = _expMestMetyReqs;
            this._ExpMestMatyReqs = _expMestMatyReqs;
            this.expMesttSttId__Approval = _expMesttSttId__Approval;
            this.expMesttSttId__Export = _expMesttSttId__Export;
            this._MedicineTypes = _medicineTypes;
            this._MaterialTypes = _materialTypes;
            this._Medicines = _medicine;
            this._Materials = _material;
            this._Tittle = _tittle;
        }
    }
}
