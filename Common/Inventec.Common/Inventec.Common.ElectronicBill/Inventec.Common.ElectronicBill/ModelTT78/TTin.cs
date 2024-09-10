using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventec.Common.ElectronicBill.ModelTT78
{
    [XmlType(TypeName = "TTin")]
    public class TTin
    {
        /// <summary>
        /// Tên trường
        /// </summary>
        [XmlElement("TTruong")]
        public string TTruong { get; set; }

        /// <summary>
        /// Kiểu dữ liệu
        /// </summary>
        [XmlElement("KDLieu")]
        public string KDLieu { get; set; }

        /// <summary>
        /// Dữ liệu
        /// </summary>
        [XmlElement("DLieu")]
        public string DLieu { get; set; }
    }
}
