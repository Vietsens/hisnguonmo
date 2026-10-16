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
using HIS.Desktop.Plugins.BedRoomPartial.ADO;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.BedRoomPartial.Base
{
    /// <summary>
    /// Task 57971: splits the type-grouped order rows of the Bed room screen
    /// (service type row -> service request row -> service row) into one list per CLS tab group.
    /// Grouping is done on service rows; a service type row / request row is copied into every group
    /// that has at least one of its services, so each tab keeps the former "CLS" tree layout.
    /// Rows are shared (not cloned): every row keeps the same parent/leaf role in every list it appears in.
    /// </summary>
    internal class ClsTabDataBuilder
    {
        Dictionary<long, V_HIS_SERVICE> serviceById;
        Dictionary<string, V_HIS_SERVICE> serviceByCode;

        /// <summary>Rows of each group, in the order of the input list</summary>
        internal Dictionary<string, List<SereServADO>> RowsByGroup { get; private set; }
        /// <summary>Number of service rows of each group (number shown on the tab caption)</summary>
        internal Dictionary<string, int> ServiceCountByGroup { get; private set; }
        /// <summary>Groups of the services of each service request (SERVICE_REQ_ID)</summary>
        internal Dictionary<long, HashSet<string>> GroupsByServiceReq { get; private set; }
        /// <summary>Catalog name (DIIM_TYPE_NAME) of each dynamic DIIM group found</summary>
        internal Dictionary<string, string> DiimGroupNames { get; private set; }

        /// <param name="serviceById">Service catalog by ID (DIIM type lookup by SERVICE_ID)</param>
        /// <param name="serviceByCode">Service catalog by code (fallback when the backend does not send SERVICE_ID)</param>
        internal ClsTabDataBuilder(Dictionary<long, V_HIS_SERVICE> serviceById, Dictionary<string, V_HIS_SERVICE> serviceByCode)
        {
            this.serviceById = serviceById;
            this.serviceByCode = serviceByCode;
            this.RowsByGroup = new Dictionary<string, List<SereServADO>>();
            this.ServiceCountByGroup = new Dictionary<string, int>();
            this.GroupsByServiceReq = new Dictionary<long, HashSet<string>>();
            this.DiimGroupNames = new Dictionary<string, string>();
        }

        /// <summary>
        /// Builds the per-group lists from the sorted type-grouped rows of one load.
        /// </summary>
        internal void Build(List<SereServADO> rows)
        {
            try
            {
                this.RowsByGroup = new Dictionary<string, List<SereServADO>>();
                this.ServiceCountByGroup = new Dictionary<string, int>();
                this.GroupsByServiceReq = new Dictionary<long, HashSet<string>>();
                this.DiimGroupNames = new Dictionary<string, string>();
                if (rows == null || rows.Count == 0)
                    return;

                Dictionary<string, SereServADO> rowByKey = new Dictionary<string, SereServADO>();
                HashSet<string> parentKeys = new HashSet<string>();
                IndexRows(rows, rowByKey, parentKeys);

                Dictionary<SereServADO, string> groupOfServiceRow = new Dictionary<SereServADO, string>();
                Dictionary<string, HashSet<string>> groupsOfStructureRow = new Dictionary<string, HashSet<string>>();
                this.ClassifyServiceRows(rows, rowByKey, parentKeys, groupOfServiceRow, groupsOfStructureRow);
                this.DistributeRows(rows, groupOfServiceRow, groupsOfStructureRow);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Row by key (CONCRETE_ID__IN_SETY) and the keys used as a parent.
        /// </summary>
        private static void IndexRows(List<SereServADO> rows, Dictionary<string, SereServADO> rowByKey, HashSet<string> parentKeys)
        {
            try
            {
                foreach (SereServADO row in rows)
                {
                    if (row == null || String.IsNullOrEmpty(row.CONCRETE_ID__IN_SETY))
                        continue;
                    if (!rowByKey.ContainsKey(row.CONCRETE_ID__IN_SETY))
                        rowByKey[row.CONCRETE_ID__IN_SETY] = row;
                    if (!String.IsNullOrEmpty(row.PARENT_ID__IN_SETY))
                        parentKeys.Add(row.PARENT_ID__IN_SETY);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Group of every service row, counts, groups of each request and of each structural (ancestor) row.
        /// </summary>
        private void ClassifyServiceRows(List<SereServADO> rows, Dictionary<string, SereServADO> rowByKey, HashSet<string> parentKeys,
            Dictionary<SereServADO, string> groupOfServiceRow, Dictionary<string, HashSet<string>> groupsOfStructureRow)
        {
            try
            {
                foreach (SereServADO row in rows)
                {
                    if (!IsClsServiceRow(row, parentKeys))
                        continue;
                    string group = this.ClassifyServiceRow(row);
                    groupOfServiceRow[row] = group;
                    int count;
                    this.ServiceCountByGroup.TryGetValue(group, out count);
                    this.ServiceCountByGroup[group] = count + 1;
                    if (row.SERVICE_REQ_ID.HasValue)
                        AddToSet(this.GroupsByServiceReq, row.SERVICE_REQ_ID.Value, group);
                    MarkAncestors(row, group, rowByKey, groupsOfStructureRow);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Puts every row into the list of its group(s), keeping the input order.
        /// </summary>
        private void DistributeRows(List<SereServADO> rows, Dictionary<SereServADO, string> groupOfServiceRow, Dictionary<string, HashSet<string>> groupsOfStructureRow)
        {
            try
            {
                foreach (SereServADO row in rows)
                {
                    if (row == null || String.IsNullOrEmpty(row.CONCRETE_ID__IN_SETY))
                        continue;
                    string group;
                    HashSet<string> groups;
                    if (groupOfServiceRow.TryGetValue(row, out group))
                        this.AddRow(group, row);
                    else if (groupsOfStructureRow.TryGetValue(row.CONCRETE_ID__IN_SETY, out groups))
                        foreach (string item in groups)
                            this.AddRow(item, row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// A service row is a leaf row (no child) under a request row, of a paraclinical service type.
        /// </summary>
        private static bool IsClsServiceRow(SereServADO row, HashSet<string> parentKeys)
        {
            bool result = false;
            try
            {
                result = row != null
                    && !String.IsNullOrEmpty(row.CONCRETE_ID__IN_SETY)
                    && !String.IsNullOrEmpty(row.PARENT_ID__IN_SETY)
                    && !parentKeys.Contains(row.CONCRETE_ID__IN_SETY)
                    && ClsTabGroupStore.IsBedRoomClsServiceType(row.TDL_SERVICE_TYPE_ID);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private string ClassifyServiceRow(SereServADO row)
        {
            string result = ClsTabGroupStore.GROUP_KHAC;
            try
            {
                long serviceTypeId = row.TDL_SERVICE_TYPE_ID ?? 0;
                long? diimTypeId = null;
                V_HIS_SERVICE service = null;
                if (serviceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA)
                {
                    service = this.FindService(row);
                    if (service != null)
                        diimTypeId = service.DIIM_TYPE_ID;
                }
                result = ClsTabGroupStore.Classify(serviceTypeId, diimTypeId);
                long dynamicDiimTypeId;
                if (service != null && ClsTabGroupStore.TryGetDiimTypeId(result, out dynamicDiimTypeId) && !this.DiimGroupNames.ContainsKey(result))
                    this.DiimGroupNames[result] = service.DIIM_TYPE_NAME;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Service catalog row of a service row: by SERVICE_ID (MOS with task 53745 view), else by service code.
        /// </summary>
        private V_HIS_SERVICE FindService(SereServADO row)
        {
            V_HIS_SERVICE result = null;
            try
            {
                if (row.SERVICE_ID.HasValue && row.SERVICE_ID.Value > 0 && this.serviceById != null)
                    this.serviceById.TryGetValue(row.SERVICE_ID.Value, out result);
                if (result == null && !String.IsNullOrEmpty(row.SERVICE_CODE) && this.serviceByCode != null)
                    this.serviceByCode.TryGetValue(row.SERVICE_CODE, out result);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private static void MarkAncestors(SereServADO row, string group, Dictionary<string, SereServADO> rowByKey, Dictionary<string, HashSet<string>> groupsOfStructureRow)
        {
            try
            {
                string parentKey = row.PARENT_ID__IN_SETY;
                int depth = 0;
                // Depth guard: the tree has 2 structural levels, a broken parent chain must not loop forever
                while (!String.IsNullOrEmpty(parentKey) && depth < 10)
                {
                    AddToSet(groupsOfStructureRow, parentKey, group);
                    SereServADO parent;
                    if (!rowByKey.TryGetValue(parentKey, out parent))
                        break;
                    parentKey = parent.PARENT_ID__IN_SETY;
                    depth++;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AddRow(string group, SereServADO row)
        {
            try
            {
                List<SereServADO> list;
                if (!this.RowsByGroup.TryGetValue(group, out list))
                {
                    list = new List<SereServADO>();
                    this.RowsByGroup[group] = list;
                }
                list.Add(row);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private static void AddToSet<TKey>(Dictionary<TKey, HashSet<string>> dic, TKey key, string group)
        {
            try
            {
                HashSet<string> set;
                if (!dic.TryGetValue(key, out set))
                {
                    set = new HashSet<string>();
                    dic[key] = set;
                }
                set.Add(group);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
