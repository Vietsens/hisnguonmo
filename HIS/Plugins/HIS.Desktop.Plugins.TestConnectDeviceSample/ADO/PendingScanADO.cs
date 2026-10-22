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

namespace HIS.Desktop.Plugins.TestConnectDeviceSample.ADO
{
    /// <summary>
    /// One barcode read on a port, waiting for the UI thread. ReceivedAt is the sampling time used later.
    /// </summary>
    internal class PendingScanADO
    {
        internal string PortName { get; private set; }
        internal string Code { get; private set; }
        internal DateTime ReceivedAt { get; private set; }

        internal PendingScanADO(string portName, string code, DateTime receivedAt)
        {
            this.PortName = portName;
            this.Code = code;
            this.ReceivedAt = receivedAt;
        }
    }
}
