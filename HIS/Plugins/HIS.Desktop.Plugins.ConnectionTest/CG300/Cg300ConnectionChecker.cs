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
using HIS.Desktop.Plugins.ConnectionTest.Config;
using System;
using System.Diagnostics;
using System.IO;

namespace HIS.Desktop.Plugins.ConnectionTest.CG300
{
    /// <summary>Result of the CG300 connection check.</summary>
    internal enum EnumCg300ConnectState
    {
        /// <summary>Connected: folder configured + writable, CG software running (if configured)</summary>
        Connected = 0,
        /// <summary>Workstation does not declare the CG300 folder - feature not used here</summary>
        NotConfigured = 1,
        /// <summary>Configured folder does not exist</summary>
        FolderNotFound = 2,
        /// <summary>Configured folder is not writable</summary>
        FolderNotWritable = 3,
        /// <summary>CG300 control software is not running</summary>
        ProcessNotRunning = 4
    }

    /// <summary>
    /// Method 2 has no status channel, so "connected" is decided on the workstation itself:
    /// folder configured, folder writable, CG software process running.
    /// </summary>
    internal class Cg300ConnectionChecker
    {
        /// <summary>Probe file extension - must differ from the data file extension so CG never reads it.</summary>
        private const string PROBE_EXTENSION = ".chk";

        internal static EnumCg300ConnectState Check()
        {
            try
            {
                if (String.IsNullOrWhiteSpace(Cg300CFG.FolderPath))
                    return EnumCg300ConnectState.NotConfigured;

                if (!Directory.Exists(Cg300CFG.FolderPath))
                    return EnumCg300ConnectState.FolderNotFound;

                if (!CanWrite(Cg300CFG.FolderPath))
                    return EnumCg300ConnectState.FolderNotWritable;

                if (!String.IsNullOrWhiteSpace(Cg300CFG.ProcessName))
                {
                    Process[] processes = Process.GetProcessesByName(Cg300CFG.ProcessName);
                    bool isRunning = processes != null && processes.Length > 0;
                    if (processes != null)
                    {
                        foreach (var p in processes) p.Dispose();
                    }
                    if (!isRunning)
                        return EnumCg300ConnectState.ProcessNotRunning;
                }

                return EnumCg300ConnectState.Connected;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return EnumCg300ConnectState.FolderNotWritable;
            }
        }

        private static bool CanWrite(string folder)
        {
            string probe = Path.Combine(folder, "HIS_" + Guid.NewGuid().ToString("N") + PROBE_EXTENSION);
            try
            {
                File.WriteAllText(probe, "");
                File.Delete(probe);
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Khong ghi duoc thu muc CG300: " + folder, ex);
                return false;
            }
        }
    }
}
