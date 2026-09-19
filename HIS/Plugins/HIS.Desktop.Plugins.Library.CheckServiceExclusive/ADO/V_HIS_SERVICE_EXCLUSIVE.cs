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

namespace HIS.Desktop.Plugins.Library.CheckServiceExclusive.ADO
{
    /// <summary>
    /// View V_HIS_SERVICE_EXCLUSIVE - bang goc kem ma/ten/loai cua ca 2 dau dich vu.
    /// Lop CUC BO cua FE, xem ghi chu o HIS_SERVICE_EXCLUSIVE.
    /// </summary>
    public class V_HIS_SERVICE_EXCLUSIVE : HIS_SERVICE_EXCLUSIVE
    {
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }
        public Nullable<long> SERVICE_TYPE_ID { get; set; }
        public string SERVICE_TYPE_CODE { get; set; }
        public string SERVICE_TYPE_NAME { get; set; }

        public string EXCLUSIVE_CODE { get; set; }
        public string EXCLUSIVE_NAME { get; set; }
        public Nullable<long> EXCLUSIVE_TYPE_ID { get; set; }
        public string EXCLUSIVE_TYPE_CODE { get; set; }
        public string EXCLUSIVE_TYPE_NAME { get; set; }
    }
}
