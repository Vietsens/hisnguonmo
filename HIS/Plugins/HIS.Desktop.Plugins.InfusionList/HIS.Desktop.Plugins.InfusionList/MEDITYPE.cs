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

namespace HIS.Desktop.Plugins.InfusionCreate
{
     class MEDITYPE
    {
        public long ID { get; set; }
        public long? INSTRUCTION_TIME { get; set; }
        public string INSTRUCTION_TIME_STR { get; set; }
        public string INSTRUCTION_DATE_STR { get; set; }
        public long? MEDICINE_ID { get; set; }
        public string MEDICINE_TYPE_CODE { get; set; }
        public string MEDICINE_TYPE_NAME { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public long? SERVICE_UNIT_ID { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public decimal AMOUNT { get; set; }
        public long? EXPIRED_DATE { get; set; }
        public string EXPIRED_DATE_STR { get; set; }
        public decimal? SPEED { get; set; }
        public string LOGGINNAME { get; set; }
        public Boolean ngoaikho { get; set; }
        public string mediType { get; set; }
    }
}
