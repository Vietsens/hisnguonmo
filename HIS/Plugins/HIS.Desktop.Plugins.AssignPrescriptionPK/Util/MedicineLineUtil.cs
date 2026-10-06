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
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK
{
    /// <summary>
    /// Viec 46680: dieu kien cach dung (HTU_TEXT) khi gop 2 dong ke cung 1 thuoc trong kho thanh 1 dong.
    /// </summary>
    class MedicineLineUtil
    {
        /// <summary>
        /// true khi 2 dong duoc phep gop xet theo cach dung:
        /// key HIS.Desktop.Plugins.AssignPrescription.IsSplitMedicineByHtu tat (giu nguyen hanh vi cu)
        /// hoac 2 cach dung giong nhau (bo khoang trang dau/cuoi, null va rong coi la nhu nhau).
        /// </summary>
        internal static bool IsSameHtuForMerge(string htuText1, string htuText2)
        {
            bool result = true;
            try
            {
                if (HisConfigCFG.IsSplitMedicineByHtu)
                {
                    result = NormalizeHtu(htuText1) == NormalizeHtu(htuText2);
                }
            }
            catch (Exception ex)
            {
                result = true;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        internal static string NormalizeHtu(string htuText)
        {
            string result = "";
            try
            {
                result = String.IsNullOrWhiteSpace(htuText) ? "" : htuText.Trim();
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
