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
using EMR.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.HisTrackingList.TrackingView
{
    /// <summary>
    /// One block shown in the treatment sheet viewer: either a signed document or one tracking rendered with Mps000062.
    /// </summary>
    internal class TrackingDisplayUnit
    {
        /// <summary>
        /// Tracking that gives the position of the block. For a signed document covering many trackings
        /// this is the displayed tracking with the latest treatment time.
        /// </summary>
        internal V_HIS_TRACKING Tracking { get; set; }

        /// <summary>
        /// Signed document to display. Null means the tracking is rendered with Mps000062.
        /// </summary>
        internal V_EMR_DOCUMENT Document { get; set; }
    }

    /// <summary>
    /// Decides what is displayed for each tracking and in which order (newest treatment time first).
    /// </summary>
    internal static class TrackingDocumentPlanner
    {
        const string HIS_CODE_TRACKING_PREFIX = "HIS_TRACKING:";
        static readonly char[] HIS_CODE_SEPARATORS = new char[] { ' ', ',', ';', '|' };

        internal static List<TrackingDisplayUnit> Plan(List<V_HIS_TRACKING> trackings, List<V_EMR_DOCUMENT> documents)
        {
            List<TrackingDisplayUnit> result = new List<TrackingDisplayUnit>();
            if (trackings == null || trackings.Count == 0)
                return result;

            // Latest signed document of each tracking (CREATE_TIME, then ID)
            Dictionary<long, V_EMR_DOCUMENT> documentByTrackingId = new Dictionary<long, V_EMR_DOCUMENT>();
            if (documents != null)
            {
                foreach (var document in documents)
                {
                    foreach (long trackingId in GetTrackingIds(document.HIS_CODE))
                    {
                        V_EMR_DOCUMENT current;
                        if (!documentByTrackingId.TryGetValue(trackingId, out current) || IsCreatedLater(document, current))
                            documentByTrackingId[trackingId] = document;
                    }
                }
            }

            Dictionary<long, TrackingDisplayUnit> unitByDocumentId = new Dictionary<long, TrackingDisplayUnit>();
            HashSet<long> addedTrackingIds = new HashSet<long>();
            foreach (var tracking in trackings)
            {
                if (tracking == null || !addedTrackingIds.Add(tracking.ID))
                    continue;

                V_EMR_DOCUMENT document;
                if (documentByTrackingId.TryGetValue(tracking.ID, out document))
                {
                    TrackingDisplayUnit unit;
                    if (unitByDocumentId.TryGetValue(document.ID, out unit))
                    {
                        if (IsAfter(tracking, unit.Tracking))
                            unit.Tracking = tracking;
                    }
                    else
                    {
                        unit = new TrackingDisplayUnit { Tracking = tracking, Document = document };
                        unitByDocumentId[document.ID] = unit;
                        result.Add(unit);
                    }
                }
                else
                {
                    result.Add(new TrackingDisplayUnit { Tracking = tracking });
                }
            }

            return result.OrderByDescending(o => o.Tracking.TRACKING_TIME).ThenByDescending(o => o.Tracking.ID).ToList();
        }

        /// <summary>
        /// Tracking IDs written in the HIS_CODE of a treatment sheet document,
        /// e.g. "Mps000062 TREATMENT_CODE:000000072544 HIS_TRACKING:502,HIS_TRACKING:503".
        /// Each item is matched as a whole so that "HIS_TRACKING:50" does not match "HIS_TRACKING:502".
        /// </summary>
        internal static List<long> GetTrackingIds(string hisCode)
        {
            List<long> result = new List<long>();
            if (String.IsNullOrWhiteSpace(hisCode))
                return result;

            foreach (var item in hisCode.Split(HIS_CODE_SEPARATORS, StringSplitOptions.RemoveEmptyEntries))
            {
                long trackingId;
                if (item.StartsWith(HIS_CODE_TRACKING_PREFIX)
                    && long.TryParse(item.Substring(HIS_CODE_TRACKING_PREFIX.Length), out trackingId)
                    && !result.Contains(trackingId))
                {
                    result.Add(trackingId);
                }
            }
            return result;
        }

        private static bool IsCreatedLater(V_EMR_DOCUMENT document, V_EMR_DOCUMENT other)
        {
            long createTime = document.CREATE_TIME ?? 0;
            long otherCreateTime = other.CREATE_TIME ?? 0;
            return createTime > otherCreateTime || (createTime == otherCreateTime && document.ID > other.ID);
        }

        private static bool IsAfter(V_HIS_TRACKING tracking, V_HIS_TRACKING other)
        {
            return tracking.TRACKING_TIME > other.TRACKING_TIME || (tracking.TRACKING_TIME == other.TRACKING_TIME && tracking.ID > other.ID);
        }
    }
}
