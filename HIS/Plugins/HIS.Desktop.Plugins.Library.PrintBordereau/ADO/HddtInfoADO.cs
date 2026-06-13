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

namespace HIS.Desktop.Plugins.Library.PrintBordereau.ADO
{
    /// <summary>
    /// HDDT (hóa đơn điện tử) info forwarded from the payment plugins (TransactionBill /
    /// TransactionList) down to the bordereau template so the attached PDF shows the link
    /// to the electronic invoice on the VNPT portal (PTTK 2724 - mục 3.3).
    /// Only carries the 2 fields the HDDT template renders.
    /// </summary>
    public class HddtInfoADO
    {
        /// <summary>
        /// Electronic invoice number issued by VNPT (e.g. "18487").
        /// Rendered as "Kèm theo số hóa đơn: {N}".
        /// </summary>
        public string InvoiceNumOrder { get; set; }

        /// <summary>
        /// Electronic invoice issue time, format yyyyMMddHHmmss.
        /// Rendered as "Ngày DD tháng MM năm YYYY".
        /// </summary>
        public long? InvoiceTime { get; set; }
    }
}
