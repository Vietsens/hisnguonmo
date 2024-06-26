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

namespace MPS.ADO
{
    //SDO phục vụ in đơn thuốc cho BN BHYT ngoại trú
    public class MedicineExpmestTypeADO
    {
        public string MEDICINE_TYPE_NAME { get; set; }
        public string MEDICINE_TYPE_CODE { get; set; }
        public string ACTIVE_INGR_BHYT_NAME { get; set; }
        public string MEDICINE_ACTIVE { get; set; }//hoat chat
        public string MEDICINE_CONCENTRATION { get; set; }//ham luong
        public string AMOUNT { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string TUTORIAL { get; set; }
        public long NUM_ORDER { get; set; }
    }
}
