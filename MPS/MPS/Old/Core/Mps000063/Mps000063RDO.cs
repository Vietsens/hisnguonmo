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

namespace MPS.Core.Mps000063
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000063RDO : RDOBase
    {
        internal PatientADO Patient { get; set; }
        internal List<V_HIS_DEPARTMENT_TRAN> lstDepartmentTran { get; set; }
        internal V_HIS_REHA_SUM hisRehaSumRow { get; set; }
        internal List<MPS.ADO.ExeHisSereServRehaADO> hisSereServRehaADOs { get; set; }
        internal MOS.EFMODEL.DataModels.V_HIS_REHA_SERVICE_REQ VHisRehaServiceReq { get; set; }
        string bedRoomName;

        public Mps000063RDO(
            PatientADO patient,
            List<V_HIS_DEPARTMENT_TRAN> lstDepartmentTran,
            V_HIS_REHA_SUM hisRehaSumRow,
            List<MPS.ADO.ExeHisSereServRehaADO> hisSereServRehaADOs,
            string bedRoomName
            )
        {
            try
            {
                this.Patient = patient;
                this.lstDepartmentTran = lstDepartmentTran;
                this.hisRehaSumRow = hisRehaSumRow;
                this.hisSereServRehaADOs = hisSereServRehaADOs;
                this.bedRoomName = bedRoomName;
                this.VHisRehaServiceReq = VHisRehaServiceReq;
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
                if (lstDepartmentTran != null && lstDepartmentTran.Count > 0)
                {
                    if (lstDepartmentTran[0].IN_OUT == IMSys.DbConfig.HIS_RS.HIS_DEPARTMENT_TRAN.IN_OUT__IN)
                    {
                        keyValues.Add(new KeyValue(Mps000063ExtendSingleKey.DEPARTMENT_NAME_CLOSE, lstDepartmentTran[0].DEPARTMENT_NAME));
                    }
                    else
                    {
                        keyValues.Add(new KeyValue(Mps000063ExtendSingleKey.DEPARTMENT_NAME_CLOSE, lstDepartmentTran[0].NEXT_DEPARTMENT_NAME));
                    }
                }
                else
                {
                    keyValues.Add(new KeyValue(Mps000063ExtendSingleKey.DEPARTMENT_NAME_CLOSE, ""));
                }

                keyValues.Add(new KeyValue(Mps000063ExtendSingleKey.BED_ROOM_NAME, bedRoomName));
                
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(Patient, keyValues);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_REHA_SUM>(hisRehaSumRow, keyValues, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
