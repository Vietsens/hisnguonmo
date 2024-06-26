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

namespace HIS.Desktop.Plugins.EmpUser
{
    [XmlRoot("TAI_KHOAN_NHAN_VIEN", IsNullable = true)]
    public class XML
    {
        [XmlElement(Order = 1)]
        public int STT { get; set; }

        [XmlElement(Order = 2)]
        public string MA_CSKCB { get; set; }

        [XmlElement(Order = 3)]
        public XmlCDataSection HO_TEN { get; set; }

        [XmlElement(Order = 4)]
        public string GIOI_TINH { get; set; }

        [XmlElement(Order = 5)] 
        public string MA_DANTOC { get; set; }

        [XmlElement(Order = 6)]
        public string NGAY_SINH { get; set; }

        [XmlElement(Order = 7)]
        public string SO_CCCD { get; set; }

        [XmlElement(Order = 8)]
        public string CHUCDANH_NN { get; set; }

        [XmlElement(Order = 9)]
        public string VI_TRI { get; set; }

        [XmlElement(Order = 10)]
        public XmlCDataSection MA_CCHN { get; set; }

        [XmlElement(Order = 11)]
        public string NGAYCAP_CCHN { get; set; }

        [XmlElement(Order = 12)]
        public XmlCDataSection NOICAP_CCHN { get; set; }

        [XmlElement(Order = 13)]
        public string PHAMVI_CM { get; set; }

        [XmlElement(Order = 14)]
        public string THOIGIAN_DK { get; set; }

        [XmlElement(Order = 15)]
        public string CSKCB_KHAC { get; set; }
    }
}
