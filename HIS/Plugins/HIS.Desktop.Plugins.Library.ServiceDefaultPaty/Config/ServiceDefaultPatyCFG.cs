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
using HIS.Desktop.LocalStorage.HisConfig;

namespace HIS.Desktop.Plugins.Library.ServiceDefaultPaty.Config
{
    /// <summary>
    /// PT-44730. Reads the permission config of the default patient type feature.
    /// The record is declared per branch (HIS_CONFIG.BRANCH_ID) so the backend already
    /// returns the value of the working branch.
    /// </summary>
    public class ServiceDefaultPatyCFG
    {
        /// <summary>Who may edit the patient type of the declared services. Default value = 1.</summary>
        public const string CONFIG_KEY__SERVICE_DEFAULT_PATY_EDIT_OPTION = "HIS.Desktop.Plugins.Assign.ServiceDefaultPatyEditOption";

        /// <summary>
        /// Returns the configured option. Falls back to OnlyAdmin when the config record is missing
        /// or holds a non numeric value — the config only takes effect on declared services anyway.
        /// </summary>
        public static long GetEditOption()
        {
            long result = (long)EnumServiceDefaultPatyEditOption.OnlyAdmin;
            try
            {
                string value = HisConfigs.Get<string>(CONFIG_KEY__SERVICE_DEFAULT_PATY_EDIT_OPTION);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    long parsed;
                    // Every numeric value is honoured, zero and negatives included: any value other
                    // than 1 and 2 means the feature does not restrict anybody. Only a missing record
                    // or a non numeric value falls back to OnlyAdmin.
                    if (long.TryParse(value.Trim(), out parsed)) result = parsed;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
