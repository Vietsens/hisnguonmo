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
    [XmlType(TypeName = "TToan")]
    public class TToan
    {
        /// <summary>
        /// Thông tin thuế suất
        /// </summary>
        [XmlArray("THTTLTSuat")]
        [XmlArrayItem("LTSuat")]
        public List<LTSuat> lTSuat { get; set; }

        /// <summary>
        /// Tổng tiền chưa thuế (Tổng cộng thành tiền chưa có thuế GTGT) (Bắt buộc với hóa đơn GTGT)
        /// </summary>
        [XmlElement("TgTCThue")]
        public string TgTCThue { get; set; }

        /// <summary>
        /// Tổng tiền thuế (Tổng cộng tiền thuế GTGT) (Bắt buộc với hóa đơn GTGT)
        /// </summary>
        [XmlElement("TgTThue")]
        public string TgTThue { get; set; }

        /// <summary>
        /// Tổng tiền chiết khấu thương mại
        /// </summary>
        [XmlElement("TTCKTMai")]
        public string TTCKTMai { get; set; }

        /// <summary>
        /// Tổng tiền thanh toán bằng số *
        /// </summary>
        [XmlElement("TgTTTBSo")]
        public string TgTTTBSo { get; set; }

        /// <summary>
        /// Tổng tiền thanh toán bằng chữ *
        /// </summary>
        [XmlElement("TgTTTBChu")]
        public string TgTTTBChu { get; set; }
    }
}
