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
    [XmlType(TypeName = "HHDVu")]
    public class HHDVu
    {
        /// <summary>
        /// Tính chất * (1-Hàng hóa, dịch vụ; 2-Khuyến mại; 3-Chiết khấu thương mại (trong trường hợp muốn thể hiện thông tin chiết khấu theo dòng); 4-Ghi chú/diễn giải)
        /// Mặc định 1
        /// </summary>
        [XmlElement("TChat")]
        public string TChat { get; set; }

        /// <summary>
        /// Số thứ tự
        /// </summary>
        [XmlElement("STT")]
        public string STT { get; set; }

        /// <summary>
        /// Mã hàng hóa, dịch vụ (Bắt buộc nếu có)
        /// </summary>
        [XmlElement("MHHDVu")]
        public string MHHDVu { get; set; }

        /// <summary>
        /// Tên hàng hóa, dịch vụ *
        /// </summary>
        [XmlElement("THHDVu")]
        public string THHDVu { get; set; }

        /// <summary>
        /// Đơn vị tính (Bắt buộc nếu có)
        /// </summary>
        [XmlElement("DVTinh")]
        public string DVTinh { get; set; }

        /// <summary>
        /// Số lượng (Bắt buộc nếu có)
        /// </summary>
        [XmlElement("SLuong")]
        public string SLuong { get; set; }

        /// <summary>
        /// Đơn giá (Bắt buộc nếu có)
        /// </summary>
        [XmlElement("DGia")]
        public string DGia { get; set; }

        /// <summary>
        /// Tỷ lệ % chiết khấu
        /// </summary>
        [XmlElement("TLCKhau")]
        public string TLCKhau { get; set; }

        /// <summary>
        /// Số tiền chiết khấu
        /// </summary>
        [XmlElement("STCKhau")]
        public string STCKhau { get; set; }

        /// <summary>
        /// Thành tiền (Thành tiền chưa có thuế GTGT) - Bắt buộc trừ trường hợp TChat = 4
        /// </summary>
        [XmlElement("ThTien")]
        public string ThTien { get; set; }

        /// <summary>
        /// Thuế suất (Thuế suất thuế GTGT)
        /// </summary>
        [XmlElement("TSuat")]
        public string TSuat { get; set; }

        /// <summary>
        /// Tiền thuế
        /// </summary>
        [XmlElement("TThue")]
        public string TThue { get; set; }

        /// <summary>
        /// Tiền sau thuế
        /// </summary>
        [XmlElement("TSThue")]
        public string TSThue { get; set; }

        /// <summary>
        /// Thông tin khác
        /// </summary>
        [XmlArray("TTKhac")]
        [XmlArrayItem("TTin")]
        public List<TTin> tTin { get; set; }
    }
}
