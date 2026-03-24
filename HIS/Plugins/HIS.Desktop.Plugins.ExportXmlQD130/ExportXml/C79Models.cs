using System.Xml.Serialization;
using System.Collections.Generic;

[XmlRoot("HSTHC79")]
public class HSTHC79
{
    [XmlElement("DS_CHITIET")]
    public DS_CHITIET DS_CHITIET { get; set; }

    [XmlElement("CHUKYDONVI")]
    public string CHUKYDONVI { get; set; }
}

public class DS_CHITIET
{
    [XmlAttribute("Id")]
    public string Id { get; set; }

    [XmlElement("C79_CHITIET")]
    public List<C79_CHITIET> DanhSachChiTiet { get; set; }
}

public class C79_CHITIET
{
    public string STT { get; set; }
    public string HO_TEN { get; set; }
    public string NGAY_SINH { get; set; }
    public string GIOI_TINH { get; set; }
    public string MA_THE_BHYT { get; set; }
    public string MA_BENH_CHINH { get; set; }
    public string NGAY_VAO { get; set; }
    public string NGAY_VAO_NOI_TRU { get; set; }
    public string NGAY_RA { get; set; }
    public string SO_NGAY_DTRI { get; set; }
    public string MA_LOAI_KCB { get; set; }
    public string T_TONGCHI_BV { get; set; }
    public string T_TONGCHI_BH { get; set; }
    public string T_BHTT { get; set; }
    public string T_BNCCT { get; set; }
    public string T_BNTT { get; set; }
    public string T_NGUONKHAC { get; set; }
    public string MA_CSKCB { get; set; }
    public string NAM_QT { get; set; }
    public string THANG_QT { get; set; }
}