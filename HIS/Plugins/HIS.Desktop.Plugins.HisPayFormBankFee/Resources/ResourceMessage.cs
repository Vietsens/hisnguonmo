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
using System.Resources;

namespace HIS.Desktop.Plugins.HisPayFormBankFee.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage =
            new System.Resources.ResourceManager(
                "HIS.Desktop.Plugins.HisPayFormBankFee.Resources.Message.Lang",
                System.Reflection.Assembly.GetExecutingAssembly());

        /// <summary>Tỉ lệ phụ phí phải lớn hơn 0.</summary>
        internal static string TiLePhuPhiPhaiLonHonKhong
        {
            get { return GetValue("TiLePhuPhiPhaiLonHonKhong"); }
        }

        /// <summary>Cấu hình phụ phí cho cặp hình thức thanh toán và ngân hàng này đã tồn tại. Vui lòng kiểm tra lại.</summary>
        internal static string CauHinhPhuPhiDaTonTai
        {
            get { return GetValue("CauHinhPhuPhiDaTonTai"); }
        }

        /// <summary>Tên phụ phí vượt quá 200 ký tự.</summary>
        internal static string TenPhuPhiVuotQuaGioiHan
        {
            get { return GetValue("TenPhuPhiVuotQuaGioiHan"); }
        }

        private static string GetValue(string key)
        {
            try
            {
                return Inventec.Common.Resource.Get.Value(
                    key,
                    languageMessage,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }
    }
}
