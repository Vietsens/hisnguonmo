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
using Inventec.Common.Xml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace HIS.Desktop.XmlCacheMonitor
{
    [XmlRootAttribute("CacheMonitorKeyLoadConfig", Namespace = "", IsNullable = false)]
    public class CacheMonitorKeyLoadConfig
    {
        [XmlAttributeAttribute(DataType = "date")]
        public DateTime DateTimeValue;

        // Serializes an ArrayList as a "CacheMonitorKeyList" array of XML elements of custom type CacheMonitorKeyData named "CacheMonitorKeyData".
        [XmlArray("CacheMonitorKeyList"), XmlArrayItem("CacheMonitorKeyData", typeof(CacheMonitorKeyData))]
        public ArrayList CacheMonitorKeyList = new ArrayList();
    }
}
