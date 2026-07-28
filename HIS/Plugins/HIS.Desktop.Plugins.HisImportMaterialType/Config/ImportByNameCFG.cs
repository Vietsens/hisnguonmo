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

namespace HIS.Desktop.Plugins.HisImportMaterialType.Config
{
    /// <summary>
    /// Cấu hình bật/tắt chế độ Import theo TÊN (Hãng SX / Đơn vị tính).
    /// Mặc định TẮT: import theo MÃ như cũ. BẬT (= "1"): import theo TÊN, tự tạo mã.
    /// Key toàn viện: MOS.HIS_MATERIAL_TYPE.IMPORT_BY_NAME
    /// </summary>
    internal class ImportByNameCFG
    {
        private const string KEY_IMPORT_BY_NAME = "MOS.HIS_MATERIAL_TYPE.IMPORT_BY_NAME";

        /// <summary>True = đọc/so khớp theo TÊN (ĐVT + hãng SX); False = theo MÃ (mặc định).</summary>
        internal static bool IsImportByName()
        {
            try
            {
                string value = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY_IMPORT_BY_NAME);
                return value == "1";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false; // Lỗi đọc config -> mặc định TẮT (an toàn)
            }
        }
    }
}
