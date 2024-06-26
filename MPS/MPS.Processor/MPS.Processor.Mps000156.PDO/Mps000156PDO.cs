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

namespace MPS.Processor.Mps000156.PDO
{
    public partial class Mps000156PDO : RDOBase
    {
        public Mps000156PDO() { }
        public Mps000156PDO(List<ExeMediReactADO> expMestMediReact, V_HIS_TREATMENT_2 currentTreatment, V_HIS_ROOM thisRoom)
        {
            this.expMestMediReact = expMestMediReact;
            this.currentTreatment = currentTreatment;
            this.thisRoom = thisRoom;
        }
        public Mps000156PDO(List<ExeMediReactADO> expMestMediReact, V_HIS_TREATMENT_2 currentTreatment, V_HIS_ROOM thisRoom, V_HIS_BED_LOG _bedlog)
        {
            this.expMestMediReact = expMestMediReact;
            this.currentTreatment = currentTreatment;
            this.thisRoom = thisRoom;
            this.bedLog = _bedlog;
        }
    }
}
