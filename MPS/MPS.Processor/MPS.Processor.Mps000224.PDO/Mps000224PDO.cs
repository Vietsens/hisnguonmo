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
using MPS.Processor.Mps000224.PDO.Config;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000224.PDO
{
    public partial class Mps000224PDO : RDOBase
    {
        public SingleKeyValue SingleKeyValue { get; set; }
        public long SereServPTTTId { get; set; }
        public List<V_HIS_SERVICE> Services { get; set; }
        public List<V_HIS_ROOM> Rooms { get; set; }
        public List<HIS_HEIN_SERVICE_TYPE> HeinServiceTypes { get; set; }
        public ServiceTypeCFG ServiceTypeCFG { get; set; }

        public Mps000224PDO(
            List<HIS_PATIENT_TYPE_ALTER> _patyAlters,
            long _sereServPTTTId,
            List<HIS_SERE_SERV> _sereServ,
            V_HIS_TREATMENT _treatment,
            List<HIS_TREATMENT_TYPE> _treatmentType,
            List<HIS_HEIN_SERVICE_TYPE> _heinServiceTypes,
            HIS_BRANCH _branch,
            List<V_HIS_ROOM> _rooms,
            List<V_HIS_SERVICE> _services,
            ServiceTypeCFG _serviceTypeCFG,
            SingleKeyValue _singleKeyValue
            )
        {
            try
            {
                this.PatyAlters = _patyAlters;
                this.SereServs = _sereServ;
                this.Treatment = _treatment;
                this.SingleKeyValue = _singleKeyValue;
                this.Services = _services;
                this.Rooms = _rooms;
                this.SereServPTTTId = _sereServPTTTId;
                this.Branch = _branch;
                this.TreatmentType = _treatmentType;
                this.HeinServiceTypes = _heinServiceTypes;
                this.ServiceTypeCFG = _serviceTypeCFG;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
