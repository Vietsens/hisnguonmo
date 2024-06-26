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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.ADO
{
    public class SereServGroupPlusSDO
    {
        public string ACCOUNT_BOOK_CODE { get; set; }
        public long? ACCOUNT_BOOK_ID { get; set; }
        public string ACCOUNT_BOOK_NAME { get; set; }
        public string AMOUNT { get; set; } //thay doi
        public string APP_CREATOR { get; set; }
        public string APP_MODIFIER { get; set; }
        public long? BILL_ID { get; set; }
        public string CANCEL_LOGINNAME { get; set; }
        public string CANCEL_REASON { get; set; }
        public long? CANCEL_TIME { get; set; }
        public string CANCEL_USERNAME { get; set; }
        public string CONCLUDE { get; set; }
        public long? CREATE_TIME { get; set; }
        public string CREATOR { get; set; }
        public string DESCRIPTION { get; set; }
        public decimal? EMBED_HEIN_PRICE { get; set; }
        public long? EXECUTE_TIME { get; set; }
        public string GROUP_CODE { get; set; }
        public string HEIN_CARD_NUMBER { get; set; }
        public decimal? HEIN_LIMIT_PRICE { get; set; }
        public decimal? HEIN_LIMIT_RATIO { get; set; }
        public decimal? HEIN_PRICE { get; set; }
        public decimal? HEIN_RATIO { get; set; }
        public long ID { get; set; }
        public long INTRUCTION_TIME { get; set; }
        public short? IS_ACTIVE { get; set; }
        public short? IS_CANCEL { get; set; }
        public short? IS_DELETE { get; set; }
        public short? IS_EXPEND { get; set; }
        public string JSON_HEIN_SERVICE { get; set; }
        public string JSON_PATIENT_TYPE_ALTER { get; set; }
        public string LIBRARY_CODE { get; set; }
        public string MODIFIER { get; set; }
        public long? MODIFY_TIME { get; set; }
        public long? NUM_ORDER { get; set; }
        public long? PARENT_ID { get; set; }
        public string PATIENT_TYPE_CODE { get; set; }
        public long PATIENT_TYPE_ID { get; set; }
        public string PATIENT_TYPE_NAME { get; set; }
        public string PRICE { get; set; } //thay doi
        public long? SERE_SERV_CALENDAR_ID { get; set; }
        public string SERVICE_CODE { get; set; }
        public long SERVICE_ID { get; set; }
        public string SERVICE_NAME { get; set; }
        public long? HEIN_SERVICE_TYPE_ID { get; set; }
        public string SERVICE_REQ_CODE { get; set; }
        public long SERVICE_REQ_ID { get; set; }
        public long SERVICE_REQ_STT_ID { get; set; }
        public string SERVICE_TYPE_CODE { get; set; }
        public long SERVICE_TYPE_ID { get; set; }
        public string SERVICE_TYPE_NAME { get; set; }
        public string SERVICE_UNIT_CODE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public long? TDL_TREATMENT_ID { get; set; }
        public string TRANSACTION_CODE { get; set; }
        public long? TRANSACTION_ID { get; set; }
        public long TREATMENT_ID { get; set; }
        public decimal? VIR_HEIN_PRICE { get; set; }
        public decimal? VIR_PATIENT_PRICE { get; set; }
        public string VIR_PRICE { get; set; } //thay doi
        public decimal? VIR_PRICE_NO_EXPEND { get; set; }
        public string VIR_TOTAL_HEIN_PRICE { get; set; } //thay doi
        public string VIR_TOTAL_PATIENT_PRICE { get; set; } //thay doi
        public string VIR_TOTAL_PRICE { get; set; } //thay doi
        public decimal? VIR_TOTAL_PRICE_NO_EXPEND { get; set; }
        public string VIR_TOTAL_PRICE_OTHER { get; set; } //them moi
        public string EXECUTE_ROOM_NAME { get; set; } //them moi
        public long EXECUTE_ROOM_ID { get; set; } //them moi

    }
}
