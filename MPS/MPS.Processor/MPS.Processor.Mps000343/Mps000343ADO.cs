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

namespace MPS.Processor.Mps000343
{
    public class Mps000343ADO
    {
        public long? SERVICE_ID { get; set; }
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public decimal AMOUNT { get; set; }
        public decimal PRICE { get; set; }
        public decimal VAT_RATIO { get; set; }
        public decimal PRICE_VAT { get; set; }
        public decimal TOTAL_PRICE { get; set; }

        public Mps000343ADO() { }

        public Mps000343ADO(HIS_SERE_SERV_BILL sereServ, HIS_SERVICE_UNIT serviceUnit)
        {
            this.SERVICE_ID = sereServ.TDL_SERVICE_ID;
            this.SERVICE_CODE = sereServ.TDL_SERVICE_CODE;
            this.SERVICE_NAME = sereServ.TDL_SERVICE_NAME;
            if (serviceUnit != null)
            {
                this.SERVICE_UNIT_NAME = serviceUnit.SERVICE_UNIT_NAME;
            }
            this.PRICE = sereServ.TDL_REAL_PATIENT_PRICE ?? 0;
            this.VAT_RATIO = 0;
            this.PRICE_VAT = sereServ.TDL_REAL_PATIENT_PRICE ?? 0;
            this.AMOUNT = sereServ.TDL_AMOUNT ?? 0;
            this.TOTAL_PRICE = sereServ.PRICE;
        }

        public Mps000343ADO(HIS_BILL_GOODS billGood)
        {
            this.SERVICE_NAME = billGood.GOODS_NAME;
            this.SERVICE_UNIT_NAME = billGood.GOODS_UNIT_NAME;
            this.PRICE = billGood.PRICE;
            this.VAT_RATIO = billGood.VAT_RATIO ?? 0;
            this.PRICE_VAT = billGood.AMOUNT * billGood.PRICE * (1 + (billGood.VAT_RATIO ?? 0));
            this.AMOUNT = billGood.AMOUNT;
            this.TOTAL_PRICE = this.PRICE_VAT * this.AMOUNT;
        }
    }
}
