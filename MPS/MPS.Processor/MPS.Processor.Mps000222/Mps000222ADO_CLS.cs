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
    class Mps000222ADO_CLS
    {
        public long SERVICE_TYPE_ID { get; set; }

        public long? DIIM_TYPE_ID { get; set; }
        public long? FUEX_TYPE_ID { get; set; }
        public string SERVICE_TYPE_NAME { get; set; }
        public string CONCLUDE { get; set; }

        public long INTRUCTION_TIME { get; set; }

        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }

        public long? BEGIN_TIME { get; set; }
        public long? END_TIME { get; set; }
    }
}
