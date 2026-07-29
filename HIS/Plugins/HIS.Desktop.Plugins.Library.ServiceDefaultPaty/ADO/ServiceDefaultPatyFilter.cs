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

namespace HIS.Desktop.Plugins.Library.ServiceDefaultPaty.ADO
{
    /// <summary>
    /// PT-44730. Filter sent to api/HisServiceDefaultPaty/Get and /GetView.
    /// TEMPORARY: replace by MOS.Filter.HisServiceDefaultPatyFilter after backend gencode.
    /// </summary>
    public class ServiceDefaultPatyFilter
    {
        public long? ID { get; set; }

        public long? SERVICE_ID { get; set; }

        public short? IS_ACTIVE { get; set; }

        /// <summary>Search by service code / service name.</summary>
        public string KEY_WORD { get; set; }

        public string ORDER_FIELD { get; set; }

        public string ORDER_DIRECTION { get; set; }
    }
}
