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

using MPS;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ADO;

namespace MPS.Core.Mps000013
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000013RDO : RDOBase
    {
        internal PatyAlterBhytADO PatyAlterBhyt { get; set; }
        internal PatientADO Patient { get; set; }
        internal V_HIS_DEPARTMENT_TRAN departmentTran { get; set; }
        internal HIS_DHST Dhsts { get; set; }
        internal V_HIS_PATIENT_TYPE_ALTER currentHispatientTypeAlter { get; set; }
        internal V_HIS_EXAM_SERVICE_REQ ExamServiceReqs { get; set; }
        internal V_HIS_TRAN_PATI TranPaties { get; set; }
        internal V_HIS_SERE_SERV sereServs { get; set; }
        internal List<MPS.ADO.ExeSereServ> ExesereServs { get; set; }
        internal List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV> sereServsCDHA { get; set; }
        internal List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV> sereServsTDCN { get; set; }

        public Mps000013RDO(
            PatientADO patient,
            PatyAlterBhytADO patyAlterBhyt,
            HIS_DHST dhsts,
            V_HIS_EXAM_SERVICE_REQ examServiceReqs,
            List<MPS.ADO.ExeSereServ> ExesereServs,
            List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV> sereServsCDHA,
            List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV> sereServsTDCN
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

        internal override void SetSingleKey()
        {
            try
            {
                if (Dhsts != null)
                {
                    GlobalQuery.AddObjectKeyIntoListkey<HIS_DHST>(Dhsts, keyValues);
                }
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_EXAM_SERVICE_REQ>(ExamServiceReqs, keyValues, false);
                //GlobalQuery.AddObjectKeyIntoListkey<V_HIS_DEPARTMENT_TRAN>(departmentTran, keyValues, false);
                //GlobalQuery.AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(currentHispatientTypeAlter, keyValues, false);
                //GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TRAN_PATI>(TranPaties, keyValues, false);
                //GlobalQuery.AddObjectKeyIntoListkey<V_HIS_SERE_SERV>(sereServs, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<PatyAlterBhytADO>(PatyAlterBhyt, keyValues);
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(Patient, keyValues);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
