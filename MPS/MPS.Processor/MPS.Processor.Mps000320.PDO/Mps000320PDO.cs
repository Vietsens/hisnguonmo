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

namespace MPS.Processor.Mps000320.PDO
{
    public class Mps000320PDO : RDOBase
    {
        public List<V_HIS_SERVICE_REQ_9> _ServiceReqs = null;
        public List<HIS_MACHINE> _Machines = null;
        public V_HIS_EXECUTE_ROOM ExecuteRoom = null;
        public DateTime Day;
        public DateTime FromTime;
        public DateTime ToTime;
        public int Thu;
        public List<Mps000320ADO> listAdo = new List<Mps000320ADO>();

        public Mps000320PDO(MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM executeRoom, List<V_HIS_SERVICE_REQ_9> serviceReqs, List<HIS_MACHINE> listMachine)
        {
            this.ExecuteRoom = executeRoom;
            this._ServiceReqs = serviceReqs;
            this._Machines = listMachine;
        }
    }

    public class Mps000320ADO
    {
        public string MACHINE_CA1 { get; set; }
        public long KIDNEY_SHIFT_CA1 { get; set; }
        public string KIDNEY_SHIFT_STR_CA1 { get; set; }
        public string PATIENT_NAME_CA1 { get; set; }
        public long? KIDNEY_TIMES_CA1 { get; set; }
        public long? PATIENT_DOB_CA1 { get; set; }

        public string MACHINE_CA2 { get; set; }
        public long KIDNEY_SHIFT_CA2 { get; set; }
        public string KIDNEY_SHIFT_STR_CA2 { get; set; }
        public string PATIENT_NAME_CA2 { get; set; }
        public long? KIDNEY_TIMES_CA2 { get; set; }
        public long? PATIENT_DOB_CA2 { get; set; }

        public string MACHINE_CA3 { get; set; }
        public long KIDNEY_SHIFT_CA3 { get; set; }
        public string KIDNEY_SHIFT_STR_CA3 { get; set; }
        public string PATIENT_NAME_CA3 { get; set; }
        public long? KIDNEY_TIMES_CA3 { get; set; }
        public long? PATIENT_DOB_CA3 { get; set; }

        public string MACHINE_CA4 { get; set; }
        public long KIDNEY_SHIFT_CA4 { get; set; }
        public string KIDNEY_SHIFT_STR_CA4 { get; set; }
        public string PATIENT_NAME_CA4 { get; set; }
        public long? KIDNEY_TIMES_CA4 { get; set; }
        public long? PATIENT_DOB_CA4 { get; set; }

        public string MACHINE_CA5 { get; set; }
        public long KIDNEY_SHIFT_CA5 { get; set; }
        public string KIDNEY_SHIFT_STR_CA5 { get; set; }
        public string PATIENT_NAME_CA5 { get; set; }
        public long? KIDNEY_TIMES_CA5 { get; set; }
        public long? PATIENT_DOB_CA5 { get; set; }

        public Mps000320ADO()
        {
        }

    }
}
