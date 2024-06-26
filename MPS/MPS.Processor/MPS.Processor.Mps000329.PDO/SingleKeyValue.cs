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

namespace MPS.Processor.Mps000329.PDO
{
    public class SingleKeyValue
    {
        public string SUM_CAPACITY_1 { get; set; }
        public string SUM_CAPACITY_2 { get; set; }
        public string SUM_CAPACITY_3 { get; set; }
        public string SUM_CAPACITY_4 { get; set; }
        public string SUM_CAPACITY_5 { get; set; }
        public decimal SUM_AMOUNT_1 { get; set; }
        public decimal SUM_AMOUNT_2 { get; set; }
        public decimal SUM_AMOUNT_3 { get; set; }
        public decimal SUM_AMOUNT_4 { get; set; }
        public decimal SUM_AMOUNT_5 { get; set; }
        public decimal SUM_AMOUNT { get; set; }
        public long INTRUCTION_DATE { get; set; }
        public string DEPARTMENT_NAME { get; set; }
        public long? DEPARTMENT_ID { get; set; }
    }
}
