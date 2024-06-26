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

namespace MPS.Processor.Mps000127.PDO
{
    public partial class Mps000127PDO : RDOBase
    {

        public Mps000127PDO(
            V_HIS_PATIENT _patient,
            HIS_PATIENT_TYPE_ALTER _patyAlter,
            List<HIS_SERE_SERV> _sereServKTCs,
            List<HIS_SERE_SERV> _sereServs,
            List<V_HIS_DEPARTMENT_TRAN> _departmentTrans,
            V_HIS_TREATMENT _treatment,
            List<HIS_EXECUTE_GROUP> _executeGroups,
            ServiceTypeCFG _serviceTypeCfg,
            List<HIS_HEIN_SERVICE_TYPE> _heinServiceType,
            List<V_HIS_ROOM> _rooms,
            List<V_HIS_SERVICE> _services,
            SingleKeyValue _singleKeyValue,
            List<V_HIS_EKIP_USER> _EkipUsers,
            List<HIS_SERE_SERV_EXT> _SereServExts,
            List<V_HIS_MATERIAL_TYPE> _MaterialTypes
            )
        {
            try
            {
                this.patient = _patient;
                this.sereServKTCs = _sereServKTCs;
                this.sereServs = _sereServs;
                this.treatment = _treatment;
                this.patyAlter = _patyAlter;
                this.ServiceTypeCfg = _serviceTypeCfg;
                this.departmentTrans = _departmentTrans;
                this.executeGroups = _executeGroups;
                this.Services = _services;
                this.SingleKeyValue = _singleKeyValue;
                this.HeinServiceTypes = _heinServiceType;
                this.Rooms = _rooms;
                this.EkipUsers = _EkipUsers;
                this.SereServExts = _SereServExts;
                this.MaterialTypes = _MaterialTypes;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
