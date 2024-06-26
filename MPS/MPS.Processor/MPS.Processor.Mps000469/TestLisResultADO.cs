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

namespace MPS.Processor.Mps000469
{
    class TestLisResultADO : V_HIS_SERE_SERV_TEIN
    {
        public long IS_PARENT { get; set; }
        public string SERVICE_NAME { get; set; }
        public string SERVICE_CODE { get; set; }
        public string TEST_INDEX_RANGE { get; set; }
        public decimal? VALUE { get; set; }
        public string VALUE_STR { get; set; }
        public decimal? MIN_VALUE { get; set; }
        public decimal? MAX_VALUE { get; set; }
        public string HIGH_OR_LOW { get; set; }
        public string VALUE_HL { get; set; }
        public string VALUE_HL_NEW { get; set; }
        public string NOTE { get; set; }
        public string PARENT_ID { get; set; }
        public string CHILD_ID { get; set; }
        public short IS_HAS_ONE_CHILD { get; set; }
        public short? IS_NO_EXECUTE { get; set; }
        public long? SERVICE_NUM_ORDER { get; set; }
        public long? SERVICE_ORDER { get; set; }
        public long? SERVICE_PARENT_ORDER { get; set; }
        public decimal? WARNING_MIN_VALUE { get; set; }
        public decimal? WARNING_MAX_VALUE { get; set; }
        public string WARNING_DESCRIPTION { get; set; }
        public string WARNING_NOTE { get; set; }
        public long SERVICE_PARENT_ID { get; set; }
        public string DESCRIPTION_NEW { get; set; }
        public string ISO_PROCESS_CODE { get; set; }
        public short? IS_MEET_ISO_STANDARD { get; set; }
        public string ISO_LOGO_URL { get; set; }
    }
}
