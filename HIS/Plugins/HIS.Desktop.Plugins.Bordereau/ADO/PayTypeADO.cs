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
using HIS.Desktop.Plugins.Library.PrintBordereau.Base;

namespace HIS.Desktop.Plugins.Bordereau.ADO
{
    internal class PayTypeADO
    {
        public string Option { get; set; }
        public PrintOption.PayType Value { get; set; }
        internal PayTypeADO()
        {
        }
        internal List<PayTypeADO> PayTypeADOs
        {
            get
            {
                List<PayTypeADO> result = new List<PayTypeADO>();
                result.Add(new PayTypeADO() { Option = "Tất cả", Value = PrintOption.PayType.ALL });
                result.Add(new PayTypeADO() { Option = "Chưa tạm ứng dịch vụ", Value = PrintOption.PayType.NOT_DEPOSIT });
                result.Add(new PayTypeADO() { Option = "Đã tạm ứng dịch vụ", Value = PrintOption.PayType.DEPOSIT });
                result.Add(new PayTypeADO() { Option = "Chưa thanh toán", Value = PrintOption.PayType.NOT_BILL });
                result.Add(new PayTypeADO() { Option = "Đã thanh toán", Value = PrintOption.PayType.BILL });
                return result;
            }
        }
    }
}
