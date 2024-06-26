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
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000311.PDO
{
    public partial class Mps000311PDO : RDOBase
    {
        public V_HIS_SERE_SERV_5 SereServ { get; set; }
        public List<V_HIS_SERE_SERV> SereServFollows { get; set; }
        public V_HIS_TREATMENT Treatment { get; set; }
        public List<V_HIS_ROOM> Room { get; set; }
        public List<HIS_PATIENT_TYPE> PatientType { get; set; }
        public List<HIS_SERVICE_UNIT> ServiceUnit { get; set; }

        public Mps000311PDO() { }

        public Mps000311PDO(
           List<V_HIS_SERE_SERV> SereServFollows, V_HIS_SERE_SERV_5 SereServ, V_HIS_TREATMENT Treatment, List<V_HIS_ROOM> Room, List<HIS_SERVICE_UNIT> ServiceUnit, List<HIS_PATIENT_TYPE> PatientType)
        {
            try
            {
                this.SereServFollows = SereServFollows;
                this.SereServ = SereServ;
                this.Treatment = Treatment;
                this.Room = Room;
                this.ServiceUnit = ServiceUnit;
                this.PatientType = PatientType;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
