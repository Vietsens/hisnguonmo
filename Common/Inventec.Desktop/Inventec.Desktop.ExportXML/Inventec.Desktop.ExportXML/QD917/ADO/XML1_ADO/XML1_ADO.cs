/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Inventec.Desktop.ExportXML.QD917.ADO.XML1_ADO
{
    [XmlRoot("TONG_HOP", IsNullable = true)]
    public class XML1_ADO
    {
        [XmlElement(Order = 1)]
        public XmlCDataSection MA_LK { get; set; }

        [XmlElement(Order = 2)]
        public int STT { get; set; }

        [XmlElement(Order = 3)]
        public XmlCDataSection MA_BN { get; set; }

        [XmlElement(Order = 4)]
        public XmlCDataSection HO_TEN { get; set; }

        [XmlElement(Order = 5)]
        public XmlCDataSection NGAY_SINH { get; set; }

        [XmlElement(Order = 6)]
        public int GIOI_TINH { get; set; }

        [XmlElement(Order = 7)]
        public XmlCDataSection DIA_CHI { get; set; }

        [XmlElement(Order = 8)]
        public XmlCDataSection MA_THE { get; set; }

        [XmlElement(Order = 9)]
        public XmlCDataSection MA_DKBD { get; set; }

        [XmlElement(Order = 10)]
        public XmlCDataSection GT_THE_TU { get; set; }

        [XmlElement(Order = 11)]
        public XmlCDataSection GT_THE_DEN { get; set; }

        [XmlElement(Order = 12)]
        public XmlCDataSection TEN_BENH { get; set; }

        [XmlElement(Order = 13)]
        public XmlCDataSection MA_BENH { get; set; }

        [XmlElement(Order = 14)]
        public XmlCDataSection MA_BENHKHAC { get; set; }

        [XmlElement(Order = 15)]
        public int? MA_LYDO_VVIEN { get; set; }

        [XmlElement(Order = 16)]
        public XmlCDataSection MA_NOI_CHUYEN { get; set; }

        [XmlElement(Order = 17)]
        public int? MA_TAI_NAN { get; set; }

        [XmlElement(Order = 18)]
        public XmlCDataSection NGAY_VAO { get; set; }

        [XmlElement(Order = 19)]
        public XmlCDataSection NGAY_RA { get; set; }

        [XmlElement(Order = 20)]
        public int? SO_NGAY_DTRI { get; set; }

        [XmlElement(Order = 21)]
        public int? KET_QUA_DTRI { get; set; }

        [XmlElement(Order = 22)]
        public int? TINH_TRANG_RV { get; set; }

        [XmlElement(Order = 23)]
        public XmlCDataSection NGAY_TTOAN { get; set; }

        [XmlElement(Order = 24)]
        public int MUC_HUONG { get; set; }

        [XmlElement(Order = 25)]
        public decimal T_THUOC { get; set; }

        [XmlElement(Order = 26)]
        public decimal T_VTYT { get; set; }

        [XmlElement(Order = 27)]
        public decimal T_TONGCHI { get; set; }

        [XmlElement(Order = 28)]
        public decimal T_BNTT { get; set; }

        [XmlElement(Order = 29)]
        public decimal T_BHTT { get; set; }

        [XmlElement(Order = 30)]
        public decimal? T_NGUONKHAC { get; set; }

        [XmlElement(Order = 31)]
        public decimal T_NGOAIDS { get; set; }

        [XmlElement(Order = 32)]
        public int? NAM_QT { get; set; }

        [XmlElement(Order = 33)]
        public int? THANG_QT { get; set; }

        [XmlElement(Order = 34)]
        public int? MA_LOAI_KCB { get; set; }

        [XmlElement(Order = 35)]
        public XmlCDataSection MA_KHOA { get; set; }

        [XmlElement(Order = 36)]
        public XmlCDataSection MA_CSKCB { get; set; }

        [XmlElement(Order = 37)]
        public XmlCDataSection MA_KHUVUC { get; set; }

        [XmlElement(Order = 38)]
        public XmlCDataSection MA_PTTT_QT { get; set; }

        [XmlElement(Order = 39)]
        public decimal? CAN_NANG { get; set; }
    }
}
