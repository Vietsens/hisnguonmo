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

namespace Inventec.Common.BankQrCode.Provider.BIDV.Model
{
    public class BidvInfoData
    {
        public string PayLoad { get; set; }
        public string PointOTMethod { get; set; }
        public string Guid { get; set; }
        public string MerchantCode { get; set; }
        public string MCC { get; set; }
        public string MerchantName { get; set; }
        public string MerchantCity { get; set; }
        public string Ccy { get; set; }
        public string CountryCode { get; set; }
        public string TerminalLabel { get; set; }
        public string StoreLabel { get; set; }
        public string PostalCode { get; set; }
    }
}
