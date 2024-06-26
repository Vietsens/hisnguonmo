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

namespace MPS.Processor.Mps000329.PDO
{
    public partial class Mps000329PDO : RDOBase
    {        
        public List<V_HIS_SERE_SERV_1> _vSereServs { get; set; }
        public List<HIS_SERE_SERV_EXT> _SereServExts { get; set; }
        public SingleKeyValue SingleKeyValue { get; set; }
        public List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_1> _ServiceReqs { get; set; }
    }

    public class Mps000329ADO 
    {
        public string DESCRIPTION { get; set; }
        public long INTRUCTION_DATE { get; set; }
        public string TREATMENT_CODE { get; set; }
        public string PATIENT_CODE { get; set; }
        public string PATIENT_NAME { get; set; }
        public long DOB { get; set; }

        public string SERVICE_GROUP_NAME { get; set; }
        public string BED_NAME { get; set; }
               
        public string BUA1 { get; set; }      
        public string BUA2 { get; set; }
        public string BUA3 { get; set; }
        public string BUA4 { get; set; }
        public string BUA5 { get; set; }
     
    }
}
