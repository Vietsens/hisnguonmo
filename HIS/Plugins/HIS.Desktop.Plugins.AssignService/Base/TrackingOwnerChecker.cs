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

namespace HIS.Desktop.Plugins.AssignService.Base
{
    /// <summary>
    /// Task 59656 (revised 14/10/2026 after QA): key HIS.Desktop.Plugins.Tracking.AssignToOwnTrackingOption only controls
    /// which treatment sheets (HIS_TRACKING) the "Treatment sheet" combo shows: 1 = only sheets created by the ordering user
    /// (sheet owner = HIS_TRACKING.CREATOR, ordering user = REQUEST_LOGINNAME), empty/other = all sheets (behavior unchanged).
    /// Pure logic without UI so it can be tested on its own. Identical copies live in the prescription and service assignment
    /// plugins (each plugin is a separate DLL, there is no shared tracking library).
    /// </summary>
    internal static class TrackingOwnerChecker
    {
        /// <summary>HIS_CONFIG key: 1 = only own sheets, empty/other = all sheets.</summary>
        internal const string CONFIG_KEY = "HIS.Desktop.Plugins.Tracking.AssignToOwnTrackingOption";

        /// <summary>yyyyMMddHHmmss / 1000000 = yyyyMMdd.</summary>
        private const long TIME_TO_DATE_DIVISOR = 1000000;

        /// <summary>Treatment sheet data needed by the filter (works for both HIS_TRACKING and V_HIS_TRACKING).</summary>
        internal class Item
        {
            internal long Id { get; set; }
            internal long TrackingTime { get; set; }
            internal string Creator { get; set; }
        }

        internal static EnumTrackingOwnerOption ParseOption(string value)
        {
            string trimmed = (value ?? "").Trim();
            if (trimmed == ((int)EnumTrackingOwnerOption.OwnOnly).ToString())
                return EnumTrackingOwnerOption.OwnOnly;
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

        /// <summary>Sheets created by the ordering user, original order kept. Null source returns null.</summary>
        internal static List<T> FilterOwn<T>(IEnumerable<T> trackings, Func<T, string> getCreator, string requestLoginName)
        {
            if (trackings == null)
                return null;
            return trackings.Where(o => o != null && IsOwner(getCreator(o), requestLoginName)).ToList();
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
    }
}
