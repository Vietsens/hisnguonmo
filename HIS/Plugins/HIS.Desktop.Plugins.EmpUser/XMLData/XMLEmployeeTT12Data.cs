using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace HIS.Desktop.Plugins.EmpUser.XMLData
{
    /// <summary>
    /// Root XML TT12 cho danh mục nhân lực BHXH.
    /// Cấu trúc: HSDANHMUC > DANHSACH_DMNHANLUCKBCB (với Id attribute) > DMNHANLUCKBCB (repeating)
    /// </summary>
    [XmlRoot("HSDANHMUC")]
    public class XMLEmployeeTT12Data
    {
        [XmlElement("DANHSACH_DMNHANLUCKBCB", Order = 1)]
        public XMLEmployeeTT12ListData DANHSACH_DMNHANLUCKBCB { get; set; }

        /// <summary>
        /// Giá trị chữ ký đơn vị (block signature XML).
        /// </summary>
        [XmlElement("CHUKYDONVI", Order = 2)]
        public string ChuKyDonVi { get; set; }
    }

    /// <summary>
    /// Wrapper chứa danh sách DMNHANLUCKBCB với attribute Id
    /// </summary>
    public class XMLEmployeeTT12ListData
    {
        [XmlAttribute("Id")]
        public string Id { get; set; }

        [XmlElement("DMNHANLUCKBCB")]
        public List<XMLEmployeeTT12DetailData> DMNHANLUCKBCB { get; set; }
    }
}
