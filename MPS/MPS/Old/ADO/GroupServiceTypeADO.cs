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

namespace MPS.ADO
{
    public class GroupServiceTypeADO
    {
        public long SERVICE_TYPE_ID { get; set; }
        public long HEIN_SERVICE_TYPE_ID { get; set; }
        public string HEIN_SERVICE_TYPE_NAME { get; set; }
        public string SERVICE_TYPE_NAME { get; set; }
        public string SERVICE_TYPE_CODE { get; set; }
        public decimal? VIR_HEIN_PRICE { get; set; }
        public decimal? VIR_PATIENT_PRICE { get; set; }
        public decimal? VIR_PRICE { get; set; } //thay doi
        public decimal? VIR_PRICE_NO_EXPEND { get; set; }
        public decimal? VIR_TOTAL_HEIN_PRICE { get; set; } //thay doi
        public decimal? VIR_TOTAL_PATIENT_PRICE { get; set; } //thay doi
        public decimal? VIR_TOTAL_PRICE { get; set; } //thay doi
        public decimal? VIR_TOTAL_PRICE_NO_EXPEND { get; set; }
        public decimal? VIR_TOTAL_PRICE_OTHER { get; set; } //them moi
    }
}
