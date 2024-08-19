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

namespace MPS.Processor.Mps000222
{
    class Mps000222ADO
    {
        public long SERVICE_ID { get; set; }
        public string SERVICE_NAME { get; set; }

        public string TEST_INDEX_CODE_1 { get; set; }
        public string SERVICE_CODE_1 { get; set; }
        public string SERVICE_NAME_1 { get; set; }
        public string TEST_INDEX_UNIT_NAME_1 { get; set; }
        public string TEST_INDEX_RANGE_1 { get; set; }
        public string VALUE_1 { get; set; }
        public string VALUE_RANGE_1 { get; set; }
        public string RESULT_CODE_1 { get; set; }
        public long INTRUCTION_TIME_1 { get; set; }

        public string TEST_INDEX_CODE_2 { get; set; }
        public string SERVICE_CODE_2 { get; set; }
        public string SERVICE_NAME_2 { get; set; }
        public string TEST_INDEX_UNIT_NAME_2 { get; set; }
        public string TEST_INDEX_RANGE_2 { get; set; }
        public string VALUE_2 { get; set; }
        public string VALUE_RANGE_2 { get; set; }
        public string RESULT_CODE_2 { get; set; }
        public long INTRUCTION_TIME_2 { get; set; }

        public string START_TIME_STR { get; set; }
        public string FINISH_TIME_STR { get; set; }
        public string REQUEST_DEPARTMENT_NAME { get; set; }
        public long SERVICE_REQ_ID { get; set; }
        public string VALUE_RANGE { get; set; }
        public string HIGH_OR_LOW { get; set; }

        public decimal? MIN_VALUE { get; set; }
        public decimal? MAX_VALUE { get; set; }

        public string BACTERIUM_NAME { get; set; }
        public string ANTIBIOTIC_RESISTANCE_NAME { get; set; }
        public string ANTIBIOTIC_NAME { get; set; }
        public string SRI_CODE { get; set; }
    }
}
