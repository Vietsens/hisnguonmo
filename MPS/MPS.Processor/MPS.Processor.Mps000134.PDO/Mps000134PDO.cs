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
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000134.PDO
{
    public partial class Mps000134PDO : RDOBase
    {
        public List<V_HIS_MEDICINE_TYPE> _MedicineTypes { get; set; }
        public List<V_HIS_MATERIAL_TYPE> _MaterialTypes { get; set; }
        public List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedicines { get; set; }
        public List<V_HIS_EXP_MEST_MATERIAL> _ExpMestMaterials { get; set; }
        public Mps000134PDO() { }
        public Mps000134PDO(
            List<HIS_EXP_MEST_METY_REQ> _expMestMetyReqs,
            List<HIS_EXP_MEST_MATY_REQ> _expMestMatyReqs,
            List<V_HIS_EXP_MEST_MEDICINE> _expMestMedicines,
            List<V_HIS_EXP_MEST_MATERIAL> _expMestMaterials,
            V_HIS_EXP_MEST expMest,
            long _expMesttSttId__Draft,
            long _expMesttSttId__Request,
            long _expMesttSttId__Reject,
            long _expMesttSttId__Approval,
            long _expMesttSttId__Export,
            List<V_HIS_MEDICINE_TYPE> _medicineTypes,
            List<V_HIS_MATERIAL_TYPE> _materialTypes,
            string _Title
            )
        {
            this._ExpMestMetyReqs = _expMestMetyReqs;
            this._ExpMestMatyReqs = _expMestMatyReqs;
            this._ExpMestMedicines = _expMestMedicines;
            this._ExpMestMaterials = _expMestMaterials;
            this._ExpMest = expMest;
            this.expMesttSttId__Draft = _expMesttSttId__Draft;
            this.expMesttSttId__Request = _expMesttSttId__Request;
            this.expMesttSttId__Reject = _expMesttSttId__Reject;
            this.expMesttSttId__Approval = _expMesttSttId__Approval;
            this.expMesttSttId__Export = _expMesttSttId__Export;
            this._MedicineTypes = _medicineTypes;
            this._MaterialTypes = _materialTypes;
            this.Title = _Title;
        }
    }
}
