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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.TreatmentFinish.ADO
{
    public class DataInputADO
    {
        public DataInputADO() { }
        public long? IntructionTime { get; set; }
        public long? UseTime { get; set; }
        public long? UseTimeTo { get; set; }
        public long? SoNgay { get; set; }
        public int ActionType { get; set; }
        public string Advise { get; set; }
        public List<long> AppointmentNextRoomIds { get; set; }
        public string icdCode { get; set; }
        public string icdName { get; set; }
        public string icdSubCode { get; set; }
        public string icdText { get; set; }
        public bool IsBhyt { get; set; }
        public long patientId { get; set; }
    }
}
