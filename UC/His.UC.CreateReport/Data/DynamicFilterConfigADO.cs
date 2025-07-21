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
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His.UC.CreateReport.Data
{
    internal class DynamicFilterConfigADO
    {
        public int? ID { get; set; }
        public string SQL { get; set; }
        public string FormType { get; set; }
        public string DelegateForChangeValueConfig { get; set; }
        public string PropetiesConfig { get; set; }
        public string Title { get; set; }
        public string JSON_OUTPUT { get; set; }
        public bool? IS_REQUIRE { get; set; }
        public long? NUM_ORDER { get; set; }
        public long? HEIGHT { get; set; }
        public short? WIDTH_RATIO { get; set; }
        public long? ROW_COUNT { get; set; }
        public long? COLUMN_COUNT { get; set; }
        public long? ROW_INDEX { get; set; }
        public DynamicFilterConfigADO()
            : base() { }
    }
}
