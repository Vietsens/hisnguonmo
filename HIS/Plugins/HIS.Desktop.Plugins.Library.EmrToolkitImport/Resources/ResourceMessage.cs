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

namespace HIS.Desktop.Plugins.Library.EmrToolkitImport.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage =
            new System.Resources.ResourceManager(
                "HIS.Desktop.Plugins.Library.EmrToolkitImport.Resources.Message.Lang",
                System.Reflection.Assembly.GetExecutingAssembly());

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

        /// <summary>Gửi dữ liệu qua EMRTOOLKIT thành công.</summary>
        internal static string GuiDuLieuThanhCong { get { return GetValue("GuiDuLieuThanhCong"); } }

        /// <summary>Gửi dữ liệu qua EMRTOOLKIT thất bại.</summary>
        internal static string GuiDuLieuThatBai { get { return GetValue("GuiDuLieuThatBai"); } }

        /// <summary>Đã sao chép JSON vào clipboard.</summary>
        internal static string DaSaoChepJson { get { return GetValue("DaSaoChepJson"); } }
    }
}
