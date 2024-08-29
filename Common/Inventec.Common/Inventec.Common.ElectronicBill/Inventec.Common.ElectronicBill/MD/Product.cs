using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventec.Common.ElectronicBill.MD
{
    public class Product
    {
        [XmlElement("ProdName")]
        public string ProdName { get; set; }

        [XmlElement("ProdUnit")]
        public string ProdUnit { get; set; }

        [XmlElement("ProdQuantity")]
        public string ProdQuantity { get; set; }

        [XmlElement("ProdPrice")]
        public string ProdPrice { get; set; }

        [XmlElement("Amount")]
        public string Amount { get; set; }

        [XmlElement("VATRate")]
        public string VATRate { get; set; }

        [XmlElement("VATAmount")]
        public string VATAmount { get; set; }

        [XmlElement("Total")]
        public string Total { get; set; }

        ///// <summary>
        ///// Tính chất * (1-Hàng hóa, dịch vụ; 2-Khuyến mại; 3-Chiết khấu thương mại (trong trường hợp muốn thể hiện thông tin chiết khấu theo dòng); 4-Ghi chú/diễn giải)
        ///// </summary>
        //[XmlElement("IsSum")]
        //public string IsSum { get; set; }
    }
}
