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

namespace HIS.Desktop.Plugins.HisImportCareer.ADO
{
    /// <summary>
    /// DTO gui api/HisCareer/ImportList (upsert theo CAREER_CODE).
    /// Khai bao rieng (khong ke thua HIS_CAREER) de gui du 6 truong cap 2/3/4
    /// ma khong phu thuoc phien ban MOS.EFMODEL phia client
    /// </summary>
    public class CareerImportDTO
    {
        public string CAREER_CODE { get; set; }
        public string CAREER_NAME { get; set; }
        public string LEVEL2_CODE { get; set; }
        public string LEVEL2_NAME { get; set; }
        public string LEVEL3_CODE { get; set; }
        public string LEVEL3_NAME { get; set; }
        public string LEVEL4_CODE { get; set; }
        public string LEVEL4_NAME { get; set; }
    }
}
