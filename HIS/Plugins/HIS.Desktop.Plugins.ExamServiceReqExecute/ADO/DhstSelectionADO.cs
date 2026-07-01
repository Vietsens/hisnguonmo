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

namespace HIS.Desktop.Plugins.ExamServiceReqExecute.ADO
{
    /// <summary>
    /// Item nguon cho GridLookUpEdit "Muc do y thuc" (HIS_DHST.LOC) va "AVPU" (HIS_DHST.AVPU).
    /// VALUE = gia tri luu DB (short). NAME = ten hien thi tren combo.
    /// </summary>
    public class DhstSelectionADO
    {
        /// <summary>Gia tri luu vao HIS_DHST.LOC / HIS_DHST.AVPU.</summary>
        public short VALUE { get; set; }

        /// <summary>Ten hien thi tren combo.</summary>
        public string NAME { get; set; }

        public DhstSelectionADO()
        {
        }

        public DhstSelectionADO(short value, string name)
        {
            this.VALUE = value;
            this.NAME = name;
        }
    }
}
