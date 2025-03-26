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
using System.Xml.Serialization;

namespace Inventec.Desktop.ExportXML.QD917.ADO.XML5_ADO
{
    public class XML5_CHI_TIET_DIEN_BIEN_BENH_ADO
    {
        [XmlElement(Order = 1)]
        public string MA_LK { get; set; }

        [XmlElement(Order = 2)]
        public string STT { get; set; }

        [XmlElement(Order = 3)]
        public string DIEN_BIEN { get; set; }

        [XmlElement(Order = 4)]
        public string HOI_CHAN { get; set; }

        [XmlElement(Order = 5)]
        public string PHAU_THUAT { get; set; }

        [XmlElement(Order = 6)]
        public string NGAY_YL { get; set; }
    }
}
