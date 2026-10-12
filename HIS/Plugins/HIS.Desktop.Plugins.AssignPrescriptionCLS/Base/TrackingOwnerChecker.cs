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

namespace HIS.Desktop.Plugins.AssignPrescriptionCLS.Base
{
    /// <summary>
    /// Task 59656: an order (medicine, material, technical service) may only be attached to a treatment sheet
    /// (HIS_TRACKING) created by the ordering user (REQUEST_LOGINNAME); the sheet owner is HIS_TRACKING.CREATOR.
    /// Pure logic without UI so it can be tested on its own. Identical copies live in the prescription,
    /// service assignment and treatment sheet plugins (each plugin is a separate DLL, there is no shared tracking library).
    /// </summary>
    internal static class TrackingOwnerChecker
    {
        /// <summary>HIS_CONFIG key: 1 = warn, 2 = block, empty/other = no check (behavior unchanged).</summary>
        internal const string CONFIG_KEY = "HIS.Desktop.Plugins.Tracking.AssignToOwnTrackingOption";

        /// <summary>Namespace of the treatment sheet screen (plugin HIS.Desktop.Plugins.TrackingCreate).</summary>
        private const string TRACKING_CREATE_NAMESPACE = "HIS.Desktop.Plugins.TrackingCreate";

        /// <summary>Maximum detail lines listed in one message, the rest is shown as "...".</summary>
        private const int MAX_DETAIL_LINES = 10;

        /// <summary>yyyyMMddHHmmss / 1000000 = yyyyMMdd.</summary>
        private const long TIME_TO_DATE_DIVISOR = 1000000;

        /// <summary>Treatment sheet data needed by the check (works for both HIS_TRACKING and V_HIS_TRACKING).</summary>
        internal class Item
        {
            internal long Id { get; set; }
            internal long TrackingTime { get; set; }
            internal string Creator { get; set; }
        }

        /// <summary>Violations found before saving.</summary>
        internal class Result
        {
            internal Result()
            {
                this.OtherOwnerTrackings = new List<Item>();
                this.MissingDates = new List<long>();
            }

            /// <summary>Selected treatment sheets created by someone other than the ordering user.</summary>
            internal List<Item> OtherOwnerTrackings { get; private set; }

            /// <summary>Instruction dates (yyyyMMdd) with no selected sheet while the ordering user has no own sheet on that date.</summary>
            internal List<long> MissingDates { get; private set; }

            internal bool HasViolation
            {
                get { return this.OtherOwnerTrackings.Count > 0 || this.MissingDates.Count > 0; }
            }
        }

        internal static EnumTrackingOwnerOption ParseOption(string value)
        {
            string trimmed = (value ?? "").Trim();
            if (trimmed == ((int)EnumTrackingOwnerOption.Warning).ToString())
                return EnumTrackingOwnerOption.Warning;
            if (trimmed == ((int)EnumTrackingOwnerOption.Block).ToString())
                return EnumTrackingOwnerOption.Block;
            return EnumTrackingOwnerOption.None;
        }

