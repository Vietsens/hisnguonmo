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

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.BKAV
{
    class UserDefineADO
    {
        public string DEPARTMENT_CODE { get; set; }
        public string DEPARTMENT_NAME { get; set; }
        public string END_CODE { get; set; }
        public string EXTRA_END_CODE { get; set; }
        public string MAIN_CAUSE { get; set; }
        public string OUT_CODE { get; set; }
        public string STORE_CODE { get; set; }
        public decimal? TOTAL_BILL_AMOUNT { get; set; }
        public decimal? TOTAL_BILL_EXEMPTION { get; set; }
        public decimal? TOTAL_BILL_FUND { get; set; }
        public decimal? TOTAL_BILL_OTHER_AMOUNT { get; set; }
        public decimal? TOTAL_BILL_TRANSFER_AMOUNT { get; set; }
        public decimal? TOTAL_DEBT_AMOUNT { get; set; }
        public decimal? TOTAL_DEPOSIT_AMOUNT { get; set; }
        public decimal? TOTAL_DISCOUNT { get; set; }
        public decimal? TOTAL_HEIN_PRICE { get; set; }
        public decimal? TOTAL_PATIENT_PRICE { get; set; }
        public decimal? TOTAL_PRICE { get; set; }
        public decimal? TOTAL_PRICE_EXPEND { get; set; }
        public decimal? TOTAL_REPAY_AMOUNT { get; set; }
        public string TREATMENT_CODE { get; set; }
        public decimal? TREATMENT_DAY_COUNT { get; set; }
        public string Khoa { get; set; }
    }
}
