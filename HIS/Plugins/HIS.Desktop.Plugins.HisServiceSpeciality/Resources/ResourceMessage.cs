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

namespace HIS.Desktop.Plugins.HisServiceSpeciality.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage =
            new System.Resources.ResourceManager(
                "HIS.Desktop.Plugins.HisServiceSpeciality.Resources.Message.Lang",
                System.Reflection.Assembly.GetExecutingAssembly());

        /// <summary>Chưa chọn dịch vụ</summary>
        internal static string ChuaChonDichVu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value(
                        "ChuaChonDichVu",
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

        /// <summary>Chưa chọn phạm vi chuyên môn</summary>
        internal static string ChuaChonPhamViChuyenMon
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value(
                        "ChuaChonPhamViChuyenMon",
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

        /// <summary>Không có thay đổi nào để lưu</summary>
        internal static string KhongCoThayDoiDeLuu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value(
                        "KhongCoThayDoiDeLuu",
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
}
