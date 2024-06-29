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

namespace HIS.Desktop.Plugins.BidUpdate.ADO
{
    public class MaterialTypeADO: MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE
    {
        public long IdRow { get; set; }
        public decimal? AMOUNT { get; set; }
        public decimal? ADJUST_AMOUNT { get; set; }
        public long Type { get; set; }
        public string BID_NUM_ORDER { get; set; }
        public string BID_GROUP_CODE { get; set; }
        public string BID_PACKAGE_CODE { get; set; }
        public bool IsMaterialTypeMap { get; set; }


        public long? MONTH_LIFESPAN { get; set; }
        public long? DAY_LIFESPAN { get; set; }
        public long? HOUR_LIFESPAN { get; set; }

        public string NOTE { get; set; }

        public string BID_MATERIAL_TYPE_CODE { get; set; }
        public string BID_MATERIAL_TYPE_NAME { get; set; }
    }
}
