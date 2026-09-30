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
using HIS.Desktop.LocalStorage.HisConfig;
using System;

namespace HIS.Desktop.Plugins.GenerateRegisterOrder.Config
{
    /// <summary>
    /// Doc cau hinh toan vien cho tinh nang lay so thu tu co xac dinh nguoi benh.
    /// Thiet ke: PTTK_54254 muc B.2.2.
    /// </summary>
    internal class HisConfigCFG
    {
        /// <summary>
        /// Bat/tat tinh nang lay so thu tu co xac dinh nguoi benh.
        /// Gia tri 1 = bat. Khong co ban ghi hoac khac 1 = tat.
        /// </summary>
        internal const string CONFIG_KEY__IS_ISSUE_WITH_IDENTITY = "MOS.HIS_REGISTER_REQ.IS_ISSUE_WITH_IDENTITY";

        /// <summary>
        /// So STT toi da mot nguoi duoc lay trong mot ngay lich, tinh tren toan vien.
        /// Gia tri 0 hoac rong = khong gioi han.
        /// </summary>
        internal const string CONFIG_KEY__MAX_NUM_ORDER_PER_PATIENT_PER_DAY = "MOS.HIS_REGISTER_REQ.MAX_NUM_ORDER_PER_PATIENT_PER_DAY";

        /// <summary>Tinh nang dang bat hay khong. Mac dinh tat.</summary>
        internal static bool IsIssueWithIdentity { get; private set; }

        /// <summary>So luot toi da trong ngay. 0 = khong gioi han. Chi de hien thi, may chu moi la noi chan.</summary>
        internal static long MaxNumOrderPerPatientPerDay { get; private set; }

        /// <summary>
        /// Nap cau hinh. Goi mot lan trong Load cua man lay so.
        /// Loi doc cau hinh thi coi nhu tat, de vien khong bat van chay nhu cu.
        /// </summary>
        internal static void LoadConfig()
        {
            try
            {
                IsIssueWithIdentity = false;
                MaxNumOrderPerPatientPerDay = 0;

                string isIssue = GetValue(CONFIG_KEY__IS_ISSUE_WITH_IDENTITY);
                IsIssueWithIdentity = !String.IsNullOrWhiteSpace(isIssue) && isIssue.Trim() == "1";

                string maxNumOrder = GetValue(CONFIG_KEY__MAX_NUM_ORDER_PER_PATIENT_PER_DAY);
                if (!String.IsNullOrWhiteSpace(maxNumOrder))
                {
                    MaxNumOrderPerPatientPerDay = Inventec.Common.TypeConvert.Parse.ToInt64(maxNumOrder.Trim());
                }

                Inventec.Common.Logging.LogSystem.Debug(
                    "GenerateRegisterOrder.LoadConfig"
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => IsIssueWithIdentity), IsIssueWithIdentity)
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => MaxNumOrderPerPatientPerDay), MaxNumOrderPerPatientPerDay));
            }
            catch (Exception ex)
            {
                IsIssueWithIdentity = false;
                MaxNumOrderPerPatientPerDay = 0;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private static string GetValue(string key)
        {
            string result = null;
            try
            {
                result = HisConfigs.Get<string>(key);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
