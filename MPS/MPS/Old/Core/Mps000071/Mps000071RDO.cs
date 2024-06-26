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
using FlexCel.Report;

namespace MPS.Core.Mps000071
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000071RDO : RDOBase
    {
        internal PatientADO patientADO { get; set; }
        internal PatyAlterBhytADO patyAlterBhytADO { get; set; }
        internal List<MPS.ADO.ExeSereServSdo> sereServExamServiceReqs { get; set; }
        internal MOS.EFMODEL.DataModels.V_HIS_EXAM_SERVICE_REQ examServiceReq { get; set; }
        internal V_HIS_SERE_SERV sereServExamSerivceReq { get; set; }
        internal V_HIS_SERVICE_REQ vHisServiceReq { get; set; }
        internal decimal ratio { get; set; }

        public Mps000071RDO(
            PatientADO patientADO,
            PatyAlterBhytADO patyAlterBhytADO,
            List<MPS.ADO.ExeSereServSdo> sereServExamServiceReqs,
            MOS.EFMODEL.DataModels.V_HIS_EXAM_SERVICE_REQ examServiceReq,
            V_HIS_SERE_SERV sereServExamSerivceReq,
            V_HIS_SERVICE_REQ vHisServiceReq,
            decimal ratio
            )
        {
            try
            {
                this.patientADO = patientADO;
                this.sereServExamServiceReqs = sereServExamServiceReqs;
                this.patyAlterBhytADO = patyAlterBhytADO;
                this.examServiceReq = examServiceReq;
                this.sereServExamSerivceReq = sereServExamSerivceReq;
                this.vHisServiceReq = vHisServiceReq;
                this.ratio = ratio;
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
                if (patientADO != null)
                {
                    GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(patientADO, keyValues);
                }

                if (patyAlterBhytADO != null)
                {
                    GlobalQuery.AddObjectKeyIntoListkey<PatyAlterBhytADO>(patyAlterBhytADO, keyValues, false);
                }

                if (examServiceReq != null)
                {
                    GlobalQuery.AddObjectKeyIntoListkey<V_HIS_EXAM_SERVICE_REQ>(examServiceReq, keyValues);
                }
                if (sereServExamSerivceReq != null)
                {
                    GlobalQuery.AddObjectKeyIntoListkey<V_HIS_SERE_SERV>(sereServExamSerivceReq, keyValues, false);
                }
                if (vHisServiceReq != null)
                {
                    keyValues.Add(new KeyValue(Mps000071ExtendSingleKey.FINISH_TIME_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeString(vHisServiceReq.FINISH_TIME ?? 0)));
                    keyValues.Add(new KeyValue(Mps000071ExtendSingleKey.START_TIME_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeString(vHisServiceReq.START_TIME ?? 0)));

                    keyValues.Add(new KeyValue(Mps000071ExtendSingleKey.FINISH_TIME_FULL_STR,
                        GlobalQuery.GetCurrentTimeSeparateBeginTime(
                        Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                        vHisServiceReq.FINISH_TIME ?? 0) ?? DateTime.MinValue)));

                    keyValues.Add(new KeyValue(Mps000071ExtendSingleKey.FINISH_DATE_FULL_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(vHisServiceReq.FINISH_TIME ?? 0)));

                    keyValues.Add(new KeyValue(Mps000071ExtendSingleKey.INTRUCTION_TIME_FULL_STR,
                        GlobalQuery.GetCurrentTimeSeparateBeginTime(
                        Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                        vHisServiceReq.INTRUCTION_TIME) ?? DateTime.MinValue)));

                    keyValues.Add(new KeyValue(Mps000071ExtendSingleKey.INTRUCTION_DATE_FULL_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(
                        vHisServiceReq.INTRUCTION_TIME)));

                    keyValues.Add(new KeyValue(Mps000071ExtendSingleKey.RATIO, ratio));
                    keyValues.Add(new KeyValue(Mps000071ExtendSingleKey.RATIO_STR, (ratio * 100) + "%"));

                    GlobalQuery.AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(vHisServiceReq, keyValues, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
