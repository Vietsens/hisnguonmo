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
using HIS.Desktop.Common;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.ADO
{
    public class TransReqQRADO
    {
        public long TreatmentId { get; set; }
        public CreateReqType TransReqId { get; set; }
        public HIS_CONFIG ConfigValue { get; set; }
        public RefeshReference DelegtePrint { get; set; }
        public V_HIS_DEPOSIT_REQ DepositReq { get; set; }
        public HIS_TRANSACTION Transaction { get; set; }
        public List<HIS_TRANSACTION> Transactions { get; set; }
        /// <summary>
        /// BIDV, VCB, Viettinbank, LPBank, PVCB
        /// </summary>
        public string BankName { get; set; }
    }
    public enum CreateReqType
    {
        DepositService,
        TransactionBill,
        Deposit,
        Transaction
    }
}
