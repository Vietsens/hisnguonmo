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

namespace MPS.Processor.Mps000167.PDO
{
    public partial class Mps000167PDO : RDOBase
    {
        public string PATIENT_CODE;
        public string TREATMENT_CODE { get; set; }
        public string SERVICE_REQ_CODE { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER _PatyAlterBhyt = null;
        public V_HIS_SERVICE_REQ _PaanServiceReq = null;
        public V_HIS_SERE_SERV_5 _SereServ = null;
        public Mps000167ADO Mps000167ADO = null;
        public HIS_TREATMENT _HisTreatment = null;
    }

    public class Mps000167ADO
    {
        public string PAAN_LIQUID_CODE { get; set; }
        public string PAAN_LIQUID_NAME { get; set; }
        public string PAAN_POSITION_CODE { get; set; }
        public string PAAN_POSITION_NAME { get; set; }
        public long? KSK_ORDER { get; set; }
        public string REQUEST_USER_MOBILE { get; set; }
    }
}
