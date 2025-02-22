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

namespace MPS.Processor.Mps000328.PDO
{
    public class Mps000328PDO : RDOBase
    {
        public V_HIS_IMP_MEST_PROPOSE _ImpMestPropose { get; set; }
        public List<V_HIS_IMP_MEST> _ImpMests { get; set; }
        public List<V_HIS_IMP_MEST_PAY> _HisImpMestPays { get; set; }
        public List<HIS_SUPPLIER> _HisSuppliers { get; set; }
        public List<HIS_BRANCH> _HisBranchs { get; set; }
        public List<V_HIS_IMP_MEST_MEDICINE> _Medicines { get; set; }
        public List<V_HIS_IMP_MEST_MATERIAL> _Materials { get; set; }

        public Mps000328PDO() { }

        public Mps000328PDO(V_HIS_IMP_MEST_PROPOSE impMestPropose,
            List<V_HIS_IMP_MEST> impMests,
            List<V_HIS_IMP_MEST_PAY> hisImpMestPays,
            List<HIS_SUPPLIER> hisSuppliers,
            List<HIS_BRANCH> hisBranchs
            )
        {
            this._ImpMestPropose = impMestPropose;
            this._ImpMests = impMests;
            this._HisImpMestPays = hisImpMestPays;
            this._HisSuppliers = hisSuppliers;
            this._HisBranchs = hisBranchs;
        }

        public Mps000328PDO(V_HIS_IMP_MEST_PROPOSE impMestPropose,
            List<V_HIS_IMP_MEST> impMests,
            List<V_HIS_IMP_MEST_PAY> hisImpMestPays,
            List<HIS_SUPPLIER> hisSuppliers,
            List<HIS_BRANCH> hisBranchs,
            List<V_HIS_IMP_MEST_MEDICINE> _medicines,
            List<V_HIS_IMP_MEST_MATERIAL> _materials
            )
        {
            this._ImpMestPropose = impMestPropose;
            this._ImpMests = impMests;
            this._HisImpMestPays = hisImpMestPays;
            this._HisSuppliers = hisSuppliers;
            this._HisBranchs = hisBranchs;
            this._Medicines = _medicines;
            this._Materials = _materials;
        }
    }
}
