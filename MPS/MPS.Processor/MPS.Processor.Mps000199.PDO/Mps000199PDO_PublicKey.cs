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

namespace MPS.Processor.Mps000199.PDO
{
    public partial class Mps000199PDO : RDOBase
    {
        public V_HIS_IMP_MEST _initImpMest = null;
        public V_HIS_IMP_MEST _inveImpMest = null;
        public V_HIS_IMP_MEST _otherImpMest = null;
        public List<V_HIS_IMP_MEST_MEDICINE> _ImpMestMedicines = null;
        public List<V_HIS_IMP_MEST_MATERIAL> _ImpMestMaterials = null;
        public List<V_HIS_IMP_MEST_BLOOD> _ImpMestBloods = null;
        public List<V_HIS_IMP_MEST_USER> _ListIpmMestUser = null;
        public List<MedicalContractADO> _ListMedicalContract = null;

        public class MedicalContractADO : V_HIS_MEDICAL_CONTRACT
        {
            public long MEDICINE_ID { get; set; }
            public long MATERIAL_ID { get; set; }
        }
    }
}
