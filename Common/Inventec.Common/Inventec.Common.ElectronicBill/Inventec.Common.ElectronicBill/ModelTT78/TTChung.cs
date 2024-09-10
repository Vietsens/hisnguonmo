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
