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
using LIS.EFMODEL.DataModels;

namespace MPS.Core.Mps000096
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000096RDO : RDOBase
    {
        internal PatyAlterBhytADO PatyAlterBhyt { get; set; }
        internal V_HIS_PATIENT_TYPE_ALTER hisPatientTypeAlter { get; set; }
        internal V_HIS_PATIENT_TYPE_ALTER hisPatyAlterBhyt { get; set; }
        internal V_HIS_SERVICE_REQ currentServiceReq { get; set; }
        internal V_HIS_PATIENT currentPatient { get; set; }
        internal List<LIS.EFMODEL.DataModels.V_LIS_RESULT> lstResult { get; set; }
        internal LIS_SAMPLE currentSample { get; set; }

        public Mps000096RDO(
            V_HIS_PATIENT_TYPE_ALTER hisPatientTypeAlter,
            V_HIS_PATIENT_TYPE_ALTER hisPatyAlterBhyt,
            V_HIS_SERVICE_REQ currentServiceReq,
            LIS_SAMPLE currentSample,
            List<LIS.EFMODEL.DataModels.V_LIS_RESULT> lstResult
            )
        {
            try
            {
                this.hisPatientTypeAlter = hisPatientTypeAlter;
                this.hisPatyAlterBhyt = hisPatyAlterBhyt;
                this.currentServiceReq = currentServiceReq;
                this.currentSample = currentSample;
                this.lstResult = lstResult;
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
                
                keyValues.Add(new KeyValue(Mps000096ExtendSingleKey.DOB_STR, currentServiceReq.DOB.ToString().Substring(0,4)));
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(hisPatientTypeAlter, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(hisPatyAlterBhyt, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<LIS_SAMPLE>(currentSample, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(currentServiceReq, keyValues,false);
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
