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

namespace MPS.Processor.Mps000084.PDO
{
    public partial class Mps000084PDO : RDOBase
    {
        public Mps000084PDO() { }

        public Mps000084PDO(
                V_HIS_IMP_MEST ImpMest,
                List<V_HIS_IMP_MEST_MEDICINE> hisImpMestMedicines,
                List<V_HIS_IMP_MEST_MATERIAL> hisImpMestMaterials,
                List<V_HIS_EXP_MEST_MEDICINE> _listExpMestMedicine,
                List<V_HIS_EXP_MEST_MATERIAL> _listExpMestMaterial
                )
        {
            try
            {
                this.ImpMest = ImpMest;
                this.listImpMestMedicine = hisImpMestMedicines;
                this.listImpMestMaterial = hisImpMestMaterials;
                this.listExpMestMedicine = _listExpMestMedicine;
                this.listExpMestMaterial = _listExpMestMaterial;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000084PDO(
             V_HIS_IMP_MEST ImpMest,
             V_HIS_EXP_MEST ExpMest,
             SingleKey singleKey,
             List<V_HIS_IMP_MEST_MEDICINE> hisImpMestMedicines,
             List<V_HIS_IMP_MEST_MATERIAL> hisImpMestMaterials,
             List<V_HIS_EXP_MEST_MEDICINE> _listExpMestMedicine,
             List<V_HIS_EXP_MEST_MATERIAL> _listExpMestMaterial
             )
        {
            try
            {
                this.ImpMest = ImpMest;
                this.listImpMestMedicine = hisImpMestMedicines;
                this.listImpMestMaterial = hisImpMestMaterials;
                this.listExpMestMedicine = _listExpMestMedicine;
                this.listExpMestMaterial = _listExpMestMaterial;
                this.ExpMest = ExpMest;
                this.singleKey = singleKey;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
