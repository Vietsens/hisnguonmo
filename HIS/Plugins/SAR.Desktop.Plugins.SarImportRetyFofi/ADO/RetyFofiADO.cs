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

namespace SAR.Desktop.Plugins.SarImportRetyFofi.ADO
{
    public class RetyFofiADO :SAR.EFMODEL.DataModels.SAR_RETY_FOFI
    {
        public string REPORT_TYPE_CODE { get; set; }
        public string REPORT_TYPE_NAME { get; set; }

        public string FORM_FIELD_CODE { get; set; }
        public string ERROR { get; set; }
        public string IS_REQUIRE_STR { get; set; }
        public bool IS_REQUIRE_DISPLAY { get; set; }
        public string NUM_ORDER_STR { get; set; }

        
    }
}
