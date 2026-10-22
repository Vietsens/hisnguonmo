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

namespace HIS.Desktop.Plugins.PublicMedicineMultiPatient.Config
{
    internal class Config
    {
        /// <summary>
        /// Number of day columns on one page of the disclosure form (shared with HIS.Desktop.Plugins.PublicMedicineByPhased)
        /// </summary>
        private const string CONFIG_KEY__PHIEU_CONG_KHAI_THUOC__DAY_SIZE = "HIS.PHIEU_CONG_KHAI_THUOC.DAY_SIZE";
        private const int DAY_SIZE_DEFAULT = 10;

        internal static int CongKhaiThuocDaySize = DAY_SIZE_DEFAULT;

        internal static void LoadConfig()
        {
            try
            {
                int daySize = HisConfigs.Get<int>(CONFIG_KEY__PHIEU_CONG_KHAI_THUOC__DAY_SIZE);
                CongKhaiThuocDaySize = daySize > 0 ? daySize : DAY_SIZE_DEFAULT;
            }
            catch (Exception ex)
            {
                CongKhaiThuocDaySize = DAY_SIZE_DEFAULT;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
