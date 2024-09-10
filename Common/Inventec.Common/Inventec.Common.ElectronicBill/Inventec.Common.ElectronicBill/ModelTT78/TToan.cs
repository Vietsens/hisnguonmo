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
