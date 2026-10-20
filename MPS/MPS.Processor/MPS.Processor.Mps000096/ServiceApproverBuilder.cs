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
using LIS.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MPS.Processor.Mps000096
{
    /// <summary>
    /// Dựng danh sách người duyệt KQ theo nhóm cha dịch vụ:
    /// gom theo (nhóm cha, tài khoản duyệt) — nhiều dịch vụ cùng nhóm cùng người -> 1 dòng; khác người -> nhiều dòng.
    /// Dịch vụ không có nhóm cha -> nhóm "Khác".
    /// </summary>
    internal static class ServiceApproverBuilder
    {
        private const string OTHER_GROUP_NAME = "Khác";
        private const string ITEM_SEPARATOR = ", ";

        internal static List<ServiceApproverADO> Build(IEnumerable<V_LIS_SAMPLE> samples, List<V_LIS_RESULT> results, List<V_HIS_SERVICE> listService)
        {
            List<ServiceApproverADO> result = new List<ServiceApproverADO>();
            try
            {
                var approvedSamples = samples != null
                    ? samples.Where(o => o != null && o.RESULT_APPROVAL_TIME.HasValue && !String.IsNullOrEmpty(o.RESULT_APPROVAL_LOGINNAME)).ToList()
                    : new List<V_LIS_SAMPLE>();
                if (!approvedSamples.Any() || results == null || !results.Any())
                    return result;

                List<V_HIS_SERVICE> allServices = new List<V_HIS_SERVICE>();
                if (listService != null)
                    allServices.AddRange(listService);
                try
                {
                    // Danh sách dịch vụ truyền vào chỉ chứa dịch vụ con -> lấy dịch vụ cha từ cache danh mục.
                    var cacheServices = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>();
                    if (cacheServices != null)
                        allServices.AddRange(cacheServices);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                var serviceByCode = allServices.Where(o => !String.IsNullOrEmpty(o.SERVICE_CODE))
                    .GroupBy(o => o.SERVICE_CODE).ToDictionary(g => g.Key, g => g.First());
                var serviceById = allServices.GroupBy(o => o.ID).ToDictionary(g => g.Key, g => g.First());

                var resultsBySample = results.Where(o => o.SAMPLE_ID.HasValue && o.IS_NO_EXECUTE != 1)
                    .ToLookup(o => o.SAMPLE_ID.Value);

                var dicRow = new Dictionary<string, ServiceApproverADO>();
                var dicServiceNames = new Dictionary<string, List<string>>();
                foreach (var sample in approvedSamples)
                {
                    var sampleServices = resultsBySample[sample.ID]
                        .Where(o => !String.IsNullOrEmpty(o.SERVICE_CODE))
                        .GroupBy(o => o.SERVICE_CODE)
                        .Select(g => g.First());
                    foreach (var item in sampleServices)
                    {
                        V_HIS_SERVICE parent = null;
                        V_HIS_SERVICE service;
                        if (serviceByCode.TryGetValue(item.SERVICE_CODE, out service) && service.PARENT_ID.HasValue)
                            serviceById.TryGetValue(service.PARENT_ID.Value, out parent);

                        string parentCode = parent != null ? parent.SERVICE_CODE : "";
                        string key = parentCode + "|" + sample.RESULT_APPROVAL_LOGINNAME;
                        ServiceApproverADO row;
                        if (!dicRow.TryGetValue(key, out row))
                        {
                            row = new ServiceApproverADO();
                            row.APPROVER_LOGINNAME = sample.RESULT_APPROVAL_LOGINNAME;
                            row.APPROVER_USERNAME = sample.RESULT_APPROVAL_USERNAME;
                            row.PARENT_SERVICE_CODE = parentCode;
                            row.PARENT_SERVICE_NAME = parent != null ? parent.SERVICE_NAME : OTHER_GROUP_NAME;
                            dicRow[key] = row;
                            dicServiceNames[key] = new List<string>();
                        }
                        if (sample.RESULT_APPROVAL_TIME.Value > row.APPROVE_TIME)
                            row.APPROVE_TIME = sample.RESULT_APPROVAL_TIME.Value;
                        if (!dicServiceNames[key].Contains(item.SERVICE_NAME))
                            dicServiceNames[key].Add(item.SERVICE_NAME);
                    }
                }

                foreach (var pair in dicRow)
                {
                    pair.Value.APPROVE_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(pair.Value.APPROVE_TIME);
                    pair.Value.SERVICE_NAMES = String.Join("; ", dicServiceNames[pair.Key]);
                }
                result = dicRow.Values.OrderBy(o => o.APPROVE_TIME).ThenBy(o => o.PARENT_SERVICE_CODE).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new List<ServiceApproverADO>();
            }
            return result;
        }

        /// <summary>"Việt - XNSH - 12/10/2026 10:00, Duy - XNHH - 12/10/2026 10:10". Nhóm "Khác" dùng tên nhóm thay mã.</summary>
        internal static string ToKeyString(List<ServiceApproverADO> rows)
        {
            if (rows == null || !rows.Any())
                return "";
            return String.Join(ITEM_SEPARATOR, rows.Select(o => String.Format("{0} - {1} - {2}",
                o.APPROVER_USERNAME,
                !String.IsNullOrEmpty(o.PARENT_SERVICE_CODE) ? o.PARENT_SERVICE_CODE : o.PARENT_SERVICE_NAME,
                o.APPROVE_TIME_STR)));
        }
    }
}
