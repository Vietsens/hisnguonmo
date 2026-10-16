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

namespace HIS.Desktop.Plugins.BedRoomPartial.Base
{
    /// <summary>
    /// Task 57971: paraclinical (CLS) tab groups shared by the Bed room screen (BedRoomPartial)
    /// and the Subclinical result screen (ContentSubclinical).
    /// Both plugins keep an IDENTICAL copy of this table (only the namespace differs) so that a service
    /// always lands in the same tab on both screens. Change both copies together.
    /// A service is grouped by its service type; diagnostic imaging services are split by the
    /// diagnostic imaging type of the service catalog (HIS_SERVICE.DIIM_TYPE_ID).
    /// </summary>
    internal class ClsTabGroupStore
    {
        /// <summary>Test (Xet nghiem)</summary>
        internal const string GROUP_XN = "XN";
        /// <summary>Ultrasound (Sieu am)</summary>
        internal const string GROUP_SA = "SA";
        /// <summary>Diagnostic imaging with DIIM type X-ray</summary>
        internal const string GROUP_XQ = "XQ";
        /// <summary>Diagnostic imaging with DIIM type CT or MRI</summary>
        internal const string GROUP_CTMRI = "CTMRI";
        /// <summary>Functional exploration (Tham do chuc nang)</summary>
        internal const string GROUP_TDCN = "TDCN";
        /// <summary>Endoscopy (Noi soi)</summary>
        internal const string GROUP_NS = "NS";
        /// <summary>Pathology (Giai phau benh ly)</summary>
        internal const string GROUP_GPBL = "GPBL";
        /// <summary>Procedure (Thu thuat) - only the Subclinical result screen loads this service type</summary>
        internal const string GROUP_TT = "TT";
        /// <summary>Other paraclinical: diagnostic imaging without DIIM type and anything not classified above</summary>
        internal const string GROUP_KHAC = "KHAC";
        /// <summary>Prefix of dynamic groups: one tab per extra DIIM type of the catalog (e.g. PET/CT) - code = prefix + DIIM_TYPE_ID</summary>
        internal const string GROUP_DIIM_PREFIX = "DIIM";

        /// <summary>Flag (inside the List&lt;string&gt; argument) asking ContentSubclinical to show the result tabs</summary>
        internal const string ARG__CLS_TAB = "CLS_TAB";
        /// <summary>Flag prefix (inside the List&lt;string&gt; argument): group code of the tab to open first</summary>
        internal const string ARG__CLS_TAB_INIT_PREFIX = "CLS_TAB_INIT:";

        /// <summary>
        /// Display order of the fixed groups. Dynamic DIIM groups are placed right after CTMRI, ordered by DIIM type id.
        /// </summary>
        static readonly string[] FIXED_ORDER = new string[] { GROUP_XN, GROUP_SA, GROUP_XQ, GROUP_CTMRI, GROUP_TDCN, GROUP_NS, GROUP_GPBL, GROUP_TT, GROUP_KHAC };

        /// <summary>
        /// Groups always shown on the Bed room screen when the config option is "always show main tabs" (task document 3.3).
        /// </summary>
        static readonly HashSet<string> CORE_GROUPS = new HashSet<string> { GROUP_XN, GROUP_SA, GROUP_XQ, GROUP_TDCN, GROUP_NS };

        /// <summary>
        /// Service types counted as paraclinical on the Bed room screen (same set as the former "CLS" tab).
        /// </summary>
        static readonly HashSet<long> BED_ROOM_CLS_SERVICE_TYPES = new HashSet<long>
        {
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN
        };

        /// <summary>
        /// Group code of a service.
        /// </summary>
        /// <param name="serviceTypeId">TDL_SERVICE_TYPE_ID of the service</param>
        /// <param name="diimTypeId">DIIM_TYPE_ID of the service in the catalog (only used for diagnostic imaging)</param>
        internal static string Classify(long serviceTypeId, long? diimTypeId)
        {
            string result = GROUP_KHAC;
            try
            {
                if (serviceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)
                    result = GROUP_XN;
                else if (serviceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA)
                    result = GROUP_SA;
                else if (serviceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA)
                    result = ClassifyDiagnosticImaging(diimTypeId);
                else if (serviceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN)
                    result = GROUP_TDCN;
                else if (serviceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS)
                    result = GROUP_NS;
                else if (serviceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL)
                    result = GROUP_GPBL;
                else if (serviceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT)
                    result = GROUP_TT;
            }
            catch (Exception ex)
            {
                result = GROUP_KHAC;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private static string ClassifyDiagnosticImaging(long? diimTypeId)
        {
            string result = GROUP_KHAC;
            try
            {
                if (diimTypeId.HasValue && diimTypeId.Value > 0)
                {
                    if (diimTypeId.Value == IMSys.DbConfig.HIS_RS.HIS_DIIM_TYPE.ID__XQ)
                        result = GROUP_XQ;
                    else if (diimTypeId.Value == IMSys.DbConfig.HIS_RS.HIS_DIIM_TYPE.ID__CT || diimTypeId.Value == IMSys.DbConfig.HIS_RS.HIS_DIIM_TYPE.ID__MRI)
                        result = GROUP_CTMRI;
                    else
                        result = GROUP_DIIM_PREFIX + diimTypeId.Value;
                }
            }
            catch (Exception ex)
            {
                result = GROUP_KHAC;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// True when the service type belongs to the former "CLS" tab of the Bed room screen.
        /// </summary>
        internal static bool IsBedRoomClsServiceType(long? serviceTypeId)
        {
            bool result = false;
            try
            {
                result = serviceTypeId.HasValue && BED_ROOM_CLS_SERVICE_TYPES.Contains(serviceTypeId.Value);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// True for groups always shown on the Bed room screen (option "always show main tabs").
        /// </summary>
        internal static bool IsCoreGroup(string groupCode)
        {
            bool result = false;
            try
            {
                result = !String.IsNullOrEmpty(groupCode) && CORE_GROUPS.Contains(groupCode);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// True for a dynamic group created from an extra DIIM type; returns the DIIM type id.
        /// </summary>
        internal static bool TryGetDiimTypeId(string groupCode, out long diimTypeId)
        {
            diimTypeId = 0;
            bool result = false;
            try
            {
                if (!String.IsNullOrEmpty(groupCode) && groupCode.StartsWith(GROUP_DIIM_PREFIX, StringComparison.Ordinal))
                    result = long.TryParse(groupCode.Substring(GROUP_DIIM_PREFIX.Length), out diimTypeId) && diimTypeId > 0;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Sort key of a group: fixed groups by FIXED_ORDER, dynamic DIIM groups right after CTMRI by DIIM type id,
        /// unknown codes at the end.
        /// </summary>
        internal static long GetSortKey(string groupCode)
        {
            long result = FIXED_ORDER.Length * 1000000L;
            try
            {
                long diimTypeId;
                if (TryGetDiimTypeId(groupCode, out diimTypeId))
                {
                    result = (Array.IndexOf(FIXED_ORDER, GROUP_CTMRI) * 1000000L) + Math.Min(diimTypeId, 999999L);
                }
                else
                {
                    int index = Array.IndexOf(FIXED_ORDER, groupCode);
                    if (index < 0)
                        index = FIXED_ORDER.Length;
                    result = index * 1000000L;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Fixed groups shown on the Bed room screen, in display order (Procedure stays in the "Khác" tab there).
        /// </summary>
        internal static List<string> GetBedRoomFixedGroups()
        {
            List<string> result = new List<string>();
            try
            {
                result = FIXED_ORDER.Where(o => o != GROUP_TT).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
