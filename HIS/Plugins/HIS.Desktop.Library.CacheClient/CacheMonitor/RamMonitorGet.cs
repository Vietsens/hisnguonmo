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
using HIS.Desktop.XmlRamMonitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Library.CacheClient
{
    public class RamMonitorGet
    {
        public RamMonitorGet() { }

        public RamMonitorKeyData GetByCode(string type)
        {
            try
            {
                return HIS.Desktop.XmlRamMonitor.RamMonitorKeyStore.Get().FirstOrDefault(o => o.RamMonitorKeyCode == type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        public List<RamMonitorKeyData> Get()
        {
            try
            {
                return HIS.Desktop.XmlRamMonitor.RamMonitorKeyStore.Get();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        public bool IsExistsCode(string type)
        {
            try
            {
                var xm = HIS.Desktop.XmlRamMonitor.RamMonitorKeyStore.Get().FirstOrDefault(o => o.RamMonitorKeyCode == type);
                return (xm != null && !String.IsNullOrEmpty(xm.RamMonitorKeyCode));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return false;
        }
    }
}
