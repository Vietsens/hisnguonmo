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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000355
{
    public class HisBillGoodsADO
    {
        public string GOODS_NAME { get; set; }
        public string GOODS_UNIT_NAME { get; set; }
        public decimal? PRICE { get; set; }
        public decimal? TOTAL_PRICE { get; set; }
        public decimal? AMOUNT { get; set; }
        public decimal? DISCOUNT { get; set; }
        public string DESCRIPTION { get; set; }

        public HisBillGoodsADO() { }

        public HisBillGoodsADO(HIS_BILL_GOODS data)
        {
            this.GOODS_NAME = data.GOODS_NAME;
            this.GOODS_UNIT_NAME = data.GOODS_UNIT_NAME;
            this.PRICE = data.PRICE;
            this.AMOUNT = data.AMOUNT;
            this.DISCOUNT = data.DISCOUNT;
            this.DESCRIPTION = data.DESCRIPTION;
            this.TOTAL_PRICE = ((this.AMOUNT ?? 0) * (this.PRICE ?? 0) - (this.DISCOUNT ?? 0));
        }
    }
}
