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

namespace MPS.Processor.Mps000110
{
    class SereServsADO : HIS_SESE_DEPO_REPAY
    {
        public string DEPOSIT_TRANSACTION_CODE { get; set; }
        public string DEPOSIT_ACCOUNT_BOOK_CODE { get; set; }
        public string DEPOSIT_ACCOUNT_BOOK_NAME { get; set; }
        public long DEPOSIT_NUM_ORDER { get; set; }
        public long DEPOSIT_TRANSACTION_TIME { get; set; }
        public string DEPOSIT_TRANSACTION_TIME_STR { get; set; }

        public SereServsADO(HIS_SESE_DEPO_REPAY data, List<HIS_SERE_SERV_DEPOSIT> listSsDeposit, List<V_HIS_TRANSACTION> ListDeposit)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<SereServsADO>(this, data);

                if (listSsDeposit != null && listSsDeposit.Count > 0 && ListDeposit != null && ListDeposit.Count > 0)
                {
                    var deposit = listSsDeposit.FirstOrDefault(o => o.ID == data.SERE_SERV_DEPOSIT_ID);
                    if (deposit != null)
                    {
                        var tran = ListDeposit.FirstOrDefault(o => o.ID == deposit.DEPOSIT_ID);
                        if (tran != null)
                        {
                            this.DEPOSIT_TRANSACTION_CODE = tran.TRANSACTION_CODE;
                            this.DEPOSIT_ACCOUNT_BOOK_CODE = tran.ACCOUNT_BOOK_CODE;
                            this.DEPOSIT_ACCOUNT_BOOK_NAME = tran.ACCOUNT_BOOK_NAME;
                            this.DEPOSIT_NUM_ORDER = tran.NUM_ORDER;
                            this.DEPOSIT_TRANSACTION_TIME = tran.TRANSACTION_TIME;
                            this.DEPOSIT_TRANSACTION_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(tran.TRANSACTION_TIME);
                        }
                    }
                }
            }
        }
    }
}