        /// <summary>Same account, ignoring case and surrounding spaces. An empty value never matches.</summary>
        internal static bool IsOwner(string creator, string requestLoginName)
        {
            if (String.IsNullOrWhiteSpace(creator) || String.IsNullOrWhiteSpace(requestLoginName))
                return false;
            return String.Equals(creator.Trim(), requestLoginName.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        internal static long GetDate(long time)
        {
            return time / TIME_TO_DATE_DIVISOR;
        }

        /// <summary>Latest sheet of the ordering user on the date of instructionTime, null if none.</summary>
        internal static Item GetLatestOwnTracking(IEnumerable<Item> trackings, long instructionTime, string requestLoginName)
        {
            if (trackings == null || instructionTime <= 0)
                return null;
            long date = GetDate(instructionTime);
            return trackings
                .Where(o => o != null && GetDate(o.TrackingTime) == date && IsOwner(o.Creator, requestLoginName))
                .OrderByDescending(o => o.TrackingTime)
                .FirstOrDefault();
        }

        /// <param name="requestLoginName">Ordering user, resolved exactly like the REQUEST_LOGINNAME sent to the backend.</param>
        /// <param name="instructionTimes">Instruction times of the orders being saved.</param>
        /// <param name="selectedTrackings">Sheets the orders will be attached to.</param>
        /// <param name="availableTrackings">All sheets listed by the screen (used to know whether the ordering user has an own sheet).</param>
        /// <param name="isOneTrackingForAllTimes">true: single-selection combo, the selected sheet is attached whatever the instruction date.</param>
        /// <param name="isCheckMissing">false: skip the "no own sheet" check (edit mode, opened from an unsaved sheet...).</param>
        internal static Result Check(string requestLoginName, IEnumerable<long> instructionTimes, IEnumerable<Item> selectedTrackings,
            IEnumerable<Item> availableTrackings, bool isOneTrackingForAllTimes, bool isCheckMissing)
        {
            Result result = new Result();
            List<Item> selecteds = selectedTrackings != null ? selectedTrackings.Where(o => o != null).ToList() : new List<Item>();
            HashSet<long> otherOwnerIds = new HashSet<long>();
            foreach (var item in selecteds)
            {
                if (!IsOwner(item.Creator, requestLoginName) && otherOwnerIds.Add(item.Id))
                    result.OtherOwnerTrackings.Add(item);
            }

            if (isCheckMissing && instructionTimes != null)
            {
                HashSet<long> selectedDates = new HashSet<long>(selecteds.Select(o => GetDate(o.TrackingTime)));
                HashSet<long> ownDates = new HashSet<long>();
                if (availableTrackings != null)
                {
                    foreach (var item in availableTrackings)
                    {
                        if (item != null && IsOwner(item.Creator, requestLoginName))
                            ownDates.Add(GetDate(item.TrackingTime));
                    }
                }

                foreach (long date in instructionTimes.Where(o => o > 0).Select(o => GetDate(o)).Distinct().OrderBy(o => o))
                {
                    bool isCovered = (isOneTrackingForAllTimes && selecteds.Count > 0) || selectedDates.Contains(date);
                    if (!isCovered && !ownDates.Contains(date))
                        result.MissingDates.Add(date);
                }
            }
            return result;
        }

        /// <summary>The form was opened by the treatment sheet screen (delegate DgProcessDataResult belongs to it).</summary>
        internal static bool IsCalledFromTrackingCreate(Delegate callback)
        {
            try
            {
                if (callback == null || callback.Method == null || callback.Method.DeclaringType == null)
                    return false;
                string ns = callback.Method.DeclaringType.Namespace;
                return ns != null && (ns == TRACKING_CREATE_NAMESPACE || ns.StartsWith(TRACKING_CREATE_NAMESPACE + "."));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        /// <param name="formatOtherOwnerLine">{0} sheet time, {1} sheet creator, {2} ordering user.</param>
        /// <param name="formatMissingLine">{0} ordering user, {1} date.</param>
        internal static string BuildMessage(Result result, string requestUserDisplay, Func<string, string> getUserDisplay,
            string formatOtherOwnerLine, string formatMissingLine, string footer)
        {
            List<string> lines = new List<string>();
            if (result != null)
            {
                foreach (var item in result.OtherOwnerTrackings)
                {
                    string creatorDisplay = getUserDisplay != null ? getUserDisplay(item.Creator) : item.Creator;
                    lines.Add(String.Format(formatOtherOwnerLine ?? "", TimeToString(item.TrackingTime), creatorDisplay, requestUserDisplay));
                }
                foreach (var date in result.MissingDates)
                {
                    lines.Add(String.Format(formatMissingLine ?? "", requestUserDisplay, DateToString(date)));
                }
            }
            return JoinLines(lines, footer);
        }

        /// <summary>Lines (at most MAX_DETAIL_LINES, then "...") followed by the footer.</summary>
        internal static string JoinLines(List<string> lines, string footer)
        {
            StringBuilder sb = new StringBuilder();
            if (lines != null)
            {
                foreach (var line in lines.Take(MAX_DETAIL_LINES))
                    sb.AppendLine(line);
                if (lines.Count > MAX_DETAIL_LINES)
                    sb.AppendLine("...");
            }
            sb.Append(footer ?? "");
            return sb.ToString();
        }

        /// <summary>yyyyMMddHHmmss -> dd/MM/yyyy HH:mm</summary>
        internal static string TimeToString(long time)
        {
            string s = time.ToString();
            if (s.Length < 12)
                return s;
            return s.Substring(6, 2) + "/" + s.Substring(4, 2) + "/" + s.Substring(0, 4) + " " + s.Substring(8, 2) + ":" + s.Substring(10, 2);
        }

        /// <summary>yyyyMMdd -> dd/MM/yyyy</summary>
        internal static string DateToString(long date)
        {
            string s = date.ToString();
            if (s.Length != 8)
                return s;
            return s.Substring(6, 2) + "/" + s.Substring(4, 2) + "/" + s.Substring(0, 4);
        }
    }
}
