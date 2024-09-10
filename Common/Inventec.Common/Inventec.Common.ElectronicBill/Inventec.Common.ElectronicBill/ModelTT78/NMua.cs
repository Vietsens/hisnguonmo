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
