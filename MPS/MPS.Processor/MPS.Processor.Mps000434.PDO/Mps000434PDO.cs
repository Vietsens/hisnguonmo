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

namespace MPS.Processor.Mps000434.PDO
{
    public class Mps000434PDO : RDOBase
    {
        public V_HIS_TREATMENT VHisTreatment { get; set; }
        public List<V_HIS_EXP_MEST_MEDICINE> ListExpMestMedicine { get; set; }
        public List<V_HIS_EXP_MEST_MATERIAL> ListExpMestMaterial { get; set; }
        public Mps000434PDOConfig Mps000434PdoCFG { get; set; }

        public Mps000434PDO(V_HIS_TREATMENT _hisTreatment, List<V_HIS_EXP_MEST_MEDICINE> _listExpMestMedicine, List<V_HIS_EXP_MEST_MATERIAL> _listExpMestMaterial, Mps000434PDOConfig _Mps000434PdoCFG)
        {
            this.VHisTreatment = _hisTreatment;
            this.ListExpMestMaterial = _listExpMestMaterial;
            this.ListExpMestMedicine = _listExpMestMedicine;
            this.Mps000434PdoCFG = _Mps000434PdoCFG;
        }

        public class Mps000434PDOConfig
        {
            public long ConfigKeyMergerData { get; set; }
        }
    }
}
