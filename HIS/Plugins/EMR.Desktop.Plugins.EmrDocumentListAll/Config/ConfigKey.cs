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

namespace EMR.Desktop.Plugins.EmrDocumentListAll.Config
{
    /// <summary>
    /// Cau hinh EMR_CONFIG cua man Danh sach van ban.
    /// Theo mau HIS.Desktop.Plugins.EmrDocument/Config/ConfigKey.cs: const key + static field, nap 1 lan luc Form_Load.
    /// </summary>
    class ConfigKey
    {
        /// <summary>
        /// So ngay mac dinh loc theo ngay tao khi mo man: tu hom nay lui n ngay den hom nay.
        /// Trong hoac 0: lay tu dau thang den cuoi thang hien tai nhu truoc.
        /// </summary>
        private const string LIST_ALL_DAY_NUMBER = "EMR.EMR_DOCUMENT.LIST_ALL.DAY_NUMBER";

        internal static int ListAllDayNumber;

        internal static void GetConfigKey()
        {
            try
            {
                //Tai lai cau hinh EMR truoc khi doc: cache may tram co the nap tu truoc khi key duoc them/doi tren server
                HIS.Desktop.LocalStorage.EmrConfig.ConfigLoader.Refresh();
                int dayNumber;
                ListAllDayNumber = (Int32.TryParse((GetValue(LIST_ALL_DAY_NUMBER) ?? "").Trim(), out dayNumber) && dayNumber > 0) ? dayNumber : 0;
                Inventec.Common.Logging.LogSystem.Debug("EmrDocumentListAll.ConfigKey: " + LIST_ALL_DAY_NUMBER + " = " + ListAllDayNumber);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static string GetValue(string code)
        {
            try
            {
                return HIS.Desktop.LocalStorage.EmrConfig.EmrConfigs.Get<string>(code);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }
    }
}
