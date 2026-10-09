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
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Resources;

namespace HIS.Desktop.Plugins.CallPatientDrugStoreDispense.Resources
{
    class ResourceLanguageManager
    {
        internal static ResourceManager LanguageResource;

        /// <summary>
        /// Get a UI text from Lang.*.resx by key, in the current application language
        /// </summary>
        internal static string GetValue(string key)
        {
            try
            {
                if (LanguageResource == null)
                {
                    LanguageResource = new ResourceManager("HIS.Desktop.Plugins.CallPatientDrugStoreDispense.Resources.Lang", typeof(ResourceLanguageManager).Assembly);
                }
                return Inventec.Common.Resource.Get.Value(key, LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }
    }
}
