using HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood.ADOs;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood.Base
{
    /// <summary>
    /// Xử lý chỉ số xét nghiệm hòa hợp truyền máu.
    /// Config format (mới): N bộ — mỗi bộ 3 mã chỉ số (A|B|C) tương ứng 1 túi máu.
    /// Ví dụ: "PM_H1MTM|331966|331964;PM_H2MTM|331992|331990"
    ///   - A = mã chỉ số "Mã túi máu" (giá trị = mã túi thực tế)
    ///   - B = mã chỉ số "Hòa hợp muối"
    ///   - C = mã chỉ số "Hòa hợp anti-globulin"
    /// </summary>
    public class TestIndexProcessor
    {
        /// <summary>Một bộ cấu hình = 3 mã chỉ số = 1 túi máu.</summary>
        private class HarmonyConfigSet
        {
            public string BloodCode { get; set; }
            public string SaltCode { get; set; }
            public string AntiGlobulinCode { get; set; }
        }

        // Danh sách bộ cấu hình đã parse
        private List<HarmonyConfigSet> configSets = new List<HarmonyConfigSet>();

        // Toàn bộ chỉ số xét nghiệm thuộc đợt điều trị
        private List<TestIndexResultADO> allTestIndexResults = new List<TestIndexResultADO>();

        // Danh sách dropdown XN hòa hợp — mỗi dòng 1 túi/1 y lệnh
        public List<TestHarmonyADO> TestHarmonyList { get; set; }

        public TestIndexProcessor()
        {
            TestHarmonyList = new List<TestHarmonyADO>();
        }

        /// <summary>
        /// Parse config "A|B|C;A|B|C;...". Bộ không đủ 3 mã sẽ bị bỏ qua.
        /// </summary>
        private void ParseConfig()
        {
            try
            {
                configSets.Clear();

                string config = Config.ConfigKey.BloodHarmonyTestIndexConfig;
                if (string.IsNullOrWhiteSpace(config))
                {
                    Inventec.Common.Logging.LogSystem.Warn(
                        "Chua cau hinh HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood.BloodHarmonyTestIndex");
                    return;
                }

                string[] sets = config.Split(';');
                foreach (var rawSet in sets)
                {
                    if (string.IsNullOrWhiteSpace(rawSet)) continue;

                    string[] codes = rawSet.Split('|');
                    if (codes.Length < 3)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(
                            "Bo cau hinh khong du 3 ma, bo qua: " + rawSet);
                        continue;
                    }

                    string a = (codes[0] ?? "").Trim();
                    string b = (codes[1] ?? "").Trim();
                    string c = (codes[2] ?? "").Trim();

                    if (string.IsNullOrWhiteSpace(a))
                    {
                        Inventec.Common.Logging.LogSystem.Warn(
                            "Bo cau hinh thieu ma chi so mau (A), bo qua: " + rawSet);
                        continue;
                    }

                    configSets.Add(new HarmonyConfigSet
                    {
                        BloodCode = a,
                        SaltCode = b,
                        AntiGlobulinCode = c
                    });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Tải chỉ số xét nghiệm theo TREATMENT_ID và xây dựng danh sách dropdown.
        /// </summary>
        public void LoadTestIndexData(long treatmentId)
        {
            try
            {
                ParseConfig();
                TestHarmonyList.Clear();
                allTestIndexResults.Clear();

                if (configSets.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn(
                        "Khong co bo cau hinh hop le cho XN hoa hop");
                    return;
                }

                HisSereServTeinViewFilter filter = new HisSereServTeinViewFilter();
                filter.TDL_TREATMENT_ID = treatmentId;

                List<V_HIS_SERE_SERV_TEIN> sereServTeins = new BackendAdapter(new CommonParam())
                    .Get<List<V_HIS_SERE_SERV_TEIN>>(
                        "api/HisSereServTein/GetView",
                        HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, null);

                if (sereServTeins == null || sereServTeins.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Info(
                        "Khong co chi so xet nghiem cho treatment: " + treatmentId);
                    return;
                }

                allTestIndexResults = sereServTeins.Select(o => new TestIndexResultADO
                {
                    SERE_SERV_TEIN_ID = o.ID,
                    TEST_INDEX_CODE = o.TEST_INDEX_CODE,
                    TEST_INDEX_NAME = o.TEST_INDEX_NAME,
                    VALUE = o.VALUE,
                    SERE_SERV_ID = o.SERE_SERV_ID,
                    TREATMENT_ID = o.TDL_TREATMENT_ID ?? 0,
                    SERVICE_REQ_ID = o.TDL_SERVICE_REQ_ID,
                    MODIFY_TIME = o.MODIFY_TIME
                }).ToList();

                BuildTestHarmonyList();

                Inventec.Common.Logging.LogSystem.Info(
                    "Load test index data: configSets=" + configSets.Count
                    + ", allTestIndex=" + allTestIndexResults.Count
                    + ", TestHarmony=" + TestHarmonyList.Count);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Xây dựng danh sách dropdown XN hòa hợp:
        ///   - Nhóm chỉ số theo y lệnh (SERVICE_REQ_ID)
        ///   - Với mỗi y lệnh × mỗi bộ cấu hình: tìm bản ghi A mới nhất → tạo 1 dòng
        ///   - Trong cùng y lệnh, tìm bản ghi B và C mới nhất theo mã (có thể rỗng)
        ///   - Sắp xếp toàn bộ dropdown theo thời gian sửa A giảm dần
        /// </summary>
        private void BuildTestHarmonyList()
        {
            try
            {
                // Lookup theo y lệnh × mã chỉ số → O(1) cho mỗi lần tìm
                var byServiceReq = allTestIndexResults
                    .Where(o => o.SERVICE_REQ_ID.HasValue)
                    .ToLookup(o => o.SERVICE_REQ_ID.Value);

                long nextRowId = 1;

                foreach (var serviceReqGroup in byServiceReq)
                {
                    long serviceReqId = serviceReqGroup.Key;

                    // Lookup mã chỉ số → list bản ghi cho y lệnh này (case-insensitive)
                    var byCode = serviceReqGroup
                        .Where(o => !string.IsNullOrWhiteSpace(o.TEST_INDEX_CODE))
                        .ToLookup(o => o.TEST_INDEX_CODE.Trim(), StringComparer.OrdinalIgnoreCase);

                    foreach (var set in configSets)
                    {
                        // Mã túi (A) — bắt buộc có
                        TestIndexResultADO bloodLatest = byCode[set.BloodCode]
                            .OrderByDescending(o => o.MODIFY_TIME ?? 0)
                            .ThenByDescending(o => o.SERE_SERV_TEIN_ID)
                            .FirstOrDefault();

                        if (bloodLatest == null) continue;

                        // Hòa hợp muối (B) — ƯU TIÊN cùng SERE_SERV_ID với A
                        // (cùng dịch vụ thực hiện = cùng túi máu → pair A/B/C đúng bộ).
                        // Fallback: latest theo MODIFY_TIME khi B nằm ở sere_serv khác.
                        TestIndexResultADO saltLatest = null;
                        if (!string.IsNullOrWhiteSpace(set.SaltCode))
                        {
                            saltLatest = byCode[set.SaltCode]
                                .Where(o => o.SERE_SERV_ID == bloodLatest.SERE_SERV_ID)
                                .OrderByDescending(o => o.MODIFY_TIME ?? 0)
                                .ThenByDescending(o => o.SERE_SERV_TEIN_ID)
                                .FirstOrDefault();
                            if (saltLatest == null)
                            {
                                saltLatest = byCode[set.SaltCode]
                                    .OrderByDescending(o => o.MODIFY_TIME ?? 0)
                                    .ThenByDescending(o => o.SERE_SERV_TEIN_ID)
                                    .FirstOrDefault();
                            }
                        }

                        // Hòa hợp anti-globulin (C) — ƯU TIÊN cùng SERE_SERV_ID với A
                        TestIndexResultADO antiLatest = null;
                        if (!string.IsNullOrWhiteSpace(set.AntiGlobulinCode))
                        {
                            antiLatest = byCode[set.AntiGlobulinCode]
                                .Where(o => o.SERE_SERV_ID == bloodLatest.SERE_SERV_ID)
                                .OrderByDescending(o => o.MODIFY_TIME ?? 0)
                                .ThenByDescending(o => o.SERE_SERV_TEIN_ID)
                                .FirstOrDefault();
                            if (antiLatest == null)
                            {
                                antiLatest = byCode[set.AntiGlobulinCode]
                                    .OrderByDescending(o => o.MODIFY_TIME ?? 0)
                                    .ThenByDescending(o => o.SERE_SERV_TEIN_ID)
                                    .FirstOrDefault();
                            }
                        }

                        TestHarmonyList.Add(new TestHarmonyADO
                        {
                            ROW_ID = nextRowId++,
                            SERVICE_REQ_ID = serviceReqId,
                            MODIFY_TIME = bloodLatest.MODIFY_TIME,
                            BLOOD_VALUE = bloodLatest.VALUE ?? "",
                            SALT_VALUE = saltLatest != null ? (saltLatest.VALUE ?? "") : "",
                            ANTI_GLOBULIN_VALUE = antiLatest != null ? (antiLatest.VALUE ?? "") : ""
                        });
                    }
                }

                // Sắp xếp theo MODIFY_TIME của A giảm dần (không tách block theo y lệnh)
                TestHarmonyList = TestHarmonyList
                    .OrderByDescending(o => o.MODIFY_TIME ?? 0)
                    .ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Tìm dòng dropdown khớp BLOOD_VALUE (mã túi). Nhiều dòng khớp → lấy mới nhất.
        /// </summary>
        public TestHarmonyADO FindHarmonyByBloodCode(string bloodCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(bloodCode)) return null;

                string trimmed = bloodCode.Trim();
                return TestHarmonyList
                    .Where(o => string.Equals(
                        (o.BLOOD_VALUE ?? "").Trim(),
                        trimmed,
                        StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(o => o.MODIFY_TIME ?? 0)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>
        /// Tìm dòng dropdown theo ROW_ID (giá trị EditValue của cboXNHH).
        /// </summary>
        public TestHarmonyADO FindHarmonyByRowId(long rowId)
        {
            try
            {
                return TestHarmonyList.FirstOrDefault(o => o.ROW_ID == rowId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }
    }
}
