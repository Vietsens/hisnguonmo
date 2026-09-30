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
using HIS.Desktop.LocalStorage.HisConfig;
using System;
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.Library.ExpMestAttachFile.Config
{
    /// <summary>
    /// v57853 - Hospital-wide configs of the "attach prescription to sale export ticket" feature.
    /// Read on every call (cheap dictionary lookup) so a config change applies without restarting.
    /// </summary>
    internal class AttachFileConfig
    {
        private const string KEY__HAS_CONNECTION_EMR = "MOS.HAS_CONNECTION_EMR";
        private const string KEY__IS_ENABLE = "HIS.Desktop.Plugins.ExpMestSaleCreate.AttachPrescription.IsEnable";
        private const string KEY__MAX_FILE_SIZE_MB = "HIS.Desktop.Plugins.ExpMestSaleCreate.AttachPrescription.MaxFileSizeMB";

        /// <summary>Default max size of one file when the config is empty/invalid</summary>
        private const decimal DEFAULT_MAX_FILE_SIZE_MB = 5;

        /// <summary>EMR_DOCUMENT_TYPE.DOCUMENT_TYPE_CODE of prescription attachments — MUST exist in EMR DB</summary>
        internal const string DOCUMENT_TYPE_CODE = "EXPSA";

        /// <summary>Allowed extensions (lower-case, with dot)</summary>
        internal static readonly HashSet<string> ALLOWED_EXTENSIONS = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".pdf"
        };

        internal const string OPEN_FILE_FILTER = "Ảnh/PDF (*.jpg, *.jpeg, *.png, *.bmp, *.gif, *.pdf)|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.pdf";

        /// <summary>Feature is ON only when the hospital is connected to EMR and the enable key = 1</summary>
        internal static bool IsEnable
        {
            get
            {
                try
                {
                    return HisConfigs.Get<string>(KEY__HAS_CONNECTION_EMR) == "1"
                        && HisConfigs.Get<string>(KEY__IS_ENABLE) == "1";
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                    return false;
                }
            }
        }

        /// <summary>Max size of one file in bytes</summary>
        internal static long MaxFileSizeBytes
        {
            get
            {
                return (long)(MaxFileSizeMB * 1024 * 1024);
            }
        }

        internal static decimal MaxFileSizeMB
        {
            get
            {
                decimal result = DEFAULT_MAX_FILE_SIZE_MB;
                try
                {
                    string value = HisConfigs.Get<string>(KEY__MAX_FILE_SIZE_MB);
                    decimal parsed;
                    if (!String.IsNullOrWhiteSpace(value)
                        && Decimal.TryParse(value.Trim(), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out parsed)
                        && parsed > 0)
                    {
                        result = parsed;
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return result;
            }
        }
    }
}
