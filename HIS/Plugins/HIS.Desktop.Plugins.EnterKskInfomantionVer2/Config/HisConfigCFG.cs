using HIS.Desktop.LocalStorage.HisConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Config
{
    public class HisConfigCFG
    {
        private const string KEY__DisablePartExamByExecutor = "HIS.Desktop.Plugins.EnterKskInfomantionVer2.DisablePartExamByExecutor";
        private const string KEY__CheckReq = "HIS.Desktop.Plugins.EnterKskInfomantionVer2.CheckReq";

        /// <summary>
        /// Key kết nối các cổng nhận bản tin KSK theo QĐ 2062 (khai theo chi nhánh) — cùng key
        /// HIS.Desktop.Plugins.KskSyncList dùng để đẩy hồ sơ.
        /// </summary>
        private static readonly string[] KEYS__Qd2062Connection = new string[]
        {
            "MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO",
            "MOS.HIS_KSK_SYNC.HSSK_HN_2062_CONNECTION_INFO",
            "MOS.HIS_KSK_SYNC.HSSK_HOC_2062_CONNECTION_INFO",
            "MOS.HIS_KSK_SYNC.HSSK_HCC_2062_CONNECTION_INFO"
        };

        internal static string DisablePartExamByExecutor;
        /// <summary>"1" = khi kết thúc khám (bấm nút hoặc tự động sau Lưu) cảnh báo nếu còn dịch vụ có y lệnh chưa hoàn thành.</summary>
        internal static string CheckReq;

        /// <summary>
        /// true = chi nhánh đang làm việc có khai kết nối cổng 2062 (đẩy hồ sơ KSK theo QĐ 2062) -> giới hạn
        /// độ dài các trường theo QĐ 2062 ngay lúc nhập (xem Run\frmEnterKskInfomantionVer2___Qd2062Length).
        /// </summary>
        internal static bool IsQd2062LengthCheck;

        /// <summary>Key cổng 2062 làm bật giới hạn độ dài (chỉ để ghi log).</summary>
        internal static string Qd2062ConnectionKey;

        internal static void LoadConfig()
        {
            try
            {
                DisablePartExamByExecutor = HisConfigs.Get<string>(KEY__DisablePartExamByExecutor);
                CheckReq = HisConfigs.Get<string>(KEY__CheckReq);
                LoadQd2062LengthCheck();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static void LoadQd2062LengthCheck()
        {
            IsQd2062LengthCheck = false;
            Qd2062ConnectionKey = null;
            try
            {
                var configs = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CONFIG>();
                long branchId = HIS.Desktop.LocalStorage.LocalData.BranchWorker.GetCurrentBranchId();
                foreach (string key in KEYS__Qd2062Connection)
                {
                    if (!string.IsNullOrWhiteSpace(GetBranchConfigValue(configs, key, branchId)))
                    {
                        IsQd2062LengthCheck = true;
                        Qd2062ConnectionKey = key;
                        break;
                    }
                }
                Inventec.Common.Logging.LogSystem.Info("EnterKskInfomantionVer2: gioi han do dai QD 2062 = "
                    + IsQd2062LengthCheck + " (chi nhanh " + branchId + ", key " + (Qd2062ConnectionKey ?? "-") + ")");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// VALUE của key theo chi nhánh: dòng đúng BRANCH_ID thắng, không có thì dòng dùng chung (BRANCH_ID
        /// null) — cùng quy tắc KskSyncList.KskBranchConfig (key cổng 2062 khai theo chi nhánh).
        /// KHÔNG log VALUE (chứa tài khoản / mật khẩu cổng).
        /// </summary>
        private static string GetBranchConfigValue(List<MOS.EFMODEL.DataModels.HIS_CONFIG> configs, string key, long branchId)
        {
            if (configs == null) return null;
            var ofKey = configs.Where(o => o != null && o.KEY == key).ToList();
            var cfg = ofKey.FirstOrDefault(o => o.BRANCH_ID.HasValue && o.BRANCH_ID.Value == branchId)
                ?? ofKey.FirstOrDefault(o => !o.BRANCH_ID.HasValue);
            return (cfg != null) ? cfg.VALUE : null;
        }
    }
}
