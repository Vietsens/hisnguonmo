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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.TreatmentFinish.CloseTreatment
{
    public class TreatmentEndInputADO
    {
        public HIS_TREATMENT Treatment { get; set; }
        public long AppointmentTime { get; set; }
        public MOS.SDO.HisTreatmentFinishSDO HisTreatmentFinishSDO { get; set; }
        public long TreatmentEndAppointmentTimeDefault { get; set; }
        public bool TreatmentEndHasAppointmentTimeDefault { get; set; }
        public List<long> AppointmentNextRoomIds { get; set; }
        public TreatmentEndInputADO()
        {
        }

        public TreatmentEndInputADO(HIS_TREATMENT treatment, long appointmentTime)
            : this()
        {
            this.AppointmentTime = appointmentTime;
            this.Treatment = treatment;
        }
    }
}
