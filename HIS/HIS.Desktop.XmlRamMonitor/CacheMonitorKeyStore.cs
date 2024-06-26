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
using Inventec.Common.Logging;
using Inventec.Common.Xml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace HIS.Desktop.XmlRamMonitor
{
    public class RamMonitorKeyStore
    {
        private static readonly List<RamMonitorKeyData> RAM_MONITOR_KEY_STORE = InitDataFromXml(XmlRamMonitorConfig.DATA_CONFIG_FILE_PATH);
        static RamMonitorKeyLoadConfig RamMonitorKeyLoadConfig;
        static string XmlFilePath;
        public static List<RamMonitorKeyData> Get()
        {
            return RAM_MONITOR_KEY_STORE;
        }

        public static RamMonitorKeyData GetByCode(string code)
        {
            try
            {
                return code != null && RAM_MONITOR_KEY_STORE != null ?
                    RAM_MONITOR_KEY_STORE.Where(o => o.RamMonitorKeyCode != null && o.RamMonitorKeyCode.Equals(code)).FirstOrDefault() : null;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return null;
            }
        }

        public static bool IsValidCode(string code)
        {
            if (GetByCode(code) == null)
            {
                LogSystem.Error("Ma 'Du lieu cache' khong hop le");
                return false;
            }
            return true;
        }

        private static List<RamMonitorKeyData> InitDataFromXml(string xmlFilePath)
        {
            List<RamMonitorKeyData> result = null;
            try
            {
                XmlFilePath = System.IO.Path.GetFullPath(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, xmlFilePath));
                RamMonitorKeyLoadConfig = ObjectXMLSerializer<RamMonitorKeyLoadConfig>.Load(XmlFilePath);
                result = RamMonitorKeyLoadConfig != null ? RamMonitorKeyLoadConfig.CacheMonitorKeyList.Cast<RamMonitorKeyData>().ToList() : null;
            }
            catch (Exception ex)
            {
                LogSystem.Error("Loi khi load du lieu cache local bao gom cac bang du lieu lon, du lieu dung chung, cac du lieu danh muc it bien dong, cac (phuc vu viec lay du lieu tu local ma khong can goi len server MOS) tu file cau hinh XML", ex);
            }
            return result;
        }
    }
}
