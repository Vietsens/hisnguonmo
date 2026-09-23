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

namespace HIS.Desktop.Plugins.HisServiceExclusive
{
    /// <summary>
    /// Dong du lieu cua combo "Muc xu ly" (HANDLE_TYPE_ID): 1 = Canh bao, 2 = Chan.
    /// Gia tri map voi HIS.Desktop.Plugins.Library.CheckServiceExclusive.ADO.HandleType.
    /// </summary>
    public class HandleTypeItem
    {
        public short ID { get; set; }
        public string NAME { get; set; }

        public HandleTypeItem(short id, string name)
        {
            this.ID = id;
            this.NAME = name;
        }
    }
}
