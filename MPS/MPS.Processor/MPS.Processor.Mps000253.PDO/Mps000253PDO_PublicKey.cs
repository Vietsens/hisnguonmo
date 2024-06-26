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

namespace MPS.Processor.Mps000253.PDO
{
    public partial class Mps000253PDO : RDOBase
    {
        public V_HIS_TREATMENT treatment = null;
        public V_HIS_ALLERGY_CARD allergyCard = null;
        public Mps000253ADO mps000253ADO = null;
    }

    public class Mps000253ADO
    {
        public string DEPARTMENT_CODE { get; set; }
        public string DEPARTMENT_NAME { get; set; }
    }

    public class AllergenicADO : V_HIS_ALLERGENIC
    {
        public string SURE { get; set; }
        public string DOUBT { get; set; }

        public AllergenicADO(V_HIS_ALLERGENIC data)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<AllergenicADO>(this, data);
            this.SURE = data.IS_SURE == 1 ? "x" : "";
            this.DOUBT = data.IS_DOUBT == 1 ? "x" : "";
        }
    }
}
