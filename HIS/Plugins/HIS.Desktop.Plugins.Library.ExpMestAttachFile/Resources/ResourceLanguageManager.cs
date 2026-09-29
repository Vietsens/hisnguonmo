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

namespace HIS.Desktop.Plugins.Library.ExpMestAttachFile.Resources
{
    class ResourceLanguageManager
    {
        internal static ResourceManager LanguageResource = new ResourceManager(
            "HIS.Desktop.Plugins.Library.ExpMestAttachFile.Resources.Lang",
            typeof(ResourceLanguageManager).Assembly);

        /// <summary>
        /// Get a UI text by key in the current language.
        /// Returns fallback when the key is missing so a control never ends up with an empty caption.
        /// </summary>
        internal static string GetValue(string key, string fallback)
        {
            try
            {
                string value = Inventec.Common.Resource.Get.Value(
                    key,
                    LanguageResource,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                return String.IsNullOrEmpty(value) ? fallback : value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return fallback;
        }
    }
}
