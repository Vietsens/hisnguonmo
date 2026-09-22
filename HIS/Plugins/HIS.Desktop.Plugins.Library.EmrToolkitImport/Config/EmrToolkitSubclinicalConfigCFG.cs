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
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.Library.EmrToolkitImport.Config
{
    /// <summary>
    /// Subclinical result types the gateway can share. The configuration uses the business
    /// abbreviations below so an operator never types a numeric id.
    /// </summary>
    public enum EmrToolkitSubclinicalType
    {
        /// <summary>Lab test — configuration label "XN". The only type supported for now</summary>
        TEST = 1,

        /// <summary>Diagnostic imaging — label "CDHA". The gateway has no API for it yet</summary>
        DIIM = 2,

        /// <summary>Functional exploration — label "TDCN"</summary>
        TDCN = 3,

        /// <summary>Surgery and procedure — label "PTTT"</summary>
        SURG = 4
    }

    /// <summary>
    /// Reads the single connection key shared by the EMR import flow, this view flow and the
    /// MOS background push job — so the gateway password lives in exactly one place.
    ///
    /// Key   : HIS.Desktop.Plugins.EmrToolKit.ConnectionInfo
    /// Value : BaseUrl|TaiKhoan|MatKhau|IDMauPhieu|SyncTypes|ViewTypes|ScanDayNumber|MaCskcb|TimeoutSecond
    ///
    /// The first three positions are required; anything missing after them falls back to a
    /// default, so the value a hospital already uses for the EMR import flow keeps working.
    /// Read straight from HisConfigs on every call — changing the configuration must not
    /// require restarting the application.
    /// </summary>
    public class EmrToolkitSubclinicalConfigCFG
    {
        /// <summary>Configuration key, shared with the EMR import flow</summary>
        public const string CONFIG_KEY__CONNECTION_INFO = "HIS.Desktop.Plugins.EmrToolKit.ConnectionInfo";

        private const int DEFAULT_TIMEOUT_SECOND = 120;

        /// <summary>Gateway address, without a trailing slash</summary>
        public string BaseUrl { get; private set; }

        /// <summary>Account shared by the push (backend) and the lookup (frontend)</summary>
        public string Username { get; private set; }

        public string Password { get; private set; }

        /// <summary>Subclinical types this hospital may look up on the gateway</summary>
        public List<EmrToolkitSubclinicalType> ViewTypes { get; private set; }

        /// <summary>Facility code to send; empty means the one the token belongs to</summary>
        public string MaCskcb { get; private set; }

        /// <summary>HTTP timeout in seconds — downloading a pdf takes longer than a json call</summary>
        public int TimeoutSecond { get; private set; }

        public EmrToolkitSubclinicalConfigCFG()
        {
            this.ViewTypes = new List<EmrToolkitSubclinicalType>();
            this.TimeoutSecond = DEFAULT_TIMEOUT_SECOND;
        }

        /// <summary>
        /// Loads the configuration. Returns null when it is missing or lacks a required field,
        /// which the caller treats as "the feature is not configured" — never as an error.
        /// </summary>
        public static EmrToolkitSubclinicalConfigCFG Load()
        {
            try
            {
                return Parse(HisConfigs.Get<string>(CONFIG_KEY__CONNECTION_INFO));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>
        /// Whether the button and the tab should be shown for a given subclinical type:
        /// the connection must be configured and the type listed in ViewTypes.
        /// </summary>
        public static bool IsViewEnable(EmrToolkitSubclinicalType type)
        {
            try
            {
                EmrToolkitSubclinicalConfigCFG config = Load();
                return config != null && config.ViewTypes.Contains(type);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return false;
            }
        }

        private static EmrToolkitSubclinicalConfigCFG Parse(string configValue)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(configValue)) return null;

                string[] parts = configValue.Split('|');
                EmrToolkitSubclinicalConfigCFG result = new EmrToolkitSubclinicalConfigCFG();
                result.BaseUrl = (GetPart(parts, 0) ?? "").TrimEnd('/');
                result.Username = GetPart(parts, 1);
                result.Password = GetPart(parts, 2);
                // position 3 is IDMauPhieu of the EMR import flow — not used here
                // position 4 is SyncTypes, read by the MOS push job only
                result.ViewTypes = ParseTypes(GetPart(parts, 5));
                // position 6 is ScanDayNumber, read by the MOS push job only
                result.MaCskcb = GetPart(parts, 7);
                result.TimeoutSecond = ParseTimeoutSecond(GetPart(parts, 8));

                if (string.IsNullOrEmpty(result.BaseUrl)
                    || string.IsNullOrEmpty(result.Username)
                    || string.IsNullOrEmpty(result.Password))
                {
                    LogSystem.Warn("EMRTOOLKIT: khóa " + CONFIG_KEY__CONNECTION_INFO
                        + " thiếu trường bắt buộc. Định dạng: BaseUrl|TaiKhoan|MatKhau|IDMauPhieu|SyncTypes|ViewTypes|ScanDayNumber|MaCskcb|TimeoutSecond");
                    return null;
                }

                return result;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>Labels separated by a comma. An unknown label is skipped with a warning.</summary>
        private static List<EmrToolkitSubclinicalType> ParseTypes(string raw)
        {
            List<EmrToolkitSubclinicalType> result = new List<EmrToolkitSubclinicalType>();
            try
            {
                if (string.IsNullOrWhiteSpace(raw)) return result;

                foreach (string label in raw.Split(','))
                {
                    if (string.IsNullOrWhiteSpace(label)) continue;

                    switch (label.Trim().ToUpper())
                    {
                        case "XN": AddType(result, EmrToolkitSubclinicalType.TEST); break;
                        case "CDHA": AddType(result, EmrToolkitSubclinicalType.DIIM); break;
                        case "TDCN": AddType(result, EmrToolkitSubclinicalType.TDCN); break;
                        case "PTTT": AddType(result, EmrToolkitSubclinicalType.SURG); break;
                        default:
                            LogSystem.Warn("EMRTOOLKIT: nhãn loại cận lâm sàng không hợp lệ trong cấu hình: " + label.Trim());
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return result;
        }

        private static void AddType(List<EmrToolkitSubclinicalType> list, EmrToolkitSubclinicalType type)
        {
            if (!list.Contains(type)) list.Add(type);
        }

        private static int ParseTimeoutSecond(string raw)
        {
            try
            {
                int value;
                if (!int.TryParse((raw ?? "").Trim(), out value) || value <= 0)
                {
                    return DEFAULT_TIMEOUT_SECOND;
                }
                return value;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return DEFAULT_TIMEOUT_SECOND;
            }
        }

        private static string GetPart(string[] arr, int index)
        {
            return (arr != null && index < arr.Length && arr[index] != null) ? arr[index].Trim() : null;
        }
    }
}
