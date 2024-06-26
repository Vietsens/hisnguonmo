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

namespace MPS.Processor.Mps000262.PDO
{
    public partial class Mps000262PDO : RDOBase
    {
        public List<V_HIS_EXP_MEST_MATERIAL> ListExpMestMaterials { get; set; }
        public List<V_HIS_EXP_MEST_MEDICINE> ListExpMestMedicines { get; set; }
        public List<V_HIS_TREATMENT_BED_ROOM> ListTreatmentBedRoom { get; set; }
        public List<V_HIS_TREATMENT> ListTreatment { get; set; }
        public List<V_HIS_EXP_MEST> ListExpMest { get; set; }
        public V_HIS_EXP_MEST AggrExpMest { get; set; }

        public List<HIS_SERVICE_REQ> ListServiceReq { get; set; }
    }
}
