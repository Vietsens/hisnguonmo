﻿/* IVT
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

namespace HIS.Desktop.Plugins.TransactionBill.ADO
{
    public class InformationBuyerADO
    {
        public string FullName { get; set; }
        public string TaxCode { get; set; }
        public string AccountNumber { get; set; }
        public long? UnitID { get; set; }
        public long? UnitID1 { get; set; }

        public string Address { get; set; }
        public string UnitText { get; set; }
        public string UnitText1 { get; set; }

        public string checkBox { get; set; }
        public string checkBox1 { get; set; }

        public int? BuyerType { get; set; } // 1: Cá nhân, 2: Đơn vị
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string IdentityNumber { get; set; }
        public int? IndentityCode { get; set; }
    }
}
