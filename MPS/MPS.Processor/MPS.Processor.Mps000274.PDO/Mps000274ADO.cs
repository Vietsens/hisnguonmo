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

namespace MPS.Processor.Mps000274.PDO
{
    public class Mps000274ADO
    {
        public Mps000274ADO() { }

        public string REQUEST_DEPARTMENT_NAME { get; set; }
        public string DEATH_CAUSE_NAME { get; set; }
        public string DEATH_WITHIN_CODE { get; set; }
        public string DEATH_WITHIN_NAME { get; set; }
        public string END_DEPARTMENT_CODE { get; set; }
        public string END_DEPARTMENT_NAME { get; set; }
        public string END_ROOM_CODE { get; set; }
        public string END_ROOM_NAME { get; set; }
        public string FEE_LOCK_DEPARTMENT_CODE { get; set; }
        public string FEE_LOCK_DEPARTMENT_NAME { get; set; }
        public string FEE_LOCK_ROOM_CODE { get; set; }
        public string FEE_LOCK_ROOM_NAME { get; set; }
        public string IN_DEPARTMENT_CODE { get; set; }
        public string IN_DEPARTMENT_NAME { get; set; }
        public string IN_ROOM_CODE { get; set; }
        public string IN_ROOM_NAME { get; set; }
        public string TRAN_PATI_FORM_CODE { get; set; }
        public string TRAN_PATI_FORM_NAME { get; set; }
        public string TRAN_PATI_REASON_CODE { get; set; }
        public string TRAN_PATI_REASON_NAME { get; set; }
        public string BRANCH_ADDRESS { get; set; }
    }
}
