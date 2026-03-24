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
    public class TestIndexProcessor
    {
        // Danh sách A: Chỉ số xét nghiệm túi máu
        public List<TestIndexResultADO> BloodTestIndexList { get; set; }

        // Danh sách B: Chỉ số xét nghiệm môi trường muối
        public List<TestIndexResultADO> SaltEnviTestIndexList { get; set; }

        // Danh sách C: Chỉ số xét nghiệm anti globulin
        public List<TestIndexResultADO> AntiGlobulinTestIndexList { get; set; }

        // Danh sách cho combobox XN hòa hợp
        public List<TestHarmonyADO> TestHarmonyList { get; set; }

        private List<string> bloodTestIndexCodes = new List<string>();
        private List<string> saltEnviTestIndexCodes = new List<string>();
        private List<string> antiGlobulinTestIndexCodes = new List<string>();

        public TestIndexProcessor()
        {
            BloodTestIndexList = new List<TestIndexResultADO>();
            SaltEnviTestIndexList = new List<TestIndexResultADO>();
            AntiGlobulinTestIndexList = new List<TestIndexResultADO>();
            TestHarmonyList = new List<TestHarmonyADO>();
        }

        /// <summary>
        /// Parse config
        /// Format: XN001|XN002;XN011|XN012;XN101|XN102
        /// </summary>
        private void ParseConfig()
        {
            try
            {
                string config = Config.ConfigKey.BloodHarmonyTestIndexConfig;
                if (string.IsNullOrWhiteSpace(config))
                {
                    Inventec.Common.Logging.LogSystem.Warn("Chua cau hinh HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood.BloodHarmonyTestIndex");
                    return;
                }

                string[] groups = config.Split(';');

                if (groups.Length >= 1)
                {
                    bloodTestIndexCodes = groups[0].Split('|').Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                }

                if (groups.Length >= 2)
                {
                    saltEnviTestIndexCodes = groups[1].Split('|').Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                }

                if (groups.Length >= 3)
                {
                    antiGlobulinTestIndexCodes = groups[2].Split('|').Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Lấy và phân loại chỉ số xét nghiệm theo TREATMENT_ID
        /// </summary>
        public void LoadTestIndexData(long treatmentId)
        {
            try
            {
                ParseConfig();

                if (bloodTestIndexCodes.Count == 0 &&
                    saltEnviTestIndexCodes.Count == 0 &&
                    antiGlobulinTestIndexCodes.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Khong co cau hinh chi so xet nghiem");
                    return;
                }

                // Lấy dữ liệu từ V_HIS_SERE_SERV_TEIN
                HisSereServTeinViewFilter filter = new HisSereServTeinViewFilter();
                filter.TDL_TREATMENT_ID = treatmentId;

                List<V_HIS_SERE_SERV_TEIN> sereServTeins = new BackendAdapter(new CommonParam())
                    .Get<List<V_HIS_SERE_SERV_TEIN>>("api/HisSereServTein/GetView",
                        HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, null);

                if (sereServTeins == null || sereServTeins.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Info("Khong co chi so xet nghiem cho treatment: " + treatmentId);
                    return;
                }

                // Chuyển đổi sang ADO
                List<TestIndexResultADO> allTestIndexResults = sereServTeins.Select(o => new TestIndexResultADO
                {
                    SERE_SERV_TEIN_ID = o.ID,
                    TEST_INDEX_CODE = o.TEST_INDEX_CODE,
                    TEST_INDEX_NAME = o.TEST_INDEX_NAME,
                    VALUE = o.VALUE,
                    SERE_SERV_ID = o.SERE_SERV_ID,
                    TREATMENT_ID = o.TDL_TREATMENT_ID ?? 0,
                    RESULT_TIME = o.RESULT_TIME
                }).ToList();

                // Phân loại theo config
                BloodTestIndexList = allTestIndexResults
                    .Where(o => bloodTestIndexCodes.Contains(o.TEST_INDEX_CODE))
                    .ToList();

                SaltEnviTestIndexList = allTestIndexResults
                    .Where(o => saltEnviTestIndexCodes.Contains(o.TEST_INDEX_CODE))
                    .ToList();

                AntiGlobulinTestIndexList = allTestIndexResults
                    .Where(o => antiGlobulinTestIndexCodes.Contains(o.TEST_INDEX_CODE))
                    .ToList();

                // Tạo danh sách cho combobox XN hòa hợp
                BuildTestHarmonyList();

                Inventec.Common.Logging.LogSystem.Info("Load test index data: Blood=" + BloodTestIndexList.Count +
                    ", SaltEnvi=" + SaltEnviTestIndexList.Count +
                    ", AntiGlobulin=" + AntiGlobulinTestIndexList.Count +
                    ", TestHarmony=" + TestHarmonyList.Count);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Nhóm các chỉ số xét nghiệm theo SERE_SERV_ID
        /// </summary>
        private void BuildTestHarmonyList()
        {
            try
            {
                TestHarmonyList.Clear();

                // Lấy tất cả SERE_SERV_ID từ 3 danh sách
                var allSereServIds = BloodTestIndexList.Select(o => o.SERE_SERV_ID)
                    .Union(SaltEnviTestIndexList.Select(o => o.SERE_SERV_ID))
                    .Union(AntiGlobulinTestIndexList.Select(o => o.SERE_SERV_ID))
                    .Distinct()
                    .ToList();

                foreach (var sereServId in allSereServIds)
                {
                    var bloodIndex = BloodTestIndexList.FirstOrDefault(o => o.SERE_SERV_ID == sereServId);
                    var saltIndex = SaltEnviTestIndexList.FirstOrDefault(o => o.SERE_SERV_ID == sereServId);
                    var antiGlobulinIndex = AntiGlobulinTestIndexList.FirstOrDefault(o => o.SERE_SERV_ID == sereServId);

                    TestHarmonyADO ado = new TestHarmonyADO();
                    ado.SERE_SERV_ID = sereServId;
                    ado.RESULT_TIME = bloodIndex != null ? bloodIndex.RESULT_TIME : null;
                    ado.BLOOD_VALUE = bloodIndex != null ? bloodIndex.VALUE : "";
                    ado.SALT_VALUE = saltIndex != null ? saltIndex.VALUE : "";
                    ado.ANTI_GLOBULIN_VALUE = antiGlobulinIndex != null ? antiGlobulinIndex.VALUE : "";

                    TestHarmonyList.Add(ado);
                }

                // Sắp xếp theo thời gian trả kết quả giảm dần
                TestHarmonyList = TestHarmonyList.OrderByDescending(o => o.RESULT_TIME ?? 0).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Lấy TestIndexResultADO từ danh sách A theo SERE_SERV_ID
        /// </summary>
        public TestIndexResultADO GetBloodTestIndexBySereServId(long sereServId)
        {
            return BloodTestIndexList.FirstOrDefault(o => o.SERE_SERV_ID == sereServId);
        }

        /// <summary>
        /// Lấy TestIndexResultADO từ danh sách B theo SERE_SERV_ID
        /// </summary>
        public TestIndexResultADO GetSaltEnviTestIndexBySereServId(long sereServId)
        {
            return SaltEnviTestIndexList.FirstOrDefault(o => o.SERE_SERV_ID == sereServId);
        }

        /// <summary>
        /// Lấy TestIndexResultADO từ danh sách C theo SERE_SERV_ID
        /// </summary>
        public TestIndexResultADO GetAntiGlobulinTestIndexBySereServId(long sereServId)
        {
            return AntiGlobulinTestIndexList.FirstOrDefault(o => o.SERE_SERV_ID == sereServId);
        }

        /// <summary>
        /// Tìm chỉ số xét nghiệm A1 theo BLOOD_CODE
        /// </summary>
        public TestIndexResultADO FindBloodTestIndexByBloodCode(string bloodCode)
        {
            if (string.IsNullOrWhiteSpace(bloodCode))
                return null;

            return BloodTestIndexList.FirstOrDefault(o => o.VALUE == bloodCode);
        }
    }
}