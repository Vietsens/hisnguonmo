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
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.KskSyncList
{
    /// <summary>
    /// Đọc cấu hình HIS_CONFIG THEO CHI NHÁNH (cơ sở) đang làm việc.
    ///
    /// Nhiều cơ sở dùng chung 1 DB nên cùng 1 KEY có thể tồn tại nhiều bản ghi HIS_CONFIG,
    /// phân biệt bằng BRANCH_ID (khoá nghiệp vụ backend là cặp KEY + BRANCH_ID).
    /// Thứ tự ưu tiên giống backend (MOS.MANAGER Loader.GetConfig(code, branchId)):
    ///     1. bản ghi đúng chi nhánh đang làm việc
    ///     2. bản ghi dùng chung (BRANCH_ID null)
    /// KHÔNG được lấy FirstOrDefault theo KEY: sẽ bốc trúng cấu hình của cơ sở khác.
    ///
    /// Chi nhánh hiện tại được ghi vào registry khi chọn phòng làm việc (frmChooseRoom).
    /// </summary>
    internal static class KskBranchConfig
    {
        /// <summary>Id chi nhánh đang làm việc (0 nếu chưa chọn phòng).</summary>
        internal static long CurrentBranchId()
        {
            try
            {
                return HIS.Desktop.LocalStorage.LocalData.BranchWorker.GetCurrentBranchId();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return 0; }
        }

        /// <summary>Bản ghi chi nhánh đang làm việc (null nếu không xác định).</summary>
        private static HIS_BRANCH CurrentBranch()
        {
            try
            {
                long branchId = CurrentBranchId();
                if (branchId <= 0) return null;
                return BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == branchId);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        /// <summary>Mã chi nhánh đang làm việc (rỗng nếu không xác định).</summary>
        internal static string CurrentBranchCode()
        {
            HIS_BRANCH branch = CurrentBranch();
            return (branch != null) ? branch.BRANCH_CODE : "";
        }

        /// <summary>Tên chi nhánh đang làm việc — dùng cho thông báo cấu hình thiếu.</summary>
        internal static string CurrentBranchName()
        {
            HIS_BRANCH branch = CurrentBranch();
            return (branch != null) ? branch.BRANCH_NAME : "";
        }

        /// <summary>
        /// VALUE của 1 key HIS_CONFIG theo chi nhánh đang làm việc (đọc từ cache RAM);
        /// null nếu chưa khai cho cơ sở này và cũng không có bản dùng chung.
        /// </summary>
        internal static string GetValue(string key)
        {
            try
            {
                return PickByBranch(BackendDataWorker.Get<HIS_CONFIG>(), key);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        /// <summary>
        /// Như <see cref="GetValue"/> nhưng ĐỌC THẲNG TỪ NGUỒN (bỏ qua cache) để sửa cấu hình là
        /// ăn ngay, không phải khởi động lại chương trình. Nguồn rỗng -> lùi về cache.
        /// </summary>
        internal static string GetValueFresh(string key)
        {
            try
            {
                List<HIS_CONFIG> list = BackendDataWorker.Get<HIS_CONFIG>(false, true, false, false);
                if (list == null || list.Count == 0) list = BackendDataWorker.Get<HIS_CONFIG>();
                return PickByBranch(list, key);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        /// <summary>Chọn bản ghi đúng chi nhánh, không có thì lấy bản dùng chung (BRANCH_ID null).</summary>
        private static string PickByBranch(List<HIS_CONFIG> source, string key)
        {
            if (source == null || string.IsNullOrWhiteSpace(key)) return null;

            long branchId = CurrentBranchId();

            List<HIS_CONFIG> configs = source
                .Where(o => o != null && o.KEY == key && (!o.BRANCH_ID.HasValue || o.BRANCH_ID.Value == branchId))
                .ToList();

            HIS_CONFIG cfg = configs.FirstOrDefault(o => o.BRANCH_ID.HasValue && o.BRANCH_ID.Value == branchId)
                          ?? configs.FirstOrDefault(o => !o.BRANCH_ID.HasValue);

            return (cfg != null) ? cfg.VALUE : null;
        }

        /// <summary>
        /// Ghi log CẢNH BÁO khi trường [0] BranchCode của chuỗi cấu hình khác mã chi nhánh đang làm việc.
        ///
        /// CHỈ cảnh báo, KHÔNG chặn: trường [0] trong dữ liệu thực tế không phải lúc nào cũng là
        /// BRANCH_CODE của HIS_BRANCH (có nơi khai mã cơ sở KCB, có nơi bỏ trống). Nếu truyền mã này
        /// vào Qd1551ConfigParser.Parse thì lệch một ký tự là parser trả null -> cả cổng ngừng đẩy
        /// ("PARSE LOI / THIEU TRUONG BAT BUOC"). Việc chọn đúng cấu hình đã do GetValue lo theo BRANCH_ID.
        /// </summary>
        internal static void WarnIfBranchCodeMismatch(string configValue, string configKey)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(configValue)) return;

                string[] fields = configValue.Split('|');
                if (fields.Length == 0 || string.IsNullOrWhiteSpace(fields[0])) return;

                string inConfig = fields[0].Trim();
                string current = CurrentBranchCode();
                if (string.IsNullOrWhiteSpace(current)) return;

                if (!string.Equals(inConfig, current, StringComparison.OrdinalIgnoreCase))
                {
                    Inventec.Common.Logging.LogSystem.Warn(string.Format(
                        "Cau hinh {0}: truong [0] BranchCode = '{1}' KHAC ma chi nhanh dang lam viec '{2}'."
                        + " Van dung cau hinh nay (chon theo BRANCH_ID cua ban ghi HIS_CONFIG).",
                        configKey, inConfig, current));
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }
    }
}
