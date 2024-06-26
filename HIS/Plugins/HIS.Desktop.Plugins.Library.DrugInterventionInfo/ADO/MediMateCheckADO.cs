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

namespace HIS.Desktop.Plugins.Library.DrugInterventionInfo.ADO
{
    public class MediMateCheckADO : V_HIS_MEDICINE_TYPE
    {
        public long? MATERIAL_TYPE_MAP_ID { get; set; }
        public string MATERIAL_TYPE_MAP_CODE { get; set; }
        public string MATERIAL_TYPE_MAP_NAME { get; set; }
        public decimal? AMOUNT { get; set; }
        public long? MAX_REUSE_COUNT { get; set; }//Số lần sử dụng tối đa
        public long? USE_COUNT { get; set; }//Số lần sử dụng
        public long? USE_REMAIN_COUNT { get; set; }//Số lần sử dụng còn lại
        public string Sang { get; set; }
        public string Trua { get; set; }
        public string Chieu { get; set; }
        public string Toi { get; set; }
        public long? UseTimeTo { get; set; }
        public decimal? UseDays { get; set; }

        public MediMateCheckADO()
        {

        }
    }
}
