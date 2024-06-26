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
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000343.PDO
{
    public class Mps000343PDO : RDOBase
    {
        public V_HIS_TRANSACTION _Transaction { get; set; }
        public List<HIS_SERE_SERV_BILL> _SereServBills { get; set; }
        public List<HIS_BILL_GOODS> _BillGoods { get; set; }
        public List<HIS_SERVICE_UNIT> _ServiceUnits { get; set; }

        public Mps000343PDO(V_HIS_TRANSACTION transaction, List<HIS_SERE_SERV_BILL> sereServBills, List<HIS_BILL_GOODS> billGoods, List<HIS_SERVICE_UNIT> serviceUnits)
        {
            this._Transaction = transaction;
            this._SereServBills = sereServBills;
            this._BillGoods = billGoods;
            this._ServiceUnits = serviceUnits;
        }
    }
}
