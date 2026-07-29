using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace HIS.Desktop.Plugins.HisImportXmlAdjust.XML
{
    [XmlRoot("HOSO_DIEUCHINH_GD")]
    public class XmlHoSoDieuChinhGD
    {
        [XmlElement("TT_HOSO", Order = 1)]
        public List<XmlTTHoSo> TT_HOSO { get; set; }

        [XmlElement("CHUKYDONVI", Order = 2)]
        public string ChuKyDonVi { get; set; }
    }

    public class XmlTTHoSo
    {
        [XmlAttribute("Id")]
        public string Id { get; set; }

        [XmlElement("TT_MAU", Order = 1)]
        public XmlTTMau TT_MAU { get; set; }

        [XmlElement("TT_XML1", Order = 2)]
        public XmlTTXml1 TT_XML1 { get; set; }

        [XmlElement("TT_DIEUCHINH", Order = 3)]
        public XmlTTDieuChinh TT_DIEUCHINH { get; set; }
    }

    public class XmlTTMau
    {
        [XmlElement("MAU_SO")]
        public string MAU_SO { get; set; }

        [XmlElement("MA_CSKCB")]
        public string MA_CSKCB { get; set; }

        [XmlElement("NGUOILAPBIEU")]
        public string NGUOILAPBIEU { get; set; }

        [XmlElement("THUTRUONG_DV")]
        public string THUTRUONG_DV { get; set; }

        [XmlElement("NGAYTHANGNAM")]
        public string NGAYTHANGNAM { get; set; }
    }

    public class XmlTTXml1
    {
        [XmlElement("XML1_ID")]
        public string XML1_ID { get; set; }

        [XmlElement("MA_LK")]
        public string MA_LK { get; set; }

        [XmlElement("MA_BN")]
        public string MA_BN { get; set; }

        [XmlElement("HO_TEN")]
        public string HO_TEN { get; set; }

        [XmlElement("MA_THE")]
        public string MA_THE { get; set; }

        [XmlElement("NGAY_VAO")]
        public string NGAY_VAO { get; set; }

        [XmlElement("NGAY_RA")]
        public string NGAY_RA { get; set; }

        [XmlElement("KY_QT")]
        public string KY_QT { get; set; }

        [XmlElement("TRANGTHAI")]
        public string TRANGTHAI { get; set; }
    }

    public class XmlTTDieuChinh
    {
        [XmlElement("DS_XML1_DIEUCHINH", Order = 1)]
        public XmlDsXml1DieuChinh DS_XML1_DIEUCHINH { get; set; }

        [XmlElement("DSCP_DIEUCHINH", Order = 2)]
        public XmlDsCpDieuChinh DSCP_DIEUCHINH { get; set; }
    }

    public class XmlDsXml1DieuChinh
    {
        [XmlElement("TT_XML1_DC")]
        public List<XmlTTXml1DC> Items { get; set; }
    }

    public class XmlTTXml1DC
    {
        [XmlElement("STT")]
        public string STT { get; set; }

        [XmlElement("TRUONG_TT_GOC")]
        public string TRUONG_TT_GOC { get; set; }

        [XmlElement("TT_GOC")]
        public string TT_GOC { get; set; }

        [XmlElement("TRUONG_TT_DIEUCHINH")]
        public string TRUONG_TT_DIEUCHINH { get; set; }

        [XmlElement("TT_DIEUCHINH")]
        public string TT_DIEUCHINH { get; set; }

        [XmlElement("LYDO_DIEUCHINH")]
        public string LYDO_DIEUCHINH { get; set; }
    }

    public class XmlDsCpDieuChinh
    {
        [XmlElement("CHIPHI")]
        public List<XmlChiPhi> Items { get; set; }
    }

    public class XmlChiPhi
    {
        [XmlElement("STT")]
        public string STT { get; set; }

        [XmlElement("SOBANG_XML")]
        public string SOBANG_XML { get; set; }

        [XmlElement("ID_CP")]
        public string ID_CP { get; set; }

        [XmlElement("STT_XML")]
        public string STT_XML { get; set; }

        [XmlElement("NGAY_YL")]
        public string NGAY_YL { get; set; }

        [XmlElement("TRANGTHAI")]
        public string TRANGTHAI { get; set; }

        [XmlElement("TRUONG_TT_GOC")]
        public string TRUONG_TT_GOC { get; set; }

        [XmlElement("TT_GOC")]
        public string TT_GOC { get; set; }

        [XmlElement("LYDO")]
        public string LYDO { get; set; }

        [XmlElement("TUCHOI")]
        public string TUCHOI { get; set; }

        [XmlElement("TRUONG_TT_DIEUCHINH")]
        public string TRUONG_TT_DIEUCHINH { get; set; }

        [XmlElement("TT_DIEUCHINH")]
        public string TT_DIEUCHINH { get; set; }

        [XmlElement("LYDO_DIEUCHINH")]
        public string LYDO_DIEUCHINH { get; set; }
    }
}
