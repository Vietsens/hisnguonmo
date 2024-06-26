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

namespace HIS.Desktop.Plugins.XMLViewer.XML.XML4
{
    [Serializable]
    public class XML4DetailData
    {
        [XmlElement(Order = 1)]
        public string MA_LK { get; set; }

        [XmlElement(Order = 2)]
        public string STT { get; set; }

        [XmlElement(Order = 3)]
        public string MA_DICH_VU { get; set; }

        [XmlElement(Order = 4)]
        public string MA_CHI_SO { get; set; }

        [XmlElement(Order = 5)]
        public string TEN_CHI_SO { get; set; }

        [XmlElement(Order = 6)]
        public string GIA_TRI { get; set; }

        [XmlElement(Order = 7)]
        public string MA_MAY { get; set; }

        [XmlElement(Order = 8)]
        public string MO_TA { get; set; }

        [XmlElement(Order = 9)]
        public string KET_LUAN { get; set; }

        [XmlElement(Order = 10)]
        public string NGAY_KQ { get; set; }
    }
}
