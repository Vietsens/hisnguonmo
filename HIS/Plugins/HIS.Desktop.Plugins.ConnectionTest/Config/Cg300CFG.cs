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
using HIS.Desktop.LocalStorage.ConfigApplication;
using System;

namespace HIS.Desktop.Plugins.ConnectionTest.Config
{
    /// <summary>
    /// Workstation-level (per user) configuration of the CG300 integration.
    /// Only the workstation attached to a CG300 declares these keys.
    /// </summary>
    internal class Cg300CFG
    {
        /// <summary>Exchange folder ("ALL") read by the CG300 software. Empty = this workstation does not use CG300.</summary>
        private const string CONFIG_KEY__CG300_FOLDER = "CONFIG_KEY__HIS_DESKTOP__PLUGINS__CONNECTIONTEST__CG300_FOLDER";
        /// <summary>Process name (without .exe) of the CG300 control software. Empty = do not check the process.</summary>
        private const string CONFIG_KEY__CG300_PROCESS_NAME = "CONFIG_KEY__HIS_DESKTOP__PLUGINS__CONNECTIONTEST__CG300_PROCESS_NAME";

        internal static string FolderPath;
        internal static string ProcessName;

        internal static void LoadConfig()
        {
            try
            {
                FolderPath = GetValue(CONFIG_KEY__CG300_FOLDER);
                ProcessName = GetValue(CONFIG_KEY__CG300_PROCESS_NAME);
                if (!String.IsNullOrWhiteSpace(ProcessName) && ProcessName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                {
                    ProcessName = ProcessName.Substring(0, ProcessName.Length - 4);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private static string GetValue(string key)
        {
            try
            {
                string value = ConfigApplicationWorker.Get<string>(key);
                return value != null ? value.Trim() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }
    }
}
