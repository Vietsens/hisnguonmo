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

namespace HIS.Desktop.Plugins.HisMachine.XML
{
    [Serializable]
    public class XMLCLSDetailData
    {
        [XmlElement(Order = 1)]
        public int STT { get; set; }

        [XmlElement(Order = 2)]
        public string MA_CSKCB { get; set; }

        [XmlElement(Order = 3)]
        public XmlCDataSection TEN_TB { get; set; }

        [XmlElement(Order = 4)]
        public string KY_HIEU { get; set; }

        [XmlElement(Order = 5)]
        public XmlCDataSection CONGTY_SX { get; set; }

        [XmlElement(Order = 6)]
        public XmlCDataSection NUOC_SX { get; set; }

        [XmlElement(Order = 7)]
        public string NAM_SX { get; set; }

        [XmlElement(Order = 8)]
        public string NAM_SD { get; set; }

        [XmlElement(Order = 9)]
        public string MA_MAY { get; set; }

        [XmlElement(Order = 10)]
        public string SO_LUU_HANH { get; set; }
    }
}
