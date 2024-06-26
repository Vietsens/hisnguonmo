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

namespace MPS.Processor.Mps000480
{
    public partial class MEDICINE_MATERIAL
    {
        public long? EXP_MEST_ID { get; set; }
        public string NAME { get; set; }
        public string ACTIVE_INGR_BHYT_NAME { get; set; }
        public string CONCENTRA { get; set; }
        public decimal AMOUNT { get; set; }
        public string NUM_OF_DAYS { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string TUTORIAL { get; set; }
        public string REQ_LOGINNAME { get; set; }
        public string REQ_USERNAME { get; set; }
        public string MORNING { get; set; }
        public string NOON { get; set; }
        public string AFTERNOON { get; set; }
        public string EVENING { get; set; }

        public MEDICINE_MATERIAL() { }
    }
}
