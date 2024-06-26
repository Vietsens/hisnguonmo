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
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.ADO
{
    public class RepayServiceADO
    {
        public V_HIS_TREATMENT_FEE hisTreatment { get; set; }
        public long? branchId { get; set; }
        public HisTransactionRepaySDO repaySdo { get; set; }
        public long cashierRoomId { get; set; }
        public List<V_HIS_SERE_SERV_5> ListSereServ { get; set; }

        public RepayServiceADO()
        { }

        public RepayServiceADO(V_HIS_TREATMENT_FEE _hisTreatment, HisTransactionRepaySDO _transactionData, long _branchId, long _cashierRoomId, List<V_HIS_SERE_SERV_5> _listSereServ)
        {
            try
            {
                this.hisTreatment = _hisTreatment;
                this.repaySdo = _transactionData;
                this.branchId = _branchId;
                this.cashierRoomId = _cashierRoomId;
                this.ListSereServ = _listSereServ;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
