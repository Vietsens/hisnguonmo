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

namespace MPS.Processor.Mps000275.PDO
{
    public partial class Mps000275PDO : RDOBase
    {
        public List<V_HIS_SERVICE_REQ> _vServiceReqs { get; set; }
        public List<V_HIS_SERE_SERV> _vSereServs { get; set; }
        public List<HIS_SERE_SERV_RATION> _SereServRations { get; set; }
        public List<HIS_SERE_SERV_EXT> _SereServExts { get; set; }
        public List<HIS_PATIENT_TYPE> _ListPatientType { get; set; }
    }

    public class SereServGroupPlusSDO : V_HIS_SERE_SERV
    {
        public string ACCOUNT_BOOK_CODE { get; set; }
        public long? ACCOUNT_BOOK_ID { get; set; }
        public string ACCOUNT_BOOK_NAME { get; set; }
        public long? BILL_ID { get; set; }
        public string CANCEL_LOGINNAME { get; set; }
        public string CANCEL_REASON { get; set; }
        public long? CANCEL_TIME { get; set; }
        public string CANCEL_USERNAME { get; set; }
        public string CONCLUDE { get; set; }
        public string DESCRIPTION { get; set; }
        public decimal? EMBED_HEIN_PRICE { get; set; }
        public long INTRUCTION_TIME { get; set; }
        public short? IS_CANCEL { get; set; }
        public string JSON_HEIN_SERVICE { get; set; }
        public string LIBRARY_CODE { get; set; }
        public long? NUM_ORDER { get; set; }
        public long? SERE_SERV_CALENDAR_ID { get; set; }
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }
        public string SERVICE_REQ_CODE { get; set; }
        public string TRANSACTION_CODE { get; set; }
        public string VIR_TOTAL_PRICE_OTHER { get; set; } //them moi

        public long? BEGIN_TIME { get; set; }
        public long? END_TIME { get; set; }
        public string INSTRUCTION_NOTE { get; set; }
        public string NOTE { get; set; }
    }

    public class SereServRationPlusADO : HIS_SERE_SERV_RATION
    {       
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public decimal? TOTAL_PRICE { get; set; }
        public string PATIENT_TYPE_NAME { get; set; }
    }
}
