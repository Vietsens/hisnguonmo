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

namespace Inventec.Common.ElectronicBillViettel.Model
{
    public class Metadata
    {
        /// <summary>
        /// ID của trường động
        /// Maxlength : 10
        /// </summary>
        public long invoiceCustomFieldId { get; set; }

        /// <summary>
        /// Tên của trường động khi lưu vào dữ liệu
        /// </summary>
        public string keyTag { get; set; }

        /// <summary>
        /// Kiểu dữ liệu của trường động. Chỉ bao gồm các giá trị: “text”,  “date”, “number”
        /// </summary>
        public string valueType { get; set; }

        /// <summary>
        /// Giá trị dữ liệu khi kiểu là text
        /// Maxlength: 13
        /// </summary>
        public string stringValue { get; set; }

        /// <summary>
        /// Giá trị của trường dữ liệu trong trường hợp valueType = date
        /// Đinh dạng: yyyy-MM-ddTHH:mm:sszzz 
        /// </summary>
        public string dateValue { get; set; }

        /// <summary>
        /// Giá trị dữ liệu khi kiểu là number
        /// Maxlength: 6
        /// </summary>
        public long? numberValue { get; set; }

        /// <summary>
        /// Tên hiển thị của trường động, Hiển thị trên giao diện nhập liệu khi lập hóa đơn
        /// </summary>
        public string keyLabel { get; set; }

        /// <summary>
        /// Trường có bắt buộc hay không
        /// </summary>
        public bool? isRequired { get; set; }

        /// <summary>
        /// isSeller = true: Trường dữ liệu thuộc bên bán
        /// isSeller = false: Trường dữ liệu thuộc bên mua
        /// </summary>
        public bool? isSeller { get; set; }
    }
}
