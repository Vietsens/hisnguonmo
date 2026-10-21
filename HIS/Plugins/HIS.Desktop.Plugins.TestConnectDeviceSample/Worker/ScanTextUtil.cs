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

namespace HIS.Desktop.Plugins.TestConnectDeviceSample.Worker
{
    /// <summary>
    /// Classifies a scanned string: account card ("TK-" + loginname) or sample barcode / service request code.
    /// Sample barcodes and service request codes are numeric, so the prefix never collides with them.
    /// </summary>
    internal static class ScanTextUtil
    {
        internal const string ACCOUNT_CARD_PREFIX = "TK-";
        private const int SERVICE_REQ_CODE_LENGTH = 12;

        internal static bool IsAccountCard(string text)
        {
            try
            {
                return !String.IsNullOrWhiteSpace(text)
                    && text.StartsWith(ACCOUNT_CARD_PREFIX, StringComparison.OrdinalIgnoreCase)
                    && text.Trim().Length > ACCOUNT_CARD_PREFIX.Length;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        internal static string GetLoginname(string accountCardText)
        {
            try
            {
                return accountCardText.Trim().Substring(ACCOUNT_CARD_PREFIX.Length).Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>
        /// A requisition (service request) barcode: exactly 12 digits, compared as is (no zero padding).
        /// </summary>
        internal static bool IsServiceReqCode(string text)
        {
            try
            {
                return !String.IsNullOrEmpty(text) && text.Length == SERVICE_REQ_CODE_LENGTH && text.All(Char.IsDigit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }
    }

    /// <summary>
    /// Drops a scan repeated on the same port within a few seconds (desk scanners in presentation mode re-read).
    /// Not thread-safe: callers lock around it.
    /// </summary>
    internal sealed class RepeatScanFilter
    {
        internal const int REPEAT_SECONDS = 3;
        private readonly Dictionary<string, KeyValuePair<string, DateTime>> lastScanByPort =
            new Dictionary<string, KeyValuePair<string, DateTime>>(StringComparer.OrdinalIgnoreCase);

        internal bool IsRepeat(string portName, string text, DateTime receivedTime)
        {
            try
            {
                KeyValuePair<string, DateTime> last;
                bool repeat = this.lastScanByPort.TryGetValue(portName, out last)
                    && String.Equals(last.Key, text, StringComparison.OrdinalIgnoreCase)
                    && (receivedTime - last.Value).TotalSeconds <= REPEAT_SECONDS;
                this.lastScanByPort[portName] = new KeyValuePair<string, DateTime>(text, receivedTime);
                return repeat;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }
    }
}
