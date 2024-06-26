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

namespace MPS.Processor.Mps000193.PDO
{
    public partial class Mps000193PDO : RDOBase
    {

        public Mps000193PDO(
            V_HIS_PATIENT _patient,
            HIS_PATIENT_TYPE_ALTER _patyAlter,
            long _departmentId,
            string _departmentName,
            List<HIS_SERE_SERV> _sereServKTCs,
            List<HIS_SERE_SERV> _sereServs,
            List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> _departmentTrans,
            V_HIS_TREATMENT _treatment,
            List<V_HIS_SERVICE> _services,
            List<HIS_HEIN_SERVICE_TYPE> _heinServiceTypes,
            List<HIS_EXECUTE_GROUP> _executeGroups,
            List<V_HIS_ROOM> _rooms,
            long _today,
            ServiceTypeCFG _erviceTypeCfg,
            string _userNameReturnResult,
            string _statusTreatmentOut
            )
        {
            try
            {
                this.patient = _patient;
                this.sereServKTCs = _sereServKTCs;
                this.sereServs = _sereServs;
                this.treatment = _treatment;
                this.today = _today;
                this.patyAlter = _patyAlter;
                this.ServiceTypeCfg = _erviceTypeCfg;
                this.departmentName = _departmentName;
                this.departmentTrans = _departmentTrans;
                this.executeGroups = _executeGroups;
                this.departmentId = _departmentId;
                this.userNameReturnResult = _userNameReturnResult;
                this.statusTreatmentOut = _statusTreatmentOut;
                this.Services = _services;
                this.HeinServiceTypes = _heinServiceTypes;
                this.Rooms = _rooms;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
