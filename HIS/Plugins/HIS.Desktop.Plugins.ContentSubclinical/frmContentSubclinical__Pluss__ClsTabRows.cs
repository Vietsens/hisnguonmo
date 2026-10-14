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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ContentSubclinical.ADO;
using HIS.Desktop.Plugins.ContentSubclinical.Base;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.ContentSubclinical
{
    /// <summary>
    /// Task 57971 - rows of the result tabs: group of every row, number of services per group, rows of each group tab
    /// (tab bar and binding: frmContentSubclinical__Pluss__ClsTab.cs).
    /// </summary>
    public partial class frmContentSubclinical : HIS.Desktop.Utility.FormBase
    {
        private static void AddRowsToIndex(Dictionary<string, TreeSereServADO> rowByKey, List<TreeSereServADO> rows)
        {
            try
            {
                if (rows == null)
                    return;
                foreach (TreeSereServADO row in rows)
                {
                    if (row != null && !String.IsNullOrEmpty(row.CONCRETE_ID__IN_SETY) && !rowByKey.ContainsKey(row.CONCRETE_ID__IN_SETY))
                        rowByKey[row.CONCRETE_ID__IN_SETY] = row;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Group of every row below a service type row. Row keys are paths "date_serviceType_..." so the service type
        /// is the 2nd segment; diagnostic imaging rows are split by the DIIM type of their service.
        /// Date rows and service type rows are structural (they belong to the tabs of their children).
        /// </summary>
        private Dictionary<string, string> ClassifyClsRows(Dictionary<string, TreeSereServADO> rowByKey, Dictionary<string, string> diimGroupNames)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            try
            {
                this.EnsureClsServiceLookup();
                foreach (KeyValuePair<string, TreeSereServADO> pair in rowByKey)
                {
                    string[] segments = pair.Key.Split('_');
                    long serviceTypeId;
                    if (segments.Length < 3 || !long.TryParse(segments[1], out serviceTypeId))
                        continue;

                    long? diimTypeId = null;
                    string diimTypeName = null;
                    if (serviceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA)
                    {
                        V_HIS_SERVICE service = this.FindClsService(pair.Value, rowByKey);
                        if (service != null)
                        {
                            diimTypeId = service.DIIM_TYPE_ID;
                            diimTypeName = service.DIIM_TYPE_NAME;
                        }
                    }
                    string group = ClsTabGroupStore.Classify(serviceTypeId, diimTypeId);
                    result[pair.Key] = group;
                    long dynamicDiimTypeId;
                    if (ClsTabGroupStore.TryGetDiimTypeId(group, out dynamicDiimTypeId) && !diimGroupNames.ContainsKey(group) && !String.IsNullOrEmpty(diimTypeName))
                        diimGroupNames[group] = diimTypeName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Catalog service of a row: the row itself or its nearest ancestor that carries a SERVICE_ID.
        /// </summary>
        private V_HIS_SERVICE FindClsService(TreeSereServADO row, Dictionary<string, TreeSereServADO> rowByKey)
        {
            V_HIS_SERVICE result = null;
            try
            {
                TreeSereServADO current = row;
                int depth = 0;
                while (current != null && depth < 10)
                {
                    V_HIS_SERVICE service;
                    if (current.SERVICE_ID > 0 && this.clsServiceById != null && this.clsServiceById.TryGetValue(current.SERVICE_ID, out service))
                    {
                        result = service;
                        break;
                    }
                    TreeSereServADO parent = null;
                    if (String.IsNullOrEmpty(current.PARENT_ID__IN_SETY) || !rowByKey.TryGetValue(current.PARENT_ID__IN_SETY, out parent))
                        break;
                    current = parent;
                    depth++;
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void EnsureClsServiceLookup()
        {
            try
            {
                List<V_HIS_SERVICE> services = BackendDataWorker.Get<V_HIS_SERVICE>();
                if (services != null && Object.ReferenceEquals(services, this.clsServiceSource) && this.clsServiceById != null)
                    return;
                this.clsServiceSource = services;
                this.clsServiceById = new Dictionary<long, V_HIS_SERVICE>();
                if (services == null)
                    return;
                foreach (V_HIS_SERVICE service in services)
                {
                    if (service != null && service.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA && !this.clsServiceById.ContainsKey(service.ID))
                        this.clsServiceById[service.ID] = service;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Number of services (service rows, distinct sere_serv) of each group, before the test filters.
        /// </summary>
        private static Dictionary<string, int> CountClsServices(List<TreeSereServADO> source, Dictionary<string, string> groupByKey)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            try
            {
                Dictionary<string, HashSet<string>> servicesByGroup = new Dictionary<string, HashSet<string>>();
                foreach (TreeSereServADO row in source)
                {
                    string group;
                    if (row == null || String.IsNullOrEmpty(row.CONCRETE_ID__IN_SETY) || !(row.IS_SERE_SERV_DATA || row.IS_SERE_SERV_HAS_MIC)
                        || !groupByKey.TryGetValue(row.CONCRETE_ID__IN_SETY, out group))
                        continue;
                    HashSet<string> services;
                    if (!servicesByGroup.TryGetValue(group, out services))
                    {
                        services = new HashSet<string>();
                        servicesByGroup[group] = services;
                    }
                    services.Add(row.ID > 0 ? "S" + row.ID : "K" + row.CONCRETE_ID__IN_SETY);
                }
                result = servicesByGroup.ToDictionary(o => o.Key, o => o.Value.Count);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Rows of one group tab: date rows (newest first) then the group rows; rows that hung under the
        /// service type row now hang directly under their date row (Ngày -> dịch vụ -> chỉ số).
        /// A filtered source (Test tab with Trên/Dưới ngưỡng) only keeps index rows: their service row (and parent
        /// service row) are added back from the full index so that each value stays under its date and test.
        /// </summary>
        private static List<KeyValuePair<TreeSereServADO, string>> BuildClsGroupRows(string code, List<TreeSereServADO> groupSource, Dictionary<string, TreeSereServADO> rowByKey, Dictionary<string, string> groupByKey)
        {
            List<KeyValuePair<TreeSereServADO, string>> result = new List<KeyValuePair<TreeSereServADO, string>>();
            try
            {
                HashSet<string> includedKeys = new HashSet<string>();
                List<TreeSereServADO> rows = SelectClsGroupRows(code, groupSource, groupByKey, includedKeys);
                rows.AddRange(GetMissingClsAncestors(rows, rowByKey, includedKeys));
                result = ReparentClsGroupRows(rows, rowByKey);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private static List<TreeSereServADO> SelectClsGroupRows(string code, List<TreeSereServADO> groupSource, Dictionary<string, string> groupByKey, HashSet<string> includedKeys)
        {
            List<TreeSereServADO> result = new List<TreeSereServADO>();
            try
            {
                foreach (TreeSereServADO row in groupSource)
                {
                    string group;
                    if (row == null || String.IsNullOrEmpty(row.CONCRETE_ID__IN_SETY)
                        || !groupByKey.TryGetValue(row.CONCRETE_ID__IN_SETY, out group) || group != code
                        || !includedKeys.Add(row.CONCRETE_ID__IN_SETY))
                        continue;
                    result.Add(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Ancestors below the service type row (service row, parent service row) missing from the kept rows.
        /// </summary>
        private static List<TreeSereServADO> GetMissingClsAncestors(List<TreeSereServADO> rows, Dictionary<string, TreeSereServADO> rowByKey, HashSet<string> includedKeys)
        {
            List<TreeSereServADO> result = new List<TreeSereServADO>();
            try
            {
                foreach (TreeSereServADO row in rows)
                {
                    string parentKey = row.PARENT_ID__IN_SETY;
                    int depth = 0;
                    while (!String.IsNullOrEmpty(parentKey) && parentKey.Split('_').Length >= 3 && !includedKeys.Contains(parentKey) && depth < 10)
                    {
                        TreeSereServADO parent;
                        if (!rowByKey.TryGetValue(parentKey, out parent))
                            break;
                        includedKeys.Add(parentKey);
                        result.Add(parent);
                        parentKey = parent.PARENT_ID__IN_SETY;
                        depth++;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Date rows (newest first) then the rows with their parent inside the tab: a row under a service type row
        /// ("date_serviceType") hangs under the date row.
        /// </summary>
        private static List<KeyValuePair<TreeSereServADO, string>> ReparentClsGroupRows(List<TreeSereServADO> rows, Dictionary<string, TreeSereServADO> rowByKey)
        {
            List<KeyValuePair<TreeSereServADO, string>> result = new List<KeyValuePair<TreeSereServADO, string>>();
            try
            {
                List<KeyValuePair<TreeSereServADO, string>> groupRows = new List<KeyValuePair<TreeSereServADO, string>>();
                HashSet<string> dateKeys = new HashSet<string>();
                foreach (TreeSereServADO row in rows)
                {
                    string parentKey = row.PARENT_ID__IN_SETY;
                    if (!String.IsNullOrEmpty(parentKey) && parentKey.Split('_').Length == 2)
                    {
                        parentKey = parentKey.Substring(0, parentKey.IndexOf('_'));
                        dateKeys.Add(parentKey);
                    }
                    groupRows.Add(new KeyValuePair<TreeSereServADO, string>(row, parentKey));
                }

                foreach (string dateKey in dateKeys.OrderByDescending(o => Inventec.Common.TypeConvert.Parse.ToInt64(o)))
                {
                    TreeSereServADO dateRow;
                    if (rowByKey.TryGetValue(dateKey, out dateRow))
                        result.Add(new KeyValuePair<TreeSereServADO, string>(dateRow, null));
                }
                result.AddRange(groupRows);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
