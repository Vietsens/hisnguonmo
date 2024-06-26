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

namespace HIS.Desktop.Plugins.CompensationByBaseCreate.ADO
{
    public class MetyMatyADO
    {
        public long METY_MATY_ID { get; set; }
        public long TYPE { get; set; }
        public string METY_MATY_CODE { get; set; }
        public string METY_MATY_NAME { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string CONCENTRA { get; set; }
        public string ACTIVE_INGR_BHYT_CODE { get; set; }
        public string ACTIVE_INGR_BHYT_NAME { get; set; }
        public decimal AMOUNT { get; set; }
        public decimal BASE_AMOUNT { get; set; }
        public decimal IN_STOCK_AMOUNT { get; set; }
        public decimal COMPENSATION_AMOUNT { get; set; }
        public bool IsCheck { get; set; }
        public string MEDI_STOCK_NAME { get; set; }
        public decimal? AMOUT_EXP_MEDI_STOCK { get; set; }

    }
}
