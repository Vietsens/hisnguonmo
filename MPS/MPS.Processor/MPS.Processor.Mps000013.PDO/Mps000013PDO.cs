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

namespace MPS.Processor.Mps000013.PDO
{
    public class Mps000013PDO : RDOBase
    {
        public V_HIS_PATY_ALTER_BHYT PatyAlterBhyt { get; set; }
        public V_HIS_PATIENT Patient { get; set; }
        public V_HIS_DEPARTMENT_TRAN departmentTran { get; set; }
        public HIS_DHST Dhsts { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER currentHispatientTypeAlter { get; set; }
        public V_HIS_EXAM_SERVICE_REQ ExamServiceReqs { get; set; }
        public V_HIS_TRAN_PATI TranPaties { get; set; }
        public V_HIS_SERE_SERV sereServs { get; set; }
        public List<ExeSereServ> ExesereServs { get; set; }
        public List<V_HIS_SERE_SERV> sereServsCDHA { get; set; }
        public List<V_HIS_SERE_SERV> sereServsTDCN { get; set; }

        public Mps000013PDO(
            V_HIS_PATIENT patient,
            V_HIS_PATY_ALTER_BHYT patyAlterBhyt,
            HIS_DHST dhsts,
            V_HIS_EXAM_SERVICE_REQ examServiceReqs,
            List<ExeSereServ> ExesereServs,
            List<V_HIS_SERE_SERV> sereServsCDHA,
            List<V_HIS_SERE_SERV> sereServsTDCN
            )
        {
            try
            {
                this.Patient = patient;
                this.PatyAlterBhyt = patyAlterBhyt;
                this.Dhsts = dhsts;
                this.ExamServiceReqs = examServiceReqs;
                this.ExesereServs = ExesereServs;
                this.sereServsCDHA = sereServsCDHA;
                this.sereServsTDCN = sereServsTDCN;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public class ExeSereServ
        {
            public string SERVICE_NAME { get; set; }
            public string TEST_INDEX_UNIT_NAME { get; set; }
            public string TEST_INDEX_RANGE { get; set; }
            public string VALUE { get; set; }
            public string VALUE_RANGE { get; set; }
        }
    }
}
