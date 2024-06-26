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
using MOS.SDO;

namespace MPS.Core.Mps000034
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000034RDO : RDOBase
    {
        internal HisExamServiceReqResultSDO ExamServiceReqResult { get; set; }
        internal MOS.EFMODEL.DataModels.HIS_DEPARTMENT department { get; set; }
        internal PatientADO patientADO { get; set; }
        internal PatyAlterBhytADO patyAlterBhytADO { get; set; }
        internal MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ lstServiceReq { get; set; }

        public Mps000034RDO(
           HisExamServiceReqResultSDO ExamServiceReqResult,
           MOS.EFMODEL.DataModels.HIS_DEPARTMENT department,
            PatientADO patientADO,
            PatyAlterBhytADO patyAlterBhytADO,
            MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ lstServiceReq
            )
        {
            try
            {
                this.ExamServiceReqResult = ExamServiceReqResult;
                this.department = department;
                this.patientADO = patientADO;
                this.patyAlterBhytADO = patyAlterBhytADO;
                this.lstServiceReq = lstServiceReq;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000034RDO(MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ lstServiceReq)
        {
            this.lstServiceReq = lstServiceReq;
        }

        internal override void SetSingleKey()
        {
            try
            {
                if (ExamServiceReqResult != null)
                {
                    if (ExamServiceReqResult.HisExamServiceReq.INTRUCTION_TIME != null)
                    {
                        keyValues.Add(new KeyValue(Mps000034ExtendSingleKey.REQUEST_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString((ExamServiceReqResult.HisExamServiceReq.INTRUCTION_TIME))));
                    }
                    if (ExamServiceReqResult.HisExamServiceReq.SERVICE_REQ_CODE != null)
                    {
                        keyValues.Add(new KeyValue(Mps000034ExtendSingleKey.SERVICE_REQ_CODE, ExamServiceReqResult.HisExamServiceReq.SERVICE_REQ_CODE));
                    }
                    if (ExamServiceReqResult.HisExamServiceReq.EXECUTE_ROOM_NAME != null)
                    {
                        keyValues.Add(new KeyValue(Mps000034ExtendSingleKey.EXECUTE_ROOM_NAME, ExamServiceReqResult.HisExamServiceReq.EXECUTE_ROOM_NAME));
                    }
                    if (ExamServiceReqResult.HisExamServiceReq.NUM_ORDER != null)
                    {
                        keyValues.Add(new KeyValue(Mps000034ExtendSingleKey.NUM_ORDER, ExamServiceReqResult.HisExamServiceReq.NUM_ORDER));
                    }
                }

                GlobalQuery.AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>(department, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(patientADO, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<PatyAlterBhytADO>(patyAlterBhytADO, keyValues, false);
                if (ExamServiceReqResult == null)
                {
                    if (lstServiceReq.INTRUCTION_TIME != null)
                    {
                        keyValues.Add(new KeyValue(Mps000034ExtendSingleKey.REQUEST_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString((lstServiceReq.INTRUCTION_TIME))));
                    }
                    GlobalQuery.AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>(lstServiceReq, keyValues);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
