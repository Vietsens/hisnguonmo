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

namespace MPS.Processor.Mps000105.PDO
{
    public partial class Mps000105PDO : RDOBase
    {
        public V_HIS_TRANSACTION _Transaction { get; set; }//K được ghi đè //sx theo thứ tự
        //public V_HIS_BILL _Bill { get; set; }//K được ghi đè
        public V_HIS_SERVICE_REQ _ServiceReq { get; set; }//K được ghi đè
        public V_HIS_PATIENT_TYPE_ALTER _PatyAlterBhyt { get; set; }//K được ghi đè
        public HeinCardADO _HeinCard { get; set; }//K được ghi đè
        public List<V_HIS_SERE_SERV> _ListSereServ = null;
        public HIS_TREATMENT treatment { get; set; }
        public V_HIS_PATIENT patient { get; set; }
        public string ratioText { get; set; }
    }

    public class HeinCardADO
    {
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public string HeinCardNumber { get; set; }
        public string MediOrgCode { get; set; }
        public string Address { get; set; }
        public string RatioText { get; set; }
    }
}
