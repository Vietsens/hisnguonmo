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
using System.Xml.Serialization;

namespace Inventec.Common.ElectronicBill.ModelTT78
{
    [XmlType(TypeName = "NMua")]
    public class NMua
    {
        /// <summary>
        /// Tên *
        /// </summary>
        [XmlElement("Ten")]
        public string Ten { get; set; }

        /// <summary>
        /// Mã số thuế (Bắt buộc nếu có)
        /// </summary>
        [XmlElement("MST")]
        public string MST { get; set; }

        /// <summary>
        /// Địa chỉ *
        /// </summary>
        [XmlElement("DChi")]
        public string DChi { get; set; }

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        [XmlElement("MKHang")]
        public string MKHang { get; set; }

        /// <summary>
        /// Số điện thoại
        /// </summary>
        [XmlElement("SDThoai")]
        public string SDThoai { get; set; }

        /// <summary>
        /// Địa chỉ thư điện tử
        /// </summary>
        [XmlElement("DCTDTu")]
        public string DCTDTu { get; set; }

        /// <summary>
        /// Họ và tên người mua hàng
        /// </summary>
        [XmlElement("HVTNMHang")]
        public string HVTNMHang { get; set; }

        /// <summary>
        /// Số tài khoản ngân hàng
        /// </summary>
        [XmlElement("STKNHang")]
        public string STKNHang { get; set; }

        /// <summary>
        /// Tên ngân hàng
        /// </summary>
        [XmlElement("TNHang")]
        public string TNHang { get; set; }

        /// <summary>
        /// Họ và tên người nhận hàng (phiếu xuất kho)
        /// </summary>
        [XmlElement("HVTNNHang")]
        public string HVTNNHang { get; set; }
    }
}
