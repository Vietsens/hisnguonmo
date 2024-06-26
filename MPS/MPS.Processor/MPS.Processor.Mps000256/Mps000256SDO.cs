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

namespace MPS.Processor.Mps000256
{
    public class Mps000256SDO : MOS.EFMODEL.DataModels.V_HIS_SERVICE
    {
        public string PACKAGE_CODE { get; set; }
        public string PACKAGE_NAME { get; set; }
        public string PTTT_GROUP_CODE { get; set; }
        public string PTTT_GROUP_NAME { get; set; }
        public string PTTT_METHOD_CODE { get; set; }
        public string PTTT_METHOD_NAME { get; set; }
        public decimal? BHYT_PRICE { get; set; }
        public decimal? VP_PRICE { get; set; }
        public string SERVICE_CODE_PARENT { get; set; }
        public string SERVICE_NAME_PARENT { get; set; }
    }
}
