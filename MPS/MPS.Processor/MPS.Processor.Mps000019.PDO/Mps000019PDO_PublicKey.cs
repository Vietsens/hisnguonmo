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

namespace MPS.Processor.Mps000019.PDO
{
    public partial class Mps000019PDO : RDOBase
    {
        public V_HIS_PATIENT Patient { get; set; }
        public V_HIS_DEBATE currentHisDebate { get; set; }
        public List<HIS_DEBATE_USER> lstHisDebateUser { get; set; }
        public V_HIS_DEPARTMENT_TRAN departmentTran { get; set; }
        public V_HIS_TREATMENT treatment { get; set; }
        public Mps000019SingleKey SingleKey { get; set; }

        public class Mps000019SingleKey
        {
            public string departmentName { get; set; }
            public string bebRoomName { get; set; }
            public string genderCode__Male { get; set; }
            public string genderCode__FeMale { get; set; }
            public string BED_NAME { get; set; }
            public string BED_CODE { get; set; }
            public string IN_CODE { get; set; }
        }
    }
}
