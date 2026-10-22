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
using System.Configuration;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace HIS.Desktop.Plugins.TestConnectDeviceSample.Worker
{
    /// <summary>
    /// Reads barcodes from one scanner in serial (COM) mode. Each barcode ends with CR, LF or TAB.
    /// Runs the callback on the serial port thread: callers must marshal to the UI thread.
    /// </summary>
    internal sealed class ComPortReader
    {
        internal delegate void LineReceivedHandler(string portName, string line, DateTime receivedTime);

        private const int DEFAULT_BAUD_RATE = 9600;
        private const int DEFAULT_DATA_BITS = 8;
        private const int MAX_BUFFER_LENGTH = 4096;
        private static readonly char[] LINE_TERMINATORS = new char[] { '\r', '\n', '\t' };

        private readonly object syncRoot = new object();
        private readonly StringBuilder buffer = new StringBuilder();
        private readonly LineReceivedHandler onLineReceived;
        private SerialPort port;
        // Kept to close it ourselves: once the device is unplugged SerialPort.BaseStream throws, and a stream left
        // to the finalizer can raise an IOException there, which would terminate HIS (no global exception handler).
        private Stream baseStream;

        internal string PortName { get; private set; }

        internal ComPortReader(string portName, LineReceivedHandler onLineReceived)
        {
            this.PortName = portName;
            this.onLineReceived = onLineReceived;
        }

        /// <summary>
        /// Opens the port. Throws UnauthorizedAccessException (port in use) or IOException (device missing)
        /// so that the caller can show the matching message.
        /// </summary>
        internal void Open()
        {
            SerialPort newPort = new SerialPort(this.PortName);
            Stream newStream = null;
            try
            {
                newPort.BaudRate = ReadIntSetting("PortConfig.BaudRate", DEFAULT_BAUD_RATE);
                newPort.DataBits = ReadIntSetting("PortConfig.DataBits", DEFAULT_DATA_BITS);
                newPort.Parity = ReadEnumSetting("PortConfig.Parity", Parity.None);
                newPort.StopBits = ReadEnumSetting("PortConfig.StopBits", StopBits.One);
                newPort.Handshake = ReadEnumSetting("PortConfig.Handshake", Handshake.None);
                // Many USB-COM scanners only send data while the host raises DTR/RTS
                newPort.DtrEnable = true;
                newPort.RtsEnable = true;
                newPort.DataReceived += Port_DataReceived;
                newPort.ErrorReceived += Port_ErrorReceived;
                newPort.Open();
                newStream = newPort.BaseStream;
                GC.SuppressFinalize(newStream);
                newPort.DiscardInBuffer();
                this.port = newPort;
                this.baseStream = newStream;
            }
            catch
            {
                CloseSafely(newPort, newStream);
                throw;
            }
        }

        /// <summary>
        /// True while the port is open and the device is still plugged in.
        /// </summary>
        internal bool IsAlive()
        {
            try
            {
                SerialPort current = this.port;
                if (current == null || !current.IsOpen)
                    return false;
                return SerialPort.GetPortNames().Any(o => String.Equals(o, this.PortName, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        internal void Close()
        {
            try
            {
                SerialPort current = this.port;
                Stream currentStream = this.baseStream;
                this.port = null;
                this.baseStream = null;
                CloseSafely(current, currentStream);
                lock (this.syncRoot)
                {
                    this.buffer.Clear();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Moves complete barcodes out of the buffer. Each barcode is trimmed; empty lines are dropped.
        /// Internal for the test harness.
        /// </summary>
        internal static List<string> ExtractLines(StringBuilder data)
        {
            List<string> result = new List<string>();
            try
            {
                string text = data.ToString();
                int lastTerminator = text.LastIndexOfAny(LINE_TERMINATORS);
                if (lastTerminator < 0)
                {
                    // No terminator yet: keep the partial barcode, but never let garbage grow without limit
                    if (text.Length > MAX_BUFFER_LENGTH)
                        data.Clear();
                    return result;
                }

                string complete = text.Substring(0, lastTerminator + 1);
                data.Remove(0, lastTerminator + 1);
                foreach (string part in complete.Split(LINE_TERMINATORS, StringSplitOptions.RemoveEmptyEntries))
                {
                    string line = new string(part.Where(c => !Char.IsControl(c)).ToArray()).Trim();
                    if (!String.IsNullOrEmpty(line))
                        result.Add(line);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort current = this.port;
                if (current == null || !current.IsOpen)
                    return;

                string chunk = current.ReadExisting();
                DateTime receivedTime = DateTime.Now;
                List<string> lines;
                lock (this.syncRoot)
                {
                    this.buffer.Append(chunk);
                    lines = ExtractLines(this.buffer);
                }

                if (this.onLineReceived != null)
                {
                    foreach (string line in lines)
                        this.onLineReceived(this.PortName, line, receivedTime);
                }
            }
            catch (Exception ex)
            {
                // Must never escape: an exception on the serial thread would close HIS
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Port_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Warn("ComPortReader " + this.PortName + " error: " + e.EventType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CloseSafely(SerialPort current, Stream currentStream)
        {
            if (current != null)
            {
                try
                {
                    current.DataReceived -= Port_DataReceived;
                    current.ErrorReceived -= Port_ErrorReceived;
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }

            if (currentStream != null)
            {
                try
                {
                    currentStream.Close();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }

            if (current != null)
            {
                try
                {
                    current.Close();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                try
                {
                    current.Dispose();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
        }

        private static int ReadIntSetting(string key, int defaultValue)
        {
            try
            {
                int value;
                string raw = ConfigurationManager.AppSettings[key];
                if (!String.IsNullOrWhiteSpace(raw) && Int32.TryParse(raw.Trim(), out value) && value > 0)
                    return value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return defaultValue;
        }

        private static T ReadEnumSetting<T>(string key, T defaultValue) where T : struct
        {
            try
            {
                T value;
                string raw = ConfigurationManager.AppSettings[key];
                if (!String.IsNullOrWhiteSpace(raw) && Enum.TryParse<T>(raw.Trim(), true, out value))
                    return value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return defaultValue;
        }
    }
}
