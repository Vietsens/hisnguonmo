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
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.Plugins.TestConnectDeviceSample.Resources;
using HIS.Desktop.Plugins.TestConnectDeviceSample.Worker;
using HIS.Desktop.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HIS.Desktop.Plugins.TestConnectDeviceSample
{
    /// <summary>
    /// Opening, closing and losing COM ports; signing the port account out with an audit line.
    /// </summary>
    public partial class UCTestConnectDeviceSample : UserControlBase
    {
        private void ConnectPort(string portName)
        {
            try
            {
                ComPortReader existing;
                if (this.readers.TryGetValue(portName, out existing))
                {
                    if (!this.lostPorts.Contains(portName))
                    {
                        Notify(portName, "", DateTime.Now, String.Format(ResourceMessage.CongDaKetNoi, portName), false);
                        return;
                    }
                    // Scanner plugged back in: drop the dead reader and open a fresh one
                    if (existing != null)
                        existing.Close();
                    this.readers.Remove(portName);
                    this.lostPorts.Remove(portName);
                }

                ComPortReader reader = new ComPortReader(portName, this.OnLineReceived);
                try
                {
                    reader.Open();
                }
                catch (UnauthorizedAccessException ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                    Notify(portName, "", DateTime.Now, String.Format(ResourceMessage.CongDangDuocSuDung, portName), false);
                    return;
                }
                catch (IOException ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                    Notify(portName, "", DateTime.Now, String.Format(ResourceMessage.KhongMoDuocCong, portName), false);
                    return;
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                    Notify(portName, "", DateTime.Now, String.Format(ResourceMessage.KhongMoDuocCongLoiKhac, portName), false);
                    return;
                }

                AttachPort(portName, reader);
                Notify(portName, "", DateTime.Now, String.Format(ResourceMessage.DaKetNoiCong, portName), true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Registers an open port. Internal so that the test harness can attach a port without hardware (reader = null).
        /// </summary>
        internal void AttachPort(string portName, ComPortReader reader)
        {
            try
            {
                this.readers[portName] = reader;
                this.lostPorts.Remove(portName);
                RefreshTiles();
                SelectPort(portName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DisconnectPort(string portName)
        {
            try
            {
                ComPortReader reader;
                if (!this.readers.TryGetValue(portName, out reader))
                {
                    Notify(portName, "", DateTime.Now, String.Format(ResourceMessage.CongChuaKetNoi, portName), false);
                    return;
                }
                if (reader != null)
                    reader.Close();
                this.readers.Remove(portName);
                this.lostPorts.Remove(portName);
                this.lastScanByPort.Remove(portName);
                SignOutPort(portName, ResourceMessage.LyDoNgatKetNoi);
                Notify(portName, "", DateTime.Now, String.Format(ResourceMessage.DaNgatKetNoiCong, portName), true);
                RefreshTiles();
                UpdateButtonState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Unbinds the account of the port (the port itself stays as it is) and writes the audit line.
        /// </summary>
        private void SignOutPort(string portName, string reason)
        {
            try
            {
                foreach (TestDeviceSampleADO session in SessionStore.RemoveByPort(portName))
                {
                    EventLogWorker.Log(String.Format(ResourceMessage.NhatKyDangXuat, portName,
                        FormatUser(session.Username, session.Loginname), reason));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Unplugged scanners: close the reader at once (see ComPortReader) and sign the account out.
        /// </summary>
        private void CheckLostPorts()
        {
            try
            {
                foreach (KeyValuePair<string, ComPortReader> pair in this.readers.ToList())
                {
                    if (pair.Value == null || this.lostPorts.Contains(pair.Key) || pair.Value.IsAlive())
                        continue;

                    pair.Value.Close();
                    this.lostPorts.Add(pair.Key);
                    SignOutPort(pair.Key, ResourceMessage.LyDoMatKetNoi);
                    Notify(pair.Key, "", DateTime.Now, String.Format(ResourceMessage.MatKetNoiCong, pair.Key), false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
