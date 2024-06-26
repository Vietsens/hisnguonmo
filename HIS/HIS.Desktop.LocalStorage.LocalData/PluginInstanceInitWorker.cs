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

namespace HIS.Desktop.LocalStorage.LocalData
{
    public class PluginInstanceInitADO
    {
        public PluginInstanceInitADO() { }
        public string PluginName { get; set; }
        public object PluginInstance { get; set; }
        public bool IsUsed { get; set; }
    }

    public class PluginInstanceInitWorker
    {
        static Dictionary<string, PluginInstanceInitADO> DicPluginInstance { get; set; }

        public static bool AddPlugin(string pluginName, object pluginInstance, bool isUsed)
        {
            bool success = false;
            try
            {
                DicPluginInstance = DicPluginInstance == null ? new Dictionary<string, PluginInstanceInitADO>() : DicPluginInstance;
                DicPluginInstance.Add(pluginName, new PluginInstanceInitADO() { PluginName = pluginName, PluginInstance = pluginInstance, IsUsed = isUsed });
                success = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return success;
        }

        public static PluginInstanceInitADO GetAvailaiblePlugin(string pluginNameFilter)
        {
            PluginInstanceInitADO pluginInstance = null;
            try
            {
                foreach (var item in DicPluginInstance)
                {
                    if (item.Value.PluginName.Contains(pluginNameFilter) && item.Value.IsUsed == false)
                    {
                        pluginInstance = item.Value;
                        return pluginInstance;
                    }
                }
            }
            catch (Exception ex)
            {
                pluginInstance = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return pluginInstance;
        }

        public static bool UpdatePlugin(string pluginName, object pluginInstance, bool isUsed)
        {
            bool success = false;
            try
            {
                DicPluginInstance = DicPluginInstance == null ? new Dictionary<string, PluginInstanceInitADO>() : DicPluginInstance;
                PluginInstanceInitADO oldPlugin = null;
                if (DicPluginInstance.TryGetValue(pluginName, out oldPlugin))
                {
                    oldPlugin.PluginInstance = pluginInstance;
                    oldPlugin.IsUsed = isUsed;
                    DicPluginInstance[pluginName] = oldPlugin;
                    success = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return success;
        }

        public static bool IsAvailaiblePlugin(string pluginName)
        {
            return DicPluginInstance != null && DicPluginInstance.ContainsKey(pluginName) && DicPluginInstance[pluginName].IsUsed == false;
        }
    }
}
