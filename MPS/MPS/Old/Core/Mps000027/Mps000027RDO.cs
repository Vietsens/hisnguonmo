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

namespace MPS.Core.Mps000027
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000027RDO : RDOBase
    {
        internal PatientADO Patient { get; set; }
        internal List<V_HIS_DEPARTMENT_TRAN> lstDepartmentTran { get; set; }
        internal V_HIS_SERVICE_REQ ServiceReqPrint { get; set; }
        internal decimal ratio { get; set; }

        public Mps000027RDO(
            PatientADO patient,
            List<V_HIS_DEPARTMENT_TRAN> lstDepartmentTran,
            V_HIS_SERVICE_REQ ServiceReqPrint,
            decimal ratio
            )
        {
            try
            {
                this.Patient = patient;
                this.lstDepartmentTran = lstDepartmentTran;
                this.ServiceReqPrint = ServiceReqPrint;
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

                //if (Patient.DOB > 0)
                //{
                //    keyValues.Add(new KeyValue(Mps000027ExtendSingleKey.D_O_B, Inventec.Common.DateTime.Convert.TimeNumberToDateString(Patient.DOB_YEAR)));
                //}
                //else
                //{
                //    keyValues.Add(new KeyValue(Mps000027ExtendSingleKey.D_O_B, ""));
                //}
                //keyValues.Add(new KeyValue(Mps000027ExtendSingleKey.AGE, Patient.AGE));



                
                if (ServiceReqPrint != null)
                {
                    keyValues.Add(new KeyValue(Mps000027ExtendSingleKey.EXECUTE_ROOM_NAME, ServiceReqPrint.EXECUTE_ROOM_NAME));
                    keyValues.Add(new KeyValue(Mps000027ExtendSingleKey.INSTRUCTION_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ServiceReqPrint.INTRUCTION_TIME)));


                    keyValues.Add(new KeyValue(Mps000027ExtendSingleKey.FINISH_TIME_FULL_STR,
                        GlobalQuery.GetCurrentTimeSeparateBeginTime(
                        Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                        ServiceReqPrint.FINISH_TIME ?? 0) ?? DateTime.MinValue)));

                    keyValues.Add(new KeyValue(Mps000027ExtendSingleKey.FINISH_DATE_FULL_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(ServiceReqPrint.FINISH_TIME ?? 0)));

                    keyValues.Add(new KeyValue(Mps000027ExtendSingleKey.INTRUCTION_TIME_FULL_STR,
                        GlobalQuery.GetCurrentTimeSeparateBeginTime(
                        Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                        ServiceReqPrint.INTRUCTION_TIME) ?? DateTime.MinValue)));

                    keyValues.Add(new KeyValue(Mps000027ExtendSingleKey.INTRUCTION_DATE_FULL_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(
                        ServiceReqPrint.INTRUCTION_TIME)));
                }
                else
                {
                    keyValues.Add(new KeyValue(Mps000027ExtendSingleKey.EXECUTE_ROOM_NAME, ""));
                    keyValues.Add(new KeyValue(Mps000027ExtendSingleKey.INSTRUCTION_TIME_STR, ""));
                }

                keyValues.Add(new KeyValue(Mps000027ExtendSingleKey.RATIO, ratio));
                keyValues.Add(new KeyValue(Mps000027ExtendSingleKey.RATIO_STR, (ratio * 100) + "%"));

                if (lstDepartmentTran != null && lstDepartmentTran.Count > 0)
                {
                    MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN departmentTran = new MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN();
                    departmentTran = lstDepartmentTran.FirstOrDefault();
                    GlobalQuery.AddObjectKeyIntoListkey<V_HIS_DEPARTMENT_TRAN>(departmentTran, keyValues, false);
                }
                //GlobalQuery.AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(ServiceReqPrint, keyValues,false);
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(Patient, keyValues);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
