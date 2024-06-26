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
using LIS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.LisWellPlate.ADO
{
    class HeaderADO
    {
        public int? X { get; set; }
        public int? Y { get; set; }
        public string BARCODE { get; set; }
        public long? SERVICE_RESULT_ID { get; set; }
        public long? SAMPLE_ID { get; set; }
        public string LAST_NAME {get;set;}
        public string FIRST_NAME {get;set;}
        public string GENDER_NAME {get;set;}
        public string DOB {get;set;}
        public long DETAIL_ID { get; set; }
        public HeaderADO()
        {

        }

    }
}
