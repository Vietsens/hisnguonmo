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
        public Mps000209ADO _mps000209Ado = null;
    }

    public class HisBedRoomCouterSDO : V_HIS_BED_ROOM
    {
        public long DEPARTMENT_ID { get; set; }
        public string DEPARTMENT_CODE { get; set; }
        public string DEPARTMENT_NAME { get; set; }
        public decimal TOTAL_TREATMENT_BED_ROOM { get; set; }
    }

    public class Mps000209ADO
    {
        public long? CREATE_TIME { get; set; }
        public string LIST_DEPARTMENT_NAME { get; set; }
        public string ROOM_NAME_PRINT { get; set; }
        public string DEPARTMENT_NAME_PRINT { get; set; }
        public string LOGIN_NAME_PRINT { get; set; }
        public string USERNAME_PRINT { get; set; }
    }
}
