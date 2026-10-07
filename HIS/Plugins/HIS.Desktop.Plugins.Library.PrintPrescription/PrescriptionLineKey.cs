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

namespace HIS.Desktop.Plugins.Library.PrintPrescription
{
    /// <summary>
    /// Viec 46680: cung 1 thuoc ke nhieu dong voi lieu dung (TUTORIAL) hoac cach dung (HTU_TEXT) khac nhau
    /// phai in thanh cac dong rieng. Lieu dung, cach dung duoc chuan hoa truoc khi dung lam khoa gom dong
    /// de 2 dong chi khac nhau o khoang trang dau/cuoi hoac null/rong van duoc coi la 1 dong ke.
    /// </summary>
    internal static class PrescriptionLineKey
    {
        internal static string Normalize(string value)
        {
            string result = "";
            try
            {
                result = String.IsNullOrWhiteSpace(value) ? "" : value.Trim();
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
