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

namespace MPS.Processor.Mps000145.PDO
{
    public partial class Mps000145PDO : RDOBase
    {
        public Mps000145PDO() { }
        public Mps000145PDO(V_HIS_IMP_MEST chmsImpMest, List<V_HIS_IMP_MEST_MEDICINE> impMestMedicines)
        {
            this._ChmsImpMest = chmsImpMest;
            this._ImpMestMedicines = impMestMedicines;
            this._ListAdo = new List<Mps000145ADO>();
        }

        public Mps000145PDO(
            V_HIS_IMP_MEST chmsImpMest,
            List<V_HIS_IMP_MEST_MEDICINE> impMestMedicines,
            List<V_HIS_IMP_MEST_MATERIAL> impMestMaterials)
        {
            this._ChmsImpMest = chmsImpMest;
            this._ImpMestMedicines = impMestMedicines;
            this._ImpMestMaterials = impMestMaterials;
            this._ListAdo = new List<Mps000145ADO>();
        }
    }
}
