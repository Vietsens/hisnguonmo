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

namespace HIS.Desktop.Plugins.HisMachine.XML
{
    [XmlRoot("DanhSachMayCls")]
    public class XMLCLSData
    {
        [XmlElement("MayCls")]
        public List<XMLCLSDetailData> MayCls { get; set; }
    }
    [XmlRoot("DSACH_TBYTTHDV")]
    public class XMLTT12Data
    {
        [XmlElement("DM_TBYTTHDV", Order = 1)]
        public List<XMLTT12DetailData> DanhMuc { get; set; }

        [XmlElement("CHUKYDONVI", Order = 2)]
        public string ChuKyDonVi { get; set; }
    }
}
