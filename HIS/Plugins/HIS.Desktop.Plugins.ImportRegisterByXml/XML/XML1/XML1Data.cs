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

namespace HIS.Desktop.Plugins.ImportRegisterByXml.XML.XML1
{
     [XmlRoot("TONG_HOP", IsNullable = true)]
    public class XML1Data
    {
        [XmlElement(Order = 1)]
        public string MA_LK { get; set; }

        [XmlElement(Order = 2)]
        public int STT { get; set; }

        [XmlElement(Order = 3)]
        public string MA_BN { get; set; }

        [XmlElement(Order = 4)]
        public XmlCDataSection HO_TEN { get; set; }

        [XmlElement(Order = 5)]
        public string NGAY_SINH { get; set; }

        [XmlElement(Order = 6)]
        public int GIOI_TINH { get; set; }

        [XmlElement(Order = 7)]
        public XmlCDataSection DIA_CHI { get; set; }

        [XmlElement(Order = 8)]
        public string MA_THE { get; set; }

        [XmlElement(Order = 9)]
        public string MA_DKBD { get; set; }

        [XmlElement(Order = 10)]
        public string GT_THE_TU { get; set; }

        [XmlElement(Order = 11)]
        public string GT_THE_DEN { get; set; }

        [XmlElement(Order = 12)]
        public string MIEN_CUNG_CT { get; set; }

        [XmlElement(Order = 13)]
        public XmlCDataSection TEN_BENH { get; set; }

        [XmlElement(Order = 14)]
        public string MA_BENH { get; set; }

        [XmlElement(Order = 15)]
        public string MA_BENHKHAC { get; set; }

        [XmlElement(Order = 16)]
        public int MA_LYDO_VVIEN { get; set; }

        [XmlElement(Order = 17)]
        public string MA_NOI_CHUYEN { get; set; }

        [XmlElement(Order = 18)]
        public string MA_TAI_NAN { get; set; }

        [XmlElement(Order = 19)]
        public string NGAY_VAO { get; set; }

        [XmlElement(Order = 20)]
        public string NGAY_RA { get; set; }

        [XmlElement(Order = 21)]
        public string SO_NGAY_DTRI { get; set; }

        [XmlElement(Order = 22)]
        public int KET_QUA_DTRI { get; set; }

        [XmlElement(Order = 23)]
        public int TINH_TRANG_RV { get; set; }

        [XmlElement(Order = 24)]
        public string NGAY_TTOAN { get; set; }

        [XmlElement(Order = 25)]
        public string T_THUOC { get; set; }

        [XmlElement(Order = 26)]
        public string T_VTYT { get; set; }

        [XmlElement(Order = 27)]
        public string T_TONGCHI { get; set; }

        [XmlElement(Order = 28)]
        public string T_BNTT { get; set; }

        [XmlElement(Order = 29)]
        public string T_BNCCT { get; set; }

        [XmlElement(Order = 30)]
        public string T_BHTT { get; set; }

        [XmlElement(Order = 31)]
        public string T_NGUONKHAC { get; set; }

        [XmlElement(Order = 32)]
        public string T_NGOAIDS { get; set; }

        [XmlElement(Order = 33)]
        public int NAM_QT { get; set; }

        [XmlElement(Order = 34)]
        public int THANG_QT { get; set; }

        [XmlElement(Order = 35)]
        public int MA_LOAI_KCB { get; set; }

        [XmlElement(Order = 36)]
        public string MA_KHOA { get; set; }

        [XmlElement(Order = 37)]
        public string MA_CSKCB { get; set; }

        [XmlElement(Order = 38)]
        public string MA_KHUVUC { get; set; }

        [XmlElement(Order = 39)]
        public string MA_PTTT_QT { get; set; }

        [XmlElement(Order = 40)]
        public string CAN_NANG { get; set; }
    }
}
