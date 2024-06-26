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
        public List<ExeMediReactADO> expMestMediReact = new List<ExeMediReactADO>();
        public V_HIS_TREATMENT_2 currentTreatment = new V_HIS_TREATMENT_2();
        public V_HIS_ROOM thisRoom = new V_HIS_ROOM();
        public V_HIS_BED_LOG bedLog = new V_HIS_BED_LOG();
    }

    public class ExeMediReactADO : MOS.EFMODEL.DataModels.V_HIS_MEDI_REACT
    {
        public string EXECUTE_TIME_STR { get; set; }
        public string CHECK_TIME_STR { get; set; }
        public ExeMediReactADO(V_HIS_MEDI_REACT data)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<ExeMediReactADO>(this, data);
                this.EXECUTE_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.EXECUTE_TIME ?? 0);
                this.CHECK_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CHECK_TIME ?? 0);
            }
        }
    }
}
