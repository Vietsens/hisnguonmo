using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace HIS.Desktop.Plugins.EmpUser.XMLData
{
    /// <summary>
    /// Root XML TT12 cho danh mục nhân lực BHXH.
    /// Cấu trúc: HSDANHMUC > DANHSACH_DMNHANLUCKBCB > DMNHANLUCKBCB (repeating)
    /// </summary>
    [XmlRoot("HSDANHMUC")]
    public class XMLEmployeeTT12Data
    {
        /// <summary>
        /// Container chứa danh sách nhân lực.
        /// Sử dụng XmlArray để tạo container DANHSACH_DMNHANLUCKBCB,
        /// XmlArrayItem để định nghĩa element con DMNHANLUCKBCB
        /// </summary>
        [XmlArray("DANHSACH_DMNHANLUCKBCB", Order = 1)]
        [XmlArrayItem("DMNHANLUCKBCB")]
        public List<XMLEmployeeTT12DetailData> DanhMuc { get; set; }

        /// <summary>
        /// Giá trị chữ ký đơn vị (block signature XML).
        /// </summary>
        [XmlElement("CHUKYDONVI", Order = 2)]
        public string ChuKyDonVi { get; set; }
    }
}