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

namespace Inventec.Desktop.ExportXML.QD917.ADO.XML4_ADO
{
    [XmlRoot("DSACH_CHI_TIET_CLS")]
    public class XML4_DSACH_CHI_TIET_CLS_ADO
    {
        [XmlElement("CHI_TIET_CLS")]
        public List<XML4_CHI_TIET_CLS_ADO> CHI_TIET_CLS { get; set; }
    }
}
