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
//using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisAccountBookListImport.ADO
{
    class HisAccountBookListImportADO : HIS_ACCOUNT_BOOK
    {
        public string ROOM_CODE { get; set; }
        public long ROOM_TYPE_ID { get; set; }
        //public string ROOM_TYPE_CODE { get; set; }
        public string ERROR { get; set; }
        public decimal? TOTAL_STR { get; set; }
        public decimal? TOTAL_STR_ { get; set; }
        public decimal? FROM_NUM_ORDER_STR { get; set; }
        public decimal? FROM_NUM_ORDER_STR_ { get; set; }
        public string RELEASE_TIME_STR { get; set; }
        public string WORKING_SHIFT_ID_STR { get; set; }
        public string EINVOICE_TYPE_ID_STR { get; set; }
        public string WORKING_SHIFT_ID_STR_ { get; set; }
        public string EINVOICE_TYPE_ID_STR_ { get; set; }

        public string IS_NOT_GEN_TRANSACTION_ORDER_STR { get; set; }
        public string IS_FOR_BILL_STR { get; set; }
        public string IS_FOR_DEPOSIT_STR { get; set; }
        public string IS_FOR_DEBT_STR { get; set; }
        public string IS_FOR_OTHER_SALE_STR { get; set; }
        public string IS_FOR_REPAY_STR { get; set; }
    }
}
