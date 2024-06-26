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

namespace HIS.Desktop.Plugins.ImpMestCreate.ADO
{
    public class VHisServicePatyADO : V_HIS_SERVICE_PATY
    {
        //public HIS_MEDICINE_PATY HisMedicinePaty { get; set; }
        //public HIS_MATERIAL_PATY HisMaterialPaty { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsReusable { get; set; }
        public decimal ExpVatRatio { get; set; }
        public decimal PercentProfit { get; set; }
        public decimal ExpPriceVat { get; set; }
        public bool IsNotSell { get; set; }
        public bool IsNotEdit { get; set; }
        public long ServiceId { get; set; }
        public long ServiceTypeId { get; set; }
        public decimal ExpPrice { get; set; }
        public decimal? PRE_PRICE_Str { get; set; }
        public bool IsSetExpPrice { get; set; }
        public VHisServicePatyADO() { }

    }
}
