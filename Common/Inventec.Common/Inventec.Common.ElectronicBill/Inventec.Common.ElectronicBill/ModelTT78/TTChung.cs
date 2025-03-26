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
    [XmlType(TypeName = "TTChung")]
    public class TTChung
    {
        /// <summary>
        /// Số hóa đơn
        /// </summary>
        [XmlElement("SHDon")]
        public string SHDon { get; set; }

        /// <summary>
        /// Mã hồ sơ
        /// </summary>
        [XmlElement("MHSo")]
        public string MHSo { get; set; }

        /// <summary>
        /// Số bảng kê (Số của bảng kê các loại hàng hóa, dịch vụ đã bán kèm theo hóa đơn)
        /// </summary>
        [XmlElement("SBKe")]
        public string SBKe { get; set; }

        /// <summary>
        /// Ngày bảng kê (Ngày của bảng kê các loại hàng hóa, dịch vụ đã bán kèm theo hóa đơn)
        /// </summary>
        [XmlElement("NBKe")]
        public string NBKe { get; set; }

        /// <summary>
        /// Đơn vị tiền tệ *
        /// Mặc định VND
        /// </summary>
        [XmlElement("DVTTe")]
        public string DVTTe { get; set; }

        /// <summary>
        /// Tỷ giá (Bắt buộc (Trừ trường hợp Đơn vị tiền tệ là VND))
        /// </summary>
        [XmlElement("TGia")]
        public string TGia { get; set; }

        /// <summary>
        /// Hình thức thanh toán
        /// </summary>
        [XmlElement("HTTToan")]
        public string HTTToan { get; set; }
    }
}
