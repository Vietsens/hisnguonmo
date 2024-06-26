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
using MPS.ProcessorBase.Core;

namespace MPS.Processor.Mps000021.PDO
{
    public partial class Mps000021PDO : RDOBase
    {
        public string bebRoomName;
        public string genderCode__Male;
        public string genderCode__FeMale;

        public Mps000021PDO(
            V_HIS_PATIENT patient,
            List<V_HIS_DEPARTMENT_TRAN> lstDepartmentTran,
            V_HIS_SERVICE_REQ ServiceReqPrint,
            V_HIS_TRAN_PATI tranPaties,
            V_HIS_SERE_SERV sereServs,
            string bebRoomName,
            string genderCode__Male,
            string genderCode__FeMale
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
                this.genderCode__Male = genderCode__Male;
                this.genderCode__FeMale = genderCode__FeMale;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public Mps000021PDO(
            V_HIS_PATIENT patient,
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
    }
}
