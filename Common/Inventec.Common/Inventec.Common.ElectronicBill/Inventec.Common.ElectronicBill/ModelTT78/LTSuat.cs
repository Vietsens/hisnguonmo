using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventec.Common.ElectronicBill.ModelTT78
{
    [XmlType(TypeName = "LTSuat")]
    public class LTSuat
    {
        /// <summary>
        /// Thuế suất (Thuế suất thuế GTGT)
        /// </summary>
        [XmlElement("TSuat")]
        public string TSuat { get; set; }

        /// <summary>
        /// Thành tiền (Thành tiền chưa có thuế GTGT)
        /// </summary>
        [XmlElement("ThTien")]
        public string ThTien { get; set; }

        /// <summary>
        /// Tiền thuế (Tiền thuế GTGT)
        /// </summary>
        [XmlElement("TThue")]
        public string TThue { get; set; }
    }
}
