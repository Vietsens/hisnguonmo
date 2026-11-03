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
    /// Nguồn ưu tiên: duyệt theo từng dịch vụ (LIS_SAMPLE_SERVICE.SERVICE_APPROVER_* qua api/LisSampleService/Get).
    /// Không có dịch vụ nào được duyệt theo dịch vụ -> dùng duyệt cấp mẫu (LIS_SAMPLE.RESULT_APPROVAL_*).
    /// Dịch vụ không có nhóm cha -> nhóm "Khác".
    /// </summary>
    internal static class ServiceApproverBuilder
    {
        private const string OTHER_GROUP_NAME = "Khác";
        private const string ITEM_SEPARATOR = ", ";
        private const string URI_SAMPLE_SERVICE_GET = "api/LisSampleService/Get";

        /// <summary>Map JSON LIS_SAMPLE_SERVICE (đọc được 3 cột mới kể cả khi LIS.EFMODEL.dll phía client còn bản cũ).</summary>
        internal class SampleServiceApproveInfo
        {
            public long ID { get; set; }
            public long SAMPLE_ID { get; set; }
            public string SERVICE_CODE { get; set; }
            public string SERVICE_NAME { get; set; }
            public short? IS_NO_EXECUTE { get; set; }
            public string SERVICE_APPROVER_LOGINNAME { get; set; }
            public string SERVICE_APPROVER_USERNAME { get; set; }
            public long? SERVICE_APPROVE_TIME { get; set; }
        }

        private class Aggregator
        {
            private readonly Dictionary<string, V_HIS_SERVICE> serviceByCode;
            private readonly Dictionary<long, V_HIS_SERVICE> serviceById;
            private readonly Dictionary<string, ServiceApproverADO> dicRow = new Dictionary<string, ServiceApproverADO>();
            private readonly Dictionary<string, List<string>> dicServiceNames = new Dictionary<string, List<string>>();

            internal Aggregator(List<V_HIS_SERVICE> allServices)
            {
                serviceByCode = allServices.Where(o => !String.IsNullOrEmpty(o.SERVICE_CODE))
                    .GroupBy(o => o.SERVICE_CODE).ToDictionary(g => g.Key, g => g.First());
                serviceById = allServices.GroupBy(o => o.ID).ToDictionary(g => g.Key, g => g.First());
            }

            internal void Add(string serviceCode, string serviceName, string loginname, string username, long approveTime)
            {
                V_HIS_SERVICE parent = null;
                V_HIS_SERVICE service;
                if (!String.IsNullOrEmpty(serviceCode) && serviceByCode.TryGetValue(serviceCode, out service) && service.PARENT_ID.HasValue)
                    serviceById.TryGetValue(service.PARENT_ID.Value, out parent);

                string parentCode = parent != null ? parent.SERVICE_CODE : "";
                string key = parentCode + "|" + loginname;
                ServiceApproverADO row;
                if (!dicRow.TryGetValue(key, out row))
                {
                    row = new ServiceApproverADO();
                    row.APPROVER_LOGINNAME = loginname;
                    row.APPROVER_USERNAME = username;
                    row.PARENT_SERVICE_CODE = parentCode;
                    row.PARENT_SERVICE_NAME = parent != null ? parent.SERVICE_NAME : OTHER_GROUP_NAME;
                    dicRow[key] = row;
                    dicServiceNames[key] = new List<string>();
                }
                if (approveTime > row.APPROVE_TIME)
                    row.APPROVE_TIME = approveTime;
                if (!String.IsNullOrEmpty(serviceName) && !dicServiceNames[key].Contains(serviceName))
                    dicServiceNames[key].Add(serviceName);
            }

            internal List<ServiceApproverADO> ToList()
            {
                foreach (var pair in dicRow)
                {
                    pair.Value.APPROVE_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(pair.Value.APPROVE_TIME);
                    pair.Value.SERVICE_NAMES = String.Join("; ", dicServiceNames[pair.Key]);
                }
                return dicRow.Values.OrderBy(o => o.APPROVE_TIME).ThenBy(o => o.PARENT_SERVICE_CODE).ToList();
            }
        }

        internal static List<ServiceApproverADO> Build(IEnumerable<V_LIS_SAMPLE> samples, List<V_LIS_RESULT> results, List<V_HIS_SERVICE> listService)
        {
            List<ServiceApproverADO> result = new List<ServiceApproverADO>();
            try
            {
                var sampleList = samples != null ? samples.Where(o => o != null).ToList() : new List<V_LIS_SAMPLE>();
                if (!sampleList.Any() || results == null || !results.Any())
                    return result;

                Aggregator aggregator = new Aggregator(GetAllServices(listService));

                // 1. Duyệt theo từng dịch vụ
                var serviceApproves = GetServiceApproves(sampleList.Select(o => o.ID).Distinct().ToList())
                    .Where(o => o.SERVICE_APPROVE_TIME.HasValue && !String.IsNullOrEmpty(o.SERVICE_APPROVER_LOGINNAME) && o.IS_NO_EXECUTE != 1)
                    .ToList();
                if (serviceApproves.Any())
                {
                    foreach (var item in serviceApproves)
                    {
                        aggregator.Add(item.SERVICE_CODE, item.SERVICE_NAME, item.SERVICE_APPROVER_LOGINNAME, item.SERVICE_APPROVER_USERNAME, item.SERVICE_APPROVE_TIME.Value);
                    }
                    return aggregator.ToList();
                }

                // 2. Dự phòng: duyệt cấp mẫu (dữ liệu trước khi có duyệt theo dịch vụ)
                var resultsBySample = results.Where(o => o.SAMPLE_ID.HasValue && o.IS_NO_EXECUTE != 1).ToLookup(o => o.SAMPLE_ID.Value);
                foreach (var sample in sampleList.Where(o => o.RESULT_APPROVAL_TIME.HasValue && !String.IsNullOrEmpty(o.RESULT_APPROVAL_LOGINNAME)))
                {
                    var sampleServices = resultsBySample[sample.ID]
                        .Where(o => !String.IsNullOrEmpty(o.SERVICE_CODE))
                        .GroupBy(o => o.SERVICE_CODE)
                        .Select(g => g.First());
                    foreach (var item in sampleServices)
                    {
                        aggregator.Add(item.SERVICE_CODE, item.SERVICE_NAME, sample.RESULT_APPROVAL_LOGINNAME, sample.RESULT_APPROVAL_USERNAME, sample.RESULT_APPROVAL_TIME.Value);
                    }
                }
                result = aggregator.ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new List<ServiceApproverADO>();
            }
            return result;
        }

        private static List<V_HIS_SERVICE> GetAllServices(List<V_HIS_SERVICE> listService)
        {
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
            return allServices;
        }

        private static List<SampleServiceApproveInfo> GetServiceApproves(List<long> sampleIds)
        {
            List<SampleServiceApproveInfo> result = null;
            try
            {
                if (sampleIds != null && sampleIds.Any())
                {
                    Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                    var filter = new { SAMPLE_IDs = sampleIds };
                    result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<SampleServiceApproveInfo>>(URI_SAMPLE_SERVICE_GET, HIS.Desktop.ApiConsumer.ApiConsumers.LisConsumer, filter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result ?? new List<SampleServiceApproveInfo>();
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
