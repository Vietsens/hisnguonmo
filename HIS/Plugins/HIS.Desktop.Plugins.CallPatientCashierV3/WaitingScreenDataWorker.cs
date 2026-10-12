using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.CallPatientCashierV3
{
    /// <summary>
    /// Lấy danh sách hồ sơ chờ nộp viện phí cho tivi (R1–R6, R18):
    /// 1. api/HisTreatment/GetFeeView theo điều kiện lọc của tivi (chi nhánh, chưa khóa viện phí, ngày vào, khoa, loại điều trị, đối tượng)
    /// 2. Tính "BN cần nộp thêm" đúng công thức màn Viện phí, lọc trạng thái nợ
    /// 3. api/HisSereServ/GetView cho hồ sơ mới / có thay đổi để gom nhóm dịch vụ (có bộ nhớ đệm theo hồ sơ)
    /// Chạy trên luồng nền; mỗi lần gọi trả về danh sách mới hoặc null nếu lỗi (giữ dữ liệu cũ trên tivi).
    /// </summary>
    internal class WaitingScreenDataWorker
    {
        private const int MAX_TREATMENTS = 500;
        private const int SERE_SERV_BATCH_SIZE = 100;

        private class GroupCacheItem
        {
            public long? ModifyTime { get; set; }
            public bool SplitCdha { get; set; }
            public List<ServiceGroupItemADO> Groups { get; set; }
        }

        private readonly Dictionary<long, GroupCacheItem> groupCache = new Dictionary<long, GroupCacheItem>();

        internal List<WaitingPatientADO> Load(WaitingScreenConfigADO cfg)
        {
            if (cfg == null)
            {
                Inventec.Common.Logging.LogSystem.Info(LOG_PREFIX + "Load: cfg null -> bo qua");
                return null;
            }
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            List<V_HIS_TREATMENT_FEE> fees = LoadTreatmentFees(cfg);
            if (fees == null)
            {
                Inventec.Common.Logging.LogSystem.Info(LOG_PREFIX + "Load: GetFeeView loi/null -> giu du lieu cu tren tivi");
                return null;
            }

            // Lọc khoa (khoa hiện tại của bệnh nhân) rồi trạng thái nợ trước để giảm số hồ sơ phải tra dịch vụ
            bool filterDepartment = cfg.DepartmentIds != null && cfg.DepartmentIds.Count > 0;
            int afterDepartment = 0;
            List<KeyValuePair<V_HIS_TREATMENT_FEE, decimal>> candidates = new List<KeyValuePair<V_HIS_TREATMENT_FEE, decimal>>();
            foreach (var fee in fees)
            {
                if (filterDepartment)
                {
                    long? departmentId = GetCurrentDepartmentId(fee);
                    if (!departmentId.HasValue || !cfg.DepartmentIds.Contains(departmentId.Value)) continue;
                }
                afterDepartment++;
                decimal receiveMore = CalculateReceiveMore(fee);
                if (cfg.DebtStatus == 1 && receiveMore <= 0) continue;
                if (cfg.DebtStatus == 2 && receiveMore > 0) continue;
                candidates.Add(new KeyValuePair<V_HIS_TREATMENT_FEE, decimal>(fee, receiveMore));
            }

            RefreshGroupCache(candidates.Select(o => o.Key).ToList(), cfg.SplitCdhaByExecuteGroup);
            PruneGroupCache(fees);

            List<WaitingPatientADO> result = new List<WaitingPatientADO>();
            foreach (var kv in candidates.OrderBy(o => o.Key.IN_TIME).ThenBy(o => o.Key.ID))
            {
                V_HIS_TREATMENT_FEE fee = kv.Key;
                GroupCacheItem cacheItem;
                List<ServiceGroupItemADO> groups = groupCache.TryGetValue(fee.ID, out cacheItem) && cacheItem.Groups != null
                    ? cacheItem.Groups
                    : new List<ServiceGroupItemADO>();

                // Lọc nhóm (R8, chốt 08/10/2026): tích nhóm nào thì tivi chỉ hiện nhóm đó.
                // - Hồ sơ hiện khi có ít nhất một nhóm được tích.
                // - Cột "Nhóm dịch vụ" chỉ ghi các nhóm được tích (BN có khám + siêu âm + xét nghiệm, tích mỗi Khám → hiện "KHÁM").
                // - Không tích gì = hiện mọi bệnh nhân với đủ nhóm.
                List<ServiceGroupItemADO> displayGroups = groups;
                if (cfg.ServiceGroupKeys != null && cfg.ServiceGroupKeys.Count > 0)
                {
                    HashSet<string> selected = new HashSet<string>(cfg.ServiceGroupKeys);
                    displayGroups = groups.Where(g => selected.Contains(g.Key)).ToList();
                    if (displayGroups.Count == 0)
                    {
                        continue;
                    }
                }

                WaitingPatientADO row = new WaitingPatientADO();
                row.TREATMENT_ID = fee.ID;
                row.TREATMENT_CODE = fee.TREATMENT_CODE;
                row.PATIENT_NAME = (fee.TDL_PATIENT_NAME ?? "").Trim().ToUpper();
                row.YEAR_OF_BIRTH = GetYear(fee.TDL_PATIENT_DOB);
                row.SERVICE_GROUPS = string.Join(", ", displayGroups.Select(g => (g.Name ?? "").Trim().ToUpper()));
                row.IN_TIME = fee.IN_TIME;
                row.RECEIVE_MORE = kv.Value;
                row.GroupKeys = groups.Select(g => g.Key).ToList();
                result.Add(row);
            }

            // Log chẩn đoán: chỉ ghi số lượng và mã điều trị, không ghi tên bệnh nhân
            int noGroup = candidates.Count(o => { GroupCacheItem ci; return !groupCache.TryGetValue(o.Key.ID, out ci) || ci.Groups == null || ci.Groups.Count == 0; });
            Inventec.Common.Logging.LogSystem.Info(LOG_PREFIX + string.Format(
                "Load xong {0}ms: GetFeeView={1} | sau loc khoa hien tai={8} | sau loc no (DebtStatus={2})={3} | ho so khong co nhom DV={4} | sau loc nhom ({5} nhom chon)={6} | ma dieu tri hien thi=[{7}]",
                sw.ElapsedMilliseconds, fees.Count, cfg.DebtStatus, candidates.Count, noGroup,
                cfg.ServiceGroupKeys != null ? cfg.ServiceGroupKeys.Count : 0, result.Count,
                string.Join(",", result.Take(30).Select(o => o.TREATMENT_CODE)), afterDepartment));
            if (fees.Count > 0 && result.Count == 0)
            {
                Inventec.Common.Logging.LogSystem.Info(LOG_PREFIX + "Co ho so tu API nhung bi loc het. Ma dieu tri API tra ve=["
                    + string.Join(",", fees.Take(30).Select(o => o.TREATMENT_CODE + "(no=" + CalculateReceiveMore(o).ToString("0") + ",khoa hien tai=" + GetCurrentDepartmentId(o) + ")")) + "]");
            }
            return result;
        }

        private const string LOG_PREFIX = "CallPatientCashierV3 | ";

        /// <summary>
        /// Khoa hiện tại của bệnh nhân: khoa điều trị cuối cùng; trống thì khoa vào viện; trống nữa thì khoa kết thúc.
        /// (Không dùng khoa kết thúc làm chính vì trường này chỉ có khi đã kết thúc điều trị.)
        /// </summary>
        internal static long? GetCurrentDepartmentId(V_HIS_TREATMENT_FEE t)
        {
            if (t == null) return null;
            if (t.LAST_DEPARTMENT_ID.HasValue) return t.LAST_DEPARTMENT_ID;
            if (t.IN_DEPARTMENT_ID.HasValue) return t.IN_DEPARTMENT_ID;
            return t.END_DEPARTMENT_ID;
        }

        internal static string GetYear(long dob)
        {
            string s = dob.ToString();
            return s.Length >= 4 ? s.Substring(0, 4) : "";
        }

        /// <summary>
        /// "BN cần nộp thêm" — sao đúng công thức khối tổng tiền của màn Viện phí (R4):
        /// phải trả − (tạm ứng + đã thanh toán − chuyển − quỹ − hoàn ứng thường − miễn giảm + tiền khóa) − quỹ − miễn giảm.
        /// </summary>
        internal static decimal CalculateReceiveMore(V_HIS_TREATMENT_FEE t)
        {
            if (t == null) return 0;
            decimal totalReceive = ((t.TOTAL_DEPOSIT_AMOUNT ?? 0) + (t.TOTAL_BILL_AMOUNT ?? 0)
                - (t.TOTAL_BILL_TRANSFER_AMOUNT ?? 0) - (t.TOTAL_BILL_FUND ?? 0)
                - ((t.TOTAL_REPAY_AMOUNT ?? 0) - (t.TOTAL_COMPENSATION_REPAY ?? 0)))
                - (t.TOTAL_BILL_EXEMPTION ?? 0) + (t.LOCKING_AMOUNT ?? 0);
            decimal totalReceiveMore = (t.TOTAL_PATIENT_PRICE ?? 0) - totalReceive - (t.TOTAL_BILL_FUND ?? 0);
            return totalReceiveMore - (t.TOTAL_BILL_EXEMPTION ?? 0);
        }

        private List<V_HIS_TREATMENT_FEE> LoadTreatmentFees(WaitingScreenConfigADO cfg)
        {
            try
            {
                CommonParam param = new CommonParam(0, MAX_TREATMENTS);
                HisTreatmentFeeViewFilter filter = new HisTreatmentFeeViewFilter();
                filter.BRANCH_ID = BranchDataWorker.Branch.ID;
                filter.IS_ACTIVE = 1; // chưa khóa viện phí (R1, R2)

                long from;
                long to;
                if (cfg.AlwaysToday || !cfg.InDateFrom.HasValue || !cfg.InDateTo.HasValue)
                {
                    string today = DateTime.Now.ToString("yyyyMMdd");
                    from = long.Parse(today + "000000");
                    to = long.Parse(today + "235959");
                }
                else
                {
                    from = cfg.InDateFrom.Value;
                    to = cfg.InDateTo.Value;
                }
                filter.IN_DATE_FROM = from;
                filter.IN_DATE_TO = to;

                // Khoa KHÔNG gửi lên API: filter END_DEPARTMENT_IDs so với khoa kết thúc, trường này chỉ có khi đã kết thúc điều trị.
                // Lọc khoa làm ở máy trạm theo khoa hiện tại của bệnh nhân (xem GetCurrentDepartmentId).
                if (cfg.TreatmentTypeIds != null && cfg.TreatmentTypeIds.Count > 0)
                {
                    filter.TDL_TREATMENT_TYPE_IDs = cfg.TreatmentTypeIds;
                }
                if (cfg.PatientTypeIds != null && cfg.PatientTypeIds.Count > 0)
                {
                    filter.TDL_PATIENT_TYPE_IDs = cfg.PatientTypeIds;
                }
                filter.ORDER_FIELD = "IN_TIME";
                filter.ORDER_DIRECTION = "ASC";

                Inventec.Common.Logging.LogSystem.Info(LOG_PREFIX + string.Format(
                    "GetFeeView gui: BRANCH_ID={0}, IS_ACTIVE=1, IN_DATE {1}-{2} (AlwaysToday={3}), khoa hien tai loc o may tram=[{4}], TDL_TREATMENT_TYPE_IDs=[{5}], TDL_PATIENT_TYPE_IDs=[{6}], limit={7}",
                    filter.BRANCH_ID, from, to, cfg.AlwaysToday,
                    cfg.DepartmentIds != null ? string.Join(",", cfg.DepartmentIds) : "",
                    filter.TDL_TREATMENT_TYPE_IDs != null ? string.Join(",", filter.TDL_TREATMENT_TYPE_IDs) : "",
                    filter.TDL_PATIENT_TYPE_IDs != null ? string.Join(",", filter.TDL_PATIENT_TYPE_IDs) : "",
                    MAX_TREATMENTS));
                var result = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, filter, param);
                if (result == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn(LOG_PREFIX + "GetFeeView tra ve null. HasException=" + param.HasException
                        + ", Messages=[" + (param.Messages != null ? string.Join(" | ", param.Messages) : "") + "]"
                        + ", BugCodes=[" + (param.BugCodes != null ? string.Join(",", param.BugCodes) : "") + "]");
                    return null;
                }
                Inventec.Common.Logging.LogSystem.Info(LOG_PREFIX + string.Format("GetFeeView nhan {0} ho so (Param.Count={1})", result.Count, param.Count));
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>Tra dịch vụ cho hồ sơ mới xuất hiện hoặc có thay đổi so với lần trước (theo MODIFY_TIME và cờ tách CĐHA).</summary>
        private void RefreshGroupCache(List<V_HIS_TREATMENT_FEE> fees, bool splitCdha)
        {
            try
            {
                List<V_HIS_TREATMENT_FEE> needed = new List<V_HIS_TREATMENT_FEE>();
                foreach (var fee in fees)
                {
                    GroupCacheItem item;
                    if (!groupCache.TryGetValue(fee.ID, out item) || item.ModifyTime != fee.MODIFY_TIME || item.SplitCdha != splitCdha)
                    {
                        needed.Add(fee);
                    }
                }
                if (needed.Count == 0) return;

                var serviceTypes = ServiceGroupCatalog.GetServiceTypes();
                var executeGroups = ServiceGroupCatalog.GetExecuteGroups();

                for (int i = 0; i < needed.Count; i += SERE_SERV_BATCH_SIZE)
                {
                    List<V_HIS_TREATMENT_FEE> batch = needed.Skip(i).Take(SERE_SERV_BATCH_SIZE).ToList();
                    List<V_HIS_SERE_SERV> sereServs = LoadSereServs(batch.Select(o => o.ID).ToList());
                    if (sereServs == null) continue; // lỗi → giữ cache cũ, lần sau tra lại

                    var byTreatment = sereServs.Where(o => o.TDL_TREATMENT_ID.HasValue)
                        .GroupBy(o => o.TDL_TREATMENT_ID.Value)
                        .ToDictionary(g => g.Key, g => g.ToList());

                    foreach (var fee in batch)
                    {
                        List<V_HIS_SERE_SERV> list;
                        byTreatment.TryGetValue(fee.ID, out list);
                        groupCache[fee.ID] = new GroupCacheItem
                        {
                            ModifyTime = fee.MODIFY_TIME,
                            SplitCdha = splitCdha,
                            Groups = ServiceGroupCatalog.BuildGroups(list, splitCdha, serviceTypes, executeGroups)
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PruneGroupCache(List<V_HIS_TREATMENT_FEE> fees)
        {
            try
            {
                HashSet<long> ids = new HashSet<long>(fees.Select(o => o.ID));
                List<long> toRemove = groupCache.Keys.Where(k => !ids.Contains(k)).ToList();
                foreach (long k in toRemove)
                {
                    groupCache.Remove(k);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<V_HIS_SERE_SERV> LoadSereServs(List<long> treatmentIds)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSereServViewFilter filter = new HisSereServViewFilter();
                filter.TREATMENT_IDs = treatmentIds;
                var result = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumers.MosConsumer, filter, param);
                if (result == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn(LOG_PREFIX + "HisSereServ/GetView tra ve null cho " + treatmentIds.Count + " ho so. HasException=" + param.HasException
                        + ", Messages=[" + (param.Messages != null ? string.Join(" | ", param.Messages) : "") + "]");
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info(LOG_PREFIX + string.Format("HisSereServ/GetView: {0} ho so -> {1} dong dich vu", treatmentIds.Count, result.Count));
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }
    }
}
