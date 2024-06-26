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

namespace MPS.Core.Mps000035
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000035RDO : RDOBase
    {
        internal PatientADO Patient { get; set; }
        internal V_HIS_DEPARTMENT_TRAN departmentTran { get; set; }
        internal V_HIS_SERVICE_REQ ServiceReqPrint { get; set; }
        internal V_HIS_SERE_SERV_PTTT sereServsPttt { get; set; }
        internal V_HIS_TREATMENT vHisTreatment { get; set; }

        public Mps000035RDO(
            PatientADO patient,
            V_HIS_DEPARTMENT_TRAN departmentTran,
            V_HIS_SERVICE_REQ ServiceReqPrint,
            V_HIS_TREATMENT vHisTreatment
            )
        {
            try
            {
                this.Patient = patient;
                this.departmentTran = departmentTran;
                this.ServiceReqPrint = ServiceReqPrint;
                this.vHisTreatment = vHisTreatment;
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
                //if (ServiceReqPrint!=null)
                //{
                //    keyValues.Add(new KeyValue(Mps000035ExtendSingleKey.OPEN_DATE_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ServiceReqPrint.LOCK_TIME ??0)));
                //keyValues.Add(new KeyValue(Mps000035ExtendSingleKey.START_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ServiceReqPrint.START_TIME ?? 0)));
                //}
                //else
                //{

                //    keyValues.Add(new KeyValue(Mps000035ExtendSingleKey.OPEN_DATE_SEPARATE_STR, ""));
                //    keyValues.Add(new KeyValue(Mps000035ExtendSingleKey.START_TIME_STR, ""));
                //}
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TREATMENT>(vHisTreatment, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_DEPARTMENT_TRAN>(departmentTran, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(ServiceReqPrint, keyValues,false);
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(Patient, keyValues);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
