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

namespace MPS.Core.Mps000021
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000021RDO : RDOBase
    {
        internal PatientADO Patient { get; set; }
        internal List<V_HIS_DEPARTMENT_TRAN> lstDepartmentTran { get; set; }
        internal V_HIS_TRAN_PATI TranPaties { get; set; }
        internal V_HIS_SERVICE_REQ ServiceReqPrint { get; set; }
        internal V_HIS_SERE_SERV sereServs { get; set; }
        string bebRoomName;

        public Mps000021RDO(
            PatientADO patient,
            List<V_HIS_DEPARTMENT_TRAN> lstDepartmentTran,
            V_HIS_SERVICE_REQ ServiceReqPrint,
            V_HIS_TRAN_PATI tranPaties,
            V_HIS_SERE_SERV sereServs,
            string bebRoomName
            )
        {
            try
            {
                this.Patient = patient;
                this.lstDepartmentTran = lstDepartmentTran;
                this.ServiceReqPrint = ServiceReqPrint;
                this.TranPaties = tranPaties;
                this.sereServs = sereServs;
                this.bebRoomName = bebRoomName;
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

                if (Patient.DOB > 0)
                {
                    keyValues.Add(new KeyValue(Mps000021ExtendSingleKey.D_O_B, Inventec.Common.DateTime.Convert.TimeNumberToDateString(Patient.DOB_YEAR)));
                }
                else
                {
                    keyValues.Add(new KeyValue(Mps000021ExtendSingleKey.D_O_B, ""));
                }
               
                if (lstDepartmentTran != null && lstDepartmentTran.Count > 0)
                {
                    MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN departmentTran = new MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN();
                    departmentTran = lstDepartmentTran.FirstOrDefault();
                    GlobalQuery.AddObjectKeyIntoListkey<V_HIS_DEPARTMENT_TRAN>(departmentTran, keyValues);
                }

                keyValues.Add(new KeyValue(Mps000021ExtendSingleKey.BED_ROOM_NAME, bebRoomName));
                keyValues.Add(new KeyValue(Mps000021ExtendSingleKey.SERVICE_NAME, sereServs.SERVICE_NAME));

                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_SERE_SERV>(sereServs, keyValues);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(ServiceReqPrint, keyValues);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TRAN_PATI>(TranPaties, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(Patient, keyValues);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
