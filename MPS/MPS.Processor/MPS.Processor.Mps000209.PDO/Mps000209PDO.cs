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

namespace MPS.Processor.Mps000209.PDO
{
    public partial class Mps000209PDO : RDOBase
    {
        public List<L_HIS_ROOM_COUNTER> _roomCounters = null;
        public List<HisBedRoomCouterSDO> _bedRoomCounterSdos = null;
        public List<HIS_DEPARTMENT> _departments = null;

        public Mps000209PDO(Mps000209ADO mps000209Ado, List<HisBedRoomCouterSDO> bedRoomCounterSdos, List<L_HIS_ROOM_COUNTER> roomCounters, List<HIS_DEPARTMENT> departments)
        {
            this._bedRoomCounterSdos = bedRoomCounterSdos;
            this._roomCounters = roomCounters;
            this._departments = departments;
            this._mps000209Ado = mps000209Ado;
        }
    }
}
