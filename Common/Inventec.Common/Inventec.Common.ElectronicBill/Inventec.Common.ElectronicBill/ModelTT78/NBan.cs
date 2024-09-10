using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventec.Common.ElectronicBill.ModelTT78
{
    [XmlType(TypeName = "NBan")]
    public class NBan
    {
        /// <summary>
        /// Tên *
        /// </summary>
        [XmlElement("Ten")]
        public string Ten { get; set; }

        /// <summary>
        /// Mã số thuế *
        /// </summary>
        [XmlElement("MST")]
        public string MST { get; set; }

        /// <summary>
        /// Địa chỉ *
        /// </summary>
        [XmlElement("DChi")]
        public string DChi { get; set; }

        /// <summary>
        /// Số điện thoại
        /// </summary>
        [XmlElement("SDThoai")]
        public string SDThoai { get; set; }

        /// <summary>
        /// Địa chỉ thư điện tử - email
        /// </summary>
        [XmlElement("DCTDTu")]
        public string DCTDTu { get; set; }

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
        /// Fax
        /// </summary>
        [XmlElement("Fax")]
        public string Fax { get; set; }

        /// <summary>
        /// Lệnh điều động nội bộ (Bắt buộc đối với phiếu xuất kho vận chuyển nội bộ)
        /// </summary>
        [XmlElement("LDDNBo")]
        public string LDDNBo { get; set; }

        /// <summary>
        /// Hợp đồng số (Hợp đồng vận chuyển) (phiếu xuất kho vận chuyển nội bộ)
        /// </summary>
        [XmlElement("HDSo")]
        public string HDSo { get; set; }

        /// <summary>
        /// Họ và tên người xuất hàng (phiếu xuất kho vận chuyển nội bộ)
        /// </summary>
        [XmlElement("HVTNXHang")]
        public string HVTNXHang { get; set; }

        /// <summary>
        /// Tên người vận chuyển (phiếu xuất kho)
        /// </summary>
        [XmlElement("TNVChuyen")]
        public string TNVChuyen { get; set; }

        /// <summary>
        /// Phương tiện vận chuyển (Bắt buộc đối với phiếu xuất kho)
        /// </summary>
        [XmlElement("PTVChuyen")]
        public string PTVChuyen { get; set; }

        /// <summary>
        /// Hợp đồng kinh tế số (Bắt buộc đối với phiếu xuất kho gửi bán đại lý)
        /// </summary>
        [XmlElement("HDKTSo")]
        public string HDKTSo { get; set; }

        /// <summary>
        /// Hợp đồng kinh tế ngày (Bắt buộc đối với phiếu xuất kho gửi bán đại lý)
        /// </summary>
        [XmlElement("HDKTNgay")]
        public string HDKTNgay { get; set; }
    }
}
