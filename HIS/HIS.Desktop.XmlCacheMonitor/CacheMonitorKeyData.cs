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
using System.Xml.Serialization;
namespace HIS.Desktop.XmlCacheMonitor
{
    [Serializable]
    public class CacheMonitorKeyData
    {
        [XmlAttribute]
        public string CacheMonitorKeyName { get; set; }
        [XmlAttribute]
        public string CacheMonitorKeyCode { get; set; }
        [XmlAttribute]
        public string IsReload { get; set; }
        [XmlAttribute]
        public string Description { get; set; }

        public CacheMonitorKeyData()
        {
        }

        public CacheMonitorKeyData(string heinMediOrgCode, string heinMediOrgName, string isReload)
        {
            this.CacheMonitorKeyName = heinMediOrgName;
            this.CacheMonitorKeyCode = heinMediOrgCode;
            this.IsReload = isReload;
        }
    }
}
